using System;
using System.Drawing;
using mrbControls;

namespace Mids_Reborn.Forms
{
    partial class FrmMain2
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
            this.components = new System.ComponentModel.Container();
            this.DlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.DlgSave = new System.Windows.Forms.SaveFileDialog();
            this.tmrGfx = new System.Windows.Forms.Timer(this.components);
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblHero = new System.Windows.Forms.Label();
            this.ibTeam = new System.Windows.Forms.Button();
            this.heroVillain = new System.Windows.Forms.Button();
            this.petsButton = new System.Windows.Forms.Button();
            this.tempPowersButton = new System.Windows.Forms.Button();
            this.accoladeButton = new System.Windows.Forms.Button();
            this.incarnateButton = new System.Windows.Forms.Button();
            this.prestigeButton = new System.Windows.Forms.Button();
            this.ibPvX = new System.Windows.Forms.Button();
            this.ibRecipe = new System.Windows.Forms.Button();
            this.ibPopup = new System.Windows.Forms.Button();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.dvPlaceholder = new System.Windows.Forms.Panel();
            this.secondaryPlaceholder = new System.Windows.Forms.Panel();
            this.primaryPlaceholder = new System.Windows.Forms.Panel();
            this.epicPlaceholder = new System.Windows.Forms.Panel();
            this.pool3Placeholder = new System.Windows.Forms.Panel();
            this.pool2Placeholder = new System.Windows.Forms.Panel();
            this.pool1Placeholder = new System.Windows.Forms.Panel();
            this.pool0Placeholder = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAT = new System.Windows.Forms.Label();
            this.lblOrigin = new System.Windows.Forms.Label();
            this.sbMode = new System.Windows.Forms.Button();
            this.ibSets = new System.Windows.Forms.Button();
            this.ibSlots = new System.Windows.Forms.Button();
            this.ibSlotLevels = new System.Windows.Forms.Button();
            this.ibTotals = new System.Windows.Forms.Button();
            this.pbDynMode = new System.Windows.Forms.Button();
            this.lblLockedSecondary = new System.Windows.Forms.Label();
            this.lblATLocked = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cbSecondary = new System.Windows.Forms.ComboBox();
            this.lblSecondary = new System.Windows.Forms.Label();
            this.cbPrimary = new System.Windows.Forms.ComboBox();
            this.cbOrigin = new System.Windows.Forms.ComboBox();
            this.cbAT = new System.Windows.Forms.ComboBox();
            this.lblPrimary = new System.Windows.Forms.Label();
            this.lblPool1 = new System.Windows.Forms.Label();
            this.cbPool0 = new System.Windows.Forms.ComboBox();
            this.lblPool2 = new System.Windows.Forms.Label();
            this.cbPool1 = new System.Windows.Forms.ComboBox();
            this.lblPool3 = new System.Windows.Forms.Label();
            this.cbPool2 = new System.Windows.Forms.ComboBox();
            this.lblPool4 = new System.Windows.Forms.Label();
            this.cbPool3 = new System.Windows.Forms.ComboBox();
            this.lblEpic = new System.Windows.Forms.Label();
            this.cbAncillary = new System.Windows.Forms.ComboBox();
            this.lblLocked0 = new System.Windows.Forms.Label();
            this.lblLocked1 = new System.Windows.Forms.Label();
            this.lblLocked2 = new System.Windows.Forms.Label();
            this.lblLocked3 = new System.Windows.Forms.Label();
            this.lblLockedAncillary = new System.Windows.Forms.Label();
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImport = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsExportLong = new System.Windows.Forms.ToolStripMenuItem();
            this.tsExportDataLink = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.tsGenFreebies = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.tsExportDiscord = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.tsUpdateCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.AdvancedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAdvDBEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.tsAdvFreshInstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAdvResetTips = new System.Windows.Forms.ToolStripMenuItem();
            this.CharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetAllIOsToDefault35ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsIODefault = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.tsIOMin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsIOMax = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToSO = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToDO = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToTO = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToPlus5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToPlus4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToPlus3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToPlus2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToPlus1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToEven = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToMinus1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToMinus2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToMinus3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEnhToNone = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.SlotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFlipAllEnh = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsClearAllEnh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsRemoveAllSlots = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.AutoArrangeAllSlotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.ToggleCheckModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView2Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView3Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView4Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView5Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView6Col = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsViewIOLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewRelative = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSlotLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewRelativeAsSigns = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsViewActualDamage_New = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewDPS_New = new System.Windows.Forms.ToolStripMenuItem();
            this.tlsDPA = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPatchNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBugCrytilis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHCMRBForum = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.tsKoFi = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPatreon = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.tsGitHubCrytilis = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSets = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewGraphs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSetCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewData = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewTotals = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.tsRecipeViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDPSCalc = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSetFind = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.InGameRespecHelperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelperShort = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelperLong = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.tsHelperShort2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelperLong2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.AccoladesWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IncarnateWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TemporaryPowersWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSOLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBuildRcv = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.mainPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlGFX1 = new mrbControls.pnlGFX();
            this.topPanel.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.menuBar.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGFX1)).BeginInit();
            this.SuspendLayout();
            // 
            // DlgOpen
            // 
            this.DlgOpen.DefaultExt = "mxd";
            this.DlgOpen.Filter = "Hero/Villain Builds (*.mxd)|*.mxd;*.txt|Text Files (*.txt)|*.txt";
            // 
            // DlgSave
            // 
            this.DlgSave.DefaultExt = "mxd";
            this.DlgSave.Filter = "Hero/Villain Builds (*.mxd)|*.mxd";
            // 
            // topPanel
            // 
            this.topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topPanel.Controls.Add(this.lblHero);
            this.topPanel.Controls.Add(this.ibTeam);
            this.topPanel.Controls.Add(this.heroVillain);
            this.topPanel.Controls.Add(this.petsButton);
            this.topPanel.Controls.Add(this.tempPowersButton);
            this.topPanel.Controls.Add(this.accoladeButton);
            this.topPanel.Controls.Add(this.incarnateButton);
            this.topPanel.Controls.Add(this.prestigeButton);
            this.topPanel.Controls.Add(this.ibPvX);
            this.topPanel.Controls.Add(this.ibRecipe);
            this.topPanel.Controls.Add(this.ibPopup);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 24);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1326, 64);
            this.topPanel.TabIndex = 0;
            // 
            // lblHero
            // 
            this.lblHero.BackColor = System.Drawing.Color.Transparent;
            this.lblHero.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHero.ForeColor = System.Drawing.Color.White;
            this.lblHero.Location = new System.Drawing.Point(3, 3);
            this.lblHero.Name = "lblHero";
            this.lblHero.Size = new System.Drawing.Size(756, 58);
            this.lblHero.TabIndex = 43;
            this.lblHero.Text = "Name: Level 0 Origin Archetype (Primary / Secondary)";
            // 
            // ibTeam
            // 
            this.ibTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibTeam.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibTeam.Location = new System.Drawing.Point(765, 3);
            this.ibTeam.Name = "ibTeam";
            this.ibTeam.Size = new System.Drawing.Size(105, 22);
            this.ibTeam.TabIndex = 100;
            // 
            // heroVillain
            // 
            this.heroVillain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.heroVillain.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.heroVillain.Location = new System.Drawing.Point(876, 3);
            this.heroVillain.Name = "heroVillain";
            this.heroVillain.Size = new System.Drawing.Size(105, 22);
            this.heroVillain.TabIndex = 116;
            // 
            // petsButton
            // 
            this.petsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.petsButton.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.petsButton.Location = new System.Drawing.Point(765, 31);
            this.petsButton.Name = "petsButton";
            this.petsButton.Size = new System.Drawing.Size(105, 22);
            this.petsButton.TabIndex = 117;
            // 
            // tempPowersButton
            // 
            this.tempPowersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tempPowersButton.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tempPowersButton.Location = new System.Drawing.Point(1209, 31);
            this.tempPowersButton.Name = "tempPowersButton";
            this.tempPowersButton.Size = new System.Drawing.Size(105, 22);
            this.tempPowersButton.TabIndex = 115;
            // 
            // accoladeButton
            // 
            this.accoladeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.accoladeButton.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.accoladeButton.Location = new System.Drawing.Point(987, 31);
            this.accoladeButton.Name = "accoladeButton";
            this.accoladeButton.Size = new System.Drawing.Size(105, 22);
            this.accoladeButton.TabIndex = 114;
            // 
            // incarnateButton
            // 
            this.incarnateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.incarnateButton.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.incarnateButton.Location = new System.Drawing.Point(1098, 31);
            this.incarnateButton.Name = "incarnateButton";
            this.incarnateButton.Size = new System.Drawing.Size(105, 22);
            this.incarnateButton.TabIndex = 113;
            // 
            // prestigeButton
            // 
            this.prestigeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prestigeButton.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.prestigeButton.Location = new System.Drawing.Point(876, 31);
            this.prestigeButton.Name = "prestigeButton";
            this.prestigeButton.Size = new System.Drawing.Size(105, 22);
            this.prestigeButton.TabIndex = 111;
            // 
            // ibPvX
            // 
            this.ibPvX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibPvX.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibPvX.Location = new System.Drawing.Point(987, 3);
            this.ibPvX.Name = "ibPvX";
            this.ibPvX.Size = new System.Drawing.Size(105, 22);
            this.ibPvX.TabIndex = 111;
            // 
            // ibRecipe
            // 
            this.ibRecipe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibRecipe.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibRecipe.Location = new System.Drawing.Point(1098, 3);
            this.ibRecipe.Name = "ibRecipe";
            this.ibRecipe.Size = new System.Drawing.Size(105, 22);
            this.ibRecipe.TabIndex = 105;
            // 
            // ibPopup
            // 
            this.ibPopup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibPopup.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibPopup.Location = new System.Drawing.Point(1209, 3);
            this.ibPopup.Name = "ibPopup";
            this.ibPopup.Size = new System.Drawing.Size(105, 22);
            this.ibPopup.TabIndex = 104;
            // 
            // leftPanel
            // 
            this.leftPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftPanel.BackColor = System.Drawing.Color.Black;
            this.leftPanel.Controls.Add(this.dvPlaceholder);
            this.leftPanel.Controls.Add(this.secondaryPlaceholder);
            this.leftPanel.Controls.Add(this.primaryPlaceholder);
            this.leftPanel.Controls.Add(this.epicPlaceholder);
            this.leftPanel.Controls.Add(this.pool3Placeholder);
            this.leftPanel.Controls.Add(this.pool2Placeholder);
            this.leftPanel.Controls.Add(this.pool1Placeholder);
            this.leftPanel.Controls.Add(this.pool0Placeholder);
            this.leftPanel.Controls.Add(this.lblName);
            this.leftPanel.Controls.Add(this.lblAT);
            this.leftPanel.Controls.Add(this.lblOrigin);
            this.leftPanel.Controls.Add(this.sbMode);
            this.leftPanel.Controls.Add(this.ibSets);
            this.leftPanel.Controls.Add(this.ibSlots);
            this.leftPanel.Controls.Add(this.ibSlotLevels);
            this.leftPanel.Controls.Add(this.ibTotals);
            this.leftPanel.Controls.Add(this.pbDynMode);
            this.leftPanel.Controls.Add(this.lblLockedSecondary);
            this.leftPanel.Controls.Add(this.lblATLocked);
            this.leftPanel.Controls.Add(this.txtName);
            this.leftPanel.Controls.Add(this.cbSecondary);
            this.leftPanel.Controls.Add(this.lblSecondary);
            this.leftPanel.Controls.Add(this.cbPrimary);
            this.leftPanel.Controls.Add(this.cbOrigin);
            this.leftPanel.Controls.Add(this.cbAT);
            this.leftPanel.Controls.Add(this.lblPrimary);
            this.leftPanel.Controls.Add(this.lblLocked0);
            this.leftPanel.Controls.Add(this.lblPool1);
            this.leftPanel.Controls.Add(this.cbPool0);
            this.leftPanel.Controls.Add(this.lblPool2);
            this.leftPanel.Controls.Add(this.lblLocked1);
            this.leftPanel.Controls.Add(this.cbPool1);
            this.leftPanel.Controls.Add(this.lblPool3);
            this.leftPanel.Controls.Add(this.lblLocked2);
            this.leftPanel.Controls.Add(this.cbPool2);
            this.leftPanel.Controls.Add(this.lblPool4);
            this.leftPanel.Controls.Add(this.lblLocked3);
            this.leftPanel.Controls.Add(this.cbPool3);
            this.leftPanel.Controls.Add(this.lblEpic);
            this.leftPanel.Controls.Add(this.lblLockedAncillary);
            this.leftPanel.Controls.Add(this.cbAncillary);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.leftPanel.Location = new System.Drawing.Point(0, 88);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(501, 874);
            this.leftPanel.TabIndex = 1;
            // 
            // dvPlaceholder
            // 
            this.dvPlaceholder.Location = new System.Drawing.Point(12, 458);
            this.dvPlaceholder.Name = "dvPlaceholder";
            this.dvPlaceholder.Size = new System.Drawing.Size(317, 404);
            this.dvPlaceholder.TabIndex = 126;
            // 
            // secondaryPlaceholder
            // 
            this.secondaryPlaceholder.Location = new System.Drawing.Point(177, 175);
            this.secondaryPlaceholder.Name = "secondaryPlaceholder";
            this.secondaryPlaceholder.Size = new System.Drawing.Size(152, 250);
            this.secondaryPlaceholder.TabIndex = 125;
            // 
            // primaryPlaceholder
            // 
            this.primaryPlaceholder.Location = new System.Drawing.Point(12, 175);
            this.primaryPlaceholder.Name = "primaryPlaceholder";
            this.primaryPlaceholder.Size = new System.Drawing.Size(152, 250);
            this.primaryPlaceholder.TabIndex = 124;
            // 
            // epicPlaceholder
            // 
            this.epicPlaceholder.Location = new System.Drawing.Point(337, 759);
            this.epicPlaceholder.Name = "epicPlaceholder";
            this.epicPlaceholder.Size = new System.Drawing.Size(138, 95);
            this.epicPlaceholder.TabIndex = 123;
            // 
            // pool3Placeholder
            // 
            this.pool3Placeholder.Location = new System.Drawing.Point(337, 613);
            this.pool3Placeholder.Name = "pool3Placeholder";
            this.pool3Placeholder.Size = new System.Drawing.Size(138, 95);
            this.pool3Placeholder.TabIndex = 113;
            // 
            // pool2Placeholder
            // 
            this.pool2Placeholder.Location = new System.Drawing.Point(337, 467);
            this.pool2Placeholder.Name = "pool2Placeholder";
            this.pool2Placeholder.Size = new System.Drawing.Size(138, 95);
            this.pool2Placeholder.TabIndex = 112;
            // 
            // pool1Placeholder
            // 
            this.pool1Placeholder.Location = new System.Drawing.Point(337, 321);
            this.pool1Placeholder.Name = "pool1Placeholder";
            this.pool1Placeholder.Size = new System.Drawing.Size(138, 95);
            this.pool1Placeholder.TabIndex = 111;
            // 
            // pool0Placeholder
            // 
            this.pool0Placeholder.Location = new System.Drawing.Point(337, 175);
            this.pool0Placeholder.Name = "pool0Placeholder";
            this.pool0Placeholder.Size = new System.Drawing.Size(138, 95);
            this.pool0Placeholder.TabIndex = 110;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(29, 6);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 21);
            this.lblName.TabIndex = 44;
            this.lblName.Text = "Name:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAT
            // 
            this.lblAT.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblAT.Location = new System.Drawing.Point(5, 35);
            this.lblAT.Name = "lblAT";
            this.lblAT.Size = new System.Drawing.Size(69, 21);
            this.lblAT.TabIndex = 45;
            this.lblAT.Text = "Archetype:";
            this.lblAT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            this.lblOrigin.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblOrigin.Location = new System.Drawing.Point(25, 63);
            this.lblOrigin.Name = "lblOrigin";
            this.lblOrigin.Size = new System.Drawing.Size(49, 21);
            this.lblOrigin.TabIndex = 46;
            this.lblOrigin.Text = "Origin:";
            this.lblOrigin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sbMode
            // 
            this.sbMode.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sbMode.ForeColor = System.Drawing.Color.White;
            this.sbMode.Location = new System.Drawing.Point(259, 9);
            this.sbMode.Name = "sbMode";
            this.sbMode.Size = new System.Drawing.Size(105, 22);
            this.sbMode.TabIndex = 122;
            this.sbMode.Click += new System.EventHandler(this.sbMode_ButtonClicked);
            // 
            // ibSets
            // 
            this.ibSets.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSets.Location = new System.Drawing.Point(259, 37);
            this.ibSets.Name = "ibSets";
            this.ibSets.Size = new System.Drawing.Size(105, 22);
            this.ibSets.TabIndex = 98;
            // 
            // ibSlots
            // 
            this.ibSlots.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSlots.Location = new System.Drawing.Point(370, 65);
            this.ibSlots.Name = "ibSlots";
            this.ibSlots.Size = new System.Drawing.Size(105, 22);
            this.ibSlots.TabIndex = 106;
            // 
            // ibSlotLevels
            // 
            this.ibSlotLevels.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSlotLevels.Location = new System.Drawing.Point(259, 64);
            this.ibSlotLevels.Name = "ibSlotLevels";
            this.ibSlotLevels.Size = new System.Drawing.Size(105, 22);
            this.ibSlotLevels.TabIndex = 101;
            // 
            // ibTotals
            // 
            this.ibTotals.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibTotals.Location = new System.Drawing.Point(370, 37);
            this.ibTotals.Name = "ibTotals";
            this.ibTotals.Size = new System.Drawing.Size(105, 22);
            this.ibTotals.TabIndex = 99;
            // 
            // pbDynMode
            // 
            this.pbDynMode.Location = new System.Drawing.Point(370, 9);
            this.pbDynMode.Name = "pbDynMode";
            this.pbDynMode.Size = new System.Drawing.Size(105, 22);
            this.pbDynMode.TabIndex = 92;
            this.pbDynMode.TabStop = false;
            // 
            // lblLockedSecondary
            // 
            this.lblLockedSecondary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLockedSecondary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLockedSecondary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedSecondary.ForeColor = System.Drawing.Color.Black;
            this.lblLockedSecondary.Location = new System.Drawing.Point(177, 146);
            this.lblLockedSecondary.Name = "lblLockedSecondary";
            this.lblLockedSecondary.Size = new System.Drawing.Size(152, 22);
            this.lblLockedSecondary.TabIndex = 109;
            this.lblLockedSecondary.Text = "Sec. Locked";
            this.lblLockedSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblATLocked
            // 
            this.lblATLocked.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblATLocked.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblATLocked.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblATLocked.ForeColor = System.Drawing.Color.Black;
            this.lblATLocked.Location = new System.Drawing.Point(80, 35);
            this.lblATLocked.Name = "lblATLocked";
            this.lblATLocked.Size = new System.Drawing.Size(144, 23);
            this.lblATLocked.TabIndex = 53;
            this.lblATLocked.Text = "Archetype Locked";
            this.lblATLocked.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtName.ForeColor = System.Drawing.Color.Black;
            this.txtName.Location = new System.Drawing.Point(80, 7);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(144, 20);
            this.txtName.TabIndex = 1;
            // 
            // cbSecondary
            // 
            this.cbSecondary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbSecondary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecondary.ForeColor = System.Drawing.Color.Black;
            this.cbSecondary.ItemHeight = 16;
            this.cbSecondary.Location = new System.Drawing.Point(177, 147);
            this.cbSecondary.MaxDropDownItems = 15;
            this.cbSecondary.Name = "cbSecondary";
            this.cbSecondary.Size = new System.Drawing.Size(152, 22);
            this.cbSecondary.TabIndex = 11;
            // 
            // lblSecondary
            // 
            this.lblSecondary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblSecondary.ForeColor = System.Drawing.Color.White;
            this.lblSecondary.Location = new System.Drawing.Point(175, 127);
            this.lblSecondary.Name = "lblSecondary";
            this.lblSecondary.Size = new System.Drawing.Size(154, 17);
            this.lblSecondary.TabIndex = 10;
            this.lblSecondary.Text = "Secondary Power Set";
            this.lblSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPrimary
            // 
            this.cbPrimary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPrimary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrimary.ForeColor = System.Drawing.Color.Black;
            this.cbPrimary.ItemHeight = 16;
            this.cbPrimary.Location = new System.Drawing.Point(12, 147);
            this.cbPrimary.MaxDropDownItems = 15;
            this.cbPrimary.Name = "cbPrimary";
            this.cbPrimary.Size = new System.Drawing.Size(152, 22);
            this.cbPrimary.TabIndex = 7;
            // 
            // cbOrigin
            // 
            this.cbOrigin.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbOrigin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrigin.ForeColor = System.Drawing.Color.Black;
            this.cbOrigin.ItemHeight = 17;
            this.cbOrigin.Location = new System.Drawing.Point(80, 64);
            this.cbOrigin.Name = "cbOrigin";
            this.cbOrigin.Size = new System.Drawing.Size(144, 23);
            this.cbOrigin.TabIndex = 5;
            // 
            // cbAT
            // 
            this.cbAT.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbAT.DisplayMember = "DisplayName";
            this.cbAT.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAT.ForeColor = System.Drawing.Color.Black;
            this.cbAT.ItemHeight = 17;
            this.cbAT.Location = new System.Drawing.Point(80, 35);
            this.cbAT.MaxDropDownItems = 15;
            this.cbAT.Name = "cbAT";
            this.cbAT.Size = new System.Drawing.Size(144, 23);
            this.cbAT.TabIndex = 3;
            this.cbAT.ValueMember = "Idx";
            // 
            // lblPrimary
            // 
            this.lblPrimary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPrimary.ForeColor = System.Drawing.Color.White;
            this.lblPrimary.Location = new System.Drawing.Point(12, 127);
            this.lblPrimary.Name = "lblPrimary";
            this.lblPrimary.Size = new System.Drawing.Size(152, 17);
            this.lblPrimary.TabIndex = 9;
            this.lblPrimary.Text = "Primary Power Set";
            this.lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPool1
            // 
            this.lblPool1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool1.ForeColor = System.Drawing.Color.White;
            this.lblPool1.Location = new System.Drawing.Point(337, 127);
            this.lblPool1.Name = "lblPool1";
            this.lblPool1.Size = new System.Drawing.Size(138, 17);
            this.lblPool1.TabIndex = 14;
            this.lblPool1.Text = "Pool 1";
            this.lblPool1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool0
            // 
            this.cbPool0.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool0.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool0.ForeColor = System.Drawing.Color.Black;
            this.cbPool0.ItemHeight = 16;
            this.cbPool0.Location = new System.Drawing.Point(337, 147);
            this.cbPool0.MaxDropDownItems = 15;
            this.cbPool0.Name = "cbPool0";
            this.cbPool0.Size = new System.Drawing.Size(138, 22);
            this.cbPool0.TabIndex = 15;
            // 
            // lblPool2
            // 
            this.lblPool2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool2.ForeColor = System.Drawing.Color.White;
            this.lblPool2.Location = new System.Drawing.Point(337, 273);
            this.lblPool2.Name = "lblPool2";
            this.lblPool2.Size = new System.Drawing.Size(138, 17);
            this.lblPool2.TabIndex = 17;
            this.lblPool2.Text = "Pool 2";
            this.lblPool2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool1
            // 
            this.cbPool1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool1.ForeColor = System.Drawing.Color.Black;
            this.cbPool1.ItemHeight = 16;
            this.cbPool1.Location = new System.Drawing.Point(337, 293);
            this.cbPool1.MaxDropDownItems = 15;
            this.cbPool1.Name = "cbPool1";
            this.cbPool1.Size = new System.Drawing.Size(138, 22);
            this.cbPool1.TabIndex = 18;
            // 
            // lblPool3
            // 
            this.lblPool3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool3.ForeColor = System.Drawing.Color.White;
            this.lblPool3.Location = new System.Drawing.Point(337, 419);
            this.lblPool3.Name = "lblPool3";
            this.lblPool3.Size = new System.Drawing.Size(138, 17);
            this.lblPool3.TabIndex = 20;
            this.lblPool3.Text = "Pool 3";
            this.lblPool3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool2
            // 
            this.cbPool2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool2.ForeColor = System.Drawing.Color.Black;
            this.cbPool2.ItemHeight = 16;
            this.cbPool2.Location = new System.Drawing.Point(337, 439);
            this.cbPool2.MaxDropDownItems = 15;
            this.cbPool2.Name = "cbPool2";
            this.cbPool2.Size = new System.Drawing.Size(138, 22);
            this.cbPool2.TabIndex = 21;
            // 
            // lblPool4
            // 
            this.lblPool4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool4.ForeColor = System.Drawing.Color.White;
            this.lblPool4.Location = new System.Drawing.Point(337, 565);
            this.lblPool4.Name = "lblPool4";
            this.lblPool4.Size = new System.Drawing.Size(138, 17);
            this.lblPool4.TabIndex = 23;
            this.lblPool4.Text = "Pool 4";
            this.lblPool4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool3
            // 
            this.cbPool3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool3.ForeColor = System.Drawing.Color.Black;
            this.cbPool3.ItemHeight = 16;
            this.cbPool3.Location = new System.Drawing.Point(337, 585);
            this.cbPool3.MaxDropDownItems = 15;
            this.cbPool3.Name = "cbPool3";
            this.cbPool3.Size = new System.Drawing.Size(138, 22);
            this.cbPool3.TabIndex = 24;
            // 
            // lblEpic
            // 
            this.lblEpic.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblEpic.ForeColor = System.Drawing.Color.White;
            this.lblEpic.Location = new System.Drawing.Point(335, 711);
            this.lblEpic.Name = "lblEpic";
            this.lblEpic.Size = new System.Drawing.Size(140, 17);
            this.lblEpic.TabIndex = 26;
            this.lblEpic.Text = "Ancillary/Epic Pool";
            this.lblEpic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbAncillary
            // 
            this.cbAncillary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbAncillary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbAncillary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAncillary.ForeColor = System.Drawing.Color.Black;
            this.cbAncillary.ItemHeight = 16;
            this.cbAncillary.Location = new System.Drawing.Point(337, 731);
            this.cbAncillary.Name = "cbAncillary";
            this.cbAncillary.Size = new System.Drawing.Size(138, 22);
            this.cbAncillary.TabIndex = 27;
            // 
            // lblLocked0
            // 
            this.lblLocked0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLocked0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked0.ForeColor = System.Drawing.Color.Black;
            this.lblLocked0.Location = new System.Drawing.Point(337, 146);
            this.lblLocked0.Name = "lblLocked0";
            this.lblLocked0.Size = new System.Drawing.Size(138, 22);
            this.lblLocked0.TabIndex = 72;
            this.lblLocked0.Text = "Pool Locked";
            this.lblLocked0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLocked1
            // 
            this.lblLocked1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLocked1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked1.ForeColor = System.Drawing.Color.Black;
            this.lblLocked1.Location = new System.Drawing.Point(337, 290);
            this.lblLocked1.Name = "lblLocked1";
            this.lblLocked1.Size = new System.Drawing.Size(138, 22);
            this.lblLocked1.TabIndex = 73;
            this.lblLocked1.Text = "Pool Locked";
            this.lblLocked1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLocked2
            // 
            this.lblLocked2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLocked2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked2.ForeColor = System.Drawing.Color.Black;
            this.lblLocked2.Location = new System.Drawing.Point(337, 439);
            this.lblLocked2.Name = "lblLocked2";
            this.lblLocked2.Size = new System.Drawing.Size(138, 22);
            this.lblLocked2.TabIndex = 74;
            this.lblLocked2.Text = "Pool Locked";
            this.lblLocked2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLocked3
            // 
            this.lblLocked3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLocked3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked3.ForeColor = System.Drawing.Color.Black;
            this.lblLocked3.Location = new System.Drawing.Point(337, 584);
            this.lblLocked3.Name = "lblLocked3";
            this.lblLocked3.Size = new System.Drawing.Size(138, 22);
            this.lblLocked3.TabIndex = 75;
            this.lblLocked3.Text = "Pool Locked";
            this.lblLocked3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLockedAncillary
            // 
            this.lblLockedAncillary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLockedAncillary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLockedAncillary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedAncillary.ForeColor = System.Drawing.Color.Black;
            this.lblLockedAncillary.Location = new System.Drawing.Point(337, 730);
            this.lblLockedAncillary.Name = "lblLockedAncillary";
            this.lblLockedAncillary.Size = new System.Drawing.Size(138, 22);
            this.lblLockedAncillary.TabIndex = 76;
            this.lblLockedAncillary.Text = "Pool Locked";
            this.lblLockedAncillary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tTip
            // 
            this.tTip.AutoPopDelay = 5000;
            this.tTip.InitialDelay = 500;
            this.tTip.ReshowDelay = 100;
            // 
            // menuBar
            // 
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ImportExportToolStripMenuItem,
            this.OptionsToolStripMenuItem,
            this.CharacterToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.HelpToolStripMenuItem1,
            this.WindowToolStripMenuItem});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(1326, 24);
            this.menuBar.TabIndex = 3;
            this.menuBar.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsFileNew,
            this.ToolStripSeparator7,
            this.tsFileOpen,
            this.tsFileSave,
            this.tsFileSaveAs,
            this.ToolStripSeparator8,
            this.tsFilePrint,
            this.ToolStripSeparator9,
            this.tsFileQuit});
            this.FileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // tsFileNew
            // 
            this.tsFileNew.Name = "tsFileNew";
            this.tsFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsFileNew.Size = new System.Drawing.Size(179, 22);
            this.tsFileNew.Text = "&New / Clear";
            this.tsFileNew.Click += new System.EventHandler(this.tsFileNew_Click);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(176, 6);
            // 
            // tsFileOpen
            // 
            this.tsFileOpen.Name = "tsFileOpen";
            this.tsFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsFileOpen.Size = new System.Drawing.Size(179, 22);
            this.tsFileOpen.Text = "&Open...";
            this.tsFileOpen.Click += new System.EventHandler(this.tsFileOpen_Click);
            // 
            // tsFileSave
            // 
            this.tsFileSave.Name = "tsFileSave";
            this.tsFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsFileSave.Size = new System.Drawing.Size(179, 22);
            this.tsFileSave.Text = "&Save";
            this.tsFileSave.Click += new System.EventHandler(this.tsFileSave_Click);
            // 
            // tsFileSaveAs
            // 
            this.tsFileSaveAs.Name = "tsFileSaveAs";
            this.tsFileSaveAs.Size = new System.Drawing.Size(179, 22);
            this.tsFileSaveAs.Text = "Save &As...";
            this.tsFileSaveAs.Click += new System.EventHandler(this.tsFileSaveAs_Click);
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(176, 6);
            // 
            // tsFilePrint
            // 
            this.tsFilePrint.Name = "tsFilePrint";
            this.tsFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.tsFilePrint.Size = new System.Drawing.Size(179, 22);
            this.tsFilePrint.Text = "&Print...";
            this.tsFilePrint.Click += new System.EventHandler(this.tsFilePrint_Click);
            // 
            // ToolStripSeparator9
            // 
            this.ToolStripSeparator9.Name = "ToolStripSeparator9";
            this.ToolStripSeparator9.Size = new System.Drawing.Size(176, 6);
            // 
            // tsFileQuit
            // 
            this.tsFileQuit.Name = "tsFileQuit";
            this.tsFileQuit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tsFileQuit.Size = new System.Drawing.Size(179, 22);
            this.tsFileQuit.Text = "&Quit";
            this.tsFileQuit.Click += new System.EventHandler(this.tsFileQuit_Click);
            // 
            // ImportExportToolStripMenuItem
            // 
            this.ImportExportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsImport,
            this.ToolStripSeparator12,
            this.tsExport,
            this.tsExportLong,
            this.tsExportDataLink,
            this.ToolStripSeparator25,
            this.tsGenFreebies,
            this.ToolStripSeparator27,
            this.tsExportDiscord});
            this.ImportExportToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ImportExportToolStripMenuItem.Name = "ImportExportToolStripMenuItem";
            this.ImportExportToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.ImportExportToolStripMenuItem.Text = "&Import / Export";
            // 
            // tsImport
            // 
            this.tsImport.Name = "tsImport";
            this.tsImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsImport.Size = new System.Drawing.Size(240, 22);
            this.tsImport.Text = "&Import from Forum Post";
            this.tsImport.Click += new System.EventHandler(this.tsImport_Click);
            // 
            // ToolStripSeparator12
            // 
            this.ToolStripSeparator12.Name = "ToolStripSeparator12";
            this.ToolStripSeparator12.Size = new System.Drawing.Size(237, 6);
            // 
            // tsExport
            // 
            this.tsExport.Name = "tsExport";
            this.tsExport.Size = new System.Drawing.Size(240, 22);
            this.tsExport.Text = "&Short Forum Export...";
            this.tsExport.Click += new System.EventHandler(this.tsExport_Click);
            // 
            // tsExportLong
            // 
            this.tsExportLong.Name = "tsExportLong";
            this.tsExportLong.Size = new System.Drawing.Size(240, 22);
            this.tsExportLong.Text = "&Long Forum Export...";
            this.tsExportLong.Click += new System.EventHandler(this.tsExportLong_Click);
            // 
            // tsExportDataLink
            // 
            this.tsExportDataLink.Name = "tsExportDataLink";
            this.tsExportDataLink.Size = new System.Drawing.Size(240, 22);
            this.tsExportDataLink.Text = "Export Data Link";
            this.tsExportDataLink.Click += new System.EventHandler(this.tsExportDataLink_Click);
            // 
            // ToolStripSeparator25
            // 
            this.ToolStripSeparator25.Name = "ToolStripSeparator25";
            this.ToolStripSeparator25.Size = new System.Drawing.Size(237, 6);
            // 
            // tsGenFreebies
            // 
            this.tsGenFreebies.Name = "tsGenFreebies";
            this.tsGenFreebies.Size = new System.Drawing.Size(240, 22);
            this.tsGenFreebies.Text = "Export Build to Beta Server";
            this.tsGenFreebies.Click += new System.EventHandler(this.tsGenFreebies_Click);
            // 
            // ToolStripSeparator27
            // 
            this.ToolStripSeparator27.Name = "ToolStripSeparator27";
            this.ToolStripSeparator27.Size = new System.Drawing.Size(237, 6);
            // 
            // tsExportDiscord
            // 
            this.tsExportDiscord.Name = "tsExportDiscord";
            this.tsExportDiscord.Size = new System.Drawing.Size(240, 22);
            this.tsExportDiscord.Text = "Export to Discord";
            this.tsExportDiscord.Click += new System.EventHandler(this.tsExportDiscord_Click);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsConfig,
            this.ToolStripSeparator14,
            this.tsUpdateCheck,
            this.ToolStripSeparator5,
            this.AdvancedToolStripMenuItem1});
            this.OptionsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.OptionsToolStripMenuItem.Text = "&Options";
            // 
            // tsConfig
            // 
            this.tsConfig.Name = "tsConfig";
            this.tsConfig.Size = new System.Drawing.Size(199, 22);
            this.tsConfig.Text = "&Configuration...";
            this.tsConfig.Click += new System.EventHandler(this.tsConfig_Click);
            // 
            // ToolStripSeparator14
            // 
            this.ToolStripSeparator14.Name = "ToolStripSeparator14";
            this.ToolStripSeparator14.Size = new System.Drawing.Size(196, 6);
            // 
            // tsUpdateCheck
            // 
            this.tsUpdateCheck.Name = "tsUpdateCheck";
            this.tsUpdateCheck.Size = new System.Drawing.Size(199, 22);
            this.tsUpdateCheck.Text = "Check for &Updates Now";
            this.tsUpdateCheck.Click += new System.EventHandler(this.tsUpdateCheck_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(196, 6);
            // 
            // AdvancedToolStripMenuItem1
            // 
            this.AdvancedToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsAdvDBEdit,
            this.ToolStripSeparator15,
            this.tsAdvFreshInstall,
            this.tsAdvResetTips});
            this.AdvancedToolStripMenuItem1.Name = "AdvancedToolStripMenuItem1";
            this.AdvancedToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.AdvancedToolStripMenuItem1.Text = "&Advanced";
            // 
            // tsAdvDBEdit
            // 
            this.tsAdvDBEdit.Name = "tsAdvDBEdit";
            this.tsAdvDBEdit.Size = new System.Drawing.Size(165, 22);
            this.tsAdvDBEdit.Text = "&Database Editor...";
            this.tsAdvDBEdit.Click += new System.EventHandler(this.tsAdvDBEdit_Click);
            // 
            // ToolStripSeparator15
            // 
            this.ToolStripSeparator15.Name = "ToolStripSeparator15";
            this.ToolStripSeparator15.Size = new System.Drawing.Size(162, 6);
            this.ToolStripSeparator15.Visible = false;
            // 
            // tsAdvFreshInstall
            // 
            this.tsAdvFreshInstall.Name = "tsAdvFreshInstall";
            this.tsAdvFreshInstall.Size = new System.Drawing.Size(165, 22);
            this.tsAdvFreshInstall.Text = "FreshInstall Flag";
            this.tsAdvFreshInstall.Visible = false;
            this.tsAdvFreshInstall.Click += new System.EventHandler(this.tsAdvFreshInstall_Click);
            // 
            // tsAdvResetTips
            // 
            this.tsAdvResetTips.Name = "tsAdvResetTips";
            this.tsAdvResetTips.Size = new System.Drawing.Size(165, 22);
            this.tsAdvResetTips.Text = "Reset Tips";
            this.tsAdvResetTips.Visible = false;
            this.tsAdvResetTips.Click += new System.EventHandler(this.tsAdvResetTips_Click);
            // 
            // CharacterToolStripMenuItem
            // 
            this.CharacterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetAllIOsToDefault35ToolStripMenuItem,
            this.ToolStripSeparator16,
            this.ToolStripMenuItem1,
            this.ToolStripMenuItem2,
            this.ToolStripSeparator17,
            this.SlotsToolStripMenuItem,
            this.ToolStripSeparator28,
            this.ToggleCheckModeToolStripMenuItem});
            this.CharacterToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CharacterToolStripMenuItem.Name = "CharacterToolStripMenuItem";
            this.CharacterToolStripMenuItem.Size = new System.Drawing.Size(133, 20);
            this.CharacterToolStripMenuItem.Text = "&Slots / Enhancements";
            // 
            // SetAllIOsToDefault35ToolStripMenuItem
            // 
            this.SetAllIOsToDefault35ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsIODefault,
            this.ToolStripSeparator11,
            this.tsIOMin,
            this.tsIOMax});
            this.SetAllIOsToDefault35ToolStripMenuItem.Name = "SetAllIOsToDefault35ToolStripMenuItem";
            this.SetAllIOsToDefault35ToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.SetAllIOsToDefault35ToolStripMenuItem.Text = "&Set all IOs to...";
            // 
            // tsIODefault
            // 
            this.tsIODefault.Name = "tsIODefault";
            this.tsIODefault.Size = new System.Drawing.Size(135, 22);
            this.tsIODefault.Text = "Default (35)";
            this.tsIODefault.Click += new System.EventHandler(this.tsIODefault_Click);
            // 
            // ToolStripSeparator11
            // 
            this.ToolStripSeparator11.Name = "ToolStripSeparator11";
            this.ToolStripSeparator11.Size = new System.Drawing.Size(132, 6);
            // 
            // tsIOMin
            // 
            this.tsIOMin.Name = "tsIOMin";
            this.tsIOMin.Size = new System.Drawing.Size(135, 22);
            this.tsIOMin.Text = "Minimum";
            this.tsIOMin.Click += new System.EventHandler(this.tsIOMin_Click);
            // 
            // tsIOMax
            // 
            this.tsIOMax.Name = "tsIOMax";
            this.tsIOMax.Size = new System.Drawing.Size(135, 22);
            this.tsIOMax.Text = "Maximum";
            this.tsIOMax.Click += new System.EventHandler(this.tsIOMax_Click);
            // 
            // ToolStripSeparator16
            // 
            this.ToolStripSeparator16.Name = "ToolStripSeparator16";
            this.ToolStripSeparator16.Size = new System.Drawing.Size(252, 6);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEnhToSO,
            this.tsEnhToDO,
            this.tsEnhToTO});
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(255, 22);
            this.ToolStripMenuItem1.Text = "Set all Enhancements\' &Origin to...";
            // 
            // tsEnhToSO
            // 
            this.tsEnhToSO.Name = "tsEnhToSO";
            this.tsEnhToSO.Size = new System.Drawing.Size(142, 22);
            this.tsEnhToSO.Text = "Single Origin";
            this.tsEnhToSO.Click += new System.EventHandler(this.tsEnhToSO_Click);
            // 
            // tsEnhToDO
            // 
            this.tsEnhToDO.Name = "tsEnhToDO";
            this.tsEnhToDO.Size = new System.Drawing.Size(142, 22);
            this.tsEnhToDO.Text = "Dual Origin";
            this.tsEnhToDO.Click += new System.EventHandler(this.tsEnhToDO_Click);
            // 
            // tsEnhToTO
            // 
            this.tsEnhToTO.Name = "tsEnhToTO";
            this.tsEnhToTO.Size = new System.Drawing.Size(142, 22);
            this.tsEnhToTO.Text = "Training";
            this.tsEnhToTO.Click += new System.EventHandler(this.tsEnhToTO_Click);
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEnhToPlus5,
            this.tsEnhToPlus4,
            this.tsEnhToPlus3,
            this.tsEnhToPlus2,
            this.tsEnhToPlus1,
            this.tsEnhToEven,
            this.tsEnhToMinus1,
            this.tsEnhToMinus2,
            this.tsEnhToMinus3,
            this.tsEnhToNone});
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(255, 22);
            this.ToolStripMenuItem2.Text = "Set all &Relative Levels to...";
            // 
            // tsEnhToPlus5
            // 
            this.tsEnhToPlus5.Name = "tsEnhToPlus5";
            this.tsEnhToPlus5.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToPlus5.Text = "+5 Levels";
            this.tsEnhToPlus5.Click += new System.EventHandler(this.tsEnhToPlus5_Click);
            // 
            // tsEnhToPlus4
            // 
            this.tsEnhToPlus4.Name = "tsEnhToPlus4";
            this.tsEnhToPlus4.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToPlus4.Text = "+4 Levels";
            this.tsEnhToPlus4.Click += new System.EventHandler(this.tsEnhToPlus4_Click);
            // 
            // tsEnhToPlus3
            // 
            this.tsEnhToPlus3.Name = "tsEnhToPlus3";
            this.tsEnhToPlus3.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToPlus3.Text = "+3 Levels";
            this.tsEnhToPlus3.Click += new System.EventHandler(this.tsEnhToPlus3_Click);
            // 
            // tsEnhToPlus2
            // 
            this.tsEnhToPlus2.Name = "tsEnhToPlus2";
            this.tsEnhToPlus2.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToPlus2.Text = "+2 Levels";
            this.tsEnhToPlus2.Click += new System.EventHandler(this.tsEnhToPlus2_Click);
            // 
            // tsEnhToPlus1
            // 
            this.tsEnhToPlus1.Name = "tsEnhToPlus1";
            this.tsEnhToPlus1.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToPlus1.Text = "+1 Level";
            this.tsEnhToPlus1.Click += new System.EventHandler(this.tsEnhToPlus1_Click);
            // 
            // tsEnhToEven
            // 
            this.tsEnhToEven.Name = "tsEnhToEven";
            this.tsEnhToEven.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToEven.Text = "Even Level";
            this.tsEnhToEven.Click += new System.EventHandler(this.tsEnhToEven_Click);
            // 
            // tsEnhToMinus1
            // 
            this.tsEnhToMinus1.Name = "tsEnhToMinus1";
            this.tsEnhToMinus1.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToMinus1.Text = "-1 Level";
            this.tsEnhToMinus1.Click += new System.EventHandler(this.tsEnhToMinus1_Click);
            // 
            // tsEnhToMinus2
            // 
            this.tsEnhToMinus2.Name = "tsEnhToMinus2";
            this.tsEnhToMinus2.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToMinus2.Text = "-2 Levels";
            this.tsEnhToMinus2.Click += new System.EventHandler(this.tsEnhToMinus2_Click);
            // 
            // tsEnhToMinus3
            // 
            this.tsEnhToMinus3.Name = "tsEnhToMinus3";
            this.tsEnhToMinus3.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToMinus3.Text = "-3 Levels";
            this.tsEnhToMinus3.Click += new System.EventHandler(this.tsEnhToMinus3_Click);
            // 
            // tsEnhToNone
            // 
            this.tsEnhToNone.Name = "tsEnhToNone";
            this.tsEnhToNone.Size = new System.Drawing.Size(205, 22);
            this.tsEnhToNone.Text = "None (Enh has no effect)";
            this.tsEnhToNone.Click += new System.EventHandler(this.tsEnhToNone_Click);
            // 
            // ToolStripSeparator17
            // 
            this.ToolStripSeparator17.Name = "ToolStripSeparator17";
            this.ToolStripSeparator17.Size = new System.Drawing.Size(252, 6);
            // 
            // SlotsToolStripMenuItem
            // 
            this.SlotsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsFlipAllEnh,
            this.ToolStripSeparator4,
            this.tsClearAllEnh,
            this.tsRemoveAllSlots,
            this.ToolStripSeparator1,
            this.AutoArrangeAllSlotsToolStripMenuItem});
            this.SlotsToolStripMenuItem.Name = "SlotsToolStripMenuItem";
            this.SlotsToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.SlotsToolStripMenuItem.Text = "Slo&ts";
            // 
            // tsFlipAllEnh
            // 
            this.tsFlipAllEnh.Name = "tsFlipAllEnh";
            this.tsFlipAllEnh.Size = new System.Drawing.Size(199, 22);
            this.tsFlipAllEnh.Text = "Flip All to Alternate";
            this.tsFlipAllEnh.Click += new System.EventHandler(this.tsFlipAllEnh_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(196, 6);
            // 
            // tsClearAllEnh
            // 
            this.tsClearAllEnh.Name = "tsClearAllEnh";
            this.tsClearAllEnh.Size = new System.Drawing.Size(199, 22);
            this.tsClearAllEnh.Text = "Clear All Enhancements";
            this.tsClearAllEnh.Click += new System.EventHandler(this.tsClearAllEnh_Click);
            // 
            // tsRemoveAllSlots
            // 
            this.tsRemoveAllSlots.Name = "tsRemoveAllSlots";
            this.tsRemoveAllSlots.Size = new System.Drawing.Size(199, 22);
            this.tsRemoveAllSlots.Text = "Remove All Slots";
            this.tsRemoveAllSlots.Click += new System.EventHandler(this.tsRemoveAllSlots_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // AutoArrangeAllSlotsToolStripMenuItem
            // 
            this.AutoArrangeAllSlotsToolStripMenuItem.Name = "AutoArrangeAllSlotsToolStripMenuItem";
            this.AutoArrangeAllSlotsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.AutoArrangeAllSlotsToolStripMenuItem.Text = "&Auto-Arrange All Slots";
            this.AutoArrangeAllSlotsToolStripMenuItem.Click += new System.EventHandler(this.AutoArrangeAllSlotsToolStripMenuItem_Click);
            // 
            // ToolStripSeparator28
            // 
            this.ToolStripSeparator28.Name = "ToolStripSeparator28";
            this.ToolStripSeparator28.Size = new System.Drawing.Size(252, 6);
            // 
            // ToggleCheckModeToolStripMenuItem
            // 
            this.ToggleCheckModeToolStripMenuItem.Name = "ToggleCheckModeToolStripMenuItem";
            this.ToggleCheckModeToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.ToggleCheckModeToolStripMenuItem.Text = "Toggle Enhancement Check Mode";
            this.ToggleCheckModeToolStripMenuItem.Click += new System.EventHandler(this.tsToggleCheckModeToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsView2Col,
            this.tsView3Col,
            this.tsView4Col,
            this.tsView5Col,
            this.tsView6Col,
            this.ToolStripSeparator13,
            this.tsViewIOLevels,
            this.tsViewRelative,
            this.tsViewSlotLevels,
            this.tsViewRelativeAsSigns,
            this.ToolStripSeparator2,
            this.tsViewActualDamage_New,
            this.tsViewDPS_New,
            this.tlsDPA});
            this.ViewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "&View";
            // 
            // tsView2Col
            // 
            this.tsView2Col.Name = "tsView2Col";
            this.tsView2Col.Size = new System.Drawing.Size(282, 22);
            this.tsView2Col.Text = "&2 Columns";
            this.tsView2Col.Click += new System.EventHandler(this.tsView2Col_Click);
            // 
            // tsView3Col
            // 
            this.tsView3Col.Name = "tsView3Col";
            this.tsView3Col.Size = new System.Drawing.Size(282, 22);
            this.tsView3Col.Text = "&3 Columns";
            this.tsView3Col.Click += new System.EventHandler(this.tsView3Col_Click);
            // 
            // tsView4Col
            // 
            this.tsView4Col.Name = "tsView4Col";
            this.tsView4Col.Size = new System.Drawing.Size(282, 22);
            this.tsView4Col.Text = "&4 Columns";
            this.tsView4Col.Click += new System.EventHandler(this.tsView4Col_Click);
            // 
            // tsView5Col
            // 
            this.tsView5Col.Name = "tsView5Col";
            this.tsView5Col.Size = new System.Drawing.Size(282, 22);
            this.tsView5Col.Text = "&5 Columns";
            this.tsView5Col.Click += new System.EventHandler(this.tsView5Col_Click);
            // 
            // tsView6Col
            // 
            this.tsView6Col.Name = "tsView6Col";
            this.tsView6Col.Size = new System.Drawing.Size(282, 22);
            this.tsView6Col.Text = "&6 Columns";
            this.tsView6Col.Click += new System.EventHandler(this.tsView6Col_Click);
            // 
            // ToolStripSeparator13
            // 
            this.ToolStripSeparator13.Name = "ToolStripSeparator13";
            this.ToolStripSeparator13.Size = new System.Drawing.Size(279, 6);
            // 
            // tsViewIOLevels
            // 
            this.tsViewIOLevels.Checked = true;
            this.tsViewIOLevels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsViewIOLevels.Name = "tsViewIOLevels";
            this.tsViewIOLevels.Size = new System.Drawing.Size(282, 22);
            this.tsViewIOLevels.Text = "Show &IO Levels";
            this.tsViewIOLevels.Click += new System.EventHandler(this.tsViewIOLevels_Click);
            // 
            // tsViewRelative
            // 
            this.tsViewRelative.Name = "tsViewRelative";
            this.tsViewRelative.Size = new System.Drawing.Size(282, 22);
            this.tsViewRelative.Text = "Show &Enhancement Relative Levels";
            this.tsViewRelative.Click += new System.EventHandler(this.tsViewRelative_Click);
            // 
            // tsViewSlotLevels
            // 
            this.tsViewSlotLevels.Name = "tsViewSlotLevels";
            this.tsViewSlotLevels.Size = new System.Drawing.Size(282, 22);
            this.tsViewSlotLevels.Text = "Show &Slot Placement Levels";
            this.tsViewSlotLevels.Click += new System.EventHandler(this.tsViewSlotLevels_Click);
            // 
            // tsViewRelativeAsSigns
            // 
            this.tsViewRelativeAsSigns.Name = "tsViewRelativeAsSigns";
            this.tsViewRelativeAsSigns.Size = new System.Drawing.Size(282, 22);
            this.tsViewRelativeAsSigns.Text = "Show Relative Levels with signs (\'+\'/\'-\')";
            this.tsViewRelativeAsSigns.Click += new System.EventHandler(this.tsViewRelativeAsSigns_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(279, 6);
            // 
            // tsViewActualDamage_New
            // 
            this.tsViewActualDamage_New.Checked = true;
            this.tsViewActualDamage_New.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsViewActualDamage_New.Name = "tsViewActualDamage_New";
            this.tsViewActualDamage_New.Size = new System.Drawing.Size(282, 22);
            this.tsViewActualDamage_New.Text = "Show Damage Per Activation (Level 50)";
            this.tsViewActualDamage_New.Click += new System.EventHandler(this.tsViewActualDamage_New_Click);
            // 
            // tsViewDPS_New
            // 
            this.tsViewDPS_New.Name = "tsViewDPS_New";
            this.tsViewDPS_New.Size = new System.Drawing.Size(282, 22);
            this.tsViewDPS_New.Text = "Show Damage Per Second (Level 50)";
            this.tsViewDPS_New.Click += new System.EventHandler(this.tsViewDPS_New_Click);
            // 
            // tlsDPA
            // 
            this.tlsDPA.Name = "tlsDPA";
            this.tlsDPA.Size = new System.Drawing.Size(282, 22);
            this.tlsDPA.Text = "Show Damage Per Animation (Level 50)";
            this.tlsDPA.Click += new System.EventHandler(this.tlsDPA_Click);
            // 
            // HelpToolStripMenuItem1
            // 
            this.HelpToolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.HelpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsHelp,
            this.tsPatchNotes,
            this.ToolStripSeparator10,
            this.tsBugCrytilis,
            this.tsHCMRBForum,
            this.ToolStripSeparator23,
            this.tsKoFi,
            this.tsPatreon,
            this.ToolStripSeparator24,
            this.tsGitHubCrytilis});
            this.HelpToolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.HelpToolStripMenuItem1.Name = "HelpToolStripMenuItem1";
            this.HelpToolStripMenuItem1.Size = new System.Drawing.Size(102, 20);
            this.HelpToolStripMenuItem1.Text = "&Help && Support";
            // 
            // tsHelp
            // 
            this.tsHelp.Name = "tsHelp";
            this.tsHelp.Size = new System.Drawing.Size(266, 22);
            // 
            // tsPatchNotes
            // 
            this.tsPatchNotes.Name = "tsPatchNotes";
            this.tsPatchNotes.Size = new System.Drawing.Size(266, 22);
            this.tsPatchNotes.Text = "&View Patch Notes";
            this.tsPatchNotes.Click += new System.EventHandler(this.tsPatchNotes_Click);
            // 
            // ToolStripSeparator10
            // 
            this.ToolStripSeparator10.Name = "ToolStripSeparator10";
            this.ToolStripSeparator10.Size = new System.Drawing.Size(263, 6);
            // 
            // tsBugCrytilis
            // 
            this.tsBugCrytilis.Name = "tsBugCrytilis";
            this.tsBugCrytilis.Size = new System.Drawing.Size(266, 22);
            this.tsBugCrytilis.Text = "F&ile Issue/Suggestion Report";
            this.tsBugCrytilis.Click += new System.EventHandler(this.tsBugReportCrytilis_Click);
            // 
            // tsHCMRBForum
            // 
            this.tsHCMRBForum.Name = "tsHCMRBForum";
            this.tsHCMRBForum.Size = new System.Drawing.Size(266, 22);
            this.tsHCMRBForum.Text = "Official Homecoming Forum Thread";
            this.tsHCMRBForum.Click += new System.EventHandler(this.TsForumLink);
            // 
            // ToolStripSeparator23
            // 
            this.ToolStripSeparator23.Name = "ToolStripSeparator23";
            this.ToolStripSeparator23.Size = new System.Drawing.Size(263, 6);
            // 
            // tsKoFi
            // 
            this.tsKoFi.Name = "tsKoFi";
            this.tsKoFi.Size = new System.Drawing.Size(266, 22);
            this.tsKoFi.Text = "Support MRB via Ko-Fi";
            this.tsKoFi.Click += new System.EventHandler(this.tsKoFi_Click);
            // 
            // tsPatreon
            // 
            this.tsPatreon.Name = "tsPatreon";
            this.tsPatreon.Size = new System.Drawing.Size(266, 22);
            this.tsPatreon.Text = "Support MRB via Patreon";
            this.tsPatreon.Click += new System.EventHandler(this.tsPatreon_Click);
            // 
            // ToolStripSeparator24
            // 
            this.ToolStripSeparator24.Name = "ToolStripSeparator24";
            this.ToolStripSeparator24.Size = new System.Drawing.Size(263, 6);
            // 
            // tsGitHubCrytilis
            // 
            this.tsGitHubCrytilis.Name = "tsGitHubCrytilis";
            this.tsGitHubCrytilis.Size = new System.Drawing.Size(266, 22);
            this.tsGitHubCrytilis.Text = "MRB &GitHub";
            this.tsGitHubCrytilis.Click += new System.EventHandler(this.TsCrytilisLink);
            // 
            // WindowToolStripMenuItem
            // 
            this.WindowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsViewSets,
            this.tsViewGraphs,
            this.tsViewSetCompare,
            this.tsViewData,
            this.tsViewTotals,
            this.ToolStripSeparator18,
            this.tsRecipeViewer,
            this.tsDPSCalc,
            this.ToolStripSeparator19,
            this.tsSetFind,
            this.ToolStripSeparator21,
            this.InGameRespecHelperToolStripMenuItem,
            this.ToolStripMenuItem4,
            this.AccoladesWindowToolStripMenuItem,
            this.IncarnateWindowToolStripMenuItem,
            this.TemporaryPowersWindowToolStripMenuItem});
            this.WindowToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.WindowToolStripMenuItem.Name = "WindowToolStripMenuItem";
            this.WindowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.WindowToolStripMenuItem.Text = "&Window";
            // 
            // tsViewSets
            // 
            this.tsViewSets.Name = "tsViewSets";
            this.tsViewSets.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.tsViewSets.Size = new System.Drawing.Size(232, 22);
            this.tsViewSets.Text = "&Sets && Bonuses";
            this.tsViewSets.Click += new System.EventHandler(this.tsViewSets_Click);
            // 
            // tsViewGraphs
            // 
            this.tsViewGraphs.Name = "tsViewGraphs";
            this.tsViewGraphs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.tsViewGraphs.Size = new System.Drawing.Size(232, 22);
            this.tsViewGraphs.Text = "Power &Graphs";
            this.tsViewGraphs.Click += new System.EventHandler(this.tsViewGraphs_Click);
            // 
            // tsViewSetCompare
            // 
            this.tsViewSetCompare.Name = "tsViewSetCompare";
            this.tsViewSetCompare.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsViewSetCompare.Size = new System.Drawing.Size(232, 22);
            this.tsViewSetCompare.Text = "Powerset &Comparison";
            this.tsViewSetCompare.Click += new System.EventHandler(this.tsViewSetCompare_Click);
            // 
            // tsViewData
            // 
            this.tsViewData.Name = "tsViewData";
            this.tsViewData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.tsViewData.Size = new System.Drawing.Size(232, 22);
            this.tsViewData.Text = "&Data View";
            this.tsViewData.Click += new System.EventHandler(this.tsViewData_Click);
            // 
            // tsViewTotals
            // 
            this.tsViewTotals.Name = "tsViewTotals";
            this.tsViewTotals.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tsViewTotals.Size = new System.Drawing.Size(232, 22);
            this.tsViewTotals.Text = "Advanced &Totals";
            this.tsViewTotals.Click += new System.EventHandler(this.tsViewTotals_Click);
            // 
            // ToolStripSeparator18
            // 
            this.ToolStripSeparator18.Name = "ToolStripSeparator18";
            this.ToolStripSeparator18.Size = new System.Drawing.Size(229, 6);
            // 
            // tsRecipeViewer
            // 
            this.tsRecipeViewer.Name = "tsRecipeViewer";
            this.tsRecipeViewer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsRecipeViewer.Size = new System.Drawing.Size(232, 22);
            this.tsRecipeViewer.Text = "&Recipe Viewer";
            this.tsRecipeViewer.Click += new System.EventHandler(this.tsRecipeViewer_Click);
            // 
            // tsDPSCalc
            // 
            this.tsDPSCalc.Name = "tsDPSCalc";
            this.tsDPSCalc.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.tsDPSCalc.Size = new System.Drawing.Size(232, 22);
            this.tsDPSCalc.Text = "DPS Calculator (Beta)";
            this.tsDPSCalc.Click += new System.EventHandler(this.tsDPSCalc_Click);
            // 
            // ToolStripSeparator19
            // 
            this.ToolStripSeparator19.Name = "ToolStripSeparator19";
            this.ToolStripSeparator19.Size = new System.Drawing.Size(229, 6);
            // 
            // tsSetFind
            // 
            this.tsSetFind.Name = "tsSetFind";
            this.tsSetFind.Size = new System.Drawing.Size(232, 22);
            this.tsSetFind.Text = "Set &Bonus Finder";
            this.tsSetFind.Click += new System.EventHandler(this.tsSetFind_Click);
            // 
            // ToolStripSeparator21
            // 
            this.ToolStripSeparator21.Name = "ToolStripSeparator21";
            this.ToolStripSeparator21.Size = new System.Drawing.Size(229, 6);
            // 
            // InGameRespecHelperToolStripMenuItem
            // 
            this.InGameRespecHelperToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsHelperShort,
            this.tsHelperLong,
            this.ToolStripSeparator20,
            this.tsHelperShort2,
            this.tsHelperLong2});
            this.InGameRespecHelperToolStripMenuItem.Name = "InGameRespecHelperToolStripMenuItem";
            this.InGameRespecHelperToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.InGameRespecHelperToolStripMenuItem.Text = "In-Game &Respec Helper";
            // 
            // tsHelperShort
            // 
            this.tsHelperShort.Name = "tsHelperShort";
            this.tsHelperShort.Size = new System.Drawing.Size(143, 22);
            this.tsHelperShort.Text = "Profile &Short";
            this.tsHelperShort.Click += new System.EventHandler(this.tsHelperShort_Click);
            // 
            // tsHelperLong
            // 
            this.tsHelperLong.Name = "tsHelperLong";
            this.tsHelperLong.Size = new System.Drawing.Size(143, 22);
            this.tsHelperLong.Text = "Profile &Long";
            this.tsHelperLong.Click += new System.EventHandler(this.tsHelperLong_Click);
            // 
            // ToolStripSeparator20
            // 
            this.ToolStripSeparator20.Name = "ToolStripSeparator20";
            this.ToolStripSeparator20.Size = new System.Drawing.Size(140, 6);
            // 
            // tsHelperShort2
            // 
            this.tsHelperShort2.Name = "tsHelperShort2";
            this.tsHelperShort2.Size = new System.Drawing.Size(143, 22);
            this.tsHelperShort2.Text = "History S&hort";
            this.tsHelperShort2.Click += new System.EventHandler(this.tsHelperShort2_Click);
            // 
            // tsHelperLong2
            // 
            this.tsHelperLong2.Name = "tsHelperLong2";
            this.tsHelperLong2.Size = new System.Drawing.Size(143, 22);
            this.tsHelperLong2.Text = "History L&ong";
            this.tsHelperLong2.Click += new System.EventHandler(this.tsHelperLong2_Click);
            // 
            // ToolStripMenuItem4
            // 
            this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
            this.ToolStripMenuItem4.Size = new System.Drawing.Size(229, 6);
            // 
            // AccoladesWindowToolStripMenuItem
            // 
            this.AccoladesWindowToolStripMenuItem.Name = "AccoladesWindowToolStripMenuItem";
            this.AccoladesWindowToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AccoladesWindowToolStripMenuItem.Text = "&Accolades Window";
            this.AccoladesWindowToolStripMenuItem.Click += new System.EventHandler(this.AccoladesWindowToolStripMenuItem_Click);
            // 
            // IncarnateWindowToolStripMenuItem
            // 
            this.IncarnateWindowToolStripMenuItem.Name = "IncarnateWindowToolStripMenuItem";
            this.IncarnateWindowToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.IncarnateWindowToolStripMenuItem.Text = "&Incarnate Window";
            this.IncarnateWindowToolStripMenuItem.Click += new System.EventHandler(this.IncarnateWindowToolStripMenuItem_Click);
            // 
            // TemporaryPowersWindowToolStripMenuItem
            // 
            this.TemporaryPowersWindowToolStripMenuItem.Name = "TemporaryPowersWindowToolStripMenuItem";
            this.TemporaryPowersWindowToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.TemporaryPowersWindowToolStripMenuItem.Text = "T&emporary Powers Window";
            this.TemporaryPowersWindowToolStripMenuItem.Click += new System.EventHandler(this.TemporaryPowersWindowToolStripMenuItem_Click);
            // 
            // tsViewSOLevels
            // 
            this.tsViewSOLevels.Name = "tsViewSOLevels";
            this.tsViewSOLevels.Size = new System.Drawing.Size(282, 22);
            this.tsViewSOLevels.Text = "Show SO/HO Levels";
            this.tsViewSOLevels.Click += new System.EventHandler(this.tsViewSOLevels_Click);
            // 
            // tsBuildRcv
            // 
            this.tsBuildRcv.Name = "tsBuildRcv";
            this.tsBuildRcv.Size = new System.Drawing.Size(32, 19);
            // 
            // ToolStripSeparator26
            // 
            this.ToolStripSeparator26.Name = "ToolStripSeparator26";
            this.ToolStripSeparator26.Size = new System.Drawing.Size(6, 6);
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainPanel.Controls.Add(this.pnlGFX1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(501, 88);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(825, 874);
            this.mainPanel.TabIndex = 4;
            // 
            // pnlGFX1
            // 
            this.pnlGFX1.BackColor = System.Drawing.Color.Black;
            this.mainPanel.SetFlowBreak(this.pnlGFX1, true);
            this.pnlGFX1.Location = new System.Drawing.Point(3, 3);
            this.pnlGFX1.Name = "pnlGFX1";
            this.pnlGFX1.Size = new System.Drawing.Size(810, 859);
            this.pnlGFX1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pnlGFX1.TabIndex = 0;
            this.pnlGFX1.TabStop = false;
            // 
            // FrmMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1326, 962);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.menuBar);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.MainMenuStrip = this.menuBar;
            this.Name = "FrmMain2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "frmMain2";
            this.topPanel.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlGFX1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblHero;
        private System.Windows.Forms.Button accoladeButton;
        private System.Windows.Forms.Button heroVillain;
        private System.Windows.Forms.Button petsButton;
        private System.Windows.Forms.Button tempPowersButton;
        private System.Windows.Forms.Button incarnateButton;
        private System.Windows.Forms.Button prestigeButton;
        private System.Windows.Forms.Button ibPvX;
        private System.Windows.Forms.Button ibRecipe;
        private System.Windows.Forms.Button ibPopup;
        private System.Windows.Forms.Button ibTeam;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cbAncillary;
        private System.Windows.Forms.ComboBox cbAT;
        private System.Windows.Forms.ComboBox cbOrigin;
        private System.Windows.Forms.ComboBox cbPool0;
        private System.Windows.Forms.ComboBox cbPool1;
        private System.Windows.Forms.ComboBox cbPool2;
        private System.Windows.Forms.ComboBox cbPool3;
        private System.Windows.Forms.ComboBox cbPrimary;
        private System.Windows.Forms.ComboBox cbSecondary;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAT;
        private System.Windows.Forms.Label lblOrigin;
        private System.Windows.Forms.Button sbMode;
        private System.Windows.Forms.Button ibSets;
        private System.Windows.Forms.Button ibSlots;
        private System.Windows.Forms.Button ibTotals;
        private System.Windows.Forms.Button ibSlotLevels;
        private System.Windows.Forms.Button pbDynMode;
        private System.Windows.Forms.Label lblATLocked;
        private System.Windows.Forms.Label lblEpic;
        private System.Windows.Forms.Label lblLocked0;
        private System.Windows.Forms.Label lblLocked1;
        private System.Windows.Forms.Label lblLocked2;
        private System.Windows.Forms.Label lblLocked3;
        private System.Windows.Forms.Label lblLockedAncillary;
        private System.Windows.Forms.Label lblLockedSecondary;
        private System.Windows.Forms.Label lblPool1;
        private System.Windows.Forms.Label lblPool2;
        private System.Windows.Forms.Label lblPool3;
        private System.Windows.Forms.Label lblPool4;
        private System.Windows.Forms.Label lblPrimary;
        private System.Windows.Forms.Label lblSecondary;
        private System.Windows.Forms.ToolTip tTip;
        private System.Windows.Forms.Timer tmrGfx;
        private System.Windows.Forms.OpenFileDialog DlgOpen;
        private System.Windows.Forms.SaveFileDialog DlgSave;
        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem tsAdvDBEdit;
        private System.Windows.Forms.ToolStripMenuItem tsAdvFreshInstall;
        private System.Windows.Forms.ToolStripMenuItem tsAdvResetTips;
        private System.Windows.Forms.ToolStripMenuItem tsBugCrytilis;
        private System.Windows.Forms.ToolStripMenuItem tsPatchNotes;
        private System.Windows.Forms.ToolStripMenuItem tsClearAllEnh;
        private System.Windows.Forms.ToolStripMenuItem tsConfig;
        private System.Windows.Forms.ToolStripMenuItem tsKoFi;
        private System.Windows.Forms.ToolStripMenuItem tsPatreon;
        private System.Windows.Forms.ToolStripMenuItem tsDynamic;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToDO;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToEven;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus1;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus2;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus3;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToNone;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus1;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus2;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus3;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus4;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus5;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToSO;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToTO;
        private System.Windows.Forms.ToolStripMenuItem tsExport;
        private System.Windows.Forms.ToolStripMenuItem tsExportDataLink;
        private System.Windows.Forms.ToolStripMenuItem tsGenFreebies;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator25;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem tsExportDiscord;
        private System.Windows.Forms.ToolStripMenuItem tsExportLong;
        private System.Windows.Forms.ToolStripMenuItem tsFileNew;
        private System.Windows.Forms.ToolStripMenuItem tsFileOpen;
        private System.Windows.Forms.ToolStripMenuItem tsBuildRcv;
        private System.Windows.Forms.ToolStripMenuItem tsFilePrint;
        private System.Windows.Forms.ToolStripMenuItem tsFileQuit;
        private System.Windows.Forms.ToolStripMenuItem tsFileSave;
        private System.Windows.Forms.ToolStripMenuItem tsFileSaveAs;
        private System.Windows.Forms.ToolStripMenuItem tsFlipAllEnh;
        private System.Windows.Forms.ToolStripMenuItem tsHelp;
        private System.Windows.Forms.ToolStripMenuItem tsHelperLong;
        private System.Windows.Forms.ToolStripMenuItem tsHelperLong2;
        private System.Windows.Forms.ToolStripMenuItem tsHelperShort;
        private System.Windows.Forms.ToolStripMenuItem tsHelperShort2;
        private System.Windows.Forms.ToolStripMenuItem tsImport;
        private System.Windows.Forms.ToolStripMenuItem tsIODefault;
        private System.Windows.Forms.ToolStripMenuItem tsIOMax;
        private System.Windows.Forms.ToolStripMenuItem tsIOMin;
        private System.Windows.Forms.ToolStripMenuItem tsLevelUp;
        private System.Windows.Forms.ToolStripMenuItem tsRecipeViewer;
        private System.Windows.Forms.ToolStripMenuItem tsDPSCalc;
        private System.Windows.Forms.ToolStripMenuItem tsRemoveAllSlots;
        private System.Windows.Forms.ToolStripMenuItem tsSetFind;
        private System.Windows.Forms.ToolStripMenuItem tsHCMRBForum;
        private System.Windows.Forms.ToolStripMenuItem tsGitHubCrytilis;
        private System.Windows.Forms.ToolStripMenuItem tsUpdateCheck;
        private System.Windows.Forms.ToolStripMenuItem tsView2Col;
        private System.Windows.Forms.ToolStripMenuItem tsView3Col;
        private System.Windows.Forms.ToolStripMenuItem tsView4Col;
        private System.Windows.Forms.ToolStripMenuItem tsView5Col;
        private System.Windows.Forms.ToolStripMenuItem tsView6Col;
        private System.Windows.Forms.ToolStripMenuItem tsViewActualDamage_New;
        private System.Windows.Forms.ToolStripMenuItem tsViewData;
        private System.Windows.Forms.ToolStripMenuItem tsViewDPS_New;
        private System.Windows.Forms.ToolStripMenuItem tsViewGraphs;
        private System.Windows.Forms.ToolStripMenuItem tsViewIOLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewSOLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewRelative;
        private System.Windows.Forms.ToolStripMenuItem tsViewRelativeAsSigns;
        private System.Windows.Forms.ToolStripMenuItem tsViewSetCompare;
        private System.Windows.Forms.ToolStripMenuItem tsViewSets;
        private System.Windows.Forms.ToolStripMenuItem tsViewSlotLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewTotals;
        private System.Windows.Forms.ToolStripMenuItem AccoladesWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AdvancedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem AutoArrangeAllSlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ImportExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IncarnateWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InGameRespecHelperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetAllIOsToDefault35ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TemporaryPowersWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tlsDPA;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ToggleCheckModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator12;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator14;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator15;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator16;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator18;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator19;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator20;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator21;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator22;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator23;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator24;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator26;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator28;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel mainPanel;
        private mrbControls.pnlGFX pnlGFX1;
        private System.Windows.Forms.Panel pool3Placeholder;
        private System.Windows.Forms.Panel pool2Placeholder;
        private System.Windows.Forms.Panel pool1Placeholder;
        private System.Windows.Forms.Panel pool0Placeholder;
        private System.Windows.Forms.Panel epicPlaceholder;
        private System.Windows.Forms.Panel dvPlaceholder;
        private System.Windows.Forms.Panel secondaryPlaceholder;
        private System.Windows.Forms.Panel primaryPlaceholder;
    }
}
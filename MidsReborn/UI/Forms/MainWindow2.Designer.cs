using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Forms.Controls;

namespace Mids_Reborn.UI.Forms
{
    partial class MainWindow
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
            mainLayoutPanel = new TableLayoutPanel();
            buttonsLayoutPanel = new FlowLayoutPanel();
            ibTeamEx = new MidsVectorButton();
            ibAlignmentEx = new MidsVectorButton();
            ibTempPowersEx = new MidsVectorButton();
            ibAccoladesEx = new MidsVectorButton();
            ibIncarnatePowersEx = new MidsVectorButton();
            ibPrestigePowersEx = new MidsVectorButton();
            ibPvXEx = new MidsVectorButton();
            ibRecipeEx = new MidsVectorButton();
            ibPopupEx = new MidsVectorButton();
            pnlGFXFlow = new ScrollPanelEx();
            pnlGFX = new PanelGfx();
            lblCharacter = new Label();
            leftControlPanel = new TableLayoutPanel();
            poolsPanel = new ScrollPanelEx();
            lblLockedAncillary = new Label();
            lblLockedPool3 = new Label();
            lblLockedPool2 = new Label();
            lblLockedPool1 = new Label();
            lblLockedPool0 = new Label();
            cbPool0 = new ComboBox();
            cbPool1 = new ComboBox();
            cbPool2 = new ComboBox();
            cbPool3 = new ComboBox();
            cbAncillary = new ComboBox();
            lblPool1 = new Label();
            lblPool2 = new Label();
            lblPool3 = new Label();
            lblPool4 = new Label();
            lblEpic = new Label();
            llPool0 = new ListLabel();
            llPool1 = new ListLabel();
            llPool2 = new ListLabel();
            llPool3 = new ListLabel();
            llAncillary = new ListLabel();
            leftInsidePanel = new TableLayoutPanel();
            lblLockedSecondary = new Label();
            cbSecondary = new ComboBox();
            lblSecondary = new Label();
            cbPrimary = new ComboBox();
            lblPrimary = new Label();
            llPrimary = new ListLabel();
            llSecondary = new ListLabel();
            topPanel = new Panel();
            lblATLocked = new Label();
            ibTotalsEx = new MidsVectorButton();
            ibSlotLevelsEx = new MidsVectorButton();
            ibModeEx = new MidsVectorButton();
            ibSetsEx = new MidsVectorButton();
            ibDynMode = new MidsVectorButton();
            lblName = new Label();
            lblOrigin = new Label();
            lblAT = new Label();
            txtName = new TextBox();
            cbOrigin = new ComboBox();
            cbAT = new ComboBox();
            ibSlotInfoEx = new MidsVectorButton();
            MenuBar = new MidsMenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            tsFileNew = new ToolStripMenuItem();
            ToolStripSeparator7 = new ToolStripSeparator();
            tsFileOpen = new ToolStripMenuItem();
            tsFileSave = new ToolStripMenuItem();
            tsFileSaveAs = new ToolStripMenuItem();
            ToolStripSeparator22 = new ToolStripSeparator();
            ExportToolStripMenuItem = new ToolStripMenuItem();
            tsGenFreebies = new ToolStripMenuItem();
            ToolStripSeparator8 = new ToolStripSeparator();
            tsFilePrint = new ToolStripMenuItem();
            ToolStripSeparator9 = new ToolStripSeparator();
            tsFileQuit = new ToolStripMenuItem();
            OptionsToolStripMenuItem = new ToolStripMenuItem();
            tsChangeDb = new ToolStripMenuItem();
            ToolStripSeparator29 = new ToolStripSeparator();
            tsConfig = new ToolStripMenuItem();
            ToolStripSeparator5 = new ToolStripSeparator();
            AdvancedToolStripMenuItem1 = new ToolStripMenuItem();
            tsAdvDBEdit = new ToolStripMenuItem();
            ToolStripSeparator15 = new ToolStripSeparator();
            tsAdvFreshInstall = new ToolStripMenuItem();
            tsAdvResetTips = new ToolStripMenuItem();
            ShareToolStripMenuItem = new ToolStripMenuItem();
            tsShareMenu = new ToolStripMenuItem();
            LegacyToolStripMenuItem = new ToolStripMenuItem();
            tsShareLegacy = new ToolStripMenuItem();
            ToolStripSeparator26 = new ToolStripSeparator();
            tsImport = new ToolStripMenuItem();
            ToolStripSeparator24 = new ToolStripSeparator();
            tsImportDataChunk = new ToolStripMenuItem();
            ToolStripSeparator27 = new ToolStripSeparator();
            tsViewSharedBuilds = new ToolStripMenuItem();
            CharacterToolStripMenuItem = new ToolStripMenuItem();
            SetAllIOsToDefault35ToolStripMenuItem = new ToolStripMenuItem();
            tsIODefault = new ToolStripMenuItem();
            ToolStripSeparator11 = new ToolStripSeparator();
            tsIOMin = new ToolStripMenuItem();
            tsIOMax = new ToolStripMenuItem();
            ToolStripSeparator16 = new ToolStripSeparator();
            ToolStripMenuItem1 = new ToolStripMenuItem();
            tsEnhToSO = new ToolStripMenuItem();
            tsEnhToDO = new ToolStripMenuItem();
            tsEnhToTO = new ToolStripMenuItem();
            ToolStripMenuItem2 = new ToolStripMenuItem();
            tsEnhToPlus5 = new ToolStripMenuItem();
            tsEnhToPlus4 = new ToolStripMenuItem();
            tsEnhToPlus3 = new ToolStripMenuItem();
            tsEnhToPlus2 = new ToolStripMenuItem();
            tsEnhToPlus1 = new ToolStripMenuItem();
            tsEnhToEven = new ToolStripMenuItem();
            tsEnhToMinus1 = new ToolStripMenuItem();
            tsEnhToMinus2 = new ToolStripMenuItem();
            tsEnhToMinus3 = new ToolStripMenuItem();
            tsEnhToNone = new ToolStripMenuItem();
            ToolStripSeparator17 = new ToolStripSeparator();
            SlotsToolStripMenuItem = new ToolStripMenuItem();
            tsFlipAllEnh = new ToolStripMenuItem();
            ToolStripSeparator4 = new ToolStripSeparator();
            tsClearAllEnh = new ToolStripMenuItem();
            tsRemoveAllSlots = new ToolStripMenuItem();
            ToolStripSeparator1 = new ToolStripSeparator();
            AutoArrangeAllSlotsToolStripMenuItem = new ToolStripMenuItem();
            ToolStripSeparator28 = new ToolStripSeparator();
            ToggleCheckModeToolStripMenuItem = new ToolStripMenuItem();
            ViewToolStripMenuItem = new ToolStripMenuItem();
            tsView2Col = new ToolStripMenuItem();
            tsView3Col = new ToolStripMenuItem();
            tsView4Col = new ToolStripMenuItem();
            tsView5Col = new ToolStripMenuItem();
            tsView6Col = new ToolStripMenuItem();
            tsView3ColV = new ToolStripMenuItem();
            tsView3ColH = new ToolStripMenuItem();
            ToolStripSeparator13 = new ToolStripSeparator();
            tsViewIOLevels = new ToolStripMenuItem();
            tsViewSOLevels = new ToolStripMenuItem();
            tsViewRelative = new ToolStripMenuItem();
            tsViewSlotLevels = new ToolStripMenuItem();
            tsViewRelativeAsSigns = new ToolStripMenuItem();
            ToolStripSeparator2 = new ToolStripSeparator();
            tsViewActualDamage_New = new ToolStripMenuItem();
            tsViewDPS_New = new ToolStripMenuItem();
            tlsDPA = new ToolStripMenuItem();
            WindowToolStripMenuItem = new ToolStripMenuItem();
            tsViewSets = new ToolStripMenuItem();
            tsViewGraphs = new ToolStripMenuItem();
            tsViewSetCompare = new ToolStripMenuItem();
            tsViewData = new ToolStripMenuItem();
            tsViewTotals = new ToolStripMenuItem();
            ToolStripSeparator18 = new ToolStripSeparator();
            tsRecipeViewer = new ToolStripMenuItem();
            tsRotationHelper = new ToolStripMenuItem();
            ToolStripSeparator19 = new ToolStripSeparator();
            tsSetFind = new ToolStripMenuItem();
            ToolStripSeparator21 = new ToolStripSeparator();
            InGameRespecHelperToolStripMenuItem = new ToolStripMenuItem();
            tsHelperShort = new ToolStripMenuItem();
            tsHelperLong = new ToolStripMenuItem();
            HelpToolStripMenuItem = new ToolStripMenuItem();
            tsHelp = new ToolStripMenuItem();
            tsUpdateCheck = new ToolStripMenuItem();
            ToolStripSeparator10 = new ToolStripSeparator();
            tsBuildRcv = new ToolStripMenuItem();
            ToolStripSeparator30 = new ToolStripSeparator();
            tsGitHub = new ToolStripMenuItem();
            tsSupport = new ToolStripMenuItem();
            ToolStripSeparator31 = new ToolStripSeparator();
            tsAbout = new ToolStripMenuItem();
            EnemyRelativeToolStripComboBox = new ToolStripComboBox();
            EditBuildCommentMenuItem = new FontAwesome.Sharp.IconMenuItem();
            DonateToolStripMenuItem = new ToolStripMenuItem();
            tsKoFi = new ToolStripMenuItem();
            tsPatreon = new ToolStripMenuItem();
            DlgOpen = new OpenFileDialog();
            DlgSave = new SaveFileDialog();
            tTip = new ToolTip(components);
            tmrGfx = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            mainLayoutPanel.SuspendLayout();
            buttonsLayoutPanel.SuspendLayout();
            pnlGFXFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlGFX).BeginInit();
            leftControlPanel.SuspendLayout();
            poolsPanel.SuspendLayout();
            leftInsidePanel.SuspendLayout();
            topPanel.SuspendLayout();
            MenuBar.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 526F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(buttonsLayoutPanel, 1, 0);
            mainLayoutPanel.Controls.Add(pnlGFXFlow, 1, 1);
            mainLayoutPanel.Controls.Add(lblCharacter, 0, 0);
            mainLayoutPanel.Controls.Add(leftControlPanel, 0, 1);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 24);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 2;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 9.021407F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90.97859F));
            mainLayoutPanel.Size = new Size(1264, 843);
            mainLayoutPanel.TabIndex = 1;
            // 
            // buttonsLayoutPanel
            // 
            buttonsLayoutPanel.Controls.Add(ibTeamEx);
            buttonsLayoutPanel.Controls.Add(ibAlignmentEx);
            buttonsLayoutPanel.Controls.Add(ibTempPowersEx);
            buttonsLayoutPanel.Controls.Add(ibAccoladesEx);
            buttonsLayoutPanel.Controls.Add(ibIncarnatePowersEx);
            buttonsLayoutPanel.Controls.Add(ibPrestigePowersEx);
            buttonsLayoutPanel.Controls.Add(ibPvXEx);
            buttonsLayoutPanel.Controls.Add(ibRecipeEx);
            buttonsLayoutPanel.Controls.Add(ibPopupEx);
            buttonsLayoutPanel.Dock = DockStyle.Top;
            buttonsLayoutPanel.Location = new Point(529, 3);
            buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            buttonsLayoutPanel.Size = new Size(732, 50);
            buttonsLayoutPanel.TabIndex = 45;
            // 
            // ibTeamEx
            // 
            ibTeamEx.BackColor = Color.Transparent;
            ibTeamEx.CornerRadius = 6;
            ibTeamEx.Location = new Point(3, 3);
            ibTeamEx.Name = "ibTeamEx";
            ibTeamEx.Size = new Size(105, 22);
            ibTeamEx.TabIndex = 117;
            ibTeamEx.Text = "Combat Settings";
            ibTeamEx.ToggleText.Indeterminate = "Indeterminate State";
            ibTeamEx.ToggleText.ToggledOff = "Toggle Off State";
            ibTeamEx.ToggleText.ToggledOn = "Toggle On State";
            ibTeamEx.Click += ibTeamEx_OnClick;
            // 
            // ibAlignmentEx
            // 
            ibAlignmentEx.BackColor = Color.Transparent;
            ibAlignmentEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibAlignmentEx.Location = new Point(114, 3);
            ibAlignmentEx.Name = "ibAlignmentEx";
            ibAlignmentEx.Size = new Size(105, 22);
            ibAlignmentEx.TabIndex = 118;
            ibAlignmentEx.Text = "ibAlignmentEx";
            ibAlignmentEx.ToggleText.Indeterminate = "Indeterminate State";
            ibAlignmentEx.ToggleText.ToggledOff = "Hero";
            ibAlignmentEx.ToggleText.ToggledOn = "Villain";
            ibAlignmentEx.Click += ibAlignmentEx_OnClick;
            // 
            // ibTempPowersEx
            // 
            ibTempPowersEx.BackColor = Color.Transparent;
            ibTempPowersEx.Location = new Point(225, 3);
            ibTempPowersEx.Name = "ibTempPowersEx";
            ibTempPowersEx.Size = new Size(105, 22);
            ibTempPowersEx.TabIndex = 119;
            ibTempPowersEx.Text = "Temp Powers";
            ibTempPowersEx.ToggleText.Indeterminate = "Indeterminate State";
            ibTempPowersEx.ToggleText.ToggledOff = "Temp Powers: Off";
            ibTempPowersEx.ToggleText.ToggledOn = "Temp Powers: On";
            ibTempPowersEx.Click += ibTempPowersEx_OnClick;
            // 
            // ibAccoladesEx
            // 
            ibAccoladesEx.BackColor = Color.Transparent;
            ibAccoladesEx.Location = new Point(336, 3);
            ibAccoladesEx.Name = "ibAccoladesEx";
            ibAccoladesEx.Size = new Size(105, 22);
            ibAccoladesEx.TabIndex = 120;
            ibAccoladesEx.Text = "Accolades";
            ibAccoladesEx.ToggleText.Indeterminate = "Indeterminate State";
            ibAccoladesEx.ToggleText.ToggledOff = "Accolades: Off";
            ibAccoladesEx.ToggleText.ToggledOn = "Accolades: On";
            ibAccoladesEx.Click += ibAccoladesEx_OnClick;
            // 
            // ibIncarnatePowersEx
            // 
            ibIncarnatePowersEx.BackColor = Color.Transparent;
            ibIncarnatePowersEx.Location = new Point(447, 3);
            ibIncarnatePowersEx.Name = "ibIncarnatePowersEx";
            ibIncarnatePowersEx.Size = new Size(105, 22);
            ibIncarnatePowersEx.TabIndex = 121;
            ibIncarnatePowersEx.Text = "Incarnate Powers";
            ibIncarnatePowersEx.ToggleText.Indeterminate = "Indeterminate State";
            ibIncarnatePowersEx.ToggleText.ToggledOff = "Toggled Off State";
            ibIncarnatePowersEx.ToggleText.ToggledOn = "Toggled On State";
            ibIncarnatePowersEx.Click += ibIncarnatesEx_OnClick;
            // 
            // ibPrestigePowersEx
            // 
            ibPrestigePowersEx.BackColor = Color.Transparent;
            ibPrestigePowersEx.Location = new Point(558, 3);
            ibPrestigePowersEx.Name = "ibPrestigePowersEx";
            ibPrestigePowersEx.Size = new Size(105, 22);
            ibPrestigePowersEx.TabIndex = 122;
            ibPrestigePowersEx.Text = "Prestige Powers";
            ibPrestigePowersEx.ToggleText.Indeterminate = "Indeterminate State";
            ibPrestigePowersEx.ToggleText.ToggledOff = "ToggledOff State";
            ibPrestigePowersEx.ToggleText.ToggledOn = "ToggledOn State";
            ibPrestigePowersEx.Click += ibPrestigePowersEx_OnClick;
            // 
            // ibPvXEx
            // 
            ibPvXEx.BackColor = Color.Transparent;
            ibPvXEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibPvXEx.Location = new Point(3, 31);
            ibPvXEx.Name = "ibPvXEx";
            ibPvXEx.Size = new Size(105, 22);
            ibPvXEx.TabIndex = 123;
            ibPvXEx.Text = "ibPvXEx";
            ibPvXEx.ToggleText.Indeterminate = "Indeterminate State";
            ibPvXEx.ToggleText.ToggledOff = "PvE";
            ibPvXEx.ToggleText.ToggledOn = "PvP";
            ibPvXEx.Click += ibPvXEx_OnClick;
            // 
            // ibRecipeEx
            // 
            ibRecipeEx.BackColor = Color.Transparent;
            ibRecipeEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibRecipeEx.Location = new Point(114, 31);
            ibRecipeEx.Name = "ibRecipeEx";
            ibRecipeEx.Size = new Size(105, 22);
            ibRecipeEx.TabIndex = 124;
            ibRecipeEx.Text = "ibRecipeEx";
            ibRecipeEx.ToggleText.Indeterminate = "Indeterminate State";
            ibRecipeEx.ToggleText.ToggledOff = "Recipes: Off";
            ibRecipeEx.ToggleText.ToggledOn = "Recipes: On";
            ibRecipeEx.Click += ibRecipeEx_OnClick;
            // 
            // ibPopupEx
            // 
            ibPopupEx.BackColor = Color.Transparent;
            ibPopupEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibPopupEx.Location = new Point(225, 31);
            ibPopupEx.Name = "ibPopupEx";
            ibPopupEx.Size = new Size(105, 22);
            ibPopupEx.TabIndex = 125;
            ibPopupEx.Text = "ibPopupEx";
            ibPopupEx.ToggleText.Indeterminate = "Indeterminate State";
            ibPopupEx.ToggleText.ToggledOff = "Popup: Off";
            ibPopupEx.ToggleText.ToggledOn = "Popup: On";
            ibPopupEx.Click += ibPopupEx_OnClick;
            // 
            // pnlGFXFlow
            // 
            // 
            // 
            // 
            pnlGFXFlow.ContentPanel.Location = new Point(0, 0);
            pnlGFXFlow.ContentPanel.Name = "";
            pnlGFXFlow.ContentPanel.Size = new Size(729, 10);
            pnlGFXFlow.ContentPanel.TabIndex = 0;
            pnlGFXFlow.Controls.Add(pnlGFX);
            pnlGFXFlow.Dock = DockStyle.Fill;
            pnlGFXFlow.Location = new Point(532, 79);
            pnlGFXFlow.Margin = new Padding(6, 3, 3, 3);
            pnlGFXFlow.Name = "pnlGFXFlow";
            pnlGFXFlow.Size = new Size(729, 761);
            pnlGFXFlow.TabIndex = 113;
            pnlGFXFlow.Scroll += pnlGFXFlow_Scroll;
            pnlGFXFlow.MouseEnter += pnlGFXFlow_MouseEnter;
            pnlGFXFlow.Resize += pnlGFXFlow_Resize;
            // 
            // pnlGFX
            // 
            pnlGFX.BackColor = Color.Black;
            pnlGFX.Location = new Point(6, 3);
            pnlGFX.Margin = new Padding(6, 3, 3, 3);
            pnlGFX.Name = "pnlGFX";
            pnlGFX.Size = new Size(758, 885);
            pnlGFX.TabIndex = 103;
            pnlGFX.TabStop = false;
            pnlGFX.DragDrop += pnlGFX_DragDrop;
            pnlGFX.DragEnter += pnlGFX_DragEnter;
            pnlGFX.DragOver += pnlGFX_DragOver;
            pnlGFX.Paint += pnlGFX_Paint;
            pnlGFX.MouseDoubleClick += pnlGFX_MouseDoubleClick;
            pnlGFX.MouseDown += pnlGFX_MouseDown;
            pnlGFX.MouseLeave += pnlGFX_MouseLeave;
            pnlGFX.MouseMove += pnlGFX_MouseMove;
            pnlGFX.MouseUp += pnlGFX_MouseUp;
            pnlGFX.Resize += pnlGFX_Resize;
            // 
            // lblCharacter
            // 
            lblCharacter.AutoSize = true;
            lblCharacter.BackColor = Color.Transparent;
            lblCharacter.Dock = DockStyle.Fill;
            lblCharacter.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblCharacter.ForeColor = Color.White;
            lblCharacter.Location = new Point(3, 0);
            lblCharacter.Name = "lblCharacter";
            lblCharacter.Size = new Size(520, 76);
            lblCharacter.TabIndex = 44;
            lblCharacter.Text = "Name: Level 0 Origin Archetype (Primary / Secondary)";
            lblCharacter.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // leftControlPanel
            // 
            leftControlPanel.ColumnCount = 2;
            leftControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.19231F));
            leftControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.8076935F));
            leftControlPanel.Controls.Add(poolsPanel, 1, 1);
            leftControlPanel.Controls.Add(leftInsidePanel, 0, 1);
            leftControlPanel.Controls.Add(topPanel, 0, 0);
            leftControlPanel.Dock = DockStyle.Fill;
            leftControlPanel.Location = new Point(3, 79);
            leftControlPanel.Name = "leftControlPanel";
            leftControlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15.9340658F));
            leftControlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 84.06593F));
            leftControlPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            leftControlPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            leftControlPanel.Size = new Size(520, 761);
            leftControlPanel.TabIndex = 46;
            // 
            // poolsPanel
            // 
            // 
            // 
            // 
            poolsPanel.ContentPanel.Location = new Point(0, 0);
            poolsPanel.ContentPanel.Name = "";
            poolsPanel.ContentPanel.Size = new Size(172, 10);
            poolsPanel.ContentPanel.TabIndex = 0;
            poolsPanel.Controls.Add(lblLockedAncillary);
            poolsPanel.Controls.Add(lblLockedPool3);
            poolsPanel.Controls.Add(lblLockedPool2);
            poolsPanel.Controls.Add(lblLockedPool1);
            poolsPanel.Controls.Add(lblLockedPool0);
            poolsPanel.Controls.Add(cbPool0);
            poolsPanel.Controls.Add(cbPool1);
            poolsPanel.Controls.Add(cbPool2);
            poolsPanel.Controls.Add(cbPool3);
            poolsPanel.Controls.Add(cbAncillary);
            poolsPanel.Controls.Add(lblPool1);
            poolsPanel.Controls.Add(lblPool2);
            poolsPanel.Controls.Add(lblPool3);
            poolsPanel.Controls.Add(lblPool4);
            poolsPanel.Controls.Add(lblEpic);
            poolsPanel.Controls.Add(llPool0);
            poolsPanel.Controls.Add(llPool1);
            poolsPanel.Controls.Add(llPool2);
            poolsPanel.Controls.Add(llPool3);
            poolsPanel.Controls.Add(llAncillary);
            poolsPanel.Dock = DockStyle.Left;
            poolsPanel.Location = new Point(342, 121);
            poolsPanel.Name = "poolsPanel";
            leftControlPanel.SetRowSpan(poolsPanel, 2);
            poolsPanel.Size = new Size(172, 637);
            poolsPanel.TabIndex = 141;
            // 
            // lblLockedAncillary
            // 
            lblLockedAncillary.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedAncillary.BorderStyle = BorderStyle.Fixed3D;
            lblLockedAncillary.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedAncillary.ForeColor = Color.Black;
            lblLockedAncillary.Location = new Point(3, 509);
            lblLockedAncillary.Name = "lblLockedAncillary";
            lblLockedAncillary.Size = new Size(108, 22);
            lblLockedAncillary.TabIndex = 146;
            lblLockedAncillary.Text = "Pool Locked";
            lblLockedAncillary.TextAlign = ContentAlignment.MiddleCenter;
            lblLockedAncillary.Visible = false;
            lblLockedAncillary.Paint += lblLockedAncillary_Paint;
            lblLockedAncillary.MouseLeave += lblLocked_MouseLeave;
            lblLockedAncillary.MouseMove += lblLockedAncillary_MouseMove;
            // 
            // lblLockedPool3
            // 
            lblLockedPool3.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedPool3.BorderStyle = BorderStyle.Fixed3D;
            lblLockedPool3.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedPool3.ForeColor = Color.Black;
            lblLockedPool3.Location = new Point(3, 388);
            lblLockedPool3.Name = "lblLockedPool3";
            lblLockedPool3.Size = new Size(108, 22);
            lblLockedPool3.TabIndex = 145;
            lblLockedPool3.Text = "Pool Locked";
            lblLockedPool3.TextAlign = ContentAlignment.MiddleCenter;
            lblLockedPool3.Visible = false;
            lblLockedPool3.Paint += lblLockedPool3_Paint;
            lblLockedPool3.MouseLeave += lblLocked_MouseLeave;
            lblLockedPool3.MouseMove += lblLockedPool3_MouseMove;
            // 
            // lblLockedPool2
            // 
            lblLockedPool2.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedPool2.BorderStyle = BorderStyle.Fixed3D;
            lblLockedPool2.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedPool2.ForeColor = Color.Black;
            lblLockedPool2.Location = new Point(3, 267);
            lblLockedPool2.Name = "lblLockedPool2";
            lblLockedPool2.Size = new Size(108, 22);
            lblLockedPool2.TabIndex = 144;
            lblLockedPool2.Text = "Pool Locked";
            lblLockedPool2.TextAlign = ContentAlignment.MiddleCenter;
            lblLockedPool2.Visible = false;
            lblLockedPool2.Paint += lblLockedPool2_Paint;
            lblLockedPool2.MouseLeave += lblLocked_MouseLeave;
            lblLockedPool2.MouseMove += lblLockedPool2_MouseMove;
            // 
            // lblLockedPool1
            // 
            lblLockedPool1.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedPool1.BorderStyle = BorderStyle.Fixed3D;
            lblLockedPool1.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedPool1.ForeColor = Color.Black;
            lblLockedPool1.Location = new Point(3, 146);
            lblLockedPool1.Name = "lblLockedPool1";
            lblLockedPool1.Size = new Size(108, 22);
            lblLockedPool1.TabIndex = 143;
            lblLockedPool1.Text = "Pool Locked";
            lblLockedPool1.TextAlign = ContentAlignment.MiddleCenter;
            lblLockedPool1.Visible = false;
            lblLockedPool1.Paint += lblLockedPool1_Paint;
            lblLockedPool1.MouseLeave += lblLocked_MouseLeave;
            lblLockedPool1.MouseMove += lblLockedPool1_MouseMove;
            // 
            // lblLockedPool0
            // 
            lblLockedPool0.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedPool0.BorderStyle = BorderStyle.Fixed3D;
            lblLockedPool0.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedPool0.ForeColor = Color.Black;
            lblLockedPool0.Location = new Point(3, 25);
            lblLockedPool0.Name = "lblLockedPool0";
            lblLockedPool0.Size = new Size(108, 22);
            lblLockedPool0.TabIndex = 142;
            lblLockedPool0.Text = "Pool Locked";
            lblLockedPool0.TextAlign = ContentAlignment.MiddleCenter;
            lblLockedPool0.Visible = false;
            lblLockedPool0.Paint += lblLockedPool0_Paint;
            lblLockedPool0.MouseLeave += lblLocked_MouseLeave;
            lblLockedPool0.MouseMove += lblLockedPool0_MouseMove;
            // 
            // cbPool0
            // 
            cbPool0.BackColor = Color.WhiteSmoke;
            cbPool0.DrawMode = DrawMode.OwnerDrawFixed;
            cbPool0.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPool0.ForeColor = Color.Black;
            cbPool0.ItemHeight = 16;
            cbPool0.Location = new Point(3, 25);
            cbPool0.MaxDropDownItems = 15;
            cbPool0.Name = "cbPool0";
            cbPool0.Size = new Size(145, 22);
            cbPool0.TabIndex = 15;
            cbPool0.DrawItem += cbPool0_DrawItem;
            cbPool0.SelectedIndexChanged += cbPool0_SelectedIndexChanged;
            cbPool0.MouseLeave += cbPools_MouseLeave;
            cbPool0.MouseMove += cbPools_MouseMove;
            // 
            // cbPool1
            // 
            cbPool1.BackColor = Color.WhiteSmoke;
            cbPool1.DrawMode = DrawMode.OwnerDrawFixed;
            cbPool1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPool1.ForeColor = Color.Black;
            cbPool1.ItemHeight = 16;
            cbPool1.Location = new Point(3, 146);
            cbPool1.MaxDropDownItems = 15;
            cbPool1.Name = "cbPool1";
            cbPool1.Size = new Size(145, 22);
            cbPool1.TabIndex = 18;
            cbPool1.DrawItem += cbPool1_DrawItem;
            cbPool1.SelectedIndexChanged += cbPool1_SelectedIndexChanged;
            cbPool1.MouseLeave += cbPools_MouseLeave;
            cbPool1.MouseMove += cbPools_MouseMove;
            // 
            // cbPool2
            // 
            cbPool2.BackColor = Color.WhiteSmoke;
            cbPool2.DrawMode = DrawMode.OwnerDrawFixed;
            cbPool2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPool2.ForeColor = Color.Black;
            cbPool2.ItemHeight = 16;
            cbPool2.Location = new Point(3, 267);
            cbPool2.MaxDropDownItems = 15;
            cbPool2.Name = "cbPool2";
            cbPool2.Size = new Size(145, 22);
            cbPool2.TabIndex = 21;
            cbPool2.DrawItem += cbPool2_DrawItem;
            cbPool2.SelectedIndexChanged += cbPool2_SelectedIndexChanged;
            cbPool2.MouseLeave += cbPools_MouseLeave;
            cbPool2.MouseMove += cbPools_MouseMove;
            // 
            // cbPool3
            // 
            cbPool3.BackColor = Color.WhiteSmoke;
            cbPool3.DrawMode = DrawMode.OwnerDrawFixed;
            cbPool3.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPool3.ForeColor = Color.Black;
            cbPool3.ItemHeight = 16;
            cbPool3.Location = new Point(3, 388);
            cbPool3.MaxDropDownItems = 15;
            cbPool3.Name = "cbPool3";
            cbPool3.Size = new Size(145, 22);
            cbPool3.TabIndex = 24;
            cbPool3.DrawItem += cbPool3_DrawItem;
            cbPool3.SelectedIndexChanged += cbPool3_SelectedIndexChanged;
            cbPool3.MouseLeave += cbPools_MouseLeave;
            cbPool3.MouseMove += cbPools_MouseMove;
            // 
            // cbAncillary
            // 
            cbAncillary.BackColor = Color.WhiteSmoke;
            cbAncillary.DrawMode = DrawMode.OwnerDrawFixed;
            cbAncillary.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAncillary.ForeColor = Color.Black;
            cbAncillary.ItemHeight = 16;
            cbAncillary.Location = new Point(3, 509);
            cbAncillary.Name = "cbAncillary";
            cbAncillary.Size = new Size(145, 22);
            cbAncillary.TabIndex = 27;
            cbAncillary.DrawItem += cbAncillary_DrawItem;
            cbAncillary.SelectedIndexChanged += cbAncillery_SelectedIndexChanged;
            cbAncillary.MouseLeave += cbPools_MouseLeave;
            cbAncillary.MouseMove += cbPools_MouseMove;
            // 
            // lblPool1
            // 
            lblPool1.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblPool1.ForeColor = Color.White;
            lblPool1.Location = new Point(6, 5);
            lblPool1.Name = "lblPool1";
            lblPool1.Size = new Size(142, 17);
            lblPool1.TabIndex = 14;
            lblPool1.Text = "Pool 1";
            lblPool1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPool2
            // 
            lblPool2.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblPool2.ForeColor = Color.White;
            lblPool2.Location = new Point(3, 126);
            lblPool2.Name = "lblPool2";
            lblPool2.Size = new Size(145, 17);
            lblPool2.TabIndex = 17;
            lblPool2.Text = "Pool 2";
            lblPool2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPool3
            // 
            lblPool3.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblPool3.ForeColor = Color.White;
            lblPool3.Location = new Point(3, 247);
            lblPool3.Name = "lblPool3";
            lblPool3.Size = new Size(145, 17);
            lblPool3.TabIndex = 20;
            lblPool3.Text = "Pool 3";
            lblPool3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPool4
            // 
            lblPool4.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblPool4.ForeColor = Color.White;
            lblPool4.Location = new Point(3, 368);
            lblPool4.Name = "lblPool4";
            lblPool4.Size = new Size(145, 17);
            lblPool4.TabIndex = 23;
            lblPool4.Text = "Pool 4";
            lblPool4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblEpic
            // 
            lblEpic.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblEpic.ForeColor = Color.White;
            lblEpic.Location = new Point(3, 489);
            lblEpic.Name = "lblEpic";
            lblEpic.Size = new Size(145, 17);
            lblEpic.TabIndex = 26;
            lblEpic.Text = "Epic Pool";
            lblEpic.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // llPool0
            // 
            llPool0.ActualLineHeight = 18;
            llPool0.Expandable = true;
            llPool0.HighVis = true;
            llPool0.HoverColor = Color.WhiteSmoke;
            llPool0.IsExpanded = false;
            llPool0.Location = new Point(3, 53);
            llPool0.MaxHeight = 500;
            llPool0.Name = "llPool0";
            llPool0.PaddingX = 4;
            llPool0.PaddingY = 1;
            llPool0.Scrollable = true;
            llPool0.ScrollBarColor = Color.FromArgb(128, 96, 192);
            llPool0.ScrollBarWidth = 11;
            llPool0.ScrollButtonColor = Color.FromArgb(96, 0, 192);
            llPool0.Size = new Size(145, 70);
            llPool0.SizeNormal = new Size(145, 70);
            llPool0.SuspendRedraw = false;
            llPool0.TabIndex = 34;
            llPool0.ItemClick += llPool0_ItemClick;
            llPool0.ItemHover += llPool0_ItemHover;
            llPool0.EmptyHover += llAll_EmptyHover;
            // 
            // llPool1
            // 
            llPool1.ActualLineHeight = 18;
            llPool1.Expandable = true;
            llPool1.ForeColor = Color.Yellow;
            llPool1.HighVis = true;
            llPool1.HoverColor = Color.WhiteSmoke;
            llPool1.IsExpanded = false;
            llPool1.Location = new Point(3, 173);
            llPool1.MaxHeight = 500;
            llPool1.Name = "llPool1";
            llPool1.PaddingX = 4;
            llPool1.PaddingY = 1;
            llPool1.Scrollable = true;
            llPool1.ScrollBarColor = Color.FromArgb(128, 96, 192);
            llPool1.ScrollBarWidth = 11;
            llPool1.ScrollButtonColor = Color.FromArgb(96, 0, 192);
            llPool1.Size = new Size(145, 70);
            llPool1.SizeNormal = new Size(145, 70);
            llPool1.SuspendRedraw = false;
            llPool1.TabIndex = 35;
            llPool1.ItemClick += llPool1_ItemClick;
            llPool1.ItemHover += llPool1_ItemHover;
            llPool1.EmptyHover += llAll_EmptyHover;
            // 
            // llPool2
            // 
            llPool2.ActualLineHeight = 18;
            llPool2.Expandable = true;
            llPool2.ForeColor = Color.Yellow;
            llPool2.HighVis = true;
            llPool2.HoverColor = Color.WhiteSmoke;
            llPool2.IsExpanded = false;
            llPool2.Location = new Point(3, 295);
            llPool2.MaxHeight = 500;
            llPool2.Name = "llPool2";
            llPool2.PaddingX = 4;
            llPool2.PaddingY = 1;
            llPool2.Scrollable = true;
            llPool2.ScrollBarColor = Color.FromArgb(128, 96, 192);
            llPool2.ScrollBarWidth = 11;
            llPool2.ScrollButtonColor = Color.FromArgb(96, 0, 192);
            llPool2.Size = new Size(145, 70);
            llPool2.SizeNormal = new Size(145, 70);
            llPool2.SuspendRedraw = false;
            llPool2.TabIndex = 36;
            llPool2.ItemClick += llPool2_ItemClick;
            llPool2.ItemHover += llPool2_ItemHover;
            llPool2.EmptyHover += llAll_EmptyHover;
            // 
            // llPool3
            // 
            llPool3.ActualLineHeight = 18;
            llPool3.Expandable = true;
            llPool3.ForeColor = Color.Yellow;
            llPool3.HighVis = true;
            llPool3.HoverColor = Color.WhiteSmoke;
            llPool3.IsExpanded = false;
            llPool3.Location = new Point(3, 416);
            llPool3.MaxHeight = 500;
            llPool3.Name = "llPool3";
            llPool3.PaddingX = 4;
            llPool3.PaddingY = 1;
            llPool3.Scrollable = true;
            llPool3.ScrollBarColor = Color.FromArgb(128, 96, 192);
            llPool3.ScrollBarWidth = 11;
            llPool3.ScrollButtonColor = Color.FromArgb(96, 0, 192);
            llPool3.Size = new Size(145, 70);
            llPool3.SizeNormal = new Size(145, 70);
            llPool3.SuspendRedraw = false;
            llPool3.TabIndex = 37;
            llPool3.ItemClick += llPool3_ItemClick;
            llPool3.ItemHover += llPool3_ItemHover;
            llPool3.EmptyHover += llAll_EmptyHover;
            // 
            // llAncillary
            // 
            llAncillary.ActualLineHeight = 18;
            llAncillary.Expandable = false;
            llAncillary.HighVis = true;
            llAncillary.HoverColor = Color.WhiteSmoke;
            llAncillary.IsExpanded = true;
            llAncillary.Location = new Point(3, 537);
            llAncillary.MaxHeight = 500;
            llAncillary.Name = "llAncillary";
            llAncillary.PaddingX = 4;
            llAncillary.PaddingY = 1;
            llAncillary.Scrollable = true;
            llAncillary.ScrollBarColor = Color.Red;
            llAncillary.ScrollBarWidth = 11;
            llAncillary.ScrollButtonColor = Color.FromArgb(192, 0, 0);
            llAncillary.Size = new Size(145, 14);
            llAncillary.SizeNormal = new Size(145, 70);
            llAncillary.SuspendRedraw = false;
            llAncillary.TabIndex = 110;
            llAncillary.ItemClick += llAncillary_ItemClick;
            llAncillary.ItemHover += llAncillary_ItemHover;
            llAncillary.EmptyHover += llAll_EmptyHover;
            // 
            // leftInsidePanel
            // 
            leftInsidePanel.ColumnCount = 2;
            leftInsidePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftInsidePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftInsidePanel.Controls.Add(lblLockedSecondary, 0, 3);
            leftInsidePanel.Controls.Add(cbSecondary, 1, 1);
            leftInsidePanel.Controls.Add(lblSecondary, 1, 0);
            leftInsidePanel.Controls.Add(cbPrimary, 0, 1);
            leftInsidePanel.Controls.Add(lblPrimary, 0, 0);
            leftInsidePanel.Controls.Add(llPrimary, 0, 2);
            leftInsidePanel.Controls.Add(llSecondary, 1, 2);
            leftInsidePanel.Controls.Add(panel1, 0, 4);
            leftInsidePanel.Dock = DockStyle.Left;
            leftInsidePanel.Location = new Point(3, 121);
            leftInsidePanel.Name = "leftInsidePanel";
            leftInsidePanel.RowCount = 1;
            leftControlPanel.SetRowSpan(leftInsidePanel, 2);
            leftInsidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 3.97983551F));
            leftInsidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 7.075263F));
            leftInsidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 77.73585F));
            leftInsidePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 11.6981134F));
            leftInsidePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 370F));
            leftInsidePanel.Size = new Size(331, 637);
            leftInsidePanel.TabIndex = 145;
            // 
            // lblLockedSecondary
            // 
            lblLockedSecondary.BackColor = Color.FromArgb(224, 224, 224);
            lblLockedSecondary.BorderStyle = BorderStyle.Fixed3D;
            lblLockedSecondary.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLockedSecondary.ForeColor = Color.Black;
            lblLockedSecondary.Location = new Point(3, 234);
            lblLockedSecondary.Name = "lblLockedSecondary";
            lblLockedSecondary.Size = new Size(92, 29);
            lblLockedSecondary.TabIndex = 150;
            lblLockedSecondary.Text = "Sec. Locked";
            lblLockedSecondary.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cbSecondary
            // 
            cbSecondary.BackColor = Color.WhiteSmoke;
            cbSecondary.Dock = DockStyle.Fill;
            cbSecondary.DrawMode = DrawMode.OwnerDrawFixed;
            cbSecondary.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSecondary.ForeColor = Color.Black;
            cbSecondary.ItemHeight = 16;
            cbSecondary.Location = new Point(168, 13);
            cbSecondary.MaxDropDownItems = 15;
            cbSecondary.Name = "cbSecondary";
            cbSecondary.Size = new Size(160, 22);
            cbSecondary.TabIndex = 147;
            cbSecondary.DrawItem += cbSecondary_DrawItem;
            cbSecondary.SelectedIndexChanged += cbSecondary_SelectedIndexChanged;
            cbSecondary.MouseLeave += cbSecondary_MouseLeave;
            cbSecondary.MouseMove += cbSecondary_MouseMove;
            // 
            // lblSecondary
            // 
            lblSecondary.Dock = DockStyle.Fill;
            lblSecondary.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblSecondary.ForeColor = Color.White;
            lblSecondary.Location = new Point(168, 0);
            lblSecondary.Name = "lblSecondary";
            lblSecondary.Size = new Size(160, 10);
            lblSecondary.TabIndex = 146;
            lblSecondary.Text = "Secondary Power Set";
            lblSecondary.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cbPrimary
            // 
            cbPrimary.BackColor = Color.WhiteSmoke;
            cbPrimary.Dock = DockStyle.Fill;
            cbPrimary.DrawMode = DrawMode.OwnerDrawFixed;
            cbPrimary.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPrimary.ForeColor = Color.Black;
            cbPrimary.ItemHeight = 16;
            cbPrimary.Location = new Point(3, 13);
            cbPrimary.MaxDropDownItems = 15;
            cbPrimary.Name = "cbPrimary";
            cbPrimary.Size = new Size(159, 22);
            cbPrimary.TabIndex = 144;
            cbPrimary.DrawItem += cbPrimary_DrawItem;
            cbPrimary.SelectedIndexChanged += cbPrimary_SelectedIndexChanged;
            cbPrimary.MouseLeave += cbPrimary_MouseLeave;
            cbPrimary.MouseMove += cbPrimary_MouseMove;
            // 
            // lblPrimary
            // 
            lblPrimary.Dock = DockStyle.Fill;
            lblPrimary.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblPrimary.ForeColor = Color.White;
            lblPrimary.Location = new Point(3, 0);
            lblPrimary.Name = "lblPrimary";
            lblPrimary.Size = new Size(159, 10);
            lblPrimary.TabIndex = 145;
            lblPrimary.Text = "Primary Power Set";
            lblPrimary.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // llPrimary
            // 
            llPrimary.ActualLineHeight = 18;
            llPrimary.Expandable = true;
            llPrimary.HighVis = true;
            llPrimary.HoverColor = Color.WhiteSmoke;
            llPrimary.IsExpanded = false;
            llPrimary.Location = new Point(3, 31);
            llPrimary.MaxHeight = 600;
            llPrimary.Name = "llPrimary";
            llPrimary.PaddingX = 4;
            llPrimary.PaddingY = 1;
            llPrimary.Scrollable = true;
            llPrimary.ScrollBarColor = Color.Red;
            llPrimary.ScrollBarWidth = 11;
            llPrimary.ScrollButtonColor = Color.FromArgb(192, 0, 0);
            llPrimary.Size = new Size(144, 200);
            llPrimary.SizeNormal = new Size(145, 175);
            llPrimary.SuspendRedraw = false;
            llPrimary.TabIndex = 148;
            llPrimary.ItemClick += llPrimary_ItemClick;
            llPrimary.ItemHover += llPrimary_ItemHover;
            // 
            // llSecondary
            // 
            llSecondary.ActualLineHeight = 18;
            llSecondary.Expandable = true;
            llSecondary.HighVis = true;
            llSecondary.HoverColor = Color.WhiteSmoke;
            llSecondary.IsExpanded = false;
            llSecondary.Location = new Point(168, 31);
            llSecondary.MaxHeight = 600;
            llSecondary.Name = "llSecondary";
            llSecondary.PaddingX = 4;
            llSecondary.PaddingY = 1;
            llSecondary.Scrollable = true;
            llSecondary.ScrollBarColor = Color.Red;
            llSecondary.ScrollBarWidth = 11;
            llSecondary.ScrollButtonColor = Color.FromArgb(192, 0, 0);
            llSecondary.Size = new Size(153, 200);
            llSecondary.SizeNormal = new Size(145, 175);
            llSecondary.SuspendRedraw = false;
            llSecondary.TabIndex = 149;
            llSecondary.ItemClick += llSecondary_ItemClick;
            llSecondary.ItemHover += llSecondary_ItemHover;
            // 
            // topPanel
            // 
            leftControlPanel.SetColumnSpan(topPanel, 2);
            topPanel.Controls.Add(lblATLocked);
            topPanel.Controls.Add(ibTotalsEx);
            topPanel.Controls.Add(ibSlotLevelsEx);
            topPanel.Controls.Add(ibModeEx);
            topPanel.Controls.Add(ibSetsEx);
            topPanel.Controls.Add(ibDynMode);
            topPanel.Controls.Add(lblName);
            topPanel.Controls.Add(lblOrigin);
            topPanel.Controls.Add(lblAT);
            topPanel.Controls.Add(txtName);
            topPanel.Controls.Add(cbOrigin);
            topPanel.Controls.Add(cbAT);
            topPanel.Controls.Add(ibSlotInfoEx);
            topPanel.Dock = DockStyle.Fill;
            topPanel.Location = new Point(3, 3);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(514, 112);
            topPanel.TabIndex = 144;
            // 
            // lblATLocked
            // 
            lblATLocked.BackColor = Color.FromArgb(224, 224, 224);
            lblATLocked.BorderStyle = BorderStyle.Fixed3D;
            lblATLocked.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            lblATLocked.ForeColor = Color.Black;
            lblATLocked.Location = new Point(110, 28);
            lblATLocked.Name = "lblATLocked";
            lblATLocked.Size = new Size(144, 23);
            lblATLocked.TabIndex = 155;
            lblATLocked.Text = "Archetype Locked";
            lblATLocked.TextAlign = ContentAlignment.MiddleCenter;
            lblATLocked.Visible = false;
            lblATLocked.Paint += lblATLocked_Paint;
            lblATLocked.MouseLeave += lblLocked_MouseLeave;
            lblATLocked.MouseMove += lblATLocked_MouseMove;
            // 
            // ibTotalsEx
            // 
            ibTotalsEx.BackColor = Color.Transparent;
            ibTotalsEx.BackgroundImageLayout = ImageLayout.None;
            ibTotalsEx.Location = new Point(371, 29);
            ibTotalsEx.Name = "ibTotalsEx";
            ibTotalsEx.Size = new Size(105, 22);
            ibTotalsEx.TabIndex = 152;
            ibTotalsEx.Text = "View Totals";
            ibTotalsEx.ToggleText.Indeterminate = "Indeterminate State";
            ibTotalsEx.ToggleText.ToggledOff = "Toggled Off State";
            ibTotalsEx.ToggleText.ToggledOn = "Toggled On State";
            ibTotalsEx.Click += ibTotalsEx_OnClick;
            // 
            // ibSlotLevelsEx
            // 
            ibSlotLevelsEx.BackColor = Color.Transparent;
            ibSlotLevelsEx.BackgroundImageLayout = ImageLayout.None;
            ibSlotLevelsEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibSlotLevelsEx.Location = new Point(260, 53);
            ibSlotLevelsEx.Name = "ibSlotLevelsEx";
            ibSlotLevelsEx.Size = new Size(105, 22);
            ibSlotLevelsEx.TabIndex = 151;
            ibSlotLevelsEx.Text = "ibSlotLevelsEx";
            ibSlotLevelsEx.ToggleText.Indeterminate = "Indeterminate State";
            ibSlotLevelsEx.ToggleText.ToggledOff = "Slot Levels: Off";
            ibSlotLevelsEx.ToggleText.ToggledOn = "Slot Levels: On";
            ibSlotLevelsEx.Click += ibSlotLevelsEx_OnClick;
            // 
            // ibModeEx
            // 
            ibModeEx.BackColor = Color.Transparent;
            ibModeEx.BackgroundImageLayout = ImageLayout.None;
            ibModeEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibModeEx.ForeColor = Color.White;
            ibModeEx.Location = new Point(260, 0);
            ibModeEx.Name = "ibModeEx";
            ibModeEx.Size = new Size(105, 22);
            ibModeEx.TabIndex = 153;
            ibModeEx.Text = "ibModeEx1";
            ibModeEx.ThreeState = true;
            ibModeEx.ToggleText.Indeterminate = "Respec";
            ibModeEx.ToggleText.ToggledOff = "Level-Up";
            ibModeEx.ToggleText.ToggledOn = "Normal";
            ibModeEx.Click += ibModeEx_OnClick;
            // 
            // ibSetsEx
            // 
            ibSetsEx.BackColor = Color.Transparent;
            ibSetsEx.BackgroundImageLayout = ImageLayout.None;
            ibSetsEx.Location = new Point(260, 29);
            ibSetsEx.Name = "ibSetsEx";
            ibSetsEx.Size = new Size(105, 22);
            ibSetsEx.TabIndex = 149;
            ibSetsEx.Text = "View Active Sets";
            ibSetsEx.ToggleText.Indeterminate = "Indeterminate State";
            ibSetsEx.ToggleText.ToggledOff = "Toggled Off State";
            ibSetsEx.ToggleText.ToggledOn = "Toggled On State";
            ibSetsEx.Click += ibSetsEx_OnClick;
            // 
            // ibDynMode
            // 
            ibDynMode.BackColor = Color.Transparent;
            ibDynMode.BackgroundImageLayout = ImageLayout.None;
            ibDynMode.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibDynMode.ForeColor = Color.White;
            ibDynMode.Location = new Point(371, 0);
            ibDynMode.Name = "ibDynMode";
            ibDynMode.Size = new Size(105, 22);
            ibDynMode.TabIndex = 154;
            ibDynMode.Text = "ibDynMode1";
            ibDynMode.ToggleText.Indeterminate = "Power / Slot";
            ibDynMode.ToggleText.ToggledOff = "Power";
            ibDynMode.ToggleText.ToggledOn = "Slot";
            ibDynMode.Click += ibDynMode_Click;
            // 
            // lblName
            // 
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblName.ForeColor = Color.White;
            lblName.Location = new Point(18, 2);
            lblName.Name = "lblName";
            lblName.Size = new Size(92, 21);
            lblName.TabIndex = 146;
            lblName.Text = "Name:";
            lblName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            lblOrigin.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblOrigin.Location = new Point(18, 53);
            lblOrigin.Name = "lblOrigin";
            lblOrigin.Size = new Size(92, 21);
            lblOrigin.TabIndex = 148;
            lblOrigin.Text = "Origin:";
            lblOrigin.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblAT
            // 
            lblAT.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAT.Location = new Point(18, 29);
            lblAT.Name = "lblAT";
            lblAT.Size = new Size(92, 21);
            lblAT.TabIndex = 147;
            lblAT.Text = "Archetype:";
            lblAT.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            txtName.BackColor = Color.WhiteSmoke;
            txtName.ForeColor = Color.Black;
            txtName.Location = new Point(110, -1);
            txtName.Name = "txtName";
            txtName.Size = new Size(144, 27);
            txtName.TabIndex = 143;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // cbOrigin
            // 
            cbOrigin.BackColor = Color.WhiteSmoke;
            cbOrigin.DrawMode = DrawMode.OwnerDrawFixed;
            cbOrigin.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOrigin.ForeColor = Color.Black;
            cbOrigin.ItemHeight = 17;
            cbOrigin.Location = new Point(110, 53);
            cbOrigin.Name = "cbOrigin";
            cbOrigin.Size = new Size(144, 23);
            cbOrigin.TabIndex = 145;
            cbOrigin.DrawItem += cbOrigin_DrawItem;
            cbOrigin.SelectedIndexChanged += cbOrigin_SelectedIndexChanged;
            // 
            // cbAT
            // 
            cbAT.BackColor = Color.WhiteSmoke;
            cbAT.DisplayMember = "DisplayName";
            cbAT.DrawMode = DrawMode.OwnerDrawFixed;
            cbAT.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAT.ForeColor = Color.Black;
            cbAT.ItemHeight = 17;
            cbAT.Location = new Point(110, 28);
            cbAT.MaxDropDownItems = 15;
            cbAT.Name = "cbAT";
            cbAT.Size = new Size(144, 23);
            cbAT.TabIndex = 144;
            cbAT.ValueMember = "Idx";
            cbAT.DrawItem += cbAT_DrawItem;
            cbAT.SelectedIndexChanged += cbAT_SelectedIndexChanged;
            cbAT.MouseLeave += cbAT_MouseLeave;
            cbAT.MouseMove += cbAT_MouseMove;
            // 
            // ibSlotInfoEx
            // 
            ibSlotInfoEx.BackColor = Color.Transparent;
            ibSlotInfoEx.BackgroundImageLayout = ImageLayout.None;
            ibSlotInfoEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            ibSlotInfoEx.Location = new Point(371, 53);
            ibSlotInfoEx.Name = "ibSlotInfoEx";
            ibSlotInfoEx.Size = new Size(105, 22);
            ibSlotInfoEx.TabIndex = 150;
            ibSlotInfoEx.Text = "ibSlotInfoEx";
            ibSlotInfoEx.ToggleText.Indeterminate = "Indeterminate State";
            ibSlotInfoEx.ToggleText.ToggledOff = "X Slots to go";
            ibSlotInfoEx.ToggleText.ToggledOn = "X Slots placed";
            ibSlotInfoEx.Click += ibSlotInfoEx_Onclick;
            // 
            // MenuBar
            // 
            MenuBar.BackColor = Color.Transparent;
            MenuBar.ForeColor = Color.WhiteSmoke;
            MenuBar.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, OptionsToolStripMenuItem, ShareToolStripMenuItem, CharacterToolStripMenuItem, ViewToolStripMenuItem, WindowToolStripMenuItem, HelpToolStripMenuItem });
            MenuBar.Location = new Point(0, 0);
            MenuBar.Name = "MenuBar";
            MenuBar.Size = new Size(1264, 24);
            MenuBar.TabIndex = 85;
            MenuBar.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsFileNew, ToolStripSeparator7, tsFileOpen, tsFileSave, tsFileSaveAs, ToolStripSeparator22, ExportToolStripMenuItem, ToolStripSeparator8, tsFilePrint, ToolStripSeparator9, tsFileQuit });
            FileToolStripMenuItem.ForeColor = SystemColors.ControlText;
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(37, 20);
            FileToolStripMenuItem.Text = "&File";
            // 
            // tsFileNew
            // 
            tsFileNew.Name = "tsFileNew";
            tsFileNew.ShortcutKeys = Keys.Control | Keys.N;
            tsFileNew.Size = new Size(179, 22);
            tsFileNew.Text = "&New / Clear";
            tsFileNew.Click += tsFileNew_Click;
            // 
            // ToolStripSeparator7
            // 
            ToolStripSeparator7.Name = "ToolStripSeparator7";
            ToolStripSeparator7.Size = new Size(176, 6);
            // 
            // tsFileOpen
            // 
            tsFileOpen.Name = "tsFileOpen";
            tsFileOpen.ShortcutKeys = Keys.Control | Keys.O;
            tsFileOpen.Size = new Size(179, 22);
            tsFileOpen.Text = "&Open...";
            tsFileOpen.Click += tsFileOpen_Click;
            // 
            // tsFileSave
            // 
            tsFileSave.Name = "tsFileSave";
            tsFileSave.ShortcutKeys = Keys.Control | Keys.S;
            tsFileSave.Size = new Size(179, 22);
            tsFileSave.Text = "&Save";
            tsFileSave.Click += tsFileSave_Click;
            // 
            // tsFileSaveAs
            // 
            tsFileSaveAs.Name = "tsFileSaveAs";
            tsFileSaveAs.Size = new Size(179, 22);
            tsFileSaveAs.Text = "Save &As...";
            tsFileSaveAs.Click += tsFileSaveAs_Click;
            // 
            // ToolStripSeparator22
            // 
            ToolStripSeparator22.Name = "ToolStripSeparator22";
            ToolStripSeparator22.Size = new Size(176, 6);
            // 
            // ExportToolStripMenuItem
            // 
            ExportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsGenFreebies });
            ExportToolStripMenuItem.ForeColor = SystemColors.ControlText;
            ExportToolStripMenuItem.Name = "ExportToolStripMenuItem";
            ExportToolStripMenuItem.Size = new Size(179, 22);
            ExportToolStripMenuItem.Text = "Export...";
            // 
            // tsGenFreebies
            // 
            tsGenFreebies.Name = "tsGenFreebies";
            tsGenFreebies.Size = new Size(254, 22);
            tsGenFreebies.Text = "Export Build to Beta Server (Menu)";
            tsGenFreebies.Click += tsGenFreebies_Click;
            // 
            // ToolStripSeparator8
            // 
            ToolStripSeparator8.Name = "ToolStripSeparator8";
            ToolStripSeparator8.Size = new Size(176, 6);
            // 
            // tsFilePrint
            // 
            tsFilePrint.Name = "tsFilePrint";
            tsFilePrint.ShortcutKeys = Keys.Control | Keys.P;
            tsFilePrint.Size = new Size(179, 22);
            tsFilePrint.Text = "&Print...";
            tsFilePrint.Click += tsFilePrint_Click;
            // 
            // ToolStripSeparator9
            // 
            ToolStripSeparator9.Name = "ToolStripSeparator9";
            ToolStripSeparator9.Size = new Size(176, 6);
            // 
            // tsFileQuit
            // 
            tsFileQuit.Name = "tsFileQuit";
            tsFileQuit.ShortcutKeys = Keys.Control | Keys.Q;
            tsFileQuit.Size = new Size(179, 22);
            tsFileQuit.Text = "&Quit";
            tsFileQuit.Click += tsFileQuit_Click;
            // 
            // OptionsToolStripMenuItem
            // 
            OptionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsChangeDb, ToolStripSeparator29, tsConfig, ToolStripSeparator5, AdvancedToolStripMenuItem1 });
            OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            OptionsToolStripMenuItem.Size = new Size(61, 20);
            OptionsToolStripMenuItem.Text = "&Options";
            // 
            // tsChangeDb
            // 
            tsChangeDb.Name = "tsChangeDb";
            tsChangeDb.Size = new Size(166, 22);
            tsChangeDb.Text = "&Change Database";
            tsChangeDb.Click += tsChangeDb_Click;
            // 
            // ToolStripSeparator29
            // 
            ToolStripSeparator29.Name = "ToolStripSeparator29";
            ToolStripSeparator29.Size = new Size(163, 6);
            // 
            // tsConfig
            // 
            tsConfig.Name = "tsConfig";
            tsConfig.Size = new Size(166, 22);
            tsConfig.Text = "&Configuration...";
            tsConfig.Click += tsConfig_Click;
            // 
            // ToolStripSeparator5
            // 
            ToolStripSeparator5.Name = "ToolStripSeparator5";
            ToolStripSeparator5.Size = new Size(163, 6);
            // 
            // AdvancedToolStripMenuItem1
            // 
            AdvancedToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { tsAdvDBEdit, ToolStripSeparator15, tsAdvFreshInstall, tsAdvResetTips });
            AdvancedToolStripMenuItem1.Name = "AdvancedToolStripMenuItem1";
            AdvancedToolStripMenuItem1.Size = new Size(166, 22);
            AdvancedToolStripMenuItem1.Text = "&Advanced";
            // 
            // tsAdvDBEdit
            // 
            tsAdvDBEdit.Name = "tsAdvDBEdit";
            tsAdvDBEdit.Size = new Size(158, 22);
            tsAdvDBEdit.Text = "&Database Menu";
            tsAdvDBEdit.Click += tsAdvDBEdit_Click;
            // 
            // ToolStripSeparator15
            // 
            ToolStripSeparator15.Name = "ToolStripSeparator15";
            ToolStripSeparator15.Size = new Size(155, 6);
            ToolStripSeparator15.Visible = false;
            // 
            // tsAdvFreshInstall
            // 
            tsAdvFreshInstall.Name = "tsAdvFreshInstall";
            tsAdvFreshInstall.Size = new Size(158, 22);
            tsAdvFreshInstall.Text = "FreshInstall Flag";
            tsAdvFreshInstall.Visible = false;
            tsAdvFreshInstall.Click += tsAdvFreshInstall_Click;
            // 
            // tsAdvResetTips
            // 
            tsAdvResetTips.Name = "tsAdvResetTips";
            tsAdvResetTips.Size = new Size(158, 22);
            tsAdvResetTips.Text = "Reset Tips";
            tsAdvResetTips.Visible = false;
            tsAdvResetTips.Click += tsAdvResetTips_Click;
            // 
            // ShareToolStripMenuItem
            // 
            ShareToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsShareMenu, LegacyToolStripMenuItem, ToolStripSeparator24, tsImportDataChunk, ToolStripSeparator27, tsViewSharedBuilds });
            ShareToolStripMenuItem.ForeColor = SystemColors.ControlText;
            ShareToolStripMenuItem.Name = "ShareToolStripMenuItem";
            ShareToolStripMenuItem.Size = new Size(89, 20);
            ShareToolStripMenuItem.Text = "&Build Sharing";
            // 
            // tsShareMenu
            // 
            tsShareMenu.Name = "tsShareMenu";
            tsShareMenu.Size = new Size(233, 22);
            tsShareMenu.Text = "Secure Share (Recommended)";
            tsShareMenu.Click += ShareMenu_Click;
            // 
            // LegacyToolStripMenuItem
            // 
            LegacyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsShareLegacy, ToolStripSeparator26, tsImport });
            LegacyToolStripMenuItem.ForeColor = SystemColors.ControlText;
            LegacyToolStripMenuItem.Name = "LegacyToolStripMenuItem";
            LegacyToolStripMenuItem.Size = new Size(233, 22);
            LegacyToolStripMenuItem.Text = "Legacy Share";
            // 
            // tsShareLegacy
            // 
            tsShareLegacy.Name = "tsShareLegacy";
            tsShareLegacy.Size = new Size(242, 22);
            tsShareLegacy.Text = "Export Datalink";
            tsShareLegacy.Click += tsShareLegacy_Click;
            // 
            // ToolStripSeparator26
            // 
            ToolStripSeparator26.Name = "ToolStripSeparator26";
            ToolStripSeparator26.Size = new Size(239, 6);
            // 
            // tsImport
            // 
            tsImport.Name = "tsImport";
            tsImport.ShortcutKeys = Keys.Control | Keys.I;
            tsImport.Size = new Size(242, 22);
            tsImport.Text = "Import From Forum Post";
            tsImport.Click += tsImport_Click;
            // 
            // ToolStripSeparator24
            // 
            ToolStripSeparator24.Name = "ToolStripSeparator24";
            ToolStripSeparator24.Size = new Size(230, 6);
            // 
            // tsImportDataChunk
            // 
            tsImportDataChunk.Name = "tsImportDataChunk";
            tsImportDataChunk.Size = new Size(233, 22);
            tsImportDataChunk.Text = "Import DataChunk";
            tsImportDataChunk.Click += tsImportChunk_Click;
            // 
            // ToolStripSeparator27
            // 
            ToolStripSeparator27.Name = "ToolStripSeparator27";
            ToolStripSeparator27.Size = new Size(230, 6);
            // 
            // tsViewSharedBuilds
            // 
            tsViewSharedBuilds.Name = "tsViewSharedBuilds";
            tsViewSharedBuilds.Size = new Size(233, 22);
            tsViewSharedBuilds.Text = "View Secure Shared Builds";
            tsViewSharedBuilds.Click += tsViewSharedBuilds_Click;
            // 
            // CharacterToolStripMenuItem
            // 
            CharacterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { SetAllIOsToDefault35ToolStripMenuItem, ToolStripSeparator16, ToolStripMenuItem1, ToolStripMenuItem2, ToolStripSeparator17, SlotsToolStripMenuItem, ToolStripSeparator28, ToggleCheckModeToolStripMenuItem });
            CharacterToolStripMenuItem.ForeColor = SystemColors.ControlText;
            CharacterToolStripMenuItem.Name = "CharacterToolStripMenuItem";
            CharacterToolStripMenuItem.Size = new Size(133, 20);
            CharacterToolStripMenuItem.Text = "&Slots / Enhancements";
            // 
            // SetAllIOsToDefault35ToolStripMenuItem
            // 
            SetAllIOsToDefault35ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsIODefault, ToolStripSeparator11, tsIOMin, tsIOMax });
            SetAllIOsToDefault35ToolStripMenuItem.Name = "SetAllIOsToDefault35ToolStripMenuItem";
            SetAllIOsToDefault35ToolStripMenuItem.Size = new Size(256, 22);
            SetAllIOsToDefault35ToolStripMenuItem.Text = "&Set all IOs to...";
            // 
            // tsIODefault
            // 
            tsIODefault.Name = "tsIODefault";
            tsIODefault.Size = new Size(135, 22);
            tsIODefault.Text = "Default (35)";
            tsIODefault.Click += tsIODefault_Click;
            // 
            // ToolStripSeparator11
            // 
            ToolStripSeparator11.Name = "ToolStripSeparator11";
            ToolStripSeparator11.Size = new Size(132, 6);
            // 
            // tsIOMin
            // 
            tsIOMin.Name = "tsIOMin";
            tsIOMin.Size = new Size(135, 22);
            tsIOMin.Text = "Minimum";
            tsIOMin.Click += tsIOMin_Click;
            // 
            // tsIOMax
            // 
            tsIOMax.Name = "tsIOMax";
            tsIOMax.Size = new Size(135, 22);
            tsIOMax.Text = "Maximum";
            tsIOMax.Click += tsIOMax_Click;
            // 
            // ToolStripSeparator16
            // 
            ToolStripSeparator16.Name = "ToolStripSeparator16";
            ToolStripSeparator16.Size = new Size(253, 6);
            // 
            // ToolStripMenuItem1
            // 
            ToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { tsEnhToSO, tsEnhToDO, tsEnhToTO });
            ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            ToolStripMenuItem1.Size = new Size(256, 22);
            ToolStripMenuItem1.Text = "Set all Enhancements' &Origin to...";
            // 
            // tsEnhToSO
            // 
            tsEnhToSO.Name = "tsEnhToSO";
            tsEnhToSO.Size = new Size(142, 22);
            tsEnhToSO.Text = "Single Origin";
            tsEnhToSO.Click += tsEnhToSO_Click;
            // 
            // tsEnhToDO
            // 
            tsEnhToDO.Name = "tsEnhToDO";
            tsEnhToDO.Size = new Size(142, 22);
            tsEnhToDO.Text = "Dual Origin";
            tsEnhToDO.Click += tsEnhToDO_Click;
            // 
            // tsEnhToTO
            // 
            tsEnhToTO.Name = "tsEnhToTO";
            tsEnhToTO.Size = new Size(142, 22);
            tsEnhToTO.Text = "Training";
            tsEnhToTO.Click += tsEnhToTO_Click;
            // 
            // ToolStripMenuItem2
            // 
            ToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { tsEnhToPlus5, tsEnhToPlus4, tsEnhToPlus3, tsEnhToPlus2, tsEnhToPlus1, tsEnhToEven, tsEnhToMinus1, tsEnhToMinus2, tsEnhToMinus3, tsEnhToNone });
            ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            ToolStripMenuItem2.Size = new Size(256, 22);
            ToolStripMenuItem2.Text = "Set all &Relative Levels to...";
            // 
            // tsEnhToPlus5
            // 
            tsEnhToPlus5.Name = "tsEnhToPlus5";
            tsEnhToPlus5.Size = new Size(205, 22);
            tsEnhToPlus5.Text = "+5 Levels";
            tsEnhToPlus5.Click += tsEnhToPlus5_Click;
            // 
            // tsEnhToPlus4
            // 
            tsEnhToPlus4.Name = "tsEnhToPlus4";
            tsEnhToPlus4.Size = new Size(205, 22);
            tsEnhToPlus4.Text = "+4 Levels";
            tsEnhToPlus4.Click += tsEnhToPlus4_Click;
            // 
            // tsEnhToPlus3
            // 
            tsEnhToPlus3.Name = "tsEnhToPlus3";
            tsEnhToPlus3.Size = new Size(205, 22);
            tsEnhToPlus3.Text = "+3 Levels";
            tsEnhToPlus3.Click += tsEnhToPlus3_Click;
            // 
            // tsEnhToPlus2
            // 
            tsEnhToPlus2.Name = "tsEnhToPlus2";
            tsEnhToPlus2.Size = new Size(205, 22);
            tsEnhToPlus2.Text = "+2 Levels";
            tsEnhToPlus2.Click += tsEnhToPlus2_Click;
            // 
            // tsEnhToPlus1
            // 
            tsEnhToPlus1.Name = "tsEnhToPlus1";
            tsEnhToPlus1.Size = new Size(205, 22);
            tsEnhToPlus1.Text = "+1 Level";
            tsEnhToPlus1.Click += tsEnhToPlus1_Click;
            // 
            // tsEnhToEven
            // 
            tsEnhToEven.Name = "tsEnhToEven";
            tsEnhToEven.Size = new Size(205, 22);
            tsEnhToEven.Text = "Even Level";
            tsEnhToEven.Click += tsEnhToEven_Click;
            // 
            // tsEnhToMinus1
            // 
            tsEnhToMinus1.Name = "tsEnhToMinus1";
            tsEnhToMinus1.Size = new Size(205, 22);
            tsEnhToMinus1.Text = "-1 Level";
            tsEnhToMinus1.Click += tsEnhToMinus1_Click;
            // 
            // tsEnhToMinus2
            // 
            tsEnhToMinus2.Name = "tsEnhToMinus2";
            tsEnhToMinus2.Size = new Size(205, 22);
            tsEnhToMinus2.Text = "-2 Levels";
            tsEnhToMinus2.Click += tsEnhToMinus2_Click;
            // 
            // tsEnhToMinus3
            // 
            tsEnhToMinus3.Name = "tsEnhToMinus3";
            tsEnhToMinus3.Size = new Size(205, 22);
            tsEnhToMinus3.Text = "-3 Levels";
            tsEnhToMinus3.Click += tsEnhToMinus3_Click;
            // 
            // tsEnhToNone
            // 
            tsEnhToNone.Name = "tsEnhToNone";
            tsEnhToNone.Size = new Size(205, 22);
            tsEnhToNone.Text = "None (Enh has no effect)";
            tsEnhToNone.Click += tsEnhToNone_Click;
            // 
            // ToolStripSeparator17
            // 
            ToolStripSeparator17.Name = "ToolStripSeparator17";
            ToolStripSeparator17.Size = new Size(253, 6);
            // 
            // SlotsToolStripMenuItem
            // 
            SlotsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsFlipAllEnh, ToolStripSeparator4, tsClearAllEnh, tsRemoveAllSlots, ToolStripSeparator1, AutoArrangeAllSlotsToolStripMenuItem });
            SlotsToolStripMenuItem.Name = "SlotsToolStripMenuItem";
            SlotsToolStripMenuItem.Size = new Size(256, 22);
            SlotsToolStripMenuItem.Text = "Slo&ts";
            // 
            // tsFlipAllEnh
            // 
            tsFlipAllEnh.Name = "tsFlipAllEnh";
            tsFlipAllEnh.Size = new Size(199, 22);
            tsFlipAllEnh.Text = "Flip All to Alternate";
            tsFlipAllEnh.Click += tsFlipAllEnh_Click;
            // 
            // ToolStripSeparator4
            // 
            ToolStripSeparator4.Name = "ToolStripSeparator4";
            ToolStripSeparator4.Size = new Size(196, 6);
            // 
            // tsClearAllEnh
            // 
            tsClearAllEnh.Name = "tsClearAllEnh";
            tsClearAllEnh.Size = new Size(199, 22);
            tsClearAllEnh.Text = "Clear All Enhancements";
            tsClearAllEnh.Click += tsClearAllEnh_Click;
            // 
            // tsRemoveAllSlots
            // 
            tsRemoveAllSlots.Name = "tsRemoveAllSlots";
            tsRemoveAllSlots.Size = new Size(199, 22);
            tsRemoveAllSlots.Text = "Remove All Slots";
            tsRemoveAllSlots.Click += tsRemoveAllSlots_Click;
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(196, 6);
            // 
            // AutoArrangeAllSlotsToolStripMenuItem
            // 
            AutoArrangeAllSlotsToolStripMenuItem.Name = "AutoArrangeAllSlotsToolStripMenuItem";
            AutoArrangeAllSlotsToolStripMenuItem.Size = new Size(199, 22);
            AutoArrangeAllSlotsToolStripMenuItem.Text = "&Auto-Arrange All Slots";
            AutoArrangeAllSlotsToolStripMenuItem.Click += AutoArrangeAllSlotsToolStripMenuItem_Click;
            // 
            // ToolStripSeparator28
            // 
            ToolStripSeparator28.Name = "ToolStripSeparator28";
            ToolStripSeparator28.Size = new Size(253, 6);
            // 
            // ToggleCheckModeToolStripMenuItem
            // 
            ToggleCheckModeToolStripMenuItem.Name = "ToggleCheckModeToolStripMenuItem";
            ToggleCheckModeToolStripMenuItem.Size = new Size(256, 22);
            ToggleCheckModeToolStripMenuItem.Text = "Toggle Enhancement Check Mode";
            ToggleCheckModeToolStripMenuItem.Click += tsToggleCheckModeToolStripMenuItem_Click;
            // 
            // ViewToolStripMenuItem
            // 
            ViewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsView2Col, tsView3Col, tsView4Col, tsView5Col, tsView6Col, tsView3ColV, tsView3ColH, ToolStripSeparator13, tsViewIOLevels, tsViewSOLevels, tsViewRelative, tsViewSlotLevels, tsViewRelativeAsSigns, ToolStripSeparator2, tsViewActualDamage_New, tsViewDPS_New, tlsDPA });
            ViewToolStripMenuItem.ForeColor = SystemColors.ControlText;
            ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            ViewToolStripMenuItem.Size = new Size(44, 20);
            ViewToolStripMenuItem.Text = "&View";
            // 
            // tsView2Col
            // 
            tsView2Col.Name = "tsView2Col";
            tsView2Col.Size = new Size(338, 22);
            tsView2Col.Text = "&2 Columns";
            tsView2Col.Click += tsView2Col_Click;
            // 
            // tsView3Col
            // 
            tsView3Col.Name = "tsView3Col";
            tsView3Col.Size = new Size(338, 22);
            tsView3Col.Text = "&3 Columns";
            tsView3Col.Click += tsView3Col_Click;
            // 
            // tsView4Col
            // 
            tsView4Col.Name = "tsView4Col";
            tsView4Col.Size = new Size(338, 22);
            tsView4Col.Text = "&4 Columns";
            tsView4Col.Click += tsView4Col_Click;
            // 
            // tsView5Col
            // 
            tsView5Col.Name = "tsView5Col";
            tsView5Col.Size = new Size(338, 22);
            tsView5Col.Text = "&5 Columns";
            tsView5Col.Click += tsView5Col_Click;
            // 
            // tsView6Col
            // 
            tsView6Col.Name = "tsView6Col";
            tsView6Col.Size = new Size(338, 22);
            tsView6Col.Text = "&6 Columns";
            tsView6Col.Click += tsView6Col_Click;
            // 
            // tsView3ColV
            // 
            tsView3ColV.Name = "tsView3ColV";
            tsView3ColV.Size = new Size(338, 22);
            tsView3ColV.Text = "3 Columns (per powerset, vertical stacking)";
            tsView3ColV.Click += tsView3ColV_Click;
            // 
            // tsView3ColH
            // 
            tsView3ColH.Name = "tsView3ColH";
            tsView3ColH.Size = new Size(338, 22);
            tsView3ColH.Text = "Multi-columns (per powerset, horizontal stacking)";
            tsView3ColH.Click += tsView3ColH_Click;
            // 
            // ToolStripSeparator13
            // 
            ToolStripSeparator13.Name = "ToolStripSeparator13";
            ToolStripSeparator13.Size = new Size(335, 6);
            // 
            // tsViewIOLevels
            // 
            tsViewIOLevels.Checked = true;
            tsViewIOLevels.CheckState = CheckState.Checked;
            tsViewIOLevels.Name = "tsViewIOLevels";
            tsViewIOLevels.Size = new Size(338, 22);
            tsViewIOLevels.Text = "Show &IO Levels";
            tsViewIOLevels.Click += tsViewIOLevels_Click;
            // 
            // tsViewSOLevels
            // 
            tsViewSOLevels.Name = "tsViewSOLevels";
            tsViewSOLevels.Size = new Size(338, 22);
            tsViewSOLevels.Text = "Show SO/HO Levels";
            tsViewSOLevels.Click += tsViewSOLevels_Click;
            // 
            // tsViewRelative
            // 
            tsViewRelative.Name = "tsViewRelative";
            tsViewRelative.Size = new Size(338, 22);
            tsViewRelative.Text = "Show &Enhancement Relative Levels";
            tsViewRelative.Click += tsViewRelative_Click;
            // 
            // tsViewSlotLevels
            // 
            tsViewSlotLevels.Name = "tsViewSlotLevels";
            tsViewSlotLevels.Size = new Size(338, 22);
            tsViewSlotLevels.Text = "Show &Slot Placement Levels";
            tsViewSlotLevels.Click += tsViewSlotLevels_Click;
            // 
            // tsViewRelativeAsSigns
            // 
            tsViewRelativeAsSigns.Name = "tsViewRelativeAsSigns";
            tsViewRelativeAsSigns.Size = new Size(338, 22);
            tsViewRelativeAsSigns.Text = "Show Relative Levels with signs ('+'/'-')";
            tsViewRelativeAsSigns.Click += tsViewRelativeAsSigns_Click;
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(335, 6);
            // 
            // tsViewActualDamage_New
            // 
            tsViewActualDamage_New.Checked = true;
            tsViewActualDamage_New.CheckState = CheckState.Checked;
            tsViewActualDamage_New.Name = "tsViewActualDamage_New";
            tsViewActualDamage_New.Size = new Size(338, 22);
            tsViewActualDamage_New.Text = "Show Damage Per Activation (Level 50)";
            tsViewActualDamage_New.Click += tsViewActualDamage_New_Click;
            // 
            // tsViewDPS_New
            // 
            tsViewDPS_New.Name = "tsViewDPS_New";
            tsViewDPS_New.Size = new Size(338, 22);
            tsViewDPS_New.Text = "Show Damage Per Second (Level 50)";
            tsViewDPS_New.Click += tsViewDPS_New_Click;
            // 
            // tlsDPA
            // 
            tlsDPA.Name = "tlsDPA";
            tlsDPA.Size = new Size(338, 22);
            tlsDPA.Text = "Show Damage Per Animation (Level 50)";
            tlsDPA.Click += tlsDPA_Click;
            // 
            // WindowToolStripMenuItem
            // 
            WindowToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsViewSets, tsViewGraphs, tsViewSetCompare, tsViewData, tsViewTotals, ToolStripSeparator18, tsRecipeViewer, tsRotationHelper, ToolStripSeparator19, tsSetFind, ToolStripSeparator21, InGameRespecHelperToolStripMenuItem });
            WindowToolStripMenuItem.ForeColor = SystemColors.ControlText;
            WindowToolStripMenuItem.Name = "WindowToolStripMenuItem";
            WindowToolStripMenuItem.Size = new Size(68, 20);
            WindowToolStripMenuItem.Text = "&Windows";
            // 
            // tsViewSets
            // 
            tsViewSets.Name = "tsViewSets";
            tsViewSets.ShortcutKeys = Keys.Control | Keys.B;
            tsViewSets.Size = new Size(232, 22);
            tsViewSets.Text = "&Sets && Bonuses";
            tsViewSets.Click += tsViewSets_Click;
            // 
            // tsViewGraphs
            // 
            tsViewGraphs.Name = "tsViewGraphs";
            tsViewGraphs.ShortcutKeys = Keys.Control | Keys.G;
            tsViewGraphs.Size = new Size(232, 22);
            tsViewGraphs.Text = "Power &Graphs";
            tsViewGraphs.Click += tsViewGraphs_Click;
            // 
            // tsViewSetCompare
            // 
            tsViewSetCompare.Name = "tsViewSetCompare";
            tsViewSetCompare.ShortcutKeys = Keys.Control | Keys.C;
            tsViewSetCompare.Size = new Size(232, 22);
            tsViewSetCompare.Text = "Powerset &Comparison";
            tsViewSetCompare.Click += tsViewSetCompare_Click;
            // 
            // tsViewData
            // 
            tsViewData.Name = "tsViewData";
            tsViewData.ShortcutKeys = Keys.Control | Keys.D;
            tsViewData.Size = new Size(232, 22);
            tsViewData.Text = "&Data View";
            tsViewData.Click += tsViewData_Click;
            // 
            // tsViewTotals
            // 
            tsViewTotals.Name = "tsViewTotals";
            tsViewTotals.ShortcutKeys = Keys.Control | Keys.T;
            tsViewTotals.Size = new Size(232, 22);
            tsViewTotals.Text = "Advanced &Totals";
            tsViewTotals.Click += tsViewTotals_Click;
            // 
            // ToolStripSeparator18
            // 
            ToolStripSeparator18.Name = "ToolStripSeparator18";
            ToolStripSeparator18.Size = new Size(229, 6);
            // 
            // tsRecipeViewer
            // 
            tsRecipeViewer.Name = "tsRecipeViewer";
            tsRecipeViewer.ShortcutKeys = Keys.Control | Keys.R;
            tsRecipeViewer.Size = new Size(232, 22);
            tsRecipeViewer.Text = "&Recipe Viewer";
            tsRecipeViewer.Click += tsRecipeViewer_Click;
            // 
            // tsRotationHelper
            // 
            tsRotationHelper.Name = "tsRotationHelper";
            tsRotationHelper.ShortcutKeys = Keys.Control | Keys.Z;
            tsRotationHelper.Size = new Size(232, 22);
            tsRotationHelper.Text = "Rotation Helper (Beta)";
            tsRotationHelper.Click += tsRotationHelper_Click;
            // 
            // ToolStripSeparator19
            // 
            ToolStripSeparator19.Name = "ToolStripSeparator19";
            ToolStripSeparator19.Size = new Size(229, 6);
            // 
            // tsSetFind
            // 
            tsSetFind.Name = "tsSetFind";
            tsSetFind.Size = new Size(232, 22);
            tsSetFind.Text = "Set &Inspector";
            tsSetFind.Click += tsSetFind_Click;
            // 
            // ToolStripSeparator21
            // 
            ToolStripSeparator21.Name = "ToolStripSeparator21";
            ToolStripSeparator21.Size = new Size(229, 6);
            // 
            // InGameRespecHelperToolStripMenuItem
            // 
            InGameRespecHelperToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsHelperShort, tsHelperLong });
            InGameRespecHelperToolStripMenuItem.Name = "InGameRespecHelperToolStripMenuItem";
            InGameRespecHelperToolStripMenuItem.Size = new Size(232, 22);
            InGameRespecHelperToolStripMenuItem.Text = "In-Game &Respec Helper";
            // 
            // tsHelperShort
            // 
            tsHelperShort.Name = "tsHelperShort";
            tsHelperShort.Size = new Size(139, 22);
            tsHelperShort.Text = "Profile &Short";
            tsHelperShort.Click += tsHelperShort_Click;
            // 
            // tsHelperLong
            // 
            tsHelperLong.Name = "tsHelperLong";
            tsHelperLong.Size = new Size(139, 22);
            tsHelperLong.Text = "Profile &Long";
            tsHelperLong.Click += tsHelperLong_Click;
            // 
            // HelpToolStripMenuItem
            // 
            HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsHelp, tsUpdateCheck, ToolStripSeparator10, tsBuildRcv, ToolStripSeparator30, tsGitHub, tsSupport, ToolStripSeparator31, tsAbout });
            HelpToolStripMenuItem.ForeColor = SystemColors.ControlText;
            HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            HelpToolStripMenuItem.Size = new Size(44, 20);
            HelpToolStripMenuItem.Text = "Help";
            // 
            // tsHelp
            // 
            tsHelp.Name = "tsHelp";
            tsHelp.Size = new Size(205, 22);
            // 
            // tsUpdateCheck
            // 
            tsUpdateCheck.Name = "tsUpdateCheck";
            tsUpdateCheck.Size = new Size(205, 22);
            tsUpdateCheck.Text = "Check for &Updates";
            tsUpdateCheck.Click += tsUpdateCheck_Click;
            // 
            // ToolStripSeparator10
            // 
            ToolStripSeparator10.Name = "ToolStripSeparator10";
            ToolStripSeparator10.Size = new Size(202, 6);
            // 
            // tsBuildRcv
            // 
            tsBuildRcv.Name = "tsBuildRcv";
            tsBuildRcv.Size = new Size(205, 22);
            tsBuildRcv.Text = "Attempt Build recovery...";
            tsBuildRcv.Click += tsBuildRcv_Click;
            // 
            // ToolStripSeparator30
            // 
            ToolStripSeparator30.Name = "ToolStripSeparator30";
            ToolStripSeparator30.Size = new Size(202, 6);
            // 
            // tsGitHub
            // 
            tsGitHub.Name = "tsGitHub";
            tsGitHub.Size = new Size(205, 22);
            tsGitHub.Text = "View Our GitHub";
            tsGitHub.Click += Github_Link;
            // 
            // tsSupport
            // 
            tsSupport.Name = "tsSupport";
            tsSupport.Size = new Size(205, 22);
            tsSupport.Text = "Get Support (Discord)";
            tsSupport.Click += tsSupport_Click;
            // 
            // ToolStripSeparator31
            // 
            ToolStripSeparator31.Name = "ToolStripSeparator31";
            ToolStripSeparator31.Size = new Size(202, 6);
            // 
            // tsAbout
            // 
            tsAbout.Name = "tsAbout";
            tsAbout.Size = new Size(205, 22);
            tsAbout.Text = "About...";
            tsAbout.Click += tsAbout_Click;
            // 
            // EnemyRelativeToolStripComboBox
            // 
            EnemyRelativeToolStripComboBox.BackColor = Color.FromArgb(27, 27, 27);
            EnemyRelativeToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            EnemyRelativeToolStripComboBox.FlatStyle = FlatStyle.Standard;
            EnemyRelativeToolStripComboBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            EnemyRelativeToolStripComboBox.ForeColor = Color.FromArgb(238, 238, 238);
            EnemyRelativeToolStripComboBox.Margin = new Padding(20, 0, 0, 0);
            EnemyRelativeToolStripComboBox.Name = "EnemyRelativeToolStripComboBox";
            EnemyRelativeToolStripComboBox.Size = new Size(225, 27);
            // 
            // EditBuildCommentMenuItem
            // 
            EditBuildCommentMenuItem.IconChar = FontAwesome.Sharp.IconChar.Pencil;
            EditBuildCommentMenuItem.IconColor = Color.WhiteSmoke;
            EditBuildCommentMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            EditBuildCommentMenuItem.Margin = new Padding(20, 0, 0, 0);
            EditBuildCommentMenuItem.Name = "EditBuildCommentMenuItem";
            EditBuildCommentMenuItem.Size = new Size(142, 27);
            EditBuildCommentMenuItem.Text = "Edit Build Comment";
            EditBuildCommentMenuItem.Click += EditBuildComment_Click;
            EditBuildCommentMenuItem.MouseEnter += EditBuildComment_MouseEnter;
            EditBuildCommentMenuItem.MouseLeave += EditBuildComment_MouseLeave;
            // 
            // DonateToolStripMenuItem
            // 
            DonateToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            DonateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsKoFi, tsPatreon });
            DonateToolStripMenuItem.ForeColor = SystemColors.ControlText;
            DonateToolStripMenuItem.Name = "DonateToolStripMenuItem";
            DonateToolStripMenuItem.Size = new Size(131, 27);
            DonateToolStripMenuItem.Text = "Support Mids Reborn";
            // 
            // tsKoFi
            // 
            tsKoFi.Name = "tsKoFi";
            tsKoFi.Size = new Size(174, 22);
            tsKoFi.Text = "Donate via Ko-Fi";
            tsKoFi.Click += tsKoFi_Click;
            // 
            // tsPatreon
            // 
            tsPatreon.Name = "tsPatreon";
            tsPatreon.Size = new Size(174, 22);
            tsPatreon.Text = "Donate via Patreon";
            tsPatreon.Click += tsPatreon_Click;
            // 
            // DlgOpen
            // 
            DlgOpen.DefaultExt = "mbd";
            DlgOpen.Filter = "All Supported Formats (*.mbd; *.mxd; *.txt)|*.mbd;*.mxd;*.txt|Character Builds (*.mbd)|*.mbd|Legacy Character Builds (*.mxd;*.txt)|*.mxd;*.txt|Game Export Builds (*.txt)|*.txt";
            // 
            // DlgSave
            // 
            DlgSave.DefaultExt = "mbd";
            DlgSave.Filter = "Character Build, MBD Format (*.mbd)|*.mbd|Character Build, Legacy Format (*.mxd;)|*.mxd;";
            // 
            // tTip
            // 
            tTip.AutoPopDelay = 5000;
            tTip.InitialDelay = 500;
            tTip.ReshowDelay = 100;
            // 
            // panel1
            // 
            leftInsidePanel.SetColumnSpan(panel1, 2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 268);
            panel1.Name = "panel1";
            panel1.Size = new Size(325, 366);
            panel1.TabIndex = 151;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(1264, 867);
            Controls.Add(mainLayoutPanel);
            Controls.Add(MenuBar);
            Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.WhiteSmoke;
            Name = "MainWindow";
            StartPosition = FormStartPosition.Manual;
            Text = "MainWindow";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            buttonsLayoutPanel.ResumeLayout(false);
            pnlGFXFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlGFX).EndInit();
            leftControlPanel.ResumeLayout(false);
            poolsPanel.ResumeLayout(false);
            leftInsidePanel.ResumeLayout(false);
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            MenuBar.ResumeLayout(false);
            MenuBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private MidsMenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsFileNew;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem tsFileOpen;
        private System.Windows.Forms.ToolStripMenuItem tsFileSave;
        private System.Windows.Forms.ToolStripMenuItem tsFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem ExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsGenFreebies;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem tsFilePrint;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem tsFileQuit;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsChangeDb;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator29;
        private System.Windows.Forms.ToolStripMenuItem tsConfig;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem AdvancedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsAdvDBEdit;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem tsAdvFreshInstall;
        private System.Windows.Forms.ToolStripMenuItem tsAdvResetTips;
        private System.Windows.Forms.ToolStripMenuItem ShareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsShareMenu;
        private System.Windows.Forms.ToolStripMenuItem LegacyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsShareLegacy;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator26;
        private System.Windows.Forms.ToolStripMenuItem tsImport;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator24;
        private System.Windows.Forms.ToolStripMenuItem tsImportDataChunk;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem tsViewSharedBuilds;
        private System.Windows.Forms.ToolStripMenuItem CharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetAllIOsToDefault35ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsIODefault;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem tsIOMin;
        private System.Windows.Forms.ToolStripMenuItem tsIOMax;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToSO;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToDO;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToTO;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus5;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus4;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus3;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus2;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToPlus1;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToEven;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus1;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus2;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToMinus3;
        private System.Windows.Forms.ToolStripMenuItem tsEnhToNone;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem SlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsFlipAllEnh;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsClearAllEnh;
        private System.Windows.Forms.ToolStripMenuItem tsRemoveAllSlots;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem AutoArrangeAllSlotsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator28;
        private System.Windows.Forms.ToolStripMenuItem ToggleCheckModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsView2Col;
        private System.Windows.Forms.ToolStripMenuItem tsView3Col;
        private System.Windows.Forms.ToolStripMenuItem tsView4Col;
        private System.Windows.Forms.ToolStripMenuItem tsView5Col;
        private System.Windows.Forms.ToolStripMenuItem tsView6Col;
        private System.Windows.Forms.ToolStripMenuItem tsView3ColV;
        private System.Windows.Forms.ToolStripMenuItem tsView3ColH;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem tsViewIOLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewSOLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewRelative;
        private System.Windows.Forms.ToolStripMenuItem tsViewSlotLevels;
        private System.Windows.Forms.ToolStripMenuItem tsViewRelativeAsSigns;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsViewActualDamage_New;
        private System.Windows.Forms.ToolStripMenuItem tsViewDPS_New;
        private System.Windows.Forms.ToolStripMenuItem tlsDPA;
        private System.Windows.Forms.ToolStripMenuItem WindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsViewSets;
        private System.Windows.Forms.ToolStripMenuItem tsViewGraphs;
        private System.Windows.Forms.ToolStripMenuItem tsViewSetCompare;
        private System.Windows.Forms.ToolStripMenuItem tsViewData;
        private System.Windows.Forms.ToolStripMenuItem tsViewTotals;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem tsRecipeViewer;
        private System.Windows.Forms.ToolStripMenuItem tsRotationHelper;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem tsSetFind;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem InGameRespecHelperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsHelperShort;
        private System.Windows.Forms.ToolStripMenuItem tsHelperLong;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsHelp;
        private System.Windows.Forms.ToolStripMenuItem tsUpdateCheck;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem tsBuildRcv;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator30;
        private System.Windows.Forms.ToolStripMenuItem tsGitHub;
        private System.Windows.Forms.ToolStripMenuItem tsSupport;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator31;
        private System.Windows.Forms.ToolStripMenuItem tsAbout;
        private System.Windows.Forms.ToolStripComboBox EnemyRelativeToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem DonateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsKoFi;
        private System.Windows.Forms.ToolStripMenuItem tsPatreon;
        private System.Windows.Forms.Label lblCharacter;
        private System.Windows.Forms.FlowLayoutPanel buttonsLayoutPanel;
        private MidsVectorButton ibTeamEx;
        private MidsVectorButton ibAlignmentEx;
        private MidsVectorButton ibTempPowersEx;
        private MidsVectorButton ibAccoladesEx;
        internal MidsVectorButton ibIncarnatePowersEx;
        internal MidsVectorButton ibPrestigePowersEx;
        private MidsVectorButton ibPvXEx;
        private MidsVectorButton ibRecipeEx;
        private MidsVectorButton ibPopupEx;
        private System.Windows.Forms.TableLayoutPanel leftControlPanel;
        private ScrollPanelEx pnlGFXFlow;
        private PanelGfx pnlGFX;
        internal System.Windows.Forms.OpenFileDialog DlgOpen;
        internal System.Windows.Forms.SaveFileDialog DlgSave;
        private System.Windows.Forms.ToolTip tTip;
        private System.Windows.Forms.Timer tmrGfx;
        private FontAwesome.Sharp.IconMenuItem EditBuildCommentMenuItem;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblATLocked;
        private MidsVectorButton ibTotalsEx;
        private MidsVectorButton ibSlotLevelsEx;
        private MidsVectorButton ibModeEx;
        private MidsVectorButton ibSetsEx;
        private MidsVectorButton ibDynMode;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblOrigin;
        private System.Windows.Forms.Label lblAT;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cbOrigin;
        private System.Windows.Forms.ComboBox cbAT;
        private MidsVectorButton ibSlotInfoEx;
        private System.Windows.Forms.TableLayoutPanel leftInsidePanel;
        private System.Windows.Forms.ComboBox cbSecondary;
        private System.Windows.Forms.Label lblSecondary;
        private System.Windows.Forms.ComboBox cbPrimary;
        private System.Windows.Forms.Label lblPrimary;
        private ListLabel llPrimary;
        private ListLabel llSecondary;
        private ScrollPanelEx poolsPanel;
        private System.Windows.Forms.Label lblLockedAncillary;
        private System.Windows.Forms.Label lblLockedPool3;
        private System.Windows.Forms.Label lblLockedPool2;
        private System.Windows.Forms.Label lblLockedPool1;
        private System.Windows.Forms.Label lblLockedPool0;
        private System.Windows.Forms.ComboBox cbPool0;
        private System.Windows.Forms.ComboBox cbPool1;
        private System.Windows.Forms.ComboBox cbPool2;
        private System.Windows.Forms.ComboBox cbPool3;
        private System.Windows.Forms.ComboBox cbAncillary;
        private System.Windows.Forms.Label lblPool1;
        private System.Windows.Forms.Label lblPool2;
        private System.Windows.Forms.Label lblPool3;
        private System.Windows.Forms.Label lblPool4;
        private System.Windows.Forms.Label lblEpic;
        private ListLabel llPool0;
        private ListLabel llPool1;
        private ListLabel llPool2;
        private ListLabel llPool3;
        private ListLabel llAncillary;
        private Label lblLockedSecondary;
        private Panel panel1;
    }
}
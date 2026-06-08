using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Forms.Controls;
using Timer = System.Windows.Forms.Timer;

namespace Mids_Reborn.UI.Forms
{
    partial class MainWindow2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow2));
            mainLayoutPanel = new TableLayoutPanel();
            characterLayoutPanel = new TableLayoutPanel();
            leftLayoutPanel = new MidsSmartLayoutPanel();
            rightLayoutPanel = new TableLayoutPanel();
            buttonsLayoutPanel = new TableLayoutPanel();
            canvasScrollPanel = new MidsVScrollPanel();
            canvas = new MidsBufferedImagePanel();
            dynMode = new MidsVectorButton();
            lblName = new Label();
            lblAT = new Label();
            lblOrigin = new Label();
            characterPanel = new Panel();
            originDropDown = new OriginDropDownList();
            atDropDown = new ArchetypeDropDownList();
            txtName = new TextBox();
            modeEx = new MidsVectorButton();
            totalsEx = new MidsVectorButton();
            slotInfoEx = new MidsVectorButton();
            setsEx = new MidsVectorButton();
            slotLevelsEx = new MidsVectorButton();
            midsvScrollPanel1 = new MidsVScrollPanel();
            rightInnerLayoutPanel = new TableLayoutPanel();
            ancillaryList = new MidsListView();
            ancillaryDropDown = new PowersetDropDownList();
            ancillaryLabel = new Label();
            pool3List = new MidsListView();
            pool3DropDown = new PowersetDropDownList();
            pool3Label = new Label();
            pool2List = new MidsListView();
            pool2DropDown = new PowersetDropDownList();
            pool2Label = new Label();
            pool1List = new MidsListView();
            pool1DropDown = new PowersetDropDownList();
            pool1Label = new Label();
            pool0DropDown = new PowersetDropDownList();
            pool0Label = new Label();
            pool0List = new MidsListView();
            leftInnerLayoutPanel = new TableLayoutPanel();
            dataView = new MidsDataViewNeo();
            secondaryList = new MidsListView();
            secondaryDropDown = new PowersetDropDownList();
            label1 = new Label();
            lblPrimary = new Label();
            primaryList = new MidsListView();
            primaryDropDown = new PowersetDropDownList();
            combatEx = new MidsVectorButton();
            tempPowersEx = new MidsVectorButton();
            ibPrestigePowersEx = new MidsVectorButton();
            incarnatesEx = new MidsVectorButton();
            accoladesEx = new MidsVectorButton();
            popupEx = new MidsVectorButton();
            pvXEx = new MidsVectorButton();
            recipeEx = new MidsVectorButton();
            footerPanel = new Panel();
            footerLayoutPanel = new TableLayoutPanel();
            footerSummaryLabel = new Label();
            footerRightPanel = new FlowLayoutPanel();
            footerTotalSlotsLabel = new Label();
            footerSlotsLeftLabel = new Label();
            MenuBar = new MidsMenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            tsFileNew = new ToolStripMenuItem();
            ToolStripSeparator7 = new ToolStripSeparator();
            tsFileOpen = new ToolStripMenuItem();
            tsBuildRcv = new ToolStripMenuItem();
            toolStripSeparator14 = new ToolStripSeparator();
            tsFileSave = new ToolStripMenuItem();
            tsFileSaveAs = new ToolStripMenuItem();
            ToolStripSeparator22 = new ToolStripSeparator();
            tsGenFreebies = new ToolStripMenuItem();
            ToolStripSeparator8 = new ToolStripSeparator();
            tsFilePrint = new ToolStripMenuItem();
            ToolStripSeparator9 = new ToolStripSeparator();
            CharacterToolStripMenuItem = new ToolStripMenuItem();
            setEnemyRelativeLevelToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem11 = new ToolStripMenuItem();
            toolStripMenuItem12 = new ToolStripMenuItem();
            toolStripMenuItem13 = new ToolStripMenuItem();
            toolStripMenuItem14 = new ToolStripMenuItem();
            defaultToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem15 = new ToolStripMenuItem();
            toolStripMenuItem16 = new ToolStripMenuItem();
            toolStripMenuItem17 = new ToolStripMenuItem();
            toolStripMenuItem18 = new ToolStripMenuItem();
            toolStripMenuItem19 = new ToolStripMenuItem();
            toolStripMenuItem20 = new ToolStripMenuItem();
            toolStripMenuItem21 = new ToolStripMenuItem();
            ToolStripSeparator16 = new ToolStripSeparator();
            SetAllIOsToDefault35ToolStripMenuItem = new ToolStripMenuItem();
            tsIODefault = new ToolStripMenuItem();
            ToolStripSeparator11 = new ToolStripSeparator();
            tsIOMin = new ToolStripMenuItem();
            tsIOMax = new ToolStripMenuItem();
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
            ToolStripSeparator28 = new ToolStripSeparator();
            SlotsToolStripMenuItem = new ToolStripMenuItem();
            tsFlipAllEnh = new ToolStripMenuItem();
            ToolStripSeparator4 = new ToolStripSeparator();
            tsClearAllEnh = new ToolStripMenuItem();
            tsRemoveAllSlots = new ToolStripMenuItem();
            ToolStripSeparator1 = new ToolStripSeparator();
            AutoArrangeAllSlotsToolStripMenuItem = new ToolStripMenuItem();
            ViewToolStripMenuItem = new ToolStripMenuItem();
            layoutMenuItem = new ToolStripMenuItem();
            tsView2Col = new ToolStripMenuItem();
            tsView3Col = new ToolStripMenuItem();
            tsView4Col = new ToolStripMenuItem();
            themeMenuItem = new ToolStripMenuItem();
            toolStripSeparator15 = new ToolStripSeparator();
            ToolStripSeparator13 = new ToolStripSeparator();
            toolStripMenuItem4 = new ToolStripMenuItem();
            tsViewIOLevels = new ToolStripMenuItem();
            tsViewSOLevels = new ToolStripMenuItem();
            tsViewRelative = new ToolStripMenuItem();
            tsViewSlotLevels = new ToolStripMenuItem();
            tsViewRelativeAsSigns = new ToolStripMenuItem();
            ToolStripSeparator2 = new ToolStripSeparator();
            toolStripMenuItem5 = new ToolStripMenuItem();
            tsViewActualDamage_New = new ToolStripMenuItem();
            tsViewDPS_New = new ToolStripMenuItem();
            tlsDPA = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            ToggleCheckModeToolStripMenuItem = new ToolStripMenuItem();
            ShareToolStripMenuItem = new ToolStripMenuItem();
            tsShareMenu = new ToolStripMenuItem();
            ToolStripSeparator24 = new ToolStripSeparator();
            toolStripMenuItem6 = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            importBuildFromToolStripMenuItem = new ToolStripMenuItem();
            tsImportLegacyForumPost = new ToolStripMenuItem();
            tsImportDataChunk = new ToolStripMenuItem();
            ToolStripSeparator27 = new ToolStripSeparator();
            tsViewSharedBuilds = new ToolStripMenuItem();
            WindowToolStripMenuItem = new ToolStripMenuItem();
            tsViewSets = new ToolStripMenuItem();
            tsViewGraphs = new ToolStripMenuItem();
            tsViewSetCompare = new ToolStripMenuItem();
            tsViewData = new ToolStripMenuItem();
            tsSetFind = new ToolStripMenuItem();
            ToolStripSeparator18 = new ToolStripSeparator();
            tsRecipeViewer = new ToolStripMenuItem();
            tsRotationHelper = new ToolStripMenuItem();
            ToolStripSeparator19 = new ToolStripSeparator();
            InGameRespecHelperToolStripMenuItem = new ToolStripMenuItem();
            tsHelperShort = new ToolStripMenuItem();
            tsHelperLong = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            tsChangeDb = new ToolStripMenuItem();
            tsConfig = new ToolStripMenuItem();
            tsAdvDBEdit = new ToolStripMenuItem();
            HelpToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem7 = new ToolStripMenuItem();
            toolStripMenuItem8 = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            tsUpdateCheck = new ToolStripMenuItem();
            ToolStripSeparator10 = new ToolStripSeparator();
            toolStripMenuItem9 = new ToolStripMenuItem();
            tsSupport = new ToolStripMenuItem();
            tsGitHub = new ToolStripMenuItem();
            ToolStripSeparator31 = new ToolStripSeparator();
            tsAbout = new ToolStripMenuItem();
            supportIconMenuItem = new FontAwesome.Sharp.IconMenuItem();
            kofiIconMenuItem = new FontAwesome.Sharp.IconMenuItem();
            patreonIconMenuItem = new FontAwesome.Sharp.IconMenuItem();
            DlgOpen = new OpenFileDialog();
            DlgSave = new SaveFileDialog();
            tmrGfx = new Timer(components);
            tTip = new ToolTip(components);
            btnMinimize = new FontAwesome.Sharp.IconButton();
            btnMaximize = new FontAwesome.Sharp.IconButton();
            btnClose = new FontAwesome.Sharp.IconButton();
            titlePanel = new Panel();
            logoPanel = new MidsLogoPanel();
            titleLabel = new Label();
            mainLayoutPanel.SuspendLayout();
            characterLayoutPanel.SuspendLayout();
            leftLayoutPanel.SuspendLayout();
            rightLayoutPanel.SuspendLayout();
            buttonsLayoutPanel.SuspendLayout();
            canvasScrollPanel.ContentPanel.SuspendLayout();
            canvasScrollPanel.SuspendLayout();
            characterPanel.SuspendLayout();
            midsvScrollPanel1.ContentPanel.SuspendLayout();
            midsvScrollPanel1.SuspendLayout();
            rightInnerLayoutPanel.SuspendLayout();
            leftInnerLayoutPanel.SuspendLayout();
            footerPanel.SuspendLayout();
            footerLayoutPanel.SuspendLayout();
            footerRightPanel.SuspendLayout();
            MenuBar.SuspendLayout();
            titlePanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.BackColor = Color.Black;
            mainLayoutPanel.ColumnCount = 2;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 625F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(characterLayoutPanel, 0, 0);
            mainLayoutPanel.Controls.Add(leftLayoutPanel, 0, 1);
            mainLayoutPanel.Controls.Add(rightLayoutPanel, 1, 1);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(10, 71);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 2;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Size = new Size(1260, 649);
            mainLayoutPanel.TabIndex = 0;
            // 
            // characterLayoutPanel
            // 
            characterLayoutPanel.BackColor = Color.Black;
            characterLayoutPanel.ColumnCount = 9;
            mainLayoutPanel.SetColumnSpan(characterLayoutPanel, 2);
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            characterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            characterLayoutPanel.Controls.Add(lblName, 0, 0);
            characterLayoutPanel.Controls.Add(txtName, 1, 0);
            characterLayoutPanel.Controls.Add(lblAT, 2, 0);
            characterLayoutPanel.Controls.Add(atDropDown, 3, 0);
            characterLayoutPanel.Controls.Add(lblOrigin, 4, 0);
            characterLayoutPanel.Controls.Add(originDropDown, 5, 0);
            characterLayoutPanel.Controls.Add(modeEx, 6, 0);
            characterLayoutPanel.Controls.Add(totalsEx, 7, 0);
            characterLayoutPanel.Controls.Add(combatEx, 8, 0);
            characterLayoutPanel.Dock = DockStyle.Fill;
            characterLayoutPanel.Location = new Point(3, 3);
            characterLayoutPanel.Name = "characterLayoutPanel";
            characterLayoutPanel.RowCount = 1;
            characterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            characterLayoutPanel.Size = new Size(1254, 40);
            characterLayoutPanel.TabIndex = 0;
            // 
            // leftLayoutPanel
            // 
            leftLayoutPanel.BackColor = Color.Black;
            leftLayoutPanel.ColumnCount = 2;
            leftLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leftLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 194F));
            leftLayoutPanel.Controls.Add(midsvScrollPanel1, 1, 0);
            leftLayoutPanel.Controls.Add(leftInnerLayoutPanel, 0, 0);
            leftLayoutPanel.Dock = DockStyle.Fill;
            leftLayoutPanel.FlexibleRow = 1;
            leftLayoutPanel.ForeColor = Color.WhiteSmoke;
            leftLayoutPanel.Location = new Point(3, 49);
            leftLayoutPanel.Margin = new Padding(3, 3, 0, 0);
            leftLayoutPanel.Name = "leftLayoutPanel";
            leftLayoutPanel.PreferredGrowRow = 0;
            leftLayoutPanel.RowCount = 2;
            leftLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            leftLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftLayoutPanel.Size = new Size(619, 597);
            leftLayoutPanel.TabIndex = 1;
            // 
            // rightLayoutPanel
            // 
            rightLayoutPanel.BackColor = Color.Black;
            rightLayoutPanel.ColumnCount = 1;
            rightLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightLayoutPanel.Controls.Add(buttonsLayoutPanel, 0, 0);
            rightLayoutPanel.Controls.Add(canvasScrollPanel, 0, 1);
            rightLayoutPanel.Dock = DockStyle.Fill;
            rightLayoutPanel.Location = new Point(625, 49);
            rightLayoutPanel.Margin = new Padding(0, 3, 0, 0);
            rightLayoutPanel.Name = "rightLayoutPanel";
            rightLayoutPanel.RowCount = 2;
            rightLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            rightLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightLayoutPanel.Size = new Size(629, 597);
            rightLayoutPanel.TabIndex = 2;
            // 
            // buttonsLayoutPanel
            // 
            buttonsLayoutPanel.BackColor = Color.FromArgb(7, 15, 24);
            buttonsLayoutPanel.ColumnCount = 6;
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.Controls.Add(dynMode, 0, 0);
            buttonsLayoutPanel.Controls.Add(pvXEx, 1, 0);
            buttonsLayoutPanel.Controls.Add(accoladesEx, 2, 0);
            buttonsLayoutPanel.Controls.Add(incarnatesEx, 3, 0);
            buttonsLayoutPanel.Controls.Add(ibPrestigePowersEx, 4, 0);
            buttonsLayoutPanel.Controls.Add(tempPowersEx, 5, 0);
            buttonsLayoutPanel.Dock = DockStyle.Fill;
            buttonsLayoutPanel.ForeColor = Color.WhiteSmoke;
            buttonsLayoutPanel.Location = new Point(3, 3);
            buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            buttonsLayoutPanel.RowCount = 1;
            buttonsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            buttonsLayoutPanel.Size = new Size(623, 34);
            buttonsLayoutPanel.TabIndex = 46;
            // 
            // canvasScrollPanel
            // 
            canvasScrollPanel.BackColor = Color.FromArgb(7, 15, 24);
            canvasScrollPanel.ContentPanel.BackColor = Color.FromArgb(7, 15, 24);
            canvasScrollPanel.ContentPanel.Controls.Add(canvas);
            canvasScrollPanel.ContentPanel.Location = new Point(0, 0);
            canvasScrollPanel.ContentPanel.Margin = new Padding(0);
            canvasScrollPanel.ContentPanel.Name = "ContentPanel";
            canvasScrollPanel.ContentPanel.Size = new Size(610, 620);
            canvasScrollPanel.ContentPanel.TabIndex = 0;
            canvasScrollPanel.Dock = DockStyle.Fill;
            canvasScrollPanel.ForeColor = Color.WhiteSmoke;
            canvasScrollPanel.Location = new Point(3, 43);
            canvasScrollPanel.Margin = new Padding(3, 3, 0, 0);
            canvasScrollPanel.MinimumSize = new Size(64, 64);
            canvasScrollPanel.Name = "canvasScrollPanel";
            canvasScrollPanel.Size = new Size(626, 554);
            canvasScrollPanel.TabIndex = 114;
            canvasScrollPanel.TabStop = true;
            // 
            // canvas
            // 
            canvas.BackColor = Color.FromArgb(7, 15, 24);
            canvas.Dock = DockStyle.Top;
            canvas.Location = new Point(0, 0);
            canvas.Name = "canvas";
            canvas.Size = new Size(610, 610);
            canvas.TabIndex = 0;
            canvas.Text = "midsBufferedImagePanel1";
            canvas.DragDrop += Canvas_DragDrop;
            canvas.DragEnter += Canvas_DragEnter;
            canvas.DragOver += Canvas_DragOver;
            canvas.MouseDoubleClick += Canvas_MouseDoubleClick;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseLeave += Canvas_MouseLeave;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            // 
            // dynMode
            // 
            dynMode.BackColor = Color.Transparent;
            dynMode.BackgroundImageLayout = ImageLayout.None;
            dynMode.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            dynMode.CornerRadius = 6;
            dynMode.Dock = DockStyle.Fill;
            dynMode.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            dynMode.ForeColor = Color.White;
            dynMode.Location = new Point(3, 4);
            dynMode.Margin = new Padding(3, 4, 3, 4);
            dynMode.Name = "dynMode";
            dynMode.Size = new Size(97, 26);
            dynMode.TabIndex = 161;
            dynMode.Text = "ibDynMode1";
            dynMode.ToggleText.Indeterminate = "Power / Slot";
            dynMode.ToggleText.ToggledOff = "Power";
            dynMode.ToggleText.ToggledOn = "Slot";
            dynMode.Click += DynMode_Click;
            // 
            // lblName
            // 
            lblName.Dock = DockStyle.Fill;
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblName.ForeColor = Color.White;
            lblName.Location = new Point(3, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(46, 40);
            lblName.TabIndex = 147;
            lblName.Text = "Name:";
            lblName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblAT
            // 
            lblAT.Dock = DockStyle.Fill;
            lblAT.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAT.ForeColor = Color.White;
            lblAT.Location = new Point(295, 0);
            lblAT.Name = "lblAT";
            lblAT.Size = new Size(72, 40);
            lblAT.TabIndex = 148;
            lblAT.Text = "Archetype:";
            lblAT.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            lblOrigin.Dock = DockStyle.Fill;
            lblOrigin.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblOrigin.ForeColor = Color.White;
            lblOrigin.Location = new Point(535, 0);
            lblOrigin.Name = "lblOrigin";
            lblOrigin.Size = new Size(52, 40);
            lblOrigin.TabIndex = 149;
            lblOrigin.Text = "Origin:";
            lblOrigin.TextAlign = ContentAlignment.MiddleRight;
            // 
            // characterPanel
            // 
            characterPanel.Location = new Point(0, 0);
            characterPanel.Name = "characterPanel";
            characterPanel.Size = new Size(0, 0);
            characterPanel.TabIndex = 150;
            characterPanel.Visible = false;
            // 
            // originDropDown
            // 
            originDropDown.BackColor = Color.WhiteSmoke;
            originDropDown.Dock = DockStyle.Fill;
            originDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            originDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            originDropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            originDropDown.ForeColor = Color.Black;
            originDropDown.IconProvider = null;
            originDropDown.ItemHeight = 17;
            originDropDown.Location = new Point(593, 7);
            originDropDown.Margin = new Padding(3, 7, 3, 7);
            originDropDown.Name = "originDropDown";
            originDropDown.PlaceholderText = null;
            originDropDown.Size = new Size(134, 23);
            originDropDown.TabIndex = 146;
            originDropDown.SelectedIndexChanged += OriginDropDown_SelectedIndexChanged;
            // 
            // atDropDown
            // 
            atDropDown.BackColor = Color.WhiteSmoke;
            atDropDown.DisplayMember = "DisplayName";
            atDropDown.Dock = DockStyle.Fill;
            atDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            atDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            atDropDown.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            atDropDown.ForeColor = Color.Black;
            atDropDown.IconProvider = null;
            atDropDown.ItemHeight = 17;
            atDropDown.Items.AddRange(new object[] { "Peacebringer", "Arachnos Widow", "Arachnos Soldier" });
            atDropDown.Location = new Point(373, 7);
            atDropDown.Margin = new Padding(3, 7, 3, 7);
            atDropDown.MaxDropDownItems = 15;
            atDropDown.Name = "atDropDown";
            atDropDown.PlaceholderText = null;
            atDropDown.SelectedItem = null;
            atDropDown.Size = new Size(154, 23);
            atDropDown.TabIndex = 145;
            atDropDown.ValueMember = "Idx";
            atDropDown.SelectedIndexChanged += AtDropDown_SelectedIndexChanged;
            // 
            // txtName
            // 
            txtName.BackColor = Color.WhiteSmoke;
            txtName.Dock = DockStyle.Fill;
            txtName.ForeColor = Color.Black;
            txtName.Location = new Point(55, 8);
            txtName.Margin = new Padding(3, 8, 3, 8);
            txtName.Name = "txtName";
            txtName.Size = new Size(234, 23);
            txtName.TabIndex = 144;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // modeEx
            // 
            modeEx.BackColor = Color.Transparent;
            modeEx.BackgroundImageLayout = ImageLayout.None;
            modeEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            modeEx.CornerRadius = 6;
            modeEx.Dock = DockStyle.Fill;
            modeEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            modeEx.ForeColor = Color.White;
            modeEx.Location = new Point(731, 4);
            modeEx.Margin = new Padding(4);
            modeEx.Name = "modeEx";
            modeEx.Size = new Size(168, 32);
            modeEx.TabIndex = 154;
            modeEx.Text = "ibModeEx1";
            modeEx.ThreeState = false;
            modeEx.ToggleText.Indeterminate = "Respec";
            modeEx.ToggleText.ToggledOff = "Level-Up";
            modeEx.ToggleText.ToggledOn = "Respec";
            tTip.SetToolTip(modeEx, "Build Mode");
            modeEx.Click += ibModeEx_OnClick;
            // 
            // totalsEx
            // 
            totalsEx.BackColor = Color.Transparent;
            totalsEx.BackgroundImageLayout = ImageLayout.None;
            totalsEx.CornerRadius = 6;
            totalsEx.Dock = DockStyle.Fill;
            totalsEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            totalsEx.Location = new Point(907, 4);
            totalsEx.Margin = new Padding(4);
            totalsEx.Name = "totalsEx";
            totalsEx.Size = new Size(168, 32);
            totalsEx.TabIndex = 157;
            totalsEx.Text = "Advanced Totals";
            totalsEx.ToggleText.Indeterminate = "Indeterminate State";
            totalsEx.ToggleText.ToggledOff = "Toggled Off State";
            totalsEx.ToggleText.ToggledOn = "Toggled On State";
            tTip.SetToolTip(totalsEx, "View Advanced Totals");
            totalsEx.Click += TotalsEx_OnClick;
            // 
            // slotInfoEx
            // 
            slotInfoEx.BackColor = Color.Transparent;
            slotInfoEx.BackgroundImageLayout = ImageLayout.None;
            slotInfoEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            slotInfoEx.CornerRadius = 6;
            slotInfoEx.Dock = DockStyle.Fill;
            slotInfoEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            slotInfoEx.Location = new Point(454, 72);
            slotInfoEx.Margin = new Padding(3, 4, 3, 4);
            slotInfoEx.Name = "slotInfoEx";
            slotInfoEx.Size = new Size(156, 28);
            slotInfoEx.TabIndex = 158;
            slotInfoEx.Text = "ibSlotInfoEx";
            slotInfoEx.ToggleText.Indeterminate = "Indeterminate State";
            slotInfoEx.ToggleText.ToggledOff = "X Slots to go";
            slotInfoEx.ToggleText.ToggledOn = "X Slots placed";
            // 
            // setsEx
            // 
            setsEx.BackColor = Color.Transparent;
            setsEx.BackgroundImageLayout = ImageLayout.None;
            setsEx.Dock = DockStyle.Fill;
            setsEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            setsEx.Location = new Point(293, 38);
            setsEx.Margin = new Padding(3, 4, 3, 4);
            setsEx.Name = "setsEx";
            setsEx.Size = new Size(155, 26);
            setsEx.TabIndex = 159;
            setsEx.Text = "View Active Sets";
            setsEx.ToggleText.Indeterminate = "Indeterminate State";
            setsEx.ToggleText.ToggledOff = "Toggled Off State";
            setsEx.ToggleText.ToggledOn = "Toggled On State";
            // 
            // slotLevelsEx
            // 
            slotLevelsEx.BackColor = Color.Transparent;
            slotLevelsEx.BackgroundImageLayout = ImageLayout.None;
            slotLevelsEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            slotLevelsEx.Dock = DockStyle.Fill;
            slotLevelsEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            slotLevelsEx.Location = new Point(293, 72);
            slotLevelsEx.Margin = new Padding(3, 4, 3, 4);
            slotLevelsEx.Name = "slotLevelsEx";
            slotLevelsEx.Size = new Size(155, 28);
            slotLevelsEx.TabIndex = 160;
            slotLevelsEx.Text = "ibSlotLevelsEx";
            slotLevelsEx.ToggleText.Indeterminate = "Indeterminate State";
            slotLevelsEx.ToggleText.ToggledOff = "Slot Levels: Off";
            slotLevelsEx.ToggleText.ToggledOn = "Slot Levels: On";
            // 
            // midsvScrollPanel1
            // 
            // 
            // midsvScrollPanel1.ContentPanel
            // 
            midsvScrollPanel1.BackColor = Color.FromArgb(7, 15, 24);
            midsvScrollPanel1.ContentPanel.BackColor = Color.FromArgb(7, 15, 24);
            midsvScrollPanel1.ContentPanel.Controls.Add(rightInnerLayoutPanel);
            midsvScrollPanel1.ContentPanel.Location = new Point(0, 0);
            midsvScrollPanel1.ContentPanel.Margin = new Padding(0);
            midsvScrollPanel1.ContentPanel.Name = "ContentPanel";
            midsvScrollPanel1.ContentPanel.Size = new Size(187, 840);
            midsvScrollPanel1.ContentPanel.TabIndex = 0;
            midsvScrollPanel1.Dock = DockStyle.Fill;
            midsvScrollPanel1.Location = new Point(429, 3);
            midsvScrollPanel1.Margin = new Padding(1, 3, 0, 3);
            midsvScrollPanel1.MinimumSize = new Size(64, 64);
            midsvScrollPanel1.Name = "midsvScrollPanel1";
            leftLayoutPanel.SetRowSpan(midsvScrollPanel1, 2);
            midsvScrollPanel1.Size = new Size(191, 588);
            midsvScrollPanel1.TabIndex = 4;
            midsvScrollPanel1.TabStop = true;
            // 
            // rightInnerLayoutPanel
            // 
            rightInnerLayoutPanel.BackColor = Color.FromArgb(7, 15, 24);
            rightInnerLayoutPanel.AutoSize = true;
            rightInnerLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            rightInnerLayoutPanel.ColumnCount = 1;
            rightInnerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightInnerLayoutPanel.Controls.Add(ancillaryList, 0, 14);
            rightInnerLayoutPanel.Controls.Add(ancillaryDropDown, 0, 13);
            rightInnerLayoutPanel.Controls.Add(ancillaryLabel, 0, 12);
            rightInnerLayoutPanel.Controls.Add(pool3List, 0, 11);
            rightInnerLayoutPanel.Controls.Add(pool3DropDown, 0, 10);
            rightInnerLayoutPanel.Controls.Add(pool3Label, 0, 9);
            rightInnerLayoutPanel.Controls.Add(pool2List, 0, 8);
            rightInnerLayoutPanel.Controls.Add(pool2DropDown, 0, 7);
            rightInnerLayoutPanel.Controls.Add(pool2Label, 0, 6);
            rightInnerLayoutPanel.Controls.Add(pool1List, 0, 5);
            rightInnerLayoutPanel.Controls.Add(pool1DropDown, 0, 4);
            rightInnerLayoutPanel.Controls.Add(pool1Label, 0, 3);
            rightInnerLayoutPanel.Controls.Add(pool0DropDown, 0, 1);
            rightInnerLayoutPanel.Controls.Add(pool0Label, 0, 0);
            rightInnerLayoutPanel.Controls.Add(pool0List, 0, 2);
            rightInnerLayoutPanel.Location = new Point(0, 0);
            rightInnerLayoutPanel.Name = "rightInnerLayoutPanel";
            rightInnerLayoutPanel.Padding = new Padding(0, 0, 4, 0);
            rightInnerLayoutPanel.RowCount = 15;
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 114F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 114F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 114F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 114F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            rightInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 114F));
            rightInnerLayoutPanel.Size = new Size(195, 830);
            rightInnerLayoutPanel.TabIndex = 0;
            // 
            // ancillaryList
            // 
            ancillaryList.DesignerHideHeadings = true;
            ancillaryList.DesignerItemCount = 5;
            ancillaryList.Dock = DockStyle.Fill;
            ancillaryList.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            ancillaryList.HoverColor = Color.FromArgb(40, 40, 40);
            ancillaryList.LineSpacing = -2;
            ancillaryList.Location = new Point(3, 713);
            ancillaryList.Name = "ancillaryList";
            ancillaryList.PaddingX = 2;
            ancillaryList.PaddingY = 0;
            ancillaryList.Scrollable = false;
            ancillaryList.ScrollBarWidth = 10;
            ancillaryList.Size = new Size(181, 114);
            ancillaryList.TabIndex = 165;
            ancillaryList.TextWrapMode = WordwrapMode.New;
            ancillaryList.ItemClicked += AncillaryList_ItemClicked;
            ancillaryList.ItemHovered += MListView_ItemHovered;
            // 
            // ancillaryDropDown
            // 
            ancillaryDropDown.Dock = DockStyle.Fill;
            ancillaryDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            ancillaryDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            ancillaryDropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            ancillaryDropDown.FormattingEnabled = true;
            ancillaryDropDown.IconProvider = null;
            ancillaryDropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            ancillaryDropDown.Location = new Point(3, 687);
            ancillaryDropDown.Name = "ancillaryDropDown";
            ancillaryDropDown.PlaceholderText = null;
            ancillaryDropDown.SelectedItem = null;
            ancillaryDropDown.Size = new Size(181, 26);
            ancillaryDropDown.TabIndex = 164;
            ancillaryDropDown.SelectedIndexChanged += AncillaryDropDown_SelectedIndexChanged;
            // 
            // ancillaryLabel
            // 
            ancillaryLabel.BackColor = Color.Transparent;
            ancillaryLabel.Dock = DockStyle.Fill;
            ancillaryLabel.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            ancillaryLabel.ForeColor = Color.White;
            ancillaryLabel.Location = new Point(3, 664);
            ancillaryLabel.Name = "ancillaryLabel";
            ancillaryLabel.Size = new Size(181, 20);
            ancillaryLabel.TabIndex = 163;
            ancillaryLabel.Text = "Ancillary/Epic Pool";
            ancillaryLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pool3List
            // 
            pool3List.DesignerHideHeadings = true;
            pool3List.DesignerItemCount = 5;
            pool3List.Dock = DockStyle.Fill;
            pool3List.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            pool3List.HoverColor = Color.FromArgb(40, 40, 40);
            pool3List.LineSpacing = -2;
            pool3List.Location = new Point(3, 547);
            pool3List.Name = "pool3List";
            pool3List.PaddingX = 2;
            pool3List.PaddingY = 0;
            pool3List.Scrollable = false;
            pool3List.ScrollBarWidth = 10;
            pool3List.Size = new Size(181, 114);
            pool3List.TabIndex = 162;
            pool3List.TextWrapMode = WordwrapMode.New;
            pool3List.ItemClicked += Pool3List_ItemClicked;
            pool3List.ItemHovered += MListView_ItemHovered;
            // 
            // pool3DropDown
            // 
            pool3DropDown.Dock = DockStyle.Fill;
            pool3DropDown.DrawMode = DrawMode.OwnerDrawFixed;
            pool3DropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            pool3DropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            pool3DropDown.FormattingEnabled = true;
            pool3DropDown.IconProvider = null;
            pool3DropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            pool3DropDown.Location = new Point(3, 521);
            pool3DropDown.Name = "pool3DropDown";
            pool3DropDown.PlaceholderText = null;
            pool3DropDown.SelectedItem = null;
            pool3DropDown.Size = new Size(181, 26);
            pool3DropDown.TabIndex = 161;
            pool3DropDown.SelectedIndexChanged += PoolsDropDown_SelectedIndexChanged;
            // 
            // pool3Label
            // 
            pool3Label.BackColor = Color.Transparent;
            pool3Label.Dock = DockStyle.Fill;
            pool3Label.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            pool3Label.ForeColor = Color.White;
            pool3Label.Location = new Point(3, 498);
            pool3Label.Name = "pool3Label";
            pool3Label.Size = new Size(181, 20);
            pool3Label.TabIndex = 160;
            pool3Label.Text = "Pool 4";
            pool3Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pool2List
            // 
            pool2List.DesignerHideHeadings = true;
            pool2List.DesignerItemCount = 5;
            pool2List.Dock = DockStyle.Fill;
            pool2List.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            pool2List.HoverColor = Color.FromArgb(40, 40, 40);
            pool2List.LineSpacing = -2;
            pool2List.Location = new Point(3, 381);
            pool2List.Name = "pool2List";
            pool2List.PaddingX = 2;
            pool2List.PaddingY = 0;
            pool2List.Scrollable = false;
            pool2List.ScrollBarWidth = 10;
            pool2List.Size = new Size(181, 114);
            pool2List.TabIndex = 159;
            pool2List.TextWrapMode = WordwrapMode.New;
            pool2List.ItemClicked += Pool2List_ItemClicked;
            pool2List.ItemHovered += MListView_ItemHovered;
            // 
            // pool2DropDown
            // 
            pool2DropDown.Dock = DockStyle.Fill;
            pool2DropDown.DrawMode = DrawMode.OwnerDrawFixed;
            pool2DropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            pool2DropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            pool2DropDown.FormattingEnabled = true;
            pool2DropDown.IconProvider = null;
            pool2DropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            pool2DropDown.Location = new Point(3, 355);
            pool2DropDown.Name = "pool2DropDown";
            pool2DropDown.PlaceholderText = null;
            pool2DropDown.SelectedItem = null;
            pool2DropDown.Size = new Size(181, 26);
            pool2DropDown.TabIndex = 158;
            pool2DropDown.SelectedIndexChanged += PoolsDropDown_SelectedIndexChanged;
            // 
            // pool2Label
            // 
            pool2Label.BackColor = Color.Transparent;
            pool2Label.Dock = DockStyle.Fill;
            pool2Label.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            pool2Label.ForeColor = Color.White;
            pool2Label.Location = new Point(3, 332);
            pool2Label.Name = "pool2Label";
            pool2Label.Size = new Size(181, 20);
            pool2Label.TabIndex = 157;
            pool2Label.Text = "Pool 3";
            pool2Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pool1List
            // 
            pool1List.DesignerHideHeadings = true;
            pool1List.DesignerItemCount = 5;
            pool1List.Dock = DockStyle.Fill;
            pool1List.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            pool1List.HoverColor = Color.FromArgb(40, 40, 40);
            pool1List.LineSpacing = -2;
            pool1List.Location = new Point(3, 215);
            pool1List.Name = "pool1List";
            pool1List.PaddingX = 2;
            pool1List.PaddingY = 0;
            pool1List.Scrollable = false;
            pool1List.ScrollBarWidth = 10;
            pool1List.Size = new Size(181, 114);
            pool1List.TabIndex = 156;
            pool1List.TextWrapMode = WordwrapMode.New;
            pool1List.ItemClicked += Pool1List_ItemClicked;
            pool1List.ItemHovered += MListView_ItemHovered;
            // 
            // pool1DropDown
            // 
            pool1DropDown.Dock = DockStyle.Fill;
            pool1DropDown.DrawMode = DrawMode.OwnerDrawFixed;
            pool1DropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            pool1DropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            pool1DropDown.FormattingEnabled = true;
            pool1DropDown.IconProvider = null;
            pool1DropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            pool1DropDown.Location = new Point(3, 189);
            pool1DropDown.Name = "pool1DropDown";
            pool1DropDown.PlaceholderText = null;
            pool1DropDown.SelectedItem = null;
            pool1DropDown.Size = new Size(181, 26);
            pool1DropDown.TabIndex = 155;
            pool1DropDown.SelectedIndexChanged += PoolsDropDown_SelectedIndexChanged;
            // 
            // pool1Label
            // 
            pool1Label.BackColor = Color.Transparent;
            pool1Label.Dock = DockStyle.Fill;
            pool1Label.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            pool1Label.ForeColor = Color.White;
            pool1Label.Location = new Point(3, 166);
            pool1Label.Name = "pool1Label";
            pool1Label.Size = new Size(181, 20);
            pool1Label.TabIndex = 154;
            pool1Label.Text = "Pool 2";
            pool1Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pool0DropDown
            // 
            pool0DropDown.Dock = DockStyle.Fill;
            pool0DropDown.DrawMode = DrawMode.OwnerDrawFixed;
            pool0DropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            pool0DropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            pool0DropDown.FormattingEnabled = true;
            pool0DropDown.IconProvider = null;
            pool0DropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            pool0DropDown.Location = new Point(3, 23);
            pool0DropDown.Name = "pool0DropDown";
            pool0DropDown.PlaceholderText = null;
            pool0DropDown.SelectedItem = null;
            pool0DropDown.Size = new Size(181, 26);
            pool0DropDown.TabIndex = 153;
            pool0DropDown.SelectedIndexChanged += PoolsDropDown_SelectedIndexChanged;
            // 
            // pool0Label
            // 
            pool0Label.BackColor = Color.Transparent;
            pool0Label.Dock = DockStyle.Fill;
            pool0Label.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            pool0Label.ForeColor = Color.White;
            pool0Label.Location = new Point(3, 0);
            pool0Label.Name = "pool0Label";
            pool0Label.Size = new Size(181, 20);
            pool0Label.TabIndex = 150;
            pool0Label.Text = "Pool 1";
            pool0Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pool0List
            // 
            pool0List.DesignerHideHeadings = true;
            pool0List.DesignerItemCount = 5;
            pool0List.Dock = DockStyle.Fill;
            pool0List.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            pool0List.HoverColor = Color.FromArgb(40, 40, 40);
            pool0List.LineSpacing = -2;
            pool0List.Location = new Point(3, 49);
            pool0List.Name = "pool0List";
            pool0List.PaddingX = 2;
            pool0List.PaddingY = 0;
            pool0List.Scrollable = false;
            pool0List.ScrollBarWidth = 10;
            pool0List.Size = new Size(181, 114);
            pool0List.TabIndex = 152;
            pool0List.TextWrapMode = WordwrapMode.New;
            pool0List.ItemClicked += Pool0List_ItemClicked;
            pool0List.ItemHovered += MListView_ItemHovered;
            // 
            // leftInnerLayoutPanel
            // 
            leftInnerLayoutPanel.BackColor = Color.FromArgb(7, 15, 24);
            leftInnerLayoutPanel.ColumnCount = 2;
            leftInnerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftInnerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            leftInnerLayoutPanel.Controls.Add(dataView, 0, 3);
            leftInnerLayoutPanel.Controls.Add(secondaryList, 1, 2);
            leftInnerLayoutPanel.Controls.Add(secondaryDropDown, 1, 1);
            leftInnerLayoutPanel.Controls.Add(label1, 1, 0);
            leftInnerLayoutPanel.Controls.Add(lblPrimary, 0, 0);
            leftInnerLayoutPanel.Controls.Add(primaryList, 0, 2);
            leftInnerLayoutPanel.Controls.Add(primaryDropDown, 0, 1);
            leftInnerLayoutPanel.Dock = DockStyle.Fill;
            leftInnerLayoutPanel.Location = new Point(3, 3);
            leftInnerLayoutPanel.Margin = new Padding(3, 3, 1, 3);
            leftInnerLayoutPanel.Name = "leftInnerLayoutPanel";
            leftInnerLayoutPanel.RowCount = 4;
            leftLayoutPanel.SetRowSpan(leftInnerLayoutPanel, 2);
            leftInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            leftInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            leftInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            leftInnerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 66.6666641F));
            leftInnerLayoutPanel.Size = new Size(422, 591);
            leftInnerLayoutPanel.TabIndex = 5;
            // 
            // dataView
            // 
            dataView.BackColor = Color.FromArgb(1, 7, 15);
            leftInnerLayoutPanel.SetColumnSpan(dataView, 2);
            dataView.Dock = DockStyle.Fill;
            dataView.Font = new Font("Segoe UI", 9F);
            dataView.ForeColor = Color.FromArgb(248, 241, 212);
            dataView.IsLocked = false;
            dataView.Location = new Point(3, 222);
            dataView.Name = "dataView";
            dataView.Padding = new Padding(2, 0, 2, 2);
            dataView.Size = new Size(416, 366);
            dataView.TabIndex = 3;
            // 
            // secondaryList
            // 
            secondaryList.DesignerItemCount = 20;
            secondaryList.Dock = DockStyle.Fill;
            secondaryList.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            secondaryList.HoverColor = Color.FromArgb(40, 40, 40);
            secondaryList.LineSpacing = -2;
            secondaryList.Location = new Point(205, 49);
            secondaryList.Name = "secondaryList";
            secondaryList.PaddingX = 2;
            secondaryList.PaddingY = 0;
            secondaryList.Scrollable = true;
            secondaryList.ScrollBarWidth = 10;
            secondaryList.Size = new Size(205, 167);
            secondaryList.TabIndex = 153;
            secondaryList.TextWrapMode = WordwrapMode.New;
            secondaryList.ItemClicked += SecondaryList_ItemClicked;
            secondaryList.ItemHovered += MListView_ItemHovered;
            // 
            // secondaryDropDown
            // 
            secondaryDropDown.Dock = DockStyle.Fill;
            secondaryDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            secondaryDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            secondaryDropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            secondaryDropDown.FormattingEnabled = true;
            secondaryDropDown.IconProvider = null;
            secondaryDropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2" });
            secondaryDropDown.Location = new Point(205, 23);
            secondaryDropDown.Name = "secondaryDropDown";
            secondaryDropDown.PlaceholderText = null;
            secondaryDropDown.SelectedItem = null;
            secondaryDropDown.Size = new Size(205, 26);
            secondaryDropDown.TabIndex = 152;
            secondaryDropDown.SelectedIndexChanged += SecondaryDropDown_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(205, 0);
            label1.Name = "label1";
            label1.Size = new Size(205, 20);
            label1.TabIndex = 151;
            label1.Text = "Secondary Power Set";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPrimary
            // 
            lblPrimary.BackColor = Color.Transparent;
            lblPrimary.Dock = DockStyle.Fill;
            lblPrimary.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold);
            lblPrimary.ForeColor = Color.White;
            lblPrimary.Location = new Point(3, 0);
            lblPrimary.Name = "lblPrimary";
            lblPrimary.Size = new Size(205, 20);
            lblPrimary.TabIndex = 148;
            lblPrimary.Text = "Primary Power Set";
            lblPrimary.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // primaryList
            // 
            primaryList.DesignerItemCount = 25;
            primaryList.Dock = DockStyle.Fill;
            primaryList.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            primaryList.HoverColor = Color.FromArgb(50, 245, 245, 245);
            primaryList.LineSpacing = -2;
            primaryList.Location = new Point(3, 49);
            primaryList.Name = "primaryList";
            primaryList.PaddingX = 2;
            primaryList.PaddingY = 0;
            primaryList.Scrollable = true;
            primaryList.ScrollBarWidth = 10;
            primaryList.Size = new Size(196, 167);
            primaryList.TabIndex = 150;
            primaryList.TextWrapMode = WordwrapMode.New;
            primaryList.ItemClicked += PrimaryList_ItemClicked;
            primaryList.ItemHovered += MListView_ItemHovered;
            // 
            // primaryDropDown
            // 
            primaryDropDown.Dock = DockStyle.Fill;
            primaryDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            primaryDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            primaryDropDown.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            primaryDropDown.FormattingEnabled = true;
            primaryDropDown.IconProvider = null;
            primaryDropDown.Items.AddRange(new object[] { "Test Item #1", "Test Item #2", "Electricity Manipulation", "Fire Mastery", "Ice Control", "Wind Manipulation", "Earth Shaping", "Shadow Arts", "Light Weaving", "Time Distortion", "Gravity Mastery", "Sound Manipulation", "Telekinesis", "Mind Control", "Illusion Casting", "Water Mastery", "Metal Manipulation", "Nature's Wrath", "Poison Expertise", "Plasma Generation", "Magnetism", "Healing Arts", "Energy Absorption", "Force Field Projection", "Dimensional Travel", "Technomancy" });
            primaryDropDown.Location = new Point(3, 23);
            primaryDropDown.Name = "primaryDropDown";
            primaryDropDown.PlaceholderText = null;
            primaryDropDown.SelectedItem = null;
            primaryDropDown.Size = new Size(196, 26);
            primaryDropDown.TabIndex = 149;
            primaryDropDown.Tag = "";
            primaryDropDown.SelectedIndexChanged += PrimaryDropDown_SelectedIndexChanged;
            // 
            // buttonsLayoutPanel
            // 
            buttonsLayoutPanel.BackColor = Color.FromArgb(7, 15, 24);
            buttonsLayoutPanel.ColumnCount = 6;
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666666F));
            buttonsLayoutPanel.Controls.Add(dynMode, 0, 0);
            buttonsLayoutPanel.Controls.Add(pvXEx, 1, 0);
            buttonsLayoutPanel.Controls.Add(accoladesEx, 2, 0);
            buttonsLayoutPanel.Controls.Add(incarnatesEx, 3, 0);
            buttonsLayoutPanel.Controls.Add(ibPrestigePowersEx, 4, 0);
            buttonsLayoutPanel.Controls.Add(tempPowersEx, 5, 0);
            buttonsLayoutPanel.Dock = DockStyle.Fill;
            buttonsLayoutPanel.ForeColor = Color.WhiteSmoke;
            buttonsLayoutPanel.Location = new Point(3, 3);
            buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            buttonsLayoutPanel.RowCount = 1;
            buttonsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            buttonsLayoutPanel.Size = new Size(623, 34);
            buttonsLayoutPanel.TabIndex = 46;
            // 
            // combatEx
            // 
            combatEx.BackColor = Color.Transparent;
            combatEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            combatEx.CornerRadius = 6;
            combatEx.Dock = DockStyle.Fill;
            combatEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            combatEx.Location = new Point(1083, 4);
            combatEx.Margin = new Padding(4);
            combatEx.Name = "combatEx";
            combatEx.Size = new Size(167, 32);
            combatEx.TabIndex = 131;
            combatEx.Text = "Combat";
            combatEx.ToggleText.Indeterminate = "Combat";
            combatEx.ToggleText.ToggledOff = "Combat";
            combatEx.ToggleText.ToggledOn = "Combat";
            tTip.SetToolTip(combatEx, "Combat Context");
            // 
            // tempPowersEx
            // 
            tempPowersEx.BackColor = Color.Transparent;
            tempPowersEx.CornerRadius = 6;
            tempPowersEx.Dock = DockStyle.Fill;
            tempPowersEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            tempPowersEx.Location = new Point(523, 4);
            tempPowersEx.Margin = new Padding(3, 4, 3, 4);
            tempPowersEx.Name = "tempPowersEx";
            tempPowersEx.Size = new Size(97, 26);
            tempPowersEx.TabIndex = 130;
            tempPowersEx.Text = "Temps";
            tempPowersEx.ToggleText.Indeterminate = "Indeterminate State";
            tempPowersEx.ToggleText.ToggledOff = "Temp Powers: Off";
            tempPowersEx.ToggleText.ToggledOn = "Temp Powers: On";
            tTip.SetToolTip(tempPowersEx, "Temporary Powers");
            tempPowersEx.Click += TempPowersEx_OnClick;
            // 
            // ibPrestigePowersEx
            // 
            ibPrestigePowersEx.BackColor = Color.Transparent;
            ibPrestigePowersEx.CornerRadius = 6;
            ibPrestigePowersEx.Dock = DockStyle.Fill;
            ibPrestigePowersEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            ibPrestigePowersEx.Location = new Point(419, 4);
            ibPrestigePowersEx.Margin = new Padding(3, 4, 3, 4);
            ibPrestigePowersEx.Name = "ibPrestigePowersEx";
            ibPrestigePowersEx.Size = new Size(98, 26);
            ibPrestigePowersEx.TabIndex = 129;
            ibPrestigePowersEx.Text = "Prestige";
            ibPrestigePowersEx.ToggleText.Indeterminate = "Indeterminate State";
            ibPrestigePowersEx.ToggleText.ToggledOff = "ToggledOff State";
            ibPrestigePowersEx.ToggleText.ToggledOn = "ToggledOn State";
            tTip.SetToolTip(ibPrestigePowersEx, "Prestige Powers");
            ibPrestigePowersEx.Click += ibPrestigePowersEx_OnClick;
            // 
            // incarnatesEx
            // 
            incarnatesEx.BackColor = Color.Transparent;
            incarnatesEx.CornerRadius = 6;
            incarnatesEx.Dock = DockStyle.Fill;
            incarnatesEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            incarnatesEx.Location = new Point(315, 4);
            incarnatesEx.Margin = new Padding(3, 4, 3, 4);
            incarnatesEx.Name = "incarnatesEx";
            incarnatesEx.Size = new Size(98, 26);
            incarnatesEx.TabIndex = 128;
            incarnatesEx.Text = "Incarnates";
            incarnatesEx.ToggleText.Indeterminate = "Indeterminate State";
            incarnatesEx.ToggleText.ToggledOff = "Toggled Off State";
            incarnatesEx.ToggleText.ToggledOn = "Toggled On State";
            tTip.SetToolTip(incarnatesEx, "Incarnate Powers");
            incarnatesEx.Click += IncarnatesEx_OnClick;
            // 
            // accoladesEx
            // 
            accoladesEx.BackColor = Color.Transparent;
            accoladesEx.CornerRadius = 6;
            accoladesEx.Dock = DockStyle.Fill;
            accoladesEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            accoladesEx.Location = new Point(211, 4);
            accoladesEx.Margin = new Padding(3, 4, 3, 4);
            accoladesEx.Name = "accoladesEx";
            accoladesEx.Size = new Size(98, 26);
            accoladesEx.TabIndex = 127;
            accoladesEx.Text = "Accolades";
            accoladesEx.ToggleText.Indeterminate = "Indeterminate State";
            accoladesEx.ToggleText.ToggledOff = "Accolades: Off";
            accoladesEx.ToggleText.ToggledOn = "Accolades: On";
            tTip.SetToolTip(accoladesEx, "Accolade Powers");
            accoladesEx.Click += ibAccoladesEx_OnClick;
            // 
            // popupEx
            // 
            popupEx.BackColor = Color.Transparent;
            popupEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            popupEx.CornerRadius = 6;
            popupEx.Dock = DockStyle.Fill;
            popupEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            popupEx.Location = new Point(474, 4);
            popupEx.Margin = new Padding(3, 4, 3, 4);
            popupEx.Name = "popupEx";
            popupEx.Size = new Size(152, 25);
            popupEx.TabIndex = 126;
            popupEx.Text = "ibPopupEx";
            popupEx.ToggleText.Indeterminate = "Indeterminate State";
            popupEx.ToggleText.ToggledOff = "Popup: Off";
            popupEx.ToggleText.ToggledOn = "Popup: On";
            tTip.SetToolTip(popupEx, "Display Info Popups");
            popupEx.Visible = false;
            // 
            // pvXEx
            // 
            pvXEx.BackColor = Color.Transparent;
            pvXEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            pvXEx.CornerRadius = 6;
            pvXEx.Dock = DockStyle.Fill;
            pvXEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            pvXEx.Location = new Point(107, 4);
            pvXEx.Margin = new Padding(3, 4, 3, 4);
            pvXEx.Name = "pvXEx";
            pvXEx.Size = new Size(98, 26);
            pvXEx.TabIndex = 124;
            pvXEx.Text = "ibPvXEx";
            pvXEx.ToggleText.Indeterminate = "Indeterminate State";
            pvXEx.ToggleText.ToggledOff = "Mode: PvE";
            pvXEx.ToggleText.ToggledOn = "Mode: PvP";
            tTip.SetToolTip(pvXEx, "Mode");
            pvXEx.Click += PvXEx_OnClick;
            // 
            // recipeEx
            // 
            recipeEx.BackColor = Color.Transparent;
            recipeEx.ButtonType = MidsVectorButton.ButtonTypes.Toggle;
            recipeEx.CornerRadius = 6;
            recipeEx.Dock = DockStyle.Fill;
            recipeEx.Font = new Font("Noto Sans SemiBold", 9.75F, FontStyle.Bold);
            recipeEx.Location = new Point(317, 4);
            recipeEx.Margin = new Padding(3, 4, 3, 4);
            recipeEx.Name = "recipeEx";
            recipeEx.Size = new Size(151, 25);
            recipeEx.TabIndex = 125;
            recipeEx.Text = "ibRecipeEx";
            recipeEx.ToggleText.Indeterminate = "Indeterminate State";
            recipeEx.ToggleText.ToggledOff = "Recipes: Off";
            recipeEx.ToggleText.ToggledOn = "Recipes: On";
            tTip.SetToolTip(recipeEx, "Display Recipes");
            recipeEx.Visible = false;
            // 
            // footerPanel
            // 
            footerPanel.BackColor = Color.Black;
            footerPanel.Controls.Add(footerLayoutPanel);
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Location = new Point(10, 720);
            footerPanel.Name = "footerPanel";
            footerPanel.Padding = new Padding(8, 4, 8, 4);
            footerPanel.Size = new Size(1260, 38);
            footerPanel.TabIndex = 105;
            // 
            // footerLayoutPanel
            // 
            footerLayoutPanel.BackColor = Color.FromArgb(6, 17, 35);
            footerLayoutPanel.ColumnCount = 2;
            footerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            footerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            footerLayoutPanel.Controls.Add(footerSummaryLabel, 0, 0);
            footerLayoutPanel.Controls.Add(footerRightPanel, 1, 0);
            footerLayoutPanel.Dock = DockStyle.Fill;
            footerLayoutPanel.Location = new Point(8, 4);
            footerLayoutPanel.Name = "footerLayoutPanel";
            footerLayoutPanel.RowCount = 1;
            footerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            footerLayoutPanel.Size = new Size(1244, 30);
            footerLayoutPanel.TabIndex = 0;
            // 
            // footerSummaryLabel
            // 
            footerSummaryLabel.Dock = DockStyle.Fill;
            footerSummaryLabel.Font = new Font("Noto Sans", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            footerSummaryLabel.ForeColor = Color.WhiteSmoke;
            footerSummaryLabel.Location = new Point(0, 0);
            footerSummaryLabel.Margin = new Padding(0);
            footerSummaryLabel.Name = "footerSummaryLabel";
            footerSummaryLabel.Size = new Size(622, 30);
            footerSummaryLabel.TabIndex = 0;
            footerSummaryLabel.Text = "Character Summary";
            footerSummaryLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // footerRightPanel
            // 
            footerRightPanel.AutoSize = true;
            footerRightPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            footerRightPanel.BackColor = Color.FromArgb(6, 17, 35);
            footerRightPanel.Controls.Add(footerTotalSlotsLabel);
            footerRightPanel.Controls.Add(footerSlotsLeftLabel);
            footerRightPanel.Dock = DockStyle.Fill;
            footerRightPanel.FlowDirection = FlowDirection.RightToLeft;
            footerRightPanel.Location = new Point(622, 0);
            footerRightPanel.Margin = new Padding(0);
            footerRightPanel.Name = "footerRightPanel";
            footerRightPanel.Padding = new Padding(0, 3, 0, 0);
            footerRightPanel.Size = new Size(622, 30);
            footerRightPanel.TabIndex = 1;
            footerRightPanel.WrapContents = false;
            // 
            // footerTotalSlotsLabel
            // 
            footerTotalSlotsLabel.AutoSize = true;
            footerTotalSlotsLabel.Font = new Font("Noto Sans", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            footerTotalSlotsLabel.ForeColor = Color.WhiteSmoke;
            footerTotalSlotsLabel.Margin = new Padding(16, 0, 0, 0);
            footerTotalSlotsLabel.Name = "footerTotalSlotsLabel";
            footerTotalSlotsLabel.Size = new Size(117, 18);
            footerTotalSlotsLabel.TabIndex = 0;
            footerTotalSlotsLabel.Text = "Total Slots: 311";
            footerTotalSlotsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // footerSlotsLeftLabel
            // 
            footerSlotsLeftLabel.AutoSize = true;
            footerSlotsLeftLabel.Font = new Font("Noto Sans", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            footerSlotsLeftLabel.ForeColor = Color.FromArgb(115, 255, 110);
            footerSlotsLeftLabel.Margin = new Padding(16, 0, 0, 0);
            footerSlotsLeftLabel.Name = "footerSlotsLeftLabel";
            footerSlotsLeftLabel.Size = new Size(86, 18);
            footerSlotsLeftLabel.TabIndex = 1;
            footerSlotsLeftLabel.Text = "Slots Left: 0";
            // 
            // MenuBar
            // 
            MenuBar.BackColor = Color.Transparent;
            MenuBar.Font = new Font("Noto Sans SemiBold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MenuBar.ForeColor = Color.WhiteSmoke;
            MenuBar.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, CharacterToolStripMenuItem, ViewToolStripMenuItem, ShareToolStripMenuItem, WindowToolStripMenuItem, HelpToolStripMenuItem, supportIconMenuItem });
            MenuBar.Location = new Point(10, 45);
            MenuBar.Name = "MenuBar";
            MenuBar.Padding = new Padding(0, 2, 0, 2);
            MenuBar.Size = new Size(1260, 26);
            MenuBar.TabIndex = 86;
            MenuBar.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsFileNew, ToolStripSeparator7, tsFileOpen, tsBuildRcv, toolStripSeparator14, tsFileSave, tsFileSaveAs, ToolStripSeparator22, tsGenFreebies, ToolStripSeparator8, tsFilePrint, ToolStripSeparator9 });
            FileToolStripMenuItem.ForeColor = SystemColors.ControlText;
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(50, 22);
            FileToolStripMenuItem.Text = "&Build";
            // 
            // tsFileNew
            // 
            tsFileNew.Name = "tsFileNew";
            tsFileNew.ShortcutKeys = Keys.Control | Keys.N;
            tsFileNew.ShowShortcutKeys = false;
            tsFileNew.Size = new Size(273, 22);
            tsFileNew.Text = "&New Build";
            // 
            // ToolStripSeparator7
            // 
            ToolStripSeparator7.Name = "ToolStripSeparator7";
            ToolStripSeparator7.Size = new Size(270, 6);
            // 
            // tsFileOpen
            // 
            tsFileOpen.Name = "tsFileOpen";
            tsFileOpen.ShortcutKeys = Keys.Control | Keys.O;
            tsFileOpen.ShowShortcutKeys = false;
            tsFileOpen.Size = new Size(273, 22);
            tsFileOpen.Text = "&Open Build...";
            tsFileOpen.Click += TsFileOpen_Click;
            // 
            // tsBuildRcv
            // 
            tsBuildRcv.Name = "tsBuildRcv";
            tsBuildRcv.Size = new Size(273, 22);
            tsBuildRcv.Text = "Attempt Build recovery...";
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new Size(270, 6);
            // 
            // tsFileSave
            // 
            tsFileSave.Name = "tsFileSave";
            tsFileSave.ShortcutKeys = Keys.Control | Keys.S;
            tsFileSave.ShowShortcutKeys = false;
            tsFileSave.Size = new Size(273, 22);
            tsFileSave.Text = "&Save Build";
            tsFileSave.Click += TsFileSave_Click;
            // 
            // tsFileSaveAs
            // 
            tsFileSaveAs.Name = "tsFileSaveAs";
            tsFileSaveAs.Size = new Size(273, 22);
            tsFileSaveAs.Text = "Save Build &As...";
            tsFileSaveAs.Click += TsFileSaveAs_Click;
            // 
            // ToolStripSeparator22
            // 
            ToolStripSeparator22.Name = "ToolStripSeparator22";
            ToolStripSeparator22.Size = new Size(270, 6);
            // 
            // tsGenFreebies
            // 
            tsGenFreebies.Name = "tsGenFreebies";
            tsGenFreebies.Size = new Size(273, 22);
            tsGenFreebies.Text = "Generate Beta Server Pop Menu...";
            tsGenFreebies.Click += tsGenFreebies_Click;
            // 
            // ToolStripSeparator8
            // 
            ToolStripSeparator8.Name = "ToolStripSeparator8";
            ToolStripSeparator8.Size = new Size(270, 6);
            // 
            // tsFilePrint
            // 
            tsFilePrint.Name = "tsFilePrint";
            tsFilePrint.ShortcutKeys = Keys.Control | Keys.P;
            tsFilePrint.ShowShortcutKeys = false;
            tsFilePrint.Size = new Size(273, 22);
            tsFilePrint.Text = "&Print Build Summary...";
            tsFilePrint.Click += tsFilePrint_Click;
            // 
            // ToolStripSeparator9
            // 
            ToolStripSeparator9.Name = "ToolStripSeparator9";
            ToolStripSeparator9.Size = new Size(270, 6);
            // 
            // CharacterToolStripMenuItem
            // 
            CharacterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setEnemyRelativeLevelToolStripMenuItem, ToolStripSeparator16, SetAllIOsToDefault35ToolStripMenuItem, ToolStripMenuItem1, ToolStripMenuItem2, ToolStripSeparator28, SlotsToolStripMenuItem });
            CharacterToolStripMenuItem.ForeColor = SystemColors.ControlText;
            CharacterToolStripMenuItem.Name = "CharacterToolStripMenuItem";
            CharacterToolStripMenuItem.Size = new Size(68, 22);
            CharacterToolStripMenuItem.Text = "&Manage";
            // 
            // setEnemyRelativeLevelToolStripMenuItem
            // 
            setEnemyRelativeLevelToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem11, toolStripMenuItem12, toolStripMenuItem13, toolStripMenuItem14, defaultToolStripMenuItem, toolStripMenuItem15, toolStripMenuItem16, toolStripMenuItem17, toolStripMenuItem18, toolStripMenuItem19, toolStripMenuItem20, toolStripMenuItem21 });
            setEnemyRelativeLevelToolStripMenuItem.Name = "setEnemyRelativeLevelToolStripMenuItem";
            setEnemyRelativeLevelToolStripMenuItem.Size = new Size(273, 22);
            setEnemyRelativeLevelToolStripMenuItem.Text = "Set Enemy Relative Level";
            // 
            // toolStripMenuItem11
            // 
            toolStripMenuItem11.CheckOnClick = true;
            toolStripMenuItem11.Name = "toolStripMenuItem11";
            toolStripMenuItem11.Size = new Size(119, 22);
            toolStripMenuItem11.Text = "-4";
            // 
            // toolStripMenuItem12
            // 
            toolStripMenuItem12.CheckOnClick = true;
            toolStripMenuItem12.Name = "toolStripMenuItem12";
            toolStripMenuItem12.Size = new Size(119, 22);
            toolStripMenuItem12.Text = "-3";
            // 
            // toolStripMenuItem13
            // 
            toolStripMenuItem13.CheckOnClick = true;
            toolStripMenuItem13.Name = "toolStripMenuItem13";
            toolStripMenuItem13.Size = new Size(119, 22);
            toolStripMenuItem13.Text = "-2";
            // 
            // toolStripMenuItem14
            // 
            toolStripMenuItem14.CheckOnClick = true;
            toolStripMenuItem14.Name = "toolStripMenuItem14";
            toolStripMenuItem14.Size = new Size(119, 22);
            toolStripMenuItem14.Text = "-1";
            // 
            // defaultToolStripMenuItem
            // 
            defaultToolStripMenuItem.Checked = true;
            defaultToolStripMenuItem.CheckOnClick = true;
            defaultToolStripMenuItem.CheckState = CheckState.Checked;
            defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            defaultToolStripMenuItem.Size = new Size(119, 22);
            defaultToolStripMenuItem.Text = "Default";
            // 
            // toolStripMenuItem15
            // 
            toolStripMenuItem15.CheckOnClick = true;
            toolStripMenuItem15.Name = "toolStripMenuItem15";
            toolStripMenuItem15.Size = new Size(119, 22);
            toolStripMenuItem15.Text = "+1";
            // 
            // toolStripMenuItem16
            // 
            toolStripMenuItem16.CheckOnClick = true;
            toolStripMenuItem16.Name = "toolStripMenuItem16";
            toolStripMenuItem16.Size = new Size(119, 22);
            toolStripMenuItem16.Text = "+2";
            // 
            // toolStripMenuItem17
            // 
            toolStripMenuItem17.CheckOnClick = true;
            toolStripMenuItem17.Name = "toolStripMenuItem17";
            toolStripMenuItem17.Size = new Size(119, 22);
            toolStripMenuItem17.Text = "+3";
            // 
            // toolStripMenuItem18
            // 
            toolStripMenuItem18.CheckOnClick = true;
            toolStripMenuItem18.Name = "toolStripMenuItem18";
            toolStripMenuItem18.Size = new Size(119, 22);
            toolStripMenuItem18.Text = "+4";
            // 
            // toolStripMenuItem19
            // 
            toolStripMenuItem19.CheckOnClick = true;
            toolStripMenuItem19.Name = "toolStripMenuItem19";
            toolStripMenuItem19.Size = new Size(119, 22);
            toolStripMenuItem19.Text = "+5";
            // 
            // toolStripMenuItem20
            // 
            toolStripMenuItem20.CheckOnClick = true;
            toolStripMenuItem20.Name = "toolStripMenuItem20";
            toolStripMenuItem20.Size = new Size(119, 22);
            toolStripMenuItem20.Text = "+6";
            // 
            // toolStripMenuItem21
            // 
            toolStripMenuItem21.CheckOnClick = true;
            toolStripMenuItem21.Name = "toolStripMenuItem21";
            toolStripMenuItem21.Size = new Size(119, 22);
            toolStripMenuItem21.Text = "+7";
            // 
            // ToolStripSeparator16
            // 
            ToolStripSeparator16.Name = "ToolStripSeparator16";
            ToolStripSeparator16.Size = new Size(270, 6);
            // 
            // SetAllIOsToDefault35ToolStripMenuItem
            // 
            SetAllIOsToDefault35ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsIODefault, ToolStripSeparator11, tsIOMin, tsIOMax });
            SetAllIOsToDefault35ToolStripMenuItem.Name = "SetAllIOsToDefault35ToolStripMenuItem";
            SetAllIOsToDefault35ToolStripMenuItem.Size = new Size(273, 22);
            SetAllIOsToDefault35ToolStripMenuItem.Text = "&Set All IOs to...";
            // 
            // tsIODefault
            // 
            tsIODefault.Name = "tsIODefault";
            tsIODefault.Size = new Size(144, 22);
            tsIODefault.Text = "Default (35)";
            tsIODefault.Click += tsIODefault_Click;
            // 
            // ToolStripSeparator11
            // 
            ToolStripSeparator11.Name = "ToolStripSeparator11";
            ToolStripSeparator11.Size = new Size(141, 6);
            // 
            // tsIOMin
            // 
            tsIOMin.Name = "tsIOMin";
            tsIOMin.Size = new Size(144, 22);
            tsIOMin.Text = "Minimum";
            tsIOMin.Click += tsIOMin_Click;
            // 
            // tsIOMax
            // 
            tsIOMax.Name = "tsIOMax";
            tsIOMax.Size = new Size(144, 22);
            tsIOMax.Text = "Maximum";
            tsIOMax.Click += tsIOMax_Click;
            // 
            // ToolStripMenuItem1
            // 
            ToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { tsEnhToSO, tsEnhToDO, tsEnhToTO });
            ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            ToolStripMenuItem1.Size = new Size(273, 22);
            ToolStripMenuItem1.Text = "Set All Enhancement &Origins to...";
            // 
            // tsEnhToSO
            // 
            tsEnhToSO.Name = "tsEnhToSO";
            tsEnhToSO.Size = new Size(151, 22);
            tsEnhToSO.Text = "Single Origin";
            tsEnhToSO.Click += tsEnhToSO_Click;
            // 
            // tsEnhToDO
            // 
            tsEnhToDO.Name = "tsEnhToDO";
            tsEnhToDO.Size = new Size(151, 22);
            tsEnhToDO.Text = "Dual Origin";
            tsEnhToDO.Click += tsEnhToDO_Click;
            // 
            // tsEnhToTO
            // 
            tsEnhToTO.Name = "tsEnhToTO";
            tsEnhToTO.Size = new Size(151, 22);
            tsEnhToTO.Text = "Training";
            tsEnhToTO.Click += tsEnhToTO_Click;
            // 
            // ToolStripMenuItem2
            // 
            ToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { tsEnhToPlus5, tsEnhToPlus4, tsEnhToPlus3, tsEnhToPlus2, tsEnhToPlus1, tsEnhToEven, tsEnhToMinus1, tsEnhToMinus2, tsEnhToMinus3, tsEnhToNone });
            ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            ToolStripMenuItem2.Size = new Size(273, 22);
            ToolStripMenuItem2.Text = "Set All &Relative Levels to...";
            // 
            // tsEnhToPlus5
            // 
            tsEnhToPlus5.Name = "tsEnhToPlus5";
            tsEnhToPlus5.Size = new Size(221, 22);
            tsEnhToPlus5.Text = "+5 Levels";
            tsEnhToPlus5.Click += tsEnhToPlus5_Click;
            // 
            // tsEnhToPlus4
            // 
            tsEnhToPlus4.Name = "tsEnhToPlus4";
            tsEnhToPlus4.Size = new Size(221, 22);
            tsEnhToPlus4.Text = "+4 Levels";
            tsEnhToPlus4.Click += tsEnhToPlus4_Click;
            // 
            // tsEnhToPlus3
            // 
            tsEnhToPlus3.Name = "tsEnhToPlus3";
            tsEnhToPlus3.Size = new Size(221, 22);
            tsEnhToPlus3.Text = "+3 Levels";
            tsEnhToPlus3.Click += tsEnhToPlus3_Click;
            // 
            // tsEnhToPlus2
            // 
            tsEnhToPlus2.Name = "tsEnhToPlus2";
            tsEnhToPlus2.Size = new Size(221, 22);
            tsEnhToPlus2.Text = "+2 Levels";
            tsEnhToPlus2.Click += tsEnhToPlus2_Click;
            // 
            // tsEnhToPlus1
            // 
            tsEnhToPlus1.Name = "tsEnhToPlus1";
            tsEnhToPlus1.Size = new Size(221, 22);
            tsEnhToPlus1.Text = "+1 Level";
            tsEnhToPlus1.Click += tsEnhToPlus1_Click;
            // 
            // tsEnhToEven
            // 
            tsEnhToEven.Name = "tsEnhToEven";
            tsEnhToEven.Size = new Size(221, 22);
            tsEnhToEven.Text = "Even Level";
            tsEnhToEven.Click += tsEnhToEven_Click;
            // 
            // tsEnhToMinus1
            // 
            tsEnhToMinus1.Name = "tsEnhToMinus1";
            tsEnhToMinus1.Size = new Size(221, 22);
            tsEnhToMinus1.Text = "-1 Level";
            tsEnhToMinus1.Click += tsEnhToMinus1_Click;
            // 
            // tsEnhToMinus2
            // 
            tsEnhToMinus2.Name = "tsEnhToMinus2";
            tsEnhToMinus2.Size = new Size(221, 22);
            tsEnhToMinus2.Text = "-2 Levels";
            tsEnhToMinus2.Click += tsEnhToMinus2_Click;
            // 
            // tsEnhToMinus3
            // 
            tsEnhToMinus3.Name = "tsEnhToMinus3";
            tsEnhToMinus3.Size = new Size(221, 22);
            tsEnhToMinus3.Text = "-3 Levels";
            tsEnhToMinus3.Click += tsEnhToMinus3_Click;
            // 
            // tsEnhToNone
            // 
            tsEnhToNone.Name = "tsEnhToNone";
            tsEnhToNone.Size = new Size(221, 22);
            tsEnhToNone.Text = "None (Enh has no effect)";
            tsEnhToNone.Click += tsEnhToNone_Click;
            // 
            // ToolStripSeparator28
            // 
            ToolStripSeparator28.Name = "ToolStripSeparator28";
            ToolStripSeparator28.Size = new Size(270, 6);
            // 
            // SlotsToolStripMenuItem
            // 
            SlotsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsFlipAllEnh, ToolStripSeparator4, tsClearAllEnh, tsRemoveAllSlots, ToolStripSeparator1, AutoArrangeAllSlotsToolStripMenuItem });
            SlotsToolStripMenuItem.Name = "SlotsToolStripMenuItem";
            SlotsToolStripMenuItem.Size = new Size(273, 22);
            SlotsToolStripMenuItem.Text = "Manage Slots and Enhancements";
            // 
            // tsFlipAllEnh
            // 
            tsFlipAllEnh.Name = "tsFlipAllEnh";
            tsFlipAllEnh.Size = new Size(215, 22);
            tsFlipAllEnh.Text = "Flip All to Alternate";
            tsFlipAllEnh.Click += tsFlipAllEnh_Click;
            // 
            // ToolStripSeparator4
            // 
            ToolStripSeparator4.Name = "ToolStripSeparator4";
            ToolStripSeparator4.Size = new Size(212, 6);
            // 
            // tsClearAllEnh
            // 
            tsClearAllEnh.Name = "tsClearAllEnh";
            tsClearAllEnh.Size = new Size(215, 22);
            tsClearAllEnh.Text = "Clear All Enhancements";
            tsClearAllEnh.Click += tsClearAllEnh_Click;
            // 
            // tsRemoveAllSlots
            // 
            tsRemoveAllSlots.Name = "tsRemoveAllSlots";
            tsRemoveAllSlots.Size = new Size(215, 22);
            tsRemoveAllSlots.Text = "Remove All Slots";
            tsRemoveAllSlots.Click += tsRemoveAllSlots_Click;
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(212, 6);
            // 
            // AutoArrangeAllSlotsToolStripMenuItem
            // 
            AutoArrangeAllSlotsToolStripMenuItem.Name = "AutoArrangeAllSlotsToolStripMenuItem";
            AutoArrangeAllSlotsToolStripMenuItem.Size = new Size(215, 22);
            AutoArrangeAllSlotsToolStripMenuItem.Text = "&Auto-Arrange All Slots";
            AutoArrangeAllSlotsToolStripMenuItem.Click += AutoArrangeAllSlotsToolStripMenuItem_Click;
            // 
            // ViewToolStripMenuItem
            // 
            ViewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { layoutMenuItem, themeMenuItem, ToolStripSeparator13, toolStripMenuItem4, ToolStripSeparator2, toolStripMenuItem5, toolStripSeparator3, ToggleCheckModeToolStripMenuItem });
            ViewToolStripMenuItem.ForeColor = SystemColors.ControlText;
            ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            ViewToolStripMenuItem.Size = new Size(48, 22);
            ViewToolStripMenuItem.Text = "&View";
            // 
            // layoutMenuItem
            // 
            layoutMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsView2Col, tsView3Col, tsView4Col });
            layoutMenuItem.Name = "layoutMenuItem";
            layoutMenuItem.Size = new Size(276, 22);
            layoutMenuItem.Text = "Layout";
            // 
            // tsView2Col
            // 
            tsView2Col.Name = "tsView2Col";
            tsView2Col.Size = new Size(138, 22);
            tsView2Col.Text = "2 Columns";
            tsView2Col.Click += tsView2Col_Click;
            // 
            // tsView3Col
            // 
            tsView3Col.Name = "tsView3Col";
            tsView3Col.Size = new Size(138, 22);
            tsView3Col.Text = "3 Columns";
            tsView3Col.Click += tsView3Col_Click;
            // 
            // tsView4Col
            // 
            tsView4Col.Name = "tsView4Col";
            tsView4Col.Size = new Size(138, 22);
            tsView4Col.Text = "4 Columns";
            tsView4Col.Click += tsView4Col_Click;
            // 
            // themeMenuItem
            // 
            themeMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripSeparator15 });
            themeMenuItem.Name = "themeMenuItem";
            themeMenuItem.Size = new Size(276, 22);
            themeMenuItem.Text = "Theme";
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new Size(57, 6);
            // 
            // ToolStripSeparator13
            // 
            ToolStripSeparator13.Name = "ToolStripSeparator13";
            ToolStripSeparator13.Size = new Size(273, 6);
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.DropDownItems.AddRange(new ToolStripItem[] { tsViewIOLevels, tsViewSOLevels, tsViewRelative, tsViewSlotLevels, tsViewRelativeAsSigns });
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(276, 22);
            toolStripMenuItem4.Text = "Enhancement Display";
            // 
            // tsViewIOLevels
            // 
            tsViewIOLevels.Checked = true;
            tsViewIOLevels.CheckState = CheckState.Checked;
            tsViewIOLevels.Name = "tsViewIOLevels";
            tsViewIOLevels.Size = new Size(300, 22);
            tsViewIOLevels.Text = "Show &IO Levels";
            tsViewIOLevels.Click += tsViewIOLevels_Click;
            // 
            // tsViewSOLevels
            // 
            tsViewSOLevels.Name = "tsViewSOLevels";
            tsViewSOLevels.Size = new Size(300, 22);
            tsViewSOLevels.Text = "Show SO/HO Levels";
            tsViewSOLevels.Click += tsViewSOLevels_Click;
            // 
            // tsViewRelative
            // 
            tsViewRelative.Name = "tsViewRelative";
            tsViewRelative.Size = new Size(300, 22);
            tsViewRelative.Text = "Show &Enhancement Relative Levels";
            tsViewRelative.Click += tsViewRelative_Click;
            // 
            // tsViewSlotLevels
            // 
            tsViewSlotLevels.Name = "tsViewSlotLevels";
            tsViewSlotLevels.Size = new Size(300, 22);
            tsViewSlotLevels.Text = "Show &Slot Placement Levels";
            tsViewSlotLevels.Click += tsViewSlotLevels_Click;
            // 
            // tsViewRelativeAsSigns
            // 
            tsViewRelativeAsSigns.Name = "tsViewRelativeAsSigns";
            tsViewRelativeAsSigns.Size = new Size(300, 22);
            tsViewRelativeAsSigns.Text = "Show Relative Levels with signs ('+'/'-')";
            tsViewRelativeAsSigns.Click += tsViewRelativeAsSigns_Click;
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(273, 6);
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.DropDownItems.AddRange(new ToolStripItem[] { tsViewActualDamage_New, tsViewDPS_New, tlsDPA });
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new Size(276, 22);
            toolStripMenuItem5.Text = "Damage Display";
            // 
            // tsViewActualDamage_New
            // 
            tsViewActualDamage_New.Checked = true;
            tsViewActualDamage_New.CheckState = CheckState.Checked;
            tsViewActualDamage_New.Name = "tsViewActualDamage_New";
            tsViewActualDamage_New.Size = new Size(247, 22);
            tsViewActualDamage_New.Text = "Show Damage Per Activation";
            tsViewActualDamage_New.Click += tsViewActualDamage_New_Click;
            // 
            // tsViewDPS_New
            // 
            tsViewDPS_New.Name = "tsViewDPS_New";
            tsViewDPS_New.Size = new Size(247, 22);
            tsViewDPS_New.Text = "Show Damage Per Second";
            tsViewDPS_New.Click += tsViewDPS_New_Click;
            // 
            // tlsDPA
            // 
            tlsDPA.Name = "tlsDPA";
            tlsDPA.Size = new Size(247, 22);
            tlsDPA.Text = "Show Damage Per Animation";
            tlsDPA.Click += tlsDPA_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(273, 6);
            // 
            // ToggleCheckModeToolStripMenuItem
            // 
            ToggleCheckModeToolStripMenuItem.Name = "ToggleCheckModeToolStripMenuItem";
            ToggleCheckModeToolStripMenuItem.Size = new Size(276, 22);
            ToggleCheckModeToolStripMenuItem.Text = "Toggle Enhancement Check Mode";
            // 
            // ShareToolStripMenuItem
            // 
            ShareToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsShareMenu, ToolStripSeparator24, toolStripMenuItem6, toolStripSeparator6, importBuildFromToolStripMenuItem, ToolStripSeparator27, tsViewSharedBuilds });
            ShareToolStripMenuItem.ForeColor = SystemColors.ControlText;
            ShareToolStripMenuItem.Name = "ShareToolStripMenuItem";
            ShareToolStripMenuItem.Size = new Size(54, 22);
            ShareToolStripMenuItem.Text = "&Share";
            // 
            // tsShareMenu
            // 
            tsShareMenu.Name = "tsShareMenu";
            tsShareMenu.Size = new Size(227, 22);
            tsShareMenu.Text = "Export / Share Build...";
            tsShareMenu.Click += ShareMenu_Click;
            // 
            // ToolStripSeparator24
            // 
            ToolStripSeparator24.Name = "ToolStripSeparator24";
            ToolStripSeparator24.Size = new Size(224, 6);
            // 
            // toolStripMenuItem6
            // 
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            toolStripMenuItem6.Size = new Size(227, 22);
            toolStripMenuItem6.Text = "Create Social Media Post...";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(224, 6);
            // 
            // importBuildFromToolStripMenuItem
            // 
            importBuildFromToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsImportLegacyForumPost, tsImportDataChunk });
            importBuildFromToolStripMenuItem.Name = "importBuildFromToolStripMenuItem";
            importBuildFromToolStripMenuItem.Size = new Size(227, 22);
            importBuildFromToolStripMenuItem.Text = "Import Build from...";
            // 
            // tsImportLegacyForumPost
            // 
            tsImportLegacyForumPost.Name = "tsImportLegacyForumPost";
            tsImportLegacyForumPost.Size = new Size(195, 22);
            tsImportLegacyForumPost.Text = "Forum Post (Legacy)";
            tsImportLegacyForumPost.Click += tsImportLegacyForumPost_Click;
            // 
            // tsImportDataChunk
            // 
            tsImportDataChunk.Name = "tsImportDataChunk";
            tsImportDataChunk.Size = new Size(195, 22);
            tsImportDataChunk.Text = "Datachunk";
            tsImportDataChunk.Click += tsImportChunk_Click;
            // 
            // ToolStripSeparator27
            // 
            ToolStripSeparator27.Name = "ToolStripSeparator27";
            ToolStripSeparator27.Size = new Size(224, 6);
            // 
            // tsViewSharedBuilds
            // 
            tsViewSharedBuilds.Name = "tsViewSharedBuilds";
            tsViewSharedBuilds.Size = new Size(227, 22);
            tsViewSharedBuilds.Text = "View Shared Build Library";
            tsViewSharedBuilds.Click += tsViewSharedBuilds_Click;
            // 
            // WindowToolStripMenuItem
            // 
            WindowToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsViewSets, tsViewGraphs, tsViewSetCompare, tsViewData, tsSetFind, ToolStripSeparator18, tsRecipeViewer, tsRotationHelper, ToolStripSeparator19, InGameRespecHelperToolStripMenuItem, toolStripSeparator5, tsChangeDb, tsConfig, tsAdvDBEdit });
            WindowToolStripMenuItem.ForeColor = SystemColors.ControlText;
            WindowToolStripMenuItem.Name = "WindowToolStripMenuItem";
            WindowToolStripMenuItem.Size = new Size(49, 22);
            WindowToolStripMenuItem.Text = "&Tools";
            // 
            // tsViewSets
            // 
            tsViewSets.Name = "tsViewSets";
            tsViewSets.ShortcutKeys = Keys.Control | Keys.B;
            tsViewSets.ShowShortcutKeys = false;
            tsViewSets.Size = new Size(214, 22);
            tsViewSets.Text = "&Sets && Bonuses";
            tsViewSets.Click += tsViewSets_Click;
            // 
            // tsViewGraphs
            // 
            tsViewGraphs.Name = "tsViewGraphs";
            tsViewGraphs.ShortcutKeys = Keys.Control | Keys.G;
            tsViewGraphs.ShowShortcutKeys = false;
            tsViewGraphs.Size = new Size(214, 22);
            tsViewGraphs.Text = "Power &Graphs";
            tsViewGraphs.Click += tsViewGraphs_Click;
            // 
            // tsViewSetCompare
            // 
            tsViewSetCompare.Name = "tsViewSetCompare";
            tsViewSetCompare.ShortcutKeys = Keys.Control | Keys.C;
            tsViewSetCompare.ShowShortcutKeys = false;
            tsViewSetCompare.Size = new Size(214, 22);
            tsViewSetCompare.Text = "Powerset &Comparison";
            tsViewSetCompare.Click += tsViewSetCompare_Click;
            // 
            // tsViewData
            // 
            tsViewData.Name = "tsViewData";
            tsViewData.ShortcutKeys = Keys.Control | Keys.D;
            tsViewData.ShowShortcutKeys = false;
            tsViewData.Size = new Size(214, 22);
            tsViewData.Text = "&Power Data";
            tsViewData.Click += tsViewData_Click;
            // 
            // tsSetFind
            // 
            tsSetFind.Name = "tsSetFind";
            tsSetFind.Size = new Size(214, 22);
            tsSetFind.Text = "Set &Inspector";
            tsSetFind.Click += tsSetFind_Click;
            // 
            // ToolStripSeparator18
            // 
            ToolStripSeparator18.Name = "ToolStripSeparator18";
            ToolStripSeparator18.Size = new Size(211, 6);
            // 
            // tsRecipeViewer
            // 
            tsRecipeViewer.Name = "tsRecipeViewer";
            tsRecipeViewer.ShortcutKeys = Keys.Control | Keys.R;
            tsRecipeViewer.ShowShortcutKeys = false;
            tsRecipeViewer.Size = new Size(214, 22);
            tsRecipeViewer.Text = "&Recipe Viewer";
            tsRecipeViewer.Click += tsRecipeViewer_Click;
            // 
            // tsRotationHelper
            // 
            tsRotationHelper.Name = "tsRotationHelper";
            tsRotationHelper.ShortcutKeys = Keys.Control | Keys.Z;
            tsRotationHelper.ShowShortcutKeys = false;
            tsRotationHelper.Size = new Size(214, 22);
            tsRotationHelper.Text = "Rotation Helper (Beta)";
            tsRotationHelper.Click += tsRotationHelper_Click;
            // 
            // ToolStripSeparator19
            // 
            ToolStripSeparator19.Name = "ToolStripSeparator19";
            ToolStripSeparator19.Size = new Size(211, 6);
            // 
            // InGameRespecHelperToolStripMenuItem
            // 
            InGameRespecHelperToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsHelperShort, tsHelperLong });
            InGameRespecHelperToolStripMenuItem.Name = "InGameRespecHelperToolStripMenuItem";
            InGameRespecHelperToolStripMenuItem.Size = new Size(214, 22);
            InGameRespecHelperToolStripMenuItem.Text = "In-Game &Respec Helper";
            // 
            // tsHelperShort
            // 
            tsHelperShort.Name = "tsHelperShort";
            tsHelperShort.Size = new Size(148, 22);
            tsHelperShort.Text = "Profile &Short";
            tsHelperShort.Click += tsHelperShort_Click;
            // 
            // tsHelperLong
            // 
            tsHelperLong.Name = "tsHelperLong";
            tsHelperLong.Size = new Size(148, 22);
            tsHelperLong.Text = "Profile &Long";
            tsHelperLong.Click += tsHelperLong_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(211, 6);
            // 
            // tsChangeDb
            // 
            tsChangeDb.Name = "tsChangeDb";
            tsChangeDb.Size = new Size(214, 22);
            tsChangeDb.Text = "&Change Database";
            tsChangeDb.Click += tsChangeDb_Click;
            // 
            // tsConfig
            // 
            tsConfig.Name = "tsConfig";
            tsConfig.Size = new Size(214, 22);
            tsConfig.Text = "&Options...";
            tsConfig.Click += tsConfig_Click;
            // 
            // tsAdvDBEdit
            // 
            tsAdvDBEdit.Name = "tsAdvDBEdit";
            tsAdvDBEdit.Size = new Size(214, 22);
            tsAdvDBEdit.Text = "&Database Editor Suite";
            tsAdvDBEdit.Click += tsAdvDBEdit_Click;
            // 
            // HelpToolStripMenuItem
            // 
            HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem7, toolStripMenuItem8, toolStripSeparator12, tsUpdateCheck, ToolStripSeparator10, toolStripMenuItem9, tsSupport, tsGitHub, ToolStripSeparator31, tsAbout });
            HelpToolStripMenuItem.ForeColor = SystemColors.ControlText;
            HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            HelpToolStripMenuItem.Size = new Size(47, 22);
            HelpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem7
            // 
            toolStripMenuItem7.Name = "toolStripMenuItem7";
            toolStripMenuItem7.Size = new Size(198, 22);
            toolStripMenuItem7.Text = "View Help";
            // 
            // toolStripMenuItem8
            // 
            toolStripMenuItem8.Name = "toolStripMenuItem8";
            toolStripMenuItem8.Size = new Size(198, 22);
            toolStripMenuItem8.Text = "Quick Start Guide...";
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(195, 6);
            // 
            // tsUpdateCheck
            // 
            tsUpdateCheck.Name = "tsUpdateCheck";
            tsUpdateCheck.Size = new Size(198, 22);
            tsUpdateCheck.Text = "Check for &Updates";
            tsUpdateCheck.Click += tsUpdateCheck_Click;
            // 
            // ToolStripSeparator10
            // 
            ToolStripSeparator10.Name = "ToolStripSeparator10";
            ToolStripSeparator10.Size = new Size(195, 6);
            // 
            // toolStripMenuItem9
            // 
            toolStripMenuItem9.Name = "toolStripMenuItem9";
            toolStripMenuItem9.Size = new Size(198, 22);
            toolStripMenuItem9.Text = "Official Website...";
            // 
            // tsSupport
            // 
            tsSupport.Name = "tsSupport";
            tsSupport.Size = new Size(198, 22);
            tsSupport.Text = "Join our Discord...";
            tsSupport.Click += tsSupport_Click;
            // 
            // tsGitHub
            // 
            tsGitHub.Name = "tsGitHub";
            tsGitHub.Size = new Size(198, 22);
            tsGitHub.Text = "Project on GiHub...";
            tsGitHub.Click += Github_Link;
            // 
            // ToolStripSeparator31
            // 
            ToolStripSeparator31.Name = "ToolStripSeparator31";
            ToolStripSeparator31.Size = new Size(195, 6);
            // 
            // tsAbout
            // 
            tsAbout.Name = "tsAbout";
            tsAbout.Size = new Size(198, 22);
            tsAbout.Text = "About Mids Reborn...";
            tsAbout.Click += tsAbout_Click;
            // 
            // supportIconMenuItem
            // 
            supportIconMenuItem.Alignment = ToolStripItemAlignment.Right;
            supportIconMenuItem.DropDownItems.AddRange(new ToolStripItem[] { kofiIconMenuItem, patreonIconMenuItem });
            supportIconMenuItem.IconChar = FontAwesome.Sharp.IconChar.Heart;
            supportIconMenuItem.IconColor = Color.Red;
            supportIconMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            supportIconMenuItem.Name = "supportIconMenuItem";
            supportIconMenuItem.Size = new Size(161, 22);
            supportIconMenuItem.Text = "Support Mids Reborn";
            // 
            // kofiIconMenuItem
            // 
            kofiIconMenuItem.IconChar = FontAwesome.Sharp.IconChar.MugHot;
            kofiIconMenuItem.IconColor = Color.White;
            kofiIconMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            kofiIconMenuItem.Name = "kofiIconMenuItem";
            kofiIconMenuItem.Size = new Size(236, 22);
            kofiIconMenuItem.Text = "Ko-Fi (Buy us a coffee)";
            // 
            // patreonIconMenuItem
            // 
            patreonIconMenuItem.IconChar = FontAwesome.Sharp.IconChar.Patreon;
            patreonIconMenuItem.IconColor = Color.OrangeRed;
            patreonIconMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            patreonIconMenuItem.Name = "patreonIconMenuItem";
            patreonIconMenuItem.Size = new Size(236, 22);
            patreonIconMenuItem.Text = "Patreon  (Become a patron)";
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
            // btnMinimize
            // 
            btnMinimize.BackColor = Color.Transparent;
            btnMinimize.Dock = DockStyle.Right;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
            btnMinimize.IconColor = Color.WhiteSmoke;
            btnMinimize.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMinimize.IconSize = 24;
            btnMinimize.Location = new Point(1164, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(32, 35);
            btnMinimize.TabIndex = 0;
            tTip.SetToolTip(btnMinimize, "Minimize");
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += BtnMinimize_Click;
            // 
            // btnMaximize
            // 
            btnMaximize.BackColor = Color.Transparent;
            btnMaximize.Dock = DockStyle.Right;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnMaximize.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
            btnMaximize.IconColor = Color.WhiteSmoke;
            btnMaximize.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnMaximize.IconSize = 24;
            btnMaximize.Location = new Point(1196, 0);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(32, 35);
            btnMaximize.TabIndex = 1;
            tTip.SetToolTip(btnMaximize, "Maximize");
            btnMaximize.UseVisualStyleBackColor = false;
            btnMaximize.Click += BtnMaximize_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Transparent;
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 100, 150, 210);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.Transparent;
            btnClose.IconChar = FontAwesome.Sharp.IconChar.Close;
            btnClose.IconColor = Color.WhiteSmoke;
            btnClose.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnClose.IconSize = 24;
            btnClose.Location = new Point(1228, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(32, 35);
            btnClose.TabIndex = 2;
            tTip.SetToolTip(btnClose, "Close");
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += BtnClose_Click;
            // 
            // titlePanel
            // 
            titlePanel.BackColor = Color.Transparent;
            titlePanel.Controls.Add(logoPanel);
            titlePanel.Controls.Add(titleLabel);
            titlePanel.Controls.Add(btnMinimize);
            titlePanel.Controls.Add(btnMaximize);
            titlePanel.Controls.Add(btnClose);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Location = new Point(10, 10);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new Size(1260, 35);
            titlePanel.TabIndex = 104;
            titlePanel.MouseDown += Title_MouseDown;
            // 
            // logoPanel
            // 
            logoPanel.Dock = DockStyle.Left;
            logoPanel.Location = new Point(0, 0);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new Size(350, 50);
            logoPanel.TabIndex = 6;
            logoPanel.MouseDown += Title_MouseDown;
            // 
            // titleLabel
            // 
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Noto Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            titleLabel.Location = new Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(1164, 35);
            titleLabel.TabIndex = 5;
            titleLabel.Text = "Issue 4: Resurgence";
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.MouseDown += Title_MouseDown;
            // 
            // MainWindow2
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(23, 23, 25);
            ClientSize = new Size(1280, 768);
            Controls.Add(mainLayoutPanel);
            Controls.Add(footerPanel);
            Controls.Add(MenuBar);
            Controls.Add(titlePanel);
            DoubleBuffered = true;
            ForeColor = Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1280, 768);
            Name = "MainWindow2";
            Padding = new Padding(10);
            Text = "Mids Reborn v4.0 (alpha)";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            characterLayoutPanel.ResumeLayout(false);
            characterLayoutPanel.PerformLayout();
            leftLayoutPanel.ResumeLayout(false);
            rightLayoutPanel.ResumeLayout(false);
            canvasScrollPanel.ContentPanel.ResumeLayout(false);
            canvasScrollPanel.ResumeLayout(false);
            characterPanel.ResumeLayout(false);
            characterPanel.PerformLayout();
            midsvScrollPanel1.ContentPanel.ResumeLayout(false);
            midsvScrollPanel1.ContentPanel.PerformLayout();
            midsvScrollPanel1.ResumeLayout(false);
            rightInnerLayoutPanel.ResumeLayout(false);
            leftInnerLayoutPanel.ResumeLayout(false);
            buttonsLayoutPanel.ResumeLayout(false);
            footerPanel.ResumeLayout(false);
            footerLayoutPanel.ResumeLayout(false);
            footerLayoutPanel.PerformLayout();
            footerRightPanel.ResumeLayout(false);
            footerRightPanel.PerformLayout();
            MenuBar.ResumeLayout(false);
            MenuBar.PerformLayout();
            titlePanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal System.Windows.Forms.OpenFileDialog DlgOpen;
        internal System.Windows.Forms.SaveFileDialog DlgSave;
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private TableLayoutPanel rightLayoutPanel;
        private MidsSmartLayoutPanel leftLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel buttonsLayoutPanel;
        private MidsVectorButton pvXEx;
        private MidsVectorButton recipeEx;
        private MidsVectorButton popupEx;
        private MidsVectorButton accoladesEx;
        internal MidsVectorButton incarnatesEx;
        internal MidsVectorButton ibPrestigePowersEx;
        private MidsVectorButton tempPowersEx;
        private MidsVScrollPanel canvasScrollPanel;
        private MidsMenuStrip MenuBar;
        private ToolStripMenuItem FileToolStripMenuItem;
        private ToolStripMenuItem tsFileNew;
        private ToolStripSeparator ToolStripSeparator7;
        private ToolStripMenuItem tsFileOpen;
        private ToolStripMenuItem tsFileSave;
        private ToolStripMenuItem tsFileSaveAs;
        private ToolStripSeparator ToolStripSeparator22;
        private ToolStripSeparator ToolStripSeparator8;
        private ToolStripMenuItem tsFilePrint;
        private ToolStripSeparator ToolStripSeparator9;
        private ToolStripMenuItem CharacterToolStripMenuItem;
        private ToolStripMenuItem SetAllIOsToDefault35ToolStripMenuItem;
        private ToolStripMenuItem tsIODefault;
        private ToolStripSeparator ToolStripSeparator11;
        private ToolStripMenuItem tsIOMin;
        private ToolStripMenuItem tsIOMax;
        private ToolStripSeparator ToolStripSeparator16;
        private ToolStripMenuItem ToolStripMenuItem1;
        private ToolStripMenuItem tsEnhToSO;
        private ToolStripMenuItem tsEnhToDO;
        private ToolStripMenuItem tsEnhToTO;
        private ToolStripMenuItem ToolStripMenuItem2;
        private ToolStripMenuItem tsEnhToPlus5;
        private ToolStripMenuItem tsEnhToPlus4;
        private ToolStripMenuItem tsEnhToPlus3;
        private ToolStripMenuItem tsEnhToPlus2;
        private ToolStripMenuItem tsEnhToPlus1;
        private ToolStripMenuItem tsEnhToEven;
        private ToolStripMenuItem tsEnhToMinus1;
        private ToolStripMenuItem tsEnhToMinus2;
        private ToolStripMenuItem tsEnhToMinus3;
        private ToolStripMenuItem tsEnhToNone;
        private ToolStripMenuItem SlotsToolStripMenuItem;
        private ToolStripMenuItem tsFlipAllEnh;
        private ToolStripSeparator ToolStripSeparator4;
        private ToolStripMenuItem tsClearAllEnh;
        private ToolStripMenuItem tsRemoveAllSlots;
        private ToolStripSeparator ToolStripSeparator1;
        private ToolStripMenuItem AutoArrangeAllSlotsToolStripMenuItem;
        private ToolStripSeparator ToolStripSeparator28;
        private ToolStripMenuItem ViewToolStripMenuItem;
        private ToolStripSeparator ToolStripSeparator13;
        private ToolStripSeparator ToolStripSeparator2;
        private ToolStripMenuItem WindowToolStripMenuItem;
        private ToolStripMenuItem tsViewSets;
        private ToolStripMenuItem tsViewGraphs;
        private ToolStripMenuItem tsViewSetCompare;
        private ToolStripMenuItem tsViewData;
        private ToolStripSeparator ToolStripSeparator18;
        private ToolStripMenuItem tsRecipeViewer;
        private ToolStripMenuItem tsRotationHelper;
        private ToolStripSeparator ToolStripSeparator19;
        private ToolStripMenuItem tsSetFind;
        private ToolStripMenuItem InGameRespecHelperToolStripMenuItem;
        private ToolStripMenuItem tsHelperShort;
        private ToolStripMenuItem tsHelperLong;
        private ToolStripMenuItem HelpToolStripMenuItem;
        private ToolStripMenuItem tsUpdateCheck;
        private ToolStripSeparator ToolStripSeparator10;
        private ToolStripMenuItem tsGitHub;
        private ToolStripMenuItem tsSupport;
        private ToolStripSeparator ToolStripSeparator31;
        private ToolStripMenuItem tsAbout;
        private TableLayoutPanel characterLayoutPanel;
        private Label lblName;
        private Label lblAT;
        private Label lblOrigin;
        private Panel characterPanel;
        private TextBox txtName;
        private ArchetypeDropDownList atDropDown;
        private OriginDropDownList originDropDown;
        private MidsVectorButton modeEx;
        private MidsVectorButton totalsEx;
        private Timer tmrGfx;
        private ToolTip tTip;
        private MidsDataViewNeo dataView;
        private MidsVScrollPanel midsvScrollPanel1;
        private TableLayoutPanel leftInnerLayoutPanel;
        private Label lblPrimary;
        private PowersetDropDownList primaryDropDown;
        private MidsListView primaryList;
        private MidsListView secondaryList;
        private PowersetDropDownList secondaryDropDown;
        private Label label1;
        private Panel titlePanel;
        private FontAwesome.Sharp.IconButton btnClose;
        private FontAwesome.Sharp.IconButton btnMaximize;
        private FontAwesome.Sharp.IconButton btnMinimize;
        private Label titleLabel;
        private MidsLogoPanel logoPanel;
        private Panel footerPanel;
        private TableLayoutPanel footerLayoutPanel;
        private Label footerSummaryLabel;
        private FlowLayoutPanel footerRightPanel;
        private Label footerTotalSlotsLabel;
        private Label footerSlotsLeftLabel;
        private ToolStripMenuItem layoutMenuItem;
        private ToolStripMenuItem tsView2Col;
        private ToolStripMenuItem tsView3Col;
        private ToolStripMenuItem tsView4Col;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem tsViewIOLevels;
        private ToolStripMenuItem tsViewSOLevels;
        private ToolStripMenuItem tsViewRelative;
        private ToolStripMenuItem tsViewSlotLevels;
        private ToolStripMenuItem tsViewRelativeAsSigns;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem tsViewActualDamage_New;
        private ToolStripMenuItem tsViewDPS_New;
        private ToolStripMenuItem tlsDPA;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem ToggleCheckModeToolStripMenuItem;
        private ToolStripMenuItem tsGenFreebies;
        private ToolStripMenuItem ShareToolStripMenuItem;
        private ToolStripMenuItem tsShareMenu;
        private ToolStripSeparator ToolStripSeparator24;
        private ToolStripSeparator ToolStripSeparator27;
        private ToolStripMenuItem tsViewSharedBuilds;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem importBuildFromToolStripMenuItem;
        private ToolStripMenuItem tsImportLegacyForumPost;
        private ToolStripMenuItem tsImportDataChunk;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem tsChangeDb;
        private ToolStripMenuItem tsConfig;
        private ToolStripMenuItem tsAdvDBEdit;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripSeparator toolStripSeparator12;
        private FontAwesome.Sharp.IconMenuItem supportIconMenuItem;
        private FontAwesome.Sharp.IconMenuItem kofiIconMenuItem;
        private FontAwesome.Sharp.IconMenuItem patreonIconMenuItem;
        private ToolStripMenuItem tsBuildRcv;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem toolStripMenuItem9;
        private MidsVectorButton slotInfoEx;
        private MidsVectorButton setsEx;
        private MidsVectorButton slotLevelsEx;
        private MidsVectorButton dynMode;
        private ToolStripMenuItem setEnemyRelativeLevelToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem11;
        private ToolStripMenuItem toolStripMenuItem12;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem toolStripMenuItem14;
        private ToolStripMenuItem defaultToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem toolStripMenuItem17;
        private ToolStripMenuItem toolStripMenuItem18;
        private ToolStripMenuItem toolStripMenuItem19;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripMenuItem toolStripMenuItem21;
        private TableLayoutPanel rightInnerLayoutPanel;
        private Label pool0Label;
        private MidsListView pool0List;
        private MidsListView ancillaryList;
        private PowersetDropDownList ancillaryDropDown;
        private Label ancillaryLabel;
        private MidsListView pool3List;
        private PowersetDropDownList pool3DropDown;
        private Label pool3Label;
        private MidsListView pool2List;
        private PowersetDropDownList pool2DropDown;
        private Label pool2Label;
        private MidsListView pool1List;
        private PowersetDropDownList pool1DropDown;
        private Label pool1Label;
        private PowersetDropDownList pool0DropDown;
        private MidsBufferedImagePanel canvas;
        private ToolStripMenuItem themeMenuItem;
        private ToolStripSeparator toolStripSeparator15;
        private MidsVectorButton combatEx;
    }
}

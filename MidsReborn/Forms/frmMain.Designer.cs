using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Mids_Reborn.Controls;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace Mids_Reborn.Forms
{
    public sealed partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.components = new System.ComponentModel.Container();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cbAT = new System.Windows.Forms.ComboBox();
            this.cbOrigin = new System.Windows.Forms.ComboBox();
            this.cbPrimary = new System.Windows.Forms.ComboBox();
            this.lblPrimary = new System.Windows.Forms.Label();
            this.lblSecondary = new System.Windows.Forms.Label();
            this.cbSecondary = new System.Windows.Forms.ComboBox();
            this.cbPool0 = new System.Windows.Forms.ComboBox();
            this.lblPool1 = new System.Windows.Forms.Label();
            this.cbPool1 = new System.Windows.Forms.ComboBox();
            this.lblPool2 = new System.Windows.Forms.Label();
            this.cbPool2 = new System.Windows.Forms.ComboBox();
            this.lblPool3 = new System.Windows.Forms.Label();
            this.cbPool3 = new System.Windows.Forms.ComboBox();
            this.lblPool4 = new System.Windows.Forms.Label();
            this.cbAncillary = new System.Windows.Forms.ComboBox();
            this.lblEpic = new System.Windows.Forms.Label();
            this.lblATLocked = new System.Windows.Forms.Label();
            this.DlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.DlgSave = new System.Windows.Forms.SaveFileDialog();
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblLocked0 = new System.Windows.Forms.Label();
            this.lblLocked1 = new System.Windows.Forms.Label();
            this.lblLocked2 = new System.Windows.Forms.Label();
            this.lblLocked3 = new System.Windows.Forms.Label();
            this.lblLockedAncillary = new System.Windows.Forms.Label();
            this.lblLockedSecondary = new System.Windows.Forms.Label();
            this.tmrGfx = new System.Windows.Forms.Timer(this.components);
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBuildRcv = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.LegacyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImport = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsExportDataLink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsGenFreebies = new System.Windows.Forms.ToolStripMenuItem();
            this.tsGenJsonExport = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.tsShareDiscord = new System.Windows.Forms.ToolStripMenuItem();
            this.tsShareMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImportDataChunk = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsChangeDb = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
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
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView2Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView3Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView4Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView5Col = new System.Windows.Forms.ToolStripMenuItem();
            this.tsView6Col = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsViewIOLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSOLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewRelative = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewRelativeAsSigns = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewSlotLevels = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsViewActualDamage_New = new System.Windows.Forms.ToolStripMenuItem();
            this.tsViewDPS_New = new System.Windows.Forms.ToolStripMenuItem();
            this.tlsDPA = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.tsPatchNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSupport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.tsKoFi = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.tsPatreon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCoinbase = new ToolStripMenuItem();
            this.tsGitHub = new System.Windows.Forms.ToolStripMenuItem();
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
            this.ToolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.ToggleCheckModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DonateToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator22 = new ToolStripSeparator();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblCharacter = new System.Windows.Forms.Label();
            this.ibModeEx = new ImageButtonEx();
            this.ibDynMode = new ImageButtonEx();
            this.ibAlignmentEx = new ImageButtonEx();
            this.ibSlotInfoEx = new ImageButtonEx();
            this.ibAccoladesEx = new ImageButtonEx();
            this.ibPetsEx = new ImageButtonEx();
            this.ibTeamEx = new ImageButtonEx();
            this.ibPopupEx = new ImageButtonEx();
            this.ibPvXEx = new ImageButtonEx();
            this.ibRecipeEx = new ImageButtonEx();
            this.ibSetsEx = new ImageButtonEx();
            this.ibSlotLevelsEx = new ImageButtonEx();
            this.ibTotalsEx = new ImageButtonEx();
            this.ibPrestigePowersEx = new ImageButtonEx();
            this.ibIncarnatePowersEx = new ImageButtonEx();
            this.ibTempPowersEx = new ImageButtonEx();
            this.tsBuildRcv = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripSeparator30 = new ToolStripSeparator();
            this.ToolStripSeparator31 = new ToolStripSeparator();
            this.ToolStripSeparator32 = new ToolStripSeparator();
            this.pnlGFX = new pnlGFX();
            this.pnlGFXFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblName = new Label();
            this.lblOrigin = new Label();
            this.lblAT = new Label();
            this.llPool0 = new ListLabelV3();
            this.llPool1 = new ListLabelV3();
            this.llSecondary = new ListLabelV3();
            this.llPrimary = new ListLabelV3();
            this.llPool3 = new ListLabelV3();
            this.llPool2 = new ListLabelV3();
            this.llAncillary = new ListLabelV3();
            this.i9Picker = new I9Picker();
            this.I9Popup = new ctlPopUp();
            this.poolsPanel = new Panel();
            this.enhCheckMode = new EnhCheckMode(this);
            this.EnemyRelativeToolStripComboBox = new ToolStripComboBox();
            this.tsViewBuildComment = new ToolStripMenuItem();
            this.MenuBar.SuspendLayout();
            this.topPanel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.pbDynMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGFX)).BeginInit();
            this.pnlGFXFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtName.ForeColor = System.Drawing.Color.Black;
            this.txtName.Location = new System.Drawing.Point(94, 82);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(144, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // cbAT
            // 
            this.cbAT.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbAT.ForeColor = System.Drawing.Color.Black;
            this.cbAT.DisplayMember = "DisplayName";
            this.cbAT.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAT.ForeColor = System.Drawing.Color.Black;
            this.cbAT.ItemHeight = 17;
            this.cbAT.Location = new System.Drawing.Point(94, 108);
            this.cbAT.MaxDropDownItems = 15;
            this.cbAT.Name = "cbAT";
            this.cbAT.Size = new System.Drawing.Size(144, 23);
            this.cbAT.TabIndex = 3;
            this.cbAT.ValueMember = "Idx";
            this.cbAT.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbAT_DrawItem);
            this.cbAT.SelectionChangeCommitted += new System.EventHandler(this.cbAT_SelectedIndexChanged);
            this.cbAT.MouseLeave += new System.EventHandler(this.cbAT_MouseLeave);
            this.cbAT.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbAT_MouseMove);
            // 
            // cbOrigin
            // 
            this.cbOrigin.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbOrigin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrigin.ForeColor = System.Drawing.Color.Black;
            this.cbOrigin.ItemHeight = 17;
            this.cbOrigin.Location = new System.Drawing.Point(94, 133);
            this.cbOrigin.Name = "cbOrigin";
            this.cbOrigin.Size = new System.Drawing.Size(144, 23);
            this.cbOrigin.TabIndex = 5;
            this.cbOrigin.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbOrigin_DrawItem);
            this.cbOrigin.SelectionChangeCommitted += new System.EventHandler(this.cbOrigin_SelectedIndexChanged);
            // 
            // cbPrimary
            // 
            this.cbPrimary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPrimary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrimary.ForeColor = System.Drawing.Color.Black;
            this.cbPrimary.ItemHeight = 16;
            this.cbPrimary.Location = new System.Drawing.Point(16, 182);
            this.cbPrimary.MaxDropDownItems = 15;
            this.cbPrimary.Name = "cbPrimary";
            this.cbPrimary.Size = new System.Drawing.Size(144, 22);
            this.cbPrimary.TabIndex = 7;
            this.cbPrimary.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbPrimary_DrawItem);
            this.cbPrimary.SelectionChangeCommitted += new System.EventHandler(this.cbPrimary_SelectedIndexChanged);
            this.cbPrimary.MouseLeave += new System.EventHandler(this.cbPrimary_MouseLeave);
            this.cbPrimary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPrimary_MouseMove);
            // 
            // lblPrimary
            // 
            this.lblPrimary.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPrimary.ForeColor = System.Drawing.Color.White;
            this.lblPrimary.Location = new System.Drawing.Point(20, 166);
            this.lblPrimary.Name = "lblPrimary";
            this.lblPrimary.Size = new System.Drawing.Size(136, 17);
            this.lblPrimary.TabIndex = 9;
            this.lblPrimary.Text = "Primary Power Set";
            this.lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSecondary
            // 
            this.lblSecondary.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblSecondary.ForeColor = System.Drawing.Color.White;
            this.lblSecondary.Location = new System.Drawing.Point(172, 166);
            this.lblSecondary.Name = "lblSecondary";
            this.lblSecondary.Size = new System.Drawing.Size(136, 17);
            this.lblSecondary.TabIndex = 10;
            this.lblSecondary.Text = "Secondary Power Set";
            this.lblSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbSecondary
            // 
            this.cbSecondary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbSecondary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecondary.ForeColor = System.Drawing.Color.Black;
            this.cbSecondary.ItemHeight = 16;
            this.cbSecondary.Location = new System.Drawing.Point(168, 182);
            this.cbSecondary.MaxDropDownItems = 15;
            this.cbSecondary.Name = "cbSecondary";
            this.cbSecondary.Size = new System.Drawing.Size(144, 22);
            this.cbSecondary.TabIndex = 11;
            this.cbSecondary.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbSecondary_DrawItem);
            this.cbSecondary.SelectionChangeCommitted += new System.EventHandler(this.cbSecondary_SelectedIndexChanged);
            this.cbSecondary.MouseLeave += new System.EventHandler(this.cbSecondary_MouseLeave);
            this.cbSecondary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbSecondary_MouseMove);
            // 
            // cbPool0
            // 
            this.cbPool0.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool0.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool0.ForeColor = System.Drawing.Color.Black;
            this.cbPool0.ItemHeight = 16;
            this.cbPool0.Location = new System.Drawing.Point(0, 21); // 328, 182
            this.cbPool0.MaxDropDownItems = 15;
            this.cbPool0.Name = "cbPool0";
            this.cbPool0.Size = new System.Drawing.Size(145, 22);
            this.cbPool0.TabIndex = 15;
            this.cbPool0.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbPool0_DrawItem);
            this.cbPool0.SelectionChangeCommitted += new System.EventHandler(this.cbPool0_SelectedIndexChanged);
            this.cbPool0.MouseLeave += new System.EventHandler(this.cbPool0_MouseLeave);
            this.cbPool0.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPools_MouseMove);
            // 
            // lblPool1
            // 
            this.lblPool1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool1.ForeColor = System.Drawing.Color.White;
            this.lblPool1.Location = new System.Drawing.Point(0, 5); // 328, 166
            this.lblPool1.Name = "lblPool1";
            this.lblPool1.Size = new System.Drawing.Size(136, 17);
            this.lblPool1.TabIndex = 14;
            this.lblPool1.Text = "Pool 1";
            this.lblPool1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool1
            // 
            this.cbPool1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool1.ForeColor = System.Drawing.Color.Black;
            this.cbPool1.ItemHeight = 16;
            this.cbPool1.Location = new System.Drawing.Point(0, 129); // 328, 290
            this.cbPool1.MaxDropDownItems = 15;
            this.cbPool1.Name = "cbPool1";
            this.cbPool1.Size = new System.Drawing.Size(145, 22);
            this.cbPool1.TabIndex = 18;
            this.cbPool1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbPool1_DrawItem);
            this.cbPool1.SelectionChangeCommitted += new System.EventHandler(this.cbPool1_SelectedIndexChanged);
            this.cbPool1.MouseLeave += new System.EventHandler(this.cbPool0_MouseLeave);
            this.cbPool1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPools_MouseMove);
            // 
            // lblPool2
            // 
            this.lblPool2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool2.ForeColor = System.Drawing.Color.White;
            this.lblPool2.Location = new System.Drawing.Point(0, 113); // 328, 274
            this.lblPool2.Name = "lblPool2";
            this.lblPool2.Size = new System.Drawing.Size(136, 17);
            this.lblPool2.TabIndex = 17;
            this.lblPool2.Text = "Pool 2";
            this.lblPool2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool2
            // 
            this.cbPool2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool2.ForeColor = System.Drawing.Color.Black;
            this.cbPool2.ItemHeight = 16;
            this.cbPool2.Location = new System.Drawing.Point(0, 237); // 328, 398
            this.cbPool2.MaxDropDownItems = 15;
            this.cbPool2.Name = "cbPool2";
            this.cbPool2.Size = new System.Drawing.Size(145, 22);
            this.cbPool2.TabIndex = 21;
            this.cbPool2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbPool2_DrawItem);
            this.cbPool2.SelectionChangeCommitted += new System.EventHandler(this.cbPool2_SelectedIndexChanged);
            this.cbPool2.MouseLeave += new System.EventHandler(this.cbPool0_MouseLeave);
            this.cbPool2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPools_MouseMove);
            // 
            // lblPool3
            // 
            this.lblPool3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool3.ForeColor = System.Drawing.Color.White;
            this.lblPool3.Location = new System.Drawing.Point(0, 221); // 328, 382
            this.lblPool3.Name = "lblPool3";
            this.lblPool3.Size = new System.Drawing.Size(136, 17);
            this.lblPool3.TabIndex = 20;
            this.lblPool3.Text = "Pool 3";
            this.lblPool3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPool3
            // 
            this.cbPool3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbPool3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPool3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool3.ForeColor = System.Drawing.Color.Black;
            this.cbPool3.ItemHeight = 16;
            this.cbPool3.Location = new System.Drawing.Point(0, 345); // 328, 506
            this.cbPool3.MaxDropDownItems = 15;
            this.cbPool3.Name = "cbPool3";
            this.cbPool3.Size = new System.Drawing.Size(145, 22);
            this.cbPool3.TabIndex = 24;
            this.cbPool3.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbPool3_DrawItem);
            this.cbPool3.SelectionChangeCommitted += new System.EventHandler(this.cbPool3_SelectedIndexChanged);
            this.cbPool3.MouseLeave += new System.EventHandler(this.cbPool0_MouseLeave);
            this.cbPool3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPools_MouseMove);
            // 
            // lblPool4
            // 
            this.lblPool4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblPool4.ForeColor = System.Drawing.Color.White;
            this.lblPool4.Location = new System.Drawing.Point(0, 329); // 328, 490
            this.lblPool4.Name = "lblPool4";
            this.lblPool4.Size = new System.Drawing.Size(136, 17);
            this.lblPool4.TabIndex = 23;
            this.lblPool4.Text = "Pool 4";
            this.lblPool4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbAncillary
            // 
            this.cbAncillary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbAncillary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbAncillary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAncillary.ForeColor = System.Drawing.Color.Black;
            this.cbAncillary.ItemHeight = 16;
            this.cbAncillary.Location = new System.Drawing.Point(0, 453); // 328, 614
            this.cbAncillary.Name = "cbAncillary";
            this.cbAncillary.Size = new System.Drawing.Size(145, 22);
            this.cbAncillary.TabIndex = 27;
            this.cbAncillary.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbAncillary_DrawItem);
            this.cbAncillary.SelectionChangeCommitted += new System.EventHandler(this.cbAncillery_SelectedIndexChanged);
            this.cbAncillary.MouseLeave += new System.EventHandler(this.cbPool0_MouseLeave);
            this.cbAncillary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cbPools_MouseMove);
            // 
            // lblEpic
            // 
            this.lblEpic.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblEpic.ForeColor = System.Drawing.Color.White;
            this.lblEpic.Location = new System.Drawing.Point(0, 437); // 328, 598
            this.lblEpic.Name = "lblEpic";
            this.lblEpic.Size = new System.Drawing.Size(136, 17);
            this.lblEpic.TabIndex = 26;
            this.lblEpic.Text = "Ancillary/Epic Pool";
            this.lblEpic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblATLocked
            // 
            this.lblATLocked.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))),
                ((int)(((byte)(224)))));
            this.lblATLocked.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblATLocked.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblATLocked.ForeColor = System.Drawing.Color.Black;
            this.lblATLocked.Location = new System.Drawing.Point(94, 110);
            this.lblATLocked.Name = "lblATLocked";
            this.lblATLocked.Size = new System.Drawing.Size(92, 29);
            this.lblATLocked.TabIndex = 53;
            this.lblATLocked.Text = "Archetype Locked";
            this.lblATLocked.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblATLocked.Paint += new System.Windows.Forms.PaintEventHandler(this.lblATLocked_Paint);
            this.lblATLocked.MouseLeave += new System.EventHandler(this.lblATLocked_MouseLeave);
            this.lblATLocked.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblATLocked_MouseMove);
            // 
            // DlgOpen
            // 
            this.DlgOpen.InitialDirectory = MidsContext.Config.BuildsPath;
            this.DlgOpen.DefaultExt = "mbd";
            this.DlgOpen.Filter = "All Supported Formats (*.mbd; *.mxd; *.txt)|*.mbd;*.mxd;*.txt|Character Builds (*.mbd)|*.mbd|Legacy Character Builds (*.mxd;*.txt)|*.mxd;*.txt|Game Export Builds (*.txt)|*.txt";
            // 
            // DlgSave
            // 
            this.DlgSave.InitialDirectory = MidsContext.Config.BuildsPath;
            this.DlgSave.DefaultExt = "mbd";
            this.DlgSave.Filter = "Character Builds (*.mbd)|*.mbd";
            // 
            // tTip
            // 
            this.tTip.AutoPopDelay = 5000;
            this.tTip.InitialDelay = 500;
            this.tTip.ReshowDelay = 100;
            // 
            // lblLocked0
            // 
            this.lblLocked0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))),
                ((int)(((byte)(224)))));
            this.lblLocked0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked0.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked0.ForeColor = System.Drawing.Color.Black;
            this.lblLocked0.Location = new System.Drawing.Point(308, 166);
            this.lblLocked0.Name = "lblLocked0";
            this.lblLocked0.Size = new System.Drawing.Size(92, 29);
            this.lblLocked0.TabIndex = 72;
            this.lblLocked0.Text = "Pool Locked";
            this.lblLocked0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLocked0.Paint += new System.Windows.Forms.PaintEventHandler(this.lblLocked0_Paint);
            this.lblLocked0.MouseLeave += new System.EventHandler(this.lblLocked0_MouseLeave);
            this.lblLocked0.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLocked0_MouseMove);
            // 
            // lblLocked1
            // 
            this.lblLocked1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))),
                ((int)(((byte)(224)))));
            this.lblLocked1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked1.ForeColor = System.Drawing.Color.Black;
            this.lblLocked1.Location = new System.Drawing.Point(308, 186);
            this.lblLocked1.Name = "lblLocked1";
            this.lblLocked1.Size = new System.Drawing.Size(92, 29);
            this.lblLocked1.TabIndex = 73;
            this.lblLocked1.Text = "Pool Locked";
            this.lblLocked1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLocked1.Paint += new System.Windows.Forms.PaintEventHandler(this.lblLocked1_Paint);
            this.lblLocked1.MouseLeave += new System.EventHandler(this.lblLocked0_MouseLeave);
            this.lblLocked1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLocked1_MouseMove);
            // 
            // lblLocked2
            // 
            this.lblLocked2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))),
                ((int)(((byte)(224)))));
            this.lblLocked2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked2.ForeColor = System.Drawing.Color.Black;
            this.lblLocked2.Location = new System.Drawing.Point(304, 194);
            this.lblLocked2.Name = "lblLocked2";
            this.lblLocked2.Size = new System.Drawing.Size(92, 29);
            this.lblLocked2.TabIndex = 74;
            this.lblLocked2.Text = "Pool Locked";
            this.lblLocked2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLocked2.Paint += new System.Windows.Forms.PaintEventHandler(this.lblLocked2_Paint);
            this.lblLocked2.MouseLeave += new System.EventHandler(this.lblLocked0_MouseLeave);
            this.lblLocked2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLocked2_MouseMove);
            // 
            // lblLocked3
            // 
            this.lblLocked3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))),
                ((int)(((byte)(224)))));
            this.lblLocked3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocked3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLocked3.ForeColor = System.Drawing.Color.Black;
            this.lblLocked3.Location = new System.Drawing.Point(284, 210);
            this.lblLocked3.Name = "lblLocked3";
            this.lblLocked3.Size = new System.Drawing.Size(92, 29);
            this.lblLocked3.TabIndex = 75;
            this.lblLocked3.Text = "Pool Locked";
            this.lblLocked3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLocked3.Paint += new System.Windows.Forms.PaintEventHandler(this.lblLocked3_Paint);
            this.lblLocked3.MouseLeave += new System.EventHandler(this.lblLocked0_MouseLeave);
            this.lblLocked3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLocked3_MouseMove);
            // 
            // lblLockedAncillary
            // 
            this.lblLockedAncillary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))),
                ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLockedAncillary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLockedAncillary.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedAncillary.ForeColor = System.Drawing.Color.Black;
            this.lblLockedAncillary.Location = new System.Drawing.Point(268, 230);
            this.lblLockedAncillary.Name = "lblLockedAncillary";
            this.lblLockedAncillary.Size = new System.Drawing.Size(92, 29);
            this.lblLockedAncillary.TabIndex = 76;
            this.lblLockedAncillary.Text = "Pool Locked";
            this.lblLockedAncillary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLockedAncillary.Paint += new System.Windows.Forms.PaintEventHandler(this.lblLockedAncillary_Paint);
            this.lblLockedAncillary.MouseLeave += new System.EventHandler(this.lblLocked0_MouseLeave);
            this.lblLockedAncillary.MouseMove +=
                new System.Windows.Forms.MouseEventHandler(this.lblLockedAncillary_MouseMove);
            // 
            // lblLockedSecondary
            // 
            this.lblLockedSecondary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))),
                ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLockedSecondary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLockedSecondary.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLockedSecondary.ForeColor = System.Drawing.Color.Black;
            this.lblLockedSecondary.Location = new System.Drawing.Point(257, 246);
            this.lblLockedSecondary.Name = "lblLockedSecondary";
            this.lblLockedSecondary.Size = new System.Drawing.Size(92, 29);
            this.lblLockedSecondary.TabIndex = 109;
            this.lblLockedSecondary.Text = "Sec. Locked";
            this.lblLockedSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLockedSecondary.MouseLeave += new System.EventHandler(this.lblLockedSecondary_MouseLeave);
            this.lblLockedSecondary.MouseMove +=
                new System.Windows.Forms.MouseEventHandler(this.lblLockedSecondary_MouseMove);
            // 
            // MenuBar
            // 
            this.MenuBar.BackColor = System.Drawing.SystemColors.Control;
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.FileToolStripMenuItem,
                this.OptionsToolStripMenuItem,
                this.ShareToolStripMenuItem,
                this.CharacterToolStripMenuItem,
                this.ViewToolStripMenuItem,
                this.WindowToolStripMenuItem,
                this.HelpToolStripMenuItem,
                this.EnemyRelativeToolStripComboBox,
                this.DonateToolStripMenuItem
            });
            this.MenuBar.Location = new System.Drawing.Point(0, 0);
            this.MenuBar.Name = "MenuBar";
            this.MenuBar.Size = new System.Drawing.Size(1180, 24);
            this.MenuBar.TabIndex = 84;
            this.MenuBar.Text = "MenuStrip1";
            //
            // DynamicToHitToolStripComboBox
            //
            this.EnemyRelativeToolStripComboBox.FlatStyle = FlatStyle.Flat;
            this.EnemyRelativeToolStripComboBox.ComboBox.BackColor = SystemColors.Control;
            this.EnemyRelativeToolStripComboBox.Name = "EnemyRelativeToolStripComboBox";
            this.EnemyRelativeToolStripComboBox.Size = new Size(185, 22);
            this.EnemyRelativeToolStripComboBox.Text = "Enemy Relative Level";
            this.EnemyRelativeToolStripComboBox.Margin = new Padding(70, 0, 0, 0);
            this.EnemyRelativeToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.EnemyRelativeToolStripComboBox.ComboBox.SelectionChangeCommitted +=
                new EventHandler(this.EnemyRelativeLevel_Changed);
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsFileNew,
                this.ToolStripSeparator7,
                this.tsFileOpen,
                this.tsFileSave,
                this.tsFileSaveAs,
                this.ToolStripSeparator22,
                this.ExportToolStripMenuItem,
                this.LegacyToolStripMenuItem,
                this.ToolStripSeparator8,
                this.tsFilePrint,
                this.ToolStripSeparator9,
                this.tsFileQuit
            });
            this.FileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // tsFileNew
            // 
            this.tsFileNew.Name = "tsFileNew";
            this.tsFileNew.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
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
            this.tsFileOpen.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsFileOpen.Size = new System.Drawing.Size(179, 22);
            this.tsFileOpen.Text = "&Open...";
            this.tsFileOpen.Click += new System.EventHandler(this.tsFileOpen_Click);
            // 
            // tsBuildRcv
            // 
            this.tsBuildRcv.Name = "tsBuildRcv";
            this.tsBuildRcv.Size = new System.Drawing.Size(179, 22);
            this.tsBuildRcv.Text = "Attempt Build recovery...";
            this.tsBuildRcv.Click += new System.EventHandler(this.tsBuildRcv_Click);
            // 
            // tsFileSave
            // 
            this.tsFileSave.Name = "tsFileSave";
            this.tsFileSave.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
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
            // ImportToolStripMenuItem
            //
            this.LegacyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsImport
            });
            this.LegacyToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LegacyToolStripMenuItem.Name = "LegacyToolStripMenuItem";
            this.LegacyToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.LegacyToolStripMenuItem.Text = "Legacy Import...";
            //
            // ExportToolStripMenuItem
            //
            this.ExportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsGenFreebies,
                this.tsGenJsonExport

            });
            this.ExportToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem";
            this.ExportToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.ExportToolStripMenuItem.Text = "Export...";
            // 
            // tsFilePrint
            // 
            this.tsFilePrint.Name = "tsFilePrint";
            this.tsFilePrint.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
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
            this.tsFileQuit.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tsFileQuit.Size = new System.Drawing.Size(179, 22);
            this.tsFileQuit.Text = "&Quit";
            this.tsFileQuit.Click += new System.EventHandler(this.tsFileQuit_Click);
            // 
            // tsImport
            // 
            this.tsImport.Name = "tsImport";
            this.tsImport.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsImport.Size = new System.Drawing.Size(240, 22);
            this.tsImport.Text = "From Forum Post";
            this.tsImport.Click += new System.EventHandler(this.tsImport_Click);
            // 
            // ToolStripSeparator12
            // 
            this.ToolStripSeparator12.Name = "ToolStripSeparator12";
            this.ToolStripSeparator12.Size = new System.Drawing.Size(237, 6);
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
            this.tsGenFreebies.Text = "Export Build to Beta Server (Menu)";
            this.tsGenFreebies.Click += new System.EventHandler(tsGenFreebies_Click);
            //
            // tsGenJsonExport
            //
            this.tsGenJsonExport.Name = "tsGenJsonExport";
            this.tsGenJsonExport.Size = new System.Drawing.Size(240, 22);
            this.tsGenJsonExport.Text = "Export Build to Test Server (Json)";
            this.tsGenJsonExport.Click += new System.EventHandler(tsGenJsonExport_Click);
            //
            // ToolStripSeparator27
            // 
            this.ToolStripSeparator27.Name = "ToolStripSeparator27";
            this.ToolStripSeparator27.Size = new System.Drawing.Size(237, 6);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsChangeDb,
                this.ToolStripSeparator29,
                this.tsConfig,
                this.ToolStripSeparator5,
                this.AdvancedToolStripMenuItem1
            });
            this.OptionsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.OptionsToolStripMenuItem.Text = "&Options";
            // 
            // tsChangeDb
            // 
            this.tsChangeDb.Name = "tsChangeDb";
            this.tsChangeDb.Size = new System.Drawing.Size(199, 22);
            this.tsChangeDb.Text = "&Change Database";
            this.tsChangeDb.Click += new System.EventHandler(this.tsChangeDb_Click);
            //
            // ToolStripSeparator29
            //
            this.ToolStripSeparator29.Name = "ToolStripSeparator29";
            this.ToolStripSeparator29.Size = new System.Drawing.Size(242, 6);
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
            this.tsUpdateCheck.Text = "Check for &Updates";
            this.tsUpdateCheck.Click += new System.EventHandler(this.tsUpdateCheck_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(196, 6);
            // 
            // AdvancedToolStripMenuItem1
            // 
            this.AdvancedToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsAdvDBEdit,
                this.ToolStripSeparator15,
                this.tsAdvFreshInstall,
                this.tsAdvResetTips
            });
            this.AdvancedToolStripMenuItem1.Name = "AdvancedToolStripMenuItem1";
            this.AdvancedToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.AdvancedToolStripMenuItem1.Text = "&Advanced";
            // 
            // tsAdvDBEdit
            // 
            this.tsAdvDBEdit.Name = "tsAdvDBEdit";
            this.tsAdvDBEdit.Size = new System.Drawing.Size(165, 22);
            this.tsAdvDBEdit.Text = "&Database Menu";
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
            this.CharacterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.SetAllIOsToDefault35ToolStripMenuItem,
                this.ToolStripSeparator16,
                this.ToolStripMenuItem1,
                this.ToolStripMenuItem2,
                this.ToolStripSeparator17,
                this.SlotsToolStripMenuItem,
                this.ToolStripSeparator28,
                this.ToggleCheckModeToolStripMenuItem
            });
            this.CharacterToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CharacterToolStripMenuItem.Name = "CharacterToolStripMenuItem";
            this.CharacterToolStripMenuItem.Size = new System.Drawing.Size(133, 20);
            this.CharacterToolStripMenuItem.Text = "&Slots / Enhancements";
            // 
            // SetAllIOsToDefault35ToolStripMenuItem
            // 
            this.SetAllIOsToDefault35ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsIODefault,
                this.ToolStripSeparator11,
                this.tsIOMin,
                this.tsIOMax
            });
            this.SetAllIOsToDefault35ToolStripMenuItem.Name = "SetAllIOsToDefault35ToolStripMenuItem";
            this.SetAllIOsToDefault35ToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
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
            this.ToolStripSeparator16.Size = new System.Drawing.Size(242, 6);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsEnhToSO,
                this.tsEnhToDO,
                this.tsEnhToTO
            });
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(245, 22);
            this.ToolStripMenuItem1.Text = "Set all Enhancements' &Origin to...";
            //
            // ToolStripSeparator28
            //
            this.ToolStripSeparator28.Name = "ToolStripSeparator28";
            this.ToolStripSeparator28.Size = new System.Drawing.Size(242, 6);
            //
            // ToggleCheckModeToolStripMenuItem
            //
            this.ToggleCheckModeToolStripMenuItem.Name = "ToggleCheckModeToolStripMenuItem";
            this.ToggleCheckModeToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.ToggleCheckModeToolStripMenuItem.Text = "Toggle Enhancement Check Mode";
            this.ToggleCheckModeToolStripMenuItem.Click +=
                new System.EventHandler(this.tsToggleCheckModeToolStripMenuItem_Click);
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
            this.ToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsEnhToPlus5,
                this.tsEnhToPlus4,
                this.tsEnhToPlus3,
                this.tsEnhToPlus2,
                this.tsEnhToPlus1,
                this.tsEnhToEven,
                this.tsEnhToMinus1,
                this.tsEnhToMinus2,
                this.tsEnhToMinus3,
                this.tsEnhToNone
            });
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(245, 22);
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
            this.ToolStripSeparator17.Size = new System.Drawing.Size(242, 6);
            // 
            // SlotsToolStripMenuItem
            // 
            this.SlotsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsFlipAllEnh,
                this.ToolStripSeparator4,
                this.tsClearAllEnh,
                this.tsRemoveAllSlots,
                this.ToolStripSeparator1,
                this.AutoArrangeAllSlotsToolStripMenuItem
            });
            this.SlotsToolStripMenuItem.Name = "SlotsToolStripMenuItem";
            this.SlotsToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
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
            this.AutoArrangeAllSlotsToolStripMenuItem.Click +=
                new System.EventHandler(this.AutoArrangeAllSlotsToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
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
                this.tlsDPA,
                this.ToolStripSeparator32,
                this.tsViewBuildComment,
            });
            this.ViewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "&View";
            //
            // tsViewBuildComment
            //
            this.tsViewBuildComment.Name = "tsViewBuildComment";
            this.tsViewBuildComment.Size = new System.Drawing.Size(282, 22);
            this.tsViewBuildComment.Text = "Build comment...";
            this.tsViewBuildComment.Click += new System.EventHandler(this.tsViewBuildComment_Click);
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
            // tsViewSOLevels
            // 
            this.tsViewSOLevels.Checked = false;
            this.tsViewSOLevels.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.tsViewSOLevels.Name = "tsViewSOLevels";
            this.tsViewSOLevels.Size = new System.Drawing.Size(282, 22);
            this.tsViewSOLevels.Text = "Show SO/HO Levels";
            this.tsViewSOLevels.Click += new System.EventHandler(tsViewSOLevels_Click);
            // 
            // tsViewRelative
            // 
            this.tsViewRelative.Name = "tsViewRelative";
            this.tsViewRelative.Size = new System.Drawing.Size(282, 22);
            this.tsViewRelative.Text = "Show &Enhancement Relative Levels";
            this.tsViewRelative.Click += new System.EventHandler(this.tsViewRelative_Click);
            // 
            // tsViewRelativeAsSigns
            // 
            this.tsViewRelativeAsSigns.Checked = false;
            this.tsViewRelativeAsSigns.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.tsViewRelativeAsSigns.Name = "tsViewRelativeAsSigns";
            this.tsViewRelativeAsSigns.Size = new System.Drawing.Size(282, 22);
            this.tsViewRelativeAsSigns.Text = "Show Relative Levels with signs ('+'/'-')";
            this.tsViewRelativeAsSigns.Click += new System.EventHandler(tsViewRelativeAsSigns_Click);
            // 
            // tsViewSlotLevels
            // 
            this.tsViewSlotLevels.Name = "tsViewSlotLevels";
            this.tsViewSlotLevels.Size = new System.Drawing.Size(282, 22);
            this.tsViewSlotLevels.Text = "Show &Slot Placement Levels";
            this.tsViewSlotLevels.Click += new System.EventHandler(this.tsViewSlotLevels_Click);
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
            // ShareToolStripMenuItem1
            //
            this.ShareToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsShareDiscord,
                this.tsShareMenu,
                this.ToolStripSeparator27,
                this.tsImportDataChunk
            });
            this.ShareToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ShareToolStripMenuItem.Name = "ShareToolStripMenuItem1";
            this.ShareToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.ShareToolStripMenuItem.Text = "&Build Sharing";
            //
            // ToolStripSeparator30
            //
            this.ToolStripSeparator30.Name = "ToolStripSeparator30";
            this.ToolStripSeparator30.Size = new System.Drawing.Size(242, 6);
            //
            // ToolStripSeparator31
            //
            this.ToolStripSeparator31.Name = "ToolStripSeparator31";
            this.ToolStripSeparator31.Size = new System.Drawing.Size(242, 6);
            // 
            // tsImportShortCode
            //
            this.tsImportDataChunk.Name = "tsImportDataChunk";
            this.tsImportDataChunk.Size = new System.Drawing.Size(240, 22);
            this.tsImportDataChunk.Text = "Import via DataChunk";
            this.tsImportDataChunk.Click += new System.EventHandler(this.tsImportShortCode_Click);
            // 
            // tsShareDiscord
            //
            this.tsShareDiscord.Name = "tsShareDiscord";
            this.tsShareDiscord.Size = new System.Drawing.Size(240, 22);
            this.tsShareDiscord.Text = "Via Discord";
            this.tsShareDiscord.Click += new System.EventHandler(this.tsShareDiscord_Click);
            // 
            // tsShareForum2
            //
            this.tsShareMenu.Name = "tsShareMenu";
            this.tsShareMenu.Size = new System.Drawing.Size(240, 22);
            this.tsShareMenu.Text = "Open Share Menu";
            this.tsShareMenu.Click += new System.EventHandler(this.ShareMenu_Click);
            //
            // HelpToolStripMenuItem
            //
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsHelp,
                this.tsUpdateCheck,
                this.ToolStripSeparator10,
                this.tsBuildRcv,
                this.ToolStripSeparator30,
                this.tsGitHub,
                this.tsSupport,
                this.ToolStripSeparator31,
                this.tsAbout
            });
            this.HelpToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.HelpToolStripMenuItem.Text = "Help";
            // 
            // tsHelp
            // 
            this.tsHelp.Name = "tsHelp";
            this.tsHelp.Size = new System.Drawing.Size(266, 22);
            // 
            // ToolStripSeparator10
            // 
            this.ToolStripSeparator10.Name = "ToolStripSeparator10";
            this.ToolStripSeparator10.Size = new System.Drawing.Size(263, 6);
            // 
            // tsSupport
            // 
            this.tsSupport.Name = "tsSupport";
            this.tsSupport.Size = new System.Drawing.Size(266, 22);
            this.tsSupport.Text = "Support (Discord)";
            this.tsSupport.Click += new System.EventHandler(this.tsSupport_Click);
            // 
            // tsAbout
            // 
            this.tsAbout.Name = "tsAbout";
            this.tsAbout.Size = new System.Drawing.Size(266, 22);
            this.tsAbout.Text = "About...";
            this.tsAbout.Click += new System.EventHandler(this.tsAbout_Click);
            // 
            // ToolStripSeparator23
            // 
            this.ToolStripSeparator23.Name = "ToolStripSeparator23";
            this.ToolStripSeparator23.Size = new System.Drawing.Size(263, 6);
            //
            // DonateToolStripMenuItem
            //
            this.DonateToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.DonateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsKoFi,
                this.tsPatreon,
                this.tsCoinbase
            });
            this.DonateToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DonateToolStripMenuItem.Name = "DonateToolStripMenuItem";
            this.DonateToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.DonateToolStripMenuItem.Text = "Donate?";
            // 
            // tsKoFi
            // 
            this.tsKoFi.Name = "tsKoFi";
            this.tsKoFi.Size = new System.Drawing.Size(266, 22);
            this.tsKoFi.Text = "Donate via Ko-Fi";
            this.tsKoFi.Click += new System.EventHandler(this.tsKoFi_Click);
            // 
            // ToolStripSeparator24
            // 
            this.ToolStripSeparator24.Name = "ToolStripSeparator24";
            this.ToolStripSeparator24.Size = new System.Drawing.Size(263, 6);
            // 
            // tsPatreon
            // 
            this.tsPatreon.Name = "tsPatreon";
            this.tsPatreon.Size = new System.Drawing.Size(266, 22);
            this.tsPatreon.Text = "Donate via Patreon";
            this.tsPatreon.Click += new System.EventHandler(this.tsPatreon_Click);
            // 
            // tsCoinbase
            // 
            this.tsCoinbase.Name = "tsCoinbase";
            this.tsCoinbase.Size = new System.Drawing.Size(266, 22);
            this.tsCoinbase.Text = "Donate Crypto via Coinbase";
            this.tsCoinbase.Click += new System.EventHandler(this.tsCoinbase_Click);
            // 
            // tsGitHub
            // 
            this.tsGitHub.Name = "tsGitHub";
            this.tsGitHub.Size = new System.Drawing.Size(266, 22);
            this.tsGitHub.Text = "View Our GitHub";
            this.tsGitHub.Click += new System.EventHandler(this.Github_Link);
            // 
            // WindowToolStripMenuItem
            // 
            this.WindowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsViewSets,
                this.tsViewGraphs,
                this.tsViewSetCompare,
                this.tsViewData,
                this.tsViewTotals,
                this.ToolStripSeparator18,
                this.tsRecipeViewer,
                //this.tsDPSCalc,
                //this.ToolStripSeparator19,
                this.tsSetFind,
                this.ToolStripSeparator21,
                this.InGameRespecHelperToolStripMenuItem
            });
            this.WindowToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.WindowToolStripMenuItem.Name = "WindowToolStripMenuItem";
            this.WindowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.WindowToolStripMenuItem.Text = "&Windows";
            // 
            // tsViewSets
            // 
            this.tsViewSets.Name = "tsViewSets";
            this.tsViewSets.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.tsViewSets.Size = new System.Drawing.Size(232, 22);
            this.tsViewSets.Text = "&Sets && Bonuses";
            this.tsViewSets.Click += new System.EventHandler(this.tsViewSets_Click);
            // 
            // tsViewGraphs
            // 
            this.tsViewGraphs.Name = "tsViewGraphs";
            this.tsViewGraphs.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.tsViewGraphs.Size = new System.Drawing.Size(232, 22);
            this.tsViewGraphs.Text = "Power &Graphs";
            this.tsViewGraphs.Click += new System.EventHandler(this.tsViewGraphs_Click);
            // 
            // tsViewSetCompare
            // 
            this.tsViewSetCompare.Name = "tsViewSetCompare";
            this.tsViewSetCompare.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsViewSetCompare.Size = new System.Drawing.Size(232, 22);
            this.tsViewSetCompare.Text = "Powerset &Comparison";
            this.tsViewSetCompare.Click += new System.EventHandler(this.tsViewSetCompare_Click);
            // 
            // tsViewData
            // 
            this.tsViewData.Name = "tsViewData";
            this.tsViewData.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.tsViewData.Size = new System.Drawing.Size(232, 22);
            this.tsViewData.Text = "&Data View";
            this.tsViewData.Click += new System.EventHandler(this.tsViewData_Click);
            // 
            // tsViewTotals
            // 
            this.tsViewTotals.Name = "tsViewTotals";
            this.tsViewTotals.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
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
            this.tsRecipeViewer.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsRecipeViewer.Size = new System.Drawing.Size(232, 22);
            this.tsRecipeViewer.Text = "&Recipe Viewer";
            this.tsRecipeViewer.Click += new System.EventHandler(this.tsRecipeViewer_Click);
            // 
            // tsDPSCalc
            // 
            this.tsDPSCalc.Name = "tsDPSCalc";
            this.tsDPSCalc.ShortcutKeys =
                ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
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
            this.InGameRespecHelperToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsHelperShort,
                this.tsHelperLong,
                this.ToolStripSeparator20,
                this.tsHelperShort2,
                this.tsHelperLong2
            });
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
            // topPanel
            // 
            this.topPanel.Controls.Add(this.lblCharacter);
            this.topPanel.Controls.Add(this.ibTeamEx);
            this.topPanel.Controls.Add(this.ibAlignmentEx);
            this.topPanel.Controls.Add(this.ibPetsEx);
            this.topPanel.Controls.Add(this.ibTempPowersEx);
            this.topPanel.Controls.Add(this.ibAccoladesEx);
            this.topPanel.Controls.Add(this.ibIncarnatePowersEx);
            this.topPanel.Controls.Add(this.ibPrestigePowersEx);
            this.topPanel.Controls.Add(this.ibPvXEx);
            this.topPanel.Controls.Add(this.ibRecipeEx);
            this.topPanel.Controls.Add(this.ibPopupEx);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 24);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1180, 53);
            this.topPanel.TabIndex = 90;
            // 
            // lblHero
            // 
            this.lblCharacter.AutoSize = true;
            this.lblCharacter.BackColor = System.Drawing.Color.Transparent;
            this.lblCharacter.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblCharacter.ForeColor = System.Drawing.Color.White;
            this.lblCharacter.Location = new System.Drawing.Point(3, 3);
            this.lblCharacter.Name = "lblCharacter";
            this.lblCharacter.Size = new System.Drawing.Size(310, 15);
            this.lblCharacter.TabIndex = 43;
            this.lblCharacter.Text = "Name: Level 0 Origin Archetype (Primary / Secondary)";
            // 
            // ibAlignmentEx
            // 
            this.ibAlignmentEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibAlignmentEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibAlignmentEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibAlignmentEx.CurrentText = "ibAlignmentEx";
            this.ibAlignmentEx.Images.Background = Resources.HeroButton;
            this.ibAlignmentEx.Images.Hover = Resources.HeroButtonHover;
            this.ibAlignmentEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibAlignmentEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibAlignmentEx.Location = new System.Drawing.Point(713, 3);
            this.ibAlignmentEx.Lock = false;
            this.ibAlignmentEx.Name = "ibAlignmentEx";
            this.ibAlignmentEx.Size = new System.Drawing.Size(105, 22);
            this.ibAlignmentEx.TabIndex = 116;
            this.ibAlignmentEx.Text = "ibAlignmentEx";
            this.ibAlignmentEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibAlignmentEx.TextOutline.Width = 3;
            this.ibAlignmentEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibAlignmentEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibAlignmentEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibAlignmentEx.ToggleText.ToggledOff = "Hero";
            this.ibAlignmentEx.ToggleText.ToggledOn = "Villain";
            this.ibAlignmentEx.UseAlt = false;
            this.ibAlignmentEx.Click += new EventHandler(this.ibAlignmentEx_OnClick);
            // 
            // ibPetsEx
            // 
            this.ibPetsEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibPetsEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibPetsEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibPetsEx.CurrentText = "ibPetsEx";
            this.ibPetsEx.Images.Background = Resources.HeroButton;
            this.ibPetsEx.Images.Hover = Resources.HeroButtonHover;
            this.ibPetsEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibPetsEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibPetsEx.Location = new System.Drawing.Point(602, 29);
            this.ibPetsEx.Lock = false;
            this.ibPetsEx.Name = "ibPetsEx";
            this.ibPetsEx.Size = new System.Drawing.Size(105, 22);
            this.ibPetsEx.TabIndex = 116;
            this.ibPetsEx.Text = "Pet Powers";
            this.ibPetsEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibPetsEx.TextOutline.Width = 3;
            this.ibPetsEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibPetsEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibPetsEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibPetsEx.ToggleText.ToggledOff = "Toggled Off State";
            this.ibPetsEx.ToggleText.ToggledOn = "Toggled On State";
            this.ibPetsEx.UseAlt = false;
            this.ibPetsEx.Click += new EventHandler(this.ibPetsEx_OnClick);
            // 
            // ibTempPowersEx
            // 
            this.ibTempPowersEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibTempPowersEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibTempPowersEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibTempPowersEx.Images.Background = Resources.HeroButton;
            this.ibTempPowersEx.Images.Hover = Resources.HeroButtonHover;
            this.ibTempPowersEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibTempPowersEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibTempPowersEx.Location = new System.Drawing.Point(1046, 29);
            this.ibTempPowersEx.Lock = false;
            this.ibTempPowersEx.Name = "ibTempPowersEx";
            this.ibTempPowersEx.Size = new System.Drawing.Size(105, 22);
            this.ibTempPowersEx.TabIndex = 116;
            this.ibTempPowersEx.Text = "Temp Powers";
            this.ibTempPowersEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibTempPowersEx.TextOutline.Width = 3;
            this.ibTempPowersEx.ThreeState = false;
            this.ibTempPowersEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibTempPowersEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibTempPowersEx.ToggleText.ToggledOff = "Temp Powers: Off";
            this.ibTempPowersEx.ToggleText.ToggledOn = "Temp Powers: On";
            this.ibTempPowersEx.UseAlt = false;
            this.ibTempPowersEx.Click += new EventHandler(ibTempPowersEx_OnClick);
            // 
            // ibAccoladesEx
            // 
            this.ibAccoladesEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibAccoladesEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibAccoladesEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibAccoladesEx.CurrentText = "ibAccoladesEx";
            this.ibAccoladesEx.Images.Background = Resources.HeroButton;
            this.ibAccoladesEx.Images.Hover = Resources.HeroButtonHover;
            this.ibAccoladesEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibAccoladesEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibAccoladesEx.Location = new System.Drawing.Point(824, 29);
            this.ibAccoladesEx.Lock = false;
            this.ibAccoladesEx.Name = "ibAccoladesEx";
            this.ibAccoladesEx.Size = new System.Drawing.Size(105, 22);
            this.ibAccoladesEx.TabIndex = 116;
            this.ibAccoladesEx.Text = "Accolades";
            this.ibAccoladesEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibAccoladesEx.TextOutline.Width = 3;
            this.ibAccoladesEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibAccoladesEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibAccoladesEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibAccoladesEx.ToggleText.ToggledOff = "Accolades: Off";
            this.ibAccoladesEx.ToggleText.ToggledOn = "Accolades: On";
            this.ibAccoladesEx.UseAlt = false;
            this.ibAccoladesEx.Click += new EventHandler(this.ibAccoladesEx_OnClick);
            // 
            // ibIncarnatePowersEx
            // 
            this.ibIncarnatePowersEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibIncarnatePowersEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibIncarnatePowersEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibIncarnatePowersEx.Images.Background = Resources.HeroButton;
            this.ibIncarnatePowersEx.Images.Hover = Resources.HeroButtonHover;
            this.ibIncarnatePowersEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibIncarnatePowersEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibIncarnatePowersEx.Location = new System.Drawing.Point(935, 29);
            this.ibIncarnatePowersEx.Lock = false;
            this.ibIncarnatePowersEx.Name = "ibIncarnatePowersEx";
            this.ibIncarnatePowersEx.Size = new System.Drawing.Size(105, 22);
            this.ibIncarnatePowersEx.TabIndex = 116;
            this.ibIncarnatePowersEx.Text = "Incarnate Powers";
            this.ibIncarnatePowersEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibIncarnatePowersEx.TextOutline.Width = 3;
            this.ibIncarnatePowersEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibIncarnatePowersEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibIncarnatePowersEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibIncarnatePowersEx.ToggleText.ToggledOff = "Toggled Off State";
            this.ibIncarnatePowersEx.ToggleText.ToggledOn = "Toggled On State";
            this.ibIncarnatePowersEx.UseAlt = false;
            this.ibIncarnatePowersEx.Click += new EventHandler(this.ibIncarnatesEx_OnClick);
            // 
            // ibPrestigePowersEx
            // 
            this.ibPrestigePowersEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibPrestigePowersEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibPrestigePowersEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibPrestigePowersEx.Images.Background = Resources.HeroButton;
            this.ibPrestigePowersEx.Images.Hover = Resources.HeroButtonHover;
            this.ibPrestigePowersEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibPrestigePowersEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibPrestigePowersEx.Location = new System.Drawing.Point(713, 29);
            this.ibPrestigePowersEx.Lock = false;
            this.ibPrestigePowersEx.Name = "ibPrestigePowersEx";
            this.ibPrestigePowersEx.Size = new System.Drawing.Size(105, 22);
            this.ibPrestigePowersEx.TabIndex = 116;
            this.ibPrestigePowersEx.Text = "Prestige Powers";
            this.ibPrestigePowersEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibPrestigePowersEx.TextOutline.Width = 3;
            this.ibPrestigePowersEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibPrestigePowersEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibPrestigePowersEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibPrestigePowersEx.ToggleText.ToggledOff = "ToggledOff State";
            this.ibPrestigePowersEx.ToggleText.ToggledOn = "ToggledOn State";
            this.ibPrestigePowersEx.UseAlt = false;
            this.ibPrestigePowersEx.Click += new EventHandler(this.ibPrestigePowersEx_OnClick);
            // 
            // ibPvXEx
            // 
            this.ibPvXEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibPvXEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibPvXEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibPvXEx.Images.Background = Resources.HeroButton;
            this.ibPvXEx.Images.Hover = Resources.HeroButtonHover;
            this.ibPvXEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibPvXEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibPvXEx.Location = new System.Drawing.Point(824, 3);
            this.ibPvXEx.Lock = false;
            this.ibPvXEx.Name = "ibPvXEx";
            this.ibPvXEx.Size = new System.Drawing.Size(105, 22);
            this.ibPvXEx.TabIndex = 116;
            this.ibPvXEx.Text = "ibPvXEx";
            this.ibPvXEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibPvXEx.TextOutline.Width = 3;
            this.ibPvXEx.ThreeState = false;
            this.ibPvXEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibPvXEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibPvXEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibPvXEx.ToggleText.ToggledOff = "PvE";
            this.ibPvXEx.ToggleText.ToggledOn = "PvP";
            this.ibPvXEx.UseAlt = false;
            this.ibPvXEx.Click += new EventHandler(this.ibPvXEx_OnClick);
            // 
            // ibRecipeEx
            // 
            this.ibRecipeEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibRecipeEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibRecipeEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibRecipeEx.Images.Background = Resources.HeroButton;
            this.ibRecipeEx.Images.Hover = Resources.HeroButtonHover;
            this.ibRecipeEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibRecipeEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibRecipeEx.Location = new System.Drawing.Point(935, 3);
            this.ibRecipeEx.Lock = false;
            this.ibRecipeEx.Name = "ibRecipeEx";
            this.ibRecipeEx.Size = new System.Drawing.Size(105, 22);
            this.ibRecipeEx.TabIndex = 116;
            this.ibRecipeEx.Text = "ibRecipeEx";
            this.ibRecipeEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibRecipeEx.TextOutline.Width = 3;
            this.ibRecipeEx.ThreeState = false;
            this.ibRecipeEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibRecipeEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibRecipeEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibRecipeEx.ToggleText.ToggledOff = "Recipes: Off";
            this.ibRecipeEx.ToggleText.ToggledOn = "Recipes: On";
            this.ibRecipeEx.UseAlt = false;
            this.ibRecipeEx.Click += new EventHandler(this.ibRecipeEx_OnClick);
            // 
            // ibPopupEx
            // 
            this.ibPopupEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibPopupEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibPopupEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibPopupEx.Images.Background = Resources.HeroButton;
            this.ibPopupEx.Images.Hover = Resources.HeroButtonHover;
            this.ibPopupEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibPopupEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibPopupEx.Location = new System.Drawing.Point(1046, 3);
            this.ibPopupEx.Lock = false;
            this.ibPopupEx.Name = "ibPopupEx";
            this.ibPopupEx.Size = new System.Drawing.Size(105, 22);
            this.ibPopupEx.TabIndex = 116;
            this.ibPopupEx.Text = "ibPopupEx";
            this.ibPopupEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibPopupEx.TextOutline.Width = 3;
            this.ibPopupEx.ThreeState = false;
            this.ibPopupEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibPopupEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibPopupEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibPopupEx.ToggleText.ToggledOff = "Popup: Off";
            this.ibPopupEx.ToggleText.ToggledOn = "Popup: On";
            this.ibPopupEx.UseAlt = false;
            this.ibPopupEx.Click += new EventHandler(this.ibPopupEx_OnClick);
            // 
            // ToolStripSeparator26
            // 
            this.ToolStripSeparator26.Name = "ToolStripSeparator26";
            this.ToolStripSeparator26.Size = new System.Drawing.Size(255, 6);
            // 
            // ibDynMode
            // 
            this.ibDynMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibDynMode.ButtonType = Mids_Reborn.Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            this.ibDynMode.Font = new System.Drawing.Font("MS Sans Serif", 11F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point);
            this.ibDynMode.ForeColor = Color.White;
            this.ibDynMode.Images.Background = Resources.HeroButton;
            this.ibDynMode.Images.Hover = Resources.HeroButtonHover;
            this.ibDynMode.ImagesAlt.Background = Resources.VillainButton;
            this.ibDynMode.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibDynMode.Location = new System.Drawing.Point(355, 80);
            this.ibDynMode.Name = "ibDynMode";
            this.ibDynMode.Size = new System.Drawing.Size(105, 22);
            this.ibDynMode.TabIndex = 122;
            this.ibDynMode.Text = "ibDynMode1";
            this.ibDynMode.TextOutline.Color = System.Drawing.Color.Black;
            this.ibDynMode.TextOutline.Width = 3;
            this.ibDynMode.ThreeState = false;
            this.ibDynMode.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibDynMode.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibDynMode.ToggleText.Indeterminate = "Power / Slot";
            this.ibDynMode.ToggleText.ToggledOff = "Power";
            this.ibDynMode.ToggleText.ToggledOn = "Slot";
            this.ibDynMode.UseAlt = false;
            this.ibDynMode.Click += new EventHandler(ibDynMode_Click);
            // 
            // pnlGFX
            // 
            this.pnlGFX.BackColor = System.Drawing.Color.Black;
            this.pnlGFX.Location = new System.Drawing.Point(3, 3);
            this.pnlGFX.Name = "pnlGFX";
            this.pnlGFX.Size = new System.Drawing.Size(838, 885);
            this.pnlGFX.TabIndex = 103;
            this.pnlGFX.TabStop = false;
            this.pnlGFX.DragDrop += new System.Windows.Forms.DragEventHandler(this.pnlGFX_DragDrop);
            this.pnlGFX.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnlGFX_DragEnter);
            this.pnlGFX.DragOver += new System.Windows.Forms.DragEventHandler(this.pnlGFX_DragOver);
            this.pnlGFX.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pnlGFX_MouseDoubleClick);
            this.pnlGFX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlGFX_MouseDown);
            this.pnlGFX.MouseEnter += new System.EventHandler(this.pnlGFX_MouseEnter);
            this.pnlGFX.MouseLeave += new System.EventHandler(this.pnlGFX_MouseLeave);
            this.pnlGFX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlGFX_MouseMove);
            this.pnlGFX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlGFX_MouseUp);
            // 
            // pnlGFXFlow
            // 
            this.pnlGFXFlow.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.pnlGFXFlow.AutoScroll = true;
            this.pnlGFXFlow.Controls.Add(this.pnlGFX);
            this.pnlGFXFlow.Location = new System.Drawing.Point(480, 80);
            this.pnlGFXFlow.Name = "pnlGFXFlow";
            this.pnlGFXFlow.Size = new System.Drawing.Size(850, 891);
            this.pnlGFXFlow.TabIndex = 112;
            this.pnlGFXFlow.MouseEnter += new System.EventHandler(this.pnlGFXFlow_MouseEnter);
            this.pnlGFXFlow.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlGFXFlow_Scroll);
            // 
            // llPrimary
            // 
            this.llPrimary.Expandable = true;
            this.llPrimary.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llPrimary.HighVis = true;
            this.llPrimary.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llPrimary.Location = new System.Drawing.Point(16, 206);
            this.llPrimary.MaxHeight = 600;
            this.llPrimary.Name = "llPrimary";
            this.llPrimary.PaddingX = 2;
            this.llPrimary.PaddingY = 2;
            this.llPrimary.Scrollable = true;
            this.llPrimary.ScrollBarColor = System.Drawing.Color.Red;
            this.llPrimary.ScrollBarWidth = 11;
            this.llPrimary.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.llPrimary.Size = new System.Drawing.Size(145, 160);
            this.llPrimary.SizeNormal = new System.Drawing.Size(145, 160);
            this.llPrimary.SuspendRedraw = false;
            this.llPrimary.TabIndex = 107;
            this.llPrimary.ItemHover += new ListLabelV3.ItemHoverEventHandler(llPrimary_ItemHover);
            this.llPrimary.ItemClick += new ListLabelV3.ItemClickEventHandler(llPrimary_ItemClick);
            this.llPrimary.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            this.llPrimary.ExpandChanged += new ListLabelV3.ExpandChangedEventHandler(PriSec_ExpandChanged);
            // 
            // llSecondary
            // 
            this.llSecondary.Expandable = true;
            this.llSecondary.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llSecondary.HighVis = true;
            this.llSecondary.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llSecondary.Location = new System.Drawing.Point(170, 206);
            this.llSecondary.MaxHeight = 600;
            this.llSecondary.Name = "llSecondary";
            this.llSecondary.PaddingX = 2;
            this.llSecondary.PaddingY = 2;
            this.llSecondary.Scrollable = true;
            this.llSecondary.ScrollBarColor = System.Drawing.Color.Red;
            this.llSecondary.ScrollBarWidth = 11;
            this.llSecondary.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.llSecondary.Size = new System.Drawing.Size(145, 160);
            this.llSecondary.SizeNormal = new System.Drawing.Size(145, 160);
            this.llSecondary.SuspendRedraw = false;
            this.llSecondary.TabIndex = 108;
            this.llSecondary.ItemHover += new ListLabelV3.ItemHoverEventHandler(llSecondary_ItemHover);
            this.llSecondary.ItemClick += new ListLabelV3.ItemClickEventHandler(llSecondary_ItemClick);
            this.llSecondary.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            this.llSecondary.ExpandChanged += new ListLabelV3.ExpandChangedEventHandler(PriSec_ExpandChanged);
            // 
            // llPool0
            // 
            this.llPool0.Expandable = true;
            this.llPool0.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llPool0.HighVis = true;
            this.llPool0.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llPool0.Location = new System.Drawing.Point(0, 47); // 328, 206
            this.llPool0.MaxHeight = 500;
            this.llPool0.Name = "llPool0";
            this.llPool0.PaddingX = 2;
            this.llPool0.PaddingY = 2;
            this.llPool0.Scrollable = true;
            this.llPool0.ScrollBarColor = System.Drawing.Color.FromArgb(128, 96, 192);
            this.llPool0.ScrollBarWidth = 11;
            this.llPool0.ScrollButtonColor = System.Drawing.Color.FromArgb(96, 0, 192);
            this.llPool0.Size = new System.Drawing.Size(145, 69);
            this.llPool0.SizeNormal = new System.Drawing.Size(145, 69);
            this.llPool0.SuspendRedraw = false;
            this.llPool0.TabIndex = 34;
            this.llPool0.ItemHover += new ListLabelV3.ItemHoverEventHandler(llPool0_ItemHover);
            this.llPool0.ItemClick += new ListLabelV3.ItemClickEventHandler(llPool0_ItemClick);
            //this.llPool0.MouseLeave += new System.EventHandler(llALL_MouseLeave);
            this.llPool0.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            // 
            // llPool1
            // 
            this.llPool1.Expandable = true;
            this.llPool1.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llPool1.ForeColor = System.Drawing.Color.Yellow;
            this.llPool1.HighVis = true;
            this.llPool1.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llPool1.Location = new System.Drawing.Point(0, 152); // 328, 314
            this.llPool1.MaxHeight = 500;
            this.llPool1.Name = "llPool1";
            this.llPool1.PaddingX = 2;
            this.llPool1.PaddingY = 2;
            this.llPool1.Scrollable = true;
            this.llPool1.ScrollBarColor = System.Drawing.Color.FromArgb(128, 96, 192);
            this.llPool1.ScrollBarWidth = 11;
            this.llPool1.ScrollButtonColor = System.Drawing.Color.FromArgb(96, 0, 192);
            this.llPool1.Size = new System.Drawing.Size(145, 69);
            this.llPool1.SizeNormal = new System.Drawing.Size(145, 69);
            this.llPool1.SuspendRedraw = false;
            this.llPool1.TabIndex = 35;
            this.llPool1.ItemHover += new ListLabelV3.ItemHoverEventHandler(llPool1_ItemHover);
            this.llPool1.ItemClick += new ListLabelV3.ItemClickEventHandler(llPool1_ItemClick);
            //this.llPool1.MouseLeave += new System.EventHandler(llALL_MouseLeave);
            this.llPool1.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            // 
            // llPool2
            // 
            this.llPool2.Expandable = true;
            this.llPool2.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llPool2.ForeColor = System.Drawing.Color.Yellow;
            this.llPool2.HighVis = true;
            this.llPool2.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llPool2.Location = new System.Drawing.Point(0, 266); // 328, 422
            this.llPool2.MaxHeight = 500;
            this.llPool2.Name = "llPool2";
            this.llPool2.PaddingX = 2;
            this.llPool2.PaddingY = 2;
            this.llPool2.Scrollable = true;
            this.llPool2.ScrollBarColor = System.Drawing.Color.FromArgb(128, 96, 192);
            this.llPool2.ScrollBarWidth = 11;
            this.llPool2.ScrollButtonColor = System.Drawing.Color.FromArgb(96, 0, 192);
            this.llPool2.Size = new System.Drawing.Size(145, 69);
            this.llPool2.SizeNormal = new System.Drawing.Size(145, 69);
            this.llPool2.SuspendRedraw = false;
            this.llPool2.TabIndex = 36;
            this.llPool2.ItemHover += new ListLabelV3.ItemHoverEventHandler(llPool2_ItemHover);
            this.llPool2.ItemClick += new ListLabelV3.ItemClickEventHandler(llPool2_ItemClick);
            //this.llPool2.MouseLeave += new System.EventHandler(llALL_MouseLeave);
            this.llPool2.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            // 
            // llPool3
            // 
            this.llPool3.Expandable = true;
            this.llPool3.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llPool3.ForeColor = System.Drawing.Color.Yellow;
            this.llPool3.HighVis = true;
            this.llPool3.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llPool3.Location = new System.Drawing.Point(0, 374); // 328, 530
            this.llPool3.MaxHeight = 500;
            this.llPool3.Name = "llPool3";
            this.llPool3.PaddingX = 2;
            this.llPool3.PaddingY = 2;
            this.llPool3.Scrollable = true;
            this.llPool3.ScrollBarColor = System.Drawing.Color.FromArgb(128, 96, 192);
            this.llPool3.ScrollBarWidth = 11;
            this.llPool3.ScrollButtonColor = System.Drawing.Color.FromArgb(96, 0, 192);
            this.llPool3.Size = new System.Drawing.Size(145, 69);
            this.llPool3.SizeNormal = new System.Drawing.Size(145, 69);
            this.llPool3.SuspendRedraw = false;
            this.llPool3.TabIndex = 37;
            this.llPool3.ItemHover += new ListLabelV3.ItemHoverEventHandler(llPool3_ItemHover);
            this.llPool3.ItemClick += new ListLabelV3.ItemClickEventHandler(llPool3_ItemClick);
            //this.llPool3.MouseLeave += new System.EventHandler(llALL_MouseLeave);
            this.llPool3.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            // 
            // llAncillary
            // 
            this.llAncillary.Expandable = true;
            this.llAncillary.Font = new System.Drawing.Font(Fonts.Family("Noto Sans"), 12f, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.llAncillary.HighVis = true;
            this.llAncillary.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.llAncillary.Location = new System.Drawing.Point(0, 477); // 328, 638
            this.llAncillary.MaxHeight = 500;
            this.llAncillary.Name = "llAncillary";
            this.llAncillary.PaddingX = 2;
            this.llAncillary.PaddingY = 2;
            this.llAncillary.Scrollable = true;
            this.llAncillary.ScrollBarColor = System.Drawing.Color.Red;
            this.llAncillary.ScrollBarWidth = 11;
            this.llAncillary.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.llAncillary.Size = new System.Drawing.Size(145, 69);
            this.llAncillary.SizeNormal = new System.Drawing.Size(145, 69);
            this.llAncillary.SuspendRedraw = false;
            this.llAncillary.TabIndex = 110;
            this.llAncillary.ItemHover += new ListLabelV3.ItemHoverEventHandler(llAncillary_ItemHover);
            this.llAncillary.ItemClick += new ListLabelV3.ItemClickEventHandler(llAncillary_ItemClick);
            this.llAncillary.EmptyHover += new ListLabelV3.EmptyHoverEventHandler(llAll_EmptyHover);
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Text = "Name:";
            this.lblName.Location = new System.Drawing.Point(2, 82);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(92, 21);
            this.lblName.TabIndex = 44;
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOrigin
            // 
            this.lblOrigin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrigin.Text = "Origin:";
            this.lblOrigin.Location = new System.Drawing.Point(2, 133);
            this.lblOrigin.Name = "lblOrigin";
            this.lblOrigin.Size = new System.Drawing.Size(92, 21);
            this.lblOrigin.TabIndex = 46;
            this.lblOrigin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAT
            // 
            this.lblAT.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAT.Text = "Archetype:";
            this.lblAT.Location = new System.Drawing.Point(2, 109);
            this.lblAT.Name = "lblAT";
            this.lblAT.Size = new System.Drawing.Size(92, 21);
            this.lblAT.TabIndex = 45;
            this.lblAT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // i9Picker
            // 
            this.i9Picker.BackColor = System.Drawing.Color.Black;
            this.i9Picker.ForeColor = System.Drawing.Color.Blue;
            this.i9Picker.Highlight = System.Drawing.Color.MediumSlateBlue;
            this.i9Picker.ImageSize = 30;
            this.i9Picker.Location = new System.Drawing.Point(452, 131);
            this.i9Picker.Name = "i9Picker";
            this.i9Picker.Selected = System.Drawing.Color.SlateBlue;
            this.i9Picker.Size = new System.Drawing.Size(198, 235);
            this.i9Picker.TabIndex = 83;
            this.i9Picker.Visible = false;
            this.i9Picker.EnhancementPicked += new I9Picker.EnhancementPickedEventHandler(this.I9Picker_EnhancementPicked);
            this.i9Picker.EnhancementSelectionCancelled += new I9Picker.EnhancementSelectionCancelledEventHandler(this.I9Picker_EnhancementSelectionCancelled);
            this.i9Picker.HoverEnhancement += new I9Picker.HoverEnhancementEventHandler(this.I9Picker_HoverEnhancement);
            this.i9Picker.HoverSet += new I9Picker.HoverSetEventHandler(this.I9Picker_HoverSet);
            this.i9Picker.Moved += new I9Picker.MovedEventHandler(this.I9Picker_Moved);
            this.i9Picker.MouseDown += new System.Windows.Forms.MouseEventHandler(this.I9Picker_MouseDown);
            this.i9Picker.MouseEnter += new System.EventHandler(this.I9Picker_MouseEnter);
            this.i9Picker.MouseLeave += new System.EventHandler(this.I9Picker_MouseLeave);
            this.i9Picker.KeyDown += new KeyEventHandler(this.I9Picker_KeyDown);
            // 
            // I9Popup
            // 
            this.I9Popup.BackColor = System.Drawing.Color.Black;
            this.I9Popup.BXHeight = 675;
            this.I9Popup.ColumnPosition = 0.5F;
            this.I9Popup.ColumnRight = false;
            this.I9Popup.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.I9Popup.ForeColor =
                System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(48)))), ((int)(((byte)(255)))));
            this.I9Popup.InternalPadding = 3;
            this.I9Popup.Location = new System.Drawing.Point(513, 490);
            this.I9Popup.Name = "I9Popup";
            this.I9Popup.ScrollY = 0F;
            this.I9Popup.SectionPadding = 8;
            this.I9Popup.Size = new System.Drawing.Size(450, 203);
            this.I9Popup.TabIndex = 102;
            this.I9Popup.Visible = false;
            this.I9Popup.MouseMove += new System.Windows.Forms.MouseEventHandler(this.I9Popup_MouseMove);
            // 
            // ibTotalsEx
            // 
            this.ibTotalsEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibTotalsEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibTotalsEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibTotalsEx.Images.Background = Resources.HeroButton;
            this.ibTotalsEx.Images.Hover = Resources.HeroButtonHover;
            this.ibTotalsEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibTotalsEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibTotalsEx.Location = new System.Drawing.Point(355, 109);
            this.ibTotalsEx.Lock = false;
            this.ibTotalsEx.Name = "ibTotalsEx";
            this.ibTotalsEx.Size = new System.Drawing.Size(105, 22);
            this.ibTotalsEx.TabIndex = 116;
            this.ibTotalsEx.Text = "View Totals";
            this.ibTotalsEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibTotalsEx.TextOutline.Width = 3;
            this.ibTotalsEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibTotalsEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibTotalsEx.ToggleText.ToggledOff = "Toggled Off State";
            this.ibTotalsEx.ToggleText.ToggledOn = "Toggled On State";
            this.ibTotalsEx.UseAlt = false;
            this.ibTotalsEx.Click += new EventHandler(this.ibTotalsEx_OnClick);
            // 
            // ibSlotLevelsEx
            // 
            this.ibSlotLevelsEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibSlotLevelsEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibSlotLevelsEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibSlotLevelsEx.Images.Background = Resources.HeroButton;
            this.ibSlotLevelsEx.Images.Hover = Resources.HeroButtonHover;
            this.ibSlotLevelsEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibSlotLevelsEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibSlotLevelsEx.Location = new System.Drawing.Point(244, 133);
            this.ibSlotLevelsEx.Lock = false;
            this.ibSlotLevelsEx.Name = "ibSlotLevelsEx";
            this.ibSlotLevelsEx.Size = new System.Drawing.Size(105, 22);
            this.ibSlotLevelsEx.TabIndex = 116;
            this.ibSlotLevelsEx.Text = "ibSlotLevelsEx";
            this.ibSlotLevelsEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibSlotLevelsEx.TextOutline.Width = 3;
            this.ibSlotLevelsEx.ThreeState = false;
            this.ibSlotLevelsEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibSlotLevelsEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibSlotLevelsEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibSlotLevelsEx.ToggleText.ToggledOff = "Slot Levels: Off";
            this.ibSlotLevelsEx.ToggleText.ToggledOn = "Slot Levels: On";
            this.ibSlotLevelsEx.UseAlt = false;
            this.ibSlotLevelsEx.Click += new EventHandler(this.ibSlotLevelsEx_OnClick);
            // 
            // ibTeamEx
            // 
            this.ibTeamEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibTeamEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibTeamEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibTeamEx.Images.Background = Resources.HeroButton;
            this.ibTeamEx.Images.Hover = Resources.HeroButtonHover;
            this.ibTeamEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibTeamEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibTeamEx.Location = new System.Drawing.Point(602, 3);
            this.ibTeamEx.Lock = false;
            this.ibTeamEx.Name = "ibTeamEx";
            this.ibTeamEx.Size = new System.Drawing.Size(105, 22);
            this.ibTeamEx.TabIndex = 116;
            this.ibTeamEx.Text = "Team Members";
            this.ibTeamEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibTeamEx.TextOutline.Width = 3;
            this.ibTeamEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibTeamEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibTeamEx.ToggleText.ToggledOff = "Toggle Off State";
            this.ibTeamEx.ToggleText.ToggledOn = "Toggle On State";
            this.ibTeamEx.UseAlt = false;
            this.ibTeamEx.Click += new EventHandler(this.ibTeamEx_OnClick);
            // 
            // ibModeEx
            // 
            this.ibModeEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibModeEx.ButtonType = Mids_Reborn.Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            this.ibModeEx.Font = new System.Drawing.Font("MS Sans Serif", 11F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point);
            this.ibModeEx.ForeColor = Color.White;
            this.ibModeEx.Images.Background = Resources.HeroButton;
            this.ibModeEx.Images.Hover = Resources.HeroButtonHover;
            this.ibModeEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibModeEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibModeEx.Location = new System.Drawing.Point(244, 80);
            this.ibModeEx.Name = "ibModeEx";
            this.ibModeEx.Size = new System.Drawing.Size(105, 22);
            this.ibModeEx.TabIndex = 122;
            this.ibModeEx.Text = "ibModeEx1";
            this.ibModeEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibModeEx.TextOutline.Width = 3;
            this.ibModeEx.ThreeState = true;
            this.ibModeEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibModeEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibModeEx.ToggleText.Indeterminate = "Respec";
            this.ibModeEx.ToggleText.ToggledOff = "Level-Up";
            this.ibModeEx.ToggleText.ToggledOn = "Normal";
            this.ibModeEx.UseAlt = false;
            this.ibModeEx.Click += new EventHandler(ibModeEx_OnClick);
            // 
            // ibSetsEx
            // 
            this.ibSetsEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibSetsEx.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            this.ibSetsEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibSetsEx.Images.Background = Resources.HeroButton;
            this.ibSetsEx.Images.Hover = Resources.HeroButtonHover;
            this.ibSetsEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibSetsEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibSetsEx.Location = new System.Drawing.Point(244, 109);
            this.ibSetsEx.Lock = false;
            this.ibSetsEx.Name = "ibSetsEx";
            this.ibSetsEx.Size = new System.Drawing.Size(105, 22);
            this.ibSetsEx.TabIndex = 116;
            this.ibSetsEx.Text = "View Active Sets";
            this.ibSetsEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibSetsEx.TextOutline.Width = 3;
            this.ibSetsEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibSetsEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibSetsEx.ToggleText.ToggledOff = "Toggled Off State";
            this.ibSetsEx.ToggleText.ToggledOn = "Toggled On State";
            this.ibSetsEx.UseAlt = false;
            this.ibSetsEx.Click += new EventHandler(this.ibSetsEx_OnClick);
            // 
            // ibSlotInfoEx
            // 
            this.ibSlotInfoEx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ibSlotInfoEx.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            this.ibSlotInfoEx.Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.ibSlotInfoEx.CurrentText = "ibSlotInfoEx";
            this.ibSlotInfoEx.Images.Background = Resources.HeroButton;
            this.ibSlotInfoEx.Images.Hover = Resources.HeroButtonHover;
            this.ibSlotInfoEx.ImagesAlt.Background = Resources.VillainButton;
            this.ibSlotInfoEx.ImagesAlt.Hover = Resources.VillainButtonHover;
            this.ibSlotInfoEx.Location = new System.Drawing.Point(355, 133);
            this.ibSlotInfoEx.Lock = false;
            this.ibSlotInfoEx.Name = "ibSlotInfoEx";
            this.ibSlotInfoEx.Size = new System.Drawing.Size(105, 22);
            this.ibSlotInfoEx.TabIndex = 116;
            this.ibSlotInfoEx.Text = "ibSlotInfoEx";
            this.ibSlotInfoEx.TextOutline.Color = System.Drawing.Color.Black;
            this.ibSlotInfoEx.TextOutline.Width = 3;
            this.ibSlotInfoEx.ThreeState = false;
            this.ibSlotInfoEx.ToggleActivation = ImageButtonEx.MouseClicks.LeftButton;
            this.ibSlotInfoEx.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.ibSlotInfoEx.ToggleText.Indeterminate = "Indeterminate State";
            this.ibSlotInfoEx.ToggleText.ToggledOff = "X Slots to go";
            this.ibSlotInfoEx.ToggleText.ToggledOn = "X Slots placed";
            this.ibSlotInfoEx.UseAlt = false;
            this.ibSlotInfoEx.Click += new EventHandler(this.ibSlotInfoEx_Onclick);
            // 
            // poolsPanel
            //
            this.poolsPanel.AutoScroll = true;
            this.poolsPanel.Location = new System.Drawing.Point(322, 161); // 318, 161
            this.poolsPanel.Name = "poolsPanel";
            this.poolsPanel.Size = new System.Drawing.Size(158, 726); // 162, 726
            this.poolsPanel.TabIndex = 0;
            this.poolsPanel.BorderStyle = BorderStyle.None;
            this.poolsPanel.HorizontalScroll.Enabled = false;
            this.poolsPanel.VerticalScroll.Enabled = true;
            this.poolsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);

            this.poolsPanel.Controls.Add(this.lblLocked0);
            this.poolsPanel.Controls.Add(this.lblLocked1);
            this.poolsPanel.Controls.Add(this.lblLocked2);
            this.poolsPanel.Controls.Add(this.lblLocked3);
            this.poolsPanel.Controls.Add(this.lblLockedAncillary);

            this.poolsPanel.Controls.Add(this.cbPool0);
            this.poolsPanel.Controls.Add(this.cbPool1);
            this.poolsPanel.Controls.Add(this.cbPool2);
            this.poolsPanel.Controls.Add(this.cbPool3);
            this.poolsPanel.Controls.Add(this.cbAncillary);

            this.poolsPanel.Controls.Add(this.lblPool1);
            this.poolsPanel.Controls.Add(this.lblPool2);
            this.poolsPanel.Controls.Add(this.lblPool3);
            this.poolsPanel.Controls.Add(this.lblPool4);
            this.poolsPanel.Controls.Add(this.lblEpic);

            this.poolsPanel.Controls.Add(this.llPool0);
            this.poolsPanel.Controls.Add(this.llPool1);
            this.poolsPanel.Controls.Add(this.llPool2);
            this.poolsPanel.Controls.Add(this.llPool3);
            this.poolsPanel.Controls.Add(this.llAncillary);
            //
            // enhCheckMode
            //
            this.enhCheckMode.Location = new Point(5, 825); // poolsPanel.Bottom + 2;
            this.enhCheckMode.Name = "enhCheckMode";
            this.enhCheckMode.Size = new Size(445, 35);
            this.enhCheckMode.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new Size(1342, 1001);
            //this.ClientSize = new System.Drawing.Size(1318, 857);
            this.Controls.Add(this.i9Picker);
            this.Controls.Add(this.I9Popup);
            this.Controls.Add(this.lblLockedSecondary);
            this.Controls.Add(this.ibTotalsEx);
            this.Controls.Add(this.ibSlotLevelsEx);
            this.Controls.Add(this.ibModeEx);
            this.Controls.Add(this.ibSetsEx);
            this.Controls.Add(this.ibDynMode);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.MenuBar);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblATLocked);
            this.Controls.Add(this.lblOrigin);
            this.Controls.Add(this.lblAT);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cbSecondary);
            this.Controls.Add(this.lblSecondary);
            this.Controls.Add(this.cbPrimary);
            this.Controls.Add(this.cbOrigin);
            this.Controls.Add(this.cbAT);
            this.Controls.Add(this.ibSlotInfoEx);
            this.Controls.Add(this.lblPrimary);
            this.Controls.Add(this.pnlGFXFlow);
            this.Controls.Add(this.llPrimary);
            this.Controls.Add(this.llSecondary);
            this.Controls.Add(this.poolsPanel);
            this.Controls.Add(this.enhCheckMode);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuBar;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MRB";
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.pbDynMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGFX)).EndInit();
            this.pnlGFXFlow.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Panel topPanel;
        ToolStripMenuItem tsAbout;
        ToolStripMenuItem tsAdvDBEdit;
        ToolStripMenuItem tsAdvFreshInstall;
        ToolStripMenuItem tsAdvResetTips;
        ToolStripMenuItem tsSupport;
        ToolStripMenuItem tsPatchNotes;
        ToolStripMenuItem tsClearAllEnh;
        ToolStripMenuItem tsChangeDb;
        ToolStripMenuItem tsConfig;
        ToolStripMenuItem tsKoFi;
        ToolStripMenuItem tsPatreon;
        ToolStripMenuItem tsCoinbase;
        ToolStripMenuItem tsEnhToDO;
        ToolStripMenuItem tsEnhToEven;
        ToolStripMenuItem tsEnhToMinus1;
        ToolStripMenuItem tsEnhToMinus2;
        ToolStripMenuItem tsEnhToMinus3;
        ToolStripMenuItem tsEnhToNone;
        ToolStripMenuItem tsEnhToPlus1;
        ToolStripMenuItem tsEnhToPlus2;
        ToolStripMenuItem tsEnhToPlus3;
        ToolStripMenuItem tsEnhToPlus4;
        ToolStripMenuItem tsEnhToPlus5;
        ToolStripMenuItem tsEnhToSO;
        ToolStripMenuItem tsEnhToTO;
        ToolStripMenuItem tsExportDataLink;
        ToolStripMenuItem tsGenFreebies;
        ToolStripMenuItem tsGenJsonExport;
        ToolStripSeparator ToolStripSeparator25;
        ToolStripSeparator ToolStripSeparator27;
        ToolStripMenuItem tsShareDiscord;
        ToolStripMenuItem tsShareMenu;
        ToolStripMenuItem tsImportDataChunk;
        ToolStripMenuItem tsFileNew;
        ToolStripMenuItem tsFileOpen;
        ToolStripMenuItem tsBuildRcv;
        ToolStripMenuItem tsFilePrint;
        ToolStripMenuItem tsFileQuit;
        ToolStripMenuItem tsFileSave;
        ToolStripMenuItem tsFileSaveAs;
        ToolStripMenuItem tsFlipAllEnh;
        ToolStripMenuItem tsHelp;
        ToolStripMenuItem tsHelperLong;
        ToolStripMenuItem tsHelperLong2;
        ToolStripMenuItem tsHelperShort;
        ToolStripMenuItem tsHelperShort2;
        ToolStripMenuItem tsImport;
        ToolStripMenuItem tsIODefault;
        ToolStripMenuItem tsIOMax;
        ToolStripMenuItem tsIOMin;
        ToolStripMenuItem tsRecipeViewer;
        ToolStripMenuItem tsDPSCalc;
        ToolStripMenuItem tsRemoveAllSlots;
        ToolStripMenuItem tsSetFind;
        ToolStripMenuItem tsGitHub;
        ToolStripMenuItem tsUpdateCheck;
        ToolStripMenuItem tsView2Col;
        ToolStripMenuItem tsView3Col;
        ToolStripMenuItem tsView4Col;
        ToolStripMenuItem tsView5Col;
        ToolStripMenuItem tsView6Col;
        ToolStripMenuItem tsViewActualDamage_New;
        ToolStripMenuItem tsViewData;
        ToolStripMenuItem tsViewDPS_New;
        ToolStripMenuItem tsViewGraphs;
        ToolStripMenuItem tsViewIOLevels;
        ToolStripMenuItem tsViewSOLevels;
        ToolStripMenuItem tsViewRelative;
        ToolStripMenuItem tsViewRelativeAsSigns;
        ToolStripMenuItem tsViewSetCompare;
        ToolStripMenuItem tsViewSets;
        ToolStripMenuItem tsViewSlotLevels;
        ToolStripMenuItem tsViewTotals;
        ToolStripMenuItem AdvancedToolStripMenuItem1;
        ToolStripMenuItem AutoArrangeAllSlotsToolStripMenuItem;
        private ToolStripMenuItem ExportToolStripMenuItem;
        private ToolStripMenuItem LegacyToolStripMenuItem;
        ComboBox cbAncillary;
        ComboBox cbAT;
        ComboBox cbOrigin;
        ComboBox cbPool0;
        ComboBox cbPool1;
        ComboBox cbPool2;
        ComboBox cbPool3;
        ComboBox cbPrimary;
        ComboBox cbSecondary;
        ToolStripMenuItem CharacterToolStripMenuItem;
        DataView dvAnchored;
        ToolStripMenuItem FileToolStripMenuItem;
        ToolStripMenuItem HelpToolStripMenuItem;
        ToolStripMenuItem InGameRespecHelperToolStripMenuItem;
        Label lblAT;
        Label lblATLocked;
        Label lblEpic;
        Label lblCharacter;
        Label lblLocked0;
        Label lblLocked1;
        Label lblLocked2;
        Label lblLocked3;
        Label lblLockedAncillary;
        Label lblLockedSecondary;
        Label lblName;
        Label lblOrigin;
        Label lblPool1;
        Label lblPool2;
        Label lblPool3;
        Label lblPool4;
        Label lblPrimary;
        Label lblSecondary;
        ListLabelV3 llAncillary;
        ListLabelV3 llPool0;
        ListLabelV3 llPool1;
        ListLabelV3 llPool2;
        ListLabelV3 llPool3;
        ListLabelV3 llPrimary;
        ListLabelV3 llSecondary;
        MenuStrip MenuBar;
        ToolStripMenuItem OptionsToolStripMenuItem;
        public pnlGFX pnlGFX;
        public FlowLayoutPanel pnlGFXFlow;
        ToolStripMenuItem SetAllIOsToDefault35ToolStripMenuItem;
        ToolStripMenuItem SlotsToolStripMenuItem;
        ToolStripMenuItem tlsDPA;
        ToolStripMenuItem ToolStripMenuItem1;
        ToolStripMenuItem ToolStripMenuItem2;
        ToolStripMenuItem ToggleCheckModeToolStripMenuItem;
        ToolStripSeparator ToolStripMenuItem4;
        ToolStripSeparator ToolStripSeparator1;
        ToolStripSeparator ToolStripSeparator10;
        ToolStripSeparator ToolStripSeparator11;
        ToolStripSeparator ToolStripSeparator12;
        ToolStripSeparator ToolStripSeparator13;
        ToolStripSeparator ToolStripSeparator14;
        ToolStripSeparator ToolStripSeparator15;
        ToolStripSeparator ToolStripSeparator16;
        ToolStripSeparator ToolStripSeparator17;
        ToolStripSeparator ToolStripSeparator18;
        ToolStripSeparator ToolStripSeparator19;
        ToolStripSeparator ToolStripSeparator2;
        ToolStripSeparator ToolStripSeparator20;
        ToolStripSeparator ToolStripSeparator21;
        ToolStripSeparator ToolStripSeparator22;
        ToolStripSeparator ToolStripSeparator23;
        ToolStripSeparator ToolStripSeparator24;
        ToolStripSeparator ToolStripSeparator26;
        ToolStripSeparator ToolStripSeparator28;
        ToolStripSeparator ToolStripSeparator4;
        ToolStripSeparator ToolStripSeparator5;
        ToolStripSeparator ToolStripSeparator7;
        ToolStripSeparator ToolStripSeparator8;
        ToolStripSeparator ToolStripSeparator9;
        ToolStripMenuItem ViewToolStripMenuItem;
        ToolStripMenuItem WindowToolStripMenuItem;
        ToolStripSeparator ToolStripSeparator29;
        private ToolStripSeparator ToolStripSeparator30;
        private ToolStripSeparator ToolStripSeparator31;
        private ToolStripSeparator ToolStripSeparator32;
        ToolStripMenuItem ShareToolStripMenuItem;
        ToolStripMenuItem DonateToolStripMenuItem;
        public ToolStripComboBox EnemyRelativeToolStripComboBox;
        private ToolStripMenuItem tsViewBuildComment;
        Timer tmrGfx;
        Panel poolsPanel;
        ToolTip tTip;
        TextBox txtName;
        I9Picker i9Picker;
        ctlPopUp I9Popup;
        EnhCheckMode enhCheckMode;
        ImageButtonEx ibModeEx;
        ImageButtonEx ibDynMode;
        ImageButtonEx ibAlignmentEx;
        ImageButtonEx ibSlotInfoEx;
        public ImageButtonEx ibAccoladesEx;
        public ImageButtonEx ibPetsEx;
        ImageButtonEx ibTeamEx;
        ImageButtonEx ibPopupEx;
        ImageButtonEx ibPvXEx;
        ImageButtonEx ibRecipeEx;
        ImageButtonEx ibSetsEx;
        ImageButtonEx ibSlotLevelsEx;
        ImageButtonEx ibTotalsEx;
        public ImageButtonEx ibPrestigePowersEx;
        public ImageButtonEx ibIncarnatePowersEx;
        public ImageButtonEx ibTempPowersEx;
        internal OpenFileDialog DlgOpen;
        internal SaveFileDialog DlgSave;
    }
}
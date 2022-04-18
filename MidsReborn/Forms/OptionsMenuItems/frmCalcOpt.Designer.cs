using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class frmCalcOpt
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCalcOpt));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.chkEnableExtra = new System.Windows.Forms.CheckBox();
            this.chkShowSelfBuffsAny = new System.Windows.Forms.CheckBox();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.chkOldStyle = new System.Windows.Forms.CheckBox();
            this.cbTotalsWindowTitleOpt = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkNoTips = new System.Windows.Forms.CheckBox();
            this.chkMiddle = new System.Windows.Forms.CheckBox();
            this.GroupBox17 = new System.Windows.Forms.GroupBox();
            this.chkPowersBold = new System.Windows.Forms.CheckBox();
            this.chkPowSelBold = new System.Windows.Forms.CheckBox();
            this.udPowersSize = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.udPowSelectSize = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.chkShowAlphaPopup = new System.Windows.Forms.CheckBox();
            this.chkHighVis = new System.Windows.Forms.CheckBox();
            this.Label36 = new System.Windows.Forms.Label();
            this.chkStatBold = new System.Windows.Forms.CheckBox();
            this.chkTextBold = new System.Windows.Forms.CheckBox();
            this.btnFontColor = new System.Windows.Forms.Button();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.udStatSize = new System.Windows.Forms.NumericUpDown();
            this.udRTFSize = new System.Windows.Forms.NumericUpDown();
            this.chkIOPrintLevels = new System.Windows.Forms.CheckBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.chkEnableDmgGraph = new System.Windows.Forms.CheckBox();
            this.rbGraphSimple = new System.Windows.Forms.RadioButton();
            this.rbGraphStacked = new System.Windows.Forms.RadioButton();
            this.rbGraphTwoLine = new System.Windows.Forms.RadioButton();
            this.GroupBox14 = new System.Windows.Forms.GroupBox();
            this.cbCurrency = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.chkIOLevel = new System.Windows.Forms.CheckBox();
            this.btnIOReset = new System.Windows.Forms.Button();
            this.Label40 = new System.Windows.Forms.Label();
            this.udIOLevel = new System.Windows.Forms.NumericUpDown();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.chkShowSOLevels = new System.Windows.Forms.CheckBox();
            this.chkRelSignOnly = new System.Windows.Forms.CheckBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.optTO = new System.Windows.Forms.RadioButton();
            this.optDO = new System.Windows.Forms.RadioButton();
            this.optSO = new System.Windows.Forms.RadioButton();
            this.optEnh = new System.Windows.Forms.Label();
            this.cbEnhLevel = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.Label16 = new System.Windows.Forms.Label();
            this.TeamSize = new System.Windows.Forms.NumericUpDown();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.clbSuppression = new System.Windows.Forms.CheckedListBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.chkUseArcanaTime = new System.Windows.Forms.CheckBox();
            this.GroupBox15 = new System.Windows.Forms.GroupBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.chkSetBonus = new System.Windows.Forms.CheckBox();
            this.chkIOEffects = new System.Windows.Forms.CheckBox();
            this.GroupBox8 = new System.Windows.Forms.GroupBox();
            this.rbChanceIgnore = new System.Windows.Forms.RadioButton();
            this.rbChanceAverage = new System.Windows.Forms.RadioButton();
            this.rbChanceMax = new System.Windows.Forms.RadioButton();
            this.Label9 = new System.Windows.Forms.Label();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.rbPvP = new System.Windows.Forms.RadioButton();
            this.rbPvE = new System.Windows.Forms.RadioButton();
            this.TabPage4 = new System.Windows.Forms.TabPage();
            this.GroupBox12 = new System.Windows.Forms.GroupBox();
            this.fcReset = new System.Windows.Forms.Button();
            this.fcSet = new System.Windows.Forms.Button();
            this.fcNotes = new System.Windows.Forms.TextBox();
            this.fcDelete = new System.Windows.Forms.Button();
            this.fcAdd = new System.Windows.Forms.Button();
            this.fcName = new System.Windows.Forms.TextBox();
            this.fcWSTab = new System.Windows.Forms.RadioButton();
            this.fcWSSpace = new System.Windows.Forms.RadioButton();
            this.fcUnderlineOff = new System.Windows.Forms.TextBox();
            this.fcUnderlineOn = new System.Windows.Forms.TextBox();
            this.Label32 = new System.Windows.Forms.Label();
            this.fcItalicOff = new System.Windows.Forms.TextBox();
            this.fcItalicOn = new System.Windows.Forms.TextBox();
            this.Label31 = new System.Windows.Forms.Label();
            this.fcBoldOff = new System.Windows.Forms.TextBox();
            this.fcBoldOn = new System.Windows.Forms.TextBox();
            this.Label30 = new System.Windows.Forms.Label();
            this.fcTextOff = new System.Windows.Forms.TextBox();
            this.fcTextOn = new System.Windows.Forms.TextBox();
            this.Label29 = new System.Windows.Forms.Label();
            this.Label28 = new System.Windows.Forms.Label();
            this.Label27 = new System.Windows.Forms.Label();
            this.fcColorOff = new System.Windows.Forms.TextBox();
            this.fcColorOn = new System.Windows.Forms.TextBox();
            this.Label26 = new System.Windows.Forms.Label();
            this.fcList = new System.Windows.Forms.ListBox();
            this.Label25 = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.GroupBox11 = new System.Windows.Forms.GroupBox();
            this.csReset = new System.Windows.Forms.Button();
            this.csBtnEdit = new System.Windows.Forms.Button();
            this.csDelete = new System.Windows.Forms.Button();
            this.csAdd = new System.Windows.Forms.Button();
            this.csList = new System.Windows.Forms.ListBox();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.GroupBox9 = new System.Windows.Forms.GroupBox();
            this.lblExample = new System.Windows.Forms.Label();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.listScenarios = new System.Windows.Forms.ListBox();
            this.TabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.lblDatabaseLoc = new System.Windows.Forms.TextBox();
            this.btnResetDatabaseLoc = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.btnDatabaseLoc = new System.Windows.Forms.Button();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.btnFileAssoc = new System.Windows.Forms.Button();
            this.lblFileAssoc = new System.Windows.Forms.Label();
            this.lblAssocStatus = new System.Windows.Forms.Label();
            this.btnSaveFolderReset = new System.Windows.Forms.Button();
            this.lblSaveFolder = new System.Windows.Forms.Label();
            this.btnSaveFolder = new System.Windows.Forms.Button();
            this.chkLoadLastFile = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label37 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.chkUpdates = new System.Windows.Forms.CheckBox();
            this.cbDBUpdateURL = new System.Windows.Forms.ComboBox();
            this.cbUpdateURL = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.lblUpdateURL = new System.Windows.Forms.Label();
            this.TabPage6 = new System.Windows.Forms.TabPage();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.label38 = new System.Windows.Forms.Label();
            this.udStaminaSecond = new System.Windows.Forms.NumericUpDown();
            this.udHealthSecond = new System.Windows.Forms.NumericUpDown();
            this.label52 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.udStaminaFirst = new System.Windows.Forms.NumericUpDown();
            this.udHealthFirst = new System.Windows.Forms.NumericUpDown();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.udStaminaSlots = new System.Windows.Forms.NumericUpDown();
            this.label46 = new System.Windows.Forms.Label();
            this.udHealthSlots = new System.Windows.Forms.NumericUpDown();
            this.label43 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label48 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.udMaxSlots = new System.Windows.Forms.NumericUpDown();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label45 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.udMaxRunSpeed = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.udMaxFlySpeed = new System.Windows.Forms.NumericUpDown();
            this.udMaxJumpSpeed = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label44 = new System.Windows.Forms.Label();
            this.udBasePerception = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.udBaseRunSpeed = new System.Windows.Forms.NumericUpDown();
            this.udBaseToHit = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.udBaseJumpHeight = new System.Windows.Forms.NumericUpDown();
            this.udBaseFlySpeed = new System.Windows.Forms.NumericUpDown();
            this.udBaseJumpSpeed = new System.Windows.Forms.NumericUpDown();
            this.chkColorPrint = new System.Windows.Forms.CheckBox();
            this.myTip = new System.Windows.Forms.ToolTip(this.components);
            this.cPicker = new System.Windows.Forms.ColorDialog();
            this.fbdSave = new System.Windows.Forms.FolderBrowserDialog();
            this.TabControl1.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.GroupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPowersSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPowSelectSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStatSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRTFSize)).BeginInit();
            this.GroupBox5.SuspendLayout();
            this.GroupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udIOLevel)).BeginInit();
            this.GroupBox3.SuspendLayout();
            this.TabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TeamSize)).BeginInit();
            this.GroupBox2.SuspendLayout();
            this.GroupBox15.SuspendLayout();
            this.GroupBox8.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            this.TabPage4.SuspendLayout();
            this.GroupBox12.SuspendLayout();
            this.GroupBox11.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.GroupBox9.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.TabPage5.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.TabPage6.SuspendLayout();
            this.groupBox20.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaFirst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthFirst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaSlots)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthSlots)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxSlots)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxRunSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxFlySpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxJumpSpeed)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udBasePerception)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseRunSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseToHit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseJumpHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseFlySpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseJumpSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(504, 380);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 59;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(408, 380);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 60;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage3);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Controls.Add(this.TabPage4);
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage5);
            this.TabControl1.Controls.Add(this.TabPage6);
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(785, 372);
            this.TabControl1.TabIndex = 0;
            // 
            // TabPage3
            // 
            this.TabPage3.Controls.Add(this.chkEnableExtra);
            this.TabPage3.Controls.Add(this.chkShowSelfBuffsAny);
            this.TabPage3.Controls.Add(this.groupBox18);
            this.TabPage3.Controls.Add(this.chkNoTips);
            this.TabPage3.Controls.Add(this.chkMiddle);
            this.TabPage3.Controls.Add(this.GroupBox17);
            this.TabPage3.Controls.Add(this.chkIOPrintLevels);
            this.TabPage3.Controls.Add(this.GroupBox5);
            this.TabPage3.Controls.Add(this.GroupBox14);
            this.TabPage3.Controls.Add(this.GroupBox3);
            this.TabPage3.Location = new System.Drawing.Point(4, 23);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Size = new System.Drawing.Size(777, 345);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Enhancements & View";
            this.TabPage3.UseVisualStyleBackColor = true;
            // 
            // chkEnableExtra
            // 
            this.chkEnableExtra.AutoSize = true;
            this.chkEnableExtra.Location = new System.Drawing.Point(499, 324);
            this.chkEnableExtra.Name = "chkEnableExtra";
            this.chkEnableExtra.Size = new System.Drawing.Size(127, 18);
            this.chkEnableExtra.TabIndex = 83;
            this.chkEnableExtra.Text = "Enable Inherent Slots";
            this.chkEnableExtra.UseVisualStyleBackColor = true;
            this.chkEnableExtra.CheckedChanged += new System.EventHandler(this.chkEnableExtra_CheckedChanged);
            // 
            // chkShowSelfBuffsAny
            // 
            this.chkShowSelfBuffsAny.Location = new System.Drawing.Point(194, 324);
            this.chkShowSelfBuffsAny.Name = "chkShowSelfBuffsAny";
            this.chkShowSelfBuffsAny.Size = new System.Drawing.Size(190, 18);
            this.chkShowSelfBuffsAny.TabIndex = 80;
            this.chkShowSelfBuffsAny.Text = "Show \"in PvE/PvP\" for self buffs";
            this.chkShowSelfBuffsAny.Visible = false;
            this.chkShowSelfBuffsAny.CheckedChanged += new System.EventHandler(this.chkShowSelfBuffsAny_CheckedChanged);
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.chkOldStyle);
            this.groupBox18.Controls.Add(this.cbTotalsWindowTitleOpt);
            this.groupBox18.Controls.Add(this.label2);
            this.groupBox18.Location = new System.Drawing.Point(388, 120);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(353, 44);
            this.groupBox18.TabIndex = 79;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "Totals Window:";
            // 
            // chkOldStyle
            // 
            this.chkOldStyle.Location = new System.Drawing.Point(11, 17);
            this.chkOldStyle.Name = "chkOldStyle";
            this.chkOldStyle.Size = new System.Drawing.Size(88, 18);
            this.chkOldStyle.TabIndex = 80;
            this.chkOldStyle.Text = "Use old style";
            this.chkOldStyle.CheckedChanged += new System.EventHandler(this.chkOldStyle_CheckedChanged);
            // 
            // cbTotalsWindowTitleOpt
            // 
            this.cbTotalsWindowTitleOpt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTotalsWindowTitleOpt.FormattingEnabled = true;
            this.cbTotalsWindowTitleOpt.Items.AddRange(new object[] {
            "Generic - Totals for Self",
            "Character name + Archetype + Powersets",
            "Build file name + Archetype + Powersets",
            "Character name + Build file name (fallback to generic if none)"});
            this.cbTotalsWindowTitleOpt.Location = new System.Drawing.Point(199, 13);
            this.cbTotalsWindowTitleOpt.Name = "cbTotalsWindowTitleOpt";
            this.cbTotalsWindowTitleOpt.Size = new System.Drawing.Size(148, 22);
            this.cbTotalsWindowTitleOpt.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Show in titlebar:";
            // 
            // chkNoTips
            // 
            this.chkNoTips.Location = new System.Drawing.Point(417, 304);
            this.chkNoTips.Name = "chkNoTips";
            this.chkNoTips.Size = new System.Drawing.Size(78, 18);
            this.chkNoTips.TabIndex = 78;
            this.chkNoTips.Text = "No Tooltips";
            this.chkNoTips.Visible = false;
            // 
            // chkMiddle
            // 
            this.chkMiddle.Location = new System.Drawing.Point(500, 304);
            this.chkMiddle.Name = "chkMiddle";
            this.chkMiddle.Size = new System.Drawing.Size(222, 18);
            this.chkMiddle.TabIndex = 77;
            this.chkMiddle.Text = "Middle-Click repeats last enhancement";
            // 
            // GroupBox17
            // 
            this.GroupBox17.Controls.Add(this.chkPowersBold);
            this.GroupBox17.Controls.Add(this.chkPowSelBold);
            this.GroupBox17.Controls.Add(this.udPowersSize);
            this.GroupBox17.Controls.Add(this.label18);
            this.GroupBox17.Controls.Add(this.udPowSelectSize);
            this.GroupBox17.Controls.Add(this.label17);
            this.GroupBox17.Controls.Add(this.chkShowAlphaPopup);
            this.GroupBox17.Controls.Add(this.chkHighVis);
            this.GroupBox17.Controls.Add(this.Label36);
            this.GroupBox17.Controls.Add(this.chkStatBold);
            this.GroupBox17.Controls.Add(this.chkTextBold);
            this.GroupBox17.Controls.Add(this.btnFontColor);
            this.GroupBox17.Controls.Add(this.Label22);
            this.GroupBox17.Controls.Add(this.Label21);
            this.GroupBox17.Controls.Add(this.udStatSize);
            this.GroupBox17.Controls.Add(this.udRTFSize);
            this.GroupBox17.Location = new System.Drawing.Point(196, 166);
            this.GroupBox17.Name = "GroupBox17";
            this.GroupBox17.Size = new System.Drawing.Size(545, 132);
            this.GroupBox17.TabIndex = 76;
            this.GroupBox17.TabStop = false;
            this.GroupBox17.Text = "Font Size/Colors:";
            // 
            // chkPowersBold
            // 
            this.chkPowersBold.AutoSize = true;
            this.chkPowersBold.Location = new System.Drawing.Point(173, 101);
            this.chkPowersBold.Name = "chkPowersBold";
            this.chkPowersBold.Size = new System.Drawing.Size(15, 14);
            this.chkPowersBold.TabIndex = 85;
            this.chkPowersBold.UseVisualStyleBackColor = true;
            // 
            // chkPowSelBold
            // 
            this.chkPowSelBold.AutoSize = true;
            this.chkPowSelBold.Location = new System.Drawing.Point(173, 76);
            this.chkPowSelBold.Name = "chkPowSelBold";
            this.chkPowSelBold.Size = new System.Drawing.Size(15, 14);
            this.chkPowSelBold.TabIndex = 84;
            this.chkPowSelBold.UseVisualStyleBackColor = true;
            // 
            // udPowersSize
            // 
            this.udPowersSize.DecimalPlaces = 2;
            this.udPowersSize.Location = new System.Drawing.Point(108, 98);
            this.udPowersSize.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.udPowersSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.udPowersSize.Name = "udPowersSize";
            this.udPowersSize.Size = new System.Drawing.Size(52, 20);
            this.udPowersSize.TabIndex = 83;
            this.udPowersSize.Value = new decimal(new int[] {
            825,
            0,
            0,
            131072});
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(29, 98);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(78, 20);
            this.label18.TabIndex = 82;
            this.label18.Text = "Powers:";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udPowSelectSize
            // 
            this.udPowSelectSize.DecimalPlaces = 2;
            this.udPowSelectSize.Location = new System.Drawing.Point(108, 73);
            this.udPowSelectSize.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.udPowSelectSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.udPowSelectSize.Name = "udPowSelectSize";
            this.udPowSelectSize.Size = new System.Drawing.Size(52, 20);
            this.udPowSelectSize.TabIndex = 81;
            this.udPowSelectSize.Value = new decimal(new int[] {
            825,
            0,
            0,
            131072});
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(11, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(96, 20);
            this.label17.TabIndex = 80;
            this.label17.Text = "Power Selections:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkShowAlphaPopup
            // 
            this.chkShowAlphaPopup.Location = new System.Drawing.Point(280, 82);
            this.chkShowAlphaPopup.Name = "chkShowAlphaPopup";
            this.chkShowAlphaPopup.Size = new System.Drawing.Size(190, 18);
            this.chkShowAlphaPopup.TabIndex = 79;
            this.chkShowAlphaPopup.Text = "Include Alpha buffs in popups";
            // 
            // chkHighVis
            // 
            this.chkHighVis.Location = new System.Drawing.Point(280, 106);
            this.chkHighVis.Name = "chkHighVis";
            this.chkHighVis.Size = new System.Drawing.Size(222, 20);
            this.chkHighVis.TabIndex = 69;
            this.chkHighVis.Text = "Use High-Visiblity text on the build view";
            this.myTip.SetToolTip(this.chkHighVis, "Draw white text with a black outline on the build view (power bars on the right o" +
        "f the screen)");
            // 
            // Label36
            // 
            this.Label36.Location = new System.Drawing.Point(161, 8);
            this.Label36.Name = "Label36";
            this.Label36.Size = new System.Drawing.Size(39, 16);
            this.Label36.TabIndex = 64;
            this.Label36.Text = "Bold";
            this.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkStatBold
            // 
            this.chkStatBold.AutoSize = true;
            this.chkStatBold.Location = new System.Drawing.Point(173, 50);
            this.chkStatBold.Name = "chkStatBold";
            this.chkStatBold.Size = new System.Drawing.Size(15, 14);
            this.chkStatBold.TabIndex = 63;
            this.chkStatBold.UseVisualStyleBackColor = true;
            // 
            // chkTextBold
            // 
            this.chkTextBold.AutoSize = true;
            this.chkTextBold.Location = new System.Drawing.Point(173, 25);
            this.chkTextBold.Name = "chkTextBold";
            this.chkTextBold.Size = new System.Drawing.Size(15, 14);
            this.chkTextBold.TabIndex = 62;
            this.chkTextBold.UseVisualStyleBackColor = true;
            // 
            // btnFontColor
            // 
            this.btnFontColor.Location = new System.Drawing.Point(280, 32);
            this.btnFontColor.Name = "btnFontColor";
            this.btnFontColor.Size = new System.Drawing.Size(172, 27);
            this.btnFontColor.TabIndex = 61;
            this.btnFontColor.Text = "Set Colors...";
            this.btnFontColor.UseVisualStyleBackColor = true;
            this.btnFontColor.Click += new System.EventHandler(this.btnFontColor_Click);
            // 
            // Label22
            // 
            this.Label22.Location = new System.Drawing.Point(16, 47);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(91, 20);
            this.Label22.TabIndex = 60;
            this.Label22.Text = "Stats:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label21
            // 
            this.Label21.Location = new System.Drawing.Point(5, 21);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(102, 20);
            this.Label21.TabIndex = 59;
            this.Label21.Text = "InfoPanel Tab Text:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udStatSize
            // 
            this.udStatSize.DecimalPlaces = 2;
            this.udStatSize.Location = new System.Drawing.Point(108, 47);
            this.udStatSize.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.udStatSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.udStatSize.Name = "udStatSize";
            this.udStatSize.Size = new System.Drawing.Size(52, 20);
            this.udStatSize.TabIndex = 1;
            this.udStatSize.Value = new decimal(new int[] {
            825,
            0,
            0,
            131072});
            // 
            // udRTFSize
            // 
            this.udRTFSize.Location = new System.Drawing.Point(108, 21);
            this.udRTFSize.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.udRTFSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.udRTFSize.Name = "udRTFSize";
            this.udRTFSize.Size = new System.Drawing.Size(52, 20);
            this.udRTFSize.TabIndex = 0;
            this.udRTFSize.Value = new decimal(new int[] {
            825,
            0,
            0,
            131072});
            // 
            // chkIOPrintLevels
            // 
            this.chkIOPrintLevels.Location = new System.Drawing.Point(194, 304);
            this.chkIOPrintLevels.Name = "chkIOPrintLevels";
            this.chkIOPrintLevels.Size = new System.Drawing.Size(221, 18);
            this.chkIOPrintLevels.TabIndex = 75;
            this.chkIOPrintLevels.Text = "Display Invention levels in printed builds";
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.chkEnableDmgGraph);
            this.GroupBox5.Controls.Add(this.rbGraphSimple);
            this.GroupBox5.Controls.Add(this.rbGraphStacked);
            this.GroupBox5.Controls.Add(this.rbGraphTwoLine);
            this.GroupBox5.Location = new System.Drawing.Point(388, 4);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(353, 117);
            this.GroupBox5.TabIndex = 72;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Damage Graph Style:";
            // 
            // chkEnableDmgGraph
            // 
            this.chkEnableDmgGraph.AutoSize = true;
            this.chkEnableDmgGraph.Location = new System.Drawing.Point(6, 21);
            this.chkEnableDmgGraph.Name = "chkEnableDmgGraph";
            this.chkEnableDmgGraph.Size = new System.Drawing.Size(133, 18);
            this.chkEnableDmgGraph.TabIndex = 6;
            this.chkEnableDmgGraph.Text = "Enable Damage Graph";
            this.chkEnableDmgGraph.UseVisualStyleBackColor = true;
            this.chkEnableDmgGraph.CheckedChanged += new System.EventHandler(this.chkEnableDmgGraph_CheckedChanged);
            // 
            // rbGraphSimple
            // 
            this.rbGraphSimple.Location = new System.Drawing.Point(6, 87);
            this.rbGraphSimple.Name = "rbGraphSimple";
            this.rbGraphSimple.Size = new System.Drawing.Size(164, 23);
            this.rbGraphSimple.TabIndex = 5;
            this.rbGraphSimple.Text = "Base against Enhanced";
            this.myTip.SetToolTip(this.rbGraphSimple, "This graph type doesn\'t reflect the max damage potential of other powers.");
            // 
            // rbGraphStacked
            // 
            this.rbGraphStacked.Location = new System.Drawing.Point(6, 66);
            this.rbGraphStacked.Name = "rbGraphStacked";
            this.rbGraphStacked.Size = new System.Drawing.Size(286, 23);
            this.rbGraphStacked.TabIndex = 4;
            this.rbGraphStacked.Text = "Base + Enhanced (stacked) against Max Enhancable";
            this.myTip.SetToolTip(this.rbGraphStacked, "\'Max Enhacable\' is damage if slotted with 6 +3 damage enhancements.");
            // 
            // rbGraphTwoLine
            // 
            this.rbGraphTwoLine.Checked = true;
            this.rbGraphTwoLine.Location = new System.Drawing.Point(6, 45);
            this.rbGraphTwoLine.Name = "rbGraphTwoLine";
            this.rbGraphTwoLine.Size = new System.Drawing.Size(286, 23);
            this.rbGraphTwoLine.TabIndex = 3;
            this.rbGraphTwoLine.TabStop = true;
            this.rbGraphTwoLine.Text = "Base / Enhanced against Max Enhancable (Default)";
            this.myTip.SetToolTip(this.rbGraphTwoLine, "The blue bar will indicate base damage, the yellow bar will indicate enhanced dam" +
        "age.");
            // 
            // GroupBox14
            // 
            this.GroupBox14.Controls.Add(this.cbCurrency);
            this.GroupBox14.Controls.Add(this.label19);
            this.GroupBox14.Controls.Add(this.chkIOLevel);
            this.GroupBox14.Controls.Add(this.btnIOReset);
            this.GroupBox14.Controls.Add(this.Label40);
            this.GroupBox14.Controls.Add(this.udIOLevel);
            this.GroupBox14.Location = new System.Drawing.Point(196, 4);
            this.GroupBox14.Name = "GroupBox14";
            this.GroupBox14.Size = new System.Drawing.Size(188, 160);
            this.GroupBox14.TabIndex = 69;
            this.GroupBox14.TabStop = false;
            this.GroupBox14.Text = "Inventions:";
            // 
            // cbCurrency
            // 
            this.cbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCurrency.FormattingEnabled = true;
            this.cbCurrency.Location = new System.Drawing.Point(9, 81);
            this.cbCurrency.Name = "cbCurrency";
            this.cbCurrency.Size = new System.Drawing.Size(171, 22);
            this.cbCurrency.TabIndex = 62;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 64);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(103, 14);
            this.label19.TabIndex = 61;
            this.label19.Text = "Preferred currency:";
            // 
            // chkIOLevel
            // 
            this.chkIOLevel.Location = new System.Drawing.Point(8, 40);
            this.chkIOLevel.Name = "chkIOLevel";
            this.chkIOLevel.Size = new System.Drawing.Size(172, 24);
            this.chkIOLevel.TabIndex = 60;
            this.chkIOLevel.Text = "Display IO Levels";
            this.myTip.SetToolTip(this.chkIOLevel, "Show the level of Inventions in the build view");
            // 
            // btnIOReset
            // 
            this.btnIOReset.Location = new System.Drawing.Point(8, 109);
            this.btnIOReset.Name = "btnIOReset";
            this.btnIOReset.Size = new System.Drawing.Size(172, 44);
            this.btnIOReset.TabIndex = 59;
            this.btnIOReset.Text = "Set All IO and SetO levels to default";
            this.btnIOReset.Click += new System.EventHandler(this.btnIOReset_Click);
            // 
            // Label40
            // 
            this.Label40.Location = new System.Drawing.Point(8, 20);
            this.Label40.Name = "Label40";
            this.Label40.Size = new System.Drawing.Size(96, 20);
            this.Label40.TabIndex = 58;
            this.Label40.Text = "Default IO Level:";
            this.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udIOLevel
            // 
            this.udIOLevel.Location = new System.Drawing.Point(108, 20);
            this.udIOLevel.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udIOLevel.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIOLevel.Name = "udIOLevel";
            this.udIOLevel.Size = new System.Drawing.Size(72, 20);
            this.udIOLevel.TabIndex = 57;
            this.myTip.SetToolTip(this.udIOLevel, "Inventions will be placed at this level by default. You can override the default " +
        "by typing a level number in the picker.");
            this.udIOLevel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.chkShowSOLevels);
            this.GroupBox3.Controls.Add(this.chkRelSignOnly);
            this.GroupBox3.Controls.Add(this.Label3);
            this.GroupBox3.Controls.Add(this.optTO);
            this.GroupBox3.Controls.Add(this.optDO);
            this.GroupBox3.Controls.Add(this.optSO);
            this.GroupBox3.Controls.Add(this.optEnh);
            this.GroupBox3.Controls.Add(this.cbEnhLevel);
            this.GroupBox3.Controls.Add(this.Label4);
            this.GroupBox3.Location = new System.Drawing.Point(4, 4);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(184, 338);
            this.GroupBox3.TabIndex = 62;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Regular Enhancements:";
            // 
            // chkShowSOLevels
            // 
            this.chkShowSOLevels.Location = new System.Drawing.Point(11, 257);
            this.chkShowSOLevels.Name = "chkShowSOLevels";
            this.chkShowSOLevels.Size = new System.Drawing.Size(150, 23);
            this.chkShowSOLevels.TabIndex = 71;
            this.chkShowSOLevels.Text = "Display SO/HO Levels";
            this.chkShowSOLevels.UseVisualStyleBackColor = true;
            this.chkShowSOLevels.CheckedChanged += new System.EventHandler(this.chkShowSOLevels_CheckedChanged);
            // 
            // chkRelSignOnly
            // 
            this.chkRelSignOnly.Location = new System.Drawing.Point(11, 278);
            this.chkRelSignOnly.Name = "chkRelSignOnly";
            this.chkRelSignOnly.Size = new System.Drawing.Size(167, 43);
            this.chkRelSignOnly.TabIndex = 69;
            this.chkRelSignOnly.Text = "Show signs only for relative levels (\'++\' rather than \'+2\')";
            this.myTip.SetToolTip(this.chkRelSignOnly, "Show signs only for relative levels (\'++\' rather than \'+2\')");
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(6, 142);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(172, 79);
            this.Label3.TabIndex = 59;
            this.Label3.Text = "Default Relative Level:\r\n(Ehancements can function up to five levels above or thr" +
    "ee below that of the character.)";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optTO
            // 
            this.optTO.Appearance = System.Windows.Forms.Appearance.Button;
            this.optTO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optTO.Location = new System.Drawing.Point(24, 74);
            this.optTO.Name = "optTO";
            this.optTO.Size = new System.Drawing.Size(44, 44);
            this.optTO.TabIndex = 48;
            this.optTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.myTip.SetToolTip(this.optTO, "Training enhancements are the weakest kind.");
            this.optTO.CheckedChanged += new System.EventHandler(this.optTO_CheckedChanged);
            // 
            // optDO
            // 
            this.optDO.Appearance = System.Windows.Forms.Appearance.Button;
            this.optDO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optDO.Location = new System.Drawing.Point(72, 74);
            this.optDO.Name = "optDO";
            this.optDO.Size = new System.Drawing.Size(44, 44);
            this.optDO.TabIndex = 49;
            this.optDO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.myTip.SetToolTip(this.optDO, "Dual Origin enhancements can be bought from level 12 onwards.");
            this.optDO.CheckedChanged += new System.EventHandler(this.optDO_CheckedChanged);
            // 
            // optSO
            // 
            this.optSO.Appearance = System.Windows.Forms.Appearance.Button;
            this.optSO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optSO.Checked = true;
            this.optSO.Location = new System.Drawing.Point(120, 74);
            this.optSO.Name = "optSO";
            this.optSO.Size = new System.Drawing.Size(44, 44);
            this.optSO.TabIndex = 50;
            this.optSO.TabStop = true;
            this.optSO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.myTip.SetToolTip(this.optSO, "Single Origin enhancements are the most powerful kind, and can be bought from lev" +
        "el 22.");
            this.optSO.CheckedChanged += new System.EventHandler(this.optSO_CheckedChanged);
            // 
            // optEnh
            // 
            this.optEnh.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optEnh.ForeColor = System.Drawing.SystemColors.ControlText;
            this.optEnh.Location = new System.Drawing.Point(21, 121);
            this.optEnh.Name = "optEnh";
            this.optEnh.Size = new System.Drawing.Size(143, 16);
            this.optEnh.TabIndex = 52;
            this.optEnh.Text = "Single Origin";
            this.optEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbEnhLevel
            // 
            this.cbEnhLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnhLevel.Items.AddRange(new object[] {
            "None (Enh. has no effect)",
            "-3 Levels",
            "-2 Levels",
            "-1 Level",
            "Even Level",
            "+1 Level",
            "+2 Levels",
            "+3 Levels"});
            this.cbEnhLevel.Location = new System.Drawing.Point(9, 224);
            this.cbEnhLevel.Name = "cbEnhLevel";
            this.cbEnhLevel.Size = new System.Drawing.Size(167, 22);
            this.cbEnhLevel.TabIndex = 53;
            this.cbEnhLevel.Tag = "";
            this.myTip.SetToolTip(this.cbEnhLevel, "This is the relative level of the enhancements in relation to your own. ");
            // 
            // Label4
            // 
            this.Label4.Location = new System.Drawing.Point(6, 18);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(172, 50);
            this.Label4.TabIndex = 58;
            this.Label4.Text = "Default Enhancement Type:\r\n(This does not affect Inventions or Special enhancemen" +
    "ts)";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.Label16);
            this.TabPage2.Controls.Add(this.TeamSize);
            this.TabPage2.Controls.Add(this.GroupBox2);
            this.TabPage2.Controls.Add(this.chkUseArcanaTime);
            this.TabPage2.Controls.Add(this.GroupBox15);
            this.TabPage2.Controls.Add(this.GroupBox8);
            this.TabPage2.Controls.Add(this.GroupBox6);
            this.TabPage2.Location = new System.Drawing.Point(4, 23);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Size = new System.Drawing.Size(777, 345);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Effects & Maths";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // Label16
            // 
            this.Label16.Enabled = false;
            this.Label16.Location = new System.Drawing.Point(575, 322);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(57, 18);
            this.Label16.TabIndex = 66;
            this.Label16.Text = "Team Size";
            this.Label16.Visible = false;
            // 
            // TeamSize
            // 
            this.TeamSize.Enabled = false;
            this.TeamSize.Location = new System.Drawing.Point(638, 320);
            this.TeamSize.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.TeamSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TeamSize.Name = "TeamSize";
            this.TeamSize.Size = new System.Drawing.Size(88, 20);
            this.TeamSize.TabIndex = 70;
            this.myTip.SetToolTip(this.TeamSize, "Set this to the number of players on your team.");
            this.TeamSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.TeamSize.Visible = false;
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.clbSuppression);
            this.GroupBox2.Controls.Add(this.Label8);
            this.GroupBox2.Location = new System.Drawing.Point(547, 4);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(221, 310);
            this.GroupBox2.TabIndex = 69;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Suppression";
            // 
            // clbSuppression
            // 
            this.clbSuppression.CheckOnClick = true;
            this.clbSuppression.FormattingEnabled = true;
            this.clbSuppression.Location = new System.Drawing.Point(9, 75);
            this.clbSuppression.Name = "clbSuppression";
            this.clbSuppression.Size = new System.Drawing.Size(206, 229);
            this.clbSuppression.TabIndex = 9;
            this.clbSuppression.SelectedIndexChanged += new System.EventHandler(this.clbSuppression_SelectedIndexChanged);
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(6, 28);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(209, 44);
            this.Label8.TabIndex = 65;
            this.Label8.Text = "Some effects are suppressed on specific conditions. You can enable those conditio" +
    "ns here.";
            // 
            // chkUseArcanaTime
            // 
            this.chkUseArcanaTime.Location = new System.Drawing.Point(8, 320);
            this.chkUseArcanaTime.Name = "chkUseArcanaTime";
            this.chkUseArcanaTime.Size = new System.Drawing.Size(204, 22);
            this.chkUseArcanaTime.TabIndex = 66;
            this.chkUseArcanaTime.Text = "Use ArcanaTime for Animation Times";
            this.myTip.SetToolTip(this.chkUseArcanaTime, "Displays all cast times in ArcanaTime, the real time all animations take due to s" +
        "erver clock ticks.");
            // 
            // GroupBox15
            // 
            this.GroupBox15.Controls.Add(this.Label20);
            this.GroupBox15.Controls.Add(this.chkSetBonus);
            this.GroupBox15.Controls.Add(this.chkIOEffects);
            this.GroupBox15.Location = new System.Drawing.Point(315, 194);
            this.GroupBox15.Name = "GroupBox15";
            this.GroupBox15.Size = new System.Drawing.Size(226, 121);
            this.GroupBox15.TabIndex = 68;
            this.GroupBox15.TabStop = false;
            this.GroupBox15.Text = "Invention Effects:";
            // 
            // Label20
            // 
            this.Label20.Location = new System.Drawing.Point(6, 17);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(213, 49);
            this.Label20.TabIndex = 65;
            this.Label20.Text = "The effects of set bonusses and special IO enhancements can be included when stat" +
    "s are calculated.";
            // 
            // chkSetBonus
            // 
            this.chkSetBonus.Location = new System.Drawing.Point(11, 93);
            this.chkSetBonus.Name = "chkSetBonus";
            this.chkSetBonus.Size = new System.Drawing.Size(169, 22);
            this.chkSetBonus.TabIndex = 64;
            this.chkSetBonus.Text = "Include Set Bonus effects\r\n";
            this.myTip.SetToolTip(this.chkSetBonus, "Add set bonus effects to the totals view.");
            // 
            // chkIOEffects
            // 
            this.chkIOEffects.Location = new System.Drawing.Point(11, 69);
            this.chkIOEffects.Name = "chkIOEffects";
            this.chkIOEffects.Size = new System.Drawing.Size(169, 22);
            this.chkIOEffects.TabIndex = 63;
            this.chkIOEffects.Text = "Include Enhancement effects";
            this.myTip.SetToolTip(this.chkIOEffects, "Some enhancments have power effects, such as minor Psionic resistance. This effec" +
        "t can be added into the power.");
            // 
            // GroupBox8
            // 
            this.GroupBox8.Controls.Add(this.rbChanceIgnore);
            this.GroupBox8.Controls.Add(this.rbChanceAverage);
            this.GroupBox8.Controls.Add(this.rbChanceMax);
            this.GroupBox8.Controls.Add(this.Label9);
            this.GroupBox8.Location = new System.Drawing.Point(4, 4);
            this.GroupBox8.Name = "GroupBox8";
            this.GroupBox8.Size = new System.Drawing.Size(537, 182);
            this.GroupBox8.TabIndex = 3;
            this.GroupBox8.TabStop = false;
            this.GroupBox8.Text = "Chance of Damage:";
            // 
            // rbChanceIgnore
            // 
            this.rbChanceIgnore.Location = new System.Drawing.Point(347, 156);
            this.rbChanceIgnore.Name = "rbChanceIgnore";
            this.rbChanceIgnore.Size = new System.Drawing.Size(183, 20);
            this.rbChanceIgnore.TabIndex = 8;
            this.rbChanceIgnore.Text = "Minimum (Ignore Extra Damage)\r\n";
            // 
            // rbChanceAverage
            // 
            this.rbChanceAverage.Checked = true;
            this.rbChanceAverage.Location = new System.Drawing.Point(6, 156);
            this.rbChanceAverage.Name = "rbChanceAverage";
            this.rbChanceAverage.Size = new System.Drawing.Size(168, 20);
            this.rbChanceAverage.TabIndex = 7;
            this.rbChanceAverage.TabStop = true;
            this.rbChanceAverage.Text = "Average (Damage x Chance)";
            // 
            // rbChanceMax
            // 
            this.rbChanceMax.Location = new System.Drawing.Point(180, 156);
            this.rbChanceMax.Name = "rbChanceMax";
            this.rbChanceMax.Size = new System.Drawing.Size(161, 20);
            this.rbChanceMax.TabIndex = 6;
            this.rbChanceMax.Text = "Maximum (Always Include)";
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(8, 16);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(522, 137);
            this.Label9.TabIndex = 4;
            this.Label9.Text = resources.GetString("Label9.Text");
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.Label7);
            this.GroupBox6.Controls.Add(this.rbPvP);
            this.GroupBox6.Controls.Add(this.rbPvE);
            this.GroupBox6.Location = new System.Drawing.Point(8, 192);
            this.GroupBox6.Name = "GroupBox6";
            this.GroupBox6.Size = new System.Drawing.Size(301, 122);
            this.GroupBox6.TabIndex = 1;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Targets:";
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(8, 16);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(295, 54);
            this.Label7.TabIndex = 2;
            this.Label7.Text = "Some powers  have different effects when used against players (Mez effects are a " +
    "good example of this). Where a power has different effects, which should be disp" +
    "layed?";
            // 
            // rbPvP
            // 
            this.rbPvP.Location = new System.Drawing.Point(11, 93);
            this.rbPvP.Name = "rbPvP";
            this.rbPvP.Size = new System.Drawing.Size(260, 20);
            this.rbPvP.TabIndex = 1;
            this.rbPvP.Text = "Show values for Players (PvP)";
            // 
            // rbPvE
            // 
            this.rbPvE.Checked = true;
            this.rbPvE.Location = new System.Drawing.Point(11, 73);
            this.rbPvE.Name = "rbPvE";
            this.rbPvE.Size = new System.Drawing.Size(260, 20);
            this.rbPvE.TabIndex = 0;
            this.rbPvE.TabStop = true;
            this.rbPvE.Text = "Show values for Mobs (PvE)";
            // 
            // TabPage4
            // 
            this.TabPage4.Controls.Add(this.GroupBox12);
            this.TabPage4.Controls.Add(this.GroupBox11);
            this.TabPage4.Location = new System.Drawing.Point(4, 23);
            this.TabPage4.Name = "TabPage4";
            this.TabPage4.Size = new System.Drawing.Size(777, 345);
            this.TabPage4.TabIndex = 3;
            this.TabPage4.Text = "Forum Export Settings";
            this.TabPage4.UseVisualStyleBackColor = true;
            // 
            // GroupBox12
            // 
            this.GroupBox12.Controls.Add(this.fcReset);
            this.GroupBox12.Controls.Add(this.fcSet);
            this.GroupBox12.Controls.Add(this.fcNotes);
            this.GroupBox12.Controls.Add(this.fcDelete);
            this.GroupBox12.Controls.Add(this.fcAdd);
            this.GroupBox12.Controls.Add(this.fcName);
            this.GroupBox12.Controls.Add(this.fcWSTab);
            this.GroupBox12.Controls.Add(this.fcWSSpace);
            this.GroupBox12.Controls.Add(this.fcUnderlineOff);
            this.GroupBox12.Controls.Add(this.fcUnderlineOn);
            this.GroupBox12.Controls.Add(this.Label32);
            this.GroupBox12.Controls.Add(this.fcItalicOff);
            this.GroupBox12.Controls.Add(this.fcItalicOn);
            this.GroupBox12.Controls.Add(this.Label31);
            this.GroupBox12.Controls.Add(this.fcBoldOff);
            this.GroupBox12.Controls.Add(this.fcBoldOn);
            this.GroupBox12.Controls.Add(this.Label30);
            this.GroupBox12.Controls.Add(this.fcTextOff);
            this.GroupBox12.Controls.Add(this.fcTextOn);
            this.GroupBox12.Controls.Add(this.Label29);
            this.GroupBox12.Controls.Add(this.Label28);
            this.GroupBox12.Controls.Add(this.Label27);
            this.GroupBox12.Controls.Add(this.fcColorOff);
            this.GroupBox12.Controls.Add(this.fcColorOn);
            this.GroupBox12.Controls.Add(this.Label26);
            this.GroupBox12.Controls.Add(this.fcList);
            this.GroupBox12.Controls.Add(this.Label25);
            this.GroupBox12.Controls.Add(this.Label24);
            this.GroupBox12.Controls.Add(this.Label33);
            this.GroupBox12.Location = new System.Drawing.Point(180, 8);
            this.GroupBox12.Name = "GroupBox12";
            this.GroupBox12.Size = new System.Drawing.Size(589, 308);
            this.GroupBox12.TabIndex = 1;
            this.GroupBox12.TabStop = false;
            this.GroupBox12.Text = "Formatting Codes:";
            // 
            // fcReset
            // 
            this.fcReset.Location = new System.Drawing.Point(16, 280);
            this.fcReset.Name = "fcReset";
            this.fcReset.Size = new System.Drawing.Size(124, 24);
            this.fcReset.TabIndex = 18;
            this.fcReset.Text = "Reset To Defaults";
            this.fcReset.Click += new System.EventHandler(this.fcReset_Click);
            // 
            // fcSet
            // 
            this.fcSet.Location = new System.Drawing.Point(8, 200);
            this.fcSet.Name = "fcSet";
            this.fcSet.Size = new System.Drawing.Size(136, 20);
            this.fcSet.TabIndex = 4;
            this.fcSet.Text = "Set New Name";
            this.fcSet.Click += new System.EventHandler(this.fcSet_Click);
            // 
            // fcNotes
            // 
            this.fcNotes.Location = new System.Drawing.Point(8, 228);
            this.fcNotes.Multiline = true;
            this.fcNotes.Name = "fcNotes";
            this.fcNotes.Size = new System.Drawing.Size(136, 48);
            this.fcNotes.TabIndex = 5;
            this.fcNotes.TextChanged += new System.EventHandler(this.fcNotes_TextChanged);
            // 
            // fcDelete
            // 
            this.fcDelete.Location = new System.Drawing.Point(8, 148);
            this.fcDelete.Name = "fcDelete";
            this.fcDelete.Size = new System.Drawing.Size(64, 20);
            this.fcDelete.TabIndex = 1;
            this.fcDelete.Text = "Delete";
            this.fcDelete.Click += new System.EventHandler(this.fcDelete_Click);
            // 
            // fcAdd
            // 
            this.fcAdd.Location = new System.Drawing.Point(80, 148);
            this.fcAdd.Name = "fcAdd";
            this.fcAdd.Size = new System.Drawing.Size(64, 20);
            this.fcAdd.TabIndex = 2;
            this.fcAdd.Text = "Add";
            this.fcAdd.Click += new System.EventHandler(this.fcAdd_Click);
            // 
            // fcName
            // 
            this.fcName.Location = new System.Drawing.Point(8, 176);
            this.fcName.Name = "fcName";
            this.fcName.Size = new System.Drawing.Size(136, 20);
            this.fcName.TabIndex = 3;
            // 
            // fcWSTab
            // 
            this.fcWSTab.Location = new System.Drawing.Point(304, 196);
            this.fcWSTab.Name = "fcWSTab";
            this.fcWSTab.Size = new System.Drawing.Size(80, 20);
            this.fcWSTab.TabIndex = 17;
            this.fcWSTab.Text = "Tab";
            this.fcWSTab.CheckedChanged += new System.EventHandler(this.fcWSSpace_CheckedChanged);
            // 
            // fcWSSpace
            // 
            this.fcWSSpace.Location = new System.Drawing.Point(220, 196);
            this.fcWSSpace.Name = "fcWSSpace";
            this.fcWSSpace.Size = new System.Drawing.Size(80, 20);
            this.fcWSSpace.TabIndex = 16;
            this.fcWSSpace.Text = "Space";
            this.fcWSSpace.CheckedChanged += new System.EventHandler(this.fcWSSpace_CheckedChanged);
            // 
            // fcUnderlineOff
            // 
            this.fcUnderlineOff.Location = new System.Drawing.Point(324, 160);
            this.fcUnderlineOff.Name = "fcUnderlineOff";
            this.fcUnderlineOff.Size = new System.Drawing.Size(60, 20);
            this.fcUnderlineOff.TabIndex = 15;
            this.fcUnderlineOff.TextChanged += new System.EventHandler(this.fcUnderlineOff_TextChanged);
            // 
            // fcUnderlineOn
            // 
            this.fcUnderlineOn.Location = new System.Drawing.Point(220, 160);
            this.fcUnderlineOn.Name = "fcUnderlineOn";
            this.fcUnderlineOn.Size = new System.Drawing.Size(100, 20);
            this.fcUnderlineOn.TabIndex = 14;
            this.fcUnderlineOn.TextChanged += new System.EventHandler(this.fcUnderlineOn_TextChanged);
            // 
            // Label32
            // 
            this.Label32.Location = new System.Drawing.Point(148, 160);
            this.Label32.Name = "Label32";
            this.Label32.Size = new System.Drawing.Size(68, 20);
            this.Label32.TabIndex = 30;
            this.Label32.Text = "Underline:";
            this.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fcItalicOff
            // 
            this.fcItalicOff.Location = new System.Drawing.Point(324, 136);
            this.fcItalicOff.Name = "fcItalicOff";
            this.fcItalicOff.Size = new System.Drawing.Size(60, 20);
            this.fcItalicOff.TabIndex = 13;
            this.fcItalicOff.TextChanged += new System.EventHandler(this.fcItalicOff_TextChanged);
            // 
            // fcItalicOn
            // 
            this.fcItalicOn.Location = new System.Drawing.Point(220, 136);
            this.fcItalicOn.Name = "fcItalicOn";
            this.fcItalicOn.Size = new System.Drawing.Size(100, 20);
            this.fcItalicOn.TabIndex = 12;
            this.fcItalicOn.TextChanged += new System.EventHandler(this.fcItalicOn_TextChanged);
            // 
            // Label31
            // 
            this.Label31.Location = new System.Drawing.Point(148, 136);
            this.Label31.Name = "Label31";
            this.Label31.Size = new System.Drawing.Size(68, 20);
            this.Label31.TabIndex = 27;
            this.Label31.Text = "Italic:";
            this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fcBoldOff
            // 
            this.fcBoldOff.Location = new System.Drawing.Point(324, 112);
            this.fcBoldOff.Name = "fcBoldOff";
            this.fcBoldOff.Size = new System.Drawing.Size(60, 20);
            this.fcBoldOff.TabIndex = 11;
            this.fcBoldOff.TextChanged += new System.EventHandler(this.fcBoldOff_TextChanged);
            // 
            // fcBoldOn
            // 
            this.fcBoldOn.Location = new System.Drawing.Point(220, 112);
            this.fcBoldOn.Name = "fcBoldOn";
            this.fcBoldOn.Size = new System.Drawing.Size(100, 20);
            this.fcBoldOn.TabIndex = 10;
            this.fcBoldOn.TextChanged += new System.EventHandler(this.fcBoldOn_TextChanged);
            // 
            // Label30
            // 
            this.Label30.Location = new System.Drawing.Point(148, 112);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(68, 20);
            this.Label30.TabIndex = 24;
            this.Label30.Text = "Bold:";
            this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fcTextOff
            // 
            this.fcTextOff.Location = new System.Drawing.Point(324, 88);
            this.fcTextOff.Name = "fcTextOff";
            this.fcTextOff.Size = new System.Drawing.Size(60, 20);
            this.fcTextOff.TabIndex = 9;
            this.fcTextOff.TextChanged += new System.EventHandler(this.fcTextOff_TextChanged);
            // 
            // fcTextOn
            // 
            this.fcTextOn.Location = new System.Drawing.Point(220, 88);
            this.fcTextOn.Name = "fcTextOn";
            this.fcTextOn.Size = new System.Drawing.Size(100, 20);
            this.fcTextOn.TabIndex = 8;
            this.fcTextOn.TextChanged += new System.EventHandler(this.fcTextOn_TextChanged);
            // 
            // Label29
            // 
            this.Label29.Location = new System.Drawing.Point(148, 88);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(68, 20);
            this.Label29.TabIndex = 21;
            this.Label29.Text = "Code Block:";
            this.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label28
            // 
            this.Label28.Location = new System.Drawing.Point(324, 44);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(60, 16);
            this.Label28.TabIndex = 20;
            this.Label28.Text = "Off";
            this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label27
            // 
            this.Label27.Location = new System.Drawing.Point(220, 44);
            this.Label27.Name = "Label27";
            this.Label27.Size = new System.Drawing.Size(100, 16);
            this.Label27.TabIndex = 19;
            this.Label27.Text = "On";
            this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fcColorOff
            // 
            this.fcColorOff.Location = new System.Drawing.Point(324, 64);
            this.fcColorOff.Name = "fcColorOff";
            this.fcColorOff.Size = new System.Drawing.Size(60, 20);
            this.fcColorOff.TabIndex = 7;
            this.fcColorOff.TextChanged += new System.EventHandler(this.fcColorOff_TextChanged);
            // 
            // fcColorOn
            // 
            this.fcColorOn.Location = new System.Drawing.Point(220, 64);
            this.fcColorOn.Name = "fcColorOn";
            this.fcColorOn.Size = new System.Drawing.Size(100, 20);
            this.fcColorOn.TabIndex = 6;
            this.fcColorOn.TextChanged += new System.EventHandler(this.fcColorOn_TextChanged);
            // 
            // Label26
            // 
            this.Label26.Location = new System.Drawing.Point(148, 64);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(68, 20);
            this.Label26.TabIndex = 16;
            this.Label26.Text = "Color:";
            this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fcList
            // 
            this.fcList.ItemHeight = 14;
            this.fcList.Location = new System.Drawing.Point(8, 40);
            this.fcList.Name = "fcList";
            this.fcList.Size = new System.Drawing.Size(136, 102);
            this.fcList.TabIndex = 0;
            this.fcList.SelectedIndexChanged += new System.EventHandler(this.fcList_SelectedIndexChanged);
            this.fcList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fcList_KeyPress);
            // 
            // Label25
            // 
            this.Label25.Location = new System.Drawing.Point(148, 224);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(435, 76);
            this.Label25.TabIndex = 14;
            this.Label25.Text = "When defining a formatting code which takes a value, such as a color tag, use %VA" +
    "L% as a placeholder for the actual value, which will replace it when a build is " +
    "exported.";
            this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label24
            // 
            this.Label24.Location = new System.Drawing.Point(8, 16);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(344, 20);
            this.Label24.TabIndex = 13;
            this.Label24.Text = "You can set the formatting codes available for Forum Export here.";
            // 
            // Label33
            // 
            this.Label33.Location = new System.Drawing.Point(140, 196);
            this.Label33.Name = "Label33";
            this.Label33.Size = new System.Drawing.Size(76, 20);
            this.Label33.TabIndex = 33;
            this.Label33.Text = "White Space:";
            this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox11
            // 
            this.GroupBox11.Controls.Add(this.csReset);
            this.GroupBox11.Controls.Add(this.csBtnEdit);
            this.GroupBox11.Controls.Add(this.csDelete);
            this.GroupBox11.Controls.Add(this.csAdd);
            this.GroupBox11.Controls.Add(this.csList);
            this.GroupBox11.Location = new System.Drawing.Point(12, 8);
            this.GroupBox11.Name = "GroupBox11";
            this.GroupBox11.Size = new System.Drawing.Size(160, 308);
            this.GroupBox11.TabIndex = 0;
            this.GroupBox11.TabStop = false;
            this.GroupBox11.Text = "Color Schemes:";
            // 
            // csReset
            // 
            this.csReset.Location = new System.Drawing.Point(8, 280);
            this.csReset.Name = "csReset";
            this.csReset.Size = new System.Drawing.Size(144, 24);
            this.csReset.TabIndex = 4;
            this.csReset.Text = "Reset To Defaults";
            this.csReset.Click += new System.EventHandler(this.csReset_Click);
            // 
            // csBtnEdit
            // 
            this.csBtnEdit.Location = new System.Drawing.Point(8, 242);
            this.csBtnEdit.Name = "csBtnEdit";
            this.csBtnEdit.Size = new System.Drawing.Size(144, 32);
            this.csBtnEdit.TabIndex = 3;
            this.csBtnEdit.Text = "Edit...";
            this.csBtnEdit.Click += new System.EventHandler(this.csBtnEdit_Click);
            // 
            // csDelete
            // 
            this.csDelete.Location = new System.Drawing.Point(8, 216);
            this.csDelete.Name = "csDelete";
            this.csDelete.Size = new System.Drawing.Size(64, 20);
            this.csDelete.TabIndex = 1;
            this.csDelete.Text = "Delete";
            this.csDelete.Click += new System.EventHandler(this.csDelete_Click);
            // 
            // csAdd
            // 
            this.csAdd.Location = new System.Drawing.Point(88, 216);
            this.csAdd.Name = "csAdd";
            this.csAdd.Size = new System.Drawing.Size(64, 20);
            this.csAdd.TabIndex = 2;
            this.csAdd.Text = "Add";
            this.csAdd.Click += new System.EventHandler(this.csAdd_Click);
            // 
            // csList
            // 
            this.csList.ItemHeight = 14;
            this.csList.Location = new System.Drawing.Point(8, 20);
            this.csList.Name = "csList";
            this.csList.Size = new System.Drawing.Size(144, 186);
            this.csList.TabIndex = 0;
            this.csList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.csList_KeyPress);
            // 
            // TabPage1
            // 
            this.TabPage1.Controls.Add(this.Label15);
            this.TabPage1.Controls.Add(this.Label10);
            this.TabPage1.Controls.Add(this.cmbAction);
            this.TabPage1.Controls.Add(this.GroupBox9);
            this.TabPage1.Controls.Add(this.GroupBox7);
            this.TabPage1.Location = new System.Drawing.Point(4, 23);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Size = new System.Drawing.Size(777, 345);
            this.TabPage1.TabIndex = 6;
            this.TabPage1.Text = "Drag & Drop";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // Label15
            // 
            this.Label15.Location = new System.Drawing.Point(14, 9);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(755, 92);
            this.Label15.TabIndex = 4;
            this.Label15.Text = resources.GetString("Label15.Text");
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(19, 268);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(285, 15);
            this.Label10.TabIndex = 3;
            this.Label10.Text = "Action to take whenever the above scenario occurs:";
            // 
            // cmbAction
            // 
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Location = new System.Drawing.Point(18, 293);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(356, 22);
            this.cmbAction.TabIndex = 2;
            this.cmbAction.SelectedIndexChanged += new System.EventHandler(this.cmbAction_SelectedIndexChanged);
            // 
            // GroupBox9
            // 
            this.GroupBox9.Controls.Add(this.lblExample);
            this.GroupBox9.Location = new System.Drawing.Point(403, 104);
            this.GroupBox9.Name = "GroupBox9";
            this.GroupBox9.Size = new System.Drawing.Size(366, 161);
            this.GroupBox9.TabIndex = 1;
            this.GroupBox9.TabStop = false;
            this.GroupBox9.Text = "Description / Example(s)";
            // 
            // lblExample
            // 
            this.lblExample.Location = new System.Drawing.Point(13, 17);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(347, 141);
            this.lblExample.TabIndex = 0;
            // 
            // GroupBox7
            // 
            this.GroupBox7.Controls.Add(this.listScenarios);
            this.GroupBox7.Location = new System.Drawing.Point(8, 102);
            this.GroupBox7.Name = "GroupBox7";
            this.GroupBox7.Size = new System.Drawing.Size(380, 163);
            this.GroupBox7.TabIndex = 0;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Scenario";
            // 
            // listScenarios
            // 
            this.listScenarios.FormattingEnabled = true;
            this.listScenarios.ItemHeight = 14;
            this.listScenarios.Items.AddRange(new object[] {
            "Power is moved or swapped too low",
            "Power is moved too high (some powers will no longer fit)",
            "Power is moved or swapped higher than slots\' levels",
            "Power is moved or swapped too high to have # slots",
            "Power being replaced is swapped too low",
            "Power being replaced is swapped higher than slots\' levels",
            "Power being replaced is swapped too high to have # slots",
            "Power being shifted down cannot shift to the necessary level",
            "Power being shifted up has slots from lower levels",
            "Power being shifted up has impossible # of slots",
            "There is a gap in a group of powers that are being shifted",
            "A power placed at its minimum level is being shifted up",
            "The power in the destination slot is prevented from being shifted up",
            "Slot being level-swapped is too low for the destination power",
            "Slot being level-swapped is too low for the source power"});
            this.listScenarios.Location = new System.Drawing.Point(13, 19);
            this.listScenarios.Name = "listScenarios";
            this.listScenarios.Size = new System.Drawing.Size(353, 116);
            this.listScenarios.TabIndex = 0;
            this.listScenarios.SelectedIndexChanged += new System.EventHandler(this.listScenarios_SelectedIndexChanged);
            // 
            // TabPage5
            // 
            this.TabPage5.Controls.Add(this.groupBox16);
            this.TabPage5.Controls.Add(this.groupBox19);
            this.TabPage5.Controls.Add(this.btnSaveFolderReset);
            this.TabPage5.Controls.Add(this.lblSaveFolder);
            this.TabPage5.Controls.Add(this.btnSaveFolder);
            this.TabPage5.Controls.Add(this.chkLoadLastFile);
            this.TabPage5.Controls.Add(this.Label1);
            this.TabPage5.Controls.Add(this.GroupBox1);
            this.TabPage5.Location = new System.Drawing.Point(4, 23);
            this.TabPage5.Name = "TabPage5";
            this.TabPage5.Size = new System.Drawing.Size(777, 345);
            this.TabPage5.TabIndex = 4;
            this.TabPage5.Text = "Updates & Paths";
            this.TabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.lblDatabaseLoc);
            this.groupBox16.Controls.Add(this.btnResetDatabaseLoc);
            this.groupBox16.Controls.Add(this.label35);
            this.groupBox16.Controls.Add(this.btnDatabaseLoc);
            this.groupBox16.Location = new System.Drawing.Point(8, 151);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(403, 87);
            this.groupBox16.TabIndex = 77;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Database Save Location:";
            // 
            // lblDatabaseLoc
            // 
            this.lblDatabaseLoc.Location = new System.Drawing.Point(37, 32);
            this.lblDatabaseLoc.Name = "lblDatabaseLoc";
            this.lblDatabaseLoc.ReadOnly = true;
            this.lblDatabaseLoc.Size = new System.Drawing.Size(261, 20);
            this.lblDatabaseLoc.TabIndex = 78;
            // 
            // btnResetDatabaseLoc
            // 
            this.btnResetDatabaseLoc.Location = new System.Drawing.Point(303, 59);
            this.btnResetDatabaseLoc.Name = "btnResetDatabaseLoc";
            this.btnResetDatabaseLoc.Size = new System.Drawing.Size(94, 22);
            this.btnResetDatabaseLoc.TabIndex = 77;
            this.btnResetDatabaseLoc.Text = "Reset to Default";
            this.btnResetDatabaseLoc.UseVisualStyleBackColor = true;
            this.btnResetDatabaseLoc.Click += new System.EventHandler(this.btnResetDatabaseLoc_Click);
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(6, 30);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(34, 24);
            this.label35.TabIndex = 74;
            this.label35.Text = "Path:";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDatabaseLoc
            // 
            this.btnDatabaseLoc.Location = new System.Drawing.Point(304, 31);
            this.btnDatabaseLoc.Name = "btnDatabaseLoc";
            this.btnDatabaseLoc.Size = new System.Drawing.Size(93, 22);
            this.btnDatabaseLoc.TabIndex = 76;
            this.btnDatabaseLoc.Text = "Browse";
            this.btnDatabaseLoc.UseVisualStyleBackColor = true;
            this.btnDatabaseLoc.Click += new System.EventHandler(this.btnDatabaseLoc_Click);
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.btnFileAssoc);
            this.groupBox19.Controls.Add(this.lblFileAssoc);
            this.groupBox19.Controls.Add(this.lblAssocStatus);
            this.groupBox19.Location = new System.Drawing.Point(417, 151);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(324, 87);
            this.groupBox19.TabIndex = 71;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "File Association Repair:";
            // 
            // btnFileAssoc
            // 
            this.btnFileAssoc.Location = new System.Drawing.Point(9, 47);
            this.btnFileAssoc.Name = "btnFileAssoc";
            this.btnFileAssoc.Size = new System.Drawing.Size(129, 36);
            this.btnFileAssoc.TabIndex = 68;
            this.btnFileAssoc.Text = "Rebuild file associations";
            this.btnFileAssoc.UseVisualStyleBackColor = true;
            this.btnFileAssoc.Click += new System.EventHandler(this.btnFileAssoc_Click);
            // 
            // lblFileAssoc
            // 
            this.lblFileAssoc.Location = new System.Drawing.Point(5, 16);
            this.lblFileAssoc.Name = "lblFileAssoc";
            this.lblFileAssoc.Size = new System.Drawing.Size(313, 28);
            this.lblFileAssoc.TabIndex = 69;
            this.lblFileAssoc.Text = "If you need to reassociate your .MXD build files with MRB you can do so by clicki" +
    "ng the button below.\r\n";
            // 
            // lblAssocStatus
            // 
            this.lblAssocStatus.Location = new System.Drawing.Point(144, 57);
            this.lblAssocStatus.Name = "lblAssocStatus";
            this.lblAssocStatus.Size = new System.Drawing.Size(174, 14);
            this.lblAssocStatus.TabIndex = 70;
            this.lblAssocStatus.Text = "Status: --";
            // 
            // btnSaveFolderReset
            // 
            this.btnSaveFolderReset.Location = new System.Drawing.Point(616, 298);
            this.btnSaveFolderReset.Name = "btnSaveFolderReset";
            this.btnSaveFolderReset.Size = new System.Drawing.Size(105, 22);
            this.btnSaveFolderReset.TabIndex = 64;
            this.btnSaveFolderReset.Text = "Reset to Default";
            this.btnSaveFolderReset.UseVisualStyleBackColor = true;
            this.btnSaveFolderReset.Click += new System.EventHandler(this.btnSaveFolderReset_Click);
            // 
            // lblSaveFolder
            // 
            this.lblSaveFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSaveFolder.Location = new System.Drawing.Point(99, 270);
            this.lblSaveFolder.Name = "lblSaveFolder";
            this.lblSaveFolder.Size = new System.Drawing.Size(511, 22);
            this.lblSaveFolder.TabIndex = 63;
            this.lblSaveFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSaveFolder.UseMnemonic = false;
            // 
            // btnSaveFolder
            // 
            this.btnSaveFolder.Location = new System.Drawing.Point(616, 270);
            this.btnSaveFolder.Name = "btnSaveFolder";
            this.btnSaveFolder.Size = new System.Drawing.Size(105, 22);
            this.btnSaveFolder.TabIndex = 62;
            this.btnSaveFolder.Text = "Browse...";
            this.btnSaveFolder.UseVisualStyleBackColor = true;
            this.btnSaveFolder.Click += new System.EventHandler(this.btnSaveFolder_Click);
            // 
            // chkLoadLastFile
            // 
            this.chkLoadLastFile.Location = new System.Drawing.Point(11, 301);
            this.chkLoadLastFile.Name = "chkLoadLastFile";
            this.chkLoadLastFile.Size = new System.Drawing.Size(156, 18);
            this.chkLoadLastFile.TabIndex = 61;
            this.chkLoadLastFile.Text = "Load last build on startup";
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(8, 269);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(85, 24);
            this.Label1.TabIndex = 8;
            this.Label1.Text = "Save Builds To:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.Label37);
            this.GroupBox1.Controls.Add(this.Label34);
            this.GroupBox1.Controls.Add(this.chkUpdates);
            this.GroupBox1.Controls.Add(this.cbDBUpdateURL);
            this.GroupBox1.Controls.Add(this.cbUpdateURL);
            this.GroupBox1.Controls.Add(this.label23);
            this.GroupBox1.Controls.Add(this.lblUpdateURL);
            this.GroupBox1.Location = new System.Drawing.Point(8, 3);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(733, 142);
            this.GroupBox1.TabIndex = 7;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Automatic Updates:";
            // 
            // Label37
            // 
            this.Label37.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label37.Location = new System.Drawing.Point(67, 48);
            this.Label37.Name = "Label37";
            this.Label37.Size = new System.Drawing.Size(353, 30);
            this.Label37.TabIndex = 7;
            this.Label37.Text = "*Please note that the availability of automatic updates may vary due to bandwidth" +
    " use of the hosting site.*";
            this.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label34
            // 
            this.Label34.Location = new System.Drawing.Point(6, 16);
            this.Label34.Name = "Label34";
            this.Label34.Size = new System.Drawing.Size(478, 32);
            this.Label34.TabIndex = 5;
            this.Label34.Text = "Mids Reborn can automatically check for updates and download newer versions when " +
    "it starts. This feature requires an internet connection.";
            this.Label34.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // chkUpdates
            // 
            this.chkUpdates.Location = new System.Drawing.Point(535, 48);
            this.chkUpdates.Name = "chkUpdates";
            this.chkUpdates.Size = new System.Drawing.Size(167, 24);
            this.chkUpdates.TabIndex = 6;
            this.chkUpdates.Text = "Check for updates on startup";
            // 
            // cbDBUpdateURL
            // 
            this.cbDBUpdateURL.AutoCompleteCustomSource.AddRange(new string[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml",
            "http://keepers.dk/mids/update.xml"});
            this.cbDBUpdateURL.FormattingEnabled = true;
            this.cbDBUpdateURL.Items.AddRange(new object[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml"});
            this.cbDBUpdateURL.Location = new System.Drawing.Point(174, 114);
            this.cbDBUpdateURL.Name = "cbDBUpdateURL";
            this.cbDBUpdateURL.Size = new System.Drawing.Size(479, 22);
            this.cbDBUpdateURL.TabIndex = 73;
            // 
            // cbUpdateURL
            // 
            this.cbUpdateURL.AutoCompleteCustomSource.AddRange(new string[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml",
            "http://keepers.dk/mids/update.xml"});
            this.cbUpdateURL.FormattingEnabled = true;
            this.cbUpdateURL.Items.AddRange(new object[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml"});
            this.cbUpdateURL.Location = new System.Drawing.Point(174, 86);
            this.cbUpdateURL.Name = "cbUpdateURL";
            this.cbUpdateURL.Size = new System.Drawing.Size(479, 22);
            this.cbUpdateURL.TabIndex = 67;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(25, 112);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(137, 24);
            this.label23.TabIndex = 72;
            this.label23.Text = "Database Update Manifest:";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUpdateURL
            // 
            this.lblUpdateURL.Location = new System.Drawing.Point(19, 84);
            this.lblUpdateURL.Name = "lblUpdateURL";
            this.lblUpdateURL.Size = new System.Drawing.Size(149, 24);
            this.lblUpdateURL.TabIndex = 65;
            this.lblUpdateURL.Text = "Application Update Manifest:";
            this.lblUpdateURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TabPage6
            // 
            this.TabPage6.Controls.Add(this.groupBox20);
            this.TabPage6.Controls.Add(this.groupBox13);
            this.TabPage6.Controls.Add(this.groupBox10);
            this.TabPage6.Controls.Add(this.groupBox4);
            this.TabPage6.Location = new System.Drawing.Point(4, 23);
            this.TabPage6.Name = "TabPage6";
            this.TabPage6.Size = new System.Drawing.Size(777, 345);
            this.TabPage6.TabIndex = 5;
            this.TabPage6.Text = "Server Specific Values";
            this.TabPage6.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.label38);
            this.groupBox20.Controls.Add(this.udStaminaSecond);
            this.groupBox20.Controls.Add(this.udHealthSecond);
            this.groupBox20.Controls.Add(this.label52);
            this.groupBox20.Controls.Add(this.label51);
            this.groupBox20.Controls.Add(this.udStaminaFirst);
            this.groupBox20.Controls.Add(this.udHealthFirst);
            this.groupBox20.Controls.Add(this.label50);
            this.groupBox20.Controls.Add(this.label49);
            this.groupBox20.Controls.Add(this.udStaminaSlots);
            this.groupBox20.Controls.Add(this.label46);
            this.groupBox20.Controls.Add(this.udHealthSlots);
            this.groupBox20.Controls.Add(this.label43);
            this.groupBox20.Location = new System.Drawing.Point(404, 236);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(337, 106);
            this.groupBox20.TabIndex = 73;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "Extra Fitness Slotting (Overrides Max Slots)";
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(6, 19);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(325, 29);
            this.label38.TabIndex = 82;
            this.label38.Text = "Note: You need to add the extra slots to your Level map mhds prior to setting the" +
    "se.\r\n";
            // 
            // udStaminaSecond
            // 
            this.udStaminaSecond.Location = new System.Drawing.Point(281, 77);
            this.udStaminaSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udStaminaSecond.Name = "udStaminaSecond";
            this.udStaminaSecond.Size = new System.Drawing.Size(41, 20);
            this.udStaminaSecond.TabIndex = 81;
            this.myTip.SetToolTip(this.udStaminaSecond, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udStaminaSecond.Value = new decimal(new int[] {
            22,
            0,
            0,
            0});
            // 
            // udHealthSecond
            // 
            this.udHealthSecond.Location = new System.Drawing.Point(281, 51);
            this.udHealthSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udHealthSecond.Name = "udHealthSecond";
            this.udHealthSecond.Size = new System.Drawing.Size(41, 20);
            this.udHealthSecond.TabIndex = 80;
            this.myTip.SetToolTip(this.udHealthSecond, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udHealthSecond.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(261, 80);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(14, 14);
            this.label52.TabIndex = 79;
            this.label52.Text = "&&";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(261, 54);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(14, 14);
            this.label51.TabIndex = 78;
            this.label51.Text = "&&";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udStaminaFirst
            // 
            this.udStaminaFirst.Location = new System.Drawing.Point(220, 77);
            this.udStaminaFirst.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udStaminaFirst.Name = "udStaminaFirst";
            this.udStaminaFirst.Size = new System.Drawing.Size(35, 20);
            this.udStaminaFirst.TabIndex = 77;
            this.myTip.SetToolTip(this.udStaminaFirst, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udStaminaFirst.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // udHealthFirst
            // 
            this.udHealthFirst.Location = new System.Drawing.Point(220, 51);
            this.udHealthFirst.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udHealthFirst.Name = "udHealthFirst";
            this.udHealthFirst.Size = new System.Drawing.Size(35, 20);
            this.udHealthFirst.TabIndex = 76;
            this.myTip.SetToolTip(this.udHealthFirst, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udHealthFirst.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(129, 80);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(85, 14);
            this.label50.TabIndex = 75;
            this.label50.Text = "Given at Levels:";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(129, 53);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(85, 14);
            this.label49.TabIndex = 74;
            this.label49.Text = "Given at Levels:";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udStaminaSlots
            // 
            this.udStaminaSlots.Location = new System.Drawing.Point(90, 77);
            this.udStaminaSlots.Name = "udStaminaSlots";
            this.udStaminaSlots.Size = new System.Drawing.Size(33, 20);
            this.udStaminaSlots.TabIndex = 72;
            this.myTip.SetToolTip(this.udStaminaSlots, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udStaminaSlots.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label46
            // 
            this.label46.Location = new System.Drawing.Point(9, 77);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(75, 20);
            this.label46.TabIndex = 73;
            this.label46.Text = "Stamina Slots:";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udHealthSlots
            // 
            this.udHealthSlots.Location = new System.Drawing.Point(90, 51);
            this.udHealthSlots.Name = "udHealthSlots";
            this.udHealthSlots.Size = new System.Drawing.Size(33, 20);
            this.udHealthSlots.TabIndex = 70;
            this.myTip.SetToolTip(this.udHealthSlots, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udHealthSlots.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(9, 51);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(75, 20);
            this.label43.TabIndex = 71;
            this.label43.Text = "Health Slots:";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.label48);
            this.groupBox13.Controls.Add(this.label47);
            this.groupBox13.Controls.Add(this.udMaxSlots);
            this.groupBox13.Location = new System.Drawing.Point(8, 236);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(390, 106);
            this.groupBox13.TabIndex = 72;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Other Values (Mids Specific)";
            // 
            // label48
            // 
            this.label48.Location = new System.Drawing.Point(6, 19);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(370, 41);
            this.label48.TabIndex = 68;
            this.label48.Text = "These values should never change unless you allow more for your server.\r\nNote: Yo" +
    "u may need to change the level maps if you increase the slots.\r\n";
            // 
            // label47
            // 
            this.label47.Location = new System.Drawing.Point(6, 63);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(176, 20);
            this.label47.TabIndex = 67;
            this.label47.Text = "Max Slots (Minus Default 1st Slot):";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udMaxSlots
            // 
            this.udMaxSlots.Location = new System.Drawing.Point(188, 63);
            this.udMaxSlots.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxSlots.Name = "udMaxSlots";
            this.udMaxSlots.Size = new System.Drawing.Size(60, 20);
            this.udMaxSlots.TabIndex = 66;
            this.myTip.SetToolTip(this.udMaxSlots, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udMaxSlots.Value = new decimal(new int[] {
            67,
            0,
            0,
            0});
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.label45);
            this.groupBox10.Controls.Add(this.label39);
            this.groupBox10.Controls.Add(this.udMaxRunSpeed);
            this.groupBox10.Controls.Add(this.label41);
            this.groupBox10.Controls.Add(this.label42);
            this.groupBox10.Controls.Add(this.udMaxFlySpeed);
            this.groupBox10.Controls.Add(this.udMaxJumpSpeed);
            this.groupBox10.Location = new System.Drawing.Point(429, 3);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(312, 227);
            this.groupBox10.TabIndex = 71;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Cap/Max Values";
            // 
            // label45
            // 
            this.label45.Location = new System.Drawing.Point(6, 16);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(300, 48);
            this.label45.TabIndex = 70;
            this.label45.Text = "These values are used as the maximum for stats calculation. They shouldn\'t ever n" +
    "eed to be changed unless they differ on your server.";
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(3, 135);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(99, 20);
            this.label39.TabIndex = 69;
            this.label39.Text = "Max Run Speed:";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udMaxRunSpeed
            // 
            this.udMaxRunSpeed.DecimalPlaces = 2;
            this.udMaxRunSpeed.Location = new System.Drawing.Point(108, 136);
            this.udMaxRunSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udMaxRunSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxRunSpeed.Name = "udMaxRunSpeed";
            this.udMaxRunSpeed.Size = new System.Drawing.Size(88, 20);
            this.udMaxRunSpeed.TabIndex = 68;
            this.myTip.SetToolTip(this.udMaxRunSpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udMaxRunSpeed.Value = new decimal(new int[] {
            13567,
            0,
            0,
            131072});
            // 
            // label41
            // 
            this.label41.Location = new System.Drawing.Point(3, 84);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(99, 20);
            this.label41.TabIndex = 65;
            this.label41.Text = "Max Fly Speed:";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label42
            // 
            this.label42.Location = new System.Drawing.Point(3, 110);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(99, 20);
            this.label42.TabIndex = 67;
            this.label42.Text = "Max Jump Speed:";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udMaxFlySpeed
            // 
            this.udMaxFlySpeed.DecimalPlaces = 2;
            this.udMaxFlySpeed.Location = new System.Drawing.Point(108, 84);
            this.udMaxFlySpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udMaxFlySpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxFlySpeed.Name = "udMaxFlySpeed";
            this.udMaxFlySpeed.Size = new System.Drawing.Size(88, 20);
            this.udMaxFlySpeed.TabIndex = 62;
            this.myTip.SetToolTip(this.udMaxFlySpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udMaxFlySpeed.Value = new decimal(new int[] {
            86,
            0,
            0,
            0});
            // 
            // udMaxJumpSpeed
            // 
            this.udMaxJumpSpeed.DecimalPlaces = 2;
            this.udMaxJumpSpeed.Location = new System.Drawing.Point(108, 110);
            this.udMaxJumpSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udMaxJumpSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxJumpSpeed.Name = "udMaxJumpSpeed";
            this.udMaxJumpSpeed.Size = new System.Drawing.Size(88, 20);
            this.udMaxJumpSpeed.TabIndex = 64;
            this.myTip.SetToolTip(this.udMaxJumpSpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udMaxJumpSpeed.Value = new decimal(new int[] {
            1144,
            0,
            0,
            65536});
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label44);
            this.groupBox4.Controls.Add(this.udBasePerception);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.udBaseRunSpeed);
            this.groupBox4.Controls.Add(this.udBaseToHit);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.Label14);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.Label13);
            this.groupBox4.Controls.Add(this.udBaseJumpHeight);
            this.groupBox4.Controls.Add(this.udBaseFlySpeed);
            this.groupBox4.Controls.Add(this.udBaseJumpSpeed);
            this.groupBox4.Location = new System.Drawing.Point(8, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(415, 227);
            this.groupBox4.TabIndex = 70;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Base Values";
            // 
            // label44
            // 
            this.label44.Location = new System.Drawing.Point(214, 84);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(99, 20);
            this.label44.TabIndex = 73;
            this.label44.Text = "Base Perception:";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBasePerception
            // 
            this.udBasePerception.DecimalPlaces = 1;
            this.udBasePerception.Location = new System.Drawing.Point(319, 84);
            this.udBasePerception.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udBasePerception.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBasePerception.Name = "udBasePerception";
            this.udBasePerception.Size = new System.Drawing.Size(88, 20);
            this.udBasePerception.TabIndex = 72;
            this.myTip.SetToolTip(this.udBasePerception, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBasePerception.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 20);
            this.label5.TabIndex = 63;
            this.label5.Text = "Base Jump Height:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 135);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 20);
            this.label12.TabIndex = 69;
            this.label12.Text = "Base Run Speed:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBaseRunSpeed
            // 
            this.udBaseRunSpeed.DecimalPlaces = 2;
            this.udBaseRunSpeed.Location = new System.Drawing.Point(111, 135);
            this.udBaseRunSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBaseRunSpeed.Name = "udBaseRunSpeed";
            this.udBaseRunSpeed.Size = new System.Drawing.Size(88, 20);
            this.udBaseRunSpeed.TabIndex = 68;
            this.myTip.SetToolTip(this.udBaseRunSpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBaseRunSpeed.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // udBaseToHit
            // 
            this.udBaseToHit.Location = new System.Drawing.Point(319, 111);
            this.udBaseToHit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBaseToHit.Name = "udBaseToHit";
            this.udBaseToHit.Size = new System.Drawing.Size(88, 20);
            this.udBaseToHit.TabIndex = 57;
            this.myTip.SetToolTip(this.udBaseToHit, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBaseToHit.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 20);
            this.label6.TabIndex = 65;
            this.label6.Text = "Base Fly Speed:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label14
            // 
            this.Label14.Location = new System.Drawing.Point(217, 110);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(96, 20);
            this.Label14.TabIndex = 58;
            this.Label14.Text = "Base ToHit:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 83);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 20);
            this.label11.TabIndex = 67;
            this.label11.Text = "Base Jump Speed:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label13
            // 
            this.Label13.Location = new System.Drawing.Point(6, 16);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(355, 38);
            this.Label13.TabIndex = 0;
            this.Label13.Text = "These values are used as the base for stats calculation. They shouldn\'t ever need" +
    " to be changed unless they differ on your server.";
            // 
            // udBaseJumpHeight
            // 
            this.udBaseJumpHeight.DecimalPlaces = 2;
            this.udBaseJumpHeight.Location = new System.Drawing.Point(111, 109);
            this.udBaseJumpHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBaseJumpHeight.Name = "udBaseJumpHeight";
            this.udBaseJumpHeight.Size = new System.Drawing.Size(88, 20);
            this.udBaseJumpHeight.TabIndex = 66;
            this.myTip.SetToolTip(this.udBaseJumpHeight, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBaseJumpHeight.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // udBaseFlySpeed
            // 
            this.udBaseFlySpeed.DecimalPlaces = 2;
            this.udBaseFlySpeed.Location = new System.Drawing.Point(111, 57);
            this.udBaseFlySpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBaseFlySpeed.Name = "udBaseFlySpeed";
            this.udBaseFlySpeed.Size = new System.Drawing.Size(88, 20);
            this.udBaseFlySpeed.TabIndex = 62;
            this.myTip.SetToolTip(this.udBaseFlySpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBaseFlySpeed.Value = new decimal(new int[] {
            315,
            0,
            0,
            65536});
            // 
            // udBaseJumpSpeed
            // 
            this.udBaseJumpSpeed.DecimalPlaces = 2;
            this.udBaseJumpSpeed.Location = new System.Drawing.Point(111, 83);
            this.udBaseJumpSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udBaseJumpSpeed.Name = "udBaseJumpSpeed";
            this.udBaseJumpSpeed.Size = new System.Drawing.Size(88, 20);
            this.udBaseJumpSpeed.TabIndex = 64;
            this.myTip.SetToolTip(this.udBaseJumpSpeed, "Chance of hitting a foe. Accuracy = (Base ToHit + Buffs) * Enhancements");
            this.udBaseJumpSpeed.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // chkColorPrint
            // 
            this.chkColorPrint.Location = new System.Drawing.Point(246, 387);
            this.chkColorPrint.Name = "chkColorPrint";
            this.chkColorPrint.Size = new System.Drawing.Size(156, 16);
            this.chkColorPrint.TabIndex = 2;
            this.chkColorPrint.Text = "Print in color";
            this.chkColorPrint.Visible = false;
            // 
            // myTip
            // 
            this.myTip.AutoPopDelay = 10000;
            this.myTip.InitialDelay = 500;
            this.myTip.ReshowDelay = 100;
            // 
            // cPicker
            // 
            this.cPicker.FullOpen = true;
            // 
            // frmCalcOpt
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(785, 412);
            this.Controls.Add(this.chkColorPrint);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCalcOpt";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.TabControl1.ResumeLayout(false);
            this.TabPage3.ResumeLayout(false);
            this.TabPage3.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.GroupBox17.ResumeLayout(false);
            this.GroupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPowersSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPowSelectSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStatSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRTFSize)).EndInit();
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox14.ResumeLayout(false);
            this.GroupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udIOLevel)).EndInit();
            this.GroupBox3.ResumeLayout(false);
            this.TabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TeamSize)).EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox15.ResumeLayout(false);
            this.GroupBox8.ResumeLayout(false);
            this.GroupBox6.ResumeLayout(false);
            this.TabPage4.ResumeLayout(false);
            this.GroupBox12.ResumeLayout(false);
            this.GroupBox12.PerformLayout();
            this.GroupBox11.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.GroupBox9.ResumeLayout(false);
            this.GroupBox7.ResumeLayout(false);
            this.TabPage5.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.TabPage6.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaFirst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthFirst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStaminaSlots)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udHealthSlots)).EndInit();
            this.groupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udMaxSlots)).EndInit();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udMaxRunSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxFlySpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxJumpSpeed)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udBasePerception)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseRunSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseToHit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseJumpHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseFlySpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseJumpSpeed)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        Button btnCancel;
        Button btnFontColor;
        Button btnIOReset;
        Button btnOK;
        Button btnSaveFolder;
        Button btnSaveFolderReset;
        ComboBox cbEnhLevel;
        CheckBox chkColorPrint;
        CheckBox chkHighVis;
        CheckBox chkIOEffects;
        CheckBox chkIOLevel;
        CheckBox chkIOPrintLevels;
        CheckBox chkLoadLastFile;
        CheckBox chkMiddle;
        CheckBox chkNoTips;
        CheckBox chkRelSignOnly;
        CheckBox chkSetBonus;
        CheckBox chkShowAlphaPopup;
        CheckBox chkStatBold;
        CheckBox chkTextBold;
        CheckBox chkUpdates;
        CheckBox chkUseArcanaTime;
        CheckedListBox clbSuppression;
        ComboBox cmbAction;
        ColorDialog cPicker;
        Button csAdd;
        Button csBtnEdit;
        Button csDelete;
        ListBox csList;
        Button csReset;
        FolderBrowserDialog fbdSave;
        Button fcAdd;
        TextBox fcBoldOff;
        TextBox fcBoldOn;
        TextBox fcColorOff;
        TextBox fcColorOn;
        Button fcDelete;
        TextBox fcItalicOff;
        TextBox fcItalicOn;
        ListBox fcList;
        TextBox fcName;
        TextBox fcNotes;
        Button fcReset;
        Button fcSet;
        TextBox fcTextOff;
        TextBox fcTextOn;
        TextBox fcUnderlineOff;
        TextBox fcUnderlineOn;
        RadioButton fcWSSpace;
        RadioButton fcWSTab;
        GroupBox GroupBox1;
        GroupBox GroupBox11;
        GroupBox GroupBox12;
        GroupBox GroupBox14;
        GroupBox GroupBox15;
        GroupBox GroupBox17;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        GroupBox GroupBox5;
        GroupBox GroupBox6;
        GroupBox GroupBox7;
        GroupBox GroupBox8;
        GroupBox GroupBox9;
        Label Label1;
        Label Label10;
        Label Label13;
        Label Label14;
        Label Label15;
        Label Label16;
        //System.Windows.Forms.Label Label2;
        Label Label20;
        Label Label21;
        Label Label22;
        Label Label24;
        Label Label25;
        Label Label26;
        Label Label27;
        Label Label28;
        Label Label29;
        Label Label3;
        Label Label30;
        Label Label31;
        Label Label32;
        Label Label33;
        Label Label34;
        Label Label36;
        Label Label4;
        Label Label40;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblExample;
        Label lblSaveFolder;
        ListBox listScenarios;
        ToolTip myTip;
        RadioButton optDO;
        Label optEnh;
        RadioButton optSO;
        RadioButton optTO;
        RadioButton rbChanceAverage;
        RadioButton rbChanceIgnore;
        RadioButton rbChanceMax;
        RadioButton rbGraphSimple;
        RadioButton rbGraphStacked;
        RadioButton rbGraphTwoLine;
        RadioButton rbPvE;
        RadioButton rbPvP;
        TabControl TabControl1;
        TabPage TabPage1;
        TabPage TabPage2;
        TabPage TabPage3;
        TabPage TabPage4;
        TabPage TabPage5;
        TabPage TabPage6;
        NumericUpDown TeamSize;
        //System.Windows.Forms.TextBox txtUpdatePath;
        NumericUpDown udBaseToHit;
        NumericUpDown udIOLevel;
        NumericUpDown udRTFSize;
        NumericUpDown udStatSize;
        private Label lblUpdateURL;
        private ComboBox cbUpdateURL;
        private CheckBox chkShowSOLevels;
        private CheckBox chkEnableDmgGraph;
        private Label lblFileAssoc;
        private Button btnFileAssoc;
        private Label lblAssocStatus;
        private GroupBox groupBox18;
        private ComboBox cbTotalsWindowTitleOpt;
        private Label label2;
        private Label label17;
        private NumericUpDown udPowSelectSize;
        private CheckBox chkPowersBold;
        private CheckBox chkPowSelBold;
        private NumericUpDown udPowersSize;
        private Label label18;
        private CheckBox chkOldStyle;
        private ComboBox cbDBUpdateURL;
        private Label label23;
        private GroupBox groupBox19;
        private Button btnDatabaseLoc;
        private Label label35;
        private GroupBox groupBox16;
        private Button btnResetDatabaseLoc;
        private TextBox lblDatabaseLoc;
        private Label Label37;
        private ComboBox cbCurrency;
        private Label label19;
        private CheckBox chkShowSelfBuffsAny;
        private NumericUpDown udBaseRunSpeed;
        private Label label12;
        private NumericUpDown udBaseJumpHeight;
        private Label label11;
        private NumericUpDown udBaseJumpSpeed;
        private Label label6;
        private NumericUpDown udBaseFlySpeed;
        private Label label5;
        private GroupBox groupBox4;
        private GroupBox groupBox10;
        private Label label45;
        private Label label39;
        private NumericUpDown udMaxRunSpeed;
        private Label label41;
        private Label label42;
        private NumericUpDown udMaxFlySpeed;
        private NumericUpDown udMaxJumpSpeed;
        private Label label44;
        private NumericUpDown udBasePerception;
        private GroupBox groupBox13;
        private Label label47;
        private NumericUpDown udMaxSlots;
        private Label label48;
        private GroupBox groupBox20;
        private NumericUpDown udStaminaSecond;
        private NumericUpDown udHealthSecond;
        private Label label52;
        private Label label51;
        private NumericUpDown udStaminaFirst;
        private NumericUpDown udHealthFirst;
        private Label label50;
        private Label label49;
        private NumericUpDown udStaminaSlots;
        private Label label46;
        private NumericUpDown udHealthSlots;
        private Label label43;
        private CheckBox chkEnableExtra;
        private Label label38;
    }
}
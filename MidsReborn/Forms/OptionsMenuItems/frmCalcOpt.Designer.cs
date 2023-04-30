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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmCalcOpt));
            btnOK = new Button();
            btnCancel = new Button();
            chkColorPrint = new CheckBox();
            myTip = new ToolTip(components);
            chkIOEffects = new CheckBox();
            chkSetBonus = new CheckBox();
            chkUseArcanaTime = new CheckBox();
            TeamSize = new NumericUpDown();
            cbEnhLevel = new ComboBox();
            optSO = new RadioButton();
            optDO = new RadioButton();
            optTO = new RadioButton();
            chkRelSignOnly = new CheckBox();
            udIOLevel = new NumericUpDown();
            chkIOLevel = new CheckBox();
            rbGraphTwoLine = new RadioButton();
            rbGraphStacked = new RadioButton();
            rbGraphSimple = new RadioButton();
            chkHighVis = new CheckBox();
            cPicker = new ColorDialog();
            fbdSave = new FolderBrowserDialog();
            TabPage5 = new TabPage();
            groupBox4 = new GroupBox();
            SchemaStatus = new Label();
            btnRepairSchemaAssoc = new Button();
            label6 = new Label();
            lblSchemaAssoc = new Label();
            groupBox16 = new GroupBox();
            Label1 = new Label();
            lblSaveFolder = new Label();
            chkLoadLastFile = new CheckBox();
            btnSaveFolderReset = new Button();
            btnSaveFolder = new Button();
            groupBox19 = new GroupBox();
            FileAssocStatus = new Label();
            btnRepairFileAssoc = new Button();
            lblFileAssocTxt = new Label();
            lblFileAssoc = new Label();
            GroupBox1 = new GroupBox();
            Label34 = new Label();
            chkUpdates = new CheckBox();
            TabPage1 = new TabPage();
            Label15 = new Label();
            Label10 = new Label();
            cmbAction = new ComboBox();
            GroupBox9 = new GroupBox();
            lblExample = new Label();
            GroupBox7 = new GroupBox();
            listScenarios = new ListBox();
            TabPage2 = new TabPage();
            Label16 = new Label();
            GroupBox2 = new GroupBox();
            clbSuppression = new CheckedListBox();
            Label8 = new Label();
            GroupBox15 = new GroupBox();
            Label20 = new Label();
            GroupBox8 = new GroupBox();
            rbChanceIgnore = new RadioButton();
            rbChanceAverage = new RadioButton();
            rbChanceMax = new RadioButton();
            Label9 = new Label();
            GroupBox6 = new GroupBox();
            Label7 = new Label();
            rbPvP = new RadioButton();
            rbPvE = new RadioButton();
            TabPage3 = new TabPage();
            chkShowSelfBuffsAny = new CheckBox();
            groupBox18 = new GroupBox();
            chkOldStyle = new CheckBox();
            cbTotalsWindowTitleOpt = new ComboBox();
            label2 = new Label();
            chkNoTips = new CheckBox();
            chkMiddle = new CheckBox();
            GroupBox17 = new GroupBox();
            chkPowersBold = new CheckBox();
            chkPowSelBold = new CheckBox();
            udPowersSize = new NumericUpDown();
            label18 = new Label();
            udPowSelectSize = new NumericUpDown();
            label17 = new Label();
            chkShowAlphaPopup = new CheckBox();
            Label36 = new Label();
            chkStatBold = new CheckBox();
            chkTextBold = new CheckBox();
            btnFontColor = new Button();
            Label22 = new Label();
            Label21 = new Label();
            udStatSize = new NumericUpDown();
            udRTFSize = new NumericUpDown();
            chkIOPrintLevels = new CheckBox();
            GroupBox5 = new GroupBox();
            chkEnableDmgGraph = new CheckBox();
            GroupBox14 = new GroupBox();
            cbCurrency = new ComboBox();
            label19 = new Label();
            btnIOReset = new Button();
            Label40 = new Label();
            GroupBox3 = new GroupBox();
            chkShowSOLevels = new CheckBox();
            Label3 = new Label();
            optEnh = new Label();
            Label4 = new Label();
            TabControl1 = new TabControl();
            ((ISupportInitialize)TeamSize).BeginInit();
            ((ISupportInitialize)udIOLevel).BeginInit();
            TabPage5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox16.SuspendLayout();
            groupBox19.SuspendLayout();
            GroupBox1.SuspendLayout();
            TabPage1.SuspendLayout();
            GroupBox9.SuspendLayout();
            GroupBox7.SuspendLayout();
            TabPage2.SuspendLayout();
            GroupBox2.SuspendLayout();
            GroupBox15.SuspendLayout();
            GroupBox8.SuspendLayout();
            GroupBox6.SuspendLayout();
            TabPage3.SuspendLayout();
            groupBox18.SuspendLayout();
            GroupBox17.SuspendLayout();
            ((ISupportInitialize)udPowersSize).BeginInit();
            ((ISupportInitialize)udPowSelectSize).BeginInit();
            ((ISupportInitialize)udStatSize).BeginInit();
            ((ISupportInitialize)udRTFSize).BeginInit();
            GroupBox5.SuspendLayout();
            GroupBox14.SuspendLayout();
            GroupBox3.SuspendLayout();
            TabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(504, 380);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 28);
            btnOK.TabIndex = 59;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(408, 380);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 28);
            btnCancel.TabIndex = 60;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // chkColorPrint
            // 
            chkColorPrint.Location = new System.Drawing.Point(246, 387);
            chkColorPrint.Name = "chkColorPrint";
            chkColorPrint.Size = new System.Drawing.Size(156, 16);
            chkColorPrint.TabIndex = 2;
            chkColorPrint.Text = "Print in color";
            chkColorPrint.Visible = false;
            // 
            // myTip
            // 
            myTip.AutoPopDelay = 10000;
            myTip.InitialDelay = 500;
            myTip.ReshowDelay = 100;
            // 
            // chkIOEffects
            // 
            chkIOEffects.Location = new System.Drawing.Point(11, 69);
            chkIOEffects.Name = "chkIOEffects";
            chkIOEffects.Size = new System.Drawing.Size(169, 22);
            chkIOEffects.TabIndex = 63;
            chkIOEffects.Text = "Include Enhancement effects";
            myTip.SetToolTip(chkIOEffects, "Some enhancments have power effects, such as minor Psionic resistance. This effect can be added into the power.");
            // 
            // chkSetBonus
            // 
            chkSetBonus.Location = new System.Drawing.Point(11, 93);
            chkSetBonus.Name = "chkSetBonus";
            chkSetBonus.Size = new System.Drawing.Size(169, 22);
            chkSetBonus.TabIndex = 64;
            chkSetBonus.Text = "Include Set Bonus effects\r\n";
            myTip.SetToolTip(chkSetBonus, "Add set bonus effects to the totals view.");
            // 
            // chkUseArcanaTime
            // 
            chkUseArcanaTime.Location = new System.Drawing.Point(8, 320);
            chkUseArcanaTime.Name = "chkUseArcanaTime";
            chkUseArcanaTime.Size = new System.Drawing.Size(204, 22);
            chkUseArcanaTime.TabIndex = 66;
            chkUseArcanaTime.Text = "Use ArcanaTime for Animation Times";
            myTip.SetToolTip(chkUseArcanaTime, "Displays all cast times in ArcanaTime, the real time all animations take due to server clock ticks.");
            // 
            // TeamSize
            // 
            TeamSize.Enabled = false;
            TeamSize.Location = new System.Drawing.Point(638, 320);
            TeamSize.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            TeamSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            TeamSize.Name = "TeamSize";
            TeamSize.Size = new System.Drawing.Size(88, 20);
            TeamSize.TabIndex = 70;
            myTip.SetToolTip(TeamSize, "Sets the number of players on your team. (Depreciated: Use TeamMembers from main window.)");
            TeamSize.Value = new decimal(new int[] { 1, 0, 0, 0 });
            TeamSize.Visible = false;
            // 
            // cbEnhLevel
            // 
            cbEnhLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEnhLevel.Items.AddRange(new object[] { "None (Enh. has no effect)", "-3 Levels", "-2 Levels", "-1 Level", "Even Level", "+1 Level", "+2 Levels", "+3 Levels" });
            cbEnhLevel.Location = new System.Drawing.Point(9, 224);
            cbEnhLevel.Name = "cbEnhLevel";
            cbEnhLevel.Size = new System.Drawing.Size(167, 22);
            cbEnhLevel.TabIndex = 53;
            cbEnhLevel.Tag = "";
            myTip.SetToolTip(cbEnhLevel, "This is the relative level of the enhancements in relation to your own. ");
            // 
            // optSO
            // 
            optSO.Appearance = Appearance.Button;
            optSO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            optSO.Checked = true;
            optSO.Location = new System.Drawing.Point(120, 74);
            optSO.Name = "optSO";
            optSO.Size = new System.Drawing.Size(44, 44);
            optSO.TabIndex = 50;
            optSO.TabStop = true;
            optSO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            myTip.SetToolTip(optSO, "Single Origin enhancements are the most powerful kind, and can be bought from level 22.");
            optSO.CheckedChanged += optSO_CheckedChanged;
            // 
            // optDO
            // 
            optDO.Appearance = Appearance.Button;
            optDO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            optDO.Location = new System.Drawing.Point(72, 74);
            optDO.Name = "optDO";
            optDO.Size = new System.Drawing.Size(44, 44);
            optDO.TabIndex = 49;
            optDO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            myTip.SetToolTip(optDO, "Dual Origin enhancements can be bought from level 12 onwards.");
            optDO.CheckedChanged += optDO_CheckedChanged;
            // 
            // optTO
            // 
            optTO.Appearance = Appearance.Button;
            optTO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            optTO.Location = new System.Drawing.Point(24, 74);
            optTO.Name = "optTO";
            optTO.Size = new System.Drawing.Size(44, 44);
            optTO.TabIndex = 48;
            optTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            myTip.SetToolTip(optTO, "Training enhancements are the weakest kind.");
            optTO.CheckedChanged += optTO_CheckedChanged;
            // 
            // chkRelSignOnly
            // 
            chkRelSignOnly.Location = new System.Drawing.Point(11, 278);
            chkRelSignOnly.Name = "chkRelSignOnly";
            chkRelSignOnly.Size = new System.Drawing.Size(167, 43);
            chkRelSignOnly.TabIndex = 69;
            chkRelSignOnly.Text = "Show signs only for relative levels ('++' rather than '+2')";
            myTip.SetToolTip(chkRelSignOnly, "Show signs only for relative levels ('++' rather than '+2')");
            // 
            // udIOLevel
            // 
            udIOLevel.Location = new System.Drawing.Point(108, 20);
            udIOLevel.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            udIOLevel.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            udIOLevel.Name = "udIOLevel";
            udIOLevel.Size = new System.Drawing.Size(72, 20);
            udIOLevel.TabIndex = 57;
            myTip.SetToolTip(udIOLevel, "Inventions will be placed at this level by default. You can override the default by typing a level number in the picker.");
            udIOLevel.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // chkIOLevel
            // 
            chkIOLevel.Location = new System.Drawing.Point(8, 40);
            chkIOLevel.Name = "chkIOLevel";
            chkIOLevel.Size = new System.Drawing.Size(172, 24);
            chkIOLevel.TabIndex = 60;
            chkIOLevel.Text = "Display IO Levels";
            myTip.SetToolTip(chkIOLevel, "Show the level of Inventions in the build view");
            // 
            // rbGraphTwoLine
            // 
            rbGraphTwoLine.Checked = true;
            rbGraphTwoLine.Location = new System.Drawing.Point(6, 45);
            rbGraphTwoLine.Name = "rbGraphTwoLine";
            rbGraphTwoLine.Size = new System.Drawing.Size(286, 23);
            rbGraphTwoLine.TabIndex = 3;
            rbGraphTwoLine.TabStop = true;
            rbGraphTwoLine.Text = "Base / Enhanced against Max Enhancable (Default)";
            myTip.SetToolTip(rbGraphTwoLine, "The blue bar will indicate base damage, the yellow bar will indicate enhanced damage.");
            // 
            // rbGraphStacked
            // 
            rbGraphStacked.Location = new System.Drawing.Point(6, 66);
            rbGraphStacked.Name = "rbGraphStacked";
            rbGraphStacked.Size = new System.Drawing.Size(286, 23);
            rbGraphStacked.TabIndex = 4;
            rbGraphStacked.Text = "Base + Enhanced (stacked) against Max Enhancable";
            myTip.SetToolTip(rbGraphStacked, "'Max Enhacable' is damage if slotted with 6 +3 damage enhancements.");
            // 
            // rbGraphSimple
            // 
            rbGraphSimple.Location = new System.Drawing.Point(6, 87);
            rbGraphSimple.Name = "rbGraphSimple";
            rbGraphSimple.Size = new System.Drawing.Size(164, 23);
            rbGraphSimple.TabIndex = 5;
            rbGraphSimple.Text = "Base against Enhanced";
            myTip.SetToolTip(rbGraphSimple, "This graph type doesn't reflect the max damage potential of other powers.");
            // 
            // chkHighVis
            // 
            chkHighVis.Location = new System.Drawing.Point(280, 106);
            chkHighVis.Name = "chkHighVis";
            chkHighVis.Size = new System.Drawing.Size(222, 20);
            chkHighVis.TabIndex = 69;
            chkHighVis.Text = "Use High-Visiblity text on the build view";
            myTip.SetToolTip(chkHighVis, "Draw white text with a black outline on the build view (power bars on the right of the screen)");
            // 
            // cPicker
            // 
            cPicker.FullOpen = true;
            // 
            // TabPage5
            // 
            TabPage5.Controls.Add(groupBox4);
            TabPage5.Controls.Add(groupBox16);
            TabPage5.Controls.Add(groupBox19);
            TabPage5.Controls.Add(GroupBox1);
            TabPage5.Location = new System.Drawing.Point(4, 23);
            TabPage5.Name = "TabPage5";
            TabPage5.Size = new System.Drawing.Size(777, 345);
            TabPage5.TabIndex = 4;
            TabPage5.Text = "Updates & Paths";
            TabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(SchemaStatus);
            groupBox4.Controls.Add(btnRepairSchemaAssoc);
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(lblSchemaAssoc);
            groupBox4.Location = new System.Drawing.Point(394, 76);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(375, 87);
            groupBox4.TabIndex = 73;
            groupBox4.TabStop = false;
            groupBox4.Text = "Schema Association (Build Sharing)";
            // 
            // SchemaStatus
            // 
            SchemaStatus.AutoSize = true;
            SchemaStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            SchemaStatus.ForeColor = System.Drawing.Color.Goldenrod;
            SchemaStatus.Location = new System.Drawing.Point(252, 54);
            SchemaStatus.Name = "SchemaStatus";
            SchemaStatus.Size = new System.Drawing.Size(69, 16);
            SchemaStatus.TabIndex = 71;
            SchemaStatus.Text = "WARNING";
            SchemaStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            SchemaStatus.MouseHover += Status_MouseHover;
            // 
            // btnRepairSchemaAssoc
            // 
            btnRepairSchemaAssoc.FlatStyle = FlatStyle.System;
            btnRepairSchemaAssoc.Location = new System.Drawing.Point(24, 45);
            btnRepairSchemaAssoc.Name = "btnRepairSchemaAssoc";
            btnRepairSchemaAssoc.Size = new System.Drawing.Size(141, 36);
            btnRepairSchemaAssoc.TabIndex = 68;
            btnRepairSchemaAssoc.Text = "Repair Schema";
            btnRepairSchemaAssoc.UseVisualStyleBackColor = true;
            btnRepairSchemaAssoc.Click += btnRepairSchemaAssoc_Click;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(5, 18);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(374, 17);
            label6.TabIndex = 69;
            label6.Text = "If your status is in the warning state you can repair it via the button below.\r\n";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSchemaAssoc
            // 
            lblSchemaAssoc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSchemaAssoc.Location = new System.Drawing.Point(196, 54);
            lblSchemaAssoc.Name = "lblSchemaAssoc";
            lblSchemaAssoc.Size = new System.Drawing.Size(50, 14);
            lblSchemaAssoc.TabIndex = 70;
            lblSchemaAssoc.Text = "Status: ";
            lblSchemaAssoc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox16
            // 
            groupBox16.Controls.Add(Label1);
            groupBox16.Controls.Add(lblSaveFolder);
            groupBox16.Controls.Add(chkLoadLastFile);
            groupBox16.Controls.Add(btnSaveFolderReset);
            groupBox16.Controls.Add(btnSaveFolder);
            groupBox16.Location = new System.Drawing.Point(8, 169);
            groupBox16.Name = "groupBox16";
            groupBox16.Size = new System.Drawing.Size(761, 120);
            groupBox16.TabIndex = 72;
            groupBox16.TabStop = false;
            groupBox16.Text = "Character Builds Location";
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(6, 30);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(85, 24);
            Label1.TabIndex = 8;
            Label1.Text = "Save Builds To:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSaveFolder
            // 
            lblSaveFolder.BorderStyle = BorderStyle.FixedSingle;
            lblSaveFolder.Location = new System.Drawing.Point(97, 31);
            lblSaveFolder.Name = "lblSaveFolder";
            lblSaveFolder.Size = new System.Drawing.Size(511, 22);
            lblSaveFolder.TabIndex = 63;
            lblSaveFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblSaveFolder.UseMnemonic = false;
            // 
            // chkLoadLastFile
            // 
            chkLoadLastFile.Location = new System.Drawing.Point(9, 72);
            chkLoadLastFile.Name = "chkLoadLastFile";
            chkLoadLastFile.Size = new System.Drawing.Size(156, 18);
            chkLoadLastFile.TabIndex = 61;
            chkLoadLastFile.Text = "Load last build on startup";
            // 
            // btnSaveFolderReset
            // 
            btnSaveFolderReset.Location = new System.Drawing.Point(614, 72);
            btnSaveFolderReset.Name = "btnSaveFolderReset";
            btnSaveFolderReset.Size = new System.Drawing.Size(105, 22);
            btnSaveFolderReset.TabIndex = 64;
            btnSaveFolderReset.Text = "Reset to Default";
            btnSaveFolderReset.UseVisualStyleBackColor = true;
            btnSaveFolderReset.Click += btnSaveFolderReset_Click;
            // 
            // btnSaveFolder
            // 
            btnSaveFolder.Location = new System.Drawing.Point(614, 30);
            btnSaveFolder.Name = "btnSaveFolder";
            btnSaveFolder.Size = new System.Drawing.Size(105, 22);
            btnSaveFolder.TabIndex = 62;
            btnSaveFolder.Text = "Browse...";
            btnSaveFolder.UseVisualStyleBackColor = true;
            btnSaveFolder.Click += btnSaveFolder_Click;
            // 
            // groupBox19
            // 
            groupBox19.Controls.Add(FileAssocStatus);
            groupBox19.Controls.Add(btnRepairFileAssoc);
            groupBox19.Controls.Add(lblFileAssocTxt);
            groupBox19.Controls.Add(lblFileAssoc);
            groupBox19.Location = new System.Drawing.Point(8, 76);
            groupBox19.Name = "groupBox19";
            groupBox19.Size = new System.Drawing.Size(380, 87);
            groupBox19.TabIndex = 71;
            groupBox19.TabStop = false;
            groupBox19.Text = "File Associations";
            // 
            // FileAssocStatus
            // 
            FileAssocStatus.AutoSize = true;
            FileAssocStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            FileAssocStatus.ForeColor = System.Drawing.Color.Goldenrod;
            FileAssocStatus.Location = new System.Drawing.Point(252, 54);
            FileAssocStatus.Name = "FileAssocStatus";
            FileAssocStatus.Size = new System.Drawing.Size(69, 16);
            FileAssocStatus.TabIndex = 71;
            FileAssocStatus.Text = "WARNING";
            FileAssocStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            FileAssocStatus.MouseHover += Status_MouseHover;
            // 
            // btnRepairFileAssoc
            // 
            btnRepairFileAssoc.FlatStyle = FlatStyle.System;
            btnRepairFileAssoc.Location = new System.Drawing.Point(24, 45);
            btnRepairFileAssoc.Name = "btnRepairFileAssoc";
            btnRepairFileAssoc.Size = new System.Drawing.Size(141, 36);
            btnRepairFileAssoc.TabIndex = 68;
            btnRepairFileAssoc.Text = "Repair Associations";
            btnRepairFileAssoc.UseVisualStyleBackColor = true;
            btnRepairFileAssoc.Click += btnRepairFileAssoc_Click;
            // 
            // lblFileAssocTxt
            // 
            lblFileAssocTxt.Location = new System.Drawing.Point(5, 18);
            lblFileAssocTxt.Name = "lblFileAssocTxt";
            lblFileAssocTxt.Size = new System.Drawing.Size(369, 17);
            lblFileAssocTxt.TabIndex = 69;
            lblFileAssocTxt.Text = "If your status is in the warning state you can repair it via the button below.";
            lblFileAssocTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFileAssoc
            // 
            lblFileAssoc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblFileAssoc.Location = new System.Drawing.Point(196, 54);
            lblFileAssoc.Name = "lblFileAssoc";
            lblFileAssoc.Size = new System.Drawing.Size(50, 14);
            lblFileAssoc.TabIndex = 70;
            lblFileAssoc.Text = "Status: ";
            lblFileAssoc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(Label34);
            GroupBox1.Controls.Add(chkUpdates);
            GroupBox1.Location = new System.Drawing.Point(8, 3);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(761, 67);
            GroupBox1.TabIndex = 7;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Automatic Updates";
            // 
            // Label34
            // 
            Label34.Location = new System.Drawing.Point(6, 16);
            Label34.Name = "Label34";
            Label34.Size = new System.Drawing.Size(486, 39);
            Label34.TabIndex = 5;
            Label34.Text = "Mids Reborn can automatically check for updates and download newer versions when it starts. This feature requires an internet connection.";
            Label34.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // chkUpdates
            // 
            chkUpdates.Location = new System.Drawing.Point(552, 19);
            chkUpdates.Name = "chkUpdates";
            chkUpdates.Size = new System.Drawing.Size(167, 24);
            chkUpdates.TabIndex = 6;
            chkUpdates.Text = "Check for updates on startup";
            // 
            // TabPage1
            // 
            TabPage1.Controls.Add(Label15);
            TabPage1.Controls.Add(Label10);
            TabPage1.Controls.Add(cmbAction);
            TabPage1.Controls.Add(GroupBox9);
            TabPage1.Controls.Add(GroupBox7);
            TabPage1.Location = new System.Drawing.Point(4, 24);
            TabPage1.Name = "TabPage1";
            TabPage1.Size = new System.Drawing.Size(777, 344);
            TabPage1.TabIndex = 6;
            TabPage1.Text = "Drag & Drop";
            TabPage1.UseVisualStyleBackColor = true;
            // 
            // Label15
            // 
            Label15.Location = new System.Drawing.Point(14, 9);
            Label15.Name = "Label15";
            Label15.Size = new System.Drawing.Size(755, 92);
            Label15.TabIndex = 4;
            Label15.Text = resources.GetString("Label15.Text");
            Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label10
            // 
            Label10.AutoSize = true;
            Label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Label10.Location = new System.Drawing.Point(19, 268);
            Label10.Name = "Label10";
            Label10.Size = new System.Drawing.Size(285, 15);
            Label10.TabIndex = 3;
            Label10.Text = "Action to take whenever the above scenario occurs:";
            // 
            // cmbAction
            // 
            cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAction.FormattingEnabled = true;
            cmbAction.Location = new System.Drawing.Point(18, 293);
            cmbAction.Name = "cmbAction";
            cmbAction.Size = new System.Drawing.Size(356, 22);
            cmbAction.TabIndex = 2;
            cmbAction.SelectedIndexChanged += cmbAction_SelectedIndexChanged;
            // 
            // GroupBox9
            // 
            GroupBox9.Controls.Add(lblExample);
            GroupBox9.Location = new System.Drawing.Point(403, 104);
            GroupBox9.Name = "GroupBox9";
            GroupBox9.Size = new System.Drawing.Size(366, 161);
            GroupBox9.TabIndex = 1;
            GroupBox9.TabStop = false;
            GroupBox9.Text = "Description / Example(s)";
            // 
            // lblExample
            // 
            lblExample.Location = new System.Drawing.Point(13, 17);
            lblExample.Name = "lblExample";
            lblExample.Size = new System.Drawing.Size(347, 141);
            lblExample.TabIndex = 0;
            // 
            // GroupBox7
            // 
            GroupBox7.Controls.Add(listScenarios);
            GroupBox7.Location = new System.Drawing.Point(8, 102);
            GroupBox7.Name = "GroupBox7";
            GroupBox7.Size = new System.Drawing.Size(380, 163);
            GroupBox7.TabIndex = 0;
            GroupBox7.TabStop = false;
            GroupBox7.Text = "Scenario";
            // 
            // listScenarios
            // 
            listScenarios.FormattingEnabled = true;
            listScenarios.ItemHeight = 14;
            listScenarios.Items.AddRange(new object[] { "Power is moved or swapped too low", "Power is moved too high (some powers will no longer fit)", "Power is moved or swapped higher than slots' levels", "Power is moved or swapped too high to have # slots", "Power being replaced is swapped too low", "Power being replaced is swapped higher than slots' levels", "Power being replaced is swapped too high to have # slots", "Power being shifted down cannot shift to the necessary level", "Power being shifted up has slots from lower levels", "Power being shifted up has impossible # of slots", "There is a gap in a group of powers that are being shifted", "A power placed at its minimum level is being shifted up", "The power in the destination slot is prevented from being shifted up", "Slot being level-swapped is too low for the destination power", "Slot being level-swapped is too low for the source power" });
            listScenarios.Location = new System.Drawing.Point(13, 19);
            listScenarios.Name = "listScenarios";
            listScenarios.Size = new System.Drawing.Size(353, 116);
            listScenarios.TabIndex = 0;
            listScenarios.SelectedIndexChanged += listScenarios_SelectedIndexChanged;
            // 
            // TabPage2
            // 
            TabPage2.Controls.Add(Label16);
            TabPage2.Controls.Add(TeamSize);
            TabPage2.Controls.Add(GroupBox2);
            TabPage2.Controls.Add(chkUseArcanaTime);
            TabPage2.Controls.Add(GroupBox15);
            TabPage2.Controls.Add(GroupBox8);
            TabPage2.Controls.Add(GroupBox6);
            TabPage2.Location = new System.Drawing.Point(4, 24);
            TabPage2.Name = "TabPage2";
            TabPage2.Size = new System.Drawing.Size(777, 344);
            TabPage2.TabIndex = 1;
            TabPage2.Text = "Effects & Maths";
            TabPage2.UseVisualStyleBackColor = true;
            // 
            // Label16
            // 
            Label16.Enabled = false;
            Label16.Location = new System.Drawing.Point(575, 322);
            Label16.Name = "Label16";
            Label16.Size = new System.Drawing.Size(57, 18);
            Label16.TabIndex = 66;
            Label16.Text = "Team Size";
            Label16.Visible = false;
            // 
            // GroupBox2
            // 
            GroupBox2.Controls.Add(clbSuppression);
            GroupBox2.Controls.Add(Label8);
            GroupBox2.Location = new System.Drawing.Point(547, 4);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(221, 310);
            GroupBox2.TabIndex = 69;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Suppression";
            // 
            // clbSuppression
            // 
            clbSuppression.CheckOnClick = true;
            clbSuppression.FormattingEnabled = true;
            clbSuppression.Location = new System.Drawing.Point(9, 75);
            clbSuppression.Name = "clbSuppression";
            clbSuppression.Size = new System.Drawing.Size(206, 229);
            clbSuppression.TabIndex = 9;
            clbSuppression.SelectedIndexChanged += clbSuppression_SelectedIndexChanged;
            // 
            // Label8
            // 
            Label8.Location = new System.Drawing.Point(6, 28);
            Label8.Name = "Label8";
            Label8.Size = new System.Drawing.Size(209, 44);
            Label8.TabIndex = 65;
            Label8.Text = "Some effects are suppressed on specific conditions. You can enable those conditions here.";
            // 
            // GroupBox15
            // 
            GroupBox15.Controls.Add(Label20);
            GroupBox15.Controls.Add(chkSetBonus);
            GroupBox15.Controls.Add(chkIOEffects);
            GroupBox15.Location = new System.Drawing.Point(315, 194);
            GroupBox15.Name = "GroupBox15";
            GroupBox15.Size = new System.Drawing.Size(226, 121);
            GroupBox15.TabIndex = 68;
            GroupBox15.TabStop = false;
            GroupBox15.Text = "Invention Effects:";
            // 
            // Label20
            // 
            Label20.Location = new System.Drawing.Point(6, 17);
            Label20.Name = "Label20";
            Label20.Size = new System.Drawing.Size(213, 49);
            Label20.TabIndex = 65;
            Label20.Text = "The effects of set bonusses and special IO enhancements can be included when stats are calculated.";
            // 
            // GroupBox8
            // 
            GroupBox8.Controls.Add(rbChanceIgnore);
            GroupBox8.Controls.Add(rbChanceAverage);
            GroupBox8.Controls.Add(rbChanceMax);
            GroupBox8.Controls.Add(Label9);
            GroupBox8.Location = new System.Drawing.Point(4, 4);
            GroupBox8.Name = "GroupBox8";
            GroupBox8.Size = new System.Drawing.Size(537, 182);
            GroupBox8.TabIndex = 3;
            GroupBox8.TabStop = false;
            GroupBox8.Text = "Chance of Damage:";
            // 
            // rbChanceIgnore
            // 
            rbChanceIgnore.Location = new System.Drawing.Point(347, 156);
            rbChanceIgnore.Name = "rbChanceIgnore";
            rbChanceIgnore.Size = new System.Drawing.Size(183, 20);
            rbChanceIgnore.TabIndex = 8;
            rbChanceIgnore.Text = "Minimum (Ignore Extra Damage)\r\n";
            // 
            // rbChanceAverage
            // 
            rbChanceAverage.Checked = true;
            rbChanceAverage.Location = new System.Drawing.Point(6, 156);
            rbChanceAverage.Name = "rbChanceAverage";
            rbChanceAverage.Size = new System.Drawing.Size(168, 20);
            rbChanceAverage.TabIndex = 7;
            rbChanceAverage.TabStop = true;
            rbChanceAverage.Text = "Average (Damage x Chance)";
            // 
            // rbChanceMax
            // 
            rbChanceMax.Location = new System.Drawing.Point(180, 156);
            rbChanceMax.Name = "rbChanceMax";
            rbChanceMax.Size = new System.Drawing.Size(161, 20);
            rbChanceMax.TabIndex = 6;
            rbChanceMax.Text = "Maximum (Always Include)";
            // 
            // Label9
            // 
            Label9.Location = new System.Drawing.Point(8, 16);
            Label9.Name = "Label9";
            Label9.Size = new System.Drawing.Size(522, 137);
            Label9.TabIndex = 4;
            Label9.Text = resources.GetString("Label9.Text");
            Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupBox6
            // 
            GroupBox6.Controls.Add(Label7);
            GroupBox6.Controls.Add(rbPvP);
            GroupBox6.Controls.Add(rbPvE);
            GroupBox6.Location = new System.Drawing.Point(8, 192);
            GroupBox6.Name = "GroupBox6";
            GroupBox6.Size = new System.Drawing.Size(301, 122);
            GroupBox6.TabIndex = 1;
            GroupBox6.TabStop = false;
            GroupBox6.Text = "Targets:";
            // 
            // Label7
            // 
            Label7.Location = new System.Drawing.Point(8, 16);
            Label7.Name = "Label7";
            Label7.Size = new System.Drawing.Size(295, 54);
            Label7.TabIndex = 2;
            Label7.Text = "Some powers  have different effects when used against players (Mez effects are a good example of this). Where a power has different effects, which should be displayed?";
            // 
            // rbPvP
            // 
            rbPvP.Location = new System.Drawing.Point(11, 93);
            rbPvP.Name = "rbPvP";
            rbPvP.Size = new System.Drawing.Size(260, 20);
            rbPvP.TabIndex = 1;
            rbPvP.Text = "Show values for Players (PvP)";
            // 
            // rbPvE
            // 
            rbPvE.Checked = true;
            rbPvE.Location = new System.Drawing.Point(11, 73);
            rbPvE.Name = "rbPvE";
            rbPvE.Size = new System.Drawing.Size(260, 20);
            rbPvE.TabIndex = 0;
            rbPvE.TabStop = true;
            rbPvE.Text = "Show values for Mobs (PvE)";
            // 
            // TabPage3
            // 
            TabPage3.Controls.Add(chkShowSelfBuffsAny);
            TabPage3.Controls.Add(groupBox18);
            TabPage3.Controls.Add(chkNoTips);
            TabPage3.Controls.Add(chkMiddle);
            TabPage3.Controls.Add(GroupBox17);
            TabPage3.Controls.Add(chkIOPrintLevels);
            TabPage3.Controls.Add(GroupBox5);
            TabPage3.Controls.Add(GroupBox14);
            TabPage3.Controls.Add(GroupBox3);
            TabPage3.Location = new System.Drawing.Point(4, 23);
            TabPage3.Name = "TabPage3";
            TabPage3.Size = new System.Drawing.Size(777, 345);
            TabPage3.TabIndex = 2;
            TabPage3.Text = "Enhancements & View";
            TabPage3.UseVisualStyleBackColor = true;
            // 
            // chkShowSelfBuffsAny
            // 
            chkShowSelfBuffsAny.Location = new System.Drawing.Point(194, 324);
            chkShowSelfBuffsAny.Name = "chkShowSelfBuffsAny";
            chkShowSelfBuffsAny.Size = new System.Drawing.Size(190, 18);
            chkShowSelfBuffsAny.TabIndex = 80;
            chkShowSelfBuffsAny.Text = "Show \"in PvE/PvP\" for self buffs";
            chkShowSelfBuffsAny.Visible = false;
            chkShowSelfBuffsAny.CheckedChanged += chkShowSelfBuffsAny_CheckedChanged;
            // 
            // groupBox18
            // 
            groupBox18.Controls.Add(chkOldStyle);
            groupBox18.Controls.Add(cbTotalsWindowTitleOpt);
            groupBox18.Controls.Add(label2);
            groupBox18.Location = new System.Drawing.Point(388, 120);
            groupBox18.Name = "groupBox18";
            groupBox18.Size = new System.Drawing.Size(353, 44);
            groupBox18.TabIndex = 79;
            groupBox18.TabStop = false;
            groupBox18.Text = "Totals Window:";
            // 
            // chkOldStyle
            // 
            chkOldStyle.Location = new System.Drawing.Point(11, 17);
            chkOldStyle.Name = "chkOldStyle";
            chkOldStyle.Size = new System.Drawing.Size(88, 18);
            chkOldStyle.TabIndex = 80;
            chkOldStyle.Text = "Use old style";
            chkOldStyle.CheckedChanged += chkOldStyle_CheckedChanged;
            // 
            // cbTotalsWindowTitleOpt
            // 
            cbTotalsWindowTitleOpt.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTotalsWindowTitleOpt.FormattingEnabled = true;
            cbTotalsWindowTitleOpt.Items.AddRange(new object[] { "Generic - Totals for Self", "Character name + Archetype + Powersets", "Build file name + Archetype + Powersets", "Character name + Build file name (fallback to generic if none)" });
            cbTotalsWindowTitleOpt.Location = new System.Drawing.Point(199, 13);
            cbTotalsWindowTitleOpt.Name = "cbTotalsWindowTitleOpt";
            cbTotalsWindowTitleOpt.Size = new System.Drawing.Size(148, 22);
            cbTotalsWindowTitleOpt.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(108, 18);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(85, 14);
            label2.TabIndex = 0;
            label2.Text = "Show in titlebar:";
            // 
            // chkNoTips
            // 
            chkNoTips.Location = new System.Drawing.Point(417, 304);
            chkNoTips.Name = "chkNoTips";
            chkNoTips.Size = new System.Drawing.Size(78, 18);
            chkNoTips.TabIndex = 78;
            chkNoTips.Text = "No Tooltips";
            chkNoTips.Visible = false;
            // 
            // chkMiddle
            // 
            chkMiddle.Location = new System.Drawing.Point(500, 304);
            chkMiddle.Name = "chkMiddle";
            chkMiddle.Size = new System.Drawing.Size(222, 18);
            chkMiddle.TabIndex = 77;
            chkMiddle.Text = "Middle-Click repeats last enhancement";
            // 
            // GroupBox17
            // 
            GroupBox17.Controls.Add(chkPowersBold);
            GroupBox17.Controls.Add(chkPowSelBold);
            GroupBox17.Controls.Add(udPowersSize);
            GroupBox17.Controls.Add(label18);
            GroupBox17.Controls.Add(udPowSelectSize);
            GroupBox17.Controls.Add(label17);
            GroupBox17.Controls.Add(chkShowAlphaPopup);
            GroupBox17.Controls.Add(chkHighVis);
            GroupBox17.Controls.Add(Label36);
            GroupBox17.Controls.Add(chkStatBold);
            GroupBox17.Controls.Add(chkTextBold);
            GroupBox17.Controls.Add(btnFontColor);
            GroupBox17.Controls.Add(Label22);
            GroupBox17.Controls.Add(Label21);
            GroupBox17.Controls.Add(udStatSize);
            GroupBox17.Controls.Add(udRTFSize);
            GroupBox17.Location = new System.Drawing.Point(196, 166);
            GroupBox17.Name = "GroupBox17";
            GroupBox17.Size = new System.Drawing.Size(545, 132);
            GroupBox17.TabIndex = 76;
            GroupBox17.TabStop = false;
            GroupBox17.Text = "Font Size/Colors:";
            // 
            // chkPowersBold
            // 
            chkPowersBold.AutoSize = true;
            chkPowersBold.Location = new System.Drawing.Point(173, 101);
            chkPowersBold.Name = "chkPowersBold";
            chkPowersBold.Size = new System.Drawing.Size(15, 14);
            chkPowersBold.TabIndex = 85;
            chkPowersBold.UseVisualStyleBackColor = true;
            // 
            // chkPowSelBold
            // 
            chkPowSelBold.AutoSize = true;
            chkPowSelBold.Location = new System.Drawing.Point(173, 76);
            chkPowSelBold.Name = "chkPowSelBold";
            chkPowSelBold.Size = new System.Drawing.Size(15, 14);
            chkPowSelBold.TabIndex = 84;
            chkPowSelBold.UseVisualStyleBackColor = true;
            // 
            // udPowersSize
            // 
            udPowersSize.DecimalPlaces = 2;
            udPowersSize.Location = new System.Drawing.Point(108, 98);
            udPowersSize.Maximum = new decimal(new int[] { 14, 0, 0, 0 });
            udPowersSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            udPowersSize.Name = "udPowersSize";
            udPowersSize.Size = new System.Drawing.Size(52, 20);
            udPowersSize.TabIndex = 83;
            udPowersSize.Value = new decimal(new int[] { 825, 0, 0, 131072 });
            // 
            // label18
            // 
            label18.Location = new System.Drawing.Point(29, 98);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(78, 20);
            label18.TabIndex = 82;
            label18.Text = "Powers:";
            label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udPowSelectSize
            // 
            udPowSelectSize.DecimalPlaces = 2;
            udPowSelectSize.Location = new System.Drawing.Point(108, 73);
            udPowSelectSize.Maximum = new decimal(new int[] { 14, 0, 0, 0 });
            udPowSelectSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            udPowSelectSize.Name = "udPowSelectSize";
            udPowSelectSize.Size = new System.Drawing.Size(52, 20);
            udPowSelectSize.TabIndex = 81;
            udPowSelectSize.Value = new decimal(new int[] { 825, 0, 0, 131072 });
            // 
            // label17
            // 
            label17.Location = new System.Drawing.Point(11, 73);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(96, 20);
            label17.TabIndex = 80;
            label17.Text = "Power Selections:";
            label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkShowAlphaPopup
            // 
            chkShowAlphaPopup.Location = new System.Drawing.Point(280, 82);
            chkShowAlphaPopup.Name = "chkShowAlphaPopup";
            chkShowAlphaPopup.Size = new System.Drawing.Size(190, 18);
            chkShowAlphaPopup.TabIndex = 79;
            chkShowAlphaPopup.Text = "Include Alpha buffs in popups";
            // 
            // Label36
            // 
            Label36.Location = new System.Drawing.Point(161, 8);
            Label36.Name = "Label36";
            Label36.Size = new System.Drawing.Size(39, 16);
            Label36.TabIndex = 64;
            Label36.Text = "Bold";
            Label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkStatBold
            // 
            chkStatBold.AutoSize = true;
            chkStatBold.Location = new System.Drawing.Point(173, 50);
            chkStatBold.Name = "chkStatBold";
            chkStatBold.Size = new System.Drawing.Size(15, 14);
            chkStatBold.TabIndex = 63;
            chkStatBold.UseVisualStyleBackColor = true;
            // 
            // chkTextBold
            // 
            chkTextBold.AutoSize = true;
            chkTextBold.Location = new System.Drawing.Point(173, 25);
            chkTextBold.Name = "chkTextBold";
            chkTextBold.Size = new System.Drawing.Size(15, 14);
            chkTextBold.TabIndex = 62;
            chkTextBold.UseVisualStyleBackColor = true;
            // 
            // btnFontColor
            // 
            btnFontColor.Location = new System.Drawing.Point(280, 32);
            btnFontColor.Name = "btnFontColor";
            btnFontColor.Size = new System.Drawing.Size(172, 27);
            btnFontColor.TabIndex = 61;
            btnFontColor.Text = "Set Colors...";
            btnFontColor.UseVisualStyleBackColor = true;
            btnFontColor.Click += btnFontColor_Click;
            // 
            // Label22
            // 
            Label22.Location = new System.Drawing.Point(16, 47);
            Label22.Name = "Label22";
            Label22.Size = new System.Drawing.Size(91, 20);
            Label22.TabIndex = 60;
            Label22.Text = "Stats:";
            Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label21
            // 
            Label21.Location = new System.Drawing.Point(5, 21);
            Label21.Name = "Label21";
            Label21.Size = new System.Drawing.Size(102, 20);
            Label21.TabIndex = 59;
            Label21.Text = "InfoPanel Tab Text:";
            Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udStatSize
            // 
            udStatSize.DecimalPlaces = 2;
            udStatSize.Location = new System.Drawing.Point(108, 47);
            udStatSize.Maximum = new decimal(new int[] { 14, 0, 0, 0 });
            udStatSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            udStatSize.Name = "udStatSize";
            udStatSize.Size = new System.Drawing.Size(52, 20);
            udStatSize.TabIndex = 1;
            udStatSize.Value = new decimal(new int[] { 825, 0, 0, 131072 });
            // 
            // udRTFSize
            // 
            udRTFSize.Location = new System.Drawing.Point(108, 21);
            udRTFSize.Maximum = new decimal(new int[] { 14, 0, 0, 0 });
            udRTFSize.Minimum = new decimal(new int[] { 6, 0, 0, 0 });
            udRTFSize.Name = "udRTFSize";
            udRTFSize.Size = new System.Drawing.Size(52, 20);
            udRTFSize.TabIndex = 0;
            udRTFSize.Value = new decimal(new int[] { 825, 0, 0, 131072 });
            // 
            // chkIOPrintLevels
            // 
            chkIOPrintLevels.Location = new System.Drawing.Point(194, 304);
            chkIOPrintLevels.Name = "chkIOPrintLevels";
            chkIOPrintLevels.Size = new System.Drawing.Size(221, 18);
            chkIOPrintLevels.TabIndex = 75;
            chkIOPrintLevels.Text = "Display Invention levels in printed builds";
            // 
            // GroupBox5
            // 
            GroupBox5.Controls.Add(chkEnableDmgGraph);
            GroupBox5.Controls.Add(rbGraphSimple);
            GroupBox5.Controls.Add(rbGraphStacked);
            GroupBox5.Controls.Add(rbGraphTwoLine);
            GroupBox5.Location = new System.Drawing.Point(388, 4);
            GroupBox5.Name = "GroupBox5";
            GroupBox5.Size = new System.Drawing.Size(353, 117);
            GroupBox5.TabIndex = 72;
            GroupBox5.TabStop = false;
            GroupBox5.Text = "Damage Graph Style:";
            // 
            // chkEnableDmgGraph
            // 
            chkEnableDmgGraph.AutoSize = true;
            chkEnableDmgGraph.Location = new System.Drawing.Point(6, 21);
            chkEnableDmgGraph.Name = "chkEnableDmgGraph";
            chkEnableDmgGraph.Size = new System.Drawing.Size(133, 18);
            chkEnableDmgGraph.TabIndex = 6;
            chkEnableDmgGraph.Text = "Enable Damage Graph";
            chkEnableDmgGraph.UseVisualStyleBackColor = true;
            chkEnableDmgGraph.CheckedChanged += chkEnableDmgGraph_CheckedChanged;
            // 
            // GroupBox14
            // 
            GroupBox14.Controls.Add(cbCurrency);
            GroupBox14.Controls.Add(label19);
            GroupBox14.Controls.Add(chkIOLevel);
            GroupBox14.Controls.Add(btnIOReset);
            GroupBox14.Controls.Add(Label40);
            GroupBox14.Controls.Add(udIOLevel);
            GroupBox14.Location = new System.Drawing.Point(196, 4);
            GroupBox14.Name = "GroupBox14";
            GroupBox14.Size = new System.Drawing.Size(188, 160);
            GroupBox14.TabIndex = 69;
            GroupBox14.TabStop = false;
            GroupBox14.Text = "Inventions:";
            // 
            // cbCurrency
            // 
            cbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCurrency.FormattingEnabled = true;
            cbCurrency.Location = new System.Drawing.Point(9, 81);
            cbCurrency.Name = "cbCurrency";
            cbCurrency.Size = new System.Drawing.Size(171, 22);
            cbCurrency.TabIndex = 62;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(5, 64);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(103, 14);
            label19.TabIndex = 61;
            label19.Text = "Preferred currency:";
            // 
            // btnIOReset
            // 
            btnIOReset.Location = new System.Drawing.Point(8, 109);
            btnIOReset.Name = "btnIOReset";
            btnIOReset.Size = new System.Drawing.Size(172, 44);
            btnIOReset.TabIndex = 59;
            btnIOReset.Text = "Set All IO and SetO levels to default";
            btnIOReset.Click += btnIOReset_Click;
            // 
            // Label40
            // 
            Label40.Location = new System.Drawing.Point(8, 20);
            Label40.Name = "Label40";
            Label40.Size = new System.Drawing.Size(96, 20);
            Label40.TabIndex = 58;
            Label40.Text = "Default IO Level:";
            Label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox3
            // 
            GroupBox3.Controls.Add(chkShowSOLevels);
            GroupBox3.Controls.Add(chkRelSignOnly);
            GroupBox3.Controls.Add(Label3);
            GroupBox3.Controls.Add(optTO);
            GroupBox3.Controls.Add(optDO);
            GroupBox3.Controls.Add(optSO);
            GroupBox3.Controls.Add(optEnh);
            GroupBox3.Controls.Add(cbEnhLevel);
            GroupBox3.Controls.Add(Label4);
            GroupBox3.Location = new System.Drawing.Point(4, 4);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new System.Drawing.Size(184, 338);
            GroupBox3.TabIndex = 62;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "Regular Enhancements:";
            // 
            // chkShowSOLevels
            // 
            chkShowSOLevels.Location = new System.Drawing.Point(11, 257);
            chkShowSOLevels.Name = "chkShowSOLevels";
            chkShowSOLevels.Size = new System.Drawing.Size(150, 23);
            chkShowSOLevels.TabIndex = 71;
            chkShowSOLevels.Text = "Display SO/HO Levels";
            chkShowSOLevels.UseVisualStyleBackColor = true;
            chkShowSOLevels.CheckedChanged += chkShowSOLevels_CheckedChanged;
            // 
            // Label3
            // 
            Label3.Location = new System.Drawing.Point(6, 142);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(172, 79);
            Label3.TabIndex = 59;
            Label3.Text = "Default Relative Level:\r\n(Ehancements can function up to five levels above or three below that of the character.)";
            Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optEnh
            // 
            optEnh.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            optEnh.ForeColor = System.Drawing.SystemColors.ControlText;
            optEnh.Location = new System.Drawing.Point(21, 121);
            optEnh.Name = "optEnh";
            optEnh.Size = new System.Drawing.Size(143, 16);
            optEnh.TabIndex = 52;
            optEnh.Text = "Single Origin";
            optEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            Label4.Location = new System.Drawing.Point(6, 18);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(172, 50);
            Label4.TabIndex = 58;
            Label4.Text = "Default Enhancement Type:\r\n(This does not affect Inventions or Special enhancements)";
            Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TabControl1
            // 
            TabControl1.Controls.Add(TabPage3);
            TabControl1.Controls.Add(TabPage2);
            TabControl1.Controls.Add(TabPage1);
            TabControl1.Controls.Add(TabPage5);
            TabControl1.Location = new System.Drawing.Point(0, 0);
            TabControl1.Name = "TabControl1";
            TabControl1.SelectedIndex = 0;
            TabControl1.Size = new System.Drawing.Size(785, 372);
            TabControl1.TabIndex = 0;
            TabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            // 
            // frmCalcOpt
            // 
            AcceptButton = btnOK;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.SystemColors.Control;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(785, 412);
            Controls.Add(chkColorPrint);
            Controls.Add(TabControl1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            ForeColor = System.Drawing.SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCalcOpt";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Options";
            ((ISupportInitialize)TeamSize).EndInit();
            ((ISupportInitialize)udIOLevel).EndInit();
            TabPage5.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox16.ResumeLayout(false);
            groupBox19.ResumeLayout(false);
            groupBox19.PerformLayout();
            GroupBox1.ResumeLayout(false);
            TabPage1.ResumeLayout(false);
            TabPage1.PerformLayout();
            GroupBox9.ResumeLayout(false);
            GroupBox7.ResumeLayout(false);
            TabPage2.ResumeLayout(false);
            GroupBox2.ResumeLayout(false);
            GroupBox15.ResumeLayout(false);
            GroupBox8.ResumeLayout(false);
            GroupBox6.ResumeLayout(false);
            TabPage3.ResumeLayout(false);
            groupBox18.ResumeLayout(false);
            groupBox18.PerformLayout();
            GroupBox17.ResumeLayout(false);
            GroupBox17.PerformLayout();
            ((ISupportInitialize)udPowersSize).EndInit();
            ((ISupportInitialize)udPowSelectSize).EndInit();
            ((ISupportInitialize)udStatSize).EndInit();
            ((ISupportInitialize)udRTFSize).EndInit();
            GroupBox5.ResumeLayout(false);
            GroupBox5.PerformLayout();
            GroupBox14.ResumeLayout(false);
            GroupBox14.PerformLayout();
            GroupBox3.ResumeLayout(false);
            TabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        Button btnCancel;
        Button btnOK;
        CheckBox chkColorPrint;
        ColorDialog cPicker;
        FolderBrowserDialog fbdSave;
        ToolTip myTip;
        private TabPage TabPage5;
        private GroupBox groupBox16;
        private Label Label1;
        private Label lblSaveFolder;
        private CheckBox chkLoadLastFile;
        private Button btnSaveFolderReset;
        private Button btnSaveFolder;
        private GroupBox groupBox19;
        private Button btnRepairFileAssoc;
        private Label lblFileAssocTxt;
        private Label lblFileAssoc;
        private GroupBox GroupBox1;
        private Label Label34;
        private CheckBox chkUpdates;
        private TabPage TabPage1;
        private Label Label15;
        private Label Label10;
        private ComboBox cmbAction;
        private GroupBox GroupBox9;
        private Label lblExample;
        private GroupBox GroupBox7;
        private ListBox listScenarios;
        private TabPage TabPage2;
        private Label Label16;
        private NumericUpDown TeamSize;
        private GroupBox GroupBox2;
        private CheckedListBox clbSuppression;
        private Label Label8;
        private CheckBox chkUseArcanaTime;
        private GroupBox GroupBox15;
        private Label Label20;
        private CheckBox chkSetBonus;
        private CheckBox chkIOEffects;
        private GroupBox GroupBox8;
        private RadioButton rbChanceIgnore;
        private RadioButton rbChanceAverage;
        private RadioButton rbChanceMax;
        private Label Label9;
        private GroupBox GroupBox6;
        private Label Label7;
        private RadioButton rbPvP;
        private RadioButton rbPvE;
        private TabPage TabPage3;
        private CheckBox chkShowSelfBuffsAny;
        private GroupBox groupBox18;
        private CheckBox chkOldStyle;
        private ComboBox cbTotalsWindowTitleOpt;
        private Label label2;
        private CheckBox chkNoTips;
        private CheckBox chkMiddle;
        private GroupBox GroupBox17;
        private CheckBox chkPowersBold;
        private CheckBox chkPowSelBold;
        private NumericUpDown udPowersSize;
        private Label label18;
        private NumericUpDown udPowSelectSize;
        private Label label17;
        private CheckBox chkShowAlphaPopup;
        private CheckBox chkHighVis;
        private Label Label36;
        private CheckBox chkStatBold;
        private CheckBox chkTextBold;
        private Button btnFontColor;
        private Label Label22;
        private Label Label21;
        private NumericUpDown udStatSize;
        private NumericUpDown udRTFSize;
        private CheckBox chkIOPrintLevels;
        private GroupBox GroupBox5;
        private CheckBox chkEnableDmgGraph;
        private RadioButton rbGraphSimple;
        private RadioButton rbGraphStacked;
        private RadioButton rbGraphTwoLine;
        private GroupBox GroupBox14;
        private ComboBox cbCurrency;
        private Label label19;
        private CheckBox chkIOLevel;
        private Button btnIOReset;
        private Label Label40;
        private NumericUpDown udIOLevel;
        private GroupBox GroupBox3;
        private CheckBox chkShowSOLevels;
        private CheckBox chkRelSignOnly;
        private Label Label3;
        private RadioButton optTO;
        private RadioButton optDO;
        private RadioButton optSO;
        private Label optEnh;
        private ComboBox cbEnhLevel;
        private Label Label4;
        private TabControl TabControl1;
        private GroupBox groupBox4;
        private Label SchemaStatus;
        private Button btnRepairSchemaAssoc;
        private Label label6;
        private Label lblSchemaAssoc;
        private Label FileAssocStatus;
    }
}
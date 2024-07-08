using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditPower
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
            tcPower = new TabControl();
            tpText = new TabPage();
            chkNoAUReq = new CheckBox();
            chkNoAutoUpdate = new CheckBox();
            GroupBox4 = new GroupBox();
            lblInherentType = new Label();
            cbInherentType = new ComboBox();
            chkSortOverride = new CheckBox();
            chkSubInclude = new CheckBox();
            chkAlwaysToggle = new CheckBox();
            chkBuffCycle = new CheckBox();
            chkGraphFix = new CheckBox();
            Label21 = new Label();
            txtVisualLocation = new TextBox();
            GroupBox7 = new GroupBox();
            cbForcedClass = new ComboBox();
            Label29 = new Label();
            GroupBox6 = new GroupBox();
            chkSummonDisplayEntity = new CheckBox();
            chkSummonStealAttributes = new CheckBox();
            chkSummonStealEffects = new CheckBox();
            GroupBox5 = new GroupBox();
            txtDescLong = new TextBox();
            txtDescShort = new TextBox();
            Label2 = new Label();
            Label3 = new Label();
            GroupBox3 = new GroupBox();
            udScaleStart = new NumericUpDown();
            label19 = new Label();
            overideScale = new CheckBox();
            Label28 = new Label();
            udScaleMax = new NumericUpDown();
            Label27 = new Label();
            udScaleMin = new NumericUpDown();
            Label26 = new Label();
            txtScaleName = new TextBox();
            chkScale = new CheckBox();
            GroupBox1 = new GroupBox();
            lblStaticIndex = new Label();
            chkHidden = new CheckBox();
            lblNameUnique = new Label();
            Label1 = new Label();
            lblNameFull = new Label();
            cbNameSet = new ComboBox();
            txtNameDisplay = new TextBox();
            cbNameGroup = new ComboBox();
            Label22 = new Label();
            Label31 = new Label();
            txtNamePower = new TextBox();
            Label33 = new Label();
            tpBasic = new TabPage();
            Label46 = new Label();
            Label47 = new Label();
            txtLifeTimeReal = new TextBox();
            Label44 = new Label();
            Label45 = new Label();
            txtLifeTimeGame = new TextBox();
            Label42 = new Label();
            Label43 = new Label();
            txtUseageTime = new TextBox();
            Label41 = new Label();
            txtNumCharges = new TextBox();
            chkIgnoreStrength = new CheckBox();
            Label12 = new Label();
            Label17 = new Label();
            txtRangeSec = new TextBox();
            Label18 = new Label();
            GroupBox9 = new GroupBox();
            rbFlagCastThrough = new RadioButton();
            rbFlagDisallow = new RadioButton();
            rbFlagRequired = new RadioButton();
            rbFlagVector = new RadioButton();
            rbFlagCast = new RadioButton();
            clbFlags = new CheckedListBox();
            rbFlagTargetsSec = new RadioButton();
            rbFlagTargets = new RadioButton();
            rbFlagAffected = new RadioButton();
            rbFlagAutoHit = new RadioButton();
            cbNotify = new ComboBox();
            chkLos = new CheckBox();
            txtMaxTargets = new TextBox();
            lblEndCost = new Label();
            Label20 = new Label();
            lblAcc = new Label();
            Label40 = new Label();
            Label39 = new Label();
            Label38 = new Label();
            Label37 = new Label();
            Label36 = new Label();
            Label35 = new Label();
            Label34 = new Label();
            Label16 = new Label();
            txtArc = new TextBox();
            Label15 = new Label();
            txtRadius = new TextBox();
            txtLevel = new TextBox();
            cbEffectArea = new ComboBox();
            Label14 = new Label();
            Label13 = new Label();
            txtEndCost = new TextBox();
            Label10 = new Label();
            txtActivate = new TextBox();
            Label11 = new Label();
            txtRechargeTime = new TextBox();
            Label8 = new Label();
            txtCastTime = new TextBox();
            Label9 = new Label();
            txtInterrupt = new TextBox();
            Label7 = new Label();
            txtRange = new TextBox();
            Label6 = new Label();
            txtAcc = new TextBox();
            cbPowerType = new ComboBox();
            Label5 = new Label();
            Label4 = new Label();
            tpEffects = new TabPage();
            lvFX = new ListBox();
            pnlFX = new Panel();
            cbCoDFormat = new CheckBox();
            btnSetDamage = new Button();
            btnFXEdit = new Button();
            btnFXDown = new Button();
            btnFXUp = new Button();
            btnFXRemove = new Button();
            btnFXDuplicate = new Button();
            btnFXAdd = new Button();
            tpEnh = new TabPage();
            chkBoostUsePlayerLevel = new CheckBox();
            chkBoostBoostable = new CheckBox();
            Label23 = new Label();
            pbEnhancements = new PictureBox();
            chkPRFrontLoad = new CheckBox();
            pbEnhancementList = new PictureBox();
            lblEnhName = new Label();
            tpSets = new TabPage();
            Label24 = new Label();
            lblInvSet = new Label();
            pbInvSetList = new PictureBox();
            pbInvSetUsed = new PictureBox();
            tpPreReq = new TabPage();
            GroupBox11 = new GroupBox();
            btnPrReset = new Button();
            btnPrSetNone = new Button();
            btnPrDown = new Button();
            btnPrUp = new Button();
            rbPrRemove = new Button();
            rbPrAdd = new Button();
            rbPrPowerB = new RadioButton();
            rbPrPowerA = new RadioButton();
            lvPrPower = new ListView();
            ColumnHeader9 = new ColumnHeader();
            lvPrSet = new ListView();
            ColumnHeader10 = new ColumnHeader();
            lvPrGroup = new ListView();
            ColumnHeader11 = new ColumnHeader();
            lvPrListing = new ListView();
            ColumnHeader6 = new ColumnHeader();
            ColumnHeader7 = new ColumnHeader();
            ColumnHeader8 = new ColumnHeader();
            GroupBox10 = new GroupBox();
            clbClassExclude = new CheckedListBox();
            GroupBox8 = new GroupBox();
            clbClassReq = new CheckedListBox();
            tpSpecialEnh = new TabPage();
            Label32 = new Label();
            Label30 = new Label();
            lvDisablePass4 = new ListBox();
            lvDisablePass1 = new ListBox();
            tpMutex = new TabPage();
            GroupBox2 = new GroupBox();
            btnMutexAdd = new Button();
            clbMutex = new CheckedListBox();
            chkMutexAuto = new CheckBox();
            chkMutexSkip = new CheckBox();
            tpSubPower = new TabPage();
            btnSPAdd = new Button();
            btnSPRemove = new Button();
            lvSPSelected = new ListView();
            ColumnHeader4 = new ColumnHeader();
            lvSPPower = new ListView();
            ColumnHeader1 = new ColumnHeader();
            lvSPSet = new ListView();
            ColumnHeader2 = new ColumnHeader();
            lvSPGroup = new ListView();
            ColumnHeader3 = new ColumnHeader();
            btnOK = new Button();
            btnCancel = new Button();
            btnFullPaste = new Button();
            btnFullCopy = new Button();
            btnJsonExport = new Button();
            btnJsonImport = new Button();
            tcPower.SuspendLayout();
            tpText.SuspendLayout();
            GroupBox4.SuspendLayout();
            GroupBox7.SuspendLayout();
            GroupBox6.SuspendLayout();
            GroupBox5.SuspendLayout();
            GroupBox3.SuspendLayout();
            ((ISupportInitialize)udScaleStart).BeginInit();
            ((ISupportInitialize)udScaleMax).BeginInit();
            ((ISupportInitialize)udScaleMin).BeginInit();
            GroupBox1.SuspendLayout();
            tpBasic.SuspendLayout();
            GroupBox9.SuspendLayout();
            tpEffects.SuspendLayout();
            pnlFX.SuspendLayout();
            tpEnh.SuspendLayout();
            ((ISupportInitialize)pbEnhancements).BeginInit();
            ((ISupportInitialize)pbEnhancementList).BeginInit();
            tpSets.SuspendLayout();
            ((ISupportInitialize)pbInvSetList).BeginInit();
            ((ISupportInitialize)pbInvSetUsed).BeginInit();
            tpPreReq.SuspendLayout();
            GroupBox11.SuspendLayout();
            GroupBox10.SuspendLayout();
            GroupBox8.SuspendLayout();
            tpSpecialEnh.SuspendLayout();
            tpMutex.SuspendLayout();
            GroupBox2.SuspendLayout();
            tpSubPower.SuspendLayout();
            SuspendLayout();
            // 
            // tcPower
            // 
            tcPower.Controls.Add(tpText);
            tcPower.Controls.Add(tpBasic);
            tcPower.Controls.Add(tpEffects);
            tcPower.Controls.Add(tpEnh);
            tcPower.Controls.Add(tpSets);
            tcPower.Controls.Add(tpPreReq);
            tcPower.Controls.Add(tpSpecialEnh);
            tcPower.Controls.Add(tpMutex);
            tcPower.Controls.Add(tpSubPower);
            tcPower.Location = new System.Drawing.Point(8, 9);
            tcPower.Name = "tcPower";
            tcPower.SelectedIndex = 0;
            tcPower.Size = new System.Drawing.Size(840, 439);
            tcPower.TabIndex = 2;
            // 
            // tpText
            // 
            tpText.Controls.Add(chkNoAUReq);
            tpText.Controls.Add(chkNoAutoUpdate);
            tpText.Controls.Add(GroupBox4);
            tpText.Controls.Add(GroupBox7);
            tpText.Controls.Add(GroupBox6);
            tpText.Controls.Add(GroupBox5);
            tpText.Controls.Add(GroupBox3);
            tpText.Controls.Add(GroupBox1);
            tpText.Location = new System.Drawing.Point(4, 22);
            tpText.Name = "tpText";
            tpText.Size = new System.Drawing.Size(832, 413);
            tpText.TabIndex = 2;
            tpText.Text = "Basic";
            tpText.UseVisualStyleBackColor = true;
            // 
            // chkNoAUReq
            // 
            chkNoAUReq.Location = new System.Drawing.Point(332, 338);
            chkNoAUReq.Name = "chkNoAUReq";
            chkNoAUReq.Size = new System.Drawing.Size(269, 38);
            chkNoAUReq.TabIndex = 46;
            chkNoAUReq.Text = "Do not automatically update this power's requirements";
            chkNoAUReq.UseVisualStyleBackColor = true;
            chkNoAUReq.CheckedChanged += chkNoAUReq_CheckedChanged;
            // 
            // chkNoAutoUpdate
            // 
            chkNoAutoUpdate.Location = new System.Drawing.Point(13, 353);
            chkNoAutoUpdate.Name = "chkNoAutoUpdate";
            chkNoAutoUpdate.Size = new System.Drawing.Size(313, 38);
            chkNoAutoUpdate.TabIndex = 23;
            chkNoAutoUpdate.Text = "Do not automatically update this power during bulk-imports";
            chkNoAutoUpdate.UseVisualStyleBackColor = true;
            chkNoAutoUpdate.CheckedChanged += chkNoAutoUpdate_CheckedChanged;
            // 
            // GroupBox4
            // 
            GroupBox4.Controls.Add(lblInherentType);
            GroupBox4.Controls.Add(cbInherentType);
            GroupBox4.Controls.Add(chkSortOverride);
            GroupBox4.Controls.Add(chkSubInclude);
            GroupBox4.Controls.Add(chkAlwaysToggle);
            GroupBox4.Controls.Add(chkBuffCycle);
            GroupBox4.Controls.Add(chkGraphFix);
            GroupBox4.Controls.Add(Label21);
            GroupBox4.Controls.Add(txtVisualLocation);
            GroupBox4.Location = new System.Drawing.Point(607, 133);
            GroupBox4.Name = "GroupBox4";
            GroupBox4.Size = new System.Drawing.Size(214, 243);
            GroupBox4.TabIndex = 45;
            GroupBox4.TabStop = false;
            GroupBox4.Text = "MxD Special Flags";
            // 
            // lblInherentType
            // 
            lblInherentType.AutoSize = true;
            lblInherentType.Location = new System.Drawing.Point(6, 174);
            lblInherentType.Name = "lblInherentType";
            lblInherentType.Size = new System.Drawing.Size(80, 13);
            lblInherentType.TabIndex = 44;
            lblInherentType.Text = "Inherent Type:";
            // 
            // cbInherentType
            // 
            cbInherentType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbInherentType.Enabled = false;
            cbInherentType.FormattingEnabled = true;
            cbInherentType.Items.AddRange(new object[] { "None", "Accolade", "Class", "Incarnate", "Inherent", "Pet", "Power", "Powerset", "Temp" });
            cbInherentType.Location = new System.Drawing.Point(87, 171);
            cbInherentType.Name = "cbInherentType";
            cbInherentType.Size = new System.Drawing.Size(121, 21);
            cbInherentType.TabIndex = 43;
            cbInherentType.SelectedIndexChanged += cbInherentType_SelectedIndexChanged;
            // 
            // chkSortOverride
            // 
            chkSortOverride.Location = new System.Drawing.Point(6, 111);
            chkSortOverride.Name = "chkSortOverride";
            chkSortOverride.Size = new System.Drawing.Size(184, 25);
            chkSortOverride.TabIndex = 42;
            chkSortOverride.Text = "Priority Sort Order";
            chkSortOverride.UseVisualStyleBackColor = true;
            chkSortOverride.CheckedChanged += chkSortOverride_CheckedChanged;
            // 
            // chkSubInclude
            // 
            chkSubInclude.Location = new System.Drawing.Point(6, 141);
            chkSubInclude.Name = "chkSubInclude";
            chkSubInclude.Size = new System.Drawing.Size(153, 23);
            chkSubInclude.TabIndex = 4;
            chkSubInclude.Text = "Display in Inherent Grid";
            chkSubInclude.UseVisualStyleBackColor = true;
            chkSubInclude.CheckedChanged += chkSubInclude_CheckedChanged;
            // 
            // chkAlwaysToggle
            // 
            chkAlwaysToggle.Location = new System.Drawing.Point(6, 48);
            chkAlwaysToggle.Name = "chkAlwaysToggle";
            chkAlwaysToggle.Size = new System.Drawing.Size(202, 24);
            chkAlwaysToggle.TabIndex = 1;
            chkAlwaysToggle.Text = "Toggle/Auto Defaults to ON";
            chkAlwaysToggle.CheckedChanged += chkAlwaysToggle_CheckedChanged;
            // 
            // chkBuffCycle
            // 
            chkBuffCycle.Location = new System.Drawing.Point(6, 18);
            chkBuffCycle.Name = "chkBuffCycle";
            chkBuffCycle.Size = new System.Drawing.Size(168, 24);
            chkBuffCycle.TabIndex = 0;
            chkBuffCycle.Text = "Power is a Click-Buff";
            chkBuffCycle.CheckedChanged += chkBuffCycle_CheckedChanged;
            // 
            // chkGraphFix
            // 
            chkGraphFix.Location = new System.Drawing.Point(6, 78);
            chkGraphFix.Name = "chkGraphFix";
            chkGraphFix.Size = new System.Drawing.Size(202, 27);
            chkGraphFix.TabIndex = 2;
            chkGraphFix.Text = "Ignore when setting graph scale";
            chkGraphFix.UseVisualStyleBackColor = true;
            chkGraphFix.CheckedChanged += chkGraphFix_CheckedChanged;
            // 
            // Label21
            // 
            Label21.AutoSize = true;
            Label21.Location = new System.Drawing.Point(8, 208);
            Label21.Name = "Label21";
            Label21.Size = new System.Drawing.Size(77, 13);
            Label21.TabIndex = 41;
            Label21.Text = "Grid Position:";
            Label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtVisualLocation
            // 
            txtVisualLocation.Location = new System.Drawing.Point(87, 205);
            txtVisualLocation.Name = "txtVisualLocation";
            txtVisualLocation.Size = new System.Drawing.Size(76, 22);
            txtVisualLocation.TabIndex = 5;
            txtVisualLocation.Text = "0";
            txtVisualLocation.TextChanged += txtVisualLocation_TextChanged;
            txtVisualLocation.Leave += txtVisualLocation_Leave;
            // 
            // GroupBox7
            // 
            GroupBox7.Controls.Add(cbForcedClass);
            GroupBox7.Controls.Add(Label29);
            GroupBox7.Location = new System.Drawing.Point(332, 243);
            GroupBox7.Name = "GroupBox7";
            GroupBox7.Size = new System.Drawing.Size(269, 86);
            GroupBox7.TabIndex = 22;
            GroupBox7.TabStop = false;
            GroupBox7.Text = "Forced Class";
            // 
            // cbForcedClass
            // 
            cbForcedClass.DropDownStyle = ComboBoxStyle.DropDownList;
            cbForcedClass.FormattingEnabled = true;
            cbForcedClass.Location = new System.Drawing.Point(88, 35);
            cbForcedClass.Name = "cbForcedClass";
            cbForcedClass.Size = new System.Drawing.Size(175, 21);
            cbForcedClass.TabIndex = 0;
            cbForcedClass.SelectedIndexChanged += cbForcedClass_SelectedIndexChanged;
            // 
            // Label29
            // 
            Label29.Location = new System.Drawing.Point(7, 33);
            Label29.Name = "Label29";
            Label29.Size = new System.Drawing.Size(77, 23);
            Label29.TabIndex = 18;
            Label29.Text = "Class Name:";
            Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox6
            // 
            GroupBox6.Controls.Add(chkSummonDisplayEntity);
            GroupBox6.Controls.Add(chkSummonStealAttributes);
            GroupBox6.Controls.Add(chkSummonStealEffects);
            GroupBox6.Location = new System.Drawing.Point(332, 133);
            GroupBox6.Name = "GroupBox6";
            GroupBox6.Size = new System.Drawing.Size(269, 104);
            GroupBox6.TabIndex = 21;
            GroupBox6.TabStop = false;
            GroupBox6.Text = "Summon Handling";
            // 
            // chkSummonDisplayEntity
            // 
            chkSummonDisplayEntity.Location = new System.Drawing.Point(6, 78);
            chkSummonDisplayEntity.Name = "chkSummonDisplayEntity";
            chkSummonDisplayEntity.Size = new System.Drawing.Size(257, 24);
            chkSummonDisplayEntity.TabIndex = 2;
            chkSummonDisplayEntity.Text = "Display entity even if absorbed";
            chkSummonDisplayEntity.UseVisualStyleBackColor = true;
            chkSummonDisplayEntity.CheckedChanged += chkSummonDisplayEntity_CheckedChanged;
            // 
            // chkSummonStealAttributes
            // 
            chkSummonStealAttributes.Location = new System.Drawing.Point(6, 48);
            chkSummonStealAttributes.Name = "chkSummonStealAttributes";
            chkSummonStealAttributes.Size = new System.Drawing.Size(257, 24);
            chkSummonStealAttributes.TabIndex = 1;
            chkSummonStealAttributes.Text = "Power absorbs summoned entity's attributes\r\n";
            chkSummonStealAttributes.UseVisualStyleBackColor = true;
            chkSummonStealAttributes.CheckedChanged += chkSummonStealAttributes_CheckedChanged;
            // 
            // chkSummonStealEffects
            // 
            chkSummonStealEffects.Location = new System.Drawing.Point(6, 18);
            chkSummonStealEffects.Name = "chkSummonStealEffects";
            chkSummonStealEffects.Size = new System.Drawing.Size(257, 24);
            chkSummonStealEffects.TabIndex = 0;
            chkSummonStealEffects.Text = "Power absorbs summoned entity's effects";
            chkSummonStealEffects.UseVisualStyleBackColor = true;
            chkSummonStealEffects.CheckedChanged += chkSummonStealEffects_CheckedChanged;
            // 
            // GroupBox5
            // 
            GroupBox5.Controls.Add(txtDescLong);
            GroupBox5.Controls.Add(txtDescShort);
            GroupBox5.Controls.Add(Label2);
            GroupBox5.Controls.Add(Label3);
            GroupBox5.Location = new System.Drawing.Point(332, 3);
            GroupBox5.Name = "GroupBox5";
            GroupBox5.Size = new System.Drawing.Size(489, 123);
            GroupBox5.TabIndex = 20;
            GroupBox5.TabStop = false;
            GroupBox5.Text = "Descriptions";
            // 
            // txtDescLong
            // 
            txtDescLong.Location = new System.Drawing.Point(58, 48);
            txtDescLong.Multiline = true;
            txtDescLong.Name = "txtDescLong";
            txtDescLong.ScrollBars = ScrollBars.Vertical;
            txtDescLong.Size = new System.Drawing.Size(425, 66);
            txtDescLong.TabIndex = 1;
            txtDescLong.Text = "Power Desc Long";
            txtDescLong.TextChanged += txtDescLong_TextChanged;
            // 
            // txtDescShort
            // 
            txtDescShort.Location = new System.Drawing.Point(58, 18);
            txtDescShort.Name = "txtDescShort";
            txtDescShort.Size = new System.Drawing.Size(425, 22);
            txtDescShort.TabIndex = 0;
            txtDescShort.Text = "Power Desc Short";
            txtDescShort.TextChanged += txtDescShort_TextChanged;
            // 
            // Label2
            // 
            Label2.Location = new System.Drawing.Point(6, 18);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(48, 24);
            Label2.TabIndex = 3;
            Label2.Text = "Short:";
            Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            Label3.Location = new System.Drawing.Point(6, 48);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(48, 24);
            Label3.TabIndex = 5;
            Label3.Text = "Long:";
            Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox3
            // 
            GroupBox3.Controls.Add(udScaleStart);
            GroupBox3.Controls.Add(label19);
            GroupBox3.Controls.Add(overideScale);
            GroupBox3.Controls.Add(Label28);
            GroupBox3.Controls.Add(udScaleMax);
            GroupBox3.Controls.Add(Label27);
            GroupBox3.Controls.Add(udScaleMin);
            GroupBox3.Controls.Add(Label26);
            GroupBox3.Controls.Add(txtScaleName);
            GroupBox3.Controls.Add(chkScale);
            GroupBox3.Location = new System.Drawing.Point(13, 230);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new System.Drawing.Size(313, 116);
            GroupBox3.TabIndex = 8;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "Power Scaling";
            // 
            // udScaleStart
            // 
            udScaleStart.Location = new System.Drawing.Point(76, 84);
            udScaleStart.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            udScaleStart.Name = "udScaleStart";
            udScaleStart.Size = new System.Drawing.Size(45, 22);
            udScaleStart.TabIndex = 10;
            udScaleStart.ValueChanged += udScaleStart_ValueChanged;
            udScaleStart.KeyPress += udScaleStart_KeyPress;
            udScaleStart.Leave += udScaleStart_Leave;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(2, 87);
            label19.Margin = new Padding(0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(70, 13);
            label19.TabIndex = 9;
            label19.Text = "Initial Value:";
            label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overideScale
            // 
            overideScale.AutoSize = true;
            overideScale.Location = new System.Drawing.Point(172, 26);
            overideScale.Name = "overideScale";
            overideScale.Size = new System.Drawing.Size(137, 17);
            overideScale.TabIndex = 8;
            overideScale.Text = "Override Scaling Limit";
            overideScale.UseVisualStyleBackColor = true;
            overideScale.CheckedChanged += overideScale_CheckedChanged;
            // 
            // Label28
            // 
            Label28.AutoSize = true;
            Label28.Location = new System.Drawing.Point(222, 87);
            Label28.Margin = new Padding(0);
            Label28.Name = "Label28";
            Label28.Size = new System.Drawing.Size(31, 13);
            Label28.TabIndex = 7;
            Label28.Text = "Max:";
            Label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // udScaleMax
            // 
            udScaleMax.Location = new System.Drawing.Point(257, 84);
            udScaleMax.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            udScaleMax.Name = "udScaleMax";
            udScaleMax.Size = new System.Drawing.Size(45, 22);
            udScaleMax.TabIndex = 3;
            udScaleMax.ValueChanged += udScaleMax_ValueChanged;
            udScaleMax.KeyPress += udScaleMax_KeyPress;
            udScaleMax.Leave += udScaleMax_Leave;
            // 
            // Label27
            // 
            Label27.AutoSize = true;
            Label27.Location = new System.Drawing.Point(132, 87);
            Label27.Margin = new Padding(0);
            Label27.Name = "Label27";
            Label27.Size = new System.Drawing.Size(30, 13);
            Label27.TabIndex = 5;
            Label27.Text = "Min:";
            Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udScaleMin
            // 
            udScaleMin.Location = new System.Drawing.Point(166, 84);
            udScaleMin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            udScaleMin.Name = "udScaleMin";
            udScaleMin.Size = new System.Drawing.Size(45, 22);
            udScaleMin.TabIndex = 2;
            udScaleMin.ValueChanged += udScaleMin_ValueChanged;
            udScaleMin.KeyPress += udScaleMin_KeyPress;
            udScaleMin.Leave += udScaleMin_Leave;
            // 
            // Label26
            // 
            Label26.AutoSize = true;
            Label26.Location = new System.Drawing.Point(12, 56);
            Label26.Name = "Label26";
            Label26.Size = new System.Drawing.Size(91, 13);
            Label26.TabIndex = 3;
            Label26.Text = "Scaling Variable:";
            Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtScaleName
            // 
            txtScaleName.Location = new System.Drawing.Point(112, 53);
            txtScaleName.Name = "txtScaleName";
            txtScaleName.Size = new System.Drawing.Size(190, 22);
            txtScaleName.TabIndex = 1;
            txtScaleName.Text = "Foes Hit";
            txtScaleName.TextChanged += txtScaleName_TextChanged;
            // 
            // chkScale
            // 
            chkScale.Location = new System.Drawing.Point(6, 22);
            chkScale.Name = "chkScale";
            chkScale.Size = new System.Drawing.Size(115, 24);
            chkScale.TabIndex = 0;
            chkScale.Text = "Enable Scaling";
            chkScale.UseVisualStyleBackColor = true;
            chkScale.CheckedChanged += chkScale_CheckedChanged;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(lblStaticIndex);
            GroupBox1.Controls.Add(chkHidden);
            GroupBox1.Controls.Add(lblNameUnique);
            GroupBox1.Controls.Add(Label1);
            GroupBox1.Controls.Add(lblNameFull);
            GroupBox1.Controls.Add(cbNameSet);
            GroupBox1.Controls.Add(txtNameDisplay);
            GroupBox1.Controls.Add(cbNameGroup);
            GroupBox1.Controls.Add(Label22);
            GroupBox1.Controls.Add(Label31);
            GroupBox1.Controls.Add(txtNamePower);
            GroupBox1.Controls.Add(Label33);
            GroupBox1.Location = new System.Drawing.Point(13, 3);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(313, 220);
            GroupBox1.TabIndex = 19;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Name";
            // 
            // lblStaticIndex
            // 
            lblStaticIndex.BorderStyle = BorderStyle.FixedSingle;
            lblStaticIndex.Location = new System.Drawing.Point(6, 48);
            lblStaticIndex.Name = "lblStaticIndex";
            lblStaticIndex.Size = new System.Drawing.Size(56, 24);
            lblStaticIndex.TabIndex = 25;
            lblStaticIndex.Text = "000";
            lblStaticIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblStaticIndex.Click += lblStaticIndex_Click;
            // 
            // chkHidden
            // 
            chkHidden.Location = new System.Drawing.Point(5, 80);
            chkHidden.Name = "chkHidden";
            chkHidden.Size = new System.Drawing.Size(57, 28);
            chkHidden.TabIndex = 24;
            chkHidden.Text = "Hide";
            chkHidden.UseVisualStyleBackColor = true;
            chkHidden.CheckedChanged += chkHidden_CheckedChanged;
            // 
            // lblNameUnique
            // 
            lblNameUnique.Location = new System.Drawing.Point(6, 188);
            lblNameUnique.Name = "lblNameUnique";
            lblNameUnique.Size = new System.Drawing.Size(296, 23);
            lblNameUnique.TabIndex = 19;
            lblNameUnique.Text = "This name is unique.";
            lblNameUnique.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(6, 16);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(96, 24);
            Label1.TabIndex = 1;
            Label1.Text = "Display Name:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNameFull
            // 
            lblNameFull.BorderStyle = BorderStyle.FixedSingle;
            lblNameFull.Location = new System.Drawing.Point(9, 147);
            lblNameFull.Name = "lblNameFull";
            lblNameFull.Size = new System.Drawing.Size(293, 36);
            lblNameFull.TabIndex = 16;
            lblNameFull.Text = "Group_Name.Powerset_Name.Power_Name";
            lblNameFull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbNameSet
            // 
            cbNameSet.FormattingEnabled = true;
            cbNameSet.Location = new System.Drawing.Point(106, 81);
            cbNameSet.Name = "cbNameSet";
            cbNameSet.Size = new System.Drawing.Size(196, 21);
            cbNameSet.TabIndex = 2;
            cbNameSet.SelectedIndexChanged += cbNameSet_SelectedIndexChanged;
            cbNameSet.TextChanged += cbNameSet_TextChanged;
            cbNameSet.Leave += cbNameSet_Leave;
            // 
            // txtNameDisplay
            // 
            txtNameDisplay.Location = new System.Drawing.Point(106, 18);
            txtNameDisplay.Name = "txtNameDisplay";
            txtNameDisplay.Size = new System.Drawing.Size(196, 22);
            txtNameDisplay.TabIndex = 0;
            txtNameDisplay.Text = "PowerName";
            txtNameDisplay.TextChanged += txtPowerName_TextChanged;
            txtNameDisplay.Leave += txtNameDisplay_Leave;
            // 
            // cbNameGroup
            // 
            cbNameGroup.FormattingEnabled = true;
            cbNameGroup.Location = new System.Drawing.Point(106, 48);
            cbNameGroup.Name = "cbNameGroup";
            cbNameGroup.Size = new System.Drawing.Size(196, 21);
            cbNameGroup.TabIndex = 1;
            cbNameGroup.SelectedIndexChanged += cbNameGroup_SelectedIndexChanged;
            cbNameGroup.TextChanged += cbNameGroup_TextChanged;
            cbNameGroup.Leave += cbNameGroup_Leave;
            // 
            // Label22
            // 
            Label22.Location = new System.Drawing.Point(53, 46);
            Label22.Name = "Label22";
            Label22.Size = new System.Drawing.Size(49, 24);
            Label22.TabIndex = 10;
            Label22.Text = "Group:";
            Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label31
            // 
            Label31.Location = new System.Drawing.Point(65, 81);
            Label31.Name = "Label31";
            Label31.Size = new System.Drawing.Size(35, 23);
            Label31.TabIndex = 12;
            Label31.Text = "Set:";
            Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNamePower
            // 
            txtNamePower.Location = new System.Drawing.Point(106, 113);
            txtNamePower.Name = "txtNamePower";
            txtNamePower.Size = new System.Drawing.Size(196, 22);
            txtNamePower.TabIndex = 3;
            txtNamePower.Text = "PowerName";
            txtNamePower.TextChanged += txtNamePower_TextChanged;
            txtNamePower.Leave += txtNamePower_Leave;
            // 
            // Label33
            // 
            Label33.Location = new System.Drawing.Point(14, 111);
            Label33.Name = "Label33";
            Label33.Size = new System.Drawing.Size(86, 23);
            Label33.TabIndex = 14;
            Label33.Text = "Power Name:";
            Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tpBasic
            // 
            tpBasic.Controls.Add(Label46);
            tpBasic.Controls.Add(Label47);
            tpBasic.Controls.Add(txtLifeTimeReal);
            tpBasic.Controls.Add(Label44);
            tpBasic.Controls.Add(Label45);
            tpBasic.Controls.Add(txtLifeTimeGame);
            tpBasic.Controls.Add(Label42);
            tpBasic.Controls.Add(Label43);
            tpBasic.Controls.Add(txtUseageTime);
            tpBasic.Controls.Add(Label41);
            tpBasic.Controls.Add(txtNumCharges);
            tpBasic.Controls.Add(chkIgnoreStrength);
            tpBasic.Controls.Add(Label12);
            tpBasic.Controls.Add(Label17);
            tpBasic.Controls.Add(txtRangeSec);
            tpBasic.Controls.Add(Label18);
            tpBasic.Controls.Add(GroupBox9);
            tpBasic.Controls.Add(cbNotify);
            tpBasic.Controls.Add(chkLos);
            tpBasic.Controls.Add(txtMaxTargets);
            tpBasic.Controls.Add(lblEndCost);
            tpBasic.Controls.Add(Label20);
            tpBasic.Controls.Add(lblAcc);
            tpBasic.Controls.Add(Label40);
            tpBasic.Controls.Add(Label39);
            tpBasic.Controls.Add(Label38);
            tpBasic.Controls.Add(Label37);
            tpBasic.Controls.Add(Label36);
            tpBasic.Controls.Add(Label35);
            tpBasic.Controls.Add(Label34);
            tpBasic.Controls.Add(Label16);
            tpBasic.Controls.Add(txtArc);
            tpBasic.Controls.Add(Label15);
            tpBasic.Controls.Add(txtRadius);
            tpBasic.Controls.Add(txtLevel);
            tpBasic.Controls.Add(cbEffectArea);
            tpBasic.Controls.Add(Label14);
            tpBasic.Controls.Add(Label13);
            tpBasic.Controls.Add(txtEndCost);
            tpBasic.Controls.Add(Label10);
            tpBasic.Controls.Add(txtActivate);
            tpBasic.Controls.Add(Label11);
            tpBasic.Controls.Add(txtRechargeTime);
            tpBasic.Controls.Add(Label8);
            tpBasic.Controls.Add(txtCastTime);
            tpBasic.Controls.Add(Label9);
            tpBasic.Controls.Add(txtInterrupt);
            tpBasic.Controls.Add(Label7);
            tpBasic.Controls.Add(txtRange);
            tpBasic.Controls.Add(Label6);
            tpBasic.Controls.Add(txtAcc);
            tpBasic.Controls.Add(cbPowerType);
            tpBasic.Controls.Add(Label5);
            tpBasic.Controls.Add(Label4);
            tpBasic.Location = new System.Drawing.Point(4, 24);
            tpBasic.Name = "tpBasic";
            tpBasic.Size = new System.Drawing.Size(832, 411);
            tpBasic.TabIndex = 0;
            tpBasic.Text = "Power Attributes";
            tpBasic.UseVisualStyleBackColor = true;
            tpBasic.Visible = false;
            // 
            // Label46
            // 
            Label46.Location = new System.Drawing.Point(447, 251);
            Label46.Name = "Label46";
            Label46.Size = new System.Drawing.Size(20, 23);
            Label46.TabIndex = 118;
            Label46.Text = "s";
            Label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label47
            // 
            Label47.Location = new System.Drawing.Point(256, 253);
            Label47.Name = "Label47";
            Label47.Size = new System.Drawing.Size(124, 23);
            Label47.TabIndex = 117;
            Label47.Text = "Life Time Real-World:";
            Label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLifeTimeReal
            // 
            txtLifeTimeReal.Location = new System.Drawing.Point(384, 253);
            txtLifeTimeReal.Name = "txtLifeTimeReal";
            txtLifeTimeReal.Size = new System.Drawing.Size(57, 22);
            txtLifeTimeReal.TabIndex = 116;
            txtLifeTimeReal.Text = "1";
            txtLifeTimeReal.TextAlign = HorizontalAlignment.Right;
            txtLifeTimeReal.TextChanged += txtLifeTimeReal_TextChanged;
            txtLifeTimeReal.Leave += txtLifeTimeReal_Leave;
            // 
            // Label44
            // 
            Label44.Location = new System.Drawing.Point(447, 221);
            Label44.Name = "Label44";
            Label44.Size = new System.Drawing.Size(20, 23);
            Label44.TabIndex = 115;
            Label44.Text = "s";
            Label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label45
            // 
            Label45.Location = new System.Drawing.Point(256, 223);
            Label45.Name = "Label45";
            Label45.Size = new System.Drawing.Size(124, 23);
            Label45.TabIndex = 114;
            Label45.Text = "Life Time In-Game:";
            Label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLifeTimeGame
            // 
            txtLifeTimeGame.Location = new System.Drawing.Point(384, 223);
            txtLifeTimeGame.Name = "txtLifeTimeGame";
            txtLifeTimeGame.Size = new System.Drawing.Size(57, 22);
            txtLifeTimeGame.TabIndex = 113;
            txtLifeTimeGame.Text = "1";
            txtLifeTimeGame.TextAlign = HorizontalAlignment.Right;
            txtLifeTimeGame.TextChanged += txtLifeTimeGame_TextChanged;
            txtLifeTimeGame.Leave += txtLifeTimeGame_Leave;
            // 
            // Label42
            // 
            Label42.Location = new System.Drawing.Point(447, 191);
            Label42.Name = "Label42";
            Label42.Size = new System.Drawing.Size(20, 23);
            Label42.TabIndex = 112;
            Label42.Text = "s";
            Label42.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label43
            // 
            Label43.Location = new System.Drawing.Point(256, 193);
            Label43.Name = "Label43";
            Label43.Size = new System.Drawing.Size(124, 23);
            Label43.TabIndex = 111;
            Label43.Text = "Usage Time:";
            Label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtUseageTime
            // 
            txtUseageTime.Location = new System.Drawing.Point(384, 193);
            txtUseageTime.Name = "txtUseageTime";
            txtUseageTime.Size = new System.Drawing.Size(57, 22);
            txtUseageTime.TabIndex = 110;
            txtUseageTime.Text = "1";
            txtUseageTime.TextAlign = HorizontalAlignment.Right;
            txtUseageTime.TextChanged += txtUseageTime_TextChanged;
            txtUseageTime.Leave += txtUseageTime_Leave;
            // 
            // Label41
            // 
            Label41.Location = new System.Drawing.Point(256, 163);
            Label41.Name = "Label41";
            Label41.Size = new System.Drawing.Size(124, 23);
            Label41.TabIndex = 108;
            Label41.Text = "Charge Count:";
            Label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNumCharges
            // 
            txtNumCharges.Location = new System.Drawing.Point(384, 163);
            txtNumCharges.Name = "txtNumCharges";
            txtNumCharges.Size = new System.Drawing.Size(57, 22);
            txtNumCharges.TabIndex = 107;
            txtNumCharges.Text = "1";
            txtNumCharges.TextAlign = HorizontalAlignment.Right;
            txtNumCharges.TextChanged += txtNumCharges_TextChanged;
            txtNumCharges.Leave += txtNumCharges_Leave;
            // 
            // chkIgnoreStrength
            // 
            chkIgnoreStrength.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkIgnoreStrength.Checked = true;
            chkIgnoreStrength.CheckState = CheckState.Checked;
            chkIgnoreStrength.Location = new System.Drawing.Point(256, 134);
            chkIgnoreStrength.Name = "chkIgnoreStrength";
            chkIgnoreStrength.Size = new System.Drawing.Size(223, 23);
            chkIgnoreStrength.TabIndex = 16;
            chkIgnoreStrength.Text = "Ignore Strength";
            chkIgnoreStrength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkIgnoreStrength.CheckedChanged += chkIgnoreStrength_CheckedChanged;
            // 
            // Label12
            // 
            Label12.Location = new System.Drawing.Point(180, 251);
            Label12.Name = "Label12";
            Label12.Size = new System.Drawing.Size(20, 23);
            Label12.TabIndex = 106;
            Label12.Text = "ft";
            Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label17
            // 
            Label17.Location = new System.Drawing.Point(9, 253);
            Label17.Name = "Label17";
            Label17.Size = new System.Drawing.Size(104, 23);
            Label17.TabIndex = 105;
            Label17.Text = "Secondary  Range:";
            Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRangeSec
            // 
            txtRangeSec.Location = new System.Drawing.Point(117, 253);
            txtRangeSec.Name = "txtRangeSec";
            txtRangeSec.Size = new System.Drawing.Size(57, 22);
            txtRangeSec.TabIndex = 8;
            txtRangeSec.Text = "1.0";
            txtRangeSec.TextAlign = HorizontalAlignment.Right;
            txtRangeSec.TextChanged += txtRangeSec_TextChanged;
            txtRangeSec.Leave += txtRangeSec_Leave;
            // 
            // Label18
            // 
            Label18.Location = new System.Drawing.Point(256, 73);
            Label18.Name = "Label18";
            Label18.Size = new System.Drawing.Size(122, 23);
            Label18.TabIndex = 33;
            Label18.Text = "Notify Mobs:";
            Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox9
            // 
            GroupBox9.Controls.Add(rbFlagCastThrough);
            GroupBox9.Controls.Add(rbFlagDisallow);
            GroupBox9.Controls.Add(rbFlagRequired);
            GroupBox9.Controls.Add(rbFlagVector);
            GroupBox9.Controls.Add(rbFlagCast);
            GroupBox9.Controls.Add(clbFlags);
            GroupBox9.Controls.Add(rbFlagTargetsSec);
            GroupBox9.Controls.Add(rbFlagTargets);
            GroupBox9.Controls.Add(rbFlagAffected);
            GroupBox9.Controls.Add(rbFlagAutoHit);
            GroupBox9.Location = new System.Drawing.Point(521, 9);
            GroupBox9.Name = "GroupBox9";
            GroupBox9.Size = new System.Drawing.Size(300, 392);
            GroupBox9.TabIndex = 103;
            GroupBox9.TabStop = false;
            GroupBox9.Text = "Adv. Attributes";
            // 
            // rbFlagCastThrough
            // 
            rbFlagCastThrough.Location = new System.Drawing.Point(152, 128);
            rbFlagCastThrough.Name = "rbFlagCastThrough";
            rbFlagCastThrough.Size = new System.Drawing.Size(140, 20);
            rbFlagCastThrough.TabIndex = 9;
            rbFlagCastThrough.TabStop = true;
            rbFlagCastThrough.Text = "Cast Through...";
            rbFlagCastThrough.UseVisualStyleBackColor = true;
            rbFlagCastThrough.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagDisallow
            // 
            rbFlagDisallow.Location = new System.Drawing.Point(152, 102);
            rbFlagDisallow.Name = "rbFlagDisallow";
            rbFlagDisallow.Size = new System.Drawing.Size(140, 19);
            rbFlagDisallow.TabIndex = 7;
            rbFlagDisallow.TabStop = true;
            rbFlagDisallow.Text = "Modes Disallowed";
            rbFlagDisallow.UseVisualStyleBackColor = true;
            rbFlagDisallow.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagRequired
            // 
            rbFlagRequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagRequired.Location = new System.Drawing.Point(6, 102);
            rbFlagRequired.Name = "rbFlagRequired";
            rbFlagRequired.Size = new System.Drawing.Size(140, 19);
            rbFlagRequired.TabIndex = 6;
            rbFlagRequired.TabStop = true;
            rbFlagRequired.Text = "Modes Required";
            rbFlagRequired.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagRequired.UseVisualStyleBackColor = true;
            rbFlagRequired.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagVector
            // 
            rbFlagVector.Location = new System.Drawing.Point(152, 75);
            rbFlagVector.Name = "rbFlagVector";
            rbFlagVector.Size = new System.Drawing.Size(140, 20);
            rbFlagVector.TabIndex = 5;
            rbFlagVector.TabStop = true;
            rbFlagVector.Text = "Vector / Damage Types";
            rbFlagVector.UseVisualStyleBackColor = true;
            rbFlagVector.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagCast
            // 
            rbFlagCast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagCast.Location = new System.Drawing.Point(6, 75);
            rbFlagCast.Name = "rbFlagCast";
            rbFlagCast.Size = new System.Drawing.Size(140, 20);
            rbFlagCast.TabIndex = 4;
            rbFlagCast.TabStop = true;
            rbFlagCast.Text = "Cast Flags";
            rbFlagCast.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagCast.UseVisualStyleBackColor = true;
            rbFlagCast.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // clbFlags
            // 
            clbFlags.FormattingEnabled = true;
            clbFlags.Location = new System.Drawing.Point(6, 150);
            clbFlags.Name = "clbFlags";
            clbFlags.Size = new System.Drawing.Size(288, 225);
            clbFlags.TabIndex = 10;
            clbFlags.ItemCheck += clbFlags_ItemCheck;
            // 
            // rbFlagTargetsSec
            // 
            rbFlagTargetsSec.Location = new System.Drawing.Point(152, 48);
            rbFlagTargetsSec.Name = "rbFlagTargetsSec";
            rbFlagTargetsSec.Size = new System.Drawing.Size(140, 20);
            rbFlagTargetsSec.TabIndex = 3;
            rbFlagTargetsSec.TabStop = true;
            rbFlagTargetsSec.Text = "Secondary Targets";
            rbFlagTargetsSec.UseVisualStyleBackColor = true;
            rbFlagTargetsSec.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagTargets
            // 
            rbFlagTargets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagTargets.Location = new System.Drawing.Point(6, 48);
            rbFlagTargets.Name = "rbFlagTargets";
            rbFlagTargets.Size = new System.Drawing.Size(140, 20);
            rbFlagTargets.TabIndex = 2;
            rbFlagTargets.TabStop = true;
            rbFlagTargets.Text = "Targets";
            rbFlagTargets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagTargets.UseVisualStyleBackColor = true;
            rbFlagTargets.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagAffected
            // 
            rbFlagAffected.Location = new System.Drawing.Point(152, 22);
            rbFlagAffected.Name = "rbFlagAffected";
            rbFlagAffected.Size = new System.Drawing.Size(140, 20);
            rbFlagAffected.TabIndex = 1;
            rbFlagAffected.TabStop = true;
            rbFlagAffected.Text = "Entities Affected";
            rbFlagAffected.UseVisualStyleBackColor = true;
            rbFlagAffected.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // rbFlagAutoHit
            // 
            rbFlagAutoHit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagAutoHit.Location = new System.Drawing.Point(6, 22);
            rbFlagAutoHit.Name = "rbFlagAutoHit";
            rbFlagAutoHit.Size = new System.Drawing.Size(140, 20);
            rbFlagAutoHit.TabIndex = 0;
            rbFlagAutoHit.TabStop = true;
            rbFlagAutoHit.Text = "Entities AutoHit";
            rbFlagAutoHit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            rbFlagAutoHit.UseVisualStyleBackColor = true;
            rbFlagAutoHit.CheckedChanged += rbFlagX_CheckedChanged;
            // 
            // cbNotify
            // 
            cbNotify.DropDownStyle = ComboBoxStyle.DropDownList;
            cbNotify.Location = new System.Drawing.Point(384, 73);
            cbNotify.Name = "cbNotify";
            cbNotify.Size = new System.Drawing.Size(95, 21);
            cbNotify.TabIndex = 14;
            cbNotify.SelectedIndexChanged += cbNotify_SelectedIndexChanged;
            // 
            // chkLos
            // 
            chkLos.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkLos.Checked = true;
            chkLos.CheckState = CheckState.Checked;
            chkLos.Location = new System.Drawing.Point(256, 104);
            chkLos.Name = "chkLos";
            chkLos.Size = new System.Drawing.Size(223, 23);
            chkLos.TabIndex = 15;
            chkLos.Text = "Requires Line of Sight";
            chkLos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkLos.CheckedChanged += chkLos_CheckedChanged;
            // 
            // txtMaxTargets
            // 
            txtMaxTargets.Location = new System.Drawing.Point(117, 340);
            txtMaxTargets.Name = "txtMaxTargets";
            txtMaxTargets.Size = new System.Drawing.Size(57, 22);
            txtMaxTargets.TabIndex = 11;
            txtMaxTargets.Text = "1";
            txtMaxTargets.TextAlign = HorizontalAlignment.Right;
            txtMaxTargets.TextChanged += txtMaxTargets_TextChanged;
            txtMaxTargets.Leave += txtMaxTargets_Leave;
            // 
            // lblEndCost
            // 
            lblEndCost.Location = new System.Drawing.Point(180, 191);
            lblEndCost.Name = "lblEndCost";
            lblEndCost.Size = new System.Drawing.Size(70, 23);
            lblEndCost.TabIndex = 101;
            lblEndCost.Text = "(1.05/s)";
            lblEndCost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label20
            // 
            Label20.Location = new System.Drawing.Point(12, 340);
            Label20.Name = "Label20";
            Label20.Size = new System.Drawing.Size(101, 23);
            Label20.TabIndex = 39;
            Label20.Text = "Max Targets:";
            Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAcc
            // 
            lblAcc.Location = new System.Drawing.Point(180, 41);
            lblAcc.Name = "lblAcc";
            lblAcc.Size = new System.Drawing.Size(95, 23);
            lblAcc.TabIndex = 100;
            lblAcc.Text = "(75%)";
            lblAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label40
            // 
            Label40.Location = new System.Drawing.Point(180, 308);
            Label40.Name = "Label40";
            Label40.Size = new System.Drawing.Size(30, 23);
            Label40.TabIndex = 99;
            Label40.Text = "deg.";
            Label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label39
            // 
            Label39.Location = new System.Drawing.Point(180, 281);
            Label39.Name = "Label39";
            Label39.Size = new System.Drawing.Size(30, 23);
            Label39.TabIndex = 98;
            Label39.Text = "ft";
            Label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label38
            // 
            Label38.Location = new System.Drawing.Point(180, 221);
            Label38.Name = "Label38";
            Label38.Size = new System.Drawing.Size(20, 23);
            Label38.TabIndex = 97;
            Label38.Text = "ft";
            Label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label37
            // 
            Label37.Location = new System.Drawing.Point(180, 161);
            Label37.Name = "Label37";
            Label37.Size = new System.Drawing.Size(20, 23);
            Label37.TabIndex = 96;
            Label37.Text = "s";
            Label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label36
            // 
            Label36.Location = new System.Drawing.Point(180, 131);
            Label36.Name = "Label36";
            Label36.Size = new System.Drawing.Size(20, 23);
            Label36.TabIndex = 95;
            Label36.Text = "s";
            Label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label35
            // 
            Label35.Location = new System.Drawing.Point(180, 101);
            Label35.Name = "Label35";
            Label35.Size = new System.Drawing.Size(20, 23);
            Label35.TabIndex = 94;
            Label35.Text = "s";
            Label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label34
            // 
            Label34.Location = new System.Drawing.Point(180, 71);
            Label34.Name = "Label34";
            Label34.Size = new System.Drawing.Size(20, 23);
            Label34.TabIndex = 93;
            Label34.Text = "s";
            Label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label16
            // 
            Label16.Location = new System.Drawing.Point(35, 310);
            Label16.Name = "Label16";
            Label16.Size = new System.Drawing.Size(78, 23);
            Label16.TabIndex = 30;
            Label16.Text = "Arc:";
            Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtArc
            // 
            txtArc.Location = new System.Drawing.Point(117, 310);
            txtArc.Name = "txtArc";
            txtArc.Size = new System.Drawing.Size(57, 22);
            txtArc.TabIndex = 10;
            txtArc.Text = "1";
            txtArc.TextAlign = HorizontalAlignment.Right;
            txtArc.TextChanged += txtArc_TextChanged;
            txtArc.Leave += txtArc_Leave;
            // 
            // Label15
            // 
            Label15.Location = new System.Drawing.Point(35, 283);
            Label15.Name = "Label15";
            Label15.Size = new System.Drawing.Size(78, 23);
            Label15.TabIndex = 28;
            Label15.Text = "Radius:";
            Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRadius
            // 
            txtRadius.Location = new System.Drawing.Point(117, 283);
            txtRadius.Name = "txtRadius";
            txtRadius.Size = new System.Drawing.Size(57, 22);
            txtRadius.TabIndex = 9;
            txtRadius.Text = "1";
            txtRadius.TextAlign = HorizontalAlignment.Right;
            txtRadius.TextChanged += txtRadius_TextChanged;
            txtRadius.Leave += txtRadius_Leave;
            // 
            // txtLevel
            // 
            txtLevel.Location = new System.Drawing.Point(117, 13);
            txtLevel.Name = "txtLevel";
            txtLevel.Size = new System.Drawing.Size(57, 22);
            txtLevel.TabIndex = 0;
            txtLevel.Text = "1";
            txtLevel.TextAlign = HorizontalAlignment.Right;
            txtLevel.TextChanged += txtLevel_TextChanged;
            txtLevel.Leave += txtLevel_Leave;
            // 
            // cbEffectArea
            // 
            cbEffectArea.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEffectArea.Items.AddRange(new object[] { "None", "Character", "Sphere", "Cone", "Location" });
            cbEffectArea.Location = new System.Drawing.Point(384, 43);
            cbEffectArea.Name = "cbEffectArea";
            cbEffectArea.Size = new System.Drawing.Size(95, 21);
            cbEffectArea.TabIndex = 13;
            cbEffectArea.SelectedIndexChanged += cbEffectArea_SelectedIndexChanged;
            // 
            // Label14
            // 
            Label14.Location = new System.Drawing.Point(256, 43);
            Label14.Name = "Label14";
            Label14.Size = new System.Drawing.Size(122, 23);
            Label14.TabIndex = 24;
            Label14.Text = "Effect Area:";
            Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label13
            // 
            Label13.Location = new System.Drawing.Point(9, 193);
            Label13.Name = "Label13";
            Label13.Size = new System.Drawing.Size(104, 23);
            Label13.TabIndex = 19;
            Label13.Text = "Endurance Cost:";
            Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEndCost
            // 
            txtEndCost.Location = new System.Drawing.Point(117, 193);
            txtEndCost.Name = "txtEndCost";
            txtEndCost.Size = new System.Drawing.Size(57, 22);
            txtEndCost.TabIndex = 6;
            txtEndCost.Text = "1.0";
            txtEndCost.TextAlign = HorizontalAlignment.Right;
            txtEndCost.TextChanged += txtEndCost_TextChanged;
            txtEndCost.Leave += txtEndCost_Leave;
            // 
            // Label10
            // 
            Label10.Location = new System.Drawing.Point(9, 163);
            Label10.Name = "Label10";
            Label10.Size = new System.Drawing.Size(104, 23);
            Label10.TabIndex = 17;
            Label10.Text = "Activate Interval:";
            Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtActivate
            // 
            txtActivate.Location = new System.Drawing.Point(117, 163);
            txtActivate.Name = "txtActivate";
            txtActivate.Size = new System.Drawing.Size(57, 22);
            txtActivate.TabIndex = 5;
            txtActivate.Text = "1.0";
            txtActivate.TextAlign = HorizontalAlignment.Right;
            txtActivate.TextChanged += txtActivate_TextChanged;
            txtActivate.Leave += txtActivate_Leave;
            // 
            // Label11
            // 
            Label11.Location = new System.Drawing.Point(9, 133);
            Label11.Name = "Label11";
            Label11.Size = new System.Drawing.Size(104, 23);
            Label11.TabIndex = 15;
            Label11.Text = "Recharge Time:";
            Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRechargeTime
            // 
            txtRechargeTime.Location = new System.Drawing.Point(117, 133);
            txtRechargeTime.Name = "txtRechargeTime";
            txtRechargeTime.Size = new System.Drawing.Size(57, 22);
            txtRechargeTime.TabIndex = 4;
            txtRechargeTime.Text = "1.0";
            txtRechargeTime.TextAlign = HorizontalAlignment.Right;
            txtRechargeTime.TextChanged += txtRechargeTime_TextChanged;
            txtRechargeTime.Leave += txtRechargeTime_Leave;
            // 
            // Label8
            // 
            Label8.Location = new System.Drawing.Point(9, 103);
            Label8.Name = "Label8";
            Label8.Size = new System.Drawing.Size(104, 23);
            Label8.TabIndex = 13;
            Label8.Text = "Casting Time:";
            Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCastTime
            // 
            txtCastTime.Location = new System.Drawing.Point(117, 103);
            txtCastTime.Name = "txtCastTime";
            txtCastTime.Size = new System.Drawing.Size(57, 22);
            txtCastTime.TabIndex = 3;
            txtCastTime.Text = "1.0";
            txtCastTime.TextAlign = HorizontalAlignment.Right;
            txtCastTime.TextChanged += txtCastTime_TextChanged;
            txtCastTime.Leave += txtCastTime_Leave;
            // 
            // Label9
            // 
            Label9.Location = new System.Drawing.Point(9, 73);
            Label9.Name = "Label9";
            Label9.Size = new System.Drawing.Size(104, 23);
            Label9.TabIndex = 11;
            Label9.Text = "Interrupt Time:";
            Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtInterrupt
            // 
            txtInterrupt.Location = new System.Drawing.Point(117, 73);
            txtInterrupt.Name = "txtInterrupt";
            txtInterrupt.Size = new System.Drawing.Size(57, 22);
            txtInterrupt.TabIndex = 2;
            txtInterrupt.Text = "1.0";
            txtInterrupt.TextAlign = HorizontalAlignment.Right;
            txtInterrupt.TextChanged += txtInterrupt_TextChanged;
            txtInterrupt.Leave += txtInterrupt_Leave;
            // 
            // Label7
            // 
            Label7.Location = new System.Drawing.Point(9, 223);
            Label7.Name = "Label7";
            Label7.Size = new System.Drawing.Size(104, 23);
            Label7.TabIndex = 9;
            Label7.Text = "Range:";
            Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRange
            // 
            txtRange.Location = new System.Drawing.Point(117, 223);
            txtRange.Name = "txtRange";
            txtRange.Size = new System.Drawing.Size(57, 22);
            txtRange.TabIndex = 7;
            txtRange.Text = "1.0";
            txtRange.TextAlign = HorizontalAlignment.Right;
            txtRange.TextChanged += txtRange_TextChanged;
            txtRange.Leave += txtRange_Leave;
            // 
            // Label6
            // 
            Label6.Location = new System.Drawing.Point(9, 43);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(104, 23);
            Label6.TabIndex = 7;
            Label6.Text = "Accuracy:";
            Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAcc
            // 
            txtAcc.Location = new System.Drawing.Point(117, 43);
            txtAcc.Name = "txtAcc";
            txtAcc.Size = new System.Drawing.Size(57, 22);
            txtAcc.TabIndex = 1;
            txtAcc.Text = "1.0";
            txtAcc.TextAlign = HorizontalAlignment.Right;
            txtAcc.TextChanged += txtAcc_TextChanged;
            txtAcc.Leave += txtAcc_Leave;
            // 
            // cbPowerType
            // 
            cbPowerType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPowerType.Items.AddRange(new object[] { "Click", "Passive", "Toggle" });
            cbPowerType.Location = new System.Drawing.Point(384, 13);
            cbPowerType.Name = "cbPowerType";
            cbPowerType.Size = new System.Drawing.Size(95, 21);
            cbPowerType.TabIndex = 12;
            cbPowerType.SelectedIndexChanged += cbPowerType_SelectedIndexChanged;
            // 
            // Label5
            // 
            Label5.Location = new System.Drawing.Point(256, 13);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(122, 23);
            Label5.TabIndex = 4;
            Label5.Text = "Power Type:";
            Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label4
            // 
            Label4.Location = new System.Drawing.Point(9, 13);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(104, 23);
            Label4.TabIndex = 2;
            Label4.Text = "Level Available:";
            Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tpEffects
            // 
            tpEffects.Controls.Add(lvFX);
            tpEffects.Controls.Add(pnlFX);
            tpEffects.Location = new System.Drawing.Point(4, 24);
            tpEffects.Name = "tpEffects";
            tpEffects.Size = new System.Drawing.Size(832, 411);
            tpEffects.TabIndex = 1;
            tpEffects.Text = "Effects";
            tpEffects.UseVisualStyleBackColor = true;
            tpEffects.Visible = false;
            // 
            // lvFX
            // 
            lvFX.HorizontalScrollbar = true;
            lvFX.Location = new System.Drawing.Point(8, 9);
            lvFX.Name = "lvFX";
            lvFX.Size = new System.Drawing.Size(744, 303);
            lvFX.TabIndex = 71;
            lvFX.DoubleClick += lvFX_DoubleClick;
            // 
            // pnlFX
            // 
            pnlFX.Controls.Add(cbCoDFormat);
            pnlFX.Controls.Add(btnSetDamage);
            pnlFX.Controls.Add(btnFXEdit);
            pnlFX.Controls.Add(btnFXDown);
            pnlFX.Controls.Add(btnFXUp);
            pnlFX.Controls.Add(btnFXRemove);
            pnlFX.Controls.Add(btnFXDuplicate);
            pnlFX.Controls.Add(btnFXAdd);
            pnlFX.Location = new System.Drawing.Point(4, 5);
            pnlFX.Name = "pnlFX";
            pnlFX.Size = new System.Drawing.Size(824, 383);
            pnlFX.TabIndex = 71;
            // 
            // cbCoDFormat
            // 
            cbCoDFormat.AutoSize = true;
            cbCoDFormat.Location = new System.Drawing.Point(580, 337);
            cbCoDFormat.Name = "cbCoDFormat";
            cbCoDFormat.Size = new System.Drawing.Size(139, 17);
            cbCoDFormat.TabIndex = 79;
            cbCoDFormat.Text = "Use CoD effect format";
            cbCoDFormat.UseVisualStyleBackColor = true;
            cbCoDFormat.CheckedChanged += cbCoDFormat_CheckedChanged;
            // 
            // btnSetDamage
            // 
            btnSetDamage.Location = new System.Drawing.Point(312, 332);
            btnSetDamage.Name = "btnSetDamage";
            btnSetDamage.Size = new System.Drawing.Size(212, 28);
            btnSetDamage.TabIndex = 78;
            btnSetDamage.Text = "Set damage types from effect list";
            // 
            // btnFXEdit
            // 
            btnFXEdit.Location = new System.Drawing.Point(160, 332);
            btnFXEdit.Name = "btnFXEdit";
            btnFXEdit.Size = new System.Drawing.Size(64, 28);
            btnFXEdit.TabIndex = 77;
            btnFXEdit.Text = "Edit...";
            btnFXEdit.Click += btnFXEdit_Click;
            // 
            // btnFXDown
            // 
            btnFXDown.Location = new System.Drawing.Point(756, 46);
            btnFXDown.Name = "btnFXDown";
            btnFXDown.Size = new System.Drawing.Size(64, 28);
            btnFXDown.TabIndex = 11;
            btnFXDown.Text = "Down";
            btnFXDown.Click += btnFXDown_Click;
            // 
            // btnFXUp
            // 
            btnFXUp.Location = new System.Drawing.Point(756, 9);
            btnFXUp.Name = "btnFXUp";
            btnFXUp.Size = new System.Drawing.Size(64, 28);
            btnFXUp.TabIndex = 12;
            btnFXUp.Text = "Up";
            btnFXUp.Click += btnFXUp_Click;
            // 
            // btnFXRemove
            // 
            btnFXRemove.Location = new System.Drawing.Point(236, 332);
            btnFXRemove.Name = "btnFXRemove";
            btnFXRemove.Size = new System.Drawing.Size(64, 28);
            btnFXRemove.TabIndex = 10;
            btnFXRemove.Text = "Remove";
            btnFXRemove.Click += btnFXRemove_Click;
            // 
            // btnFXDuplicate
            // 
            btnFXDuplicate.Location = new System.Drawing.Point(84, 332);
            btnFXDuplicate.Name = "btnFXDuplicate";
            btnFXDuplicate.Size = new System.Drawing.Size(64, 28);
            btnFXDuplicate.TabIndex = 69;
            btnFXDuplicate.Text = "Duplicate";
            btnFXDuplicate.Click += btnFXDuplicate_Click;
            // 
            // btnFXAdd
            // 
            btnFXAdd.Location = new System.Drawing.Point(8, 332);
            btnFXAdd.Name = "btnFXAdd";
            btnFXAdd.Size = new System.Drawing.Size(64, 28);
            btnFXAdd.TabIndex = 9;
            btnFXAdd.Text = "Add";
            btnFXAdd.Click += btnFXAdd_Click;
            // 
            // tpEnh
            // 
            tpEnh.Controls.Add(chkBoostUsePlayerLevel);
            tpEnh.Controls.Add(chkBoostBoostable);
            tpEnh.Controls.Add(Label23);
            tpEnh.Controls.Add(pbEnhancements);
            tpEnh.Controls.Add(chkPRFrontLoad);
            tpEnh.Controls.Add(pbEnhancementList);
            tpEnh.Controls.Add(lblEnhName);
            tpEnh.Location = new System.Drawing.Point(4, 24);
            tpEnh.Name = "tpEnh";
            tpEnh.Size = new System.Drawing.Size(832, 411);
            tpEnh.TabIndex = 9;
            tpEnh.Text = "Enhancements";
            tpEnh.UseVisualStyleBackColor = true;
            // 
            // chkBoostUsePlayerLevel
            // 
            chkBoostUsePlayerLevel.Location = new System.Drawing.Point(357, 157);
            chkBoostUsePlayerLevel.Name = "chkBoostUsePlayerLevel";
            chkBoostUsePlayerLevel.Size = new System.Drawing.Size(324, 23);
            chkBoostUsePlayerLevel.TabIndex = 91;
            chkBoostUsePlayerLevel.Text = "Attuned Enhancement";
            chkBoostUsePlayerLevel.CheckedChanged += chkBoostUsePlayerLevel_CheckedChanged;
            // 
            // chkBoostBoostable
            // 
            chkBoostBoostable.Location = new System.Drawing.Point(357, 127);
            chkBoostBoostable.Name = "chkBoostBoostable";
            chkBoostBoostable.Size = new System.Drawing.Size(324, 23);
            chkBoostBoostable.TabIndex = 90;
            chkBoostBoostable.Text = "Allow Enhancement Boosters (Enhancement only)";
            chkBoostBoostable.CheckedChanged += chkBoostBoostable_CheckedChanged;
            // 
            // Label23
            // 
            Label23.Location = new System.Drawing.Point(24, 14);
            Label23.Name = "Label23";
            Label23.Size = new System.Drawing.Size(300, 18);
            Label23.TabIndex = 89;
            Label23.Text = "Enhancement Classes Accepted:";
            Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbEnhancements
            // 
            pbEnhancements.BackColor = System.Drawing.Color.FromArgb(255, 128, 0);
            pbEnhancements.Location = new System.Drawing.Point(24, 32);
            pbEnhancements.Name = "pbEnhancements";
            pbEnhancements.Size = new System.Drawing.Size(780, 46);
            pbEnhancements.TabIndex = 43;
            pbEnhancements.TabStop = false;
            pbEnhancements.Paint += pbEnhancements_Paint;
            pbEnhancements.MouseDown += pbEnhancements_MouseDown;
            pbEnhancements.MouseMove += pbEnhancements_Hover;
            // 
            // chkPRFrontLoad
            // 
            chkPRFrontLoad.Location = new System.Drawing.Point(357, 97);
            chkPRFrontLoad.Name = "chkPRFrontLoad";
            chkPRFrontLoad.Size = new System.Drawing.Size(324, 23);
            chkPRFrontLoad.TabIndex = 88;
            chkPRFrontLoad.Text = "Allow front-loading of enhancement slots (for Kheld forms)";
            chkPRFrontLoad.CheckedChanged += chkPRFrontLoad_CheckedChanged;
            // 
            // pbEnhancementList
            // 
            pbEnhancementList.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
            pbEnhancementList.Location = new System.Drawing.Point(24, 97);
            pbEnhancementList.Name = "pbEnhancementList";
            pbEnhancementList.Size = new System.Drawing.Size(316, 254);
            pbEnhancementList.TabIndex = 44;
            pbEnhancementList.TabStop = false;
            pbEnhancementList.Paint += pbEnhancementList_Paint;
            pbEnhancementList.MouseDown += pbEnhancementList_MouseDown;
            pbEnhancementList.MouseMove += pbEnhancementList_Hover;
            // 
            // lblEnhName
            // 
            lblEnhName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblEnhName.Location = new System.Drawing.Point(21, 78);
            lblEnhName.Name = "lblEnhName";
            lblEnhName.Size = new System.Drawing.Size(316, 19);
            lblEnhName.TabIndex = 46;
            // 
            // tpSets
            // 
            tpSets.Controls.Add(Label24);
            tpSets.Controls.Add(lblInvSet);
            tpSets.Controls.Add(pbInvSetList);
            tpSets.Controls.Add(pbInvSetUsed);
            tpSets.Location = new System.Drawing.Point(4, 24);
            tpSets.Name = "tpSets";
            tpSets.Size = new System.Drawing.Size(832, 411);
            tpSets.TabIndex = 5;
            tpSets.Text = "Invention Set Types";
            tpSets.UseVisualStyleBackColor = true;
            // 
            // Label24
            // 
            Label24.Location = new System.Drawing.Point(24, 14);
            Label24.Name = "Label24";
            Label24.Size = new System.Drawing.Size(300, 18);
            Label24.TabIndex = 93;
            Label24.Text = "Invention Set Types Accepted:";
            Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblInvSet
            // 
            lblInvSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblInvSet.Location = new System.Drawing.Point(24, 78);
            lblInvSet.Name = "lblInvSet";
            lblInvSet.Size = new System.Drawing.Size(316, 19);
            lblInvSet.TabIndex = 92;
            // 
            // pbInvSetList
            // 
            pbInvSetList.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            pbInvSetList.Location = new System.Drawing.Point(24, 97);
            pbInvSetList.Name = "pbInvSetList";
            pbInvSetList.Size = new System.Drawing.Size(316, 295);
            pbInvSetList.TabIndex = 91;
            pbInvSetList.TabStop = false;
            pbInvSetList.Paint += pbInvSetList_Paint;
            pbInvSetList.MouseDown += pbInvSetList_MouseDown;
            pbInvSetList.MouseMove += pbInvSetList_MouseMove;
            // 
            // pbInvSetUsed
            // 
            pbInvSetUsed.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            pbInvSetUsed.Location = new System.Drawing.Point(24, 32);
            pbInvSetUsed.Name = "pbInvSetUsed";
            pbInvSetUsed.Size = new System.Drawing.Size(780, 46);
            pbInvSetUsed.TabIndex = 90;
            pbInvSetUsed.TabStop = false;
            pbInvSetUsed.Paint += pbInvSetUsed_Paint;
            pbInvSetUsed.MouseDown += pbInvSetUsed_MouseDown;
            pbInvSetUsed.MouseMove += pbInvSetUsed_MouseMove;
            // 
            // tpPreReq
            // 
            tpPreReq.Controls.Add(GroupBox11);
            tpPreReq.Controls.Add(GroupBox10);
            tpPreReq.Controls.Add(GroupBox8);
            tpPreReq.Location = new System.Drawing.Point(4, 24);
            tpPreReq.Name = "tpPreReq";
            tpPreReq.Size = new System.Drawing.Size(832, 411);
            tpPreReq.TabIndex = 4;
            tpPreReq.Text = "Requirements";
            tpPreReq.UseVisualStyleBackColor = true;
            tpPreReq.Visible = false;
            // 
            // GroupBox11
            // 
            GroupBox11.Controls.Add(btnPrReset);
            GroupBox11.Controls.Add(btnPrSetNone);
            GroupBox11.Controls.Add(btnPrDown);
            GroupBox11.Controls.Add(btnPrUp);
            GroupBox11.Controls.Add(rbPrRemove);
            GroupBox11.Controls.Add(rbPrAdd);
            GroupBox11.Controls.Add(rbPrPowerB);
            GroupBox11.Controls.Add(rbPrPowerA);
            GroupBox11.Controls.Add(lvPrPower);
            GroupBox11.Controls.Add(lvPrSet);
            GroupBox11.Controls.Add(lvPrGroup);
            GroupBox11.Controls.Add(lvPrListing);
            GroupBox11.Location = new System.Drawing.Point(3, 16);
            GroupBox11.Name = "GroupBox11";
            GroupBox11.Size = new System.Drawing.Size(611, 369);
            GroupBox11.TabIndex = 97;
            GroupBox11.TabStop = false;
            GroupBox11.Text = "Required Powers";
            // 
            // btnPrReset
            // 
            btnPrReset.Location = new System.Drawing.Point(432, 336);
            btnPrReset.Name = "btnPrReset";
            btnPrReset.Size = new System.Drawing.Size(156, 26);
            btnPrReset.TabIndex = 15;
            btnPrReset.Text = "Reset";
            btnPrReset.UseVisualStyleBackColor = true;
            btnPrReset.Click += btnPrReset_Click;
            // 
            // btnPrSetNone
            // 
            btnPrSetNone.Location = new System.Drawing.Point(432, 177);
            btnPrSetNone.Name = "btnPrSetNone";
            btnPrSetNone.Size = new System.Drawing.Size(156, 26);
            btnPrSetNone.TabIndex = 14;
            btnPrSetNone.Text = "Set Power A to None";
            btnPrSetNone.UseVisualStyleBackColor = true;
            btnPrSetNone.Click += btnPrSetNone_Click;
            // 
            // btnPrDown
            // 
            btnPrDown.Location = new System.Drawing.Point(513, 235);
            btnPrDown.Name = "btnPrDown";
            btnPrDown.Size = new System.Drawing.Size(75, 27);
            btnPrDown.TabIndex = 13;
            btnPrDown.Text = "Down";
            btnPrDown.UseVisualStyleBackColor = true;
            btnPrDown.Click += btnPrDown_Click;
            // 
            // btnPrUp
            // 
            btnPrUp.Location = new System.Drawing.Point(432, 235);
            btnPrUp.Name = "btnPrUp";
            btnPrUp.Size = new System.Drawing.Size(75, 27);
            btnPrUp.TabIndex = 12;
            btnPrUp.Text = "Up";
            btnPrUp.UseVisualStyleBackColor = true;
            btnPrUp.Click += btnPrUp_Click;
            // 
            // rbPrRemove
            // 
            rbPrRemove.Location = new System.Drawing.Point(432, 302);
            rbPrRemove.Name = "rbPrRemove";
            rbPrRemove.Size = new System.Drawing.Size(156, 27);
            rbPrRemove.TabIndex = 11;
            rbPrRemove.Text = "Remove Selected";
            rbPrRemove.UseVisualStyleBackColor = true;
            rbPrRemove.Click += rbPrRemove_Click;
            // 
            // rbPrAdd
            // 
            rbPrAdd.Location = new System.Drawing.Point(432, 269);
            rbPrAdd.Name = "rbPrAdd";
            rbPrAdd.Size = new System.Drawing.Size(156, 26);
            rbPrAdd.TabIndex = 10;
            rbPrAdd.Text = "Add New";
            rbPrAdd.UseVisualStyleBackColor = true;
            rbPrAdd.Click += rbPrAdd_Click;
            // 
            // rbPrPowerB
            // 
            rbPrPowerB.Appearance = Appearance.Button;
            rbPrPowerB.Location = new System.Drawing.Point(513, 143);
            rbPrPowerB.Name = "rbPrPowerB";
            rbPrPowerB.Size = new System.Drawing.Size(75, 27);
            rbPrPowerB.TabIndex = 9;
            rbPrPowerB.Text = "Power B";
            rbPrPowerB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            rbPrPowerB.UseVisualStyleBackColor = true;
            rbPrPowerB.CheckedChanged += rbPrPowerX_CheckedChanged;
            // 
            // rbPrPowerA
            // 
            rbPrPowerA.Appearance = Appearance.Button;
            rbPrPowerA.Checked = true;
            rbPrPowerA.Location = new System.Drawing.Point(432, 143);
            rbPrPowerA.Name = "rbPrPowerA";
            rbPrPowerA.Size = new System.Drawing.Size(75, 27);
            rbPrPowerA.TabIndex = 8;
            rbPrPowerA.TabStop = true;
            rbPrPowerA.Text = "Power A";
            rbPrPowerA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            rbPrPowerA.UseVisualStyleBackColor = true;
            rbPrPowerA.CheckedChanged += rbPrPowerX_CheckedChanged;
            // 
            // lvPrPower
            // 
            lvPrPower.Columns.AddRange(new ColumnHeader[] { ColumnHeader9 });
            lvPrPower.FullRowSelect = true;
            lvPrPower.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPrPower.Location = new System.Drawing.Point(290, 125);
            lvPrPower.MultiSelect = false;
            lvPrPower.Name = "lvPrPower";
            lvPrPower.Size = new System.Drawing.Size(136, 237);
            lvPrPower.TabIndex = 3;
            lvPrPower.UseCompatibleStateImageBehavior = false;
            lvPrPower.View = View.Details;
            lvPrPower.SelectedIndexChanged += lvPrPower_SelectedIndexChanged;
            // 
            // ColumnHeader9
            // 
            ColumnHeader9.Text = "Power";
            ColumnHeader9.Width = 110;
            // 
            // lvPrSet
            // 
            lvPrSet.Columns.AddRange(new ColumnHeader[] { ColumnHeader10 });
            lvPrSet.FullRowSelect = true;
            lvPrSet.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPrSet.Location = new System.Drawing.Point(148, 125);
            lvPrSet.MultiSelect = false;
            lvPrSet.Name = "lvPrSet";
            lvPrSet.Size = new System.Drawing.Size(136, 237);
            lvPrSet.TabIndex = 2;
            lvPrSet.UseCompatibleStateImageBehavior = false;
            lvPrSet.View = View.Details;
            lvPrSet.SelectedIndexChanged += lvPrSet_SelectedIndexChanged;
            // 
            // ColumnHeader10
            // 
            ColumnHeader10.Text = "Set";
            ColumnHeader10.Width = 110;
            // 
            // lvPrGroup
            // 
            lvPrGroup.Columns.AddRange(new ColumnHeader[] { ColumnHeader11 });
            lvPrGroup.FullRowSelect = true;
            lvPrGroup.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPrGroup.Location = new System.Drawing.Point(6, 125);
            lvPrGroup.MultiSelect = false;
            lvPrGroup.Name = "lvPrGroup";
            lvPrGroup.Size = new System.Drawing.Size(136, 237);
            lvPrGroup.TabIndex = 1;
            lvPrGroup.UseCompatibleStateImageBehavior = false;
            lvPrGroup.View = View.Details;
            lvPrGroup.SelectedIndexChanged += lvPrGroup_SelectedIndexChanged;
            // 
            // ColumnHeader11
            // 
            ColumnHeader11.Text = "Group";
            ColumnHeader11.Width = 110;
            // 
            // lvPrListing
            // 
            lvPrListing.Columns.AddRange(new ColumnHeader[] { ColumnHeader6, ColumnHeader7, ColumnHeader8 });
            lvPrListing.FullRowSelect = true;
            lvPrListing.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPrListing.Location = new System.Drawing.Point(6, 22);
            lvPrListing.MultiSelect = false;
            lvPrListing.Name = "lvPrListing";
            lvPrListing.Size = new System.Drawing.Size(599, 96);
            lvPrListing.TabIndex = 0;
            lvPrListing.UseCompatibleStateImageBehavior = false;
            lvPrListing.View = View.Details;
            lvPrListing.SelectedIndexChanged += lvPrListing_SelectedIndexChanged;
            // 
            // ColumnHeader6
            // 
            ColumnHeader6.Text = "Power A";
            ColumnHeader6.Width = 265;
            // 
            // ColumnHeader7
            // 
            ColumnHeader7.Text = "";
            ColumnHeader7.TextAlign = HorizontalAlignment.Center;
            ColumnHeader7.Width = 40;
            // 
            // ColumnHeader8
            // 
            ColumnHeader8.Text = "Power B";
            ColumnHeader8.Width = 265;
            // 
            // GroupBox10
            // 
            GroupBox10.Controls.Add(clbClassExclude);
            GroupBox10.Location = new System.Drawing.Point(620, 181);
            GroupBox10.Name = "GroupBox10";
            GroupBox10.Size = new System.Drawing.Size(207, 158);
            GroupBox10.TabIndex = 96;
            GroupBox10.TabStop = false;
            GroupBox10.Text = "Excluded Classes";
            // 
            // clbClassExclude
            // 
            clbClassExclude.FormattingEnabled = true;
            clbClassExclude.Location = new System.Drawing.Point(6, 22);
            clbClassExclude.Name = "clbClassExclude";
            clbClassExclude.Size = new System.Drawing.Size(192, 106);
            clbClassExclude.TabIndex = 0;
            // 
            // GroupBox8
            // 
            GroupBox8.Controls.Add(clbClassReq);
            GroupBox8.Location = new System.Drawing.Point(620, 16);
            GroupBox8.Name = "GroupBox8";
            GroupBox8.Size = new System.Drawing.Size(207, 158);
            GroupBox8.TabIndex = 95;
            GroupBox8.TabStop = false;
            GroupBox8.Text = "Reqired Classes";
            // 
            // clbClassReq
            // 
            clbClassReq.FormattingEnabled = true;
            clbClassReq.Location = new System.Drawing.Point(6, 22);
            clbClassReq.Name = "clbClassReq";
            clbClassReq.Size = new System.Drawing.Size(192, 106);
            clbClassReq.TabIndex = 0;
            // 
            // tpSpecialEnh
            // 
            tpSpecialEnh.Controls.Add(Label32);
            tpSpecialEnh.Controls.Add(Label30);
            tpSpecialEnh.Controls.Add(lvDisablePass4);
            tpSpecialEnh.Controls.Add(lvDisablePass1);
            tpSpecialEnh.Location = new System.Drawing.Point(4, 24);
            tpSpecialEnh.Name = "tpSpecialEnh";
            tpSpecialEnh.Padding = new Padding(3);
            tpSpecialEnh.Size = new System.Drawing.Size(832, 411);
            tpSpecialEnh.TabIndex = 7;
            tpSpecialEnh.Text = "Enhancement Disabling";
            tpSpecialEnh.UseVisualStyleBackColor = true;
            // 
            // Label32
            // 
            Label32.Location = new System.Drawing.Point(246, 8);
            Label32.Name = "Label32";
            Label32.Size = new System.Drawing.Size(211, 39);
            Label32.TabIndex = 113;
            Label32.Text = "Pass Four\r\n(Buffs applied after ED)";
            // 
            // Label30
            // 
            Label30.Location = new System.Drawing.Point(7, 8);
            Label30.Name = "Label30";
            Label30.Size = new System.Drawing.Size(211, 39);
            Label30.TabIndex = 112;
            Label30.Text = "Pass One\r\n(Enhancements before ED is applied)";
            // 
            // lvDisablePass4
            // 
            lvDisablePass4.Location = new System.Drawing.Point(245, 51);
            lvDisablePass4.Name = "lvDisablePass4";
            lvDisablePass4.SelectionMode = SelectionMode.MultiSimple;
            lvDisablePass4.Size = new System.Drawing.Size(212, 316);
            lvDisablePass4.TabIndex = 111;
            lvDisablePass4.SelectedIndexChanged += lvDisablePass4_SelectedIndexChanged;
            // 
            // lvDisablePass1
            // 
            lvDisablePass1.Location = new System.Drawing.Point(6, 51);
            lvDisablePass1.Name = "lvDisablePass1";
            lvDisablePass1.SelectionMode = SelectionMode.MultiSimple;
            lvDisablePass1.Size = new System.Drawing.Size(212, 316);
            lvDisablePass1.TabIndex = 110;
            lvDisablePass1.SelectedIndexChanged += lvDisablePass1_SelectedIndexChanged;
            // 
            // tpMutex
            // 
            tpMutex.Controls.Add(GroupBox2);
            tpMutex.Controls.Add(chkMutexAuto);
            tpMutex.Controls.Add(chkMutexSkip);
            tpMutex.Location = new System.Drawing.Point(4, 24);
            tpMutex.Name = "tpMutex";
            tpMutex.Size = new System.Drawing.Size(832, 411);
            tpMutex.TabIndex = 8;
            tpMutex.Text = "Mutal Exclusivity";
            tpMutex.UseVisualStyleBackColor = true;
            // 
            // GroupBox2
            // 
            GroupBox2.Controls.Add(btnMutexAdd);
            GroupBox2.Controls.Add(clbMutex);
            GroupBox2.Location = new System.Drawing.Point(9, 16);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(325, 360);
            GroupBox2.TabIndex = 5;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Group Membership";
            // 
            // btnMutexAdd
            // 
            btnMutexAdd.Location = new System.Drawing.Point(6, 327);
            btnMutexAdd.Name = "btnMutexAdd";
            btnMutexAdd.Size = new System.Drawing.Size(124, 26);
            btnMutexAdd.TabIndex = 1;
            btnMutexAdd.Text = "Add New Group...";
            btnMutexAdd.UseVisualStyleBackColor = true;
            btnMutexAdd.Click += btnMutexAdd_Click;
            // 
            // clbMutex
            // 
            clbMutex.FormattingEnabled = true;
            clbMutex.Location = new System.Drawing.Point(6, 22);
            clbMutex.Name = "clbMutex";
            clbMutex.Size = new System.Drawing.Size(313, 293);
            clbMutex.TabIndex = 0;
            // 
            // chkMutexAuto
            // 
            chkMutexAuto.Location = new System.Drawing.Point(340, 66);
            chkMutexAuto.Name = "chkMutexAuto";
            chkMutexAuto.Size = new System.Drawing.Size(217, 21);
            chkMutexAuto.TabIndex = 4;
            chkMutexAuto.Text = "Auto-Detoggle other powers";
            chkMutexAuto.UseVisualStyleBackColor = true;
            chkMutexAuto.CheckedChanged += chkMutexAuto_CheckedChanged;
            // 
            // chkMutexSkip
            // 
            chkMutexSkip.Location = new System.Drawing.Point(340, 38);
            chkMutexSkip.Name = "chkMutexSkip";
            chkMutexSkip.Size = new System.Drawing.Size(217, 21);
            chkMutexSkip.TabIndex = 3;
            chkMutexSkip.Text = "Skip Mutal Exclusivity for this power";
            chkMutexSkip.UseVisualStyleBackColor = true;
            chkMutexSkip.CheckedChanged += chkMutexSkip_CheckedChanged;
            // 
            // tpSubPower
            // 
            tpSubPower.Controls.Add(btnSPAdd);
            tpSubPower.Controls.Add(btnSPRemove);
            tpSubPower.Controls.Add(lvSPSelected);
            tpSubPower.Controls.Add(lvSPPower);
            tpSubPower.Controls.Add(lvSPSet);
            tpSubPower.Controls.Add(lvSPGroup);
            tpSubPower.Location = new System.Drawing.Point(4, 24);
            tpSubPower.Name = "tpSubPower";
            tpSubPower.Size = new System.Drawing.Size(832, 411);
            tpSubPower.TabIndex = 10;
            tpSubPower.Text = "Sub-Powers";
            tpSubPower.UseVisualStyleBackColor = true;
            // 
            // btnSPAdd
            // 
            btnSPAdd.Location = new System.Drawing.Point(437, 163);
            btnSPAdd.Name = "btnSPAdd";
            btnSPAdd.Size = new System.Drawing.Size(48, 26);
            btnSPAdd.TabIndex = 13;
            btnSPAdd.Text = ">>";
            btnSPAdd.UseVisualStyleBackColor = true;
            btnSPAdd.Click += btnSPAdd_Click;
            // 
            // btnSPRemove
            // 
            btnSPRemove.Location = new System.Drawing.Point(437, 196);
            btnSPRemove.Name = "btnSPRemove";
            btnSPRemove.Size = new System.Drawing.Size(48, 27);
            btnSPRemove.TabIndex = 12;
            btnSPRemove.Text = "<<";
            btnSPRemove.UseVisualStyleBackColor = true;
            btnSPRemove.Click += btnSPRemove_Click;
            // 
            // lvSPSelected
            // 
            lvSPSelected.Columns.AddRange(new ColumnHeader[] { ColumnHeader4 });
            lvSPSelected.FullRowSelect = true;
            lvSPSelected.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSPSelected.Location = new System.Drawing.Point(491, 18);
            lvSPSelected.MultiSelect = false;
            lvSPSelected.Name = "lvSPSelected";
            lvSPSelected.Size = new System.Drawing.Size(324, 352);
            lvSPSelected.TabIndex = 7;
            lvSPSelected.UseCompatibleStateImageBehavior = false;
            lvSPSelected.View = View.Details;
            // 
            // ColumnHeader4
            // 
            ColumnHeader4.Text = "Power";
            ColumnHeader4.Width = 293;
            // 
            // lvSPPower
            // 
            lvSPPower.Columns.AddRange(new ColumnHeader[] { ColumnHeader1 });
            lvSPPower.FullRowSelect = true;
            lvSPPower.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSPPower.Location = new System.Drawing.Point(295, 18);
            lvSPPower.MultiSelect = false;
            lvSPPower.Name = "lvSPPower";
            lvSPPower.Size = new System.Drawing.Size(136, 352);
            lvSPPower.TabIndex = 6;
            lvSPPower.UseCompatibleStateImageBehavior = false;
            lvSPPower.View = View.Details;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Power";
            ColumnHeader1.Width = 110;
            // 
            // lvSPSet
            // 
            lvSPSet.Columns.AddRange(new ColumnHeader[] { ColumnHeader2 });
            lvSPSet.FullRowSelect = true;
            lvSPSet.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSPSet.Location = new System.Drawing.Point(153, 18);
            lvSPSet.MultiSelect = false;
            lvSPSet.Name = "lvSPSet";
            lvSPSet.Size = new System.Drawing.Size(136, 352);
            lvSPSet.TabIndex = 5;
            lvSPSet.UseCompatibleStateImageBehavior = false;
            lvSPSet.View = View.Details;
            lvSPSet.SelectedIndexChanged += lvSPSet_SelectedIndexChanged;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Set";
            ColumnHeader2.Width = 110;
            // 
            // lvSPGroup
            // 
            lvSPGroup.Columns.AddRange(new ColumnHeader[] { ColumnHeader3 });
            lvSPGroup.FullRowSelect = true;
            lvSPGroup.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSPGroup.Location = new System.Drawing.Point(11, 18);
            lvSPGroup.MultiSelect = false;
            lvSPGroup.Name = "lvSPGroup";
            lvSPGroup.Size = new System.Drawing.Size(136, 352);
            lvSPGroup.TabIndex = 4;
            lvSPGroup.UseCompatibleStateImageBehavior = false;
            lvSPGroup.View = View.Details;
            lvSPGroup.SelectedIndexChanged += lvSPGroup_SelectedIndexChanged;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Group";
            ColumnHeader3.Width = 110;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(740, 455);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(104, 41);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(632, 455);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(104, 41);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnFullPaste
            // 
            btnFullPaste.Location = new System.Drawing.Point(434, 455);
            btnFullPaste.Name = "btnFullPaste";
            btnFullPaste.Size = new System.Drawing.Size(104, 41);
            btnFullPaste.TabIndex = 5;
            btnFullPaste.Text = "Paste";
            btnFullPaste.UseVisualStyleBackColor = true;
            btnFullPaste.Click += btnFullPaste_Click;
            // 
            // btnFullCopy
            // 
            btnFullCopy.Location = new System.Drawing.Point(324, 455);
            btnFullCopy.Name = "btnFullCopy";
            btnFullCopy.Size = new System.Drawing.Size(104, 41);
            btnFullCopy.TabIndex = 6;
            btnFullCopy.Text = "Copy";
            btnFullCopy.UseVisualStyleBackColor = true;
            btnFullCopy.Click += btnFullCopy_Click;
            // 
            // btnJsonExport
            // 
            btnJsonExport.Location = new System.Drawing.Point(8, 455);
            btnJsonExport.Name = "btnJsonExport";
            btnJsonExport.Size = new System.Drawing.Size(130, 41);
            btnJsonExport.TabIndex = 7;
            btnJsonExport.Text = "Export to JSON";
            btnJsonExport.UseVisualStyleBackColor = true;
            btnJsonExport.Click += btnJsonExport_Click;
            // 
            // btnJsonImport
            // 
            btnJsonImport.Location = new System.Drawing.Point(146, 455);
            btnJsonImport.Name = "btnJsonImport";
            btnJsonImport.Size = new System.Drawing.Size(130, 41);
            btnJsonImport.TabIndex = 8;
            btnJsonImport.Text = "Import from JSON";
            btnJsonImport.UseVisualStyleBackColor = true;
            btnJsonImport.Click += btnJsonImport_Click;
            // 
            // frmEditPower
            // 
            AcceptButton = btnOK;
            AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(858, 509);
            Controls.Add(btnJsonImport);
            Controls.Add(btnJsonExport);
            Controls.Add(btnFullCopy);
            Controls.Add(btnFullPaste);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(tcPower);
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmEditPower";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Power (Group_Name.Set_Name.Power_Name)";
            tcPower.ResumeLayout(false);
            tpText.ResumeLayout(false);
            GroupBox4.ResumeLayout(false);
            GroupBox4.PerformLayout();
            GroupBox7.ResumeLayout(false);
            GroupBox6.ResumeLayout(false);
            GroupBox5.ResumeLayout(false);
            GroupBox5.PerformLayout();
            GroupBox3.ResumeLayout(false);
            GroupBox3.PerformLayout();
            ((ISupportInitialize)udScaleStart).EndInit();
            ((ISupportInitialize)udScaleMax).EndInit();
            ((ISupportInitialize)udScaleMin).EndInit();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            tpBasic.ResumeLayout(false);
            tpBasic.PerformLayout();
            GroupBox9.ResumeLayout(false);
            tpEffects.ResumeLayout(false);
            pnlFX.ResumeLayout(false);
            pnlFX.PerformLayout();
            tpEnh.ResumeLayout(false);
            ((ISupportInitialize)pbEnhancements).EndInit();
            ((ISupportInitialize)pbEnhancementList).EndInit();
            tpSets.ResumeLayout(false);
            ((ISupportInitialize)pbInvSetList).EndInit();
            ((ISupportInitialize)pbInvSetUsed).EndInit();
            tpPreReq.ResumeLayout(false);
            GroupBox11.ResumeLayout(false);
            GroupBox10.ResumeLayout(false);
            GroupBox8.ResumeLayout(false);
            tpSpecialEnh.ResumeLayout(false);
            tpMutex.ResumeLayout(false);
            GroupBox2.ResumeLayout(false);
            tpSubPower.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        TextBox txtVisualLocation;
        ComboBox cbNameGroup;
        Button btnCancel;
        Button btnJsonExport;
        Button btnFullCopy;
        Button btnFullPaste;
        Button btnFXAdd;
        Button btnFXDown;
        Button btnFXDuplicate;
        Button btnFXEdit;
        Button btnFXRemove;
        Button btnFXUp;
        Button btnMutexAdd;
        Button btnOK;
        Button btnPrDown;
        Button btnPrReset;
        Button btnPrSetNone;
        Button btnPrUp;
        Button btnSetDamage;
        Button btnSPAdd;
        Button btnSPRemove;
        ComboBox cbEffectArea;
        ComboBox cbForcedClass;
        ComboBox cbNameSet;
        ComboBox cbNotify;
        ComboBox cbPowerType;
        CheckBox chkAlwaysToggle;
        CheckBox chkBoostBoostable;
        CheckBox chkBoostUsePlayerLevel;
        CheckBox chkBuffCycle;
        CheckBox chkGraphFix;
        CheckBox chkHidden;
        CheckBox chkIgnoreStrength;
        CheckBox chkLos;
        CheckBox chkMutexAuto;
        CheckBox chkMutexSkip;
        CheckBox chkNoAUReq;
        CheckBox chkNoAutoUpdate;
        CheckBox chkPRFrontLoad;
        CheckBox chkScale;
        CheckBox chkSortOverride;
        CheckBox chkSubInclude;
        CheckBox chkSummonDisplayEntity;
        CheckBox chkSummonStealAttributes;
        CheckBox chkSummonStealEffects;
        CheckedListBox clbClassExclude;
        CheckedListBox clbClassReq;
        CheckedListBox clbFlags;
        CheckedListBox clbMutex;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader10;
        ColumnHeader ColumnHeader11;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader6;
        ColumnHeader ColumnHeader7;
        ColumnHeader ColumnHeader8;
        ColumnHeader ColumnHeader9;
        GroupBox GroupBox1;
        GroupBox GroupBox10;
        GroupBox GroupBox11;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        GroupBox GroupBox4;
        GroupBox GroupBox5;
        GroupBox GroupBox6;
        GroupBox GroupBox7;
        GroupBox GroupBox8;
        GroupBox GroupBox9;
        Label Label1;
        Label Label10;
        Label Label11;
        Label Label12;
        Label Label13;
        Label Label14;
        Label Label15;
        Label Label16;
        Label Label17;
        Label Label18;
        Label Label2;
        Label Label20;
        Label Label21;
        Label Label22;
        Label Label23;
        Label Label24;
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
        Label Label35;
        Label Label36;
        Label Label37;
        Label Label38;
        Label Label39;
        Label Label4;
        Label Label40;
        Label Label41;
        Label Label42;
        Label Label43;
        Label Label44;
        Label Label45;
        Label Label46;
        Label Label47;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblAcc;
        Label lblEndCost;
        Label lblEnhName;
        Label lblInvSet;
        Label lblNameFull;
        Label lblNameUnique;
        Label lblStaticIndex;
        ListBox lvDisablePass1;
        ListBox lvDisablePass4;
        ListBox lvFX;
        ListView lvPrGroup;
        ListView lvPrListing;
        ListView lvPrPower;
        ListView lvPrSet;
        ListView lvSPGroup;
        ListView lvSPPower;
        ListView lvSPSelected;
        ListView lvSPSet;
        PictureBox pbEnhancementList;
        PictureBox pbEnhancements;
        PictureBox pbInvSetList;
        PictureBox pbInvSetUsed;
        Panel pnlFX;
        RadioButton rbFlagAffected;
        RadioButton rbFlagAutoHit;
        RadioButton rbFlagCast;
        RadioButton rbFlagCastThrough;
        RadioButton rbFlagDisallow;
        RadioButton rbFlagRequired;
        RadioButton rbFlagTargets;
        RadioButton rbFlagTargetsSec;
        RadioButton rbFlagVector;
        Button rbPrAdd;
        RadioButton rbPrPowerA;
        RadioButton rbPrPowerB;
        Button rbPrRemove;
        TabControl tcPower;
        TabPage tpBasic;
        TabPage tpEffects;
        TabPage tpEnh;
        TabPage tpMutex;
        TabPage tpPreReq;
        TabPage tpSets;
        TabPage tpSpecialEnh;
        TabPage tpSubPower;
        TabPage tpText;
        TextBox txtAcc;
        TextBox txtActivate;
        TextBox txtArc;
        TextBox txtCastTime;
        TextBox txtDescLong;
        TextBox txtDescShort;
        TextBox txtEndCost;
        TextBox txtInterrupt;
        TextBox txtLevel;
        TextBox txtLifeTimeGame;
        TextBox txtLifeTimeReal;
        TextBox txtMaxTargets;
        TextBox txtNameDisplay;
        TextBox txtNamePower;
        TextBox txtNumCharges;
        TextBox txtRadius;
        TextBox txtRange;
        TextBox txtRangeSec;
        TextBox txtRechargeTime;
        TextBox txtScaleName;
        TextBox txtUseageTime;
        NumericUpDown udScaleMax;
        NumericUpDown udScaleMin;
        ComboBox cbInherentType;
        Label lblInherentType;
        private CheckBox overideScale;
        private NumericUpDown udScaleStart;
        private Label label19;
        private CheckBox cbCoDFormat;
        private Button btnJsonImport;
    }
}
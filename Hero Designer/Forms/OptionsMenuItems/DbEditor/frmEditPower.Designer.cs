using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer
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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.tcPower = new System.Windows.Forms.TabControl();
            this.tpText = new System.Windows.Forms.TabPage();
            this.chkNoAUReq = new System.Windows.Forms.CheckBox();
            this.chkNoAutoUpdate = new System.Windows.Forms.CheckBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.chkSortOverride = new System.Windows.Forms.CheckBox();
            this.chkSubInclude = new System.Windows.Forms.CheckBox();
            this.chkAlwaysToggle = new System.Windows.Forms.CheckBox();
            this.chkBuffCycle = new System.Windows.Forms.CheckBox();
            this.chkGraphFix = new System.Windows.Forms.CheckBox();
            this.chkAltSub = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.txtVisualLocation = new System.Windows.Forms.TextBox();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.cbForcedClass = new System.Windows.Forms.ComboBox();
            this.Label29 = new System.Windows.Forms.Label();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.chkSummonDisplayEntity = new System.Windows.Forms.CheckBox();
            this.chkSummonStealAttributes = new System.Windows.Forms.CheckBox();
            this.chkSummonStealEffects = new System.Windows.Forms.CheckBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.txtDescLong = new System.Windows.Forms.TextBox();
            this.txtDescShort = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.Label28 = new System.Windows.Forms.Label();
            this.udScaleMax = new System.Windows.Forms.NumericUpDown();
            this.Label27 = new System.Windows.Forms.Label();
            this.udScaleMin = new System.Windows.Forms.NumericUpDown();
            this.Label26 = new System.Windows.Forms.Label();
            this.txtScaleName = new System.Windows.Forms.TextBox();
            this.chkScale = new System.Windows.Forms.CheckBox();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStaticIndex = new System.Windows.Forms.Label();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.lblNameUnique = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblNameFull = new System.Windows.Forms.Label();
            this.cbNameSet = new System.Windows.Forms.ComboBox();
            this.txtNameDisplay = new System.Windows.Forms.TextBox();
            this.cbNameGroup = new System.Windows.Forms.ComboBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label31 = new System.Windows.Forms.Label();
            this.txtNamePower = new System.Windows.Forms.TextBox();
            this.Label33 = new System.Windows.Forms.Label();
            this.tpBasic = new System.Windows.Forms.TabPage();
            this.Label46 = new System.Windows.Forms.Label();
            this.Label47 = new System.Windows.Forms.Label();
            this.txtLifeTimeReal = new System.Windows.Forms.TextBox();
            this.Label44 = new System.Windows.Forms.Label();
            this.Label45 = new System.Windows.Forms.Label();
            this.txtLifeTimeGame = new System.Windows.Forms.TextBox();
            this.Label42 = new System.Windows.Forms.Label();
            this.Label43 = new System.Windows.Forms.Label();
            this.txtUseageTime = new System.Windows.Forms.TextBox();
            this.Label41 = new System.Windows.Forms.Label();
            this.txtNumCharges = new System.Windows.Forms.TextBox();
            this.chkIgnoreStrength = new System.Windows.Forms.CheckBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.txtRangeSec = new System.Windows.Forms.TextBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.GroupBox9 = new System.Windows.Forms.GroupBox();
            this.rbFlagCastThrough = new System.Windows.Forms.RadioButton();
            this.rbFlagDisallow = new System.Windows.Forms.RadioButton();
            this.rbFlagRequired = new System.Windows.Forms.RadioButton();
            this.rbFlagVector = new System.Windows.Forms.RadioButton();
            this.rbFlagCast = new System.Windows.Forms.RadioButton();
            this.clbFlags = new System.Windows.Forms.CheckedListBox();
            this.rbFlagTargetsSec = new System.Windows.Forms.RadioButton();
            this.rbFlagTargets = new System.Windows.Forms.RadioButton();
            this.rbFlagAffected = new System.Windows.Forms.RadioButton();
            this.rbFlagAutoHit = new System.Windows.Forms.RadioButton();
            this.cbNotify = new System.Windows.Forms.ComboBox();
            this.chkLos = new System.Windows.Forms.CheckBox();
            this.txtMaxTargets = new System.Windows.Forms.TextBox();
            this.lblEndCost = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.lblAcc = new System.Windows.Forms.Label();
            this.Label40 = new System.Windows.Forms.Label();
            this.Label39 = new System.Windows.Forms.Label();
            this.Label38 = new System.Windows.Forms.Label();
            this.Label37 = new System.Windows.Forms.Label();
            this.Label36 = new System.Windows.Forms.Label();
            this.Label35 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.txtArc = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.txtLevel = new System.Windows.Forms.TextBox();
            this.cbEffectArea = new System.Windows.Forms.ComboBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.txtEndCost = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtActivate = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.txtRechargeTime = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.txtCastTime = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.txtInterrupt = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.txtAcc = new System.Windows.Forms.TextBox();
            this.cbPowerType = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.tpEffects = new System.Windows.Forms.TabPage();
            this.lvFX = new System.Windows.Forms.ListBox();
            this.pnlFX = new System.Windows.Forms.Panel();
            this.btnSetDamage = new System.Windows.Forms.Button();
            this.btnFXEdit = new System.Windows.Forms.Button();
            this.btnFXDown = new System.Windows.Forms.Button();
            this.btnFXUp = new System.Windows.Forms.Button();
            this.btnFXRemove = new System.Windows.Forms.Button();
            this.btnFXDuplicate = new System.Windows.Forms.Button();
            this.btnFXAdd = new System.Windows.Forms.Button();
            this.tpEnh = new System.Windows.Forms.TabPage();
            this.chkBoostUsePlayerLevel = new System.Windows.Forms.CheckBox();
            this.chkBoostBoostable = new System.Windows.Forms.CheckBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.pbEnhancements = new System.Windows.Forms.PictureBox();
            this.chkPRFrontLoad = new System.Windows.Forms.CheckBox();
            this.pbEnhancementList = new System.Windows.Forms.PictureBox();
            this.lblEnhName = new System.Windows.Forms.Label();
            this.tpSets = new System.Windows.Forms.TabPage();
            this.Label24 = new System.Windows.Forms.Label();
            this.lblInvSet = new System.Windows.Forms.Label();
            this.pbInvSetList = new System.Windows.Forms.PictureBox();
            this.pbInvSetUsed = new System.Windows.Forms.PictureBox();
            this.tpPreReq = new System.Windows.Forms.TabPage();
            this.GroupBox11 = new System.Windows.Forms.GroupBox();
            this.btnPrReset = new System.Windows.Forms.Button();
            this.btnPrSetNone = new System.Windows.Forms.Button();
            this.btnPrDown = new System.Windows.Forms.Button();
            this.btnPrUp = new System.Windows.Forms.Button();
            this.rbPrRemove = new System.Windows.Forms.Button();
            this.rbPrAdd = new System.Windows.Forms.Button();
            this.rbPrPowerB = new System.Windows.Forms.RadioButton();
            this.rbPrPowerA = new System.Windows.Forms.RadioButton();
            this.lvPrPower = new System.Windows.Forms.ListView();
            this.ColumnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.lvPrSet = new System.Windows.Forms.ListView();
            this.ColumnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.lvPrGroup = new System.Windows.Forms.ListView();
            this.ColumnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.lvPrListing = new System.Windows.Forms.ListView();
            this.ColumnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.GroupBox10 = new System.Windows.Forms.GroupBox();
            this.clbClassExclude = new System.Windows.Forms.CheckedListBox();
            this.GroupBox8 = new System.Windows.Forms.GroupBox();
            this.clbClassReq = new System.Windows.Forms.CheckedListBox();
            this.tpSpecialEnh = new System.Windows.Forms.TabPage();
            this.Label32 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.lvDisablePass4 = new System.Windows.Forms.ListBox();
            this.lvDisablePass1 = new System.Windows.Forms.ListBox();
            this.tpMutex = new System.Windows.Forms.TabPage();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnMutexAdd = new System.Windows.Forms.Button();
            this.clbMutex = new System.Windows.Forms.CheckedListBox();
            this.chkMutexAuto = new System.Windows.Forms.CheckBox();
            this.chkMutexSkip = new System.Windows.Forms.CheckBox();
            this.tpSubPower = new System.Windows.Forms.TabPage();
            this.btnSPAdd = new System.Windows.Forms.Button();
            this.btnSPRemove = new System.Windows.Forms.Button();
            this.lvSPSelected = new System.Windows.Forms.ListView();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.lvSPPower = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvSPSet = new System.Windows.Forms.ListView();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvSPGroup = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFullPaste = new System.Windows.Forms.Button();
            this.btnFullCopy = new System.Windows.Forms.Button();
            this.btnCSVImport = new System.Windows.Forms.Button();
            this.tcPower.SuspendLayout();
            this.tpText.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.udScaleMax.BeginInit();
            this.udScaleMin.BeginInit();
            this.GroupBox1.SuspendLayout();
            this.tpBasic.SuspendLayout();
            this.GroupBox9.SuspendLayout();
            this.tpEffects.SuspendLayout();
            this.pnlFX.SuspendLayout();
            this.tpEnh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pbEnhancements).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pbEnhancementList).BeginInit();
            this.tpSets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pbInvSetList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pbInvSetUsed).BeginInit();
            this.tpPreReq.SuspendLayout();
            this.GroupBox11.SuspendLayout();
            this.GroupBox10.SuspendLayout();
            this.GroupBox8.SuspendLayout();
            this.tpSpecialEnh.SuspendLayout();
            this.tpMutex.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.tpSubPower.SuspendLayout();
            this.SuspendLayout();
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpText);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpBasic);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpEffects);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpEnh);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpSets);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpPreReq);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpSpecialEnh);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpMutex);
            this.tcPower.Controls.Add((System.Windows.Forms.Control)this.tpSubPower);

            this.tcPower.Location = new System.Drawing.Point(8, 8);
            this.tcPower.Name = "tcPower";
            this.tcPower.SelectedIndex = 0;

            this.tcPower.Size = new System.Drawing.Size(840, 364);
            this.tcPower.TabIndex = 2;
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.chkNoAUReq);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.chkNoAutoUpdate);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox4);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox7);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox6);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox5);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox3);
            this.tpText.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);

            this.tpText.Location = new System.Drawing.Point(4, 23);
            this.tpText.Name = "tpText";

            this.tpText.Size = new System.Drawing.Size(832, 337);
            this.tpText.TabIndex = 2;
            this.tpText.Text = "Basic";
            this.tpText.UseVisualStyleBackColor = true;

            this.chkNoAUReq.Location = new System.Drawing.Point(332, 293);
            this.chkNoAUReq.Name = "chkNoAUReq";

            this.chkNoAUReq.Size = new System.Drawing.Size(269, 33);
            this.chkNoAUReq.TabIndex = 46;
            this.chkNoAUReq.Text = "Do not automatically update this power's requirements";
            this.chkNoAUReq.UseVisualStyleBackColor = true;
            this.chkNoAUReq.CheckedChanged += new System.EventHandler(chkNoAUReq_CheckedChanged);

            this.chkNoAutoUpdate.Location = new System.Drawing.Point(13, 306);
            this.chkNoAutoUpdate.Name = "chkNoAutoUpdate";

            this.chkNoAutoUpdate.Size = new System.Drawing.Size(313, 20);
            this.chkNoAutoUpdate.TabIndex = 23;
            this.chkNoAutoUpdate.Text = "Do not automatically update this power during bulk-imports";
            this.chkNoAutoUpdate.UseVisualStyleBackColor = true;
            this.chkNoAutoUpdate.CheckedChanged += new System.EventHandler(chkNoAutoUpdate_CheckedChanged);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkSortOverride);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkSubInclude);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkAlwaysToggle);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkBuffCycle);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkGraphFix);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkAltSub);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label21);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.txtVisualLocation);

            this.GroupBox4.Location = new System.Drawing.Point(607, 115);
            this.GroupBox4.Name = "GroupBox4";

            this.GroupBox4.Size = new System.Drawing.Size(214, 211);
            this.GroupBox4.TabIndex = 45;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "MxD Special Flags";

            this.chkSortOverride.Location = new System.Drawing.Point(6, 96);
            this.chkSortOverride.Name = "chkSortOverride";

            this.chkSortOverride.Size = new System.Drawing.Size(184, 22);
            this.chkSortOverride.TabIndex = 42;
            this.chkSortOverride.Text = "Priority Sort Order";
            this.chkSortOverride.UseVisualStyleBackColor = true;
            this.chkSortOverride.CheckedChanged += new System.EventHandler(chkSortOverride_CheckedChanged);

            this.chkSubInclude.Location = new System.Drawing.Point(6, 152);
            this.chkSubInclude.Name = "chkSubInclude";

            this.chkSubInclude.Size = new System.Drawing.Size(153, 20);
            this.chkSubInclude.TabIndex = 4;
            this.chkSubInclude.Text = "Sub-Power Include Flag";
            this.chkSubInclude.UseVisualStyleBackColor = true;
            this.chkSubInclude.CheckedChanged += new System.EventHandler(chkSubInclude_CheckedChanged);

            this.chkAlwaysToggle.Location = new System.Drawing.Point(6, 42);
            this.chkAlwaysToggle.Name = "chkAlwaysToggle";

            this.chkAlwaysToggle.Size = new System.Drawing.Size(168, 20);
            this.chkAlwaysToggle.TabIndex = 1;
            this.chkAlwaysToggle.Text = "Toggle Defaults to ON";
            this.chkAlwaysToggle.CheckedChanged += new System.EventHandler(chkAlwaysToggle_CheckedChanged);

            this.chkBuffCycle.Location = new System.Drawing.Point(6, 16);
            this.chkBuffCycle.Name = "chkBuffCycle";

            this.chkBuffCycle.Size = new System.Drawing.Size(168, 20);
            this.chkBuffCycle.TabIndex = 0;
            this.chkBuffCycle.Text = "Power is a Click-Buff";
            this.chkBuffCycle.CheckedChanged += new System.EventHandler(chkBuffCycle_CheckedChanged);

            this.chkGraphFix.Location = new System.Drawing.Point(6, 68);
            this.chkGraphFix.Name = "chkGraphFix";

            this.chkGraphFix.Size = new System.Drawing.Size(184, 22);
            this.chkGraphFix.TabIndex = 2;
            this.chkGraphFix.Text = "Ignore when setting graph scale";
            this.chkGraphFix.UseVisualStyleBackColor = true;
            this.chkGraphFix.CheckedChanged += new System.EventHandler(chkGraphFix_CheckedChanged);

            this.chkAltSub.Location = new System.Drawing.Point(6, 124);
            this.chkAltSub.Name = "chkAltSub";

            this.chkAltSub.Size = new System.Drawing.Size(183, 22);
            this.chkAltSub.TabIndex = 3;
            this.chkAltSub.Text = "Sub-Power Colour Flag (ToDo)";
            this.chkAltSub.UseVisualStyleBackColor = true;
            this.chkAltSub.CheckedChanged += new System.EventHandler(chkAltSub_CheckedChanged);

            this.Label21.Location = new System.Drawing.Point(21, 178);
            this.Label21.Name = "Label21";

            this.Label21.Size = new System.Drawing.Size(88, 20);
            this.Label21.TabIndex = 41;
            this.Label21.Text = "Inherent Index:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtVisualLocation.Location = new System.Drawing.Point(113, 178);
            this.txtVisualLocation.Name = "txtVisualLocation";

            this.txtVisualLocation.Size = new System.Drawing.Size(76, 20);
            this.txtVisualLocation.TabIndex = 5;
            this.txtVisualLocation.Text = "0";
            this.txtVisualLocation.Leave += this.txtVisualLocation_Leave;
            this.txtVisualLocation.TextChanged += this.txtVisualLocation_TextChanged;
            this.GroupBox7.Controls.Add((System.Windows.Forms.Control)this.cbForcedClass);
            this.GroupBox7.Controls.Add((System.Windows.Forms.Control)this.Label29);

            this.GroupBox7.Location = new System.Drawing.Point(332, 211);
            this.GroupBox7.Name = "GroupBox7";

            this.GroupBox7.Size = new System.Drawing.Size(269, 74);
            this.GroupBox7.TabIndex = 22;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Forced Class";
            this.cbForcedClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbForcedClass.FormattingEnabled = true;

            this.cbForcedClass.Location = new System.Drawing.Point(88, 30);
            this.cbForcedClass.Name = "cbForcedClass";

            this.cbForcedClass.Size = new System.Drawing.Size(175, 22);
            this.cbForcedClass.TabIndex = 0;
            this.cbForcedClass.SelectedIndexChanged += new System.EventHandler(cbForcedClass_SelectedIndexChanged);

            this.Label29.Location = new System.Drawing.Point(7, 30);
            this.Label29.Name = "Label29";

            this.Label29.Size = new System.Drawing.Size(77, 20);
            this.Label29.TabIndex = 18;
            this.Label29.Text = "Class Name:";
            this.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox6.Controls.Add((System.Windows.Forms.Control)this.chkSummonDisplayEntity);
            this.GroupBox6.Controls.Add((System.Windows.Forms.Control)this.chkSummonStealAttributes);
            this.GroupBox6.Controls.Add((System.Windows.Forms.Control)this.chkSummonStealEffects);

            this.GroupBox6.Location = new System.Drawing.Point(332, 115);
            this.GroupBox6.Name = "GroupBox6";

            this.GroupBox6.Size = new System.Drawing.Size(269, 90);
            this.GroupBox6.TabIndex = 21;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Summon Handling";

            this.chkSummonDisplayEntity.Location = new System.Drawing.Point(6, 68);
            this.chkSummonDisplayEntity.Name = "chkSummonDisplayEntity";

            this.chkSummonDisplayEntity.Size = new System.Drawing.Size(257, 20);
            this.chkSummonDisplayEntity.TabIndex = 2;
            this.chkSummonDisplayEntity.Text = "Display entity even if absorbed";
            this.chkSummonDisplayEntity.UseVisualStyleBackColor = true;
            this.chkSummonDisplayEntity.CheckedChanged += new System.EventHandler(chkSummonDisplayEntity_CheckedChanged);

            this.chkSummonStealAttributes.Location = new System.Drawing.Point(6, 42);
            this.chkSummonStealAttributes.Name = "chkSummonStealAttributes";

            this.chkSummonStealAttributes.Size = new System.Drawing.Size(257, 20);
            this.chkSummonStealAttributes.TabIndex = 1;
            this.chkSummonStealAttributes.Text = "Power absorbs summoned entity's attributes\r\n";
            this.chkSummonStealAttributes.UseVisualStyleBackColor = true;
            this.chkSummonStealAttributes.CheckedChanged += new System.EventHandler(chkSummonStealAttributes_CheckedChanged);

            this.chkSummonStealEffects.Location = new System.Drawing.Point(6, 16);
            this.chkSummonStealEffects.Name = "chkSummonStealEffects";

            this.chkSummonStealEffects.Size = new System.Drawing.Size(257, 20);
            this.chkSummonStealEffects.TabIndex = 0;
            this.chkSummonStealEffects.Text = "Power absorbs summoned entity's effects";
            this.chkSummonStealEffects.UseVisualStyleBackColor = true;
            this.chkSummonStealEffects.CheckedChanged += new System.EventHandler(chkSummonStealEffects_CheckedChanged);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.txtDescLong);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.txtDescShort);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.Label3);

            this.GroupBox5.Location = new System.Drawing.Point(332, 3);
            this.GroupBox5.Name = "GroupBox5";

            this.GroupBox5.Size = new System.Drawing.Size(489, 106);
            this.GroupBox5.TabIndex = 20;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Descriptions";

            this.txtDescLong.Location = new System.Drawing.Point(58, 42);
            this.txtDescLong.Multiline = true;
            this.txtDescLong.Name = "txtDescLong";
            this.txtDescLong.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.txtDescLong.Size = new System.Drawing.Size(425, 57);
            this.txtDescLong.TabIndex = 1;
            this.txtDescLong.Text = "Power Desc Long";
            this.txtDescLong.TextChanged += new System.EventHandler(txtDescLong_TextChanged);

            this.txtDescShort.Location = new System.Drawing.Point(58, 16);
            this.txtDescShort.Name = "txtDescShort";

            this.txtDescShort.Size = new System.Drawing.Size(425, 20);
            this.txtDescShort.TabIndex = 0;
            this.txtDescShort.Text = "Power Desc Short";
            this.txtDescShort.TextChanged += new System.EventHandler(txtDescShort_TextChanged);

            this.Label2.Location = new System.Drawing.Point(6, 16);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(48, 20);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Short:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label3.Location = new System.Drawing.Point(6, 42);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(48, 20);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Long";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.Label28);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.udScaleMax);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.Label27);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.udScaleMin);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.Label26);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.txtScaleName);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.chkScale);

            this.GroupBox3.Location = new System.Drawing.Point(13, 199);
            this.GroupBox3.Name = "GroupBox3";

            this.GroupBox3.Size = new System.Drawing.Size(313, 101);
            this.GroupBox3.TabIndex = 8;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Power Scaling";

            this.Label28.Location = new System.Drawing.Point(181, 72);
            this.Label28.Name = "Label28";

            this.Label28.Size = new System.Drawing.Size(52, 20);
            this.Label28.TabIndex = 7;
            this.Label28.Text = "To";
            this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.udScaleMax.Location = new System.Drawing.Point(239, 72);
            this.udScaleMax.Name = "udScaleMax";

            this.udScaleMax.Size = new System.Drawing.Size(63, 20);
            this.udScaleMax.TabIndex = 3;
            this.udScaleMax.ValueChanged += this.udScaleMax_ValueChanged;
            this.udScaleMax.Leave += new System.EventHandler(this.udScaleMax_Leave);
            this.udScaleMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.udScaleMax_KeyPress);

            this.Label27.Location = new System.Drawing.Point(5, 72);
            this.Label27.Name = "Label27";

            this.Label27.Size = new System.Drawing.Size(101, 20);
            this.Label27.TabIndex = 5;
            this.Label27.Text = "Scale Range:";
            this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udScaleMin.Location = new System.Drawing.Point(112, 72);
            this.udScaleMin.Name = "udScaleMin";

            this.udScaleMin.Size = new System.Drawing.Size(63, 20);
            this.udScaleMin.TabIndex = 2;

            this.udScaleMin.ValueChanged += this.udScaleMin_ValueChanged;
            this.udScaleMin.Leave += this.udScaleMin_Leave;
            this.udScaleMin.KeyPress += this.udScaleMin_KeyPress;

            this.Label26.Location = new System.Drawing.Point(5, 46);
            this.Label26.Name = "Label26";

            this.Label26.Size = new System.Drawing.Size(101, 20);
            this.Label26.TabIndex = 3;
            this.Label26.Text = "Scaling Variable:";
            this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtScaleName.Location = new System.Drawing.Point(112, 46);
            this.txtScaleName.Name = "txtScaleName";

            this.txtScaleName.Size = new System.Drawing.Size(190, 20);
            this.txtScaleName.TabIndex = 1;
            this.txtScaleName.Text = "Foes Hit";
            this.txtScaleName.TextChanged += new System.EventHandler(txtScaleName_TextChanged);

            this.chkScale.Location = new System.Drawing.Point(6, 19);
            this.chkScale.Name = "chkScale";

            this.chkScale.Size = new System.Drawing.Size(302, 21);
            this.chkScale.TabIndex = 0;
            this.chkScale.Text = "Enable scaling for this power";
            this.chkScale.UseVisualStyleBackColor = true;
            this.chkScale.CheckedChanged += new System.EventHandler(chkScale_CheckedChanged);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblStaticIndex);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.chkHidden);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblNameUnique);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblNameFull);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbNameSet);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtNameDisplay);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbNameGroup);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label22);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label31);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtNamePower);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label33);

            this.GroupBox1.Location = new System.Drawing.Point(13, 3);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(313, 190);
            this.GroupBox1.TabIndex = 19;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Name";
            this.lblStaticIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblStaticIndex.Location = new System.Drawing.Point(6, 42);
            this.lblStaticIndex.Name = "lblStaticIndex";

            this.lblStaticIndex.Size = new System.Drawing.Size(56, 20);
            this.lblStaticIndex.TabIndex = 25;
            this.lblStaticIndex.Text = "000";
            this.lblStaticIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStaticIndex.Click += new System.EventHandler(lblStaticIndex_Click);

            this.chkHidden.Location = new System.Drawing.Point(5, 69);
            this.chkHidden.Name = "chkHidden";

            this.chkHidden.Size = new System.Drawing.Size(57, 25);
            this.chkHidden.TabIndex = 24;
            this.chkHidden.Text = "Hide";
            this.chkHidden.UseVisualStyleBackColor = true;
            this.chkHidden.CheckedChanged += new System.EventHandler(chkHidden_CheckedChanged);

            this.lblNameUnique.Location = new System.Drawing.Point(6, 163);
            this.lblNameUnique.Name = "lblNameUnique";

            this.lblNameUnique.Size = new System.Drawing.Size(296, 20);
            this.lblNameUnique.TabIndex = 19;
            this.lblNameUnique.Text = "This name is unique.";
            this.lblNameUnique.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.Label1.Location = new System.Drawing.Point(6, 16);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(96, 20);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Display Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNameFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblNameFull.Location = new System.Drawing.Point(9, (int)sbyte.MaxValue);
            this.lblNameFull.Name = "lblNameFull";

            this.lblNameFull.Size = new System.Drawing.Size(293, 32);
            this.lblNameFull.TabIndex = 16;
            this.lblNameFull.Text = "Group_Name.Powerset_Name.Power_Name";
            this.lblNameFull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbNameSet.FormattingEnabled = true;

            this.cbNameSet.Location = new System.Drawing.Point(106, 70);
            this.cbNameSet.Name = "cbNameSet";

            this.cbNameSet.Size = new System.Drawing.Size(196, 22);
            this.cbNameSet.TabIndex = 2;
            this.cbNameSet.TextChanged += this.cbNameSet_TextChanged;
            this.cbNameSet.SelectedIndexChanged += this.cbNameSet_SelectedIndexChanged;
            this.cbNameSet.Leave += this.cbNameSet_Leave;

            this.txtNameDisplay.Location = new System.Drawing.Point(106, 16);
            this.txtNameDisplay.Name = "txtNameDisplay";

            this.txtNameDisplay.Size = new System.Drawing.Size(196, 20);
            this.txtNameDisplay.TabIndex = 0;
            this.txtNameDisplay.Text = "PowerName";
            this.txtNameDisplay.TextChanged += new System.EventHandler(txtPowerName_TextChanged);
            this.cbNameGroup.FormattingEnabled = true;
            this.cbNameGroup.Location = new System.Drawing.Point(106, 42);
            this.cbNameGroup.Name = "cbNameGroup";
            this.cbNameGroup.Size = new System.Drawing.Size(196, 22);
            this.cbNameGroup.TabIndex = 1;
            this.cbNameGroup.TextChanged += this.cbNameGroup_TextChanged;
            this.cbNameGroup.SelectedIndexChanged += this.cbNameGroup_SelectedIndexChanged;
            this.cbNameGroup.Leave += this.cbNameGroup_Leave;

            this.Label22.Location = new System.Drawing.Point(53, 42);
            this.Label22.Name = "Label22";

            this.Label22.Size = new System.Drawing.Size(49, 20);
            this.Label22.TabIndex = 10;
            this.Label22.Text = "Group:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label31.Location = new System.Drawing.Point(65, 70);
            this.Label31.Name = "Label31";

            this.Label31.Size = new System.Drawing.Size(35, 20);
            this.Label31.TabIndex = 12;
            this.Label31.Text = "Set:";
            this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNamePower.Location = new System.Drawing.Point(106, 98);
            this.txtNamePower.Name = "txtNamePower";

            this.txtNamePower.Size = new System.Drawing.Size(196, 20);
            this.txtNamePower.TabIndex = 3;
            this.txtNamePower.Text = "PowerName";
            this.txtNamePower.TextChanged += this.txtNamePower_TextChanged;
            this.txtNamePower.Leave += this.txtNamePower_Leave;

            this.Label33.Location = new System.Drawing.Point(14, 98);
            this.Label33.Name = "Label33";

            this.Label33.Size = new System.Drawing.Size(86, 20);
            this.Label33.TabIndex = 14;
            this.Label33.Text = "Power Name:";
            this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label46);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label47);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtLifeTimeReal);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label44);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label45);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtLifeTimeGame);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label42);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label43);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtUseageTime);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label41);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtNumCharges);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.chkIgnoreStrength);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label12);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label17);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtRangeSec);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label18);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.GroupBox9);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.cbNotify);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.chkLos);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtMaxTargets);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.lblEndCost);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label20);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.lblAcc);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label40);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label39);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label38);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label37);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label36);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label35);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label34);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label16);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtArc);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label15);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtRadius);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtLevel);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.cbEffectArea);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label14);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label13);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtEndCost);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label10);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtActivate);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label11);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtRechargeTime);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label8);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtCastTime);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label9);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtInterrupt);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label7);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtRange);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label6);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.txtAcc);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.cbPowerType);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.tpBasic.Controls.Add((System.Windows.Forms.Control)this.Label4);

            this.tpBasic.Location = new System.Drawing.Point(4, 23);
            this.tpBasic.Name = "tpBasic";

            this.tpBasic.Size = new System.Drawing.Size(832, 337);
            this.tpBasic.TabIndex = 0;
            this.tpBasic.Text = "Power Attributes";
            this.tpBasic.UseVisualStyleBackColor = true;
            this.tpBasic.Visible = false;

            this.Label46.Location = new System.Drawing.Point(447, 219);
            this.Label46.Name = "Label46";

            this.Label46.Size = new System.Drawing.Size(20, 20);
            this.Label46.TabIndex = 118;
            this.Label46.Text = "s";
            this.Label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label47.Location = new System.Drawing.Point(256, 219);
            this.Label47.Name = "Label47";

            this.Label47.Size = new System.Drawing.Size(124, 20);
            this.Label47.TabIndex = 117;
            this.Label47.Text = "Life Time Real-World:";
            this.Label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtLifeTimeReal.Location = new System.Drawing.Point(384, 219);
            this.txtLifeTimeReal.Name = "txtLifeTimeReal";

            this.txtLifeTimeReal.Size = new System.Drawing.Size(57, 20);
            this.txtLifeTimeReal.TabIndex = 116;
            this.txtLifeTimeReal.Text = "1";
            this.txtLifeTimeReal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLifeTimeReal.TextChanged += this.txtLifeTimeReal_TextChanged;
            this.txtLifeTimeReal.Leave += new System.EventHandler(this.txtLifeTimeReal_Leave);

            this.Label44.Location = new System.Drawing.Point(447, 193);
            this.Label44.Name = "Label44";

            this.Label44.Size = new System.Drawing.Size(20, 20);
            this.Label44.TabIndex = 115;
            this.Label44.Text = "s";
            this.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label45.Location = new System.Drawing.Point(256, 193);
            this.Label45.Name = "Label45";

            this.Label45.Size = new System.Drawing.Size(124, 20);
            this.Label45.TabIndex = 114;
            this.Label45.Text = "Life Time In-Game:";
            this.Label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtLifeTimeGame.Location = new System.Drawing.Point(384, 193);
            this.txtLifeTimeGame.Name = "txtLifeTimeGame";

            this.txtLifeTimeGame.Size = new System.Drawing.Size(57, 20);
            this.txtLifeTimeGame.TabIndex = 113;
            this.txtLifeTimeGame.Text = "1";
            this.txtLifeTimeGame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.txtLifeTimeGame.TextChanged += this.txtLifeTimeGame_TextChanged;
            this.txtLifeTimeGame.Leave += new System.EventHandler(this.txtLifeTimeGame_Leave);

            this.Label42.Location = new System.Drawing.Point(447, 167);
            this.Label42.Name = "Label42";

            this.Label42.Size = new System.Drawing.Size(20, 20);
            this.Label42.TabIndex = 112;
            this.Label42.Text = "s";
            this.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label43.Location = new System.Drawing.Point(256, 167);
            this.Label43.Name = "Label43";

            this.Label43.Size = new System.Drawing.Size(124, 20);
            this.Label43.TabIndex = 111;
            this.Label43.Text = "Usage Time:";
            this.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtUseageTime.Location = new System.Drawing.Point(384, 167);
            this.txtUseageTime.Name = "txtUseageTime";

            this.txtUseageTime.Size = new System.Drawing.Size(57, 20);
            this.txtUseageTime.TabIndex = 110;
            this.txtUseageTime.Text = "1";
            this.txtUseageTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtUseageTime.TextChanged += this.txtUseageTime_TextChanged;
            this.txtUseageTime.Leave += new System.EventHandler(this.txtUseageTime_Leave);

            this.Label41.Location = new System.Drawing.Point(256, 141);
            this.Label41.Name = "Label41";

            this.Label41.Size = new System.Drawing.Size(124, 20);
            this.Label41.TabIndex = 108;
            this.Label41.Text = "Charge Count:";
            this.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNumCharges.Location = new System.Drawing.Point(384, 141);
            this.txtNumCharges.Name = "txtNumCharges";

            this.txtNumCharges.Size = new System.Drawing.Size(57, 20);
            this.txtNumCharges.TabIndex = 107;
            this.txtNumCharges.Text = "1";
            this.txtNumCharges.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNumCharges.TextChanged += this.txtNumCharges_TextChanged;
            this.txtNumCharges.Leave += this.txtNumCharges_Leave;
            this.chkIgnoreStrength.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIgnoreStrength.Checked = true;
            this.chkIgnoreStrength.CheckState = System.Windows.Forms.CheckState.Checked;

            this.chkIgnoreStrength.Location = new System.Drawing.Point(256, 116);
            this.chkIgnoreStrength.Name = "chkIgnoreStrength";

            this.chkIgnoreStrength.Size = new System.Drawing.Size(223, 20);
            this.chkIgnoreStrength.TabIndex = 16;
            this.chkIgnoreStrength.Text = "Ignore Strength";
            this.chkIgnoreStrength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIgnoreStrength.CheckedChanged += new System.EventHandler(chkIgnoreStrength_CheckedChanged);

            this.Label12.Location = new System.Drawing.Point(180, 219);
            this.Label12.Name = "Label12";

            this.Label12.Size = new System.Drawing.Size(20, 20);
            this.Label12.TabIndex = 106;
            this.Label12.Text = "ft";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label17.Location = new System.Drawing.Point(9, 219);
            this.Label17.Name = "Label17";

            this.Label17.Size = new System.Drawing.Size(104, 20);
            this.Label17.TabIndex = 105;
            this.Label17.Text = "Secondary  Range:";
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtRangeSec.Location = new System.Drawing.Point(117, 219);
            this.txtRangeSec.Name = "txtRangeSec";

            this.txtRangeSec.Size = new System.Drawing.Size(57, 20);
            this.txtRangeSec.TabIndex = 8;
            this.txtRangeSec.Text = "1.0";
            this.txtRangeSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRangeSec.TextChanged += this.txtRangeSec_TextChanged;
            this.txtRangeSec.Leave += this.txtRangeSec_Leave;

            this.Label18.Location = new System.Drawing.Point(256, 63);
            this.Label18.Name = "Label18";

            this.Label18.Size = new System.Drawing.Size(122, 20);
            this.Label18.TabIndex = 33;
            this.Label18.Text = "Notify Mobs:";
            this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagCastThrough);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagDisallow);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagRequired);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagVector);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagCast);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.clbFlags);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagTargetsSec);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagTargets);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagAffected);
            this.GroupBox9.Controls.Add((System.Windows.Forms.Control)this.rbFlagAutoHit);

            this.GroupBox9.Location = new System.Drawing.Point(521, 8);
            this.GroupBox9.Name = "GroupBox9";

            this.GroupBox9.Size = new System.Drawing.Size(300, 320);
            this.GroupBox9.TabIndex = 103;
            this.GroupBox9.TabStop = false;
            this.GroupBox9.Text = "Adv. Attributes";

            this.rbFlagCastThrough.Location = new System.Drawing.Point(152, 111);
            this.rbFlagCastThrough.Name = "rbFlagCastThrough";

            this.rbFlagCastThrough.Size = new System.Drawing.Size(140, 17);
            this.rbFlagCastThrough.TabIndex = 9;
            this.rbFlagCastThrough.TabStop = true;
            this.rbFlagCastThrough.Text = "Cast Through...";
            this.rbFlagCastThrough.UseVisualStyleBackColor = true;
            this.rbFlagCastThrough.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);

            this.rbFlagDisallow.Location = new System.Drawing.Point(152, 88);
            this.rbFlagDisallow.Name = "rbFlagDisallow";

            this.rbFlagDisallow.Size = new System.Drawing.Size(140, 17);
            this.rbFlagDisallow.TabIndex = 7;
            this.rbFlagDisallow.TabStop = true;
            this.rbFlagDisallow.Text = "Modes Disallowed";
            this.rbFlagDisallow.UseVisualStyleBackColor = true;
            this.rbFlagDisallow.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.rbFlagRequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.rbFlagRequired.Location = new System.Drawing.Point(6, 88);
            this.rbFlagRequired.Name = "rbFlagRequired";

            this.rbFlagRequired.Size = new System.Drawing.Size(140, 17);
            this.rbFlagRequired.TabIndex = 6;
            this.rbFlagRequired.TabStop = true;
            this.rbFlagRequired.Text = "Modes Required";
            this.rbFlagRequired.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbFlagRequired.UseVisualStyleBackColor = true;
            this.rbFlagRequired.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);

            this.rbFlagVector.Location = new System.Drawing.Point(152, 65);
            this.rbFlagVector.Name = "rbFlagVector";

            this.rbFlagVector.Size = new System.Drawing.Size(140, 17);
            this.rbFlagVector.TabIndex = 5;
            this.rbFlagVector.TabStop = true;
            this.rbFlagVector.Text = "Vector / Damage Types";
            this.rbFlagVector.UseVisualStyleBackColor = true;
            this.rbFlagVector.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.rbFlagCast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.rbFlagCast.Location = new System.Drawing.Point(6, 65);
            this.rbFlagCast.Name = "rbFlagCast";

            this.rbFlagCast.Size = new System.Drawing.Size(140, 17);
            this.rbFlagCast.TabIndex = 4;
            this.rbFlagCast.TabStop = true;
            this.rbFlagCast.Text = "Cast Flags";
            this.rbFlagCast.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbFlagCast.UseVisualStyleBackColor = true;
            this.rbFlagCast.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.clbFlags.FormattingEnabled = true;

            this.clbFlags.Location = new System.Drawing.Point(6, 130);
            this.clbFlags.Name = "clbFlags";

            this.clbFlags.Size = new System.Drawing.Size(288, 184);
            this.clbFlags.TabIndex = 10;
            this.clbFlags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(clbFlags_ItemCheck);

            this.rbFlagTargetsSec.Location = new System.Drawing.Point(152, 42);
            this.rbFlagTargetsSec.Name = "rbFlagTargetsSec";

            this.rbFlagTargetsSec.Size = new System.Drawing.Size(140, 17);
            this.rbFlagTargetsSec.TabIndex = 3;
            this.rbFlagTargetsSec.TabStop = true;
            this.rbFlagTargetsSec.Text = "Secondary Targets";
            this.rbFlagTargetsSec.UseVisualStyleBackColor = true;
            this.rbFlagTargetsSec.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.rbFlagTargets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.rbFlagTargets.Location = new System.Drawing.Point(6, 42);
            this.rbFlagTargets.Name = "rbFlagTargets";

            this.rbFlagTargets.Size = new System.Drawing.Size(140, 17);
            this.rbFlagTargets.TabIndex = 2;
            this.rbFlagTargets.TabStop = true;
            this.rbFlagTargets.Text = "Targets";
            this.rbFlagTargets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbFlagTargets.UseVisualStyleBackColor = true;
            this.rbFlagTargets.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);

            this.rbFlagAffected.Location = new System.Drawing.Point(152, 19);
            this.rbFlagAffected.Name = "rbFlagAffected";

            this.rbFlagAffected.Size = new System.Drawing.Size(140, 17);
            this.rbFlagAffected.TabIndex = 1;
            this.rbFlagAffected.TabStop = true;
            this.rbFlagAffected.Text = "Entities Affected";
            this.rbFlagAffected.UseVisualStyleBackColor = true;
            this.rbFlagAffected.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.rbFlagAutoHit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.rbFlagAutoHit.Location = new System.Drawing.Point(6, 19);
            this.rbFlagAutoHit.Name = "rbFlagAutoHit";

            this.rbFlagAutoHit.Size = new System.Drawing.Size(140, 17);
            this.rbFlagAutoHit.TabIndex = 0;
            this.rbFlagAutoHit.TabStop = true;
            this.rbFlagAutoHit.Text = "Entities AutoHit";
            this.rbFlagAutoHit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbFlagAutoHit.UseVisualStyleBackColor = true;
            this.rbFlagAutoHit.CheckedChanged += new System.EventHandler(rbFlagX_CheckedChanged);
            this.cbNotify.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbNotify.Location = new System.Drawing.Point(384, 63);
            this.cbNotify.Name = "cbNotify";

            this.cbNotify.Size = new System.Drawing.Size(95, 22);
            this.cbNotify.TabIndex = 14;
            this.cbNotify.SelectedIndexChanged += new System.EventHandler(cbNotify_SelectedIndexChanged);
            this.chkLos.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLos.Checked = true;
            this.chkLos.CheckState = System.Windows.Forms.CheckState.Checked;

            this.chkLos.Location = new System.Drawing.Point(256, 90);
            this.chkLos.Name = "chkLos";

            this.chkLos.Size = new System.Drawing.Size(223, 20);
            this.chkLos.TabIndex = 15;
            this.chkLos.Text = "Requires Line of Sight";
            this.chkLos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLos.CheckedChanged += new System.EventHandler(chkLos_CheckedChanged);

            this.txtMaxTargets.Location = new System.Drawing.Point(117, 295);
            this.txtMaxTargets.Name = "txtMaxTargets";

            this.txtMaxTargets.Size = new System.Drawing.Size(57, 20);
            this.txtMaxTargets.TabIndex = 11;
            this.txtMaxTargets.Text = "1";
            this.txtMaxTargets.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxTargets.Leave += new System.EventHandler(this.txtMaxTargets_Leave);
            this.txtMaxTargets.TextChanged += this.txtMaxTargets_TextChanged;

            this.lblEndCost.Location = new System.Drawing.Point(180, 167);
            this.lblEndCost.Name = "lblEndCost";

            this.lblEndCost.Size = new System.Drawing.Size(50, 20);
            this.lblEndCost.TabIndex = 101;
            this.lblEndCost.Text = "(1.05/s)";
            this.lblEndCost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label20.Location = new System.Drawing.Point(12, 295);
            this.Label20.Name = "Label20";

            this.Label20.Size = new System.Drawing.Size(101, 20);
            this.Label20.TabIndex = 39;
            this.Label20.Text = "Max Targets:";
            this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.lblAcc.Location = new System.Drawing.Point(180, 37);
            this.lblAcc.Name = "lblAcc";

            this.lblAcc.Size = new System.Drawing.Size(50, 20);
            this.lblAcc.TabIndex = 100;
            this.lblAcc.Text = "(75%)";
            this.lblAcc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label40.Location = new System.Drawing.Point(180, 269);
            this.Label40.Name = "Label40";

            this.Label40.Size = new System.Drawing.Size(30, 20);
            this.Label40.TabIndex = 99;
            this.Label40.Text = "deg.";
            this.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label39.Location = new System.Drawing.Point(180, 245);
            this.Label39.Name = "Label39";

            this.Label39.Size = new System.Drawing.Size(30, 20);
            this.Label39.TabIndex = 98;
            this.Label39.Text = "ft";
            this.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label38.Location = new System.Drawing.Point(180, 193);
            this.Label38.Name = "Label38";

            this.Label38.Size = new System.Drawing.Size(20, 20);
            this.Label38.TabIndex = 97;
            this.Label38.Text = "ft";
            this.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label37.Location = new System.Drawing.Point(180, 141);
            this.Label37.Name = "Label37";

            this.Label37.Size = new System.Drawing.Size(20, 20);
            this.Label37.TabIndex = 96;
            this.Label37.Text = "s";
            this.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label36.Location = new System.Drawing.Point(180, 115);
            this.Label36.Name = "Label36";

            this.Label36.Size = new System.Drawing.Size(20, 20);
            this.Label36.TabIndex = 95;
            this.Label36.Text = "s";
            this.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label35.Location = new System.Drawing.Point(180, 89);
            this.Label35.Name = "Label35";

            this.Label35.Size = new System.Drawing.Size(20, 20);
            this.Label35.TabIndex = 94;
            this.Label35.Text = "s";
            this.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label34.Location = new System.Drawing.Point(180, 63);
            this.Label34.Name = "Label34";

            this.Label34.Size = new System.Drawing.Size(20, 20);
            this.Label34.TabIndex = 93;
            this.Label34.Text = "s";
            this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.Label16.Location = new System.Drawing.Point(35, 269);
            this.Label16.Name = "Label16";

            this.Label16.Size = new System.Drawing.Size(78, 20);
            this.Label16.TabIndex = 30;
            this.Label16.Text = "Arc:";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtArc.Location = new System.Drawing.Point(117, 269);
            this.txtArc.Name = "txtArc";

            this.txtArc.Size = new System.Drawing.Size(57, 20);
            this.txtArc.TabIndex = 10;
            this.txtArc.Text = "1";
            this.txtArc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtArc.Leave += new System.EventHandler(this.txtArc_Leave);
            this.txtArc.TextChanged += this.txtArc_TextChanged;

            this.Label15.Location = new System.Drawing.Point(35, 245);
            this.Label15.Name = "Label15";

            this.Label15.Size = new System.Drawing.Size(78, 20);
            this.Label15.TabIndex = 28;
            this.Label15.Text = "Radius:";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtRadius.Location = new System.Drawing.Point(117, 245);
            this.txtRadius.Name = "txtRadius";

            this.txtRadius.Size = new System.Drawing.Size(57, 20);
            this.txtRadius.TabIndex = 9;
            this.txtRadius.Text = "1";
            this.txtRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRadius.Leave += new System.EventHandler(this.txtRadius_Leave);
            this.txtRadius.TextChanged += this.txtRadius_TextChanged;

            this.txtLevel.Location = new System.Drawing.Point(117, 11);
            this.txtLevel.Name = "txtLevel";

            this.txtLevel.Size = new System.Drawing.Size(57, 20);
            this.txtLevel.TabIndex = 0;
            this.txtLevel.Text = "1";
            this.txtLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLevel.Leave += new System.EventHandler(this.txtLevel_Leave);
            this.txtLevel.TextChanged += this.txtLevel_TextChanged;
            this.cbEffectArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEffectArea.Items.AddRange(new object[5]
            {
         "None",
         "Character",
         "Sphere",
         "Cone",
         "Location"
            });

            this.cbEffectArea.Location = new System.Drawing.Point(384, 37);
            this.cbEffectArea.Name = "cbEffectArea";

            this.cbEffectArea.Size = new System.Drawing.Size(95, 22);
            this.cbEffectArea.TabIndex = 13;
            this.cbEffectArea.SelectedIndexChanged += new System.EventHandler(cbEffectArea_SelectedIndexChanged);

            this.Label14.Location = new System.Drawing.Point(256, 37);
            this.Label14.Name = "Label14";

            this.Label14.Size = new System.Drawing.Size(122, 20);
            this.Label14.TabIndex = 24;
            this.Label14.Text = "Effect Area:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label13.Location = new System.Drawing.Point(9, 167);
            this.Label13.Name = "Label13";

            this.Label13.Size = new System.Drawing.Size(104, 20);
            this.Label13.TabIndex = 19;
            this.Label13.Text = "Endurance Cost:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtEndCost.Location = new System.Drawing.Point(117, 167);
            this.txtEndCost.Name = "txtEndCost";

            this.txtEndCost.Size = new System.Drawing.Size(57, 20);
            this.txtEndCost.TabIndex = 6;
            this.txtEndCost.Text = "1.0";
            this.txtEndCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtEndCost.Leave += this.txtEndCost_Leave;
            this.txtEndCost.TextChanged += this.txtEndCost_TextChanged;


            this.Label10.Location = new System.Drawing.Point(9, 141);
            this.Label10.Name = "Label10";

            this.Label10.Size = new System.Drawing.Size(104, 20);
            this.Label10.TabIndex = 17;
            this.Label10.Text = "Activate Interval:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtActivate.Location = new System.Drawing.Point(117, 141);
            this.txtActivate.Name = "txtActivate";

            this.txtActivate.Size = new System.Drawing.Size(57, 20);
            this.txtActivate.TabIndex = 5;
            this.txtActivate.Text = "1.0";
            this.txtActivate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtActivate.Leave += new System.EventHandler(this.txtActivate_Leave);
            this.txtActivate.TextChanged += this.txtActivate_TextChanged;

            this.Label11.Location = new System.Drawing.Point(9, 115);
            this.Label11.Name = "Label11";

            this.Label11.Size = new System.Drawing.Size(104, 20);
            this.Label11.TabIndex = 15;
            this.Label11.Text = "Recharge Time:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtRechargeTime.Location = new System.Drawing.Point(117, 115);
            this.txtRechargeTime.Name = "txtRechargeTime";

            this.txtRechargeTime.Size = new System.Drawing.Size(57, 20);
            this.txtRechargeTime.TabIndex = 4;
            this.txtRechargeTime.Text = "1.0";
            this.txtRechargeTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRechargeTime.Leave += new System.EventHandler(this.txtRechargeTime_Leave);
            this.txtRechargeTime.TextChanged += this.txtRechargeTime_TextChanged;

            this.Label8.Location = new System.Drawing.Point(9, 89);
            this.Label8.Name = "Label8";

            this.Label8.Size = new System.Drawing.Size(104, 20);
            this.Label8.TabIndex = 13;
            this.Label8.Text = "Casting Time:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtCastTime.Location = new System.Drawing.Point(117, 89);
            this.txtCastTime.Name = "txtCastTime";
            this.txtCastTime.Size = new System.Drawing.Size(57, 20);
            this.txtCastTime.TabIndex = 3;
            this.txtCastTime.Text = "1.0";
            this.txtCastTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCastTime.Leave += new System.EventHandler(this.txtCastTime_Leave);
            this.txtCastTime.TextChanged += this.txtCastTime_TextChanged;

            this.Label9.Location = new System.Drawing.Point(9, 63);
            this.Label9.Name = "Label9";

            this.Label9.Size = new System.Drawing.Size(104, 20);
            this.Label9.TabIndex = 11;
            this.Label9.Text = "Interruptable Time:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtInterrupt.Location = new System.Drawing.Point(117, 63);
            this.txtInterrupt.Name = "txtInterrupt";

            this.txtInterrupt.Size = new System.Drawing.Size(57, 20);
            this.txtInterrupt.TabIndex = 2;
            this.txtInterrupt.Text = "1.0";
            this.txtInterrupt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInterrupt.Leave += this.txtInterrupt_Leave;
            this.txtInterrupt.TextChanged += this.txtInterrupt_TextChanged;

            this.Label7.Location = new System.Drawing.Point(9, 193);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(104, 20);
            this.Label7.TabIndex = 9;
            this.Label7.Text = "Range:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtRange.Location = new System.Drawing.Point(117, 193);
            this.txtRange.Name = "txtRange";

            this.txtRange.Size = new System.Drawing.Size(57, 20);
            this.txtRange.TabIndex = 7;
            this.txtRange.Text = "1.0";
            this.txtRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRange.Leave += this.txtRange_Leave;
            this.txtRange.TextChanged += this.txtRange_TextChanged;

            this.Label6.Location = new System.Drawing.Point(9, 37);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(104, 20);
            this.Label6.TabIndex = 7;
            this.Label6.Text = "Accuracy:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtAcc.Location = new System.Drawing.Point(117, 37);
            this.txtAcc.Name = "txtAcc";

            this.txtAcc.Size = new System.Drawing.Size(57, 20);
            this.txtAcc.TabIndex = 1;
            this.txtAcc.Text = "1.0";
            this.txtAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAcc.Leave += new System.EventHandler(this.txtAcc_Leave);
            this.txtAcc.TextChanged += this.txtAcc_TextChanged;
            this.cbPowerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPowerType.Items.AddRange(new object[3]
            {
         "Click",
         "Passive",
         "Toggle"
            });

            this.cbPowerType.Location = new System.Drawing.Point(384, 11);
            this.cbPowerType.Name = "cbPowerType";

            this.cbPowerType.Size = new System.Drawing.Size(95, 22);
            this.cbPowerType.TabIndex = 12;
            this.cbPowerType.SelectedIndexChanged += new System.EventHandler(cbPowerType_SelectedIndexChanged);

            this.Label5.Location = new System.Drawing.Point(256, 11);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(122, 20);
            this.Label5.TabIndex = 4;
            this.Label5.Text = "Power Type:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label4.Location = new System.Drawing.Point(9, 11);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(104, 20);
            this.Label4.TabIndex = 2;
            this.Label4.Text = "Level Available:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tpEffects.Controls.Add((System.Windows.Forms.Control)this.lvFX);
            this.tpEffects.Controls.Add((System.Windows.Forms.Control)this.pnlFX);

            this.tpEffects.Location = new System.Drawing.Point(4, 23);
            this.tpEffects.Name = "tpEffects";

            this.tpEffects.Size = new System.Drawing.Size(832, 337);
            this.tpEffects.TabIndex = 1;
            this.tpEffects.Text = "Effects";
            this.tpEffects.UseVisualStyleBackColor = true;
            this.tpEffects.Visible = false;
            this.lvFX.ItemHeight = 14;

            this.lvFX.Location = new System.Drawing.Point(8, 8);
            this.lvFX.Name = "lvFX";

            this.lvFX.Size = new System.Drawing.Size(744, 270);
            this.lvFX.TabIndex = 71;
            this.lvFX.DoubleClick += new System.EventHandler(this.lvFX_DoubleClick);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnSetDamage);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXEdit);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXDown);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXUp);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXRemove);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXDuplicate);
            this.pnlFX.Controls.Add((System.Windows.Forms.Control)this.btnFXAdd);

            this.pnlFX.Location = new System.Drawing.Point(4, 4);
            this.pnlFX.Name = "pnlFX";

            this.pnlFX.Size = new System.Drawing.Size(824, 332);
            this.pnlFX.TabIndex = 71;

            this.btnSetDamage.Location = new System.Drawing.Point(312, 288);
            this.btnSetDamage.Name = "btnSetDamage";

            this.btnSetDamage.Size = new System.Drawing.Size(212, 24);
            this.btnSetDamage.TabIndex = 78;
            this.btnSetDamage.Text = "Set damage types from effect list";

            this.btnFXEdit.Location = new System.Drawing.Point(160, 288);
            this.btnFXEdit.Name = "btnFXEdit";

            this.btnFXEdit.Size = new System.Drawing.Size(64, 24);
            this.btnFXEdit.TabIndex = 77;
            this.btnFXEdit.Text = "Edit...";
            this.btnFXEdit.Click += new System.EventHandler(btnFXEdit_Click);

            this.btnFXDown.Location = new System.Drawing.Point(756, 40);
            this.btnFXDown.Name = "btnFXDown";

            this.btnFXDown.Size = new System.Drawing.Size(64, 24);
            this.btnFXDown.TabIndex = 11;
            this.btnFXDown.Text = "Down";
            this.btnFXDown.Click += new System.EventHandler(btnFXDown_Click);

            this.btnFXUp.Location = new System.Drawing.Point(756, 8);
            this.btnFXUp.Name = "btnFXUp";

            this.btnFXUp.Size = new System.Drawing.Size(64, 24);
            this.btnFXUp.TabIndex = 12;
            this.btnFXUp.Text = "Up";
            this.btnFXUp.Click += new System.EventHandler(btnFXUp_Click);

            this.btnFXRemove.Location = new System.Drawing.Point(236, 288);
            this.btnFXRemove.Name = "btnFXRemove";

            this.btnFXRemove.Size = new System.Drawing.Size(64, 24);
            this.btnFXRemove.TabIndex = 10;
            this.btnFXRemove.Text = "Remove";
            this.btnFXRemove.Click += new System.EventHandler(btnFXRemove_Click);

            this.btnFXDuplicate.Location = new System.Drawing.Point(84, 288);
            this.btnFXDuplicate.Name = "btnFXDuplicate";

            this.btnFXDuplicate.Size = new System.Drawing.Size(64, 24);
            this.btnFXDuplicate.TabIndex = 69;
            this.btnFXDuplicate.Text = "Duplicate";
            this.btnFXDuplicate.Click += new System.EventHandler(btnFXDuplicate_Click);

            this.btnFXAdd.Location = new System.Drawing.Point(8, 288);
            this.btnFXAdd.Name = "btnFXAdd";

            this.btnFXAdd.Size = new System.Drawing.Size(64, 24);
            this.btnFXAdd.TabIndex = 9;
            this.btnFXAdd.Text = "Add";
            this.btnFXAdd.Click += new System.EventHandler(btnFXAdd_Click);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.chkBoostUsePlayerLevel);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.chkBoostBoostable);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.Label23);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.pbEnhancements);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.chkPRFrontLoad);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.pbEnhancementList);
            this.tpEnh.Controls.Add((System.Windows.Forms.Control)this.lblEnhName);

            this.tpEnh.Location = new System.Drawing.Point(4, 23);
            this.tpEnh.Name = "tpEnh";

            this.tpEnh.Size = new System.Drawing.Size(832, 337);
            this.tpEnh.TabIndex = 9;
            this.tpEnh.Text = "Enhancements";
            this.tpEnh.UseVisualStyleBackColor = true;

            this.chkBoostUsePlayerLevel.Location = new System.Drawing.Point(357, 136);
            this.chkBoostUsePlayerLevel.Name = "chkBoostUsePlayerLevel";

            this.chkBoostUsePlayerLevel.Size = new System.Drawing.Size(324, 20);
            this.chkBoostUsePlayerLevel.TabIndex = 91;
            this.chkBoostUsePlayerLevel.Text = "Attuned Enhancement";
            this.chkBoostUsePlayerLevel.CheckedChanged += new System.EventHandler(chkBoostUsePlayerLevel_CheckedChanged);

            this.chkBoostBoostable.Location = new System.Drawing.Point(357, 110);
            this.chkBoostBoostable.Name = "chkBoostBoostable";

            this.chkBoostBoostable.Size = new System.Drawing.Size(324, 20);
            this.chkBoostBoostable.TabIndex = 90;
            this.chkBoostBoostable.Text = "Allow Enhancement Boosters (Enhancement only)";
            this.chkBoostBoostable.CheckedChanged += new System.EventHandler(chkBoostBoostable_CheckedChanged);

            this.Label23.Location = new System.Drawing.Point(24, 12);
            this.Label23.Name = "Label23";

            this.Label23.Size = new System.Drawing.Size(300, 16);
            this.Label23.TabIndex = 89;
            this.Label23.Text = "Enhancement Classes Accepted:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pbEnhancements.BackColor = System.Drawing.Color.FromArgb((int)byte.MaxValue, 128, 0);

            this.pbEnhancements.Location = new System.Drawing.Point(24, 28);
            this.pbEnhancements.Name = "pbEnhancements";

            this.pbEnhancements.Size = new System.Drawing.Size(780, 40);
            this.pbEnhancements.TabIndex = 43;
            this.pbEnhancements.TabStop = false;
            this.pbEnhancements.MouseDown += this.pbEnhancements_MouseDown;
            this.pbEnhancements.MouseMove += this.pbEnhancements_Hover;
            this.pbEnhancements.Paint += this.pbEnhancements_Paint;

            this.chkPRFrontLoad.Location = new System.Drawing.Point(357, 84);
            this.chkPRFrontLoad.Name = "chkPRFrontLoad";

            this.chkPRFrontLoad.Size = new System.Drawing.Size(324, 20);
            this.chkPRFrontLoad.TabIndex = 88;
            this.chkPRFrontLoad.Text = "Allow front-loading of enhancement slots (for Kheld forms)";
            this.chkPRFrontLoad.CheckedChanged += new System.EventHandler(chkPRFrontLoad_CheckedChanged);
            this.pbEnhancementList.BackColor = System.Drawing.Color.FromArgb((int)byte.MaxValue, 192, 128);
            this.pbEnhancementList.Location = new System.Drawing.Point(24, 84);
            this.pbEnhancementList.Name = "pbEnhancementList";
            this.pbEnhancementList.Size = new System.Drawing.Size(316, 220);
            this.pbEnhancementList.TabIndex = 44;
            this.pbEnhancementList.TabStop = false;
            this.pbEnhancementList.MouseDown += this.pbEnhancementList_MouseDown;
            this.pbEnhancementList.MouseMove += this.pbEnhancementList_Hover;
            this.pbEnhancementList.Paint += this.pbEnhancementList_Paint;
            this.lblEnhName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);

            this.lblEnhName.Location = new System.Drawing.Point(21, 68);
            this.lblEnhName.Name = "lblEnhName";

            this.lblEnhName.Size = new System.Drawing.Size(316, 16);
            this.lblEnhName.TabIndex = 46;
            this.tpSets.Controls.Add((System.Windows.Forms.Control)this.Label24);
            this.tpSets.Controls.Add((System.Windows.Forms.Control)this.lblInvSet);
            this.tpSets.Controls.Add((System.Windows.Forms.Control)this.pbInvSetList);
            this.tpSets.Controls.Add((System.Windows.Forms.Control)this.pbInvSetUsed);

            this.tpSets.Location = new System.Drawing.Point(4, 23);
            this.tpSets.Name = "tpSets";

            this.tpSets.Size = new System.Drawing.Size(832, 337);
            this.tpSets.TabIndex = 5;
            this.tpSets.Text = "Invention Set Types";
            this.tpSets.UseVisualStyleBackColor = true;

            this.Label24.Location = new System.Drawing.Point(24, 12);
            this.Label24.Name = "Label24";

            this.Label24.Size = new System.Drawing.Size(300, 16);
            this.Label24.TabIndex = 93;
            this.Label24.Text = "Invention Set Types Accepted:";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblInvSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);

            this.lblInvSet.Location = new System.Drawing.Point(24, 68);
            this.lblInvSet.Name = "lblInvSet";

            this.lblInvSet.Size = new System.Drawing.Size(316, 16);
            this.lblInvSet.TabIndex = 92;
            this.pbInvSetList.BackColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);

            this.pbInvSetList.Location = new System.Drawing.Point(24, 84);
            this.pbInvSetList.Name = "pbInvSetList";

            this.pbInvSetList.Size = new System.Drawing.Size(316, 220);
            this.pbInvSetList.TabIndex = 91;
            this.pbInvSetList.TabStop = false;
            this.pbInvSetList.MouseMove += this.pbInvSetList_MouseMove;
            this.pbInvSetList.MouseDown += this.pbInvSetList_MouseDown;
            this.pbInvSetList.Paint += this.pbInvSetList_Paint;
            this.pbInvSetUsed.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);

            this.pbInvSetUsed.Location = new System.Drawing.Point(24, 28);
            this.pbInvSetUsed.Name = "pbInvSetUsed";

            this.pbInvSetUsed.Size = new System.Drawing.Size(780, 40);
            this.pbInvSetUsed.TabIndex = 90;
            this.pbInvSetUsed.TabStop = false;
            this.pbInvSetUsed.MouseDown += this.pbInvSetUsed_MouseDown;
            this.pbInvSetUsed.MouseMove += this.pbInvSetUsed_MouseMove;
            this.pbInvSetUsed.Paint += this.pbInvSetUsed_Paint;

            this.tpPreReq.Controls.Add((System.Windows.Forms.Control)this.GroupBox11);
            this.tpPreReq.Controls.Add((System.Windows.Forms.Control)this.GroupBox10);
            this.tpPreReq.Controls.Add((System.Windows.Forms.Control)this.GroupBox8);

            this.tpPreReq.Location = new System.Drawing.Point(4, 23);
            this.tpPreReq.Name = "tpPreReq";

            this.tpPreReq.Size = new System.Drawing.Size(832, 337);
            this.tpPreReq.TabIndex = 4;
            this.tpPreReq.Text = "Requirements";
            this.tpPreReq.UseVisualStyleBackColor = true;
            this.tpPreReq.Visible = false;
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.btnPrReset);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.btnPrSetNone);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.btnPrDown);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.btnPrUp);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.rbPrRemove);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.rbPrAdd);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.rbPrPowerB);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.rbPrPowerA);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.lvPrPower);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.lvPrSet);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.lvPrGroup);
            this.GroupBox11.Controls.Add((System.Windows.Forms.Control)this.lvPrListing);

            this.GroupBox11.Location = new System.Drawing.Point(3, 14);
            this.GroupBox11.Name = "GroupBox11";

            this.GroupBox11.Size = new System.Drawing.Size(611, 320);
            this.GroupBox11.TabIndex = 97;
            this.GroupBox11.TabStop = false;
            this.GroupBox11.Text = "Required Powers";

            this.btnPrReset.Location = new System.Drawing.Point(432, 291);
            this.btnPrReset.Name = "btnPrReset";

            this.btnPrReset.Size = new System.Drawing.Size(156, 23);
            this.btnPrReset.TabIndex = 15;
            this.btnPrReset.Text = "Reset";
            this.btnPrReset.UseVisualStyleBackColor = true;
            this.btnPrReset.Click += new System.EventHandler(btnPrReset_Click);

            this.btnPrSetNone.Location = new System.Drawing.Point(432, 153);
            this.btnPrSetNone.Name = "btnPrSetNone";

            this.btnPrSetNone.Size = new System.Drawing.Size(156, 23);
            this.btnPrSetNone.TabIndex = 14;
            this.btnPrSetNone.Text = "Set Power A to None";
            this.btnPrSetNone.UseVisualStyleBackColor = true;
            this.btnPrSetNone.Click += new System.EventHandler(btnPrSetNone_Click);

            this.btnPrDown.Location = new System.Drawing.Point(513, 204);
            this.btnPrDown.Name = "btnPrDown";

            this.btnPrDown.Size = new System.Drawing.Size(75, 23);
            this.btnPrDown.TabIndex = 13;
            this.btnPrDown.Text = "Down";
            this.btnPrDown.UseVisualStyleBackColor = true;
            this.btnPrDown.Click += new System.EventHandler(btnPrDown_Click);

            this.btnPrUp.Location = new System.Drawing.Point(432, 204);
            this.btnPrUp.Name = "btnPrUp";

            this.btnPrUp.Size = new System.Drawing.Size(75, 23);
            this.btnPrUp.TabIndex = 12;
            this.btnPrUp.Text = "Up";
            this.btnPrUp.UseVisualStyleBackColor = true;
            this.btnPrUp.Click += new System.EventHandler(btnPrUp_Click);

            this.rbPrRemove.Location = new System.Drawing.Point(432, 262);
            this.rbPrRemove.Name = "rbPrRemove";

            this.rbPrRemove.Size = new System.Drawing.Size(156, 23);
            this.rbPrRemove.TabIndex = 11;
            this.rbPrRemove.Text = "Remove Selected";
            this.rbPrRemove.UseVisualStyleBackColor = true;
            this.rbPrRemove.Click += new System.EventHandler(rbPrRemove_Click);

            this.rbPrAdd.Location = new System.Drawing.Point(432, 233);
            this.rbPrAdd.Name = "rbPrAdd";

            this.rbPrAdd.Size = new System.Drawing.Size(156, 23);
            this.rbPrAdd.TabIndex = 10;
            this.rbPrAdd.Text = "Add New";
            this.rbPrAdd.UseVisualStyleBackColor = true;
            this.rbPrAdd.Click += new System.EventHandler(rbPrAdd_Click);
            this.rbPrPowerB.Appearance = System.Windows.Forms.Appearance.Button;

            this.rbPrPowerB.Location = new System.Drawing.Point(513, 124);
            this.rbPrPowerB.Name = "rbPrPowerB";

            this.rbPrPowerB.Size = new System.Drawing.Size(75, 23);
            this.rbPrPowerB.TabIndex = 9;
            this.rbPrPowerB.Text = "Power B";
            this.rbPrPowerB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPrPowerB.UseVisualStyleBackColor = true;
            this.rbPrPowerB.CheckedChanged += new System.EventHandler(rbPrPowerX_CheckedChanged);
            this.rbPrPowerA.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPrPowerA.Checked = true;

            this.rbPrPowerA.Location = new System.Drawing.Point(432, 124);
            this.rbPrPowerA.Name = "rbPrPowerA";

            this.rbPrPowerA.Size = new System.Drawing.Size(75, 23);
            this.rbPrPowerA.TabIndex = 8;
            this.rbPrPowerA.TabStop = true;
            this.rbPrPowerA.Text = "Power A";
            this.rbPrPowerA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPrPowerA.UseVisualStyleBackColor = true;
            this.rbPrPowerA.CheckedChanged += new System.EventHandler(rbPrPowerX_CheckedChanged);
            this.lvPrPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader9
            });
            this.lvPrPower.FullRowSelect = true;
            this.lvPrPower.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPrPower.HideSelection = false;

            this.lvPrPower.Location = new System.Drawing.Point(290, 108);
            this.lvPrPower.MultiSelect = false;
            this.lvPrPower.Name = "lvPrPower";

            this.lvPrPower.Size = new System.Drawing.Size(136, 206);
            this.lvPrPower.TabIndex = 3;
            this.lvPrPower.UseCompatibleStateImageBehavior = false;
            this.lvPrPower.View = System.Windows.Forms.View.Details;
            this.lvPrPower.SelectedIndexChanged += new System.EventHandler(lvPrPower_SelectedIndexChanged);
            this.ColumnHeader9.Text = "Power";
            this.ColumnHeader9.Width = 110;
            this.lvPrSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader10
            });
            this.lvPrSet.FullRowSelect = true;
            this.lvPrSet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPrSet.HideSelection = false;

            this.lvPrSet.Location = new System.Drawing.Point(148, 108);
            this.lvPrSet.MultiSelect = false;
            this.lvPrSet.Name = "lvPrSet";

            this.lvPrSet.Size = new System.Drawing.Size(136, 206);
            this.lvPrSet.TabIndex = 2;
            this.lvPrSet.UseCompatibleStateImageBehavior = false;
            this.lvPrSet.View = System.Windows.Forms.View.Details;
            this.lvPrSet.SelectedIndexChanged += new System.EventHandler(lvPrSet_SelectedIndexChanged);
            this.ColumnHeader10.Text = "Set";
            this.ColumnHeader10.Width = 110;
            this.lvPrGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader11
            });
            this.lvPrGroup.FullRowSelect = true;
            this.lvPrGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPrGroup.HideSelection = false;

            this.lvPrGroup.Location = new System.Drawing.Point(6, 108);
            this.lvPrGroup.MultiSelect = false;
            this.lvPrGroup.Name = "lvPrGroup";

            this.lvPrGroup.Size = new System.Drawing.Size(136, 206);
            this.lvPrGroup.TabIndex = 1;
            this.lvPrGroup.UseCompatibleStateImageBehavior = false;
            this.lvPrGroup.View = System.Windows.Forms.View.Details;
            this.lvPrGroup.SelectedIndexChanged += new System.EventHandler(lvPrGroup_SelectedIndexChanged);
            this.ColumnHeader11.Text = "Group";
            this.ColumnHeader11.Width = 110;
            this.lvPrListing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3]
            {
        this.ColumnHeader6,
        this.ColumnHeader7,
        this.ColumnHeader8
            });
            this.lvPrListing.FullRowSelect = true;
            this.lvPrListing.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPrListing.HideSelection = false;

            this.lvPrListing.Location = new System.Drawing.Point(6, 19);
            this.lvPrListing.MultiSelect = false;
            this.lvPrListing.Name = "lvPrListing";

            this.lvPrListing.Size = new System.Drawing.Size(599, 83);
            this.lvPrListing.TabIndex = 0;
            this.lvPrListing.UseCompatibleStateImageBehavior = false;
            this.lvPrListing.View = System.Windows.Forms.View.Details;
            this.lvPrListing.SelectedIndexChanged += new System.EventHandler(lvPrListing_SelectedIndexChanged);
            this.ColumnHeader6.Text = "Power A";
            this.ColumnHeader6.Width = 265;
            this.ColumnHeader7.Text = "";
            this.ColumnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColumnHeader7.Width = 40;
            this.ColumnHeader8.Text = "Power B";
            this.ColumnHeader8.Width = 265;
            this.GroupBox10.Controls.Add((System.Windows.Forms.Control)this.clbClassExclude);

            this.GroupBox10.Location = new System.Drawing.Point(620, 157);
            this.GroupBox10.Name = "GroupBox10";

            this.GroupBox10.Size = new System.Drawing.Size(207, 137);
            this.GroupBox10.TabIndex = 96;
            this.GroupBox10.TabStop = false;
            this.GroupBox10.Text = "Excluded Classes";
            this.clbClassExclude.FormattingEnabled = true;

            this.clbClassExclude.Location = new System.Drawing.Point(6, 19);
            this.clbClassExclude.Name = "clbClassExclude";

            this.clbClassExclude.Size = new System.Drawing.Size(192, 109);
            this.clbClassExclude.TabIndex = 0;
            this.GroupBox8.Controls.Add((System.Windows.Forms.Control)this.clbClassReq);

            this.GroupBox8.Location = new System.Drawing.Point(620, 14);
            this.GroupBox8.Name = "GroupBox8";

            this.GroupBox8.Size = new System.Drawing.Size(207, 137);
            this.GroupBox8.TabIndex = 95;
            this.GroupBox8.TabStop = false;
            this.GroupBox8.Text = "Reqired Classes";
            this.clbClassReq.FormattingEnabled = true;

            this.clbClassReq.Location = new System.Drawing.Point(6, 19);
            this.clbClassReq.Name = "clbClassReq";

            this.clbClassReq.Size = new System.Drawing.Size(192, 109);
            this.clbClassReq.TabIndex = 0;
            this.tpSpecialEnh.Controls.Add((System.Windows.Forms.Control)this.Label32);
            this.tpSpecialEnh.Controls.Add((System.Windows.Forms.Control)this.Label30);
            this.tpSpecialEnh.Controls.Add((System.Windows.Forms.Control)this.lvDisablePass4);
            this.tpSpecialEnh.Controls.Add((System.Windows.Forms.Control)this.lvDisablePass1);

            this.tpSpecialEnh.Location = new System.Drawing.Point(4, 23);
            this.tpSpecialEnh.Name = "tpSpecialEnh";
            this.tpSpecialEnh.Padding = new System.Windows.Forms.Padding(3);

            this.tpSpecialEnh.Size = new System.Drawing.Size(832, 337);
            this.tpSpecialEnh.TabIndex = 7;
            this.tpSpecialEnh.Text = "Enhancement Disabling";
            this.tpSpecialEnh.UseVisualStyleBackColor = true;

            this.Label32.Location = new System.Drawing.Point(246, 7);
            this.Label32.Name = "Label32";

            this.Label32.Size = new System.Drawing.Size(211, 34);
            this.Label32.TabIndex = 113;
            this.Label32.Text = "Pass Four\r\n(Buffs applied after ED)";

            this.Label30.Location = new System.Drawing.Point(7, 7);
            this.Label30.Name = "Label30";

            this.Label30.Size = new System.Drawing.Size(211, 34);
            this.Label30.TabIndex = 112;
            this.Label30.Text = "Pass One\r\n(Enhancements before ED is applied)";
            this.lvDisablePass4.ItemHeight = 14;

            this.lvDisablePass4.Location = new System.Drawing.Point(245, 44);
            this.lvDisablePass4.Name = "lvDisablePass4";
            this.lvDisablePass4.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;

            this.lvDisablePass4.Size = new System.Drawing.Size(212, 284);
            this.lvDisablePass4.TabIndex = 111;
            this.lvDisablePass4.SelectedIndexChanged += new System.EventHandler(lvDisablePass4_SelectedIndexChanged);
            this.lvDisablePass1.ItemHeight = 14;

            this.lvDisablePass1.Location = new System.Drawing.Point(6, 44);
            this.lvDisablePass1.Name = "lvDisablePass1";
            this.lvDisablePass1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;

            this.lvDisablePass1.Size = new System.Drawing.Size(212, 284);
            this.lvDisablePass1.TabIndex = 110;
            this.lvDisablePass1.SelectedIndexChanged += new System.EventHandler(lvDisablePass1_SelectedIndexChanged);
            this.tpMutex.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.tpMutex.Controls.Add((System.Windows.Forms.Control)this.chkMutexAuto);
            this.tpMutex.Controls.Add((System.Windows.Forms.Control)this.chkMutexSkip);

            this.tpMutex.Location = new System.Drawing.Point(4, 23);
            this.tpMutex.Name = "tpMutex";

            this.tpMutex.Size = new System.Drawing.Size(832, 337);
            this.tpMutex.TabIndex = 8;
            this.tpMutex.Text = "Mutal Exclusivity";
            this.tpMutex.UseVisualStyleBackColor = true;
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnMutexAdd);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.clbMutex);

            this.GroupBox2.Location = new System.Drawing.Point(9, 14);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(325, 312);
            this.GroupBox2.TabIndex = 5;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Group Membership";

            this.btnMutexAdd.Location = new System.Drawing.Point(6, 283);
            this.btnMutexAdd.Name = "btnMutexAdd";

            this.btnMutexAdd.Size = new System.Drawing.Size(124, 23);
            this.btnMutexAdd.TabIndex = 1;
            this.btnMutexAdd.Text = "Add New Group...";
            this.btnMutexAdd.UseVisualStyleBackColor = true;
            this.btnMutexAdd.Click += new System.EventHandler(btnMutexAdd_Click);
            this.clbMutex.FormattingEnabled = true;

            this.clbMutex.Location = new System.Drawing.Point(6, 19);
            this.clbMutex.Name = "clbMutex";

            this.clbMutex.Size = new System.Drawing.Size(313, 259);
            this.clbMutex.TabIndex = 0;

            this.chkMutexAuto.Location = new System.Drawing.Point(340, 57);
            this.chkMutexAuto.Name = "chkMutexAuto";

            this.chkMutexAuto.Size = new System.Drawing.Size(217, 18);
            this.chkMutexAuto.TabIndex = 4;
            this.chkMutexAuto.Text = "Auto-Detoggle other powers";
            this.chkMutexAuto.UseVisualStyleBackColor = true;
            this.chkMutexAuto.CheckedChanged += new System.EventHandler(chkMutexAuto_CheckedChanged);

            this.chkMutexSkip.Location = new System.Drawing.Point(340, 33);
            this.chkMutexSkip.Name = "chkMutexSkip";

            this.chkMutexSkip.Size = new System.Drawing.Size(217, 18);
            this.chkMutexSkip.TabIndex = 3;
            this.chkMutexSkip.Text = "Skip Mutal Exclusivity for this power";
            this.chkMutexSkip.UseVisualStyleBackColor = true;
            this.chkMutexSkip.CheckedChanged += new System.EventHandler(chkMutexSkip_CheckedChanged);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.btnSPAdd);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.btnSPRemove);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.lvSPSelected);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.lvSPPower);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.lvSPSet);
            this.tpSubPower.Controls.Add((System.Windows.Forms.Control)this.lvSPGroup);
            this.tpSubPower.Location = new System.Drawing.Point(4, 23);
            this.tpSubPower.Name = "tpSubPower";
            this.tpSubPower.Size = new System.Drawing.Size(832, 337);
            this.tpSubPower.TabIndex = 10;
            this.tpSubPower.Text = "Sub-Powers";
            this.tpSubPower.UseVisualStyleBackColor = true;
            this.btnSPAdd.Location = new System.Drawing.Point(437, 141);
            this.btnSPAdd.Name = "btnSPAdd";
            this.btnSPAdd.Size = new System.Drawing.Size(48, 23);
            this.btnSPAdd.TabIndex = 13;
            this.btnSPAdd.Text = ">>";
            this.btnSPAdd.UseVisualStyleBackColor = true;
            this.btnSPAdd.Click += new System.EventHandler(btnSPAdd_Click);
            this.btnSPRemove.Location = new System.Drawing.Point(437, 170);
            this.btnSPRemove.Name = "btnSPRemove";
            this.btnSPRemove.Size = new System.Drawing.Size(48, 23);
            this.btnSPRemove.TabIndex = 12;
            this.btnSPRemove.Text = "<<";
            this.btnSPRemove.UseVisualStyleBackColor = true;
            this.btnSPRemove.Click += new System.EventHandler(btnSPRemove_Click);
            this.lvSPSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
                this.ColumnHeader4
            });
            this.lvSPSelected.FullRowSelect = true;
            this.lvSPSelected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSPSelected.HideSelection = false;
            this.lvSPSelected.Location = new System.Drawing.Point(491, 16);
            this.lvSPSelected.MultiSelect = false;
            this.lvSPSelected.Name = "lvSPSelected";
            this.lvSPSelected.Size = new System.Drawing.Size(324, 305);
            this.lvSPSelected.TabIndex = 7;
            this.lvSPSelected.UseCompatibleStateImageBehavior = false;
            this.lvSPSelected.View = System.Windows.Forms.View.Details;
            this.ColumnHeader4.Text = "Power";
            this.ColumnHeader4.Width = 293;
            this.lvSPPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader1
            });
            this.lvSPPower.FullRowSelect = true;
            this.lvSPPower.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSPPower.HideSelection = false;
            this.lvSPPower.Location = new System.Drawing.Point(295, 16);
            this.lvSPPower.MultiSelect = false;
            this.lvSPPower.Name = "lvSPPower";
            this.lvSPPower.Size = new System.Drawing.Size(136, 305);
            this.lvSPPower.TabIndex = 6;
            this.lvSPPower.UseCompatibleStateImageBehavior = false;
            this.lvSPPower.View = System.Windows.Forms.View.Details;
            this.ColumnHeader1.Text = "Power";
            this.ColumnHeader1.Width = 110;
            this.lvSPSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader2
            });
            this.lvSPSet.FullRowSelect = true;
            this.lvSPSet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSPSet.HideSelection = false;
            this.lvSPSet.Location = new System.Drawing.Point(153, 16);
            this.lvSPSet.MultiSelect = false;
            this.lvSPSet.Name = "lvSPSet";
            this.lvSPSet.Size = new System.Drawing.Size(136, 305);
            this.lvSPSet.TabIndex = 5;
            this.lvSPSet.UseCompatibleStateImageBehavior = false;
            this.lvSPSet.View = System.Windows.Forms.View.Details;
            this.lvSPSet.SelectedIndexChanged += new System.EventHandler(lvSPSet_SelectedIndexChanged);
            this.ColumnHeader2.Text = "Set";
            this.ColumnHeader2.Width = 110;
            this.lvSPGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader3
            });
            this.lvSPGroup.FullRowSelect = true;
            this.lvSPGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSPGroup.HideSelection = false;
            this.lvSPGroup.Location = new System.Drawing.Point(11, 16);
            this.lvSPGroup.MultiSelect = false;
            this.lvSPGroup.Name = "lvSPGroup";
            this.lvSPGroup.Size = new System.Drawing.Size(136, 305);
            this.lvSPGroup.TabIndex = 4;
            this.lvSPGroup.UseCompatibleStateImageBehavior = false;
            this.lvSPGroup.View = System.Windows.Forms.View.Details;
            this.lvSPGroup.SelectedIndexChanged += new System.EventHandler(lvSPGroup_SelectedIndexChanged);
            this.ColumnHeader3.Text = "Group";
            this.ColumnHeader3.Width = 110;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(740, 380);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 36);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(632, 380);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 36);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
            this.btnFullPaste.Location = new System.Drawing.Point(434, 380);
            this.btnFullPaste.Name = "btnFullPaste";
            this.btnFullPaste.Size = new System.Drawing.Size(104, 36);
            this.btnFullPaste.TabIndex = 5;
            this.btnFullPaste.Text = "Paste";
            this.btnFullPaste.UseVisualStyleBackColor = true;
            this.btnFullPaste.Click += new System.EventHandler(btnFullPaste_Click);
            this.btnFullCopy.Location = new System.Drawing.Point(324, 380);
            this.btnFullCopy.Name = "btnFullCopy";
            this.btnFullCopy.Size = new System.Drawing.Size(104, 36);
            this.btnFullCopy.TabIndex = 6;
            this.btnFullCopy.Text = "Copy";
            this.btnFullCopy.UseVisualStyleBackColor = true;
            this.btnFullCopy.Click += new System.EventHandler(btnFullCopy_Click);
            this.btnCSVImport.Location = new System.Drawing.Point(8, 380);
            this.btnCSVImport.Name = "btnCSVImport";
            this.btnCSVImport.Size = new System.Drawing.Size(151, 36);
            this.btnCSVImport.TabIndex = 7;
            this.btnCSVImport.Text = "Import CSV String";
            this.btnCSVImport.UseVisualStyleBackColor = true;
            this.btnCSVImport.Click += new System.EventHandler(btnCSVImport_Click);
            this.AcceptButton = (System.Windows.Forms.IButtonControl)this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = (System.Windows.Forms.IButtonControl)this.btnCancel;
            this.ClientSize = new System.Drawing.Size(870, 428);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCSVImport);
            this.Controls.Add((System.Windows.Forms.Control)this.btnFullCopy);
            this.Controls.Add((System.Windows.Forms.Control)this.btnFullPaste);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control)this.tcPower);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Power (Group_Name.Set_Name.Power_Name)";
            this.tcPower.ResumeLayout(false);
            this.tpText.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.GroupBox7.ResumeLayout(false);
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.udScaleMax.EndInit();
            this.udScaleMin.EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.tpBasic.ResumeLayout(false);
            this.tpBasic.PerformLayout();
            this.GroupBox9.ResumeLayout(false);
            this.tpEffects.ResumeLayout(false);
            this.pnlFX.ResumeLayout(false);
            this.tpEnh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pbEnhancements).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pbEnhancementList).EndInit();
            this.tpSets.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pbInvSetList).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pbInvSetUsed).EndInit();
            this.tpPreReq.ResumeLayout(false);
            this.GroupBox11.ResumeLayout(false);
            this.GroupBox10.ResumeLayout(false);
            this.GroupBox8.ResumeLayout(false);
            this.tpSpecialEnh.ResumeLayout(false);
            this.tpMutex.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.tpSubPower.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        TextBox txtVisualLocation;
        ComboBox cbNameGroup;
        Button btnCancel;
        Button btnCSVImport;
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
        CheckBox chkAltSub;
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
    }
}
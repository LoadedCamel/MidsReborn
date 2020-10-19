using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmPowerEffect
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
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.chkStack = new System.Windows.Forms.CheckBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.clbSuppression = new System.Windows.Forms.CheckedListBox();
            this.chkFXResistable = new System.Windows.Forms.CheckBox();
            this.chkFXBuffable = new System.Windows.Forms.CheckBox();
            this.Label26 = new System.Windows.Forms.Label();
            this.txtFXProb = new System.Windows.Forms.TextBox();
            this.Label25 = new System.Windows.Forms.Label();
            this.txtFXDelay = new System.Windows.Forms.TextBox();
            this.Label24 = new System.Windows.Forms.Label();
            this.txtFXTicks = new System.Windows.Forms.TextBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.txtFXDuration = new System.Windows.Forms.TextBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.txtFXMag = new System.Windows.Forms.TextBox();
            this.Label28 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.cbFXClass = new System.Windows.Forms.ComboBox();
            this.cbFXSpecialCase = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.removeConditional = new System.Windows.Forms.Button();
            this.addConditional = new System.Windows.Forms.Button();
            this.lvActiveConditionals = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvConditionalBool = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvConditionalType = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSubConditional = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblEffectDescription = new System.Windows.Forms.Label();
            this.chkVariable = new System.Windows.Forms.CheckBox();
            this.cbPercentageOverride = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtFXScale = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cbAffects = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.cbAttribute = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.cbAspect = new System.Windows.Forms.ComboBox();
            this.chkNearGround = new System.Windows.Forms.CheckBox();
            this.lblAffectsCaster = new System.Windows.Forms.Label();
            this.lvEffectType = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSubAttribute = new System.Windows.Forms.ListView();
            this.chSub = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSubSub = new System.Windows.Forms.ListView();
            this.chSubSub = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Label9 = new System.Windows.Forms.Label();
            this.cmbEffectId = new System.Windows.Forms.ComboBox();
            this.IgnoreED = new System.Windows.Forms.CheckBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtOverride = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.txtPPM = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbModifier = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tpPowerAttribs = new System.Windows.Forms.TableLayoutPanel();
            this.txtMaxTargets = new System.Windows.Forms.Label();
            this.txtArc = new System.Windows.Forms.Label();
            this.txtEffectArea = new System.Windows.Forms.Label();
            this.cbFXEffectArea = new System.Windows.Forms.ComboBox();
            this.txtFXAccuracy = new System.Windows.Forms.TextBox();
            this.txtRange = new System.Windows.Forms.Label();
            this.txtAccuracy = new System.Windows.Forms.Label();
            this.txtCastTime = new System.Windows.Forms.Label();
            this.txtFXRange = new System.Windows.Forms.TextBox();
            this.txtInterruptTime = new System.Windows.Forms.Label();
            this.txtFXInterruptTime = new System.Windows.Forms.TextBox();
            this.txtRechargeTime = new System.Windows.Forms.Label();
            this.txtFXCastTime = new System.Windows.Forms.TextBox();
            this.txtFXRechargeTime = new System.Windows.Forms.TextBox();
            this.txtActivateInterval = new System.Windows.Forms.Label();
            this.txtFXActivateInterval = new System.Windows.Forms.TextBox();
            this.txtEnduranceCost = new System.Windows.Forms.Label();
            this.txtFXEnduranceCost = new System.Windows.Forms.TextBox();
            this.txtSecondaryRange = new System.Windows.Forms.Label();
            this.txtRadius = new System.Windows.Forms.Label();
            this.txtFXSecondaryRange = new System.Windows.Forms.TextBox();
            this.txtFXRadius = new System.Windows.Forms.TextBox();
            this.txtFXArc = new System.Windows.Forms.TextBox();
            this.txtFXMaxTargets = new System.Windows.Forms.TextBox();
            this.GroupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tpPowerAttribs.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(43, 54);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(135, 26);
            this.btnPaste.TabIndex = 116;
            this.btnPaste.Text = "Paste Effect Data";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(43, 22);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(135, 26);
            this.btnCopy.TabIndex = 115;
            this.btnCopy.Text = "Copy Effect Data";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // chkStack
            // 
            this.chkStack.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkStack.Location = new System.Drawing.Point(3, 3);
            this.chkStack.Name = "chkStack";
            this.chkStack.Size = new System.Drawing.Size(222, 20);
            this.chkStack.TabIndex = 112;
            this.chkStack.Text = "Effect Can Stack";
            this.chkStack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.CheckedChanged += new System.EventHandler(this.chkFxNoStack_CheckedChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.clbSuppression);
            this.GroupBox3.Location = new System.Drawing.Point(1040, 150);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(212, 283);
            this.GroupBox3.TabIndex = 107;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Suppress Effect When:";
            // 
            // clbSuppression
            // 
            this.clbSuppression.CheckOnClick = true;
            this.clbSuppression.FormattingEnabled = true;
            this.clbSuppression.Location = new System.Drawing.Point(6, 17);
            this.clbSuppression.Name = "clbSuppression";
            this.clbSuppression.Size = new System.Drawing.Size(200, 259);
            this.clbSuppression.TabIndex = 0;
            this.clbSuppression.SelectedIndexChanged += new System.EventHandler(this.clbSuppression_SelectedIndexChanged);
            // 
            // chkFXResistable
            // 
            this.chkFXResistable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkFXResistable.Location = new System.Drawing.Point(3, 81);
            this.chkFXResistable.Name = "chkFXResistable";
            this.chkFXResistable.Size = new System.Drawing.Size(222, 20);
            this.chkFXResistable.TabIndex = 90;
            this.chkFXResistable.Text = "Effect is Unresistible";
            this.chkFXResistable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.CheckedChanged += new System.EventHandler(this.chkFXResistable_CheckedChanged);
            // 
            // chkFXBuffable
            // 
            this.chkFXBuffable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkFXBuffable.Location = new System.Drawing.Point(3, 29);
            this.chkFXBuffable.Name = "chkFXBuffable";
            this.chkFXBuffable.Size = new System.Drawing.Size(222, 20);
            this.chkFXBuffable.TabIndex = 89;
            this.chkFXBuffable.Text = "Ignore Buffs / Enhancements";
            this.chkFXBuffable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.CheckedChanged += new System.EventHandler(this.chkFXBuffable_CheckedChanged);
            // 
            // Label26
            // 
            this.Label26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label26.Location = new System.Drawing.Point(3, 158);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(76, 26);
            this.Label26.TabIndex = 101;
            this.Label26.Text = "Probability %:";
            this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXProb
            // 
            this.txtFXProb.Location = new System.Drawing.Point(85, 161);
            this.txtFXProb.Name = "txtFXProb";
            this.txtFXProb.Size = new System.Drawing.Size(100, 20);
            this.txtFXProb.TabIndex = 156;
            // 
            // Label25
            // 
            this.Label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label25.Location = new System.Drawing.Point(3, 132);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(76, 26);
            this.Label25.TabIndex = 100;
            this.Label25.Text = "Delay Time:";
            this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDelay
            // 
            this.txtFXDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXDelay.Location = new System.Drawing.Point(85, 135);
            this.txtFXDelay.Name = "txtFXDelay";
            this.txtFXDelay.Size = new System.Drawing.Size(140, 20);
            this.txtFXDelay.TabIndex = 84;
            this.txtFXDelay.Text = "0";
            this.txtFXDelay.TextChanged += new System.EventHandler(this.txtFXDelay_TextChanged);
            this.txtFXDelay.Leave += new System.EventHandler(this.txtFXDelay_Leave);
            // 
            // Label24
            // 
            this.Label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label24.Location = new System.Drawing.Point(3, 106);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(76, 26);
            this.Label24.TabIndex = 99;
            this.Label24.Text = "Ticks:";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXTicks
            // 
            this.txtFXTicks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXTicks.Location = new System.Drawing.Point(85, 109);
            this.txtFXTicks.Name = "txtFXTicks";
            this.txtFXTicks.Size = new System.Drawing.Size(140, 20);
            this.txtFXTicks.TabIndex = 83;
            this.txtFXTicks.Text = "0";
            this.txtFXTicks.TextChanged += new System.EventHandler(this.txtFXTicks_TextChanged);
            this.txtFXTicks.Leave += new System.EventHandler(this.txtFXTicks_Leave);
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label23.Location = new System.Drawing.Point(3, 54);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(76, 26);
            this.Label23.TabIndex = 98;
            this.Label23.Text = "Duration:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDuration
            // 
            this.txtFXDuration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXDuration.Location = new System.Drawing.Point(85, 57);
            this.txtFXDuration.Name = "txtFXDuration";
            this.txtFXDuration.Size = new System.Drawing.Size(140, 20);
            this.txtFXDuration.TabIndex = 82;
            this.txtFXDuration.Text = "0";
            this.txtFXDuration.TextChanged += new System.EventHandler(this.txtFXDuration_TextChanged);
            this.txtFXDuration.Leave += new System.EventHandler(this.txtFXDuration_Leave);
            // 
            // Label22
            // 
            this.Label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label22.Location = new System.Drawing.Point(3, 80);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(76, 26);
            this.Label22.TabIndex = 97;
            this.Label22.Text = "Magnitude:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXMag
            // 
            this.txtFXMag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXMag.Location = new System.Drawing.Point(85, 83);
            this.txtFXMag.Name = "txtFXMag";
            this.txtFXMag.Size = new System.Drawing.Size(140, 20);
            this.txtFXMag.TabIndex = 80;
            this.txtFXMag.Text = "0";
            this.txtFXMag.TextChanged += new System.EventHandler(this.txtFXMag_TextChanged);
            this.txtFXMag.Leave += new System.EventHandler(this.txtFXMag_Leave);
            // 
            // Label28
            // 
            this.Label28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label28.Location = new System.Drawing.Point(3, 0);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(81, 28);
            this.Label28.TabIndex = 104;
            this.Label28.Text = "Display Priority:";
            this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label30
            // 
            this.Label30.Location = new System.Drawing.Point(1091, 548);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(100, 20);
            this.Label30.TabIndex = 105;
            this.Label30.Text = "Old Special Case:";
            this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFXClass
            // 
            this.cbFXClass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFXClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXClass.Location = new System.Drawing.Point(90, 3);
            this.cbFXClass.Name = "cbFXClass";
            this.cbFXClass.Size = new System.Drawing.Size(99, 22);
            this.cbFXClass.TabIndex = 93;
            this.cbFXClass.SelectedIndexChanged += new System.EventHandler(this.cbFXClass_SelectedIndexChanged);
            // 
            // cbFXSpecialCase
            // 
            this.cbFXSpecialCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXSpecialCase.Location = new System.Drawing.Point(1075, 571);
            this.cbFXSpecialCase.Name = "cbFXSpecialCase";
            this.cbFXSpecialCase.Size = new System.Drawing.Size(136, 22);
            this.cbFXSpecialCase.TabIndex = 94;
            this.cbFXSpecialCase.SelectedIndexChanged += new System.EventHandler(this.cbFXSpecialCase_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.removeConditional);
            this.groupBox2.Controls.Add(this.addConditional);
            this.groupBox2.Controls.Add(this.lvActiveConditionals);
            this.groupBox2.Controls.Add(this.lvConditionalBool);
            this.groupBox2.Controls.Add(this.lvConditionalType);
            this.groupBox2.Controls.Add(this.lvSubConditional);
            this.groupBox2.Location = new System.Drawing.Point(246, 439);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(788, 311);
            this.groupBox2.TabIndex = 160;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Conditionals";
            // 
            // removeConditional
            // 
            this.removeConditional.Location = new System.Drawing.Point(563, 279);
            this.removeConditional.Name = "removeConditional";
            this.removeConditional.Size = new System.Drawing.Size(219, 29);
            this.removeConditional.TabIndex = 165;
            this.removeConditional.Text = "Remove Selected Conditional";
            this.removeConditional.UseVisualStyleBackColor = true;
            this.removeConditional.Click += new System.EventHandler(this.removeConditional_Click);
            // 
            // addConditional
            // 
            this.addConditional.Location = new System.Drawing.Point(460, 279);
            this.addConditional.Name = "addConditional";
            this.addConditional.Size = new System.Drawing.Size(97, 29);
            this.addConditional.TabIndex = 164;
            this.addConditional.Text = "Add Conditional";
            this.addConditional.UseVisualStyleBackColor = true;
            this.addConditional.Click += new System.EventHandler(this.addConditional_Click);
            // 
            // lvActiveConditionals
            // 
            this.lvActiveConditionals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.lvActiveConditionals.FullRowSelect = true;
            this.lvActiveConditionals.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvActiveConditionals.HideSelection = false;
            this.lvActiveConditionals.Location = new System.Drawing.Point(563, 14);
            this.lvActiveConditionals.MultiSelect = false;
            this.lvActiveConditionals.Name = "lvActiveConditionals";
            this.lvActiveConditionals.Size = new System.Drawing.Size(219, 259);
            this.lvActiveConditionals.TabIndex = 163;
            this.lvActiveConditionals.UseCompatibleStateImageBehavior = false;
            this.lvActiveConditionals.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Active Conditional";
            this.columnHeader5.Width = 170;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            // 
            // lvConditionalBool
            // 
            this.lvConditionalBool.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvConditionalBool.Enabled = false;
            this.lvConditionalBool.FullRowSelect = true;
            this.lvConditionalBool.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConditionalBool.HideSelection = false;
            this.lvConditionalBool.Location = new System.Drawing.Point(460, 14);
            this.lvConditionalBool.MultiSelect = false;
            this.lvConditionalBool.Name = "lvConditionalBool";
            this.lvConditionalBool.Size = new System.Drawing.Size(97, 259);
            this.lvConditionalBool.TabIndex = 162;
            this.lvConditionalBool.UseCompatibleStateImageBehavior = false;
            this.lvConditionalBool.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Conditional Boolean";
            this.columnHeader4.Width = 81;
            // 
            // lvConditionalType
            // 
            this.lvConditionalType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvConditionalType.FullRowSelect = true;
            this.lvConditionalType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConditionalType.HideSelection = false;
            this.lvConditionalType.Location = new System.Drawing.Point(6, 14);
            this.lvConditionalType.MultiSelect = false;
            this.lvConditionalType.Name = "lvConditionalType";
            this.lvConditionalType.Size = new System.Drawing.Size(116, 294);
            this.lvConditionalType.TabIndex = 161;
            this.lvConditionalType.UseCompatibleStateImageBehavior = false;
            this.lvConditionalType.View = System.Windows.Forms.View.Details;
            this.lvConditionalType.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvConditionalType_SelectionChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Effect Conditional";
            this.columnHeader3.Width = 102;
            // 
            // lvSubConditional
            // 
            this.lvSubConditional.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvSubConditional.FullRowSelect = true;
            this.lvSubConditional.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubConditional.HideSelection = false;
            this.lvSubConditional.Location = new System.Drawing.Point(128, 14);
            this.lvSubConditional.MultiSelect = false;
            this.lvSubConditional.Name = "lvSubConditional";
            this.lvSubConditional.Size = new System.Drawing.Size(326, 294);
            this.lvSubConditional.TabIndex = 160;
            this.lvSubConditional.UseCompatibleStateImageBehavior = false;
            this.lvSubConditional.View = System.Windows.Forms.View.Details;
            this.lvSubConditional.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvSubConditional_SelectionChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sub-Conditional Attribute";
            this.columnHeader2.Width = 202;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(111, 109);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 36);
            this.btnOK.TabIndex = 119;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(3, 109);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 36);
            this.btnCancel.TabIndex = 118;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblEffectDescription
            // 
            this.lblEffectDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEffectDescription.Location = new System.Drawing.Point(12, 9);
            this.lblEffectDescription.Name = "lblEffectDescription";
            this.lblEffectDescription.Size = new System.Drawing.Size(1240, 112);
            this.lblEffectDescription.TabIndex = 120;
            this.lblEffectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEffectDescription.UseMnemonic = false;
            // 
            // chkVariable
            // 
            this.chkVariable.Location = new System.Drawing.Point(613, 129);
            this.chkVariable.Name = "chkVariable";
            this.chkVariable.Size = new System.Drawing.Size(178, 19);
            this.chkVariable.TabIndex = 126;
            this.chkVariable.Text = "Enable Power Scaling Override";
            this.chkVariable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVariable.CheckedChanged += new System.EventHandler(this.chkVariable_CheckedChanged);
            // 
            // cbPercentageOverride
            // 
            this.cbPercentageOverride.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPercentageOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPercentageOverride.Location = new System.Drawing.Point(85, 3);
            this.cbPercentageOverride.Name = "cbPercentageOverride";
            this.cbPercentageOverride.Size = new System.Drawing.Size(140, 22);
            this.cbPercentageOverride.TabIndex = 127;
            this.cbPercentageOverride.SelectedIndexChanged += new System.EventHandler(this.cbPercentageOverride_SelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label2.Location = new System.Drawing.Point(3, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(76, 28);
            this.Label2.TabIndex = 128;
            this.Label2.Text = "Percentage:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Location = new System.Drawing.Point(3, 28);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(76, 26);
            this.Label1.TabIndex = 130;
            this.Label1.Text = "Scale:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXScale
            // 
            this.txtFXScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXScale.Location = new System.Drawing.Point(85, 31);
            this.txtFXScale.Name = "txtFXScale";
            this.txtFXScale.Size = new System.Drawing.Size(140, 20);
            this.txtFXScale.TabIndex = 129;
            this.txtFXScale.Text = "0";
            this.txtFXScale.TextChanged += new System.EventHandler(this.txtFXScale_TextChanged);
            this.txtFXScale.Leave += new System.EventHandler(this.txtFXScale_Leave);
            // 
            // Label3
            // 
            this.Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label3.Location = new System.Drawing.Point(3, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(76, 28);
            this.Label3.TabIndex = 132;
            this.Label3.Text = "Affects:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAffects
            // 
            this.cbAffects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAffects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAffects.Location = new System.Drawing.Point(85, 3);
            this.cbAffects.Name = "cbAffects";
            this.cbAffects.Size = new System.Drawing.Size(140, 22);
            this.cbAffects.TabIndex = 131;
            this.cbAffects.SelectedIndexChanged += new System.EventHandler(this.cbAffects_SelectedIndexChanged);
            // 
            // Label4
            // 
            this.Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label4.Location = new System.Drawing.Point(3, 210);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(76, 28);
            this.Label4.TabIndex = 134;
            this.Label4.Text = "AttribType:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAttribute
            // 
            this.cbAttribute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttribute.Location = new System.Drawing.Point(85, 213);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(140, 22);
            this.cbAttribute.TabIndex = 133;
            this.cbAttribute.SelectedIndexChanged += new System.EventHandler(this.cbAttribute_SelectedIndexChanged);
            // 
            // Label5
            // 
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Location = new System.Drawing.Point(3, 238);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(76, 28);
            this.Label5.TabIndex = 136;
            this.Label5.Text = "Aspect:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAspect
            // 
            this.cbAspect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAspect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAspect.Location = new System.Drawing.Point(85, 241);
            this.cbAspect.Name = "cbAspect";
            this.cbAspect.Size = new System.Drawing.Size(140, 22);
            this.cbAspect.TabIndex = 135;
            this.cbAspect.SelectedIndexChanged += new System.EventHandler(this.cbAspect_SelectedIndexChanged);
            // 
            // chkNearGround
            // 
            this.chkNearGround.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNearGround.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkNearGround.Location = new System.Drawing.Point(3, 107);
            this.chkNearGround.Name = "chkNearGround";
            this.chkNearGround.Size = new System.Drawing.Size(222, 26);
            this.chkNearGround.TabIndex = 139;
            this.chkNearGround.Text = "Target must be Near Ground";
            this.chkNearGround.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAffectsCaster
            // 
            this.lblAffectsCaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAffectsCaster.Location = new System.Drawing.Point(85, 28);
            this.lblAffectsCaster.Name = "lblAffectsCaster";
            this.lblAffectsCaster.Size = new System.Drawing.Size(140, 32);
            this.lblAffectsCaster.TabIndex = 140;
            this.lblAffectsCaster.Text = "Power also affects caster";
            this.lblAffectsCaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvEffectType
            // 
            this.lvEffectType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1});
            this.lvEffectType.FullRowSelect = true;
            this.lvEffectType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvEffectType.HideSelection = false;
            this.lvEffectType.Location = new System.Drawing.Point(246, 158);
            this.lvEffectType.MultiSelect = false;
            this.lvEffectType.Name = "lvEffectType";
            this.lvEffectType.Size = new System.Drawing.Size(226, 275);
            this.lvEffectType.TabIndex = 141;
            this.lvEffectType.UseCompatibleStateImageBehavior = false;
            this.lvEffectType.View = System.Windows.Forms.View.Details;
            this.lvEffectType.SelectedIndexChanged += new System.EventHandler(this.lvEffectType_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Effect Attribute";
            this.ColumnHeader1.Width = 202;
            // 
            // lvSubAttribute
            // 
            this.lvSubAttribute.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSub});
            this.lvSubAttribute.FullRowSelect = true;
            this.lvSubAttribute.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubAttribute.HideSelection = false;
            this.lvSubAttribute.Location = new System.Drawing.Point(478, 167);
            this.lvSubAttribute.MultiSelect = false;
            this.lvSubAttribute.Name = "lvSubAttribute";
            this.lvSubAttribute.Size = new System.Drawing.Size(275, 266);
            this.lvSubAttribute.TabIndex = 142;
            this.lvSubAttribute.UseCompatibleStateImageBehavior = false;
            this.lvSubAttribute.View = System.Windows.Forms.View.Details;
            this.lvSubAttribute.SelectedIndexChanged += new System.EventHandler(this.lvSubAttribute_SelectedIndexChanged);
            // 
            // chSub
            // 
            this.chSub.Text = "Sub-Attribute";
            this.chSub.Width = 254;
            // 
            // lvSubSub
            // 
            this.lvSubSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSubSub});
            this.lvSubSub.FullRowSelect = true;
            this.lvSubSub.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubSub.HideSelection = false;
            this.lvSubSub.Location = new System.Drawing.Point(759, 158);
            this.lvSubSub.MultiSelect = false;
            this.lvSubSub.Name = "lvSubSub";
            this.lvSubSub.Size = new System.Drawing.Size(275, 275);
            this.lvSubSub.TabIndex = 144;
            this.lvSubSub.UseCompatibleStateImageBehavior = false;
            this.lvSubSub.View = System.Windows.Forms.View.Details;
            this.lvSubSub.SelectedIndexChanged += new System.EventHandler(this.lvSubSub_SelectedIndexChanged);
            // 
            // chSubSub
            // 
            this.chSubSub.Text = "Sub-Sub";
            this.chSubSub.Width = 254;
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(1081, 438);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(120, 19);
            this.Label9.TabIndex = 149;
            this.Label9.Text = "GlobalChanceMod Flag:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbEffectId
            // 
            this.cmbEffectId.FormattingEnabled = true;
            this.cmbEffectId.Location = new System.Drawing.Point(1062, 460);
            this.cmbEffectId.Name = "cmbEffectId";
            this.cmbEffectId.Size = new System.Drawing.Size(166, 22);
            this.cmbEffectId.TabIndex = 150;
            this.cmbEffectId.TextChanged += new System.EventHandler(this.cmbEffectId_TextChanged);
            // 
            // IgnoreED
            // 
            this.IgnoreED.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IgnoreED.Location = new System.Drawing.Point(3, 55);
            this.IgnoreED.Name = "IgnoreED";
            this.IgnoreED.Size = new System.Drawing.Size(222, 20);
            this.IgnoreED.TabIndex = 151;
            this.IgnoreED.Text = "Ignore ED";
            this.IgnoreED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.CheckedChanged += new System.EventHandler(this.IgnoreED_CheckedChanged);
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(1076, 494);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(143, 19);
            this.Label10.TabIndex = 152;
            this.Label10.Text = "Override GlobalChanceMod:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtOverride
            // 
            this.txtOverride.Location = new System.Drawing.Point(1062, 516);
            this.txtOverride.Name = "txtOverride";
            this.txtOverride.Size = new System.Drawing.Size(166, 20);
            this.txtOverride.TabIndex = 153;
            this.txtOverride.TextChanged += new System.EventHandler(this.txtOverride_TextChanged);
            // 
            // Label11
            // 
            this.Label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label11.Location = new System.Drawing.Point(3, 184);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(76, 26);
            this.Label11.TabIndex = 155;
            this.Label11.Text = "PPM:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPPM
            // 
            this.txtPPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPPM.Location = new System.Drawing.Point(85, 187);
            this.txtPPM.Name = "txtPPM";
            this.txtPPM.Size = new System.Drawing.Size(140, 20);
            this.txtPPM.TabIndex = 154;
            this.txtPPM.Text = "0";
            this.txtPPM.TextChanged += new System.EventHandler(this.txtPPM_TextChanged);
            this.txtPPM.Leave += new System.EventHandler(this.txtPPM_Leave);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel1.Controls.Add(this.Label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbPercentageOverride, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtFXScale, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label11, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.Label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label22, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtPPM, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.Label23, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtFXDuration, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.Label24, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtFXMag, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtFXTicks, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.Label25, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtFXDelay, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.Label26, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtFXProb, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.Label4, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.cbAttribute, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.Label5, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.cbAspect, 1, 9);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 158);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(228, 275);
            this.tableLayoutPanel1.TabIndex = 161;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.cbModifier, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.Label6, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 439);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.55814F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67.44186F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(228, 43);
            this.tableLayoutPanel2.TabIndex = 162;
            // 
            // cbModifier
            // 
            this.cbModifier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModifier.Location = new System.Drawing.Point(3, 17);
            this.cbModifier.Name = "cbModifier";
            this.cbModifier.Size = new System.Drawing.Size(222, 22);
            this.cbModifier.TabIndex = 137;
            this.cbModifier.SelectedIndexChanged += new System.EventHandler(this.cbModifier_SelectedIndexChanged);
            // 
            // Label6
            // 
            this.Label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label6.Location = new System.Drawing.Point(3, 0);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(222, 14);
            this.Label6.TabIndex = 138;
            this.Label6.Text = "Modifier Table:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel3.Controls.Add(this.lblAffectsCaster, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.cbAffects, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.Label3, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 488);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 13;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(228, 63);
            this.tableLayoutPanel3.TabIndex = 163;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.chkStack, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.chkFXBuffable, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.IgnoreED, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.chkFXResistable, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.chkNearGround, 0, 4);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(12, 596);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 6;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(228, 136);
            this.tableLayoutPanel4.TabIndex = 164;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.08911F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.91089F));
            this.tableLayoutPanel5.Controls.Add(this.cbTarget, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 557);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(228, 33);
            this.tableLayoutPanel5.TabIndex = 165;
            // 
            // cbTarget
            // 
            this.cbTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTarget.Items.AddRange(new object[] {
            "Any",
            "Mobs",
            "Players"});
            this.cbTarget.Location = new System.Drawing.Point(96, 3);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(129, 22);
            this.cbTarget.TabIndex = 166;
            this.cbTarget.SelectedIndexChanged += new System.EventHandler(this.cbTarget_IndexChanged);
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 33);
            this.label12.TabIndex = 166;
            this.label12.Text = "Target:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.81005F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.18995F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Controls.Add(this.Label28, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.cbFXClass, 1, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(415, 124);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(192, 28);
            this.tableLayoutPanel6.TabIndex = 166;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCopy);
            this.panel1.Controls.Add(this.btnPaste);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(1040, 599);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(212, 152);
            this.panel1.TabIndex = 167;
            // 
            // tpPowerAttribs
            // 
            this.tpPowerAttribs.ColumnCount = 2;
            this.tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this.tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tpPowerAttribs.Controls.Add(this.txtMaxTargets, 0, 14);
            this.tpPowerAttribs.Controls.Add(this.txtArc, 0, 13);
            this.tpPowerAttribs.Controls.Add(this.txtEffectArea, 0, 0);
            this.tpPowerAttribs.Controls.Add(this.cbFXEffectArea, 1, 0);
            this.tpPowerAttribs.Controls.Add(this.txtFXAccuracy, 1, 1);
            this.tpPowerAttribs.Controls.Add(this.txtRange, 0, 7);
            this.tpPowerAttribs.Controls.Add(this.txtAccuracy, 0, 1);
            this.tpPowerAttribs.Controls.Add(this.txtCastTime, 0, 3);
            this.tpPowerAttribs.Controls.Add(this.txtFXRange, 1, 7);
            this.tpPowerAttribs.Controls.Add(this.txtInterruptTime, 0, 2);
            this.tpPowerAttribs.Controls.Add(this.txtFXInterruptTime, 1, 2);
            this.tpPowerAttribs.Controls.Add(this.txtRechargeTime, 0, 4);
            this.tpPowerAttribs.Controls.Add(this.txtFXCastTime, 1, 3);
            this.tpPowerAttribs.Controls.Add(this.txtFXRechargeTime, 1, 4);
            this.tpPowerAttribs.Controls.Add(this.txtActivateInterval, 0, 5);
            this.tpPowerAttribs.Controls.Add(this.txtFXActivateInterval, 1, 5);
            this.tpPowerAttribs.Controls.Add(this.txtEnduranceCost, 0, 6);
            this.tpPowerAttribs.Controls.Add(this.txtFXEnduranceCost, 1, 6);
            this.tpPowerAttribs.Controls.Add(this.txtSecondaryRange, 0, 8);
            this.tpPowerAttribs.Controls.Add(this.txtRadius, 0, 9);
            this.tpPowerAttribs.Controls.Add(this.txtFXSecondaryRange, 1, 8);
            this.tpPowerAttribs.Controls.Add(this.txtFXRadius, 1, 9);
            this.tpPowerAttribs.Controls.Add(this.txtFXArc, 1, 13);
            this.tpPowerAttribs.Controls.Add(this.txtFXMaxTargets, 1, 14);
            this.tpPowerAttribs.Location = new System.Drawing.Point(12, 158);
            this.tpPowerAttribs.Name = "tpPowerAttribs";
            this.tpPowerAttribs.RowCount = 15;
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpPowerAttribs.Size = new System.Drawing.Size(228, 320);
            this.tpPowerAttribs.TabIndex = 168;
            this.tpPowerAttribs.Visible = false;
            // 
            // txtMaxTargets
            // 
            this.txtMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMaxTargets.Location = new System.Drawing.Point(3, 293);
            this.txtMaxTargets.Name = "txtMaxTargets";
            this.txtMaxTargets.Size = new System.Drawing.Size(111, 27);
            this.txtMaxTargets.TabIndex = 161;
            this.txtMaxTargets.Text = "Max Targets:";
            this.txtMaxTargets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtArc
            // 
            this.txtArc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtArc.Location = new System.Drawing.Point(3, 266);
            this.txtArc.Name = "txtArc";
            this.txtArc.Size = new System.Drawing.Size(111, 27);
            this.txtArc.TabIndex = 156;
            this.txtArc.Text = "Arc:";
            this.txtArc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEffectArea
            // 
            this.txtEffectArea.AutoSize = true;
            this.txtEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEffectArea.Location = new System.Drawing.Point(3, 0);
            this.txtEffectArea.Name = "txtEffectArea";
            this.txtEffectArea.Size = new System.Drawing.Size(111, 28);
            this.txtEffectArea.TabIndex = 128;
            this.txtEffectArea.Text = "Effect Area:";
            this.txtEffectArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFXEffectArea
            // 
            this.cbFXEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFXEffectArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXEffectArea.Location = new System.Drawing.Point(120, 3);
            this.cbFXEffectArea.Name = "cbFXEffectArea";
            this.cbFXEffectArea.Size = new System.Drawing.Size(105, 22);
            this.cbFXEffectArea.TabIndex = 127;
            this.cbFXEffectArea.SelectedIndexChanged += new System.EventHandler(this.cbFXEffectArea_SelectedIndexChanged);
            // 
            // txtFXAccuracy
            // 
            this.txtFXAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXAccuracy.Location = new System.Drawing.Point(120, 31);
            this.txtFXAccuracy.Name = "txtFXAccuracy";
            this.txtFXAccuracy.Size = new System.Drawing.Size(105, 20);
            this.txtFXAccuracy.TabIndex = 129;
            this.txtFXAccuracy.Text = "0";
            this.txtFXAccuracy.TextChanged += new System.EventHandler(this.txtFXAccuracy_TextChanged);
            // 
            // txtRange
            // 
            this.txtRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRange.Location = new System.Drawing.Point(3, 184);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(111, 26);
            this.txtRange.TabIndex = 155;
            this.txtRange.Text = "Range:";
            this.txtRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccuracy
            // 
            this.txtAccuracy.AutoSize = true;
            this.txtAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAccuracy.Location = new System.Drawing.Point(3, 28);
            this.txtAccuracy.Name = "txtAccuracy";
            this.txtAccuracy.Size = new System.Drawing.Size(111, 26);
            this.txtAccuracy.TabIndex = 130;
            this.txtAccuracy.Text = "Accuracy:";
            this.txtAccuracy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCastTime
            // 
            this.txtCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCastTime.Location = new System.Drawing.Point(3, 80);
            this.txtCastTime.Name = "txtCastTime";
            this.txtCastTime.Size = new System.Drawing.Size(111, 26);
            this.txtCastTime.TabIndex = 97;
            this.txtCastTime.Text = "Casting Time:";
            this.txtCastTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXRange
            // 
            this.txtFXRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRange.Location = new System.Drawing.Point(120, 187);
            this.txtFXRange.Name = "txtFXRange";
            this.txtFXRange.Size = new System.Drawing.Size(105, 20);
            this.txtFXRange.TabIndex = 154;
            this.txtFXRange.Text = "0";
            this.txtFXRange.TextChanged += new System.EventHandler(this.txtFXRange_TextChanged);
            // 
            // txtInterruptTime
            // 
            this.txtInterruptTime.AutoSize = true;
            this.txtInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInterruptTime.Location = new System.Drawing.Point(3, 54);
            this.txtInterruptTime.Name = "txtInterruptTime";
            this.txtInterruptTime.Size = new System.Drawing.Size(111, 26);
            this.txtInterruptTime.TabIndex = 98;
            this.txtInterruptTime.Text = "Interruptable Time:";
            this.txtInterruptTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXInterruptTime
            // 
            this.txtFXInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXInterruptTime.Location = new System.Drawing.Point(120, 57);
            this.txtFXInterruptTime.Name = "txtFXInterruptTime";
            this.txtFXInterruptTime.Size = new System.Drawing.Size(105, 20);
            this.txtFXInterruptTime.TabIndex = 82;
            this.txtFXInterruptTime.Text = "0";
            this.txtFXInterruptTime.TextChanged += new System.EventHandler(this.txtFXInterruptTime_TextChanged);
            // 
            // txtRechargeTime
            // 
            this.txtRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRechargeTime.Location = new System.Drawing.Point(3, 106);
            this.txtRechargeTime.Name = "txtRechargeTime";
            this.txtRechargeTime.Size = new System.Drawing.Size(111, 26);
            this.txtRechargeTime.TabIndex = 99;
            this.txtRechargeTime.Text = "Recharge Time:";
            this.txtRechargeTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXCastTime
            // 
            this.txtFXCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXCastTime.Location = new System.Drawing.Point(120, 83);
            this.txtFXCastTime.Name = "txtFXCastTime";
            this.txtFXCastTime.Size = new System.Drawing.Size(105, 20);
            this.txtFXCastTime.TabIndex = 80;
            this.txtFXCastTime.Text = "0";
            this.txtFXCastTime.TextChanged += new System.EventHandler(this.txtFXCastTime_TextChanged);
            // 
            // txtFXRechargeTime
            // 
            this.txtFXRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRechargeTime.Location = new System.Drawing.Point(120, 109);
            this.txtFXRechargeTime.Name = "txtFXRechargeTime";
            this.txtFXRechargeTime.Size = new System.Drawing.Size(105, 20);
            this.txtFXRechargeTime.TabIndex = 83;
            this.txtFXRechargeTime.Text = "0";
            this.txtFXRechargeTime.TextChanged += new System.EventHandler(this.txtFXRechargeTime_TextChanged);
            // 
            // txtActivateInterval
            // 
            this.txtActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtActivateInterval.Location = new System.Drawing.Point(3, 132);
            this.txtActivateInterval.Name = "txtActivateInterval";
            this.txtActivateInterval.Size = new System.Drawing.Size(111, 26);
            this.txtActivateInterval.TabIndex = 100;
            this.txtActivateInterval.Text = "Activate Interval:";
            this.txtActivateInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXActivateInterval
            // 
            this.txtFXActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXActivateInterval.Location = new System.Drawing.Point(120, 135);
            this.txtFXActivateInterval.Name = "txtFXActivateInterval";
            this.txtFXActivateInterval.Size = new System.Drawing.Size(105, 20);
            this.txtFXActivateInterval.TabIndex = 84;
            this.txtFXActivateInterval.Text = "0";
            this.txtFXActivateInterval.TextChanged += new System.EventHandler(this.txtFXActivateInterval_TextChanged);
            // 
            // txtEnduranceCost
            // 
            this.txtEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnduranceCost.Location = new System.Drawing.Point(3, 158);
            this.txtEnduranceCost.Name = "txtEnduranceCost";
            this.txtEnduranceCost.Size = new System.Drawing.Size(111, 26);
            this.txtEnduranceCost.TabIndex = 101;
            this.txtEnduranceCost.Text = "Endurance Cost:";
            this.txtEnduranceCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXEnduranceCost
            // 
            this.txtFXEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXEnduranceCost.Location = new System.Drawing.Point(120, 161);
            this.txtFXEnduranceCost.Name = "txtFXEnduranceCost";
            this.txtFXEnduranceCost.Size = new System.Drawing.Size(105, 20);
            this.txtFXEnduranceCost.TabIndex = 85;
            this.txtFXEnduranceCost.Text = "1";
            this.txtFXEnduranceCost.TextChanged += new System.EventHandler(this.txtFXEnduranceCost_TextChanged);
            // 
            // txtSecondaryRange
            // 
            this.txtSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSecondaryRange.Location = new System.Drawing.Point(3, 210);
            this.txtSecondaryRange.Name = "txtSecondaryRange";
            this.txtSecondaryRange.Size = new System.Drawing.Size(111, 28);
            this.txtSecondaryRange.TabIndex = 134;
            this.txtSecondaryRange.Text = "Secondary Range:";
            this.txtSecondaryRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRadius
            // 
            this.txtRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRadius.Location = new System.Drawing.Point(3, 238);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(111, 28);
            this.txtRadius.TabIndex = 136;
            this.txtRadius.Text = "Radius:";
            this.txtRadius.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXSecondaryRange
            // 
            this.txtFXSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXSecondaryRange.Location = new System.Drawing.Point(120, 213);
            this.txtFXSecondaryRange.Name = "txtFXSecondaryRange";
            this.txtFXSecondaryRange.Size = new System.Drawing.Size(105, 20);
            this.txtFXSecondaryRange.TabIndex = 157;
            this.txtFXSecondaryRange.Text = "0";
            this.txtFXSecondaryRange.TextChanged += new System.EventHandler(this.txtFXSecondaryRange_TextChanged);
            // 
            // txtFXRadius
            // 
            this.txtFXRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRadius.Location = new System.Drawing.Point(120, 241);
            this.txtFXRadius.Name = "txtFXRadius";
            this.txtFXRadius.Size = new System.Drawing.Size(105, 20);
            this.txtFXRadius.TabIndex = 158;
            this.txtFXRadius.Text = "0";
            this.txtFXRadius.TextChanged += new System.EventHandler(this.txtFXRadius_TextChanged);
            // 
            // txtFXArc
            // 
            this.txtFXArc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXArc.Location = new System.Drawing.Point(120, 269);
            this.txtFXArc.Name = "txtFXArc";
            this.txtFXArc.Size = new System.Drawing.Size(105, 20);
            this.txtFXArc.TabIndex = 159;
            this.txtFXArc.Text = "0";
            this.txtFXArc.TextChanged += new System.EventHandler(this.txtFXArc_TextChanged);
            // 
            // txtFXMaxTargets
            // 
            this.txtFXMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXMaxTargets.Location = new System.Drawing.Point(120, 296);
            this.txtFXMaxTargets.Name = "txtFXMaxTargets";
            this.txtFXMaxTargets.Size = new System.Drawing.Size(105, 20);
            this.txtFXMaxTargets.TabIndex = 162;
            this.txtFXMaxTargets.Text = "0";
            this.txtFXMaxTargets.TextChanged += new System.EventHandler(this.txtFXMaxTargets_TextChanged);
            // 
            // frmPowerEffect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 756);
            this.Controls.Add(this.tpPowerAttribs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.chkVariable);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtOverride);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.cmbEffectId);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.lvSubSub);
            this.Controls.Add(this.lvSubAttribute);
            this.Controls.Add(this.lvEffectType);
            this.Controls.Add(this.lblEffectDescription);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.Label30);
            this.Controls.Add(this.cbFXSpecialCase);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPowerEffect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Effect";
            this.GroupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tpPowerAttribs.ResumeLayout(false);
            this.tpPowerAttribs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        Button btnCancel;
        Button btnCopy;
        Button btnOK;
        Button btnPaste;
        ComboBox cbAffects;
        ComboBox cbAspect;
        ComboBox cbAttribute;
        ComboBox cbFXClass;
        ComboBox cbFXSpecialCase;
        ComboBox cbPercentageOverride;
        CheckBox chkFXBuffable;
        CheckBox chkFXResistable;
        CheckBox chkNearGround;
        CheckBox chkStack;
        CheckBox chkVariable;
        ColumnHeader chSub;
        ColumnHeader chSubSub;
        CheckedListBox clbSuppression;
        ComboBox cmbEffectId;
        ColumnHeader ColumnHeader1;
        GroupBox GroupBox3;
        CheckBox IgnoreED;
        Label Label1;
        Label Label10;
        Label Label11;
        Label Label2;
        Label Label22;
        Label Label23;
        Label Label24;
        Label Label25;
        Label Label26;
        Label Label28;
        Label Label3;
        Label Label30;
        Label Label4;
        Label Label5;
        Label Label9;
        Label lblAffectsCaster;
        Label lblEffectDescription;
        ListView lvEffectType;
        ListView lvSubAttribute;
        ListView lvSubSub;
        TextBox txtFXDelay;
        TextBox txtFXDuration;
        TextBox txtFXMag;
        TextBox txtFXProb;
        TextBox txtFXScale;
        TextBox txtFXTicks;
        TextBox txtOverride;
        TextBox txtPPM;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox cbModifier;
        private Label Label6;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private ComboBox cbTarget;
        private Label label12;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel1;
        private TableLayoutPanel tpPowerAttribs;
        private Label txtMaxTargets;
        private Label txtArc;
        private Label txtEffectArea;
        private ComboBox cbFXEffectArea;
        private TextBox txtFXAccuracy;
        private Label txtRange;
        private Label txtAccuracy;
        private Label txtCastTime;
        private TextBox txtFXRange;
        private Label txtInterruptTime;
        private TextBox txtFXInterruptTime;
        private Label txtRechargeTime;
        private TextBox txtFXCastTime;
        private TextBox txtFXRechargeTime;
        private Label txtActivateInterval;
        private TextBox txtFXActivateInterval;
        private Label txtEnduranceCost;
        private TextBox txtFXEnduranceCost;
        private Label txtSecondaryRange;
        private Label txtRadius;
        private TextBox txtFXSecondaryRange;
        private TextBox txtFXRadius;
        private TextBox txtFXArc;
        private TextBox txtFXMaxTargets;
        private GroupBox groupBox2;
        private ListView lvConditionalType;
        private ColumnHeader columnHeader3;
        private ListView lvSubConditional;
        private ColumnHeader columnHeader2;
        private ListView lvActiveConditionals;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ListView lvConditionalBool;
        private ColumnHeader columnHeader4;
        private Button removeConditional;
        private Button addConditional;
    }
}
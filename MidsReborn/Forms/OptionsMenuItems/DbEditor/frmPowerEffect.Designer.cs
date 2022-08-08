using System;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmPowerEffect
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
            this.cbCoDFormat = new System.Windows.Forms.CheckBox();
            this.lblEffectDescription = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Label2 = new System.Windows.Forms.Label();
            this.cbPercentageOverride = new System.Windows.Forms.ComboBox();
            this.txtFXScale = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.txtPPM = new System.Windows.Forms.TextBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.txtFXDuration = new System.Windows.Forms.TextBox();
            this.Label24 = new System.Windows.Forms.Label();
            this.txtFXMag = new System.Windows.Forms.TextBox();
            this.txtFXTicks = new System.Windows.Forms.TextBox();
            this.Label25 = new System.Windows.Forms.Label();
            this.txtFXDelay = new System.Windows.Forms.TextBox();
            this.Label26 = new System.Windows.Forms.Label();
            this.txtFXProb = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.cbAttribute = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.cbAspect = new System.Windows.Forms.ComboBox();
            this.lvSubSub = new ctlListViewColored();
            this.chSubSub = new System.Windows.Forms.ColumnHeader();
            this.lvSubAttribute = new ctlListViewColored();
            this.chSub = new System.Windows.Forms.ColumnHeader();
            this.lvEffectType = new ctlListViewColored();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.lblAffectsCaster = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.cbAffects = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.cbModifier = new System.Windows.Forms.ComboBox();
            this.chkStack = new System.Windows.Forms.CheckBox();
            this.chkNearGround = new System.Windows.Forms.CheckBox();
            this.chkFXResistable = new System.Windows.Forms.CheckBox();
            this.IgnoreED = new System.Windows.Forms.CheckBox();
            this.chkFXBuffable = new System.Windows.Forms.CheckBox();
            this.chkCancelOnMiss = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkRqToHitCheck = new System.Windows.Forms.CheckBox();
            this.chkIgnoreScale = new System.Windows.Forms.CheckBox();
            this.Label28 = new System.Windows.Forms.Label();
            this.cbFXClass = new System.Windows.Forms.ComboBox();
            this.chkVariable = new System.Windows.Forms.CheckBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtOverride = new System.Windows.Forms.TextBox();
            this.cmbEffectId = new System.Windows.Forms.ComboBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.cbFXSpecialCase = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.clbSuppression = new System.Windows.Forms.CheckedListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEditConditions = new System.Windows.Forms.Button();
            this.btnExprBuilder = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tpPowerAttribs.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbCoDFormat
            // 
            this.cbCoDFormat.AutoSize = true;
            this.cbCoDFormat.BackColor = System.Drawing.Color.Transparent;
            this.cbCoDFormat.Location = new System.Drawing.Point(1152, 15);
            this.cbCoDFormat.Margin = new System.Windows.Forms.Padding(4);
            this.cbCoDFormat.Name = "cbCoDFormat";
            this.cbCoDFormat.Size = new System.Drawing.Size(141, 20);
            this.cbCoDFormat.TabIndex = 177;
            this.cbCoDFormat.Text = "Use CoD effect format";
            this.cbCoDFormat.UseVisualStyleBackColor = false;
            this.cbCoDFormat.CheckedChanged += new System.EventHandler(this.cbCoDFormat_CheckedChanged);
            // 
            // lblEffectDescription
            // 
            this.lblEffectDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEffectDescription.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEffectDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblEffectDescription.Location = new System.Drawing.Point(14, 11);
            this.lblEffectDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEffectDescription.Name = "lblEffectDescription";
            this.lblEffectDescription.Size = new System.Drawing.Size(1304, 110);
            this.lblEffectDescription.TabIndex = 176;
            this.lblEffectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEffectDescription.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 124);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(266, 338);
            this.tableLayoutPanel1.TabIndex = 178;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label2.Location = new System.Drawing.Point(4, 0);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(88, 33);
            this.Label2.TabIndex = 128;
            this.Label2.Text = "Percentage:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPercentageOverride
            // 
            this.cbPercentageOverride.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPercentageOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPercentageOverride.Location = new System.Drawing.Point(100, 4);
            this.cbPercentageOverride.Margin = new System.Windows.Forms.Padding(4);
            this.cbPercentageOverride.Name = "cbPercentageOverride";
            this.cbPercentageOverride.Size = new System.Drawing.Size(162, 24);
            this.cbPercentageOverride.TabIndex = 127;
            this.cbPercentageOverride.SelectedIndexChanged += new System.EventHandler(this.cbPercentageOverride_SelectedIndexChanged);
            // 
            // txtFXScale
            // 
            this.txtFXScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXScale.Location = new System.Drawing.Point(100, 37);
            this.txtFXScale.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXScale.Name = "txtFXScale";
            this.txtFXScale.Size = new System.Drawing.Size(162, 23);
            this.txtFXScale.TabIndex = 129;
            this.txtFXScale.Text = "0";
            this.txtFXScale.TextChanged += new System.EventHandler(this.txtFXScale_TextChanged);
            this.txtFXScale.Leave += new System.EventHandler(this.txtFXScale_Leave);
            // 
            // Label11
            // 
            this.Label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label11.Location = new System.Drawing.Point(4, 231);
            this.Label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(88, 33);
            this.Label11.TabIndex = 155;
            this.Label11.Text = "PPM:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Location = new System.Drawing.Point(4, 33);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(88, 33);
            this.Label1.TabIndex = 130;
            this.Label1.Text = "Scale:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label22
            // 
            this.Label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label22.Location = new System.Drawing.Point(4, 99);
            this.Label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(88, 33);
            this.Label22.TabIndex = 97;
            this.Label22.Text = "Magnitude:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPPM
            // 
            this.txtPPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPPM.Location = new System.Drawing.Point(100, 235);
            this.txtPPM.Margin = new System.Windows.Forms.Padding(4);
            this.txtPPM.Name = "txtPPM";
            this.txtPPM.Size = new System.Drawing.Size(162, 23);
            this.txtPPM.TabIndex = 154;
            this.txtPPM.Text = "0";
            this.txtPPM.TextChanged += new System.EventHandler(this.txtPPM_TextChanged);
            this.txtPPM.Leave += new System.EventHandler(this.txtPPM_Leave);
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label23.Location = new System.Drawing.Point(4, 66);
            this.Label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(88, 33);
            this.Label23.TabIndex = 98;
            this.Label23.Text = "Duration:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDuration
            // 
            this.txtFXDuration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXDuration.Location = new System.Drawing.Point(100, 70);
            this.txtFXDuration.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXDuration.Name = "txtFXDuration";
            this.txtFXDuration.Size = new System.Drawing.Size(162, 23);
            this.txtFXDuration.TabIndex = 82;
            this.txtFXDuration.Text = "0";
            this.txtFXDuration.TextChanged += new System.EventHandler(this.txtFXDuration_TextChanged);
            this.txtFXDuration.Leave += new System.EventHandler(this.txtFXDuration_Leave);
            // 
            // Label24
            // 
            this.Label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label24.Location = new System.Drawing.Point(4, 132);
            this.Label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(88, 33);
            this.Label24.TabIndex = 99;
            this.Label24.Text = "Ticks:";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXMag
            // 
            this.txtFXMag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXMag.Location = new System.Drawing.Point(100, 103);
            this.txtFXMag.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXMag.Name = "txtFXMag";
            this.txtFXMag.Size = new System.Drawing.Size(162, 23);
            this.txtFXMag.TabIndex = 80;
            this.txtFXMag.Text = "0";
            this.txtFXMag.TextChanged += new System.EventHandler(this.txtFXMag_TextChanged);
            this.txtFXMag.Leave += new System.EventHandler(this.txtFXMag_Leave);
            // 
            // txtFXTicks
            // 
            this.txtFXTicks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXTicks.Location = new System.Drawing.Point(100, 136);
            this.txtFXTicks.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXTicks.Name = "txtFXTicks";
            this.txtFXTicks.Size = new System.Drawing.Size(162, 23);
            this.txtFXTicks.TabIndex = 83;
            this.txtFXTicks.Text = "0";
            this.txtFXTicks.TextChanged += new System.EventHandler(this.txtFXTicks_TextChanged);
            this.txtFXTicks.Leave += new System.EventHandler(this.txtFXTicks_Leave);
            // 
            // Label25
            // 
            this.Label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label25.Location = new System.Drawing.Point(4, 165);
            this.Label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(88, 33);
            this.Label25.TabIndex = 100;
            this.Label25.Text = "Delay Time:";
            this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDelay
            // 
            this.txtFXDelay.Location = new System.Drawing.Point(100, 169);
            this.txtFXDelay.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXDelay.Name = "txtFXDelay";
            this.txtFXDelay.Size = new System.Drawing.Size(75, 23);
            this.txtFXDelay.TabIndex = 84;
            this.txtFXDelay.Text = "0";
            this.txtFXDelay.TextChanged += new System.EventHandler(this.txtFXDelay_TextChanged);
            this.txtFXDelay.Leave += new System.EventHandler(this.txtFXDelay_Leave);
            // 
            // Label26
            // 
            this.Label26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label26.Location = new System.Drawing.Point(4, 198);
            this.Label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(88, 33);
            this.Label26.TabIndex = 101;
            this.Label26.Text = "Probability %:";
            this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXProb
            // 
            this.txtFXProb.Location = new System.Drawing.Point(100, 202);
            this.txtFXProb.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXProb.Name = "txtFXProb";
            this.txtFXProb.Size = new System.Drawing.Size(116, 23);
            this.txtFXProb.TabIndex = 156;
            this.txtFXProb.TextChanged += new System.EventHandler(this.txtFXProb_TextChanged);
            this.txtFXProb.MouseLeave += new System.EventHandler(this.txtFXProb_Leave);
            // 
            // Label4
            // 
            this.Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label4.Location = new System.Drawing.Point(4, 264);
            this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(88, 33);
            this.Label4.TabIndex = 134;
            this.Label4.Text = "AttribType:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAttribute
            // 
            this.cbAttribute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttribute.Location = new System.Drawing.Point(100, 268);
            this.cbAttribute.Margin = new System.Windows.Forms.Padding(4);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(162, 24);
            this.cbAttribute.TabIndex = 133;
            this.cbAttribute.SelectedIndexChanged += new System.EventHandler(this.cbAttribute_SelectedIndexChanged);
            // 
            // Label5
            // 
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Location = new System.Drawing.Point(4, 297);
            this.Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(88, 41);
            this.Label5.TabIndex = 136;
            this.Label5.Text = "Aspect:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAspect
            // 
            this.cbAspect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAspect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAspect.Location = new System.Drawing.Point(100, 301);
            this.cbAspect.Margin = new System.Windows.Forms.Padding(4);
            this.cbAspect.Name = "cbAspect";
            this.cbAspect.Size = new System.Drawing.Size(162, 24);
            this.cbAspect.TabIndex = 135;
            this.cbAspect.SelectedIndexChanged += new System.EventHandler(this.cbAspect_SelectedIndexChanged);
            // 
            // lvSubSub
            // 
            this.lvSubSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSubSub});
            this.lvSubSub.FullRowSelect = true;
            this.lvSubSub.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubSub.Location = new System.Drawing.Point(948, 124);
            this.lvSubSub.LostFocusItem = -1;
            this.lvSubSub.Margin = new System.Windows.Forms.Padding(4);
            this.lvSubSub.MultiSelect = false;
            this.lvSubSub.Name = "lvSubSub";
            this.lvSubSub.OwnerDraw = true;
            this.lvSubSub.ShowItemToolTips = true;
            this.lvSubSub.Size = new System.Drawing.Size(434, 431);
            this.lvSubSub.TabIndex = 181;
            this.lvSubSub.UseCompatibleStateImageBehavior = false;
            this.lvSubSub.View = System.Windows.Forms.View.Details;
            this.lvSubSub.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvSubSub.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvSubSub.SelectedIndexChanged += new System.EventHandler(this.lvSubSub_SelectedIndexChanged);
            this.lvSubSub.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // chSubSub
            // 
            this.chSubSub.Text = "Sub-Sub";
            this.chSubSub.Width = 254;
            // 
            // lvSubAttribute
            // 
            this.lvSubAttribute.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSub});
            this.lvSubAttribute.FullRowSelect = true;
            this.lvSubAttribute.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubAttribute.Location = new System.Drawing.Point(621, 124);
            this.lvSubAttribute.LostFocusItem = -1;
            this.lvSubAttribute.Margin = new System.Windows.Forms.Padding(4);
            this.lvSubAttribute.MultiSelect = false;
            this.lvSubAttribute.Name = "lvSubAttribute";
            this.lvSubAttribute.OwnerDraw = true;
            this.lvSubAttribute.Size = new System.Drawing.Size(320, 431);
            this.lvSubAttribute.TabIndex = 180;
            this.lvSubAttribute.UseCompatibleStateImageBehavior = false;
            this.lvSubAttribute.View = System.Windows.Forms.View.Details;
            this.lvSubAttribute.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvSubAttribute.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvSubAttribute.SelectedIndexChanged += new System.EventHandler(this.lvSubAttribute_SelectedIndexChanged);
            this.lvSubAttribute.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // chSub
            // 
            this.chSub.Text = "Sub-Attribute";
            this.chSub.Width = 254;
            // 
            // lvEffectType
            // 
            this.lvEffectType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1});
            this.lvEffectType.FullRowSelect = true;
            this.lvEffectType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvEffectType.Location = new System.Drawing.Point(287, 124);
            this.lvEffectType.LostFocusItem = -1;
            this.lvEffectType.Margin = new System.Windows.Forms.Padding(4);
            this.lvEffectType.MultiSelect = false;
            this.lvEffectType.Name = "lvEffectType";
            this.lvEffectType.OwnerDraw = true;
            this.lvEffectType.Size = new System.Drawing.Size(326, 431);
            this.lvEffectType.TabIndex = 179;
            this.lvEffectType.UseCompatibleStateImageBehavior = false;
            this.lvEffectType.View = System.Windows.Forms.View.Details;
            this.lvEffectType.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvEffectType.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvEffectType.SelectedIndexChanged += new System.EventHandler(this.lvEffectType_SelectedIndexChanged);
            this.lvEffectType.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Effect Attribute";
            this.ColumnHeader1.Width = 202;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 162F));
            this.tableLayoutPanel3.Controls.Add(this.cbTarget, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.lblAffectsCaster, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.Label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cbAffects, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(14, 588);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(266, 98);
            this.tableLayoutPanel3.TabIndex = 182;
            // 
            // cbTarget
            // 
            this.cbTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTarget.Items.AddRange(new object[] {
            "Any",
            "Mobs",
            "Players"});
            this.cbTarget.Location = new System.Drawing.Point(108, 68);
            this.cbTarget.Margin = new System.Windows.Forms.Padding(4);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(154, 24);
            this.cbTarget.TabIndex = 201;
            this.cbTarget.SelectedIndexChanged += new System.EventHandler(this.cbTarget_IndexChanged);
            // 
            // lblAffectsCaster
            // 
            this.lblAffectsCaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAffectsCaster.Location = new System.Drawing.Point(108, 32);
            this.lblAffectsCaster.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAffectsCaster.Name = "lblAffectsCaster";
            this.lblAffectsCaster.Size = new System.Drawing.Size(154, 32);
            this.lblAffectsCaster.TabIndex = 141;
            this.lblAffectsCaster.Text = "Power also affects caster";
            this.lblAffectsCaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(4, 64);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 34);
            this.label12.TabIndex = 202;
            this.label12.Text = "Target:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label3.Location = new System.Drawing.Point(4, 0);
            this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(96, 32);
            this.Label3.TabIndex = 132;
            this.Label3.Text = "Affects:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAffects
            // 
            this.cbAffects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAffects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAffects.Location = new System.Drawing.Point(108, 4);
            this.cbAffects.Margin = new System.Windows.Forms.Padding(4);
            this.cbAffects.Name = "cbAffects";
            this.cbAffects.Size = new System.Drawing.Size(154, 24);
            this.cbAffects.TabIndex = 131;
            this.cbAffects.SelectedIndexChanged += new System.EventHandler(this.cbAffects_SelectedIndexChanged);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(14, 538);
            this.Label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(270, 18);
            this.Label6.TabIndex = 142;
            this.Label6.Text = "Modifier Table:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbModifier
            // 
            this.cbModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModifier.Location = new System.Drawing.Point(14, 559);
            this.cbModifier.Margin = new System.Windows.Forms.Padding(4);
            this.cbModifier.Name = "cbModifier";
            this.cbModifier.Size = new System.Drawing.Size(265, 24);
            this.cbModifier.TabIndex = 143;
            this.cbModifier.SelectedIndexChanged += new System.EventHandler(this.cbModifier_SelectedIndexChanged);
            // 
            // chkStack
            // 
            this.chkStack.AutoSize = true;
            this.chkStack.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.Location = new System.Drawing.Point(4, 4);
            this.chkStack.Margin = new System.Windows.Forms.Padding(4);
            this.chkStack.Name = "chkStack";
            this.chkStack.Size = new System.Drawing.Size(77, 20);
            this.chkStack.TabIndex = 171;
            this.chkStack.Text = "Can Stack";
            this.chkStack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.CheckedChanged += new System.EventHandler(this.chkFxNoStack_CheckedChanged);
            // 
            // chkNearGround
            // 
            this.chkNearGround.AutoSize = true;
            this.chkNearGround.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNearGround.Location = new System.Drawing.Point(367, 4);
            this.chkNearGround.Margin = new System.Windows.Forms.Padding(4);
            this.chkNearGround.Name = "chkNearGround";
            this.chkNearGround.Size = new System.Drawing.Size(94, 20);
            this.chkNearGround.TabIndex = 172;
            this.chkNearGround.Text = "Near Ground";
            this.chkNearGround.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNearGround.CheckedChanged += new System.EventHandler(this.chkNearGround_CheckedChanged);
            // 
            // chkFXResistable
            // 
            this.chkFXResistable.AutoSize = true;
            this.chkFXResistable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.Location = new System.Drawing.Point(272, 4);
            this.chkFXResistable.Margin = new System.Windows.Forms.Padding(4);
            this.chkFXResistable.Name = "chkFXResistable";
            this.chkFXResistable.Size = new System.Drawing.Size(87, 20);
            this.chkFXResistable.TabIndex = 170;
            this.chkFXResistable.Text = "Unresistible";
            this.chkFXResistable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.CheckedChanged += new System.EventHandler(this.chkFXResistible_CheckedChanged);
            // 
            // IgnoreED
            // 
            this.IgnoreED.AutoSize = true;
            this.IgnoreED.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.Location = new System.Drawing.Point(187, 4);
            this.IgnoreED.Margin = new System.Windows.Forms.Padding(4);
            this.IgnoreED.Name = "IgnoreED";
            this.IgnoreED.Size = new System.Drawing.Size(77, 20);
            this.IgnoreED.TabIndex = 173;
            this.IgnoreED.Text = "Ignore ED";
            this.IgnoreED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.CheckedChanged += new System.EventHandler(this.IgnoreED_CheckedChanged);
            // 
            // chkFXBuffable
            // 
            this.chkFXBuffable.AutoSize = true;
            this.chkFXBuffable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.Location = new System.Drawing.Point(89, 4);
            this.chkFXBuffable.Margin = new System.Windows.Forms.Padding(4);
            this.chkFXBuffable.Name = "chkFXBuffable";
            this.chkFXBuffable.Size = new System.Drawing.Size(90, 20);
            this.chkFXBuffable.TabIndex = 169;
            this.chkFXBuffable.Text = "Ignore Buffs";
            this.chkFXBuffable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.CheckedChanged += new System.EventHandler(this.chkFXBuffable_CheckedChanged);
            // 
            // chkCancelOnMiss
            // 
            this.chkCancelOnMiss.AutoSize = true;
            this.chkCancelOnMiss.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCancelOnMiss.Location = new System.Drawing.Point(469, 4);
            this.chkCancelOnMiss.Margin = new System.Windows.Forms.Padding(4);
            this.chkCancelOnMiss.Name = "chkCancelOnMiss";
            this.chkCancelOnMiss.Size = new System.Drawing.Size(105, 20);
            this.chkCancelOnMiss.TabIndex = 174;
            this.chkCancelOnMiss.Text = "Cancel on Miss";
            this.chkCancelOnMiss.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCancelOnMiss.CheckedChanged += new System.EventHandler(this.chkCancelOnMiss_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chkStack);
            this.flowLayoutPanel1.Controls.Add(this.chkFXBuffable);
            this.flowLayoutPanel1.Controls.Add(this.IgnoreED);
            this.flowLayoutPanel1.Controls.Add(this.chkFXResistable);
            this.flowLayoutPanel1.Controls.Add(this.chkNearGround);
            this.flowLayoutPanel1.Controls.Add(this.chkCancelOnMiss);
            this.flowLayoutPanel1.Controls.Add(this.chkRqToHitCheck);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(493, 564);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(746, 31);
            this.flowLayoutPanel1.TabIndex = 185;
            // 
            // chkRqToHitCheck
            // 
            this.chkRqToHitCheck.AutoSize = true;
            this.chkRqToHitCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRqToHitCheck.Location = new System.Drawing.Point(582, 4);
            this.chkRqToHitCheck.Margin = new System.Windows.Forms.Padding(4);
            this.chkRqToHitCheck.Name = "chkRqToHitCheck";
            this.chkRqToHitCheck.Size = new System.Drawing.Size(132, 20);
            this.chkRqToHitCheck.TabIndex = 175;
            this.chkRqToHitCheck.Text = "Require ToHit Check";
            this.chkRqToHitCheck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRqToHitCheck.CheckedChanged += new System.EventHandler(this.chkRqToHitCheck_CheckedChanged);
            // 
            // chkIgnoreScale
            // 
            this.chkIgnoreScale.Location = new System.Drawing.Point(1371, 86);
            this.chkIgnoreScale.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreScale.Name = "chkIgnoreScale";
            this.chkIgnoreScale.Size = new System.Drawing.Size(203, 20);
            this.chkIgnoreScale.TabIndex = 189;
            this.chkIgnoreScale.Text = "Ignore Scaling For Effect";
            this.chkIgnoreScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIgnoreScale.UseVisualStyleBackColor = true;
            this.chkIgnoreScale.CheckedChanged += new System.EventHandler(this.chkIgnoreScale_CheckChanged);
            // 
            // Label28
            // 
            this.Label28.Location = new System.Drawing.Point(1325, 15);
            this.Label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(94, 27);
            this.Label28.TabIndex = 187;
            this.Label28.Text = "Display Priority:";
            this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbFXClass
            // 
            this.cbFXClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXClass.Location = new System.Drawing.Point(1427, 16);
            this.cbFXClass.Margin = new System.Windows.Forms.Padding(4);
            this.cbFXClass.Name = "cbFXClass";
            this.cbFXClass.Size = new System.Drawing.Size(190, 24);
            this.cbFXClass.TabIndex = 186;
            this.cbFXClass.SelectedIndexChanged += new System.EventHandler(this.cbFXClass_SelectedIndexChanged);
            // 
            // chkVariable
            // 
            this.chkVariable.Location = new System.Drawing.Point(1371, 55);
            this.chkVariable.Margin = new System.Windows.Forms.Padding(4);
            this.chkVariable.Name = "chkVariable";
            this.chkVariable.Size = new System.Drawing.Size(203, 23);
            this.chkVariable.TabIndex = 188;
            this.chkVariable.Text = "Enable Power Scaling Override";
            this.chkVariable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVariable.CheckedChanged += new System.EventHandler(this.chkVariable_CheckedChanged);
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(1391, 537);
            this.Label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(64, 23);
            this.Label10.TabIndex = 190;
            this.Label10.Text = "Override:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtOverride
            // 
            this.txtOverride.Location = new System.Drawing.Point(1391, 564);
            this.txtOverride.Margin = new System.Windows.Forms.Padding(4);
            this.txtOverride.Name = "txtOverride";
            this.txtOverride.Size = new System.Drawing.Size(219, 23);
            this.txtOverride.TabIndex = 191;
            this.txtOverride.TextChanged += new System.EventHandler(this.txtOverride_TextChanged);
            // 
            // cmbEffectId
            // 
            this.cmbEffectId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEffectId.Location = new System.Drawing.Point(1391, 506);
            this.cmbEffectId.Margin = new System.Windows.Forms.Padding(4);
            this.cmbEffectId.Name = "cmbEffectId";
            this.cmbEffectId.Size = new System.Drawing.Size(219, 24);
            this.cmbEffectId.TabIndex = 195;
            this.cmbEffectId.SelectedIndexChanged += new System.EventHandler(this.cmbEffectId_SelectedIndexChanged);
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(1391, 479);
            this.Label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(145, 23);
            this.Label9.TabIndex = 194;
            this.Label9.Text = "GlobalChanceMod Flag:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label30
            // 
            this.Label30.Location = new System.Drawing.Point(1391, 607);
            this.Label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(167, 25);
            this.Label30.TabIndex = 193;
            this.Label30.Text = "Old Special Case:";
            this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbFXSpecialCase
            // 
            this.cbFXSpecialCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXSpecialCase.Location = new System.Drawing.Point(1390, 635);
            this.cbFXSpecialCase.Margin = new System.Windows.Forms.Padding(4);
            this.cbFXSpecialCase.Name = "cbFXSpecialCase";
            this.cbFXSpecialCase.Size = new System.Drawing.Size(219, 24);
            this.cbFXSpecialCase.TabIndex = 192;
            this.cbFXSpecialCase.SelectedIndexChanged += new System.EventHandler(this.cbFXSpecialCase_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Arial", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(4, 6);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(209, 18);
            this.label13.TabIndex = 181;
            this.label13.Text = "Suppress Effect (When)";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clbSuppression
            // 
            this.clbSuppression.CheckOnClick = true;
            this.clbSuppression.FormattingEnabled = true;
            this.clbSuppression.Location = new System.Drawing.Point(7, 28);
            this.clbSuppression.Margin = new System.Windows.Forms.Padding(4);
            this.clbSuppression.Name = "clbSuppression";
            this.clbSuppression.Size = new System.Drawing.Size(205, 292);
            this.clbSuppression.TabIndex = 0;
            this.clbSuppression.SelectedIndexChanged += new System.EventHandler(this.clbSuppression_SelectedIndexChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label13);
            this.panel4.Controls.Add(this.clbSuppression);
            this.panel4.Location = new System.Drawing.Point(1391, 124);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(219, 351);
            this.panel4.TabIndex = 196;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(1134, 620);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(4);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(219, 32);
            this.btnCopy.TabIndex = 197;
            this.btnCopy.Text = "Copy Effect Data";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(1134, 656);
            this.btnPaste.Margin = new System.Windows.Forms.Padding(4);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(219, 32);
            this.btnPaste.TabIndex = 198;
            this.btnPaste.Text = "Paste Effect Data";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(763, 620);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 44);
            this.btnOK.TabIndex = 200;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(875, 620);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 44);
            this.btnCancel.TabIndex = 199;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEditConditions
            // 
            this.btnEditConditions.Location = new System.Drawing.Point(332, 656);
            this.btnEditConditions.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditConditions.Name = "btnEditConditions";
            this.btnEditConditions.Size = new System.Drawing.Size(266, 32);
            this.btnEditConditions.TabIndex = 205;
            this.btnEditConditions.Text = "Edit Effect Conditions";
            this.btnEditConditions.Click += new System.EventHandler(this.btnEditConditions_Click);
            // 
            // btnExprBuilder
            // 
            this.btnExprBuilder.Location = new System.Drawing.Point(332, 620);
            this.btnExprBuilder.Margin = new System.Windows.Forms.Padding(4);
            this.btnExprBuilder.Name = "btnExprBuilder";
            this.btnExprBuilder.Size = new System.Drawing.Size(266, 32);
            this.btnExprBuilder.TabIndex = 206;
            this.btnExprBuilder.Text = "Expression Builder";
            this.btnExprBuilder.Click += new System.EventHandler(this.btnExprBuilder_Click);
            // 
            // tpPowerAttribs
            // 
            this.tpPowerAttribs.ColumnCount = 2;
            this.tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136F));
            this.tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
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
            this.tpPowerAttribs.Location = new System.Drawing.Point(14, 124);
            this.tpPowerAttribs.Margin = new System.Windows.Forms.Padding(4);
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
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tpPowerAttribs.Size = new System.Drawing.Size(266, 408);
            this.tpPowerAttribs.TabIndex = 207;
            this.tpPowerAttribs.Visible = false;
            // 
            // txtMaxTargets
            // 
            this.txtMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMaxTargets.Location = new System.Drawing.Point(4, 379);
            this.txtMaxTargets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtMaxTargets.Name = "txtMaxTargets";
            this.txtMaxTargets.Size = new System.Drawing.Size(128, 29);
            this.txtMaxTargets.TabIndex = 161;
            this.txtMaxTargets.Text = "Max Targets:";
            this.txtMaxTargets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtArc
            // 
            this.txtArc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtArc.Location = new System.Drawing.Point(4, 346);
            this.txtArc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtArc.Name = "txtArc";
            this.txtArc.Size = new System.Drawing.Size(128, 33);
            this.txtArc.TabIndex = 156;
            this.txtArc.Text = "Arc:";
            this.txtArc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEffectArea
            // 
            this.txtEffectArea.AutoSize = true;
            this.txtEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEffectArea.Location = new System.Drawing.Point(4, 0);
            this.txtEffectArea.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtEffectArea.Name = "txtEffectArea";
            this.txtEffectArea.Size = new System.Drawing.Size(128, 32);
            this.txtEffectArea.TabIndex = 128;
            this.txtEffectArea.Text = "Effect Area:";
            this.txtEffectArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFXEffectArea
            // 
            this.cbFXEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFXEffectArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXEffectArea.Location = new System.Drawing.Point(140, 4);
            this.cbFXEffectArea.Margin = new System.Windows.Forms.Padding(4);
            this.cbFXEffectArea.Name = "cbFXEffectArea";
            this.cbFXEffectArea.Size = new System.Drawing.Size(122, 24);
            this.cbFXEffectArea.TabIndex = 127;
            this.cbFXEffectArea.SelectedIndexChanged += new System.EventHandler(this.cbFXEffectArea_SelectedIndexChanged);
            // 
            // txtFXAccuracy
            // 
            this.txtFXAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXAccuracy.Location = new System.Drawing.Point(140, 36);
            this.txtFXAccuracy.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXAccuracy.Name = "txtFXAccuracy";
            this.txtFXAccuracy.Size = new System.Drawing.Size(122, 23);
            this.txtFXAccuracy.TabIndex = 129;
            this.txtFXAccuracy.Text = "0";
            this.txtFXAccuracy.TextChanged += new System.EventHandler(this.txtFXAccuracy_TextChanged);
            // 
            // txtRange
            // 
            this.txtRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRange.Location = new System.Drawing.Point(4, 238);
            this.txtRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(128, 36);
            this.txtRange.TabIndex = 155;
            this.txtRange.Text = "Range:";
            this.txtRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccuracy
            // 
            this.txtAccuracy.AutoSize = true;
            this.txtAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAccuracy.Location = new System.Drawing.Point(4, 32);
            this.txtAccuracy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtAccuracy.Name = "txtAccuracy";
            this.txtAccuracy.Size = new System.Drawing.Size(128, 31);
            this.txtAccuracy.TabIndex = 130;
            this.txtAccuracy.Text = "Accuracy:";
            this.txtAccuracy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCastTime
            // 
            this.txtCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCastTime.Location = new System.Drawing.Point(4, 94);
            this.txtCastTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtCastTime.Name = "txtCastTime";
            this.txtCastTime.Size = new System.Drawing.Size(128, 36);
            this.txtCastTime.TabIndex = 97;
            this.txtCastTime.Text = "Casting Time:";
            this.txtCastTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXRange
            // 
            this.txtFXRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRange.Location = new System.Drawing.Point(140, 242);
            this.txtFXRange.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXRange.Name = "txtFXRange";
            this.txtFXRange.Size = new System.Drawing.Size(122, 23);
            this.txtFXRange.TabIndex = 154;
            this.txtFXRange.Text = "0";
            this.txtFXRange.TextChanged += new System.EventHandler(this.txtFXRange_TextChanged);
            // 
            // txtInterruptTime
            // 
            this.txtInterruptTime.AutoSize = true;
            this.txtInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInterruptTime.Location = new System.Drawing.Point(4, 63);
            this.txtInterruptTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtInterruptTime.Name = "txtInterruptTime";
            this.txtInterruptTime.Size = new System.Drawing.Size(128, 31);
            this.txtInterruptTime.TabIndex = 98;
            this.txtInterruptTime.Text = "Interruptable Time:";
            this.txtInterruptTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXInterruptTime
            // 
            this.txtFXInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXInterruptTime.Location = new System.Drawing.Point(140, 67);
            this.txtFXInterruptTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXInterruptTime.Name = "txtFXInterruptTime";
            this.txtFXInterruptTime.Size = new System.Drawing.Size(122, 23);
            this.txtFXInterruptTime.TabIndex = 82;
            this.txtFXInterruptTime.Text = "0";
            this.txtFXInterruptTime.TextChanged += new System.EventHandler(this.txtFXInterruptTime_TextChanged);
            // 
            // txtRechargeTime
            // 
            this.txtRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRechargeTime.Location = new System.Drawing.Point(4, 130);
            this.txtRechargeTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtRechargeTime.Name = "txtRechargeTime";
            this.txtRechargeTime.Size = new System.Drawing.Size(128, 36);
            this.txtRechargeTime.TabIndex = 99;
            this.txtRechargeTime.Text = "Recharge Time:";
            this.txtRechargeTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXCastTime
            // 
            this.txtFXCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXCastTime.Location = new System.Drawing.Point(140, 98);
            this.txtFXCastTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXCastTime.Name = "txtFXCastTime";
            this.txtFXCastTime.Size = new System.Drawing.Size(122, 23);
            this.txtFXCastTime.TabIndex = 80;
            this.txtFXCastTime.Text = "0";
            this.txtFXCastTime.TextChanged += new System.EventHandler(this.txtFXCastTime_TextChanged);
            // 
            // txtFXRechargeTime
            // 
            this.txtFXRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRechargeTime.Location = new System.Drawing.Point(140, 134);
            this.txtFXRechargeTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXRechargeTime.Name = "txtFXRechargeTime";
            this.txtFXRechargeTime.Size = new System.Drawing.Size(122, 23);
            this.txtFXRechargeTime.TabIndex = 83;
            this.txtFXRechargeTime.Text = "0";
            this.txtFXRechargeTime.TextChanged += new System.EventHandler(this.txtFXRechargeTime_TextChanged);
            // 
            // txtActivateInterval
            // 
            this.txtActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtActivateInterval.Location = new System.Drawing.Point(4, 166);
            this.txtActivateInterval.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtActivateInterval.Name = "txtActivateInterval";
            this.txtActivateInterval.Size = new System.Drawing.Size(128, 36);
            this.txtActivateInterval.TabIndex = 100;
            this.txtActivateInterval.Text = "Activate Interval:";
            this.txtActivateInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXActivateInterval
            // 
            this.txtFXActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXActivateInterval.Location = new System.Drawing.Point(140, 170);
            this.txtFXActivateInterval.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXActivateInterval.Name = "txtFXActivateInterval";
            this.txtFXActivateInterval.Size = new System.Drawing.Size(122, 23);
            this.txtFXActivateInterval.TabIndex = 84;
            this.txtFXActivateInterval.Text = "0";
            this.txtFXActivateInterval.TextChanged += new System.EventHandler(this.txtFXActivateInterval_TextChanged);
            // 
            // txtEnduranceCost
            // 
            this.txtEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnduranceCost.Location = new System.Drawing.Point(4, 202);
            this.txtEnduranceCost.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtEnduranceCost.Name = "txtEnduranceCost";
            this.txtEnduranceCost.Size = new System.Drawing.Size(128, 36);
            this.txtEnduranceCost.TabIndex = 101;
            this.txtEnduranceCost.Text = "Endurance Cost:";
            this.txtEnduranceCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXEnduranceCost
            // 
            this.txtFXEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXEnduranceCost.Location = new System.Drawing.Point(140, 206);
            this.txtFXEnduranceCost.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXEnduranceCost.Name = "txtFXEnduranceCost";
            this.txtFXEnduranceCost.Size = new System.Drawing.Size(122, 23);
            this.txtFXEnduranceCost.TabIndex = 85;
            this.txtFXEnduranceCost.Text = "1";
            this.txtFXEnduranceCost.TextChanged += new System.EventHandler(this.txtFXEnduranceCost_TextChanged);
            // 
            // txtSecondaryRange
            // 
            this.txtSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSecondaryRange.Location = new System.Drawing.Point(4, 274);
            this.txtSecondaryRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtSecondaryRange.Name = "txtSecondaryRange";
            this.txtSecondaryRange.Size = new System.Drawing.Size(128, 36);
            this.txtSecondaryRange.TabIndex = 134;
            this.txtSecondaryRange.Text = "Secondary Range:";
            this.txtSecondaryRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRadius
            // 
            this.txtRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRadius.Location = new System.Drawing.Point(4, 310);
            this.txtRadius.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(128, 36);
            this.txtRadius.TabIndex = 136;
            this.txtRadius.Text = "Radius:";
            this.txtRadius.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXSecondaryRange
            // 
            this.txtFXSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXSecondaryRange.Location = new System.Drawing.Point(140, 278);
            this.txtFXSecondaryRange.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXSecondaryRange.Name = "txtFXSecondaryRange";
            this.txtFXSecondaryRange.Size = new System.Drawing.Size(122, 23);
            this.txtFXSecondaryRange.TabIndex = 157;
            this.txtFXSecondaryRange.Text = "0";
            this.txtFXSecondaryRange.TextChanged += new System.EventHandler(this.txtFXSecondaryRange_TextChanged);
            // 
            // txtFXRadius
            // 
            this.txtFXRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXRadius.Location = new System.Drawing.Point(140, 314);
            this.txtFXRadius.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXRadius.Name = "txtFXRadius";
            this.txtFXRadius.Size = new System.Drawing.Size(122, 23);
            this.txtFXRadius.TabIndex = 158;
            this.txtFXRadius.Text = "0";
            this.txtFXRadius.TextChanged += new System.EventHandler(this.txtFXRadius_TextChanged);
            // 
            // txtFXArc
            // 
            this.txtFXArc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXArc.Location = new System.Drawing.Point(140, 350);
            this.txtFXArc.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXArc.Name = "txtFXArc";
            this.txtFXArc.Size = new System.Drawing.Size(122, 23);
            this.txtFXArc.TabIndex = 159;
            this.txtFXArc.Text = "0";
            this.txtFXArc.TextChanged += new System.EventHandler(this.txtFXArc_TextChanged);
            // 
            // txtFXMaxTargets
            // 
            this.txtFXMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFXMaxTargets.Location = new System.Drawing.Point(140, 383);
            this.txtFXMaxTargets.Margin = new System.Windows.Forms.Padding(4);
            this.txtFXMaxTargets.Name = "txtFXMaxTargets";
            this.txtFXMaxTargets.Size = new System.Drawing.Size(122, 23);
            this.txtFXMaxTargets.TabIndex = 162;
            this.txtFXMaxTargets.Text = "0";
            this.txtFXMaxTargets.TextChanged += new System.EventHandler(this.txtFXMaxTargets_TextChanged);
            // 
            // frmPowerEffect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1623, 698);
            this.Controls.Add(this.tpPowerAttribs);
            this.Controls.Add(this.btnExprBuilder);
            this.Controls.Add(this.btnEditConditions);
            this.Controls.Add(this.chkIgnoreScale);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.cmbEffectId);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.Label30);
            this.Controls.Add(this.cbFXSpecialCase);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.txtOverride);
            this.Controls.Add(this.chkVariable);
            this.Controls.Add(this.Label28);
            this.Controls.Add(this.cbFXClass);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.lvSubSub);
            this.Controls.Add(this.lvSubAttribute);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.lvEffectType);
            this.Controls.Add(this.cbModifier);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cbCoDFormat);
            this.Controls.Add(this.lblEffectDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPowerEffect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmPowerEffect2";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.tpPowerAttribs.ResumeLayout(false);
            this.tpPowerAttribs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbCoDFormat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.ComboBox cbPercentageOverride;
        private System.Windows.Forms.TextBox txtFXScale;
        private System.Windows.Forms.Label Label11;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label22;
        private System.Windows.Forms.TextBox txtPPM;
        private System.Windows.Forms.Label Label23;
        private System.Windows.Forms.TextBox txtFXDuration;
        private System.Windows.Forms.Label Label24;
        private System.Windows.Forms.TextBox txtFXMag;
        private System.Windows.Forms.TextBox txtFXTicks;
        private System.Windows.Forms.Label Label25;
        private System.Windows.Forms.TextBox txtFXDelay;
        private System.Windows.Forms.Label Label26;
        private System.Windows.Forms.TextBox txtFXProb;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.ComboBox cbAttribute;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.ComboBox cbAspect;
        private ctlListViewColored lvSubSub;
        private System.Windows.Forms.ColumnHeader chSubSub;
        private ctlListViewColored lvSubAttribute;
        private System.Windows.Forms.ColumnHeader chSub;
        private ctlListViewColored lvEffectType;
        private System.Windows.Forms.ColumnHeader ColumnHeader1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ComboBox cbAffects;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Label lblAffectsCaster;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.ComboBox cbModifier;
        private System.Windows.Forms.CheckBox chkStack;
        private System.Windows.Forms.CheckBox chkNearGround;
        private System.Windows.Forms.CheckBox chkFXResistable;
        private System.Windows.Forms.CheckBox IgnoreED;
        private System.Windows.Forms.CheckBox chkFXBuffable;
        private System.Windows.Forms.CheckBox chkCancelOnMiss;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label Label28;
        private System.Windows.Forms.ComboBox cbFXClass;
        private System.Windows.Forms.CheckBox chkVariable;
        private System.Windows.Forms.CheckBox chkIgnoreScale;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.TextBox txtOverride;
        private System.Windows.Forms.ComboBox cmbEffectId;
        private System.Windows.Forms.Label Label9;
        private System.Windows.Forms.Label Label30;
        private System.Windows.Forms.ComboBox cbFXSpecialCase;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckedListBox clbSuppression;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbTarget;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnEditConditions;
        private System.Windows.Forms.Button btnExprBuilder;
        private System.Windows.Forms.TableLayoutPanel tpPowerAttribs;
        private System.Windows.Forms.Label txtMaxTargets;
        private System.Windows.Forms.Label txtArc;
        private System.Windows.Forms.Label txtEffectArea;
        private System.Windows.Forms.ComboBox cbFXEffectArea;
        private System.Windows.Forms.TextBox txtFXAccuracy;
        private System.Windows.Forms.Label txtRange;
        private System.Windows.Forms.Label txtAccuracy;
        private System.Windows.Forms.Label txtCastTime;
        private System.Windows.Forms.TextBox txtFXRange;
        private System.Windows.Forms.Label txtInterruptTime;
        private System.Windows.Forms.TextBox txtFXInterruptTime;
        private System.Windows.Forms.Label txtRechargeTime;
        private System.Windows.Forms.TextBox txtFXCastTime;
        private System.Windows.Forms.TextBox txtFXRechargeTime;
        private System.Windows.Forms.Label txtActivateInterval;
        private System.Windows.Forms.TextBox txtFXActivateInterval;
        private System.Windows.Forms.Label txtEnduranceCost;
        private System.Windows.Forms.TextBox txtFXEnduranceCost;
        private System.Windows.Forms.Label txtSecondaryRange;
        private System.Windows.Forms.Label txtRadius;
        private System.Windows.Forms.TextBox txtFXSecondaryRange;
        private System.Windows.Forms.TextBox txtFXRadius;
        private System.Windows.Forms.TextBox txtFXArc;
        private System.Windows.Forms.TextBox txtFXMaxTargets;
        private System.Windows.Forms.Label lblEffectDescription;
        private System.Windows.Forms.CheckBox chkRqToHitCheck;
    }
}
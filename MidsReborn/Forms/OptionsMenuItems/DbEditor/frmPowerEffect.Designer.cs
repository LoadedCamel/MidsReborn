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
            cbCoDFormat = new System.Windows.Forms.CheckBox();
            lblEffectDescription = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            Label2 = new System.Windows.Forms.Label();
            cbPercentageOverride = new System.Windows.Forms.ComboBox();
            txtFXScale = new System.Windows.Forms.TextBox();
            Label11 = new System.Windows.Forms.Label();
            Label1 = new System.Windows.Forms.Label();
            Label22 = new System.Windows.Forms.Label();
            txtPPM = new System.Windows.Forms.TextBox();
            Label23 = new System.Windows.Forms.Label();
            txtFXDuration = new System.Windows.Forms.TextBox();
            Label24 = new System.Windows.Forms.Label();
            txtFXMag = new System.Windows.Forms.TextBox();
            txtFXTicks = new System.Windows.Forms.TextBox();
            Label25 = new System.Windows.Forms.Label();
            txtFXDelay = new System.Windows.Forms.TextBox();
            Label26 = new System.Windows.Forms.Label();
            txtFXProb = new System.Windows.Forms.TextBox();
            Label4 = new System.Windows.Forms.Label();
            cbAttribute = new System.Windows.Forms.ComboBox();
            Label5 = new System.Windows.Forms.Label();
            cbAspect = new System.Windows.Forms.ComboBox();
            lvSubSub = new Mids_Reborn.Controls.ctlListViewColored();
            chSubSub = new System.Windows.Forms.ColumnHeader();
            lvSubAttribute = new Mids_Reborn.Controls.ctlListViewColored();
            chSub = new System.Windows.Forms.ColumnHeader();
            lvEffectType = new Mids_Reborn.Controls.ctlListViewColored();
            ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            cbTarget = new System.Windows.Forms.ComboBox();
            lblAffectsCaster = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            Label3 = new System.Windows.Forms.Label();
            cbAffects = new System.Windows.Forms.ComboBox();
            Label6 = new System.Windows.Forms.Label();
            cbModifier = new System.Windows.Forms.ComboBox();
            chkStack = new System.Windows.Forms.CheckBox();
            chkNearGround = new System.Windows.Forms.CheckBox();
            chkFXResistable = new System.Windows.Forms.CheckBox();
            IgnoreED = new System.Windows.Forms.CheckBox();
            chkFXBuffable = new System.Windows.Forms.CheckBox();
            chkCancelOnMiss = new System.Windows.Forms.CheckBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            chkRqToHitCheck = new System.Windows.Forms.CheckBox();
            chkIgnoreScale = new System.Windows.Forms.CheckBox();
            Label28 = new System.Windows.Forms.Label();
            cbFXClass = new System.Windows.Forms.ComboBox();
            chkVariable = new System.Windows.Forms.CheckBox();
            Label10 = new System.Windows.Forms.Label();
            txtOverride = new System.Windows.Forms.TextBox();
            cmbEffectId = new System.Windows.Forms.ComboBox();
            Label9 = new System.Windows.Forms.Label();
            Label30 = new System.Windows.Forms.Label();
            cbFXSpecialCase = new System.Windows.Forms.ComboBox();
            label13 = new System.Windows.Forms.Label();
            clbSuppression = new System.Windows.Forms.CheckedListBox();
            panel4 = new System.Windows.Forms.Panel();
            btnCopy = new System.Windows.Forms.Button();
            btnPaste = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnEditConditions = new System.Windows.Forms.Button();
            btnExprBuilder = new System.Windows.Forms.Button();
            tpPowerAttribs = new System.Windows.Forms.TableLayoutPanel();
            txtMaxTargets = new System.Windows.Forms.Label();
            txtArc = new System.Windows.Forms.Label();
            txtEffectArea = new System.Windows.Forms.Label();
            cbFXEffectArea = new System.Windows.Forms.ComboBox();
            txtFXAccuracy = new System.Windows.Forms.TextBox();
            txtRange = new System.Windows.Forms.Label();
            txtAccuracy = new System.Windows.Forms.Label();
            txtCastTime = new System.Windows.Forms.Label();
            txtFXRange = new System.Windows.Forms.TextBox();
            txtInterruptTime = new System.Windows.Forms.Label();
            txtFXInterruptTime = new System.Windows.Forms.TextBox();
            txtRechargeTime = new System.Windows.Forms.Label();
            txtFXCastTime = new System.Windows.Forms.TextBox();
            txtFXRechargeTime = new System.Windows.Forms.TextBox();
            txtActivateInterval = new System.Windows.Forms.Label();
            txtFXActivateInterval = new System.Windows.Forms.TextBox();
            txtEnduranceCost = new System.Windows.Forms.Label();
            txtFXEnduranceCost = new System.Windows.Forms.TextBox();
            txtSecondaryRange = new System.Windows.Forms.Label();
            txtRadius = new System.Windows.Forms.Label();
            txtFXSecondaryRange = new System.Windows.Forms.TextBox();
            txtFXRadius = new System.Windows.Forms.TextBox();
            txtFXArc = new System.Windows.Forms.TextBox();
            txtFXMaxTargets = new System.Windows.Forms.TextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            panel4.SuspendLayout();
            tpPowerAttribs.SuspendLayout();
            SuspendLayout();
            // 
            // cbCoDFormat
            // 
            cbCoDFormat.AutoSize = true;
            cbCoDFormat.BackColor = System.Drawing.Color.Transparent;
            cbCoDFormat.Location = new System.Drawing.Point(1152, 15);
            cbCoDFormat.Margin = new System.Windows.Forms.Padding(4);
            cbCoDFormat.Name = "cbCoDFormat";
            cbCoDFormat.Size = new System.Drawing.Size(143, 19);
            cbCoDFormat.TabIndex = 177;
            cbCoDFormat.Text = "Use CoD effect format";
            cbCoDFormat.UseVisualStyleBackColor = false;
            cbCoDFormat.CheckedChanged += cbCoDFormat_CheckedChanged;
            // 
            // lblEffectDescription
            // 
            lblEffectDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            lblEffectDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            lblEffectDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            lblEffectDescription.Location = new System.Drawing.Point(14, 11);
            lblEffectDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEffectDescription.Name = "lblEffectDescription";
            lblEffectDescription.Size = new System.Drawing.Size(1304, 110);
            lblEffectDescription.TabIndex = 176;
            lblEffectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblEffectDescription.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            tableLayoutPanel1.Controls.Add(Label2, 0, 0);
            tableLayoutPanel1.Controls.Add(cbPercentageOverride, 1, 0);
            tableLayoutPanel1.Controls.Add(txtFXScale, 1, 1);
            tableLayoutPanel1.Controls.Add(Label11, 0, 7);
            tableLayoutPanel1.Controls.Add(Label1, 0, 1);
            tableLayoutPanel1.Controls.Add(Label22, 0, 3);
            tableLayoutPanel1.Controls.Add(txtPPM, 1, 7);
            tableLayoutPanel1.Controls.Add(Label23, 0, 2);
            tableLayoutPanel1.Controls.Add(txtFXDuration, 1, 2);
            tableLayoutPanel1.Controls.Add(Label24, 0, 4);
            tableLayoutPanel1.Controls.Add(txtFXMag, 1, 3);
            tableLayoutPanel1.Controls.Add(txtFXTicks, 1, 4);
            tableLayoutPanel1.Controls.Add(Label25, 0, 5);
            tableLayoutPanel1.Controls.Add(txtFXDelay, 1, 5);
            tableLayoutPanel1.Controls.Add(Label26, 0, 6);
            tableLayoutPanel1.Controls.Add(txtFXProb, 1, 6);
            tableLayoutPanel1.Controls.Add(Label4, 0, 8);
            tableLayoutPanel1.Controls.Add(cbAttribute, 1, 8);
            tableLayoutPanel1.Controls.Add(Label5, 0, 9);
            tableLayoutPanel1.Controls.Add(cbAspect, 1, 9);
            tableLayoutPanel1.Location = new System.Drawing.Point(14, 124);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new System.Drawing.Size(266, 338);
            tableLayoutPanel1.TabIndex = 178;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Dock = System.Windows.Forms.DockStyle.Fill;
            Label2.Location = new System.Drawing.Point(4, 0);
            Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(88, 33);
            Label2.TabIndex = 128;
            Label2.Text = "Percentage:";
            Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPercentageOverride
            // 
            cbPercentageOverride.Dock = System.Windows.Forms.DockStyle.Fill;
            cbPercentageOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPercentageOverride.Location = new System.Drawing.Point(100, 4);
            cbPercentageOverride.Margin = new System.Windows.Forms.Padding(4);
            cbPercentageOverride.Name = "cbPercentageOverride";
            cbPercentageOverride.Size = new System.Drawing.Size(162, 23);
            cbPercentageOverride.TabIndex = 127;
            cbPercentageOverride.SelectedIndexChanged += cbPercentageOverride_SelectedIndexChanged;
            // 
            // txtFXScale
            // 
            txtFXScale.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXScale.Location = new System.Drawing.Point(100, 37);
            txtFXScale.Margin = new System.Windows.Forms.Padding(4);
            txtFXScale.Name = "txtFXScale";
            txtFXScale.Size = new System.Drawing.Size(162, 23);
            txtFXScale.TabIndex = 129;
            txtFXScale.Text = "0";
            txtFXScale.TextChanged += txtFXScale_TextChanged;
            txtFXScale.Leave += txtFXScale_Leave;
            // 
            // Label11
            // 
            Label11.Dock = System.Windows.Forms.DockStyle.Fill;
            Label11.Location = new System.Drawing.Point(4, 231);
            Label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label11.Name = "Label11";
            Label11.Size = new System.Drawing.Size(88, 33);
            Label11.TabIndex = 155;
            Label11.Text = "PPM:";
            Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            Label1.Location = new System.Drawing.Point(4, 33);
            Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(88, 33);
            Label1.TabIndex = 130;
            Label1.Text = "Scale:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label22
            // 
            Label22.Dock = System.Windows.Forms.DockStyle.Fill;
            Label22.Location = new System.Drawing.Point(4, 99);
            Label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label22.Name = "Label22";
            Label22.Size = new System.Drawing.Size(88, 33);
            Label22.TabIndex = 97;
            Label22.Text = "Magnitude:";
            Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPPM
            // 
            txtPPM.Dock = System.Windows.Forms.DockStyle.Fill;
            txtPPM.Location = new System.Drawing.Point(100, 235);
            txtPPM.Margin = new System.Windows.Forms.Padding(4);
            txtPPM.Name = "txtPPM";
            txtPPM.Size = new System.Drawing.Size(162, 23);
            txtPPM.TabIndex = 154;
            txtPPM.Text = "0";
            txtPPM.TextChanged += txtPPM_TextChanged;
            txtPPM.Leave += txtPPM_Leave;
            // 
            // Label23
            // 
            Label23.AutoSize = true;
            Label23.Dock = System.Windows.Forms.DockStyle.Fill;
            Label23.Location = new System.Drawing.Point(4, 66);
            Label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label23.Name = "Label23";
            Label23.Size = new System.Drawing.Size(88, 33);
            Label23.TabIndex = 98;
            Label23.Text = "Duration:";
            Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDuration
            // 
            txtFXDuration.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXDuration.Location = new System.Drawing.Point(100, 70);
            txtFXDuration.Margin = new System.Windows.Forms.Padding(4);
            txtFXDuration.Name = "txtFXDuration";
            txtFXDuration.Size = new System.Drawing.Size(162, 23);
            txtFXDuration.TabIndex = 82;
            txtFXDuration.Text = "0";
            txtFXDuration.TextChanged += txtFXDuration_TextChanged;
            txtFXDuration.Leave += txtFXDuration_Leave;
            // 
            // Label24
            // 
            Label24.Dock = System.Windows.Forms.DockStyle.Fill;
            Label24.Location = new System.Drawing.Point(4, 132);
            Label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label24.Name = "Label24";
            Label24.Size = new System.Drawing.Size(88, 33);
            Label24.TabIndex = 99;
            Label24.Text = "Ticks:";
            Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXMag
            // 
            txtFXMag.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXMag.Location = new System.Drawing.Point(100, 103);
            txtFXMag.Margin = new System.Windows.Forms.Padding(4);
            txtFXMag.Name = "txtFXMag";
            txtFXMag.Size = new System.Drawing.Size(162, 23);
            txtFXMag.TabIndex = 80;
            txtFXMag.Text = "0";
            txtFXMag.TextChanged += txtFXMag_TextChanged;
            txtFXMag.Leave += txtFXMag_Leave;
            // 
            // txtFXTicks
            // 
            txtFXTicks.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXTicks.Location = new System.Drawing.Point(100, 136);
            txtFXTicks.Margin = new System.Windows.Forms.Padding(4);
            txtFXTicks.Name = "txtFXTicks";
            txtFXTicks.Size = new System.Drawing.Size(162, 23);
            txtFXTicks.TabIndex = 83;
            txtFXTicks.Text = "0";
            txtFXTicks.TextChanged += txtFXTicks_TextChanged;
            txtFXTicks.Leave += txtFXTicks_Leave;
            // 
            // Label25
            // 
            Label25.Dock = System.Windows.Forms.DockStyle.Fill;
            Label25.Location = new System.Drawing.Point(4, 165);
            Label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label25.Name = "Label25";
            Label25.Size = new System.Drawing.Size(88, 33);
            Label25.TabIndex = 100;
            Label25.Text = "Delay Time:";
            Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDelay
            // 
            txtFXDelay.Location = new System.Drawing.Point(100, 169);
            txtFXDelay.Margin = new System.Windows.Forms.Padding(4);
            txtFXDelay.Name = "txtFXDelay";
            txtFXDelay.Size = new System.Drawing.Size(75, 23);
            txtFXDelay.TabIndex = 84;
            txtFXDelay.Text = "0";
            txtFXDelay.TextChanged += txtFXDelay_TextChanged;
            txtFXDelay.Leave += txtFXDelay_Leave;
            // 
            // Label26
            // 
            Label26.Dock = System.Windows.Forms.DockStyle.Fill;
            Label26.Location = new System.Drawing.Point(4, 198);
            Label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label26.Name = "Label26";
            Label26.Size = new System.Drawing.Size(88, 33);
            Label26.TabIndex = 101;
            Label26.Text = "Probability %:";
            Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXProb
            // 
            txtFXProb.Location = new System.Drawing.Point(100, 202);
            txtFXProb.Margin = new System.Windows.Forms.Padding(4);
            txtFXProb.Name = "txtFXProb";
            txtFXProb.Size = new System.Drawing.Size(116, 23);
            txtFXProb.TabIndex = 156;
            txtFXProb.TextChanged += txtFXProb_TextChanged;
            txtFXProb.MouseLeave += txtFXProb_Leave;
            // 
            // Label4
            // 
            Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            Label4.Location = new System.Drawing.Point(4, 264);
            Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(88, 33);
            Label4.TabIndex = 134;
            Label4.Text = "AttribType:";
            Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAttribute
            // 
            cbAttribute.Dock = System.Windows.Forms.DockStyle.Fill;
            cbAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAttribute.Location = new System.Drawing.Point(100, 268);
            cbAttribute.Margin = new System.Windows.Forms.Padding(4);
            cbAttribute.Name = "cbAttribute";
            cbAttribute.Size = new System.Drawing.Size(162, 23);
            cbAttribute.TabIndex = 133;
            cbAttribute.SelectedIndexChanged += cbAttribute_SelectedIndexChanged;
            // 
            // Label5
            // 
            Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            Label5.Location = new System.Drawing.Point(4, 297);
            Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(88, 41);
            Label5.TabIndex = 136;
            Label5.Text = "Aspect:";
            Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAspect
            // 
            cbAspect.Dock = System.Windows.Forms.DockStyle.Fill;
            cbAspect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAspect.Location = new System.Drawing.Point(100, 301);
            cbAspect.Margin = new System.Windows.Forms.Padding(4);
            cbAspect.Name = "cbAspect";
            cbAspect.Size = new System.Drawing.Size(162, 23);
            cbAspect.TabIndex = 135;
            cbAspect.SelectedIndexChanged += cbAspect_SelectedIndexChanged;
            // 
            // lvSubSub
            // 
            lvSubSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chSubSub });
            lvSubSub.FullRowSelect = true;
            lvSubSub.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvSubSub.Location = new System.Drawing.Point(948, 124);
            lvSubSub.LostFocusItem = -1;
            lvSubSub.Margin = new System.Windows.Forms.Padding(4);
            lvSubSub.MultiSelect = false;
            lvSubSub.Name = "lvSubSub";
            lvSubSub.OwnerDraw = true;
            lvSubSub.ShowItemToolTips = true;
            lvSubSub.Size = new System.Drawing.Size(434, 431);
            lvSubSub.TabIndex = 181;
            lvSubSub.UseCompatibleStateImageBehavior = false;
            lvSubSub.View = System.Windows.Forms.View.Details;
            lvSubSub.DrawColumnHeader += ListView_DrawColumnHeader;
            lvSubSub.DrawItem += ListView_DrawItem;
            lvSubSub.SelectedIndexChanged += lvSubSub_SelectedIndexChanged;
            lvSubSub.Leave += ListView_Leave;
            // 
            // chSubSub
            // 
            chSubSub.Text = "Sub-Sub";
            chSubSub.Width = 254;
            // 
            // lvSubAttribute
            // 
            lvSubAttribute.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { chSub });
            lvSubAttribute.FullRowSelect = true;
            lvSubAttribute.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvSubAttribute.Location = new System.Drawing.Point(621, 124);
            lvSubAttribute.LostFocusItem = -1;
            lvSubAttribute.Margin = new System.Windows.Forms.Padding(4);
            lvSubAttribute.MultiSelect = false;
            lvSubAttribute.Name = "lvSubAttribute";
            lvSubAttribute.OwnerDraw = true;
            lvSubAttribute.Size = new System.Drawing.Size(320, 431);
            lvSubAttribute.TabIndex = 180;
            lvSubAttribute.UseCompatibleStateImageBehavior = false;
            lvSubAttribute.View = System.Windows.Forms.View.Details;
            lvSubAttribute.DrawColumnHeader += ListView_DrawColumnHeader;
            lvSubAttribute.DrawItem += ListView_DrawItem;
            lvSubAttribute.SelectedIndexChanged += lvSubAttribute_SelectedIndexChanged;
            lvSubAttribute.Leave += ListView_Leave;
            // 
            // chSub
            // 
            chSub.Text = "Sub-Attribute";
            chSub.Width = 254;
            // 
            // lvEffectType
            // 
            lvEffectType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ColumnHeader1 });
            lvEffectType.FullRowSelect = true;
            lvEffectType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvEffectType.Location = new System.Drawing.Point(287, 124);
            lvEffectType.LostFocusItem = -1;
            lvEffectType.Margin = new System.Windows.Forms.Padding(4);
            lvEffectType.MultiSelect = false;
            lvEffectType.Name = "lvEffectType";
            lvEffectType.OwnerDraw = true;
            lvEffectType.Size = new System.Drawing.Size(326, 431);
            lvEffectType.TabIndex = 179;
            lvEffectType.UseCompatibleStateImageBehavior = false;
            lvEffectType.View = System.Windows.Forms.View.Details;
            lvEffectType.DrawColumnHeader += ListView_DrawColumnHeader;
            lvEffectType.DrawItem += ListView_DrawItem;
            lvEffectType.SelectedIndexChanged += lvEffectType_SelectedIndexChanged;
            lvEffectType.Leave += ListView_Leave;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Effect Attribute";
            ColumnHeader1.Width = 202;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 162F));
            tableLayoutPanel3.Controls.Add(cbTarget, 1, 2);
            tableLayoutPanel3.Controls.Add(lblAffectsCaster, 1, 1);
            tableLayoutPanel3.Controls.Add(label12, 0, 2);
            tableLayoutPanel3.Controls.Add(Label3, 0, 0);
            tableLayoutPanel3.Controls.Add(cbAffects, 1, 0);
            tableLayoutPanel3.Location = new System.Drawing.Point(14, 588);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel3.Size = new System.Drawing.Size(266, 98);
            tableLayoutPanel3.TabIndex = 182;
            // 
            // cbTarget
            // 
            cbTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            cbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTarget.Items.AddRange(new object[] { "Any", "Mobs", "Players" });
            cbTarget.Location = new System.Drawing.Point(108, 68);
            cbTarget.Margin = new System.Windows.Forms.Padding(4);
            cbTarget.Name = "cbTarget";
            cbTarget.Size = new System.Drawing.Size(154, 23);
            cbTarget.TabIndex = 201;
            cbTarget.SelectedIndexChanged += cbTarget_IndexChanged;
            // 
            // lblAffectsCaster
            // 
            lblAffectsCaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            lblAffectsCaster.Location = new System.Drawing.Point(108, 32);
            lblAffectsCaster.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAffectsCaster.Name = "lblAffectsCaster";
            lblAffectsCaster.Size = new System.Drawing.Size(154, 32);
            lblAffectsCaster.TabIndex = 141;
            lblAffectsCaster.Text = "Power also affects caster";
            lblAffectsCaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.Dock = System.Windows.Forms.DockStyle.Fill;
            label12.Location = new System.Drawing.Point(4, 64);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(96, 34);
            label12.TabIndex = 202;
            label12.Text = "Target:";
            label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            Label3.Location = new System.Drawing.Point(4, 0);
            Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(96, 32);
            Label3.TabIndex = 132;
            Label3.Text = "Affects:";
            Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAffects
            // 
            cbAffects.Dock = System.Windows.Forms.DockStyle.Fill;
            cbAffects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAffects.Location = new System.Drawing.Point(108, 4);
            cbAffects.Margin = new System.Windows.Forms.Padding(4);
            cbAffects.Name = "cbAffects";
            cbAffects.Size = new System.Drawing.Size(154, 23);
            cbAffects.TabIndex = 131;
            cbAffects.SelectedIndexChanged += cbAffects_SelectedIndexChanged;
            // 
            // Label6
            // 
            Label6.Location = new System.Drawing.Point(14, 538);
            Label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(270, 18);
            Label6.TabIndex = 142;
            Label6.Text = "Modifier Table:";
            Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbModifier
            // 
            cbModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbModifier.Location = new System.Drawing.Point(14, 559);
            cbModifier.Margin = new System.Windows.Forms.Padding(4);
            cbModifier.Name = "cbModifier";
            cbModifier.Size = new System.Drawing.Size(265, 23);
            cbModifier.TabIndex = 143;
            cbModifier.SelectedIndexChanged += cbModifier_SelectedIndexChanged;
            // 
            // chkStack
            // 
            chkStack.AutoSize = true;
            chkStack.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkStack.Location = new System.Drawing.Point(4, 4);
            chkStack.Margin = new System.Windows.Forms.Padding(4);
            chkStack.Name = "chkStack";
            chkStack.Size = new System.Drawing.Size(78, 19);
            chkStack.TabIndex = 171;
            chkStack.Text = "Can Stack";
            chkStack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkStack.CheckedChanged += chkFxNoStack_CheckedChanged;
            // 
            // chkNearGround
            // 
            chkNearGround.AutoSize = true;
            chkNearGround.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkNearGround.Location = new System.Drawing.Point(368, 4);
            chkNearGround.Margin = new System.Windows.Forms.Padding(4);
            chkNearGround.Name = "chkNearGround";
            chkNearGround.Size = new System.Drawing.Size(94, 19);
            chkNearGround.TabIndex = 172;
            chkNearGround.Text = "Near Ground";
            chkNearGround.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkNearGround.CheckedChanged += chkNearGround_CheckedChanged;
            // 
            // chkFXResistable
            // 
            chkFXResistable.AutoSize = true;
            chkFXResistable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkFXResistable.Location = new System.Drawing.Point(273, 4);
            chkFXResistable.Margin = new System.Windows.Forms.Padding(4);
            chkFXResistable.Name = "chkFXResistable";
            chkFXResistable.Size = new System.Drawing.Size(87, 19);
            chkFXResistable.TabIndex = 170;
            chkFXResistable.Text = "Unresistible";
            chkFXResistable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkFXResistable.CheckedChanged += chkFXResistible_CheckedChanged;
            // 
            // IgnoreED
            // 
            IgnoreED.AutoSize = true;
            IgnoreED.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            IgnoreED.Location = new System.Drawing.Point(188, 4);
            IgnoreED.Margin = new System.Windows.Forms.Padding(4);
            IgnoreED.Name = "IgnoreED";
            IgnoreED.Size = new System.Drawing.Size(77, 19);
            IgnoreED.TabIndex = 173;
            IgnoreED.Text = "Ignore ED";
            IgnoreED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            IgnoreED.CheckedChanged += IgnoreED_CheckedChanged;
            // 
            // chkFXBuffable
            // 
            chkFXBuffable.AutoSize = true;
            chkFXBuffable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkFXBuffable.Location = new System.Drawing.Point(90, 4);
            chkFXBuffable.Margin = new System.Windows.Forms.Padding(4);
            chkFXBuffable.Name = "chkFXBuffable";
            chkFXBuffable.Size = new System.Drawing.Size(90, 19);
            chkFXBuffable.TabIndex = 169;
            chkFXBuffable.Text = "Ignore Buffs";
            chkFXBuffable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkFXBuffable.CheckedChanged += chkFXBuffable_CheckedChanged;
            // 
            // chkCancelOnMiss
            // 
            chkCancelOnMiss.AutoSize = true;
            chkCancelOnMiss.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkCancelOnMiss.Location = new System.Drawing.Point(470, 4);
            chkCancelOnMiss.Margin = new System.Windows.Forms.Padding(4);
            chkCancelOnMiss.Name = "chkCancelOnMiss";
            chkCancelOnMiss.Size = new System.Drawing.Size(106, 19);
            chkCancelOnMiss.TabIndex = 174;
            chkCancelOnMiss.Text = "Cancel on Miss";
            chkCancelOnMiss.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkCancelOnMiss.CheckedChanged += chkCancelOnMiss_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(chkStack);
            flowLayoutPanel1.Controls.Add(chkFXBuffable);
            flowLayoutPanel1.Controls.Add(IgnoreED);
            flowLayoutPanel1.Controls.Add(chkFXResistable);
            flowLayoutPanel1.Controls.Add(chkNearGround);
            flowLayoutPanel1.Controls.Add(chkCancelOnMiss);
            flowLayoutPanel1.Controls.Add(chkRqToHitCheck);
            flowLayoutPanel1.Location = new System.Drawing.Point(493, 564);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(746, 31);
            flowLayoutPanel1.TabIndex = 185;
            // 
            // chkRqToHitCheck
            // 
            chkRqToHitCheck.AutoSize = true;
            chkRqToHitCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkRqToHitCheck.Location = new System.Drawing.Point(584, 4);
            chkRqToHitCheck.Margin = new System.Windows.Forms.Padding(4);
            chkRqToHitCheck.Name = "chkRqToHitCheck";
            chkRqToHitCheck.Size = new System.Drawing.Size(133, 19);
            chkRqToHitCheck.TabIndex = 175;
            chkRqToHitCheck.Text = "Require ToHit Check";
            chkRqToHitCheck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkRqToHitCheck.CheckedChanged += chkRqToHitCheck_CheckedChanged;
            // 
            // chkIgnoreScale
            // 
            chkIgnoreScale.Location = new System.Drawing.Point(1371, 86);
            chkIgnoreScale.Margin = new System.Windows.Forms.Padding(4);
            chkIgnoreScale.Name = "chkIgnoreScale";
            chkIgnoreScale.Size = new System.Drawing.Size(203, 20);
            chkIgnoreScale.TabIndex = 189;
            chkIgnoreScale.Text = "Ignore Scaling For Effect";
            chkIgnoreScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkIgnoreScale.UseVisualStyleBackColor = true;
            chkIgnoreScale.CheckedChanged += chkIgnoreScale_CheckChanged;
            // 
            // Label28
            // 
            Label28.Location = new System.Drawing.Point(1325, 15);
            Label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label28.Name = "Label28";
            Label28.Size = new System.Drawing.Size(94, 27);
            Label28.TabIndex = 187;
            Label28.Text = "Display Priority:";
            Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbFXClass
            // 
            cbFXClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFXClass.Location = new System.Drawing.Point(1427, 16);
            cbFXClass.Margin = new System.Windows.Forms.Padding(4);
            cbFXClass.Name = "cbFXClass";
            cbFXClass.Size = new System.Drawing.Size(190, 23);
            cbFXClass.TabIndex = 186;
            cbFXClass.SelectedIndexChanged += cbFXClass_SelectedIndexChanged;
            // 
            // chkVariable
            // 
            chkVariable.Location = new System.Drawing.Point(1371, 55);
            chkVariable.Margin = new System.Windows.Forms.Padding(4);
            chkVariable.Name = "chkVariable";
            chkVariable.Size = new System.Drawing.Size(203, 23);
            chkVariable.TabIndex = 188;
            chkVariable.Text = "Enable Power Scaling Override";
            chkVariable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chkVariable.CheckedChanged += chkVariable_CheckedChanged;
            // 
            // Label10
            // 
            Label10.Location = new System.Drawing.Point(1391, 537);
            Label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label10.Name = "Label10";
            Label10.Size = new System.Drawing.Size(64, 23);
            Label10.TabIndex = 190;
            Label10.Text = "Override:";
            Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtOverride
            // 
            txtOverride.Location = new System.Drawing.Point(1391, 564);
            txtOverride.Margin = new System.Windows.Forms.Padding(4);
            txtOverride.Name = "txtOverride";
            txtOverride.Size = new System.Drawing.Size(219, 23);
            txtOverride.TabIndex = 191;
            txtOverride.TextChanged += txtOverride_TextChanged;
            // 
            // cmbEffectId
            // 
            cmbEffectId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbEffectId.Location = new System.Drawing.Point(1391, 506);
            cmbEffectId.Margin = new System.Windows.Forms.Padding(4);
            cmbEffectId.Name = "cmbEffectId";
            cmbEffectId.Size = new System.Drawing.Size(219, 23);
            cmbEffectId.TabIndex = 195;
            cmbEffectId.SelectedIndexChanged += cmbEffectId_SelectedIndexChanged;
            // 
            // Label9
            // 
            Label9.Location = new System.Drawing.Point(1391, 479);
            Label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label9.Name = "Label9";
            Label9.Size = new System.Drawing.Size(145, 23);
            Label9.TabIndex = 194;
            Label9.Text = "GlobalChanceMod Flag:";
            Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label30
            // 
            Label30.Location = new System.Drawing.Point(1391, 607);
            Label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label30.Name = "Label30";
            Label30.Size = new System.Drawing.Size(167, 25);
            Label30.TabIndex = 193;
            Label30.Text = "Old Special Case:";
            Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbFXSpecialCase
            // 
            cbFXSpecialCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFXSpecialCase.Location = new System.Drawing.Point(1390, 635);
            cbFXSpecialCase.Margin = new System.Windows.Forms.Padding(4);
            cbFXSpecialCase.Name = "cbFXSpecialCase";
            cbFXSpecialCase.Size = new System.Drawing.Size(219, 23);
            cbFXSpecialCase.TabIndex = 192;
            cbFXSpecialCase.SelectedIndexChanged += cbFXSpecialCase_SelectedIndexChanged;
            // 
            // label13
            // 
            label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
            label13.Location = new System.Drawing.Point(4, 6);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(209, 18);
            label13.TabIndex = 181;
            label13.Text = "Suppress Effect (When)";
            label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clbSuppression
            // 
            clbSuppression.CheckOnClick = true;
            clbSuppression.FormattingEnabled = true;
            clbSuppression.Location = new System.Drawing.Point(7, 28);
            clbSuppression.Margin = new System.Windows.Forms.Padding(4);
            clbSuppression.Name = "clbSuppression";
            clbSuppression.Size = new System.Drawing.Size(205, 292);
            clbSuppression.TabIndex = 0;
            clbSuppression.SelectedIndexChanged += clbSuppression_SelectedIndexChanged;
            // 
            // panel4
            // 
            panel4.Controls.Add(label13);
            panel4.Controls.Add(clbSuppression);
            panel4.Location = new System.Drawing.Point(1391, 124);
            panel4.Margin = new System.Windows.Forms.Padding(4);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(219, 351);
            panel4.TabIndex = 196;
            // 
            // btnCopy
            // 
            btnCopy.Location = new System.Drawing.Point(1134, 620);
            btnCopy.Margin = new System.Windows.Forms.Padding(4);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new System.Drawing.Size(219, 32);
            btnCopy.TabIndex = 197;
            btnCopy.Text = "Copy Effect Data";
            btnCopy.Click += btnCopy_Click;
            // 
            // btnPaste
            // 
            btnPaste.Location = new System.Drawing.Point(1134, 656);
            btnPaste.Margin = new System.Windows.Forms.Padding(4);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new System.Drawing.Size(219, 32);
            btnPaste.TabIndex = 198;
            btnPaste.Text = "Paste Effect Data";
            btnPaste.Click += btnPaste_Click;
            // 
            // btnOK
            // 
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(763, 620);
            btnOK.Margin = new System.Windows.Forms.Padding(4);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(105, 44);
            btnOK.TabIndex = 200;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(875, 620);
            btnCancel.Margin = new System.Windows.Forms.Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 44);
            btnCancel.TabIndex = 199;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnEditConditions
            // 
            btnEditConditions.Location = new System.Drawing.Point(332, 656);
            btnEditConditions.Margin = new System.Windows.Forms.Padding(4);
            btnEditConditions.Name = "btnEditConditions";
            btnEditConditions.Size = new System.Drawing.Size(266, 32);
            btnEditConditions.TabIndex = 205;
            btnEditConditions.Text = "Edit Effect Conditions";
            btnEditConditions.Click += btnEditConditions_Click;
            // 
            // btnExprBuilder
            // 
            btnExprBuilder.Location = new System.Drawing.Point(332, 620);
            btnExprBuilder.Margin = new System.Windows.Forms.Padding(4);
            btnExprBuilder.Name = "btnExprBuilder";
            btnExprBuilder.Size = new System.Drawing.Size(266, 32);
            btnExprBuilder.TabIndex = 206;
            btnExprBuilder.Text = "Expression Builder";
            btnExprBuilder.Click += btnExprBuilder_Click;
            // 
            // tpPowerAttribs
            // 
            tpPowerAttribs.ColumnCount = 2;
            tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136F));
            tpPowerAttribs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            tpPowerAttribs.Controls.Add(txtMaxTargets, 0, 14);
            tpPowerAttribs.Controls.Add(txtArc, 0, 13);
            tpPowerAttribs.Controls.Add(txtEffectArea, 0, 0);
            tpPowerAttribs.Controls.Add(cbFXEffectArea, 1, 0);
            tpPowerAttribs.Controls.Add(txtFXAccuracy, 1, 1);
            tpPowerAttribs.Controls.Add(txtRange, 0, 7);
            tpPowerAttribs.Controls.Add(txtAccuracy, 0, 1);
            tpPowerAttribs.Controls.Add(txtCastTime, 0, 3);
            tpPowerAttribs.Controls.Add(txtFXRange, 1, 7);
            tpPowerAttribs.Controls.Add(txtInterruptTime, 0, 2);
            tpPowerAttribs.Controls.Add(txtFXInterruptTime, 1, 2);
            tpPowerAttribs.Controls.Add(txtRechargeTime, 0, 4);
            tpPowerAttribs.Controls.Add(txtFXCastTime, 1, 3);
            tpPowerAttribs.Controls.Add(txtFXRechargeTime, 1, 4);
            tpPowerAttribs.Controls.Add(txtActivateInterval, 0, 5);
            tpPowerAttribs.Controls.Add(txtFXActivateInterval, 1, 5);
            tpPowerAttribs.Controls.Add(txtEnduranceCost, 0, 6);
            tpPowerAttribs.Controls.Add(txtFXEnduranceCost, 1, 6);
            tpPowerAttribs.Controls.Add(txtSecondaryRange, 0, 8);
            tpPowerAttribs.Controls.Add(txtRadius, 0, 9);
            tpPowerAttribs.Controls.Add(txtFXSecondaryRange, 1, 8);
            tpPowerAttribs.Controls.Add(txtFXRadius, 1, 9);
            tpPowerAttribs.Controls.Add(txtFXArc, 1, 13);
            tpPowerAttribs.Controls.Add(txtFXMaxTargets, 1, 14);
            tpPowerAttribs.Location = new System.Drawing.Point(14, 124);
            tpPowerAttribs.Margin = new System.Windows.Forms.Padding(4);
            tpPowerAttribs.Name = "tpPowerAttribs";
            tpPowerAttribs.RowCount = 15;
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            tpPowerAttribs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            tpPowerAttribs.Size = new System.Drawing.Size(266, 408);
            tpPowerAttribs.TabIndex = 207;
            tpPowerAttribs.Visible = false;
            // 
            // txtMaxTargets
            // 
            txtMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            txtMaxTargets.Location = new System.Drawing.Point(4, 378);
            txtMaxTargets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtMaxTargets.Name = "txtMaxTargets";
            txtMaxTargets.Size = new System.Drawing.Size(128, 30);
            txtMaxTargets.TabIndex = 161;
            txtMaxTargets.Text = "Max Targets:";
            txtMaxTargets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtArc
            // 
            txtArc.Dock = System.Windows.Forms.DockStyle.Fill;
            txtArc.Location = new System.Drawing.Point(4, 345);
            txtArc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtArc.Name = "txtArc";
            txtArc.Size = new System.Drawing.Size(128, 33);
            txtArc.TabIndex = 156;
            txtArc.Text = "Arc:";
            txtArc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEffectArea
            // 
            txtEffectArea.AutoSize = true;
            txtEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            txtEffectArea.Location = new System.Drawing.Point(4, 0);
            txtEffectArea.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtEffectArea.Name = "txtEffectArea";
            txtEffectArea.Size = new System.Drawing.Size(128, 31);
            txtEffectArea.TabIndex = 128;
            txtEffectArea.Text = "Effect Area:";
            txtEffectArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFXEffectArea
            // 
            cbFXEffectArea.Dock = System.Windows.Forms.DockStyle.Fill;
            cbFXEffectArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbFXEffectArea.Location = new System.Drawing.Point(140, 4);
            cbFXEffectArea.Margin = new System.Windows.Forms.Padding(4);
            cbFXEffectArea.Name = "cbFXEffectArea";
            cbFXEffectArea.Size = new System.Drawing.Size(122, 23);
            cbFXEffectArea.TabIndex = 127;
            cbFXEffectArea.SelectedIndexChanged += cbFXEffectArea_SelectedIndexChanged;
            // 
            // txtFXAccuracy
            // 
            txtFXAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXAccuracy.Location = new System.Drawing.Point(140, 35);
            txtFXAccuracy.Margin = new System.Windows.Forms.Padding(4);
            txtFXAccuracy.Name = "txtFXAccuracy";
            txtFXAccuracy.Size = new System.Drawing.Size(122, 23);
            txtFXAccuracy.TabIndex = 129;
            txtFXAccuracy.Text = "0";
            txtFXAccuracy.TextChanged += txtFXAccuracy_TextChanged;
            // 
            // txtRange
            // 
            txtRange.Dock = System.Windows.Forms.DockStyle.Fill;
            txtRange.Location = new System.Drawing.Point(4, 237);
            txtRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtRange.Name = "txtRange";
            txtRange.Size = new System.Drawing.Size(128, 36);
            txtRange.TabIndex = 155;
            txtRange.Text = "Range:";
            txtRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAccuracy
            // 
            txtAccuracy.AutoSize = true;
            txtAccuracy.Dock = System.Windows.Forms.DockStyle.Fill;
            txtAccuracy.Location = new System.Drawing.Point(4, 31);
            txtAccuracy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtAccuracy.Name = "txtAccuracy";
            txtAccuracy.Size = new System.Drawing.Size(128, 31);
            txtAccuracy.TabIndex = 130;
            txtAccuracy.Text = "Accuracy:";
            txtAccuracy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCastTime
            // 
            txtCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtCastTime.Location = new System.Drawing.Point(4, 93);
            txtCastTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtCastTime.Name = "txtCastTime";
            txtCastTime.Size = new System.Drawing.Size(128, 36);
            txtCastTime.TabIndex = 97;
            txtCastTime.Text = "Casting Time:";
            txtCastTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXRange
            // 
            txtFXRange.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXRange.Location = new System.Drawing.Point(140, 241);
            txtFXRange.Margin = new System.Windows.Forms.Padding(4);
            txtFXRange.Name = "txtFXRange";
            txtFXRange.Size = new System.Drawing.Size(122, 23);
            txtFXRange.TabIndex = 154;
            txtFXRange.Text = "0";
            txtFXRange.TextChanged += txtFXRange_TextChanged;
            // 
            // txtInterruptTime
            // 
            txtInterruptTime.AutoSize = true;
            txtInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtInterruptTime.Location = new System.Drawing.Point(4, 62);
            txtInterruptTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtInterruptTime.Name = "txtInterruptTime";
            txtInterruptTime.Size = new System.Drawing.Size(128, 31);
            txtInterruptTime.TabIndex = 98;
            txtInterruptTime.Text = "Interruptable Time:";
            txtInterruptTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXInterruptTime
            // 
            txtFXInterruptTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXInterruptTime.Location = new System.Drawing.Point(140, 66);
            txtFXInterruptTime.Margin = new System.Windows.Forms.Padding(4);
            txtFXInterruptTime.Name = "txtFXInterruptTime";
            txtFXInterruptTime.Size = new System.Drawing.Size(122, 23);
            txtFXInterruptTime.TabIndex = 82;
            txtFXInterruptTime.Text = "0";
            txtFXInterruptTime.TextChanged += txtFXInterruptTime_TextChanged;
            // 
            // txtRechargeTime
            // 
            txtRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtRechargeTime.Location = new System.Drawing.Point(4, 129);
            txtRechargeTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtRechargeTime.Name = "txtRechargeTime";
            txtRechargeTime.Size = new System.Drawing.Size(128, 36);
            txtRechargeTime.TabIndex = 99;
            txtRechargeTime.Text = "Recharge Time:";
            txtRechargeTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXCastTime
            // 
            txtFXCastTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXCastTime.Location = new System.Drawing.Point(140, 97);
            txtFXCastTime.Margin = new System.Windows.Forms.Padding(4);
            txtFXCastTime.Name = "txtFXCastTime";
            txtFXCastTime.Size = new System.Drawing.Size(122, 23);
            txtFXCastTime.TabIndex = 80;
            txtFXCastTime.Text = "0";
            txtFXCastTime.TextChanged += txtFXCastTime_TextChanged;
            // 
            // txtFXRechargeTime
            // 
            txtFXRechargeTime.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXRechargeTime.Location = new System.Drawing.Point(140, 133);
            txtFXRechargeTime.Margin = new System.Windows.Forms.Padding(4);
            txtFXRechargeTime.Name = "txtFXRechargeTime";
            txtFXRechargeTime.Size = new System.Drawing.Size(122, 23);
            txtFXRechargeTime.TabIndex = 83;
            txtFXRechargeTime.Text = "0";
            txtFXRechargeTime.TextChanged += txtFXRechargeTime_TextChanged;
            // 
            // txtActivateInterval
            // 
            txtActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            txtActivateInterval.Location = new System.Drawing.Point(4, 165);
            txtActivateInterval.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtActivateInterval.Name = "txtActivateInterval";
            txtActivateInterval.Size = new System.Drawing.Size(128, 36);
            txtActivateInterval.TabIndex = 100;
            txtActivateInterval.Text = "Activate Interval:";
            txtActivateInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXActivateInterval
            // 
            txtFXActivateInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXActivateInterval.Location = new System.Drawing.Point(140, 169);
            txtFXActivateInterval.Margin = new System.Windows.Forms.Padding(4);
            txtFXActivateInterval.Name = "txtFXActivateInterval";
            txtFXActivateInterval.Size = new System.Drawing.Size(122, 23);
            txtFXActivateInterval.TabIndex = 84;
            txtFXActivateInterval.Text = "0";
            txtFXActivateInterval.TextChanged += txtFXActivateInterval_TextChanged;
            // 
            // txtEnduranceCost
            // 
            txtEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            txtEnduranceCost.Location = new System.Drawing.Point(4, 201);
            txtEnduranceCost.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtEnduranceCost.Name = "txtEnduranceCost";
            txtEnduranceCost.Size = new System.Drawing.Size(128, 36);
            txtEnduranceCost.TabIndex = 101;
            txtEnduranceCost.Text = "Endurance Cost:";
            txtEnduranceCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXEnduranceCost
            // 
            txtFXEnduranceCost.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXEnduranceCost.Location = new System.Drawing.Point(140, 205);
            txtFXEnduranceCost.Margin = new System.Windows.Forms.Padding(4);
            txtFXEnduranceCost.Name = "txtFXEnduranceCost";
            txtFXEnduranceCost.Size = new System.Drawing.Size(122, 23);
            txtFXEnduranceCost.TabIndex = 85;
            txtFXEnduranceCost.Text = "1";
            txtFXEnduranceCost.TextChanged += txtFXEnduranceCost_TextChanged;
            // 
            // txtSecondaryRange
            // 
            txtSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            txtSecondaryRange.Location = new System.Drawing.Point(4, 273);
            txtSecondaryRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtSecondaryRange.Name = "txtSecondaryRange";
            txtSecondaryRange.Size = new System.Drawing.Size(128, 36);
            txtSecondaryRange.TabIndex = 134;
            txtSecondaryRange.Text = "Secondary Range:";
            txtSecondaryRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRadius
            // 
            txtRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            txtRadius.Location = new System.Drawing.Point(4, 309);
            txtRadius.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtRadius.Name = "txtRadius";
            txtRadius.Size = new System.Drawing.Size(128, 36);
            txtRadius.TabIndex = 136;
            txtRadius.Text = "Radius:";
            txtRadius.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXSecondaryRange
            // 
            txtFXSecondaryRange.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXSecondaryRange.Location = new System.Drawing.Point(140, 277);
            txtFXSecondaryRange.Margin = new System.Windows.Forms.Padding(4);
            txtFXSecondaryRange.Name = "txtFXSecondaryRange";
            txtFXSecondaryRange.Size = new System.Drawing.Size(122, 23);
            txtFXSecondaryRange.TabIndex = 157;
            txtFXSecondaryRange.Text = "0";
            txtFXSecondaryRange.TextChanged += txtFXSecondaryRange_TextChanged;
            // 
            // txtFXRadius
            // 
            txtFXRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXRadius.Location = new System.Drawing.Point(140, 313);
            txtFXRadius.Margin = new System.Windows.Forms.Padding(4);
            txtFXRadius.Name = "txtFXRadius";
            txtFXRadius.Size = new System.Drawing.Size(122, 23);
            txtFXRadius.TabIndex = 158;
            txtFXRadius.Text = "0";
            txtFXRadius.TextChanged += txtFXRadius_TextChanged;
            // 
            // txtFXArc
            // 
            txtFXArc.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXArc.Location = new System.Drawing.Point(140, 349);
            txtFXArc.Margin = new System.Windows.Forms.Padding(4);
            txtFXArc.Name = "txtFXArc";
            txtFXArc.Size = new System.Drawing.Size(122, 23);
            txtFXArc.TabIndex = 159;
            txtFXArc.Text = "0";
            txtFXArc.TextChanged += txtFXArc_TextChanged;
            // 
            // txtFXMaxTargets
            // 
            txtFXMaxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFXMaxTargets.Location = new System.Drawing.Point(140, 382);
            txtFXMaxTargets.Margin = new System.Windows.Forms.Padding(4);
            txtFXMaxTargets.Name = "txtFXMaxTargets";
            txtFXMaxTargets.Size = new System.Drawing.Size(122, 23);
            txtFXMaxTargets.TabIndex = 162;
            txtFXMaxTargets.Text = "0";
            txtFXMaxTargets.TextChanged += txtFXMaxTargets_TextChanged;
            // 
            // frmPowerEffect
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1623, 698);
            Controls.Add(tpPowerAttribs);
            Controls.Add(btnExprBuilder);
            Controls.Add(btnEditConditions);
            Controls.Add(chkIgnoreScale);
            Controls.Add(btnCopy);
            Controls.Add(btnPaste);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(panel4);
            Controls.Add(cmbEffectId);
            Controls.Add(Label9);
            Controls.Add(Label30);
            Controls.Add(cbFXSpecialCase);
            Controls.Add(Label10);
            Controls.Add(txtOverride);
            Controls.Add(chkVariable);
            Controls.Add(Label28);
            Controls.Add(cbFXClass);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(tableLayoutPanel3);
            Controls.Add(lvSubSub);
            Controls.Add(lvSubAttribute);
            Controls.Add(Label6);
            Controls.Add(lvEffectType);
            Controls.Add(cbModifier);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(cbCoDFormat);
            Controls.Add(lblEffectDescription);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "frmPowerEffect";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "frmPowerEffect2";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel4.ResumeLayout(false);
            tpPowerAttribs.ResumeLayout(false);
            tpPowerAttribs.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private Mids_Reborn.Controls.ctlListViewColored lvSubSub;
        private System.Windows.Forms.ColumnHeader chSubSub;
        private Mids_Reborn.Controls.ctlListViewColored lvSubAttribute;
        private System.Windows.Forms.ColumnHeader chSub;
        private Mids_Reborn.Controls.ctlListViewColored lvEffectType;
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
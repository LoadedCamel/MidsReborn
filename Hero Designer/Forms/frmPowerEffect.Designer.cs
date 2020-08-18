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
            this.Label27 = new System.Windows.Forms.Label();
            this.rbIfPlayer = new System.Windows.Forms.RadioButton();
            this.rbIfCritter = new System.Windows.Forms.RadioButton();
            this.rbIfAny = new System.Windows.Forms.RadioButton();
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
            this.Label6 = new System.Windows.Forms.Label();
            this.cbModifier = new System.Windows.Forms.ComboBox();
            this.chkNearGround = new System.Windows.Forms.CheckBox();
            this.lblAffectsCaster = new System.Windows.Forms.Label();
            this.lvEffectType = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSubAttribute = new System.Windows.Forms.ListView();
            this.chSub = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblProb = new System.Windows.Forms.Label();
            this.lvSubSub = new System.Windows.Forms.ListView();
            this.chSubSub = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.btnCSV = new System.Windows.Forms.Button();
            this.Label9 = new System.Windows.Forms.Label();
            this.cmbEffectId = new System.Windows.Forms.ComboBox();
            this.IgnoreED = new System.Windows.Forms.CheckBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtOverride = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.txtPPM = new System.Windows.Forms.TextBox();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(804, 497);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(150, 26);
            this.btnPaste.TabIndex = 116;
            this.btnPaste.Text = "Paste Effect Data";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(804, 465);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(150, 26);
            this.btnCopy.TabIndex = 115;
            this.btnCopy.Text = "Copy Effect Data";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // chkStack
            // 
            this.chkStack.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.Location = new System.Drawing.Point(20, 510);
            this.chkStack.Name = "chkStack";
            this.chkStack.Size = new System.Drawing.Size(172, 20);
            this.chkStack.TabIndex = 112;
            this.chkStack.Text = "Effect Can Stack";
            this.chkStack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkStack.CheckedChanged += new System.EventHandler(this.chkFxNoStack_CheckedChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.clbSuppression);
            this.GroupBox3.Location = new System.Drawing.Point(794, 124);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(166, 240);
            this.GroupBox3.TabIndex = 107;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Suppression";
            // 
            // clbSuppression
            // 
            this.clbSuppression.FormattingEnabled = true;
            this.clbSuppression.Location = new System.Drawing.Point(10, 19);
            this.clbSuppression.Name = "clbSuppression";
            this.clbSuppression.Size = new System.Drawing.Size(150, 214);
            this.clbSuppression.TabIndex = 0;
            this.clbSuppression.SelectedIndexChanged += new System.EventHandler(this.clbSuppression_SelectedIndexChanged);
            // 
            // Label27
            // 
            this.Label27.Location = new System.Drawing.Point(12, 596);
            this.Label27.Name = "Label27";
            this.Label27.Size = new System.Drawing.Size(76, 20);
            this.Label27.TabIndex = 102;
            this.Label27.Text = "If Target =";
            this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rbIfPlayer
            // 
            this.rbIfPlayer.Location = new System.Drawing.Point(224, 597);
            this.rbIfPlayer.Name = "rbIfPlayer";
            this.rbIfPlayer.Size = new System.Drawing.Size(76, 20);
            this.rbIfPlayer.TabIndex = 88;
            this.rbIfPlayer.Text = "Players";
            this.rbIfPlayer.CheckedChanged += new System.EventHandler(this.rbIfACP_CheckedChanged);
            // 
            // rbIfCritter
            // 
            this.rbIfCritter.Location = new System.Drawing.Point(147, 597);
            this.rbIfCritter.Name = "rbIfCritter";
            this.rbIfCritter.Size = new System.Drawing.Size(71, 20);
            this.rbIfCritter.TabIndex = 87;
            this.rbIfCritter.Text = "Critters";
            this.rbIfCritter.CheckedChanged += new System.EventHandler(this.rbIfACP_CheckedChanged);
            // 
            // rbIfAny
            // 
            this.rbIfAny.Checked = true;
            this.rbIfAny.Location = new System.Drawing.Point(94, 597);
            this.rbIfAny.Name = "rbIfAny";
            this.rbIfAny.Size = new System.Drawing.Size(57, 20);
            this.rbIfAny.TabIndex = 86;
            this.rbIfAny.TabStop = true;
            this.rbIfAny.Text = "Any";
            this.rbIfAny.CheckedChanged += new System.EventHandler(this.rbIfACP_CheckedChanged);
            // 
            // chkFXResistable
            // 
            this.chkFXResistable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.Location = new System.Drawing.Point(20, 561);
            this.chkFXResistable.Name = "chkFXResistable";
            this.chkFXResistable.Size = new System.Drawing.Size(172, 20);
            this.chkFXResistable.TabIndex = 90;
            this.chkFXResistable.Text = "Effect is Unresistible";
            this.chkFXResistable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXResistable.CheckedChanged += new System.EventHandler(this.chkFXResistable_CheckedChanged);
            // 
            // chkFXBuffable
            // 
            this.chkFXBuffable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.Location = new System.Drawing.Point(20, 528);
            this.chkFXBuffable.Name = "chkFXBuffable";
            this.chkFXBuffable.Size = new System.Drawing.Size(172, 20);
            this.chkFXBuffable.TabIndex = 89;
            this.chkFXBuffable.Text = "Ignore Buffs / Enhancements";
            this.chkFXBuffable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFXBuffable.CheckedChanged += new System.EventHandler(this.chkFXBuffable_CheckedChanged);
            // 
            // Label26
            // 
            this.Label26.Location = new System.Drawing.Point(12, 273);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(76, 20);
            this.Label26.TabIndex = 101;
            this.Label26.Text = "Probability:";
            this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXProb
            // 
            this.txtFXProb.Location = new System.Drawing.Point(96, 273);
            this.txtFXProb.Name = "txtFXProb";
            this.txtFXProb.Size = new System.Drawing.Size(42, 20);
            this.txtFXProb.TabIndex = 85;
            this.txtFXProb.Text = "1";
            this.txtFXProb.TextChanged += new System.EventHandler(this.txtFXProb_TextChanged);
            this.txtFXProb.Leave += new System.EventHandler(this.txtFXProb_Leave);
            // 
            // Label25
            // 
            this.Label25.Location = new System.Drawing.Point(12, 247);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(76, 20);
            this.Label25.TabIndex = 100;
            this.Label25.Text = "Delay Time:";
            this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDelay
            // 
            this.txtFXDelay.Location = new System.Drawing.Point(96, 247);
            this.txtFXDelay.Name = "txtFXDelay";
            this.txtFXDelay.Size = new System.Drawing.Size(42, 20);
            this.txtFXDelay.TabIndex = 84;
            this.txtFXDelay.Text = "0";
            this.txtFXDelay.TextChanged += new System.EventHandler(this.txtFXDelay_TextChanged);
            this.txtFXDelay.Leave += new System.EventHandler(this.txtFXDelay_Leave);
            // 
            // Label24
            // 
            this.Label24.Location = new System.Drawing.Point(14, 221);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(76, 20);
            this.Label24.TabIndex = 99;
            this.Label24.Text = "Ticks:";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXTicks
            // 
            this.txtFXTicks.Location = new System.Drawing.Point(96, 221);
            this.txtFXTicks.Name = "txtFXTicks";
            this.txtFXTicks.Size = new System.Drawing.Size(42, 20);
            this.txtFXTicks.TabIndex = 83;
            this.txtFXTicks.Text = "0";
            this.txtFXTicks.TextChanged += new System.EventHandler(this.txtFXTicks_TextChanged);
            this.txtFXTicks.Leave += new System.EventHandler(this.txtFXTicks_Leave);
            // 
            // Label23
            // 
            this.Label23.Location = new System.Drawing.Point(14, 169);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(76, 20);
            this.Label23.TabIndex = 98;
            this.Label23.Text = "Duration:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXDuration
            // 
            this.txtFXDuration.Location = new System.Drawing.Point(96, 169);
            this.txtFXDuration.Name = "txtFXDuration";
            this.txtFXDuration.Size = new System.Drawing.Size(42, 20);
            this.txtFXDuration.TabIndex = 82;
            this.txtFXDuration.Text = "0";
            this.txtFXDuration.TextChanged += new System.EventHandler(this.txtFXDuration_TextChanged);
            this.txtFXDuration.Leave += new System.EventHandler(this.txtFXDuration_Leave);
            // 
            // Label22
            // 
            this.Label22.Location = new System.Drawing.Point(14, 195);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(76, 20);
            this.Label22.TabIndex = 97;
            this.Label22.Text = "Magnitude:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXMag
            // 
            this.txtFXMag.Location = new System.Drawing.Point(96, 195);
            this.txtFXMag.Name = "txtFXMag";
            this.txtFXMag.Size = new System.Drawing.Size(42, 20);
            this.txtFXMag.TabIndex = 80;
            this.txtFXMag.Text = "0";
            this.txtFXMag.TextChanged += new System.EventHandler(this.txtFXMag_TextChanged);
            this.txtFXMag.Leave += new System.EventHandler(this.txtFXMag_Leave);
            // 
            // Label28
            // 
            this.Label28.Location = new System.Drawing.Point(202, 118);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(98, 20);
            this.Label28.TabIndex = 104;
            this.Label28.Text = "DIsplay Priority:";
            this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label30
            // 
            this.Label30.Location = new System.Drawing.Point(408, 597);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(76, 20);
            this.Label30.TabIndex = 105;
            this.Label30.Text = "Special Case:";
            this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFXClass
            // 
            this.cbFXClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXClass.Location = new System.Drawing.Point(306, 116);
            this.cbFXClass.Name = "cbFXClass";
            this.cbFXClass.Size = new System.Drawing.Size(132, 22);
            this.cbFXClass.TabIndex = 93;
            this.cbFXClass.SelectedIndexChanged += new System.EventHandler(this.cbFXClass_SelectedIndexChanged);
            // 
            // cbFXSpecialCase
            // 
            this.cbFXSpecialCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFXSpecialCase.Location = new System.Drawing.Point(489, 597);
            this.cbFXSpecialCase.Name = "cbFXSpecialCase";
            this.cbFXSpecialCase.Size = new System.Drawing.Size(136, 22);
            this.cbFXSpecialCase.TabIndex = 94;
            this.cbFXSpecialCase.SelectedIndexChanged += new System.EventHandler(this.cbFXSpecialCase_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(881, 578);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 36);
            this.btnOK.TabIndex = 119;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(794, 578);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 36);
            this.btnCancel.TabIndex = 118;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblEffectDescription
            // 
            this.lblEffectDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEffectDescription.Location = new System.Drawing.Point(8, 11);
            this.lblEffectDescription.Name = "lblEffectDescription";
            this.lblEffectDescription.Size = new System.Drawing.Size(950, 102);
            this.lblEffectDescription.TabIndex = 120;
            this.lblEffectDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEffectDescription.UseMnemonic = false;
            // 
            // chkVariable
            // 
            this.chkVariable.Location = new System.Drawing.Point(461, 116);
            this.chkVariable.Name = "chkVariable";
            this.chkVariable.Size = new System.Drawing.Size(285, 20);
            this.chkVariable.TabIndex = 126;
            this.chkVariable.Text = "Enable Power Scaling (Override)";
            this.chkVariable.CheckedChanged += new System.EventHandler(this.chkVariable_CheckedChanged);
            // 
            // cbPercentageOverride
            // 
            this.cbPercentageOverride.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPercentageOverride.Location = new System.Drawing.Point(96, 116);
            this.cbPercentageOverride.Name = "cbPercentageOverride";
            this.cbPercentageOverride.Size = new System.Drawing.Size(96, 22);
            this.cbPercentageOverride.TabIndex = 127;
            this.cbPercentageOverride.SelectedIndexChanged += new System.EventHandler(this.cbPercentageOverride_SelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(14, 118);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(76, 20);
            this.Label2.TabIndex = 128;
            this.Label2.Text = "Percentage:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(14, 143);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(76, 20);
            this.Label1.TabIndex = 130;
            this.Label1.Text = "Scale:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtFXScale
            // 
            this.txtFXScale.Location = new System.Drawing.Point(96, 143);
            this.txtFXScale.Name = "txtFXScale";
            this.txtFXScale.Size = new System.Drawing.Size(42, 20);
            this.txtFXScale.TabIndex = 129;
            this.txtFXScale.Text = "0";
            this.txtFXScale.TextChanged += new System.EventHandler(this.txtFXScale_TextChanged);
            this.txtFXScale.Leave += new System.EventHandler(this.txtFXScale_Leave);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(14, 443);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(76, 20);
            this.Label3.TabIndex = 132;
            this.Label3.Text = "Affects:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAffects
            // 
            this.cbAffects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAffects.Location = new System.Drawing.Point(96, 443);
            this.cbAffects.Name = "cbAffects";
            this.cbAffects.Size = new System.Drawing.Size(122, 22);
            this.cbAffects.TabIndex = 131;
            this.cbAffects.SelectedIndexChanged += new System.EventHandler(this.cbAffects_SelectedIndexChanged);
            // 
            // Label4
            // 
            this.Label4.Location = new System.Drawing.Point(14, 328);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(76, 20);
            this.Label4.TabIndex = 134;
            this.Label4.Text = "AttribType:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAttribute
            // 
            this.cbAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttribute.Location = new System.Drawing.Point(96, 328);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(122, 22);
            this.cbAttribute.TabIndex = 133;
            this.cbAttribute.SelectedIndexChanged += new System.EventHandler(this.cbAttribute_SelectedIndexChanged);
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(14, 356);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(76, 20);
            this.Label5.TabIndex = 136;
            this.Label5.Text = "Aspect:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAspect
            // 
            this.cbAspect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAspect.Location = new System.Drawing.Point(96, 356);
            this.cbAspect.Name = "cbAspect";
            this.cbAspect.Size = new System.Drawing.Size(122, 22);
            this.cbAspect.TabIndex = 135;
            this.cbAspect.SelectedIndexChanged += new System.EventHandler(this.cbAspect_SelectedIndexChanged);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(14, 384);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(178, 20);
            this.Label6.TabIndex = 138;
            this.Label6.Text = "Modifier Table:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbModifier
            // 
            this.cbModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModifier.Location = new System.Drawing.Point(20, 405);
            this.cbModifier.Name = "cbModifier";
            this.cbModifier.Size = new System.Drawing.Size(198, 22);
            this.cbModifier.TabIndex = 137;
            this.cbModifier.SelectedIndexChanged += new System.EventHandler(this.cbModifier_SelectedIndexChanged);
            // 
            // chkNearGround
            // 
            this.chkNearGround.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNearGround.Location = new System.Drawing.Point(20, 577);
            this.chkNearGround.Name = "chkNearGround";
            this.chkNearGround.Size = new System.Drawing.Size(172, 20);
            this.chkNearGround.TabIndex = 139;
            this.chkNearGround.Text = "Target must be Near Ground";
            this.chkNearGround.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAffectsCaster
            // 
            this.lblAffectsCaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAffectsCaster.Location = new System.Drawing.Point(96, 464);
            this.lblAffectsCaster.Name = "lblAffectsCaster";
            this.lblAffectsCaster.Size = new System.Drawing.Size(122, 32);
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
            this.lvEffectType.Location = new System.Drawing.Point(224, 143);
            this.lvEffectType.MultiSelect = false;
            this.lvEffectType.Name = "lvEffectType";
            this.lvEffectType.Size = new System.Drawing.Size(197, 447);
            this.lvEffectType.TabIndex = 141;
            this.lvEffectType.UseCompatibleStateImageBehavior = false;
            this.lvEffectType.View = System.Windows.Forms.View.Details;
            this.lvEffectType.SelectedIndexChanged += new System.EventHandler(this.lvEffectType_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Effect Attribute";
            this.ColumnHeader1.Width = 174;
            // 
            // lvSubAttribute
            // 
            this.lvSubAttribute.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSub});
            this.lvSubAttribute.FullRowSelect = true;
            this.lvSubAttribute.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubAttribute.HideSelection = false;
            this.lvSubAttribute.Location = new System.Drawing.Point(427, 143);
            this.lvSubAttribute.MultiSelect = false;
            this.lvSubAttribute.Name = "lvSubAttribute";
            this.lvSubAttribute.Size = new System.Drawing.Size(197, 447);
            this.lvSubAttribute.TabIndex = 142;
            this.lvSubAttribute.UseCompatibleStateImageBehavior = false;
            this.lvSubAttribute.View = System.Windows.Forms.View.Details;
            this.lvSubAttribute.SelectedIndexChanged += new System.EventHandler(this.lvSubAttribute_SelectedIndexChanged);
            // 
            // chSub
            // 
            this.chSub.Text = "Sub-Attribute";
            this.chSub.Width = 173;
            // 
            // lblProb
            // 
            this.lblProb.Location = new System.Drawing.Point(142, 273);
            this.lblProb.Name = "lblProb";
            this.lblProb.Size = new System.Drawing.Size(76, 20);
            this.lblProb.TabIndex = 143;
            this.lblProb.Text = "(100%)";
            this.lblProb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lvSubSub
            // 
            this.lvSubSub.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSubSub});
            this.lvSubSub.FullRowSelect = true;
            this.lvSubSub.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubSub.HideSelection = false;
            this.lvSubSub.Location = new System.Drawing.Point(630, 143);
            this.lvSubSub.MultiSelect = false;
            this.lvSubSub.Name = "lvSubSub";
            this.lvSubSub.Size = new System.Drawing.Size(158, 447);
            this.lvSubSub.TabIndex = 144;
            this.lvSubSub.UseCompatibleStateImageBehavior = false;
            this.lvSubSub.View = System.Windows.Forms.View.Details;
            this.lvSubSub.SelectedIndexChanged += new System.EventHandler(this.lvSubSub_SelectedIndexChanged);
            // 
            // chSubSub
            // 
            this.chSubSub.Text = "Sub-Sub";
            this.chSubSub.Width = 130;
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(144, 247);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(76, 20);
            this.Label7.TabIndex = 145;
            this.Label7.Text = "s";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(142, 169);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(76, 20);
            this.Label8.TabIndex = 146;
            this.Label8.Text = "s";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCSV
            // 
            this.btnCSV.Location = new System.Drawing.Point(804, 529);
            this.btnCSV.Name = "btnCSV";
            this.btnCSV.Size = new System.Drawing.Size(150, 26);
            this.btnCSV.TabIndex = 147;
            this.btnCSV.Text = "Import from CSV String";
            this.btnCSV.Click += new System.EventHandler(this.btnCSV_Click);
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(794, 367);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(160, 20);
            this.Label9.TabIndex = 149;
            this.Label9.Text = "GlobalChanceMod Flag:";
            // 
            // cmbEffectId
            // 
            this.cmbEffectId.FormattingEnabled = true;
            this.cmbEffectId.Location = new System.Drawing.Point(795, 389);
            this.cmbEffectId.Name = "cmbEffectId";
            this.cmbEffectId.Size = new System.Drawing.Size(159, 22);
            this.cmbEffectId.TabIndex = 150;
            this.cmbEffectId.TextChanged += new System.EventHandler(this.cmbEffectId_TextChanged);
            // 
            // IgnoreED
            // 
            this.IgnoreED.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.Location = new System.Drawing.Point(20, 544);
            this.IgnoreED.Name = "IgnoreED";
            this.IgnoreED.Size = new System.Drawing.Size(172, 20);
            this.IgnoreED.TabIndex = 151;
            this.IgnoreED.Text = "Ignore ED";
            this.IgnoreED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IgnoreED.CheckedChanged += new System.EventHandler(this.IgnoreED_CheckedChanged);
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(794, 417);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(160, 20);
            this.Label10.TabIndex = 152;
            this.Label10.Text = "Override:";
            // 
            // txtOverride
            // 
            this.txtOverride.Location = new System.Drawing.Point(797, 435);
            this.txtOverride.Name = "txtOverride";
            this.txtOverride.Size = new System.Drawing.Size(157, 20);
            this.txtOverride.TabIndex = 153;
            this.txtOverride.TextChanged += new System.EventHandler(this.txtOverride_TextChanged);
            // 
            // Label11
            // 
            this.Label11.Location = new System.Drawing.Point(14, 299);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(76, 20);
            this.Label11.TabIndex = 155;
            this.Label11.Text = "PPM:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPPM
            // 
            this.txtPPM.Location = new System.Drawing.Point(96, 299);
            this.txtPPM.Name = "txtPPM";
            this.txtPPM.Size = new System.Drawing.Size(42, 20);
            this.txtPPM.TabIndex = 154;
            this.txtPPM.Text = "0";
            this.txtPPM.TextChanged += new System.EventHandler(this.txtPPM_TextChanged);
            this.txtPPM.Leave += new System.EventHandler(this.txtPPM_Leave);
            // 
            // frmPowerEffect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(971, 626);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.txtPPM);
            this.Controls.Add(this.txtOverride);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.IgnoreED);
            this.Controls.Add(this.cmbEffectId);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.btnCSV);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.lvSubSub);
            this.Controls.Add(this.lblProb);
            this.Controls.Add(this.lvSubAttribute);
            this.Controls.Add(this.lvEffectType);
            this.Controls.Add(this.chkNearGround);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.cbModifier);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.cbAspect);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.cbAttribute);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.cbAffects);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.txtFXScale);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.cbPercentageOverride);
            this.Controls.Add(this.chkVariable);
            this.Controls.Add(this.lblEffectDescription);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.chkStack);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.Label27);
            this.Controls.Add(this.rbIfPlayer);
            this.Controls.Add(this.rbIfCritter);
            this.Controls.Add(this.rbIfAny);
            this.Controls.Add(this.chkFXResistable);
            this.Controls.Add(this.chkFXBuffable);
            this.Controls.Add(this.Label26);
            this.Controls.Add(this.txtFXProb);
            this.Controls.Add(this.Label25);
            this.Controls.Add(this.txtFXDelay);
            this.Controls.Add(this.Label24);
            this.Controls.Add(this.txtFXTicks);
            this.Controls.Add(this.Label23);
            this.Controls.Add(this.txtFXDuration);
            this.Controls.Add(this.Label22);
            this.Controls.Add(this.txtFXMag);
            this.Controls.Add(this.Label28);
            this.Controls.Add(this.Label30);
            this.Controls.Add(this.cbFXClass);
            this.Controls.Add(this.cbFXSpecialCase);
            this.Controls.Add(this.lblAffectsCaster);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPowerEffect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Effect";
            this.GroupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        Button btnCancel;
        Button btnCopy;
        Button btnCSV;
        Button btnOK;
        Button btnPaste;
        ComboBox cbAffects;
        ComboBox cbAspect;
        ComboBox cbAttribute;
        ComboBox cbFXClass;
        ComboBox cbFXSpecialCase;
        ComboBox cbModifier;
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
        Label Label27;
        Label Label28;
        Label Label3;
        Label Label30;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblAffectsCaster;
        Label lblEffectDescription;
        Label lblProb;
        ListView lvEffectType;
        ListView lvSubAttribute;
        ListView lvSubSub;
        RadioButton rbIfAny;
        RadioButton rbIfCritter;
        RadioButton rbIfPlayer;
        TextBox txtFXDelay;
        TextBox txtFXDuration;
        TextBox txtFXMag;
        TextBox txtFXProb;
        TextBox txtFXScale;
        TextBox txtFXTicks;
        TextBox txtOverride;
        TextBox txtPPM;
    }
}
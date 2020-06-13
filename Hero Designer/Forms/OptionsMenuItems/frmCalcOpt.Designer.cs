using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.chkShowAlphaPopup = new System.Windows.Forms.CheckBox();
            this.chkNoTips = new System.Windows.Forms.CheckBox();
            this.chkMiddle = new System.Windows.Forms.CheckBox();
            this.GroupBox17 = new System.Windows.Forms.GroupBox();
            this.chkColorInherent = new System.Windows.Forms.CheckBox();
            this.chkHighVis = new System.Windows.Forms.CheckBox();
            this.Label36 = new System.Windows.Forms.Label();
            this.chkStatBold = new System.Windows.Forms.CheckBox();
            this.chkTextBold = new System.Windows.Forms.CheckBox();
            this.btnFontColor = new System.Windows.Forms.Button();
            this.Label22 = new System.Windows.Forms.Label();
            this.chkVillainColor = new System.Windows.Forms.CheckBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.udStatSize = new System.Windows.Forms.NumericUpDown();
            this.udRTFSize = new System.Windows.Forms.NumericUpDown();
            this.chkIOPrintLevels = new System.Windows.Forms.CheckBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.rbGraphSimple = new System.Windows.Forms.RadioButton();
            this.rbGraphStacked = new System.Windows.Forms.RadioButton();
            this.rbGraphTwoLine = new System.Windows.Forms.RadioButton();
            this.GroupBox14 = new System.Windows.Forms.GroupBox();
            this.chkIOLevel = new System.Windows.Forms.CheckBox();
            this.btnIOReset = new System.Windows.Forms.Button();
            this.Label40 = new System.Windows.Forms.Label();
            this.udIOLevel = new System.Windows.Forms.NumericUpDown();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
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
            this.TabPage6 = new System.Windows.Forms.TabPage();
            this.Label6 = new System.Windows.Forms.Label();
            this.GroupBox13 = new System.Windows.Forms.GroupBox();
            this.udForceLevel = new System.Windows.Forms.NumericUpDown();
            this.Label38 = new System.Windows.Forms.Label();
            this.GroupBox10 = new System.Windows.Forms.GroupBox();
            this.btnBaseReset = new System.Windows.Forms.Button();
            this.Label14 = new System.Windows.Forms.Label();
            this.udBaseToHit = new System.Windows.Forms.NumericUpDown();
            this.Label13 = new System.Windows.Forms.Label();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.udExLow = new System.Windows.Forms.NumericUpDown();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.udExHigh = new System.Windows.Forms.NumericUpDown();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label41 = new System.Windows.Forms.Label();
            this.invBot = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.checkBox24 = new System.Windows.Forms.CheckBox();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.checkBox23 = new System.Windows.Forms.CheckBox();
            this.checkBox22 = new System.Windows.Forms.CheckBox();
            this.checkBox21 = new System.Windows.Forms.CheckBox();
            this.checkBox20 = new System.Windows.Forms.CheckBox();
            this.checkBox19 = new System.Windows.Forms.CheckBox();
            this.checkBox18 = new System.Windows.Forms.CheckBox();
            this.checkBox17 = new System.Windows.Forms.CheckBox();
            this.checkBox16 = new System.Windows.Forms.CheckBox();
            this.checkBox15 = new System.Windows.Forms.CheckBox();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupBox20 = new System.Windows.Forms.GroupBox();
            this.dcChannel = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.GroupBox19 = new System.Windows.Forms.GroupBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.dcServerName = new System.Windows.Forms.TextBox();
            this.dcRemove = new System.Windows.Forms.Button();
            this.dcAdd = new System.Windows.Forms.Button();
            this.dcExList = new System.Windows.Forms.ListBox();
            this.GroupBox18 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.dcNickName = new System.Windows.Forms.TextBox();
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
            this.TabPage5 = new System.Windows.Forms.TabPage();
            this.cbUpdateURL = new System.Windows.Forms.ComboBox();
            this.lblUpdateURL = new System.Windows.Forms.Label();
            this.btnSaveFolderReset = new System.Windows.Forms.Button();
            this.lblSaveFolder = new System.Windows.Forms.Label();
            this.btnSaveFolder = new System.Windows.Forms.Button();
            this.chkLoadLastFile = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.GroupBox16 = new System.Windows.Forms.GroupBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label37 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.chkUpdates = new System.Windows.Forms.CheckBox();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.GroupBox9 = new System.Windows.Forms.GroupBox();
            this.lblExample = new System.Windows.Forms.Label();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.listScenarios = new System.Windows.Forms.ListBox();
            this.chkColorPrint = new System.Windows.Forms.CheckBox();
            this.myTip = new System.Windows.Forms.ToolTip(this.components);
            this.cPicker = new System.Windows.Forms.ColorDialog();
            this.fbdSave = new System.Windows.Forms.FolderBrowserDialog();
            this.TabControl1.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.GroupBox17.SuspendLayout();
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
            this.TabPage6.SuspendLayout();
            this.GroupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udForceLevel)).BeginInit();
            this.GroupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udBaseToHit)).BeginInit();
            this.GroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udExLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udExHigh)).BeginInit();
            this.tabPage7.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.GroupBox20.SuspendLayout();
            this.GroupBox19.SuspendLayout();
            this.GroupBox18.SuspendLayout();
            this.TabPage4.SuspendLayout();
            this.GroupBox12.SuspendLayout();
            this.GroupBox11.SuspendLayout();
            this.TabPage5.SuspendLayout();
            this.GroupBox16.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.GroupBox9.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(504, 360);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 28);
            this.btnOK.TabIndex = 59;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(408, 360);
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
            this.TabControl1.Controls.Add(this.TabPage6);
            this.TabControl1.Controls.Add(this.tabPage7);
            this.TabControl1.Controls.Add(this.TabPage4);
            this.TabControl1.Controls.Add(this.TabPage5);
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(757, 352);
            this.TabControl1.TabIndex = 0;
            // 
            // TabPage3
            // 
            this.TabPage3.Controls.Add(this.chkShowAlphaPopup);
            this.TabPage3.Controls.Add(this.chkNoTips);
            this.TabPage3.Controls.Add(this.chkMiddle);
            this.TabPage3.Controls.Add(this.GroupBox17);
            this.TabPage3.Controls.Add(this.chkIOPrintLevels);
            this.TabPage3.Controls.Add(this.GroupBox5);
            this.TabPage3.Controls.Add(this.GroupBox14);
            this.TabPage3.Controls.Add(this.GroupBox3);
            this.TabPage3.Location = new System.Drawing.Point(4, 23);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Size = new System.Drawing.Size(749, 325);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Enhancements & View";
            this.TabPage3.UseVisualStyleBackColor = true;
            // 
            // chkShowAlphaPopup
            // 
            this.chkShowAlphaPopup.Location = new System.Drawing.Point(434, 285);
            this.chkShowAlphaPopup.Name = "chkShowAlphaPopup";
            this.chkShowAlphaPopup.Size = new System.Drawing.Size(190, 18);
            this.chkShowAlphaPopup.TabIndex = 79;
            this.chkShowAlphaPopup.Text = "Include Alpha buffs in popups";
            // 
            // chkNoTips
            // 
            this.chkNoTips.Location = new System.Drawing.Point(434, 304);
            this.chkNoTips.Name = "chkNoTips";
            this.chkNoTips.Size = new System.Drawing.Size(141, 18);
            this.chkNoTips.TabIndex = 78;
            this.chkNoTips.Text = "No Tooltips";
            this.chkNoTips.Visible = false;
            // 
            // chkMiddle
            // 
            this.chkMiddle.Location = new System.Drawing.Point(206, 304);
            this.chkMiddle.Name = "chkMiddle";
            this.chkMiddle.Size = new System.Drawing.Size(222, 18);
            this.chkMiddle.TabIndex = 77;
            this.chkMiddle.Text = "Middle-Click repeats last enhancement";
            // 
            // GroupBox17
            // 
            this.GroupBox17.Controls.Add(this.chkColorInherent);
            this.GroupBox17.Controls.Add(this.chkHighVis);
            this.GroupBox17.Controls.Add(this.Label36);
            this.GroupBox17.Controls.Add(this.chkStatBold);
            this.GroupBox17.Controls.Add(this.chkTextBold);
            this.GroupBox17.Controls.Add(this.btnFontColor);
            this.GroupBox17.Controls.Add(this.Label22);
            this.GroupBox17.Controls.Add(this.chkVillainColor);
            this.GroupBox17.Controls.Add(this.Label21);
            this.GroupBox17.Controls.Add(this.udStatSize);
            this.GroupBox17.Controls.Add(this.udRTFSize);
            this.GroupBox17.Location = new System.Drawing.Point(196, 136);
            this.GroupBox17.Name = "GroupBox17";
            this.GroupBox17.Size = new System.Drawing.Size(545, 143);
            this.GroupBox17.TabIndex = 76;
            this.GroupBox17.TabStop = false;
            this.GroupBox17.Text = "Font Size/Colors:";
            // 
            // chkColorInherent
            // 
            this.chkColorInherent.Enabled = false;
            this.chkColorInherent.Location = new System.Drawing.Point(11, 117);
            this.chkColorInherent.Name = "chkColorInherent";
            this.chkColorInherent.Size = new System.Drawing.Size(222, 20);
            this.chkColorInherent.TabIndex = 70;
            this.chkColorInherent.Text = "Use alternate colors for inherents";
            this.myTip.SetToolTip(this.chkColorInherent, "Draws villain builds in red rather than blue.");
            this.chkColorInherent.Visible = false;
            // 
            // chkHighVis
            // 
            this.chkHighVis.Location = new System.Drawing.Point(140, 91);
            this.chkHighVis.Name = "chkHighVis";
            this.chkHighVis.Size = new System.Drawing.Size(222, 20);
            this.chkHighVis.TabIndex = 69;
            this.chkHighVis.Text = "Use High-Visiblity text on the build view";
            this.myTip.SetToolTip(this.chkHighVis, "Draw white text with a black outline on the build view (power bars on the right o" +
        "f the screen)");
            // 
            // Label36
            // 
            this.Label36.Location = new System.Drawing.Point(141, 9);
            this.Label36.Name = "Label36";
            this.Label36.Size = new System.Drawing.Size(39, 16);
            this.Label36.TabIndex = 64;
            this.Label36.Text = "Bold";
            this.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkStatBold
            // 
            this.chkStatBold.AutoSize = true;
            this.chkStatBold.Location = new System.Drawing.Point(150, 51);
            this.chkStatBold.Name = "chkStatBold";
            this.chkStatBold.Size = new System.Drawing.Size(15, 14);
            this.chkStatBold.TabIndex = 63;
            this.chkStatBold.UseVisualStyleBackColor = true;
            // 
            // chkTextBold
            // 
            this.chkTextBold.AutoSize = true;
            this.chkTextBold.Location = new System.Drawing.Point(150, 26);
            this.chkTextBold.Name = "chkTextBold";
            this.chkTextBold.Size = new System.Drawing.Size(15, 14);
            this.chkTextBold.TabIndex = 62;
            this.chkTextBold.UseVisualStyleBackColor = true;
            // 
            // btnFontColor
            // 
            this.btnFontColor.Location = new System.Drawing.Point(312, 36);
            this.btnFontColor.Name = "btnFontColor";
            this.btnFontColor.Size = new System.Drawing.Size(172, 27);
            this.btnFontColor.TabIndex = 61;
            this.btnFontColor.Text = "Set Colors...";
            this.btnFontColor.UseVisualStyleBackColor = true;
            this.btnFontColor.Click += new System.EventHandler(this.btnFontColor_Click);
            // 
            // Label22
            // 
            this.Label22.Location = new System.Drawing.Point(7, 48);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(78, 20);
            this.Label22.TabIndex = 60;
            this.Label22.Text = "Stats/Powers:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkVillainColor
            // 
            this.chkVillainColor.Enabled = false;
            this.chkVillainColor.Location = new System.Drawing.Point(262, 117);
            this.chkVillainColor.Name = "chkVillainColor";
            this.chkVillainColor.Size = new System.Drawing.Size(222, 20);
            this.chkVillainColor.TabIndex = 68;
            this.chkVillainColor.Text = "Use alternate colors for villains";
            this.myTip.SetToolTip(this.chkVillainColor, "Draws villain builds in red rather than blue.");
            this.chkVillainColor.Visible = false;
            // 
            // Label21
            // 
            this.Label21.Location = new System.Drawing.Point(7, 23);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(78, 20);
            this.Label21.TabIndex = 59;
            this.Label21.Text = "Info Tab Text:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udStatSize
            // 
            this.udStatSize.DecimalPlaces = 2;
            this.udStatSize.Location = new System.Drawing.Point(87, 48);
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
            this.udRTFSize.Location = new System.Drawing.Point(87, 22);
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
            8,
            0,
            0,
            0});
            // 
            // chkIOPrintLevels
            // 
            this.chkIOPrintLevels.Location = new System.Drawing.Point(207, 285);
            this.chkIOPrintLevels.Name = "chkIOPrintLevels";
            this.chkIOPrintLevels.Size = new System.Drawing.Size(221, 18);
            this.chkIOPrintLevels.TabIndex = 75;
            this.chkIOPrintLevels.Text = "Display Invention levels in printed builds";
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.rbGraphSimple);
            this.GroupBox5.Controls.Add(this.rbGraphStacked);
            this.GroupBox5.Controls.Add(this.rbGraphTwoLine);
            this.GroupBox5.Location = new System.Drawing.Point(388, 4);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(353, 125);
            this.GroupBox5.TabIndex = 72;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Damage Graph Style:";
            // 
            // rbGraphSimple
            // 
            this.rbGraphSimple.Location = new System.Drawing.Point(6, 89);
            this.rbGraphSimple.Name = "rbGraphSimple";
            this.rbGraphSimple.Size = new System.Drawing.Size(164, 32);
            this.rbGraphSimple.TabIndex = 5;
            this.rbGraphSimple.Text = "Base against Enhanced";
            this.myTip.SetToolTip(this.rbGraphSimple, "This graph type doesn\'t reflect the max damage potential of other powers.");
            // 
            // rbGraphStacked
            // 
            this.rbGraphStacked.Location = new System.Drawing.Point(6, 53);
            this.rbGraphStacked.Name = "rbGraphStacked";
            this.rbGraphStacked.Size = new System.Drawing.Size(286, 32);
            this.rbGraphStacked.TabIndex = 4;
            this.rbGraphStacked.Text = "Base + Enhanced (stacked) against Max Enhancable";
            this.myTip.SetToolTip(this.rbGraphStacked, "\'Max Enhacable\' is damage if slotted with 6 +3 damage enhancements.");
            // 
            // rbGraphTwoLine
            // 
            this.rbGraphTwoLine.Checked = true;
            this.rbGraphTwoLine.Location = new System.Drawing.Point(6, 17);
            this.rbGraphTwoLine.Name = "rbGraphTwoLine";
            this.rbGraphTwoLine.Size = new System.Drawing.Size(286, 32);
            this.rbGraphTwoLine.TabIndex = 3;
            this.rbGraphTwoLine.TabStop = true;
            this.rbGraphTwoLine.Text = "Base / Enhanced against Max Enhancable (Default)";
            this.myTip.SetToolTip(this.rbGraphTwoLine, "The blue bar will indicate base damage, the yellow bar will indicate enhanced dam" +
        "age.");
            // 
            // GroupBox14
            // 
            this.GroupBox14.Controls.Add(this.chkIOLevel);
            this.GroupBox14.Controls.Add(this.btnIOReset);
            this.GroupBox14.Controls.Add(this.Label40);
            this.GroupBox14.Controls.Add(this.udIOLevel);
            this.GroupBox14.Location = new System.Drawing.Point(196, 4);
            this.GroupBox14.Name = "GroupBox14";
            this.GroupBox14.Size = new System.Drawing.Size(188, 125);
            this.GroupBox14.TabIndex = 69;
            this.GroupBox14.TabStop = false;
            this.GroupBox14.Text = "Inventions:";
            // 
            // chkIOLevel
            // 
            this.chkIOLevel.Location = new System.Drawing.Point(8, 44);
            this.chkIOLevel.Name = "chkIOLevel";
            this.chkIOLevel.Size = new System.Drawing.Size(172, 24);
            this.chkIOLevel.TabIndex = 60;
            this.chkIOLevel.Text = "Display IO Levels";
            this.myTip.SetToolTip(this.chkIOLevel, "Show the level of Inventions in the build view");
            // 
            // btnIOReset
            // 
            this.btnIOReset.Location = new System.Drawing.Point(8, 72);
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
            this.GroupBox3.Size = new System.Drawing.Size(184, 301);
            this.GroupBox3.TabIndex = 62;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Regular Enhancements:";
            // 
            // chkRelSignOnly
            // 
            this.chkRelSignOnly.Location = new System.Drawing.Point(11, 252);
            this.chkRelSignOnly.Name = "chkRelSignOnly";
            this.chkRelSignOnly.Size = new System.Drawing.Size(167, 43);
            this.chkRelSignOnly.TabIndex = 69;
            this.chkRelSignOnly.Text = "Show signs only for relative levels (\'++\' rather than \'+2\')";
            this.myTip.SetToolTip(this.chkRelSignOnly, "Draws villain builds in red rather than blue.");
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
            this.TabPage2.Size = new System.Drawing.Size(749, 325);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Effects & Maths";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // Label16
            // 
            this.Label16.Location = new System.Drawing.Point(587, 297);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(57, 18);
            this.Label16.TabIndex = 66;
            this.Label16.Text = "Team Size";
            // 
            // TeamSize
            // 
            this.TeamSize.Location = new System.Drawing.Point(650, 295);
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
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.clbSuppression);
            this.GroupBox2.Controls.Add(this.Label8);
            this.GroupBox2.Location = new System.Drawing.Point(517, 4);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(221, 283);
            this.GroupBox2.TabIndex = 69;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Suppression";
            // 
            // clbSuppression
            // 
            this.clbSuppression.CheckOnClick = true;
            this.clbSuppression.FormattingEnabled = true;
            this.clbSuppression.Location = new System.Drawing.Point(9, 104);
            this.clbSuppression.Name = "clbSuppression";
            this.clbSuppression.Size = new System.Drawing.Size(206, 169);
            this.clbSuppression.TabIndex = 9;
            this.clbSuppression.SelectedIndexChanged += new System.EventHandler(this.clbSuppression_SelectedIndexChanged);
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(6, 28);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(209, 55);
            this.Label8.TabIndex = 65;
            this.Label8.Text = "Some effects are suppressed on specific conditions. You can enable those conditio" +
    "ns here.";
            // 
            // chkUseArcanaTime
            // 
            this.chkUseArcanaTime.Location = new System.Drawing.Point(8, 295);
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
            this.GroupBox15.Location = new System.Drawing.Point(316, 166);
            this.GroupBox15.Name = "GroupBox15";
            this.GroupBox15.Size = new System.Drawing.Size(195, 121);
            this.GroupBox15.TabIndex = 68;
            this.GroupBox15.TabStop = false;
            this.GroupBox15.Text = "Invention Effects:";
            // 
            // Label20
            // 
            this.Label20.Location = new System.Drawing.Point(6, 17);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(179, 49);
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
            this.GroupBox8.Size = new System.Drawing.Size(507, 156);
            this.GroupBox8.TabIndex = 3;
            this.GroupBox8.TabStop = false;
            this.GroupBox8.Text = "Chance of Damage:";
            // 
            // rbChanceIgnore
            // 
            this.rbChanceIgnore.Location = new System.Drawing.Point(221, 113);
            this.rbChanceIgnore.Name = "rbChanceIgnore";
            this.rbChanceIgnore.Size = new System.Drawing.Size(224, 20);
            this.rbChanceIgnore.TabIndex = 8;
            this.rbChanceIgnore.Text = "Ignore Extra Damage (Show minimum)";
            // 
            // rbChanceAverage
            // 
            this.rbChanceAverage.Checked = true;
            this.rbChanceAverage.Location = new System.Drawing.Point(11, 97);
            this.rbChanceAverage.Name = "rbChanceAverage";
            this.rbChanceAverage.Size = new System.Drawing.Size(204, 20);
            this.rbChanceAverage.TabIndex = 7;
            this.rbChanceAverage.TabStop = true;
            this.rbChanceAverage.Text = "Show Average (Damage x Chance)";
            // 
            // rbChanceMax
            // 
            this.rbChanceMax.Location = new System.Drawing.Point(221, 83);
            this.rbChanceMax.Name = "rbChanceMax";
            this.rbChanceMax.Size = new System.Drawing.Size(204, 20);
            this.rbChanceMax.TabIndex = 6;
            this.rbChanceMax.Text = "Show Max Possible Damage";
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(8, 16);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(499, 56);
            this.Label9.TabIndex = 4;
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.Label7);
            this.GroupBox6.Controls.Add(this.rbPvP);
            this.GroupBox6.Controls.Add(this.rbPvE);
            this.GroupBox6.Location = new System.Drawing.Point(8, 166);
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
            this.rbPvE.Text = "Show values for Critters (PvE)";
            // 
            // TabPage6
            // 
            this.TabPage6.Controls.Add(this.Label6);
            this.TabPage6.Controls.Add(this.GroupBox13);
            this.TabPage6.Controls.Add(this.GroupBox10);
            this.TabPage6.Controls.Add(this.GroupBox4);
            this.TabPage6.Location = new System.Drawing.Point(4, 23);
            this.TabPage6.Name = "TabPage6";
            this.TabPage6.Size = new System.Drawing.Size(749, 325);
            this.TabPage6.TabIndex = 5;
            this.TabPage6.Text = "Exemping & Base Values";
            this.TabPage6.UseVisualStyleBackColor = true;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(401, 85);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(332, 119);
            this.Label6.TabIndex = 71;
            this.Label6.Text = "Exemplar and level-accurate scaling features will be added in the future!";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GroupBox13
            // 
            this.GroupBox13.Controls.Add(this.udForceLevel);
            this.GroupBox13.Controls.Add(this.Label38);
            this.GroupBox13.Enabled = false;
            this.GroupBox13.Location = new System.Drawing.Point(284, 4);
            this.GroupBox13.Name = "GroupBox13";
            this.GroupBox13.Size = new System.Drawing.Size(96, 184);
            this.GroupBox13.TabIndex = 70;
            this.GroupBox13.TabStop = false;
            this.GroupBox13.Text = "Forced Level:";
            // 
            // udForceLevel
            // 
            this.udForceLevel.Location = new System.Drawing.Point(8, 124);
            this.udForceLevel.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udForceLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udForceLevel.Name = "udForceLevel";
            this.udForceLevel.Size = new System.Drawing.Size(80, 20);
            this.udForceLevel.TabIndex = 55;
            this.udForceLevel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // Label38
            // 
            this.Label38.Location = new System.Drawing.Point(4, 16);
            this.Label38.Name = "Label38";
            this.Label38.Size = new System.Drawing.Size(88, 100);
            this.Label38.TabIndex = 1;
            this.Label38.Text = "Slots/Powers placed after this level won\'t be included in stats, and will be dimm" +
    "ed in the build view.";
            // 
            // GroupBox10
            // 
            this.GroupBox10.Controls.Add(this.btnBaseReset);
            this.GroupBox10.Controls.Add(this.Label14);
            this.GroupBox10.Controls.Add(this.udBaseToHit);
            this.GroupBox10.Controls.Add(this.Label13);
            this.GroupBox10.Location = new System.Drawing.Point(4, 196);
            this.GroupBox10.Name = "GroupBox10";
            this.GroupBox10.Size = new System.Drawing.Size(376, 104);
            this.GroupBox10.TabIndex = 69;
            this.GroupBox10.TabStop = false;
            this.GroupBox10.Text = "Base Values:";
            // 
            // btnBaseReset
            // 
            this.btnBaseReset.Location = new System.Drawing.Point(240, 76);
            this.btnBaseReset.Name = "btnBaseReset";
            this.btnBaseReset.Size = new System.Drawing.Size(120, 20);
            this.btnBaseReset.TabIndex = 61;
            this.btnBaseReset.Text = "Reset Values";
            this.btnBaseReset.Click += new System.EventHandler(this.btnBaseReset_Click);
            // 
            // Label14
            // 
            this.Label14.Location = new System.Drawing.Point(147, 39);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(112, 20);
            this.Label14.TabIndex = 58;
            this.Label14.Text = "Base ToHit:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBaseToHit
            // 
            this.udBaseToHit.Location = new System.Drawing.Point(263, 39);
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
            // Label13
            // 
            this.Label13.Location = new System.Drawing.Point(8, 16);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(136, 84);
            this.Label13.TabIndex = 0;
            this.Label13.Text = "These values are used as the base for stats calculation. They shouldn\'t ever need" +
    " to be changed unless there\'s a change to the game.";
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.Label12);
            this.GroupBox4.Controls.Add(this.udExLow);
            this.GroupBox4.Controls.Add(this.Label11);
            this.GroupBox4.Controls.Add(this.Label5);
            this.GroupBox4.Controls.Add(this.udExHigh);
            this.GroupBox4.Enabled = false;
            this.GroupBox4.Location = new System.Drawing.Point(4, 4);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(276, 184);
            this.GroupBox4.TabIndex = 68;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Exemplar Level:";
            // 
            // Label12
            // 
            this.Label12.Location = new System.Drawing.Point(8, 148);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(160, 20);
            this.Label12.TabIndex = 58;
            this.Label12.Text = "Exemplared (Lower) Level:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udExLow
            // 
            this.udExLow.Location = new System.Drawing.Point(172, 148);
            this.udExLow.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udExLow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udExLow.Name = "udExLow";
            this.udExLow.Size = new System.Drawing.Size(88, 20);
            this.udExLow.TabIndex = 57;
            this.myTip.SetToolTip(this.udExLow, "Set the target exemplar level. Your enhancements will be calculated as though you" +
        " are exemplared to this level, and their effect will be reduced accordingly");
            this.udExLow.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // Label11
            // 
            this.Label11.Location = new System.Drawing.Point(8, 124);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(160, 20);
            this.Label11.TabIndex = 56;
            this.Label11.Text = "Starting (Higher) Level:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(8, 16);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(264, 96);
            this.Label5.TabIndex = 55;
            // 
            // udExHigh
            // 
            this.udExHigh.Location = new System.Drawing.Point(172, 124);
            this.udExHigh.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udExHigh.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udExHigh.Name = "udExHigh";
            this.udExHigh.Size = new System.Drawing.Size(88, 20);
            this.udExHigh.TabIndex = 54;
            this.udExHigh.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.label41);
            this.tabPage7.Controls.Add(this.invBot);
            this.tabPage7.Controls.Add(this.label39);
            this.tabPage7.Controls.Add(this.groupBox21);
            this.tabPage7.Controls.Add(this.GroupBox20);
            this.tabPage7.Controls.Add(this.GroupBox19);
            this.tabPage7.Controls.Add(this.GroupBox18);
            this.tabPage7.Location = new System.Drawing.Point(4, 23);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(749, 325);
            this.tabPage7.TabIndex = 7;
            this.tabPage7.Text = "Discord Export Settings";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Enabled = false;
            this.label41.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label41.Location = new System.Drawing.Point(497, 252);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(224, 30);
            this.label41.TabIndex = 7;
            this.label41.Text = "You MUST have admin permissions\r\nin your Discord to invite.";
            // 
            // invBot
            // 
            this.invBot.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invBot.Location = new System.Drawing.Point(567, 226);
            this.invBot.Name = "invBot";
            this.invBot.Size = new System.Drawing.Size(102, 23);
            this.invBot.TabIndex = 6;
            this.invBot.Text = "Invite MidsBot";
            this.invBot.UseVisualStyleBackColor = true;
            this.invBot.Click += new System.EventHandler(this.InvBot_Click);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(494, 178);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(247, 28);
            this.label39.TabIndex = 5;
            this.label39.Text = "Want to share builds in your Discord?\r\nClick the button below to invite the MidsB" +
    "ot";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.checkBox24);
            this.groupBox21.Controls.Add(this.groupBox23);
            this.groupBox21.Controls.Add(this.groupBox22);
            this.groupBox21.Controls.Add(this.checkBox23);
            this.groupBox21.Controls.Add(this.checkBox22);
            this.groupBox21.Controls.Add(this.checkBox21);
            this.groupBox21.Controls.Add(this.checkBox20);
            this.groupBox21.Controls.Add(this.checkBox19);
            this.groupBox21.Controls.Add(this.checkBox18);
            this.groupBox21.Controls.Add(this.checkBox17);
            this.groupBox21.Controls.Add(this.checkBox16);
            this.groupBox21.Controls.Add(this.checkBox15);
            this.groupBox21.Controls.Add(this.checkBox14);
            this.groupBox21.Controls.Add(this.checkBox13);
            this.groupBox21.Controls.Add(this.checkBox12);
            this.groupBox21.Controls.Add(this.checkBox11);
            this.groupBox21.Controls.Add(this.checkBox10);
            this.groupBox21.Controls.Add(this.checkBox9);
            this.groupBox21.Controls.Add(this.checkBox8);
            this.groupBox21.Controls.Add(this.checkBox7);
            this.groupBox21.Controls.Add(this.checkBox6);
            this.groupBox21.Controls.Add(this.checkBox5);
            this.groupBox21.Controls.Add(this.checkBox4);
            this.groupBox21.Controls.Add(this.checkBox3);
            this.groupBox21.Controls.Add(this.checkBox2);
            this.groupBox21.Controls.Add(this.checkBox1);
            this.groupBox21.Controls.Add(this.label35);
            this.groupBox21.Controls.Add(this.label23);
            this.groupBox21.Controls.Add(this.label2);
            this.groupBox21.Location = new System.Drawing.Point(6, 109);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(480, 189);
            this.groupBox21.TabIndex = 4;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Stats To Include In Build Preview (Min: 3 - Max: 6):";
            // 
            // checkBox24
            // 
            this.checkBox24.AutoSize = true;
            this.checkBox24.Location = new System.Drawing.Point(355, 165);
            this.checkBox24.Name = "checkBox24";
            this.checkBox24.Size = new System.Drawing.Size(52, 18);
            this.checkBox24.TabIndex = 27;
            this.checkBox24.Text = "To Hit";
            this.checkBox24.UseVisualStyleBackColor = true;
            // 
            // groupBox23
            // 
            this.groupBox23.Enabled = false;
            this.groupBox23.Location = new System.Drawing.Point(332, 35);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(10, 154);
            this.groupBox23.TabIndex = 26;
            this.groupBox23.TabStop = false;
            // 
            // groupBox22
            // 
            this.groupBox22.Enabled = false;
            this.groupBox22.Location = new System.Drawing.Point(154, 35);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(10, 154);
            this.groupBox22.TabIndex = 5;
            this.groupBox22.TabStop = false;
            // 
            // checkBox23
            // 
            this.checkBox23.AutoSize = true;
            this.checkBox23.Location = new System.Drawing.Point(355, 141);
            this.checkBox23.Name = "checkBox23";
            this.checkBox23.Size = new System.Drawing.Size(57, 18);
            this.checkBox23.TabIndex = 25;
            this.checkBox23.Text = "Regen";
            this.checkBox23.UseVisualStyleBackColor = true;
            this.checkBox23.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox22
            // 
            this.checkBox22.AutoSize = true;
            this.checkBox22.Location = new System.Drawing.Point(355, 117);
            this.checkBox22.Name = "checkBox22";
            this.checkBox22.Size = new System.Drawing.Size(73, 18);
            this.checkBox22.TabIndex = 24;
            this.checkBox22.Text = "Recovery";
            this.checkBox22.UseVisualStyleBackColor = true;
            this.checkBox22.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox21
            // 
            this.checkBox21.AutoSize = true;
            this.checkBox21.Location = new System.Drawing.Point(355, 93);
            this.checkBox21.Name = "checkBox21";
            this.checkBox21.Size = new System.Drawing.Size(106, 18);
            this.checkBox21.TabIndex = 23;
            this.checkBox21.Text = "Global Recharge";
            this.checkBox21.UseVisualStyleBackColor = true;
            this.checkBox21.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox20
            // 
            this.checkBox20.AutoSize = true;
            this.checkBox20.Location = new System.Drawing.Point(355, 69);
            this.checkBox20.Name = "checkBox20";
            this.checkBox20.Size = new System.Drawing.Size(112, 18);
            this.checkBox20.TabIndex = 22;
            this.checkBox20.Text = "Endurance Usage";
            this.checkBox20.UseVisualStyleBackColor = true;
            this.checkBox20.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox19
            // 
            this.checkBox19.AutoSize = true;
            this.checkBox19.Location = new System.Drawing.Point(355, 45);
            this.checkBox19.Name = "checkBox19";
            this.checkBox19.Size = new System.Drawing.Size(89, 18);
            this.checkBox19.TabIndex = 21;
            this.checkBox19.Text = "Damage Buff";
            this.checkBox19.UseVisualStyleBackColor = true;
            this.checkBox19.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox18
            // 
            this.checkBox18.AutoSize = true;
            this.checkBox18.Location = new System.Drawing.Point(258, 93);
            this.checkBox18.Name = "checkBox18";
            this.checkBox18.Size = new System.Drawing.Size(60, 18);
            this.checkBox18.TabIndex = 20;
            this.checkBox18.Text = "Psionic";
            this.checkBox18.UseVisualStyleBackColor = true;
            this.checkBox18.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox17
            // 
            this.checkBox17.AutoSize = true;
            this.checkBox17.Location = new System.Drawing.Point(258, 69);
            this.checkBox17.Name = "checkBox17";
            this.checkBox17.Size = new System.Drawing.Size(51, 18);
            this.checkBox17.TabIndex = 19;
            this.checkBox17.Text = "Toxic";
            this.checkBox17.UseVisualStyleBackColor = true;
            this.checkBox17.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox16
            // 
            this.checkBox16.AutoSize = true;
            this.checkBox16.Location = new System.Drawing.Point(258, 45);
            this.checkBox16.Name = "checkBox16";
            this.checkBox16.Size = new System.Drawing.Size(68, 18);
            this.checkBox16.TabIndex = 18;
            this.checkBox16.Text = "Negative";
            this.checkBox16.UseVisualStyleBackColor = true;
            this.checkBox16.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox15
            // 
            this.checkBox15.AutoSize = true;
            this.checkBox15.Location = new System.Drawing.Point(179, 141);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new System.Drawing.Size(60, 18);
            this.checkBox15.TabIndex = 17;
            this.checkBox15.Text = "Energy";
            this.checkBox15.UseVisualStyleBackColor = true;
            this.checkBox15.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox14
            // 
            this.checkBox14.AutoSize = true;
            this.checkBox14.Location = new System.Drawing.Point(179, 117);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(47, 18);
            this.checkBox14.TabIndex = 16;
            this.checkBox14.Text = "Cold";
            this.checkBox14.UseVisualStyleBackColor = true;
            this.checkBox14.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox13
            // 
            this.checkBox13.AutoSize = true;
            this.checkBox13.Location = new System.Drawing.Point(179, 93);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(44, 18);
            this.checkBox13.TabIndex = 15;
            this.checkBox13.Text = "Fire";
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox13.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox12
            // 
            this.checkBox12.AutoSize = true;
            this.checkBox12.Location = new System.Drawing.Point(179, 69);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(55, 18);
            this.checkBox12.TabIndex = 14;
            this.checkBox12.Text = "Lethal";
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox12.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(179, 45);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(73, 18);
            this.checkBox11.TabIndex = 13;
            this.checkBox11.Text = "Smashing";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(85, 141);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(48, 18);
            this.checkBox10.TabIndex = 12;
            this.checkBox10.Text = "AOE";
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox10.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Location = new System.Drawing.Point(85, 117);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(63, 18);
            this.checkBox9.TabIndex = 11;
            this.checkBox9.Text = "Ranged";
            this.checkBox9.UseVisualStyleBackColor = true;
            this.checkBox9.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(85, 93);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(54, 18);
            this.checkBox8.TabIndex = 10;
            this.checkBox8.Text = "Melee";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(85, 69);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(60, 18);
            this.checkBox7.TabIndex = 9;
            this.checkBox7.Text = "Psionic";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(85, 45);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(68, 18);
            this.checkBox6.TabIndex = 8;
            this.checkBox6.Text = "Negative";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(6, 141);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(60, 18);
            this.checkBox5.TabIndex = 7;
            this.checkBox5.Text = "Energy";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(6, 117);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(47, 18);
            this.checkBox4.TabIndex = 6;
            this.checkBox4.Text = "Cold";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(6, 93);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(44, 18);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.Text = "Fire";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 69);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(55, 18);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Lethal";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 45);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(73, 18);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Smashing";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(384, 25);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(38, 14);
            this.label35.TabIndex = 2;
            this.label35.Text = "Other";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(212, 25);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(68, 14);
            this.label23.TabIndex = 1;
            this.label23.Text = "Resistance";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(42, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Defense";
            // 
            // GroupBox20
            // 
            this.GroupBox20.Controls.Add(this.dcChannel);
            this.GroupBox20.Controls.Add(this.Label17);
            this.GroupBox20.Location = new System.Drawing.Point(270, 6);
            this.GroupBox20.Name = "GroupBox20";
            this.GroupBox20.Size = new System.Drawing.Size(216, 97);
            this.GroupBox20.TabIndex = 3;
            this.GroupBox20.TabStop = false;
            this.GroupBox20.Text = "Discord Channel (case sensitive)";
            // 
            // dcChannel
            // 
            this.dcChannel.Location = new System.Drawing.Point(6, 28);
            this.dcChannel.Name = "dcChannel";
            this.dcChannel.Size = new System.Drawing.Size(199, 20);
            this.dcChannel.TabIndex = 2;
            this.dcChannel.TextChanged += new System.EventHandler(this.dcChannel_TextChanged);
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(8, 54);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(202, 14);
            this.Label17.TabIndex = 3;
            this.Label17.Text = "Channel name to Export to without the #:";
            // 
            // GroupBox19
            // 
            this.GroupBox19.Controls.Add(this.Label18);
            this.GroupBox19.Controls.Add(this.dcServerName);
            this.GroupBox19.Controls.Add(this.dcRemove);
            this.GroupBox19.Controls.Add(this.dcAdd);
            this.GroupBox19.Controls.Add(this.dcExList);
            this.GroupBox19.Location = new System.Drawing.Point(492, 6);
            this.GroupBox19.Name = "GroupBox19";
            this.GroupBox19.Size = new System.Drawing.Size(249, 154);
            this.GroupBox19.TabIndex = 1;
            this.GroupBox19.TabStop = false;
            this.GroupBox19.Text = "Discord Server (case sensitive):";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(26, 110);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(111, 14);
            this.Label18.TabIndex = 5;
            this.Label18.Text = "Discord server name:";
            // 
            // dcServerName
            // 
            this.dcServerName.Location = new System.Drawing.Point(6, 128);
            this.dcServerName.Name = "dcServerName";
            this.dcServerName.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dcServerName.Size = new System.Drawing.Size(157, 20);
            this.dcServerName.TabIndex = 4;
            // 
            // dcRemove
            // 
            this.dcRemove.Location = new System.Drawing.Point(168, 68);
            this.dcRemove.Name = "dcRemove";
            this.dcRemove.Size = new System.Drawing.Size(75, 23);
            this.dcRemove.TabIndex = 3;
            this.dcRemove.Text = "Remove";
            this.dcRemove.UseVisualStyleBackColor = true;
            this.dcRemove.Click += new System.EventHandler(this.dcRemove_Click);
            // 
            // dcAdd
            // 
            this.dcAdd.Location = new System.Drawing.Point(168, 39);
            this.dcAdd.Name = "dcAdd";
            this.dcAdd.Size = new System.Drawing.Size(75, 23);
            this.dcAdd.TabIndex = 2;
            this.dcAdd.Text = "Add";
            this.dcAdd.UseVisualStyleBackColor = true;
            this.dcAdd.Click += new System.EventHandler(this.dcAdd_Click);
            // 
            // dcExList
            // 
            this.dcExList.FormattingEnabled = true;
            this.dcExList.ItemHeight = 14;
            this.dcExList.Items.AddRange(new object[] {
            "Mids Reborn (Default)"});
            this.dcExList.Location = new System.Drawing.Point(6, 19);
            this.dcExList.Name = "dcExList";
            this.dcExList.Size = new System.Drawing.Size(157, 88);
            this.dcExList.TabIndex = 0;
            this.dcExList.SelectedIndexChanged += new System.EventHandler(this.dcExList_SelectedIndexChanged);
            // 
            // GroupBox18
            // 
            this.GroupBox18.Controls.Add(this.richTextBox1);
            this.GroupBox18.Controls.Add(this.dcNickName);
            this.GroupBox18.Location = new System.Drawing.Point(8, 6);
            this.GroupBox18.Name = "GroupBox18";
            this.GroupBox18.Size = new System.Drawing.Size(256, 97);
            this.GroupBox18.TabIndex = 0;
            this.GroupBox18.TabStop = false;
            this.GroupBox18.Text = "Discord Nickname (case sensitive):";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox1.Location = new System.Drawing.Point(6, 54);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox1.Size = new System.Drawing.Size(244, 37);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "This should be set to the same nickname you use in Discord without the #xxxx. *It" +
    " will be checked*";
            // 
            // dcNickName
            // 
            this.dcNickName.Location = new System.Drawing.Point(6, 28);
            this.dcNickName.Name = "dcNickName";
            this.dcNickName.Size = new System.Drawing.Size(244, 20);
            this.dcNickName.TabIndex = 0;
            this.dcNickName.TextChanged += new System.EventHandler(this.dcNickName_TextChanged);
            // 
            // TabPage4
            // 
            this.TabPage4.Controls.Add(this.GroupBox12);
            this.TabPage4.Controls.Add(this.GroupBox11);
            this.TabPage4.Location = new System.Drawing.Point(4, 23);
            this.TabPage4.Name = "TabPage4";
            this.TabPage4.Size = new System.Drawing.Size(749, 325);
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
            this.GroupBox12.Size = new System.Drawing.Size(561, 308);
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
            this.Label25.Size = new System.Drawing.Size(407, 76);
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
            // TabPage5
            // 
            this.TabPage5.Controls.Add(this.cbUpdateURL);
            this.TabPage5.Controls.Add(this.lblUpdateURL);
            this.TabPage5.Controls.Add(this.btnSaveFolderReset);
            this.TabPage5.Controls.Add(this.lblSaveFolder);
            this.TabPage5.Controls.Add(this.btnSaveFolder);
            this.TabPage5.Controls.Add(this.chkLoadLastFile);
            this.TabPage5.Controls.Add(this.Label1);
            this.TabPage5.Controls.Add(this.GroupBox16);
            this.TabPage5.Controls.Add(this.GroupBox1);
            this.TabPage5.Location = new System.Drawing.Point(4, 23);
            this.TabPage5.Name = "TabPage5";
            this.TabPage5.Size = new System.Drawing.Size(749, 325);
            this.TabPage5.TabIndex = 4;
            this.TabPage5.Text = "Updates & Paths";
            this.TabPage5.UseVisualStyleBackColor = true;
            // 
            // cbUpdateURL
            // 
            this.cbUpdateURL.AutoCompleteCustomSource.AddRange(new string[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml",
            "http://keepers.dk/mids/update.xml"});
            this.cbUpdateURL.FormattingEnabled = true;
            this.cbUpdateURL.Items.AddRange(new object[] {
            "https://midsreborn.com/mids_updates/update_manifest.xml",
            "http://keepers.dk/mids/update.xml",
            "http://keepers.dk/mids/update_beta.xml",
            "http://keepers.dk/mids/update_dev.xml"});
            this.cbUpdateURL.Location = new System.Drawing.Point(17, 200);
            this.cbUpdateURL.Name = "cbUpdateURL";
            this.cbUpdateURL.Size = new System.Drawing.Size(593, 22);
            this.cbUpdateURL.TabIndex = 67;
            // 
            // lblUpdateURL
            // 
            this.lblUpdateURL.Location = new System.Drawing.Point(8, 175);
            this.lblUpdateURL.Name = "lblUpdateURL";
            this.lblUpdateURL.Size = new System.Drawing.Size(168, 24);
            this.lblUpdateURL.TabIndex = 65;
            this.lblUpdateURL.Text = "Update URL:";
            this.lblUpdateURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.lblSaveFolder.Location = new System.Drawing.Point(17, 270);
            this.lblSaveFolder.Name = "lblSaveFolder";
            this.lblSaveFolder.Size = new System.Drawing.Size(593, 22);
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
            this.chkLoadLastFile.Location = new System.Drawing.Point(17, 295);
            this.chkLoadLastFile.Name = "chkLoadLastFile";
            this.chkLoadLastFile.Size = new System.Drawing.Size(156, 16);
            this.chkLoadLastFile.TabIndex = 61;
            this.chkLoadLastFile.Text = "Load last build on startup";
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(5, 246);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(168, 24);
            this.Label1.TabIndex = 8;
            this.Label1.Text = "Save Mids Builds To:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GroupBox16
            // 
            this.GroupBox16.Controls.Add(this.Label19);
            this.GroupBox16.Location = new System.Drawing.Point(417, 3);
            this.GroupBox16.Name = "GroupBox16";
            this.GroupBox16.Size = new System.Drawing.Size(324, 169);
            this.GroupBox16.TabIndex = 6;
            this.GroupBox16.TabStop = false;
            this.GroupBox16.Text = "Panic Button:";
            this.GroupBox16.Visible = false;
            // 
            // Label19
            // 
            this.Label19.Location = new System.Drawing.Point(5, 21);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(313, 117);
            this.Label19.TabIndex = 7;
            this.Label19.Text = "If the database is damaged and not working properly, you can force the applicatio" +
    "n to redownload the most recent version.";
            this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.Label37);
            this.GroupBox1.Controls.Add(this.Label34);
            this.GroupBox1.Controls.Add(this.chkUpdates);
            this.GroupBox1.Location = new System.Drawing.Point(8, 3);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(403, 169);
            this.GroupBox1.TabIndex = 7;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Automatic Updates:";
            // 
            // Label37
            // 
            this.Label37.Location = new System.Drawing.Point(13, 108);
            this.Label37.Name = "Label37";
            this.Label37.Size = new System.Drawing.Size(384, 43);
            this.Label37.TabIndex = 7;
            this.Label37.Text = "Please note that the availability of automatic updates may vary due to bandwidth " +
    "use of the hosting site.";
            this.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label34
            // 
            this.Label34.Location = new System.Drawing.Point(6, 16);
            this.Label34.Name = "Label34";
            this.Label34.Size = new System.Drawing.Size(384, 41);
            this.Label34.TabIndex = 5;
            this.Label34.Text = "Mids Reborn can automatically check for updates and download newer versions when " +
    "it starts. This feature requires an internet connection.";
            this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkUpdates
            // 
            this.chkUpdates.Location = new System.Drawing.Point(9, 68);
            this.chkUpdates.Name = "chkUpdates";
            this.chkUpdates.Size = new System.Drawing.Size(304, 24);
            this.chkUpdates.TabIndex = 6;
            this.chkUpdates.Text = "Check for updates on startup";
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
            this.TabPage1.Size = new System.Drawing.Size(749, 325);
            this.TabPage1.TabIndex = 6;
            this.TabPage1.Text = "Drag & Drop";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // Label15
            // 
            this.Label15.Location = new System.Drawing.Point(14, 9);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(721, 92);
            this.Label15.TabIndex = 4;
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
            this.GroupBox9.Size = new System.Drawing.Size(338, 161);
            this.GroupBox9.TabIndex = 1;
            this.GroupBox9.TabStop = false;
            this.GroupBox9.Text = "Description / Example(s)";
            // 
            // lblExample
            // 
            this.lblExample.Location = new System.Drawing.Point(13, 17);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(319, 130);
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
            // chkColorPrint
            // 
            this.chkColorPrint.Location = new System.Drawing.Point(246, 367);
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
            this.ClientSize = new System.Drawing.Size(757, 392);
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
            this.GroupBox17.ResumeLayout(false);
            this.GroupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udStatSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRTFSize)).EndInit();
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udIOLevel)).EndInit();
            this.GroupBox3.ResumeLayout(false);
            this.TabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TeamSize)).EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox15.ResumeLayout(false);
            this.GroupBox8.ResumeLayout(false);
            this.GroupBox6.ResumeLayout(false);
            this.TabPage6.ResumeLayout(false);
            this.GroupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udForceLevel)).EndInit();
            this.GroupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udBaseToHit)).EndInit();
            this.GroupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udExLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udExHigh)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.GroupBox20.ResumeLayout(false);
            this.GroupBox20.PerformLayout();
            this.GroupBox19.ResumeLayout(false);
            this.GroupBox19.PerformLayout();
            this.GroupBox18.ResumeLayout(false);
            this.GroupBox18.PerformLayout();
            this.TabPage4.ResumeLayout(false);
            this.GroupBox12.ResumeLayout(false);
            this.GroupBox12.PerformLayout();
            this.GroupBox11.ResumeLayout(false);
            this.TabPage5.ResumeLayout(false);
            this.GroupBox16.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.GroupBox9.ResumeLayout(false);
            this.GroupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        Button btnBaseReset;
        Button btnCancel;
        Button btnFontColor;
        Button btnIOReset;
        Button btnOK;
        Button btnSaveFolder;
        Button btnSaveFolderReset;
        ComboBox cbEnhLevel;
        CheckBox chkColorInherent;
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
        CheckBox chkVillainColor;
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
        Button dcAdd;
        Button dcRemove;
        Button fcReset;
        Button fcSet;
        TextBox fcTextOff;
        TextBox fcTextOn;
        TextBox fcUnderlineOff;
        TextBox fcUnderlineOn;
        RadioButton fcWSSpace;
        RadioButton fcWSTab;
        GroupBox GroupBox1;
        GroupBox GroupBox10;
        GroupBox GroupBox11;
        GroupBox GroupBox12;
        GroupBox GroupBox13;
        GroupBox GroupBox14;
        GroupBox GroupBox15;
        GroupBox GroupBox16;
        GroupBox GroupBox17;
        GroupBox GroupBox18;
        GroupBox GroupBox19;
        GroupBox GroupBox2;
        GroupBox GroupBox20;
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
        Label Label19;
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
        Label Label37;
        Label Label38;
        Label Label4;
        Label Label40;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblExample;
        Label lblSaveFolder;
        ListBox dcExList;
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
        RichTextBox richTextBox1;
        TabControl TabControl1;
        TabPage TabPage1;
        TabPage TabPage2;
        TabPage TabPage3;
        TabPage TabPage4;
        TabPage TabPage5;
        TabPage TabPage6;
        TabPage tabPage7;
        NumericUpDown TeamSize;
        TextBox dcChannel;
        TextBox dcNickName;
        TextBox dcServerName;
        //System.Windows.Forms.TextBox txtUpdatePath;
        NumericUpDown udBaseToHit;
        NumericUpDown udExHigh;
        NumericUpDown udExLow;
        NumericUpDown udForceLevel;
        NumericUpDown udIOLevel;
        NumericUpDown udRTFSize;
        NumericUpDown udStatSize;
        private GroupBox groupBox21;
        private CheckBox checkBox10;
        private CheckBox checkBox9;
        private CheckBox checkBox8;
        private CheckBox checkBox7;
        private CheckBox checkBox6;
        private CheckBox checkBox5;
        private CheckBox checkBox4;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Label label35;
        private Label label23;
        private Label label2;
        private Button invBot;
        private Label label39;
        private GroupBox groupBox23;
        private GroupBox groupBox22;
        private CheckBox checkBox23;
        private CheckBox checkBox22;
        private CheckBox checkBox21;
        private CheckBox checkBox20;
        private CheckBox checkBox19;
        private CheckBox checkBox18;
        private CheckBox checkBox17;
        private CheckBox checkBox16;
        private CheckBox checkBox15;
        private CheckBox checkBox14;
        private CheckBox checkBox13;
        private CheckBox checkBox12;
        private CheckBox checkBox11;
        private Label label41;
        private CheckBox checkBox24;
        private Label lblUpdateURL;
        private ComboBox cbUpdateURL;
    }
}
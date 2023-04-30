using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEnhData
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
            this.gbBasic = new System.Windows.Forms.GroupBox();
            this.txtInternal = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.udMinLevel = new System.Windows.Forms.NumericUpDown();
            this.udMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtNameShort = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtNameFull = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.btnImage = new System.Windows.Forms.Button();
            this.gbType = new System.Windows.Forms.GroupBox();
            this.cbSubType = new System.Windows.Forms.ComboBox();
            this.typeSet = new System.Windows.Forms.RadioButton();
            this.typeIO = new System.Windows.Forms.RadioButton();
            this.typeRegular = new System.Windows.Forms.RadioButton();
            this.typeHO = new System.Windows.Forms.RadioButton();
            this.cbSet = new System.Windows.Forms.ComboBox();
            this.gbSet = new System.Windows.Forms.GroupBox();
            this.chkSuperior = new System.Windows.Forms.CheckBox();
            this.chkUnique = new System.Windows.Forms.CheckBox();
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.rbDebuff = new System.Windows.Forms.RadioButton();
            this.rbBuff = new System.Windows.Forms.RadioButton();
            this.btnAutoFill = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtProb = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAddFX = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.gbMod = new System.Windows.Forms.GroupBox();
            this.rbMod4 = new System.Windows.Forms.RadioButton();
            this.txtModOther = new System.Windows.Forms.TextBox();
            this.rbModOther = new System.Windows.Forms.RadioButton();
            this.rbMod3 = new System.Windows.Forms.RadioButton();
            this.rbMod2 = new System.Windows.Forms.RadioButton();
            this.rbMod1 = new System.Windows.Forms.RadioButton();
            this.lstSelected = new System.Windows.Forms.ListBox();
            this.lstAvailable = new System.Windows.Forms.ListBox();
            this.cbSched = new System.Windows.Forms.ComboBox();
            this.lblSched = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbClass = new System.Windows.Forms.GroupBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.pnlClassList = new System.Windows.Forms.Panel();
            this.pnlClass = new System.Windows.Forms.Panel();
            this.ImagePicker = new System.Windows.Forms.OpenFileDialog();
            this.btnNoImage = new System.Windows.Forms.Button();
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbMutEx = new System.Windows.Forms.ComboBox();
            this.cbRecipe = new System.Windows.Forms.ComboBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.btnEditPowerData = new System.Windows.Forms.Button();
            this.StaticIndex = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.gbFlags = new System.Windows.Forms.GroupBox();
            this.chkScalable = new System.Windows.Forms.CheckBox();
            this.chkProc = new System.Windows.Forms.CheckBox();
            this.gbBasic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMinLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxLevel)).BeginInit();
            this.gbType.SuspendLayout();
            this.gbSet.SuspendLayout();
            this.gbEffects.SuspendLayout();
            this.gbMod.SuspendLayout();
            this.gbClass.SuspendLayout();
            this.gbFlags.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBasic
            // 
            this.gbBasic.Controls.Add(this.txtInternal);
            this.gbBasic.Controls.Add(this.Label9);
            this.gbBasic.Controls.Add(this.Label7);
            this.gbBasic.Controls.Add(this.Label6);
            this.gbBasic.Controls.Add(this.udMinLevel);
            this.gbBasic.Controls.Add(this.udMaxLevel);
            this.gbBasic.Controls.Add(this.txtDesc);
            this.gbBasic.Controls.Add(this.Label4);
            this.gbBasic.Controls.Add(this.txtNameShort);
            this.gbBasic.Controls.Add(this.Label3);
            this.gbBasic.Controls.Add(this.txtNameFull);
            this.gbBasic.Controls.Add(this.Label2);
            this.gbBasic.Location = new System.Drawing.Point(96, 8);
            this.gbBasic.Name = "gbBasic";
            this.gbBasic.Size = new System.Drawing.Size(248, 169);
            this.gbBasic.TabIndex = 11;
            this.gbBasic.TabStop = false;
            this.gbBasic.Text = "Basic:";
            // 
            // txtInternal
            // 
            this.txtInternal.Location = new System.Drawing.Point(84, 68);
            this.txtInternal.Name = "txtInternal";
            this.txtInternal.Size = new System.Drawing.Size(156, 20);
            this.txtInternal.TabIndex = 21;
            this.txtInternal.TextChanged += new System.EventHandler(this.txtInternal_TextChanged);
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(8, 68);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(72, 20);
            this.Label9.TabIndex = 20;
            this.Label9.Text = "Internal:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(134, 140);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(56, 20);
            this.Label7.TabIndex = 19;
            this.Label7.Text = "to";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(6, 140);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(74, 20);
            this.Label6.TabIndex = 18;
            this.Label6.Text = "Level range:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udMinLevel
            // 
            this.udMinLevel.Location = new System.Drawing.Point(84, 140);
            this.udMinLevel.Maximum = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.udMinLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMinLevel.Name = "udMinLevel";
            this.udMinLevel.Size = new System.Drawing.Size(44, 20);
            this.udMinLevel.TabIndex = 17;
            this.udMinLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMinLevel.ValueChanged += new System.EventHandler(this.udMinLevel_ValueChanged);
            this.udMinLevel.Leave += new System.EventHandler(this.udMinLevel_Leave);
            // 
            // udMaxLevel
            // 
            this.udMaxLevel.Location = new System.Drawing.Point(196, 140);
            this.udMaxLevel.Maximum = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.udMaxLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMaxLevel.Name = "udMaxLevel";
            this.udMaxLevel.Size = new System.Drawing.Size(44, 20);
            this.udMaxLevel.TabIndex = 16;
            this.udMaxLevel.Value = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.udMaxLevel.ValueChanged += new System.EventHandler(this.udMaxLevel_ValueChanged);
            this.udMaxLevel.Leave += new System.EventHandler(this.udMaxLevel_Leave);
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(84, 94);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(156, 40);
            this.txtDesc.TabIndex = 15;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // Label4
            // 
            this.Label4.Location = new System.Drawing.Point(8, 98);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(72, 20);
            this.Label4.TabIndex = 14;
            this.Label4.Text = "Description:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameShort
            // 
            this.txtNameShort.Location = new System.Drawing.Point(84, 42);
            this.txtNameShort.Name = "txtNameShort";
            this.txtNameShort.Size = new System.Drawing.Size(156, 20);
            this.txtNameShort.TabIndex = 13;
            this.txtNameShort.TextChanged += new System.EventHandler(this.txtNameShort_TextChanged);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(8, 42);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(72, 20);
            this.Label3.TabIndex = 12;
            this.Label3.Text = "Short Name:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameFull
            // 
            this.txtNameFull.Location = new System.Drawing.Point(84, 16);
            this.txtNameFull.Name = "txtNameFull";
            this.txtNameFull.Size = new System.Drawing.Size(156, 20);
            this.txtNameFull.TabIndex = 11;
            this.txtNameFull.TextChanged += new System.EventHandler(this.txtNameFull_TextChanged);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(8, 16);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 20);
            this.Label2.TabIndex = 10;
            this.Label2.Text = "Full Name:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnImage
            // 
            this.btnImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnImage.Location = new System.Drawing.Point(8, 12);
            this.btnImage.Name = "btnImage";
            this.btnImage.Size = new System.Drawing.Size(80, 68);
            this.btnImage.TabIndex = 9;
            this.btnImage.Text = "ImageName";
            this.btnImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.cbSubType);
            this.gbType.Controls.Add(this.typeSet);
            this.gbType.Controls.Add(this.typeIO);
            this.gbType.Controls.Add(this.typeRegular);
            this.gbType.Controls.Add(this.typeHO);
            this.gbType.Location = new System.Drawing.Point(352, 8);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(140, 169);
            this.gbType.TabIndex = 2;
            this.gbType.TabStop = false;
            this.gbType.Text = "Enhancement Type:";
            // 
            // cbSubType
            // 
            this.cbSubType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubType.Location = new System.Drawing.Point(8, 138);
            this.cbSubType.Name = "cbSubType";
            this.cbSubType.Size = new System.Drawing.Size(124, 22);
            this.cbSubType.TabIndex = 54;
            this.tTip.SetToolTip(this.cbSubType, "(Currently only applicable to Stealth IOs");
            this.cbSubType.SelectedIndexChanged += new System.EventHandler(this.cbSubType_SelectedIndexChanged);
            // 
            // typeSet
            // 
            this.typeSet.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeSet.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeSet.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeSet.Location = new System.Drawing.Point(72, 76);
            this.typeSet.Name = "typeSet";
            this.typeSet.Size = new System.Drawing.Size(60, 56);
            this.typeSet.TabIndex = 53;
            this.typeSet.Text = "IO Set";
            this.typeSet.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.typeSet.CheckedChanged += new System.EventHandler(this.type_CheckedChanged);
            // 
            // typeIO
            // 
            this.typeIO.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeIO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeIO.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeIO.Location = new System.Drawing.Point(72, 16);
            this.typeIO.Name = "typeIO";
            this.typeIO.Size = new System.Drawing.Size(60, 56);
            this.typeIO.TabIndex = 52;
            this.typeIO.Text = "Invention";
            this.typeIO.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.typeIO.CheckedChanged += new System.EventHandler(this.type_CheckedChanged);
            // 
            // typeRegular
            // 
            this.typeRegular.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeRegular.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeRegular.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeRegular.Location = new System.Drawing.Point(8, 16);
            this.typeRegular.Name = "typeRegular";
            this.typeRegular.Size = new System.Drawing.Size(60, 56);
            this.typeRegular.TabIndex = 50;
            this.typeRegular.Text = "Regular";
            this.typeRegular.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.typeRegular.CheckedChanged += new System.EventHandler(this.type_CheckedChanged);
            // 
            // typeHO
            // 
            this.typeHO.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeHO.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeHO.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.typeHO.Location = new System.Drawing.Point(8, 76);
            this.typeHO.Name = "typeHO";
            this.typeHO.Size = new System.Drawing.Size(60, 56);
            this.typeHO.TabIndex = 51;
            this.typeHO.Text = "Special";
            this.typeHO.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.typeHO.CheckedChanged += new System.EventHandler(this.type_CheckedChanged);
            // 
            // cbSet
            // 
            this.cbSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSet.Location = new System.Drawing.Point(8, 20);
            this.cbSet.Name = "cbSet";
            this.cbSet.Size = new System.Drawing.Size(356, 22);
            this.cbSet.TabIndex = 13;
            this.cbSet.SelectedIndexChanged += new System.EventHandler(this.cbSet_SelectedIndexChanged);
            // 
            // gbSet
            // 
            this.gbSet.Controls.Add(this.cbSet);
            this.gbSet.Location = new System.Drawing.Point(496, 8);
            this.gbSet.Name = "gbSet";
            this.gbSet.Size = new System.Drawing.Size(372, 52);
            this.gbSet.TabIndex = 14;
            this.gbSet.TabStop = false;
            this.gbSet.Text = "Invention Origin Set:";
            // 
            // chkSuperior
            // 
            this.chkSuperior.Location = new System.Drawing.Point(8, 38);
            this.chkSuperior.Name = "chkSuperior";
            this.chkSuperior.Size = new System.Drawing.Size(72, 19);
            this.chkSuperior.TabIndex = 21;
            this.chkSuperior.Text = "Superior";
            this.chkSuperior.CheckedChanged += new System.EventHandler(this.chkSuperior_CheckedChanged);
            // 
            // chkUnique
            // 
            this.chkUnique.Location = new System.Drawing.Point(8, 16);
            this.chkUnique.Name = "chkUnique";
            this.chkUnique.Size = new System.Drawing.Size(64, 22);
            this.chkUnique.TabIndex = 20;
            this.chkUnique.Text = "Unique";
            this.chkUnique.CheckedChanged += new System.EventHandler(this.chkUnique_CheckedChanged);
            // 
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.btnDown);
            this.gbEffects.Controls.Add(this.btnUp);
            this.gbEffects.Controls.Add(this.rbBoth);
            this.gbEffects.Controls.Add(this.rbDebuff);
            this.gbEffects.Controls.Add(this.rbBuff);
            this.gbEffects.Controls.Add(this.btnAutoFill);
            this.gbEffects.Controls.Add(this.Label5);
            this.gbEffects.Controls.Add(this.txtProb);
            this.gbEffects.Controls.Add(this.Label1);
            this.gbEffects.Controls.Add(this.btnEdit);
            this.gbEffects.Controls.Add(this.btnAddFX);
            this.gbEffects.Controls.Add(this.btnRemove);
            this.gbEffects.Controls.Add(this.btnAdd);
            this.gbEffects.Controls.Add(this.gbMod);
            this.gbEffects.Controls.Add(this.lstSelected);
            this.gbEffects.Controls.Add(this.lstAvailable);
            this.gbEffects.Controls.Add(this.cbSched);
            this.gbEffects.Controls.Add(this.lblSched);
            this.gbEffects.Location = new System.Drawing.Point(4, 210);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(584, 303);
            this.gbEffects.TabIndex = 15;
            this.gbEffects.TabStop = false;
            this.gbEffects.Text = "Effects:";
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(188, 172);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(48, 20);
            this.btnDown.TabIndex = 32;
            this.btnDown.Text = "Down";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(188, 148);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(48, 20);
            this.btnUp.TabIndex = 31;
            this.btnUp.Text = "Up";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // rbBoth
            // 
            this.rbBoth.Checked = true;
            this.rbBoth.Location = new System.Drawing.Point(428, 228);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(148, 16);
            this.rbBoth.TabIndex = 30;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "Buff/Debuff Effects";
            this.tTip.SetToolTip(this.rbBoth, "Apply to effects regardles of whether the Mag is positive or negative");
            this.rbBoth.CheckedChanged += new System.EventHandler(this.rbBuffDebuff_CheckedChanged);
            // 
            // rbDebuff
            // 
            this.rbDebuff.Location = new System.Drawing.Point(428, 212);
            this.rbDebuff.Name = "rbDebuff";
            this.rbDebuff.Size = new System.Drawing.Size(148, 16);
            this.rbDebuff.TabIndex = 29;
            this.rbDebuff.Text = "Debuff Effects";
            this.tTip.SetToolTip(this.rbDebuff, "Apply only to effects with a negative Mag");
            this.rbDebuff.CheckedChanged += new System.EventHandler(this.rbBuffDebuff_CheckedChanged);
            // 
            // rbBuff
            // 
            this.rbBuff.Location = new System.Drawing.Point(428, 196);
            this.rbBuff.Name = "rbBuff";
            this.rbBuff.Size = new System.Drawing.Size(148, 16);
            this.rbBuff.TabIndex = 28;
            this.rbBuff.Text = "Buff Effects";
            this.tTip.SetToolTip(this.rbBuff, "Apply only to effects with a positive Mag");
            this.rbBuff.CheckedChanged += new System.EventHandler(this.rbBuffDebuff_CheckedChanged);
            // 
            // btnAutoFill
            // 
            this.btnAutoFill.Location = new System.Drawing.Point(128, 24);
            this.btnAutoFill.Name = "btnAutoFill";
            this.btnAutoFill.Size = new System.Drawing.Size(108, 32);
            this.btnAutoFill.TabIndex = 27;
            this.btnAutoFill.Text = "AutoFill Names";
            this.btnAutoFill.Click += new System.EventHandler(this.btnAutoFill_Click);
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(196, 244);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(28, 20);
            this.Label5.TabIndex = 26;
            this.Label5.Text = "(0-1)";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtProb
            // 
            this.txtProb.Location = new System.Drawing.Point(156, 244);
            this.txtProb.Name = "txtProb";
            this.txtProb.Size = new System.Drawing.Size(36, 20);
            this.txtProb.TabIndex = 25;
            this.txtProb.Text = "1";
            this.txtProb.TextChanged += new System.EventHandler(this.txtProb_TextChanged);
            this.txtProb.Leave += new System.EventHandler(this.txtProb_Leave);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(8, 244);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(148, 20);
            this.Label1.TabIndex = 24;
            this.Label1.Text = "Special Effect Probability:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnEdit
            // 
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnEdit.Location = new System.Drawing.Point(424, 248);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(152, 28);
            this.btnEdit.TabIndex = 23;
            this.btnEdit.Text = "Edit Selected...";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAddFX
            // 
            this.btnAddFX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddFX.Location = new System.Drawing.Point(8, 208);
            this.btnAddFX.Name = "btnAddFX";
            this.btnAddFX.Size = new System.Drawing.Size(228, 28);
            this.btnAddFX.TabIndex = 22;
            this.btnAddFX.Text = "Add Special Effect... ->";
            this.btnAddFX.Click += new System.EventHandler(this.btnAddFX_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRemove.Location = new System.Drawing.Point(240, 248);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(176, 28);
            this.btnRemove.TabIndex = 21;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAdd.Location = new System.Drawing.Point(128, 100);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(108, 28);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = "Add ->";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gbMod
            // 
            this.gbMod.Controls.Add(this.rbMod4);
            this.gbMod.Controls.Add(this.txtModOther);
            this.gbMod.Controls.Add(this.rbModOther);
            this.gbMod.Controls.Add(this.rbMod3);
            this.gbMod.Controls.Add(this.rbMod2);
            this.gbMod.Controls.Add(this.rbMod1);
            this.gbMod.Location = new System.Drawing.Point(424, 44);
            this.gbMod.Name = "gbMod";
            this.gbMod.Size = new System.Drawing.Size(152, 148);
            this.gbMod.TabIndex = 19;
            this.gbMod.TabStop = false;
            this.gbMod.Text = "Effect Modifier:";
            // 
            // rbMod4
            // 
            this.rbMod4.Location = new System.Drawing.Point(12, 80);
            this.rbMod4.Name = "rbMod4";
            this.rbMod4.Size = new System.Drawing.Size(128, 20);
            this.rbMod4.TabIndex = 5;
            this.rbMod4.Text = "0.4375 (4-Effect IO)";
            this.rbMod4.CheckedChanged += new System.EventHandler(this.rbMod_CheckedChanged);
            // 
            // txtModOther
            // 
            this.txtModOther.Enabled = false;
            this.txtModOther.Location = new System.Drawing.Point(28, 120);
            this.txtModOther.Name = "txtModOther";
            this.txtModOther.Size = new System.Drawing.Size(112, 20);
            this.txtModOther.TabIndex = 4;
            this.txtModOther.TextChanged += new System.EventHandler(this.txtModOther_TextChanged);
            // 
            // rbModOther
            // 
            this.rbModOther.Location = new System.Drawing.Point(12, 100);
            this.rbModOther.Name = "rbModOther";
            this.rbModOther.Size = new System.Drawing.Size(128, 20);
            this.rbModOther.TabIndex = 3;
            this.rbModOther.Text = "Other";
            this.rbModOther.CheckedChanged += new System.EventHandler(this.rbMod_CheckedChanged);
            // 
            // rbMod3
            // 
            this.rbMod3.Location = new System.Drawing.Point(12, 60);
            this.rbMod3.Name = "rbMod3";
            this.rbMod3.Size = new System.Drawing.Size(128, 20);
            this.rbMod3.TabIndex = 2;
            this.rbMod3.Text = "0.5 (3-Effect IO)";
            this.rbMod3.CheckedChanged += new System.EventHandler(this.rbMod_CheckedChanged);
            // 
            // rbMod2
            // 
            this.rbMod2.Location = new System.Drawing.Point(12, 40);
            this.rbMod2.Name = "rbMod2";
            this.rbMod2.Size = new System.Drawing.Size(128, 20);
            this.rbMod2.TabIndex = 1;
            this.rbMod2.Text = "0.625 (2-Effect IO)";
            this.rbMod2.CheckedChanged += new System.EventHandler(this.rbMod_CheckedChanged);
            // 
            // rbMod1
            // 
            this.rbMod1.Checked = true;
            this.rbMod1.Location = new System.Drawing.Point(12, 20);
            this.rbMod1.Name = "rbMod1";
            this.rbMod1.Size = new System.Drawing.Size(128, 20);
            this.rbMod1.TabIndex = 0;
            this.rbMod1.TabStop = true;
            this.rbMod1.Text = "1.0 (No modifier)";
            this.rbMod1.CheckedChanged += new System.EventHandler(this.rbMod_CheckedChanged);
            // 
            // lstSelected
            // 
            this.lstSelected.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstSelected.ItemHeight = 14;
            this.lstSelected.Location = new System.Drawing.Point(240, 20);
            this.lstSelected.Name = "lstSelected";
            this.lstSelected.Size = new System.Drawing.Size(176, 214);
            this.lstSelected.TabIndex = 16;
            this.lstSelected.SelectedIndexChanged += new System.EventHandler(this.lstSelected_SelectedIndexChanged);
            // 
            // lstAvailable
            // 
            this.lstAvailable.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstAvailable.ItemHeight = 14;
            this.lstAvailable.Location = new System.Drawing.Point(8, 20);
            this.lstAvailable.Name = "lstAvailable";
            this.lstAvailable.Size = new System.Drawing.Size(116, 172);
            this.lstAvailable.TabIndex = 15;
            this.lstAvailable.DoubleClick += new System.EventHandler(this.lstAvailable_DoubleClick);
            // 
            // cbSched
            // 
            this.cbSched.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSched.Location = new System.Drawing.Point(488, 20);
            this.cbSched.Name = "cbSched";
            this.cbSched.Size = new System.Drawing.Size(88, 22);
            this.cbSched.TabIndex = 14;
            this.cbSched.SelectedIndexChanged += new System.EventHandler(this.cbSched_SelectedIndexChanged);
            // 
            // lblSched
            // 
            this.lblSched.Location = new System.Drawing.Point(424, 20);
            this.lblSched.Name = "lblSched";
            this.lblSched.Size = new System.Drawing.Size(64, 20);
            this.lblSched.TabIndex = 3;
            this.lblSched.Text = "Schedule:";
            this.lblSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(594, 451);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(272, 28);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(594, 485);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(272, 28);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbClass
            // 
            this.gbClass.Controls.Add(this.lblClass);
            this.gbClass.Controls.Add(this.pnlClassList);
            this.gbClass.Controls.Add(this.pnlClass);
            this.gbClass.Location = new System.Drawing.Point(596, 178);
            this.gbClass.Name = "gbClass";
            this.gbClass.Size = new System.Drawing.Size(272, 268);
            this.gbClass.TabIndex = 18;
            this.gbClass.TabStop = false;
            this.gbClass.Text = "Class(es):";
            // 
            // lblClass
            // 
            this.lblClass.BackColor = System.Drawing.SystemColors.Control;
            this.lblClass.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClass.Location = new System.Drawing.Point(8, 248);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(68, 16);
            this.lblClass.TabIndex = 2;
            // 
            // pnlClassList
            // 
            this.pnlClassList.BackColor = System.Drawing.Color.Black;
            this.pnlClassList.Location = new System.Drawing.Point(11, 16);
            this.pnlClassList.Name = "pnlClassList";
            this.pnlClassList.Size = new System.Drawing.Size(180, 230);
            this.pnlClassList.TabIndex = 1;
            // 
            // pnlClass
            // 
            this.pnlClass.BackColor = System.Drawing.Color.Black;
            this.pnlClass.Location = new System.Drawing.Point(196, 16);
            this.pnlClass.Name = "pnlClass";
            this.pnlClass.Size = new System.Drawing.Size(68, 230);
            this.pnlClass.TabIndex = 0;
            // 
            // ImagePicker
            // 
            this.ImagePicker.Filter = "PNG Images|*.png";
            this.ImagePicker.Title = "Select Image File";
            // 
            // btnNoImage
            // 
            this.btnNoImage.Location = new System.Drawing.Point(8, 84);
            this.btnNoImage.Name = "btnNoImage";
            this.btnNoImage.Size = new System.Drawing.Size(80, 20);
            this.btnNoImage.TabIndex = 19;
            this.btnNoImage.Text = "Clear Image";
            this.btnNoImage.Click += new System.EventHandler(this.btnNoImage_Click);
            // 
            // tTip
            // 
            this.tTip.AutoPopDelay = 10000;
            this.tTip.InitialDelay = 250;
            this.tTip.ReshowDelay = 100;
            // 
            // cbMutEx
            // 
            this.cbMutEx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMutEx.Location = new System.Drawing.Point(10, 80);
            this.cbMutEx.Name = "cbMutEx";
            this.cbMutEx.Size = new System.Drawing.Size(354, 22);
            this.cbMutEx.TabIndex = 21;
            this.tTip.SetToolTip(this.cbMutEx, "(Currently only apllicable to Stealth IOs");
            this.cbMutEx.SelectedIndexChanged += new System.EventHandler(this.cbMutEx_SelectedIndexChanged);
            // 
            // cbRecipe
            // 
            this.cbRecipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRecipe.Location = new System.Drawing.Point(96, 183);
            this.cbRecipe.Name = "cbRecipe";
            this.cbRecipe.Size = new System.Drawing.Size(248, 22);
            this.cbRecipe.TabIndex = 23;
            this.tTip.SetToolTip(this.cbRecipe, "(Currently only apllicable to Stealth IOs");
            this.cbRecipe.SelectedIndexChanged += new System.EventHandler(this.cbRecipe_SelectedIndexChanged);
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(2, 64);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(80, 16);
            this.Label8.TabIndex = 22;
            this.Label8.Text = "MutEx Group:";
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(10, 183);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(80, 22);
            this.Label10.TabIndex = 24;
            this.Label10.Text = "Recipe:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnEditPowerData
            // 
            this.btnEditPowerData.Location = new System.Drawing.Point(352, 183);
            this.btnEditPowerData.Name = "btnEditPowerData";
            this.btnEditPowerData.Size = new System.Drawing.Size(236, 22);
            this.btnEditPowerData.TabIndex = 25;
            this.btnEditPowerData.Text = "Edit Power Data";
            this.btnEditPowerData.UseVisualStyleBackColor = true;
            this.btnEditPowerData.Click += new System.EventHandler(this.btnEditPowerData_Click);
            // 
            // StaticIndex
            // 
            this.StaticIndex.Location = new System.Drawing.Point(8, 146);
            this.StaticIndex.Name = "StaticIndex";
            this.StaticIndex.Size = new System.Drawing.Size(82, 20);
            this.StaticIndex.TabIndex = 26;
            this.StaticIndex.TextChanged += new System.EventHandler(this.StaticIndex_TextChanged);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(8, 126);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(63, 14);
            this.Label11.TabIndex = 27;
            this.Label11.Text = "Static Index";
            // 
            // gbFlags
            // 
            this.gbFlags.Controls.Add(this.chkScalable);
            this.gbFlags.Controls.Add(this.chkProc);
            this.gbFlags.Controls.Add(this.chkUnique);
            this.gbFlags.Controls.Add(this.chkSuperior);
            this.gbFlags.Controls.Add(this.cbMutEx);
            this.gbFlags.Controls.Add(this.Label8);
            this.gbFlags.Location = new System.Drawing.Point(496, 66);
            this.gbFlags.Name = "gbFlags";
            this.gbFlags.Size = new System.Drawing.Size(372, 111);
            this.gbFlags.TabIndex = 28;
            this.gbFlags.TabStop = false;
            this.gbFlags.Text = "Enhancement Flags";
            // 
            // chkScalable
            // 
            this.chkScalable.AutoSize = true;
            this.chkScalable.Location = new System.Drawing.Point(185, 39);
            this.chkScalable.Name = "chkScalable";
            this.chkScalable.Size = new System.Drawing.Size(92, 18);
            this.chkScalable.TabIndex = 23;
            this.chkScalable.Text = "Allow Scaling";
            this.chkScalable.UseVisualStyleBackColor = true;
            // 
            // chkProc
            // 
            this.chkProc.AutoSize = true;
            this.chkProc.Location = new System.Drawing.Point(185, 16);
            this.chkProc.Name = "chkProc";
            this.chkProc.Size = new System.Drawing.Size(48, 18);
            this.chkProc.TabIndex = 22;
            this.chkProc.Text = "Proc";
            this.chkProc.UseVisualStyleBackColor = true;
            this.chkProc.CheckedChanged += new System.EventHandler(this.chkProc_CheckedChanged);
            // 
            // frmEnhData
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(873, 525);
            this.Controls.Add(this.gbFlags);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.StaticIndex);
            this.Controls.Add(this.btnEditPowerData);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.cbRecipe);
            this.Controls.Add(this.btnNoImage);
            this.Controls.Add(this.gbClass);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbEffects);
            this.Controls.Add(this.gbSet);
            this.Controls.Add(this.gbBasic);
            this.Controls.Add(this.gbType);
            this.Controls.Add(this.btnImage);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEnhData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit [EnhancementName]";
            this.gbBasic.ResumeLayout(false);
            this.gbBasic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMinLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxLevel)).EndInit();
            this.gbType.ResumeLayout(false);
            this.gbSet.ResumeLayout(false);
            this.gbEffects.ResumeLayout(false);
            this.gbEffects.PerformLayout();
            this.gbMod.ResumeLayout(false);
            this.gbMod.PerformLayout();
            this.gbClass.ResumeLayout(false);
            this.gbFlags.ResumeLayout(false);
            this.gbFlags.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        Button btnAdd;
        Button btnAddFX;
        Button btnAutoFill;
        Button btnCancel;
        Button btnDown;
        Button btnEdit;
        Button btnEditPowerData;
        Button btnImage;
        Button btnNoImage;
        Button btnOK;
        Button btnRemove;
        Button btnUp;
        ComboBox cbMutEx;
        ComboBox cbRecipe;
        ComboBox cbSched;
        ComboBox cbSet;
        ComboBox cbSubType;
        CheckBox chkSuperior;
        CheckBox chkUnique;
        GroupBox gbBasic;
        GroupBox gbClass;
        GroupBox gbEffects;
        GroupBox gbMod;
        GroupBox gbSet;
        GroupBox gbType;
        OpenFileDialog ImagePicker;
        Label Label1;
        Label Label10;
        Label Label11;
        Label Label2;
        Label Label3;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblClass;
        Label lblSched;
        ListBox lstAvailable;
        ListBox lstSelected;
        Panel pnlClass;
        Panel pnlClassList;
        RadioButton rbBoth;
        RadioButton rbBuff;
        RadioButton rbDebuff;
        RadioButton rbMod1;
        RadioButton rbMod2;
        RadioButton rbMod3;
        RadioButton rbMod4;
        RadioButton rbModOther;
        TextBox StaticIndex;
        ToolTip tTip;
        TextBox txtDesc;
        TextBox txtInternal;
        TextBox txtModOther;
        TextBox txtNameFull;
        TextBox txtNameShort;
        TextBox txtProb;
        RadioButton typeHO;
        RadioButton typeIO;
        RadioButton typeRegular;
        RadioButton typeSet;
        NumericUpDown udMaxLevel;
        NumericUpDown udMinLevel;
        private GroupBox gbFlags;
        private CheckBox chkProc;
        private CheckBox chkScalable;
    }
}
using System.ComponentModel;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditArchetype
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

            this.txtName = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtHP = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtResCap = new System.Windows.Forms.TextBox();
            this.clbOrigin = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Label6 = new System.Windows.Forms.Label();
            this.txtHPCap = new System.Windows.Forms.TextBox();
            this.cbClassType = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtClassName = new System.Windows.Forms.TextBox();
            this.udColumn = new System.Windows.Forms.NumericUpDown();
            this.Label8 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.udThreat = new System.Windows.Forms.NumericUpDown();
            this.chkPlayable = new System.Windows.Forms.CheckBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRecCap = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.txtRegCap = new System.Windows.Forms.TextBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.txtPerceptionCap = new System.Windows.Forms.TextBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.txtBaseRegen = new System.Windows.Forms.TextBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.txtBaseRec = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.txtRechargeCap = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.txtDamCap = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.txtDescLong = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtDescShort = new System.Windows.Forms.TextBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.cbSecGroup = new System.Windows.Forms.ComboBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.cbPriGroup = new System.Windows.Forms.ComboBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.udColumn.BeginInit();
            this.GroupBox1.SuspendLayout();
            this.udThreat.BeginInit();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.SuspendLayout();

            this.txtName.Location = new System.Drawing.Point(123, 16);
            this.txtName.Name = "txtName";

            this.txtName.Size = new System.Drawing.Size(118, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(txtName_TextChanged);

            this.Label1.Location = new System.Drawing.Point(30, 16);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(87, 20);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Display Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label2.Location = new System.Drawing.Point(6, 19);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(121, 20);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "Hit Points:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtHP.Location = new System.Drawing.Point(133, 19);
            this.txtHP.Name = "txtHP";

            this.txtHP.Size = new System.Drawing.Size(108, 20);
            this.txtHP.TabIndex = 4;

            this.Label3.Location = new System.Drawing.Point(6, 123);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(121, 20);
            this.Label3.TabIndex = 7;
            this.Label3.Text = "Resistance Cap:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtResCap.Location = new System.Drawing.Point(133, 123);
            this.txtResCap.Name = "txtResCap";

            this.txtResCap.Size = new System.Drawing.Size(90, 20);
            this.txtResCap.TabIndex = 6;
            this.txtResCap.Text = "80";
            this.txtResCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.clbOrigin.Location = new System.Drawing.Point(6, 16);
            this.clbOrigin.Name = "clbOrigin";

            this.clbOrigin.Size = new System.Drawing.Size(235, 244);
            this.clbOrigin.TabIndex = 8;

            this.btnOK.Location = new System.Drawing.Point(437, 585);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(353, 585);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);

            this.Label6.Location = new System.Drawing.Point(6, 45);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(121, 20);
            this.Label6.TabIndex = 16;
            this.Label6.Text = "Hit Point Cap:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtHPCap.Location = new System.Drawing.Point(133, 45);
            this.txtHPCap.Name = "txtHPCap";

            this.txtHPCap.Size = new System.Drawing.Size(108, 20);
            this.txtHPCap.TabIndex = 15;
            this.cbClassType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbClassType.FormattingEnabled = true;

            this.cbClassType.Location = new System.Drawing.Point(123, 68);
            this.cbClassType.Name = "cbClassType";

            this.cbClassType.Size = new System.Drawing.Size(118, 21);
            this.cbClassType.TabIndex = 17;
            this.cbClassType.SelectedIndexChanged += new System.EventHandler(cbClassType_SelectedIndexChanged);

            this.Label7.Location = new System.Drawing.Point(30, 68);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(87, 21);
            this.Label7.TabIndex = 18;
            this.Label7.Text = "Class Type:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label5.Location = new System.Drawing.Point(30, 42);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(87, 20);
            this.Label5.TabIndex = 20;
            this.Label5.Text = "Class Name:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtClassName.Location = new System.Drawing.Point(123, 42);
            this.txtClassName.Name = "txtClassName";

            this.txtClassName.Size = new System.Drawing.Size(118, 20);
            this.txtClassName.TabIndex = 19;
            this.txtClassName.TextChanged += new System.EventHandler(txtClassName_TextChanged);

            this.udColumn.Location = new System.Drawing.Point(121, 95);
            this.udColumn.Name = "udColumn";

            this.udColumn.Size = new System.Drawing.Size(120, 20);
            this.udColumn.TabIndex = 21;
            this.udColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udColumn.Value = new System.Decimal(new int[4] { 2, 0, 0, 0 });

            this.Label8.Location = new System.Drawing.Point(30, 95);
            this.Label8.Name = "Label8";

            this.Label8.Size = new System.Drawing.Size(87, 20);
            this.Label8.TabIndex = 22;
            this.Label8.Text = "Modifier Column:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label18);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udThreat);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.chkPlayable);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label8);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtName);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udColumn);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbClassType);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label7);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtClassName);

            this.GroupBox1.Location = new System.Drawing.Point(12, 8);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(247, 151);
            this.GroupBox1.TabIndex = 23;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Basic";

            this.Label18.Location = new System.Drawing.Point(121, 122);
            this.Label18.Name = "Label18";

            this.Label18.Size = new System.Drawing.Size(62, 19);
            this.Label18.TabIndex = 25;
            this.Label18.Text = "Threat:";
            this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udThreat.Location = new System.Drawing.Point(189, 121);
            this.udThreat.Maximum = new System.Decimal(new int[4] { 10, 0, 0, 0 });
            this.udThreat.Name = "udThreat";

            this.udThreat.Size = new System.Drawing.Size(52, 20);
            this.udThreat.TabIndex = 24;
            this.udThreat.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udThreat.Value = new System.Decimal(new int[4] { 2, 0, 0, 0 });
            this.chkPlayable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.chkPlayable.Location = new System.Drawing.Point(13, 121);
            this.chkPlayable.Name = "chkPlayable";

            this.chkPlayable.Size = new System.Drawing.Size(85, 24);
            this.chkPlayable.TabIndex = 23;
            this.chkPlayable.Text = "Playable:";
            this.chkPlayable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkPlayable.UseVisualStyleBackColor = true;
            this.chkPlayable.CheckedChanged += new System.EventHandler(chkPlayable_CheckedChanged);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtRecCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label21);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label22);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtRegCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label23);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label24);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtPerceptionCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label20);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtBaseRegen);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label19);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtBaseRec);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label17);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtRechargeCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label12);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label13);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtDamCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label9);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtResCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtHP);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label6);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtHPCap);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label10);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label11);

            this.GroupBox2.Location = new System.Drawing.Point(265, 8);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(247, 352);
            this.GroupBox2.TabIndex = 24;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "HP && Caps";

            this.txtRecCap.Location = new System.Drawing.Point(133, 148);
            this.txtRecCap.Name = "txtRecCap";

            this.txtRecCap.Size = new System.Drawing.Size(90, 20);
            this.txtRecCap.TabIndex = 34;
            this.txtRecCap.Text = "500";
            this.txtRecCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label21.Location = new System.Drawing.Point(6, 148);
            this.Label21.Name = "Label21";

            this.Label21.Size = new System.Drawing.Size(121, 20);
            this.Label21.TabIndex = 35;
            this.Label21.Text = "Recovery Cap:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label22.Location = new System.Drawing.Point(221, 148);
            this.Label22.Name = "Label22";

            this.Label22.Size = new System.Drawing.Size(20, 20);
            this.Label22.TabIndex = 36;
            this.Label22.Text = "%";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.txtRegCap.Location = new System.Drawing.Point(133, 174);
            this.txtRegCap.Name = "txtRegCap";

            this.txtRegCap.Size = new System.Drawing.Size(90, 20);
            this.txtRegCap.TabIndex = 31;
            this.txtRegCap.Text = "2000";
            this.txtRegCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label23.Location = new System.Drawing.Point(6, 174);
            this.Label23.Name = "Label23";

            this.Label23.Size = new System.Drawing.Size(121, 20);
            this.Label23.TabIndex = 32;
            this.Label23.Text = "Regeneration Cap:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label24.Location = new System.Drawing.Point(221, 174);
            this.Label24.Name = "Label24";

            this.Label24.Size = new System.Drawing.Size(20, 20);
            this.Label24.TabIndex = 33;
            this.Label24.Text = "%";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.txtPerceptionCap.Location = new System.Drawing.Point(133, 252);
            this.txtPerceptionCap.Name = "txtPerceptionCap";

            this.txtPerceptionCap.Size = new System.Drawing.Size(90, 20);
            this.txtPerceptionCap.TabIndex = 29;
            this.txtPerceptionCap.Text = "100";
            this.txtPerceptionCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label20.Location = new System.Drawing.Point(6, 252);
            this.Label20.Name = "Label20";

            this.Label20.Size = new System.Drawing.Size(121, 20);
            this.Label20.TabIndex = 30;
            this.Label20.Text = "Perception Cap";
            this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtBaseRegen.Location = new System.Drawing.Point(133, 224);
            this.txtBaseRegen.Name = "txtBaseRegen";

            this.txtBaseRegen.Size = new System.Drawing.Size(90, 20);
            this.txtBaseRegen.TabIndex = 27;
            this.txtBaseRegen.Text = "100";
            this.txtBaseRegen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label19.Location = new System.Drawing.Point(6, 224);
            this.Label19.Name = "Label19";

            this.Label19.Size = new System.Drawing.Size(121, 20);
            this.Label19.TabIndex = 28;
            this.Label19.Text = "Base Regeneration:";
            this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtBaseRec.Location = new System.Drawing.Point(133, 198);
            this.txtBaseRec.Name = "txtBaseRec";

            this.txtBaseRec.Size = new System.Drawing.Size(90, 20);
            this.txtBaseRec.TabIndex = 24;
            this.txtBaseRec.Text = "1.67";
            this.txtBaseRec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label17.Location = new System.Drawing.Point(6, 198);
            this.Label17.Name = "Label17";

            this.Label17.Size = new System.Drawing.Size(121, 20);
            this.Label17.TabIndex = 25;
            this.Label17.Text = "Base Recovery:";
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtRechargeCap.Location = new System.Drawing.Point(133, 97);
            this.txtRechargeCap.Name = "txtRechargeCap";

            this.txtRechargeCap.Size = new System.Drawing.Size(90, 20);
            this.txtRechargeCap.TabIndex = 21;
            this.txtRechargeCap.Text = "600";
            this.txtRechargeCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label12.Location = new System.Drawing.Point(6, 97);
            this.Label12.Name = "Label12";

            this.Label12.Size = new System.Drawing.Size(121, 20);
            this.Label12.TabIndex = 22;
            this.Label12.Text = "Recharge Cap:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label13.Location = new System.Drawing.Point(221, 97);
            this.Label13.Name = "Label13";

            this.Label13.Size = new System.Drawing.Size(20, 20);
            this.Label13.TabIndex = 23;
            this.Label13.Text = "%";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.txtDamCap.Location = new System.Drawing.Point(133, 71);
            this.txtDamCap.Name = "txtDamCap";

            this.txtDamCap.Size = new System.Drawing.Size(90, 20);
            this.txtDamCap.TabIndex = 17;
            this.txtDamCap.Text = "400";
            this.txtDamCap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.Label9.Location = new System.Drawing.Point(6, 71);
            this.Label9.Name = "Label9";

            this.Label9.Size = new System.Drawing.Size(121, 20);
            this.Label9.TabIndex = 18;
            this.Label9.Text = "Damage Cap:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label10.Location = new System.Drawing.Point(221, 123);
            this.Label10.Name = "Label10";

            this.Label10.Size = new System.Drawing.Size(20, 20);
            this.Label10.TabIndex = 19;
            this.Label10.Text = "%";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.Label11.Location = new System.Drawing.Point(221, 71);
            this.Label11.Name = "Label11";

            this.Label11.Size = new System.Drawing.Size(20, 20);
            this.Label11.TabIndex = 20;
            this.Label11.Text = "%";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.clbOrigin);

            this.GroupBox3.Location = new System.Drawing.Point(12, 165);
            this.GroupBox3.Name = "GroupBox3";

            this.GroupBox3.Size = new System.Drawing.Size(247, 276);
            this.GroupBox3.TabIndex = 25;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Origins";
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label14);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.txtDescLong);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.txtDescShort);

            this.GroupBox4.Location = new System.Drawing.Point(12, 447);
            this.GroupBox4.Name = "GroupBox4";

            this.GroupBox4.Size = new System.Drawing.Size(500, 132);
            this.GroupBox4.TabIndex = 26;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Descriptions";

            this.Label14.Location = new System.Drawing.Point(12, 45);
            this.Label14.Name = "Label14";

            this.Label14.Size = new System.Drawing.Size(58, 20);
            this.Label14.TabIndex = 5;
            this.Label14.Text = "Long:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDescLong.Location = new System.Drawing.Point(76, 45);
            this.txtDescLong.Multiline = true;
            this.txtDescLong.Name = "txtDescLong";
            this.txtDescLong.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.txtDescLong.Size = new System.Drawing.Size(418, 81);
            this.txtDescLong.TabIndex = 4;
            this.txtDescLong.TextChanged += new System.EventHandler(txtDescLong_TextChanged);

            this.Label4.Location = new System.Drawing.Point(12, 19);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(58, 20);
            this.Label4.TabIndex = 3;
            this.Label4.Text = "Short:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDescShort.Location = new System.Drawing.Point(76, 19);
            this.txtDescShort.Name = "txtDescShort";

            this.txtDescShort.Size = new System.Drawing.Size(418, 20);
            this.txtDescShort.TabIndex = 2;
            this.txtDescShort.TextChanged += new System.EventHandler(txtDescShort_TextChanged);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.cbSecGroup);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.Label16);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.cbPriGroup);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.Label15);

            this.GroupBox5.Location = new System.Drawing.Point(265, 366);
            this.GroupBox5.Name = "GroupBox5";

            this.GroupBox5.Size = new System.Drawing.Size(247, 75);
            this.GroupBox5.TabIndex = 27;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Power Set Groups";
            this.cbSecGroup.FormattingEnabled = true;

            this.cbSecGroup.Location = new System.Drawing.Point(121, 46);
            this.cbSecGroup.Name = "cbSecGroup";

            this.cbSecGroup.Size = new System.Drawing.Size(118, 21);
            this.cbSecGroup.TabIndex = 21;

            this.Label16.Location = new System.Drawing.Point(5, 46);
            this.Label16.Name = "Label16";

            this.Label16.Size = new System.Drawing.Size(110, 21);
            this.Label16.TabIndex = 22;
            this.Label16.Text = "Secondary Group:";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbPriGroup.FormattingEnabled = true;

            this.cbPriGroup.Location = new System.Drawing.Point(121, 19);
            this.cbPriGroup.Name = "cbPriGroup";

            this.cbPriGroup.Size = new System.Drawing.Size(118, 21);
            this.cbPriGroup.TabIndex = 19;

            this.Label15.Location = new System.Drawing.Point(5, 19);
            this.Label15.Name = "Label15";

            this.Label15.Size = new System.Drawing.Size(110, 21);
            this.Label15.TabIndex = 20;
            this.Label15.Text = "Primary Group:";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AcceptButton = (System.Windows.Forms.IButtonControl)this.btnOK;

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = (System.Windows.Forms.IButtonControl)this.btnCancel;

            this.ClientSize = new System.Drawing.Size(522, 614);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox5);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox4);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox3);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Class (Class_Blaster - Blaster)";
            this.udColumn.EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.udThreat.EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.GroupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
    }


}
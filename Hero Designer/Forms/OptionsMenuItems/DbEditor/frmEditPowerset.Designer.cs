using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditPowerset
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
            this.cbAT = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.cbSetType = new System.Windows.Forms.ComboBox();
            this.btnIcon = new System.Windows.Forms.Button();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lvPowers = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.Label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnClearIcon = new System.Windows.Forms.Button();
            this.ImagePicker = new System.Windows.Forms.OpenFileDialog();
            this.lblNameUnique = new System.Windows.Forms.Label();
            this.lblNameFull = new System.Windows.Forms.Label();
            this.cbNameGroup = new System.Windows.Forms.ComboBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.txtNameSet = new System.Windows.Forms.TextBox();
            this.Label33 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.chkNoTrunk = new System.Windows.Forms.CheckBox();
            this.cbTrunkSet = new System.Windows.Forms.ComboBox();
            this.cbTrunkGroup = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label31 = new System.Windows.Forms.Label();
            this.gbLink = new System.Windows.Forms.GroupBox();
            this.chkNoLink = new System.Windows.Forms.CheckBox();
            this.cbLinkSet = new System.Windows.Forms.ComboBox();
            this.cbLinkGroup = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.lvMutexSets = new System.Windows.Forms.ListBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.cbMutexGroup = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)this.picIcon).BeginInit();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.gbLink.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.SuspendLayout();

            this.txtName.Location = new System.Drawing.Point(110, 16);
            this.txtName.Name = "txtName";

            this.txtName.Size = new System.Drawing.Size(196, 20);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "TextBox1";
            this.txtName.TextChanged += new System.EventHandler(txtName_TextChanged);

            this.Label1.Location = new System.Drawing.Point(6, 16);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(100, 20);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Display Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbAT.Location = new System.Drawing.Point(403, 122);
            this.cbAT.Name = "cbAT";

            this.cbAT.Size = new System.Drawing.Size(124, 22);
            this.cbAT.TabIndex = 2;
            this.cbAT.SelectedIndexChanged += new System.EventHandler(cbAT_SelectedIndexChanged);

            this.Label2.Location = new System.Drawing.Point(336, 122);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(63, 20);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Archetype:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label3.Location = new System.Drawing.Point(336, 150);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(63, 20);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Set Type:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbSetType.Location = new System.Drawing.Point(403, 150);
            this.cbSetType.Name = "cbSetType";

            this.cbSetType.Size = new System.Drawing.Size(124, 22);
            this.cbSetType.TabIndex = 4;
            this.cbSetType.SelectedIndexChanged += new System.EventHandler(cbSetType_SelectedIndexChanged);

            this.btnIcon.Location = new System.Drawing.Point(6, 52);
            this.btnIcon.Name = "btnIcon";

            this.btnIcon.Size = new System.Drawing.Size(179, 20);
            this.btnIcon.TabIndex = 6;
            this.btnIcon.Text = "Select Icon";
            this.btnIcon.Click += new System.EventHandler(btnIcon_Click);
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.picIcon.Location = new System.Drawing.Point(85, 22);
            this.picIcon.Name = "picIcon";

            this.picIcon.Size = new System.Drawing.Size(20, 20);
            this.picIcon.TabIndex = 7;
            this.picIcon.TabStop = false;
            this.lvPowers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3]
            {
        this.ColumnHeader3,
        this.ColumnHeader1,
        this.ColumnHeader2
            });
            this.lvPowers.FullRowSelect = true;
            this.lvPowers.HideSelection = false;

            this.lvPowers.Location = new System.Drawing.Point(12, 448);
            this.lvPowers.MultiSelect = false;
            this.lvPowers.Name = "lvPowers";

            this.lvPowers.Size = new System.Drawing.Size(515, 121);
            this.lvPowers.TabIndex = 8;
            this.lvPowers.UseCompatibleStateImageBehavior = false;
            this.lvPowers.View = System.Windows.Forms.View.Details;
            this.ColumnHeader3.Text = "Level";
            this.ColumnHeader3.Width = 47;
            this.ColumnHeader1.Text = "Power";
            this.ColumnHeader1.Width = 124;
            this.ColumnHeader2.Text = "Short Description";
            this.ColumnHeader2.Width = 313;

            this.Label4.Location = new System.Drawing.Point(9, 425);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(100, 20);
            this.Label4.TabIndex = 9;
            this.Label4.Text = "Powers:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnClose.Location = new System.Drawing.Point(452, 575);
            this.btnClose.Name = "btnClose";

            this.btnClose.Size = new System.Drawing.Size(75, 36);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "OK";
            this.btnClose.Click += new System.EventHandler(btnClose_Click);

            this.btnClearIcon.Location = new System.Drawing.Point(6, 76);
            this.btnClearIcon.Name = "btnClearIcon";

            this.btnClearIcon.Size = new System.Drawing.Size(179, 20);
            this.btnClearIcon.TabIndex = 16;
            this.btnClearIcon.Text = "Clear Icon";
            this.btnClearIcon.Click += new System.EventHandler(btnClearIcon_Click);
            this.ImagePicker.Filter = "PNG Images|*.png";
            this.ImagePicker.Title = "Select Image File";

            this.lblNameUnique.Location = new System.Drawing.Point(10, 131);
            this.lblNameUnique.Name = "lblNameUnique";

            this.lblNameUnique.Size = new System.Drawing.Size(296, 20);
            this.lblNameUnique.TabIndex = 25;
            this.lblNameUnique.Text = "This name is unique.";
            this.lblNameUnique.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNameFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblNameFull.Location = new System.Drawing.Point(13, 95);
            this.lblNameFull.Name = "lblNameFull";

            this.lblNameFull.Size = new System.Drawing.Size(293, 32);
            this.lblNameFull.TabIndex = 24;
            this.lblNameFull.Text = "Group_Name.Powerset_Name";
            this.lblNameFull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbNameGroup.FormattingEnabled = true;

            this.cbNameGroup.Location = new System.Drawing.Point(110, 44);
            this.cbNameGroup.Name = "cbNameGroup";

            this.cbNameGroup.Size = new System.Drawing.Size(196, 22);
            this.cbNameGroup.TabIndex = 20;
            this.cbNameGroup.TextChanged += this.cbNameGroup_TextChanged;
            this.cbNameGroup.SelectedIndexChanged += this.cbNameGroup_SelectedIndexChanged;
            this.cbNameGroup.Leave += this.cbNameGroup_Leave;

            this.Label22.Location = new System.Drawing.Point(10, 44);
            this.Label22.Name = "Label22";

            this.Label22.Size = new System.Drawing.Size(96, 20);
            this.Label22.TabIndex = 22;
            this.Label22.Text = "Group:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNameSet.Location = new System.Drawing.Point(110, 72);
            this.txtNameSet.Name = "txtNameSet";

            this.txtNameSet.Size = new System.Drawing.Size(196, 20);
            this.txtNameSet.TabIndex = 21;
            this.txtNameSet.Text = "PowerName";
            this.txtNameSet.TextChanged += this.txtNameSet_TextChanged;
            this.txtNameSet.Leave += this.txtNameSet_Leave;


            this.Label33.Location = new System.Drawing.Point(3, 72);
            this.Label33.Name = "Label33";

            this.Label33.Size = new System.Drawing.Size(103, 20);
            this.Label33.TabIndex = 23;
            this.Label33.Text = "Powerset Name:";
            this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblNameUnique);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblNameFull);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbNameGroup);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label22);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtNameSet);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label33);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtName);

            this.GroupBox1.Location = new System.Drawing.Point(12, 12);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(318, 160);
            this.GroupBox1.TabIndex = 26;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Powerset Name";
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnClearIcon);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.picIcon);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnIcon);

            this.GroupBox2.Location = new System.Drawing.Point(336, 12);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(191, 102);
            this.GroupBox2.TabIndex = 27;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Icon";
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnCancel.Location = new System.Drawing.Point(371, 575);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(75, 36);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.txtDesc);

            this.GroupBox3.Location = new System.Drawing.Point(12, 178);
            this.GroupBox3.Name = "GroupBox3";

            this.GroupBox3.Size = new System.Drawing.Size(515, 80);
            this.GroupBox3.TabIndex = 29;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Description";

            this.txtDesc.Location = new System.Drawing.Point(6, 19);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.txtDesc.Size = new System.Drawing.Size(503, 55);
            this.txtDesc.TabIndex = 1;
            this.txtDesc.Text = "TextBox1";
            this.txtDesc.TextChanged += new System.EventHandler(txtDesc_TextChanged);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkNoTrunk);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.cbTrunkSet);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.cbTrunkGroup);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label31);

            this.GroupBox4.Location = new System.Drawing.Point(12, 264);
            this.GroupBox4.Name = "GroupBox4";

            this.GroupBox4.Size = new System.Drawing.Size(515, 75);
            this.GroupBox4.TabIndex = 30;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Trunk Set:";

            this.chkNoTrunk.Location = new System.Drawing.Point(279, 16);
            this.chkNoTrunk.Name = "chkNoTrunk";

            this.chkNoTrunk.Size = new System.Drawing.Size(210, 50);
            this.chkNoTrunk.TabIndex = 17;
            this.chkNoTrunk.Text = "This power has no Trunk set";
            this.chkNoTrunk.UseVisualStyleBackColor = true;
            this.chkNoTrunk.CheckedChanged += new System.EventHandler(chkNoTrunk_CheckedChanged);
            this.cbTrunkSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrunkSet.FormattingEnabled = true;

            this.cbTrunkSet.Location = new System.Drawing.Point(68, 44);
            this.cbTrunkSet.Name = "cbTrunkSet";

            this.cbTrunkSet.Size = new System.Drawing.Size(196, 22);
            this.cbTrunkSet.TabIndex = 14;
            this.cbTrunkSet.SelectedIndexChanged += new System.EventHandler(cbTrunkSet_SelectedIndexChanged);
            this.cbTrunkGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrunkGroup.FormattingEnabled = true;

            this.cbTrunkGroup.Location = new System.Drawing.Point(68, 16);
            this.cbTrunkGroup.Name = "cbTrunkGroup";

            this.cbTrunkGroup.Size = new System.Drawing.Size(196, 22);
            this.cbTrunkGroup.TabIndex = 13;
            this.cbTrunkGroup.SelectedIndexChanged += new System.EventHandler(cbTrunkGroup_SelectedIndexChanged);

            this.Label5.Location = new System.Drawing.Point(10, 16);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(54, 20);
            this.Label5.TabIndex = 15;
            this.Label5.Text = "Group:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label31.Location = new System.Drawing.Point(13, 44);
            this.Label31.Name = "Label31";

            this.Label31.Size = new System.Drawing.Size(49, 20);
            this.Label31.TabIndex = 16;
            this.Label31.Text = "Set:";
            this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.gbLink.Controls.Add((System.Windows.Forms.Control)this.chkNoLink);
            this.gbLink.Controls.Add((System.Windows.Forms.Control)this.cbLinkSet);
            this.gbLink.Controls.Add((System.Windows.Forms.Control)this.cbLinkGroup);
            this.gbLink.Controls.Add((System.Windows.Forms.Control)this.Label6);
            this.gbLink.Controls.Add((System.Windows.Forms.Control)this.Label7);

            this.gbLink.Location = new System.Drawing.Point(12, 345);
            this.gbLink.Name = "gbLink";

            this.gbLink.Size = new System.Drawing.Size(515, 75);
            this.gbLink.TabIndex = 31;
            this.gbLink.TabStop = false;
            this.gbLink.Text = "Linked Secondary";

            this.chkNoLink.Location = new System.Drawing.Point(279, 16);
            this.chkNoLink.Name = "chkNoLink";

            this.chkNoLink.Size = new System.Drawing.Size(210, 50);
            this.chkNoLink.TabIndex = 17;
            this.chkNoLink.Text = "No link";
            this.chkNoLink.UseVisualStyleBackColor = true;
            this.chkNoLink.CheckedChanged += new System.EventHandler(chkNoLink_CheckedChanged);
            this.cbLinkSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLinkSet.FormattingEnabled = true;

            this.cbLinkSet.Location = new System.Drawing.Point(68, 44);
            this.cbLinkSet.Name = "cbLinkSet";

            this.cbLinkSet.Size = new System.Drawing.Size(196, 22);
            this.cbLinkSet.TabIndex = 14;
            this.cbLinkSet.SelectedIndexChanged += new System.EventHandler(cbLinkSet_SelectedIndexChanged);
            this.cbLinkGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLinkGroup.FormattingEnabled = true;

            this.cbLinkGroup.Location = new System.Drawing.Point(68, 16);
            this.cbLinkGroup.Name = "cbLinkGroup";

            this.cbLinkGroup.Size = new System.Drawing.Size(196, 22);
            this.cbLinkGroup.TabIndex = 13;
            this.cbLinkGroup.SelectedIndexChanged += new System.EventHandler(cbLinkGroup_SelectedIndexChanged);

            this.Label6.Location = new System.Drawing.Point(10, 16);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(54, 20);
            this.Label6.TabIndex = 15;
            this.Label6.Text = "Group:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label7.Location = new System.Drawing.Point(13, 44);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(49, 20);
            this.Label7.TabIndex = 16;
            this.Label7.Text = "Set:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.lvMutexSets);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.Label8);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.cbMutexGroup);

            this.GroupBox5.Location = new System.Drawing.Point(533, 12);
            this.GroupBox5.Name = "GroupBox5";

            this.GroupBox5.Size = new System.Drawing.Size(253, 327);
            this.GroupBox5.TabIndex = 32;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Mutually Exclusive Sets";
            this.lvMutexSets.ItemHeight = 14;

            this.lvMutexSets.Location = new System.Drawing.Point(9, 72);
            this.lvMutexSets.Name = "lvMutexSets";
            this.lvMutexSets.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;

            this.lvMutexSets.Size = new System.Drawing.Size(238, 242);
            this.lvMutexSets.TabIndex = 111;
            this.lvMutexSets.SelectedIndexChanged += new System.EventHandler(lvMutexSets_SelectedIndexChanged);

            this.Label8.Location = new System.Drawing.Point(6, 16);
            this.Label8.Name = "Label8";

            this.Label8.Size = new System.Drawing.Size(100, 20);
            this.Label8.TabIndex = 22;
            this.Label8.Text = "Group (Only one):";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbMutexGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMutexGroup.FormattingEnabled = true;

            this.cbMutexGroup.Location = new System.Drawing.Point(9, 44);
            this.cbMutexGroup.Name = "cbMutexGroup";

            this.cbMutexGroup.Size = new System.Drawing.Size(238, 22);
            this.cbMutexGroup.TabIndex = 21;
            this.cbMutexGroup.SelectionChangeCommitted += new System.EventHandler(cbMutexGroup_SelectionChangeCommitted);

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);

            this.ClientSize = new System.Drawing.Size(798, 621);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox5);
            this.Controls.Add((System.Windows.Forms.Control)this.gbLink);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox4);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox3);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.btnClose);
            this.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.Controls.Add((System.Windows.Forms.Control)this.lvPowers);
            this.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.Controls.Add((System.Windows.Forms.Control)this.cbSetType);
            this.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.Controls.Add((System.Windows.Forms.Control)this.cbAT);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Powerset (Group_Name.Set_Name)";
            ((System.ComponentModel.ISupportInitialize)this.picIcon).EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.gbLink.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        Button btnCancel;
        Button btnClearIcon;
        Button btnClose;
        Button btnIcon;
        ComboBox cbAT;
        ComboBox cbLinkGroup;
        ComboBox cbLinkSet;
        ComboBox cbMutexGroup;
        ComboBox cbNameGroup;
        ComboBox cbSetType;
        ComboBox cbTrunkGroup;
        ComboBox cbTrunkSet;
        CheckBox chkNoLink;
        CheckBox chkNoTrunk;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        GroupBox gbLink;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        GroupBox GroupBox4;
        GroupBox GroupBox5;
        OpenFileDialog ImagePicker;
        Label Label1;
        Label Label2;
        Label Label22;
        Label Label3;
        Label Label31;
        Label Label33;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label lblNameFull;
        Label lblNameUnique;
        ListBox lvMutexSets;
        ListView lvPowers;
        PictureBox picIcon;
        TextBox txtDesc;
        TextBox txtName;
        TextBox txtNameSet;
    }
}
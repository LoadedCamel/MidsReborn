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
            txtName = new TextBox();
            Label1 = new Label();
            cbAT = new ComboBox();
            Label2 = new Label();
            Label3 = new Label();
            cbSetType = new ComboBox();
            btnIcon = new Button();
            picIcon = new PictureBox();
            lvPowers = new ListView();
            ColumnHeader3 = new ColumnHeader();
            ColumnHeader1 = new ColumnHeader();
            ColumnHeader2 = new ColumnHeader();
            Label4 = new Label();
            btnClose = new Button();
            btnClearIcon = new Button();
            ImagePicker = new OpenFileDialog();
            lblNameUnique = new Label();
            lblNameFull = new Label();
            cbNameGroup = new ComboBox();
            Label22 = new Label();
            txtNameSet = new TextBox();
            Label33 = new Label();
            GroupBox1 = new GroupBox();
            GroupBox2 = new GroupBox();
            btnCancel = new Button();
            GroupBox3 = new GroupBox();
            txtDesc = new TextBox();
            GroupBox4 = new GroupBox();
            chkNoTrunk = new CheckBox();
            cbTrunkSet = new ComboBox();
            cbTrunkGroup = new ComboBox();
            Label5 = new Label();
            Label31 = new Label();
            gbLink = new GroupBox();
            chkNoLink = new CheckBox();
            cbLinkSet = new ComboBox();
            cbLinkGroup = new ComboBox();
            Label6 = new Label();
            Label7 = new Label();
            GroupBox5 = new GroupBox();
            label10 = new Label();
            lbAssignedSets = new ListBox();
            label9 = new Label();
            label8 = new Label();
            lbAvailbleSets = new ListBox();
            ((ISupportInitialize)picIcon).BeginInit();
            GroupBox1.SuspendLayout();
            GroupBox2.SuspendLayout();
            GroupBox3.SuspendLayout();
            GroupBox4.SuspendLayout();
            gbLink.SuspendLayout();
            GroupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(110, 18);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(196, 22);
            txtName.TabIndex = 0;
            txtName.Text = "TextBox1";
            txtName.TextChanged += txtName_TextChanged;
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(6, 18);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(100, 24);
            Label1.TabIndex = 1;
            Label1.Text = "Display Name:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbAT
            // 
            cbAT.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAT.Location = new System.Drawing.Point(403, 141);
            cbAT.Name = "cbAT";
            cbAT.Size = new System.Drawing.Size(124, 21);
            cbAT.TabIndex = 2;
            cbAT.SelectedIndexChanged += cbAT_SelectedIndexChanged;
            // 
            // Label2
            // 
            Label2.Location = new System.Drawing.Point(336, 141);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(63, 23);
            Label2.TabIndex = 3;
            Label2.Text = "Archetype:";
            Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            Label3.Location = new System.Drawing.Point(336, 173);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(63, 23);
            Label3.TabIndex = 5;
            Label3.Text = "Set Type:";
            Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSetType
            // 
            cbSetType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSetType.Location = new System.Drawing.Point(403, 173);
            cbSetType.Name = "cbSetType";
            cbSetType.Size = new System.Drawing.Size(124, 21);
            cbSetType.TabIndex = 4;
            cbSetType.SelectedIndexChanged += cbSetType_SelectedIndexChanged;
            // 
            // btnIcon
            // 
            btnIcon.Location = new System.Drawing.Point(6, 60);
            btnIcon.Name = "btnIcon";
            btnIcon.Size = new System.Drawing.Size(179, 23);
            btnIcon.TabIndex = 6;
            btnIcon.Text = "Select Icon";
            btnIcon.Click += btnIcon_Click;
            // 
            // picIcon
            // 
            picIcon.BorderStyle = BorderStyle.FixedSingle;
            picIcon.Location = new System.Drawing.Point(85, 25);
            picIcon.Name = "picIcon";
            picIcon.Size = new System.Drawing.Size(20, 23);
            picIcon.TabIndex = 7;
            picIcon.TabStop = false;
            // 
            // lvPowers
            // 
            lvPowers.Columns.AddRange(new ColumnHeader[] { ColumnHeader3, ColumnHeader1, ColumnHeader2 });
            lvPowers.FullRowSelect = true;
            lvPowers.Location = new System.Drawing.Point(12, 517);
            lvPowers.MultiSelect = false;
            lvPowers.Name = "lvPowers";
            lvPowers.Size = new System.Drawing.Size(515, 140);
            lvPowers.TabIndex = 8;
            lvPowers.UseCompatibleStateImageBehavior = false;
            lvPowers.View = View.Details;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Level";
            ColumnHeader3.Width = 47;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Power";
            ColumnHeader1.Width = 124;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Short Description";
            ColumnHeader2.Width = 313;
            // 
            // Label4
            // 
            Label4.Location = new System.Drawing.Point(9, 490);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(100, 23);
            Label4.TabIndex = 9;
            Label4.Text = "Powers:";
            Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.OK;
            btnClose.Location = new System.Drawing.Point(452, 663);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 42);
            btnClose.TabIndex = 15;
            btnClose.Text = "OK";
            btnClose.Click += btnClose_Click;
            // 
            // btnClearIcon
            // 
            btnClearIcon.Location = new System.Drawing.Point(6, 88);
            btnClearIcon.Name = "btnClearIcon";
            btnClearIcon.Size = new System.Drawing.Size(179, 23);
            btnClearIcon.TabIndex = 16;
            btnClearIcon.Text = "Clear Icon";
            btnClearIcon.Click += btnClearIcon_Click;
            // 
            // ImagePicker
            // 
            ImagePicker.Filter = "PNG Images|*.png";
            ImagePicker.Title = "Select Image File";
            // 
            // lblNameUnique
            // 
            lblNameUnique.Location = new System.Drawing.Point(10, 151);
            lblNameUnique.Name = "lblNameUnique";
            lblNameUnique.Size = new System.Drawing.Size(296, 23);
            lblNameUnique.TabIndex = 25;
            lblNameUnique.Text = "This name is unique.";
            lblNameUnique.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNameFull
            // 
            lblNameFull.BorderStyle = BorderStyle.FixedSingle;
            lblNameFull.Location = new System.Drawing.Point(13, 110);
            lblNameFull.Name = "lblNameFull";
            lblNameFull.Size = new System.Drawing.Size(293, 37);
            lblNameFull.TabIndex = 24;
            lblNameFull.Text = "Group_Name.Powerset_Name";
            lblNameFull.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbNameGroup
            // 
            cbNameGroup.FormattingEnabled = true;
            cbNameGroup.Location = new System.Drawing.Point(110, 51);
            cbNameGroup.Name = "cbNameGroup";
            cbNameGroup.Size = new System.Drawing.Size(196, 21);
            cbNameGroup.TabIndex = 20;
            cbNameGroup.SelectedIndexChanged += cbNameGroup_SelectedIndexChanged;
            cbNameGroup.TextChanged += cbNameGroup_TextChanged;
            cbNameGroup.Leave += cbNameGroup_Leave;
            // 
            // Label22
            // 
            Label22.Location = new System.Drawing.Point(10, 51);
            Label22.Name = "Label22";
            Label22.Size = new System.Drawing.Size(96, 23);
            Label22.TabIndex = 22;
            Label22.Text = "Group:";
            Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameSet
            // 
            txtNameSet.Location = new System.Drawing.Point(110, 83);
            txtNameSet.Name = "txtNameSet";
            txtNameSet.Size = new System.Drawing.Size(196, 22);
            txtNameSet.TabIndex = 21;
            txtNameSet.Text = "PowerName";
            txtNameSet.TextChanged += txtNameSet_TextChanged;
            txtNameSet.Leave += txtNameSet_Leave;
            // 
            // Label33
            // 
            Label33.Location = new System.Drawing.Point(3, 83);
            Label33.Name = "Label33";
            Label33.Size = new System.Drawing.Size(103, 23);
            Label33.TabIndex = 23;
            Label33.Text = "Powerset Name:";
            Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(lblNameUnique);
            GroupBox1.Controls.Add(lblNameFull);
            GroupBox1.Controls.Add(cbNameGroup);
            GroupBox1.Controls.Add(Label22);
            GroupBox1.Controls.Add(txtNameSet);
            GroupBox1.Controls.Add(Label33);
            GroupBox1.Controls.Add(Label1);
            GroupBox1.Controls.Add(txtName);
            GroupBox1.Location = new System.Drawing.Point(12, 14);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(318, 184);
            GroupBox1.TabIndex = 26;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Powerset Name";
            // 
            // GroupBox2
            // 
            GroupBox2.Controls.Add(btnClearIcon);
            GroupBox2.Controls.Add(picIcon);
            GroupBox2.Controls.Add(btnIcon);
            GroupBox2.Location = new System.Drawing.Point(336, 14);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(191, 118);
            GroupBox2.TabIndex = 27;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Icon";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.OK;
            btnCancel.Location = new System.Drawing.Point(371, 663);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 42);
            btnCancel.TabIndex = 28;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // GroupBox3
            // 
            GroupBox3.Controls.Add(txtDesc);
            GroupBox3.Location = new System.Drawing.Point(12, 205);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new System.Drawing.Size(515, 93);
            GroupBox3.TabIndex = 29;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "Description";
            // 
            // txtDesc
            // 
            txtDesc.Location = new System.Drawing.Point(6, 22);
            txtDesc.Multiline = true;
            txtDesc.Name = "txtDesc";
            txtDesc.ScrollBars = ScrollBars.Vertical;
            txtDesc.Size = new System.Drawing.Size(503, 63);
            txtDesc.TabIndex = 1;
            txtDesc.Text = "TextBox1";
            txtDesc.TextChanged += txtDesc_TextChanged;
            // 
            // GroupBox4
            // 
            GroupBox4.Controls.Add(chkNoTrunk);
            GroupBox4.Controls.Add(cbTrunkSet);
            GroupBox4.Controls.Add(cbTrunkGroup);
            GroupBox4.Controls.Add(Label5);
            GroupBox4.Controls.Add(Label31);
            GroupBox4.Location = new System.Drawing.Point(12, 305);
            GroupBox4.Name = "GroupBox4";
            GroupBox4.Size = new System.Drawing.Size(515, 86);
            GroupBox4.TabIndex = 30;
            GroupBox4.TabStop = false;
            GroupBox4.Text = "Trunk Set:";
            // 
            // chkNoTrunk
            // 
            chkNoTrunk.Location = new System.Drawing.Point(279, 18);
            chkNoTrunk.Name = "chkNoTrunk";
            chkNoTrunk.Size = new System.Drawing.Size(210, 58);
            chkNoTrunk.TabIndex = 17;
            chkNoTrunk.Text = "This power has no Trunk set";
            chkNoTrunk.UseVisualStyleBackColor = true;
            chkNoTrunk.CheckedChanged += chkNoTrunk_CheckedChanged;
            // 
            // cbTrunkSet
            // 
            cbTrunkSet.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTrunkSet.FormattingEnabled = true;
            cbTrunkSet.Location = new System.Drawing.Point(68, 51);
            cbTrunkSet.Name = "cbTrunkSet";
            cbTrunkSet.Size = new System.Drawing.Size(196, 21);
            cbTrunkSet.TabIndex = 14;
            cbTrunkSet.SelectedIndexChanged += cbTrunkSet_SelectedIndexChanged;
            // 
            // cbTrunkGroup
            // 
            cbTrunkGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTrunkGroup.FormattingEnabled = true;
            cbTrunkGroup.Location = new System.Drawing.Point(68, 18);
            cbTrunkGroup.Name = "cbTrunkGroup";
            cbTrunkGroup.Size = new System.Drawing.Size(196, 21);
            cbTrunkGroup.TabIndex = 13;
            cbTrunkGroup.SelectedIndexChanged += cbTrunkGroup_SelectedIndexChanged;
            // 
            // Label5
            // 
            Label5.Location = new System.Drawing.Point(10, 18);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(54, 24);
            Label5.TabIndex = 15;
            Label5.Text = "Group:";
            Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label31
            // 
            Label31.Location = new System.Drawing.Point(13, 51);
            Label31.Name = "Label31";
            Label31.Size = new System.Drawing.Size(49, 23);
            Label31.TabIndex = 16;
            Label31.Text = "Set:";
            Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbLink
            // 
            gbLink.Controls.Add(chkNoLink);
            gbLink.Controls.Add(cbLinkSet);
            gbLink.Controls.Add(cbLinkGroup);
            gbLink.Controls.Add(Label6);
            gbLink.Controls.Add(Label7);
            gbLink.Location = new System.Drawing.Point(12, 398);
            gbLink.Name = "gbLink";
            gbLink.Size = new System.Drawing.Size(515, 87);
            gbLink.TabIndex = 31;
            gbLink.TabStop = false;
            gbLink.Text = "Linked Secondary";
            // 
            // chkNoLink
            // 
            chkNoLink.Location = new System.Drawing.Point(279, 18);
            chkNoLink.Name = "chkNoLink";
            chkNoLink.Size = new System.Drawing.Size(210, 58);
            chkNoLink.TabIndex = 17;
            chkNoLink.Text = "No link";
            chkNoLink.UseVisualStyleBackColor = true;
            chkNoLink.CheckedChanged += chkNoLink_CheckedChanged;
            // 
            // cbLinkSet
            // 
            cbLinkSet.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLinkSet.FormattingEnabled = true;
            cbLinkSet.Location = new System.Drawing.Point(68, 51);
            cbLinkSet.Name = "cbLinkSet";
            cbLinkSet.Size = new System.Drawing.Size(196, 21);
            cbLinkSet.TabIndex = 14;
            cbLinkSet.SelectedIndexChanged += cbLinkSet_SelectedIndexChanged;
            // 
            // cbLinkGroup
            // 
            cbLinkGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLinkGroup.FormattingEnabled = true;
            cbLinkGroup.Location = new System.Drawing.Point(68, 18);
            cbLinkGroup.Name = "cbLinkGroup";
            cbLinkGroup.Size = new System.Drawing.Size(196, 21);
            cbLinkGroup.TabIndex = 13;
            cbLinkGroup.SelectedIndexChanged += cbLinkGroup_SelectedIndexChanged;
            // 
            // Label6
            // 
            Label6.Location = new System.Drawing.Point(10, 18);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(54, 24);
            Label6.TabIndex = 15;
            Label6.Text = "Group:";
            Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label7
            // 
            Label7.Location = new System.Drawing.Point(13, 51);
            Label7.Name = "Label7";
            Label7.Size = new System.Drawing.Size(49, 23);
            Label7.TabIndex = 16;
            Label7.Text = "Set:";
            Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox5
            // 
            GroupBox5.Controls.Add(label10);
            GroupBox5.Controls.Add(lbAssignedSets);
            GroupBox5.Controls.Add(label9);
            GroupBox5.Controls.Add(label8);
            GroupBox5.Controls.Add(lbAvailbleSets);
            GroupBox5.Location = new System.Drawing.Point(533, 14);
            GroupBox5.Name = "GroupBox5";
            GroupBox5.Size = new System.Drawing.Size(253, 643);
            GroupBox5.TabIndex = 32;
            GroupBox5.TabStop = false;
            GroupBox5.Text = "Mutual Exclusivity";
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(10, 25);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(237, 47);
            label10.TabIndex = 124;
            label10.Text = "To assign or remove a Powerset simply double-click the set in the corresponding list.";
            // 
            // lbAssignedSets
            // 
            lbAssignedSets.BackColor = System.Drawing.SystemColors.Info;
            lbAssignedSets.ForeColor = System.Drawing.Color.Black;
            lbAssignedSets.FormattingEnabled = true;
            lbAssignedSets.Location = new System.Drawing.Point(9, 380);
            lbAssignedSets.Name = "lbAssignedSets";
            lbAssignedSets.Size = new System.Drawing.Size(238, 238);
            lbAssignedSets.TabIndex = 123;
            lbAssignedSets.DoubleClick += lbAssignedSets_DoubleClick;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(10, 362);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(86, 15);
            label9.TabIndex = 122;
            label9.Text = "Sets Assigned:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(10, 83);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(87, 15);
            label8.TabIndex = 121;
            label8.Text = "Sets Available:";
            // 
            // lbAvailbleSets
            // 
            lbAvailbleSets.FormattingEnabled = true;
            lbAvailbleSets.Location = new System.Drawing.Point(9, 101);
            lbAvailbleSets.Name = "lbAvailbleSets";
            lbAvailbleSets.Size = new System.Drawing.Size(238, 238);
            lbAvailbleSets.TabIndex = 117;
            lbAvailbleSets.DoubleClick += lbAvailableSets_DoubleClick;
            // 
            // frmEditPowerset
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            ClientSize = new System.Drawing.Size(798, 720);
            Controls.Add(GroupBox5);
            Controls.Add(gbLink);
            Controls.Add(GroupBox4);
            Controls.Add(GroupBox3);
            Controls.Add(btnCancel);
            Controls.Add(GroupBox2);
            Controls.Add(GroupBox1);
            Controls.Add(btnClose);
            Controls.Add(Label4);
            Controls.Add(lvPowers);
            Controls.Add(Label3);
            Controls.Add(cbSetType);
            Controls.Add(Label2);
            Controls.Add(cbAT);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmEditPowerset";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Powerset (Group_Name.Set_Name)";
            ((ISupportInitialize)picIcon).EndInit();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            GroupBox2.ResumeLayout(false);
            GroupBox3.ResumeLayout(false);
            GroupBox3.PerformLayout();
            GroupBox4.ResumeLayout(false);
            gbLink.ResumeLayout(false);
            GroupBox5.ResumeLayout(false);
            GroupBox5.PerformLayout();
            ResumeLayout(false);
        }
        #endregion
        Button btnCancel;
        Button btnClearIcon;
        Button btnClose;
        Button btnIcon;
        ComboBox cbAT;
        ComboBox cbLinkGroup;
        ComboBox cbLinkSet;
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
        Label lblNameFull;
        Label lblNameUnique;
        ListView lvPowers;
        PictureBox picIcon;
        TextBox txtDesc;
        TextBox txtName;
        TextBox txtNameSet;
        private ListBox lbAvailbleSets;
        private ComboBox cbMutexSetType;
        private Button mutExlRem;
        private Button mutExlAdd;
        private Label label9;
        private Label label8;
        private Label label10;
        private ListBox lbAssignedSets;
    }
}
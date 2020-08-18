using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEntityEdit
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

            this.txtEntName = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.cbEntType = new System.Windows.Forms.ComboBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPDelete = new System.Windows.Forms.Button();
            this.lvPower = new System.Windows.Forms.ListView();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.btnPAdd = new System.Windows.Forms.Button();
            this.lvPSSet = new System.Windows.Forms.ListView();
            this.ColumnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.btnPDown = new System.Windows.Forms.Button();
            this.lvPSGroup = new System.Windows.Forms.ListView();
            this.ColumnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.btnPUp = new System.Windows.Forms.Button();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUGDelete = new System.Windows.Forms.Button();
            this.btnUGAdd = new System.Windows.Forms.Button();
            this.btnUGDown = new System.Windows.Forms.Button();
            this.btnUGUp = new System.Windows.Forms.Button();
            this.lvUpgrade = new System.Windows.Forms.ListView();
            this.ColumnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.lvUGPower = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lvUGSet = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvUGGroup = new System.Windows.Forms.ListView();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.cbClass = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();

            this.txtEntName.Location = new System.Drawing.Point(101, 9);
            this.txtEntName.Name = "txtEntName";

            this.txtEntName.Size = new System.Drawing.Size(172, 20);
            this.txtEntName.TabIndex = 0;
            this.txtEntName.TextChanged += new System.EventHandler(txtEntName_TextChanged);

            this.Label1.Location = new System.Drawing.Point(12, 9);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(83, 20);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label2.Location = new System.Drawing.Point(12, 35);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(83, 20);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Display Name:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDisplayName.Location = new System.Drawing.Point(101, 35);
            this.txtDisplayName.Name = "txtDisplayName";

            this.txtDisplayName.Size = new System.Drawing.Size(172, 20);
            this.txtDisplayName.TabIndex = 2;
            this.txtDisplayName.TextChanged += new System.EventHandler(txtDisplayName_TextChanged);
            this.cbEntType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEntType.FormattingEnabled = true;

            this.cbEntType.Location = new System.Drawing.Point(101, 61);
            this.cbEntType.Name = "cbEntType";

            this.cbEntType.Size = new System.Drawing.Size(172, 22);
            this.cbEntType.TabIndex = 4;
            this.cbEntType.SelectedIndexChanged += new System.EventHandler(cbEntType_SelectedIndexChanged);

            this.Label3.Location = new System.Drawing.Point(12, 61);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(83, 20);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Entity Type:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnPDelete);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lvPower);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnPAdd);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lvPSSet);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnPDown);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lvPSGroup);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnPUp);

            this.GroupBox1.Location = new System.Drawing.Point(12, 117);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(373, 375);
            this.GroupBox1.TabIndex = 6;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Power Sets";

            this.btnPDelete.Location = new System.Drawing.Point(283, 134);
            this.btnPDelete.Name = "btnPDelete";

            this.btnPDelete.Size = new System.Drawing.Size(84, 23);
            this.btnPDelete.TabIndex = 11;
            this.btnPDelete.Text = "Remove";
            this.btnPDelete.UseVisualStyleBackColor = true;
            this.btnPDelete.Click += new System.EventHandler(btnPDelete_Click);
            this.lvPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader4
            });
            this.lvPower.FullRowSelect = true;
            this.lvPower.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPower.HideSelection = false;

            this.lvPower.Location = new System.Drawing.Point(6, 19);
            this.lvPower.MultiSelect = false;
            this.lvPower.Name = "lvPower";

            this.lvPower.Size = new System.Drawing.Size(271, 138);
            this.lvPower.TabIndex = 18;
            this.lvPower.UseCompatibleStateImageBehavior = false;
            this.lvPower.View = System.Windows.Forms.View.Details;
            this.lvPower.SelectedIndexChanged += new System.EventHandler(lvPower_SelectedIndexChanged);
            this.ColumnHeader4.Text = "Current Sets";
            this.ColumnHeader4.Width = 244;

            this.btnPAdd.Location = new System.Drawing.Point(283, 105);
            this.btnPAdd.Name = "btnPAdd";

            this.btnPAdd.Size = new System.Drawing.Size(84, 23);
            this.btnPAdd.TabIndex = 10;
            this.btnPAdd.Text = "Add";
            this.btnPAdd.UseVisualStyleBackColor = true;
            this.btnPAdd.Click += new System.EventHandler(btnPAdd_Click);
            this.lvPSSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader10
            });
            this.lvPSSet.FullRowSelect = true;
            this.lvPSSet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPSSet.HideSelection = false;

            this.lvPSSet.Location = new System.Drawing.Point(148, 163);
            this.lvPSSet.MultiSelect = false;
            this.lvPSSet.Name = "lvPSSet";

            this.lvPSSet.Size = new System.Drawing.Size(219, 206);
            this.lvPSSet.TabIndex = 17;
            this.lvPSSet.UseCompatibleStateImageBehavior = false;
            this.lvPSSet.View = System.Windows.Forms.View.Details;
            this.lvPSSet.SelectedIndexChanged += this.lvPSSet_SelectedIndexChanged;
            this.lvPSSet.Click += new System.EventHandler(this.lvPSSet_Click);
            this.ColumnHeader10.Text = "Set";
            this.ColumnHeader10.Width = 188;

            this.btnPDown.Location = new System.Drawing.Point(283, 48);
            this.btnPDown.Name = "btnPDown";

            this.btnPDown.Size = new System.Drawing.Size(84, 23);
            this.btnPDown.TabIndex = 9;
            this.btnPDown.Text = "Down";
            this.btnPDown.UseVisualStyleBackColor = true;
            this.btnPDown.Click += new System.EventHandler(btnPDown_Click);
            this.lvPSGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader11
            });
            this.lvPSGroup.FullRowSelect = true;
            this.lvPSGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPSGroup.HideSelection = false;

            this.lvPSGroup.Location = new System.Drawing.Point(6, 163);
            this.lvPSGroup.MultiSelect = false;
            this.lvPSGroup.Name = "lvPSGroup";

            this.lvPSGroup.Size = new System.Drawing.Size(136, 206);
            this.lvPSGroup.TabIndex = 16;
            this.lvPSGroup.UseCompatibleStateImageBehavior = false;
            this.lvPSGroup.View = System.Windows.Forms.View.Details;
            this.lvPSGroup.SelectedIndexChanged += new System.EventHandler(lvPSGroup_SelectedIndexChanged);
            this.ColumnHeader11.Text = "Group";
            this.ColumnHeader11.Width = 110;

            this.btnPUp.Location = new System.Drawing.Point(283, 19);
            this.btnPUp.Name = "btnPUp";

            this.btnPUp.Size = new System.Drawing.Size(84, 23);
            this.btnPUp.TabIndex = 8;
            this.btnPUp.Text = "Up";
            this.btnPUp.UseVisualStyleBackColor = true;
            this.btnPUp.Click += new System.EventHandler(btnPUp_Click);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnUGDelete);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnUGAdd);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnUGDown);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnUGUp);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lvUpgrade);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lvUGPower);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lvUGSet);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lvUGGroup);

            this.GroupBox2.Location = new System.Drawing.Point(391, 117);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(514, 375);
            this.GroupBox2.TabIndex = 7;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Upgrade Powers";

            this.btnUGDelete.Location = new System.Drawing.Point(424, 108);
            this.btnUGDelete.Name = "btnUGDelete";

            this.btnUGDelete.Size = new System.Drawing.Size(84, 23);
            this.btnUGDelete.TabIndex = 23;
            this.btnUGDelete.Text = "Remove";
            this.btnUGDelete.UseVisualStyleBackColor = true;
            this.btnUGDelete.Click += new System.EventHandler(btnUGDelete_Click);

            this.btnUGAdd.Location = new System.Drawing.Point(424, 79);
            this.btnUGAdd.Name = "btnUGAdd";

            this.btnUGAdd.Size = new System.Drawing.Size(84, 23);
            this.btnUGAdd.TabIndex = 22;
            this.btnUGAdd.Text = "Add";
            this.btnUGAdd.UseVisualStyleBackColor = true;
            this.btnUGAdd.Click += new System.EventHandler(btnUGAdd_Click);

            this.btnUGDown.Location = new System.Drawing.Point(424, 48);
            this.btnUGDown.Name = "btnUGDown";

            this.btnUGDown.Size = new System.Drawing.Size(84, 23);
            this.btnUGDown.TabIndex = 21;
            this.btnUGDown.Text = "Down";
            this.btnUGDown.UseVisualStyleBackColor = true;
            this.btnUGDown.Click += new System.EventHandler(btnUGDown_Click);

            this.btnUGUp.Location = new System.Drawing.Point(424, 19);
            this.btnUGUp.Name = "btnUGUp";

            this.btnUGUp.Size = new System.Drawing.Size(84, 23);
            this.btnUGUp.TabIndex = 20;
            this.btnUGUp.Text = "Up";
            this.btnUGUp.UseVisualStyleBackColor = true;
            this.btnUGUp.Click += new System.EventHandler(btnUGUp_Click);
            this.lvUpgrade.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader5
            });
            this.lvUpgrade.FullRowSelect = true;
            this.lvUpgrade.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUpgrade.HideSelection = false;

            this.lvUpgrade.Location = new System.Drawing.Point(6, 19);
            this.lvUpgrade.MultiSelect = false;
            this.lvUpgrade.Name = "lvUpgrade";

            this.lvUpgrade.Size = new System.Drawing.Size(412, 112);
            this.lvUpgrade.TabIndex = 19;
            this.lvUpgrade.UseCompatibleStateImageBehavior = false;
            this.lvUpgrade.View = System.Windows.Forms.View.Details;
            this.lvUpgrade.SelectedIndexChanged += new System.EventHandler(lvUpgrade_SelectedIndexChanged);
            this.ColumnHeader5.Text = "Upgrades";
            this.ColumnHeader5.Width = 382;
            this.lvUGPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
                this.ColumnHeader3
            });
            this.lvUGPower.FullRowSelect = true;
            this.lvUGPower.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUGPower.HideSelection = false;

            this.lvUGPower.Location = new System.Drawing.Point(370, 137);
            this.lvUGPower.MultiSelect = false;
            this.lvUGPower.Name = "lvUGPower";

            this.lvUGPower.Size = new System.Drawing.Size(138, 232);
            this.lvUGPower.TabIndex = 18;
            this.lvUGPower.UseCompatibleStateImageBehavior = false;
            this.lvUGPower.View = System.Windows.Forms.View.Details;
            this.lvUGPower.SelectedIndexChanged += this.lvUGPower_SelectedIndexChanged;
            this.lvUGPower.Click += new System.EventHandler(this.lvUGPower_Click);
            this.ColumnHeader3.Text = "Power";
            this.ColumnHeader3.Width = 110;
            this.lvUGSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader1
            });
            this.lvUGSet.FullRowSelect = true;
            this.lvUGSet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUGSet.HideSelection = false;

            this.lvUGSet.Location = new System.Drawing.Point(148, 137);
            this.lvUGSet.MultiSelect = false;
            this.lvUGSet.Name = "lvUGSet";

            this.lvUGSet.Size = new System.Drawing.Size(216, 232);
            this.lvUGSet.TabIndex = 17;
            this.lvUGSet.UseCompatibleStateImageBehavior = false;
            this.lvUGSet.View = System.Windows.Forms.View.Details;
            this.lvUGSet.SelectedIndexChanged += new System.EventHandler(lvUGSet_SelectedIndexChanged);
            this.ColumnHeader1.Text = "Set";
            this.ColumnHeader1.Width = 188;
            this.lvUGGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
                this.ColumnHeader2
            });
            this.lvUGGroup.FullRowSelect = true;
            this.lvUGGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvUGGroup.HideSelection = false;

            this.lvUGGroup.Location = new System.Drawing.Point(6, 137);
            this.lvUGGroup.MultiSelect = false;
            this.lvUGGroup.Name = "lvUGGroup";

            this.lvUGGroup.Size = new System.Drawing.Size(136, 232);
            this.lvUGGroup.TabIndex = 16;
            this.lvUGGroup.UseCompatibleStateImageBehavior = false;
            this.lvUGGroup.View = System.Windows.Forms.View.Details;
            this.lvUGGroup.SelectedIndexChanged += new System.EventHandler(lvUGGroup_SelectedIndexChanged);
            this.ColumnHeader2.Text = "Group";
            this.ColumnHeader2.Width = 110;

            this.Label4.Location = new System.Drawing.Point(279, 9);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(629, 102);
            this.Label4.TabIndex = 8;

            this.Label5.Location = new System.Drawing.Point(12, 89);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(83, 20);
            this.Label5.TabIndex = 10;
            this.Label5.Text = "Class:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbClass.FormattingEnabled = true;

            this.cbClass.Location = new System.Drawing.Point(101, 89);
            this.cbClass.Name = "cbClass";

            this.cbClass.Size = new System.Drawing.Size(172, 22);
            this.cbClass.TabIndex = 9;
            this.cbClass.SelectedIndexChanged += new System.EventHandler(cbClass_SelectedIndexChanged);

            this.btnOK.Location = new System.Drawing.Point(830, 498);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(749, 498);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.AcceptButton = (System.Windows.Forms.IButtonControl)this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = (System.Windows.Forms.IButtonControl)this.btnCancel;

            this.ClientSize = new System.Drawing.Size(918, 547);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.Controls.Add((System.Windows.Forms.Control)this.cbClass);
            this.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.Controls.Add((System.Windows.Forms.Control)this.cbEntType);
            this.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.Controls.Add((System.Windows.Forms.Control)this.txtDisplayName);
            this.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.Controls.Add((System.Windows.Forms.Control)this.txtEntName);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editing Entity: Entity_Name";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        Button btnCancel;
        Button btnOK;
        Button btnPAdd;
        Button btnPDelete;
        Button btnPDown;
        Button btnPUp;
        Button btnUGAdd;
        Button btnUGDelete;
        Button btnUGDown;
        Button btnUGUp;
        ComboBox cbClass;
        ComboBox cbEntType;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader10;
        ColumnHeader ColumnHeader11;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        Label Label1;
        Label Label2;
        Label Label3;
        Label Label4;
        Label Label5;
        ListView lvPower;
        ListView lvPSGroup;
        ListView lvPSSet;
        ListView lvUGGroup;
        ListView lvUGPower;
        ListView lvUGSet;
        ListView lvUpgrade;
        TextBox txtDisplayName;
        TextBox txtEntName;
    }


}
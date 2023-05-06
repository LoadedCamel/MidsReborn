using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerBrowser
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
            components = new Container();
            lvPower = new ListView();
            ColumnHeader3 = new ColumnHeader();
            ColumnHeader5 = new ColumnHeader();
            ColumnHeader7 = new ColumnHeader();
            lvSet = new ListView();
            ColumnHeader1 = new ColumnHeader();
            ColumnHeader4 = new ColumnHeader();
            ColumnHeader6 = new ColumnHeader();
            ilPS = new ImageList(components);
            lvGroup = new ListView();
            ColumnHeader2 = new ColumnHeader();
            ilAT = new ImageList(components);
            cbFilter = new ComboBox();
            Label1 = new Label();
            ilPower = new ImageList(components);
            btnPowerUp = new Button();
            btnPowerDown = new Button();
            btnPowerAdd = new Button();
            btnPowerDelete = new Button();
            btnPowerClone = new Button();
            btnPowerEdit = new Button();
            btnSetEdit = new Button();
            btnSetDelete = new Button();
            btnSetAdd = new Button();
            btnClassClone = new Button();
            btnClassEdit = new Button();
            btnClassDelete = new Button();
            btnClassAdd = new Button();
            pnlGroup = new Panel();
            btnClassUp = new Button();
            btnClassDown = new Button();
            pnlSet = new Panel();
            btnPSUp = new Button();
            btnPSDown = new Button();
            pnlPower = new Panel();
            lblSet = new Label();
            lblPower = new Label();
            btnOK = new Button();
            btnCancel = new Button();
            Label2 = new Label();
            btnManageHiddenPowers = new Button();
            btnMassOp = new Button();
            btnDbQueries = new Button();
            pnlGroup.SuspendLayout();
            pnlSet.SuspendLayout();
            pnlPower.SuspendLayout();
            SuspendLayout();
            // 
            // lvPower
            // 
            lvPower.BorderStyle = BorderStyle.FixedSingle;
            lvPower.Columns.AddRange(new ColumnHeader[] { ColumnHeader3, ColumnHeader5, ColumnHeader7 });
            lvPower.FullRowSelect = true;
            lvPower.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPower.Location = new System.Drawing.Point(802, 37);
            lvPower.MultiSelect = false;
            lvPower.Name = "lvPower";
            lvPower.Size = new System.Drawing.Size(543, 429);
            lvPower.TabIndex = 21;
            lvPower.UseCompatibleStateImageBehavior = false;
            lvPower.View = View.Details;
            lvPower.SelectedIndexChanged += lvPower_SelectedIndexChanged;
            lvPower.DoubleClick += lvPower_DoubleClick;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Power";
            ColumnHeader3.Width = 291;
            // 
            // ColumnHeader5
            // 
            ColumnHeader5.Text = "Name";
            ColumnHeader5.Width = 185;
            // 
            // ColumnHeader7
            // 
            ColumnHeader7.Text = "Level";
            ColumnHeader7.Width = 63;
            // 
            // lvSet
            // 
            lvSet.Activation = ItemActivation.OneClick;
            lvSet.BorderStyle = BorderStyle.FixedSingle;
            lvSet.Columns.AddRange(new ColumnHeader[] { ColumnHeader1, ColumnHeader4, ColumnHeader6 });
            lvSet.FullRowSelect = true;
            lvSet.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSet.Location = new System.Drawing.Point(358, 37);
            lvSet.MultiSelect = false;
            lvSet.Name = "lvSet";
            lvSet.Size = new System.Drawing.Size(434, 429);
            lvSet.SmallImageList = ilPS;
            lvSet.TabIndex = 20;
            lvSet.UseCompatibleStateImageBehavior = false;
            lvSet.View = View.Details;
            lvSet.SelectedIndexChanged += lvSet_SelectedIndexChanged;
            lvSet.DoubleClick += lvSet_DoubleClick;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Set";
            ColumnHeader1.Width = 196;
            // 
            // ColumnHeader4
            // 
            ColumnHeader4.Text = "Name";
            ColumnHeader4.Width = 174;
            // 
            // ColumnHeader6
            // 
            ColumnHeader6.Text = "Type";
            // 
            // ilPS
            // 
            ilPS.ColorDepth = ColorDepth.Depth32Bit;
            ilPS.ImageSize = new System.Drawing.Size(34, 18);
            ilPS.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvGroup
            // 
            lvGroup.BorderStyle = BorderStyle.FixedSingle;
            lvGroup.Columns.AddRange(new ColumnHeader[] { ColumnHeader2 });
            lvGroup.FullRowSelect = true;
            lvGroup.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvGroup.Location = new System.Drawing.Point(12, 37);
            lvGroup.MultiSelect = false;
            lvGroup.Name = "lvGroup";
            lvGroup.Size = new System.Drawing.Size(340, 429);
            lvGroup.SmallImageList = ilAT;
            lvGroup.TabIndex = 19;
            lvGroup.UseCompatibleStateImageBehavior = false;
            lvGroup.View = View.Details;
            lvGroup.SelectedIndexChanged += lvGroup_SelectedIndexChanged;
            lvGroup.DoubleClick += lvGroup_DoubleClick;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Group";
            ColumnHeader2.Width = 207;
            // 
            // ilAT
            // 
            ilAT.ColorDepth = ColorDepth.Depth32Bit;
            ilAT.ImageSize = new System.Drawing.Size(16, 16);
            ilAT.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cbFilter
            // 
            cbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFilter.FormattingEnabled = true;
            cbFilter.Location = new System.Drawing.Point(86, 9);
            cbFilter.Name = "cbFilter";
            cbFilter.Size = new System.Drawing.Size(221, 21);
            cbFilter.TabIndex = 22;
            cbFilter.SelectedIndexChanged += cbFilter_SelectedIndexChanged;
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(12, 9);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(68, 22);
            Label1.TabIndex = 23;
            Label1.Text = "Filter By:";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ilPower
            // 
            ilPower.ColorDepth = ColorDepth.Depth32Bit;
            ilPower.ImageSize = new System.Drawing.Size(16, 16);
            ilPower.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnPowerUp
            // 
            btnPowerUp.Location = new System.Drawing.Point(3, 3);
            btnPowerUp.Name = "btnPowerUp";
            btnPowerUp.Size = new System.Drawing.Size(75, 23);
            btnPowerUp.TabIndex = 28;
            btnPowerUp.Text = "Up";
            btnPowerUp.UseVisualStyleBackColor = true;
            btnPowerUp.Click += btnPowerUp_Click;
            // 
            // btnPowerDown
            // 
            btnPowerDown.Location = new System.Drawing.Point(3, 32);
            btnPowerDown.Name = "btnPowerDown";
            btnPowerDown.Size = new System.Drawing.Size(75, 23);
            btnPowerDown.TabIndex = 29;
            btnPowerDown.Text = "Down";
            btnPowerDown.UseVisualStyleBackColor = true;
            btnPowerDown.Click += btnPowerDown_Click;
            // 
            // btnPowerAdd
            // 
            btnPowerAdd.Location = new System.Drawing.Point(463, 3);
            btnPowerAdd.Name = "btnPowerAdd";
            btnPowerAdd.Size = new System.Drawing.Size(75, 23);
            btnPowerAdd.TabIndex = 30;
            btnPowerAdd.Text = "Add";
            btnPowerAdd.UseVisualStyleBackColor = true;
            btnPowerAdd.Click += btnPowerAdd_Click;
            // 
            // btnPowerDelete
            // 
            btnPowerDelete.Location = new System.Drawing.Point(382, 3);
            btnPowerDelete.Name = "btnPowerDelete";
            btnPowerDelete.Size = new System.Drawing.Size(75, 23);
            btnPowerDelete.TabIndex = 31;
            btnPowerDelete.Text = "Delete";
            btnPowerDelete.UseVisualStyleBackColor = true;
            btnPowerDelete.Click += btnPowerDelete_Click;
            // 
            // btnPowerClone
            // 
            btnPowerClone.Location = new System.Drawing.Point(301, 32);
            btnPowerClone.Name = "btnPowerClone";
            btnPowerClone.Size = new System.Drawing.Size(75, 23);
            btnPowerClone.TabIndex = 33;
            btnPowerClone.Text = "Clone";
            btnPowerClone.UseVisualStyleBackColor = true;
            btnPowerClone.Click += btnPowerClone_Click;
            // 
            // btnPowerEdit
            // 
            btnPowerEdit.Location = new System.Drawing.Point(301, 3);
            btnPowerEdit.Name = "btnPowerEdit";
            btnPowerEdit.Size = new System.Drawing.Size(75, 23);
            btnPowerEdit.TabIndex = 32;
            btnPowerEdit.Text = "Edit";
            btnPowerEdit.UseVisualStyleBackColor = true;
            btnPowerEdit.Click += btnPowerEdit_Click;
            // 
            // btnSetEdit
            // 
            btnSetEdit.Location = new System.Drawing.Point(192, 3);
            btnSetEdit.Name = "btnSetEdit";
            btnSetEdit.Size = new System.Drawing.Size(75, 23);
            btnSetEdit.TabIndex = 40;
            btnSetEdit.Text = "Edit";
            btnSetEdit.UseVisualStyleBackColor = true;
            btnSetEdit.Click += btnSetEdit_Click;
            // 
            // btnSetDelete
            // 
            btnSetDelete.Location = new System.Drawing.Point(273, 3);
            btnSetDelete.Name = "btnSetDelete";
            btnSetDelete.Size = new System.Drawing.Size(75, 23);
            btnSetDelete.TabIndex = 39;
            btnSetDelete.Text = "Delete";
            btnSetDelete.UseVisualStyleBackColor = true;
            btnSetDelete.Click += btnSetDelete_Click;
            // 
            // btnSetAdd
            // 
            btnSetAdd.Location = new System.Drawing.Point(354, 3);
            btnSetAdd.Name = "btnSetAdd";
            btnSetAdd.Size = new System.Drawing.Size(75, 23);
            btnSetAdd.TabIndex = 38;
            btnSetAdd.Text = "Add";
            btnSetAdd.UseVisualStyleBackColor = true;
            btnSetAdd.Click += btnSetAdd_Click;
            // 
            // btnClassClone
            // 
            btnClassClone.Location = new System.Drawing.Point(98, 32);
            btnClassClone.Name = "btnClassClone";
            btnClassClone.Size = new System.Drawing.Size(75, 23);
            btnClassClone.TabIndex = 46;
            btnClassClone.Text = "Clone";
            btnClassClone.UseVisualStyleBackColor = true;
            btnClassClone.Click += btnClassClone_Click;
            // 
            // btnClassEdit
            // 
            btnClassEdit.Location = new System.Drawing.Point(98, 3);
            btnClassEdit.Name = "btnClassEdit";
            btnClassEdit.Size = new System.Drawing.Size(75, 23);
            btnClassEdit.TabIndex = 45;
            btnClassEdit.Text = "Edit";
            btnClassEdit.UseVisualStyleBackColor = true;
            btnClassEdit.Click += btnClassEdit_Click;
            // 
            // btnClassDelete
            // 
            btnClassDelete.Location = new System.Drawing.Point(179, 3);
            btnClassDelete.Name = "btnClassDelete";
            btnClassDelete.Size = new System.Drawing.Size(75, 23);
            btnClassDelete.TabIndex = 44;
            btnClassDelete.Text = "Delete";
            btnClassDelete.UseVisualStyleBackColor = true;
            btnClassDelete.Click += btnClassDelete_Click;
            // 
            // btnClassAdd
            // 
            btnClassAdd.Location = new System.Drawing.Point(260, 3);
            btnClassAdd.Name = "btnClassAdd";
            btnClassAdd.Size = new System.Drawing.Size(75, 23);
            btnClassAdd.TabIndex = 43;
            btnClassAdd.Text = "Add";
            btnClassAdd.UseVisualStyleBackColor = true;
            btnClassAdd.Click += btnClassAdd_Click;
            // 
            // pnlGroup
            // 
            pnlGroup.BorderStyle = BorderStyle.FixedSingle;
            pnlGroup.Controls.Add(btnClassUp);
            pnlGroup.Controls.Add(btnClassDown);
            pnlGroup.Controls.Add(btnClassClone);
            pnlGroup.Controls.Add(btnClassDelete);
            pnlGroup.Controls.Add(btnClassAdd);
            pnlGroup.Controls.Add(btnClassEdit);
            pnlGroup.Location = new System.Drawing.Point(12, 497);
            pnlGroup.Name = "pnlGroup";
            pnlGroup.Size = new System.Drawing.Size(340, 68);
            pnlGroup.TabIndex = 47;
            // 
            // btnClassUp
            // 
            btnClassUp.Location = new System.Drawing.Point(3, 3);
            btnClassUp.Name = "btnClassUp";
            btnClassUp.Size = new System.Drawing.Size(64, 23);
            btnClassUp.TabIndex = 47;
            btnClassUp.Text = "Up";
            btnClassUp.UseVisualStyleBackColor = true;
            btnClassUp.Click += btnClassUp_Click;
            // 
            // btnClassDown
            // 
            btnClassDown.Location = new System.Drawing.Point(3, 32);
            btnClassDown.Name = "btnClassDown";
            btnClassDown.Size = new System.Drawing.Size(64, 23);
            btnClassDown.TabIndex = 48;
            btnClassDown.Text = "Down";
            btnClassDown.UseVisualStyleBackColor = true;
            btnClassDown.Click += btnClassDown_Click;
            // 
            // pnlSet
            // 
            pnlSet.BorderStyle = BorderStyle.FixedSingle;
            pnlSet.Controls.Add(btnPSUp);
            pnlSet.Controls.Add(btnPSDown);
            pnlSet.Controls.Add(btnSetDelete);
            pnlSet.Controls.Add(btnSetAdd);
            pnlSet.Controls.Add(btnSetEdit);
            pnlSet.Location = new System.Drawing.Point(358, 497);
            pnlSet.Name = "pnlSet";
            pnlSet.Size = new System.Drawing.Size(434, 68);
            pnlSet.TabIndex = 48;
            // 
            // btnPSUp
            // 
            btnPSUp.Location = new System.Drawing.Point(3, 3);
            btnPSUp.Name = "btnPSUp";
            btnPSUp.Size = new System.Drawing.Size(75, 23);
            btnPSUp.TabIndex = 41;
            btnPSUp.Text = "Up";
            btnPSUp.UseVisualStyleBackColor = true;
            btnPSUp.Click += btnPSUp_Click;
            // 
            // btnPSDown
            // 
            btnPSDown.Location = new System.Drawing.Point(3, 32);
            btnPSDown.Name = "btnPSDown";
            btnPSDown.Size = new System.Drawing.Size(75, 23);
            btnPSDown.TabIndex = 42;
            btnPSDown.Text = "Down";
            btnPSDown.UseVisualStyleBackColor = true;
            btnPSDown.Click += btnPSDown_Click;
            // 
            // pnlPower
            // 
            pnlPower.BorderStyle = BorderStyle.FixedSingle;
            pnlPower.Controls.Add(btnPowerClone);
            pnlPower.Controls.Add(btnPowerUp);
            pnlPower.Controls.Add(btnPowerDown);
            pnlPower.Controls.Add(btnPowerEdit);
            pnlPower.Controls.Add(btnPowerDelete);
            pnlPower.Controls.Add(btnPowerAdd);
            pnlPower.Location = new System.Drawing.Point(798, 497);
            pnlPower.Name = "pnlPower";
            pnlPower.Size = new System.Drawing.Size(543, 68);
            pnlPower.TabIndex = 49;
            // 
            // lblSet
            // 
            lblSet.Location = new System.Drawing.Point(355, 469);
            lblSet.Name = "lblSet";
            lblSet.Size = new System.Drawing.Size(437, 24);
            lblSet.TabIndex = 50;
            lblSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPower
            // 
            lblPower.Location = new System.Drawing.Point(799, 469);
            lblPower.Name = "lblPower";
            lblPower.Size = new System.Drawing.Size(542, 24);
            lblPower.TabIndex = 51;
            lblPower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(1209, 571);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(132, 32);
            btnOK.TabIndex = 52;
            btnOK.Text = "Save && Close";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(1071, 571);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(132, 32);
            btnCancel.TabIndex = 53;
            btnCancel.Text = "Cancel && Discard";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // Label2
            // 
            Label2.Location = new System.Drawing.Point(313, 13);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(323, 16);
            Label2.TabIndex = 54;
            Label2.Text = "To edit Archetype Classes, change filtering to Classes";
            // 
            // btnManageHiddenPowers
            // 
            btnManageHiddenPowers.Location = new System.Drawing.Point(652, 571);
            btnManageHiddenPowers.Name = "btnManageHiddenPowers";
            btnManageHiddenPowers.Size = new System.Drawing.Size(140, 32);
            btnManageHiddenPowers.TabIndex = 55;
            btnManageHiddenPowers.Text = "Manage Hidden Powers";
            btnManageHiddenPowers.UseVisualStyleBackColor = true;
            btnManageHiddenPowers.Click += btnManageHiddenPowers_Click;
            // 
            // btnMassOp
            // 
            btnMassOp.Location = new System.Drawing.Point(514, 571);
            btnMassOp.Name = "btnMassOp";
            btnMassOp.Size = new System.Drawing.Size(132, 32);
            btnMassOp.TabIndex = 56;
            btnMassOp.Text = "Mass op.";
            btnMassOp.UseVisualStyleBackColor = true;
            btnMassOp.Visible = false;
            btnMassOp.Click += btnMassOp_Click;
            // 
            // btnDbQueries
            // 
            btnDbQueries.Location = new System.Drawing.Point(798, 571);
            btnDbQueries.Name = "btnDbQueries";
            btnDbQueries.Size = new System.Drawing.Size(132, 32);
            btnDbQueries.TabIndex = 57;
            btnDbQueries.Text = "DB Queries";
            btnDbQueries.UseVisualStyleBackColor = true;
            btnDbQueries.Click += btnDbQueries_Click;
            // 
            // frmPowerBrowser
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new System.Drawing.Size(1356, 612);
            Controls.Add(btnDbQueries);
            Controls.Add(btnMassOp);
            Controls.Add(btnManageHiddenPowers);
            Controls.Add(Label2);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(lblPower);
            Controls.Add(lblSet);
            Controls.Add(Label1);
            Controls.Add(cbFilter);
            Controls.Add(lvPower);
            Controls.Add(lvSet);
            Controls.Add(lvGroup);
            Controls.Add(pnlGroup);
            Controls.Add(pnlSet);
            Controls.Add(pnlPower);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmPowerBrowser";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Power Database Browser";
            pnlGroup.ResumeLayout(false);
            pnlSet.ResumeLayout(false);
            pnlPower.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        Button btnCancel;
        Button btnClassAdd;
        Button btnClassClone;
        Button btnClassDelete;
        Button btnClassDown;
        Button btnClassEdit;
        Button btnOK;
        Button btnPowerAdd;
        Button btnPowerClone;
        Button btnPowerDelete;
        Button btnPowerDown;
        Button btnPowerEdit;
        Button btnPowerUp;
        Button btnPSDown;
        Button btnPSUp;
        Button btnSetAdd;
        Button btnSetDelete;
        Button btnSetEdit;
        ComboBox cbFilter;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ColumnHeader ColumnHeader6;
        ColumnHeader ColumnHeader7;
        ImageList ilAT;
        ImageList ilPower;
        ImageList ilPS;
        Label Label1;
        Label Label2;
        Label lblPower;
        Label lblSet;
        ListView lvGroup;
        ListView lvPower;
        ListView lvSet;
        Panel pnlGroup;
        Panel pnlPower;
        Panel pnlSet;
        private Button btnManageHiddenPowers;
        private Button btnMassOp;
        private Button btnClassUp;
        private Button btnDbQueries;
    }
}
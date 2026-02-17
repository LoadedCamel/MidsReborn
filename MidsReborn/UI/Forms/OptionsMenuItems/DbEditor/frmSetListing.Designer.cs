using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSetListing
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
            ilSets = new ImageList(components);
            lvSets = new ListView();
            ColumnHeader1 = new ColumnHeader();
            ColumnHeader2 = new ColumnHeader();
            ColumnHeader3 = new ColumnHeader();
            ColumnHeader4 = new ColumnHeader();
            ColumnHeader5 = new ColumnHeader();
            ColumnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            btnClone = new Button();
            btnCancel = new Button();
            btnSave = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnAdd = new Button();
            btnDown = new Button();
            btnUp = new Button();
            NoReload = new CheckBox();
            SuspendLayout();
            // 
            // ilSets
            // 
            ilSets.ColorDepth = ColorDepth.Depth32Bit;
            ilSets.ImageSize = new System.Drawing.Size(16, 16);
            ilSets.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvSets
            // 
            lvSets.Columns.AddRange(new ColumnHeader[] { ColumnHeader1, ColumnHeader2, ColumnHeader3, ColumnHeader4, ColumnHeader5, ColumnHeader6, columnHeader7 });
            lvSets.FullRowSelect = true;
            lvSets.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSets.LargeImageList = ilSets;
            lvSets.Location = new System.Drawing.Point(19, 20);
            lvSets.MultiSelect = false;
            lvSets.Name = "lvSets";
            lvSets.Size = new System.Drawing.Size(689, 640);
            lvSets.SmallImageList = ilSets;
            lvSets.TabIndex = 0;
            lvSets.UseCompatibleStateImageBehavior = false;
            lvSets.View = View.Details;
            lvSets.SelectedIndexChanged += lvSets_SelectedIndexChanged;
            lvSets.DoubleClick += lvSets_DoubleClick;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Set Name";
            ColumnHeader1.Width = 233;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Type";
            ColumnHeader2.Width = 104;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Min Level";
            ColumnHeader3.Width = 70;
            // 
            // ColumnHeader4
            // 
            ColumnHeader4.Text = "Max Level";
            ColumnHeader4.Width = 70;
            // 
            // ColumnHeader5
            // 
            ColumnHeader5.Text = "Enh's";
            ColumnHeader5.Width = 48;
            // 
            // ColumnHeader6
            // 
            ColumnHeader6.Text = "FX";
            ColumnHeader6.Width = 53;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Has PvP FX";
            columnHeader7.Width = 76;
            // 
            // btnClone
            // 
            btnClone.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnClone.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnClone.Location = new System.Drawing.Point(738, 162);
            btnClone.Name = "btnClone";
            btnClone.Size = new System.Drawing.Size(90, 29);
            btnClone.TabIndex = 32;
            btnClone.Text = "Clone...";
            btnClone.UseVisualStyleBackColor = true;
            btnClone.Click += btnClone_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnCancel.Location = new System.Drawing.Point(596, 670);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(232, 39);
            btnCancel.TabIndex = 31;
            btnCancel.Text = "Cancel and Discard Changes";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnSave.DialogResult = DialogResult.OK;
            btnSave.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnSave.Location = new System.Drawing.Point(438, 670);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(134, 39);
            btnSave.TabIndex = 30;
            btnSave.Text = "Save and Close";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnEdit
            // 
            btnEdit.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnEdit.Location = new System.Drawing.Point(738, 212);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new System.Drawing.Size(90, 28);
            btnEdit.TabIndex = 29;
            btnEdit.Text = "Edit...";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnDelete.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnDelete.Location = new System.Drawing.Point(738, 260);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(90, 28);
            btnDelete.TabIndex = 28;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnAdd.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnAdd.Location = new System.Drawing.Point(738, 113);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(90, 29);
            btnAdd.TabIndex = 27;
            btnAdd.Text = "Add...";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDown
            // 
            btnDown.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnDown.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnDown.Location = new System.Drawing.Point(738, 54);
            btnDown.Name = "btnDown";
            btnDown.Size = new System.Drawing.Size(90, 28);
            btnDown.TabIndex = 26;
            btnDown.Text = "Move Down";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // btnUp
            // 
            btnUp.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            btnUp.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            btnUp.Location = new System.Drawing.Point(738, 20);
            btnUp.Name = "btnUp";
            btnUp.Size = new System.Drawing.Size(90, 28);
            btnUp.TabIndex = 25;
            btnUp.Text = "Move Up";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // NoReload
            // 
            NoReload.ForeColor = System.Drawing.Color.White;
            NoReload.Location = new System.Drawing.Point(24, 679);
            NoReload.Name = "NoReload";
            NoReload.Size = new System.Drawing.Size(298, 20);
            NoReload.TabIndex = 33;
            NoReload.Text = "Disable Image Reload";
            NoReload.CheckedChanged += NoReload_CheckedChanged;
            // 
            // frmSetListing
            // 
            AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            ClientSize = new System.Drawing.Size(847, 734);
            Controls.Add(NoReload);
            Controls.Add(btnClone);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(lvSets);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSetListing";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Invention Sets";
            ResumeLayout(false);

        }
        #endregion

        Button btnAdd;
        Button btnCancel;
        Button btnClone;
        Button btnDelete;
        Button btnDown;
        Button btnEdit;
        Button btnSave;
        Button btnUp;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ColumnHeader ColumnHeader6;
        ImageList ilSets;
        ListView lvSets;
        CheckBox NoReload;
        private ColumnHeader columnHeader7;
    }
}
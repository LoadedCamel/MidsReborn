using System.ComponentModel;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEntityListing
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
            lvEntity = new System.Windows.Forms.ListView();
            ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            btnUp = new System.Windows.Forms.Button();
            btnDown = new System.Windows.Forms.Button();
            btnAdd = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            btnEdit = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnClone = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lvEntity
            // 
            lvEntity.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ColumnHeader1, ColumnHeader2, ColumnHeader3 });
            lvEntity.FullRowSelect = true;
            lvEntity.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvEntity.Location = new System.Drawing.Point(12, 12);
            lvEntity.MultiSelect = false;
            lvEntity.Name = "lvEntity";
            lvEntity.Size = new System.Drawing.Size(664, 431);
            lvEntity.TabIndex = 0;
            lvEntity.UseCompatibleStateImageBehavior = false;
            lvEntity.View = System.Windows.Forms.View.Details;
            lvEntity.DoubleClick += lvEntity_DoubleClick;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Entity";
            ColumnHeader1.Width = 265;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Name";
            ColumnHeader2.Width = 265;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Type";
            ColumnHeader3.Width = 96;
            // 
            // btnUp
            // 
            btnUp.Location = new System.Drawing.Point(682, 12);
            btnUp.Name = "btnUp";
            btnUp.Size = new System.Drawing.Size(75, 23);
            btnUp.TabIndex = 1;
            btnUp.Text = "Up";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // btnDown
            // 
            btnDown.Location = new System.Drawing.Point(682, 41);
            btnDown.Name = "btnDown";
            btnDown.Size = new System.Drawing.Size(75, 23);
            btnDown.TabIndex = 2;
            btnDown.Text = "Down";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(682, 100);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(75, 23);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(682, 158);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(75, 23);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Remove";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnEdit
            // 
            btnEdit.Location = new System.Drawing.Point(682, 187);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new System.Drawing.Size(75, 23);
            btnEdit.TabIndex = 5;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(682, 391);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 23);
            btnOK.TabIndex = 6;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(682, 420);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnClone
            // 
            btnClone.Location = new System.Drawing.Point(682, 129);
            btnClone.Name = "btnClone";
            btnClone.Size = new System.Drawing.Size(75, 23);
            btnClone.TabIndex = 8;
            btnClone.Text = "Clone";
            btnClone.UseVisualStyleBackColor = true;
            btnClone.Click += btnClone_Click;
            // 
            // frmEntityListing
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            ClientSize = new System.Drawing.Size(769, 454);
            Controls.Add(btnClone);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(lvEntity);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmEntityListing";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Entity Editor";
            ResumeLayout(false);
        }

        #endregion
    }
}
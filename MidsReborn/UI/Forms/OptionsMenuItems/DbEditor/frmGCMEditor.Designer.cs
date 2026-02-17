using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    partial class FrmGCMEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            lvModifiers = new ctlListViewColored();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            btnRemoveMod = new System.Windows.Forms.Button();
            btnAddMod = new System.Windows.Forms.Button();
            btnImportMods = new System.Windows.Forms.Button();
            btnExportMods = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            fdJSONImport = new System.Windows.Forms.OpenFileDialog();
            SuspendLayout();
            // 
            // lvModifiers
            // 
            lvModifiers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1 });
            lvModifiers.Location = new System.Drawing.Point(12, 12);
            lvModifiers.LostFocusItem = -1;
            lvModifiers.Name = "lvModifiers";
            lvModifiers.OwnerDraw = true;
            lvModifiers.Size = new System.Drawing.Size(208, 381);
            lvModifiers.TabIndex = 0;
            lvModifiers.UseCompatibleStateImageBehavior = false;
            lvModifiers.View = System.Windows.Forms.View.Details;
            lvModifiers.DrawColumnHeader += ListView_DrawColumnHeader;
            lvModifiers.DrawItem += ListView_DrawItem;
            lvModifiers.Leave += ListView_Leave;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Current Modifiers";
            columnHeader1.Width = 187;
            // 
            // btnRemoveMod
            // 
            btnRemoveMod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            btnRemoveMod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnRemoveMod.Location = new System.Drawing.Point(232, 12);
            btnRemoveMod.Name = "btnRemoveMod";
            btnRemoveMod.Size = new System.Drawing.Size(173, 43);
            btnRemoveMod.TabIndex = 1;
            btnRemoveMod.Text = "Remove Selected Modifier";
            btnRemoveMod.UseVisualStyleBackColor = true;
            btnRemoveMod.Click += btnRemoveMod_Click;
            // 
            // btnAddMod
            // 
            btnAddMod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            btnAddMod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnAddMod.Location = new System.Drawing.Point(232, 61);
            btnAddMod.Name = "btnAddMod";
            btnAddMod.Size = new System.Drawing.Size(173, 44);
            btnAddMod.TabIndex = 2;
            btnAddMod.Text = "Add New Modifier";
            btnAddMod.UseVisualStyleBackColor = true;
            btnAddMod.Click += btnAddMod_Click;
            // 
            // btnImportMods
            // 
            btnImportMods.Anchor = System.Windows.Forms.AnchorStyles.Right;
            btnImportMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnImportMods.Location = new System.Drawing.Point(232, 174);
            btnImportMods.Name = "btnImportMods";
            btnImportMods.Size = new System.Drawing.Size(173, 40);
            btnImportMods.TabIndex = 3;
            btnImportMods.Text = "Import Modifiers from JSON";
            btnImportMods.UseVisualStyleBackColor = true;
            btnImportMods.Click += btnImportMods_Click;
            // 
            // btnExportMods
            // 
            btnExportMods.Anchor = System.Windows.Forms.AnchorStyles.Right;
            btnExportMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnExportMods.Location = new System.Drawing.Point(232, 220);
            btnExportMods.Name = "btnExportMods";
            btnExportMods.Size = new System.Drawing.Size(173, 37);
            btnExportMods.TabIndex = 4;
            btnExportMods.Text = "Export Modifiers to JSON";
            btnExportMods.UseVisualStyleBackColor = true;
            btnExportMods.Click += btnExportMods_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCancel.Location = new System.Drawing.Point(334, 357);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(71, 33);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnSave.Location = new System.Drawing.Point(232, 357);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(96, 33);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save && Close";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // FrmGCMEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(417, 402);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(btnExportMods);
            Controls.Add(btnImportMods);
            Controls.Add(btnAddMod);
            Controls.Add(btnRemoveMod);
            Controls.Add(lvModifiers);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmGCMEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Global Chance Modifiers Editor";
            ResumeLayout(false);

        }

        #endregion

        private ctlListViewColored lvModifiers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnRemoveMod;
        private System.Windows.Forms.Button btnAddMod;
        private System.Windows.Forms.Button btnImportMods;
        private System.Windows.Forms.Button btnExportMods;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.OpenFileDialog fdJSONImport;
    }
}
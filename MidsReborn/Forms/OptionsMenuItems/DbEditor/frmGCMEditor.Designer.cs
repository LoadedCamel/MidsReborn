using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
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
            this.lvModifiers = new ctlListViewColored();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemoveMod = new System.Windows.Forms.Button();
            this.btnAddMod = new System.Windows.Forms.Button();
            this.btnImportMods = new System.Windows.Forms.Button();
            this.btnExportMods = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.fdJSONImport = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lvModifiers
            // 
            this.lvModifiers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvModifiers.HideSelection = false;
            this.lvModifiers.Location = new System.Drawing.Point(12, 12);
            this.lvModifiers.LostFocusItem = -1;
            this.lvModifiers.Name = "lvModifiers";
            this.lvModifiers.OwnerDraw = true;
            this.lvModifiers.Size = new System.Drawing.Size(208, 381);
            this.lvModifiers.TabIndex = 0;
            this.lvModifiers.UseCompatibleStateImageBehavior = false;
            this.lvModifiers.View = System.Windows.Forms.View.Details;
            this.lvModifiers.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvModifiers.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvModifiers.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Current Modifiers";
            this.columnHeader1.Width = 187;
            // 
            // btnRemoveMod
            // 
            this.btnRemoveMod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRemoveMod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveMod.Location = new System.Drawing.Point(232, 12);
            this.btnRemoveMod.Name = "btnRemoveMod";
            this.btnRemoveMod.Size = new System.Drawing.Size(173, 43);
            this.btnRemoveMod.TabIndex = 1;
            this.btnRemoveMod.Text = "Remove Selected Modifier";
            this.btnRemoveMod.UseVisualStyleBackColor = true;
            this.btnRemoveMod.Click += new System.EventHandler(this.btnRemoveMod_Click);
            // 
            // btnAddMod
            // 
            this.btnAddMod.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddMod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddMod.Location = new System.Drawing.Point(232, 61);
            this.btnAddMod.Name = "btnAddMod";
            this.btnAddMod.Size = new System.Drawing.Size(173, 44);
            this.btnAddMod.TabIndex = 2;
            this.btnAddMod.Text = "Add New Modifier";
            this.btnAddMod.UseVisualStyleBackColor = true;
            this.btnAddMod.Click += new System.EventHandler(this.btnAddMod_Click);
            // 
            // btnImportMods
            // 
            this.btnImportMods.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnImportMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportMods.Location = new System.Drawing.Point(232, 174);
            this.btnImportMods.Name = "btnImportMods";
            this.btnImportMods.Size = new System.Drawing.Size(173, 40);
            this.btnImportMods.TabIndex = 3;
            this.btnImportMods.Text = "Import Modifiers from JSON";
            this.btnImportMods.UseVisualStyleBackColor = true;
            this.btnImportMods.Click += new System.EventHandler(this.btnImportMods_Click);
            // 
            // btnExportMods
            // 
            this.btnExportMods.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExportMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportMods.Location = new System.Drawing.Point(232, 220);
            this.btnExportMods.Name = "btnExportMods";
            this.btnExportMods.Size = new System.Drawing.Size(173, 37);
            this.btnExportMods.TabIndex = 4;
            this.btnExportMods.Text = "Export Modifiers to JSON";
            this.btnExportMods.UseVisualStyleBackColor = true;
            this.btnExportMods.Click += new System.EventHandler(this.btnExportMods_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(232, 357);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 33);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(309, 357);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 33);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save && Close";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmGCMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(417, 402);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExportMods);
            this.Controls.Add(this.btnImportMods);
            this.Controls.Add(this.btnAddMod);
            this.Controls.Add(this.btnRemoveMod);
            this.Controls.Add(this.lvModifiers);
            this.Name = "FrmGCMEditor";
            this.Text = "Global Chance Modifiers Editor";
            this.ResumeLayout(false);

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
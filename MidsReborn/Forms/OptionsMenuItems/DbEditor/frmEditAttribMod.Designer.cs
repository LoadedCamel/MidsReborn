using System.Windows.Forms;
using mrbControls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmEditAttribMod
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditAttribMod));
            this.listBoxTables = new System.Windows.Forms.ListBox();
            this.btnImportJson = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cbArchetype = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnAddNewTable = new System.Windows.Forms.Button();
            this.bnRemoveTable = new System.Windows.Forms.Button();
            this.lblRevision = new System.Windows.Forms.NumericUpDown();
            this.lblRevisionDate = new System.Windows.Forms.TextBox();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.pbGraph = new ctlDataGraph();
            ((System.ComponentModel.ISupportInitialize)(this.lblRevision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxTables
            // 
            this.listBoxTables.FormattingEnabled = true;
            this.listBoxTables.Location = new System.Drawing.Point(7, 40);
            this.listBoxTables.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.listBoxTables.Name = "listBoxTables";
            this.listBoxTables.Size = new System.Drawing.Size(204, 368);
            this.listBoxTables.TabIndex = 0;
            this.listBoxTables.SelectedValueChanged += new System.EventHandler(this.listBoxTables_SelectedValueChanged);
            // 
            // btnImportJson
            // 
            this.btnImportJson.Location = new System.Drawing.Point(671, 4);
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(102, 23);
            this.btnImportJson.TabIndex = 2;
            this.btnImportJson.Text = "Import from JSON";
            this.btnImportJson.UseVisualStyleBackColor = true;
            this.btnImportJson.Click += new System.EventHandler(this.btnImportJson_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tables:";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(641, 399);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 39);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Discard changes";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(789, 399);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(127, 39);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save and close";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label2.Location = new System.Drawing.Point(234, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "1";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(226, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "11";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(226, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "21";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(226, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "31";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(226, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "41";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(226, 300);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "51";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(892, 250);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 16);
            this.label9.TabIndex = 19;
            this.label9.Text = "50";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(892, 200);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 16);
            this.label10.TabIndex = 18;
            this.label10.Text = "40";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(892, 150);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "30";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(892, 100);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(24, 16);
            this.label12.TabIndex = 16;
            this.label12.Text = "20";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label13.Location = new System.Drawing.Point(892, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 16);
            this.label13.TabIndex = 15;
            this.label13.Text = "10";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(646, 348);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Revision:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(646, 374);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "Revision Date:";
            // 
            // cbArchetype
            // 
            this.cbArchetype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchetype.FormattingEnabled = true;
            this.cbArchetype.Location = new System.Drawing.Point(299, 5);
            this.cbArchetype.Name = "cbArchetype";
            this.cbArchetype.Size = new System.Drawing.Size(209, 21);
            this.cbArchetype.TabIndex = 24;
            this.cbArchetype.SelectionChangeCommitted += new System.EventHandler(this.cbArchetype_SelectionChangeCommitted);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(227, 9);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(68, 13);
            this.label17.TabIndex = 25;
            this.label17.Text = "Archetype:";
            // 
            // btnAddNewTable
            // 
            this.btnAddNewTable.Location = new System.Drawing.Point(15, 416);
            this.btnAddNewTable.Name = "btnAddNewTable";
            this.btnAddNewTable.Size = new System.Drawing.Size(87, 23);
            this.btnAddNewTable.TabIndex = 27;
            this.btnAddNewTable.Text = "Add New";
            this.btnAddNewTable.UseVisualStyleBackColor = true;
            this.btnAddNewTable.Click += new System.EventHandler(this.btnAddNewTable_Click);
            // 
            // bnRemoveTable
            // 
            this.bnRemoveTable.Location = new System.Drawing.Point(117, 416);
            this.bnRemoveTable.Name = "bnRemoveTable";
            this.bnRemoveTable.Size = new System.Drawing.Size(86, 23);
            this.bnRemoveTable.TabIndex = 28;
            this.bnRemoveTable.Text = "Remove";
            this.bnRemoveTable.UseVisualStyleBackColor = true;
            this.bnRemoveTable.Click += new System.EventHandler(this.bnRemoveTable_Click);
            // 
            // lblRevision
            // 
            this.lblRevision.Location = new System.Drawing.Point(739, 346);
            this.lblRevision.Margin = new System.Windows.Forms.Padding(0);
            this.lblRevision.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.lblRevision.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(91, 20);
            this.lblRevision.TabIndex = 24;
            this.lblRevision.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lblRevision.ValueChanged += new System.EventHandler(this.lblRevision_ValueChanged);
            this.lblRevision.Validating += new System.ComponentModel.CancelEventHandler(this.lblRevision_Validating);
            // 
            // lblRevisionDate
            // 
            this.lblRevisionDate.Location = new System.Drawing.Point(738, 371);
            this.lblRevisionDate.Name = "lblRevisionDate";
            this.lblRevisionDate.Size = new System.Drawing.Size(91, 20);
            this.lblRevisionDate.TabIndex = 29;
            this.lblRevisionDate.Text = "MM/DD/AAAA";
            this.lblRevisionDate.Leave += new System.EventHandler(this.lblRevisionDate_Leave);
            this.lblRevisionDate.Validating += new System.ComponentModel.CancelEventHandler(this.lblRevisionDate_Validating);
            // 
            // btnExportJson
            // 
            this.btnExportJson.Location = new System.Drawing.Point(797, 4);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(102, 23);
            this.btnExportJson.TabIndex = 31;
            this.btnExportJson.Text = "Export to JSON";
            this.btnExportJson.UseVisualStyleBackColor = true;
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            // 
            // pbGraph
            // 
            this.pbGraph.BackColor = System.Drawing.Color.Black;
            this.pbGraph.Location = new System.Drawing.Point(257, 344);
            this.pbGraph.Name = "pbGraph";
            this.pbGraph.Size = new System.Drawing.Size(360, 94);
            this.pbGraph.TabIndex = 30;
            this.pbGraph.TabStop = false;
            // 
            // frmEditAttribMod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 450);
            this.Controls.Add(this.btnExportJson);
            this.Controls.Add(this.pbGraph);
            this.Controls.Add(this.lblRevisionDate);
            this.Controls.Add(this.lblRevision);
            this.Controls.Add(this.bnRemoveTable);
            this.Controls.Add(this.btnAddNewTable);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cbArchetype);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnImportJson);
            this.Controls.Add(this.listBoxTables);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditAttribMod";
            this.Text = "AttribMod Editor";
            this.Load += new System.EventHandler(this.frmEditAttribMod_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lblRevision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxTables;
        private System.Windows.Forms.Button btnImportJson;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cbArchetype;
        private Button btnAddNewTable;
        private Button bnRemoveTable;
        private Control[] dgCells;
        private NumericUpDown lblRevision;
        private TextBox lblRevisionDate;
        private ctlDataGraph pbGraph;
        private Button btnExportJson;
    }
}
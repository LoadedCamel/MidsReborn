using System.ComponentModel;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImport_SetAssignments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImport_SetAssignments));
            this.Label8 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.udRevision = new System.Windows.Forms.NumericUpDown();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.btnFile = new System.Windows.Forms.Button();
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(346, 85);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(65, 18);
            this.Label8.TabIndex = 55;
            this.Label8.Text = "Revision:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(9, 85);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(175, 18);
            this.lblDate.TabIndex = 54;
            this.lblDate.Text = "Date:";
            // 
            // udRevision
            // 
            this.udRevision.Location = new System.Drawing.Point(417, 83);
            this.udRevision.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.udRevision.Name = "udRevision";
            this.udRevision.Size = new System.Drawing.Size(116, 20);
            this.udRevision.TabIndex = 53;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(539, 81);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 23);
            this.btnClose.TabIndex = 52;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(539, 38);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(86, 23);
            this.btnImport.TabIndex = 50;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lblFile
            // 
            this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFile.Location = new System.Drawing.Point(12, 9);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(521, 46);
            this.lblFile.TabIndex = 51;
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(539, 9);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(86, 23);
            this.btnFile.TabIndex = 49;
            this.btnFile.Text = "Browse...";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.DefaultExt = "csv";
            this.dlgBrowse.Filter = "CSV Spreadsheets (*.csv)|*.csv";
            // 
            // frmImport_SetAssignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 115);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.udRevision);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImport_SetAssignments";
            this.ShowInTaskbar = false;
            this.Text = "Invention Set Assignment Import";
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImportPowerLevels
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportPowerLevels));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.btnFile = new System.Windows.Forms.Button();
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            this.Label8 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.udRevision = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(545, 77);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 23);
            this.btnClose.TabIndex = 27;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(545, 34);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(86, 23);
            this.btnImport.TabIndex = 24;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(18, -11);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(150, 14);
            this.Label3.TabIndex = 26;
            this.Label3.Text = "Index:";
            // 
            // lblFile
            // 
            this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFile.Location = new System.Drawing.Point(18, 5);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(521, 46);
            this.lblFile.TabIndex = 25;
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(545, 5);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(86, 23);
            this.btnFile.TabIndex = 23;
            this.btnFile.Text = "Browse...";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.DefaultExt = "csv";
            this.dlgBrowse.Filter = "CSV Spreadsheets (*.csv)|*.csv";
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(352, 81);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(65, 18);
            this.Label8.TabIndex = 48;
            this.Label8.Text = "Revision:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(15, 81);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(175, 18);
            this.lblDate.TabIndex = 46;
            this.lblDate.Text = "Date:";
            // 
            // udRevision
            // 
            this.udRevision.Location = new System.Drawing.Point(423, 79);
            this.udRevision.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.udRevision.Name = "udRevision";
            this.udRevision.Size = new System.Drawing.Size(116, 20);
            this.udRevision.TabIndex = 45;
            // 
            // frmImportPowerLevels
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(637, 109);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.udRevision);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnFile);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportPowerLevels";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Power Level Data Import";
            this.Load += new System.EventHandler(this.frmImportPowerLevels_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        Button btnClose;
        Button btnFile;
        Button btnImport;
        OpenFileDialog dlgBrowse;
        Label Label3;
        Label Label8;
        Label lblDate;
        Label lblFile;
        NumericUpDown udRevision;
    }
}
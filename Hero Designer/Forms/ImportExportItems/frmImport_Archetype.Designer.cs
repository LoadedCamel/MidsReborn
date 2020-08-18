using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImport_Archetype
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImport_Archetype));
            this.Label8 = new System.Windows.Forms.Label();
            this.lblATCount = new System.Windows.Forms.Label();
            this.lblATDate = new System.Windows.Forms.Label();
            this.udATRevision = new System.Windows.Forms.NumericUpDown();
            this.Label6 = new System.Windows.Forms.Label();
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            this.lblATFile = new System.Windows.Forms.Label();
            this.btnATFile = new System.Windows.Forms.Button();
            this.lstImport = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnImport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udATRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(346, 53);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(65, 18);
            this.Label8.TabIndex = 25;
            this.Label8.Text = "Revision:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblATCount
            // 
            this.lblATCount.Location = new System.Drawing.Point(208, 53);
            this.lblATCount.Name = "lblATCount";
            this.lblATCount.Size = new System.Drawing.Size(132, 18);
            this.lblATCount.TabIndex = 24;
            this.lblATCount.Text = "Classes:";
            // 
            // lblATDate
            // 
            this.lblATDate.Location = new System.Drawing.Point(9, 53);
            this.lblATDate.Name = "lblATDate";
            this.lblATDate.Size = new System.Drawing.Size(175, 18);
            this.lblATDate.TabIndex = 23;
            this.lblATDate.Text = "Date:";
            // 
            // udATRevision
            // 
            this.udATRevision.Location = new System.Drawing.Point(417, 51);
            this.udATRevision.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.udATRevision.Name = "udATRevision";
            this.udATRevision.Size = new System.Drawing.Size(116, 20);
            this.udATRevision.TabIndex = 22;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(12, 9);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(150, 14);
            this.Label6.TabIndex = 21;
            this.Label6.Text = "Class Definition File:";
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.DefaultExt = "csv";
            this.dlgBrowse.Filter = "CSV Spreadsheets (*.csv)|*.csv";
            // 
            // lblATFile
            // 
            this.lblATFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblATFile.Location = new System.Drawing.Point(12, 25);
            this.lblATFile.Name = "lblATFile";
            this.lblATFile.Size = new System.Drawing.Size(521, 23);
            this.lblATFile.TabIndex = 20;
            this.lblATFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnATFile
            // 
            this.btnATFile.Location = new System.Drawing.Point(539, 25);
            this.btnATFile.Name = "btnATFile";
            this.btnATFile.Size = new System.Drawing.Size(165, 23);
            this.btnATFile.TabIndex = 19;
            this.btnATFile.Text = "Load / Re-Load";
            this.btnATFile.UseVisualStyleBackColor = true;
            this.btnATFile.Click += new System.EventHandler(this.btnATFile_Click);
            // 
            // lstImport
            // 
            this.lstImport.CheckBoxes = true;
            this.lstImport.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2,
            this.ColumnHeader3,
            this.ColumnHeader4,
            this.ColumnHeader5,
            this.ColumnHeader6});
            this.lstImport.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstImport.HideSelection = false;
            this.lstImport.Location = new System.Drawing.Point(12, 77);
            this.lstImport.Name = "lstImport";
            this.lstImport.Size = new System.Drawing.Size(600, 309);
            this.lstImport.TabIndex = 26;
            this.lstImport.UseCompatibleStateImageBehavior = false;
            this.lstImport.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Class";
            this.ColumnHeader1.Width = 128;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "ClassID";
            this.ColumnHeader2.Width = 165;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Playable";
            this.ColumnHeader3.Width = 72;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Text = "New";
            this.ColumnHeader4.Width = 71;
            // 
            // ColumnHeader5
            // 
            this.ColumnHeader5.Text = "Modified";
            // 
            // ColumnHeader6
            // 
            this.ColumnHeader6.Text = "Change Description";
            this.ColumnHeader6.Width = 98;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(618, 77);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(86, 22);
            this.btnImport.TabIndex = 27;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(618, 363);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 23);
            this.btnClose.TabIndex = 28;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmImport_Archetype
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(711, 400);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lstImport);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.lblATCount);
            this.Controls.Add(this.lblATDate);
            this.Controls.Add(this.udATRevision);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.lblATFile);
            this.Controls.Add(this.btnATFile);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImport_Archetype";
            this.ShowInTaskbar = false;
            this.Text = "Archetype Class Import";
            ((System.ComponentModel.ISupportInitialize)(this.udATRevision)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        Button btnATFile;
        Button btnClose;
        Button btnImport;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ColumnHeader ColumnHeader6;
        OpenFileDialog dlgBrowse;
        Label Label6;
        Label Label8;
        Label lblATCount;
        Label lblATDate;
        Label lblATFile;
        ListView lstImport;
        NumericUpDown udATRevision;
    }
}
using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImport_Powerset
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImport_Powerset));
            this.Label8 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.udRevision = new System.Windows.Forms.NumericUpDown();
            this.Label6 = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            this.btnImport = new System.Windows.Forms.Button();
            this.lstImport = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(346, 53);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(65, 18);
            this.Label8.TabIndex = 32;
            this.Label8.Text = "Revision:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCount
            // 
            this.lblCount.Location = new System.Drawing.Point(208, 53);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(132, 18);
            this.lblCount.TabIndex = 31;
            this.lblCount.Text = "Sets:";
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(9, 53);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(175, 18);
            this.lblDate.TabIndex = 30;
            this.lblDate.Text = "Date:";
            // 
            // udRevision
            // 
            this.udRevision.Location = new System.Drawing.Point(417, 51);
            this.udRevision.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.udRevision.Name = "udRevision";
            this.udRevision.Size = new System.Drawing.Size(116, 20);
            this.udRevision.TabIndex = 29;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(12, 9);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(150, 14);
            this.Label6.TabIndex = 28;
            this.Label6.Text = "Set Definition File:";
            // 
            // lblFile
            // 
            this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFile.Location = new System.Drawing.Point(12, 25);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(521, 23);
            this.lblFile.TabIndex = 27;
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(539, 25);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(165, 23);
            this.btnFile.TabIndex = 26;
            this.btnFile.Text = "Load / Re-Load";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(618, 516);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 23);
            this.btnClose.TabIndex = 35;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.DefaultExt = "csv";
            this.dlgBrowse.Filter = "CSV Spreadsheets (*.csv)|*.csv";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(618, 77);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(86, 22);
            this.btnImport.TabIndex = 34;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lstImport
            // 
            this.lstImport.CheckBoxes = true;
            this.lstImport.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2,
            this.ColumnHeader4,
            this.ColumnHeader5,
            this.ColumnHeader3});
            this.lstImport.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstImport.HideSelection = false;
            this.lstImport.Location = new System.Drawing.Point(12, 77);
            this.lstImport.Name = "lstImport";
            this.lstImport.Size = new System.Drawing.Size(600, 462);
            this.lstImport.TabIndex = 33;
            this.lstImport.UseCompatibleStateImageBehavior = false;
            this.lstImport.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Powerset";
            this.ColumnHeader1.Width = 180;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Group";
            this.ColumnHeader2.Width = 176;
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
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Change Description";
            this.ColumnHeader3.Width = 106;
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(12, 545);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnCheckAll.TabIndex = 36;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Location = new System.Drawing.Point(93, 545);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnUncheckAll.TabIndex = 37;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // frmImport_Powerset
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(711, 574);
            this.Controls.Add(this.btnUncheckAll);
            this.Controls.Add(this.btnCheckAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lstImport);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.udRevision);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnFile);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImport_Powerset";
            this.ShowInTaskbar = false;
            this.Text = "Powerset Import";
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        Button btnCheckAll;
        Button btnClose;
        Button btnFile;
        Button btnImport;
        Button btnUncheckAll;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        OpenFileDialog dlgBrowse;
        Label Label6;
        Label Label8;
        Label lblCount;
        Label lblDate;
        Label lblFile;
        ListView lstImport;
        NumericUpDown udRevision;
    }
}
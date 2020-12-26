using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImport_Power
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImport_Power));
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.lstImport = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Label8 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.udRevision = new System.Windows.Forms.NumericUpDown();
            this.Label6 = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnCheckModified = new System.Windows.Forms.Button();
            this.btnEraseAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).BeginInit();
            this.SuspendLayout();
            // 
            // dlgBrowse
            // 
            this.dlgBrowse.DefaultExt = "csv";
            this.dlgBrowse.Filter = "CSV Spreadsheets (*.csv)|*.csv";
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Location = new System.Drawing.Point(93, 545);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnUncheckAll.TabIndex = 49;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(12, 545);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnCheckAll.TabIndex = 48;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(618, 516);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 23);
            this.btnClose.TabIndex = 47;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(618, 77);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(86, 22);
            this.btnImport.TabIndex = 46;
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
            this.lstImport.TabIndex = 45;
            this.lstImport.UseCompatibleStateImageBehavior = false;
            this.lstImport.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Power";
            this.ColumnHeader1.Width = 237;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Name";
            this.ColumnHeader2.Width = 117;
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
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(346, 53);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(65, 18);
            this.Label8.TabIndex = 44;
            this.Label8.Text = "Revision:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCount
            // 
            this.lblCount.Location = new System.Drawing.Point(208, 53);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(132, 18);
            this.lblCount.TabIndex = 43;
            this.lblCount.Text = "Sets:";
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(9, 53);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(175, 18);
            this.lblDate.TabIndex = 42;
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
            this.udRevision.TabIndex = 41;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(12, 9);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(150, 14);
            this.Label6.TabIndex = 40;
            this.Label6.Text = "Power Definition File:";
            // 
            // lblFile
            // 
            this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFile.Location = new System.Drawing.Point(12, 25);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(521, 23);
            this.lblFile.TabIndex = 39;
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(539, 25);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(165, 23);
            this.btnFile.TabIndex = 38;
            this.btnFile.Text = "Load / Re-Load";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnCheckModified
            // 
            this.btnCheckModified.Location = new System.Drawing.Point(192, 545);
            this.btnCheckModified.Name = "btnCheckModified";
            this.btnCheckModified.Size = new System.Drawing.Size(171, 23);
            this.btnCheckModified.TabIndex = 50;
            this.btnCheckModified.Text = "Modified Only (Skip New)";
            this.btnCheckModified.UseVisualStyleBackColor = true;
            this.btnCheckModified.Click += new System.EventHandler(this.btnCheckModified_Click);
            // 
            // btnEraseAll
            // 
            this.btnEraseAll.Location = new System.Drawing.Point(618, 137);
            this.btnEraseAll.Name = "btnEraseAll";
            this.btnEraseAll.Size = new System.Drawing.Size(86, 69);
            this.btnEraseAll.TabIndex = 63;
            this.btnEraseAll.Text = "Erase All Powers";
            this.btnEraseAll.UseVisualStyleBackColor = true;
            this.btnEraseAll.Click += new System.EventHandler(this.btnEraseAll_Click);
            // 
            // frmImport_Power
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(716, 575);
            this.Controls.Add(this.btnEraseAll);
            this.Controls.Add(this.btnCheckModified);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImport_Power";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Power Import";
            ((System.ComponentModel.ISupportInitialize)(this.udRevision)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        Button btnCheckAll;
        Button btnCheckModified;
        Button btnClose;
        Button btnEraseAll;
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
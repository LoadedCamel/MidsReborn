namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmDbDiff
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
            borderPanel1 = new Mids_Reborn.Forms.Controls.BorderPanel();
            listBox1 = new System.Windows.Forms.ListBox();
            borderPanel2 = new Mids_Reborn.Forms.Controls.BorderPanel();
            listView1 = new System.Windows.Forms.ListView();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            btnClose = new System.Windows.Forms.Button();
            btnSelectDb = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            btnOpRun = new System.Windows.Forms.Button();
            cbType = new System.Windows.Forms.ComboBox();
            cbOpMode = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            btnSelectExportDir = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            borderPanel1.SuspendLayout();
            borderPanel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.SystemColors.Highlight;
            borderPanel1.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(listBox1);
            borderPanel1.Location = new System.Drawing.Point(12, 50);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(279, 390);
            borderPanel1.TabIndex = 0;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(23, 17);
            listBox1.Name = "listBox1";
            listBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            listBox1.Size = new System.Drawing.Size(236, 349);
            listBox1.TabIndex = 0;
            listBox1.TabStop = false;
            listBox1.UseTabStops = false;
            // 
            // borderPanel2
            // 
            borderPanel2.Border.Color = System.Drawing.SystemColors.Highlight;
            borderPanel2.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            borderPanel2.Border.Thickness = 1;
            borderPanel2.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel2.Controls.Add(listView1);
            borderPanel2.Location = new System.Drawing.Point(297, 50);
            borderPanel2.Name = "borderPanel2";
            borderPanel2.Size = new System.Drawing.Size(848, 390);
            borderPanel2.TabIndex = 1;
            // 
            // listView1
            // 
            listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader5, columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listView1.HideSelection = true;
            listView1.Location = new System.Drawing.Point(18, 17);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(811, 349);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            listView1.VirtualMode = true;
            listView1.RetrieveVirtualItem += listView1_RetrieveVirtualItem;
            listView1.MouseClick += listView1_MouseClick;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Mod";
            columnHeader5.Width = 40;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Type";
            columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Display Name";
            columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "AT/Group";
            columnHeader3.Width = 210;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Internal Name";
            columnHeader4.Width = 294;
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(1069, 448);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(76, 23);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnSelectDb
            // 
            btnSelectDb.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnSelectDb.Location = new System.Drawing.Point(12, 12);
            btnSelectDb.Name = "btnSelectDb";
            btnSelectDb.Size = new System.Drawing.Size(224, 23);
            btnSelectDb.TabIndex = 7;
            btnSelectDb.Text = "Select secondary database...";
            btnSelectDb.UseVisualStyleBackColor = true;
            btnSelectDb.Click += btnSelectDb_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(btnOpRun);
            panel1.Controls.Add(cbType);
            panel1.Controls.Add(cbOpMode);
            panel1.Controls.Add(label1);
            panel1.Location = new System.Drawing.Point(12, 448);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1051, 23);
            panel1.TabIndex = 8;
            panel1.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(588, 0);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(451, 23);
            progressBar1.TabIndex = 17;
            progressBar1.Visible = false;
            // 
            // btnOpRun
            // 
            btnOpRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnOpRun.Location = new System.Drawing.Point(480, 0);
            btnOpRun.Name = "btnOpRun";
            btnOpRun.Size = new System.Drawing.Size(76, 23);
            btnOpRun.TabIndex = 16;
            btnOpRun.Text = "Run";
            btnOpRun.UseVisualStyleBackColor = true;
            btnOpRun.Click += btnOpRun_Click;
            // 
            // cbType
            // 
            cbType.FormattingEnabled = true;
            cbType.Location = new System.Drawing.Point(278, 0);
            cbType.Name = "cbType";
            cbType.Size = new System.Drawing.Size(179, 23);
            cbType.TabIndex = 15;
            // 
            // cbOpMode
            // 
            cbOpMode.FormattingEnabled = true;
            cbOpMode.Items.AddRange(new object[] { "Export to JSON", "Import to current DB" });
            cbOpMode.Location = new System.Drawing.Point(80, 0);
            cbOpMode.Name = "cbOpMode";
            cbOpMode.Size = new System.Drawing.Size(179, 23);
            cbOpMode.TabIndex = 14;
            cbOpMode.SelectedIndexChanged += cbOpMode_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(3, 3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 15);
            label1.TabIndex = 13;
            label1.Text = "Operation:";
            // 
            // btnSelectExportDir
            // 
            btnSelectExportDir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnSelectExportDir.Location = new System.Drawing.Point(245, 12);
            btnSelectExportDir.Name = "btnSelectExportDir";
            btnSelectExportDir.Size = new System.Drawing.Size(224, 23);
            btnSelectExportDir.TabIndex = 9;
            btnSelectExportDir.Text = "Select export directory";
            btnSelectExportDir.UseVisualStyleBackColor = true;
            btnSelectExportDir.Click += btnSelectExportDir_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(492, 16);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(58, 15);
            label2.TabIndex = 10;
            label2.Text = "Export to:";
            label2.Visible = false;
            // 
            // frmDbDiff
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1157, 482);
            Controls.Add(label2);
            Controls.Add(btnSelectExportDir);
            Controls.Add(panel1);
            Controls.Add(btnSelectDb);
            Controls.Add(btnClose);
            Controls.Add(borderPanel2);
            Controls.Add(borderPanel1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "frmDbDiff";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "frmDbDiff";
            Load += frmDbDiff_Load;
            borderPanel1.ResumeLayout(false);
            borderPanel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.BorderPanel borderPanel1;
        private Controls.BorderPanel borderPanel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSelectDb;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnOpRun;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.ComboBox cbOpMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectExportDir;
        private System.Windows.Forms.Label label2;
    }
}
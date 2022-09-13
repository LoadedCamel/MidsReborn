namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmDbQueries
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbStaticIndex = new System.Windows.Forms.TextBox();
            this.tbPowerName = new System.Windows.Forms.TextBox();
            this.btnSearchByIndex = new System.Windows.Forms.Button();
            this.btnSearchByName = new System.Windows.Forms.Button();
            this.btnFirstAvailableIndex = new System.Windows.Forms.Button();
            this.btnHighestAvailableIndex = new System.Windows.Forms.Button();
            this.btnAllAvailableIndices = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnListIndices = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Power Static Index:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Power Display Name:";
            // 
            // tbStaticIndex
            // 
            this.tbStaticIndex.Location = new System.Drawing.Point(166, 22);
            this.tbStaticIndex.Name = "tbStaticIndex";
            this.tbStaticIndex.Size = new System.Drawing.Size(352, 23);
            this.tbStaticIndex.TabIndex = 2;
            this.tbStaticIndex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbStaticIndex_KeyDown);
            // 
            // tbPowerName
            // 
            this.tbPowerName.Location = new System.Drawing.Point(166, 68);
            this.tbPowerName.Name = "tbPowerName";
            this.tbPowerName.Size = new System.Drawing.Size(352, 23);
            this.tbPowerName.TabIndex = 4;
            this.tbPowerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPowerName_KeyDown);
            // 
            // btnSearchByIndex
            // 
            this.btnSearchByIndex.Location = new System.Drawing.Point(524, 19);
            this.btnSearchByIndex.Name = "btnSearchByIndex";
            this.btnSearchByIndex.Size = new System.Drawing.Size(75, 22);
            this.btnSearchByIndex.TabIndex = 3;
            this.btnSearchByIndex.Text = "Search";
            this.btnSearchByIndex.UseVisualStyleBackColor = true;
            this.btnSearchByIndex.Click += new System.EventHandler(this.btnSearchByIndex_Click);
            // 
            // btnSearchByName
            // 
            this.btnSearchByName.Location = new System.Drawing.Point(524, 68);
            this.btnSearchByName.Name = "btnSearchByName";
            this.btnSearchByName.Size = new System.Drawing.Size(75, 22);
            this.btnSearchByName.TabIndex = 5;
            this.btnSearchByName.Text = "Search";
            this.btnSearchByName.UseVisualStyleBackColor = true;
            this.btnSearchByName.Click += new System.EventHandler(this.btnSearchByName_Click);
            // 
            // btnFirstAvailableIndex
            // 
            this.btnFirstAvailableIndex.Location = new System.Drawing.Point(35, 104);
            this.btnFirstAvailableIndex.Name = "btnFirstAvailableIndex";
            this.btnFirstAvailableIndex.Size = new System.Drawing.Size(162, 28);
            this.btnFirstAvailableIndex.TabIndex = 6;
            this.btnFirstAvailableIndex.Text = "First Available Index";
            this.btnFirstAvailableIndex.UseVisualStyleBackColor = true;
            this.btnFirstAvailableIndex.Click += new System.EventHandler(this.btnFirstAvailableIndex_Click);
            // 
            // btnHighestAvailableIndex
            // 
            this.btnHighestAvailableIndex.Location = new System.Drawing.Point(234, 104);
            this.btnHighestAvailableIndex.Name = "btnHighestAvailableIndex";
            this.btnHighestAvailableIndex.Size = new System.Drawing.Size(162, 28);
            this.btnHighestAvailableIndex.TabIndex = 7;
            this.btnHighestAvailableIndex.Text = "Highest Available Index";
            this.btnHighestAvailableIndex.UseVisualStyleBackColor = true;
            this.btnHighestAvailableIndex.Click += new System.EventHandler(this.btnHighestAvailableIndex_Click);
            // 
            // btnAllAvailableIndices
            // 
            this.btnAllAvailableIndices.Location = new System.Drawing.Point(432, 104);
            this.btnAllAvailableIndices.Name = "btnAllAvailableIndices";
            this.btnAllAvailableIndices.Size = new System.Drawing.Size(162, 28);
            this.btnAllAvailableIndices.TabIndex = 8;
            this.btnAllAvailableIndices.Text = "All Available Indices";
            this.btnAllAvailableIndices.UseVisualStyleBackColor = true;
            this.btnAllAvailableIndices.Click += new System.EventHandler(this.btnAllAvailableIndices_Click);
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(12, 178);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(607, 181);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.VirtualMode = true;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Static Index";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Power Name";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Power Full Name";
            this.columnHeader3.Width = 280;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(132, 376);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(162, 28);
            this.btnCopy.TabIndex = 10;
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(338, 376);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(162, 28);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnListIndices
            // 
            this.btnListIndices.Location = new System.Drawing.Point(234, 144);
            this.btnListIndices.Name = "btnListIndices";
            this.btnListIndices.Size = new System.Drawing.Size(162, 28);
            this.btnListIndices.TabIndex = 12;
            this.btnListIndices.Text = "List StaticIndex table";
            this.btnListIndices.UseVisualStyleBackColor = true;
            this.btnListIndices.Click += new System.EventHandler(this.btnListIndices_Click);
            // 
            // frmDbQueries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 422);
            this.Controls.Add(this.btnListIndices);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnAllAvailableIndices);
            this.Controls.Add(this.btnHighestAvailableIndex);
            this.Controls.Add(this.btnFirstAvailableIndex);
            this.Controls.Add(this.btnSearchByName);
            this.Controls.Add(this.btnSearchByIndex);
            this.Controls.Add(this.tbPowerName);
            this.Controls.Add(this.tbStaticIndex);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmDbQueries";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DB Queries";
            this.Load += new System.EventHandler(this.frmDbQueries_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbStaticIndex;
        private System.Windows.Forms.TextBox tbPowerName;
        private System.Windows.Forms.Button btnSearchByIndex;
        private System.Windows.Forms.Button btnSearchByName;
        private System.Windows.Forms.Button btnFirstAvailableIndex;
        private System.Windows.Forms.Button btnHighestAvailableIndex;
        private System.Windows.Forms.Button btnAllAvailableIndices;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnListIndices;
    }
}
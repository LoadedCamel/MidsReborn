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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            tbStaticIndex = new System.Windows.Forms.TextBox();
            tbPowerName = new System.Windows.Forms.TextBox();
            btnSearchByIndex = new System.Windows.Forms.Button();
            btnSearchByName = new System.Windows.Forms.Button();
            listView1 = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            btnCopy = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            cbSpecialFilter = new System.Windows.Forms.ComboBox();
            btnSpecFilterSearch = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(27, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 15);
            label1.TabIndex = 0;
            label1.Text = "Power Static Index:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(27, 71);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(119, 15);
            label2.TabIndex = 1;
            label2.Text = "Power Display Name:";
            // 
            // tbStaticIndex
            // 
            tbStaticIndex.Location = new System.Drawing.Point(166, 22);
            tbStaticIndex.Name = "tbStaticIndex";
            tbStaticIndex.Size = new System.Drawing.Size(352, 23);
            tbStaticIndex.TabIndex = 2;
            tbStaticIndex.KeyDown += tbStaticIndex_KeyDown;
            // 
            // tbPowerName
            // 
            tbPowerName.Location = new System.Drawing.Point(166, 68);
            tbPowerName.Name = "tbPowerName";
            tbPowerName.Size = new System.Drawing.Size(352, 23);
            tbPowerName.TabIndex = 4;
            tbPowerName.KeyDown += tbPowerName_KeyDown;
            // 
            // btnSearchByIndex
            // 
            btnSearchByIndex.Location = new System.Drawing.Point(524, 19);
            btnSearchByIndex.Name = "btnSearchByIndex";
            btnSearchByIndex.Size = new System.Drawing.Size(75, 22);
            btnSearchByIndex.TabIndex = 3;
            btnSearchByIndex.Text = "Search";
            btnSearchByIndex.UseVisualStyleBackColor = true;
            btnSearchByIndex.Click += btnSearchByIndex_Click;
            // 
            // btnSearchByName
            // 
            btnSearchByName.Location = new System.Drawing.Point(524, 68);
            btnSearchByName.Name = "btnSearchByName";
            btnSearchByName.Size = new System.Drawing.Size(75, 22);
            btnSearchByName.TabIndex = 5;
            btnSearchByName.Text = "Search";
            btnSearchByName.UseVisualStyleBackColor = true;
            btnSearchByName.Click += btnSearchByName_Click;
            // 
            // listView1
            // 
            listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listView1.FullRowSelect = true;
            listView1.Location = new System.Drawing.Point(12, 178);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(607, 181);
            listView1.TabIndex = 9;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            listView1.VirtualMode = true;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.RetrieveVirtualItem += listView1_RetrieveVirtualItem;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Static Index";
            columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Power Name";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Power Full Name";
            columnHeader3.Width = 280;
            // 
            // btnCopy
            // 
            btnCopy.Location = new System.Drawing.Point(132, 376);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new System.Drawing.Size(162, 28);
            btnCopy.TabIndex = 10;
            btnCopy.Text = "Copy to Clipboard";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(338, 376);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(162, 28);
            btnClose.TabIndex = 11;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // cbSpecialFilter
            // 
            cbSpecialFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSpecialFilter.FormattingEnabled = true;
            cbSpecialFilter.Items.AddRange(new object[] { "First Available Index", "Highest Available Index", "All Available Indices", "List Indices", "Check Orphan Entries", "Duplicate Indices", "Bogus MaxRunSpeed effect", "Powers with Absorbed Entity" });
            cbSpecialFilter.Location = new System.Drawing.Point(230, 122);
            cbSpecialFilter.Name = "cbSpecialFilter";
            cbSpecialFilter.Size = new System.Drawing.Size(220, 23);
            cbSpecialFilter.TabIndex = 12;
            // 
            // btnSpecFilterSearch
            // 
            btnSpecFilterSearch.Location = new System.Drawing.Point(524, 121);
            btnSpecFilterSearch.Name = "btnSpecFilterSearch";
            btnSpecFilterSearch.Size = new System.Drawing.Size(75, 22);
            btnSpecFilterSearch.TabIndex = 13;
            btnSpecFilterSearch.Text = "Search";
            btnSpecFilterSearch.UseVisualStyleBackColor = true;
            btnSpecFilterSearch.Click += btnSpecFilterSearch_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(27, 125);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(76, 15);
            label3.TabIndex = 14;
            label3.Text = "Special Filter:";
            // 
            // frmDbQueries
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(631, 422);
            Controls.Add(label3);
            Controls.Add(btnSpecFilterSearch);
            Controls.Add(cbSpecialFilter);
            Controls.Add(btnClose);
            Controls.Add(btnCopy);
            Controls.Add(listView1);
            Controls.Add(btnSearchByName);
            Controls.Add(btnSearchByIndex);
            Controls.Add(tbPowerName);
            Controls.Add(tbStaticIndex);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "frmDbQueries";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "DB Queries";
            Load += frmDbQueries_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbStaticIndex;
        private System.Windows.Forms.TextBox tbPowerName;
        private System.Windows.Forms.Button btnSearchByIndex;
        private System.Windows.Forms.Button btnSearchByName;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cbSpecialFilter;
        private System.Windows.Forms.Button btnSpecFilterSearch;
        private System.Windows.Forms.Label label3;
    }
}
namespace Mids_Reborn.UI.Forms
{
    partial class frmCustomGraphsSelector
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
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnAdd = new System.Windows.Forms.Button();
            btnRemove = new System.Windows.Forms.Button();
            btnUp = new System.Windows.Forms.Button();
            btnDown = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            lbAvailableStats = new System.Windows.Forms.ListBox();
            lbActiveStats = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(49, 38);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(153, 15);
            label1.TabIndex = 0;
            label1.Text = "Select up to 8 items to show.";
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(659, 405);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.TabIndex = 3;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(762, 405);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(393, 171);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(40, 23);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "-->";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRemove
            // 
            btnRemove.Location = new System.Drawing.Point(393, 241);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new System.Drawing.Size(40, 23);
            btnRemove.TabIndex = 6;
            btnRemove.Text = "<--";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnUp
            // 
            btnUp.Location = new System.Drawing.Point(797, 171);
            btnUp.Name = "btnUp";
            btnUp.Size = new System.Drawing.Size(40, 23);
            btnUp.TabIndex = 7;
            btnUp.Text = "^";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // btnDown
            // 
            btnDown.Location = new System.Drawing.Point(797, 241);
            btnDown.Name = "btnDown";
            btnDown.Size = new System.Drawing.Size(40, 23);
            btnDown.TabIndex = 8;
            btnDown.Text = "v";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(461, 38);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(86, 15);
            label2.TabIndex = 9;
            label2.Text = "Selected items:";
            // 
            // lbAvailableStats
            // 
            lbAvailableStats.BackColor = System.Drawing.SystemColors.ControlDark;
            lbAvailableStats.FormattingEnabled = true;
            lbAvailableStats.ItemHeight = 15;
            lbAvailableStats.Location = new System.Drawing.Point(34, 69);
            lbAvailableStats.Name = "lbAvailableStats";
            lbAvailableStats.Size = new System.Drawing.Size(332, 304);
            lbAvailableStats.TabIndex = 10;
            lbAvailableStats.DoubleClick += lbAvailableStats_DoubleClick;
            // 
            // lbActiveStats
            // 
            lbActiveStats.BackColor = System.Drawing.SystemColors.ControlDark;
            lbActiveStats.FormattingEnabled = true;
            lbActiveStats.ItemHeight = 15;
            lbActiveStats.Location = new System.Drawing.Point(461, 69);
            lbActiveStats.Name = "lbActiveStats";
            lbActiveStats.Size = new System.Drawing.Size(309, 304);
            lbActiveStats.TabIndex = 11;
            lbActiveStats.SelectedIndexChanged += lbActiveStats_SelectedIndexChanged;
            lbActiveStats.DoubleClick += lbActiveStats_DoubleClick;
            // 
            // frmCustomGraphsSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(862, 450);
            Controls.Add(lbActiveStats);
            Controls.Add(lbAvailableStats);
            Controls.Add(label2);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(btnRemove);
            Controls.Add(btnAdd);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(label1);
            DoubleBuffered = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCustomGraphsSelector";
            ShowInTaskbar = false;
            Text = "Select custom graphs";
            Load += frmCustomGraphsSelector_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbAvailableStats;
        private System.Windows.Forms.ListBox lbActiveStats;
    }
}
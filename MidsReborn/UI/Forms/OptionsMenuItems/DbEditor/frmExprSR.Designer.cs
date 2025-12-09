namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    partial class frmExprSR
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
            searchPattern = new System.Windows.Forms.TextBox();
            replacePattern = new System.Windows.Forms.TextBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            chkIncludeConditionals = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(21, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(63, 15);
            label1.TabIndex = 0;
            label1.Text = "Search for:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(20, 79);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 15);
            label2.TabIndex = 1;
            label2.Text = "Replace with:";
            // 
            // searchPattern
            // 
            searchPattern.Location = new System.Drawing.Point(21, 38);
            searchPattern.Name = "searchPattern";
            searchPattern.Size = new System.Drawing.Size(287, 23);
            searchPattern.TabIndex = 2;
            searchPattern.TextChanged += searchPattern_TextChanged;
            // 
            // replacePattern
            // 
            replacePattern.Location = new System.Drawing.Point(23, 97);
            replacePattern.Name = "replacePattern";
            replacePattern.Size = new System.Drawing.Size(287, 23);
            replacePattern.TabIndex = 3;
            replacePattern.TextChanged += replacePattern_TextChanged;
            // 
            // btnOk
            // 
            btnOk.Enabled = false;
            btnOk.Location = new System.Drawing.Point(84, 169);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.TabIndex = 5;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(174, 169);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // chkIncludeConditionals
            // 
            chkIncludeConditionals.AutoSize = true;
            chkIncludeConditionals.Location = new System.Drawing.Point(23, 135);
            chkIncludeConditionals.Name = "chkIncludeConditionals";
            chkIncludeConditionals.Size = new System.Drawing.Size(167, 19);
            chkIncludeConditionals.TabIndex = 4;
            chkIncludeConditionals.Text = "Also look into conditionals";
            chkIncludeConditionals.UseVisualStyleBackColor = true;
            chkIncludeConditionals.CheckedChanged += chkIncludeConditionals_CheckedChanged;
            // 
            // frmExprSR
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(333, 208);
            Controls.Add(chkIncludeConditionals);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(replacePattern);
            Controls.Add(searchPattern);
            Controls.Add(label2);
            Controls.Add(label1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmExprSR";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Expressions search/replace";
            Load += frmExprSR_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox searchPattern;
        private System.Windows.Forms.TextBox replacePattern;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkIncludeConditionals;
    }
}
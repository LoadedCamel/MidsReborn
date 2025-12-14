namespace Mids_Reborn.Forms
{
    partial class frmTemplateManage
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
            panel1 = new System.Windows.Forms.Panel();
            btnClose = new System.Windows.Forms.Button();
            panel2 = new System.Windows.Forms.Panel();
            btnBrowse = new System.Windows.Forms.Button();
            txtBuildName = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            radioButton2 = new System.Windows.Forms.RadioButton();
            label2 = new System.Windows.Forms.Label();
            radioButton1 = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Control;
            panel1.Controls.Add(btnClose);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 189);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(559, 42);
            panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(242, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 3;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnBrowse);
            panel2.Controls.Add(txtBuildName);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(radioButton2);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(radioButton1);
            panel2.Controls.Add(label1);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(559, 189);
            panel2.TabIndex = 2;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(434, 120);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(89, 23);
            btnBrowse.TabIndex = 7;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtBuildName
            // 
            txtBuildName.Location = new System.Drawing.Point(150, 119);
            txtBuildName.Name = "txtBuildName";
            txtBuildName.ReadOnly = true;
            txtBuildName.Size = new System.Drawing.Size(278, 23);
            txtBuildName.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(48, 162);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(348, 15);
            label4.TabIndex = 5;
            label4.Text = "This includes everything but primary, secondary and epic powers.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(48, 144);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(288, 15);
            label3.TabIndex = 4;
            label3.Text = "New builds will have powers from this build imported.";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new System.Drawing.Point(26, 120);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(121, 19);
            radioButton2.TabIndex = 3;
            radioButton2.TabStop = true;
            radioButton2.Text = "Use existing build:";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(48, 88);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(153, 15);
            label2.TabIndex = 2;
            label2.Text = "New builds will come blank.";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new System.Drawing.Point(26, 67);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(54, 19);
            radioButton1.TabIndex = 1;
            radioButton1.TabStop = true;
            radioButton1.Text = "None";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(26, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(167, 15);
            label1.TabIndex = 0;
            label1.Text = "Use template for new builds:";
            // 
            // frmTemplateManage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(559, 231);
            Controls.Add(panel2);
            Controls.Add(panel1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmTemplateManage";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Select Template";
            Load += frmTemplateManage_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBuildName;
        private System.Windows.Forms.Button btnBrowse;
    }
}
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    partial class frmEditComment
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
            label1 = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            label1.Location = new System.Drawing.Point(22, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(259, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter your build comment/description below:";
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(438, 262);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 22);
            btnOk.TabIndex = 3;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(541, 262);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 22);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.SystemColors.Window;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBox1.ForeColor = System.Drawing.Color.Black;
            textBox1.Location = new System.Drawing.Point(22, 27);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new System.Drawing.Size(594, 229);
            textBox1.TabIndex = 5;
            // 
            // frmEditComment
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(639, 296);
            Controls.Add(textBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(label1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmEditComment";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Build Notes";
            Load += frmEditComment_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox textBox1;
    }
}
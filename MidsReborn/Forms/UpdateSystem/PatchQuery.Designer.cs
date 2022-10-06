
namespace Mids_Reborn.Forms.UpdateSystem
{
    partial class PatchQuery
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
            this.appButton = new System.Windows.Forms.Button();
            this.dbButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Which type of patch notes do you want to read?";
            // 
            // appButton
            // 
            this.appButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.appButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.appButton.Location = new System.Drawing.Point(21, 19);
            this.appButton.Name = "appButton";
            this.appButton.Size = new System.Drawing.Size(65, 23);
            this.appButton.TabIndex = 2;
            this.appButton.Text = "App";
            this.appButton.UseVisualStyleBackColor = true;
            // 
            // dbButton
            // 
            this.dbButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.dbButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dbButton.Location = new System.Drawing.Point(107, 19);
            this.dbButton.Name = "dbButton";
            this.dbButton.Size = new System.Drawing.Size(65, 23);
            this.dbButton.TabIndex = 3;
            this.dbButton.Text = "Database";
            this.dbButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(192, 19);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(65, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cancelButton);
            this.groupBox1.Controls.Add(this.appButton);
            this.groupBox1.Controls.Add(this.dbButton);
            this.groupBox1.Location = new System.Drawing.Point(-9, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // PatchQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(260, 98);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PatchQuery";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "QueryBox";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button appButton;
        private System.Windows.Forms.Button dbButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
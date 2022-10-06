
namespace Mids_Reborn.Forms.UpdateSystem
{
    partial class UpdateQuery
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
            this.queryLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.justupdateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // queryLabel
            // 
            this.queryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryLabel.ForeColor = System.Drawing.Color.Black;
            this.queryLabel.Location = new System.Drawing.Point(12, 9);
            this.queryLabel.Name = "queryLabel";
            this.queryLabel.Size = new System.Drawing.Size(334, 20);
            this.queryLabel.TabIndex = 0;
            this.queryLabel.Text = "There is a new {type} update available.\r\n";
            this.queryLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Do you want to download and view the patch notes?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // yesButton
            // 
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.yesButton.Location = new System.Drawing.Point(12, 58);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 2;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // noButton
            // 
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.noButton.Location = new System.Drawing.Point(143, 58);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 3;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // justupdateButton
            // 
            this.justupdateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.justupdateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.justupdateButton.Location = new System.Drawing.Point(271, 58);
            this.justupdateButton.Name = "justupdateButton";
            this.justupdateButton.Size = new System.Drawing.Size(75, 23);
            this.justupdateButton.TabIndex = 4;
            this.justupdateButton.Text = "Just Update";
            this.justupdateButton.UseVisualStyleBackColor = true;
            // 
            // UpdateQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(358, 93);
            this.Controls.Add(this.justupdateButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.queryLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateQuery";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "QueryBox";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label queryLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Button justupdateButton;
    }
}
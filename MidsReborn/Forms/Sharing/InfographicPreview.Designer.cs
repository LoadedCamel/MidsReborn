namespace Mids_Reborn.Forms.Sharing
{
    partial class InfographicPreview
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
            pictureBox1 = new System.Windows.Forms.PictureBox();
            cancelButton = new System.Windows.Forms.Button();
            continueButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(450, 250);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cancelButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            cancelButton.Location = new System.Drawing.Point(12, 268);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(130, 39);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "CANCEL";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // continueButton
            // 
            continueButton.DialogResult = System.Windows.Forms.DialogResult.Continue;
            continueButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            continueButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            continueButton.Location = new System.Drawing.Point(332, 268);
            continueButton.Name = "continueButton";
            continueButton.Size = new System.Drawing.Size(130, 39);
            continueButton.TabIndex = 2;
            continueButton.Text = "CONTINUE";
            continueButton.UseVisualStyleBackColor = true;
            // 
            // InfographicPreview
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(474, 319);
            Controls.Add(continueButton);
            Controls.Add(cancelButton);
            Controls.Add(pictureBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "InfographicPreview";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Preview";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button continueButton;
    }
}
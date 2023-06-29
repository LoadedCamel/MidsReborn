using MRBUpdater.Controls;

namespace MRBUpdater
{
    partial class Update
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            lblStatus = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            lblFileName = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            progressBar = new ProgressBarEx();
            panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatus.Location = new System.Drawing.Point(108, 28);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(356, 36);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Downloading Update...";
            lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (System.Drawing.Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox1.Location = new System.Drawing.Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(90, 68);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // lblFileName
            // 
            lblFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblFileName.Location = new System.Drawing.Point(108, 64);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new System.Drawing.Size(334, 19);
            lblFileName.TabIndex = 3;
            lblFileName.Text = "AttribMods.mhd";
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(108, 4);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(356, 24);
            label3.TabIndex = 4;
            label3.Text = "Mids will automatically restart when update is complete.";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressBar
            // 
            progressBar.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            progressBar.Border.Thickness = 2;
            progressBar.Border.Which = ProgressBarEx.ProgressBorder.BorderToDraw.All;
            progressBar.Colors.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            progressBar.Colors.BarEndColor = System.Drawing.Color.FromArgb(64, 78, 237);
            progressBar.Colors.BarStartColor = System.Drawing.Color.FromArgb(30, 144, 255);
            progressBar.Colors.BorderColor = System.Drawing.Color.WhiteSmoke;
            progressBar.Colors.TextColor = System.Drawing.Color.WhiteSmoke;
            progressBar.Location = new System.Drawing.Point(12, 86);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(452, 20);
            progressBar.Step = 1;
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(476, 122);
            panel1.TabIndex = 5;
            // 
            // Update
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(476, 122);
            Controls.Add(label3);
            Controls.Add(lblFileName);
            Controls.Add(pictureBox1);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(panel1);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Update";
            Text = "Update";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ProgressBarEx progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}
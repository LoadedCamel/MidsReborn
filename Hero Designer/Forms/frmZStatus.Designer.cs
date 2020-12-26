using System.ComponentModel;

namespace Mids_Reborn.Forms
{
    public partial class frmZStatus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
            this.SuspendLayout();
            this.lblTitle.BackColor = System.Drawing.Color.Black;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);

            this.lblTitle.Location = new System.Drawing.Point(18, 33);
            this.lblTitle.Name = "lblTitle";

            this.lblTitle.Size = new System.Drawing.Size(480, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus1.BackColor = System.Drawing.Color.Black;
            this.lblStatus1.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);

            this.lblStatus1.Location = new System.Drawing.Point(20, 35);
            this.lblStatus1.Name = "lblStatus1";

            this.lblStatus1.Size = new System.Drawing.Size(476, 16);
            this.lblStatus1.TabIndex = 1;
            this.lblStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus2.BackColor = System.Drawing.Color.Black;
            this.lblStatus2.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);

            this.lblStatus2.Location = new System.Drawing.Point(20, 52);
            this.lblStatus2.Name = "lblStatus2";

            this.lblStatus2.Size = new System.Drawing.Size(476, 16);
            this.lblStatus2.TabIndex = 2;
            this.lblStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.PictureBox1.Location = new System.Drawing.Point(1, 1);
            this.PictureBox1.Name = "PictureBox1";

            this.PictureBox1.Size = new System.Drawing.Size(515, 75);
            this.PictureBox1.TabIndex = 3;
            this.PictureBox1.TabStop = false;

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(517, 77);
            this.Controls.Add((System.Windows.Forms.Control)this.lblStatus2);
            this.Controls.Add((System.Windows.Forms.Control)this.lblStatus1);
            this.Controls.Add((System.Windows.Forms.Control)this.lblTitle);
            this.Controls.Add((System.Windows.Forms.Control)this.PictureBox1);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Status";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
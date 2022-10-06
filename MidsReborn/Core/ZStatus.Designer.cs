using System.ComponentModel;
using System.Windows.Forms;
using MRBResourceLib;

namespace Mids_Reborn.Core
{
    public partial class ZStatus : Form
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

            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(ZStatus));
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.lblStatus2.Location = new System.Drawing.Point(8, 50);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(472, 16);
            this.lblStatus2.TabIndex = 5;
            this.lblStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus1.Location = new System.Drawing.Point(8, 30);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(472, 16);
            this.lblStatus1.TabIndex = 4;
            this.lblStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.lblTitle.Location = new System.Drawing.Point(8, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(472, 20);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Please Wait...";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 14f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.ClientSize = new System.Drawing.Size(488, 72);
            this.Controls.Add((System.Windows.Forms.Control)this.lblStatus2);
            this.Controls.Add((System.Windows.Forms.Control)this.lblStatus1);
            this.Controls.Add((System.Windows.Forms.Control)this.lblTitle);
            this.Font = new System.Drawing.Font("Arial", 8.25f);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle= System.Windows.Forms.FormBorderStyle.None;
            this.Icon = Resources.MRB_Icon_Concept;
            this.Name = nameof(ZStatus);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Status";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        #endregion
    }
}
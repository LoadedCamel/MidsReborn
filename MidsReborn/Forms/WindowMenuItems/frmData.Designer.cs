using System.ComponentModel;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmData
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
            this.pInfo = new ctlPopUp();
            this.SuspendLayout();
            this.pInfo.BXHeight = 2048;
            this.pInfo.ColumnPosition = 0.5f;
            this.pInfo.ColumnRight = false;
            this.pInfo.Font = new System.Drawing.Font("Segoe UI", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.pInfo.InternalPadding = 3;
            this.pInfo.Location = new System.Drawing.Point(0, 0);
            this.pInfo.Name = "pInfo";
            this.pInfo.SectionPadding = 8;

            this.pInfo.Size = new System.Drawing.Size(566, 230);
            this.pInfo.TabIndex = 0;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;

            this.ClientSize = new System.Drawing.Size(567, 299);
            this.Controls.Add((System.Windows.Forms.Control)this.pInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Power Data";
            this.TopMost = true;
            this.ResumeLayout(false);
        }
        #endregion
    }

}
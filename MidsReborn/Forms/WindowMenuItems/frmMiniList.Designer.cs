using System.ComponentModel;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmMiniList
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

            this.PInfo = new ctlPopUp();
            this.VScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            this.PInfo.BXHeight = 2048;
            this.PInfo.ColumnPosition = 0.5f;
            this.PInfo.ColumnRight = true;
            this.PInfo.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.PInfo.InternalPadding = 3;

            this.PInfo.Location = new System.Drawing.Point(0, 0);
            this.PInfo.Name = "PInfo";
            this.PInfo.ScrollY = 0.0f;
            this.PInfo.SectionPadding = 8;

            this.PInfo.Size = new System.Drawing.Size(230, 227);
            this.PInfo.TabIndex = 0;

            this.VScrollBar1.Location = new System.Drawing.Point(233, 0);
            this.VScrollBar1.Name = "VScrollBar1";

            this.VScrollBar1.Size = new System.Drawing.Size(17, 284);
            this.VScrollBar1.TabIndex = 1;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;

            this.ClientSize = new System.Drawing.Size(249, 284);
            this.Controls.Add((System.Windows.Forms.Control)this.VScrollBar1);
            this.Controls.Add((System.Windows.Forms.Control)this.PInfo);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

            this.MaximumSize = new System.Drawing.Size(600, 600);

            this.MinimumSize = new System.Drawing.Size(100, 150);
            this.Text = "Mini List";
            this.TopMost = true;


            this.ResumeLayout(false);
        }
        #endregion

    }
}
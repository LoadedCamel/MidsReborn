using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmBusy
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

            this.Message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.Message.Location = new System.Drawing.Point(12, 9);
            this.Message.Name = "Message";

            this.Message.Size = new System.Drawing.Size(381, 61);
            this.Message.TabIndex = 0;
            this.Message.Text = "Busy";
            this.Message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            this.ClientSize = new System.Drawing.Size(405, 79);
            this.ControlBox = false;
            this.Controls.Add(this.Message);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Working...";
            this.Name = "frmBusy";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        #endregion

        private Label Message;
    }
}
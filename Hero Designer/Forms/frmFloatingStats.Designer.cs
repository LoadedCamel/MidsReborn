using System.ComponentModel;

namespace Hero_Designer
{
    public partial class frmFloatingStats
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

            this.dvFloat = new DataView();
            this.SuspendLayout();
            this.dvFloat.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.dvFloat.DrawVillain = false;
            this.dvFloat.Floating = true;
            this.dvFloat.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.dvFloat.Location = new System.Drawing.Point(0, 0);
            this.dvFloat.Name = "dvFloat";

            this.dvFloat.Size = new System.Drawing.Size(300, 348);
            this.dvFloat.TabIndex = 0;
            this.dvFloat.VisibleSize = Enums.eVisibleSize.Full;
            this.dvFloat.Load += new System.EventHandler(this.dvFloat_Load);
            this.dvFloat.SizeChange += this.dvFloat_SizeChange;
            this.dvFloat.FloatChange += this.dvFloat_FloatChanged;
            this.dvFloat.Unlock_Click += new DataView.Unlock_ClickEventHandler(this.dvFloat_Unlock);
            this.dvFloat.TabChanged += this.dvFloat_TabChanged;
            this.dvFloat.SlotUpdate += new DataView.SlotUpdateEventHandler(this.dvFloat_SlotUpdate);
            this.dvFloat.SlotFlip += new DataView.SlotFlipEventHandler(this.dvFloat_SlotFlip);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);

            this.ClientSize = new System.Drawing.Size(298, 348);
            this.Controls.Add((System.Windows.Forms.Control)this.dvFloat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Info";
            this.TopMost = true;
            this.ResumeLayout(false);
        }


        #endregion
    }
}
using System.ComponentModel;
using Mids_Reborn.Core;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms
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
            dvFloat = new DataView();
            SuspendLayout();
            // 
            // dvFloat
            // 
            dvFloat.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            dvFloat.DrawVillain = false;
            dvFloat.Floating = true;
            dvFloat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            dvFloat.Location = new System.Drawing.Point(0, 0);
            dvFloat.Name = "dvFloat";
            dvFloat.Size = new System.Drawing.Size(300, 348);
            dvFloat.TabIndex = 0;
            dvFloat.VisibleSize = Enums.eVisibleSize.Full;
            dvFloat.FloatChange += dvFloat_FloatChanged;
            dvFloat.SizeChange += dvFloat_SizeChange;
            dvFloat.SlotFlip += dvFloat_SlotFlip;
            dvFloat.SlotUpdate += dvFloat_SlotUpdate;
            dvFloat.TabChanged += dvFloat_TabChanged;
            dvFloat.UnlockClick += dvFloat_Unlock;
            dvFloat.Load += dvFloat_Load;
            // 
            // frmFloatingStats
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(298, 348);
            Controls.Add(dvFloat);
            ForeColor = System.Drawing.Color.Gainsboro;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(314, 387);
            Name = "frmFloatingStats";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Info";
            TopMost = true;
            ResumeLayout(false);
        }


        #endregion
        public DataView dvFloat;
    }
}
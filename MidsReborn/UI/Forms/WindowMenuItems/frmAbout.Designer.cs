using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    partial class frmAbout
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
            panel1 = new System.Windows.Forms.Panel();
            pbAppIcon = new System.Windows.Forms.PictureBox();
            btnClose = new ImageButtonEx();
            btnCopy = new ImageButtonEx();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAppIcon).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.Transparent;
            panel1.Controls.Add(pbAppIcon);
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(btnCopy);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(612, 344);
            panel1.TabIndex = 0;
            panel1.MouseMove += panel1_MouseMove;
            // 
            // pbAppIcon
            // 
            pbAppIcon.BackColor = System.Drawing.Color.Transparent;
            pbAppIcon.Location = new System.Drawing.Point(324, 15);
            pbAppIcon.Name = "pbAppIcon";
            pbAppIcon.Size = new System.Drawing.Size(48, 48);
            pbAppIcon.TabIndex = 14;
            pbAppIcon.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnClose.CurrentText = "Close";
            btnClose.DisplayVertically = false;
            btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnClose.Location = new System.Drawing.Point(138, 304);
            btnClose.Lock = false;
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(110, 30);
            btnClose.TabIndex = 13;
            btnClose.Text = "Close";
            btnClose.TextOutline.Color = System.Drawing.Color.Black;
            btnClose.TextOutline.Width = 2;
            btnClose.ToggleState = ImageButtonEx.States.ToggledOff;
            btnClose.ToggleText.Indeterminate = "Indeterminate State";
            btnClose.ToggleText.ToggledOff = "ToggledOff State";
            btnClose.ToggleText.ToggledOn = "ToggledOn State";
            btnClose.UseAlt = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnCopy
            // 
            btnCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnCopy.CurrentText = "Copy";
            btnCopy.DisplayVertically = false;
            btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnCopy.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnCopy.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCopy.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCopy.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCopy.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCopy.Location = new System.Drawing.Point(9, 304);
            btnCopy.Lock = false;
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new System.Drawing.Size(110, 30);
            btnCopy.TabIndex = 12;
            btnCopy.Text = "Copy";
            btnCopy.TextOutline.Color = System.Drawing.Color.Black;
            btnCopy.TextOutline.Width = 2;
            btnCopy.ToggleState = ImageButtonEx.States.ToggledOff;
            btnCopy.ToggleText.Indeterminate = "Indeterminate State";
            btnCopy.ToggleText.ToggledOff = "ToggledOff State";
            btnCopy.ToggleText.ToggledOn = "ToggledOn State";
            btnCopy.UseAlt = false;
            btnCopy.Click += btnCopy_Click;
            // 
            // frmAbout
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            ClientSize = new System.Drawing.Size(612, 344);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            ForeColor = System.Drawing.Color.Black;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAbout";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About Mids Reborn";
            Load += frmAbout_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbAppIcon).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbAppIcon;
        private Controls.ImageButtonEx btnClose;
        private Controls.ImageButtonEx btnCopy;
    }
}
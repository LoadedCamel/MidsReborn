namespace Mids_Reborn.Forms.WindowMenuItems
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
            this.btnCopy = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.btnClose = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.pbAppIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbAppIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCopy
            // 
            this.btnCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCopy.CurrentText = "Copy";
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCopy.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnCopy.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnCopy.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnCopy.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnCopy.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnCopy.Location = new System.Drawing.Point(9, 304);
            this.btnCopy.Lock = false;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(110, 30);
            this.btnCopy.TabIndex = 9;
            this.btnCopy.Text = "Copy";
            this.btnCopy.TextOutline.Color = System.Drawing.Color.Black;
            this.btnCopy.TextOutline.Width = 2;
            this.btnCopy.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.btnCopy.ToggleText.Indeterminate = "Indeterminate State";
            this.btnCopy.ToggleText.ToggledOff = "ToggledOff State";
            this.btnCopy.ToggleText.ToggledOn = "ToggledOn State";
            this.btnCopy.UseAlt = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.CurrentText = "Close";
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnClose.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnClose.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnClose.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnClose.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnClose.Location = new System.Drawing.Point(138, 304);
            this.btnClose.Lock = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 30);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.TextOutline.Color = System.Drawing.Color.Black;
            this.btnClose.TextOutline.Width = 2;
            this.btnClose.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.btnClose.ToggleText.Indeterminate = "Indeterminate State";
            this.btnClose.ToggleText.ToggledOff = "ToggledOff State";
            this.btnClose.ToggleText.ToggledOn = "ToggledOn State";
            this.btnClose.UseAlt = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pbAppIcon
            // 
            this.pbAppIcon.BackColor = System.Drawing.Color.Transparent;
            this.pbAppIcon.Location = new System.Drawing.Point(324, 15);
            this.pbAppIcon.Name = "pbAppIcon";
            this.pbAppIcon.Size = new System.Drawing.Size(48, 48);
            this.pbAppIcon.TabIndex = 11;
            this.pbAppIcon.TabStop = false;
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(612, 344);
            this.Controls.Add(this.pbAppIcon);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCopy);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbAppIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnCopy;
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnClose;
        private System.Windows.Forms.PictureBox pbAppIcon;
    }
}
namespace Mids_Reborn.Forms.WindowMenuItems
{
    partial class FrmEntityDetails
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
            this.btnTopMost = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.btnClose = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.lblEntityName = new System.Windows.Forms.Label();
            this.petView1 = new Mids_Reborn.Forms.Controls.PetView();
            this.SuspendLayout();
            // 
            // btnTopMost
            // 
            this.btnTopMost.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnTopMost.ButtonType = Mids_Reborn.Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            this.btnTopMost.CurrentText = "ToggledOn State";
            this.btnTopMost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTopMost.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnTopMost.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnTopMost.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnTopMost.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnTopMost.Location = new System.Drawing.Point(525, 488);
            this.btnTopMost.Lock = false;
            this.btnTopMost.Name = "btnTopMost";
            this.btnTopMost.Size = new System.Drawing.Size(126, 30);
            this.btnTopMost.TabIndex = 2;
            this.btnTopMost.Text = "Top Most";
            this.btnTopMost.TextOutline.Color = System.Drawing.Color.Black;
            this.btnTopMost.TextOutline.Width = 2;
            this.btnTopMost.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOn;
            this.btnTopMost.ToggleText.Indeterminate = "Indeterminate State";
            this.btnTopMost.ToggleText.ToggledOff = "To Top Most";
            this.btnTopMost.ToggleText.ToggledOn = "Top Most";
            this.btnTopMost.UseAlt = false;
            this.btnTopMost.Click += new System.EventHandler(this.btnTopMost_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.CurrentText = "Close";
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClose.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnClose.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnClose.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnClose.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnClose.Location = new System.Drawing.Point(525, 524);
            this.btnClose.Lock = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(126, 30);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.TextOutline.Color = System.Drawing.Color.Black;
            this.btnClose.TextOutline.Width = 2;
            this.btnClose.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.btnClose.ToggleText.Indeterminate = "Indeterminate State";
            this.btnClose.ToggleText.ToggledOff = "Close";
            this.btnClose.ToggleText.ToggledOn = "Close";
            this.btnClose.UseAlt = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblEntityName
            // 
            this.lblEntityName.AutoSize = true;
            this.lblEntityName.Font = new System.Drawing.Font("Segoe UI Variable Display", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.lblEntityName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblEntityName.Location = new System.Drawing.Point(16, 9);
            this.lblEntityName.Name = "lblEntityName";
            this.lblEntityName.Size = new System.Drawing.Size(121, 26);
            this.lblEntityName.TabIndex = 4;
            this.lblEntityName.Text = "Entity Name";
            // 
            // petView1
            // 
            this.petView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(56)))), ((int)(((byte)(100)))));
            this.petView1.ForeColor = System.Drawing.Color.Black;
            this.petView1.Location = new System.Drawing.Point(16, 40);
            this.petView1.Name = "petView1";
            this.petView1.Size = new System.Drawing.Size(442, 519);
            this.petView1.TabIndex = 5;
            this.petView1.UseAlt = false;
            // 
            // FrmEntityDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(712, 570);
            this.Controls.Add(this.petView1);
            this.Controls.Add(this.lblEntityName);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnTopMost);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(41, 18, 41, 18);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(562, 517);
            this.Name = "FrmEntityDetails";
            this.ShowIcon = false;
            this.Text = "Entity Details";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmEntityDetails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnTopMost;
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnClose;
        private System.Windows.Forms.Label lblEntityName;
        private Controls.PetView petView1;
    }
}
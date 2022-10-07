namespace Forms.WindowMenuItems
{
    partial class frmEntityDetails
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
            this.dataView1 = new Mids_Reborn.Forms.Controls.DataView();
            this.btnTopMost = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.btnClose = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.lblEntityName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dataView1
            // 
            this.dataView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dataView1.DrawVillain = false;
            this.dataView1.Floating = false;
            this.dataView1.Font = new System.Drawing.Font("Arial", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dataView1.Location = new System.Drawing.Point(16, 77);
            this.dataView1.Name = "dataView1";
            this.dataView1.Size = new System.Drawing.Size(303, 464);
            this.dataView1.TabIndex = 0;
            this.dataView1.VisibleSize = Mids_Reborn.Core.Enums.eVisibleSize.Full;
            this.dataView1.Unlock_Click += new Mids_Reborn.Forms.Controls.DataView.Unlock_ClickEventHandler(this.dataView1_Unlock_Click);
            // 
            // btnTopMost
            // 
            this.btnTopMost.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnTopMost.CurrentText = "Top Most";
            this.btnTopMost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTopMost.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnTopMost.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnTopMost.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnTopMost.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnTopMost.Location = new System.Drawing.Point(369, 475);
            this.btnTopMost.Lock = false;
            this.btnTopMost.Name = "btnTopMost";
            this.btnTopMost.Size = new System.Drawing.Size(126, 30);
            this.btnTopMost.TabIndex = 2;
            this.btnTopMost.Text = "Top Most";
            this.btnTopMost.TextOutline.Color = System.Drawing.Color.Black;
            this.btnTopMost.TextOutline.Width = 2;
            this.btnTopMost.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
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
            this.btnClose.Location = new System.Drawing.Point(369, 511);
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
            this.lblEntityName.Font = new System.Drawing.Font("Segoe UI Variable Display", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEntityName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblEntityName.Location = new System.Drawing.Point(16, 9);
            this.lblEntityName.Name = "lblEntityName";
            this.lblEntityName.Size = new System.Drawing.Size(121, 26);
            this.lblEntityName.TabIndex = 4;
            this.lblEntityName.Text = "Entity Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(43, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Showing base values only.";
            // 
            // frmEntityDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(546, 561);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblEntityName);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnTopMost);
            this.Controls.Add(this.dataView1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(41, 18, 41, 18);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEntityDetails";
            this.ShowIcon = false;
            this.Text = "Entity Details";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmEntityDetails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Mids_Reborn.Forms.Controls.DataView dataView1;
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnTopMost;
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnClose;
        private System.Windows.Forms.Label lblEntityName;
        private System.Windows.Forms.Label label1;
    }
}
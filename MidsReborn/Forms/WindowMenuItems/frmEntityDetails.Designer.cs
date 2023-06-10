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
            btnTopMost = new Controls.ImageButtonEx();
            btnClose = new Controls.ImageButtonEx();
            lblEntityName = new System.Windows.Forms.Label();
            petView1 = new Controls.PetView();
            powersCombo1 = new Controls.PowersCombo();
            SuspendLayout();
            // 
            // btnTopMost
            // 
            btnTopMost.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnTopMost.ButtonType = Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            btnTopMost.CurrentText = "ToggledOn State";
            btnTopMost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnTopMost.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnTopMost.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnTopMost.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnTopMost.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnTopMost.Location = new System.Drawing.Point(196, 573);
            btnTopMost.Lock = false;
            btnTopMost.Name = "btnTopMost";
            btnTopMost.Size = new System.Drawing.Size(126, 30);
            btnTopMost.TabIndex = 2;
            btnTopMost.Text = "Top Most";
            btnTopMost.TextOutline.Color = System.Drawing.Color.Black;
            btnTopMost.TextOutline.Width = 2;
            btnTopMost.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOn;
            btnTopMost.ToggleText.Indeterminate = "Indeterminate State";
            btnTopMost.ToggleText.ToggledOff = "To Top Most";
            btnTopMost.ToggleText.ToggledOn = "Top Most";
            btnTopMost.UseAlt = false;
            btnTopMost.Click += btnTopMost_Click;
            // 
            // btnClose
            // 
            btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnClose.CurrentText = "Close";
            btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnClose.Location = new System.Drawing.Point(328, 573);
            btnClose.Lock = false;
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(126, 30);
            btnClose.TabIndex = 3;
            btnClose.Text = "Close";
            btnClose.TextOutline.Color = System.Drawing.Color.Black;
            btnClose.TextOutline.Width = 2;
            btnClose.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnClose.ToggleText.Indeterminate = "Indeterminate State";
            btnClose.ToggleText.ToggledOff = "Close";
            btnClose.ToggleText.ToggledOn = "Close";
            btnClose.UseAlt = false;
            btnClose.Click += btnClose_Click;
            // 
            // lblEntityName
            // 
            lblEntityName.AutoEllipsis = true;
            lblEntityName.Font = new System.Drawing.Font("Segoe UI Variable Display", 14.25F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            lblEntityName.ForeColor = System.Drawing.Color.WhiteSmoke;
            lblEntityName.Location = new System.Drawing.Point(12, 8);
            lblEntityName.Name = "lblEntityName";
            lblEntityName.Size = new System.Drawing.Size(264, 34);
            lblEntityName.TabIndex = 4;
            lblEntityName.Text = "Entity Name";
            lblEntityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // petView1
            // 
            petView1.BackColor = System.Drawing.Color.FromArgb(12, 56, 100);
            petView1.ForeColor = System.Drawing.Color.Black;
            petView1.Location = new System.Drawing.Point(12, 48);
            petView1.Name = "petView1";
            petView1.Size = new System.Drawing.Size(442, 519);
            petView1.TabIndex = 5;
            petView1.UseAlt = false;
            // 
            // powersCombo1
            // 
            powersCombo1.BackColor = System.Drawing.Color.Black;
            powersCombo1.BorderColor = System.Drawing.Color.DodgerBlue;
            powersCombo1.BorderWidth = 1;
            powersCombo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            powersCombo1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            powersCombo1.HighlightColor = System.Drawing.Color.Goldenrod;
            powersCombo1.HighlightTextColor = System.Drawing.Color.Black;
            powersCombo1.Location = new System.Drawing.Point(281, 8);
            powersCombo1.MinimumSize = new System.Drawing.Size(171, 34);
            powersCombo1.Name = "powersCombo1";
            powersCombo1.Padding = new System.Windows.Forms.Padding(1);
            powersCombo1.Size = new System.Drawing.Size(171, 34);
            powersCombo1.TabIndex = 6;
            // 
            // FrmEntityDetails
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(466, 614);
            Controls.Add(powersCombo1);
            Controls.Add(petView1);
            Controls.Add(lblEntityName);
            Controls.Add(btnClose);
            Controls.Add(btnTopMost);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(41, 18, 41, 18);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEntityDetails";
            ShowIcon = false;
            Text = "Entity Details";
            TopMost = true;
            Load += frmEntityDetails_Load;
            ResumeLayout(false);
        }

        #endregion
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnTopMost;
        private Mids_Reborn.Forms.Controls.ImageButtonEx btnClose;
        private System.Windows.Forms.Label lblEntityName;
        private Controls.PetView petView1;
        private Controls.PowersCombo powersCombo1;
    }
}
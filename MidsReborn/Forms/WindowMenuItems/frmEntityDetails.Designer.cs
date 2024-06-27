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
            powersCombo1 = new Controls.MidsComboBox();
            petView1 = new Controls.PetView();
            lblEntityName = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // powersCombo1
            // 
            powersCombo1.BackColor = System.Drawing.Color.Black;
            powersCombo1.BorderColor = System.Drawing.Color.DodgerBlue;
            powersCombo1.BorderWidth = 1;
            powersCombo1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            powersCombo1.ForeColor = System.Drawing.Color.WhiteSmoke;
            powersCombo1.HighlightColor = System.Drawing.Color.Goldenrod;
            powersCombo1.HighlightTextColor = System.Drawing.Color.Black;
            powersCombo1.Location = new System.Drawing.Point(281, 9);
            powersCombo1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            powersCombo1.MinimumSize = new System.Drawing.Size(171, 34);
            powersCombo1.Name = "powersCombo1";
            powersCombo1.Padding = new System.Windows.Forms.Padding(1);
            powersCombo1.Size = new System.Drawing.Size(171, 34);
            powersCombo1.TabIndex = 9;
            // 
            // petView1
            // 
            petView1.BackColor = System.Drawing.Color.FromArgb(12, 56, 100);
            petView1.ForeColor = System.Drawing.Color.Black;
            petView1.Location = new System.Drawing.Point(12, 49);
            petView1.Name = "petView1";
            petView1.Size = new System.Drawing.Size(442, 571);
            petView1.TabIndex = 8;
            petView1.UseAlt = false;
            // 
            // lblEntityName
            // 
            lblEntityName.AutoEllipsis = true;
            lblEntityName.Font = new System.Drawing.Font("Segoe UI Variable Display", 14.25F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            lblEntityName.ForeColor = System.Drawing.Color.WhiteSmoke;
            lblEntityName.Location = new System.Drawing.Point(12, 9);
            lblEntityName.Name = "lblEntityName";
            lblEntityName.Size = new System.Drawing.Size(264, 34);
            lblEntityName.TabIndex = 7;
            lblEntityName.Text = "Entity Name";
            lblEntityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmEntityDetails
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(466, 632);
            Controls.Add(powersCombo1);
            Controls.Add(petView1);
            Controls.Add(lblEntityName);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(41, 18, 41, 18);
            Name = "FrmEntityDetails";
            ShowIcon = false;
            Text = "Entity Details";
            ResumeLayout(false);
        }

        #endregion

        private Controls.MidsComboBox powersCombo1;
        private Controls.PetView petView1;
        private System.Windows.Forms.Label lblEntityName;
    }
}
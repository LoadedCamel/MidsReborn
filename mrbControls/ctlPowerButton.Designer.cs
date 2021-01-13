namespace mrbControls
{
    partial class ctlPowerButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlPowerButton));
            this.PowerButton = new System.Windows.Forms.Button();
            this.buttonColors = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // PowerButton
            // 
            this.PowerButton.BackColor = System.Drawing.Color.Transparent;
            this.PowerButton.FlatAppearance.BorderSize = 0;
            this.PowerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PowerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PowerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PowerButton.ImageIndex = 0;
            this.PowerButton.ImageList = this.buttonColors;
            this.PowerButton.Location = new System.Drawing.Point(3, 3);
            this.PowerButton.Name = "PowerButton";
            this.PowerButton.Size = new System.Drawing.Size(190, 36);
            this.PowerButton.TabIndex = 0;
            this.PowerButton.UseVisualStyleBackColor = false;
            // 
            // buttonColors
            // 
            this.buttonColors.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("buttonColors.ImageStream")));
            this.buttonColors.TransparentColor = System.Drawing.Color.Transparent;
            this.buttonColors.Images.SetKeyName(0, "pSlot2.png");
            this.buttonColors.Images.SetKeyName(1, "pSlot3.png");
            this.buttonColors.Images.SetKeyName(2, "pSlot4.png");
            // 
            // ctlPowerButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.PowerButton);
            this.Name = "ctlPowerButton";
            this.Size = new System.Drawing.Size(252, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PowerButton;
        private System.Windows.Forms.ImageList buttonColors;
    }
}

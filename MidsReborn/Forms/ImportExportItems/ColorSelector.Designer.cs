namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class ColorSelector
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
            colorWheel1 = new Controls.ColorWheel();
            btnOk = new Controls.ImageButtonEx();
            btnCancel = new Controls.ImageButtonEx();
            borderPanel1 = new Controls.BorderPanel();
            SuspendLayout();
            // 
            // colorWheel1
            // 
            colorWheel1.AutoSize = true;
            colorWheel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            colorWheel1.BackColor = System.Drawing.Color.Transparent;
            colorWheel1.Location = new System.Drawing.Point(3, 3);
            colorWheel1.Name = "colorWheel1";
            colorWheel1.Size = new System.Drawing.Size(352, 344);
            colorWheel1.TabIndex = 0;
            colorWheel1.SelectionChanged += colorWheel1_SelectionChanged;
            // 
            // btnOk
            // 
            btnOk.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnOk.CurrentText = "OK";
            btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnOk.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnOk.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnOk.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnOk.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnOk.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnOk.Location = new System.Drawing.Point(50, 369);
            btnOk.Lock = false;
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(100, 30);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            btnOk.TextOutline.Color = System.Drawing.Color.Black;
            btnOk.TextOutline.Width = 2;
            btnOk.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnOk.ToggleText.Indeterminate = "Indeterminate State";
            btnOk.ToggleText.ToggledOff = "ToggledOff State";
            btnOk.ToggleText.ToggledOn = "ToggledOn State";
            btnOk.UseAlt = false;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnCancel.CurrentText = "Cancel";
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCancel.Location = new System.Drawing.Point(204, 369);
            btnCancel.Lock = false;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.TextOutline.Color = System.Drawing.Color.Black;
            btnCancel.TextOutline.Width = 2;
            btnCancel.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnCancel.ToggleText.Indeterminate = "Indeterminate State";
            btnCancel.ToggleText.ToggledOff = "ToggledOff State";
            btnCancel.ToggleText.ToggledOn = "ToggledOn State";
            btnCancel.UseAlt = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.Color.Silver;
            borderPanel1.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 2;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            borderPanel1.Location = new System.Drawing.Point(0, 0);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(358, 411);
            borderPanel1.TabIndex = 3;
            // 
            // ColorSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            ClientSize = new System.Drawing.Size(358, 411);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(colorWheel1);
            Controls.Add(borderPanel1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ColorSelector";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "ColorSelector";
            Load += ColorSelector_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.ColorWheel colorWheel1;
        private Controls.ImageButtonEx btnOk;
        private Controls.ImageButtonEx btnCancel;
        private Controls.BorderPanel borderPanel1;
    }
}
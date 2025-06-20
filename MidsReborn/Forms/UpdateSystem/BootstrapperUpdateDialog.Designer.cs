namespace Mids_Reborn.Forms.UpdateSystem
{
    partial class BootstrapperUpdateDialog
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
            borderPanel1 = new Mids_Reborn.Forms.Controls.BorderPanel();
            imageButtonEx1 = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            label1 = new System.Windows.Forms.Label();
            skControl1 = new SkiaSharp.Views.Desktop.SKControl();
            borderPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.Color.FromArgb(0, 192, 192);
            borderPanel1.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 2;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(imageButtonEx1);
            borderPanel1.Controls.Add(label1);
            borderPanel1.Controls.Add(skControl1);
            borderPanel1.Location = new System.Drawing.Point(12, 12);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(340, 189);
            borderPanel1.TabIndex = 0;
            // 
            // imageButtonEx1
            // 
            imageButtonEx1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            imageButtonEx1.CurrentText = "Close";
            imageButtonEx1.DisplayVertically = false;
            imageButtonEx1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            imageButtonEx1.Images.Background = MRBResourceLib.Resources.HeroButton;
            imageButtonEx1.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            imageButtonEx1.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            imageButtonEx1.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            imageButtonEx1.Location = new System.Drawing.Point(120, 147);
            imageButtonEx1.Lock = false;
            imageButtonEx1.Name = "imageButtonEx1";
            imageButtonEx1.Size = new System.Drawing.Size(100, 30);
            imageButtonEx1.TabIndex = 2;
            imageButtonEx1.Text = "Close";
            imageButtonEx1.TextOutline.Color = System.Drawing.Color.Black;
            imageButtonEx1.TextOutline.Width = 2;
            imageButtonEx1.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            imageButtonEx1.ToggleText.Indeterminate = "Indeterminate State";
            imageButtonEx1.ToggleText.ToggledOff = "ToggledOff State";
            imageButtonEx1.ToggleText.ToggledOn = "ToggledOn State";
            imageButtonEx1.UseAlt = false;
            imageButtonEx1.Visible = false;
            imageButtonEx1.Click += imageButtonEx1_Click;
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.Color.FromArgb(192, 220, 255);
            label1.Location = new System.Drawing.Point(3, 118);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(334, 23);
            label1.TabIndex = 1;
            label1.Text = "label1";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // skControl1
            // 
            skControl1.BackColor = System.Drawing.Color.Black;
            skControl1.Location = new System.Drawing.Point(131, 20);
            skControl1.Name = "skControl1";
            skControl1.Size = new System.Drawing.Size(80, 80);
            skControl1.TabIndex = 0;
            skControl1.Text = "skControl1";
            skControl1.PaintSurface += skControl1_PaintSurface;
            // 
            // BootstrapperUpdateDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(0, 8, 32);
            ClientSize = new System.Drawing.Size(364, 213);
            Controls.Add(borderPanel1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BootstrapperUpdateDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Bootstrapper update";
            TopMost = true;
            Load += BootstrapperUpdateDialog_Load;
            borderPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.BorderPanel borderPanel1;
        private System.Windows.Forms.Label label1;
        private SkiaSharp.Views.Desktop.SKControl skControl1;
        private Controls.ImageButtonEx imageButtonEx1;
    }
}
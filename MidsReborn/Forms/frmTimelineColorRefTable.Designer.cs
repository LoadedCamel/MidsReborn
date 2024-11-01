namespace Mids_Reborn.Forms
{
    partial class frmTimelineColorRefTable
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
            ibClose = new Controls.ImageButtonEx();
            borderPanel1 = new Controls.BorderPanel();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // ibClose
            // 
            ibClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibClose.CurrentText = "Close";
            ibClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            ibClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibClose.Location = new System.Drawing.Point(268, 408);
            ibClose.Lock = false;
            ibClose.Name = "ibClose";
            ibClose.Size = new System.Drawing.Size(120, 30);
            ibClose.TabIndex = 9;
            ibClose.Text = "Close";
            ibClose.TextOutline.Color = System.Drawing.Color.Black;
            ibClose.TextOutline.Width = 2;
            ibClose.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibClose.ToggleText.Indeterminate = "Indeterminate State";
            ibClose.ToggleText.ToggledOff = "ToggledOff State";
            ibClose.ToggleText.ToggledOn = "ToggledOn State";
            ibClose.UseAlt = false;
            ibClose.Click += ibClose_Click;
            // 
            // borderPanel1
            // 
            borderPanel1.BackColor = System.Drawing.Color.FromArgb(10, 10, 10);
            borderPanel1.Border.Color = System.Drawing.Color.FromArgb(12, 56, 100);
            borderPanel1.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Location = new System.Drawing.Point(12, 44);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(633, 358);
            borderPanel1.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.Gainsboro;
            label1.Location = new System.Drawing.Point(12, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(268, 19);
            label1.TabIndex = 11;
            label1.Text = "Combat Timeline Colors Reference Table:";
            // 
            // frmTimelineColorRefTable
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(658, 450);
            Controls.Add(label1);
            Controls.Add(borderPanel1);
            Controls.Add(ibClose);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "frmTimelineColorRefTable";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "frmTimelineColorRefTable";
            Load += frmTimelineColorRefTable_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.ImageButtonEx ibClose;
        private Controls.BorderPanel borderPanel1;
        private System.Windows.Forms.Label label1;
    }
}
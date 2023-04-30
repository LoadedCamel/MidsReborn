using Mids_Reborn.Controls;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.Controls
{
    partial class EnhCheckMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnhCheckMode));
            this.pSalvageSummary = new System.Windows.Forms.Panel();
            this.imageButtonEx1 = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.lblCatalysts = new System.Windows.Forms.Label();
            this.lblBoosters = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblEnhObtained = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pSalvageSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pSalvageSummary
            // 
            this.pSalvageSummary.BackColor = System.Drawing.Color.Transparent;
            this.pSalvageSummary.Controls.Add(this.imageButtonEx1);
            this.pSalvageSummary.Controls.Add(this.lblCatalysts);
            this.pSalvageSummary.Controls.Add(this.lblBoosters);
            this.pSalvageSummary.Controls.Add(this.pictureBox3);
            this.pSalvageSummary.Controls.Add(this.pictureBox2);
            this.pSalvageSummary.Controls.Add(this.lblEnhObtained);
            this.pSalvageSummary.Controls.Add(this.pictureBox1);
            this.pSalvageSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pSalvageSummary.Location = new System.Drawing.Point(0, 0);
            this.pSalvageSummary.Margin = new System.Windows.Forms.Padding(0);
            this.pSalvageSummary.Name = "pSalvageSummary";
            this.pSalvageSummary.Size = new System.Drawing.Size(434, 37);
            this.pSalvageSummary.TabIndex = 20;
            // 
            // imageButtonEx1
            // 
            this.imageButtonEx1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imageButtonEx1.Font = new System.Drawing.Font("MS Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.imageButtonEx1.Images.Background = ((System.Drawing.Image)(resources.GetObject("resource.Background")));
            this.imageButtonEx1.Images.Hover = ((System.Drawing.Image)(resources.GetObject("resource.Hover")));
            this.imageButtonEx1.ImagesAlt.Background = ((System.Drawing.Image)(resources.GetObject("resource.Background1")));
            this.imageButtonEx1.ImagesAlt.Hover = ((System.Drawing.Image)(resources.GetObject("resource.Hover1")));
            this.imageButtonEx1.Location = new System.Drawing.Point(324, 5);
            this.imageButtonEx1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageButtonEx1.Name = "imageButtonEx1";
            this.imageButtonEx1.Size = new System.Drawing.Size(107, 28);
            this.imageButtonEx1.TabIndex = 7;
            this.imageButtonEx1.Text = "Exit Check Mode";
            this.imageButtonEx1.TextOutline.Color = System.Drawing.Color.Black;
            this.imageButtonEx1.TextOutline.Width = 3;
            this.imageButtonEx1.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.imageButtonEx1.ToggleText.Indeterminate = "Indeterminate State";
            this.imageButtonEx1.ToggleText.ToggledOff = "ToggledOff State";
            this.imageButtonEx1.ToggleText.ToggledOn = "ToggledOn State";
            this.imageButtonEx1.UseAlt = false;
            this.imageButtonEx1.Click += new System.EventHandler(this.imageButtonEx1_Click);
            // 
            // lblCatalysts
            // 
            this.lblCatalysts.AutoSize = true;
            this.lblCatalysts.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCatalysts.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblCatalysts.Location = new System.Drawing.Point(196, 13);
            this.lblCatalysts.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCatalysts.Name = "lblCatalysts";
            this.lblCatalysts.Size = new System.Drawing.Size(33, 15);
            this.lblCatalysts.TabIndex = 6;
            this.lblCatalysts.Text = "x100";
            // 
            // lblBoosters
            // 
            this.lblBoosters.AutoSize = true;
            this.lblBoosters.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblBoosters.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblBoosters.Location = new System.Drawing.Point(286, 13);
            this.lblBoosters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBoosters.Name = "lblBoosters";
            this.lblBoosters.Size = new System.Drawing.Size(33, 15);
            this.lblBoosters.TabIndex = 5;
            this.lblBoosters.Text = "x100";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(245, 0);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(37, 39);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(159, 0);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(37, 39);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // lblEnhObtained
            // 
            this.lblEnhObtained.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblEnhObtained.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEnhObtained.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblEnhObtained.Location = new System.Drawing.Point(40, 11);
            this.lblEnhObtained.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEnhObtained.Name = "lblEnhObtained";
            this.lblEnhObtained.Size = new System.Drawing.Size(116, 19);
            this.lblEnhObtained.TabIndex = 1;
            this.lblEnhObtained.Text = "Obtained: 100/100";
            this.lblEnhObtained.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.lblEnhObtained, "Left click to toggle all slots, right click to toggle display format");
            this.lblEnhObtained.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblEnhObtained_MouseClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 39);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // EnhCheckMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pSalvageSummary);
            this.DoubleBuffered = true;
            this.Name = "EnhCheckMode";
            this.Size = new System.Drawing.Size(434, 37);
            this.pSalvageSummary.ResumeLayout(false);
            this.pSalvageSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pSalvageSummary;
        private System.Windows.Forms.Label lblCatalysts;
        private System.Windows.Forms.Label lblBoosters;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblEnhObtained;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ImageButtonEx imageButtonEx1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

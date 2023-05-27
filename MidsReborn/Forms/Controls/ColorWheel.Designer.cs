namespace Mids_Reborn.Forms.Controls
{
    partial class ColorWheel
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
            colorPicker = new System.Windows.Forms.PictureBox();
            colorPreview = new System.Windows.Forms.Panel();
            colorSelection = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)colorPicker).BeginInit();
            SuspendLayout();
            // 
            // colorPicker
            // 
            colorPicker.Image = MRBResourceLib.Resources.ColorSpectrum;
            colorPicker.Location = new System.Drawing.Point(3, 0);
            colorPicker.Name = "colorPicker";
            colorPicker.Size = new System.Drawing.Size(300, 300);
            colorPicker.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            colorPicker.TabIndex = 0;
            colorPicker.TabStop = false;
            colorPicker.MouseDown += ColorPicker_MouseDown;
            colorPicker.MouseMove += ColorPicker_MouseMove;
            // 
            // colorPreview
            // 
            colorPreview.BackColor = System.Drawing.Color.Transparent;
            colorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            colorPreview.Location = new System.Drawing.Point(3, 306);
            colorPreview.Name = "colorPreview";
            colorPreview.Size = new System.Drawing.Size(146, 35);
            colorPreview.TabIndex = 1;
            // 
            // colorSelection
            // 
            colorSelection.BackColor = System.Drawing.Color.Transparent;
            colorSelection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            colorSelection.Location = new System.Drawing.Point(157, 306);
            colorSelection.Name = "colorSelection";
            colorSelection.Size = new System.Drawing.Size(146, 35);
            colorSelection.TabIndex = 2;
            // 
            // ColorWheel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.Transparent;
            Controls.Add(colorSelection);
            Controls.Add(colorPreview);
            Controls.Add(colorPicker);
            DoubleBuffered = true;
            Name = "ColorWheel";
            Size = new System.Drawing.Size(306, 344);
            ((System.ComponentModel.ISupportInitialize)colorPicker).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox colorPicker;
        private System.Windows.Forms.Panel colorPreview;
        private System.Windows.Forms.Panel colorSelection;
    }
}

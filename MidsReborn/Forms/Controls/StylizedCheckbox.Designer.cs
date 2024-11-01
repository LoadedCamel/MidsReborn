using FontAwesome.Sharp;

namespace Mids_Reborn.Forms.Controls
{
    partial class StylizedCheckbox
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
            iconButton1 = new IconButton();
            SuspendLayout();
            // 
            // iconButton1
            // 
            iconButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            iconButton1.Enabled = false;
            iconButton1.IconChar = IconChar.Check;
            iconButton1.IconColor = System.Drawing.Color.Black;
            iconButton1.IconFont = IconFont.Auto;
            iconButton1.Location = new System.Drawing.Point(0, 0);
            iconButton1.Name = "iconButton1";
            iconButton1.Size = new System.Drawing.Size(16, 16);
            iconButton1.TabIndex = 0;
            iconButton1.Text = "iconButton1";
            iconButton1.UseVisualStyleBackColor = true;
            // 
            // StylizedCheckbox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(iconButton1);
            Name = "StylizedCheckbox";
            Size = new System.Drawing.Size(16, 16);
            MouseClick += StylizedCheckbox_MouseClick;
            MouseEnter += StylizedCheckbox_MouseEnter;
            MouseLeave += StylizedCheckbox_MouseLeave;
            ResumeLayout(false);
        }

        #endregion

        private FontAwesome.Sharp.IconButton iconButton1;
    }
}

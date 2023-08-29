using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    partial class BrightnessSelector
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
            SuspendLayout();
            // 
            // BrightnessSelector
            // 
            BackColor = System.Drawing.Color.Transparent;
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.White;
            Name = "BrightnessSelector";
            Size = new System.Drawing.Size(40, 300);
            MaximumSize = new Size(40, 300);
            MinimumSize = new Size(40, 300);
            ResumeLayout(false);
        }

        #endregion
    }
}

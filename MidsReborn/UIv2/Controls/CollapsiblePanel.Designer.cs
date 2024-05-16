using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.Controls
{
    partial class CollapsiblePanel
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
            this.ToggleButton = new MultiOrientButton();
            this.SuspendLayout();
            //
            // ToggleButton
            //
            this.ToggleButton.Location = new System.Drawing.Point(3, 3);
            this.ToggleButton.Dock = DockStyle.Left;
            this.ToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point);
            this.ToggleButton.Size = new System.Drawing.Size(25, 100);
            this.ToggleButton.TabIndex = 0;
            this.ToggleButton.Text = "Collapse";
            this.ToggleButton.UseVisualStyleBackColor = true;
            this.ToggleButton.UseVerticalText = true;
            this.ToggleButton.Click += ToggleButton_Click;
            //
            // CollapsiblePanel
            //
            this.Controls.Add(ToggleButton);
            this.Name = "CollapsiblePanel";
            this.Size = new System.Drawing.Size(200, 100);
            this.ResumeLayout(false);
        }

        #endregion

        private MultiOrientButton ToggleButton;
    }
}

﻿namespace Mids_Reborn.Forms.Controls
{
    partial class TimelineCursorZoom
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
            // TimelineCursorZoom
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            DoubleBuffered = true;
            Name = "TimelineCursorZoom";
            Size = new System.Drawing.Size(800, 24);
            MouseDown += TimelineCursorZoom_MouseDown;
            MouseLeave += TimelineCursorZoom_MouseLeave;
            MouseMove += TimelineCursorZoom_MouseMove;
            MouseUp += TimelineCursorZoom_MouseUp;
            MouseWheel += TimelineCursorZoom_MouseWheel;
            ResumeLayout(false);
        }

        #endregion

    }
}

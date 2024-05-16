namespace Mids_Reborn.Controls
{
    partial class ctlCombatTimeline
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
            components = new System.ComponentModel.Container();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            SuspendLayout();
            // 
            // toolTip1
            // 
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 100;
            toolTip1.UseAnimation = false;
            // 
            // ctlCombatTimeline
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            DoubleBuffered = true;
            Name = "ctlCombatTimeline";
            Size = new System.Drawing.Size(1245, 260);
            MouseLeave += ctlCombatTimeline_MouseLeave;
            MouseMove += ctlCombatTimeline_MouseMove;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
    }
}

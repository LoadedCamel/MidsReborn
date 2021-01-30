
using System.Drawing;
using System.Windows.Forms;

namespace mrbControls
{
    partial class ctlLayeredBarPb
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
            this.canvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.TTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(54, 52);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(277, 13);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.BackColor = System.Drawing.Color.Transparent;
            this.canvas.Padding = new Padding(0, 0, 0, 0);
            // 
            // TTip
            // 
            this.TTip.AutoPopDelay = 10000;
            this.TTip.InitialDelay = 500;
            this.TTip.ReshowDelay = 100;
            // 
            // ctlLayeredBarPb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.canvas);
            this.Name = "ctlLayeredBarPb";
            this.Size = new System.Drawing.Size(277, 13);
            this.Padding = new Padding(0, 0, 0, 0);
            this.MouseEnter += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBarPb_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ctlLayeredBarPb_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBarPb_MouseMove);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctlLayeredBarPb_Paint);
            this.Resize += new System.EventHandler(this.ctlLayeredBarPb_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.ToolTip TTip;
    }
}

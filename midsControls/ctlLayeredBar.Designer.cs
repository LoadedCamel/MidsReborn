
namespace mrbControls
{
    partial class ctlLayeredBar
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
            this.panel1 = new BarPanel();
            this.panel2 = new BarPanel();
            this.panel3 = new BarPanel();
            this.panel4 = new BarPanel();
            this.panel5 = new BarPanel();
            this.TTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(277, 11);
            this.panel1.TabIndex = 1;
            this.panel1.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(0)))), ((int)(((byte)(191)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(0, -1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(277, 11);
            this.panel2.TabIndex = 2;
            this.panel2.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Magenta;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(0, -1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(277, 11);
            this.panel3.TabIndex = 3;
            this.panel3.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.panel3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(64)))), ((int)(((byte)(255)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Location = new System.Drawing.Point(0, -1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(277, 11);
            this.panel4.TabIndex = 4;
            this.panel4.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.panel4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Location = new System.Drawing.Point(0, -1);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(277, 11);
            this.panel5.TabIndex = 5;
            this.panel5.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.panel5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            // 
            // TTip
            // 
            this.TTip.AutoPopDelay = 10000;
            this.TTip.InitialDelay = 500;
            this.TTip.ReshowDelay = 100;
            // 
            // ctlLayeredBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ctlLayeredBar";
            this.Size = new System.Drawing.Size(277, 13);
            this.Load += new System.EventHandler(this.ctlLayeredBar_Load);
            this.MouseLeave += new System.EventHandler(this.ctlLayeredBar_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ctlLayeredBar_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private BarPanel panel1;
        private BarPanel panel2;
        private BarPanel panel3;
        private BarPanel panel4;
        private BarPanel panel5;
        private System.Windows.Forms.ToolTip TTip;
    }
}
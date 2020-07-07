using System.ComponentModel;
using System.Windows.Forms;
using midsControls;

namespace Hero_Designer.Forms
{
    partial class frmUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.ctlProgressBar1 = new midsControls.ctlProgressBar();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ctlProgressBar2 = new midsControls.ctlProgressBar();
            this.SuspendLayout();
            // 
            // ctlProgressBar1
            // 
            this.ctlProgressBar1.BackColor = System.Drawing.Color.Black;
            this.ctlProgressBar1.ForeColor = System.Drawing.Color.Goldenrod;
            this.ctlProgressBar1.Location = new System.Drawing.Point(12, 383);
            this.ctlProgressBar1.Name = "ctlProgressBar1";
            this.ctlProgressBar1.Size = new System.Drawing.Size(468, 23);
            this.ctlProgressBar1.StatusText = null;
            this.ctlProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ctlProgressBar1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(468, 365);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // ctlProgressBar2
            // 
            this.ctlProgressBar2.BackColor = System.Drawing.Color.Black;
            this.ctlProgressBar2.ForeColor = System.Drawing.Color.Goldenrod;
            this.ctlProgressBar2.Location = new System.Drawing.Point(12, 412);
            this.ctlProgressBar2.Name = "ctlProgressBar2";
            this.ctlProgressBar2.Size = new System.Drawing.Size(468, 23);
            this.ctlProgressBar2.StatusText = null;
            this.ctlProgressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ctlProgressBar2.TabIndex = 2;
            // 
            // frmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 443);
            this.Controls.Add(this.ctlProgressBar2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.ctlProgressBar1);
            this.Name = "frmUpdate";
            this.Text = "Updater";
            this.ResumeLayout(false);

        }

        #endregion

        public ctlProgressBar ctlProgressBar1;
        public RichTextBox richTextBox1;
        public ctlProgressBar ctlProgressBar2;
    }
}
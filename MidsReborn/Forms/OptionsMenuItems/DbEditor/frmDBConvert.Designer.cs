
namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmDBConvert
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDBConvert));
            this.label1 = new System.Windows.Forms.Label();
            this.sourceFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.destinationFolder = new System.Windows.Forms.TextBox();
            this.srcBrowse = new System.Windows.Forms.Button();
            this.destBrowse = new System.Windows.Forms.Button();
            this.convertBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.statusText = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // sourceFolder
            // 
            this.sourceFolder.Location = new System.Drawing.Point(78, 49);
            this.sourceFolder.Name = "sourceFolder";
            this.sourceFolder.Size = new System.Drawing.Size(293, 20);
            this.sourceFolder.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination:";
            // 
            // destinationFolder
            // 
            this.destinationFolder.Location = new System.Drawing.Point(78, 103);
            this.destinationFolder.Name = "destinationFolder";
            this.destinationFolder.Size = new System.Drawing.Size(293, 20);
            this.destinationFolder.TabIndex = 3;
            // 
            // srcBrowse
            // 
            this.srcBrowse.Location = new System.Drawing.Point(381, 47);
            this.srcBrowse.Name = "srcBrowse";
            this.srcBrowse.Size = new System.Drawing.Size(40, 23);
            this.srcBrowse.TabIndex = 4;
            this.srcBrowse.Text = "...";
            this.srcBrowse.UseVisualStyleBackColor = true;
            this.srcBrowse.Click += new System.EventHandler(this.srcBrowse_Click);
            // 
            // destBrowse
            // 
            this.destBrowse.Location = new System.Drawing.Point(381, 101);
            this.destBrowse.Name = "destBrowse";
            this.destBrowse.Size = new System.Drawing.Size(40, 23);
            this.destBrowse.TabIndex = 5;
            this.destBrowse.Text = "...";
            this.destBrowse.UseVisualStyleBackColor = true;
            this.destBrowse.Click += new System.EventHandler(this.destBrowse_Click);
            // 
            // convertBtn
            // 
            this.convertBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.convertBtn.Location = new System.Drawing.Point(144, 308);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new System.Drawing.Size(160, 23);
            this.convertBtn.TabIndex = 6;
            this.convertBtn.Text = "CONVERT";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new System.EventHandler(this.convertBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(35, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(386, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "This tool will convert your 2.7.x databases and data structure to the latest form" +
    "at.\r\n";
            // 
            // statusText
            // 
            this.statusText.BackColor = System.Drawing.Color.Ivory;
            this.statusText.ForeColor = System.Drawing.Color.Black;
            this.statusText.FormattingEnabled = true;
            this.statusText.Location = new System.Drawing.Point(12, 143);
            this.statusText.Name = "statusText";
            this.statusText.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.statusText.Size = new System.Drawing.Size(419, 160);
            this.statusText.TabIndex = 8;
            // 
            // frmDBConvert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 343);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.destBrowse);
            this.Controls.Add(this.srcBrowse);
            this.Controls.Add(this.destinationFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sourceFolder);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDBConvert";
            this.Text = "frmDBConvert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sourceFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox destinationFolder;
        private System.Windows.Forms.Button srcBrowse;
        private System.Windows.Forms.Button destBrowse;
        private System.Windows.Forms.Button convertBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox statusText;
    }
}
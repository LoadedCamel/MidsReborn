
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmDBConvert
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmDBConvert));
            this.label1 = new Label();
            this.sourceFolder = new TextBox();
            this.label2 = new Label();
            this.destinationFolder = new TextBox();
            this.srcBrowse = new Button();
            this.destBrowse = new Button();
            this.convertBtn = new Button();
            this.label3 = new Label();
            this.statusText = new ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(28, 52);
            this.label1.Name = "label1";
            this.label1.Size = new Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // sourceFolder
            // 
            this.sourceFolder.Location = new Point(78, 49);
            this.sourceFolder.Name = "sourceFolder";
            this.sourceFolder.Size = new Size(293, 20);
            this.sourceFolder.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(9, 106);
            this.label2.Name = "label2";
            this.label2.Size = new Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination:";
            // 
            // destinationFolder
            // 
            this.destinationFolder.Location = new Point(78, 103);
            this.destinationFolder.Name = "destinationFolder";
            this.destinationFolder.Size = new Size(293, 20);
            this.destinationFolder.TabIndex = 3;
            // 
            // srcBrowse
            // 
            this.srcBrowse.Location = new Point(381, 47);
            this.srcBrowse.Name = "srcBrowse";
            this.srcBrowse.Size = new Size(40, 23);
            this.srcBrowse.TabIndex = 4;
            this.srcBrowse.Text = "...";
            this.srcBrowse.UseVisualStyleBackColor = true;
            this.srcBrowse.Click += new EventHandler(this.srcBrowse_Click);
            // 
            // destBrowse
            // 
            this.destBrowse.Location = new Point(381, 101);
            this.destBrowse.Name = "destBrowse";
            this.destBrowse.Size = new Size(40, 23);
            this.destBrowse.TabIndex = 5;
            this.destBrowse.Text = "...";
            this.destBrowse.UseVisualStyleBackColor = true;
            this.destBrowse.Click += new EventHandler(this.destBrowse_Click);
            // 
            // convertBtn
            // 
            this.convertBtn.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.convertBtn.Location = new Point(144, 308);
            this.convertBtn.Name = "convertBtn";
            this.convertBtn.Size = new Size(160, 23);
            this.convertBtn.TabIndex = 6;
            this.convertBtn.Text = "CONVERT";
            this.convertBtn.UseVisualStyleBackColor = true;
            this.convertBtn.Click += new EventHandler(this.convertBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new Point(35, 9);
            this.label3.Name = "label3";
            this.label3.Size = new Size(386, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "This tool will convert your 2.7.x databases and data structure to the latest form" +
    "at.\r\n";
            // 
            // statusText
            // 
            this.statusText.BackColor = Color.Ivory;
            this.statusText.ForeColor = Color.Black;
            this.statusText.FormattingEnabled = true;
            this.statusText.Location = new Point(12, 143);
            this.statusText.Name = "statusText";
            this.statusText.SelectionMode = SelectionMode.None;
            this.statusText.Size = new Size(419, 160);
            this.statusText.TabIndex = 8;
            // 
            // frmDBConvert
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(443, 343);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.convertBtn);
            this.Controls.Add(this.destBrowse);
            this.Controls.Add(this.srcBrowse);
            this.Controls.Add(this.destinationFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sourceFolder);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDBConvert";
            this.Text = "frmDBConvert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label1;
        private TextBox sourceFolder;
        private Label label2;
        private TextBox destinationFolder;
        private Button srcBrowse;
        private Button destBrowse;
        private Button convertBtn;
        private Label label3;
        private ListBox statusText;
    }
}
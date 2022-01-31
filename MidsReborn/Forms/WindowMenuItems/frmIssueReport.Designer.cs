using System;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    partial class frmIssueReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIssueReport));
            this.cBIssueType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tBName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rTBIssue = new System.Windows.Forms.RichTextBox();
            this.iBSubmit = new mrbControls.ImageButton();
            this.label4 = new System.Windows.Forms.Label();
            this.cBOS = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tBIssueTitle = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cBIssueType
            // 
            this.cBIssueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBIssueType.FormattingEnabled = true;
            this.cBIssueType.Items.AddRange(new object[] {
            "App Bug (Crashes, Display Issues, etc)",
            "Database Bug (Incorrect attributes, missing/incorrect effects, etc)",
            "Feature Suggestion (Suggest a new feature)"});
            this.cBIssueType.Location = new System.Drawing.Point(148, 49);
            this.cBIssueType.Name = "cBIssueType";
            this.cBIssueType.Size = new System.Drawing.Size(380, 21);
            this.cBIssueType.TabIndex = 1;
            this.cBIssueType.SelectedIndexChanged += new System.EventHandler(this.cBIssueType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.CausesValidation = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Issue Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.CausesValidation = false;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tBName
            // 
            this.tBName.Location = new System.Drawing.Point(148, 82);
            this.tBName.Name = "tBName";
            this.tBName.Size = new System.Drawing.Size(380, 20);
            this.tBName.TabIndex = 2;
            this.tBName.Text = "Discord Tag or Forum Name";
            this.tBName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.CausesValidation = false;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Issue Description:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rTBIssue
            // 
            this.rTBIssue.Location = new System.Drawing.Point(12, 182);
            this.rTBIssue.Name = "rTBIssue";
            this.rTBIssue.Size = new System.Drawing.Size(516, 238);
            this.rTBIssue.TabIndex = 4;
            this.rTBIssue.Text = "";
            this.rTBIssue.TextChanged += new System.EventHandler(this.rTBIssue_TextChanged);
            // 
            // iBSubmit
            // 
            this.iBSubmit.Checked = false;
            this.iBSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iBSubmit.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.iBSubmit.Location = new System.Drawing.Point(423, 426);
            this.iBSubmit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.iBSubmit.Name = "iBSubmit";
            this.iBSubmit.Size = new System.Drawing.Size(105, 22);
            this.iBSubmit.TabIndex = 5;
            this.iBSubmit.TextOff = "SUBMIT";
            this.iBSubmit.TextOn = "Alt Text";
            this.iBSubmit.Toggle = false;
            this.iBSubmit.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.iBSubmit_ButtonClicked);
            // 
            // label4
            // 
            this.label4.CausesValidation = false;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 23);
            this.label4.TabIndex = 6;
            this.label4.Text = "Operating System:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cBOS
            // 
            this.cBOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBOS.FormattingEnabled = true;
            this.cBOS.Items.AddRange(new object[] {
            "--Officially Supported--",
            "Windows 10",
            "Windows 11",
            "--Not Officially Supported--",
            "MacOS (Wine)",
            "MacOS (Windows VM)",
            "Linux (Wine)",
            "Linux (Windows VM)"});
            this.cBOS.Location = new System.Drawing.Point(148, 12);
            this.cBOS.Name = "cBOS";
            this.cBOS.Size = new System.Drawing.Size(380, 21);
            this.cBOS.TabIndex = 0;
            this.cBOS.SelectedIndexChanged += new System.EventHandler(this.cBOS_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.CausesValidation = false;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Issue Title:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tBIssueTitle
            // 
            this.tBIssueTitle.Location = new System.Drawing.Point(15, 141);
            this.tBIssueTitle.Name = "tBIssueTitle";
            this.tBIssueTitle.Size = new System.Drawing.Size(513, 20);
            this.tBIssueTitle.TabIndex = 3;
            this.tBIssueTitle.TextChanged += new System.EventHandler(this.tBIssueTitle_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.cBIssueType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tBName);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.rTBIssue);
            this.panel1.Controls.Add(this.iBSubmit);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cBOS);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tBIssueTitle);
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(542, 456);
            this.panel1.TabIndex = 11;
            // 
            // frmIssueReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(552, 467);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmIssueReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Issue / Suggestion Report";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cBIssueType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tBName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rTBIssue;
        private mrbControls.ImageButton iBSubmit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cBOS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tBIssueTitle;
        private System.Windows.Forms.Panel panel1;
    }
}
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmPrint
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
            this.btnPrinter = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.chkProfileEnh = new System.Windows.Forms.CheckBox();
            this.rbProfileNone = new System.Windows.Forms.RadioButton();
            this.rbProfileLong = new System.Windows.Forms.RadioButton();
            this.rbProfileShort = new System.Windows.Forms.RadioButton();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.chkPrintHistoryEnh = new System.Windows.Forms.CheckBox();
            this.chkPrintHistory = new System.Windows.Forms.CheckBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dlgSetup = new System.Windows.Forms.PageSetupDialog();
            this.lblPrinter = new System.Windows.Forms.Label();
            this.btnLayout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPrinter
            // 
            this.btnPrinter.Location = new System.Drawing.Point(328, 12);
            this.btnPrinter.Name = "btnPrinter";
            this.btnPrinter.Size = new System.Drawing.Size(123, 23);
            this.btnPrinter.TabIndex = 1;
            this.btnPrinter.Text = "Properties...";
            this.btnPrinter.UseVisualStyleBackColor = true;
            this.btnPrinter.Click += new System.EventHandler(this.btnPrinter_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.chkProfileEnh);
            this.GroupBox1.Controls.Add(this.rbProfileNone);
            this.GroupBox1.Controls.Add(this.rbProfileLong);
            this.GroupBox1.Controls.Add(this.rbProfileShort);
            this.GroupBox1.Location = new System.Drawing.Point(12, 38);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(247, 102);
            this.GroupBox1.TabIndex = 3;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Profile:";
            // 
            // chkProfileEnh
            // 
            this.chkProfileEnh.Checked = true;
            this.chkProfileEnh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProfileEnh.Location = new System.Drawing.Point(110, 45);
            this.chkProfileEnh.Name = "chkProfileEnh";
            this.chkProfileEnh.Size = new System.Drawing.Size(131, 20);
            this.chkProfileEnh.TabIndex = 3;
            this.chkProfileEnh.Text = "Show Enhancements";
            this.chkProfileEnh.UseVisualStyleBackColor = true;
            // 
            // rbProfileNone
            // 
            this.rbProfileNone.Checked = true;
            this.rbProfileNone.Location = new System.Drawing.Point(6, 19);
            this.rbProfileNone.Name = "rbProfileNone";
            this.rbProfileNone.Size = new System.Drawing.Size(200, 20);
            this.rbProfileNone.TabIndex = 2;
            this.rbProfileNone.TabStop = true;
            this.rbProfileNone.Text = "Do Not Print";
            this.rbProfileNone.UseVisualStyleBackColor = true;
            // 
            // rbProfileLong
            // 
            this.rbProfileLong.Location = new System.Drawing.Point(6, 71);
            this.rbProfileLong.Name = "rbProfileLong";
            this.rbProfileLong.Size = new System.Drawing.Size(200, 20);
            this.rbProfileLong.TabIndex = 1;
            this.rbProfileLong.Text = "Multi-Page (Long Format)";
            this.rbProfileLong.UseVisualStyleBackColor = true;
            // 
            // rbProfileShort
            // 
            this.rbProfileShort.Location = new System.Drawing.Point(6, 45);
            this.rbProfileShort.Name = "rbProfileShort";
            this.rbProfileShort.Size = new System.Drawing.Size(98, 20);
            this.rbProfileShort.TabIndex = 0;
            this.rbProfileShort.Text = "Single Page";
            this.rbProfileShort.UseVisualStyleBackColor = true;
            this.rbProfileShort.CheckedChanged += new System.EventHandler(this.rbProfileShort_CheckedChanged);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.chkPrintHistoryEnh);
            this.GroupBox2.Controls.Add(this.chkPrintHistory);
            this.GroupBox2.Location = new System.Drawing.Point(265, 67);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(186, 73);
            this.GroupBox2.TabIndex = 4;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Level History:";
            // 
            // chkPrintHistoryEnh
            // 
            this.chkPrintHistoryEnh.Location = new System.Drawing.Point(21, 45);
            this.chkPrintHistoryEnh.Name = "chkPrintHistoryEnh";
            this.chkPrintHistoryEnh.Size = new System.Drawing.Size(159, 20);
            this.chkPrintHistoryEnh.TabIndex = 1;
            this.chkPrintHistoryEnh.Text = "Show Invention Levels";
            this.chkPrintHistoryEnh.UseVisualStyleBackColor = true;
            // 
            // chkPrintHistory
            // 
            this.chkPrintHistory.Location = new System.Drawing.Point(6, 19);
            this.chkPrintHistory.Name = "chkPrintHistory";
            this.chkPrintHistory.Size = new System.Drawing.Size(164, 20);
            this.chkPrintHistory.TabIndex = 0;
            this.chkPrintHistory.Text = "Print History Page";
            this.chkPrintHistory.UseVisualStyleBackColor = true;
            this.chkPrintHistory.CheckedChanged += new System.EventHandler(this.chkPrintHistory_CheckedChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(107, 145);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 23);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "&Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(203, 145);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dlgSetup
            // 
            this.dlgSetup.AllowMargins = false;
            this.dlgSetup.AllowOrientation = false;
            this.dlgSetup.EnableMetric = true;
            // 
            // lblPrinter
            // 
            this.lblPrinter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPrinter.Location = new System.Drawing.Point(57, 12);
            this.lblPrinter.Name = "lblPrinter";
            this.lblPrinter.Size = new System.Drawing.Size(265, 23);
            this.lblPrinter.TabIndex = 7;
            this.lblPrinter.Text = "No Printer";
            this.lblPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLayout
            // 
            this.btnLayout.Location = new System.Drawing.Point(327, 38);
            this.btnLayout.Name = "btnLayout";
            this.btnLayout.Size = new System.Drawing.Size(123, 23);
            this.btnLayout.TabIndex = 8;
            this.btnLayout.Text = "Layout...";
            this.btnLayout.UseVisualStyleBackColor = true;
            this.btnLayout.Click += new System.EventHandler(this.btnLayout_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Printer:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmPrint
            // 
            this.AcceptButton = this.btnPrint;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(462, 180);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLayout);
            this.Controls.Add(this.lblPrinter);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.btnPrinter);
            this.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrint";
            this.Text = "Print";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        Button btnCancel;
        Button btnLayout;

        Button btnPrint;

        Button btnPrinter;

        CheckBox chkPrintHistory;
        CheckBox chkPrintHistoryEnh;
        CheckBox chkProfileEnh;
        PageSetupDialog dlgSetup;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        Label lblPrinter;
        RadioButton rbProfileLong;
        RadioButton rbProfileNone;
        RadioButton rbProfileShort;
        private Label label1;
    }
}
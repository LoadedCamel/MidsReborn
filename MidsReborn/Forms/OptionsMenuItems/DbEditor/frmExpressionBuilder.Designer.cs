namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmExpressionBuilder
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
            panel1 = new System.Windows.Forms.Panel();
            btnPowerInsert = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnOkay = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            tbProbExpr = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            tbMagExpr = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            tbDurationExp = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            lbCommandVars = new System.Windows.Forms.ListBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel1.Controls.Add(btnPowerInsert);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOkay);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(tbProbExpr);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(tbMagExpr);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(tbDurationExp);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(lbCommandVars);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.ForeColor = System.Drawing.Color.Azure;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(882, 455);
            panel1.TabIndex = 0;
            // 
            // btnPowerInsert
            // 
            btnPowerInsert.BackColor = System.Drawing.Color.FromArgb(88, 101, 242);
            btnPowerInsert.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnPowerInsert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnPowerInsert.Location = new System.Drawing.Point(12, 402);
            btnPowerInsert.Margin = new System.Windows.Forms.Padding(4);
            btnPowerInsert.Name = "btnPowerInsert";
            btnPowerInsert.Size = new System.Drawing.Size(246, 28);
            btnPowerInsert.TabIndex = 13;
            btnPowerInsert.Text = "Select & Insert Power";
            btnPowerInsert.UseMnemonic = false;
            btnPowerInsert.UseVisualStyleBackColor = false;
            btnPowerInsert.Click += btnPowerInsert_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnCancel.Location = new System.Drawing.Point(777, 397);
            btnCancel.Margin = new System.Windows.Forms.Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 41);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += Cancel_Clicked;
            // 
            // btnOkay
            // 
            btnOkay.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOkay.Location = new System.Drawing.Point(683, 397);
            btnOkay.Margin = new System.Windows.Forms.Padding(4);
            btnOkay.Name = "btnOkay";
            btnOkay.Size = new System.Drawing.Size(88, 41);
            btnOkay.TabIndex = 11;
            btnOkay.Text = "Okay";
            btnOkay.UseVisualStyleBackColor = false;
            btnOkay.Click += Okay_Clicked;
            // 
            // label5
            // 
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.ForeColor = System.Drawing.Color.Black;
            label5.Location = new System.Drawing.Point(265, 272);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(203, 26);
            label5.TabIndex = 10;
            label5.Text = "Probability Expression:";
            // 
            // tbProbExpr
            // 
            tbProbExpr.Location = new System.Drawing.Point(265, 302);
            tbProbExpr.Margin = new System.Windows.Forms.Padding(4);
            tbProbExpr.Multiline = true;
            tbProbExpr.Name = "tbProbExpr";
            tbProbExpr.Size = new System.Drawing.Size(600, 80);
            tbProbExpr.TabIndex = 9;
            // 
            // label4
            // 
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label4.ForeColor = System.Drawing.Color.Black;
            label4.Location = new System.Drawing.Point(265, 158);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(203, 26);
            label4.TabIndex = 8;
            label4.Text = "Magnitude Expression:";
            // 
            // tbMagExpr
            // 
            tbMagExpr.Location = new System.Drawing.Point(265, 188);
            tbMagExpr.Margin = new System.Windows.Forms.Padding(4);
            tbMagExpr.Multiline = true;
            tbMagExpr.Name = "tbMagExpr";
            tbMagExpr.Size = new System.Drawing.Size(600, 80);
            tbMagExpr.TabIndex = 7;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.ForeColor = System.Drawing.Color.Black;
            label3.Location = new System.Drawing.Point(265, 44);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(203, 26);
            label3.TabIndex = 6;
            label3.Text = "Duration Expression:";
            // 
            // tbDurationExp
            // 
            tbDurationExp.Location = new System.Drawing.Point(265, 74);
            tbDurationExp.Margin = new System.Windows.Forms.Padding(4);
            tbDurationExp.Multiline = true;
            tbDurationExp.Name = "tbDurationExp";
            tbDurationExp.Size = new System.Drawing.Size(600, 80);
            tbDurationExp.TabIndex = 5;
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.Black;
            label2.Location = new System.Drawing.Point(12, 9);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(203, 26);
            label2.TabIndex = 4;
            label2.Text = "Expression Variables:";
            // 
            // lbCommandVars
            // 
            lbCommandVars.FormattingEnabled = true;
            lbCommandVars.ItemHeight = 15;
            lbCommandVars.Location = new System.Drawing.Point(12, 38);
            lbCommandVars.Margin = new System.Windows.Forms.Padding(4);
            lbCommandVars.Name = "lbCommandVars";
            lbCommandVars.Size = new System.Drawing.Size(245, 349);
            lbCommandVars.TabIndex = 1;
            // 
            // frmExpressionBuilder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(153, 170, 181);
            ClientSize = new System.Drawing.Size(882, 455);
            Controls.Add(panel1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "frmExpressionBuilder";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "frmExpressionBuilder";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbCommandVars;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbProbExpr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMagExpr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDurationExp;
        private System.Windows.Forms.Button btnPowerInsert;
    }
}
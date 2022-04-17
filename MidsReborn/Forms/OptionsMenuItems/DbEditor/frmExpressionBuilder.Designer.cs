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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPowerInsert = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbProbExpr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMagExpr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDurationExp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCommandVars = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnPowerInsert);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOkay);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tbProbExpr);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbMagExpr);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbDurationExp);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lbCommandVars);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.Azure;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(769, 370);
            this.panel1.TabIndex = 0;
            // 
            // btnPowerInsert
            // 
            this.btnPowerInsert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(101)))), ((int)(((byte)(242)))));
            this.btnPowerInsert.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPowerInsert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPowerInsert.Location = new System.Drawing.Point(10, 327);
            this.btnPowerInsert.Name = "btnPowerInsert";
            this.btnPowerInsert.Size = new System.Drawing.Size(211, 23);
            this.btnPowerInsert.TabIndex = 13;
            this.btnPowerInsert.Text = "Select & Insert Power";
            this.btnPowerInsert.UseMnemonic = false;
            this.btnPowerInsert.UseVisualStyleBackColor = false;
            this.btnPowerInsert.Click += new System.EventHandler(this.btnPowerInsert_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(40)))), ((int)(((byte)(18)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(674, 329);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 33);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(Cancel_Clicked);
            // 
            // btnOkay
            // 
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(78)))), ((int)(((byte)(237)))));
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOkay.Location = new System.Drawing.Point(593, 329);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 33);
            this.btnOkay.TabIndex = 11;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(Okay_Clicked);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(281, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 21);
            this.label5.TabIndex = 10;
            this.label5.Text = "Probability Expression:";
            // 
            // tbProbExpr
            // 
            this.tbProbExpr.Location = new System.Drawing.Point(284, 264);
            this.tbProbExpr.Multiline = true;
            this.tbProbExpr.Name = "tbProbExpr";
            this.tbProbExpr.Size = new System.Drawing.Size(465, 35);
            this.tbProbExpr.TabIndex = 9;
            this.tbProbExpr.GotFocus += ExprOnGotFocus;
            this.tbProbExpr.TextChanged += ExprOnTextChanged;
            this.tbProbExpr.MouseDown += ExpOnMouseDown;
            this.tbProbExpr.MouseMove += ExpOnMouseMove;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(281, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 21);
            this.label4.TabIndex = 8;
            this.label4.Text = "Magnitude Expression:";
            // 
            // tbMagExpr
            // 
            this.tbMagExpr.Location = new System.Drawing.Point(284, 154);
            this.tbMagExpr.Multiline = true;
            this.tbMagExpr.Name = "tbMagExpr";
            this.tbMagExpr.Size = new System.Drawing.Size(465, 35);
            this.tbMagExpr.TabIndex = 7;
            this.tbMagExpr.GotFocus += ExprOnGotFocus;
            this.tbMagExpr.TextChanged += ExprOnTextChanged;
            this.tbMagExpr.MouseDown += ExpOnMouseDown;
            this.tbMagExpr.MouseMove += ExpOnMouseMove;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(281, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 21);
            this.label3.TabIndex = 6;
            this.label3.Text = "Duration Expression:";
            // 
            // tbDurationExp
            // 
            this.tbDurationExp.Location = new System.Drawing.Point(284, 58);
            this.tbDurationExp.Multiline = true;
            this.tbDurationExp.Name = "tbDurationExp";
            this.tbDurationExp.Size = new System.Drawing.Size(465, 35);
            this.tbDurationExp.TabIndex = 5;
            this.tbDurationExp.GotFocus += ExprOnGotFocus;
            this.tbDurationExp.TextChanged += ExprOnTextChanged;
            this.tbDurationExp.MouseDown += ExpOnMouseDown;
            this.tbDurationExp.MouseMove += ExpOnMouseMove;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(10, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Expression Variables:";
            // 
            // lbCommandVars
            // 
            this.lbCommandVars.FormattingEnabled = true;
            this.lbCommandVars.Location = new System.Drawing.Point(10, 31);
            this.lbCommandVars.Name = "lbCommandVars";
            this.lbCommandVars.Size = new System.Drawing.Size(211, 290);
            this.lbCommandVars.TabIndex = 1;
            this.lbCommandVars.DoubleClick += LbCommandVarsOnDoubleClick;
            // 
            // frmExpressionBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(170)))), ((int)(((byte)(181)))));
            this.ClientSize = new System.Drawing.Size(769, 370);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmExpressionBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmExpressionBuilder";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
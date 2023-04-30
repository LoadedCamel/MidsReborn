namespace Mids_Reborn.Forms.Controls
{
    partial class PatchGenQuery
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDbGen = new System.Windows.Forms.Button();
            this.btnAppGen = new System.Windows.Forms.Button();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDbGen);
            this.panel1.Controls.Add(this.btnAppGen);
            this.panel1.Controls.Add(this.lblQuestion);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 68);
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(40)))), ((int)(((byte)(18)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(291, 32);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "None / Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDbGen
            // 
            this.btnDbGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(135)))), ((int)(((byte)(13)))));
            this.btnDbGen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDbGen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDbGen.Location = new System.Drawing.Point(152, 32);
            this.btnDbGen.Name = "btnDbGen";
            this.btnDbGen.Size = new System.Drawing.Size(115, 23);
            this.btnDbGen.TabIndex = 2;
            this.btnDbGen.Text = "Database";
            this.btnDbGen.UseVisualStyleBackColor = false;
            this.btnDbGen.Click += new System.EventHandler(this.btnDbGen_Click);
            // 
            // btnAppGen
            // 
            this.btnAppGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(78)))), ((int)(((byte)(237)))));
            this.btnAppGen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAppGen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppGen.Location = new System.Drawing.Point(10, 32);
            this.btnAppGen.Name = "btnAppGen";
            this.btnAppGen.Size = new System.Drawing.Size(115, 23);
            this.btnAppGen.TabIndex = 1;
            this.btnAppGen.Text = "Application";
            this.btnAppGen.UseVisualStyleBackColor = false;
            this.btnAppGen.Click += new System.EventHandler(this.btnAppGen_Click);
            // 
            // lblQuestion
            // 
            this.lblQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestion.Location = new System.Drawing.Point(10, 7);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(396, 22);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Text = "Which Type Of Patch Do You Wish To Generate?";
            this.lblQuestion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // PatchGenQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(420, 68);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Azure;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PatchGenQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PatchGen";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDbGen;
        private System.Windows.Forms.Button btnAppGen;
        private System.Windows.Forms.Label lblQuestion;
    }
}
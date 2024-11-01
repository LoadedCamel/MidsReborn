namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class ThemeCreator
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
            components = new System.ComponentModel.Container();
            chkIsDarkTheme = new System.Windows.Forms.CheckBox();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            panelSlots = new System.Windows.Forms.Panel();
            panelLevels = new System.Windows.Forms.Panel();
            panelHeadings = new System.Windows.Forms.Panel();
            panelTitle = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            tbName = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // chkIsDarkTheme
            // 
            chkIsDarkTheme.AutoSize = true;
            chkIsDarkTheme.Location = new System.Drawing.Point(106, 169);
            chkIsDarkTheme.Name = "chkIsDarkTheme";
            chkIsDarkTheme.Size = new System.Drawing.Size(100, 19);
            chkIsDarkTheme.TabIndex = 75;
            chkIsDarkTheme.Text = "Is Dark Theme";
            chkIsDarkTheme.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(84, 49);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 15);
            label5.TabIndex = 74;
            label5.Text = "Title Text:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(105, 133);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(35, 15);
            label4.TabIndex = 73;
            label4.Text = "Slots:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(98, 105);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(42, 15);
            label3.TabIndex = 72;
            label3.Text = "Levels:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(80, 77);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(60, 15);
            label2.TabIndex = 71;
            label2.Text = "Headings:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelSlots
            // 
            panelSlots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelSlots.Location = new System.Drawing.Point(146, 131);
            panelSlots.Name = "panelSlots";
            panelSlots.Size = new System.Drawing.Size(60, 23);
            panelSlots.TabIndex = 70;
            // 
            // panelLevels
            // 
            panelLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelLevels.Location = new System.Drawing.Point(146, 102);
            panelLevels.Name = "panelLevels";
            panelLevels.Size = new System.Drawing.Size(60, 23);
            panelLevels.TabIndex = 69;
            // 
            // panelHeadings
            // 
            panelHeadings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelHeadings.Location = new System.Drawing.Point(146, 74);
            panelHeadings.Name = "panelHeadings";
            panelHeadings.Size = new System.Drawing.Size(60, 23);
            panelHeadings.TabIndex = 68;
            // 
            // panelTitle
            // 
            panelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelTitle.Location = new System.Drawing.Point(146, 46);
            panelTitle.Name = "panelTitle";
            panelTitle.Size = new System.Drawing.Size(60, 23);
            panelTitle.TabIndex = 67;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 76;
            label1.Text = "Name:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbName
            // 
            tbName.Location = new System.Drawing.Point(60, 12);
            tbName.Name = "tbName";
            tbName.Size = new System.Drawing.Size(146, 23);
            tbName.TabIndex = 77;
            // 
            // button1
            // 
            button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            button1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button1.ForeColor = System.Drawing.Color.Black;
            button1.Location = new System.Drawing.Point(60, 206);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(103, 23);
            button1.TabIndex = 78;
            button1.Text = "Create";
            button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // ThemeCreator
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(218, 241);
            Controls.Add(button1);
            Controls.Add(tbName);
            Controls.Add(label1);
            Controls.Add(chkIsDarkTheme);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(panelSlots);
            Controls.Add(panelLevels);
            Controls.Add(panelHeadings);
            Controls.Add(panelTitle);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "ThemeCreator";
            Text = "ThemeCreator";
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox chkIsDarkTheme;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelSlots;
        private System.Windows.Forms.Panel panelLevels;
        private System.Windows.Forms.Panel panelHeadings;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
using Mids_Reborn.UI.Forms.Controls;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    partial class frmThemeDesigner
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
            panel1 = new ScrollToControlPanel();
            tbThemeName = new TextBox();
            lblThemeName = new Label();
            btnSave = new Button();
            btnClose = new Button();
            chkAutoApply = new CheckBox();
            btnApply = new Button();
            chkTopMostSelf = new CheckBox();
            label1 = new Label();
            chkTopMostParent = new CheckBox();
            label2 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(tbThemeName);
            panel1.Controls.Add(lblThemeName);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(589, 400);
            panel1.TabIndex = 0;
            // 
            // tbThemeName
            // 
            tbThemeName.BackColor = Color.FromArgb(18, 18, 18);
            tbThemeName.ForeColor = Color.Gainsboro;
            tbThemeName.Location = new Point(86, 25);
            tbThemeName.Name = "tbThemeName";
            tbThemeName.Size = new Size(187, 23);
            tbThemeName.TabIndex = 1;
            tbThemeName.TextChanged += tbThemeName_TextChanged;
            // 
            // lblThemeName
            // 
            lblThemeName.AutoSize = true;
            lblThemeName.Location = new Point(33, 28);
            lblThemeName.Name = "lblThemeName";
            lblThemeName.Size = new Size(46, 15);
            lblThemeName.TabIndex = 0;
            lblThemeName.Text = "Theme:";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(17, 412);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save Theme";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(120, 412);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 23);
            btnClose.TabIndex = 2;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // chkAutoApply
            // 
            chkAutoApply.AutoSize = true;
            chkAutoApply.Checked = true;
            chkAutoApply.CheckState = CheckState.Checked;
            chkAutoApply.Location = new Point(420, 415);
            chkAutoApply.Name = "chkAutoApply";
            chkAutoApply.Size = new Size(86, 19);
            chkAutoApply.TabIndex = 3;
            chkAutoApply.Text = "Auto Apply";
            chkAutoApply.UseVisualStyleBackColor = true;
            chkAutoApply.CheckedChanged += chkAutoApply_CheckedChanged;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(513, 412);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(65, 23);
            btnApply.TabIndex = 4;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Visible = false;
            btnApply.Click += btnApply_Click;
            // 
            // chkTopMostSelf
            // 
            chkTopMostSelf.AutoSize = true;
            chkTopMostSelf.Location = new Point(298, 403);
            chkTopMostSelf.Name = "chkTopMostSelf";
            chkTopMostSelf.Size = new Size(92, 19);
            chkTopMostSelf.TabIndex = 5;
            chkTopMostSelf.Text = "This window";
            chkTopMostSelf.UseVisualStyleBackColor = true;
            chkTopMostSelf.CheckedChanged += chkTopMostSelf_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(404, 416);
            label1.Name = "label1";
            label1.Size = new Size(10, 15);
            label1.TabIndex = 6;
            label1.Text = "|";
            // 
            // chkTopMostParent
            // 
            chkTopMostParent.AutoSize = true;
            chkTopMostParent.Location = new Point(298, 426);
            chkTopMostParent.Name = "chkTopMostParent";
            chkTopMostParent.Size = new Size(53, 19);
            chkTopMostParent.TabIndex = 7;
            chkTopMostParent.Text = "Main";
            chkTopMostParent.UseVisualStyleBackColor = true;
            chkTopMostParent.CheckedChanged += chkTopMostParent_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(225, 416);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 8;
            label2.Text = "Top Most:";
            // 
            // frmThemeDesigner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(589, 450);
            Controls.Add(label2);
            Controls.Add(chkTopMostParent);
            Controls.Add(label1);
            Controls.Add(chkTopMostSelf);
            Controls.Add(btnApply);
            Controls.Add(chkAutoApply);
            Controls.Add(btnClose);
            Controls.Add(btnSave);
            Controls.Add(panel1);
            DoubleBuffered = true;
            ForeColor = Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmThemeDesigner";
            Text = "Theme Designer";
            FormClosed += frmThemeDesigner_FormClosed;
            Load += frmThemeDesigner_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScrollToControlPanel panel1;
        private TextBox tbThemeName;
        private Label lblThemeName;
        private Button btnSave;
        private Button btnClose;
        private CheckBox chkAutoApply;
        private Button btnApply;
        private CheckBox chkTopMostSelf;
        private Label label1;
        private CheckBox chkTopMostParent;
        private Label label2;
    }
}
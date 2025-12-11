namespace Mids_Reborn.Forms
{
    partial class frmTemplateManage
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
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            chkSetNewAsActive = new System.Windows.Forms.CheckBox();
            btnCreateSelected = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            cbTemplateSelect = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            radioButton2 = new System.Windows.Forms.RadioButton();
            label3 = new System.Windows.Forms.Label();
            tbNewTemplateName = new System.Windows.Forms.TextBox();
            radioButton1 = new System.Windows.Forms.RadioButton();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            tabPage3 = new System.Windows.Forms.TabPage();
            btnUseTemplate = new System.Windows.Forms.Button();
            cbTemplateName = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            btnClose = new System.Windows.Forms.Button();
            btnModeSwitch = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(559, 284);
            tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            tabControl1.TabIndex = 0;
            tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(chkSetNewAsActive);
            tabPage1.Controls.Add(btnCreateSelected);
            tabPage1.Controls.Add(btnDelete);
            tabPage1.Controls.Add(cbTemplateSelect);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(radioButton2);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(tbNewTemplateName);
            tabPage1.Controls.Add(radioButton1);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new System.Drawing.Point(4, 27);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(551, 253);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Save";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkSetNewAsActive
            // 
            chkSetNewAsActive.AutoSize = true;
            chkSetNewAsActive.Location = new System.Drawing.Point(281, 190);
            chkSetNewAsActive.Name = "chkSetNewAsActive";
            chkSetNewAsActive.Size = new System.Drawing.Size(168, 19);
            chkSetNewAsActive.TabIndex = 10;
            chkSetNewAsActive.Text = "Use as new active template";
            chkSetNewAsActive.UseVisualStyleBackColor = true;
            // 
            // btnCreateSelected
            // 
            btnCreateSelected.Location = new System.Drawing.Point(147, 186);
            btnCreateSelected.Name = "btnCreateSelected";
            btnCreateSelected.Size = new System.Drawing.Size(120, 23);
            btnCreateSelected.TabIndex = 9;
            btnCreateSelected.Text = "Create selected";
            btnCreateSelected.UseVisualStyleBackColor = true;
            btnCreateSelected.Click += btnCreateSelected_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(349, 152);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(120, 23);
            btnDelete.TabIndex = 8;
            btnDelete.Text = "Delete selected";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // cbTemplateSelect
            // 
            cbTemplateSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTemplateSelect.FormattingEnabled = true;
            cbTemplateSelect.Location = new System.Drawing.Point(68, 152);
            cbTemplateSelect.Name = "cbTemplateSelect";
            cbTemplateSelect.Size = new System.Drawing.Size(260, 23);
            cbTemplateSelect.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(22, 155);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(42, 15);
            label4.TabIndex = 6;
            label4.Text = "Name:";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new System.Drawing.Point(6, 129);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(123, 19);
            radioButton2.TabIndex = 5;
            radioButton2.TabStop = true;
            radioButton2.Text = "Overwrite existing:";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(22, 93);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(42, 15);
            label3.TabIndex = 4;
            label3.Text = "Name:";
            // 
            // tbNewTemplateName
            // 
            tbNewTemplateName.Location = new System.Drawing.Point(68, 89);
            tbNewTemplateName.Name = "tbNewTemplateName";
            tbNewTemplateName.Size = new System.Drawing.Size(260, 23);
            tbNewTemplateName.TabIndex = 3;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new System.Drawing.Point(6, 69);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(52, 19);
            radioButton1.TabIndex = 2;
            radioButton1.TabStop = true;
            radioButton1.Text = "New:";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(6, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(518, 42);
            label2.TabIndex = 1;
            label2.Text = "Template will include pool dropdown selections, pool powers (but Epic), non-default inherents, temp/prestige/accolade, and Incarnates.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(6, 3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(207, 15);
            label1.TabIndex = 0;
            label1.Text = "Create template from current build:";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(btnUseTemplate);
            tabPage3.Controls.Add(cbTemplateName);
            tabPage3.Controls.Add(label6);
            tabPage3.Controls.Add(label5);
            tabPage3.Location = new System.Drawing.Point(4, 27);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(551, 253);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Load";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnUseTemplate
            // 
            btnUseTemplate.Location = new System.Drawing.Point(213, 115);
            btnUseTemplate.Name = "btnUseTemplate";
            btnUseTemplate.Size = new System.Drawing.Size(124, 23);
            btnUseTemplate.TabIndex = 10;
            btnUseTemplate.Text = "Use selected";
            btnUseTemplate.UseVisualStyleBackColor = true;
            btnUseTemplate.Click += btnUseTemplate_Click;
            // 
            // cbTemplateName
            // 
            cbTemplateName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTemplateName.FormattingEnabled = true;
            cbTemplateName.Location = new System.Drawing.Point(159, 75);
            cbTemplateName.Name = "cbTemplateName";
            cbTemplateName.Size = new System.Drawing.Size(260, 23);
            cbTemplateName.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(113, 78);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(42, 15);
            label6.TabIndex = 8;
            label6.Text = "Name:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(84, 52);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(115, 15);
            label5.TabIndex = 0;
            label5.Text = "Use build template:";
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.SystemColors.Control;
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(btnModeSwitch);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 242);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(559, 42);
            panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(242, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 3;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnModeSwitch
            // 
            btnModeSwitch.Location = new System.Drawing.Point(413, 10);
            btnModeSwitch.Name = "btnModeSwitch";
            btnModeSwitch.Size = new System.Drawing.Size(134, 23);
            btnModeSwitch.TabIndex = 2;
            btnModeSwitch.Text = "Template selection >";
            btnModeSwitch.UseVisualStyleBackColor = true;
            btnModeSwitch.Click += btnModeSwitch_Click;
            // 
            // frmTemplateManage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(559, 284);
            Controls.Add(panel1);
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmTemplateManage";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Manage Templates";
            Load += frmTemplateManage_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbNewTemplateName;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ComboBox cbTemplateSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbTemplateName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnModeSwitch;
        private System.Windows.Forms.Button btnCreateSelected;
        private System.Windows.Forms.CheckBox chkSetNewAsActive;
        private System.Windows.Forms.Button btnUseTemplate;
        private System.Windows.Forms.Button btnClose;
    }
}
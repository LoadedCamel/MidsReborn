namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class frmPowersetSelector
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
            rbSelectionType1 = new System.Windows.Forms.RadioButton();
            rbSelectionType2 = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            tbPsName = new System.Windows.Forms.TextBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            cbPsGroup = new System.Windows.Forms.ComboBox();
            cbPowerset = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // rbSelectionType1
            // 
            rbSelectionType1.AutoSize = true;
            rbSelectionType1.Location = new System.Drawing.Point(12, 17);
            rbSelectionType1.Name = "rbSelectionType1";
            rbSelectionType1.Size = new System.Drawing.Size(119, 20);
            rbSelectionType1.TabIndex = 0;
            rbSelectionType1.TabStop = true;
            rbSelectionType1.Text = "Select a powerset:";
            rbSelectionType1.UseVisualStyleBackColor = true;
            rbSelectionType1.CheckedChanged += rbSelectionType1_CheckedChanged;
            // 
            // rbSelectionType2
            // 
            rbSelectionType2.AutoSize = true;
            rbSelectionType2.Location = new System.Drawing.Point(12, 126);
            rbSelectionType2.Name = "rbSelectionType2";
            rbSelectionType2.Size = new System.Drawing.Size(141, 20);
            rbSelectionType2.TabIndex = 1;
            rbSelectionType2.TabStop = true;
            rbSelectionType2.Text = "Type its name directly:";
            rbSelectionType2.UseVisualStyleBackColor = true;
            rbSelectionType2.CheckedChanged += rbSelectionType2_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(43, 164);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(110, 16);
            label1.TabIndex = 2;
            label1.Text = "Powerset full name:";
            // 
            // tbPsName
            // 
            tbPsName.Location = new System.Drawing.Point(159, 161);
            tbPsName.Name = "tbPsName";
            tbPsName.Size = new System.Drawing.Size(373, 23);
            tbPsName.TabIndex = 3;
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(368, 196);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.TabIndex = 4;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(466, 196);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // cbPsGroup
            // 
            cbPsGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPsGroup.FormattingEnabled = true;
            cbPsGroup.Location = new System.Drawing.Point(32, 73);
            cbPsGroup.Name = "cbPsGroup";
            cbPsGroup.Size = new System.Drawing.Size(194, 24);
            cbPsGroup.TabIndex = 6;
            cbPsGroup.SelectedIndexChanged += cbPsGroup_SelectedIndexChanged;
            // 
            // cbPowerset
            // 
            cbPowerset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPowerset.FormattingEnabled = true;
            cbPowerset.Location = new System.Drawing.Point(307, 73);
            cbPowerset.Name = "cbPowerset";
            cbPowerset.Size = new System.Drawing.Size(194, 24);
            cbPowerset.TabIndex = 7;
            cbPowerset.SelectedIndexChanged += cbPowerset_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(32, 50);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 16);
            label2.TabIndex = 8;
            label2.Text = "Group:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(307, 50);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(58, 16);
            label3.TabIndex = 9;
            label3.Text = "Powerset:";
            // 
            // frmPowersetSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(558, 237);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cbPowerset);
            Controls.Add(cbPsGroup);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(tbPsName);
            Controls.Add(label1);
            Controls.Add(rbSelectionType2);
            Controls.Add(rbSelectionType1);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmPowersetSelector";
            ShowInTaskbar = false;
            Text = "Form1";
            Load += frmPowersetSelector_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton rbSelectionType1;
        private System.Windows.Forms.RadioButton rbSelectionType2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPsName;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbPsGroup;
        private System.Windows.Forms.ComboBox cbPowerset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
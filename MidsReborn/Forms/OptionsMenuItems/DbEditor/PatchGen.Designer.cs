namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class PatchGen
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
            formPages1 = new Controls.FormPages();
            page1 = new Controls.Page();
            label = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            btnDbGen = new System.Windows.Forms.Button();
            btnAppGen = new System.Windows.Forms.Button();
            page2 = new Controls.Page();
            label2 = new System.Windows.Forms.Label();
            btnCancel2 = new System.Windows.Forms.Button();
            btnDbGen2 = new System.Windows.Forms.Button();
            page3 = new Controls.Page();
            label3 = new System.Windows.Forms.Label();
            processLabel = new System.Windows.Forms.Label();
            progressBar = new Controls.ProgressBarEx();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            page2.SuspendLayout();
            page3.SuspendLayout();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Controls.Add(page3);
            formPages1.Dock = System.Windows.Forms.DockStyle.Fill;
            formPages1.Location = new System.Drawing.Point(0, 0);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.Pages.Add(page3);
            formPages1.SelectedPage = 1;
            formPages1.Size = new System.Drawing.Size(517, 106);
            formPages1.TabIndex = 0;
            // 
            // page1
            // 
            page1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page1.Anchor = System.Windows.Forms.AnchorStyles.None;
            page1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page1.Controls.Add(label);
            page1.Controls.Add(btnCancel);
            page1.Controls.Add(btnDbGen);
            page1.Controls.Add(btnAppGen);
            page1.Dock = System.Windows.Forms.DockStyle.Fill;
            page1.Location = new System.Drawing.Point(0, 0);
            page1.Name = "page1";
            page1.Size = new System.Drawing.Size(513, 102);
            page1.TabIndex = 0;
            // 
            // label
            // 
            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label.Location = new System.Drawing.Point(10, 17);
            label.Name = "label";
            label.Size = new System.Drawing.Size(493, 21);
            label.TabIndex = 5;
            label.Text = "Select the type of patch you wish to generate or click cancel to exit.\r\n\r\n";
            label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnCancel.Location = new System.Drawing.Point(388, 56);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(115, 23);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += Cancel_Click;
            // 
            // btnDbGen
            // 
            btnDbGen.BackColor = System.Drawing.Color.FromArgb(13, 135, 13);
            btnDbGen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnDbGen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnDbGen.Location = new System.Drawing.Point(206, 56);
            btnDbGen.Name = "btnDbGen";
            btnDbGen.Size = new System.Drawing.Size(115, 23);
            btnDbGen.TabIndex = 3;
            btnDbGen.Text = "Database";
            btnDbGen.UseVisualStyleBackColor = false;
            btnDbGen.Click += Database_Click;
            // 
            // btnAppGen
            // 
            btnAppGen.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            btnAppGen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnAppGen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnAppGen.Location = new System.Drawing.Point(85, 56);
            btnAppGen.Name = "btnAppGen";
            btnAppGen.Size = new System.Drawing.Size(115, 23);
            btnAppGen.TabIndex = 2;
            btnAppGen.Text = "Application";
            btnAppGen.UseVisualStyleBackColor = false;
            btnAppGen.Click += App_Click;
            // 
            // page2
            // 
            page2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page2.Anchor = System.Windows.Forms.AnchorStyles.None;
            page2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page2.Controls.Add(label2);
            page2.Controls.Add(btnCancel2);
            page2.Controls.Add(btnDbGen2);
            page2.Dock = System.Windows.Forms.DockStyle.Fill;
            page2.Location = new System.Drawing.Point(0, 0);
            page2.Name = "page2";
            page2.Size = new System.Drawing.Size(513, 102);
            page2.TabIndex = 1;
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(10, 17);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(493, 22);
            label2.TabIndex = 8;
            label2.Text = "Select continue to generate the database patch or cancel to exit.\r\n\r\n\r\n";
            label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnCancel2
            // 
            btnCancel2.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            btnCancel2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnCancel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnCancel2.Location = new System.Drawing.Point(388, 57);
            btnCancel2.Name = "btnCancel2";
            btnCancel2.Size = new System.Drawing.Size(115, 23);
            btnCancel2.TabIndex = 7;
            btnCancel2.Text = "Cancel";
            btnCancel2.UseVisualStyleBackColor = false;
            btnCancel2.Click += Cancel_Click;
            // 
            // btnDbGen2
            // 
            btnDbGen2.BackColor = System.Drawing.Color.FromArgb(13, 135, 13);
            btnDbGen2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnDbGen2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnDbGen2.Location = new System.Drawing.Point(206, 57);
            btnDbGen2.Name = "btnDbGen2";
            btnDbGen2.Size = new System.Drawing.Size(115, 23);
            btnDbGen2.TabIndex = 6;
            btnDbGen2.Text = "Continue";
            btnDbGen2.UseVisualStyleBackColor = false;
            btnDbGen2.Click += Database_Click;
            // 
            // page3
            // 
            page3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page3.Anchor = System.Windows.Forms.AnchorStyles.None;
            page3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page3.Controls.Add(label3);
            page3.Controls.Add(processLabel);
            page3.Controls.Add(progressBar);
            page3.Dock = System.Windows.Forms.DockStyle.Fill;
            page3.Location = new System.Drawing.Point(0, 0);
            page3.Name = "page3";
            page3.Size = new System.Drawing.Size(513, 102);
            page3.TabIndex = 2;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            label3.Location = new System.Drawing.Point(10, 7);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(493, 17);
            label3.TabIndex = 2;
            label3.Text = "This window will automatically close when the process is completed.";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // processLabel
            // 
            processLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            processLabel.Location = new System.Drawing.Point(10, 47);
            processLabel.Name = "processLabel";
            processLabel.Size = new System.Drawing.Size(493, 19);
            processLabel.TabIndex = 1;
            processLabel.Text = "Adding To Patch Container:";
            // 
            // progressBar
            // 
            progressBar.Border.Style = System.Windows.Forms.ButtonBorderStyle.Solid;
            progressBar.Border.Thickness = 2;
            progressBar.Border.Which = Forms.Controls.ProgressBarEx.ProgressBorder.BorderToDraw.All;
            progressBar.Colors.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            progressBar.Colors.BarEndColor = System.Drawing.Color.FromArgb(64, 78, 237);
            progressBar.Colors.BarStartColor = System.Drawing.Color.FromArgb(30, 144, 255);
            progressBar.Colors.BorderColor = System.Drawing.Color.WhiteSmoke;
            progressBar.Colors.TextColor = System.Drawing.Color.WhiteSmoke;
            progressBar.Location = new System.Drawing.Point(10, 69);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(493, 23);
            progressBar.Step = 1;
            progressBar.TabIndex = 0;
            // 
            // PatchGen
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(517, 106);
            Controls.Add(formPages1);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "PatchGen";
            Text = "PatchGen";
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            page2.ResumeLayout(false);
            page3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Controls.FormPages formPages1;
        private Controls.Page page1;
        private Controls.Page page2;
        private Controls.Page page3;
        private System.Windows.Forms.Button btnAppGen;
        private System.Windows.Forms.Button btnDbGen;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel2;
        private System.Windows.Forms.Button btnDbGen2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label processLabel;
        private Controls.ProgressBarEx progressBar;
    }
}
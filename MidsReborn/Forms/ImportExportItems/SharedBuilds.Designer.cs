using Mids_Reborn.Core.Base.Display;

namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class SharedBuilds
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
            alvShared = new Controls.AdvListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            advGroupBox2 = new Controls.AdvGroupBox();
            txtDescript = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            txtSecondary = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtPrimary = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtArchetype = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            txtExpiresOn = new System.Windows.Forms.TextBox();
            label20 = new System.Windows.Forms.Label();
            txtSchemaUrl = new System.Windows.Forms.TextBox();
            label21 = new System.Windows.Forms.Label();
            txtImageUrl = new System.Windows.Forms.TextBox();
            label22 = new System.Windows.Forms.Label();
            txtDownloadUrl = new System.Windows.Forms.TextBox();
            label23 = new System.Windows.Forms.Label();
            txtBuildName = new System.Windows.Forms.TextBox();
            label24 = new System.Windows.Forms.Label();
            menuStrip1 = new Controls.MidsMenuStrip();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editSelectedBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            refreshSelectedBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            advGroupBox2.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // alvShared
            // 
            alvShared.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2 });
            alvShared.Location = new System.Drawing.Point(12, 33);
            alvShared.Name = "alvShared";
            alvShared.Size = new System.Drawing.Size(356, 581);
            alvShared.TabIndex = 0;
            alvShared.UseCompatibleStateImageBehavior = false;
            alvShared.View = System.Windows.Forms.View.Details;
            alvShared.SelectedIndexChanged += SharedBuilds_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Build ID";
            columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Submitted/Updated Date";
            columnHeader2.Width = 170;
            // 
            // advGroupBox2
            // 
            advGroupBox2.BorderColor = System.Drawing.Color.DodgerBlue;
            advGroupBox2.Controls.Add(txtDescript);
            advGroupBox2.Controls.Add(label4);
            advGroupBox2.Controls.Add(txtSecondary);
            advGroupBox2.Controls.Add(label3);
            advGroupBox2.Controls.Add(txtPrimary);
            advGroupBox2.Controls.Add(label2);
            advGroupBox2.Controls.Add(txtArchetype);
            advGroupBox2.Controls.Add(label1);
            advGroupBox2.Controls.Add(txtExpiresOn);
            advGroupBox2.Controls.Add(label20);
            advGroupBox2.Controls.Add(txtSchemaUrl);
            advGroupBox2.Controls.Add(label21);
            advGroupBox2.Controls.Add(txtImageUrl);
            advGroupBox2.Controls.Add(label22);
            advGroupBox2.Controls.Add(txtDownloadUrl);
            advGroupBox2.Controls.Add(label23);
            advGroupBox2.Controls.Add(txtBuildName);
            advGroupBox2.Controls.Add(label24);
            advGroupBox2.Location = new System.Drawing.Point(374, 33);
            advGroupBox2.Name = "advGroupBox2";
            advGroupBox2.RoundedCorners.Enabled = true;
            advGroupBox2.RoundedCorners.Radius = 10;
            advGroupBox2.Size = new System.Drawing.Size(306, 581);
            advGroupBox2.TabIndex = 3;
            advGroupBox2.TabStop = false;
            advGroupBox2.Text = "Details";
            advGroupBox2.TitleColor = System.Drawing.Color.WhiteSmoke;
            advGroupBox2.TitleFont = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            // 
            // txtDescript
            // 
            txtDescript.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtDescript.Location = new System.Drawing.Point(24, 260);
            txtDescript.Multiline = true;
            txtDescript.Name = "txtDescript";
            txtDescript.ReadOnly = true;
            txtDescript.Size = new System.Drawing.Size(254, 97);
            txtDescript.TabIndex = 17;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(26, 242);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(70, 15);
            label4.TabIndex = 16;
            label4.Text = "Description:";
            // 
            // txtSecondary
            // 
            txtSecondary.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtSecondary.Location = new System.Drawing.Point(24, 207);
            txtSecondary.Name = "txtSecondary";
            txtSecondary.ReadOnly = true;
            txtSecondary.Size = new System.Drawing.Size(254, 23);
            txtSecondary.TabIndex = 15;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(24, 189);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(116, 15);
            label3.TabIndex = 14;
            label3.Text = "Secondary Powerset:";
            // 
            // txtPrimary
            // 
            txtPrimary.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtPrimary.Location = new System.Drawing.Point(24, 154);
            txtPrimary.Name = "txtPrimary";
            txtPrimary.ReadOnly = true;
            txtPrimary.Size = new System.Drawing.Size(254, 23);
            txtPrimary.TabIndex = 13;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(24, 136);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(102, 15);
            label2.TabIndex = 12;
            label2.Text = "Primary Powerset:";
            // 
            // txtArchetype
            // 
            txtArchetype.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtArchetype.Location = new System.Drawing.Point(24, 101);
            txtArchetype.Name = "txtArchetype";
            txtArchetype.ReadOnly = true;
            txtArchetype.Size = new System.Drawing.Size(254, 23);
            txtArchetype.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(24, 83);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(64, 15);
            label1.TabIndex = 10;
            label1.Text = "Archetype:";
            // 
            // txtExpiresOn
            // 
            txtExpiresOn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtExpiresOn.Location = new System.Drawing.Point(24, 550);
            txtExpiresOn.Name = "txtExpiresOn";
            txtExpiresOn.ReadOnly = true;
            txtExpiresOn.Size = new System.Drawing.Size(254, 23);
            txtExpiresOn.TabIndex = 9;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(26, 532);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(66, 15);
            label20.TabIndex = 8;
            label20.Text = "Expires On:";
            // 
            // txtSchemaUrl
            // 
            txtSchemaUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtSchemaUrl.Location = new System.Drawing.Point(24, 497);
            txtSchemaUrl.Name = "txtSchemaUrl";
            txtSchemaUrl.ReadOnly = true;
            txtSchemaUrl.Size = new System.Drawing.Size(254, 23);
            txtSchemaUrl.TabIndex = 7;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(24, 479);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(199, 15);
            label21.TabIndex = 6;
            label21.Text = "Schema URL (For use in HTML only):";
            // 
            // txtImageUrl
            // 
            txtImageUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtImageUrl.Location = new System.Drawing.Point(24, 443);
            txtImageUrl.Name = "txtImageUrl";
            txtImageUrl.ReadOnly = true;
            txtImageUrl.Size = new System.Drawing.Size(254, 23);
            txtImageUrl.TabIndex = 5;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(24, 425);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(67, 15);
            label22.TabIndex = 4;
            label22.Text = "Image URL:";
            // 
            // txtDownloadUrl
            // 
            txtDownloadUrl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtDownloadUrl.Location = new System.Drawing.Point(24, 390);
            txtDownloadUrl.Name = "txtDownloadUrl";
            txtDownloadUrl.ReadOnly = true;
            txtDownloadUrl.Size = new System.Drawing.Size(254, 23);
            txtDownloadUrl.TabIndex = 3;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(24, 372);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(88, 15);
            label23.TabIndex = 2;
            label23.Text = "Download URL:";
            // 
            // txtBuildName
            // 
            txtBuildName.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            txtBuildName.Location = new System.Drawing.Point(24, 47);
            txtBuildName.Name = "txtBuildName";
            txtBuildName.ReadOnly = true;
            txtBuildName.Size = new System.Drawing.Size(254, 23);
            txtBuildName.TabIndex = 1;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(24, 29);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(72, 15);
            label24.TabIndex = 0;
            label24.Text = "Build Name:";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { optionsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(692, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { editSelectedBuildToolStripMenuItem, refreshSelectedBuildToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            optionsToolStripMenuItem.Text = "Actions";
            // 
            // editSelectedBuildToolStripMenuItem
            // 
            editSelectedBuildToolStripMenuItem.Name = "editSelectedBuildToolStripMenuItem";
            editSelectedBuildToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            editSelectedBuildToolStripMenuItem.Text = "Edit Selected Build";
            editSelectedBuildToolStripMenuItem.Click += EditSelectedBuild_Click;
            // 
            // refreshSelectedBuildToolStripMenuItem
            // 
            refreshSelectedBuildToolStripMenuItem.Name = "refreshSelectedBuildToolStripMenuItem";
            refreshSelectedBuildToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            refreshSelectedBuildToolStripMenuItem.Text = "Refresh Selected Build";
            refreshSelectedBuildToolStripMenuItem.Click += RefreshSelectedBuild_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 621);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(692, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 5;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
            toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(297, 17);
            toolStripStatusLabel1.Text = "Select a build or choose an action item from the menu.";
            // 
            // SharedBuilds
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(692, 643);
            Controls.Add(statusStrip1);
            Controls.Add(advGroupBox2);
            Controls.Add(alvShared);
            Controls.Add(menuStrip1);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MainMenuStrip = menuStrip1;
            Name = "SharedBuilds";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Previously Shared Builds (Non-Expired)";
            advGroupBox2.ResumeLayout(false);
            advGroupBox2.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.AdvListView alvShared;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private Controls.AdvGroupBox advGroupBox2;
        private System.Windows.Forms.TextBox txtPrimary;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtArchetype;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExpiresOn;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtSchemaUrl;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtDownloadUrl;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtBuildName;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtSecondary;
        private System.Windows.Forms.Label label3;
        private Controls.MidsMenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSelectedBuildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshSelectedBuildToolStripMenuItem;
        private System.Windows.Forms.TextBox txtDescript;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}
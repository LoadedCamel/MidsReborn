using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    partial class frmDiscord
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDiscord));
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblDiscriminator = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctlAvatar1 = new midsControls.ctlAvatar();
            this.authNotice = new System.Windows.Forms.RichTextBox();
            this.authButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serverCombo = new System.Windows.Forms.ComboBox();
            this.channelCombo = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.defenseCheckedList = new System.Windows.Forms.CheckedListBox();
            this.resistCheckedList = new System.Windows.Forms.CheckedListBox();
            this.miscCheckedList = new System.Windows.Forms.CheckedListBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.btnImages = new System.Windows.Forms.ImageList(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.ForeColor = System.Drawing.SystemColors.Control;
            this.lblUsername.Location = new System.Drawing.Point(76, 17);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(93, 16);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "Unauthorized";
            // 
            // lblDiscriminator
            // 
            this.lblDiscriminator.AutoSize = true;
            this.lblDiscriminator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiscriminator.ForeColor = System.Drawing.SystemColors.Control;
            this.lblDiscriminator.Location = new System.Drawing.Point(76, 33);
            this.lblDiscriminator.Name = "lblDiscriminator";
            this.lblDiscriminator.Size = new System.Drawing.Size(42, 15);
            this.lblDiscriminator.TabIndex = 8;
            this.lblDiscriminator.Text = "#0000";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(28)))));
            this.panel1.Controls.Add(this.lblUsername);
            this.panel1.Controls.Add(this.ctlAvatar1);
            this.panel1.Controls.Add(this.lblDiscriminator);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(174, 71);
            this.panel1.TabIndex = 9;
            // 
            // ctlAvatar1
            // 
            this.ctlAvatar1.BackColor = System.Drawing.Color.White;
            this.ctlAvatar1.Image = global::Hero_Designer.Resources.defaultAvatar;
            this.ctlAvatar1.Location = new System.Drawing.Point(6, 3);
            this.ctlAvatar1.Name = "ctlAvatar1";
            this.ctlAvatar1.Size = new System.Drawing.Size(64, 64);
            this.ctlAvatar1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ctlAvatar1.TabIndex = 6;
            this.ctlAvatar1.TabStop = false;
            // 
            // authNotice
            // 
            this.authNotice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(28)))));
            this.authNotice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.authNotice.Cursor = System.Windows.Forms.Cursors.Default;
            this.authNotice.DetectUrls = false;
            this.authNotice.Enabled = false;
            this.authNotice.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authNotice.ForeColor = System.Drawing.SystemColors.Control;
            this.authNotice.Location = new System.Drawing.Point(208, 25);
            this.authNotice.Name = "authNotice";
            this.authNotice.ReadOnly = true;
            this.authNotice.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.authNotice.Size = new System.Drawing.Size(197, 54);
            this.authNotice.TabIndex = 10;
            this.authNotice.TabStop = false;
            this.authNotice.Text = "Please authorize Mids Reborn with your Discord account in order to enable this fe" +
    "ature.\n\n";
            this.authNotice.Visible = false;
            // 
            // authButton
            // 
            this.authButton.Enabled = false;
            this.authButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authButton.Location = new System.Drawing.Point(417, 41);
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(75, 23);
            this.authButton.TabIndex = 11;
            this.authButton.Text = "Authorize";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Visible = false;
            this.authButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(12, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(12, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Channel";
            // 
            // serverCombo
            // 
            this.serverCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverCombo.FormattingEnabled = true;
            this.serverCombo.Location = new System.Drawing.Point(12, 129);
            this.serverCombo.Name = "serverCombo";
            this.serverCombo.Size = new System.Drawing.Size(218, 21);
            this.serverCombo.TabIndex = 18;
            // 
            // channelCombo
            // 
            this.channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channelCombo.FormattingEnabled = true;
            this.channelCombo.Location = new System.Drawing.Point(12, 184);
            this.channelCombo.Name = "channelCombo";
            this.channelCombo.Size = new System.Drawing.Size(218, 21);
            this.channelCombo.TabIndex = 19;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.42548F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.57452F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 198F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.defenseCheckedList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.resistCheckedList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.miscCheckedList, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(236, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.3348F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.6652F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(556, 227);
            this.tableLayoutPanel1.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Defense";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(172, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "Resistance";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(359, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(193, 21);
            this.label5.TabIndex = 2;
            this.label5.Text = "Misc";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // defenseCheckedList
            // 
            this.defenseCheckedList.CheckOnClick = true;
            this.defenseCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defenseCheckedList.FormattingEnabled = true;
            this.defenseCheckedList.Items.AddRange(new object[] {
            "Smashing",
            "Lethal",
            "Fire",
            "Cold",
            "Energy",
            "Negative",
            "Psionic",
            "Melee",
            "Ranged",
            "AOE"});
            this.defenseCheckedList.Location = new System.Drawing.Point(4, 32);
            this.defenseCheckedList.Name = "defenseCheckedList";
            this.defenseCheckedList.Size = new System.Drawing.Size(161, 191);
            this.defenseCheckedList.TabIndex = 3;
            this.defenseCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // resistCheckedList
            // 
            this.resistCheckedList.CheckOnClick = true;
            this.resistCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resistCheckedList.FormattingEnabled = true;
            this.resistCheckedList.Items.AddRange(new object[] {
            "Smashing",
            "Lethal",
            "Fire",
            "Cold",
            "Energy",
            "Negative",
            "Toxic",
            "Psionic"});
            this.resistCheckedList.Location = new System.Drawing.Point(172, 32);
            this.resistCheckedList.Name = "resistCheckedList";
            this.resistCheckedList.Size = new System.Drawing.Size(180, 191);
            this.resistCheckedList.TabIndex = 4;
            this.resistCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // miscCheckedList
            // 
            this.miscCheckedList.CheckOnClick = true;
            this.miscCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.miscCheckedList.FormattingEnabled = true;
            this.miscCheckedList.Items.AddRange(new object[] {
            "Accuracy",
            "Damage",
            "Elusivity",
            "Endurance (Maximum)",
            "Endurance (Recovery)",
            "Endurance (Reduction)",
            "Endurance (Usage)",
            "Haste",
            "Hitpoints (Maximum)",
            "Hitpoints (Regeneration)",
            "ToHit"});
            this.miscCheckedList.Location = new System.Drawing.Point(359, 32);
            this.miscCheckedList.Name = "miscCheckedList";
            this.miscCheckedList.Size = new System.Drawing.Size(193, 191);
            this.miscCheckedList.TabIndex = 5;
            this.miscCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // submitButton
            // 
            this.submitButton.FlatAppearance.BorderSize = 0;
            this.submitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submitButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.submitButton.ForeColor = System.Drawing.SystemColors.Control;
            this.submitButton.ImageIndex = 0;
            this.submitButton.ImageList = this.btnImages;
            this.submitButton.Location = new System.Drawing.Point(686, 245);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(106, 25);
            this.submitButton.TabIndex = 23;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            this.submitButton.MouseEnter += new System.EventHandler(this.submitButton_MouseEnter);
            this.submitButton.MouseLeave += new System.EventHandler(this.submitButton_MouseLeave);
            // 
            // btnImages
            // 
            this.btnImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("btnImages.ImageStream")));
            this.btnImages.TransparentColor = System.Drawing.Color.Transparent;
            this.btnImages.Images.SetKeyName(0, "pSlot2.png");
            this.btnImages.Images.SetKeyName(1, "pSlot3.png");
            this.btnImages.Images.SetKeyName(2, "pSlot4.png");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(237, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(356, 15);
            this.label6.TabIndex = 22;
            this.label6.Text = "Note: You must select at least 1 stat in order to submit your build.";
            // 
            // frmDiscord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(802, 281);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.channelCombo);
            this.Controls.Add(this.serverCombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.authButton);
            this.Controls.Add(this.authNotice);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDiscord";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export as Default User";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private midsControls.ctlAvatar ctlAvatar1;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblDiscriminator;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox authNotice;
        private System.Windows.Forms.Button authButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox serverCombo;
        private System.Windows.Forms.ComboBox channelCombo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckedListBox defenseCheckedList;
        private System.Windows.Forms.CheckedListBox resistCheckedList;
        private System.Windows.Forms.CheckedListBox miscCheckedList;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.ImageList btnImages;
        private System.Windows.Forms.Label label6;
    }
}
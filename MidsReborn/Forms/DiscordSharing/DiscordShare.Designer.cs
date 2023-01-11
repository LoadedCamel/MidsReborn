using Mids_Reborn.Controls;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.DiscordSharing
{
    partial class DiscordShare
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
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.shareButton = new FontAwesome.Sharp.IconButton();
            this.refreshButton = new FontAwesome.Sharp.IconButton();
            this.channelCombo = new System.Windows.Forms.ComboBox();
            this.serverCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.ctlAvatar1 = new Mids_Reborn.Controls.ctlAvatar();
            this.lblDiscriminator = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.defenseCheckedList = new Mids_Reborn.Forms.Controls.CheckedListExt();
            this.resistCheckedList = new Mids_Reborn.Forms.Controls.CheckedListExt();
            this.miscCheckedList = new Mids_Reborn.Forms.Controls.CheckedListExt();
            this.formPages1 = new Mids_Reborn.Forms.Controls.FormPages();
            this.page1 = new Mids_Reborn.Forms.Controls.Page();
            this.page2 = new Mids_Reborn.Forms.Controls.Page();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.statusText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.formPages1.SuspendLayout();
            this.page1.SuspendLayout();
            this.page2.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView21.Location = new System.Drawing.Point(0, 0);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(788, 682);
            this.webView21.Source = new System.Uri("https://discord.com/api/oauth2/authorize?client_id=729018208824066171&redirect_ur" +
        "i=https%3A%2F%2Fmidsreborn.com%2F&response_type=code&scope=identify%20email", System.UriKind.Absolute);
            this.webView21.TabIndex = 0;
            this.webView21.ZoomFactor = 1D;
            // 
            // shareButton
            // 
            this.shareButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.shareButton.FlatAppearance.BorderSize = 0;
            this.shareButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.shareButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.shareButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.shareButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.shareButton.IconChar = FontAwesome.Sharp.IconChar.Discord;
            this.shareButton.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(101)))), ((int)(((byte)(242)))));
            this.shareButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.shareButton.IconSize = 92;
            this.shareButton.Location = new System.Drawing.Point(604, 530);
            this.shareButton.Name = "shareButton";
            this.shareButton.Size = new System.Drawing.Size(167, 84);
            this.shareButton.TabIndex = 31;
            this.shareButton.Text = "Share to Discord";
            this.shareButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.shareButton.UseVisualStyleBackColor = true;
            this.shareButton.Click += new System.EventHandler(this.shareButton_Click);
            this.shareButton.MouseEnter += new System.EventHandler(this.shareButton_MouseEnter);
            this.shareButton.MouseLeave += new System.EventHandler(this.shareButton_MouseLeave);
            // 
            // refreshButton
            // 
            this.refreshButton.FlatAppearance.BorderSize = 0;
            this.refreshButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.refreshButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.refreshButton.ForeColor = System.Drawing.Color.AliceBlue;
            this.refreshButton.IconChar = FontAwesome.Sharp.IconChar.Retweet;
            this.refreshButton.IconColor = System.Drawing.Color.Gainsboro;
            this.refreshButton.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.refreshButton.Location = new System.Drawing.Point(44, 233);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(122, 57);
            this.refreshButton.TabIndex = 30;
            this.refreshButton.Text = "Refresh Servers";
            this.refreshButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            this.refreshButton.MouseEnter += new System.EventHandler(this.refreshButton_MouseEnter);
            this.refreshButton.MouseLeave += new System.EventHandler(this.refreshButton_MouseLeave);
            // 
            // channelCombo
            // 
            this.channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.channelCombo.FormattingEnabled = true;
            this.channelCombo.Location = new System.Drawing.Point(11, 184);
            this.channelCombo.Name = "channelCombo";
            this.channelCombo.Size = new System.Drawing.Size(204, 23);
            this.channelCombo.TabIndex = 28;
            // 
            // serverCombo
            // 
            this.serverCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverCombo.FormattingEnabled = true;
            this.serverCombo.Location = new System.Drawing.Point(11, 120);
            this.serverCombo.Name = "serverCombo";
            this.serverCombo.Size = new System.Drawing.Size(204, 23);
            this.serverCombo.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(11, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Channel";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(11, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Server";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(8, 341);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(760, 23);
            this.label1.TabIndex = 24;
            this.label1.Text = "Build Description";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.richTextBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBox1.Location = new System.Drawing.Point(11, 367);
            this.richTextBox1.MaxLength = 4096;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(760, 157);
            this.richTextBox1.TabIndex = 23;
            this.richTextBox1.Text = "Enter your build description here ";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lblUsername);
            this.panel1.Controls.Add(this.ctlAvatar1);
            this.panel1.Controls.Add(this.lblDiscriminator);
            this.panel1.Location = new System.Drawing.Point(11, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 71);
            this.panel1.TabIndex = 22;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUsername.ForeColor = System.Drawing.Color.White;
            this.lblUsername.Location = new System.Drawing.Point(76, 16);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(128, 17);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "Unknown";
            // 
            // ctlAvatar1
            // 
            this.ctlAvatar1.BackColor = System.Drawing.Color.White;
            this.ctlAvatar1.Image = global::MRBResourceLib.Resources.defaultAvatar;
            this.ctlAvatar1.Location = new System.Drawing.Point(6, 3);
            this.ctlAvatar1.Name = "ctlAvatar1";
            this.ctlAvatar1.Size = new System.Drawing.Size(64, 64);
            this.ctlAvatar1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ctlAvatar1.TabIndex = 6;
            this.ctlAvatar1.TabStop = false;
            // 
            // lblDiscriminator
            // 
            this.lblDiscriminator.AutoSize = true;
            this.lblDiscriminator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDiscriminator.ForeColor = System.Drawing.Color.White;
            this.lblDiscriminator.Location = new System.Drawing.Point(76, 33);
            this.lblDiscriminator.Name = "lblDiscriminator";
            this.lblDiscriminator.Size = new System.Drawing.Size(42, 15);
            this.lblDiscriminator.TabIndex = 8;
            this.lblDiscriminator.Text = "#0000";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.42548F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.57452F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.defenseCheckedList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.resistCheckedList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.miscCheckedList, 2, 1);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(224, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.666667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.33334F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(547, 335);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 26);
            this.label3.TabIndex = 0;
            this.label3.Text = "Defense";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(148, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 26);
            this.label4.TabIndex = 1;
            this.label4.Text = "Resistance";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(308, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(235, 26);
            this.label5.TabIndex = 2;
            this.label5.Text = "Misc";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // defenseCheckedList
            // 
            this.defenseCheckedList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.defenseCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.defenseCheckedList.CheckOnClick = true;
            this.defenseCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defenseCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.defenseCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
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
            "AoE"});
            this.defenseCheckedList.Location = new System.Drawing.Point(4, 37);
            this.defenseCheckedList.Name = "defenseCheckedList";
            this.defenseCheckedList.Size = new System.Drawing.Size(137, 294);
            this.defenseCheckedList.TabIndex = 3;
            this.defenseCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // resistCheckedList
            // 
            this.resistCheckedList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.resistCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resistCheckedList.CheckOnClick = true;
            this.resistCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resistCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.resistCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
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
            this.resistCheckedList.Location = new System.Drawing.Point(148, 37);
            this.resistCheckedList.Name = "resistCheckedList";
            this.resistCheckedList.Size = new System.Drawing.Size(153, 294);
            this.resistCheckedList.TabIndex = 4;
            this.resistCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // miscCheckedList
            // 
            this.miscCheckedList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.miscCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.miscCheckedList.CheckOnClick = true;
            this.miscCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.miscCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.miscCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
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
            this.miscCheckedList.Location = new System.Drawing.Point(308, 37);
            this.miscCheckedList.Name = "miscCheckedList";
            this.miscCheckedList.Size = new System.Drawing.Size(235, 294);
            this.miscCheckedList.TabIndex = 5;
            this.miscCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // formPages1
            // 
            this.formPages1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.formPages1.Controls.Add(this.page1);
            this.formPages1.Controls.Add(this.page2);
            this.formPages1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formPages1.Location = new System.Drawing.Point(0, 0);
            this.formPages1.Name = "formPages1";
            this.formPages1.Pages.Add(this.page1);
            this.formPages1.Pages.Add(this.page2);
            this.formPages1.SelectedPage = 2;
            this.formPages1.Size = new System.Drawing.Size(788, 682);
            this.formPages1.TabIndex = 3;
            // 
            // page1
            // 
            this.page1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.page1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.page1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page1.Controls.Add(this.webView21);
            this.page1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page1.Location = new System.Drawing.Point(0, 0);
            this.page1.Name = "page1";
            this.page1.Size = new System.Drawing.Size(788, 682);
            this.page1.TabIndex = 0;
            // 
            // page2
            // 
            this.page2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.page2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.page2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page2.Controls.Add(this.tableLayoutPanel1);
            this.page2.Controls.Add(this.panel1);
            this.page2.Controls.Add(this.richTextBox1);
            this.page2.Controls.Add(this.label1);
            this.page2.Controls.Add(this.label6);
            this.page2.Controls.Add(this.label2);
            this.page2.Controls.Add(this.serverCombo);
            this.page2.Controls.Add(this.channelCombo);
            this.page2.Controls.Add(this.refreshButton);
            this.page2.Controls.Add(this.shareButton);
            this.page2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page2.Location = new System.Drawing.Point(0, 0);
            this.page2.Name = "page2";
            this.page2.Size = new System.Drawing.Size(788, 682);
            this.page2.TabIndex = 3;
            // 
            // statusPanel
            // 
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statusPanel.Controls.Add(this.statusText);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusPanel.Location = new System.Drawing.Point(0, 654);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.statusPanel.Size = new System.Drawing.Size(788, 28);
            this.statusPanel.TabIndex = 4;
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.statusText.Location = new System.Drawing.Point(3, 0);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(347, 21);
            this.statusText.TabIndex = 0;
            this.statusText.Text = "Login with your Discord account to proceed.";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DiscordShare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(788, 682);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.formPages1);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DiscordShare";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Share To Discord";
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.formPages1.ResumeLayout(false);
            this.page1.ResumeLayout(false);
            this.page2.ResumeLayout(false);
            this.page2.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private CheckedListExt defenseCheckedList;
        private CheckedListExt resistCheckedList;
        private CheckedListExt miscCheckedList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblUsername;
        private ctlAvatar ctlAvatar1;
        private System.Windows.Forms.Label lblDiscriminator;
        private System.Windows.Forms.ComboBox channelCombo;
        private System.Windows.Forms.ComboBox serverCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private FontAwesome.Sharp.IconButton refreshButton;
        private FontAwesome.Sharp.IconButton shareButton;
        private Controls.FormPages formPages1;
        private Page page1;
        private Page page2;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label statusText;
    }
}
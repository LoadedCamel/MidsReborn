using Mids_Reborn.Controls;
using Mids_Reborn.Forms.Controls;

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
            shareButton = new FontAwesome.Sharp.IconButton();
            refreshButton = new FontAwesome.Sharp.IconButton();
            channelCombo = new System.Windows.Forms.ComboBox();
            serverCombo = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            panel1 = new System.Windows.Forms.Panel();
            lblUsername = new System.Windows.Forms.Label();
            ctlAvatar1 = new ctlAvatar();
            lblDiscriminator = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            defenseCheckedList = new CheckedListExt();
            resistCheckedList = new CheckedListExt();
            miscCheckedList = new CheckedListExt();
            formPages1 = new Controls.FormPages();
            page1 = new Controls.Page();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            page2 = new Controls.Page();
            statusPanel = new System.Windows.Forms.Panel();
            statusText = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ctlAvatar1).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            page2.SuspendLayout();
            statusPanel.SuspendLayout();
            SuspendLayout();
            // 
            // shareButton
            // 
            shareButton.Cursor = System.Windows.Forms.Cursors.Hand;
            shareButton.FlatAppearance.BorderSize = 0;
            shareButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            shareButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            shareButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            shareButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            shareButton.IconChar = FontAwesome.Sharp.IconChar.Discord;
            shareButton.IconColor = System.Drawing.Color.FromArgb(88, 101, 242);
            shareButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            shareButton.IconSize = 92;
            shareButton.Location = new System.Drawing.Point(604, 530);
            shareButton.Name = "shareButton";
            shareButton.Size = new System.Drawing.Size(167, 84);
            shareButton.TabIndex = 31;
            shareButton.Text = "Share to Discord";
            shareButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            shareButton.UseVisualStyleBackColor = true;
            shareButton.Click += shareButton_Click;
            shareButton.MouseEnter += shareButton_MouseEnter;
            shareButton.MouseLeave += shareButton_MouseLeave;
            // 
            // refreshButton
            // 
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            refreshButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            refreshButton.ForeColor = System.Drawing.Color.AliceBlue;
            refreshButton.IconChar = FontAwesome.Sharp.IconChar.Retweet;
            refreshButton.IconColor = System.Drawing.Color.Gainsboro;
            refreshButton.IconFont = FontAwesome.Sharp.IconFont.Solid;
            refreshButton.Location = new System.Drawing.Point(44, 233);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new System.Drawing.Size(122, 57);
            refreshButton.TabIndex = 30;
            refreshButton.Text = "Refresh Servers";
            refreshButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            refreshButton.MouseEnter += refreshButton_MouseEnter;
            refreshButton.MouseLeave += refreshButton_MouseLeave;
            // 
            // channelCombo
            // 
            channelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            channelCombo.FormattingEnabled = true;
            channelCombo.Location = new System.Drawing.Point(11, 184);
            channelCombo.Name = "channelCombo";
            channelCombo.Size = new System.Drawing.Size(204, 23);
            channelCombo.TabIndex = 28;
            // 
            // serverCombo
            // 
            serverCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            serverCombo.FormattingEnabled = true;
            serverCombo.Location = new System.Drawing.Point(11, 120);
            serverCombo.Name = "serverCombo";
            serverCombo.Size = new System.Drawing.Size(204, 23);
            serverCombo.TabIndex = 27;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(11, 168);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 13);
            label2.TabIndex = 26;
            label2.Text = "Channel";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            label6.ForeColor = System.Drawing.Color.White;
            label6.Location = new System.Drawing.Point(11, 104);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(44, 13);
            label6.TabIndex = 25;
            label6.Text = "Server";
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(8, 341);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(760, 23);
            label1.TabIndex = 24;
            label1.Text = "Build Description";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = System.Drawing.Color.FromArgb(35, 39, 42);
            richTextBox1.DetectUrls = false;
            richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            richTextBox1.Location = new System.Drawing.Point(11, 367);
            richTextBox1.MaxLength = 4096;
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(760, 157);
            richTextBox1.TabIndex = 23;
            richTextBox1.Text = "Enter your build description here ";
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.Transparent;
            panel1.Controls.Add(lblUsername);
            panel1.Controls.Add(ctlAvatar1);
            panel1.Controls.Add(lblDiscriminator);
            panel1.Location = new System.Drawing.Point(11, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(207, 71);
            panel1.TabIndex = 22;
            // 
            // lblUsername
            // 
            lblUsername.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblUsername.ForeColor = System.Drawing.Color.White;
            lblUsername.Location = new System.Drawing.Point(76, 16);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(128, 17);
            lblUsername.TabIndex = 7;
            lblUsername.Text = "Unknown";
            // 
            // ctlAvatar1
            // 
            ctlAvatar1.BackColor = System.Drawing.Color.White;
            ctlAvatar1.Image = MRBResourceLib.Resources.defaultAvatar;
            ctlAvatar1.Location = new System.Drawing.Point(6, 3);
            ctlAvatar1.Name = "ctlAvatar1";
            ctlAvatar1.Size = new System.Drawing.Size(64, 64);
            ctlAvatar1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            ctlAvatar1.TabIndex = 6;
            ctlAvatar1.TabStop = false;
            // 
            // lblDiscriminator
            // 
            lblDiscriminator.AutoSize = true;
            lblDiscriminator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblDiscriminator.ForeColor = System.Drawing.Color.White;
            lblDiscriminator.Location = new System.Drawing.Point(76, 33);
            lblDiscriminator.Name = "lblDiscriminator";
            lblDiscriminator.Size = new System.Drawing.Size(42, 15);
            lblDiscriminator.TabIndex = 8;
            lblDiscriminator.Text = "#0000";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.42548F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.57452F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            tableLayoutPanel1.Controls.Add(label3, 0, 0);
            tableLayoutPanel1.Controls.Add(label4, 1, 0);
            tableLayoutPanel1.Controls.Add(label5, 2, 0);
            tableLayoutPanel1.Controls.Add(defenseCheckedList, 0, 1);
            tableLayoutPanel1.Controls.Add(resistCheckedList, 1, 1);
            tableLayoutPanel1.Controls.Add(miscCheckedList, 2, 1);
            tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            tableLayoutPanel1.Location = new System.Drawing.Point(224, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.666667F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.33334F));
            tableLayoutPanel1.Size = new System.Drawing.Size(547, 335);
            tableLayoutPanel1.TabIndex = 21;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = System.Windows.Forms.DockStyle.Fill;
            label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(4, 4);
            label3.Margin = new System.Windows.Forms.Padding(3);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(137, 26);
            label3.TabIndex = 0;
            label3.Text = "Defense";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = System.Windows.Forms.DockStyle.Fill;
            label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(148, 4);
            label4.Margin = new System.Windows.Forms.Padding(3);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(153, 26);
            label4.TabIndex = 1;
            label4.Text = "Resistance";
            label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = System.Windows.Forms.DockStyle.Fill;
            label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.ForeColor = System.Drawing.Color.White;
            label5.Location = new System.Drawing.Point(308, 4);
            label5.Margin = new System.Windows.Forms.Padding(3);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(235, 26);
            label5.TabIndex = 2;
            label5.Text = "Misc";
            label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // defenseCheckedList
            // 
            defenseCheckedList.BackColor = System.Drawing.Color.FromArgb(35, 39, 42);
            defenseCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            defenseCheckedList.CheckOnClick = true;
            defenseCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            defenseCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            defenseCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
            defenseCheckedList.FormattingEnabled = true;
            defenseCheckedList.Items.AddRange(new object[] { "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Psionic", "Melee", "Ranged", "AoE" });
            defenseCheckedList.Location = new System.Drawing.Point(4, 37);
            defenseCheckedList.Name = "defenseCheckedList";
            defenseCheckedList.Size = new System.Drawing.Size(137, 294);
            defenseCheckedList.TabIndex = 3;
            defenseCheckedList.ItemCheck += CheckedListBox_ItemCheck;
            // 
            // resistCheckedList
            // 
            resistCheckedList.BackColor = System.Drawing.Color.FromArgb(35, 39, 42);
            resistCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resistCheckedList.CheckOnClick = true;
            resistCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            resistCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            resistCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
            resistCheckedList.FormattingEnabled = true;
            resistCheckedList.Items.AddRange(new object[] { "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic" });
            resistCheckedList.Location = new System.Drawing.Point(148, 37);
            resistCheckedList.Name = "resistCheckedList";
            resistCheckedList.Size = new System.Drawing.Size(153, 294);
            resistCheckedList.TabIndex = 4;
            resistCheckedList.ItemCheck += CheckedListBox_ItemCheck;
            // 
            // miscCheckedList
            // 
            miscCheckedList.BackColor = System.Drawing.Color.FromArgb(35, 39, 42);
            miscCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            miscCheckedList.CheckOnClick = true;
            miscCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            miscCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            miscCheckedList.ForeColor = System.Drawing.Color.WhiteSmoke;
            miscCheckedList.FormattingEnabled = true;
            miscCheckedList.Items.AddRange(new object[] { "Accuracy", "Damage", "Elusivity", "Endurance (Maximum)", "Endurance (Recovery)", "Endurance (Reduction)", "Endurance (Usage)", "Haste", "Hitpoints (Maximum)", "Hitpoints (Regeneration)", "ToHit" });
            miscCheckedList.Location = new System.Drawing.Point(308, 37);
            miscCheckedList.Name = "miscCheckedList";
            miscCheckedList.Size = new System.Drawing.Size(235, 294);
            miscCheckedList.TabIndex = 5;
            miscCheckedList.ItemCheck += CheckedListBox_ItemCheck;
            // 
            // formPages1
            // 
            formPages1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Dock = System.Windows.Forms.DockStyle.Fill;
            formPages1.Location = new System.Drawing.Point(0, 0);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.SelectedPage = 1;
            formPages1.Size = new System.Drawing.Size(788, 682);
            formPages1.TabIndex = 3;
            // 
            // page1
            // 
            page1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page1.Anchor = System.Windows.Forms.AnchorStyles.None;
            page1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page1.Controls.Add(webView);
            page1.Dock = System.Windows.Forms.DockStyle.Fill;
            page1.Location = new System.Drawing.Point(0, 0);
            page1.Name = "page1";
            page1.Size = new System.Drawing.Size(788, 682);
            page1.TabIndex = 0;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Dock = System.Windows.Forms.DockStyle.Fill;
            webView.Location = new System.Drawing.Point(0, 0);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(788, 682);
            webView.Source = new System.Uri("https://discord.com/api/oauth2/authorize?client_id=729018208824066171&redirect_uri=https%3A%2F%2Fmidsreborn.com%2F&response_type=code&scope=identify%20email", System.UriKind.Absolute);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // page2
            // 
            page2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page2.Anchor = System.Windows.Forms.AnchorStyles.None;
            page2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page2.Controls.Add(tableLayoutPanel1);
            page2.Controls.Add(panel1);
            page2.Controls.Add(richTextBox1);
            page2.Controls.Add(label1);
            page2.Controls.Add(label6);
            page2.Controls.Add(label2);
            page2.Controls.Add(serverCombo);
            page2.Controls.Add(channelCombo);
            page2.Controls.Add(refreshButton);
            page2.Controls.Add(shareButton);
            page2.Dock = System.Windows.Forms.DockStyle.Fill;
            page2.Location = new System.Drawing.Point(0, 0);
            page2.Name = "page2";
            page2.Size = new System.Drawing.Size(788, 682);
            page2.TabIndex = 3;
            // 
            // statusPanel
            // 
            statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            statusPanel.Controls.Add(statusText);
            statusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusPanel.Location = new System.Drawing.Point(0, 654);
            statusPanel.Name = "statusPanel";
            statusPanel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            statusPanel.Size = new System.Drawing.Size(788, 28);
            statusPanel.TabIndex = 4;
            // 
            // statusText
            // 
            statusText.AutoSize = true;
            statusText.Dock = System.Windows.Forms.DockStyle.Fill;
            statusText.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            statusText.Location = new System.Drawing.Point(3, 0);
            statusText.Name = "statusText";
            statusText.Size = new System.Drawing.Size(347, 21);
            statusText.TabIndex = 0;
            statusText.Text = "Login with your Discord account to proceed.";
            statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DiscordShare
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(35, 39, 42);
            ClientSize = new System.Drawing.Size(788, 682);
            Controls.Add(statusPanel);
            Controls.Add(formPages1);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "DiscordShare";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Share To Discord";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ctlAvatar1).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            page2.ResumeLayout(false);
            page2.PerformLayout();
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
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
        private Controls.Page page1;
        private Controls.Page page2;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label statusText;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
    }
}
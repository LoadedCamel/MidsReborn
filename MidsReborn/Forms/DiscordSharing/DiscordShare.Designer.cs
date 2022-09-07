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
            this.tabControlAdv1 = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            this.tabPageAdv1 = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.tabPageAdv2 = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pg2_showPass = new FontAwesome.Sharp.IconButton();
            this.existingAccountButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.pg2_passBox = new System.Windows.Forms.TextBox();
            this.pg2_userBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageAdv3 = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pg3_showPass = new FontAwesome.Sharp.IconButton();
            this.forgotPasswordButton = new System.Windows.Forms.Button();
            this.loginButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.pg3_passBox = new System.Windows.Forms.TextBox();
            this.pg3_userBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPageAdv4 = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
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
            this.ctlAvatar1 = new ctlAvatar();
            this.lblDiscriminator = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.defenseCheckedList = new CheckedListExt();
            this.resistCheckedList = new CheckedListExt();
            this.miscCheckedList = new CheckedListExt();
            this.statusBarAdvPanel1 = new Syncfusion.Windows.Forms.Tools.StatusBarAdvPanel();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlAdv1)).BeginInit();
            this.tabControlAdv1.SuspendLayout();
            this.tabPageAdv1.SuspendLayout();
            this.tabPageAdv2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPageAdv3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPageAdv4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarAdvPanel1)).BeginInit();
            this.SuspendLayout();
            // 
            // webView21
            // 
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView21.Location = new System.Drawing.Point(0, 0);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(782, 590);
            this.webView21.Source = new System.Uri("https://discord.com/api/oauth2/authorize?client_id=729018208824066171&redirect_ur" +
        "i=https%3A%2F%2Fmidsreborn.com%2F&response_type=code&scope=identify%20email", System.UriKind.Absolute);
            this.webView21.TabIndex = 0;
            this.webView21.ZoomFactor = 1D;
            // 
            // tabControlAdv1
            // 
            this.tabControlAdv1.ActiveTabColor = System.Drawing.Color.Transparent;
            this.tabControlAdv1.BeforeTouchSize = new System.Drawing.Size(784, 616);
            this.tabControlAdv1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabControlAdv1.Controls.Add(this.tabPageAdv1);
            this.tabControlAdv1.Controls.Add(this.tabPageAdv2);
            this.tabControlAdv1.Controls.Add(this.tabPageAdv3);
            this.tabControlAdv1.Controls.Add(this.tabPageAdv4);
            this.tabControlAdv1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlAdv1.Location = new System.Drawing.Point(0, 0);
            this.tabControlAdv1.Name = "tabControlAdv1";
            this.tabControlAdv1.Padding = new System.Drawing.Point(0, 3);
            this.tabControlAdv1.Size = new System.Drawing.Size(784, 616);
            this.tabControlAdv1.TabIndex = 1;
            this.tabControlAdv1.ThemeName = "TabRenderer3D";
            this.tabControlAdv1.ThemeStyle.PrimitiveButtonStyle.DisabledNextPageImage = null;
            // 
            // tabPageAdv1
            // 
            this.tabPageAdv1.Controls.Add(this.webView21);
            this.tabPageAdv1.Image = null;
            this.tabPageAdv1.ImageSize = new System.Drawing.Size(16, 16);
            this.tabPageAdv1.Location = new System.Drawing.Point(1, 25);
            this.tabPageAdv1.Name = "tabPageAdv1";
            this.tabPageAdv1.ShowCloseButton = true;
            this.tabPageAdv1.Size = new System.Drawing.Size(782, 590);
            this.tabPageAdv1.TabForeColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageAdv1.TabIndex = 1;
            this.tabPageAdv1.TabVisible = false;
            this.tabPageAdv1.Text = "Page1";
            this.tabPageAdv1.ThemesEnabled = false;
            // 
            // tabPageAdv2
            // 
            this.tabPageAdv2.Controls.Add(this.panel2);
            this.tabPageAdv2.Image = null;
            this.tabPageAdv2.ImageSize = new System.Drawing.Size(16, 16);
            this.tabPageAdv2.Location = new System.Drawing.Point(1, 25);
            this.tabPageAdv2.Name = "tabPageAdv2";
            this.tabPageAdv2.ShowCloseButton = true;
            this.tabPageAdv2.Size = new System.Drawing.Size(782, 590);
            this.tabPageAdv2.TabForeColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageAdv2.TabIndex = 3;
            this.tabPageAdv2.TabVisible = false;
            this.tabPageAdv2.Text = "Page2";
            this.tabPageAdv2.ThemesEnabled = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.pg2_showPass);
            this.panel2.Controls.Add(this.existingAccountButton);
            this.panel2.Controls.Add(this.registerButton);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.pg2_passBox);
            this.panel2.Controls.Add(this.pg2_userBox);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(164, 117);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(472, 263);
            this.panel2.TabIndex = 4;
            // 
            // pg2_showPass
            // 
            this.pg2_showPass.FlatAppearance.BorderSize = 0;
            this.pg2_showPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pg2_showPass.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.pg2_showPass.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.pg2_showPass.IconFont = FontAwesome.Sharp.IconFont.Regular;
            this.pg2_showPass.IconSize = 32;
            this.pg2_showPass.Location = new System.Drawing.Point(383, 122);
            this.pg2_showPass.Name = "pg2_showPass";
            this.pg2_showPass.Size = new System.Drawing.Size(27, 20);
            this.pg2_showPass.TabIndex = 6;
            this.pg2_showPass.UseVisualStyleBackColor = false;
            // 
            // existingAccountButton
            // 
            this.existingAccountButton.FlatAppearance.BorderSize = 0;
            this.existingAccountButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.existingAccountButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.existingAccountButton.Location = new System.Drawing.Point(144, 209);
            this.existingAccountButton.Name = "existingAccountButton";
            this.existingAccountButton.Size = new System.Drawing.Size(201, 30);
            this.existingAccountButton.TabIndex = 5;
            this.existingAccountButton.Text = "I Already Have An Account";
            this.existingAccountButton.UseVisualStyleBackColor = true;
            this.existingAccountButton.Click += new System.EventHandler(this.existingAccountButton_Click);
            // 
            // registerButton
            // 
            this.registerButton.FlatAppearance.BorderSize = 0;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registerButton.Location = new System.Drawing.Point(185, 168);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(109, 35);
            this.registerButton.TabIndex = 4;
            this.registerButton.Text = "Register";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(83, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Username:";
            // 
            // pg2_passBox
            // 
            this.pg2_passBox.Location = new System.Drawing.Point(185, 122);
            this.pg2_passBox.MaxLength = 32;
            this.pg2_passBox.Name = "pg2_passBox";
            this.pg2_passBox.Size = new System.Drawing.Size(192, 20);
            this.pg2_passBox.TabIndex = 3;
            this.pg2_passBox.UseSystemPasswordChar = true;
            // 
            // pg2_userBox
            // 
            this.pg2_userBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pg2_userBox.Location = new System.Drawing.Point(185, 78);
            this.pg2_userBox.Name = "pg2_userBox";
            this.pg2_userBox.ReadOnly = true;
            this.pg2_userBox.Size = new System.Drawing.Size(192, 20);
            this.pg2_userBox.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(88, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "Password:";
            // 
            // tabPageAdv3
            // 
            this.tabPageAdv3.Controls.Add(this.panel3);
            this.tabPageAdv3.Image = null;
            this.tabPageAdv3.ImageSize = new System.Drawing.Size(16, 16);
            this.tabPageAdv3.Location = new System.Drawing.Point(1, 25);
            this.tabPageAdv3.Name = "tabPageAdv3";
            this.tabPageAdv3.ShowCloseButton = true;
            this.tabPageAdv3.Size = new System.Drawing.Size(782, 590);
            this.tabPageAdv3.TabForeColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageAdv3.TabIndex = 4;
            this.tabPageAdv3.TabVisible = false;
            this.tabPageAdv3.Text = "Page3";
            this.tabPageAdv3.ThemesEnabled = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.pg3_showPass);
            this.panel3.Controls.Add(this.forgotPasswordButton);
            this.panel3.Controls.Add(this.loginButton);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.pg3_passBox);
            this.panel3.Controls.Add(this.pg3_userBox);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Location = new System.Drawing.Point(164, 117);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(472, 263);
            this.panel3.TabIndex = 5;
            // 
            // pg3_showPass
            // 
            this.pg3_showPass.FlatAppearance.BorderSize = 0;
            this.pg3_showPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pg3_showPass.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.pg3_showPass.IconColor = System.Drawing.SystemColors.ControlLightLight;
            this.pg3_showPass.IconFont = FontAwesome.Sharp.IconFont.Regular;
            this.pg3_showPass.IconSize = 32;
            this.pg3_showPass.Location = new System.Drawing.Point(383, 123);
            this.pg3_showPass.Name = "pg3_showPass";
            this.pg3_showPass.Size = new System.Drawing.Size(27, 20);
            this.pg3_showPass.TabIndex = 7;
            this.pg3_showPass.UseVisualStyleBackColor = false;
            // 
            // forgotPasswordButton
            // 
            this.forgotPasswordButton.FlatAppearance.BorderSize = 0;
            this.forgotPasswordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.forgotPasswordButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.forgotPasswordButton.Location = new System.Drawing.Point(144, 209);
            this.forgotPasswordButton.Name = "forgotPasswordButton";
            this.forgotPasswordButton.Size = new System.Drawing.Size(201, 30);
            this.forgotPasswordButton.TabIndex = 5;
            this.forgotPasswordButton.Text = "Forgot Password";
            this.forgotPasswordButton.UseVisualStyleBackColor = true;
            this.forgotPasswordButton.Click += new System.EventHandler(this.forgotPasswordButton_Click);
            // 
            // loginButton
            // 
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.Location = new System.Drawing.Point(185, 168);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(109, 35);
            this.loginButton.TabIndex = 4;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(83, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Username:";
            // 
            // pg3_passBox
            // 
            this.pg3_passBox.Location = new System.Drawing.Point(185, 122);
            this.pg3_passBox.MaxLength = 32;
            this.pg3_passBox.Name = "pg3_passBox";
            this.pg3_passBox.Size = new System.Drawing.Size(192, 20);
            this.pg3_passBox.TabIndex = 3;
            this.pg3_passBox.UseSystemPasswordChar = true;
            // 
            // pg3_userBox
            // 
            this.pg3_userBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.pg3_userBox.Location = new System.Drawing.Point(185, 78);
            this.pg3_userBox.Name = "pg3_userBox";
            this.pg3_userBox.Size = new System.Drawing.Size(192, 20);
            this.pg3_userBox.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(88, 122);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 20);
            this.label10.TabIndex = 2;
            this.label10.Text = "Password:";
            // 
            // tabPageAdv4
            // 
            this.tabPageAdv4.Controls.Add(this.shareButton);
            this.tabPageAdv4.Controls.Add(this.refreshButton);
            this.tabPageAdv4.Controls.Add(this.channelCombo);
            this.tabPageAdv4.Controls.Add(this.serverCombo);
            this.tabPageAdv4.Controls.Add(this.label2);
            this.tabPageAdv4.Controls.Add(this.label6);
            this.tabPageAdv4.Controls.Add(this.label1);
            this.tabPageAdv4.Controls.Add(this.richTextBox1);
            this.tabPageAdv4.Controls.Add(this.panel1);
            this.tabPageAdv4.Controls.Add(this.tableLayoutPanel1);
            this.tabPageAdv4.Image = null;
            this.tabPageAdv4.ImageSize = new System.Drawing.Size(16, 16);
            this.tabPageAdv4.Location = new System.Drawing.Point(1, 25);
            this.tabPageAdv4.Name = "tabPageAdv4";
            this.tabPageAdv4.ShowCloseButton = true;
            this.tabPageAdv4.Size = new System.Drawing.Size(782, 590);
            this.tabPageAdv4.TabForeColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageAdv4.TabIndex = 2;
            this.tabPageAdv4.TabVisible = false;
            this.tabPageAdv4.Text = "Page4";
            this.tabPageAdv4.ThemesEnabled = false;
            // 
            // shareButton
            // 
            this.shareButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.shareButton.FlatAppearance.BorderSize = 0;
            this.shareButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.shareButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.shareButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.shareButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shareButton.IconChar = FontAwesome.Sharp.IconChar.Discord;
            this.shareButton.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(101)))), ((int)(((byte)(242)))));
            this.shareButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.shareButton.IconSize = 92;
            this.shareButton.Location = new System.Drawing.Point(604, 501);
            this.shareButton.Name = "shareButton";
            this.shareButton.Size = new System.Drawing.Size(167, 77);
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
            this.refreshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshButton.ForeColor = System.Drawing.Color.AliceBlue;
            this.refreshButton.IconChar = FontAwesome.Sharp.IconChar.Retweet;
            this.refreshButton.IconColor = System.Drawing.Color.Gainsboro;
            this.refreshButton.IconFont = FontAwesome.Sharp.IconFont.Solid;
            this.refreshButton.Location = new System.Drawing.Point(53, 211);
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
            this.channelCombo.Size = new System.Drawing.Size(204, 21);
            this.channelCombo.TabIndex = 28;
            // 
            // serverCombo
            // 
            this.serverCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverCombo.FormattingEnabled = true;
            this.serverCombo.Location = new System.Drawing.Point(11, 120);
            this.serverCombo.Name = "serverCombo";
            this.serverCombo.Size = new System.Drawing.Size(204, 21);
            this.serverCombo.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(11, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Server";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 312);
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
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBox1.Location = new System.Drawing.Point(11, 338);
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
            this.panel1.Location = new System.Drawing.Point(11, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 71);
            this.panel1.TabIndex = 22;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.ctlAvatar1.Image = Resources.defaultAvatar;
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
            this.lblDiscriminator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(224, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.666667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.33334F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(547, 293);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 22);
            this.label3.TabIndex = 0;
            this.label3.Text = "Defense";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(148, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 22);
            this.label4.TabIndex = 1;
            this.label4.Text = "Resistance";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(308, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(235, 22);
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
            this.defenseCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.defenseCheckedList.Location = new System.Drawing.Point(4, 33);
            this.defenseCheckedList.Name = "defenseCheckedList";
            this.defenseCheckedList.Size = new System.Drawing.Size(137, 256);
            this.defenseCheckedList.TabIndex = 3;
            this.defenseCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // resistCheckedList
            // 
            this.resistCheckedList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.resistCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resistCheckedList.CheckOnClick = true;
            this.resistCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resistCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.resistCheckedList.Location = new System.Drawing.Point(148, 33);
            this.resistCheckedList.Name = "resistCheckedList";
            this.resistCheckedList.Size = new System.Drawing.Size(153, 256);
            this.resistCheckedList.TabIndex = 4;
            this.resistCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // miscCheckedList
            // 
            this.miscCheckedList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.miscCheckedList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.miscCheckedList.CheckOnClick = true;
            this.miscCheckedList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.miscCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.miscCheckedList.Location = new System.Drawing.Point(308, 33);
            this.miscCheckedList.Name = "miscCheckedList";
            this.miscCheckedList.Size = new System.Drawing.Size(235, 256);
            this.miscCheckedList.TabIndex = 5;
            this.miscCheckedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBox_ItemCheck);
            // 
            // statusBarAdvPanel1
            // 
            this.statusBarAdvPanel1.BeforeTouchSize = new System.Drawing.Size(784, 28);
            this.statusBarAdvPanel1.Border3DStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.statusBarAdvPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBarAdvPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBarAdvPanel1.Location = new System.Drawing.Point(0, 615);
            this.statusBarAdvPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.statusBarAdvPanel1.Name = "statusBarAdvPanel1";
            this.statusBarAdvPanel1.Size = new System.Drawing.Size(784, 28);
            this.statusBarAdvPanel1.TabIndex = 2;
            this.statusBarAdvPanel1.Text = "Please authenticate your Discord account.";
            // 
            // OAuthGrant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(39)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(784, 643);
            this.Controls.Add(this.statusBarAdvPanel1);
            this.Controls.Add(this.tabControlAdv1);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OAuthGrant";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Share To Discord";
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlAdv1)).EndInit();
            this.tabControlAdv1.ResumeLayout(false);
            this.tabPageAdv1.ResumeLayout(false);
            this.tabPageAdv2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPageAdv3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPageAdv4.ResumeLayout(false);
            this.tabPageAdv4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctlAvatar1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarAdvPanel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Syncfusion.Windows.Forms.Tools.TabControlAdv tabControlAdv1;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv tabPageAdv1;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv tabPageAdv4;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv tabPageAdv2;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv tabPageAdv3;
        private Syncfusion.Windows.Forms.Tools.StatusBarAdvPanel statusBarAdvPanel1;
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
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button existingAccountButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox pg2_passBox;
        private System.Windows.Forms.TextBox pg2_userBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button forgotPasswordButton;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox pg3_passBox;
        private System.Windows.Forms.TextBox pg3_userBox;
        private System.Windows.Forms.Label label10;
        private FontAwesome.Sharp.IconButton pg2_showPass;
        private FontAwesome.Sharp.IconButton pg3_showPass;
        private FontAwesome.Sharp.IconButton refreshButton;
        private FontAwesome.Sharp.IconButton shareButton;
    }
}
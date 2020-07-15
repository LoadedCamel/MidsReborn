using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;

namespace Hero_Designer.Forms
{
    public partial class frmDiscord : Form
    {
        readonly frmMain _myParent;

        public frmDiscord(ref frmMain iParent)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Load += OnLoad;
            _myParent = iParent;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            UpdateForm();
        }

        void UpdateForm()
        {
            if (!MidsContext.Config.DiscordAuthorized)
            {
                Text = @"Export to Discord - Disabled";
                Size = new Size(520, 130);
                lblUsername.Text = @"Unauthorized";
                lblDiscriminator.Text = @"#0000";

                label1.Visible = false;
                label2.Visible = false;
                serverCombo.Visible = false;
                serverCombo.Enabled = false;
                channelCombo.Visible = false;
                channelCombo.Enabled = false;
                tableLayoutPanel1.Visible = false;
                tableLayoutPanel1.Enabled = false;

                authNotice.Visible = true;
                authNotice.Enabled = true;
                authButton.Visible = true;
                authButton.Enabled = true;
            }
            else
            {
                Size = new Size(818, 320);
                Text = @"Export to Discord - Enabled";
                label1.Visible = true;
                label2.Visible = true;
                serverCombo.Visible = true;
                serverCombo.Enabled = true;
                channelCombo.Visible = true;
                channelCombo.Enabled = true;
                tableLayoutPanel1.Visible = true;
                tableLayoutPanel1.Enabled = true;

                authNotice.Visible = false;
                authNotice.Enabled = false;
                authButton.Visible = false;
                authButton.Enabled = false;
                MidsContext.Config.DAuth.TryGetValue("access_token", out var token);
                clsOAuth.RequestUser(token?.ToString());
                clsOAuth.RequestServers(token?.ToString());
                PopulateUserData();
            }
            Update();
        }
        void PopulateUserData()
        {
            var userId = clsOAuth.GetCryptedValue("User", "id");
            var userAvatar = clsOAuth.GetCryptedValue("User", "avatar");
            using WebClient webClient = new WebClient();
            byte[] bytes = webClient.DownloadData($"https://cdn.discordapp.com/avatars/{userId}/{userAvatar}.png");
            using MemoryStream memoryStream = new MemoryStream(bytes);
            ctlAvatar1.Image = Image.FromStream(memoryStream);
            lblUsername.Text = clsOAuth.GetCryptedValue("User", "username");
            lblDiscriminator.Text = $@"# {clsOAuth.GetCryptedValue("User", "discriminator")}";
            //Text = $@"Export as {clsOAuth.GetCryptedValue("User", "username")}#{clsOAuth.GetCryptedValue("User", "discriminator")}";
        }

        void authButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.com/api/oauth2/authorize?client_id=729018208824066171&redirect_uri=http%3A%2F%2Flocalhost%3A60403&response_type=code&scope=identify%20guilds");
            clsOAuth.InitiateListener();
            UpdateForm();
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

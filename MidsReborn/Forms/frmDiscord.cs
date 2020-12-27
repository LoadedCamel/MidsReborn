using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmDiscord : Form
    {
        private readonly frmMain _myParent;
        private int checkedCount;
        private Dictionary<string, List<string>> SelectedStats { get; set; }
        private List<string> defList { get; set; }
        private List<string> resList { get; set; }
        private List<string> misList { get; set; }
        private List<ValidServer> ValidServers { get; set; } = new List<ValidServer>();

        private int GetCheckedCount()
        {
            return checkedCount;
        }

        private void SetCheckedCount(int value)
        {
            checkedCount = value;
        }

        public frmDiscord(ref frmMain iParent)
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw,
                true);
            Load += OnLoad;
            _myParent = iParent;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
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
                var isAuthed = MidsContext.ConfigSp.Auth.TryGetValue("DateTime", out var timestamp);
                if (isAuthed)
                {
                    var authTimestamp = DateTime.Parse(timestamp?.ToString());
                    var currentTimestamp = DateTime.Now;
                    var elapsed = (currentTimestamp - authTimestamp).TotalSeconds;
                    MidsContext.ConfigSp.Auth.TryGetValue("expires_in", out var expires);
                    if (elapsed >= Convert.ToDouble(expires))
                    {
                        MidsContext.ConfigSp.Auth.TryGetValue("refresh_token", out var refreshToken);
                        var isRefreshed = clsOAuth.RefreshToken(refreshToken?.ToString());
                        if (isRefreshed)
                        {
                            clsDiscord.MbLogin();
                            clsDiscord.ValidateServers();
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
                        }
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
                    }
                }

                MidsContext.ConfigSp.Auth.TryGetValue("access_token", out var token);
                clsOAuth.RequestUser(token?.ToString());
                clsOAuth.RequestServers(token?.ToString());
                if (MidsContext.Config.Registered == 0 && MidsContext.Config.DiscordAuthorized)
                {
                    clsDiscord.MbRegister();
                    //clsDiscord.ValidateServers();
                }
                //clsOAuth.RequestServerChannels(token?.ToString(), MidsContext.Config.DServerCount);
                //add servers to list
                PopulateUserData();
                PopulateServerData();
                StatInitializer();
            }

            Update();
        }

        private List<string> validServers(List<string> servers)
        {
            var dServers = new List<string>();
            return dServers;
        }

        private void StatInitializer()
        {
            SelectedStats = new Dictionary<string, List<string>>();
            defList = new List<string>();
            resList = new List<string>();
            misList = new List<string>();
            submitButton.ImageIndex = MidsContext.Character.IsHero() ? 0 : 2;
            SetCheckedCount(0);
            /*if (GetCheckedCount() == 0)
            {
                submitButton.Text = @"Cancel";
            }*/
        }

        private void PopulateUserData()
        {
            var userId = MidsContext.GetCryptedValue("User", "id");
            var userAvatar = MidsContext.GetCryptedValue("User", "avatar");
            using var webClient = new WebClient();
            var bytes = webClient.DownloadData($"https://cdn.discordapp.com/avatars/{userId}/{userAvatar}.png");
            using var memoryStream = new MemoryStream(bytes);
            ctlAvatar1.Image = Image.FromStream(memoryStream);
            lblUsername.Text = MidsContext.GetCryptedValue("User", "username");
            lblDiscriminator.Text = $@"# {MidsContext.GetCryptedValue("User", "discriminator")}";
            //Text = $@"Export as {MidsContext.GetCryptedValue("User", "username")}#{MidsContext.GetCryptedValue("User", "discriminator")}";
        }

        private void PopulateServerData()
        {
            foreach (var kvp in MidsContext.ConfigSp.ValidatedServers)
            {
                ValidServers.Add(new ValidServer { Name = kvp.Key, Channels = kvp.Value });
            }

            serverCombo.DisplayMember = "Name";
            serverCombo.ValueMember = "Channels";
            serverCombo.DataSource = new BindingList<ValidServer>(ValidServers);
        }

        private void serverCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverCombo.SelectedItem != null)
            {
                channelCombo.DataSource = serverCombo.SelectedValue;
            }
        }

        private void authButton_Click(object sender, EventArgs e)
        {
            Process.Start(
                "https://discord.com/api/oauth2/authorize?client_id=729018208824066171&redirect_uri=http%3A%2F%2Flocalhost%3A60403&response_type=code&scope=identify%20guilds");
            clsOAuth.InitiateListener();
            UpdateForm();
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var obj = (CheckedListBox)sender;
            switch (e.NewValue)
            {
                case CheckState.Checked:
                    SetCheckedCount(GetCheckedCount() + 1);
                    switch (obj.Name)
                    {
                        case "defenseCheckedList":
                            defList.Add(obj.Items[e.Index].ToString());
                            if (SelectedStats.ContainsKey("Defense"))
                            {
                                SelectedStats["Defense"] = defList;
                            }
                            else
                            {
                                SelectedStats.Add("Defense", defList);
                            }

                            break;
                        case "resistCheckedList":
                            resList.Add(obj.Items[e.Index].ToString());
                            if (SelectedStats.ContainsKey("Resistance"))
                            {
                                SelectedStats["Resistance"] = resList;
                            }
                            else
                            {
                                SelectedStats.Add("Resistance", resList);
                            }
                            break;
                        case "miscCheckedList":
                            {
                                var selectedItem = Regex.Replace(obj.Items[e.Index].ToString(), "[()]", "");
                                misList.Add(selectedItem);
                                if (SelectedStats.ContainsKey("Misc"))
                                {
                                    SelectedStats["Misc"] = misList;
                                }
                                else
                                {
                                    SelectedStats.Add("Misc", misList);
                                }
                                break;
                            }
                    }
                    break;
                case CheckState.Unchecked:
                    SetCheckedCount(GetCheckedCount() - 1);
                    switch (obj.Name)
                    {
                        case "defenseCheckedList":
                            defList.Remove(obj.Items[e.Index].ToString());
                            foreach (var list in from kvp in SelectedStats from list in SelectedStats.Values where kvp.Key == "Defense" select list)
                            {
                                list.Remove(obj.Items[e.Index].ToString());
                            }
                            break;
                        case "resistCheckedList":
                            resList.Remove(obj.Items[e.Index].ToString());
                            foreach (var list in from kvp in SelectedStats from list in SelectedStats.Values where kvp.Key == "Resistance" select list)
                            {
                                list.Remove(obj.Items[e.Index].ToString());
                            }
                            break;
                        case "miscCheckedList":
                            {
                                var selectedItem = Regex.Replace(obj.Items[e.Index].ToString(), "[()]", "");
                                misList.Remove(selectedItem);
                                foreach (var list in from kvp in SelectedStats from list in SelectedStats.Values where kvp.Key == "Misc" select list)
                                {
                                    list.Remove(obj.Items[e.Index].ToString());
                                }
                                break;
                            }
                    }
                    break;
                case CheckState.Indeterminate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (GetCheckedCount() > 0)
            {
                submitButton.Text = @"Submit";
            }
        }

        private void submitButton_MouseEnter(object sender, EventArgs e)
        {
            submitButton.ImageIndex = MidsContext.Character.IsHero() ? 1 : 3;
        }

        private void submitButton_MouseLeave(object sender, EventArgs e)
        {
            submitButton.ImageIndex = MidsContext.Character.IsHero() ? 0 : 2;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            if (GetCheckedCount() > 9)
            {
                MessageBox.Show("You have selected too many stats.\r\nNo more than 9 stats can be selected when submitting a build.", @"Unable to Submit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (GetCheckedCount() > -1 && GetCheckedCount() < 10)
            {
                clsDiscord.GatherData(SelectedStats, serverCombo.GetItemText(serverCombo.SelectedItem), channelCombo.GetItemText(channelCombo.SelectedItem));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using RestSharp;
using static Mids_Reborn.Forms.DiscordSharing.Structs;

namespace Mids_Reborn.Forms.DiscordSharing
{
    public partial class DiscordShare : Form
    {
        private Models.OAuthModel? _oAuth;
        private Models.MbAuthModel _mrbAuth;
        private Models.DiscordUserModel _discordUser;
        private Models.MbServersResponseModel _mbServers;
        private int _checkedCount;
        private Dictionary<string, List<string>> SelectedStats { get; set; } = new();
        private List<string> DefList { get; set; } = new();
        private List<string> ResList { get; set; } = new();
        private List<string> MisList { get; set; } = new();
        private readonly Discord _discord;
        private readonly MidsBot _midsBot;
        private readonly DataStore _dataStore;

        public DiscordShare()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _discord = new Discord(new RestClient(new RestClientOptions("https://discord.com/api/v10")));
            _midsBot = new MidsBot(new RestClient(new RestClientOptions("https://api.midsreborn.com")));
            _dataStore = new DataStore();
            Load += OnLoad;
            Closing += OnClosing;
            InitializeComponent();
            serverCombo.SelectedIndexChanged += ServerComboOnSelectedIndexChanged;
            webView21.SourceChanged += WebView21OnSourceChanged;
        }

        private void SetTabPage(int page)
        {
            //tabControlAdv1.SelectedIndex = page;
        }

        private void SetStatusText(string text)
        {
            //statusBarAdvPanel1.Text = text;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            _dataStore.Dispose();
        }

        private async void OnLoad(object? sender, EventArgs e)
        {
            if (MidsContext.Config is { Authorized: false })
            {
                SetTabPage(0);
                return;
            }

            _oAuth = await _dataStore.Retrieve<Models.OAuthModel>();
            _discordUser = await _dataStore.Retrieve<Models.DiscordUserModel>();
            var dTokenExpiry = DateTimeOffset.Parse(_oAuth.Expirey).UtcDateTime;
            if (DateTime.Now < dTokenExpiry.Subtract(TimeSpan.FromHours(1)) && DateTime.UtcNow > dTokenExpiry.Subtract(TimeSpan.FromHours(8)))
            {
                _oAuth = await _discord.RefreshAccessToken(_oAuth.RefreshToken);
                _discordUser = await _discord.GetUserInfo(_oAuth?.AccessToken);
                await _dataStore.Repsert(_oAuth);
                await _dataStore.Repsert(_discordUser);
            }
            else if (DateTime.UtcNow > dTokenExpiry)
            {
                SetTabPage(0);
                return;
            }

            if (MidsContext.Config is { Registered: false })
            {
                SetTabPage(1);
                return;
            }

            _mrbAuth = await _dataStore.Retrieve<Models.MbAuthModel>();
            var mTokenExpiry = DateTimeOffset.Parse(_mrbAuth.RefreshTokenExpiration).UtcDateTime;
            if (DateTime.UtcNow < mTokenExpiry.Subtract(TimeSpan.FromHours(1)) && DateTime.UtcNow > mTokenExpiry.Subtract(TimeSpan.FromHours(8)))
            {
                _mrbAuth = await _midsBot.RequestAccessToken(_discordUser.DiscordId, pg2_passBox.Text);
                if (!string.IsNullOrWhiteSpace(_mrbAuth.Message))
                {
                    SetTabPage(2);
                    return;
                }
                await _dataStore.Repsert(_mrbAuth);
                return;
            }

            if (DateTime.UtcNow >= mTokenExpiry)
            {
                SetTabPage(2);
                return;
            }

            _mrbAuth = await _midsBot.RefreshAccessToken(_discordUser.DiscordId, _mrbAuth.RefreshToken);
            await _dataStore.Repsert(_mrbAuth);
            SetTabPage(3);
        }

        // Page 0 - Primary

        private async void WebView21OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (!e.IsNewDocument)
            {
                return;
            }

            var currentUrl = webView21.Source;
            if (currentUrl.Query.Contains("?code="))
            {
                var redirectUrl = Regex.Replace(currentUrl.ToString(), "(\\?.*)", string.Empty);
                var accessCode = Regex.Replace(currentUrl.Query, "(\\?code=)", string.Empty);
                _oAuth = await _discord.RequestAccessToken(accessCode, redirectUrl);
                if (_oAuth?.AccessToken == null) return;
                _discordUser = await _discord.GetUserInfo(_oAuth?.AccessToken);
                if (!_discordUser.Bot)
                {
                    if (_discordUser.Verified)
                    {
                        await _dataStore.Repsert(_oAuth);
                        await _dataStore.Repsert(_discordUser);
                        if (MidsContext.Config != null) MidsContext.Config.Authorized = true;
                        SetTabPage(1);
                    }
                    else
                    {
                        await _discord.RevokeAccessToken(_oAuth?.AccessToken);
                        SetStatusText(
                            @"Access Denied: Only Discord users with a verified email address are allowed to use this feature.");
                    }
                }
                else
                {
                    await _discord.RevokeAccessToken(_oAuth?.AccessToken);
                    SetStatusText(@"Access Denied: Bot accounts are not allowed to use this feature.");
                }
            }
            else if (currentUrl.Query.Contains("access_denied"))
            {
                SetStatusText(@"Access Denied: You must allow Mids Reborn to verify your Discord account before you can use this feature.");
            }
        }

        /*private async void TabControlAdv1OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (tabControlAdv1.SelectedIndex)
            {
                case 0:
                    SetStatusText(@"Please verify your Discord account.");
                    break;
                case 1:
                    pg2_userBox.Text = _discordUser.Username;
                    SetStatusText(@"Please create your password or select 'I Already Have An Account'.");
                    break;
                case 2:
                    pg3_userBox.Text = _discordUser.Username;
                    SetStatusText(@"Please enter your login information or select 'Forgot Password' to reset your password.");
                    break;
                case 3:
                    lblUsername.Text = _discordUser.Username;
                    lblDiscriminator.Text = $@"#{_discordUser.Discriminator}";
                    ctlAvatar1.Image = await Discord.GetUserAvatar(_discordUser.DiscordId, _discordUser.Avatar);
                    await LoadServerData();
                    SetStatusText(@"Ready to share your build? Select from the options above then click share.");
                    break;
            }
        }
        */

        // Page 1
        private async void registerButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pg2_passBox.Text))
            {
                SetStatusText("Error: Password cannot be blank.");
                return;
            }

            var regResult = await _midsBot.RegisterAsync(_discordUser.DiscordId, _discordUser.Username, _discordUser.Discriminator, _discordUser.Email, pg2_passBox.Text);

            if (!regResult.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(regResult.Message) && regResult.Message.Contains("405"))
                {
                    SetStatusText(regResult.Message);
                    return;
                }

                MessageBox.Show(regResult.Message, @"Registration Error", MessageBoxButtons.OK);
                return;
            }

            _mrbAuth = await _midsBot.RequestAccessToken(_discordUser.DiscordId, pg2_passBox.Text);
            if (!string.IsNullOrWhiteSpace(_mrbAuth.Message))
            {
                SetStatusText(_mrbAuth.Message);
                return;
            }

            await _dataStore.Repsert(_mrbAuth);
            if (MidsContext.Config != null) MidsContext.Config.Registered = true;
            SetTabPage(3);
        }

        private void existingAccountButton_Click(object sender, EventArgs e)
        {
            SetTabPage(2);
        }


        // Page 2
        private async void loginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pg3_passBox.Text))
            {
                SetStatusText("Error: Password cannot be blank.");
                return;
            }

            _mrbAuth = await _midsBot.RequestAccessToken(_discordUser.DiscordId, pg3_passBox.Text);
            if (!string.IsNullOrWhiteSpace(_mrbAuth.Message))
            {
                SetStatusText(_mrbAuth.Message);
                return;
            }

            await _dataStore.Repsert(_mrbAuth);
            if (MidsContext.Config != null) MidsContext.Config.Registered = true;
            SetTabPage(3);
        }

        private async void forgotPasswordButton_Click(object sender, EventArgs e)
        {
            var tokenResponse = await _midsBot.GetResetToken(_discordUser.DiscordId, _discordUser.Email);
            if (string.IsNullOrWhiteSpace(tokenResponse.Message))
            {
                if (!string.IsNullOrWhiteSpace(tokenResponse.ResetToken))
                {
                    var newPass = InputBox.Show("Please enter your new password", "New Password", true, "your new password", InputBox.InputBoxIcon.Info, inputBox_Validating);
                    if (!newPass.OK)
                    {
                        return;
                    }

                    var resetRequest = await _midsBot.ResetPassword(_discordUser.DiscordId, newPass.Text, tokenResponse.ResetToken);
                    if (resetRequest.Succeeded)
                    {
                        MessageBox.Show(@"Your password has successfully been reset.", @"Password Reset", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0) return;
            e.Cancel = true;
            e.Message = "Required";
        }

        // Page 3 - Submission

        private async Task LoadServerData()
        {
            _mbServers = await _midsBot.GetUserChannels(_mrbAuth.AccessToken, _discordUser.DiscordId);
            if (_mbServers.AuthorizedServers.Any())
            {
                serverCombo.DataSource = new BindingSource(_mbServers.AuthorizedServers, null);
                serverCombo.DisplayMember = "ServerName";
                serverCombo.ValueMember = "ServerId";
                serverCombo.SelectedIndex = 0;
            }
        }

        private void ServerComboOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (serverCombo.SelectedItem == null)
            {
                return;
            }

            var selectedServer = serverCombo.SelectedItem is AuthorizedServer authorizedServer ? authorizedServer : default;
            var channels = _mbServers.AuthorizedServers.Where(x => x.ServerId == selectedServer.ServerId).SelectMany(x => x.Channels).ToList();
            channelCombo.DataSource = null;
            channelCombo.DataSource = new BindingSource(channels, null);
            channelCombo.DisplayMember = "ChannelName";
            channelCombo.ValueMember = "ChannelId";
            channelCombo.SelectedIndex = 0;
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            await LoadServerData();
        }

        private void refreshButton_MouseEnter(object sender, EventArgs e)
        {
            refreshButton.IconColor = Color.FromArgb(87,242,135);
        }

        private void refreshButton_MouseLeave(object sender, EventArgs e)
        {
            refreshButton.IconColor = Color.AliceBlue;
        }

        private void shareButton_MouseEnter(object sender, EventArgs e)
        {
            shareButton.IconColor = Color.AliceBlue;
        }

        private void shareButton_MouseLeave(object sender, EventArgs e)
        {
            shareButton.IconColor = Color.FromArgb(88, 101, 242);
        }
        
        private static void PerformValidation(out Validation validation)
        {
            if (MidsContext.Character == null || MidsContext.Character.Archetype == null)
            {
                validation = new Validation { IsValid = false, Message = @"You must create a character and select some powers first." };
            }
            else if (MidsContext.Character != null && string.IsNullOrWhiteSpace(MidsContext.Character.Name))
            {
                validation = new Validation { IsValid = false, Message = @"You must specify a character name first." };
            }
            else
            {
                validation = new Validation { IsValid = true, Message = "" };
            }
        }

        private async void shareButton_Click(object sender, EventArgs e)
        {
            PerformValidation(out var validation);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message, @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var subModel = new Models.MbSubmissionModel
            {
                Id = Convert.ToUInt64(_discordUser.DiscordId),
                Guild = Convert.ToUInt64(serverCombo.SelectedValue),
                Channel = Convert.ToUInt64(channelCombo.SelectedValue),
                BuildName = MidsContext.Character.Name,
                Description = richTextBox1.Text,
                Level = MidsContext.Character.Level + 1,
                Archetype = MidsContext.Character.Archetype.DisplayName,
                Primary = MidsContext.Character.Powersets[0].DisplayName,
                Secondary = MidsContext.Character.Powersets[1].DisplayName,
                Stats = await GetStats(SelectedStats),
                ByteString = MidsCharacterFileFormat.MxDGenerateByteString(true)
            };
            var subResponse = await _midsBot.SubmitBuild(_mrbAuth.AccessToken, subModel);
            if (subResponse.Succeeded)
            {
                MessageBox.Show($@"Your build was successfully shared in #{channelCombo.Text} on {serverCombo.Text}, you should see it momentarily.", @"Success!", MessageBoxButtons.OK);
            }
        }

        private static async Task<Dictionary<string, string>> GetStats(Dictionary<string, List<string>> selectedStats)
        {
            var stats = new Dictionary<string, string>();
            var gatherData = new Dictionary<string, Dictionary<string, string>>();
            var totalStat = MidsContext.Character.Totals;
            var displayStat = MidsContext.Character.DisplayStats;
            var statDictionary = new Dictionary<string, string>();
            var damTypes = Enum.GetNames(Enums.eDamage.None.GetType());

            #region DefenseStats

            for (var index = 0; index < totalStat.Def.Length; index++)
            {
                var convMath = totalStat.Def[index] * 100f;
                if (!(convMath > 0)) continue;
                var stat = $"{Convert.ToDecimal(convMath):0.##}%";
                statDictionary.Add(damTypes[index], stat);
            }

            gatherData.Add("Defense", statDictionary);
            statDictionary = new Dictionary<string, string>();

            #endregion

            #region ResistanceStats

            for (var index = 0; index < totalStat.Res.Length; index++)
            {
                var convMath = totalStat.Res[index] * 100f;
                if (!(convMath > 0)) continue;
                var stat = $"{Convert.ToDecimal(convMath):0.##}%";
                statDictionary.Add(damTypes[index], stat);
            }

            gatherData.Add("Resistance", statDictionary);
            statDictionary = new Dictionary<string, string>();

            #endregion

            var acc = $"{Convert.ToDecimal(totalStat.BuffAcc * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Accuracy", acc } };
            gatherData.Add("Accuracy", statDictionary);

            var dmg = $"{Convert.ToDecimal(totalStat.BuffDam * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Damage", dmg } };
            gatherData.Add("Damage", statDictionary);

            var endRdx = $"{Convert.ToDecimal(totalStat.BuffEndRdx * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Endurance Reduction", endRdx } };
            gatherData.Add("Endurance Reduction", statDictionary);

            var endMax = $"{Convert.ToDecimal(totalStat.EndMax + 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Endurance Maximum", endMax } };
            gatherData.Add("Endurance Maximum", statDictionary);

            var endRec = $"{displayStat.EnduranceRecoveryPercentage(false):###0}% ({Convert.ToDecimal(displayStat.EnduranceRecoveryNumeric):0.##}/s)";
            statDictionary = new Dictionary<string, string> { { "Endurance Recovery", endRec } };
            gatherData.Add("Endurance Recovery", statDictionary);

            var endUse = $"{Convert.ToDecimal(displayStat.EnduranceUsage):0.##}/s";
            statDictionary = new Dictionary<string, string> { { "Endurance Usage", endUse } };
            gatherData.Add("Endurance Usage", statDictionary);

            var elusive = $"{Convert.ToDecimal(totalStat.ElusivityMax * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Elusivity", elusive } };
            gatherData.Add("Elusivity", statDictionary);

            var toHit = $"{Convert.ToDecimal(totalStat.BuffToHit * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "ToHit", toHit } };
            gatherData.Add("ToHit", statDictionary);

            var globalRech = $"{Convert.ToDecimal(totalStat.BuffHaste * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Haste", globalRech } };
            gatherData.Add("Haste", statDictionary);

            var maxHp = $"{Convert.ToDecimal(displayStat.HealthHitpointsPercentage):0.##}% ({Convert.ToDecimal(displayStat.HealthHitpointsNumeric(false)):0.##}HP)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Maximum", maxHp } };
            gatherData.Add("Hitpoints Maximum", statDictionary);

            var regenHp = $"{Convert.ToDecimal(displayStat.HealthRegenPercent(false)):0.##}% ({Convert.ToDecimal(displayStat.HealthRegenHPPerSec):0.##}/s)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Regeneration", regenHp } };
            gatherData.Add("Hitpoints Regeneration", statDictionary);

            foreach (var kvp in selectedStats)
            {
                switch (kvp.Key)
                {
                    case "Defense":
                        foreach (var stat in gatherData[kvp.Key])
                        {
                            foreach (var item in kvp.Value)
                            {
                                if (item == stat.Key)
                                {
                                    stats.Add($"{stat.Key} {kvp.Key}", stat.Value);
                                }
                            }
                        }

                        break;
                    case "Resistance":
                        foreach (var stat in gatherData[kvp.Key])
                        {
                            foreach (var item in kvp.Value)
                            {
                                if (item == stat.Key)
                                {
                                    stats.Add($"{stat.Key} {kvp.Key}", stat.Value);
                                }
                            }
                        }

                        break;
                    case "Misc":
                        foreach (var item in kvp.Value)
                        {
                            foreach (var stat in gatherData[item])
                            {
                                if (item == stat.Key)
                                {
                                    stats.Add(stat.Key, stat.Value);
                                }
                            }
                        }

                        break;
                }
            }

            return await Task.FromResult(stats);
        }

        private int GetCheckedCount()
        {
            return _checkedCount;
        }

        private void SetCheckedCount(int value)
        {
            _checkedCount = value;
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var obj = (CheckedListExt)sender;
            switch (e.NewValue)
            {
                case CheckState.Checked:
                    SetCheckedCount(GetCheckedCount() + 1);
                    switch (obj.Name)
                    {
                        case "defenseCheckedList":
                            DefList.Add(obj.Items[e.Index].ToString());
                            if (SelectedStats.ContainsKey("Defense"))
                            {
                                SelectedStats["Defense"] = DefList;
                            }
                            else
                            {
                                SelectedStats.Add("Defense", DefList);
                            }

                            break;
                        case "resistCheckedList":
                            ResList.Add(obj.Items[e.Index].ToString());
                            if (SelectedStats.ContainsKey("Resistance"))
                            {
                                SelectedStats["Resistance"] = ResList;
                            }
                            else
                            {
                                SelectedStats.Add("Resistance", ResList);
                            }

                            break;
                        case "miscCheckedList":
                        {
                            var selectedItem = Regex.Replace(obj.Items[e.Index].ToString(), "[()]", "");
                            MisList.Add(selectedItem);
                            if (SelectedStats.ContainsKey("Misc"))
                            {
                                SelectedStats["Misc"] = MisList;
                            }
                            else
                            {
                                SelectedStats.Add("Misc", MisList);
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
                            DefList.Remove(obj.Items[e.Index].ToString());
                            foreach (var list in from kvp in SelectedStats from list in SelectedStats.Values where kvp.Key == "Defense" select list)
                            {
                                list.Remove(obj.Items[e.Index].ToString());
                            }

                            break;
                        case "resistCheckedList":
                            ResList.Remove(obj.Items[e.Index].ToString());
                            foreach (var list in from kvp in SelectedStats from list in SelectedStats.Values where kvp.Key == "Resistance" select list)
                            {
                                list.Remove(obj.Items[e.Index].ToString());
                            }

                            break;
                        case "miscCheckedList":
                        {
                            var selectedItem = Regex.Replace(obj.Items[e.Index].ToString(), "[()]", "");
                            MisList.Remove(selectedItem);
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

        }
    }
}

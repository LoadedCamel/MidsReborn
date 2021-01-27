using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;
using mrbBase;
using mrbBase.Base.Master_Classes;
using Newtonsoft.Json;
using RandomString4Net;
using RestSharp;

namespace Mids_Reborn
{
    public class Toon
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string Server { get; set; }
        public string ServerChannel { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Archetype { get; set; }
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public Dictionary<string, string> Stats { get; set; }
        public string DataLink { get; set; }
        public string Description { get; set; }
        public string SubmittedBy { get; set; }
        public string SubmittedOn { get; set; }


    }

    public class clsDiscord
    {

        private const string BOT_API_ENDPOINT = "https://api.midsreborn.com:3001";

        public static void GatherData(Dictionary<string, List<string>> selectedStats, string server, string channel)
        {
            var data = new Toon
            {
                MemberId = MidsContext.GetCryptedValue("User", "id"),
                MemberName = MidsContext.GetCryptedValue("User", "username"),
                Server = server,
                ServerChannel = channel,
                AppName = MidsContext.AppName,
                AppVersion = $"{MidsContext.AppVersion}",
                Name = MidsContext.Character.Name,
                Level = ToonLevel(),
                Archetype = MidsContext.Character.Archetype.DisplayName,
                Primary = MidsContext.Character.Powersets[0].DisplayName,
                Secondary = MidsContext.Character.Powersets[1].DisplayName,
                Stats = new Dictionary<string, string>(),
                DataLink = $"[Click Here to Download]({ShrinkTheDatalink(MidsCharacterFileFormat.MxDBuildSaveHyperlink(false, true))})"
            };

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

            var elusive = $"{Convert.ToDecimal(totalStat.Elusivity * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Elusivity", elusive } };
            gatherData.Add("Elusivity", statDictionary);

            var toHit = $"{Convert.ToDecimal(totalStat.BuffToHit * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "ToHit", toHit } };
            gatherData.Add("ToHit", statDictionary);

            var globalRech = $"{Convert.ToDecimal(totalStat.BuffHaste * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Haste", globalRech } };
            gatherData.Add("Haste", statDictionary);

            var maxHP = $"{Convert.ToDecimal(displayStat.HealthHitpointsPercentage):0.##}% ({Convert.ToDecimal(displayStat.HealthHitpointsNumeric(false)):0.##}HP)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Maximum", maxHP } };
            gatherData.Add("Hitpoints Maximum", statDictionary);

            var regenHP = $"{Convert.ToDecimal(displayStat.HealthRegenPercent(false)):0.##}% ({Convert.ToDecimal(displayStat.HealthRegenHPPerSec):0.##}/s)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Regeneration", regenHP } };
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
                                    data.Stats.Add($"{stat.Key} {kvp.Key}", stat.Value);
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
                                    data.Stats.Add($"{stat.Key} {kvp.Key}", stat.Value);
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
                                    data.Stats.Add(stat.Key, stat.Value);
                                }
                            }
                        }
                        break;
                }
            }

            Export(data);
        }

        private static int ToonLevel()
        {
            var level = MidsContext.Character.Level + 1;
            if (level > 50) level = 50;
            return level;
        }

        public static string ShrinkTheDatalink(string strUrl)
        {
            var url = "http://tinyurl.com/api-create.php?url=" + strUrl;
            var objWebRequest = (HttpWebRequest)WebRequest.Create(url);
            objWebRequest.Method = "GET";
            using var objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
            var srReader = new StreamReader(objWebResponse.GetResponseStream() ?? throw new InvalidOperationException());
            var strHtml = srReader.ReadToEnd();
            srReader.Close();
            objWebResponse.Close();
            objWebRequest.Abort();

            return strHtml;
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0) return;
            e.Cancel = true;
            e.Message = "Required";
        }

        private static void Export(Toon dataToon)
        {
            var msgResult = MessageBox.Show(@"Would you like to enter a description for people to see?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msgResult == DialogResult.Yes)
            {
                InputBoxResult result = InputBox.Show("Enter a description for your build.", "Build Description", "Enter your description here", InputBox.InputBoxIcon.Info, inputBox_Validating);
                dataToon.Description = result.OK ? result.Text : "None";
                dataToon.SubmittedBy = MidsContext.GetCryptedValue("BotUser", "username");
                dataToon.SubmittedOn = DateTime.Now.ToShortDateString();
                MbLogin();
                BuildSubmit(dataToon);
            }
            else
            {
                dataToon.Description = "None";
                dataToon.SubmittedBy = MidsContext.GetCryptedValue("BotUser", "username");
                dataToon.SubmittedOn = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
                MbLogin();
                BuildSubmit(dataToon);
            }
        }

        private static void BuildSubmit(Toon dataToon)
        {
            var client = new RestClient(BOT_API_ENDPOINT);
            var request = new RestRequest("v2/builds/submit", Method.POST); //
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("access_token", MidsContext.GetCryptedValue("BotUser", "access_token"));
            request.AddJsonBody(new
            {
                MemberId = dataToon.MemberId,
                MemberName = dataToon.MemberName,
                Server = dataToon.Server,
                ServerChannel = dataToon.ServerChannel,
                AppName = dataToon.AppName,
                AppVersion = dataToon.AppVersion,
                Name = dataToon.Name,
                Level = dataToon.Level,
                Archetype = dataToon.Archetype,
                Primary = dataToon.Primary,
                Secondary = dataToon.Secondary,
                Stats = dataToon.Stats,
                DataLink = dataToon.DataLink,
                Description = dataToon.Description,
                SubmittedBy = dataToon.SubmittedBy,
                SubmittedOn = dataToon.SubmittedOn
            });
            var response = client.Execute(request);
            if (response.Content == "Build submitted successfully")
            {
                Form.ActiveForm?.Close();
            }
            else
            {
                MessageBox.Show($"Error Code: {response.StatusCode}\r\nResponse: {response.Content}\r\nRecommendation: Please reach out to the RebornTeam to resolve this issue.", @"MidsBot Error Response");
            }
        }

        public static void MbRegister()
        {
            try
            {
                var client = new RestClient(BOT_API_ENDPOINT);
                var request = new RestRequest("v2/users/register", Method.POST);
                var regName = $"{MidsContext.GetCryptedValue("User", "username")}#{MidsContext.GetCryptedValue("User", "discriminator")}";
                var pass = RandomString.GetString(Types.ALPHABET_LOWERCASE_WITH_SYMBOLS, 25, false);
                request.AddParameter("username", regName);
                request.AddParameter("pass_token", pass);
                var response = client.Execute(request);
                if (response.Content == "Bad Request" || response.Content == "User already exists")
                {
                    MessageBox.Show($"Error Code: {response.StatusCode}\r\nResponse: {response.Content}\r\nRecommendation: Please reach out to the RebornTeam to resolve this issue.", @"MidsBot Error Response");
                    return;
                }

                var MBObject = new MidsBotUser { username = regName, pass_token = pass };
                var MBObjectSerialized = JsonConvert.SerializeObject(MBObject);
                var jMBObject = JsonConvert.DeserializeObject<MidsBotUser>(MBObjectSerialized);
                var mbDict = new Dictionary<string, object>();
                var properties = typeof(MidsBotUser).GetProperties();
                foreach (var property in properties)
                {
                    mbDict.Add(property.Name, property.GetValue(jMBObject, null));
                }

                MidsContext.ConfigSp.BotUser = mbDict;
                MidsContext.Config.Registered = 1;
                MbLogin();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}\r\n\r\nIf you received this error, please reach out to the RebornTeam.", @"Registration Method Error");
            }

        }

        public static void MbLogin()
        {
            try
            {
                var client = new RestClient(BOT_API_ENDPOINT);
                var request = new RestRequest("v2/users/login", Method.POST);
                request.AddParameter("username", MidsContext.GetCryptedValue("BotUser", "username"));
                request.AddParameter("pass_token", MidsContext.GetCryptedValue("BotUser", "pass_token"));
                var response = client.Execute(request);
                if (response.Content == "User or Pass is incorrect" || response.Content == "Bad Request")
                {
                    MessageBox.Show($"Error Code: {response.StatusCode}\r\nResponse: {response.Content}\r\nRecommendation: Please reach out to the RebornTeam to resolve this issue.", @"MidsBot Error Response");
                    return;
                }

                var jMBObject = JsonConvert.DeserializeObject<MidsBotUser>(response.Content);
                var mbDict = new Dictionary<string, object>();
                var properties = typeof(MidsBotUser).GetProperties();
                foreach (var property in properties)
                {
                    mbDict.Add(property.Name, property.GetValue(jMBObject, null));
                }

                MidsContext.ConfigSp.BotUser = mbDict;
                ValidateServers();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}\r\n\r\nIf you received this error, please reach out to the RebornTeam.", @"Login Method Error");
            }
        }

        public static void ValidateServers()
        {
            try
            {
                var client = new RestClient(BOT_API_ENDPOINT);
                var request = new RestRequest("v2/users/sc", Method.POST); //
                request.AddHeader("Content-type", "application/json");
                request.AddHeader("access_token", MidsContext.GetCryptedValue("BotUser", "access_token"));
                request.AddJsonBody(new
                {
                    Id = MidsContext.GetCryptedValue("User", "id"),
                    MidsContext.ConfigSp.ServerList
                });
                var response = client.Execute(request);
                if (response.Content == "Bad Request")
                {
                    MessageBox.Show($"Error Code: {response.StatusCode}\r\nResponse: {response.Content}\r\nRecommendation: Please reach out to the RebornTeam to resolve this issue.", @"MidsBot Error Response");
                    return;
                }
                var jValidatedServers = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(response.Content);

                MidsContext.ConfigSp.ValidatedServers = jValidatedServers;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}\r\n\r\nIf you received this error, please reach out to the RebornTeam.", @"Server Validation Method Error");
            }
        }
    }


    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MidsBotUser
    {
        [JsonProperty(PropertyName = "username")] public string username { get; set; }
        [JsonProperty(PropertyName = "pass_token")] public string pass_token { get; set; }
        [JsonProperty(PropertyName = "access_token")] public string access_token { get; set; }
        [JsonProperty(PropertyName = "expires_in")] public string expires_in { get; set; }
    }

    public class ValidServer
    {
        public string Name { get; set; }
        public List<string> Channels { get; set; }
    }
}
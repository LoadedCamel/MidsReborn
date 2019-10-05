using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;

namespace Hero_Designer
{

    public class Clshook
    {
        public static string DFileName;
        public static string DExt;

        public static string Mids_Version { get; set; }

        public static string Discord_Server { get; set; }
        public static string Discord_Channel { get; set; }
        public static string Discord_Nickname { get; set; }

        public static string Character_Name { get; set; }
        public static string Character_Level { get; set; }
        public static string Character_Archetype { get; set; }
        public static string Character_Primary_Power { get; set; }
        public static string Character_Secondary_Power { get; set; }

        public static string Embed_Link { get; set; }

        public static string Smashing_Defense { get; set; }
        public static string Lethal_Defense { get; set; }
        public static string Fire_Defense { get; set; }
        public static string Cold_Defense { get; set; }
        public static string Energy_Defense { get; set; }
        public static string Negative_Defense { get; set; }
        public static string Psionic_Defense { get; set; }
        public static string Melee_Defense { get; set; }
        public static string Ranged_Defense { get; set; }
        public static string AOE_Defense { get; set; }

        public static string Resistance_Cap { get; set; }

        public static string Smashing_Resistance { get; set; }
        public static string Lethal_Resistance { get; set; }
        public static string Fire_Resistance { get; set; }
        public static string Cold_Resistance { get; set; }
        public static string Energy_Resistance { get; set; }
        public static string Negative_Resistance { get; set; }
        public static string Toxic_Resistance { get; set; }
        public static string Psionic_Resistance { get; set; }

        public static string Damage_Buff { get; set; }
        public static string Endurance_Usage { get; set; }
        public static string Global_Recharge { get; set; }
        public static string Recovery { get; set; }
        public static string Regen { get; set; }
        public static string ToHit { get; set; }

        public static string ShrinkTheDatalink(string strUrl)
        {
            var url = "http://tinyurl.com/api-create.php?url=" + strUrl;

            var objWebRequest = (HttpWebRequest)WebRequest.Create(url);
            objWebRequest.Method = "GET";
            using (var objWebResponse = (HttpWebResponse)objWebRequest.GetResponse())
            {
                var srReader = new StreamReader(objWebResponse.GetResponseStream() ?? throw new InvalidOperationException());

                var strHtml = srReader.ReadToEnd();

                srReader.Close();
                objWebResponse.Close();
                objWebRequest.Abort();

                return (strHtml);
            }
        }

        public object this[string propertyName] => GetType().GetProperty(propertyName)?.GetValue(this, null);

        internal static async Task DiscordExport()
        {
            Statistics displayStats = MidsContext.Character.DisplayStats;
            var num = MidsContext.Character.Level + 1;
            if (num > 50) num = 50;

            string[] names = Enum.GetNames(Enums.eDamage.None.GetType());
            int num1 = names.Length - 1;

            for (int dType = 1; dType <= num1; ++dType)
            {
                var tmpMatch = new Regex(@"(unique*|toxic|special)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (tmpMatch.Match(names[dType]).Success)
                    continue;
                string defenseStat = $"{Strings.Format(displayStats.Defense(dType), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##")}%";
                switch (names[dType])
                {
                    case "Smashing":
                        Smashing_Defense = defenseStat;
                        break;
                    case "Lethal":
                        Lethal_Defense = defenseStat;
                        break;
                    case "Fire":
                        Fire_Defense = defenseStat;
                        break;
                    case "Cold":
                        Cold_Defense = defenseStat;
                        break;
                    case "Energy":
                        Energy_Defense = defenseStat;
                        break;
                    case "Negative":
                        Negative_Defense = defenseStat;
                        break;
                    case "Psionic":
                        Psionic_Defense = defenseStat;
                        break;
                    case "Melee":
                        Melee_Defense = defenseStat;
                        break;
                    case "Ranged":
                        Ranged_Defense = defenseStat;
                        break;
                    case "AoE":
                        AOE_Defense = defenseStat;
                        break;
                }
            }
            Resistance_Cap = $"{Strings.Format((float)(MidsContext.Character.Archetype.ResCap * 100.0), "###0")}%";
            for (int rType = 1; rType <= num1; ++rType)
            {
                var tmpMatch = new Regex(@"(unique*|melee|ranged|aoe|special)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (tmpMatch.Match(names[rType]).Success)
                    continue;
                var resistanceStat = $"{Strings.Format(displayStats.DamageResistance(rType, true),"##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##")}%";
                switch (names[rType])
                {
                    case "Smashing":
                        Smashing_Resistance = resistanceStat;
                        break;
                    case "Lethal":
                        Lethal_Resistance = resistanceStat;
                        break;
                    case "Fire":
                        Fire_Resistance = resistanceStat;
                        break;
                    case "Cold":
                        Cold_Resistance = resistanceStat;
                        break;
                    case "Energy":
                        Energy_Resistance = resistanceStat;
                        break;
                    case "Negative":
                        Negative_Resistance = resistanceStat;
                        break;
                    case "Toxic":
                        Toxic_Resistance = resistanceStat;
                        break;
                    case "Psionic":
                        Psionic_Resistance = resistanceStat;
                        break;
                }
            }
            Damage_Buff = Strings.Format(displayStats.BuffDamage(false) - 100f, "##0.#") + "%";
            Endurance_Usage = Strings.Format(displayStats.EnduranceUsage, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "/s";
            Global_Recharge = Strings.Format((float)(displayStats.BuffHaste(false) - 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "#") + "%";
            Recovery = Strings.Format(displayStats.EnduranceRecoveryPercentage(false), "###0") + "% (" + Strings.Format(displayStats.EnduranceRecoveryNumeric, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "/s)";
            Regen = Strings.Format(displayStats.HealthRegenPercent(false), "###0") + "%";
            ToHit = Strings.Format(displayStats.BuffToHit, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "#") + "%";

            Discord_Server = MidsContext.Config.DSelServer.Replace(" (Default)", "");
            Discord_Nickname = MidsContext.Config.DNickName;
            Discord_Channel = MidsContext.Config.DChannel;

            Mids_Version = MidsContext.AppAssemblyVersion;

            Character_Name = MidsContext.Character.Name;
            Character_Level = Convert.ToString(num);
            Character_Archetype = MidsContext.Character.Archetype.DisplayName;
            Character_Primary_Power = MidsContext.Character.Powersets[0].DisplayName;
            Character_Secondary_Power = MidsContext.Character.Powersets[1].DisplayName;

            var cDatalink = MidsCharacterFileFormat.MxDBuildSaveHyperlink(false, true);
            var shrunkData = ShrinkTheDatalink(cDatalink);
            var embedurl = $"[Click Here to Download]({shrunkData})";
            Embed_Link = embedurl;

            /*foreach (var stat in MidsContext.Config.CheckedStats)
            {
                var eStat = (Enums.eStats) Enum.Parse(typeof(Enums.eStats), stat, true);
                var combEStat = combinedStats[Convert.ToInt32(eStat) - 1];

            }*/

            byte[] data = Convert.FromBase64String("aHR0cDovL2hvb2tzLm1pZHNyZWJvcm4uY29tOjMwMDAvYXBpP3Rva2VuPVVtUWhUNGtEclMwZ0E1TUY1YUdsaTh6YllDVW1RaFQ0a0RyUzBnQTVNRjVhR2xpOHpiWUM=");
            //byte[] data = Convert.FromBase64String("aHR0cDovL2hvb2tzLm1pZHNyZWJvcm4uY29tOjMwMDEvYXBpP3Rva2VuPVVtUWhUNGtEclMwZ0E1TUY1YUdsaTh6YllDVW1RaFQ0a0RyUzBnQTVNRjVhR2xpOHpiWUM=");
            string wString = Encoding.UTF8.GetString(data);

            //Send the data to the API Server and retrieve response

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(wString);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var ExportedValues = new Dictionary<string, string>();
                ExportedValues.Add(nameof(Mids_Version), Mids_Version);
                ExportedValues.Add(nameof(Discord_Server), Discord_Server);
                ExportedValues.Add(nameof(Discord_Channel), Discord_Channel);
                ExportedValues.Add(nameof(Discord_Nickname), Discord_Nickname);
                ExportedValues.Add(nameof(Character_Name), Character_Name);
                ExportedValues.Add(nameof(Character_Level), Character_Level);
                ExportedValues.Add(nameof(Character_Archetype), Character_Archetype);
                ExportedValues.Add(nameof(Character_Primary_Power), Character_Primary_Power);
                ExportedValues.Add(nameof(Character_Secondary_Power), Character_Secondary_Power);
                ExportedValues.Add(nameof(Embed_Link), Embed_Link);
                foreach (var stat in MidsContext.Config.CheckedStats)
                {
                    Clshook val = new Clshook();
                    ExportedValues.Add(stat, (string)val[stat]);
                    //var eStat = (Enums.eStats)Enum.Parse(typeof(Enums.eStats), stat, true);
                   // MessageBox.Show(eStat.ToString());
                    //var combEStat = combinedStats[Convert.ToInt32(eStat) - 1];

                }
                var json = JsonConvert.SerializeObject(new { ExportedValues });
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                var result = streamReader.ReadToEnd();
                switch (result)
                {
                    case "Nickname not found in discord":
                        {
                            string message = $"Submission Failed: Your discord nickname '{Discord_Nickname}' was not found in the {Discord_Server} discord server.";
                            string title = "Discord Export";
                            MessageBox.Show(message, title);
                            break;
                        }

                    case "Export Successful":
                        {
                            string message = $"Submission Successful!! Your build should now be posted in {Discord_Channel} on the {Discord_Server} server.";
                            string title = "Discord Export";
                            MessageBox.Show(message, title);
                            break;
                        }

                    case "Export Failed":
                        {
                            string message = "Submission Failed: Please check your discord export settings and make sure you have the latest version of Mids' Reborn : Hero Designer.";
                            string title = "Discord Export";
                            MessageBox.Show(message, title);
                            break;
                        }

                    case "Failed to add export to queue":
                        {
                            string message = "Submission Failed: Possible server error, please contact the RebornTeam.";
                            string title = "Discord Export";
                            MessageBox.Show(message, title);
                            break;
                        }

                    case "RebornBot is not in the discord server":
                        {
                            string message = $"Submission Failed: MidsBot was not found in the {Discord_Server}.";
                            string title = "Discord Export";
                            MessageBox.Show(message, title);
                            break;
                        }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Hero_Designer.Forms.Controls;
using Newtonsoft.Json;

namespace Hero_Designer
{
    public class Toon
    {
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
        public static async void GatherData(Dictionary<string, List<string>> selectedStats)
        {
            var data = new Toon
            {
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
            statDictionary = new Dictionary<string, string> {{"Accuracy", acc}};
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

            await Export(data);
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

            var objWebRequest = (HttpWebRequest) WebRequest.Create(url);
            objWebRequest.Method = "GET";
            using var objWebResponse = (HttpWebResponse) objWebRequest.GetResponse();
            var srReader =
                new StreamReader(objWebResponse.GetResponseStream() ?? throw new InvalidOperationException());

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

        private static async Task Export(Toon dataToon)
        {
            var msgResult = MessageBox.Show(@"Would you like to enter a description for people to see?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msgResult == DialogResult.Yes)
            {
                InputBoxResult result = InputBox.Show("Enter a description for your build.", "Build Description", "Enter your description here", InputBox.InputBoxIcon.Info, inputBox_Validating);
                if (result.OK) { dataToon.Description = result.Text; }

                dataToon.SubmittedBy = $"{clsOAuth.GetCryptedValue("User", "username")}#{clsOAuth.GetCryptedValue("User", "discriminator")}";
                dataToon.SubmittedOn = DateTime.Now.ToShortDateString();
                var jsonExport = JsonConvert.SerializeObject(dataToon, Formatting.Indented);
                Console.WriteLine(jsonExport);
            }
            else
            {
                dataToon.SubmittedBy = $"{clsOAuth.GetCryptedValue("User", "username")}#{clsOAuth.GetCryptedValue("User", "discriminator")}";
                dataToon.SubmittedOn = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
                var jsonExport = JsonConvert.SerializeObject(dataToon, Formatting.Indented);
                Console.WriteLine(jsonExport);
                Form.ActiveForm?.Close();
            }
        }
    }
}
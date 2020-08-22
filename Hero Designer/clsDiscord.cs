using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Base.Master_Classes;

namespace Hero_Designer
{
    public class Toon
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Archetype { get; set; }
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public Dictionary<string, Dictionary<string, string>> Stats { get; set; }
    }

    public class clsDiscord
    {
        public static void GatherData()
        {
            var data = new Toon
            {
                Name = MidsContext.Character.Name,
                Level = ToonLevel(),
                Archetype = MidsContext.Character.Archetype.DisplayName,
                Primary = MidsContext.Character.Powersets[0].DisplayName,
                Secondary = MidsContext.Character.Powersets[1].DisplayName,
                Stats = new Dictionary<string, Dictionary<string, string>>()
            };

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
            data.Stats.Add("Defense", statDictionary);
            #endregion

            statDictionary.Clear();

            #region ResistanceStats
            for (var index = 0; index < totalStat.Res.Length; index++)
            {
                var convMath = totalStat.Res[index] * 100f;
                if (!(convMath > 0)) continue;
                var stat = $"{Convert.ToDecimal(convMath):0.##}%";
                statDictionary.Add(damTypes[index], stat);
            }
            data.Stats.Add("Resistance", statDictionary);
            #endregion

            var acc = $"{Convert.ToDecimal(totalStat.BuffAcc * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> {{"Accuracy", acc}};
            data.Stats.Add("Accuracy", statDictionary);

            var dmg = $"{Convert.ToDecimal(totalStat.BuffDam * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Damage", dmg } };
            data.Stats.Add("Damage", statDictionary);

            var endRdx = $"{Convert.ToDecimal(totalStat.BuffEndRdx * 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Endurance Reduction", endRdx } };
            data.Stats.Add("Endurance Reduction", statDictionary);

            var endMax = $"{Convert.ToDecimal(totalStat.EndMax + 100f):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Endurance Maximum", endMax } };
            data.Stats.Add("Endurance Maximum", statDictionary);

            var endRec = $"{displayStat.EnduranceRecoveryPercentage(false):###0}% ({Convert.ToDecimal(displayStat.EnduranceRecoveryNumeric):0.##}/s)";
            statDictionary = new Dictionary<string, string> { { "Endurance Recovery", endRec } };
            data.Stats.Add("Endurance Recovery", statDictionary);

            var endUse = $"{Convert.ToDecimal(displayStat.EnduranceUsage):0.##}/s";
            statDictionary = new Dictionary<string, string> { { "Endurance Usage", endUse } };
            data.Stats.Add("Endurance Usage", statDictionary);

            var elusive = $"{Convert.ToDecimal(totalStat.Elusivity * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Elusivity", elusive } };
            data.Stats.Add("Elusivity", statDictionary);

            var toHit = $"{Convert.ToDecimal(totalStat.BuffToHit * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "ToHit", toHit } };
            data.Stats.Add("ToHit", statDictionary);

            var globalRech = $"{Convert.ToDecimal(totalStat.BuffHaste * 100):0.##}%";
            statDictionary = new Dictionary<string, string> { { "Haste", globalRech } };
            data.Stats.Add("Haste", statDictionary);

            var maxHP = $"{Convert.ToDecimal(displayStat.HealthHitpointsPercentage):0.##}% ({Convert.ToDecimal(displayStat.HealthHitpointsNumeric(false)):0.##}HP)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Max", maxHP } };
            data.Stats.Add("Hitpoints Maximum", statDictionary);

            var regenHP = $"{Convert.ToDecimal(displayStat.HealthRegenPercent(false)):0.##}% ({Convert.ToDecimal(displayStat.HealthRegenHPPerSec):0.##}/s)";
            statDictionary = new Dictionary<string, string> { { "Hitpoints Regeneration", regenHP } };
            data.Stats.Add("Hitpoints Regeneration", statDictionary);
            
            var selectedStats = new Dictionary<string, string>
            {
                {"Defense", "Smashing"},
                {"Resistance", "Fire"},
                {"Accuracy", "Accuracy"}
            };

            foreach (var item in selectedStats)
            {
                foreach (var outerPair in data.Stats.Where(outerPair => item.Key == outerPair.Key))
                {
                    foreach (var innerPair in outerPair.Value.Where(innerPair => item.Value == innerPair.Key))
                    {
                        Console.WriteLine(innerPair.Key != outerPair.Key
                            ? $"{innerPair.Key} {outerPair.Key} - {innerPair.Value}"
                            : $"{outerPair.Key} - {innerPair.Value}");
                    }
                }
            }

            /*foreach (var outerPair in data.Stats)
            {
                foreach (var innerPair in outerPair.Value)
                {
                    Console.WriteLine(innerPair.Key != outerPair.Key
                        ? $"{innerPair.Key} {outerPair.Key} - {innerPair.Value}"
                        : $"{outerPair.Key} - {innerPair.Value}");
                }
            }*/
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

        public static async Task Export()
        {
        }
    }
}
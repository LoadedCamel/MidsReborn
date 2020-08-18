using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Base.Master_Classes;
using Microsoft.VisualBasic;

namespace Hero_Designer
{
    public class Toon
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Archetype { get; set; }
        public string Primary_Powerset { get; set; }
        public string Secondary_Powerset { get; set; }
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
                Primary_Powerset = MidsContext.Character.Powersets[0].DisplayName,
                Secondary_Powerset = MidsContext.Character.Powersets[1].DisplayName,
                Stats = new Dictionary<string, Dictionary<string, string>>()
            };
        }

        public static void Test()
        {
            var totalStat = MidsContext.Character.Totals;
            var displayStat = MidsContext.Character.DisplayStats;
            foreach (var value in totalStat.Def) Console.WriteLine(value * 100f);
        }

        public static int ToonLevel()
        {
            var level = MidsContext.Character.Level + 1;
            if (level > 50) level = 50;
            return level;
        }

        public static void Stats()
        {
            var displayStats = MidsContext.Character.DisplayStats;
            var names = Enum.GetNames(Enums.eDamage.None.GetType());

            var defenseDict = new Dictionary<string, string>();
            var resistanceDict = new Dictionary<string, string>();

            for (var typeIndex = 1; typeIndex <= names.Length - 1; ++typeIndex)
            {
                var tmpMatch = new Regex(@"(unique*|toxic|special)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (tmpMatch.Match(names[typeIndex]).Success)
                    continue;
                var defenseStat =
                    $"{Strings.Format(displayStats.Defense(typeIndex), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##")}%";
                switch (names[typeIndex])
                {
                    case "Smashing":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Lethal":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Fire":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Cold":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Energy":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Negative":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Psionic":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Melee":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "Ranged":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                    case "AoE":
                        defenseDict.Add(names[typeIndex], defenseStat);
                        break;
                }
            }

            for (var typeIndex = 1; typeIndex <= names.Length - 1; ++typeIndex)
            {
                var tmpMatch = new Regex(@"(unique*|melee|ranged|aoe|special)",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (tmpMatch.Match(names[typeIndex]).Success)
                    continue;
                var resistanceStat =
                    $"{Strings.Format(displayStats.DamageResistance(typeIndex, true), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##")}%";
                switch (names[typeIndex])
                {
                    case "Smashing":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Lethal":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Fire":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Cold":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Energy":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Negative":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Toxic":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                    case "Psionic":
                        resistanceDict.Add(names[typeIndex], resistanceStat);
                        break;
                }
            }
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

        #region structs

        private struct Defense
        {
            private string Smashing { get; set; }
            private string Lethal { get; set; }
            private string Fire { get; set; }
            private string Cold { get; set; }
            private string Energy { get; set; }
            private string Negative { get; set; }
            private string Psionic { get; set; }
            private string Melee { get; set; }
            private string Ranged { get; set; }
            private string AOE { get; set; }
        }

        private struct Resistance
        {
            private string Smashing { get; set; }
            private string Lethal { get; set; }
            private string Fire { get; set; }
            private string Cold { get; set; }
            private string Energy { get; set; }
            private string Negative { get; set; }
            private string Psionic { get; set; }
            private string Toxic { get; set; }
        }

        private struct Misc
        {
            private string Accuracy { get; set; }
            private string Damage_Buff { get; set; }
            private string Elusivity { get; set; }
            private string Endurance_Max { get; set; }
            private string Endurance_Recovery { get; set; }
            private string Endurance_Reduction { get; set; }
            private string Endurance_Usage { get; set; }
            private string Global_Recharge { get; set; }
            private string Hitpoints_Max { get; set; }
            private string Hitpoints_Regeneration { get; set; }
            private string ToHit { get; set; }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;

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

        public static int ToonLevel()
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

        public static async Task Export()
        {

        }
    }
}

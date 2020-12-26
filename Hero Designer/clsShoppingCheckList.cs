using System;
using System.Text.RegularExpressions;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn
{
    static class clsShoppingCheckList
    {
        private const string _header = "// Build shopping list status\r\n";

        // Write to string
        public static string Dump()
        {
            var res = "";
            var isFirst = true;
            res += _header;
            res += "{";
            var i = 0;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var s = 0; s < p.Slots.Length; s++)
                {
                    if (p.Slots[s].Enhancement.Enh <= -1) continue; // Empty slot
                    
                    if (!isFirst) res += ",";
                    res += $"({i},{s},{(p.Slots[s].Enhancement.Obtained ? "1" : "0")})";
                    isFirst = false;
                }

                i++;
            }
            res += "}";

            return res;
        }

        // Parse from build file
        public static void FromFile(string fc)
        {
            var r = new Regex(_header + @"{(,?\([0-9]+,[0-9]+,[0-1]\))+\}");

            foreach (Match m in r.Matches(fc))
            {
                string[] data = m.Groups[1].Value.Split(',');
                var powerIdx = Convert.ToInt32(data[0]);
                var slotIdx = Convert.ToInt32(data[1]);
                var status = Convert.ToBoolean(data[2]);

                MidsContext.Character.CurrentBuild.Powers[powerIdx].Slots[slotIdx].Enhancement.Obtained = status;
            }
        }
    }
}
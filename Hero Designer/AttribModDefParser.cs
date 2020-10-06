using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hero_Designer
{
    public class AttribModDefParser
    {
        private string Cnt;
        private readonly string Indent = "\t";

        public AttribModDefParser(string file)
        {
            Cnt = File.ReadAllText(file);
        }

        private int[] FindBlockBrackets(string src, int offset, int indent = 0)
        {
            Regex r1 = new Regex(@"^" + (indent > 0 ? string.Concat(Enumerable.Repeat(Indent, indent)) : "") + "{{",
                RegexOptions.CultureInvariant | RegexOptions.Multiline);

            Regex r2 = new Regex(@"^" + (indent > 0 ? string.Concat(Enumerable.Repeat(Indent, indent)) : "") + "}}",
                RegexOptions.CultureInvariant | RegexOptions.Multiline);

            Match m1 = r1.Match(src, offset);
            Match m2 = r2.Match(src, offset);

            return new []
            {
                m1.Groups.Count > 1 ? m1.Groups[1].Index : offset,
                m2.Groups.Count > 1 ? m2.Groups[1].Index : src.Length
            };
        }

        private Dictionary<string, Dictionary<string, float[]>> ParseBlock(string src = "", string keyword = "Class", int indent = 0)
        {
            Dictionary<string, Dictionary<string, float[]>> res = new Dictionary<string, Dictionary<string, float[]>>();

            if (string.IsNullOrEmpty(src)) src = Cnt;
            int offset = 0;

            while (src.IndexOf((indent > 0 ? string.Concat(Enumerable.Repeat(Indent, indent)) : "") + keyword, offset, StringComparison.InvariantCulture) > -1)
            {
                offset = src.IndexOf((indent > 0 ? string.Concat(Enumerable.Repeat(Indent, indent)) : "") + keyword, offset, StringComparison.InvariantCulture) + keyword.Length + 1;
                int[] range = FindBlockBrackets(src, offset, indent);
                string blockStr = src.Substring(range[0], range[1] - range[0]);
                Regex rName = new Regex(@"Name ([A-Za-z0-9\-_]+)", RegexOptions.CultureInvariant | RegexOptions.Multiline);
                MatchCollection mName = rName.Matches(blockStr);
                if (mName.Count == 0) break;
                if (keyword == "Class")
                {
                    res.Add(mName[1].Value, ParseBlock(blockStr, "ModTable", indent + 1)["ModTable"]);
                }
                else
                {
                    Regex rValues = new Regex(@"Values ([0-9\.\-\s\,]+)", RegexOptions.CultureInvariant | RegexOptions.Multiline);
                    float[] values = new Regex(@"\s*,\s*")
                        .Split(blockStr)
                        .Where(e => float.TryParse(e, out float dummy))
                        .Select(e => (float)Convert.ToDecimal(e, null))
                        .ToArray();

                    res.Add("ModTable", new Dictionary<string, float[]>());
                    res["ModTable"].Add(mName[1].Value, values);
                }
            }

            return res;
        }
    }
}
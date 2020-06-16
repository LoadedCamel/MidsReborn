using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Base.Master_Classes;

namespace HeroViewer.Tools
{
    struct BuilderApp
    {
        public string Software;
        public string Version;
    }

    struct Enh
    {
        public bool Filled;
        public string Base;
        public string SubMember;
        public int Level;
    }

    public class PlainTextParser
    {
        private string MXDBuild;
        private BuilderApp Builder = new BuilderApp();

        private readonly Dictionary<string, string> OldSetNames = new Dictionary<string, string>
        {
            ["Achilles"] = "AchHee",
            ["AdjTgt"] = "AdjTrg",
            ["Aegis"] = "Ags",
            ["Armgdn"] = "Arm",
            ["AnWeak"] = "AnlWkn",
            ["BasGaze"] = "BslGaz",
            ["CSndmn"] = "CaloftheS",
            ["C\"ngBlow"] = "ClvBlo",
            ["C\"ngImp"] = "CrsImp",
            ["Dct\"dW"] = "DctWnd",
            ["Decim"] = "Dcm",
            ["Det\"tn"] = "Dtn",
            ["Dev\"n"] = "Dvs",
            ["Efficacy"] = "EffAdp",
            ["Enf\"dOp"] = "EnfOpr",
            ["Erad"] = "Erd",
            ["ExRmnt"] = "ExpRnf",
            ["ExStrk"] = "ExpStr",
            ["ExtrmM"] = "ExtMsr",
            ["FotG"] = "FuroftheG",
            ["FrcFbk"] = "FrcFdb",
            ["F\"dSmite"] = "FcsSmt",
            ["GA"] = "GldArm",
            ["GSFC"] = "GssSynFr-",
            ["Hectmb"] = "Hct",
            ["HO:Micro"] = "Micro",
            ["H\"zdH"] = "HrmHln",
            ["ImpSkn"] = "ImpSki",
            ["Insult"] = "TrmIns",
            ["KinCrsh"] = "KntCrs",
            ["KntkC\"bat"] = "KntCmb",
            ["Krma"] = "Krm",
            ["Ksmt"] = "Ksm",
            ["LkGmblr"] = "LucoftheG",
            ["LgcRps"] = "LthRps",
            ["Mako"] = "Mk\"Bit",
            ["Mlais"] = "MlsIll",
            ["Mocking"] = "MckBrt",
            ["MotCorruptor"] = "MlcoftheC",
            ["Mrcl"] = "Mrc",
            ["M\"Strk"] = "Mlt",
            ["Numna"] = "NmnCnv",
            ["Oblit"] = "Obl",
            ["Panac"] = "Pnc",
            ["Posi"] = "PstBls",
            ["Prv-Heal/EndMod"] = "Prv-Heal/EndRdx",
            ["P\"ngS\"Fest"] = "PndSlg",
            ["P\"Shift"] = "PrfShf",
            ["Rec\"dRet"] = "RctRtc",
            ["RctvArm"] = "RctArm",
            ["RzDz"] = "RzzDzz",
            ["SMotCorruptor"] = "SprMlcoft",
            ["SprWntBit-Acc/Dmg/EndRdx/Rchg"] = "SprWntBit-Dmg/EndRdx/Acc/Rchg",
            ["Srng"] = "Srn",
            ["SStalkersG"] = "SprStlGl",
            ["SWotController"] = "SprWiloft",
            ["S\"fstPrt"] = "StdPrt",
            ["T\"Death"] = "TchofDth",
            ["T\"pst"] = "Tmp",
            ["Thundr"] = "Thn",
            ["ULeap"] = "UnbLea",
            ["UndDef"] = "UndDfn",
            ["WotController"] = "WiloftheC",
            ["Zephyr"] = "BlsoftheZ"
        };

        private string ShortNamesConversion(string sn)
        {
            if (string.IsNullOrEmpty(sn)) return sn;

            int i;
            foreach (KeyValuePair<string, string> k in this.OldSetNames)
            {
                if (sn.IndexOf(k.Key, StringComparison.Ordinal) > -1) return sn.Replace(k.Key, k.Value);
            }

            return sn;
        }

        public PlainTextParser(string MXDBuild)
        {
            this.MXDBuild = MXDBuild;
        }

        public bool Parse() // Return type ?
        {
            Regex r; Regex rs;
            Match m; Match ms;
            string cnt;
            int i; int j;

            int align;

            try
            {
                cnt = File.ReadAllText(this.MXDBuild);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                return false;
            }

            cnt = Regex.Replace(cnt, @"[\r\n]", "\r\n"); // Line conversion to PC/Win format (just in case)
            cnt = Regex.Replace(cnt, @"\<br \/\>", String.Empty); // For good ol' Mids 1.962 support
            cnt = Regex.Replace(cnt, @"\&nbsp\;", " "); // Nbsp html entities to spaces
            cnt = Regex.Replace(cnt, @" {2,}", "\t"); // Note: Use of \s here break newlines

            // Compact a little those modern builds
            cnt = Regex.Replace(cnt, @"\t{2,}", "\t");
            cnt = Regex.Replace(cnt, @"(\r\n){2,}", "\r\n");

            // Call NewToon() here ?

            // Alignment, builder software and version
            // Note: old Pine Hero Designer is listed as 'Hero Hero Designer'
            // Extended: Rogue/Vigilante will be added in a later release.
            r = new Regex(@"(Hero|Villain|Rogue|Vigilante) Plan by ([a-zA-Z\:\'\s]+) ([0-9\.]+)");
            m = r.Match(cnt);

            if (!m.Success)
            {
                _ = MessageBox.Show("This build cannot be recovered because it doesn't contain a valid plain text part.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            switch (m.Groups[1].Value)
            {
                case "Hero":
                    align = 0;
                    break;

                case "Vigilante":
                    align = 2;
                    break;

                case "Rogue":
                    align = 1;
                    break;

                case "Villain":
                    align = 3;
                    break;

                default:
                    align = 0;
                    break;
            }

            MidsContext.Character.Alignment = (Enums.Alignment)align;

            this.Builder.Software = m.Groups[2].Value;
            this.Builder.Version = m.Groups[3].Value;

            // Character name, level, origin and archetype
            r = new Regex(@"([^\r\n\t]+)\: Level ([0-9]{1,2}) ([a-zA-Z]+) ([a-zA-Z ]+)");
            m = r.Match(cnt);
            if (!m.Success)
            {
                // Name is empty
                rs = new Regex(@"Level ([0-9]{1,2}) ([a-zA-Z]+) ([a-zA-Z ]+)");
                ms = rs.Match(cnt);
                MidsContext.Character.Name = String.Empty;
                // MidsContext.Character.Level = Convert.ToInt32(ms.Groups[1].Value); // For reference only
                MidsContext.Character.Origin = Convert.ToInt32(ms.Groups[2].Value, null);
                MidsContext.Character.Archetype.ClassName = ms.Groups[3].Value;
            }
            else
            {
                MidsContext.Character.Name = m.Groups[1].Value;
                // MidsContext.Character.Level = Convert.ToInt32(m.Groups[2].Value); // For reference only
                MidsContext.Character.Origin = Convert.ToInt32(m.Groups[3].Value, null);
                MidsContext.Character.Archetype.ClassName = m.Groups[4].Value;
            }

            // Main powersets
            r = new Regex(@"(Primary|Secondary) Power Set\: ([^\r\n\t]+)");
            m = r.Match(cnt);
            List<string> CPowersets = new List<string>();
            while (m.Success)
            {
                CPowersets.Add(m.Groups[2].Value);
                m = m.NextMatch();
            }

            // Pools and Ancillary/Epic powersets
            r = new Regex(@"(Power|Ancillary) Pool\: ([^\r\n\t]+)");
            m = r.Match(cnt);
            while (m.Success)
            {
                if (m.Groups[2].Value != "Fitness")
                {
                    CPowersets.Add(m.Groups[2].Value);
                }
                m = m.NextMatch();
            }

            var errors = MidsContext.Character.LoadPowersetsByName(CPowersets);

            // Powers
            string PName;
            int PLevel;
            string PSlotsStr;
            string[] PSlots;
            string[] s;
            string[] en;
            string SContentEnh;
            int SLevel;
            Enh Enhs = new Enh();

            r = new Regex(@"Level ([0-9]{1,2})\:\t([^\t]+)\t([^\r\n\t]+)");
            m = r.Match(cnt);
            while (m.Success)
            {
                PSlots = Array.Empty<string>();
                PName = m.Groups[2].Value.Trim();
                PLevel = Convert.ToInt32(m.Groups[1].Value, null);
                PSlotsStr = (m.Groups.Count > 3) ? m.Groups[3].Value.Trim() : String.Empty;
                if (!String.IsNullOrEmpty(PSlotsStr))
                {
                    // Extract enhancement name and slot level ('A' for power inherent slot)
                    // Handle special enhancements with parenthesis like ExpRnf-+Res(Pets)(50)
                    PSlotsStr = Regex.Replace(PSlotsStr, @"\(([^A0-9]+)\)", "[$1]");
                    PSlots = Regex.Split(PSlotsStr, @",\s");

                    for (i = 0; i < PSlots.Length; i++)
                    {
                        s = Regex.Split(PSlots[i], @"/[\(\)]");
                        s = Array.FindAll<string>(s, e => !String.IsNullOrWhiteSpace(e));

                        SContentEnh = (s[0] == "Empty") ? null : this.ShortNamesConversion(s[0]); // Enhancement name (Enhancement.ShortName)
                        try
                        {
                            SLevel = (s[1] == "A") ? 0 : Convert.ToInt32(s[1], null); // Slot level ("A" is the auto-granted one)
                            if (String.IsNullOrEmpty(s[0]))
                            {
                                en = Array.Empty<string>();
                                en[0] = null;
                                en[1] = null;
                            }
                            else
                            {
                                en = Regex.Split(s[0], @"\-+");
                            }

                            Enhs.Filled = true;
                            Enhs.Base = en[0];
                            Enhs.SubMember = en[1].Replace("[", "(").Replace("]", ")");
                            Enhs.Level = Convert.ToInt32(s[1], null);
                        }
                        catch (FormatException) // if (isNaN(s[1]
                        {
                            Enhs.Filled = false;
                            Enhs.Base = null;
                            Enhs.SubMember = null;
                            Enhs.Level = 0;
                        }
                    }
                }
            }

            return true;
        }
    }
}
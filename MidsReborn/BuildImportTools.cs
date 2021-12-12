#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn
{
    /***********************
    ** Helpers
    */

    public abstract class ImportBase
    {
        protected RawCharacterInfo CharacterInfo { get; set; }
        protected string BuildString { get; set; }
        protected UniqueList<string> PowerSets { get; set; }
        protected string[] excludePowersets { get; } = {"Inherent.Inherent", "Inherent.Fitness", "Redirects.Inherents"};

        protected string[] excludePowers { get; } =
        {
            "Efficient_Adaptation", "Defensive_Adaptation", "Offensive_Adaptation",
            "Form_of_the_Body", "Form_of_the_Mind", "Form_of_the_Soul",
            "Ammunition",
            "Build_Up_Proc", // Martial Manipulation (Blasters)
            /*"Black_Dwarf_", "Dark_Nova_", "White_Dwarf_", "Bright_Nova_",*/
            "Sorcery.Translocation", "Experimentation.Jaunt", "Force_of_Will.Stomp"
        };

        protected Dictionary<int, int> OldFitnessPoolIDs { get; } = new Dictionary<int, int>
        {
            [2553] = 1521,
            [2554] = 1523,
            [2555] = 1522,
            [2556] = 1524
        };

        protected void SetCharacterInfo()
        {
            // Warning: DatabaseAPI.GetArchetypeByName looks up archetype info by display name, not by internal name.
            // Meaning underscores have to be replaced with spaces for VEATs...
            MidsContext.Character.Archetype = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype.Replace("_", " "));
            MidsContext.Character.Origin =
                DatabaseAPI.GetOriginByName(MidsContext.Character.Archetype, CharacterInfo.Origin);
            MidsContext.Character.Reset(MidsContext.Character.Archetype, MidsContext.Character.Origin);
            MidsContext.Character.Name = CharacterInfo.Name;
            MidsContext.Character.Alignment = CharacterInfo.Alignment switch
            {
                "Hero" => Enums.Alignment.Hero,
                "Vigilante" => Enums.Alignment.Vigilante,
                "Rogue" => Enums.Alignment.Rogue,
                "Villain" => Enums.Alignment.Villain,
                "Loyalist" => Enums.Alignment.Loyalist,
                "Resistance" => Enums.Alignment.Resistance,
                _ => MidsContext.Character.IsHero() ? Enums.Alignment.Hero : Enums.Alignment.Villain
            };

            MidsContext.Character.SetLevelTo(CharacterInfo.Level - 1);
        }

        protected I9Slot SelectEnhancementByIdx(int enhID, string enhInternalName)
        {
            var i9Slot = new I9Slot
            {
                //Enh = DatabaseAPI.Get EnhancementByUIDName(aSlots[j].InternalName);
                Enh = enhID
            };

            //str1 = buildFileLinesArray[index3].enhancementName;
            if (i9Slot.Enh == -1)
            {
                var iName = enhInternalName.Replace("Attuned", "Crafted").Replace("Synthetic_", string.Empty);
                i9Slot.Enh = DatabaseAPI.GetEnhancementByUIDName(iName);
                if (i9Slot.Enh == -1)
                {
                    _ = MessageBox.Show("Error getting data for enhancement UID: " + enhInternalName, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    i9Slot.Enh = 0;
                }
            }

            return i9Slot;
        }

        protected void AddPowerToBuildSheet(RawPowerData p, ref List<PowerEntry> listPowers)
        {
            int i;
            var powerEntry = new PowerEntry
            {
                Level = p.Level,
                StatInclude = false,
                VariableValue = 0,
                Slots = new SlotEntry[p.Slots.Count]
            };

            if (p.Slots.Count > 0)
            {
                for (i = 0; i < powerEntry.Slots.Length; i++)
                {
                    powerEntry.Slots[i] = new SlotEntry
                    {
                        Level = 49,
                        Enhancement = new I9Slot(),
                        FlippedEnhancement = new I9Slot()
                    };
                    if (p.Slots[i].InternalName == "Empty") continue;

                    var i9Slot = SelectEnhancementByIdx(p.Slots[i].eData, p.Slots[i].InternalName);
                    var enhType = DatabaseAPI.Database.Enhancements[i9Slot.Enh].TypeID;

                    if (enhType == Enums.eType.Normal || enhType == Enums.eType.SpecialO)
                    {
                        i9Slot.RelativeLevel =
                            (Enums.eEnhRelative) (p.Slots[i].Boosters + 4); // +4 === 5 boosters ??? Damn you, maths.
                        i9Slot.Grade = Enums.eEnhGrade.SingleO;
                    }

                    if (enhType == Enums.eType.InventO || enhType == Enums.eType.SetO)
                    {
                        // Current enhancement level: //p.Slots[i].Level;
                        // Set to maximum since attuned ones will give the lowest level possible.
                        i9Slot.IOLevel = DatabaseAPI.Database.Enhancements[i9Slot.Enh].LevelMax;
                        i9Slot.RelativeLevel = (Enums.eEnhRelative) (p.Slots[i].Boosters + 4);
                    }

                    powerEntry.Slots[i].Enhancement = i9Slot;
                }

                if (powerEntry.Slots.Length > 0) powerEntry.Slots[0].Level = powerEntry.Level;
            }

            powerEntry.NIDPower = p.pData.PowerIndex;
            powerEntry.NIDPowerset = p.pData.PowerSetID;
            powerEntry.IDXPower = p.pData.PowerSetIndex;
            if (powerEntry.Level == 0 && powerEntry.Power.FullSetName == "Pool.Fitness")
            {
                powerEntry.NIDPower = OldFitnessPoolIDs[powerEntry.NIDPower];
                powerEntry.NIDPowerset = p.pData.PowerSetID;
                powerEntry.IDXPower = p.pData.PowerSetIndex;
            }

            try
            {
                listPowers.Add(powerEntry);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        // CheckValid, from power/powerset fullName
        protected bool CheckValid(string input, Enums.eValidationType validateType)
        {
            string[] excludes;
            switch (validateType)
            {
                case Enums.eValidationType.Powerset:
                    excludes = excludePowersets;
                    break;

                case Enums.eValidationType.Power:
                    excludes = excludePowers;
                    break;

                default:
                    return false;
            }

            ;

            return !excludes.Any(x => input.Contains(x));
        }

        // CheckValid, for direct powerset result
        // Since DatabaseAPI.GetPowersetByName may return null
        protected bool CheckValid(IPowerset? input)
        {
            if (input == null) return false;

            return !excludePowersets.Any(x => input.FullName.Contains(x));
        }

        // CheckValid, for direct powerset result
        // Since DatabaseAPI.GetPowerByName may return null
        protected bool CheckValid(IPower? input)
        {
            if (input == null) return false;

            return !excludePowers.Any(x => input.FullName.Contains(x));
        }

        protected string FixKheldPowerNames(string powerName)
        {
            return powerName.Replace("Warshade_Defensive.Umbral_Aura.", "Inherent.Inherent.")
                .Replace("Warshade_Offensive.Umbral_Blast.", "Inherent.Inherent.")
                .Replace("Peacebringer_Offensive.Luminous_Blast.", "Inherent.Inherent.")
                .Replace("Peacebringer_Defensive.Luminous_Aura.", "Inherent.Inherent.");
        }

        public UniqueList<string> GetPowersets()
        {
            return PowerSets;
        }

        public RawCharacterInfo GetCharacterInfo()
        {
            return CharacterInfo;
        }
    }

    /***********************
    ** Import from /buildsave .txt builds
    */
    public class ImportFromBuildsave : ImportBase
    {
        private readonly int HeaderSize = 4; // Number of lines before actual build data

        public ImportFromBuildsave(string buildString)
        {
            BuildString = buildString;
            PowerSets = new UniqueList<string>();
            CharacterInfo = new RawCharacterInfo();
        }

        // Some power internal names differs from game
        private string CheckForAliases(string fullName)
        {
            return fullName.ToLowerInvariant() switch
            {
                "pool.flight.afterburner" => "Pool.Flight.Evasive_Maneuvers",
                "peacebringer_defensive.luminous_aura.quantum_acceleration" => "Peacebringer_Defensive.Luminous_Aura.Quantum_Maneuvers",
                _ => fullName
            };
        }

        public List<PowerEntry>? Parse()
        {
            string rawPowerset;
            var p = new RawPowerData {Valid = false};
            var powerSlots = new List<RawEnhData>();
            RawEnhData e;

            var listPowers = new List<PowerEntry>();
            string[] powerIDChunks;

            var r1 = new Regex(@"^Level ([0-9]+)\: (.+)$");
            var r2 = new Regex(@"^[\t\s]*EMPTY$");
            var r3 = new Regex(@"^[\t\s]*([0-9a-zA-Z\+\:\-_]+) \(([0-9]+)(\+([1-5]))?\)$");

            var line = -1;
            string lineText;
            using var streamReader = new StreamReader(BuildString);
            while ((lineText = streamReader.ReadLine()) != null)
            {
                line++;

                if (line == 0)
                {
                    // Name, Level, Origin, ClassID
                    var r = new Regex(@"^([^\:]+)\: Level ([0-9]+) ([a-zA-Z]+) ([a-zA-Z_]+)$");
                    var m = r.Match(lineText);
                    if (!m.Success)
                    {
                        MessageBox.Show("This build cannot be imported because it doesn't match the expected format.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    CharacterInfo.Name = m.Groups[1].Value;
                    CharacterInfo.Archetype = m.Groups[4].Value.Replace("Class_", "");
                    CharacterInfo.Origin = m.Groups[3].Value;
                    CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);

                    SetCharacterInfo();
                }
                else if (line < HeaderSize)
                {
                    continue;
                }

                var m1 = r1.Match(lineText);
                var m2 = r2.Match(lineText);
                var m3 = r3.Match(lineText);

                if (m1.Success)
                {
                    if (p.Valid) AddPowerToBuildSheet(p, ref listPowers);

                    powerIDChunks = m1.Groups[2].Value.Split(' ');
                    rawPowerset = (powerIDChunks[0] + "." + powerIDChunks[1]).Trim();
                    p.FullName = CheckForAliases(m1.Groups[2].Value.Replace(" ", "."));
                    p.Powerset = DatabaseAPI.GetPowersetByName(rawPowerset);
                    p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    if (p.pData == null)
                    {
                        p.FullName = FixKheldPowerNames(p.FullName);
                        powerIDChunks = p.FullName.Split('.');
                        rawPowerset = (powerIDChunks[0] + "." + powerIDChunks[1]).Trim();
                        p.Powerset = DatabaseAPI.GetPowersetByName(rawPowerset);
                        p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    }

                    p.Valid = CheckValid(p.pData);
                    p.Level = Convert.ToInt32(m1.Groups[1].Value, null);
                    p.Slots = new List<RawEnhData>();
                    if (CheckValid(p.Powerset)) PowerSets.Add(p.Powerset.FullName);
                }
                else if (m2.Success)
                {
                    // Empty slot
                    e = new RawEnhData
                    {
                        InternalName = "Empty",
                        Level = 0,
                        Boosters = 0,
                        HasCatalyst = false,
                        eData = -1
                    };
                    p.Slots.Add(e);
                }
                else if (m3.Success)
                {
                    // Enhancement: internal name, level, (dummy), boosters
                    e = new RawEnhData
                    {
                        InternalName = DatabaseAPI.GetEnhancementBaseUIDName(m3.Groups[1].Value),
                        Level = Convert.ToInt32(m3.Groups[2].Value, null),
                        Boosters = (m3.Groups.Count > 3) & !string.IsNullOrWhiteSpace(m3.Groups[4].Value)
                            ? Convert.ToInt32(m3.Groups[4].Value, null)
                            : 0,
                        HasCatalyst = DatabaseAPI.EnhHasCatalyst(m3.Groups[1].Value)
                    };
                    e.eData = DatabaseAPI.GetEnhancementByUIDName(e.InternalName);

                    p.Slots.Add(e);
                }
            }

            streamReader.Close();
            if (p.Valid) AddPowerToBuildSheet(p, ref listPowers);

            return listPowers;
        }
    }

    /***********************
    ** Plain-text import from .mxd files
    ** (Build recovery mode)
    */

    public class PlainTextParser : ImportBase
    {
        private readonly BuilderApp BuilderApp;

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

        public PlainTextParser(string buildString)
        {
            BuildString = buildString;
            PowerSets = new UniqueList<string>();
            CharacterInfo = new RawCharacterInfo();
            BuilderApp = new BuilderApp();
        }

        private string ShortNamesConversion(string sn)
        {
            if (string.IsNullOrEmpty(sn)) return sn;

            foreach (var k in OldSetNames)
                if (sn.IndexOf(k.Key, StringComparison.Ordinal) > -1)
                    return sn.Replace(k.Key, k.Value);

            return sn;
        }

        public List<PowerEntry>? Parse()
        {
            Regex r;
            Regex rs;
            Match m;
            Match ms;
            string cnt;
            int i;

            var p = new RawPowerData {Valid = false};
            var powerSlots = new List<RawEnhData>();
            var e = new RawEnhData();

            var listPowers = new List<PowerEntry>();

            try
            {
                cnt = File.ReadAllText(BuildString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }

            cnt = Regex.Replace(cnt, @"[\r\n]", "\r\n"); // Line conversion to PC/Win format (just in case)
            cnt = Regex.Replace(cnt, @"\<br \/\>", string.Empty); // For good ol' Mids 1.962 support
            cnt = Regex.Replace(cnt, @"\&nbsp\;", " "); // Nbsp html entities to spaces
            cnt = Regex.Replace(cnt, @" {2,}", "\t"); // Note: [JS] Use of \s here break newlines

            // Compact a little those modern builds
            cnt = Regex.Replace(cnt, @"\t{2,}", "\t");
            cnt = Regex.Replace(cnt, @"(\r\n){2,}", "\r\n");

            // Alignment, builder software and version
            // Note: old Pine Hero Designer is listed as 'Hero Hero Designer'
            // Extended: Rogue/Vigilante will be added in a later release.
            r = new Regex(@"(Hero|Villain|Rogue|Vigilante) Plan by ([a-zA-Z\:\'\s]+) ([0-9\.]+)");
            m = r.Match(cnt);

            if (!m.Success)
            {
                _ = MessageBox.Show(
                    "This build cannot be recovered because it doesn't contain a valid plain text part.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            CharacterInfo.Alignment = m.Groups[1].Value;
            BuilderApp.Software = m.Groups[2].Value;
            BuilderApp.Version = m.Groups[3].Value;

            // Character name, level, origin and archetype
            r = new Regex(@"([^\r\n\t]+)\: Level ([0-9]{1,2}) ([a-zA-Z]+) ([a-zA-Z ]+)");
            m = r.Match(cnt);
            if (!m.Success)
            {
                // Name is empty
                rs = new Regex(@"Level ([0-9]{1,2}) ([a-zA-Z]+) ([a-zA-Z ]+)");
                ms = rs.Match(cnt);
                CharacterInfo.Name = string.Empty;
                CharacterInfo.Level = Convert.ToInt32(ms.Groups[1].Value, null);
                CharacterInfo.Origin = ms.Groups[2].Value;
                CharacterInfo.Archetype = ms.Groups[3].Value;
            }
            else
            {
                CharacterInfo.Name = m.Groups[1].Value;
                CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);
                CharacterInfo.Origin = m.Groups[3].Value;
                CharacterInfo.Archetype = m.Groups[4].Value;
            }

            SetCharacterInfo();

            // Main powersets
            r = new Regex(@"(Primary|Secondary) Power Set\: ([^\r\n\t]+)");
            m = r.Match(cnt);
            while (m.Success)
            {
                PowerSets.Add(m.Groups[2].Value);
                m = m.NextMatch();
            }

            // Pools and Ancillary/Epic powersets
            r = new Regex(@"(Power|Ancillary) Pool\: ([^\r\n\t]+)");
            m = r.Match(cnt);
            while (m.Success)
            {
                if (m.Groups[2].Value != "Fitness") PowerSets.Add(m.Groups[2].Value);
                m = m.NextMatch();
            }

            // Powers
            string PSlotsStr;
            string[] PSlots;
            string[] s;
            string? sContentEnh;
            r = new Regex(@"Level ([0-9]{1,2})\:\t([^\t]+)\t([^\r\n\t]+)");
            m = r.Match(cnt);
            while (m.Success)
            {
                p = new RawPowerData();
                e = new RawEnhData();
                p.Slots = new List<RawEnhData>();

                p.DisplayName = m.Groups[2].Value.Trim();
                p.Level = Convert.ToInt32(m.Groups[1].Value, null);
                p.pData = DatabaseAPI.GetPowerByDisplayName(p.DisplayName,
                    DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype).Idx);
                p.Powerset = p.pData != null ? DatabaseAPI.GetPowersetByIndex(p.pData.PowerSetIndex) : null;
                p.Valid = CheckValid(p.pData);
                PSlotsStr = m.Groups.Count > 3 ? m.Groups[3].Value.Trim() : string.Empty;
                if (!string.IsNullOrEmpty(PSlotsStr))
                {
                    // Extract enhancement name and slot level ('A' for power inherent slot)
                    // Handle special enhancements with parenthesis like ExpRnf-+Res(Pets)(50)
                    PSlotsStr = Regex.Replace(PSlotsStr, @"\(([^A0-9]+)\)", "[$1]");
                    PSlots = Regex.Split(PSlotsStr, @",\s");

                    for (i = 0; i < PSlots.Length; i++)
                    {
                        s = Regex.Split(PSlots[i], @"/[\(\)]");
                        s = Array.FindAll(s, e => !string.IsNullOrWhiteSpace(e));

                        sContentEnh =
                            s[0] == "Empty"
                                ? null
                                : ShortNamesConversion(s[0]); // Enhancement name (Enhancement.ShortName)
                        try
                        {
                            e.InternalName = s[0];
                            e.Level = s[1] == "A"
                                ? 0
                                : Convert.ToInt32(s[1], null); // Slot level ("A" is the auto-granted one)
                            e.Boosters = 0; // Not handled
                            e.HasCatalyst = false;
                            e.eData = DatabaseAPI.GetEnhancementByUIDName(e.InternalName);
                            p.Slots.Add(e);
                        }
                        catch (FormatException) // if (isNaN(s[1]
                        {
                            e.InternalName = "Empty";
                            e.Level = 0;
                            e.Boosters = 0;
                            e.HasCatalyst = false;
                            e.eData = -1;
                            p.Slots.Add(e);
                        }
                    }
                }

                if (p.Valid)
                {
                    if (CheckValid(p.Powerset)) PowerSets.Add(p.Powerset.FullName);
                    AddPowerToBuildSheet(p, ref listPowers);
                }
            }

            return listPowers;
        }

/*
        public List<string> GetPowerSets()
        {
            return PowerSets;
        }
*/
    }
}
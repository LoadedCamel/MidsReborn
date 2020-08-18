using System;
using System.Collections.Generic;
using Base.Master_Classes;
using Base.Data_Classes;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using HeroViewer.Base;

namespace HeroViewer
{
    
    /***********************
    ** Helpers
    */

    public abstract class ImportBase
    {
        protected RawCharacterInfo CharacterInfo { get; set; }
        protected string BuildString { get; set; }
        protected UniqueList<string> PowerSets { get; set; }
        protected string[] excludePowersets { get; } = new[] { "Inherent.Inherent", "Inherent.Fitness", "Redirects.Inherents" };
        protected string[] excludePowers { get; } = new[] {
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
            MidsContext.Character.Origin = DatabaseAPI.GetOriginByName(MidsContext.Character.Archetype, CharacterInfo.Origin);
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
                _ => MidsContext.Character.IsHero() ? Enums.Alignment.Hero : Enums.Alignment.Villain,
            };

            MidsContext.Character.SetLevelTo(CharacterInfo.Level - 1);
        }

        protected I9Slot SelectEnhancementByIdx(int enhID, string enhInternalName)
        {
            I9Slot i9Slot = new I9Slot
            {
                //Enh = DatabaseAPI.Get EnhancementByUIDName(aSlots[j].InternalName);
                Enh = enhID
            };

            //str1 = buildFileLinesArray[index3].enhancementName;
            if (i9Slot.Enh == -1)
            {
                string iName = enhInternalName.Replace("Attuned", "Crafted").Replace("Synthetic_", string.Empty);
                i9Slot.Enh = DatabaseAPI.GetEnhancementByUIDName(iName);
                if (i9Slot.Enh == -1)
                {
                    _ = MessageBox.Show("Error getting data for enhancement UID: " + enhInternalName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    i9Slot.Enh = 0;
                }
            }

            return i9Slot;
        }
        protected void AddPowerToBuildSheet(RawPowerData p, ref List<PowerEntry> listPowers)
        {
            int i;
            PowerEntry powerEntry = new PowerEntry
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

                    I9Slot i9Slot = SelectEnhancementByIdx(p.Slots[i].eData, p.Slots[i].InternalName);
                    Enums.eType enhType = DatabaseAPI.Database.Enhancements[i9Slot.Enh].TypeID;

                    if (enhType == Enums.eType.Normal || enhType == Enums.eType.SpecialO)
                    {
                        i9Slot.RelativeLevel = (Enums.eEnhRelative)(p.Slots[i].Boosters + 4); // +4 === 5 boosters ??? Damn you, maths.
                        i9Slot.Grade = Enums.eEnhGrade.SingleO;
                    }

                    if (enhType == Enums.eType.InventO || enhType == Enums.eType.SetO)
                    {
                        // Current enhancement level: //p.Slots[i].Level;
                        // Set to maximum since attuned ones will give the lowest level possible.
                        i9Slot.IOLevel = DatabaseAPI.Database.Enhancements[i9Slot.Enh].LevelMax;
                        i9Slot.RelativeLevel = (Enums.eEnhRelative)(p.Slots[i].Boosters + 4);
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
                powerEntry.NIDPower = this.OldFitnessPoolIDs[powerEntry.NIDPower];
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
            };

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

        public List<PowerEntry>? Parse()
        {
            Regex r; Match m;
            Regex r1; Regex r2; Regex r3;
            Match m1; Match m2; Match m3;

            string rawPowerset;
            RawPowerData p = new RawPowerData { Valid = false };
            List<RawEnhData> powerSlots = new List<RawEnhData>();
            RawEnhData e;

            List<PowerEntry> listPowers = new List<PowerEntry>();
            string[] powerIDChunks;

            r1 = new Regex(@"^Level ([0-9]+)\: (.+)$"); // Picked power
            r2 = new Regex(@"^[\t\s]*EMPTY$"); // Empty enhancement slot
            r3 = new Regex(@"^[\t\s]*([0-9a-zA-Z\+\:\-_]+) \(([0-9]+)(\+([1-5]))?\)$"); // Filled enhancement slot

            int line = -1;
            string lineText;
            using StreamReader streamReader = new StreamReader(BuildString);
            while ((lineText = streamReader.ReadLine()) != null)
            {
                line++;

                if (line == 0)
                {
                    // Name, Level, Origin, ClassID
                    r = new Regex(@"^([^\:]+)\: Level ([0-9]+) ([a-zA-Z]+) ([a-zA-Z_]+)$");
                    m = r.Match(lineText);
                    if (!m.Success)
                    {
                        MessageBox.Show("This build cannot be imported because it doesn't match the expected format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    CharacterInfo.Name = m.Groups[1].Value;
                    CharacterInfo.Archetype = m.Groups[4].Value.Replace("Class_", string.Empty);
                    CharacterInfo.Origin = m.Groups[3].Value;
                    CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);

                    SetCharacterInfo();
                }
                else if (line < HeaderSize)
                {
                    continue;
                }

                m1 = r1.Match(lineText);
                m2 = r2.Match(lineText);
                m3 = r3.Match(lineText);

                if (m1.Success)
                {
                    if (p.Valid)
                    {
                        AddPowerToBuildSheet(p, ref listPowers);
                    }

                    powerIDChunks = m1.Groups[2].Value.Split(' ');
                    rawPowerset = (powerIDChunks[0] + "." + powerIDChunks[1]).Trim();
                    p.FullName = m1.Groups[2].Value.Replace(" ", ".");
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
                    if (CheckValid(p.Powerset))
                    {
                        PowerSets.Add(p.Powerset.FullName);
                    }
                }
                else if (m2.Success)
                {
                    // Empty slot
                    e = new RawEnhData() {
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
                    // Zed 08/10/20: Fix for bug "Cannot load enhancement data for 'Crafted_Artillery_X'"
                    // Internal name for these are Crafted_Shrapnel_X
                    e = new RawEnhData() {
                        InternalName = DatabaseAPI.GetEnhancementBaseUIDName(m3.Groups[1].Value.Contains("Artillery") ? m3.Groups[1].Value.Replace("Artillery", "Shrapnel") : m3.Groups[1].Value),
                        Level = Convert.ToInt32(m3.Groups[2].Value, null),
                        Boosters = m3.Groups.Count > 3 & !string.IsNullOrWhiteSpace(m3.Groups[4].Value) ? Convert.ToInt32(m3.Groups[4].Value, null) : 0,
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
        private BuilderApp BuilderApp;

        public PlainTextParser(string buildString)
        {
            BuildString = buildString;
            PowerSets = new UniqueList<string>();
            CharacterInfo = new RawCharacterInfo();
            BuilderApp = new BuilderApp();
        }

        private static string ShortNamesConversion(string sn)
        {
            if (string.IsNullOrWhiteSpace(sn)) return sn;

            sn = Database.Instance.OldSetNames.Aggregate(sn, (current, value) => current.Replace(value.Key, value.Value));

            EnhancementSet enhSet = Database.Instance.EnhancementSets
                .Where(e => e.ShortName == sn.Split('-')[0])
                .First();
            
            var enhData = Database.Instance.OldEnhNames
                .Where(e => e.Value[2] == enhSet.Uid && sn.IndexOf(e.Value[1], StringComparison.OrdinalIgnoreCase) > -1)
                .Select(e => new { UID = e.Key, OldShortName = e.Value[0] })
                .First();

            int enhIdx = DatabaseAPI.GetEnhancementByUIDName(enhData.UID);
            if (enhIdx == -1) return sn;

            string newEnhSn = Database.Instance.Enhancements[enhIdx].ShortName;

            sn = sn.Replace(enhData.OldShortName, newEnhSn);

            return sn;
        }

        public List<PowerEntry>? Parse()
        {
            Regex r; Regex rs;
            Match m; Match ms;
            string cnt;
            int i;

            RawPowerData p = new RawPowerData { Valid = false };
            List<RawEnhData> powerSlots = new List<RawEnhData>();
            RawEnhData e = new RawEnhData();

            List<PowerEntry> listPowers = new List<PowerEntry>();

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
                _ = MessageBox.Show("This build cannot be recovered because it doesn't contain a valid plain text part.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // If Name is empty
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
                if (m.Groups[2].Value != "Fitness")
                {
                    PowerSets.Add(m.Groups[2].Value);
                }
                m = m.NextMatch();
            }

            // Powers
            string PSlotsStr; string[] PSlots; string[] s;
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
                p.pData = DatabaseAPI.GetPowerByDisplayName(p.DisplayName, DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype).Idx);
                p.Powerset = p.pData != null ? DatabaseAPI.GetPowersetByIndex(p.pData.PowerSetIndex) : null;
                p.Valid = CheckValid(p.pData);
                PSlotsStr = (m.Groups.Count > 3) ? m.Groups[3].Value.Trim() : string.Empty;
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

                        // Enhancement name (Enhancement.ShortName)
                        sContentEnh = (s[0] == "Empty") ? null : ShortNamesConversion(s[0]);
                        try
                        {
                            e.InternalName = s[0];
                            e.Level = (s[1] == "A") ? 0 : Convert.ToInt32(s[1], null); // Slot level ("A" is the auto-granted one)
                            e.Boosters = 0; // Not handled
                            e.HasCatalyst = false;
                            e.eData = DatabaseAPI.GetEnhancementByUIDName(e.InternalName);
                            p.Slots.Add(e);
                        }
                        catch (FormatException) // if (isNaN(s[1]))
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
                    if (CheckValid(p.Powerset))
                    {
                        PowerSets.Add(p.Powerset.FullName);
                    }
                    AddPowerToBuildSheet(p, ref listPowers);
                }
            }

            return listPowers;
        }

        public List<string> GetPowerSets()
        {
            return PowerSets;
        }
    }
}
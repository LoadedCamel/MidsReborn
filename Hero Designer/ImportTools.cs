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
                //Enh = DatabaseAPI.GetEnhancementByUIDName(aSlots[j].InternalName);
                Enh = enhID
            };

            //str1 = buildFileLinesArray[index3].enhancementName;
            if (i9Slot.Enh == -1)
            {
                string iName = enhInternalName.Replace("Attuned", "Crafted").Replace("Synthetic_", String.Empty);
                i9Slot.Enh = DatabaseAPI.GetEnhancementByUIDName(iName);
                if (i9Slot.Enh == -1)
                {
                    _ = MessageBox.Show(("Error getting data for enhancement UID: " + enhInternalName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    excludes = this.excludePowersets;
                    break;

                case Enums.eValidationType.Power:
                    excludes = this.excludePowers;
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

            return !this.excludePowersets.Any(x => input.FullName.Contains(x));
        }

        // CheckValid, for direct powerset result
        // Since DatabaseAPI.GetPowerByName may return null
        protected bool CheckValid(IPower? input)
        {
            if (input == null) return false;

            return !this.excludePowers.Any(x => input.FullName.Contains(x));
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
            return this.PowerSets;
        }

        public RawCharacterInfo GetCharacterInfo()
        {
            return this.CharacterInfo;
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
            this.BuildString = buildString;
            this.PowerSets = new UniqueList<string>();
            this.CharacterInfo = new RawCharacterInfo();
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
            using StreamReader streamReader = new StreamReader(this.BuildString);
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

                    this.CharacterInfo.Name = m.Groups[1].Value;
                    this.CharacterInfo.Archetype = m.Groups[4].Value.Replace("Class_", String.Empty);
                    this.CharacterInfo.Origin = m.Groups[3].Value;
                    this.CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);

                    SetCharacterInfo();
                }
                else if (line < this.HeaderSize)
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
                        p.FullName = this.FixKheldPowerNames(p.FullName);
                        powerIDChunks = p.FullName.Split('.');
                        rawPowerset = (powerIDChunks[0] + "." + powerIDChunks[1]).Trim();
                        p.Powerset = DatabaseAPI.GetPowersetByName(rawPowerset);
                        p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    }
                    p.Valid = this.CheckValid(p.pData);
                    p.Level = Convert.ToInt32(m1.Groups[1].Value, null);
                    p.Slots = new List<RawEnhData>();
                    if (this.CheckValid(p.Powerset))
                    {
                        this.PowerSets.Add(p.Powerset.FullName);
                    }
                }
                else if (m2.Success)
                {
                    // Empty slot
                    e = new RawEnhData();
                    e.InternalName = "Empty";
                    e.Level = 0;
                    e.Boosters = 0;
                    e.HasCatalyst = false;
                    e.eData = -1;
                    p.Slots.Add(e);
                }
                else if (m3.Success)
                {
                    // Enhancement: internal name, level, (dummy), boosters
                    e = new RawEnhData();
                    e.InternalName = DatabaseAPI.GetEnhancementBaseUIDName(m3.Groups[1].Value);
                    e.Level = Convert.ToInt32(m3.Groups[2].Value, null);
                    e.Boosters = m3.Groups.Count > 3 & !string.IsNullOrWhiteSpace(m3.Groups[4].Value) ? Convert.ToInt32(m3.Groups[4].Value, null) : 0;
                    e.HasCatalyst = DatabaseAPI.EnhHasCatalyst(m3.Groups[1].Value);
                    e.eData = DatabaseAPI.GetEnhancementByUIDName(e.InternalName);
                    p.Slots.Add(e);
                }
            }

            streamReader.Close();
            if (p.Valid) AddPowerToBuildSheet(p, ref listPowers);

            return listPowers;
        }
    }
}
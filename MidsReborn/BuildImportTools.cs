#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

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
        protected string[] ExcludePowersets { get; } = { "Redirects." }; //{"Inherent.Inherent", "Inherent.Fitness", "Redirects.Inherents"};

        protected string[] ExcludePowers { get; } =
        {
            "Efficient_Adaptation", "Defensive_Adaptation", "Offensive_Adaptation", // Bio armor
            "Form_of_the_Body", "Form_of_the_Mind", "Form_of_the_Soul", // Staff Fighting (all but stalkers)
            "Ammunition", // Dual Pistols
            "Build_Up_Proc", // Martial Manipulation (Blasters)
            /*"Black_Dwarf_", "Dark_Nova_", "White_Dwarf_", "Bright_Nova_",*/
            "Sorcery.Translocation", "Experimentation.Jaunt", "Force_of_Will.Stomp",
            "Speed.SpeedPhase", "Leaping.Double_Jump",
            "Flight.Fly_Boost" // Afterburner
        };

        protected Dictionary<int, int> OldFitnessPoolIDs { get; } = new()
        {
            [2553] = 1521,
            [2554] = 1523,
            [2555] = 1522,
            [2556] = 1524
        };

        protected string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            var place = source.IndexOf(find, StringComparison.Ordinal);
            
            return place < 0
                ? source
                : source.Remove(place, find.Length).Insert(place, replace);
        }

        protected void SetCharacterInfo()
        {
            // Warning: DatabaseAPI.GetArchetypeByName looks up archetype info by display name, not by internal name.
            // Meaning underscores have to be replaced with spaces for VEATs...
            MidsContext.Character.Archetype = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype.Replace("_", " "));
            MidsContext.Character.Origin = DatabaseAPI.GetOriginByName(MidsContext.Character.Archetype, CharacterInfo.Origin);
            MidsContext.Character.Reset(MidsContext.Character.Archetype, MidsContext.Character.Origin);
            MidsContext.Character.Name = CharacterInfo.Name;
            /*
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
            */

            MidsContext.Character.Alignment = CharacterInfo.Alignment switch
            {
                "Hero" or "Vigilante" or "Resistance" => Enums.Alignment.Hero,
                "Villain" or "Rogue" or "Loyalist" => Enums.Alignment.Villain,
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
            if (i9Slot.Enh != -1)
            {
                return i9Slot;
            }

            var iName = enhInternalName.Replace("Attuned", "Crafted").Replace("Synthetic_", string.Empty);
            var r = new Regex(@"^(Science|Mutation|Technology|Natural)_(?!Science|Mutation|Technology|Natural|Magic)"); // SOs
            if (r.IsMatch(iName))
            {
                iName = r.Replace(iName, "Magic_");
                i9Slot.Grade = Enums.eEnhGrade.SingleO;
            }

            r = new Regex(@"^(Science|Mutation|Technology|Natural|Magic)_(Science|Mutation|Technology|Natural|Magic)"); // DOs
            if (r.IsMatch(iName))
            {
                iName = r.Replace(iName, "Magic");
                i9Slot.Grade = Enums.eEnhGrade.DualO;
            }

            i9Slot.Enh = DatabaseAPI.GetEnhancementByUIDName(iName);
            if (i9Slot.Enh != -1)
            {
                return i9Slot;
            }

            if (Debugger.IsAttached)
            {
                MessageBox.Show($"Error getting data for enhancement UID: {enhInternalName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            i9Slot.Enh = 0;

            return i9Slot;
        }

        protected void AddPowerToBuildSheet(RawPowerData p, ref List<PowerEntry> listPowers)
        {
            var powerEntry = new PowerEntry
            {
                Level = p.Level,
                StatInclude = false,
                VariableValue = 0,
                Slots = new SlotEntry[p.Slots.Count]
            };

            if (p.Slots.Count > 0)
            {
                for (var i = 0; i < powerEntry.Slots.Length; i++)
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

                    switch (enhType)
                    {
                        case Enums.eType.Normal or Enums.eType.SpecialO:
                            i9Slot.RelativeLevel = (Enums.eEnhRelative) (p.Slots[i].Boosters + 4); // +4 === 5 boosters ??? Damn you, maths.
                            i9Slot.Grade = Enums.eEnhGrade.SingleO;
                            break;
                        case Enums.eType.InventO or Enums.eType.SetO:
                            // Current enhancement level: //p.Slots[i].Level;
                            // Set to maximum since attuned ones will give the lowest level possible.
                            i9Slot.IOLevel = DatabaseAPI.Database.Enhancements[i9Slot.Enh].LevelMax;
                            i9Slot.RelativeLevel = (Enums.eEnhRelative) (p.Slots[i].Boosters + 4);
                            break;
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
                    excludes = ExcludePowersets;
                    break;

                case Enums.eValidationType.Power:
                    excludes = ExcludePowers;
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
            return input != null && !ExcludePowersets.Any(ps => input.FullName.Contains(ps));
        }

        // CheckValid, for direct powerset result
        // Since DatabaseAPI.GetPowerByName may return null
        protected bool CheckValid(IPower? input)
        {
            return input != null && !ExcludePowers.Any(p => input.FullName.Contains(p));
        }

        protected string FixPowersetsNames(string powerName)
        {
            return powerName.Replace("Warshade_Defensive.Umbral_Aura.", "Inherent.Inherent.")
                .Replace("Warshade_Offensive.Umbral_Blast.", "Inherent.Inherent.")
                .Replace("Peacebringer_Offensive.Luminous_Blast.", "Inherent.Inherent.")
                .Replace("Peacebringer_Defensive.Luminous_Aura.", "Inherent.Inherent.")
                .Replace("Mastermind_Buff.Shock_Therapy.", "Mastermind_Buff.Electrical_Affinity.");
        }

        public UniqueList<string> GetPowersets()
        {
            return PowerSets;
        }

        public RawCharacterInfo GetCharacterInfo()
        {
            return CharacterInfo;
        }

        public static int CountPools(UniqueList<string> listPowersets)
        {
            return listPowersets.
                Where(ps => ps.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0)
                .ToArray()
                .Length;
        }

        public static void PadPowerPools(ref UniqueList<string> listPowersets)
        {
            var nbPools = CountPools(listPowersets);

            if (nbPools == 4) return;

            var pickedPowerPools = listPowersets
                .Where(ps => ps.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0)
                .ToArray();
            var dbPowerPools = Database.Instance.Powersets
                .Where(ps =>
                    ps.FullName.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0 &&
                    !pickedPowerPools.Contains(ps.FullName))
                .OrderBy(e => e.DisplayName)
                .Select(e => e.FullName)
                .ToArray();

            for (var i = 0; i < 4 - nbPools; i++)
            {
                listPowersets.Add(dbPowerPools[i]);
            }
        }

        public static void FilterVEATPools(ref UniqueList<string> listPowersets)
        {
            if (listPowersets.Contains("Training_Gadgets.Bane_Spider_Training") ||
                listPowersets.Contains("Training_Gadgets.Crab_Spider_Training"))
            {
                listPowersets.FromList(listPowersets.Where(e => e != "Training_Gadgets.Training_and_Gadgets").ToList());
                return;
            }

            if (listPowersets.Contains("Widow_Training.Fortunata_Training") ||
                listPowersets.Contains("Widow_Training.Night_Widow_Training"))
                listPowersets.FromList(listPowersets
                    .Where(e => e != "Widow_Training.Widow_Training" && e != "Teamwork.Teamwork").ToList());
        }

        public static void FixUndetectedPowersets(ref UniqueList<string> listPowersets)
        {
            for (var i = 0; i < listPowersets.Count; i++)
            {
                if (i == 2) continue;

                if (listPowersets[i] == null || listPowersets[i] == "")
                {
                    var setType = i switch
                    {
                        0 => Enums.ePowerSetType.Primary,
                        1 => Enums.ePowerSetType.Secondary,
                        7 => Enums.ePowerSetType.Ancillary,
                        _ => Enums.ePowerSetType.Pool
                    };
                    var ps1 = DatabaseAPI.Database.Powersets
                        .First(ps => ps.ATClass == MidsContext.Character.Archetype.DisplayName & ps.SetType == setType);

                    listPowersets[i] = ps1.FullName;
                }
            }
        }

        public static void FinalizePowersetsList(ref UniqueList<string> listPowersets)
        {
            listPowersets.FromList(listPowersets.GetRange(0, 7));
            listPowersets.FromList(listPowersets.Select(e => e.Contains(".")
                ? e
                : DatabaseAPI.GetPowersetByName(e, MidsContext.Character.Archetype.DisplayName, true).FullName
            ).ToList());
        }

        public static void FinalizePowersetsList(ref UniqueList<string> listPowersets, List<PowerEntry> listPowers)
        {
            listPowersets.FromList(
                listPowers
                    .Where(e => e.Power != null)
                    .Select(e => e.Power.GetPowerSet()?.FullName)
                    .Where(e => !string.IsNullOrWhiteSpace(e) && e != "Inherent.Inherent" && e != "Inherent.Fitness")
                    .Distinct()
                    .ToList()!);
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
                "dominator_control.illusion_control.invisibility" => "Dominator_Control.Illusion_Control.Superior_Invisibility",
                "dominator_control.illusion_control.decoy" => "Dominator_Control.Illusion_Control.Phantom_Army",
                _ => fullName
            };
        }

        private string CheckForPowersetAliases(string fullName, string archetype)
        {
            archetype = archetype.ToLowerInvariant();
            
            return fullName.ToLowerInvariant() switch
            {
                "epic.defender_flame_mastery" when archetype == "defender" => "Epic.Def_Flame_Mastery",
                "epic.stalker_mace_mastery" when archetype == "scrapper" => "Epic.Scrapper_Mace_Mastery",
                "epic.corruptor_fire_mastery" when archetype == "corruptor" => "Epic.Corr_Flame_Mastery",
                "epic.sentinel_electricity_mastery" when archetype == "sentinel" => "Epic.Sentinel_Elec_Mastery",
                "epic.sentinel_leviathan_mastery" when archetype == "sentinel" => "Epic.Sentinel_Lev_Mastery",
                "epic.sentinel_psionic_mastery" when archetype == "sentinel" => "Epic.Sentinel_Psi_Mastery",
                _ => fullName
            };
        }

        public List<PowerEntry>? Parse()
        {
            var p = new RawPowerData {Valid = false};
            var listPowers = new List<PowerEntry>();

            var r1 = new Regex(@"^Level ([0-9]+)\: (.+)$");
            var r2 = new Regex(@"^[\t\s]*EMPTY$");
            var r3 = new Regex(@"^[\t\s]*([0-9a-zA-Z\+\:\-_]+) \(([0-9]+)(\+([1-5]))?\)$");

            var line = -1;
            using var streamReader = new StreamReader(BuildString);
            while (streamReader.ReadLine() is { } lineText)
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
                    if (p.Valid)
                    {
                        AddPowerToBuildSheet(p, ref listPowers);
                    }

                    var powerIDChunks = m1.Groups[2].Value.Split(' ');
                    var rawPowerset = $"{powerIDChunks[0]}.{powerIDChunks[1]}".Trim();
                    var powerBaseName = powerIDChunks[2].Trim();
                    p.FullName = CheckForAliases($"{CheckForPowersetAliases(rawPowerset, CharacterInfo.Archetype)}.{powerBaseName}");
                    p.Powerset = DatabaseAPI.GetPowersetByName(rawPowerset);
                    p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    if (p.pData == null)
                    {
                        p.FullName = FixPowersetsNames(p.FullName);
                        powerIDChunks = p.FullName.Split('.');
                        rawPowerset = $"{powerIDChunks[0]}.{powerIDChunks[1]}".Trim();
                        p.Powerset = DatabaseAPI.GetPowersetByName(rawPowerset);
                        p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    }

                    p.Valid = CheckValid(p.pData);
                    p.Level = Convert.ToInt32(m1.Groups[1].Value, null);
                    p.Slots = new List<RawEnhData>();
                    if (p.Valid && CheckValid(p.Powerset))
                    {
                        PowerSets.Add(p.Powerset.FullName);
                    }
                }
                else if (m2.Success)
                {
                    // Empty slot
                    var e = new RawEnhData
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
                    var e = new RawEnhData
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
            if (p.Valid)
            {
                AddPowerToBuildSheet(p, ref listPowers);
            }

            return listPowers;
        }

        /***********************
        ** Import from forum post (text, long version)
        ** Example build: https://forums.homecomingservers.com/topic/2915-enen-the-murder-bunny/?do=findComment&comment=465906
        */

        public List<PowerEntry>? ParseForumPost()
        {
            var oldEnhDict = new Dictionary<string, string>
            {
                {"Numina's Convalesence: Regen/Recovery Proc", "Numina's Convalesence: +Regeneration/+Recovery"}
            };

            var p = new RawPowerData { Valid = false };
            var listPowers = new List<PowerEntry>();
            PowerSets = new UniqueList<string>();

            var r1 = new Regex(@"^Level ([0-9]+)\: ([A-Za-z\:\-\' ]+)"); // Powers
            var r2 = new Regex(@"\((A|[0-9]+)\) ([A-Za-z0-9\%\+\-\:\'\/\(\)\, ]+)"); // Slots/enhancements
            var rCharInfo = new Regex(@"^(.+)\: Level ([0-9]+) ([A-Za-z]+) ([A-Za-z ]+)"); // Character info
            var rpMain = new Regex(@"^(Primary|Secondary) Power Set\: ([A-Za-z\:\-\' ]+)"); // Main powersets
            var rpPool = new Regex(@"^Power Pool\: ([A-Za-z\:\-\' ]+)"); // Pool powersets
            var rpEpic = new Regex(@"(Ancillary|Epic) Pool\: ([A-Za-z\:\-\' ]+)"); // Epic powersets

            var lines = BuildString.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n");
            var headerFound = false;
            foreach (var lineText in lines)
            {
                if (rCharInfo.IsMatch(lineText))
                {
                    var m = rCharInfo.Match(lineText);
                    CharacterInfo.Name = m.Groups[1].Value;
                    CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);
                    CharacterInfo.Origin = m.Groups[3].Value;
                    CharacterInfo.Archetype = m.Groups[4].Value;

                    SetCharacterInfo();
                    headerFound = true;
                }

                if (!headerFound)
                {
                    continue;
                }

                var archetypeData = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype);

                var m1 = r1.Match(lineText);
                var m2 = r2.Match(lineText);
                var mpMain = rpMain.Match(lineText);
                var mpPool = rpPool.Match(lineText);
                var mpEpic = rpEpic.Match(lineText);

                if (mpMain.Success)
                {
                    var ps = DatabaseAPI.Database.Powersets.DefaultIfEmpty(null).FirstOrDefault(e =>
                        e != null &&
                        e.DisplayName == mpMain.Groups[2].Value & e.GetArchetypes().Contains(archetypeData.ClassName) &&
                        e.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary);
                    if (ps != null)
                    {
                        PowerSets.Add(ps.FullName);
                    }
                }

                if (mpPool.Success)
                {
                    var ps = DatabaseAPI.Database.Powersets.DefaultIfEmpty(null).FirstOrDefault(e => e != null && e.DisplayName == mpPool.Groups[1].Value && e.SetType == Enums.ePowerSetType.Pool);
                    if (ps != null)
                    {
                        PowerSets.Add(ps.FullName);
                    }
                }

                if (mpEpic.Success)
                {
                    var ps = DatabaseAPI.Database.Powersets.DefaultIfEmpty(null).FirstOrDefault(e => e != null && e.DisplayName == mpEpic.Groups[2].Value && e.SetType == Enums.ePowerSetType.Ancillary && e.GetArchetypes().Contains(archetypeData.ClassName));
                    if (ps != null)
                    {
                        PowerSets.Add(ps.FullName);
                    }
                }

                if (m1.Success)
                {
                    if (p.Valid)
                    {
                        AddPowerToBuildSheet(p, ref listPowers);
                    }

                    var powerName = m1.Groups[2].Value.Trim();
                    p.Level = Convert.ToInt32(m1.Groups[1].Value);
                    p.pData = DatabaseAPI.Database.Power
                        .DefaultIfEmpty(null)
                        .FirstOrDefault(e => e.GetPowerSet() != null &&
                                             ((e.GetPowerSet().GetArchetypes().Contains(archetypeData.ClassName) & PowerSets.Contains(e.GetPowerSet().FullName)) |
                                              e.GetPowerSet().SetType is Enums.ePowerSetType.Pool or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent) &
                                             e.DisplayName == powerName);

                    p.FullName = p.pData == null ? "" : p.pData.FullName;
                    p.Powerset = p.pData?.GetPowerSet();

                    p.Valid = CheckValid(p.pData);
                    p.Slots = new List<RawEnhData>();
                    if (p.Valid && CheckValid(p.Powerset))
                    {
                        PowerSets.Add(p.Powerset.FullName);
                    }
                }
                
                if (m2.Success)
                {
                    var rawEnhName = ReplaceFirstOccurrence(m2.Groups[2].Value.Trim(), " - ", ": ");
                    if (oldEnhDict.ContainsKey(rawEnhName))
                    {
                        rawEnhName = oldEnhDict[rawEnhName];
                    }

                    var enh = DatabaseAPI.Database.Enhancements
                        .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                        .FirstOrDefault(e => e.LongName == rawEnhName);

                    if (enh == null || enh.StaticIndex < 0)
                    {
                        var re = new Regex(@"^([A-Za-z ]+) (IO|SO|DO|TO)");
                        if (!re.IsMatch(rawEnhName))
                        {
                            enh = null;
                        }
                        else
                        {
                            var mEnh = re.Match(rawEnhName);
                            enh = DatabaseAPI.Database.Enhancements
                                .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                .FirstOrDefault(e => e.Name == mEnh.Groups[1].Value.Trim() &&
                                                     (mEnh.Groups[2].Value == "IO"
                                                         ? e.TypeID == Enums.eType.InventO
                                                         : e.TypeID is Enums.eType.Normal or Enums.eType.SpecialO));
                        }
                    }

                    if (enh == null || enh.StaticIndex < 0)
                    {
                        // Not found set enhancements where boosts order differs from current DB
                        var chunks = Regex.Split(rawEnhName, @"\: |\/");
                        enh = DatabaseAPI.Database.Enhancements
                            .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                            .FirstOrDefault(e => chunks.All(e.LongName.Contains));
                    }

                    var e = enh == null || enh.StaticIndex < 0
                        ? new RawEnhData
                        {
                            InternalName = "Empty",
                            Level = 0,
                            Boosters = 0,
                            HasCatalyst = false,
                            eData = -1
                        }
                        : new RawEnhData
                        {
                            InternalName = enh.UID,
                            Level = m2.Groups[1].Value == "A" ? 0 : Convert.ToInt32(m2.Groups[1].Value),
                            Boosters = 0,
                            HasCatalyst = false,
                            eData = DatabaseAPI.GetEnhancementByUIDName(enh.UID)
                        };

                    p.Slots.Add(e);
                }
            }

            if (p.Valid)
            {
                AddPowerToBuildSheet(p, ref listPowers);
            }

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

        private readonly Dictionary<string, string> OldSetNames = new()
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
            if (string.IsNullOrEmpty(sn))
            {
                return sn;
            }

            foreach (var k in OldSetNames)
            {
                if (sn.IndexOf(k.Key, StringComparison.Ordinal) > -1)
                {
                    return sn.Replace(k.Key, k.Value);
                }
            }

            return sn;
        }

        public List<PowerEntry>? Parse()
        {
            var cnt = "";
            var p = new RawPowerData {Valid = false};
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
            // Old header
            var r = new Regex(@"(Hero|Villain|Rogue|Vigilante) Plan by ([a-zA-Z\:\'\s]+) ([0-9\.]+)");
            var m = r.Match(cnt);

            if (!m.Success)
            {
                // New header (v3.0+)
                var rNew = new Regex(@"This (Hero|Villain|Rogue|Vigilante) build was built using ([a-zA-Z\:\'\s]+) ([0-9\.]+)");
                m = rNew.Match(cnt);

                if (!m.Success)
                {
                    MessageBox.Show(
                        "This build cannot be recovered because it doesn't contain a valid plain text part.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    return null;
                }
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
                var rs = new Regex(@"Level ([0-9]{1,2}) ([a-zA-Z]+) ([a-zA-Z ]+)");
                var ms = rs.Match(cnt);
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

            var archetype = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype);
            if (archetype == null)
            {
                MessageBox.Show(
                    $"Invalid Archetype \"{CharacterInfo.Archetype}\"", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
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

            var powersetsFullNamesList = new UniqueList<string>();
            powersetsFullNamesList.AddRange(PowerSets);

            PadPowerPools(ref powersetsFullNamesList);
            FilterVEATPools(ref powersetsFullNamesList);
            FixUndetectedPowersets(ref powersetsFullNamesList);
            FinalizePowersetsList(ref powersetsFullNamesList);

            // Powers
            r = new Regex(@"Level ([0-9]{1,2})\:\t([^\t]+)\t([^\r\n\t]+)");
            var rMatches = r.Matches(cnt);

            foreach (Match mt in rMatches) // var mt is of type object?
            {
                p = new RawPowerData();
                p.Slots = new List<RawEnhData>();

                p.DisplayName = mt.Groups[2].Value.Trim();
                p.Level = Convert.ToInt32(mt.Groups[1].Value, null);
                p.pData = DatabaseAPI.GetPowerByDisplayName(p.DisplayName, archetype.Idx, powersetsFullNamesList);
                p.Powerset = p.pData?.GetPowerSet();
                p.FullName = p.pData == null ? string.Empty : p.pData.FullName;
                p.Valid = CheckValid(p.pData);

                var pSlotsStr = mt.Groups.Count > 3 ? mt.Groups[3].Value.Trim() : string.Empty;
                if (!string.IsNullOrEmpty(pSlotsStr))
                {
                    // Extract enhancement name and slot level ('A' for power inherent slot)
                    // Handle special enhancements with parenthesis like ExpRnf-+Res(Pets)(50)
                    pSlotsStr = Regex.Replace(pSlotsStr, @"\(([^A0-9]+)\)", "[$1]");
                    var pSlots = Regex.Split(pSlotsStr, @",\s");

                    foreach (var ps in pSlots)
                    {
                        var s = Regex.Split(ps, @"[\(\)]");
                        s = Array.FindAll(s, enh => !string.IsNullOrWhiteSpace(enh));

                        var sContentEnh = s[0] == "Empty"
                            ? s[0]
                            : ShortNamesConversion(s[0]);
                        var enhDataIdx = DatabaseAPI.GetEnhancementByShortName(sContentEnh);

                        try
                        {
                            e = new RawEnhData
                            {
                                InternalName = sContentEnh,
                                Level = s[1] == "A"
                                    ? 0
                                    : Convert.ToInt32(s[1], null), // Slot level ("A" is the auto-granted one)
                                Boosters = 0, // Not handled
                                HasCatalyst = false,
                                eData = enhDataIdx
                            };

                            p.Slots.Add(e);
                        }
                        catch (FormatException) // if (isNaN(s[1]))
                        {
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
                    }
                }

                if (!p.Valid) continue;
                
                if (CheckValid(p.Powerset))
                {
                    PowerSets.Add(p.Powerset.FullName);
                }

                AddPowerToBuildSheet(p, ref listPowers);
            }

            return listPowers;
        }
    }
}
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
    #region Common helpers
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
            "Flight.Fly_Boost", // Afterburner
            "Inherent.Inherent.Lightning_Aura", // Storm Blast early iteration inherents
            "Inherent.Inherent.Wind_Speed",
            "Inherent.Inherent.Category_Five_Lightning"
        };

        protected Dictionary<int, int> OldFitnessPoolIDs { get; } = new()
        {
            [2553] = 1521,
            [2554] = 1523,
            [2555] = 1522,
            [2556] = 1524
        };

        // Applies to HC db only.
        protected Dictionary<KeyValuePair<string, string?>, string> OldPowersDict = new()
        {
            {new KeyValuePair<string, string?>("Invisibility", null), "Infiltration"},
            {new KeyValuePair<string, string?>("Psionic Dart", null), "Psionic Darts"},
            {new KeyValuePair<string, string?>("Whirling Axe", null), "Axe Cyclone"},
            {new KeyValuePair<string, string?>("Category 5", null), "Category Five"},
            {new KeyValuePair<string, string?>("Will Domination", "Blaster"), "Dominate Will"},
            {new KeyValuePair<string, string?>("Will Domination", "Corruptor"), "Dominate Will"},
            {new KeyValuePair<string, string?>("Will Domination", "Defender"), "Dominate Will"},
            {new KeyValuePair<string, string?>("Scramble Thoughts", "Blaster"), "Scramble Minds"},
            {new KeyValuePair<string, string?>("Afterburner", null), "Evasive Maneuvers"},
            {new KeyValuePair<string, string?>("Quantum Acceleration", "Peacebringer"), "Quantum Maneuvers"},
        };

        protected Dictionary<string, string> OldEnhDict = new()
        {
            {"Numina's Convalesence: Regen/Recovery Proc", "Numina's Convalesence: +Regeneration/+Recovery"}
        };

        protected string ApplyPowerReplacementTable(string powerName, string? archetype, Dictionary<KeyValuePair<string, string?>, string> oldPowersDict)
        {
            if (DatabaseAPI.DatabaseName is not "Homecoming" and not "Cryptic")
            {
                return powerName;
            }

            var k = new KeyValuePair<string, string?>(powerName, archetype);
            if (oldPowersDict.TryGetValue(k, value: out var newName))
            {
                return newName;
            }

            k = new KeyValuePair<string, string?>(powerName, null);
            return oldPowersDict.TryGetValue(k, value: out newName)
                ? newName
                : powerName;
        }

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
                            // Current enhancement level: p.Slots[i].Level
                            // Set to maximum since attuned ones will give the lowest level possible.
                            i9Slot.IOLevel = DatabaseAPI.Database.Enhancements[i9Slot.Enh].LevelMax;
                            i9Slot.RelativeLevel = (Enums.eEnhRelative) (p.Slots[i].Boosters + 4);
                            break;
                    }

                    powerEntry.Slots[i].Enhancement = i9Slot;
                }

                if (powerEntry.Slots.Length > 0)
                {
                    powerEntry.Slots[0].Level = powerEntry.Level;
                }
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
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
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
        // Bug: Rebirth DB will add Disintegrating everywhere. Excluding this one as well.
        protected bool CheckValid(IPower? input)
        {
            return (DatabaseAPI.DatabaseName != "Rebirth" || input?.DisplayName != "Disintegrating") && input != null && !ExcludePowers.Any(p => input.FullName.Contains(p));
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

            listPowersets.FromList(listPowersets.OrderBy(e => e.StartsWith("Epic.")).ToList());
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
            listPowersets.FromList(listPowersets.Select(CheckOldPowersets).ToList());

            for (var i = 0; i < listPowersets.Count; i++)
            {
                if (i == 2) continue;

                if (listPowersets[i] != null && listPowersets[i] != "")
                {
                    continue;
                }

                var setType = i switch
                {
                    0 => Enums.ePowerSetType.Primary,
                    1 => Enums.ePowerSetType.Secondary,
                    7 => Enums.ePowerSetType.Ancillary,
                    _ => Enums.ePowerSetType.Pool
                };

                var ps1 = DatabaseAPI.Database.Powersets
                    .DefaultIfEmpty(new Powerset {FullName = ""})
                    .FirstOrDefault(ps => ps?.ATClass == MidsContext.Character.Archetype.DisplayName & ps?.SetType == setType);

                if (ps1 == null || ps1.FullName == "")
                {
                    continue;
                }
                    
                listPowersets[i] = ps1.FullName;
            }
        }

        private static string CheckOldPowersets(string ps)
        {
            return ps switch
            {
                "Aqua Blast" => "Water Blast",
                _ => ps
            };
        }

        public static void FinalizePowersetsList(ref UniqueList<string> listPowersets)
        {
            listPowersets.FromList(listPowersets.GetRange(0, Math.Min(7, listPowersets.Count)));
            listPowersets.FromList(listPowersets.Select(e => e.Contains('.')
                ? e
                : DatabaseAPI.GetPowersetByName(CheckOldPowersets(e), MidsContext.Character.Archetype.DisplayName, true)?.FullName ?? ""
            ).ToList());
        }

        public static void FinalizePowersetsList(ref UniqueList<string> listPowersets, List<PowerEntry> listPowers, IReadOnlyList<string> trunkPowersets)
        {
            listPowersets.FromList(
                listPowers
                    .Where(e => e.Power != null)
                    .Select(e => e.Power?.GetPowerSet()?.FullName ?? "")
                    .Where(e => !string.IsNullOrWhiteSpace(e) && e != "Inherent.Inherent" && e != "Inherent.Fitness" && !trunkPowersets.Contains(e))
                    .Distinct()
                    .ToList()!);
        }

        public static void FilterTempPowersets(ref UniqueList<string> listPowersets)
        {
            listPowersets.FromList(listPowersets
                .Where(e => !e.StartsWith("Incarnate") & !e.StartsWith("Temporary_Powers"))
                .ToList());
        }

        public static void SortPowersets(ref UniqueList<string> listPowersets)
        {
            listPowersets.FromList(listPowersets
                .Select(e => new KeyValuePair<string, int>(e, DatabaseAPI.GetPowersetByFullname(e)?.SetType switch
                {
                    Enums.ePowerSetType.Primary => 0,
                    Enums.ePowerSetType.Secondary => 1,
                    Enums.ePowerSetType.Pool => 2,
                    Enums.ePowerSetType.Ancillary => 3,
                    _ => 4
                }))
                .OrderBy(e => e.Value)
                .Select(e => e.Key)
                .ToList());
        }
    }
    #endregion

    #region Import from /buildsave .txt builds
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
                "blaster_support.tactical_arrow.quickness" => "Blaster_Support.Tactical_Arrow.Gymnastics",
                "blaster_support.tactical_arrow.gymnastics" => "Blaster_Support.Tactical_Arrow.Oil_Slick_Arrow",
                "blaster_support.electricity_manipulation.lightning_clap" => "Blaster_Support.Electricity_Manipulation.Lightning_Field",
                "blaster_support.electricity_manipulation.lightning_field" => "Blaster_Support.Electricity_Manipulation.Lightning_Clap",
                "teamwork.widow_teamwork.pain_tolerance" => "Teamwork.Widow_Teamwork.NW_Pain_Tolerance",
                "teamwork.fortunata_teamwork.fate_sealed" => "Teamwork.Fortunata_Teamwork.FRT_Fate_Sealed",
                "pool.speed.speedphase" => "Inherent.Inherent.Speed_Phase",
                "defender_buff.shock_therapy.discharge" => "Defender_Buff.Shock_Therapy.Galvanic_Sentinel",
                "controller_buff.shock_therapy.discharge" => "Controller_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "corruptor_buff.shock_therapy.discharge" => "Corruptor_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "mastermind_buff.shock_therapy.discharge" => "Mastermind_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "defender_buff.shock_therapy.defibrillate" => "Defender_Buff.Shock_Therapy.Defibrilate",
                "controller_buff.shock_therapy.defibrillate" => "Controller_Buff.Electrical_Affinity.Defibrilate",
                "corruptor_buff.shock_therapy.defibrillate" => "Corruptor_Buff.Electrical_Affinity.Defibrilate",
                "mastermind_buff.shock_therapy.defibrillate" => "Mastermind_Buff.Electrical_Affinity.Defibrilate",
                "defender_buff.electrical_affinity.discharge" => "Defender_Buff.Shock_Therapy.Galvanic_Sentinel",
                "controller_buff.electrical_affinity.discharge" => "Controller_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "corruptor_buff.electrical_affinity.discharge" => "Corruptor_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "mastermind_buff.electrical_affinity.discharge" => "Mastermind_Buff.Electrical_Affinity.Galvanic_Sentinel",
                "defender_buff.electrical_affinity.defibrillate" => "Defender_Buff.Shock_Therapy.Defibrilate",
                "controller_buff.electrical_affinity.defibrillate" => "Controller_Buff.Electrical_Affinity.Defibrilate",
                "corruptor_buff.electrical_affinity.defibrillate" => "Corruptor_Buff.Electrical_Affinity.Defibrilate",
                "mastermind_buff.electrical_affinity.defibrillate" => "Mastermind_Buff.Electrical_Affinity.Defibrilate",
                "controller_buff.marine_affinity.call_depths" => "Controller_Buff.Marine_Affinity.Power_of_the_Depths",
                "corruptor_buff.marine_affinity.call_depths" => "Corruptor_Buff.Marine_Affinity.Power_of_the_Depths",
                "defender_buff.marine_affinity.call_depths" => "Defender_Buff.Marine_Affinity.Power_of_the_Depths",
                "mastermind_buff.marine_affinity.call_depths" => "Mastermind_Buff.Marine_Affinity.Power_of_the_Depths",

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
                "blaster_support.time_manipulation" when archetype == "blaster" => "Blaster_Support.Temporal_Manipulation",
                "epic.tank_dark_mastery" when archetype is "tanker" or "brute" => "Epic.Dark_Mastery_TankBrute",
                "epic.blaster_dark_mastery" when archetype == "blaster" => "Epic.Dark_Mastery_Blaster",
                "epic.controller_dark_mastery" when archetype == "controller" => "Epic.Dark_Mastery_Controller",
                "epic.dominator_dark_mastery" when archetype == "dominator" => "Epic.Dark_Mastery_Dominator",
                "epic.mastermind_dark_mastery" when archetype == "mastermind" => "Epic.Dark_Mastery_Mastermind",
                "epic.defender_ice_mastery" when archetype is "defender" or "corruptor" => "Epic.Ice_Mastery_DefCorr",
                "epic.scrapper_ice_mastery" when archetype is "scrapper" or "stalker" => "Epic.Ice_Mastery_ScrapStalk",
                "epic.tank_psionic_mastery" when archetype is "tanker" or "brute" => "Epic.Psionic_Mastery_TankBrute",
                "epic.melee_psionic_mastery" when archetype is "scrapper" or "stalker" => "Epic.Psionic_Mastery_ScrapStalk",
                "controller_buff.shock_therapy" when archetype is "controller" => "Controller_Buff.Electrical_Affinity",
                "corruptor_buff.shock_therapy" when archetype is "corruptor" => "Corruptor_Buff.Electrical_Affinity",
                "mastermind_buff.shock_therapy" when archetype is "mastermind" => "Mastermind_Buff.Electrical_Affinity",

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
                    var powersetName = CheckForPowersetAliases(rawPowerset, CharacterInfo.Archetype);
                    p.FullName = CheckForAliases($"{powersetName}.{powerBaseName}");
                    p.Powerset = DatabaseAPI.GetPowersetByName(powersetName);
                    p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    if (p.pData == null)
                    {
                        p.FullName = FixPowersetsNames(p.FullName);
                        powerIDChunks = p.FullName.Split('.');
                        rawPowerset = $"{powerIDChunks[0]}.{powerIDChunks[1]}".Trim();
                        powersetName = CheckForPowersetAliases(rawPowerset, CharacterInfo.Archetype);
                        p.Powerset = DatabaseAPI.GetPowersetByName(powersetName);
                        p.pData = DatabaseAPI.GetPowerByFullName(p.FullName);
                    }

                    p.Valid = CheckValid(p.pData);
                    Debug.WriteLine($"Detected power: {p.FullName} [{p.pData?.DisplayName ?? "<null>"}], powerset: {p.Powerset?.FullName ?? "<null>"} [{p.Powerset?.DisplayName ?? "<null>"}], valid: {p.Valid}");
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
            // Applies to HC db only.

            var listPowers = new List<PowerEntry>();
            PowerSets = new UniqueList<string>();

            var rps = new Regex(@"^Level ([0-9]+)\:[\s]*([A-Za-z0-9\+\-][A-Za-z0-9\+\-\:\-\,\'\%\/ ]+[A-Za-z0-9\+\-\:\-\,\'\%\/])[\s]*(.*)"); // Powers/Slots/enhancements
            var rCharInfo = new Regex(@"^(.+)\: Level ([0-9]+) ([A-Za-z]+) ([A-Za-z ]+)"); // Character info (with name)
            var rCharInfo2 = new Regex(@"^Level ([0-9]+) ([A-Za-z]+) ([A-Za-z ]+)"); // Character info (nameless)
            var rpMain = new Regex(@"^(Primary|Secondary) Power Set\: ([A-Za-z\:\-\' ]+)"); // Main powersets
            var rpPool = new Regex(@"^Power Pool\: ([A-Za-z\:\-\' ]+)"); // Pool powersets
            var rpEpic = new Regex(@"(Ancillary|Epic) Pool\: ([A-Za-z\:\-\' ]+)"); // Epic powersets

            var lines = BuildString.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n");
            var headerFound = false;
            Archetype? archetypeData = null;
            var k = -1;
            foreach (var lineText in lines)
            {
                k++;
                var l = lineText.Trim();
                l = Regex.Replace(l, @"[\s\t]{2,}", "\t");

                if (rCharInfo.IsMatch(l))
                {
                    var m = rCharInfo.Match(l);
                    CharacterInfo.Name = m.Groups[1].Value;
                    CharacterInfo.Level = Convert.ToInt32(m.Groups[2].Value, null);
                    CharacterInfo.Origin = m.Groups[3].Value;
                    CharacterInfo.Archetype = m.Groups[4].Value;

                    SetCharacterInfo();
                    headerFound = true;
                    archetypeData = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype);
                }
                else if (rCharInfo2.IsMatch(l))
                {
                    var m = rCharInfo2.Match(l);
                    CharacterInfo.Name = "";
                    CharacterInfo.Level = Convert.ToInt32(m.Groups[1].Value, null);
                    CharacterInfo.Origin = m.Groups[2].Value;
                    CharacterInfo.Archetype = m.Groups[3].Value;

                    SetCharacterInfo();
                    headerFound = true;
                    archetypeData = DatabaseAPI.GetArchetypeByName(CharacterInfo.Archetype);
                }

                if (!headerFound)
                {
                    continue;
                }

                var mps = rps.Match(l);
                var mpMain = rpMain.Match(l);
                var mpPool = rpPool.Match(l);
                var mpEpic = rpEpic.Match(l);

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

                if (!mps.Success)
                {
                    continue;
                }

                var powerName = ApplyPowerReplacementTable(mps.Groups[2].Value, archetypeData?.DisplayName ?? null, OldPowersDict);
                var p = new RawPowerData
                {
                    Valid = false,
                    Level = Convert.ToInt32(mps.Groups[1].Value),
                    pData = DatabaseAPI.Database.Power
                        .DefaultIfEmpty(null)
                        .FirstOrDefault(e => e.GetPowerSet() != null &&
                                             ((e.GetPowerSet().GetArchetypes().Contains(archetypeData.ClassName) &
                                               PowerSets.Contains(e.GetPowerSet().FullName)) |
                                              e.GetPowerSet().SetType is Enums.ePowerSetType.Pool
                                                  or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent) &
                                             e.DisplayName == powerName)
                };

                p.FullName = p.pData == null ? "" : p.pData.FullName;
                p.Powerset = p.pData?.GetPowerSet();
                p.Valid = CheckValid(p.pData);
                p.Slots = new List<RawEnhData>();
                var rs = new Regex(@"^(.+)\((A|[0-9]+)\)");
                var rsFormat = 0;
                var enhSlots = Regex.Split(mps.Groups[3].Value, @",[\s\t]*")
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .ToList();
                if (enhSlots.Count <= 0)
                {
                    // Read ahead for slots
                    rs = new Regex(@"^.*\((A|[0-9]+)\)(.+)");
                    rsFormat = 1;
                    var foundFirstSlot = false;
                    for (var i = k + 1; i < Math.Min(lines.Length, k + 16); i++) // (offset=1 + max 6 slots + interline=1) x 2 (if double interlined)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                        {
                            continue;
                        }

                        if (rs.IsMatch(lines[i]))
                        {
                            enhSlots.Add(lines[i]);
                            foundFirstSlot = true;
                        }
                        else if (foundFirstSlot)
                        {
                            break;
                        }
                    }
                }

                switch (rsFormat)
                {
                    case 0:
                        foreach (var slot in enhSlots)
                        {
                            if (!rs.IsMatch(slot))
                            {
                                continue;
                            }

                            var ms = rs.Match(slot);
                            var rawEnhName = ReplaceFirstOccurrence(ms.Groups[1].Value.Trim(), "-", ": ");
                            if (OldEnhDict.ContainsKey(rawEnhName))
                            {
                                rawEnhName = OldEnhDict[rawEnhName];
                            }

                            // Direct test
                            var enh = DatabaseAPI.Database.Enhancements
                                .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                .FirstOrDefault(e => e.ShortName == rawEnhName);

                            // Regular IO/SO
                            if (enh == null || enh.StaticIndex < 0)
                            {
                                var re = new Regex(@"^([A-Za-z]+)\: (I|S|D|T)");
                                if (!re.IsMatch(rawEnhName))
                                {
                                    enh = null;
                                }
                                else
                                {
                                    var mEnh = re.Match(rawEnhName);
                                    enh = DatabaseAPI.Database.Enhancements
                                        .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                        .FirstOrDefault(e => e.ShortName == mEnh.Groups[1].Value.Trim() &&
                                                             (mEnh.Groups[2].Value == "I"
                                                                 ? e.TypeID == Enums.eType.InventO
                                                                 : e.TypeID is Enums.eType.Normal or Enums.eType.SpecialO));
                                }
                            }

                            if (enh == null || enh.StaticIndex < 0)
                            {
                                if (Regex.IsMatch(rawEnhName, @"^(HO|TI|TN|HY)\:"))
                                {
                                    //Database.SpecialEnhStringLong[0] = "None";
                                    //Database.SpecialEnhStringLong[1] = "Hamidon Origin";
                                    //Database.SpecialEnhStringLong[2] = "Hydra Origin";
                                    //Database.SpecialEnhStringLong[3] = "Titan Origin";
                                    //Database.SpecialEnhStringLong[4] = "D-Sync Origin";
                                
                                    var chunks = rawEnhName.Split(':');
                                    var subTypeId = chunks[0] switch
                                    {
                                        "HO" => 1,
                                        "TI" or "TN" => 2,
                                        "HY" => 3,
                                        _ => 0
                                    };
                                
                                    enh = DatabaseAPI.Database.Enhancements
                                        .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                        .FirstOrDefault(e =>
                                            e.ShortName == chunks[1] && e.TypeID == Enums.eType.SpecialO &&
                                            e.SubTypeID == subTypeId);

                                }
                            }
                        
                            // IO sets
                            if (enh == null || enh.StaticIndex < 0)
                            {
                                var chunks = rawEnhName.Split(": ");
                                var subChunks = Regex.Split(rawEnhName, @"\: |\/");
                            
                                // Enhancement set short names can be non-unique.
                                // E.g. Annihilation / Annoyance (Ann)
                                var setsEnh = DatabaseAPI.Database.EnhancementSets
                                    .Where(e => e.ShortName == chunks[0])
                                    .SelectMany(e => e.Enhancements)
                                    .Select(e => DatabaseAPI.Database.Enhancements[e])
                                    .ToList();

                                if (setsEnh.Count > 0)
                                {
                                    enh = setsEnh
                                        .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                        .FirstOrDefault(e => e.ShortName == chunks[1] || subChunks.Skip(1).All(s => e.ShortName.Contains(s)));
                                }
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
                                    Level = ms.Groups[2].Value == "A" ? 0 : Convert.ToInt32(ms.Groups[2].Value),
                                    Boosters = 0,
                                    HasCatalyst = false,
                                    eData = DatabaseAPI.GetEnhancementByUIDName(enh.UID)
                                };

                            p.Slots.Add(e);
                        }

                        break;

                    case 1:
                        foreach (var slot in enhSlots)
                        {
                            // [1] -> Level, [2] -> Enhancement
                            var ms = rs.Match(slot);
                            var rawEnhName = ReplaceFirstOccurrence(ms.Groups[2].Value, " - ", ": ").Trim();

                            if (rawEnhName == "Empty")
                            {
                                p.Slots.Add(new RawEnhData
                                {
                                    InternalName = "Empty",
                                    Level = 0,
                                    Boosters = 0,
                                    HasCatalyst = false,
                                    eData = -1
                                });

                                continue;
                            }

                            // Direct test, for Sets IO
                            var enh = DatabaseAPI.Database.Enhancements
                                .DefaultIfEmpty(new Enhancement { StaticIndex = -1 })
                                .FirstOrDefault(e => e.LongName == rawEnhName);

                            if (enh is {StaticIndex: >= 0})
                            {
                                p.Slots.Add(new RawEnhData
                                {
                                    InternalName = enh.UID,
                                    Level = ms.Groups[1].Value == "A" ? 0 : Convert.ToInt32(ms.Groups[1].Value),
                                    Boosters = 0,
                                    HasCatalyst = false,
                                    eData = DatabaseAPI.GetEnhancementByUIDName(enh.UID)
                                });

                                continue;
                            }

                            // Regular IO/SO
                            var enhName = rawEnhName.Replace(" IO", "");
                            var enhClass = rawEnhName.EndsWith(" IO") ? "IO" : "SO";
                            enh = enhClass == "IO"
                                ? DatabaseAPI.Database.Enhancements
                                    .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                    .FirstOrDefault(e =>
                                        e.LongName == $"Invention: {enhName}" &&
                                        e.TypeID == Enums.eType.InventO)

                                : DatabaseAPI.Database.Enhancements
                                    .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                    .FirstOrDefault(e =>
                                        e.LongName.EndsWith(enhName) &&
                                        e.UID.StartsWith("Magic_") &&
                                        e.TypeID == Enums.eType.Normal);

                            if (enh is { StaticIndex: >= 0 })
                            {
                                p.Slots.Add(new RawEnhData
                                {
                                    InternalName = enh.UID,
                                    Level = ms.Groups[1].Value == "A" ? 0 : Convert.ToInt32(ms.Groups[1].Value),
                                    Boosters = 0,
                                    HasCatalyst = false,
                                    eData = DatabaseAPI.GetEnhancementByUIDName(enh.UID)
                                });

                                continue;
                            }

                            // Hamidon/Titan/Hydra/D-Sync
                            var re = new Regex(@"^(Hamidon|Titan|Hydra|D\-Sync) Origin\:\s*(.+)$");
                            if (!re.IsMatch(rawEnhName))
                            {
                                enh = null;
                            }
                            else
                            {
                                var mEnh = re.Match(rawEnhName);
                                enhName = mEnh.Groups[2].Value.Trim();
                                enh = DatabaseAPI.Database.Enhancements
                                    .DefaultIfEmpty(new Enhancement {StaticIndex = -1})
                                    .FirstOrDefault(e => e.LongName.EndsWith(enhName) &&
                                                         e.TypeID == Enums.eType.SpecialO);
                            }

                            if (enh is { StaticIndex: >= 0 })
                            {
                                p.Slots.Add(new RawEnhData
                                {
                                    InternalName = enh.UID,
                                    Level = ms.Groups[1].Value == "A" ? 0 : Convert.ToInt32(ms.Groups[1].Value),
                                    Boosters = 0,
                                    HasCatalyst = false,
                                    eData = DatabaseAPI.GetEnhancementByUIDName(enh.UID)
                                });

                                continue;
                            }

                            p.Slots.Add(new RawEnhData
                            {
                                InternalName = "Empty",
                                Level = 0,
                                Boosters = 0,
                                HasCatalyst = false,
                                eData = -1
                            });
                        }

                        break;
                }

                if (p.Valid)
                {
                    AddPowerToBuildSheet(p, ref listPowers);
                }
            }

            return listPowers;
        }
    }
    #endregion

    #region Plain-text import from .mxd files (Build recovery mode)
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
            ["Apoc"] = "Apc",
            ["BasGaze"] = "BslGaz",
            ["CSndmn"] = "CaloftheS",
            ["CtlSpd"] = "CrtSpd",
            ["C'ngBlow"] = "ClvBlo",
            ["C'ngImp"] = "CrsImp",
            ["Dct'dW"] = "DctWnd",
            ["Decim"] = "Dcm",
            ["Det'tn"] = "Dtn",
            ["Dev'n"] = "Dvs",
            ["Efficacy"] = "EffAdp",
            ["Enf'dOp"] = "EnfOpr",
            ["Erad"] = "Erd",
            ["ExRmnt"] = "ExpRnf",
            ["ExStrk"] = "ExpStr",
            ["ExtrmM"] = "ExtMsr",
            ["FotG"] = "FuroftheG",
            ["FrcFbk"] = "FrcFdb",
            ["F'dSmite"] = "FcsSmt",
            ["GA"] = "GldArm",
            ["GSFC"] = "GssSynFr-",
            ["Hectmb"] = "Hct",
            ["HO:Micro"] = "Micro",
            ["H'zdH"] = "HrmHln",
            ["ImpSkn"] = "ImpSki",
            ["Insult"] = "TrmIns",
            ["JavVoll"] = "JvlVll",
            ["KinCrsh"] = "KntCrs",
            ["KntkC'bat"] = "KntCmb",
            ["Krma"] = "Krm",
            ["Ksmt"] = "Ksm",
            ["LkGmblr-Rchg+"] = "LucoftheG-Def/Rchg+",
            ["LucoftheG-Rchg+"] = "LucoftheG-Def/Rchg+",
            ["LkGmblr"] = "LucoftheG",
            ["LgcRps"] = "LthRps",
            ["Mantic"] = "StnoftheM",
            ["Mako"] = "Mk'Bit",
            ["Mlais"] = "MlsIll",
            ["Mocking"] = "MckBrt",
            ["MotCorruptor"] = "MlcoftheC",
            ["Mrcl"] = "Mrc",
            ["M'Strk"] = "Mlt",
            ["Numna"] = "NmnCnv",
            ["Oblit"] = "Obl",
            ["OvForce"] = "OvrFrc",
            ["Panac"] = "Pnc",
            ["Posi"] = "PstBls",
            ["Prv-Heal/EndMod"] = "Prv-Heal/EndRdx",
            ["P'ngS'Fest"] = "PndSlg",
            ["P'Shift"] = "PrfShf",
            ["Rec'dRet"] = "RctRtc",
            ["RctvArm"] = "RctArm",
            ["RzDz"] = "RzzDzz",
            ["ShldBrk"] = "ShlBrk",
            ["SMotCorruptor"] = "SprMlcoft",
            ["SMlcoftheC"] = "SprMlcoft",
            ["SprAvl-Rchg/Knockdown%"] = "SprAvl-Rchg/KDProc",
            ["SprBlsCol-Acc/Dmg/EndRdx/Rchg"] = "SprBlsCol-Dmg/EndRdx/Acc/Rchg",
            ["SprBlsCol-Rchg/Hold%"] = "SprBlsCol-Rchg/HoldProc",
            ["SprWntBit-Acc/Dmg/EndRdx/Rchg"] = "SprWntBit-Dmg/EndRdx/Acc/Rchg",
            ["SprWntBit-Rchg/-Spd/-Rch"] = "SprWntBit-Rchg/SlowProc",
            ["Srng"] = "Srn",
            ["SStalkersG"] = "SprStlGl",
            ["SWotController"] = "SprWiloft",
            ["S'fstPrt"] = "StdPrt",
            ["T'Death"] = "TchofDth",
            ["T'pst"] = "Tmp",
            ["TmpRdns"] = "TmpRdn",
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
                if (sn.StartsWith(k.Key))
                {
                    return sn.Replace(k.Key, k.Value);
                }
            }

            // Parse D-Sync, Hamidon, Hydra, Titan enhancements
            return Regex.Replace(sn, @"^(DSyncO|HO|HY|TN)\:", "");
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
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
                
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

            var powersetsWithTrunks = new UniqueList<string>();
            powersetsWithTrunks.AddRange(powersetsFullNamesList);
            var trunkPowersets = powersetsFullNamesList
                .Select(e => DatabaseAPI.GetPowersetByFullname(e) ?? null)
                .Where(e => e is {SetType: Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary, nIDTrunkSet: > -1})
                .Select(e => DatabaseAPI.Database.Powersets[e.nIDTrunkSet].FullName)
                .ToList();

            powersetsWithTrunks.AddRange(trunkPowersets);

            // Powers
            r = new Regex(@"Level ([0-9]{1,2})\:\t([^\t]+)(\t([^\r\n\t]+))?");
            var rMatches = r.Matches(cnt);

            foreach (Match mt in rMatches)
            {
                p = new RawPowerData
                {
                    Slots = new List<RawEnhData>(),
                    DisplayName = ApplyPowerReplacementTable(mt.Groups[2].Value.Trim(), CharacterInfo.Archetype, OldPowersDict),
                    Level = Convert.ToInt32(mt.Groups[1].Value, null)
                };

                p.pData = DatabaseAPI.GetPowerByDisplayName(p.DisplayName, archetype.Idx, powersetsWithTrunks);
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
                
                if (CheckValid(p.Powerset) && !trunkPowersets.Contains(p.Powerset?.FullName ?? "--"))
                {
                    PowerSets.Add(p.Powerset.FullName);
                }

                AddPowerToBuildSheet(p, ref listPowers);
            }

            return listPowers;
        }
    }
    #endregion
}
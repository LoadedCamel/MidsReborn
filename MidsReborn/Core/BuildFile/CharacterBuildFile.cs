using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile.DataModels;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Mids_Reborn.Core.BuildFile
{
    public class CharacterBuildFile
    {

        private static CharacterBuildFile? _instance;
        private static readonly object Mutex = new();

        private static int DisplayIndex { get; set; } = -1;
        private static List<PowerEntry> InherentPowers { get; set; } = new();

        private static CharacterBuildFile GetInstance(Character characterData, bool refreshData = false)
        {
            if (_instance != null && !refreshData) return _instance;
            lock (Mutex)
            {
                _instance = new CharacterBuildFile(characterData);
            }
            return _instance;
        }

        private static CharacterBuildFile CreateInstance()
        {
            return _instance = new CharacterBuildFile();
        }

        public MetaData? BuiltWith { get; set; }
        public string Class { get; set; }
        public string Origin { get; set; }
        public string Alignment { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public List<string> PowerSets { get; set; }
        public int LastPower { get; set; }
        public List<PowerData?> PowerEntries { get; set; }

        private static IEnumerable<PowerEntry> SortGridPowers(List<PowerEntry> powerList, Enums.eGridType iType)
        {
            var tList = powerList.FindAll(x => x.Power != null && x.Power.InherentType == iType);
            var tempList = new PowerEntry[tList.Count];
            for (var eIndex = 0; eIndex < tList.Count; eIndex++)
            {
                var power = tList[eIndex];
                if (power.Power != null)
                    switch (power.Power.InherentType)
                    {
                        case Enums.eGridType.Class:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Inherent:
                            switch (power.Power.PowerName)
                            {
                                case "Brawl":
                                    tempList[0] = power;
                                    break;
                                case "Sprint":
                                    tempList[1] = power;
                                    break;
                                case "Rest":
                                    tempList[2] = power;
                                    break;
                                case "Swift":
                                    tempList[3] = power;
                                    break;
                                case "Hurdle":
                                    tempList[4] = power;
                                    break;
                                case "Health":
                                    tempList[5] = power;
                                    break;
                                case "Stamina":
                                    tempList[6] = power;
                                    break;
                            }

                            break;
                        case Enums.eGridType.Powerset:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Power:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Prestige:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Incarnate:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Accolade:
                            power.Level = 49;
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Pet:
                            tempList[eIndex] = power;
                            break;
                        case Enums.eGridType.Temp:
                            tempList[eIndex] = power;
                            break;
                    }
            }

            var outList = tempList.ToList();
            return outList;
        }

        public CharacterBuildFile(string @class, string origin, string alignment, string name, string? comment, List<string> powerSets, List<PowerData?> powerEntries)
        {
            Class = @class;
            Origin = origin;
            Alignment = alignment;
            Name = name;
            Comment = comment;
            PowerSets = powerSets;
            PowerEntries = powerEntries;
        }

        private CharacterBuildFile()
        {
            Class = string.Empty;
            Origin = string.Empty;
            Alignment = string.Empty;
            Name = string.Empty;
            Comment = string.Empty;
            PowerSets = new List<string>();
            PowerEntries = new List<PowerData?>();
        }

        private CharacterBuildFile(Character? characterData)
        {
            if (characterData == null) throw new ArgumentNullException(nameof(characterData));
            if (characterData.Archetype == null) throw new NullReferenceException(nameof(characterData.Archetype));
            if (characterData.CurrentBuild == null) throw new ArgumentException(nameof(characterData.CurrentBuild));
            BuiltWith = new MetaData(MidsContext.AppName, MidsContext.AppFileVersion, DatabaseAPI.DatabaseName, DatabaseAPI.Database.Version);
            Class = characterData.Archetype.ClassName;
            Origin = characterData.Archetype.Origin[characterData.Origin];
            Alignment = characterData.Alignment.ToString();
            Name = characterData.Name;
            Comment = characterData.Comment;
            PowerSets = new List<string>();
            PowerEntries = new List<PowerData?>();

            foreach (var powerSet in characterData.Powersets)
            {
                PowerSets.Add(powerSet == null ? string.Empty : powerSet.FullName);
            }

            LastPower = characterData.CurrentBuild.LastPower + 1;

            for (var powerIndex = 0; powerIndex < characterData.CurrentBuild.Powers.Count; powerIndex++)
            {
                var powerEntry = characterData.CurrentBuild.Powers[powerIndex];
                if (powerEntry == null) continue;

                var powerData = new PowerData();
                if (powerEntry.NIDPower < 0)
                {
                    PowerEntries.Add(powerData);
                }
                else
                {
                    if (powerEntry.Power == null) continue;
                    powerData.PowerName = powerEntry.Power.FullName;
                    powerData.Level = powerEntry.Level + 1;
                    powerData.StatInclude = powerEntry.StatInclude;
                    powerData.ProcInclude = powerEntry.ProcInclude;
                    powerData.VariableValue = powerEntry.VariableValue;
                    powerData.InherentSlotsUsed = powerEntry.InherentSlotsUsed;

                    PowerEntries.Add(powerData);

                    foreach (var subPowerEntry in powerEntry.SubPowers)
                    {
                        var subPowerData = new SubPowerData();
                        if (subPowerEntry.nIDPower > 1)
                        {
                            var subPower = DatabaseAPI.Database.Power[subPowerEntry.nIDPower];
                            subPowerData.PowerName = subPower.FullName;
                        }

                        subPowerData.StatInclude = subPowerEntry.StatInclude;
                        PowerEntries[powerIndex]?.SubPowerEntries.Add(subPowerData);
                    }

                    foreach (var slot in powerEntry.Slots)
                    {
                        var slotData = new SlotData
                        {
                            Level = slot.Level + 1,
                            IsInherent = slot.IsInherent
                        };
                        WriteSlotData(ref slotData, slot.Enhancement);
                        WriteAltSlotData(ref slotData, slot.FlippedEnhancement);
                        PowerEntries[powerIndex]?.SlotEntries.Add(slotData);
                    }
                }
            }
        }

        private static void WriteSlotData(ref SlotData slotData, I9Slot slot)
        {
            if (slot.Enh < 0)
            {
                slotData.Enhancement = null;
            }
            else
            {
                slotData.Enhancement = new EnhancementData
                {
                    Enhancement = DatabaseAPI.Database.Enhancements[slot.Enh].LongName,
                    Obtained = slot.Obtained
                };

                if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.Normal | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SpecialO)
                {
                    slotData.Enhancement.RelativeLevel = slot.RelativeLevel.ToString();
                    slotData.Enhancement.Grade = slot.Grade.ToString();
                }
                else if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.InventO | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SetO)
                {
                    slotData.Enhancement.IoLevel = slot.IOLevel;
                    slotData.Enhancement.RelativeLevel = slot.RelativeLevel.ToString();
                }
            }
        }
        private static void WriteAltSlotData(ref SlotData slotData, I9Slot slot)
        {
            if (slot.Enh < 0)
            {
                slotData.FlippedEnhancement = null;
            }
            else
            {
                slotData.FlippedEnhancement = new EnhancementData
                {
                    Enhancement = DatabaseAPI.Database.Enhancements[slot.Enh].LongName,
                    Obtained = slot.Obtained
                };

                if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.Normal | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SpecialO)
                {
                    slotData.FlippedEnhancement.RelativeLevel = slot.RelativeLevel.ToString();
                    slotData.FlippedEnhancement.Grade = slot.Grade.ToString();
                }
                else if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.InventO | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SetO)
                {
                    slotData.FlippedEnhancement.IoLevel = slot.IOLevel;
                    slotData.FlippedEnhancement.RelativeLevel = slot.RelativeLevel.ToString();
                }
            }
        }


        private static bool LoadBuild()
        {
            if (_instance == null)
            {
                return false;
            }
            InherentPowers = new List<PowerEntry>();

            var atNiD = DatabaseAPI.NidFromUidClass(_instance.Class);
            var atOrigin = DatabaseAPI.NidFromUidOrigin(_instance.Origin, atNiD);
            MidsContext.Character.Reset(DatabaseAPI.Database.Classes[atNiD], atOrigin);
            MidsContext.Character.Alignment = Enum.Parse<Enums.Alignment>(_instance.Alignment);
            MidsContext.Character.Name = _instance.Name;
            MidsContext.Character.Comment = _instance.Comment;
            MidsContext.Character.LoadPowerSetsByName(_instance.PowerSets);
            MidsContext.Character.CurrentBuild!.LastPower = _instance.LastPower;

            try
            {
                for (var powerIndex = 0; powerIndex < _instance.PowerEntries.Count; powerIndex++)
                {
                    var powerEntryData = _instance.PowerEntries[powerIndex];

                    var powerId = -1;

                    if (!string.IsNullOrWhiteSpace(powerEntryData?.PowerName))
                    {
                        powerId = DatabaseAPI.PiDFromUidPower(powerEntryData.PowerName);
                    }

                    var flagged = false;
                    PowerEntry? powerEntry;

                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        powerEntry = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                    }
                    else
                    {
                        powerEntry = new PowerEntry();
                        flagged = true;
                    }

                    if (powerEntry == null) continue;
                    if (powerEntryData == null) continue;

                    if (powerId > -1)
                    {
                        powerEntry.Level = powerEntryData.Level - 1;
                        powerEntry.StatInclude = powerEntryData.StatInclude;
                        powerEntry.ProcInclude = powerEntryData.ProcInclude;
                        powerEntry.VariableValue = powerEntryData.VariableValue;
                        powerEntry.InherentSlotsUsed = powerEntryData.InherentSlotsUsed;

                        if (powerEntryData.SubPowerEntries.Any())
                        {
                            powerEntry.SubPowers = new PowerSubEntry[powerEntryData.SubPowerEntries.Count + 1];
                            for (var subPowerIndex = 0; subPowerIndex < powerEntryData.SubPowerEntries.Count; subPowerIndex++)
                            {
                                var subEntry = powerEntryData.SubPowerEntries[subPowerIndex];
                                var powerSubEntry = new PowerSubEntry();
                                powerEntry.SubPowers[subPowerIndex] = powerSubEntry;

                                powerSubEntry.nIDPower = DatabaseAPI.PiDFromUidPower(subEntry.PowerName);
                                var subPower = DatabaseAPI.Database.Power[powerSubEntry.nIDPower];

                                if (powerSubEntry.nIDPower > -1)
                                {
                                    powerSubEntry.Powerset = subPower.PowerSetID;
                                    powerSubEntry.Power = subPower.PowerSetIndex;
                                }

                                powerSubEntry.StatInclude = subEntry.StatInclude;
                                if (!(powerSubEntry.nIDPower > -1 & powerSubEntry.StatInclude)) continue;

                                var powerEntry2 = new PowerEntry(subPower)
                                {
                                    StatInclude = true
                                };

                                MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                            }
                        }
                    }

                    if (powerId < 0 && powerIndex < DatabaseAPI.Database.Levels_MainPowers.Length)
                    {
                        powerEntry.Level = DatabaseAPI.Database.Levels_MainPowers[powerIndex];
                    }

                    powerEntry.Slots = new SlotEntry[powerEntryData.SlotEntries.Count];
                    for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                    {
                        var slotEntry = powerEntryData.SlotEntries[slotIndex];
                        var i9Enhancement = new I9Slot();
                        var enhData = slotEntry.Enhancement;
                        if (enhData != null)
                        {
                            i9Enhancement.IOLevel = enhData.IoLevel;
                            i9Enhancement.Obtained = enhData.Obtained;
                            if (!string.IsNullOrWhiteSpace(enhData.Enhancement))
                            {
                                i9Enhancement.Enh = DatabaseAPI.GetEnhancementByName(enhData.Enhancement);
                            }

                            if (!string.IsNullOrWhiteSpace(enhData.Grade))
                            {
                                i9Enhancement.Grade = Enum.Parse<Enums.eEnhGrade>(enhData.Grade);
                            }

                            if (!string.IsNullOrWhiteSpace(enhData.RelativeLevel))
                            {
                                i9Enhancement.RelativeLevel = Enum.Parse<Enums.eEnhRelative>(enhData.RelativeLevel);
                            }
                        }

                        var i9Flipped = new I9Slot();
                        var flippedEnhData = slotEntry.FlippedEnhancement;
                        if (flippedEnhData != null)
                        {
                            i9Flipped.IOLevel = flippedEnhData.IoLevel;
                            i9Flipped.Obtained = flippedEnhData.Obtained;
                            if (!string.IsNullOrWhiteSpace(flippedEnhData.Enhancement))
                            {
                                i9Flipped.Enh = DatabaseAPI.GetEnhancementByName(flippedEnhData.Enhancement);
                            }

                            if (!string.IsNullOrWhiteSpace(flippedEnhData.Grade))
                            {
                                i9Flipped.Grade = Enum.Parse<Enums.eEnhGrade>(flippedEnhData.Grade);
                            }

                            if (!string.IsNullOrWhiteSpace(flippedEnhData.RelativeLevel))
                            {
                                i9Flipped.RelativeLevel =
                                    Enum.Parse<Enums.eEnhRelative>(flippedEnhData.RelativeLevel);
                            }
                        }

                        powerEntry.Slots[slotIndex] = new SlotEntry
                        {
                            Level = slotEntry.Level - 1,
                            IsInherent = slotEntry.IsInherent,
                            Enhancement = i9Enhancement,
                            FlippedEnhancement = i9Flipped
                        };
                    }

                    if (powerEntry.SubPowers.Length > 0)
                    {
                        powerId = -1;
                    }

                    if (powerId <= -1) continue;

                    powerEntry.NIDPower = powerId;
                    var power = DatabaseAPI.Database.Power[powerId];

                    powerEntry.NIDPowerset = power.PowerSetID;
                    powerEntry.IDXPower = power.PowerSetIndex;

                    var powerSet = powerEntry.Power?.GetPowerSet();
                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        var cPower = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                        if (cPower == null) continue;
                        if (powerEntry.Power != null && !(!cPower.Chosen & (powerSet is { nArchetype: > -1 } || powerEntry.Power.GroupName == "Pool")))
                        {
                            flagged = !cPower.Chosen;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (flagged)
                    {
                        if (powerEntry.Power != null && powerEntry.Power.InherentType != Enums.eGridType.None)
                        {
                            InherentPowers.Add(powerEntry);
                        }
                    }
                    else if (powerEntry.Power != null && (powerSet is { nArchetype: > -1 } || powerEntry.Power.GroupName == "Pool"))
                    {
                        MidsContext.Character.CurrentBuild.Powers[powerIndex] = powerEntry;
                    }
                }

                var newPowerList = new List<PowerEntry>();
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Class));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Inherent));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Powerset));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Power));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Prestige));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Incarnate));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Accolade));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Pet));
                newPowerList.AddRange(SortGridPowers(InherentPowers, Enums.eGridType.Temp));
                foreach (var entry in newPowerList)
                {
                    MidsContext.Character.CurrentBuild.Powers.Add(entry);
                }
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBoxEx($"An error occurred while attempting to read the build data.\r\n{ex.Message}\r\n{ex.StackTrace}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return false;
            }
            MidsContext.Archetype = MidsContext.Character.Archetype;
            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            return true;
        }

        public static bool Generate(string fileName)
        {
            if (MidsContext.Character.CurrentBuild == null) return false;
            var powerEntries = MidsContext.Character.CurrentBuild.Powers.GetRange(0, 24);
            if (powerEntries.All(x => x?.Power == null))
            {
                var warnMsg = new MessageBoxEx(@"Error", @"Unable to save build, you have not selected any powers.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                warnMsg.ShowDialog(Application.OpenForms["frmMain"]);
                return false;
            }

            var instance = GetInstance(MidsContext.Character, true);
            File.WriteAllText(fileName, JsonConvert.SerializeObject(instance, Formatting.Indented));
            return true;
        }

        public static bool Load(string fileName)
        {
            var returnedVal = false;
            _instance = CreateInstance();
            _instance = JsonConvert.DeserializeObject<CharacterBuildFile>(File.ReadAllText(fileName));
            if (_instance == null) throw new NullReferenceException(nameof(_instance));
            var metaData = _instance.BuiltWith;
            if (metaData == null) throw new NullReferenceException(nameof(metaData));


            // Compare App Version if Enabled
            if (MidsContext.Config.WarnOnOldAppMbd)
            {
                var outDatedApp = Helpers.CompareVersions(MidsContext.AppFileVersion, metaData.Version);
                var newerApp = Helpers.CompareVersions(metaData.Version, MidsContext.AppFileVersion);

                if (outDatedApp)
                {
                    var appVerMsg = new MessageBoxEx(@"Version Warning", $"This build was created with an older version of {MidsContext.AppName}, some features may not be available.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Warning, true);
                    appVerMsg.ShowDialog(Application.OpenForms["frmMain"]);
                }

                if (newerApp)
                {
                    var appVerMsg = new MessageBoxEx(@"Version Warning", $"This build was created with an newer version of {MidsContext.AppName}.\r\nIt is recommended that you update the application.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Warning, true);
                    appVerMsg.ShowDialog(Application.OpenForms["frmMain"]);
                }
            }


            var fileInfo = new FileInfo(fileName);
            if (DatabaseAPI.DatabaseName == metaData.Database)
            {
                // Compare Database Version if Enabled
                if (MidsContext.Config.WarnOnOldDbMbd)
                {
                    
                    var outDatedDb = Helpers.CompareVersions(DatabaseAPI.Database.Version, metaData.DatabaseVersion);
                    var newerDb = Helpers.CompareVersions(metaData.DatabaseVersion, DatabaseAPI.Database.Version);

                    if (outDatedDb)
                    {
                        var dbVerMsg = new MessageBoxEx(fileInfo.Name, $"This build was created in an older version of the {metaData.Database} database.\r\nSome powers may have changed, you may need to rebuild some of it.\r\n\r\nDo you wish to load it anyway?", MessageBoxEx.MessageBoxButtons.YesNo, MessageBoxEx.MessageBoxIcon.Warning, true);
                        dbVerMsg.ShowDialog(Application.OpenForms["frmMain"]);
                        if (dbVerMsg.DialogResult == DialogResult.Yes)
                        {
                            returnedVal = LoadBuild();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (newerDb)
                    {
                        var dbVerMsg = new MessageBoxEx(fileInfo.Name, $"This build was created in an newer version of the {metaData.Database} database.\r\nIt is recommended that you update the database.\r\n\r\nDo you wish to load it anyway?", MessageBoxEx.MessageBoxButtons.YesNo, MessageBoxEx.MessageBoxIcon.Warning, true);
                        dbVerMsg.ShowDialog(Application.OpenForms["frmMain"]);
                        if (dbVerMsg.DialogResult == DialogResult.Yes)
                        {
                            returnedVal = LoadBuild();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (!outDatedDb && !newerDb) returnedVal = LoadBuild();
                }
                else
                {
                    returnedVal = LoadBuild();
                }
            }
            else
            {
                var dbSwitchResult = new MessageBoxEx(fileInfo.Name, $"This build was created using the {metaData.Database} database.\r\nDo you want to reload and switch to this database, then attempt to load the build?", MessageBoxEx.MessageBoxButtons.YesNo, MessageBoxEx.MessageBoxIcon.Warning, true);
                dbSwitchResult.ShowDialog(Application.OpenForms["frmMain"]);
                switch (dbSwitchResult.DialogResult)
                {
                    case DialogResult.Yes:
                        var databases = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder)).ToList();
                        var selected = databases.FirstOrDefault(d => d.Contains(metaData.Database));
                        if (selected == null)
                        {
                            var messageBox = new MessageBoxEx($"Sorry, but it appears you do not have the {metaData.Database} installed.\r\nPlease install the database and try again.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                            messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                            return false;
                        }

                        MidsContext.Config.LastFileName = fileName;
                        MidsContext.Config.DataPath = selected;
                        MidsContext.Config.SavePath = selected;
                        MidsContext.Config.SaveConfig(Serializer.GetSerializer());
                        Application.Restart();
                        break;
                    case DialogResult.No:
                        return false;
                }
            }
            return returnedVal;
        }

        private static byte[] CreateDataStream()
        {
            _instance ??= new CharacterBuildFile(MidsContext.Character);
            using var stream = new MemoryStream();
            using var writer = new BsonDataWriter(stream);
            var serializer = new JsonSerializer();
            serializer.Serialize(writer, _instance);
            return stream.ToArray();
        }

        public static string GenerateShareData()
        {
            var buffer = CreateDataStream();
            var compressed = Compression.Compress(buffer);
            var hex = Convert.ToHexString(compressed);
            var shared = $"HEX;{hex};MBD";
            return shared;
        }

        private static bool ReadShareData(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            if (!data.StartsWith("HEX;") && !data.EndsWith(";MBD")) return false;
            data = data.Replace("HEX;", string.Empty).Replace(";MBD", string.Empty);
            var compressed = Convert.FromHexString(data);
            var decompressed = Compression.Decompress(compressed);
            using var stream = new MemoryStream(decompressed);
            try
            {
                using var reader = new BsonDataReader(stream);
                var serializer = new JsonSerializer();
                _instance ??= new CharacterBuildFile();
                _instance = serializer.Deserialize<CharacterBuildFile>(reader);
            }
            catch (Exception ex)
            {
                var errorMsg = new MessageBoxEx($"{ex.Message}\r\n{ex.StackTrace}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error, true);
                errorMsg.ShowDialog(Application.OpenForms["frmMain"]);
                return false;
            }
            return true;
        }

        public static bool LoadImportData(string data)
        {
            var dataRead = ReadShareData(data);
            return dataRead && LoadBuild();
        }

    }
}

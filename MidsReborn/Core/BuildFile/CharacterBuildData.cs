using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile.DataModels;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile
{
    public class CharacterBuildData
    {
        private readonly IBuildNotifier _notifier = new BuildNotifier();
        private static CharacterBuildData? _instance;
        private static readonly object InstanceLock = new();

        private static CharacterBuildData? GetInstance()
        {
            if (_instance is not null) return _instance;
            lock (InstanceLock)
            {
                _instance = new CharacterBuildData();
            }
            return _instance;
        }

        public static CharacterBuildData? Instance => GetInstance();

        [JsonProperty]
        public MetaData? BuiltWith { get; set; }

        [JsonProperty]
        public string Level { get; set; } = string.Empty;

        [Required]
        [JsonProperty]
        public string Class { get; set; } = string.Empty;

        [Required]
        [JsonProperty]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [JsonProperty]
        public string Alignment { get; set; } = string.Empty;

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public string? Comment { get; set; }

        [Required]
        [JsonProperty]
        public List<string> PowerSets { get; set; } = new();

        [Required]
        [JsonProperty]
        public int LastPower { get; set; }

        [Required]
        [JsonProperty]
        public List<PowerData?> PowerEntries { get; set; } = new();

        private static List<PowerEntry> InherentPowers { get; set; } = new();

        internal static void UpdateData(Character? characterData)
        {
            lock (InstanceLock)
            {
                if (_instance is null) throw new ArgumentNullException(nameof(_instance));
                if (characterData == null) throw new ArgumentNullException(nameof(characterData));
                if (characterData.Archetype == null) throw new NullReferenceException(nameof(characterData.Archetype));
                if (characterData.CurrentBuild == null) throw new ArgumentException(nameof(characterData.CurrentBuild));
                _instance.BuiltWith = new MetaData(MidsContext.AppName, MidsContext.AppFileVersion,
                    DatabaseAPI.DatabaseName, DatabaseAPI.Database.Version);
                _instance.Class = characterData.Archetype.ClassName;
                _instance.Origin = characterData.Archetype.Origin[characterData.Origin];
                _instance.Alignment = characterData.Alignment.ToString();
                _instance.Name = characterData.Name;
                _instance.Level = characterData.Level.ToString();
                _instance.Comment = characterData.Comment;
                _instance.PowerSets = new List<string>();
                _instance.PowerEntries = new List<PowerData?>();

                foreach (var powerSet in characterData.Powersets)
                {
                    _instance.PowerSets.Add(powerSet == null ? string.Empty : powerSet.FullName);
                }

                _instance.LastPower = characterData.CurrentBuild.LastPower + 1;

                foreach (var powerEntry in characterData.CurrentBuild.Powers)
                {
                    if (powerEntry == null) continue;

                    var powerData = new PowerData();
                    if (powerEntry.NIDPower < 0)
                    {
                        _instance.PowerEntries.Add(powerData);
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

                        _instance.PowerEntries.Add(powerData);

                        foreach (var subPowerEntry in powerEntry.SubPowers)
                        {
                            var subPowerData = new SubPowerData();
                            if (subPowerEntry.nIDPower > 1)
                            {
                                var subPower = DatabaseAPI.Database.Power[subPowerEntry.nIDPower];
                                subPowerData.PowerName = subPower.FullName;
                            }

                            subPowerData.StatInclude = subPowerEntry.StatInclude;
                            _instance.PowerEntries[^1]?.SubPowerEntries.Add(subPowerData);
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
                            _instance.PowerEntries[^1]?.SlotEntries.Add(slotData);
                        }
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
                    Uid = DatabaseAPI.Database.Enhancements[slot.Enh].UID,
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
                    Uid = DatabaseAPI.Database.Enhancements[slot.Enh].UID,
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

        internal bool LoadBuild()
        {
            InherentPowers = new List<PowerEntry>();

            var atNiD = DatabaseAPI.NidFromUidClass(Class);
            var atOrigin = DatabaseAPI.NidFromUidOrigin(Origin, atNiD);
            MidsContext.Character.Reset(DatabaseAPI.Database.Classes[atNiD], atOrigin);
            MidsContext.Character.Alignment = Enum.Parse<Enums.Alignment>(Alignment);
            MidsContext.Character.Name = Name;
            MidsContext.Character.Comment = Comment;
            MidsContext.Character.LoadPowerSetsByName(PowerSets);
            MidsContext.Character.CurrentBuild!.LastPower = LastPower;

            try
            {
                for (var powerIndex = 0; powerIndex < PowerEntries.Count; powerIndex++)
                {
                    var powerEntryData = PowerEntries[powerIndex];

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
                                var enh = DatabaseAPI.GetEnhancementByName(enhData.Enhancement);
                                if (enh <= -1)
                                {
                                    enh = DatabaseAPI.GetEnhancementByUIDName(enhData.Uid);
                                }
                                i9Enhancement.Enh = enh;
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
                                var enh = DatabaseAPI.GetEnhancementByName(flippedEnhData.Enhancement);
                                if (enh <= -1)
                                {
                                    enh = DatabaseAPI.GetEnhancementByUIDName(flippedEnhData.Uid);
                                }
                                i9Flipped.Enh = enh;
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
                _notifier.ShowError($"An error occurred while attempting to read the build data.\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
            MidsContext.Archetype = MidsContext.Character.Archetype;
            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            return true;
        }
        
        /*public string GenerateShareData()
        {
            UpdateData(MidsContext.Character);
            var serialized = JsonConvert.SerializeObject(this, Formatting.None);
            var iBytes = Encoding.UTF8.GetBytes(serialized);
            var output = Compression.CompressToBase64(iBytes);
            
            return output.OutString;
        }

        public static string GenerateShareDataFromStream(bool inclFlipped)
        {
            using var stream = GenerateBinaryShareStream(inclFlipped);
            var compressionResult = Compression.ZCompress(stream.ToArray());
            //var formattedString = Compression.BreakBase64String(compressionResult.OutString, 67, true);
            return $"|MBD;{compressionResult.UncompressedSize};{compressionResult.CompressedSize};{compressionResult.EncodedSize};HEX;|{string.Empty}\n{compressionResult.OutString}";
        }

        private static CompressionResult MxDBuild(MemoryStream stream)
        {
            var byteArray = stream.ToArray();
            var asciiEncoding = new ASCIIEncoding();
            var compressedBytes = ModernZlib.CompressChunk(byteArray);
            var hexBytes = ModernZlib.HexEncodeBytes(compressedBytes);
            var outString = ModernZlib.BreakString(asciiEncoding.GetString(hexBytes), 67, true);

            return new CompressionResult(outString, byteArray.Length, compressedBytes.Length, hexBytes.Length);
        }

        public static string MxDGenerateString(bool inclFlipped)
        {
            using var stream = GenerateBinaryShareStream(inclFlipped);
            var compressionResult = MxDBuild(stream);
            var separator = string.Empty;
            return $"|MxDz;{compressionResult.UncompressedSize};{compressionResult.CompressedSize};{compressionResult.EncodedSize};HEX;|{separator}\n{compressionResult.OutString}";
        }

        private static MemoryStream GenerateBinaryShareStream(bool inclFlipped = false)
        {
            var characterData = MidsContext.Character;
            if (characterData == null) throw new ArgumentNullException(nameof(characterData), @"Character data cannot be null");
            if (characterData.Archetype == null) throw new NullReferenceException(nameof(characterData.Archetype));
            if (characterData.CurrentBuild == null) throw new ArgumentException(nameof(characterData.CurrentBuild));
            var binStream = new MemoryStream();
            using var writer = new BinaryWriter(binStream);

            writer.Write(DatabaseAPI.DatabaseName);
            writer.Write(DatabaseAPI.Database.Version.ToString());
            writer.Write(characterData.Archetype.ClassName);
            writer.Write(characterData.Archetype.Origin[characterData.Origin]);
            writer.Write((int)characterData.Alignment);
            writer.Write(characterData.Name);
            writer.Write(characterData.Comment);
            writer.Write(characterData.Powersets.Length);
            foreach (var powerSet in characterData.Powersets)
            {
                writer.Write(powerSet != null ? powerSet.FullName : string.Empty);
            }
            writer.Write(characterData.CurrentBuild.LastPower);
            writer.Write(characterData.CurrentBuild.Powers.Count);
            foreach (var powerEntry in characterData.CurrentBuild.Powers)
            {
                if (powerEntry is null || powerEntry.NIDPower < 0)
                {
                    writer.Write(-1);
                }
                else
                {
                    writer.Write(DatabaseAPI.Database.Power[powerEntry.NIDPower]!.StaticIndex);
                    writer.Write(Convert.ToSByte(powerEntry.Level));
                    writer.Write(powerEntry.StatInclude);
                    writer.Write(powerEntry.ProcInclude);
                    writer.Write(powerEntry.VariableValue);
                    writer.Write(powerEntry.InherentSlotsUsed);
                    writer.Write(Convert.ToSByte(powerEntry.SubPowers.Length));
                    foreach (var subPower in powerEntry.SubPowers)
                    {
                        if (subPower.nIDPower > -1)
                        {
                            writer.Write(DatabaseAPI.Database.Power[powerEntry.NIDPower]!.StaticIndex);
                        }
                        else
                        {
                            writer.Write(-1);
                        }
                        writer.Write(subPower.StatInclude);
                    }
                }
                writer.Write(Convert.ToSByte(powerEntry!.Slots.Length));
                foreach (var slot in powerEntry.Slots)
                {
                    writer.Write(Convert.ToSByte(slot.Level));
                    writer.Write(slot.IsInherent);
                    WriteBinarySlotData(writer, slot.Enhancement);
                    writer.Write(inclFlipped);
                    if (inclFlipped) WriteBinarySlotData(writer, slot.FlippedEnhancement);
                }
            }

            binStream.Position = 0;
            return binStream;
        }

        private static void WriteBinarySlotData(BinaryWriter writer, I9Slot slot)
        {
            if (slot.Enh < 0)
            {
                writer.Write(-1);
            }
            else
            {
                writer.Write(DatabaseAPI.Database.Enhancements[slot.Enh].StaticIndex);
                writer.Write(DatabaseAPI.Database.Enhancements[slot.Enh].UID);
                writer.Write(slot.Obtained);
                if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.Normal | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SpecialO)
                {
                    writer.Write(Convert.ToSByte(slot.RelativeLevel));
                    writer.Write(Convert.ToSByte(slot.Grade));
                }
                else if (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.InventO | DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SetO)
                {
                    writer.Write(Convert.ToSByte(slot.IOLevel));
                    writer.Write(Convert.ToSByte(slot.RelativeLevel));
                }
            }
        }*/

        /*private static bool ReadShareData(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            if (!Helpers.ValidShareData(data)) return false;
            var decompressed = Compression.DecompressFromBase64(data);
            var decodedJson = Encoding.UTF8.GetString(decompressed);
            try
            {
                _instance ??= new CharacterBuildData();
                _instance = JsonConvert.DeserializeObject<CharacterBuildData>(decodedJson);
            }
            catch (Exception ex)
            {
                var errorMsg = new MessageBoxEx($"{ex.Message}\r\n{ex.StackTrace}", MessageBoxEx.MessageBoxExButtons.Okay, MessageBoxEx.MessageBoxExIcon.Error, true);
                errorMsg.ShowDialog(Application.OpenForms["frmMain"]);
                
                return false;
            }
            
            return true;
        }

        public static bool LoadImportData(string data)
        {
            var dataRead = ReadShareData(data);
            return dataRead && LoadBuild();
        }*/
    }
}

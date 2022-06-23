using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbBase.Utils;
using Newtonsoft.Json;

namespace mrbBase
{
    public class CharacterDataFile
    {
        private static readonly Version MinimumExpectedVersion = new(4, 0);
        private static readonly Version CurrentVersion = new(4, 0);

        private static List<PowerEntry?> InherentPowers { get; set; } = new();

        public Version? CharacterFileVersion { get; set; }
        public CharacterData? CharacterInfo { get; set; }

        public enum LoadReturnCode
        {
            Failure,
            Success
        }

        private class FitnessEntry
        {
            public string Name { get; set; }
            public int PowerIndex { get; set; }
            public int Sid { get; set; }
            public SlotEntry[] Slots { get; set; } = Array.Empty<SlotEntry>();
        }

        public class CharacterData
        {
            public string Archetype { get; set; }
            public string Origin { get; set; }
            public string Alignment { get; set; }
            public string? Name { get; set; }
            public List<string> PowerSets { get; set; }
            public int LastPower { get; set; }
            public List<BuildPowerEntry> PowerEntries { get; set; }
        }

        public class BuildPowerEntry
        {
            public string PowerName { get; set; }
            public int Level { get; set; }
            public bool Chosen { get; set; }
            public bool IsFitnessPower { get; set; }
            public bool StatInclude { get; set; }
            public bool ProcInclude { get; set; }
            public int VariableValue { get; set; }
            public int InherentSlotsUsed { get; set; }
            public List<BuildPowerEntry> SubPowers { get; set; }
            public List<PowerEntrySlot> Slots { get; set; }
        }

        public class PowerEntrySlot
        {
            public int SlotIndex { get; set; }
            public int Level { get; set; }
            public bool IsInherent { get; set; }
            public SlotEnhancement? Enhancement { get; set; }
            public SlotEnhancement? FlippedEnhancement { get; set; }
        }

        public class SlotEnhancement
        {
            public string? EnhancementName { get; set; }
            public string? EnhancementType { get; set; }
            public SlotEnhancementExt? ExtendedEnhInfo { get; set; }
        }

        public class SlotEnhancementExt
        {
            public string? Grade { get; set; }
            public int IoLevel { get; set; }
            public string? RelativeLevel { get; set; }
        }

        /*public static string GenerateSharedData()
        {
            var charData = GenerateCharacterData();
            var serializedData = JsonConvert.SerializeObject(charData);
            var bitwiseData = Encoding.UTF8.GetBytes(serializedData);
            var uncompressedLength = bitwiseData.Length;
            var compressedBytes = Compression.Compress(bitwiseData);
            var hexEncoded = Convert.ToHexString(compressedBytes, 0, compressedBytes.Length);
            var sharedData = new CompressionData
            {
                UncompressedLength = uncompressedLength,
                CompressedLength = compressedBytes.Length,
                EncodedLength = hexEncoded.Length,
                EncodedString = hexEncoded
            };
            return sharedData.EncodedString;
        }*/

        /*public static string GenerateDataFile()
        {
            var charData = GenerateCharacterData();
            Debug.WriteLine(charData.Archetype);
            var characterDataFile = new CharacterDataFile
            {
                CharacterFileVersion = new Version(4, 0),
                CharacterInfo = charData,
                SharedData = GetCompressionData(ref charData)
            };
            
            var output = JsonConvert.SerializeObject(characterDataFile, Formatting.Indented);
            Debug.WriteLine(output);
            return output;
        }*/

        /*private static CompressionData GetCompressionData(ref CharacterData? characterData)
        {
            var serializedData = JsonConvert.SerializeObject(characterData);
            var bitwiseData = Encoding.UTF8.GetBytes(serializedData);
            var uncompressedLength = bitwiseData.Length;
            var compressedBytes = Compression.Compress(bitwiseData);
            var hexEncoded = Convert.ToHexString(compressedBytes, 0, compressedBytes.Length);
            var charSharedData = new CompressionData
            {
                UncompressedLength = uncompressedLength,
                CompressedLength = compressedBytes.Length,
                EncodedLength = hexEncoded.Length,
                EncodedString = hexEncoded
            };
            return charSharedData;
        }*/

        private static CharacterData GenerateCharacterData()
        {
            var characterInfo = new CharacterData
            {
                Archetype = MidsContext.Character.Archetype.ClassName,
                Origin = MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin],
                Alignment = MidsContext.Character.Alignment.ToString(),
                Name = MidsContext.Character.Name,
                PowerSets = MidsContext.Character.Powersets.Where(powerSet => powerSet != null).Select(powerSet => powerSet.FullName).ToList(),
                LastPower = MidsContext.Character.CurrentBuild.LastPower,
                PowerEntries = new List<BuildPowerEntry>()
            };

            foreach (var power in MidsContext.Character.CurrentBuild.Powers)
            {
                var buildPe = new BuildPowerEntry();
                if (power is not { NIDPower: >= 0 }) continue;
                var powerName = DatabaseAPI.Database.Power[power.NIDPower].FullName;
                buildPe.PowerName = powerName;
                buildPe.Level = power.Level;
                buildPe.Chosen = power.Chosen;
                buildPe.IsFitnessPower = powerName.Contains("Fitness");
                buildPe.StatInclude = power.StatInclude;
                buildPe.ProcInclude = power.ProcInclude;
                buildPe.VariableValue = power.VariableValue;
                buildPe.InherentSlotsUsed = power.InherentSlotsUsed;

                buildPe.SubPowers = new List<BuildPowerEntry>();
                foreach (var subPower in power.SubPowers)
                {
                    var subPe = new BuildPowerEntry();
                    if (subPower.nIDPower > -1)
                    {
                        subPe.PowerName = DatabaseAPI.Database.Power[subPower.nIDPower].FullName;
                    }

                    subPe.StatInclude = subPower.StatInclude;
                    buildPe.SubPowers.Add(subPe);
                }

                buildPe.Slots = new List<PowerEntrySlot>();
                for (var index = 0; index < power.Slots.Length; index++)
                {
                    var slot = power.Slots[index];
                    var entrySlot = new PowerEntrySlot
                    {
                        SlotIndex = index,
                        Level = slot.Level,
                        IsInherent = slot.IsInherent
                    };

                    if (slot.Enhancement.Enh < 0)
                    {
                        entrySlot.Enhancement = null;
                    }
                    else
                    {
                        var slotEnhancement = new SlotEnhancement
                        {
                            EnhancementName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName,
                            EnhancementType = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID.ToString()
                        };

                        var slotInfoExt = new SlotEnhancementExt();
                        switch (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID)
                        {
                            case Enums.eType.Normal or Enums.eType.SpecialO:
                                slotInfoExt = new SlotEnhancementExt
                                {
                                    Grade = slot.Enhancement.Grade.ToString(),
                                    RelativeLevel = slot.Enhancement.RelativeLevel.ToString()
                                };
                                break;
                            case Enums.eType.InventO or Enums.eType.SetO:
                                slotInfoExt = new SlotEnhancementExt
                                {
                                    IoLevel = slot.Enhancement.IOLevel,
                                    RelativeLevel = slot.Enhancement.RelativeLevel.ToString()
                                };
                                break;
                        }

                        slotEnhancement.ExtendedEnhInfo = slotInfoExt;
                        entrySlot.Enhancement = slotEnhancement;
                    }

                    if (slot.FlippedEnhancement.Enh > -1)
                    {
                        var slotEnhancement = new SlotEnhancement
                        {
                            EnhancementName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName,
                            EnhancementType = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID.ToString()
                        };

                        var slotInfoExt = new SlotEnhancementExt();
                        switch (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID)
                        {
                            case Enums.eType.Normal or Enums.eType.SpecialO:
                                slotInfoExt = new SlotEnhancementExt
                                {
                                    Grade = slot.Enhancement.Grade.ToString(),
                                    RelativeLevel = slot.Enhancement.RelativeLevel.ToString()
                                };
                                break;
                            case Enums.eType.InventO or Enums.eType.SetO:
                                slotInfoExt = new SlotEnhancementExt
                                {
                                    IoLevel = slot.Enhancement.IOLevel,
                                    RelativeLevel = slot.Enhancement.RelativeLevel.ToString()
                                };
                                break;
                        }

                        slotEnhancement.ExtendedEnhInfo = slotInfoExt;
                        entrySlot.FlippedEnhancement = slotEnhancement;
                    }
                    buildPe.Slots.Add(entrySlot);
                }
                characterInfo.PowerEntries.Add(buildPe);
            }

            return characterInfo;
        }

        public static LoadReturnCode ExtractAndLoad(Stream? stream, string? sharedCode = null)
        {
            if (stream == null) return LoadReturnCode.Failure;
            using var reader = new StreamReader(stream);
            reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            var dataString = reader.ReadToEnd();
            reader.Close();

            if (dataString.Length <= 0)
            {
                MessageBox.Show(@"The file data is empty.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }
                
            var charFileData = JsonConvert.DeserializeObject<CharacterDataFile>(dataString);
            if (charFileData == null)
            {
                MessageBox.Show(@"File payload data is invalid.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }
                
            if (charFileData.CharacterFileVersion < MinimumExpectedVersion)
            {
                MessageBox.Show(@"Character file version not supported.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }

            if (charFileData.CharacterFileVersion > CurrentVersion)
            {
                MessageBox.Show(@"Character file was saved by a newer version of the application, please obtain the latest version to open this file.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }

            if (charFileData.CharacterInfo == null)
            {
                MessageBox.Show(@"Character payload data is invalid.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }

            var charDataLoaded = LoadCharacterData(charFileData.CharacterInfo);
            if (charDataLoaded) return LoadReturnCode.Success;

            MessageBox.Show(@"An error occurred during extraction of the character data.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return LoadReturnCode.Failure;

            /*if (!string.IsNullOrWhiteSpace(sharedCode))
            {
                if (LoadCharacterData(null, sharedCode))
                {
                    return LoadReturnCode.Success;
                }
                MessageBox.Show(@"An error occurred during extraction of the shared data.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            
            }

            MessageBox.Show(@"The shared data payload is invalid.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
        }

        public static bool LoadCharacterData(string iFile)
        {
            var jsonData = File.ReadAllText(iFile);
            var characterData = JsonConvert.DeserializeObject<CharacterDataFile>(jsonData);
            var data = characterData?.CharacterInfo;
            bool loadCompleted;
            try
            {
                var archetype = DatabaseAPI.GetArchetypeByClassName(data.Archetype);
                var origin = DatabaseAPI.GetOriginByName(archetype, data.Origin);
                MidsContext.Character.Reset(archetype, origin);
                MidsContext.Character.Alignment = Enum.Parse<Enums.Alignment>(data.Alignment);
                MidsContext.Character.Name = data.Name??="";
                var powerSets = new List<IPowerset?>();
                foreach (var powerSet in data.PowerSets)
                {
                    IPowerset? setToAdd = DatabaseAPI.GetPowersetByName(powerSet);
                    powerSets.Add(setToAdd);
                }

                MidsContext.Character.Powersets = powerSets.ToArray();
                if (MidsContext.Character.CurrentBuild != null)
                {
                    MidsContext.Character.CurrentBuild.LastPower = data.LastPower;

                    foreach (var entryPower in data.PowerEntries)
                    {
                        var powerEntry = new PowerEntry();
                        var nId = DatabaseAPI.NidFromUidPower(entryPower.PowerName);
                        var power = DatabaseAPI.Database.Power[nId];
                        if (power != null)
                        {
                            powerEntry = new PowerEntry(entryPower.Level, power, entryPower.Chosen);
                        }

                        var isInherent = entryPower.Chosen;

                        if (!string.IsNullOrEmpty(entryPower.PowerName))
                        {
                            powerEntry.Level = entryPower.Level;
                            powerEntry.StatInclude = entryPower.StatInclude;
                            powerEntry.ProcInclude = entryPower.ProcInclude;
                            powerEntry.VariableValue = entryPower.VariableValue;
                            powerEntry.InherentSlotsUsed = entryPower.InherentSlotsUsed;
                            foreach (var subEntry in entryPower.SubPowers)
                            {
                                var subPower = new PowerSubEntry
                                {
                                    nIDPower = DatabaseAPI.NidFromUidPower(subEntry.PowerName)
                                };
                                if (subPower.nIDPower > -1)
                                {
                                    subPower.Powerset = DatabaseAPI.Database.Power[subPower.nIDPower]!.PowerSetID;
                                    subPower.Power = DatabaseAPI.Database.Power[subPower.nIDPower]!.PowerSetIndex;
                                }

                                subPower.StatInclude = subEntry.StatInclude;
                                if (!((subPower.nIDPower > -1) & subPower.StatInclude))
                                    continue;
                                var powerEntry2 = new PowerEntry(DatabaseAPI.Database.Power[subPower.nIDPower]!)
                                {
                                    StatInclude = true
                                };
                                MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                            }
                        }

                        var entrySlots = new List<SlotEntry>();
                        foreach (var entrySlot in entryPower.Slots)
                        {
                            var slotEntry = new SlotEntry
                            {
                                Level = entrySlot.Level,
                                IsInherent = entrySlot.IsInherent,
                                Enhancement = new I9Slot(),
                                FlippedEnhancement = new I9Slot()
                            };

                            if (entrySlot.Enhancement != null)
                            {
                                if (!string.IsNullOrEmpty(entrySlot.Enhancement.EnhancementName))
                                {
                                    slotEntry.Enhancement.Enh =
                                        DatabaseAPI.NidFromUidEnh(entrySlot.Enhancement.EnhancementName);
                                    switch (DatabaseAPI.Database.Enhancements[slotEntry.Enhancement.Enh].TypeID)
                                    {
                                        case Enums.eType.Normal or Enums.eType.SpecialO:
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.Enhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .RelativeLevel);
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { Grade: { } })
                                                slotEntry.Enhancement.Grade =
                                                    Enum.Parse<Enums.eEnhGrade>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .Grade);
                                            break;
                                        case Enums.eType.InventO or Enums.eType.SetO:
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { })
                                                slotEntry.Enhancement.IOLevel =
                                                    entrySlot.Enhancement.ExtendedEnhInfo.IoLevel;
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.Enhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .RelativeLevel);
                                            break;
                                    }
                                }
                            }

                            if (entrySlot.FlippedEnhancement != null)
                            {
                                if (!string.IsNullOrEmpty(entrySlot.FlippedEnhancement.EnhancementName))
                                {
                                    slotEntry.FlippedEnhancement.Enh =
                                        DatabaseAPI.NidFromUidEnh(entrySlot.FlippedEnhancement.EnhancementName);
                                    switch (DatabaseAPI.Database.Enhancements[slotEntry.FlippedEnhancement.Enh].TypeID)
                                    {
                                        case Enums.eType.Normal or Enums.eType.SpecialO:
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.FlippedEnhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.RelativeLevel);
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { Grade: { } })
                                                slotEntry.FlippedEnhancement.Grade =
                                                    Enum.Parse<Enums.eEnhGrade>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.Grade);
                                            break;
                                        case Enums.eType.InventO or Enums.eType.SetO:
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { })
                                                slotEntry.FlippedEnhancement.IOLevel = entrySlot.FlippedEnhancement
                                                    .ExtendedEnhInfo.IoLevel;
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.FlippedEnhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.RelativeLevel);
                                            break;
                                    }
                                }
                            }

                            entrySlots.Add(slotEntry);
                        }

                        powerEntry.Slots = entrySlots.ToArray();
                        if (!DatabaseAPI.Database.Power[nId]!.FullName.Contains("Fitness"))
                        {
                            powerEntry.NIDPower = nId;
                            powerEntry.NIDPowerset = DatabaseAPI.Database.Power[nId]!.PowerSetID;
                            powerEntry.IDXPower = DatabaseAPI.Database.Power[nId]!.PowerSetIndex;
                        }

                        if (isInherent)
                        {
                            if (powerEntry.Power.InherentType != Enums.eGridType.None)
                            {
                                InherentPowers.Add(powerEntry);
                            }
                        }
                        else
                        {
                            MidsContext.Character.CurrentBuild.Powers.Add(powerEntry);
                        }

                        foreach (var inherent in InherentPowers)
                        {
                            MidsContext.Character.CurrentBuild.Powers.Add(inherent);
                        }
                    }
                }

                MidsContext.Archetype = MidsContext.Character.Archetype;
                MidsContext.Character.Validate();
                MidsContext.Character.Lock();
                loadCompleted = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
                loadCompleted = false;
            }

            return loadCompleted;
        }

        private static bool LoadCharacterData(CharacterData data)
        {
            bool loadCompleted;
            try
            {
                var archetype = DatabaseAPI.GetArchetypeByClassName(data.Archetype);
                var origin = DatabaseAPI.GetOriginByName(archetype, data.Origin);
                MidsContext.Character.Reset(archetype, origin);
                MidsContext.Character.Alignment = Enum.Parse<Enums.Alignment>(data.Alignment);
                MidsContext.Character.Name = data.Name??="";
                var powerSets = new List<IPowerset>();
                foreach (var powerSet in data.PowerSets)
                {
                    if (string.IsNullOrWhiteSpace(powerSet)) continue;
                    var setToAdd = DatabaseAPI.GetPowersetByName(powerSet);
                    if (setToAdd != null)
                    {
                        powerSets.Add(setToAdd);
                    }
                }

                MidsContext.Character.Powersets = powerSets.ToArray();
                if (MidsContext.Character.CurrentBuild != null)
                {
                    MidsContext.Character.CurrentBuild.LastPower = data.LastPower;

                    for (var powerIndex = 0; powerIndex < data.PowerEntries.Count; powerIndex++)
                    {
                        var powerEntry = new PowerEntry();
                        var entry = data.PowerEntries[powerIndex];
                        var nId = DatabaseAPI.NidFromUidPower(entry.PowerName);
                        var power = DatabaseAPI.Database.Power[nId];
                        if (power != null)
                        {
                            powerEntry = new PowerEntry(entry.Level, power, entry.Chosen);
                        }

                        var isInherent = entry.Chosen;

                        if (!string.IsNullOrEmpty(entry.PowerName))
                        {
                            powerEntry.Level = entry.Level;
                            powerEntry.StatInclude = entry.StatInclude;
                            powerEntry.ProcInclude = entry.ProcInclude;
                            powerEntry.VariableValue = entry.VariableValue;
                            powerEntry.InherentSlotsUsed = entry.InherentSlotsUsed;
                            foreach (var subEntry in entry.SubPowers)
                            {
                                var subPower = new PowerSubEntry
                                {
                                    nIDPower = DatabaseAPI.NidFromUidPower(subEntry.PowerName)
                                };
                                if (subPower.nIDPower > -1)
                                {
                                    subPower.Powerset = DatabaseAPI.Database.Power[subPower.nIDPower]!.PowerSetID;
                                    subPower.Power = DatabaseAPI.Database.Power[subPower.nIDPower]!.PowerSetIndex;
                                }

                                subPower.StatInclude = subEntry.StatInclude;
                                if (!((subPower.nIDPower > -1) & subPower.StatInclude))
                                    continue;
                                var powerEntry2 = new PowerEntry(DatabaseAPI.Database.Power[subPower.nIDPower]!)
                                {
                                    StatInclude = true
                                };
                                MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                            }
                        }

                        if (nId < 0 && powerIndex < DatabaseAPI.Database.Levels_MainPowers.Length)
                        {
                            powerEntry.Level = DatabaseAPI.Database.Levels_MainPowers[powerIndex];
                        }

                        var entrySlots = new List<SlotEntry>();
                        foreach (var entrySlot in entry.Slots)
                        {
                            var slotEntry = new SlotEntry
                            {
                                Level = entrySlot.Level,
                                IsInherent = entrySlot.IsInherent,
                                Enhancement = new I9Slot(),
                                FlippedEnhancement = new I9Slot()
                            };

                            if (entrySlot.Enhancement != null)
                            {
                                if (!string.IsNullOrEmpty(entrySlot.Enhancement.EnhancementName))
                                {
                                    slotEntry.Enhancement.Enh =
                                        DatabaseAPI.NidFromUidEnh(entrySlot.Enhancement.EnhancementName);
                                    switch (DatabaseAPI.Database.Enhancements[slotEntry.Enhancement.Enh].TypeID)
                                    {
                                        case Enums.eType.Normal or Enums.eType.SpecialO:
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.Enhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .RelativeLevel);
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { Grade: { } })
                                                slotEntry.Enhancement.Grade =
                                                    Enum.Parse<Enums.eEnhGrade>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .Grade);
                                            break;
                                        case Enums.eType.InventO or Enums.eType.SetO:
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { })
                                                slotEntry.Enhancement.IOLevel =
                                                    entrySlot.Enhancement.ExtendedEnhInfo.IoLevel;
                                            if (entrySlot.Enhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.Enhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.Enhancement.ExtendedEnhInfo
                                                        .RelativeLevel);
                                            break;
                                    }
                                }
                            }

                            if (entrySlot.FlippedEnhancement != null)
                            {
                                if (!string.IsNullOrEmpty(entrySlot.FlippedEnhancement.EnhancementName))
                                {
                                    slotEntry.FlippedEnhancement.Enh =
                                        DatabaseAPI.NidFromUidEnh(entrySlot.FlippedEnhancement.EnhancementName);
                                    switch (DatabaseAPI.Database.Enhancements[slotEntry.FlippedEnhancement.Enh].TypeID)
                                    {
                                        case Enums.eType.Normal or Enums.eType.SpecialO:
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.FlippedEnhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.RelativeLevel);
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { Grade: { } })
                                                slotEntry.FlippedEnhancement.Grade =
                                                    Enum.Parse<Enums.eEnhGrade>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.Grade);
                                            break;
                                        case Enums.eType.InventO or Enums.eType.SetO:
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { })
                                                slotEntry.FlippedEnhancement.IOLevel = entrySlot.FlippedEnhancement
                                                    .ExtendedEnhInfo.IoLevel;
                                            if (entrySlot.FlippedEnhancement.ExtendedEnhInfo is { RelativeLevel: { } })
                                                slotEntry.FlippedEnhancement.RelativeLevel =
                                                    Enum.Parse<Enums.eEnhRelative>(entrySlot.FlippedEnhancement
                                                        .ExtendedEnhInfo.RelativeLevel);
                                            break;
                                    }
                                }
                            }

                            entrySlots.Add(slotEntry);
                        }

                        powerEntry.Slots = entrySlots.ToArray();
                        if (!DatabaseAPI.Database.Power[nId]!.FullName.Contains("Fitness"))
                        {
                            powerEntry.NIDPower = nId;
                            powerEntry.NIDPowerset = DatabaseAPI.Database.Power[nId]!.PowerSetID;
                            powerEntry.IDXPower = DatabaseAPI.Database.Power[nId]!.PowerSetIndex;
                        }

                        if (isInherent)
                        {
                            if (powerEntry.Power.InherentType != Enums.eGridType.None)
                            {
                                InherentPowers.Add(powerEntry);
                            }
                        }
                        else
                        {
                            MidsContext.Character.CurrentBuild.Powers.Add(powerEntry);
                        }

                        foreach (var inherent in InherentPowers)
                        {
                            MidsContext.Character.CurrentBuild.Powers.Add(inherent);
                        }
                    }
                }

                MidsContext.Archetype = MidsContext.Character.Archetype;
                MidsContext.Character.Validate();
                MidsContext.Character.Lock();
                loadCompleted = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
                loadCompleted = false;
            }

            return loadCompleted;
        }

        private static void BuildSlotData(ref I9Slot slot, ref SlotEnhancement entrySlot)
        {
            if (string.IsNullOrWhiteSpace(entrySlot.EnhancementName)) return;
            slot.Enh = DatabaseAPI.NidFromUidEnh(entrySlot.EnhancementName);
            switch (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID)
            {
                case Enums.eType.Normal or Enums.eType.SpecialO:
                    if (entrySlot.ExtendedEnhInfo is { RelativeLevel: { } }) slot.RelativeLevel = Enum.Parse<Enums.eEnhRelative>(entrySlot.ExtendedEnhInfo.RelativeLevel);
                    if (entrySlot.ExtendedEnhInfo is { Grade: { } }) slot.Grade = Enum.Parse<Enums.eEnhGrade>(entrySlot.ExtendedEnhInfo.Grade);
                    break;
                case Enums.eType.InventO or Enums.eType.SetO:
                    if (entrySlot.ExtendedEnhInfo is { }) slot.IOLevel = entrySlot.ExtendedEnhInfo.IoLevel;
                    if (entrySlot.ExtendedEnhInfo is { RelativeLevel: { } }) slot.RelativeLevel = Enum.Parse<Enums.eEnhRelative>(entrySlot.ExtendedEnhInfo.RelativeLevel);
                    break;
            }
        }
    }
}

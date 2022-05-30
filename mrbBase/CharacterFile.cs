using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mrbBase.Base.Master_Classes;
using mrbBase.Utils;

namespace mrbBase
{
    public static class CharacterFile
    {
        private static List<PowerEntry> InherentPowers { get; set; } = new();
        private const string MagicCompressed = "MxDz";
        private const string MagicUncompressed = "MxDu";
        private const float ThisVersion = 3.20f;
        private const float PriorVersion = 3.10f;
        private static readonly byte[] MagicNumber =
        {
            Convert.ToByte('M'),
            Convert.ToByte('x'),
            Convert.ToByte('D'),
            Convert.ToByte(12)
        };
        private enum FileFormat
        {
            Current,
            Prior,
            Legacy
        }

        public enum LoadReturnCode
        {
            Failure,
            Success
        }

        private struct CompressionData
        {
            public int SzUncompressed;
            public int SzCompressed;
            public int SzEncoded;
        }

        private static IEnumerable<PowerEntry> SortGridPowers(List<PowerEntry> powerList, Enums.eGridType iType)
        {
            var tList = powerList.FindAll(x => x.Power.InherentType == iType);
            var tempList = new PowerEntry[tList.Count];
            for (var eIndex = 0; eIndex < tList.Count; eIndex++)
            {
                var power = tList[eIndex];
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

        private static bool BuildSaveBuffer(ref byte[] buffer, bool incAltEnh)
        {
            MemoryStream memoryStream;
            BinaryWriter writer;
            try
            {
                memoryStream = new MemoryStream();
                writer = new BinaryWriter(memoryStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Save Failed!" + ex.Message);
                return false;
            }

            writer.Write(MagicNumber, 0, MagicNumber.Length);
            writer.Write(ThisVersion);
            writer.Write(MidsContext.Character.Archetype.ClassName);
            writer.Write(MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin]);
            writer.Write((int)MidsContext.Character.Alignment);
            writer.Write(MidsContext.Character.Name);
            writer.Write(MidsContext.Character.Powersets.Length - 1);
            foreach (var powerSet in MidsContext.Character.Powersets)
            {
                writer.Write(powerSet != null ? powerSet.FullName : string.Empty);
            }
            writer.Write(MidsContext.Character.CurrentBuild.LastPower + 1);
            writer.Write(MidsContext.Character.CurrentBuild.Powers.Count - 1);
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry.NIDPower < 0)
                {
                    writer.Write(string.Empty);
                }
                else
                {
                    writer.Write(DatabaseAPI.Database.Power[powerEntry.NIDPower].FullName);
                    writer.Write(powerEntry.Level);
                    writer.Write(powerEntry.StatInclude);
                    writer.Write(powerEntry.ProcInclude);
                    writer.Write(powerEntry.VariableValue);
                    writer.Write(powerEntry.InherentSlotsUsed);
                    writer.Write(powerEntry.SubPowers.Length - 1);
                    foreach (var subPower in powerEntry.SubPowers)
                    {
                        writer.Write(subPower.nIDPower > -1
                            ? DatabaseAPI.Database.Power[subPower.nIDPower].FullName
                            : string.Empty);

                        writer.Write(subPower.StatInclude);
                    }

                    writer.Write(powerEntry.Slots.Length - 1);
                    for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                    {
                        var slot = powerEntry.Slots[slotIndex];
                        writer.Write(slot.Level);
                        writer.Write(slot.IsInherent);
                        switch (incAltEnh)
                        {
                            case true:
                                WriteSlotData(ref writer, ref powerEntry.Slots[slotIndex].Enhancement);
                                WriteSlotData(ref writer, ref powerEntry.Slots[slotIndex].FlippedEnhancement);
                                break;
                            case false:
                                WriteSlotData(ref writer, ref powerEntry.Slots[slotIndex].Enhancement);
                                break;
                        }
                    }
                }
            }
            buffer = memoryStream.ToArray();
            memoryStream.Close();
            writer.Close();
            return true;
        }

        private static void WriteSlotData(ref BinaryWriter writer, ref I9Slot slot)
        {
            if (slot.Enh < 0)
            {
                writer.Write(string.Empty);
            }
            else
            {
                if (slot.Enh <= -1)
                {
                    return;
                }

                writer.Write(DatabaseAPI.Database.Enhancements[slot.Enh].LongName);
                switch (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID)
                {
                    case Enums.eType.Normal or Enums.eType.SpecialO:
                        writer.Write((int)slot.RelativeLevel);
                        writer.Write((int)slot.Grade);
                        break;
                    case Enums.eType.InventO or Enums.eType.SetO:
                        writer.Write(slot.IOLevel);
                        writer.Write((int)slot.RelativeLevel);
                        break;
                }
            }
        }

        public static string BuildSaveString(bool incAltEnh, bool forumMode)
        {
            var cData = new CompressionData();
            var savedString = BuildSharedSaveString(ref cData, incAltEnh, true);
            if (string.IsNullOrWhiteSpace(savedString))
            {
                return string.Empty;
            }
            var str3 = "\n";
            string str4;
            if (forumMode)
            {
                var flag1 = MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].Notes.IndexOf("HTML", StringComparison.Ordinal) > -1;
                var flag2 = MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].Notes.IndexOf("no <br /> tags", StringComparison.OrdinalIgnoreCase) > -1;
                if (flag1 && !flag2)
                {
                    savedString = savedString.Replace("\n", "<br />");
                    str3 = "<br />";
                }

                var str5 = "| Copy & Paste this data into Mids Reborn to view the build |" + str3;
                if (flag1)
                {
                    str5 = str5.Replace(" ", "&nbsp;");
                }

                str4 = str5 + "|-------------------------------------------------------------------|" + str3;
            }
            else
            {
                str4 = "|              Do not modify anything below this line!              |" + str3 +
                       "|-------------------------------------------------------------------|" + str3;
            }

            const string str6 = ";HEX";
            var output = $"{str4}|{MagicCompressed};{cData.SzUncompressed};{cData.SzCompressed};{cData.SzEncoded}{str6};|{str3}{savedString}{str3}";
            // return str4 + "|" + MagicCompressed + ";" + cData.SzUncompressed + ";" + cData.SzCompressed + ";" + cData.SzEncoded + str6 + ";|" + str3 + savedString + str3 + "|-------------------------------------------------------------------|";
            return output;
        }

        private static string BuildSharedSaveString(ref CompressionData cData, bool includeAltEnh, bool @break)
        {
            var byteArray = Array.Empty<byte>();
            string output;
            if (!BuildSaveBuffer(ref byteArray, includeAltEnh))
            {
                output = string.Empty;
            }
            else
            {
                cData.SzUncompressed = byteArray.Length;
                var compressedBytes = Compression.Compress(byteArray);
                cData.SzCompressed = compressedBytes.Length;
                var hexEncoded = BitConverter.ToString(compressedBytes).Replace("-", "");
                cData.SzEncoded = hexEncoded.Length;
                output = @break ? Compression.BreakString(hexEncoded, 67, true) : hexEncoded;
            }
            return output;
        }

        public static bool ReadSaveData(ref byte[] buffer, bool silent)
        {
            if (buffer.Length < 1)
            {
                MessageBox.Show(@"Unable to read data, the buffer is empty.", @"Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            MemoryStream memoryStream;
            BinaryReader reader;
            try
            {
                memoryStream = new MemoryStream(buffer, false);
                reader = new BinaryReader(memoryStream);
                reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                MessageBox.Show($@"Unable to read data, {e.Message}.", @"Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                var streamIndex = 0;
                bool magicFound;
                do
                {
                    reader.BaseStream.Seek(streamIndex, SeekOrigin.Begin);

                    var byteArray = reader.ReadBytes(4);
                    if (byteArray.Length > 4)
                    {
                        if (!silent)
                        {
                            MessageBox.Show(@"Unable to read data, unable to fine Magic Number.", @"Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            reader.Close();
                            memoryStream.Close();
                            return false;
                        }
                    }

                    magicFound = true;
                    for (var byteIndex = 0; byteIndex < byteArray.Length; byteIndex++)
                    {
                        if (MagicNumber[byteIndex] != byteArray[byteIndex])
                        {
                            magicFound = false;
                        }
                    }

                    if (!magicFound)
                    {
                        streamIndex += 1;
                    }
                } while (!magicFound);

                var fileVersion = reader.ReadSingle();
                FileFormat fileFormat;
                switch (fileVersion)
                {
                    case > ThisVersion:
                        MessageBox.Show(@"Character file version appears to be from a more recent version of this application. Please obtain or update to a more recent version of the application.", @"FileVersion Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        reader.Close();
                        memoryStream.Close();
                        return false;
                    case PriorVersion:
                        fileFormat = FileFormat.Prior;
                        break;
                    case < PriorVersion:
                        fileFormat = FileFormat.Legacy;
                        break;
                    default:
                        fileFormat = FileFormat.Current;
                        break;
                }

                var nIdClass = DatabaseAPI.NidFromUidClass(reader.ReadString());
                if (nIdClass < 0)
                {
                    if (!silent)
                    {
                        MessageBox.Show(@"Unable to read data, invalid Class UID.", @"Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    reader.Close();
                    memoryStream.Close();
                    return false;
                }

                var iOrigin = DatabaseAPI.NidFromUidOrigin(reader.ReadString(), nIdClass);
                MidsContext.Character.Reset(DatabaseAPI.Database.Classes[nIdClass], iOrigin);
                MidsContext.Character.Alignment = (Enums.Alignment)reader.ReadInt32();
                MidsContext.Character.Name = reader.ReadString();
                var powerSetCount = reader.ReadInt32();
                if (MidsContext.Character.Powersets.Length - 1 != powerSetCount)
                {
                    MidsContext.Character.Powersets = new IPowerset[powerSetCount + 1];
                }

                for (var setIndex = 0; setIndex < MidsContext.Character.Powersets.Length; setIndex++)
                {
                    var setName = reader.ReadString();
                    if (string.IsNullOrWhiteSpace(setName))
                    {
                        MidsContext.Character.Powersets[setIndex] = null;
                    }
                    else
                    {

                    }
                }

                MidsContext.Character.CurrentBuild.LastPower = reader.ReadInt32() - 1;
                var powerCount = reader.ReadInt32();
                for (var powerIndex = 0; powerIndex <= powerCount; ++powerIndex)
                {
                    int powerNiD = -1;
                    PowerEntry powerEntry;
                    var isInherentPower = false;

                    var power = reader.ReadString();
                    
                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        powerEntry = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                    }
                    else
                    {
                        powerEntry = new PowerEntry();
                        isInherentPower = true;
                    }

                    if (!string.IsNullOrWhiteSpace(power))
                    {
                        powerNiD = DatabaseAPI.NidFromUidPower(power);
                        powerEntry.Level = reader.ReadInt32();
                        switch (fileFormat)
                        {
                            case FileFormat.Current:
                                powerEntry.StatInclude = reader.ReadBoolean();
                                powerEntry.ProcInclude = reader.ReadBoolean();
                                powerEntry.VariableValue = reader.ReadInt32();
                                powerEntry.InherentSlotsUsed = reader.ReadInt32();
                                break;
                            case FileFormat.Prior:
                                powerEntry.StatInclude = reader.ReadBoolean();
                                powerEntry.ProcInclude = reader.ReadBoolean();
                                powerEntry.VariableValue = reader.ReadInt32();
                                break;
                            case FileFormat.Legacy:
                                powerEntry.StatInclude = reader.ReadBoolean();
                                powerEntry.VariableValue = reader.ReadInt32();
                                break;
                        }

                        var subPowerCount = reader.ReadInt32();
                        powerEntry.SubPowers = new PowerSubEntry[subPowerCount + 1];
                        for (var subIndex = 0; subIndex <= subPowerCount; subIndex++)
                        {
                            var subPower = new PowerSubEntry();
                            powerEntry.SubPowers[subIndex] = subPower;

                            var subPowerName = reader.ReadString();
                            subPower.nIDPower = DatabaseAPI.NidFromUidPower(subPowerName);
                            if (subPower.nIDPower > -1)
                            {
                                subPower.Powerset = DatabaseAPI.Database.Power[subPower.nIDPower].PowerSetID;
                                subPower.Power = DatabaseAPI.Database.Power[subPower.nIDPower].PowerSetIndex;
                            }

                            subPower.StatInclude = reader.ReadBoolean();
                            if (!((subPower.nIDPower > -1) & subPower.StatInclude))
                            {
                                continue;
                            }

                            var powerEntry2 = new PowerEntry(DatabaseAPI.Database.Power[subPower.nIDPower])
                            {
                                StatInclude = true
                            };
                            MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                        }
                    }

                    if (powerNiD < 0 && powerIndex < DatabaseAPI.Database.Levels_MainPowers.Length)
                    {
                        powerEntry.Level = DatabaseAPI.Database.Levels_MainPowers[powerIndex];
                    }

                    var slotCount = reader.ReadInt32();
                    powerEntry.Slots = new SlotEntry[slotCount + 1];
                    for (var slotIndex = 0; slotIndex <= slotCount; slotIndex++)
                    {
                        switch (fileFormat)
                        {
                            case FileFormat.Current:
                                powerEntry.Slots[slotIndex] = new SlotEntry
                                {
                                    Level = reader.ReadInt32(),
                                    IsInherent = reader.ReadBoolean(),
                                    Enhancement = new I9Slot(),
                                    FlippedEnhancement = new I9Slot()
                                };
                                break;
                            case FileFormat.Legacy or FileFormat.Prior:
                                powerEntry.Slots[slotIndex] = new SlotEntry
                                {
                                    Level = reader.ReadInt32(),
                                    Enhancement = new I9Slot(),
                                    FlippedEnhancement = new I9Slot()
                                };
                                break;
                        }

                        ReadSlotData(reader, ref powerEntry.Slots[slotIndex].Enhancement, fileVersion);
                        ReadSlotData(reader, ref powerEntry.Slots[slotIndex].FlippedEnhancement, fileVersion);
                    }

                    if (powerEntry.SubPowers.Length > 0)
                    {
                        powerNiD = -1;
                    }

                    if (powerNiD <= -1)
                    {
                        continue;
                    }

                    powerEntry.NIDPower = powerNiD;
                    powerEntry.NIDPowerset = DatabaseAPI.Database.Power[powerNiD].PowerSetID;
                    powerEntry.IDXPower = DatabaseAPI.Database.Power[powerNiD].PowerSetIndex;
                    if (powerEntry.Level == 0 && powerEntry.Power.FullSetName == "Pool.Fitness")
                    {
                        var updatedPower = DatabaseAPI.GetPowerByFullName(powerEntry.Power.FullName.Replace("Pool.", "Inherent."));
                        powerEntry.NIDPowerset = updatedPower.PowerSetID;
                        powerEntry.IDXPower = updatedPower.PowerSetIndex;
                    }

                    var powerSet = powerEntry.Power?.GetPowerSet();
                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        if (powerEntry.Power != null && !(!MidsContext.Character.CurrentBuild.Powers[powerIndex].Chosen & (powerSet != null && powerSet.nArchetype > -1 || powerEntry.Power.GroupName == "Pool")))
                        {
                            isInherentPower = !MidsContext.Character.CurrentBuild.Powers[powerIndex].Chosen;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (isInherentPower)
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
                if (!silent)
                {
                    MessageBox.Show($@"An error occurred while attempting to read some of the power data.\r\n{ex.Message}\r\n\r\n{ex.StackTrace}", @"Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
            MidsContext.Archetype = MidsContext.Character.Archetype;
            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            return true;
        }

        private static void ReadSlotData(BinaryReader reader, ref I9Slot slot, float fileVersion)
        {
            var enhName = reader.ReadString();
            if (!string.IsNullOrWhiteSpace(enhName))
            {
                slot.Enh = DatabaseAPI.NidFromUidEnh(enhName);
            }

            if (slot.Enh <= 0) return;
            switch (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID)
            {
                case Enums.eType.Normal or Enums.eType.SpecialO:
                    slot.RelativeLevel = (Enums.eEnhRelative)reader.ReadInt32();
                    slot.Grade = (Enums.eEnhGrade)reader.ReadInt32();

                    break;
                case Enums.eType.InventO or Enums.eType.SetO:
                    slot.IOLevel = reader.ReadInt32();
                    if (fileVersion > 1.0f)
                    {
                        slot.RelativeLevel = (Enums.eEnhRelative)reader.ReadInt32();
                    }

                    break;
                case Enums.eType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /*public static LoadReturnCode ExtractAndLoad(Stream stream)
        {
            using var reader = new StreamReader(stream);
            reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            var dataString = reader.ReadToEnd().Replace("||", "|\n|");
            reader.Close();

            var readArray = dataString.Split('\n');
            var outArray = Array.Empty<string>();
            var headerData = string.Empty;
            var indexPos = -1;
            if (readArray.Length < 1)
            {
                MessageBox.Show(@"Unable to locate data header, zero-length input.", @"Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }

            for (var arrayIndex = 0; arrayIndex < readArray.Length; arrayIndex++)
            {
                var startIndex = readArray[arrayIndex].IndexOf(MagicUncompressed, StringComparison.Ordinal);
                if (startIndex < 0)
                {
                    startIndex = readArray[arrayIndex].IndexOf(MagicCompressed, StringComparison.Ordinal);
                }
                if (startIndex < 0)
                {
                    startIndex = readArray[arrayIndex].IndexOf(Files.Headers.Save.Compressed, StringComparison.Ordinal);
                }
                if (startIndex <= -1)
                {
                    continue;
                }

                outArray = readArray[arrayIndex].Substring(startIndex).Split(';');
                if (outArray.Length > 0)
                {
                    headerData = outArray[0];
                }

                indexPos = arrayIndex;
                break;
            }

            if (indexPos < 0)
            {
                MessageBox.Show(@"Unable to locate data header. The magic number was not found.", @"Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LoadReturnCode.Failure;
            }

            if (indexPos + 1 == readArray.Length)
            {
                MessageBox.Show("Unable to locate data - Nothing beyond header!", "ExtractAndLoad Failed");
                return LoadReturnCode.Failure;
            }

            var iString = string.Empty;
            for (var index = indexPos + 1; index <= readArray.Length - 1; ++index)
            {
                iString += readArray[index] + "\n";
            }

            var item1 = Convert.ToInt32(outArray[1]);
            var item2 = Convert.ToInt32(outArray[2]);
            var item3 = Convert.ToInt32(outArray[3]);
            var isHex = false;
            if (outArray.Length > 4)
            {
                isHex = string.Equals(outArray[4], "HEX", StringComparison.OrdinalIgnoreCase);
            }

            byte[] iBytes;
            if (isHex)
            {
                var compiledString = Compression.UnbreakString(iString, true);
                var decodedBytes = 
            }

        }*/

        /*public static bool ExtractAndLoad2(Stream iStream)
        {
            StreamReader streamReader;
            try
            {
                streamReader = new StreamReader(iStream);
                streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read data - " + ex.Message, "ExtractAndLoad Failed");
                return false;
            }

            string[] strArray1 =
            {
                "ABCD",
                "0",
                "0",
                "0"
            };
            var a = "";
            try
            {
                var str = streamReader.ReadToEnd().Replace("||", "|\n|");
                streamReader.Close();
                var strArray2 = str.Split('\n');
                var num1 = -1;
                if (strArray2.Length < 1)
                {
                    MessageBox.Show("Unable to locate data header - Zero-Length Input!", "ExtractAndLoad Failed");
                    return false;
                }
                else
                {
                    for (var index = 0; index < strArray2.Length; ++index)
                    {
                        var startIndex = strArray2[index].IndexOf(MagicUncompressed, StringComparison.Ordinal);
                        if (startIndex < 0)
                        {
                            startIndex = strArray2[index].IndexOf(MagicCompressed, StringComparison.Ordinal);
                        }

                        if (startIndex < 0)
                        {
                            startIndex = strArray2[index].IndexOf(Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase);
                        }

                        if (startIndex <= -1)
                        {
                            continue;
                        }

                        strArray1 = strArray2[index].Substring(startIndex).Split(';');
                        if (strArray1.Length > 0)
                        {
                            a = strArray1[0];
                        }
                        else
                        {
                            a = string.Empty;
                        }

                        num1 = index;
                        break;
                    }

                    if (num1 < 0)
                    {
                        MessageBox.Show("Unable to locate data header - Magic Number not found!", "ExtractAndLoad Failed");
                    }
                    else if (string.Equals(a, Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase))
                    {

                    }
                    else if (num1 + 1 == strArray2.Length)
                    {
                        MessageBox.Show("Unable to locate data - Nothing beyond header!", "ExtractAndLoad Failed");
                    }
                    else
                    {
                        var iString = string.Empty;
                        for (var index = num1 + 1; index <= strArray2.Length - 1; ++index)
                        {
                            iString = iString + strArray2[index] + "\n";
                        }

                        var int32_1 = Convert.ToInt32(strArray1[1]);
                        var int32_2 = Convert.ToInt32(strArray1[2]);
                        var int32_3 = Convert.ToInt32(strArray1[3]);
                        var isHex = false;
                        if (strArray1.Length > 4)
                        {
                            isHex = string.Equals(strArray1[4], "HEX", StringComparison.OrdinalIgnoreCase);
                        }

                        // var iBytes =
                        //     new ASCIIEncoding().GetBytes(isHex
                        //         ? Zlib.UnbreakHex(iString)
                        //         : Zlib.UnbreakString(iString, true));
                        streamReader.Close();
                        if (iBytes.Length < int32_3)
                        {
                            MessageBox.Show(
                                "Data chunk was incomplete! Check that the entire chunk was copied to the clipboard.",
                                "ExtractAndLoad Failed");
                            
                        }
                        else
                        {
                            if (iBytes.Length > int32_3)
                            {
                                Array.Resize(ref iBytes, int32_3);
                            }

                            iBytes = isHex ? Zlib.HexDecodeBytes(iBytes) : Zlib.UUDecodeBytes(iBytes);
                            if (iBytes.Length == 0)
                            {
                                
                            }
                            else
                            {
                                if (a == MagicCompressed)
                                {
                                    Array.Resize(ref iBytes, int32_2);
                                    var tempByteArray = iBytes; // Pine
                                    iBytes = Zlib.UncompressChunk(ref tempByteArray, int32_1);
                                }

                                eLoadReturnCode = iBytes.Length != 0
                                    ? MxDReadSaveData(ref iBytes, false) ? eLoadReturnCode.Success : eLoadReturnCode.Failure
                                    : eLoadReturnCode.Failure;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read data - " + ex.Message, "ExtractAndLoad Failed");
                streamReader.Close();
                eLoadReturnCode = eLoadReturnCode.Failure;
            }

            return eLoadReturnCode;
        }*/
    }
}

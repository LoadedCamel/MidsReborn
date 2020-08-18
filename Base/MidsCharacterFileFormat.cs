using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Base.Master_Classes;

public static class MidsCharacterFileFormat
{
    public enum eLoadReturnCode
    {
        Failure,
        Success,
        IsOldFormat
    }

    private const string MagicCompressed = "MxDz";
    private const string MagicUncompressed = "MxDu";
    private const float SaveVersion = 1.01f;

    private const int DataLinkMaxLength = 2048;

    private const bool UseQualifiedNames = false;

    private const bool UseOldSubpowerFields = true;

    private static readonly byte[] MagicNumber =
    {
        Convert.ToByte('M'),
        Convert.ToByte('x'),
        Convert.ToByte('D'),
        Convert.ToByte(12)
    };

    //const bool UseHexEncoding = true;


    private static bool MxDBuildSaveBuffer(ref byte[] buffer, bool includeAltEnh)
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
            MessageBox.Show("Save Failed!\n" + ex.Message);
            return false;
        }

        writer.Write(MagicNumber, 0, MagicNumber.Length);
        writer.Write(SaveVersion);
        writer.Write(UseQualifiedNames);
        writer.Write(UseOldSubpowerFields);
        writer.Write(MidsContext.Character.Archetype.ClassName);
        writer.Write(MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin]);
        writer.Write((int) MidsContext.Character.Alignment);
        writer.Write(MidsContext.Character.Name);
        writer.Write(MidsContext.Character.Powersets.Length - 1);
        foreach (var index in MidsContext.Character.Powersets)
            writer.Write(index != null ? index.FullName : string.Empty);

        writer.Write(MidsContext.Character.CurrentBuild.LastPower + 1);
        writer.Write(MidsContext.Character.CurrentBuild.Powers.Count - 1);
        foreach (var power in MidsContext.Character.CurrentBuild.Powers)
        {
            if (power.NIDPower < 0)
            {
                writer.Write(-1);
            }
            else
            {
                writer.Write(DatabaseAPI.Database.Power[power.NIDPower].StaticIndex);
                writer.Write(Convert.ToSByte(power.Level));
                writer.Write(Convert.ToBoolean(power.StatInclude));
                writer.Write(power.VariableValue);
                writer.Write(Convert.ToSByte(power.SubPowers.Length - 1));
                foreach (var index2 in power.SubPowers)
                {
                    if (index2.nIDPower > -1)
                        writer.Write(DatabaseAPI.Database.Power[index2.nIDPower].StaticIndex);
                    else
                        writer.Write(-1);
                    writer.Write(index2.StatInclude);
                }
            }

            writer.Write(Convert.ToSByte(power.Slots.Length - 1));
            for (var index2 = 0; index2 <= power.Slots.Length - 1; ++index2)
            {
                writer.Write(Convert.ToSByte(power.Slots[index2].Level));
                WriteSlotData(ref writer, ref power.Slots[index2].Enhancement);
                writer.Write(includeAltEnh);
                if (includeAltEnh)
                    WriteSlotData(ref writer, ref power.Slots[index2].FlippedEnhancement);
            }
        }

        buffer = memoryStream.ToArray();
        return true;
    }

    public static string MxDBuildSaveHyperlink(bool useBbCode, bool justLink = false)
    {
        var cData = new CompressionData();
        var str1 = MxDBuildSaveStringShared(ref cData, false, false);
        if (string.IsNullOrEmpty(str1)) return string.Empty;

        // this one seems to still work as intended, we may not need to change it
        var str3 = "http://www.cohplanner.com/mids/download.php" + "?uc=" + cData.SzUncompressed + "&c=" +
                   cData.SzCompressed + "&a=" + cData.SzEncoded + "&f=HEX&dc=" + str1;
        return str3.Length <= DataLinkMaxLength
            ? !justLink ? !useBbCode ? "<a href=\"" + str3 + "\">Click this DataLink to open the build!</a>" :
            "[url=" + str3 + "]Click this DataLink to open the build![/url]" : str3
            : "";
    }

    private static string MxDBuildSaveStringShared(
        ref CompressionData cData,
        bool includeAltEnh,
        bool @break)
    {
        var numArray = Array.Empty<byte>();
        string str;
        if (!MxDBuildSaveBuffer(ref numArray, includeAltEnh))
        {
            str = string.Empty;
        }
        else
        {
            var asciiEncoding = new ASCIIEncoding();
            cData.SzUncompressed = numArray.Length;
            var iBytes = Zlib.CompressChunk(ref numArray);
            cData.SzCompressed = iBytes.Length;
            var bytes = Zlib.HexEncodeBytes(iBytes);
            cData.SzEncoded = bytes.Length;
            str = @break ? Zlib.BreakString(asciiEncoding.GetString(bytes), 67, true) : asciiEncoding.GetString(bytes);
        }

        return str;
    }

    public static string MxDBuildSaveString(bool inncludeAltEnh, bool forumMode)
    {
        var cData = new CompressionData();
        var str1 = MxDBuildSaveStringShared(ref cData, inncludeAltEnh, true);
        if (string.IsNullOrEmpty(str1)) return string.Empty;

        var str3 = "\n";
        string str4;
        if (forumMode)
        {
            var flag1 = MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].Notes
                .IndexOf("HTML", StringComparison.Ordinal) > -1;
            var flag2 = MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].Notes
                .IndexOf("no <br /> tags", StringComparison.OrdinalIgnoreCase) > -1;
            if (flag1 && !flag2)
            {
                str1 = str1.Replace("\n", "<br />");
                str3 = "<br />";
            }

            // TODO: review this string for update to Mid's Reborn
            // needs checking the reader to see if changing this will break anything
            var str5 = "| Copy & Paste this data into Mids' Reborn : Hero Designer to view the build |" + str3;
            if (flag1)
                str5 = str5.Replace(" ", "&nbsp;");
            str4 = str5 + "|-------------------------------------------------------------------|" + str3;
        }
        else
        {
            str4 = "|              Do not modify anything below this line!              |" + str3 +
                   "|-------------------------------------------------------------------|" + str3;
        }

        var str6 = ";HEX";
        return str4 + "|" + MagicCompressed + ";" + cData.SzUncompressed + ";" + cData.SzCompressed + ";" +
               cData.SzEncoded + str6 + ";|" + str3 + str1 + str3 +
               "|-------------------------------------------------------------------|";
    }

    private static bool MxDReadSaveData(ref byte[] buffer, bool silent)
    {
        if (buffer.Length < 1)
        {
            MessageBox.Show("Unable to read data - Empty Buffer.", "ReadSaveData Failed");
            return false;
        }

        MemoryStream memoryStream;
        BinaryReader r;
        try
        {
            memoryStream = new MemoryStream(buffer, false);
            r = new BinaryReader(memoryStream);
            r.BaseStream.Seek(0L, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to read data - " + ex.Message, "ReadSaveData Failed");
            return false;
        }

        try
        {
            var streamIndex = 0;
            bool magicFound;
            // try to find magic number, reading 4 bytes at a time, offset by 1 on each failure
            do
            {
                r.BaseStream.Seek(streamIndex, SeekOrigin.Begin);

                var numArray = r.ReadBytes(4);
                if (numArray.Length >= 4)
                {
                    magicFound = true;
                    for (var index = 0; index < MagicNumber.Length; ++index)
                        if (MagicNumber[index] != numArray[index])
                            magicFound = false;
                    if (!magicFound)
                        ++streamIndex;
                }
                else
                {
                    if (!silent)
                        MessageBox.Show("Unable to read data - Magic Number not found.", "ReadSaveData Failed");
                    r.Close();
                    memoryStream.Close();
                    return false;
                }
            } while (!magicFound);

            var fVersion = r.ReadSingle();
            if (fVersion > 2.0)
            {
                MessageBox.Show(
                    "File was saved by a newer version of the application. Please obtain the most recent release in order to open this file.",
                    "Unable to Load");
                r.Close();
                memoryStream.Close();
                return false;
            }

            var qualifiedNames = r.ReadBoolean();
            var hasSubPower = r.ReadBoolean();
            var nIDClass = DatabaseAPI.NidFromUidClass(r.ReadString());
            if (nIDClass < 0)
            {
                if (!silent) MessageBox.Show("Unable to read data - Invalid Class UID.", "ReadSaveData Failed");
                r.Close();
                memoryStream.Close();
                return false;
            }

            var iOrigin = DatabaseAPI.NidFromUidOrigin(r.ReadString(), nIDClass);
            MidsContext.Character.Reset(DatabaseAPI.Database.Classes[nIDClass], iOrigin);
            if (fVersion > 1.0)
            {
                var align = r.ReadInt32();
                MidsContext.Character.Alignment = (Enums.Alignment) align;
            }

            MidsContext.Character.Name = r.ReadString();
            var powerSetCount = r.ReadInt32();
            var expectedArrayLength = new IPowerset[powerSetCount + 1].Length;
            var names = new List<string>();
            for (var index = 0; index < powerSetCount + 1; ++index)
            {
                var iName = r.ReadString();
                names.Add(iName);
            }

            var errors = MidsContext.Character.LoadPowersetsByName(names);
            foreach (var (i, n) in errors)
                MessageBox.Show($"Failed to load powerset by name:{n} at {i}", "Powerset load failure");
            MidsContext.Character.CurrentBuild.LastPower = r.ReadInt32() - 1;
            var powerCount = r.ReadInt32();
            try
            {
                for (var powerIndex = 0; powerIndex <= powerCount; ++powerIndex)
                {
                    var nId = -1;
                    var name1 = string.Empty;
                    var sidPower1 = -1;
                    if (qualifiedNames)
                    {
                        name1 = r.ReadString();
                        if (!string.IsNullOrEmpty(name1))
                            nId = DatabaseAPI.NidFromUidPower(name1);
                    }
                    else
                    {
                        sidPower1 = r.ReadInt32();
                        nId = DatabaseAPI.NidFromStaticIndexPower(sidPower1);
                    }

                    var flag5 = false;
                    PowerEntry powerEntry1;
                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        powerEntry1 = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                    }
                    else
                    {
                        powerEntry1 = new PowerEntry();
                        flag5 = true;
                    }

                    if ((sidPower1 > -1) | !string.IsNullOrEmpty(name1))
                    {
                        powerEntry1.Level = r.ReadSByte();
                        powerEntry1.StatInclude = r.ReadBoolean();
                        powerEntry1.VariableValue = r.ReadInt32();
                        if (hasSubPower)
                        {
                            powerEntry1.SubPowers = new PowerSubEntry[r.ReadSByte() + 1];
                            for (var subPowerIndex = 0; subPowerIndex < powerEntry1.SubPowers.Length; ++subPowerIndex)
                            {
                                var powerSub = new PowerSubEntry();
                                powerEntry1.SubPowers[subPowerIndex] = powerSub;
                                if (qualifiedNames)
                                {
                                    var name2 = r.ReadString();
                                    if (!string.IsNullOrEmpty(name2))
                                        powerSub.nIDPower = DatabaseAPI.NidFromUidPower(name2);
                                }
                                else
                                {
                                    var sidPower2 = r.ReadInt32();
                                    powerSub.nIDPower = DatabaseAPI.NidFromStaticIndexPower(sidPower2);
                                }

                                if (powerSub.nIDPower > -1)
                                {
                                    powerSub.Powerset = DatabaseAPI.Database.Power[powerSub.nIDPower].PowerSetID;
                                    powerSub.Power = DatabaseAPI.Database.Power[powerSub.nIDPower].PowerSetIndex;
                                }

                                powerSub.StatInclude = r.ReadBoolean();
                                if (!((powerSub.nIDPower > -1) & powerSub.StatInclude))
                                    continue;
                                var powerEntry2 = new PowerEntry(DatabaseAPI.Database.Power[powerSub.nIDPower])
                                {
                                    StatInclude = true
                                };
                                MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                            }
                        }
                    }

                    if (nId < 0 && powerIndex < DatabaseAPI.Database.Levels_MainPowers.Length)
                        powerEntry1.Level = DatabaseAPI.Database.Levels_MainPowers[powerIndex];
                    powerEntry1.Slots = new SlotEntry[r.ReadSByte() + 1];
                    for (var index3 = 0; index3 < powerEntry1.Slots.Length; ++index3)
                    {
                        powerEntry1.Slots[index3] = new SlotEntry
                        {
                            Level = r.ReadSByte(),
                            Enhancement = new I9Slot(),
                            FlippedEnhancement = new I9Slot()
                        };
                        ReadSlotData(r, ref powerEntry1.Slots[index3].Enhancement, qualifiedNames, fVersion);
                        if (r.ReadBoolean())
                            ReadSlotData(r, ref powerEntry1.Slots[index3].FlippedEnhancement, qualifiedNames, fVersion);
                    }

                    if (powerEntry1.SubPowers.Length > 0)
                        nId = -1;
                    if (nId <= -1)
                        continue;
                    powerEntry1.NIDPower = nId;
                    powerEntry1.NIDPowerset = DatabaseAPI.Database.Power[nId].PowerSetID;
                    powerEntry1.IDXPower = DatabaseAPI.Database.Power[nId].PowerSetIndex;
                    if (powerEntry1.Level == 0 && powerEntry1.Power.FullSetName == "Pool.Fitness")
                    {
                        if (powerEntry1.NIDPower == 2553)
                            powerEntry1.NIDPower = 1521;
                        if (powerEntry1.NIDPower == 2554)
                            powerEntry1.NIDPower = 1523;
                        if (powerEntry1.NIDPower == 2555)
                            powerEntry1.NIDPower = 1522;
                        if (powerEntry1.NIDPower == 2556)
                            powerEntry1.NIDPower = 1524;
                        powerEntry1.NIDPowerset = DatabaseAPI.Database.Power[nId].PowerSetID;
                        powerEntry1.IDXPower = DatabaseAPI.Database.Power[nId].PowerSetIndex;
                    }

                    var ps = powerEntry1.Power?.GetPowerSet();
                    if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        if (powerEntry1.Power != null &&
                            !(!MidsContext.Character.CurrentBuild.Powers[powerIndex].Chosen &
                              (ps != null && ps.nArchetype > -1 || powerEntry1.Power.GroupName == "Pool")))
                            flag5 = !MidsContext.Character.CurrentBuild.Powers[powerIndex].Chosen;
                        else
                            continue;
                    }

                    if (flag5)
                        MidsContext.Character.CurrentBuild.Powers.Add(powerEntry1);
                    else if (powerEntry1.Power != null &&
                             (ps != null && ps.nArchetype > -1 || powerEntry1.Power.GroupName == "Pool"))
                        MidsContext.Character.CurrentBuild.Powers[powerIndex] = powerEntry1;
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show(
                        "Error reading some power data, will attempt to build character with known data - " +
                        ex.Message, "ReadSaveData Failed");
                return false;
            }

            MidsContext.Archetype = MidsContext.Character.Archetype;
            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            return true;
        }
        catch (Exception ex)
        {
            if (!silent) MessageBox.Show("Unable to read data - " + ex.Message, "ReadSaveData Failed");
            return false;
        }
    }

    public static eLoadReturnCode MxDExtractAndLoad(
        Stream iStream)
    {
        StreamReader streamReader;
        try
        {
            streamReader = new StreamReader(iStream);
            streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
            var num = (int) MessageBox.Show("Unable to read data - " + ex.Message, "ExtractAndLoad Failed");
            return MidsCharacterFileFormat.eLoadReturnCode.Failure;
        }

        string[] strArray1 =
        {
            "ABCD",
            "0",
            "0",
            "0"
        };
        var a = "";
        eLoadReturnCode eLoadReturnCode;
        try
        {
            var str = streamReader.ReadToEnd().Replace("||", "|\n|");
            streamReader.Close();
            var strArray2 = str.Split('\n');
            var num1 = -1;
            if (strArray2.Length < 1)
            {
                var num2 = (int) MessageBox.Show("Unable to locate data header - Zero-Length Input!",
                    "ExtractAndLoad Failed");
                eLoadReturnCode = eLoadReturnCode.Failure;
            }
            else
            {
                for (var index = 0; index < strArray2.Length; ++index)
                {
                    var startIndex = strArray2[index].IndexOf(MagicUncompressed, StringComparison.Ordinal);
                    if (startIndex < 0)
                        startIndex = strArray2[index].IndexOf(MagicCompressed, StringComparison.Ordinal);
                    if (startIndex < 0)
                        startIndex = strArray2[index].IndexOf(Files.Headers.Save.Compressed,
                            StringComparison.OrdinalIgnoreCase);
                    if (startIndex <= -1)
                        continue;
                    strArray1 = strArray2[index].Substring(startIndex).Split(';');
                    a = strArray1.Length > 0 ? strArray1[0] : string.Empty;
                    num1 = index;
                    break;
                }

                if (num1 < 0)
                {
                    MessageBox.Show("Unable to locate data header - Magic Number not found!", "ExtractAndLoad Failed");
                    eLoadReturnCode = eLoadReturnCode.Failure;
                }
                else if (string.Equals(a, Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase))
                {
                    eLoadReturnCode = eLoadReturnCode.IsOldFormat;
                }
                else if (num1 + 1 == strArray2.Length)
                {
                    MessageBox.Show("Unable to locate data - Nothing beyond header!", "ExtractAndLoad Failed");
                    eLoadReturnCode = eLoadReturnCode.Failure;
                }
                else
                {
                    var iString = string.Empty;
                    for (var index = num1 + 1; index <= strArray2.Length - 1; ++index)
                        iString = iString + strArray2[index] + "\n";
                    var int32_1 = Convert.ToInt32(strArray1[1]);
                    var int32_2 = Convert.ToInt32(strArray1[2]);
                    var int32_3 = Convert.ToInt32(strArray1[3]);
                    var isHex = false;
                    if (strArray1.Length > 4)
                        isHex = string.Equals(strArray1[4], "HEX", StringComparison.OrdinalIgnoreCase);
                    var iBytes =
                        new ASCIIEncoding().GetBytes(isHex
                            ? Zlib.UnbreakHex(iString)
                            : Zlib.UnbreakString(iString, true));
                    streamReader.Close();
                    if (iBytes.Length < int32_3)
                    {
                        MessageBox.Show(
                            "Data chunk was incomplete! Check that the entire chunk was copied to the clipboard.",
                            "ExtractAndLoad Failed");
                        eLoadReturnCode = eLoadReturnCode.Failure;
                    }
                    else
                    {
                        if (iBytes.Length > int32_3)
                            Array.Resize(ref iBytes, int32_3);
                        iBytes = isHex ? Zlib.HexDecodeBytes(iBytes) : Zlib.UUDecodeBytes(iBytes);
                        if (iBytes.Length == 0)
                        {
                            eLoadReturnCode = eLoadReturnCode.Failure;
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
    }

    private static void WriteSlotData(ref BinaryWriter writer, ref I9Slot slot)
    {
        if (slot.Enh < 0)
        {
            writer.Write(-1);
        }
        else
        {
            if (slot.Enh <= -1)
                return;
            writer.Write(DatabaseAPI.Database.Enhancements[slot.Enh].StaticIndex);
            if (DatabaseAPI.Database.Enhancements[slot.Enh].StaticIndex < 0)
                return;
            if ((DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.Normal) |
                (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SpecialO))
            {
                writer.Write(Convert.ToSByte(slot.RelativeLevel));
                writer.Write(Convert.ToSByte(slot.Grade));
            }
            else if ((DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.InventO) |
                     (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID == Enums.eType.SetO))
            {
                writer.Write(Convert.ToSByte(slot.IOLevel));
                writer.Write(Convert.ToSByte(slot.RelativeLevel));
            }
        }
    }

    private static void ReadSlotData(
        BinaryReader reader,
        ref I9Slot slot,
        bool qualifiedNames,
        float fVersion)
    {
        var num = -1;
        if (qualifiedNames)
        {
            var uidEnh = reader.ReadString();
            if (!string.IsNullOrEmpty(uidEnh))
                num = DatabaseAPI.NidFromUidEnh(uidEnh);
        }
        else
        {
            num = DatabaseAPI.NidFromStaticIndexEnh(reader.ReadInt32());
        }

        if (num <= -1)
            return;
        slot.Enh = num;
        switch (DatabaseAPI.Database.Enhancements[slot.Enh].TypeID)
        {
            case Enums.eType.Normal:
            case Enums.eType.SpecialO:
                slot.RelativeLevel = (Enums.eEnhRelative) reader.ReadSByte();
                slot.Grade = (Enums.eEnhGrade) reader.ReadSByte();
                break;
            case Enums.eType.InventO:
            case Enums.eType.SetO:
            {
                slot.IOLevel = reader.ReadSByte();
                if (slot.IOLevel > 49)
                    slot.IOLevel = 49;
                if (fVersion > 1.0)
                    slot.RelativeLevel = (Enums.eEnhRelative) reader.ReadSByte();
                break;
            }
            case Enums.eType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private struct CompressionData

    {
        public int SzUncompressed;
        public int SzCompressed;
        public int SzEncoded;
    }
}
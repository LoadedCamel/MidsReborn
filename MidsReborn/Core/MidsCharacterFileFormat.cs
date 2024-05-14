using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Core
{
    public static class MidsCharacterFileFormat
    {
        private static int DisplayIndex { get; set; } = -1;
        private static List<PowerEntry> InherentPowers { get; set; } = new();

        public enum eLoadReturnCode
        {
            Failure,
            Success,
            IsOldFormat
        }

        private enum Formats
        {
            Current,
            Prior,
            Legacy
        }

        private const string MagicCompressed = "MxDz";
        private const string MagicUncompressed = "MxDu";
        private const float PriorVersion = 3.10f;
        private const float ThisVersion = 3.20f;

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

        private static string DecodeEntities(string s)
        {
            return s.Replace("&lt;", "<").Replace("&gt;", ">");
        }

        private static string EncodeEntities(string s)
        {
            return s.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string? ReadMetadata(string tagName, string s)
        {
            var r = new Regex($@"\<{tagName}\>(.+)\<\/{tagName}\>");
            if (!r.IsMatch(s))
            {
                return null;
            }

            var m = r.Match(s);

            return DecodeEntities(m.Groups[1].Value.Trim());
        }

        public static Dictionary<string, string> ReadMetadata(List<string> tagNames, string s)
        {
            var ret = new Dictionary<string, string>();

            foreach (var tag in tagNames)
            {
                var r = new Regex($@"\<{tag}\>(.+)\<\/{tag}\>");
                if (!r.IsMatch(s))
                {
                    continue;
                }

                var m = r.Match(s);

                ret.Add(tag, DecodeEntities(m.Groups[1].Value.Trim()));
            }

            return ret;
        }

        private static string SaveMetadata(string tagName, string s)
        {
            return $"<{tagName}>{EncodeEntities(s.Trim())}</{tagName}>";
        }

        private static string SaveMetadata(Dictionary<string, string> tagValues)
        {
            return string.Join("\r\n", tagValues.Select(e => $"<{e.Key}>{EncodeEntities(e.Value.Trim())}</{e.Key}>")) + (tagValues.Count > 0 ? "\r\n" : "");
        }

        private static bool MxDBuildSaveBuffer(ref byte[] buffer, bool includeAltEnh)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var binWriter = new BinaryWriter(memoryStream);

                binWriter.Write(MagicNumber, 0, MagicNumber.Length);
                binWriter.Write(ThisVersion);
                binWriter.Write(UseQualifiedNames);
                binWriter.Write(UseOldSubpowerFields);
                binWriter.Write(MidsContext.Character.Archetype.ClassName);
                binWriter.Write(MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin]);
                binWriter.Write((int)MidsContext.Character.Alignment);
                binWriter.Write(MidsContext.Character.Name);
                binWriter.Write(MidsContext.Character.Powersets.Length - 1);
                foreach (var index in MidsContext.Character.Powersets)
                {
                    binWriter.Write(index != null ? index.FullName : string.Empty);
                }

                binWriter.Write(MidsContext.Character.CurrentBuild.LastPower + 1);
                binWriter.Write(MidsContext.Character.CurrentBuild.Powers.Count - 1);
                foreach (var power in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (power.NIDPower < 0)
                    {
                        binWriter.Write(-1);
                    }
                    else
                    {
                        binWriter.Write(DatabaseAPI.Database.Power[power.NIDPower].StaticIndex);
                        binWriter.Write(Convert.ToSByte(power.Level));
                        binWriter.Write(Convert.ToBoolean(power.StatInclude));
                        binWriter.Write(Convert.ToBoolean(power.ProcInclude));
                        binWriter.Write(power.VariableValue);
                        binWriter.Write(power.InherentSlotsUsed);
                        binWriter.Write(Convert.ToSByte(power.SubPowers.Length - 1));
                        foreach (var index2 in power.SubPowers)
                        {
                            if (index2.nIDPower > -1)
                                binWriter.Write(DatabaseAPI.Database.Power[index2.nIDPower].StaticIndex);
                            else
                                binWriter.Write(-1);
                            binWriter.Write(index2.StatInclude);
                        }
                    }

                    binWriter.Write(Convert.ToSByte(power.Slots.Length - 1));
                    for (var index2 = 0; index2 <= power.Slots.Length - 1; ++index2)
                    {
                        binWriter.Write(Convert.ToSByte(power.Slots[index2].Level));
                        binWriter.Write(Convert.ToBoolean(power.Slots[index2].IsInherent));
                        var writer = binWriter;
                        WriteSlotData(ref writer, ref power.Slots[index2].Enhancement);
                        binWriter.Write(includeAltEnh);
                        if (includeAltEnh)
                            WriteSlotData(ref writer, ref power.Slots[index2].FlippedEnhancement);
                    }
                }

                buffer = memoryStream.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save Failed!\n{ex.Message}");
                return false;
            }
        }

        public static string MxDBuildSaveHyperlink(bool justLink = false, bool useMarkdown = false, bool useBbCode = false)
        {
            var cData = new CompressionData();
            var str1 = MxDBuildSaveStringShared(ref cData, false, false);
            if (string.IsNullOrEmpty(str1)) return string.Empty;

            var hyperlink = $"https://api.midsreborn.com/legacy/download?uc={cData.SzUncompressed}&c={cData.SzCompressed}&a={cData.SzEncoded}&f=HEX&dc={str1}";

            if (hyperlink.Length > DataLinkMaxLength) return string.Empty;

            if (justLink) return hyperlink;

            if (useMarkdown) return $"[-->Click Here To Download This Legacy Build<--]({hyperlink})";

            return useBbCode ? $"[url={hyperlink}]Click this legacy DataLink to open the build![/url]" : $"<a href=\"{hyperlink}\">Click this legacy DataLink to open the build!</a>";
        }

        private static string MxDBuildSaveStringShared(ref CompressionData cData, bool includeAltEnh, bool breakString)
        {
            var buffer = Array.Empty<byte>();
            if (!MxDBuildSaveBuffer(ref buffer, includeAltEnh))
            {
                return string.Empty;
            }

            cData.SzUncompressed = buffer.Length;
            var compressedBytes = ModernZlib.CompressChunk(buffer);
            cData.SzCompressed = compressedBytes.Length;
            var encodedBytes = ModernZlib.HexEncodeBytes(compressedBytes);
            cData.SzEncoded = encodedBytes.Length;
            var resultString = Encoding.ASCII.GetString(encodedBytes);

            return breakString ? ModernZlib.BreakString(resultString, 67, true) : resultString;
        }

        public static string MxDGenerateByteString(bool includeAltEnh)
        {
            var cData = new CompressionData();
            var saveString = MxDBuildSaveStringShared(ref cData, includeAltEnh, true);
            if (string.IsNullOrEmpty(saveString)) return string.Empty;
            var separator = string.Empty;
            return $"|{MagicCompressed};{cData.SzUncompressed};{cData.SzCompressed};{cData.SzEncoded};HEX;|{separator}{saveString}{saveString}";
        }

        public static string MxDBuildSaveString(bool includeAltEnh, bool forumMode)
        {
            var cData = new CompressionData();
            var str1 = MxDBuildSaveStringShared(ref cData, includeAltEnh, true);

            var str4 = "";

            // Save metadata
            string comment;
            comment = string.IsNullOrWhiteSpace(MidsContext.Character.Comment) ? "" : MidsContext.Character.Comment.Trim();
            if (!string.IsNullOrEmpty(comment))
            {
                str4 += $"\r\n<comment>{EncodeEntities(comment)}</comment>";
            }

            var enhObtainedBin = "";
            var featureUsed = false;
            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe.Power == null)
                {
                    continue;
                }

                for (var j = 0; j < pe.Slots.Length; j++)
                {
                    var obtained = pe.Slots[j].Enhancement.Obtained |
                                   pe.Slots[j].FlippedEnhancement.Obtained;

                    enhObtainedBin += obtained
                        ? '1'
                        : '0';

                    if (obtained)
                    {
                        featureUsed = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(enhObtainedBin) & featureUsed)
            {
                str4 += $"\r\n<enhobtained>{enhObtainedBin}</enhobtained>";
            }

            str4 += "\r\n\r\n\r\n";

            if (string.IsNullOrEmpty(str1)) return string.Empty;

            var str3 = "\n";
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

                // TODO: review this string for update to Mids Reborn
                // needs checking the reader to see if changing this will break anything
                var str5 = "| Copy & Paste this data into Mids Reborn to view the build |" + str3;
                if (flag1)
                    str5 = str5.Replace(" ", "&nbsp;");
                str4 += str5 + "|-------------------------------------------------------------------|" + str3;
            }
            else
            {
                str4 += "|              Do not modify anything below this line!              |" + str3 +
                        "|-------------------------------------------------------------------|" + str3;
            }

            const string str6 = ";HEX";
            var output = $"|{MagicCompressed};{cData.SzUncompressed};{cData.SzCompressed};{cData.SzEncoded}{str6};|{str3}{str1}{str3}";
            return str4 + "|" + MagicCompressed + ";" + cData.SzUncompressed + ";" + cData.SzCompressed + ";" +
                   cData.SzEncoded + str6 + ";|" + str3 + str1 + str3 +
                   "|-------------------------------------------------------------------|";
        }

        private static List<PowerEntry> SortGridPowers(List<PowerEntry> powerList, Enums.eGridType iType)
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

        internal static bool MxDReadSaveData(ref byte[] buffer, bool silent)
        {
            var formatUsed = Formats.Current;
            InherentPowers = new List<PowerEntry>();
            DisplayIndex = -1;
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
                MessageBox.Show($"Unable to read data - {ex.Message}", "ReadSaveData Failed");
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
                        {
                            if (MagicNumber[index] != numArray[index])
                            {
                                magicFound = false;
                            }
                        }

                        if (magicFound)
                        {
                            break;
                        }

                        ++streamIndex;
                    }
                    else
                    {
                        if (!silent)
                        {
                            MessageBox.Show("Unable to read data - Magic Number not found.", "ReadSaveData Failed");
                        }

                        r.Close();
                        memoryStream.Close();
                        return false;
                    }
                } while (!magicFound);

                var fVersion = r.ReadSingle();
                
                switch (fVersion)
                {
                    case > ThisVersion:
                        MessageBox.Show(@"File was saved by a newer version of the application. Please obtain the most recent release in order to open this file.", @"Unable to Load");
                        r.Close();
                        memoryStream.Close();
                        return false;
                    case < PriorVersion:
                        formatUsed = Formats.Legacy;
                        break;
                    case < ThisVersion and >= PriorVersion:
                        formatUsed = Formats.Prior;
                        break;
                    case ThisVersion:
                        formatUsed = Formats.Current;
                        break;
                }

                var qualifiedNames = r.ReadBoolean();
                var hasSubPower = r.ReadBoolean();
                var nIdClass = DatabaseAPI.NidFromUidClass(r.ReadString());
                if (nIdClass < 0)
                {
                    if (!silent)
                    {
                        MessageBox.Show("Unable to read data - Invalid Class UID.", "ReadSaveData Failed");
                    }

                    r.Close();
                    memoryStream.Close();
                    return false;
                }

                var iOrigin = DatabaseAPI.NidFromUidOrigin(r.ReadString(), nIdClass);
                var charClass = DatabaseAPI.Database.Classes[nIdClass];
                if (charClass == null)
                {
                    return false;
                }

                MidsContext.Character.Reset(charClass, iOrigin);
                if (fVersion > 1.0)
                {
                    var align = r.ReadInt32();
                    MidsContext.Character.Alignment = (Enums.Alignment) align;
                }

                MidsContext.Character.Name = r.ReadString();
                var powerSetCount = r.ReadInt32();
                var names = new List<string>();
                for (var index = 0; index < powerSetCount + 1; index++)
                {
                    var iName = r.ReadString();
                    iName = iName switch
                    {
                        "Pool.Leadership_beta" => "Pool.Leadership",

                        // Partial support for builds made with MHD 1.x
                        "Blaster_Support.Atomic_Manipulation" => "Blaster_Support.Radiation_Manipulation",
                        "Pool.Fitness" => "Pool.Invisibility",
                        _ => iName
                    };

                    names.Add(iName);
                }

                var errors = MidsContext.Character.LoadPowersetsByName(names);
                foreach (var (i, n) in errors)
                {
                    MessageBox.Show($"Failed to load powerset by name: {n} at {i} on {DatabaseAPI.DatabaseName} DB version {DatabaseAPI.Database.Version}", "Powerset load failure");
                }

                MidsContext.Character.CurrentBuild.LastPower = r.ReadInt32() - 1;

                var pEntryList = new List<PowerEntry>();
                var powerCount = r.ReadInt32() + 1;
                try
                {
                    for (var powerIndex = 0; powerIndex < powerCount; powerIndex++)
                    {
                        var nId = -1;
                        var name1 = string.Empty;
                        var sidPower1 = -1;
                        if (qualifiedNames)
                        {
                            name1 = r.ReadString();
                            if (!string.IsNullOrEmpty(name1))
                            {
                                nId = DatabaseAPI.NidFromUidPower(name1);
                            }
                        }
                        else
                        {
                            sidPower1 = r.ReadInt32();
                            var newId = DatabaseAPI.Database.ReplTable?.FetchAlternate(sidPower1, charClass.ClassName);
                            if (newId >= 0)
                            {
                                sidPower1 = (int)newId;
                            }

                            nId = DatabaseAPI.NidFromStaticIndexPower(sidPower1);
                        }

                        var flag5 = false;
                        PowerEntry? powerEntry1;
                        if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                        {
                            powerEntry1 = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                        }
                        else
                        {
                            powerEntry1 = new PowerEntry();
                            flag5 = true;
                        }

                        if (powerEntry1 == null)
                        {
                            continue;
                        }

                        if ((sidPower1 > -1) | !string.IsNullOrEmpty(name1))
                        {
                            powerEntry1.Level = r.ReadSByte();
                            switch (formatUsed)
                            {
                                case Formats.Current:
                                    powerEntry1.StatInclude = r.ReadBoolean();
                                    powerEntry1.ProcInclude = r.ReadBoolean();
                                    powerEntry1.VariableValue = r.ReadInt32();
                                    powerEntry1.InherentSlotsUsed = r.ReadInt32();
                                    break;
                                case Formats.Prior:
                                    powerEntry1.StatInclude = r.ReadBoolean();
                                    powerEntry1.ProcInclude = r.ReadBoolean();
                                    powerEntry1.VariableValue = r.ReadInt32();
                                    break;
                                case Formats.Legacy:
                                    powerEntry1.StatInclude = r.ReadBoolean();
                                    powerEntry1.VariableValue = r.ReadInt32();
                                    break;
                            }

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
                                        {
                                            powerSub.nIDPower = DatabaseAPI.NidFromUidPower(name2);
                                        }
                                    }
                                    else
                                    {
                                        var sidPower2 = r.ReadInt32();
                                        powerSub.nIDPower = DatabaseAPI.NidFromStaticIndexPower(sidPower2);
                                    }

                                    var subPower = DatabaseAPI.Database.Power[powerSub.nIDPower];

                                    if (powerSub.nIDPower > -1)
                                    {
                                        if (subPower == null)
                                        {
                                            continue;
                                        }

                                        powerSub.Powerset = subPower.PowerSetID;
                                        powerSub.Power = subPower.PowerSetIndex;
                                    }

                                    powerSub.StatInclude = r.ReadBoolean();
                                    if (!((powerSub.nIDPower > -1) & powerSub.StatInclude))
                                    {
                                        continue;
                                    }

                                    var powerEntry2 = new PowerEntry(DatabaseAPI.Database.Power[powerSub.nIDPower])
                                    {
                                        StatInclude = true
                                    };

                                    MidsContext.Character.CurrentBuild.Powers.Add(powerEntry2);
                                }
                            }
                        }

                        if (nId < 0 && powerIndex < DatabaseAPI.Database.Levels_MainPowers.Length)
                        {
                            powerEntry1.Level = DatabaseAPI.Database.Levels_MainPowers[powerIndex];
                        }

                        powerEntry1.Slots = new SlotEntry[r.ReadSByte() + 1];
                        for (var index3 = 0; index3 < powerEntry1.Slots.Length; index3++)
                        {
                            powerEntry1.Slots[index3] = new SlotEntry
                            {
                                Level = r.ReadSByte(),
                                IsInherent = formatUsed == Formats.Current && r.ReadBoolean(),
                                Enhancement = new I9Slot(),
                                FlippedEnhancement = new I9Slot()
                            };

                            ReadSlotData(r, ref powerEntry1.Slots[index3].Enhancement, qualifiedNames, fVersion);
                            if (r.ReadBoolean())
                            {
                                ReadSlotData(r, ref powerEntry1.Slots[index3].FlippedEnhancement, qualifiedNames,
                                    fVersion);
                            }
                        }

                        if (powerEntry1.SubPowers.Length > 0)
                        {
                            nId = -1;
                        }

                        if (nId <= -1)
                        {
                            continue;
                        }

                        powerEntry1.NIDPower = nId;
                        var power = DatabaseAPI.Database.Power[nId];
                        if (power == null)
                        {
                            continue;
                        }

                        powerEntry1.NIDPowerset = power.PowerSetID;
                        powerEntry1.IDXPower = power.PowerSetIndex;
                        if (powerEntry1.Level == 0 && powerEntry1.Power.FullSetName == "Pool.Fitness")
                        {
                            powerEntry1.NIDPower = powerEntry1.NIDPower switch
                            {
                                2553 => 1521,
                                2554 => 1523,
                                2555 => 1522,
                                2556 => 1524,
                                _ => powerEntry1.NIDPower
                            };

                            powerEntry1.NIDPowerset = power.PowerSetID;
                            powerEntry1.IDXPower = power.PowerSetIndex;
                        }

                        var ps = powerEntry1.Power?.GetPowerSet();
                        if (powerIndex < MidsContext.Character.CurrentBuild.Powers.Count)
                        {
                            var cPower = MidsContext.Character.CurrentBuild.Powers[powerIndex];
                            if (cPower == null)
                            {
                                continue;
                            }

                            if (powerEntry1.Power != null && !(!cPower.Chosen & (ps is { nArchetype: > -1 } || powerEntry1.Power.GroupName == "Pool")))
                            {
                                flag5 = !cPower.Chosen;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (flag5)
                        {
                            if (powerEntry1.Power != null && powerEntry1.Power.InherentType != Enums.eGridType.None)
                            {
                                InherentPowers.Add(powerEntry1);
                            }

                            //Console.WriteLine($"{powerEntry1.Power.DisplayName} - {powerEntry1.Power.InherentType}");
                            //MidsContext.Character.CurrentBuild.Powers.Add(powerEntry1);
                        }
                        else if (powerEntry1.Power != null && (ps is { nArchetype: > -1 } || powerEntry1.Power.GroupName == "Pool"))
                        {
                            MidsContext.Character.CurrentBuild.Powers[powerIndex] = powerEntry1;
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
                        MessageBox.Show($"Error reading some power data with the {DatabaseAPI.DatabaseName} database, will attempt to build character with known data.\r\n{ex.Message}\r\n\r\n{ex.StackTrace}", "ReadSaveData Failed");
                    }

                    return false;
                }

                MidsContext.Archetype = MidsContext.Character.Archetype;
                MidsContext.Character.Validate();
                MidsContext.Character.Lock();
                
                return true;
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show($"Unable to read data - {ex.Message}\r\n\r\n{ex.StackTrace}", "ReadSaveData Failed");
                }

                return false;
            }
        }

        public static eLoadReturnCode MxDExtractAndLoad(Stream? inputStream)
        {
            if (inputStream is null)
            {
                MessageBox.Show("Input stream is null", "ExtractAndLoad Failed");
                return eLoadReturnCode.Failure;
            }

            using var streamReader = new StreamReader(inputStream);
            streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);

            string data;
            try
            {
                data = streamReader.ReadToEnd().Replace("||", "|\n|");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to read data - {ex.Message}", "ExtractAndLoad Failed");
                return eLoadReturnCode.Failure;
            }

            var lines = data.Split('\n');
            string[] headers = { "ABCD", "0", "0", "0" };
            var header = "";
            var dataIndex = FindDataIndex();

            if (dataIndex < 0)
            {
                MessageBox.Show("Unable to locate data header - Magic Number not found!", "ExtractAndLoad Failed");
                return eLoadReturnCode.Failure;
            }

            if (lines.Length <= dataIndex + 1)
            {
                MessageBox.Show("Unable to locate data - Nothing beyond header!", "ExtractAndLoad Failed");
                return eLoadReturnCode.Failure;
            }

            var payload = string.Join("\n", lines[(dataIndex + 1)..]);
            var isHex = headers.Length > 4 && string.Equals(headers[4], "HEX", StringComparison.OrdinalIgnoreCase);
            var bytes =
                new ASCIIEncoding().GetBytes(isHex
                    ? ModernZlib.UnbreakHex(payload)
                    : ModernZlib.UnbreakString(payload, true));

            if (bytes.Length < Convert.ToInt32(headers[3]))
            {
                MessageBox.Show("Data chunk was incomplete! Check that the entire chunk was copied to the clipboard.",
                    "ExtractAndLoad Failed");
                return eLoadReturnCode.Failure;
            }

            if (bytes.Length > Convert.ToInt32(headers[3]))
            {
                Array.Resize(ref bytes, Convert.ToInt32(headers[3]));
            }

            bytes = isHex ? ModernZlib.HexDecodeBytes(bytes) : ModernZlib.UuDecodeBytes(bytes);

            if (bytes.Length == 0)
            {
                return eLoadReturnCode.Failure;
            }

            if (header == MagicCompressed)
            {
                bytes = ModernZlib.DecompressChunk(bytes, Convert.ToInt32(headers[1]));
            }

            return MxDReadSaveData(ref bytes, false) ? eLoadReturnCode.Success : eLoadReturnCode.Failure;

            // Local function to find the data index
            int FindDataIndex()
            {
                for (var index = 0; index < lines.Length; ++index)
                {
                    var line = lines[index];
                    var startIndex = line.IndexOf(MagicUncompressed, StringComparison.Ordinal);
                    if (startIndex < 0)
                        startIndex = line.IndexOf(MagicCompressed, StringComparison.Ordinal);
                    if (startIndex < 0)
                        startIndex = line.IndexOf(Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase);
                    if (startIndex <= -1) continue;
                    headers = line[startIndex..].Split(';');
                    header = headers.Length > 0 ? headers[0] : string.Empty;
                    return index;
                }

                return -1; // Not found
            }
        }

        private static void WriteSlotData(ref BinaryWriter writer, ref I9Slot? slot)
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

        private static void ReadSlotData(BinaryReader reader, ref I9Slot? slot, bool qualifiedNames, float fVersion)
        {
            var num = -1;
            if (qualifiedNames)
            {
                var uidEnh = reader.ReadString();
                if (!string.IsNullOrEmpty(uidEnh))
                {
                    num = DatabaseAPI.NidFromUidEnh(uidEnh);
                }
            }
            else
            {
                num = DatabaseAPI.NidFromStaticIndexEnh(reader.ReadInt32());
            }

            if (num <= -1)
            {
                return;
            }

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
}
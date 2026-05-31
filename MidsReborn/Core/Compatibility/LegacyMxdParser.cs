using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Compatibility
{
    internal enum LegacySaveBinaryFormat
    {
        Current = 0,
        Prior = 1,
        Legacy = 2
    }

    internal sealed class LegacyMxdBuild
    {
        public string LegacyTag { get; init; } = string.Empty;
        public float SaveFormatVersion { get; init; }
        public LegacySaveBinaryFormat SaveFormat { get; init; }
        public bool QualifiedNames { get; init; }
        public bool HasSubPowers { get; init; }
        public string ClassUid { get; init; } = string.Empty;
        public string OriginUid { get; init; } = string.Empty;
        public Enums.Alignment Alignment { get; init; }
        public string CharacterName { get; init; } = string.Empty;
        public List<string> PowerSets { get; init; } = [];
        public int LastPower { get; init; }
        public List<LegacyMxdPowerEntry> PowerEntries { get; init; } = [];
    }

    internal sealed class LegacyMxdPowerEntry
    {
        public int PowerIndex { get; init; }
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public int Level { get; init; }
        public bool StatInclude { get; init; }
        public bool ProcInclude { get; init; }
        public int VariableValue { get; init; }
        public int InherentSlotsUsed { get; init; }
        public List<LegacyMxdSubPowerEntry> SubPowers { get; init; } = [];
        public List<LegacyMxdSlotEntry> Slots { get; init; } = [];
    }

    internal sealed class LegacyMxdSubPowerEntry
    {
        public int SubPowerIndex { get; init; }
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public bool StatInclude { get; init; }
    }

    internal sealed class LegacyMxdSlotEntry
    {
        public int SlotIndex { get; init; }
        public int Level { get; init; }
        public bool IsGranted { get; init; }
        public LegacyMxdEnhancementRef? Enhancement { get; init; }
        public LegacyMxdEnhancementRef? FlippedEnhancement { get; init; }
    }

    internal sealed class LegacyMxdEnhancementRef
    {
        public int? SavedStaticIndex { get; init; }
        public string SavedUid { get; init; } = string.Empty;
        public int? IoLevel { get; init; }
        public int? RelativeLevelRaw { get; init; }
        public int? GradeRaw { get; init; }
    }

    internal static class LegacyMxdParser
    {
        private const string MagicCompressed = "MxDz";
        private const string MagicUncompressed = "MxDu";
        private const string ModernCompressed = "MRBz";
        private const float PriorSaveVersion = 3.10f;
        private const float CurrentSaveVersion = 3.20f;

        private static readonly byte[] MagicNumber =
        {
            Convert.ToByte('M'),
            Convert.ToByte('x'),
            Convert.ToByte('D'),
            Convert.ToByte(12)
        };

        public static bool TryParse(string text, LegacyHomecomingMap compatibilityMap, out LegacyMxdBuild? build, out string error)
        {
            build = null;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                error = "The legacy build file was empty.";
                return false;
            }

            if (!TryParseLegacyTag(text, out var legacyTag))
            {
                error = "The legacy build version could not be determined from the file preamble.";
                return false;
            }

            byte[] payloadBytes;
            try
            {
                payloadBytes = ExtractPayloadBytes(text);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            try
            {
                build = ParsePayloadBytes(payloadBytes, legacyTag, compatibilityMap);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool TryParseLegacyTag(string text, out string tag)
        {
            tag = string.Empty;
            var match = Regex.Match(
                text,
                @"built using\s+Mids(?:'?\s+Reborn)?\s+(?<version>v?\d+(?:\.\d+)+)",
                RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return false;
            }

            tag = match.Groups["version"].Value;
            return !string.IsNullOrWhiteSpace(tag);
        }

        private static LegacyMxdBuild ParsePayloadBytes(byte[] bytes, string legacyTag, LegacyHomecomingMap compatibilityMap)
        {
            using var memoryStream = new MemoryStream(bytes, writable: false);
            using var reader = new BinaryReader(memoryStream, Encoding.UTF8, leaveOpen: false);

            var magicIndex = FindMagicIndex(bytes);
            if (magicIndex < 0)
            {
                throw new InvalidDataException("The legacy payload did not contain the expected magic number.");
            }

            reader.BaseStream.Seek(magicIndex + MagicNumber.Length, SeekOrigin.Begin);
            var saveFormatVersion = reader.ReadSingle();
            var saveFormat = saveFormatVersion switch
            {
                < PriorSaveVersion => LegacySaveBinaryFormat.Legacy,
                < CurrentSaveVersion => LegacySaveBinaryFormat.Prior,
                _ => LegacySaveBinaryFormat.Current
            };

            var qualifiedNames = reader.ReadBoolean();
            var hasSubPowers = reader.ReadBoolean();
            var classUid = reader.ReadString();
            var originUid = reader.ReadString();

            var classIndex = DatabaseAPI.NidFromUidClass(classUid);
            var fallbackAlignment = classIndex >= 0 &&
                                    classIndex < DatabaseAPI.Database.Classes.Length &&
                                    DatabaseAPI.Database.Classes[classIndex]?.Hero == true
                ? Enums.Alignment.Hero
                : Enums.Alignment.Villain;

            var alignment = saveFormatVersion > 1.0f
                ? (Enums.Alignment)reader.ReadInt32()
                : fallbackAlignment;

            var characterName = reader.ReadString();
            var powerSetCount = reader.ReadInt32() + 1;
            var powerSets = new List<string>(powerSetCount);
            for (var index = 0; index < powerSetCount; index++)
            {
                powerSets.Add(reader.ReadString());
            }

            var lastPower = reader.ReadInt32();
            var powerEntryCount = reader.ReadInt32() + 1;
            var powerEntries = new List<LegacyMxdPowerEntry>(Math.Max(powerEntryCount, 0));

            for (var powerIndex = 0; powerIndex < powerEntryCount; powerIndex++)
            {
                int? savedStaticIndex = null;
                var savedUid = string.Empty;
                if (qualifiedNames)
                {
                    savedUid = reader.ReadString();
                }
                else
                {
                    var rawStaticIndex = reader.ReadInt32();
                    if (rawStaticIndex >= 0)
                    {
                        savedStaticIndex = rawStaticIndex;
                    }
                }

                var hasIdentity = savedStaticIndex.HasValue || !string.IsNullOrWhiteSpace(savedUid);
                var level = 0;
                var statInclude = false;
                var procInclude = false;
                var variableValue = 0;
                var inherentSlotsUsed = 0;
                var subPowers = new List<LegacyMxdSubPowerEntry>();

                if (hasIdentity)
                {
                    level = reader.ReadSByte();
                    switch (saveFormat)
                    {
                        case LegacySaveBinaryFormat.Current:
                            statInclude = reader.ReadBoolean();
                            procInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            inherentSlotsUsed = reader.ReadInt32();
                            break;
                        case LegacySaveBinaryFormat.Prior:
                            statInclude = reader.ReadBoolean();
                            procInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            break;
                        case LegacySaveBinaryFormat.Legacy:
                            statInclude = reader.ReadBoolean();
                            variableValue = reader.ReadInt32();
                            break;
                    }

                    if (hasSubPowers)
                    {
                        var subPowerCount = reader.ReadSByte() + 1;
                        subPowers = new List<LegacyMxdSubPowerEntry>(Math.Max(subPowerCount, 0));
                        for (var subPowerIndex = 0; subPowerIndex < subPowerCount; subPowerIndex++)
                        {
                            int? subPowerStaticIndex = null;
                            var subPowerUid = string.Empty;
                            if (qualifiedNames)
                            {
                                subPowerUid = reader.ReadString();
                            }
                            else
                            {
                                var rawSubPowerStaticIndex = reader.ReadInt32();
                                if (rawSubPowerStaticIndex >= 0)
                                {
                                    subPowerStaticIndex = rawSubPowerStaticIndex;
                                }
                            }

                            subPowers.Add(new LegacyMxdSubPowerEntry
                            {
                                SubPowerIndex = subPowerIndex,
                                SavedStaticIndex = subPowerStaticIndex,
                                SavedUid = subPowerUid,
                                StatInclude = reader.ReadBoolean()
                            });
                        }
                    }
                }

                var slotCount = reader.ReadSByte() + 1;
                var slots = new List<LegacyMxdSlotEntry>(Math.Max(slotCount, 0));
                for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
                {
                    var slotLevel = reader.ReadSByte();
                    var granted = saveFormat == LegacySaveBinaryFormat.Current && reader.ReadBoolean();
                    var enhancement = ReadEnhancement(reader, qualifiedNames, saveFormatVersion, compatibilityMap, legacyTag);
                    var hasFlipped = reader.ReadBoolean();
                    var flippedEnhancement = hasFlipped
                        ? ReadEnhancement(reader, qualifiedNames, saveFormatVersion, compatibilityMap, legacyTag)
                        : null;

                    slots.Add(new LegacyMxdSlotEntry
                    {
                        SlotIndex = slotIndex,
                        Level = slotLevel,
                        IsGranted = granted,
                        Enhancement = enhancement,
                        FlippedEnhancement = flippedEnhancement
                    });
                }

                powerEntries.Add(new LegacyMxdPowerEntry
                {
                    PowerIndex = powerIndex,
                    SavedStaticIndex = savedStaticIndex,
                    SavedUid = savedUid,
                    Level = level,
                    StatInclude = statInclude,
                    ProcInclude = procInclude,
                    VariableValue = variableValue,
                    InherentSlotsUsed = inherentSlotsUsed,
                    SubPowers = subPowers,
                    Slots = slots
                });
            }

            return new LegacyMxdBuild
            {
                LegacyTag = legacyTag,
                SaveFormatVersion = saveFormatVersion,
                SaveFormat = saveFormat,
                QualifiedNames = qualifiedNames,
                HasSubPowers = hasSubPowers,
                ClassUid = classUid,
                OriginUid = originUid,
                Alignment = alignment,
                CharacterName = characterName,
                PowerSets = powerSets,
                LastPower = lastPower,
                PowerEntries = powerEntries
            };
        }

        private static LegacyMxdEnhancementRef? ReadEnhancement(
            BinaryReader reader,
            bool qualifiedNames,
            float saveFormatVersion,
            LegacyHomecomingMap compatibilityMap,
            string legacyTag)
        {
            int? staticIndex = null;
            var savedUid = string.Empty;
            var encoding = LegacyEnhancementEncoding.None;
            if (qualifiedNames)
            {
                savedUid = reader.ReadString();
                if (string.IsNullOrWhiteSpace(savedUid))
                {
                    return null;
                }

                if (!LegacyMapResolver.TryCanonicalizeCurrentEnhancement(savedUid, out var currentEnhancementUid))
                {
                    var candidates = compatibilityMap.FindEnhancementEntries(savedUid, ExtractLeafName(savedUid));
                    var currentTargets = candidates
                        .Select(candidate => candidate.CurrentTarget)
                        .Where(target => !string.IsNullOrWhiteSpace(target))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                    if (currentTargets.Length != 1)
                    {
                        throw new InvalidDataException(
                            $"The qualified-name legacy enhancement reference '{savedUid}' could not be resolved.");
                    }

                    currentEnhancementUid = currentTargets[0];
                }

                var enhancementId = DatabaseAPI.NidFromUidEnh(currentEnhancementUid);
                if (enhancementId < 0)
                {
                    throw new InvalidDataException(
                        $"The qualified-name legacy enhancement reference '{savedUid}' targets unknown enhancement '{currentEnhancementUid}'.");
                }

                encoding = DatabaseAPI.Database.Enhancements[enhancementId].TypeID switch
                {
                    Enums.eType.Normal => LegacyEnhancementEncoding.NormalLike,
                    Enums.eType.SpecialO => LegacyEnhancementEncoding.NormalLike,
                    Enums.eType.InventO => LegacyEnhancementEncoding.InventLike,
                    Enums.eType.SetO => LegacyEnhancementEncoding.InventLike,
                    _ => LegacyEnhancementEncoding.None
                };
            }
            else
            {
                var rawStaticIndex = reader.ReadInt32();
                if (rawStaticIndex < 0)
                {
                    return null;
                }

                staticIndex = rawStaticIndex;
                if (!compatibilityMap.TryGetEnhancementEncoding(rawStaticIndex, out encoding))
                {
                    throw new InvalidDataException(
                        $"The legacy enhancement static index {rawStaticIndex} could not be resolved for Homecoming {legacyTag}.");
                }
            }

            int? ioLevel = null;
            int? relativeLevel = null;
            int? grade = null;

            if (!qualifiedNames && staticIndex.HasValue)
            {
                switch (encoding)
                {
                    case LegacyEnhancementEncoding.NormalLike:
                        relativeLevel = reader.ReadSByte();
                        grade = reader.ReadSByte();
                        break;
                    case LegacyEnhancementEncoding.InventLike:
                        ioLevel = reader.ReadSByte();
                        if (saveFormatVersion > 1.0f)
                        {
                            relativeLevel = reader.ReadSByte();
                        }
                        break;
                }
            }

            return new LegacyMxdEnhancementRef
            {
                SavedStaticIndex = staticIndex,
                SavedUid = savedUid,
                IoLevel = ioLevel,
                RelativeLevelRaw = relativeLevel,
                GradeRaw = grade
            };
        }

        private static string ExtractLeafName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var trimmed = value.Trim();
            var lastDot = trimmed.LastIndexOf('.');
            return lastDot >= 0 && lastDot < trimmed.Length - 1
                ? trimmed[(lastDot + 1)..]
                : trimmed;
        }

        private static byte[] ExtractPayloadBytes(string text)
        {
            var normalizedText = text.Replace("||", "|\n|");
            var lines = normalizedText.Split('\n');
            string[] headers = ["ABCD", "0", "0", "0"];
            var header = string.Empty;
            var dataIndex = -1;

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var startIndex = line.IndexOf(MagicUncompressed, StringComparison.Ordinal);
                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(MagicCompressed, StringComparison.Ordinal);
                }

                if (startIndex < 0)
                {
                    startIndex = line.IndexOf(ModernCompressed, StringComparison.OrdinalIgnoreCase);
                }

                if (startIndex < 0)
                {
                    continue;
                }

                headers = line[startIndex..].Split(';', StringSplitOptions.None);
                header = headers.Length > 0 ? headers[0] : string.Empty;
                dataIndex = index;
                break;
            }

            if (dataIndex < 0)
            {
                throw new InvalidDataException("Could not locate the legacy MxD payload header.");
            }

            if (lines.Length <= dataIndex + 1)
            {
                throw new InvalidDataException("The legacy MxD payload body was empty.");
            }

            var payload = string.Join("\n", lines[(dataIndex + 1)..]);
            var isHex = headers.Length > 4 && string.Equals(headers[4], "HEX", StringComparison.OrdinalIgnoreCase);
            var encodedBytes = Encoding.ASCII.GetBytes(isHex
                ? UnbreakHex(payload)
                : UnbreakString(payload, true));

            var encodedSize = Convert.ToInt32(headers[3], CultureInfo.InvariantCulture);
            if (encodedBytes.Length < encodedSize)
            {
                throw new InvalidDataException("The encoded legacy MxD payload was truncated.");
            }

            if (encodedBytes.Length > encodedSize)
            {
                Array.Resize(ref encodedBytes, encodedSize);
            }

            var rawBytes = isHex ? HexDecodeBytes(encodedBytes) : UuDecodeBytes(encodedBytes);
            if (string.Equals(header, MagicCompressed, StringComparison.Ordinal) ||
                string.Equals(header, ModernCompressed, StringComparison.OrdinalIgnoreCase))
            {
                var uncompressedSize = Convert.ToInt32(headers[1], CultureInfo.InvariantCulture);
                rawBytes = DecompressChunk(rawBytes, uncompressedSize);
            }

            return rawBytes;
        }

        private static int FindMagicIndex(IReadOnlyList<byte> bytes)
        {
            for (var index = 0; index <= bytes.Count - MagicNumber.Length; index++)
            {
                var matched = true;
                for (var offset = 0; offset < MagicNumber.Length; offset++)
                {
                    if (bytes[index + offset] == MagicNumber[offset])
                    {
                        continue;
                    }

                    matched = false;
                    break;
                }

                if (matched)
                {
                    return index;
                }
            }

            return -1;
        }

        private static byte[] HexDecodeBytes(byte[] inputBytes)
        {
            var hexString = Encoding.ASCII.GetString(inputBytes);
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        private static byte[] UuDecodeBytes(byte[] inputBytes)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            for (var index = 0; index < inputBytes.Length; index += 4)
            {
                var bytes = inputBytes
                    .Skip(index)
                    .Take(4)
                    .Select(b => b == 96 ? (byte)32 : b)
                    .ToArray();

                var byte1 = ((bytes[0] - 32) << 2) | ((bytes[1] - 32) >> 4);
                var byte2 = ((bytes[1] - 32) & 0xF) << 4 | ((bytes[2] - 32) >> 2);
                var byte3 = ((bytes[2] - 32) & 0x3) << 6 | (bytes[3] - 32);

                writer.Write((byte)byte1);
                writer.Write((byte)byte2);
                writer.Write((byte)byte3);
            }

            return memoryStream.ToArray();
        }

        private static byte[] DecompressChunk(byte[] inputBytes, int expectedLength)
        {
            using var inputStream = new MemoryStream(inputBytes);
            using var zlibStream = new ZLibStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            zlibStream.CopyTo(outputStream);
            var output = outputStream.ToArray();
            Array.Resize(ref output, expectedLength);
            return output;
        }

        private static string UnbreakHex(string inputString)
        {
            var builder = new StringBuilder(inputString.Length);
            foreach (var ch in inputString)
            {
                if (ch is >= 'A' and <= 'Z' or >= '0' and <= '9')
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private static string UnbreakString(string inputString, bool bookend)
        {
            if (bookend)
            {
                inputString = string.Join(
                    Environment.NewLine,
                    inputString
                        .Split([Environment.NewLine], StringSplitOptions.None)
                        .Select(line => line.Trim('|')));
            }

            return inputString.Replace(Environment.NewLine, string.Empty);
        }
    }
}

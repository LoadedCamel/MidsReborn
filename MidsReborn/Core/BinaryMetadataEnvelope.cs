using System;
using System.IO;
using System.Text;

namespace Mids_Reborn.Core;

internal static class BinaryMetadataEnvelope
{
    private static readonly UTF8Encoding MarkerEncoding = new(false, true);

    public static void Write(BinaryWriter writer, string marker, int version, Action<BinaryWriter> writePayload)
    {
        using var payloadStream = new MemoryStream();
        using (var payloadWriter = new BinaryWriter(payloadStream, MarkerEncoding, leaveOpen: true))
        {
            writePayload(payloadWriter);
            payloadWriter.Flush();
        }

        writer.Write(marker);
        writer.Write(version);
        writer.Write(checked((int)payloadStream.Length));
        writer.Write(payloadStream.GetBuffer(), 0, checked((int)payloadStream.Length));
    }

    public static bool TryRead(BinaryReader reader, string marker, int maxSupportedVersion, Action<int, BinaryReader> readPayload)
    {
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryConsumeMarker(reader, marker))
            {
                return false;
            }

            var version = reader.ReadInt32();
            if (version < 1 || version > maxSupportedVersion)
            {
                throw new InvalidDataException($"Unsupported metadata envelope version {version} for {marker}.");
            }

            var payloadLength = reader.ReadInt32();
            if (payloadLength < 0)
            {
                throw new InvalidDataException($"Negative metadata payload length {payloadLength} for {marker}.");
            }

            if (reader.BaseStream.CanSeek && reader.BaseStream.Length - reader.BaseStream.Position < payloadLength)
            {
                throw new EndOfStreamException($"Metadata payload for {marker} extends past the end of the stream.");
            }

            var payload = reader.ReadBytes(payloadLength);
            if (payload.Length != payloadLength)
            {
                throw new EndOfStreamException($"Metadata payload for {marker} truncated while reading {payloadLength} bytes.");
            }

            using var payloadStream = new MemoryStream(payload, writable: false);
            using var payloadReader = new BinaryReader(payloadStream, MarkerEncoding, leaveOpen: false);
            readPayload(version, payloadReader);
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            return false;
        }
    }

    public static bool TryConsumeMarker(BinaryReader reader, string marker)
    {
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryPeekExactMarker(reader.BaseStream, marker, out var bytesConsumed))
            {
                reader.BaseStream.Position = position;
                return false;
            }

            reader.BaseStream.Position = position + bytesConsumed;
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            return false;
        }
    }

    public static bool TryReadMarkedString(BinaryReader reader, string marker, out string value)
    {
        value = string.Empty;
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryConsumeMarker(reader, marker))
            {
                return false;
            }

            value = reader.ReadString();
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            value = string.Empty;
            return false;
        }
    }

    public static bool TryReadMarkedSingle(BinaryReader reader, string marker, out float value)
    {
        value = 0f;
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryConsumeMarker(reader, marker))
            {
                return false;
            }

            value = reader.ReadSingle();
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            value = 0f;
            return false;
        }
    }

    public static bool TryReadMarkedBoolean(BinaryReader reader, string marker, out bool value)
    {
        value = false;
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryConsumeMarker(reader, marker))
            {
                return false;
            }

            value = reader.ReadBoolean();
            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            value = false;
            return false;
        }
    }

    public static bool TryReadMarkedStringArray(BinaryReader reader, string marker, out string[] values)
    {
        values = [];
        if (!reader.BaseStream.CanSeek)
        {
            return false;
        }

        var position = reader.BaseStream.Position;
        try
        {
            if (!TryConsumeMarker(reader, marker))
            {
                return false;
            }

            var count = reader.ReadInt32();
            if (count <= 0)
            {
                values = [];
                return true;
            }

            values = new string[count];
            for (var index = 0; index < count; index++)
            {
                values[index] = reader.ReadString();
            }

            return true;
        }
        catch
        {
            reader.BaseStream.Position = position;
            values = [];
            return false;
        }
    }

    private static bool TryPeekExactMarker(Stream stream, string marker, out long bytesConsumed)
    {
        bytesConsumed = 0;
        if (!stream.CanSeek)
        {
            return false;
        }

        var position = stream.Position;
        try
        {
            if (!TryRead7BitEncodedInt(stream, out var markerByteCount, out var prefixBytesRead))
            {
                return false;
            }

            var expectedBytes = MarkerEncoding.GetBytes(marker);
            var expectedByteCount = expectedBytes.Length;
            if (markerByteCount != expectedByteCount || markerByteCount < 0)
            {
                return false;
            }

            var buffer = new byte[markerByteCount];
            if (!TryReadExact(stream, buffer, markerByteCount))
            {
                return false;
            }

            bytesConsumed = prefixBytesRead + markerByteCount;
            for (var index = 0; index < expectedByteCount; index++)
            {
                if (buffer[index] != expectedBytes[index])
                {
                    return false;
                }
            }

            return true;
        }
        finally
        {
            stream.Position = position;
        }
    }

    private static bool TryRead7BitEncodedInt(Stream stream, out int value, out int bytesRead)
    {
        value = 0;
        bytesRead = 0;
        var shift = 0;
        while (shift < 35)
        {
            var next = stream.ReadByte();
            if (next < 0)
            {
                value = 0;
                bytesRead = 0;
                return false;
            }

            bytesRead++;
            value |= (next & 0x7F) << shift;
            if ((next & 0x80) == 0)
            {
                return true;
            }

            shift += 7;
        }

        throw new FormatException("Invalid 7-bit encoded integer while reading metadata marker.");
    }

    private static bool TryReadExact(Stream stream, byte[] buffer, int count)
    {
        var offset = 0;
        while (offset < count)
        {
            var read = stream.Read(buffer, offset, count - offset);
            if (read <= 0)
            {
                return false;
            }

            offset += read;
        }

        return true;
    }
}

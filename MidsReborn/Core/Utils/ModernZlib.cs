using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ComponentAce.Compression.Libs.zlib;

namespace Mids_Reborn.Core.Utils
{
    public static class ModernZlib
    {
        // Compression and Decompression using Managed Zlib
        public static byte[] CompressChunk(byte[] inputBytes)
        {
            using var inputMemoryStream = new MemoryStream(inputBytes);
            using var outputMemoryStream = new MemoryStream();
            using (var compressor = new ZOutputStream(outputMemoryStream, zlibConst.Z_BEST_COMPRESSION))
            {
                inputMemoryStream.CopyTo(compressor);
                compressor.finish();  // Ensure the compression is finalized
            }
            return outputMemoryStream.ToArray();
        }

        public static byte[] DecompressChunk(byte[] inputBytes, int destLength)
        {
            using var inputMemoryStream = new MemoryStream(inputBytes);
            using var outputMemoryStream = new MemoryStream();
            using (var decompressor = new ZInputStream(inputMemoryStream))
            {
                var buffer = new byte[4096]; // Adjust the buffer size as needed
                int bytesRead;
                while ((bytesRead = decompressor.read(buffer, 0, buffer.Length)) > 0)
                {
                    outputMemoryStream.Write(buffer, 0, bytesRead);
                }
            }

            var outputArray = outputMemoryStream.ToArray();
            Array.Resize(ref outputArray, destLength);
            return outputArray;
        }

        // Encoding and Decoding
        public static byte[] UuDecodeBytes(byte[] inputBytes)
        {
            // Use MemoryStream within using block for proper disposal
            using var memoryStream = new MemoryStream();
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                for (var index = 0; index < inputBytes.Length; index += 4)
                {
                    // Process a group of 4 bytes
                    var bytes = inputBytes.Skip(index).Take(4).Select(b => b == 96 ? 32 : b).ToArray();

                    // Decode the group of 4 bytes into 3 bytes
                    var byte1 = ((bytes[0] - 32) << 2) | ((bytes[1] - 32) >> 4);
                    var byte2 = ((bytes[1] - 32) & 0xF) << 4 | ((bytes[2] - 32) >> 2);
                    var byte3 = ((bytes[2] - 32) & 0x3) << 6 | (bytes[3] - 32);

                    binaryWriter.Write((byte)byte1);
                    binaryWriter.Write((byte)byte2);
                    binaryWriter.Write((byte)byte3);
                }
            }

            // Return the decoded bytes
            return memoryStream.ToArray();
        }

        public static byte[] HexDecodeBytes(byte[] inputBytes)
        {
            // Convert inputBytes to hexString
            var hexString = Encoding.ASCII.GetString(inputBytes);
            // Calculate the number of bytes in the resulting array
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                // Convert each pair of characters (each hex byte) to a byte
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        public static byte[] HexEncodeBytes(byte[] inputBytes)
        {
            var hex = new StringBuilder(inputBytes.Length * 2);
            foreach (var b in inputBytes)
            {
                hex.AppendFormat("{0:X2}", b);
            }
            return Encoding.UTF8.GetBytes(hex.ToString());
        }

        // String Manipulation
        public static string BreakString(string inputString, int length, bool bookend = false)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < inputString.Length; i += length)
            {
                if (bookend)
                {
                    stringBuilder.Append("|");
                }

                stringBuilder.Append(inputString.Substring(i, Math.Min(length, inputString.Length - i)));

                if (bookend)
                {
                    stringBuilder.Append("|");
                }

                // Append a newline character if it's not the last line
                if (i + length < inputString.Length)
                {
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }

        public static string UnbreakHex(string inputString)
        {
            var stringBuilder = new StringBuilder();

            foreach (var ch in inputString)
            {
                if (ch is >= 'A' and <= 'Z' or >= '0' and <= '9')
                {
                    stringBuilder.Append(ch);
                }
            }

            return stringBuilder.ToString();
        }

        public static string UnbreakString(string inputString, bool bookend = false)
        {
            // If bookend is true, remove the '|' characters at the beginning and end of each line
            if (bookend)
            {
                // Split the input string by lines, trim the '|' characters and rejoin
                inputString = string.Join(Environment.NewLine,
                    inputString.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                        .Select(line => line.Trim('|')));
            }
    
            // Remove newline characters to reverse the 'AppendLine' used in 'BreakString'
            return inputString.Replace(Environment.NewLine, string.Empty);
        }
    }
}

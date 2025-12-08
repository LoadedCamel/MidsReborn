using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Mids_Reborn.Core.Utils
{

    internal static class Compression
    {

        public static CompressionResult CompressToBase64(byte[] sourceBytes)
        {
            using var uncompressedStream = new MemoryStream(sourceBytes);
            using var compressedStream = new MemoryStream();
            using (var compressionStream = new BrotliStream(compressedStream, CompressionLevel.SmallestSize))
            {
                compressionStream.Write(sourceBytes, 0, sourceBytes.Length);
            }

            var compressedBytes = compressedStream.ToArray();
            var base64String = Convert.ToBase64String(compressedBytes);

            return new CompressionResult(base64String, sourceBytes.Length, compressedBytes.Length, base64String.Length);
        }

        public static CompressionResult DecompressFromBase64(string base64String)
        {
            var compressedBytes = Convert.FromBase64String(base64String);
            using var compressedStream = new MemoryStream(compressedBytes);
            using var decompressionStream = new BrotliStream(compressedStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            decompressionStream.CopyTo(decompressedStream);
            var decompressedBytes = decompressedStream.ToArray();
            var outString = Encoding.UTF8.GetString(decompressedBytes);
            return new CompressionResult(outString, decompressedBytes.Length, compressedBytes.Length, base64String.Length);
        }
    }
}

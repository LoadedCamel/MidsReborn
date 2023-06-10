using System;
using System.IO;
using System.IO.Compression;

namespace Mids_Reborn.Core.Utils
{
    internal static class Compression
    {
        public static byte[] Compress(byte[] sourceBytes)
        {
            using var stream = new MemoryStream();
            using (var compressionStream = new BrotliStream(stream, CompressionLevel.SmallestSize))
            {
                compressionStream.Write(sourceBytes, 0, sourceBytes.Length);
            }
            return stream.ToArray();
        }

        public static byte[] Decompress(byte[] sourceBytes)
        {
            using var inputStream = new MemoryStream(sourceBytes);
            using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            compressionStream.CopyTo(outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream.ToArray();
        }

        public static string CompressToBase64(byte[] sourceBytes)
        {
            using var stream = new MemoryStream();
            using (var compressionStream = new BrotliStream(stream, CompressionLevel.SmallestSize))
            {
                compressionStream.Write(sourceBytes, 0, sourceBytes.Length);
            }

            return Convert.ToBase64String(stream.ToArray());
        }

        public static byte[] DecompressFromBase64(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            using var inputStream = new MemoryStream(bytes);
            using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            compressionStream.CopyTo(outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream.ToArray();
        }

        public static string CompressToBase64Url(byte[] sourceBytes)
        {
            using var stream = new MemoryStream();
            using (var compressionStream = new BrotliStream(stream, CompressionLevel.SmallestSize))
            {
                compressionStream.Write(sourceBytes, 0, sourceBytes.Length);
            }

            var bytes = stream.ToArray();
            return Base64UrlEncoder.Encode(bytes);
        }

        public static byte[] DecompressFromBase64Url(string base64)
        {
            var bytes = Base64UrlEncoder.DecodeBytes(base64);
            using var inputStream = new MemoryStream(bytes);
            using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            compressionStream.CopyTo(outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream.ToArray();
        }
    }
}

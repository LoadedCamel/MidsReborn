using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mids_Reborn.Core.Utils
{
    public delegate void CompressionProgressEventHandler(ProgressEventArgs e);
    public delegate void StreamProgressEventHandler(ProgressEventArgs e);

    internal static class Compression
    {
        public static event CompressionProgressEventHandler? CompressionProgress;
        public static event StreamProgressEventHandler? StreamProgress;

        public static async Task CompressToArchive(string filePath, IDictionary<string, byte[]> fileDictionary, CancellationToken cancellationToken = default)
        {
            const int chunkSize = 1024;
            var totalBytes = fileDictionary.Values.Sum(byteArray => byteArray.Length);
            await using var fileStream = new ProgressFileStream(filePath, FileMode.Create);
            await using var compressionStream = new ProgressZipStream(fileStream);
            compressionStream.SetLevel(9);
            compressionStream.UseZip64 = UseZip64.Dynamic;
            foreach (var vp in fileDictionary)
            {
                var zipEntry = new ZipEntry(vp.Key)
                {
                    DateTime = DateTime.UtcNow,
                    Size = vp.Value.Length
                };
                CompressionProgress?.Invoke(new ProgressEventArgs(0, vp.Value.Length));
                await compressionStream.PutNextEntryAsync(zipEntry, cancellationToken);
                for (var index = 0; index < vp.Value.Length; index += chunkSize)
                {
                    var chunk = Math.Min(chunkSize, vp.Value.Length - index);
                    await compressionStream.WriteAsync(vp.Value, index, chunk, cancellationToken);
                    CompressionProgress?.Invoke(new ProgressEventArgs(compressionStream.Progress, vp.Value.Length));
                    StreamProgress?.Invoke(new ProgressEventArgs(fileStream.Progress, totalBytes));
                }
            }
        }

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

        public static CompressionResult ZCompress(byte[] sourceBytes)
        {
            var compressedBytes = ModernZlib.CompressChunk(sourceBytes);
            var hexEncodedBytes = ModernZlib.HexEncodeBytes(compressedBytes);
            var hexString = Encoding.UTF8.GetString(hexEncodedBytes);
            var brokenHexString = ModernZlib.BreakString(hexString, 67, true);
            return new CompressionResult(brokenHexString, sourceBytes.Length, compressedBytes.Length, hexString.Length);
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

        // public static byte[] DecompressFromBase64(string base64)
        // {
        //     var bytes = Convert.FromBase64String(base64);
        //     using var inputStream = new MemoryStream(bytes);
        //     using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
        //     using var outputStream = new MemoryStream();
        //     compressionStream.CopyTo(outputStream);
        //     outputStream.Seek(0, SeekOrigin.Begin);
        //     return outputStream.ToArray();
        // }

        public static CompressionResult CompressToBase64Url(byte[] sourceBytes)
        {
            using var uncompressedStream = new MemoryStream(sourceBytes);
            using var compressedStream = new MemoryStream();
            using (var compressionStream = new BrotliStream(compressedStream, CompressionLevel.SmallestSize))
            {
                compressionStream.Write(sourceBytes, 0, sourceBytes.Length);
            }

            var compressedBytes = compressedStream.ToArray();
            var base64String = Base64UrlEncoder.Encode(compressedBytes);

            return new CompressionResult(base64String, sourceBytes.Length, compressedBytes.Length, base64String.Length);
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

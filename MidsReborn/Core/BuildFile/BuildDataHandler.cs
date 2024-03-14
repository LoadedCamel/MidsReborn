using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace Mids_Reborn.Core.BuildFile
{
    public static class BuildDataHandler
    {
        // Serialize without compression a CharacterBuildFile instance to a string
        public static string SerializeForFile(CharacterBuildData buildData)
        {
            var jsonString = JsonConvert.SerializeObject(buildData, Formatting.Indented);
            return jsonString;
        }

        // public static CharacterBuildData DeserializeFromFile(string file)
        // {
        //     return;
        // }

        // Serialize and compress a CharacterBuildFile instance to a string
        public static string SerializeForShare(CharacterBuildData buildFile)
        {
            var jsonString = JsonConvert.SerializeObject(buildFile, Formatting.None);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            using var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                gZipStream.Write(jsonBytes, 0, jsonBytes.Length);
            }
            return Convert.ToBase64String(outputStream.ToArray());
        }

        // Decompress and deserialize a string back to a CharacterBuildFile instance
        public static CharacterBuildData DeserializeFromShare(string compressedData)
        {
            var dataBytes = Convert.FromBase64String(compressedData);
            using var inputStream = new MemoryStream(dataBytes);
            using var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gZipStream.CopyTo(outputStream);
            var decompressedBytes = outputStream.ToArray();
            var jsonString = Encoding.UTF8.GetString(decompressedBytes);
            return JsonConvert.DeserializeObject<CharacterBuildData>(jsonString) ?? throw new InvalidOperationException();
        }
    }
}

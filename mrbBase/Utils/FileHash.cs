#nullable enable
using System;
using System.IO;
using System.Security.Cryptography;

namespace mrbBase.Utils
{
    public class FileHash
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string Hash { get; set; }

        public FileHash(string directory, string fileName, string hash)
        {
            Directory = directory;
            FileName = fileName;
            Hash = hash;
        }

        public static string ComputeHash(string file)
        {
            var byteData = File.ReadAllBytes(file);
            using var hasher = SHA256.Create();
            var hashResult = hasher.ComputeHash(byteData);
            return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
        }

        public static bool Compare(string incomingHash, string existingHash) => incomingHash.Equals(existingHash);
    }
}

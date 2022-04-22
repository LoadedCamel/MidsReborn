#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

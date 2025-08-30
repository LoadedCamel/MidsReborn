using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Mids_Reborn.Core
{
    public static class SlythinDpsToolRunner
    {
        public const string ExeName = "Sythlin_DPS_Tool.exe";
        // At your own discretion if changed.
        private const string FileHash = "156a4e4544363b747bf55747ca36bdb7c3ec3332285c25681a033218ed990cd5";

        private static string ExeFullPath => Path.Combine(AppContext.BaseDirectory, ExeName);

        public static bool FileExists()
        {
            return File.Exists(ExeFullPath);
        }

        public static bool HashMatch(bool checkFileExists = false)
        {
            if (!FileExists())
            {
                return false;
            }

            using var stream = new BufferedStream(File.OpenRead(ExeFullPath), 1200000);
            var sha = SHA256.Create();
            var checksum = sha.ComputeHash(stream);
            var hashText = BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();

            return hashText == FileHash;
        }

        public static void Run(bool checkRunnable = false)
        {
            if (checkRunnable)
            {
                if (!HashMatch(true))
                {
                    return;
                }
            }

            var psi = new ProcessStartInfo
            {
                FileName = ExeFullPath,
                Arguments = "",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}

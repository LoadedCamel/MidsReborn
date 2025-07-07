using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mids_Reborn
{
    internal static class StrapUpdater
    {
        private const string NewPrefix = "New_MRBBootstrap";
        private const string FinalPrefix = "MRBBootstrap";

        internal static void Run()
        {
            var baseDir = AppContext.BaseDirectory;

            if (IsProcessRunning(FinalPrefix))
                return;

            var newFiles = Directory.GetFiles(baseDir, $"{NewPrefix}*");

            foreach (var sourcePath in newFiles)
            {
                string fileName = Path.GetFileName(sourcePath);

                if (!fileName.StartsWith(NewPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                string suffix = fileName.Substring(NewPrefix.Length); // everything after "New_MRBBootstrap"
                string targetPath = Path.Combine(baseDir, FinalPrefix + suffix);

                try
                {
                    if (IsFileLocked(targetPath))
                        continue;

                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                    File.Move(sourcePath, targetPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[StrapUpdater] Failed to move {sourcePath} → {targetPath}: {ex.Message}");
                }
            }
        }

        private static bool IsProcessRunning(string baseName)
        {
            try
            {
                return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(baseName)).Any();
            }
            catch
            {
                return true;
            }
        }

        private static bool IsFileLocked(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}

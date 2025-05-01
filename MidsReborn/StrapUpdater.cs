using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mids_Reborn
{
    internal static class StrapUpdater
    {
        private const string NewBootstrapName = "New_MRBBootstrap.exe";
        private const string OldBootstrapName = "MRBBootstrap.exe";

        internal static void Run()
        {
            var newBootstrapPath = Path.Combine(AppContext.BaseDirectory, NewBootstrapName);
            var oldBootstrapPath = Path.Combine(AppContext.BaseDirectory, OldBootstrapName);

            if (!File.Exists(newBootstrapPath)) return;

            if (IsProcessRunning(Path.GetFileNameWithoutExtension(OldBootstrapName)))
            {
                return;
            }

            try
            {
                if (File.Exists(oldBootstrapPath))
                {
                    File.Delete(oldBootstrapPath);
                }

                File.Move(newBootstrapPath, oldBootstrapPath);
            }
            catch
            {
                // ignored
            }
        }

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }
    }
}

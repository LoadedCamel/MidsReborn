using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.UpdateSystem.Models;
using MRBLogging;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public static class UpdateCoordinator
    {
        public static async Task<bool> CheckAndHandleUpdatesAsync(IWin32Window parent, bool startupCheck = false, bool honorDelay = false, ILogger? logger = null)
        {
            if (honorDelay && !IsUpdateCheckDue()) { return false; }
            logger?.Info("[UpdateCoordinator] Starting update check...");

            var result = await UpdateUtils.CheckForUpdatesAsync(honorDelay);
            if (result is { IsAppUpdateAvailable: false, IsDbUpdateAvailable: false})
            {
                logger?.Info("[UpdateCoordinator] No updates available.");
                if (startupCheck)
                {
                    var updateMsg = new MessageBoxEx(@"Update", @"No updates available.",
                        MessageBoxEx.MessageBoxExButtons.Ok);
                    updateMsg.ShowDialog(parent);
                }

                return false;
            }

            // Show dialog
            using var dialog = new UpdateDialog(result); // uses UpdateCheckResult
            var dialogResult = dialog.ShowDialog(parent);

            if (dialogResult != DialogResult.Continue)
            {
                logger?.Info("[UpdateCoordinator] User deferred the update.");
                return false;
            }

            // Build manifest DTO entries
            var manifestEntries = BuildManifestEntryDtoList(result);

            // Write JSON patch manifest to temp
            var jsonPath = WriteTemporaryManifest(manifestEntries);
            logger?.Info($"[UpdateCoordinator] Patch manifest written: {jsonPath}");

            // Launch bootstrapper
            LaunchBootstrapper(jsonPath, logger);
            return true;
        }

        private static List<ManifestEntryDto> BuildManifestEntryDtoList(UpdateCheckResult result)
        {
            var entries = new List<ManifestEntryDto>();

            if (result.IsAppUpdateAvailable)
            {
                entries.Add(new ManifestEntryDto
                {
                    Type = PatchType.Application,
                    Name = MidsContext.AppName,
                    Version = result.AppVersion,
                    File = result.AppFile,
                    TargetPath = AppContext.BaseDirectory
                });
            }

            if (result.IsDbUpdateAvailable)
            {
                entries.Add(new ManifestEntryDto
                {
                    Type = PatchType.Database,
                    Name = DatabaseAPI.DatabaseName,
                    Version = result.DbVersion,
                    File = result.DbFile,
                    TargetPath = Files.BaseDataPath
                });
            }

            return entries;
        }

        private static string WriteTemporaryManifest(List<ManifestEntryDto> entries)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"mids_patch_{Guid.NewGuid():N}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            File.WriteAllText(tempPath, JsonSerializer.Serialize(entries, options));
            return tempPath;
        }

        private static void LaunchBootstrapper(string manifestPath, ILogger? logger = null)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "MRBBootstrap.exe",
                    Arguments = $"\"{manifestPath}\"",
                    UseShellExecute = true
                };

                logger?.Info("[UpdateCoordinator] Launching MRBBootstrap...");
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                logger?.Info($"[UpdateCoordinator] Failed to launch bootstrapper: {ex.Message}");
            }
        }

        private static bool IsUpdateCheckDue()
        {
            var delay = MidsContext.Config.AutomaticUpdates.Delay;
            var lastChecked = MidsContext.Config.AutomaticUpdates.LastChecked;
            if (lastChecked == null) return true;
            return (DateTime.UtcNow.Date - lastChecked.Value.Date).TotalDays >= delay;
        }
    }
}

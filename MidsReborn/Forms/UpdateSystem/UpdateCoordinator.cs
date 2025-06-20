using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.UpdateSystem.Models;
using MRBLogging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp.Serializers.Json;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public static class UpdateCoordinator
    {
        public static async Task<bool> CheckAndHandleUpdatesAsync(IWin32Window parent, bool startupCheck = false, bool honorDelay = false, ILogger? logger = null)
        {
            if (honorDelay && !IsUpdateCheckDue()) { return false; }
            logger?.Info("[UpdateCoordinator] Starting update check...");

            var result = await UpdateUtils.CheckForUpdatesAsync(honorDelay);
            if (result is { IsAppUpdateAvailable: false, IsDbUpdateAvailable: false, IsBootstrapperUpdateAvailable: false})
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
            var manifestEntriesFull = BuildManifestEntryDtoList(result, false);
            var manifestEntries = BuildManifestEntryDtoList(result);

            // Write JSON patch manifest to temp
            var jsonPath = WriteTemporaryManifest(manifestEntries);
            logger?.Info($"[UpdateCoordinator] Patch manifest written: {jsonPath}");
            var i = 1;
            foreach (var entry in manifestEntriesFull)
            {
                logger?.Info($"[UpdateCoordinator] Relevant manifest entry #{i++}: Name: {entry.Name ?? "<null>"}, File: {entry.File ?? "<null>"}, TargetPath: {entry.TargetPath ?? "<null>"}, {entry.Version ?? "<null>"}, Type: {entry.Type}");
            }

            if (result.IsBootstrapperUpdateAvailable)
            {
                Debug.WriteLine("Bootstrapper update available. Updating.");
                await UpdateBootstrapper(manifestEntriesFull.First(e => e.Name == "Mids Reborn Bootstrapper"));
            }

            // Launch bootstrapper
            if (result.IsAppUpdateAvailable | result.IsDbUpdateAvailable)
            {
                LaunchBootstrapper(jsonPath, logger);
            }

            return true;
        }

        public static async Task<bool> UpdateBootstrapper(ManifestEntryDto manifestEntry)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var options = new RestClientOptions($"https://updates.midsreborn.com/{manifestEntry.File}")
            {
                ThrowOnAnyError = false,
                Timeout = TimeSpan.FromSeconds(5)
            };


            Debug.WriteLine($"[Bootstrapper update] Will download from {$"https://updates.midsreborn.com/{manifestEntry.File}"}");
            using var client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson(jsonOptions));

            try
            {
                var headRequest = new RestRequest().AddHeader("Accept", "application/json");
                headRequest.Method = Method.Head;

                var headResponse = await client.ExecuteAsync(headRequest);
                if (!headResponse.IsSuccessful || headResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    // Show message box if failed or not found
                    Debug.WriteLine("File not found or rest query failed");
                    return false;
                }

                Debug.WriteLine("[Bootstrapper update] Launching bootstrapper updater gui");
                using var bootstrapperUpdaterWindow = new BootstrapperUpdateDialog($"https://updates.midsreborn.com/{manifestEntry.File}", manifestEntry.Version, manifestEntry.TargetPath);
                var ret = bootstrapperUpdaterWindow.ShowDialog();
                Debug.WriteLine($"Bootstrapper update] Gui return code: {ret}");

                return ret == DialogResult.OK;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static List<ManifestEntryDto> BuildManifestEntryDtoList(UpdateCheckResult result, bool ignoreBootstrapper = true)
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

            if (ignoreBootstrapper)
            {
                return entries;
            }

            if (result.IsBootstrapperUpdateAvailable)
            {
                entries.Add(new ManifestEntryDto
                    {
                        Type = PatchType.Bootstrapper,
                        Name = result.BootstrapperName,
                        File = result.BootstrapperFile,
                        Version = result.BootstrapperVersion,
                        TargetPath = AppContext.BaseDirectory
                    }
                );
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

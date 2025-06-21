using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.UpdateSystem.Models;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public static class UpdateUtils
    {
        public static async Task<UpdateCheckResult> CheckForUpdatesAsync(bool honorDelay = false)
        {
            var manifestEntries = await FetchAllRelevantManifestEntriesAsync();
            var bootstrapperEntries = await FetchAllRelevantBootstrapperManifestEntriesAsync();
            manifestEntries = manifestEntries.Concat(bootstrapperEntries).ToList();
            var result = CompareAgainstCurrentVersions(manifestEntries);

            if (honorDelay)
            {
                MidsContext.Config.AutomaticUpdates.LastChecked = DateTime.UtcNow.Date;
            }

            return result;
        }

        private static async Task<List<ManifestEntry>> FetchAllRelevantManifestEntriesAsync()
        {
            var list = new List<ManifestEntry>();

            var midsManifest = await FetchManifest("https://updates.midsreborn.com/update_manifest.json", DatabaseAPI.DatabaseName);
            list.AddRange(midsManifest.Updates);

            if (DatabaseAPI.DatabaseName.Equals("Homecoming", StringComparison.OrdinalIgnoreCase))
            {
                return list;
            }

            var serverUri = DatabaseAPI.ServerData.ManifestUri;

            if (string.IsNullOrWhiteSpace(serverUri))
            {
                return list;
            }

            var externalManifest = await FetchManifest(serverUri, DatabaseAPI.DatabaseName);
            list.AddRange(externalManifest.Updates);

            return list;
        }

        private static async Task<List<ManifestEntry>> FetchAllRelevantBootstrapperManifestEntriesAsync()
        {
            var midsManifest = await FetchManifest("https://updates.midsreborn.com/bootstrapper_update_manifest.json");
            
            return midsManifest.Updates;
        }

        private static UpdateCheckResult CompareAgainstCurrentVersions(List<ManifestEntry> entries)
        {
            var result = new UpdateCheckResult();

            var appEntry = entries.FirstOrDefault(e =>
                e.Type == PatchType.Application &&
                e.Name?.Equals(MidsContext.AppName, StringComparison.OrdinalIgnoreCase) == true);

            if (appEntry != null && Version.TryParse(appEntry.Version, out var newAppVersion) && newAppVersion > MidsContext.AppFileVersion)
            {
                result.IsAppUpdateAvailable = true;
                result.AppName = appEntry.Name;
                result.AppVersion = appEntry.Version;
                result.AppFile = appEntry.File;
            }

            var dbEntry = entries.FirstOrDefault(e =>
                e.Type == PatchType.Database &&
                e.Name?.Equals(DatabaseAPI.DatabaseName, StringComparison.OrdinalIgnoreCase) == true);

            var ret = Version.TryParse(dbEntry?.Version, out var newDbVersion);
            if (dbEntry != null && ret && Helpers.IsVersionNewer(newDbVersion, DatabaseAPI.Database.Version))
            {
                result.IsDbUpdateAvailable = true;
                result.DbName = dbEntry.Name;
                result.DbVersion = dbEntry.Version;
                result.DbFile = dbEntry.File;
            }

            var bootstrapperEntry = entries.FirstOrDefault(e =>
                e.Type == PatchType.Bootstrapper &&
                e.Name?.Equals("Mids Reborn Bootstrapper", StringComparison.OrdinalIgnoreCase) == true);

            var bootstrapperFile = $"{AppContext.BaseDirectory}\\MRBBootstrap.exe";
            if (!File.Exists(bootstrapperFile) | bootstrapperEntry == null)
            {
                result.IsBootstrapperUpdateAvailable = false;
            }
            else
            {
                var modTime = File.GetLastWriteTime(bootstrapperFile);
                var manifestBootstrapperVersionChunks = bootstrapperEntry.Version.Split('-');
                
                // Basic modification time check
                if ($"{modTime.Year}.{modTime.Month:0#}.{modTime.Day:0#}" == manifestBootstrapperVersionChunks[0])
                {
                    return result;
                }

                var hashByteData = SHA256.HashData(File.ReadAllBytes(bootstrapperFile));
                var sBuilder = new StringBuilder();
                foreach (var b in hashByteData)
                {
                    sBuilder.Append($"{b:x2}");
                }

                var bootstrapperHash = sBuilder.ToString();
                if (bootstrapperHash.Equals(manifestBootstrapperVersionChunks[1], StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }

                result.IsBootstrapperUpdateAvailable = true;
                result.BootstrapperVersion = bootstrapperEntry.Version;
                result.BootstrapperFile = bootstrapperEntry.File;
                result.BootstrapperName = bootstrapperEntry.Name;
            }

            return result;
        }

        private static async Task<Manifest> FetchManifest(string manifestUrl, string? database = null)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var options = new RestClientOptions(manifestUrl)
            {
                ThrowOnAnyError = false,
                Timeout = TimeSpan.FromSeconds(5)
            };

            using var client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson(jsonOptions));

            try
            {
                // === Step 0: Check if manifest URL is pointing to an old-style XML file
                if (manifestUrl.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    if (database != null)
                    {
                        ShowMissingManifestWarning(database, manifestUrl);
                    }

                    return new Manifest();
                }

                // === Step 1: HEAD check to see if manifest exists
                var headRequest = new RestRequest().AddHeader("Accept", "application/json");
                headRequest.Method = Method.Head;

                var headResponse = await client.ExecuteAsync(headRequest);
                if (!headResponse.IsSuccessful || headResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    if (database != null)
                    {
                        ShowMissingManifestWarning(database, manifestUrl);
                    }

                    return new Manifest();
                }

                // Step 2: Attempt to fetch and deserialize
                var getRequest = new RestRequest();
                var result = await client.GetAsync<Manifest>(getRequest);
                return result ?? new Manifest();
            }
            catch (Exception e)
            {
                var mbox = new MessageBoxEx(
                    $"{e.GetType()} exception raised while trying to fetch manifest from:\r\n{manifestUrl}\r\n\r\n{e.Message}",
                    MessageBoxEx.MessageBoxExButtons.Ok,
                    MessageBoxEx.MessageBoxExIcon.Error,
                    true);

                mbox.ShowDialog();
                
                return new Manifest();
            }
        }

        private static void ShowMissingManifestWarning(string serverName, string manifestUrl)
        {
            var mbox = new MessageBoxEx(@"Check for Update(s)",
                $"Could not locate the manifest for the {serverName} database.\r\n\r\n" +
                $"This may indicate a misconfiguration or an outdated or missing manifest.\r\n" +
                $"If this is a custom or community server, please reach out to the database administrator(s).\r\n\r\n" +
                $"URL: {manifestUrl}",
                MessageBoxEx.MessageBoxExButtons.Ok,
                MessageBoxEx.MessageBoxExIcon.Warning,
                true);

            mbox.ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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

            var midsManifest = await FetchManifest("https://updates.midsreborn.com/update_manifest.json");
            list.AddRange(midsManifest.Updates);

            if (DatabaseAPI.DatabaseName.Equals("Homecoming", StringComparison.OrdinalIgnoreCase))
            {
                return list;
            }

            var externalUrl = DatabaseAPI.ServerData.ManifestUri;
            if (string.IsNullOrWhiteSpace(externalUrl))
            {
                return list;
            }

            var externalManifest = await FetchManifest(externalUrl);
            list.AddRange(externalManifest.Updates);

            return list;
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

            if (dbEntry == null || !Version.TryParse(dbEntry.Version, out var newDbVersion) || !Helpers.IsVersionNewer(newDbVersion, DatabaseAPI.Database.Version))
            {
                return result;
            }

            result.IsDbUpdateAvailable = true;
            result.DbName = dbEntry.Name;
            result.DbVersion = dbEntry.Version;
            result.DbFile = dbEntry.File;

            return result;
        }

        private static async Task<Manifest> FetchManifest(string manifestUrl)
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
                var request = new RestRequest();
                var result = await client.GetAsync<Manifest>(request);

                return result ?? new Manifest(); // safe fallback
            }
            catch (Exception e)
            {
                // Fancier error message box.
                // From normal run: provide specific info
                // From debug: prevent exception locator to jump into program.cs

                var mbox = new MessageBoxEx(
                    $"{e.GetType()} exception raised while trying to fetch manifest from {manifestUrl}\r\n\r\n{e.Message}",
                    MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);

                mbox.ShowDialog();

                return new Manifest();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                result.AppSourceUri = appEntry.SourceUri;
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
            result.DbSourceUri = dbEntry.SourceUri;

            return result;
        }

        private static async Task<Manifest> FetchManifest(string manifestUrl, string database)
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
                    ShowLegacyManifestWarning(database, manifestUrl);
                    return new Manifest();
                }

                // === Step 1: HEAD check to see if manifest exists
                var headRequest = new RestRequest().AddHeader("Accept", "application/json");
                headRequest.Method = Method.Head;

                var headResponse = await client.ExecuteAsync(headRequest);
                if (!headResponse.IsSuccessful || headResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    ShowMissingManifestWarning(database, manifestUrl);
                    return new Manifest();
                }

                // === Step 2: GET and parse manifest
                var getRequest = new RestRequest();
                var result = await client.GetAsync<Manifest>(getRequest);
                if (result is null)
                {
                    return new Manifest();
                }

                // === Step 3: Tag all entries with the manifest's source URI
                foreach (var entry in result.Updates)
                {
                    entry.SourceUri = GetBaseUriFromFileUrl(manifestUrl);
                }

                return result;
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

        private static void ShowLegacyManifestWarning(string serverName, string manifestUrl)
        {
            var mbox = new MessageBoxEx(@"Check for Update(s)",
                $"Manifest file for {serverName} database has been found,\r\nbut it uses the old XML format which has been deprecated.\r\n\r\n" +
                (MidsContext.Config?.Mode != ConfigData.Modes.User
                    ? "Please update manifest URL to JSON in the Server Data from the Database Menu."
                    : "This may indicate a misconfiguration or an outdated manifest\r\n\r\nIf this is a custom or community server, please reach out to the database administrator(s).") +
                      $"\r\n\r\nURL: {manifestUrl}",
                MessageBoxEx.MessageBoxExButtons.Ok,
                MessageBoxEx.MessageBoxExIcon.Warning,
                true);

            mbox.ShowDialog();
        }

        public static string GetBaseUriFromFileUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                throw new ArgumentException(@"Invalid URL", nameof(url));

            // Remove just the last segment (file name)
            var segments = uri.Segments;
            if (segments.Length == 0)
                throw new InvalidOperationException("URL has no segments.");

            var basePath = string.Join("", segments[..^1]); // all but last
            return new Uri(uri, basePath).ToString();
        }
    }
}

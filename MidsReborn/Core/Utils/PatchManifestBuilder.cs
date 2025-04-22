using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Mids_Reborn.Forms.UpdateSystem.Models;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Mids_Reborn.Core.Utils;

public static class PatchManifestBuilder
{
    private const string MidsRebornManifestUrl = "https://updates.midsreborn.com/update_manifest.json";

    private static string ResolveManifestUri(PatchType type, string databaseName)
    {
        return type switch
        {
            PatchType.Application => MidsRebornManifestUrl,
            PatchType.Database => databaseName switch
            {
                "Homecoming" => MidsRebornManifestUrl,
                _ => DatabaseAPI.ServerData.ManifestUri
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static async Task<Manifest?> ModifyManifestAsync(PatchType type, string name, string version, string file)
    {
        var manifestUrl = ResolveManifestUri(type, name);
        var options = new RestClientOptions(manifestUrl);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        using var client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson(jsonOptions));
        var request = new RestRequest();
        var response = await client.GetAsync<Manifest>(request);

        if (response is null)
        {
            throw new Exception($"Unable to fetch manifest from {manifestUrl}");
        }

        var updatedEntry = new ManifestEntry(type, name, version, file);

        var index = response.Updates.FindIndex(e =>
            e.Type == updatedEntry.Type && e.Name != null && e.Name.Equals(updatedEntry.Name));

        if (index >= 0)
        {
            response.Updates[index] = updatedEntry;
        }
        else
        {
            response.Updates.Add(updatedEntry);
        }

        response.LastUpdated = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

        return response;

    }
}

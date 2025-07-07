using MRB_Boostrap.Models;
using System.Text.Json;
using Serilog;

namespace MRB_Boostrap.Utilities;

public static class UpdateManifestLoader
{
    public static List<UpdateEntry> Load(string jsonPath)
    {
        var logger = Log.Logger;

        if (!File.Exists(jsonPath))
        {
            logger.Error("Update manifest not found at: {Path}", jsonPath);
            throw new FileNotFoundException("Update manifest not found", jsonPath);
        }

        try
        {
            string json = File.ReadAllText(jsonPath);

            var entries = JsonSerializer.Deserialize<List<UpdateEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (entries is null || entries.Count == 0)
            {
                logger.Error("Update manifest is empty or invalid: {Path}", jsonPath);
                throw new Exception("Manifest is empty or invalid.");
            }

            logger.Information("Loaded {Count} patch entries from manifest: {Path}", entries.Count, jsonPath);
            return entries;
        }
        catch (JsonException jex)
        {
            logger.Error(jex, "JSON deserialization failed while reading manifest at: {Path}", jsonPath);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unexpected error while loading update manifest from: {Path}", jsonPath);
            throw;
        }
    }
}
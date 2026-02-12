using MRB_Boostrap.Models;
using Serilog;
using Newtonsoft.Json;

namespace MRB_Boostrap.Utilities;

public static class UpdateManifestLoader
{
    public static List<UpdateEntry> Load(string jsonPath)
    {
        var logger = Log.Logger;

        if (string.IsNullOrWhiteSpace(jsonPath))
        {
            logger.Error("Update manifest path is null/empty.");
            throw new ArgumentException("Update manifest path is null/empty.", nameof(jsonPath));
        }

        if (!File.Exists(jsonPath))
        {
            logger.Error("Update manifest not found at: {Path}", jsonPath);
            throw new FileNotFoundException("Update manifest not found", jsonPath);
        }

        try
        {
            var json = File.ReadAllText(jsonPath);

            if (string.IsNullOrWhiteSpace(json))
            {
                logger.Error("Update manifest is empty: {Path}", jsonPath);
                throw new Exception("Manifest is empty or invalid.");
            }

            var root = GetFirstNonWhitespaceChar(json);
            logger.Information("Manifest root token '{Token}' for: {Path}", root, jsonPath);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            List<UpdateEntry> entries;

            switch (root)
            {
                case '[':
                    entries = JsonConvert.DeserializeObject<List<UpdateEntry>>(json, settings) ?? new List<UpdateEntry>();
                    break;

                case '{':
                    var manifest = JsonConvert.DeserializeObject<Manifest>(json, settings);

                    if (manifest == null)
                    {
                        logger.Error("JSON deserialization failed while reading manifest at: {Path}", jsonPath);
                        throw new JsonSerializationException($"JSON deserialization failed while reading manifest at: {jsonPath}");
                    }

                    entries = manifest.Updates ?? new List<UpdateEntry>();
                    break;

                default:
                    logger.Error("Update manifest has unexpected root token '{Token}' at: {Path}", root, jsonPath);
                    throw new JsonSerializationException($"Unexpected JSON root token '{root}' in manifest: {jsonPath}");
            }

            if (entries.Count == 0)
            {
                logger.Error("Update manifest is empty or invalid: {Path}", jsonPath);
                throw new Exception("Manifest is empty or invalid.");
            }

            logger.Information("Loaded {Count} patch entries from manifest: {Path}", entries.Count, jsonPath);
            return entries;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "JSON deserialization failed while reading manifest at: {Path}", jsonPath);
            throw;
        }
    }

    private static char GetFirstNonWhitespaceChar(string json)
    {
        // Handles whitespace + BOM
        for (var i = 0; i < json.Length; i++)
        {
            var c = json[i];

            if (c == '\uFEFF') // UTF-8 BOM
                continue;

            if (!char.IsWhiteSpace(c))
                return c;
        }

        return '\0';
    }
}
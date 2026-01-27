using MRB_Boostrap.Models;
using Serilog;
using Newtonsoft.Json;

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
            var json = File.ReadAllText(jsonPath);
            var manifest = JsonConvert.DeserializeObject<Manifest>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            if (manifest == null)
            {
                logger.Error("JSON deserialization failed while reading manifest at: {Path}", jsonPath);
                throw new JsonSerializationException($"JSON deserialization failed while reading manifest at: {jsonPath}");
            }

            var entries = manifest.Updates;

            if (entries.Count == 0)
            {
                logger.Error("Update manifest is empty or invalid: {Path}", jsonPath);
                throw new Exception("Manifest is empty or invalid.");
            }

            logger.Information("Loaded {Count} patch entries from manifest: {Path}", entries.Count, jsonPath);
            
            return entries.ToList();
        }
        catch (Exception jex)
        {
            logger.Error(jex, "JSON deserialization failed while reading manifest at: {Path}", jsonPath);
            throw;
        }
    }
}
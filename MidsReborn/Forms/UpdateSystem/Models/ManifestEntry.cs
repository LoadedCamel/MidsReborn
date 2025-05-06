using Mids_Reborn.Core.Utils;
using System.Text.Json.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    public class ManifestEntry
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PatchType? Type { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? File { get; set; }

        public ManifestEntry(PatchType type, string name, string version, string file)
        {
            Type = type;
            Name = name;
            Version = version;
            File = file;
        }

        public ManifestEntry()
        {
            Type = null;
            Name = null;
            Version = null;
            File = null;
        }
    }

    public class ManifestEntryDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PatchType Type { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? File { get; set; }
        public string? TargetPath { get; set; }
    }
}

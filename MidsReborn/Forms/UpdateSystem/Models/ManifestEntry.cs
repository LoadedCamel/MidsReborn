using Mids_Reborn.Core.Utils;
using System.Text.Json.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    public class ManifestEntry(PatchType? type = null, string? name = null, string? version = null, string? file = null)
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PatchType? Type { get; set; } = type;

        public string? Name { get; set; } = name;
        public string? Version { get; set; } = version;
        public string? File { get; set; } = file;

        // Not serialized, but tracked internally
        [JsonIgnore]
        public string? SourceUri { get; set; }
    }

    public class ManifestEntryDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PatchType Type { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? File { get; set; }

        public string? SourceUri { get; set; }
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    public class Manifest
    {
        public string ManifestVersion { get; set; } = "3.0";
        public List<ManifestEntry> Updates { get; set; } = [];
        public string LastUpdated { get; set; } = string.Empty;
    }
}

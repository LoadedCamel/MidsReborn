using System.Collections.Generic;

namespace Mids_Reborn.UI.Forms.UpdateSystem.Models
{
    public class Manifest
    {
        public string ManifestVersion { get; set; } = "3.0";
        public List<ManifestEntry> Updates { get; set; } = [];
        public string LastUpdated { get; set; } = string.Empty;
    }
}

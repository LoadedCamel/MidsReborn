using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    [XmlRoot("manifest"), XmlType("manifest")]
    public class Manifest
    {
        public string ManifestVersion { get; set; } = "3.0";
        public List<ManifestEntry> Updates { get; set; } = [];
        public string LastUpdated { get; set; } = string.Empty;
    }
}

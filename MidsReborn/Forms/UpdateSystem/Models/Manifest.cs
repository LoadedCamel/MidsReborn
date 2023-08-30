using System.Xml.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    [XmlRoot("manifest")]
    public class Manifest
    {
        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("file")]
        public string File { get; set; }
    }
}

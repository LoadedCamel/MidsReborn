using System;
using System.Xml.Serialization;

namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    [XmlRoot(ElementName = "manifest")]
    public class UpdateResponse
    {
        [XmlElement(ElementName = "version")]
        public string UpdateVersion { get; set; }

        [XmlElement(ElementName = "file")]
        public string UpdateFile { get; set; }
    }
}

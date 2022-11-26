using System;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class AppUpdate
    {
        public bool Mandatory { get; set; }
        public Version Version { get; set; } = new();
        public string ChangeLog { get; set; } = string.Empty;

        public AppUpdate()
        {
            var settings = new XmlReaderSettings
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };
            if (MidsContext.Config == null) return;
            using var xmlReader = XmlReader.Create(MidsContext.Config.UpdatePath, settings);
            while (xmlReader.Read())
            {
                try
                {
                    switch (xmlReader.Name)
                    {
                        case "version":
                        {
                            Version = Version.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                        case "changelog":
                        {
                            ChangeLog = xmlReader.ReadElementContentAsString();
                            MidsContext.Config.AppChangeLog = ChangeLog;
                            break;
                        }
                        case "mandatory":
                        {
                            Mandatory = bool.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show(@"An error occurred while attempting to read from the manifest.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public bool IsAvailable => Helpers.CompareVersions(Version, MidsContext.AppFileVersion);
    }
}

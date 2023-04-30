using System;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class DbUpdate
    {
        public bool Mandatory { get; set; }
        public Version Version { get; set; } = new();
        public string ChangeLog { get; set; } = string.Empty;

        public DbUpdate()
        {
            var settings = new XmlReaderSettings
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };

            using var xmlReader = XmlReader.Create(DatabaseAPI.ServerData.ManifestUri, settings);
            try
            {
                while (xmlReader.Read())
                {
                    switch (xmlReader.Name)
                    {
                        case "db-version" or "version": // Rebirth
                        {
                            Version = Version.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                        case "changelog":
                        {
                            ChangeLog = xmlReader.ReadElementContentAsString();
                            if (MidsContext.Config != null) MidsContext.Config.DbChangeLog = ChangeLog;
                            break;
                        }
                        case "mandatory":
                        {
                            Mandatory = bool.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show(
                    $@"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}\r\n\r\n{ex.Message}",
                    @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (Version == null)
            {
                MessageBox.Show(
                    $@"Cannot fetch available DB version from manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                    @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool IsAvailable => Helpers.CompareVersions(Version, DatabaseAPI.Database.Version);
    }
}

using System;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class DbUpdate
    {
        public enum ManifestStatus
        {
            Unknown,
            Failure,
            Success
        }

        public bool Mandatory { get; set; }
        public Version Version { get; set; } = new();
        public ManifestStatus Status { get; private set; }

        public DbUpdate()
        {
            if (string.IsNullOrEmpty(DatabaseAPI.ServerData.ManifestUri)) // Manifest URI is blank with the generic database
            {
                Status = ManifestStatus.Unknown;

                return;
            }

            var settings = new XmlReaderSettings
            {
                XmlResolver = new XmlUrlResolver(),
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
                        case "mandatory":
                        {
                            Mandatory = bool.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                    }
                }

                Status = ManifestStatus.Success;
            }
            catch (ArgumentException) // May occur while parsing xml
            {
                MessageBox.Show(
                    $"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                    @"DB Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Status = ManifestStatus.Failure;
            }
            catch (XmlException)
            {
                MessageBox.Show(
                    $"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                    @"DB Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Status = ManifestStatus.Failure;
            }

            if (Version != null)
            {
                return;
            }

            MessageBox.Show(
                $"Cannot fetch available DB version from manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                @"DB Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Status = ManifestStatus.Failure;
        }

        public bool IsAvailable => Helpers.CompareVersions(Version, DatabaseAPI.Database.Version);
    }
}

using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

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

            XmlReader xmlReader;

            try
            {
                xmlReader = XmlReader.Create(DatabaseAPI.ServerData.ManifestUri, settings);
            }
            catch (HttpRequestException ex)
            {
                using var msgBox = new MessageBoxEx($"Cannot check for {DatabaseAPI.DatabaseName} database updates.\r\n{(ex.StatusCode != null ? $"Error {ex.StatusCode} - " : "")}{ex.Message}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Warning, true);
                msgBox.ShowDialog();

                return;
            }

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

                xmlReader.Close();
                Status = ManifestStatus.Success;
            }
            catch (ArgumentException) // May occur while parsing xml
            {
                using var msgBox = new MessageBoxEx(
                    $"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                    MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                msgBox.ShowDialog();

                Status = ManifestStatus.Failure;
            }
            catch (XmlException)
            {
                using var msgBox = new MessageBoxEx(
                    $"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                    MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                msgBox.ShowDialog();
                
                Status = ManifestStatus.Failure;
            }

            if (Version != null)
            {
                return;
            }

            using var msgBox2 = new MessageBoxEx(
                $"Cannot fetch available DB version from manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}",
                MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
            msgBox2.ShowDialog();

            Status = ManifestStatus.Failure;
        }

        public bool IsAvailable => Helpers.CompareVersions(Version, DatabaseAPI.Database.Version);
    }
}

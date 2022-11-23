using System;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using static Mids_Reborn.Forms.UpdateSystem.clsXMLUpdate;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class DbUpdate
    {
        private static bool Mandatory { get; set; }
        private static Version Version { get; set; }
        public static string? ChangeLog { get; set; }

        public static bool IsAvailable
        {
            get
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
                            case "version":
                            case "db-version": // Rebirth
                            {
                                Version = Version.Parse(xmlReader.ReadElementContentAsString());
                                break;
                            }
                            case "changelog":
                            {
                                ChangeLog = xmlReader.ReadElementContentAsString();
                                MidsContext.Config.DbChangeLog = ChangeLog;
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
                    MessageBox.Show($"An error occurred while attempting to read from the manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}\r\n\r\n{ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (Version == null)
                {
                    MessageBox.Show($"Cannot fetch available DB version from manifest.\r\nURL: {(string.IsNullOrWhiteSpace(DatabaseAPI.ServerData.ManifestUri) ? "(none)" : DatabaseAPI.ServerData.ManifestUri)}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return CompareVersions(Version, DatabaseAPI.Database.Version);
            }
        }

        public static void InitiateQuery(frmMain parent)
        {
            if (!Mandatory)
            {
                var dbResult = new UpdateQuery(parent)
                {
                    Type = UpdateType.Database.ToString()
                };
                dbResult.ShowDialog();
                switch (dbResult.DialogResult)
                {
                    case DialogResult.Yes:
                    {
                        var patchNotes = new PatchNotes(parent, true)
                        {
                            Type = UpdateType.Database.ToString(),
                            Version = Version.ToString()
                        };
                        patchNotes.ShowDialog();
                        break;
                    }
                    case DialogResult.No:
                        dbResult.Close();
                        break;
                    case DialogResult.OK:
                        Update(DatabaseAPI.ServerData.ManifestUri, Version.ToString(), Files.BaseDataPath);
                        break;
                }
            }
            else
            {
                Update(DatabaseAPI.ServerData.ManifestUri, Version.ToString(), Files.BaseDataPath);
            }
        }
    }
}

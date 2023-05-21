using System;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class AppUpdate
    {
        public enum ManifestStatus
        {
            Unknown,
            Failure,
            Success
        }

        public bool Mandatory { get; set; }
        public Version Version { get; set; } = new();
        public string ChangeLog { get; set; } = string.Empty;
        public ManifestStatus Status { get; private set; } = ManifestStatus.Unknown;

        public AppUpdate()
        {
            var settings = new XmlReaderSettings
            {
                XmlResolver = new XmlUrlResolver(),
                DtdProcessing = DtdProcessing.Ignore
            };
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
                        case "mandatory":
                        {
                            Mandatory = bool.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                    }

                    Status = ManifestStatus.Success;
                }
                catch
                {
                    MessageBox.Show($"An error occurred while attempting to read from the manifest.\r\nURL: {MidsContext.Config.UpdatePath}", @"App Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Status = ManifestStatus.Failure;
                }
            }
        }
        public bool IsAvailable => Helpers.CompareVersions(Version, MidsContext.AppFileVersion);
    }
}

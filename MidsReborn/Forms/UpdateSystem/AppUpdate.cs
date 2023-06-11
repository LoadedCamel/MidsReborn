using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

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

            XmlReader xmlReader;
            
            try
            {
                xmlReader = XmlReader.Create(MidsContext.Config.UpdatePath, settings);
            }
            catch (HttpRequestException ex)
            {
                using var msgBox = new MessageBoxEx($"Cannot check for application updates.\r\n{(ex.StatusCode != null ? $"Error {ex.StatusCode} - " : "")}{ex.Message}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Warning, true);
                msgBox.ShowDialog();

                return;
            }

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

                    xmlReader.Close();
                    Status = ManifestStatus.Success;
                }
                catch
                {
                    using var msgBox = new MessageBoxEx($"An error occurred while attempting to read from the manifest.\r\nURL: {MidsContext.Config.UpdatePath}", @"App Update Error", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                    msgBox.ShowDialog();

                    Status = ManifestStatus.Failure;
                }
            }
        }
        public bool IsAvailable => Helpers.CompareVersions(Version, MidsContext.AppFileVersion);
    }
}

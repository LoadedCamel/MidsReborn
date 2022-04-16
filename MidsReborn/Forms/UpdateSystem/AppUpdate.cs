using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using mrbBase.Base.Master_Classes;
using static Mids_Reborn.Forms.UpdateSystem.clsXMLUpdate;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class AppUpdate
    {
        private static bool Mandatory { get; set; }
        private static Version Version { get; set; }
        public static string ChangeLog { get; set; }

        public static bool IsAvailable
        {
            get
            {
                var settings = new XmlReaderSettings
                {
                    XmlResolver = null,
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
                        return false;
                    }
                }

                return CompareVersions(Version, MidsContext.AppFileVersion);
            }
        }

        public static void InitiateQuery(frmMain parent)
        {
            if (!Mandatory)
            {
                var appResult = new UpdateQuery(parent)
                {
                    Type = UpdateType.App.ToString()
                };
                appResult.ShowDialog();
                switch (appResult.DialogResult)
                {
                    case DialogResult.Yes:
                    {
                        var patchNotes = new PatchNotes(parent, true)
                        {
                            Type = UpdateType.App.ToString(),
                            Version = Version.ToString()
                        };
                        patchNotes.ShowDialog();
                        break;
                    }
                    case DialogResult.No:
                        appResult.Close();
                        break;
                    case DialogResult.OK:
                        Update(MidsContext.Config.UpdatePath, Version.ToString());
                        break;
                }
            }
            else
            {
                Update(MidsContext.Config.UpdatePath, Version.ToString());
            }
        }
    }
}

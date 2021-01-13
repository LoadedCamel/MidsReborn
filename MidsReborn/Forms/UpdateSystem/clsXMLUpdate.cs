using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class clsXMLUpdate
    {
        public enum ECheckResponse
        {
            NoUpdates,
            Updates,
            FailedWithMessage
        }

        private const string ReadmeUrl =
            "https://raw.githubusercontent.com/Crytilis/mids-reborn-hero-designer/master/README.md";

        public bool RestartNeeded = false;
        private static bool Mandatory { get; set; }
        private static Version AppVersion { get; set; }
        private static float DbVersion { get; set; }
        private static string ChangeLog { get; set; }

        public static void BugReportCrytilis()
        {
            LaunchBrowser("https://github.com/Reborn-Team/Hero-Designer/issues");
        }

        public static void DownloadFromDomain()
        {
            LaunchBrowser("https://midsreborn.com/download/MRB_Setup.msi");
        }

        public static void KoFi()
        {
            LaunchBrowser("https://ko-fi.com/metalios");
        }

        public static void Patreon()
        {
            LaunchBrowser("https://www.patreon.com/midsreborn");
        }

        public static void GoToGitHub()
        {
            LaunchBrowser("https://github.com/Reborn-Team/Hero-Designer");
        }

        public static void GoToForums()
        {
            LaunchBrowser("https://forums.homecomingservers.com/topic/19963-mids-reborn-hero-designer/");
        }

        private static void LaunchBrowser(string iUri)
        {
            try
            {
                Process.Start(iUri);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($@"There was an error when starting the systems default web browser. {ex.Message}", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public static void CheckUpdate(frmMain parent)
        {
            var settings = new XmlReaderSettings
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };
            using var xmlReader = XmlReader.Create(MidsContext.Config.UpdatePath, settings);
            while (xmlReader.Read())
                try
                {
                    switch (xmlReader.Name)
                    {
                        case "app-version":
                        {
                            AppVersion = new Version(xmlReader.ReadElementContentAsString());
                            break;
                        }
                        case "db-version":
                        {
                            DbVersion = xmlReader.ReadElementContentAsFloat();
                            break;
                        }
                        case "changelog":
                        {
                            ChangeLog = xmlReader.ReadElementContentAsString();
                            break;
                        }
                        case "mandatory":
                        {
                            Mandatory = bool.Parse(xmlReader.ReadElementContentAsString());
                            break;
                        }
                    }
                }
                catch (XmlException e)
                {
                    MessageBox.Show($"{e.Message}\r\n{e.StackTrace}", "Error");
                }

            var cDbVersion = DatabaseAPI.Database.Version;
            if (AppVersion > MidsContext.AppVersion)
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
                                Version = AppVersion.ToString()
                            };
                            patchNotes.ShowDialog();
                            break;
                        }
                        case DialogResult.No:
                            appResult.Close();
                            break;
                        case DialogResult.OK:
                            Update(UpdateType.App, AppVersion.ToString(), parent);
                            break;
                    }
                }
                else
                {
                    Update(UpdateType.App, AppVersion.ToString(), parent);
                }
            }
            else if (DbVersion > cDbVersion && AppVersion < MidsContext.AppVersion)
            {
                if (!Mandatory)
                {
                    var dbResult = new UpdateQuery(parent)
                    {
                        Type = UpdateType.Database.ToString()
                    };
                    switch (dbResult.DialogResult)
                    {
                        case DialogResult.Yes:
                        {
                            var patchNotes = new PatchNotes(parent, true)
                            {
                                Type = UpdateType.Database.ToString(),
                                Version = DbVersion.ToString(CultureInfo.InvariantCulture)
                            };
                            patchNotes.ShowDialog();
                            break;
                        }
                        case DialogResult.No:
                            dbResult.Close();
                            break;
                        case DialogResult.OK:
                            Update(UpdateType.Database, DbVersion.ToString(CultureInfo.InvariantCulture), parent);
                            break;
                    }
                }
                else
                {
                    Update(UpdateType.Database, DbVersion.ToString(CultureInfo.InvariantCulture), parent);
                }
            }
            else
            {
                MessageBox.Show(@"No update is available at this time.", @"Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void Update(Enum type, string updateVersion, frmMain parent)
        {
            try
            {
                using var updateForm = new Updater(parent)
                {
                    Type = type?.ToString(),
                    VersionText = updateVersion,
                };
                updateForm.ShowDialog();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}", "Error");
            }
        }

        public enum UpdateType
        {
            None,
            App,
            Database
        }

        protected enum EUpdateType
        {
            None,
            AppUpdate,
            DbUpdate
        }
    }
}
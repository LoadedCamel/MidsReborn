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
        public bool RestartNeeded = false;

        public static void BugReportCrytilis()
        {
            LaunchBrowser("https://github.com/Reborn-Team/MidsReborn/issues");
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
            LaunchBrowser("https://github.com/Reborn-Team/MidsReborn");
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
            if (AppUpdate.IsAvailable)
            {
                AppUpdate.InitiateQuery(parent);
            }
            else if (DbUpdate.IsAvailable)
            {
                DbUpdate.InitiateQuery(parent);
            }
            else
            {
                MessageBox.Show(@"There are no updates available at this time.", @"Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        public static void Update(Enum type, string updateVersion, frmMain parent)
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
            catch
            {
                // Ignored
            }
        }

        public enum UpdateType
        {
            None,
            App,
            Database
        }
    }
}
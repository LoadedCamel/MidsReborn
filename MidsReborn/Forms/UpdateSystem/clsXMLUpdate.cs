using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using mrbBase;
using MRBUpdater;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class clsXMLUpdate
    {
        public bool RestartNeeded = false;

        public static void BugReportCrytilis()
        {
            LaunchBrowser("https://github.com/LoadedCamel/MidsReborn/issues");
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
            LaunchBrowser("https://github.com/LoadedCamel/MidsReborn");
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

        public static void Update(string path, string updateVersion)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Application.StartupPath,
                    FileName = @"MRBUpdater.exe",
                    Arguments = $"{path} {updateVersion} {Process.GetCurrentProcess().Id}"
                };

                Process.Start(startInfo);


            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
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
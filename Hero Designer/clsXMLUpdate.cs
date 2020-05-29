
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml;
using Base;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using midsControls;

namespace Hero_Designer
{
    public class clsXMLUpdate
    {
        public static bool Mandatory { get; set; }
        public static Version AppVersion { get; set; }
        public static float DbVersion { get; set; }
        public static string ChangeLog { get; set; }

        public bool RestartNeeded = false;

        public static void BugReportCrytilis() => LaunchBrowser("https://github.com/Crytilis/mids-reborn-hero-designer/issues");

        public static void DownloadFromDomain() => LaunchBrowser("https://midsreborn.com/download/MRB_Setup.exe");

        public static void KoFi() { LaunchBrowser("https://ko-fi.com/metalios"); }

        public static void Patreon() { LaunchBrowser("https://www.patreon.com/midsreborn"); }

        public static void GoToGitHub()
        {
            LaunchBrowser("https://github.com/Crytilis/mids-reborn-hero-designer");
        }

        public static void GoToForums() { LaunchBrowser("https://forums.homecomingservers.com/topic/7645-mids-reborn-hero-designer/"); }

        private static void LaunchBrowser(string iUri)
        {
            try
            {
                Process.Start(iUri);
            }
            catch (Win32Exception ex)
            {
                _ = MessageBox.Show($@"There was an error when starting the systems default web browser. {ex.Message}", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //Interaction.MsgBox(("There was an error starting the default web browser: " + ex.Message), MsgBoxStyle.Exclamation, "Error!");
            }
        }
        const string ReadmeUrl = "https://raw.githubusercontent.com/Crytilis/mids-reborn-hero-designer/master/README.md";

        public static void CheckUpdate()
        {
            var settings = new XmlReaderSettings
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };
            //MessageBox.Show(MidsContext.Config.UpdatePath);
            using var xmlReader = XmlReader.Create(MidsContext.Config.UpdatePath, settings);
            while (xmlReader.Read())
            {
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
                    MessageBox.Show($"{e.Message}\n{e.StackTrace}", "Error");
                }
            }
            var cDbVersion = DatabaseAPI.Database.Version;
            if (AppVersion > MidsContext.AppVersion)
            {
                if (!Mandatory)
                {
                    var appResult = MessageBox.Show($@"A new application update is available. Do you wish to update to v{AppVersion}?", @"Application Update available!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    //MessageBox.Show(ChangeLog);
                    if (appResult == DialogResult.Yes)
                    {
                        Update(UpdateType.App, AppVersion.ToString(), ChangeLog);
                    }
                }
                else
                {
                    Update(UpdateType.App, AppVersion.ToString(), ChangeLog);
                }
            }
            else if (DbVersion > cDbVersion && AppVersion < MidsContext.AppVersion)
            {
                if (!Mandatory)
                {
                    var dbResult = MessageBox.Show($@"A new database update is available. Do you wish to update to v{DbVersion}?", @"Database Update Available!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dbResult == DialogResult.Yes)
                    {
                        Update(UpdateType.Database, DbVersion.ToString(CultureInfo.InvariantCulture), ChangeLog);
                    }
                }
                else
                {
                    Update(UpdateType.Database, DbVersion.ToString(CultureInfo.InvariantCulture), ChangeLog);
                }
            }
            else
            {
                MessageBox.Show($@"No update is available at this time.", @"Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void Update(Enum type, string updateVersion, string changeLog)
        {
            try
            {

                using WebClient client = new WebClient();
                Stream stream = client.OpenRead(changeLog);
                if (stream != null)
                {
                    using StreamReader reader = new StreamReader(stream);

                    var content = reader?.ReadToEnd();

                    using var updateForm = new frmUpdate
                    {
                        Type = type?.ToString(),
                        VersionText = updateVersion,
                        RichText = content,
                        ctlProgressBar1 = {StatusText = $"Downloading: {type} {updateVersion}"},
                        ctlProgressBar2 = {StatusText = "", Text = ""}
                    };
                    updateForm.ShowDialog();
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show($"{e.Message}\n{e.StackTrace}", "Error");
            }
        }



        /*public static (ECheckResponse, string) UpdateCheck()
        {
            string response;
            try
            {
                using var client = new HttpClient();
                var url = new Uri(ReadmeUrl);
                response = client.GetStringAsync(url).Result;
            }
            catch (HttpRequestException ex)
            {
                var msg = $"Failed to download update information ({ex.Message}) from update :" + ReadmeUrl;
                return (ECheckResponse.FailedWithMessage, msg);
            }

            if (string.IsNullOrWhiteSpace(response))
            {
                var msg = "Failed to reach update url: " + ReadmeUrl;
                return (ECheckResponse.FailedWithMessage, msg);
            }
            string remoteversion;
            try
            {
                remoteversion = response.After("Latest Version").Trim().GetLine(0).Trim().Before("-").Trim();
            }
            catch (Exception ex)
            {
                var msg = $"Failed parse text ({ex.Message}) from update url:" + ReadmeUrl;
                return (ECheckResponse.FailedWithMessage, msg);
            }
            Version availVer;
            try
            {
                availVer = new Version(remoteversion);
            }
            catch (Exception ex)
            {
                var msg = $"Failed parse version ('{remoteversion}',{ex.Message}) from update url:" + ReadmeUrl;
                return (ECheckResponse.FailedWithMessage, msg);
            }
            try
            {
                var runningVer = typeof(frmMain).Assembly.GetName().Version;
                // I don't trust that != isn't reference comparison for the version type
                return runningVer.CompareTo(availVer) < 0 ? (ECheckResponse.Updates, $"Version {remoteversion}, installed is {runningVer}") : (ECheckResponse.NoUpdates, $"Installed is {runningVer}, remote is Version {remoteversion}");
            }
            catch (Exception ex)
            {
                var msg = $"Failed compare versions ('{remoteversion}',{ex.Message}) from update url: {ReadmeUrl}";
                return (ECheckResponse.FailedWithMessage, msg);
            }
        }*/

        protected enum UpdateType
        {
            None,
            App,
            Database
        }

        public enum ECheckResponse
        {
            NoUpdates,
            Updates,
            FailedWithMessage
        }

        protected enum EUpdateType
        {
            None,
            AppUpdate,
            DbUpdate

        }
    }
}

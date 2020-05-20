
using System;
using System.Diagnostics;
using System.Net.Http;
using Base;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public class clsXMLUpdate
    {

        public bool RestartNeeded = false;

        public static void BugReportCrytilis() => LaunchBrowser("https://github.com/Crytilis/mids-reborn-hero-designer/issues");

        public static void DownloadFromDomain() => LaunchBrowser("https://midsreborn.com/download/MRB_Setup.exe");

        public static void KoFi()
        {
            LaunchBrowser("https://ko-fi.com/metalios");
        }

        public static void Patreon()
        {
            LaunchBrowser("https://www.patreon.com/midsreborn");
        }

        public static void GoToGitHubCrytilis() => LaunchBrowser("https://github.com/Crytilis/mids-reborn-hero-designer");

        public static void GoToForums()
        {
            LaunchBrowser("https://forums.homecomingservers.com/topic/7645-mids-reborn-hero-designer/");
        }

        static void LaunchBrowser(string iURI)
        {
            try
            {
                Process.Start(iURI);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Interaction.MsgBox(("There was an error starting the default web browser: " + ex.Message), MsgBoxStyle.Exclamation, "Aiee!");
                ProjectData.ClearProjectError();
            }
        }
        const string readmeUrl = "https://raw.githubusercontent.com/Crytilis/mids-hero-designer/master/README.md";

        public (eCheckResponse, string) UpdateCheck()
        {
            string response;
            try
            {
                using (var client = new HttpClient())
                    response = client.GetStringAsync(readmeUrl).Result;

            }
            catch (Exception ex)
            {
                var msg = $"Failed to download update information ({ex.Message}) from update :" + readmeUrl;
                return (eCheckResponse.FailedWithMessage, msg);
            }

            if (string.IsNullOrWhiteSpace(response))
            {
                var msg = "Failed to reach update url: " + readmeUrl;
                return (eCheckResponse.FailedWithMessage, msg);
            }
            string remoteversion;
            try
            {
                remoteversion = response.After("Latest Version").Trim().GetLine(0).Trim().Before("-").Trim();
            }
            catch (Exception ex)
            {
                var msg = $"Failed parse text ({ex.Message}) from update url:" + readmeUrl;
                return (eCheckResponse.FailedWithMessage, msg);
            }
            Version availVer;
            try
            {
                availVer = new Version(remoteversion);
            }
            catch (Exception ex)
            {
                var msg = $"Failed parse version ('{remoteversion}',{ex.Message}) from update url:" + readmeUrl;
                return (eCheckResponse.FailedWithMessage, msg);
            }
            try
            {
                var runningVer = typeof(frmMain).Assembly.GetName().Version;
                // I don't trust that != isn't reference comparison for the version type
                return runningVer.CompareTo(availVer) < 0 ? (eCheckResponse.Updates, $"Version {remoteversion}, installed is {runningVer}") : (eCheckResponse.NoUpdates, $"Installed is {runningVer}, remote is Version {remoteversion}");
            }
            catch (Exception ex)
            {
                var msg = $"Failed compare versions ('{remoteversion}',{ex.Message}) from update url: {readmeUrl}";
                return (eCheckResponse.FailedWithMessage, msg);
            }
        }

        public enum eCheckResponse
        {
            NoUpdates,
            Updates,
            FailedWithMessage
        }

        protected enum eUpdateType
        {
            None,
            AppUpdate,
            DBUpdate
        }
    }
}

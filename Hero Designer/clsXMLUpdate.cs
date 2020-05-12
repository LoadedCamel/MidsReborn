
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

        /*public static void BugReport(string at, string pri, string sec, string sData = "")
        {
            var targetUrl = "https://github.com/ImaginaryDevelopment/imaginary-hero-designer/issues";
            try
            {
                if (sData.Length > 0)
                {
                    sData = sData.Replace("\r\n", "-");
                    if (sData.Length > 96)
                        sData = sData.Substring(0, 96);
                }
                //string str = "?" + "body=" + Strings.Format(MidsContext.AppVersion, "##0.#####") + "&db=" + Strings.Format(DatabaseAPI.Database.Version, "##0.#####") + " (" + Strings.Format(DatabaseAPI.Database.Date, "dd/MM/yy") + ")" + "&at=" + at + "&p=" + pri + "&s=" + sec + "&OS=" + OS.GetQuickOsid();
                if (sData != "")
                    //str = str + "&data=" + sData;
                Process.Start(targetUrl);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Interaction.MsgBox(@"There was an error launching the default web browser to visit:"
                + "\r\n" + targetUrl + "\r\n"
                + ex.Message, MsgBoxStyle.Critical, "Error");
                ProjectData.ClearProjectError();
            }
        }*/

        public static void BugReportCrytilis() => LaunchBrowser("https://github.com/Crytilis/mids-reborn-hero-designer/issues");
        //public static void BugReportImaginary() => LaunchBrowser("https://github.com/ImaginaryDevelopment/imaginary-hero-designer/issues");

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

        //public static void GoToGitHubImaginary() => LaunchBrowser("https://github.com/ImaginaryDevelopment/imaginary-hero-designer");

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
        const string readmeUrl = "https://raw.githubusercontent.com/Crytilis/mids-reborn-hero-designer/master/README.md";

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

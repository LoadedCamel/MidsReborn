using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public class clsXMLUpdate
    {
        public bool RestartNeeded = false;

        public static void SupportServer()
        {
            LaunchBrowser("https://discord.gg/eAUuNQ3nxk");
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

        public static void CoinBase()
        {
            LaunchBrowser("https://commerce.coinbase.com/checkout/804803e4-9b18-4f18-9f6b-413856dda262");
        }

        public static void GoToForums()
        {
            LaunchBrowser("https://forums.homecomingservers.com/topic/19963-mids-reborn-hero-designer/");
        }

        private static void LaunchBrowser(string iUri)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = iUri
                });
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($@"There was an error when starting the systems default web browser. {ex.Message}", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Mids_Reborn.Core
{
    public abstract class SupportSites
    {
        public static void SupportServer()
        {
            LaunchBrowser("https://discord.gg/mids-reborn-593336669004890113");
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
using System;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms;

namespace Mids_Reborn
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
            Fonts.BuildFontCollection();
            Application.Run(new frmMain(args));
        }
    }
}
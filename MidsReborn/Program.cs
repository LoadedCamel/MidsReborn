using System;
using System.Diagnostics;
using System.Windows.Forms;
using Mids_Reborn.Forms;
using Mids_Reborn.UIv2;

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
            //Application.SetHighDpiMode(HighDpiMode.SystemAware); -- Temporarily Disabled
            Application.Run(new frmMain(args));
        }
    }
}
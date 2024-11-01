using System;
using System.Globalization;
using System.Threading;
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
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Fonts.BuildFontCollection();
            Application.Run(new MrbAppContext(args));
        }
    }
}
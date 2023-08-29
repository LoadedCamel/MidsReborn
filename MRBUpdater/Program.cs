using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MRBUpdater
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += ApplicationOnApplicationExit;
            Application.Run(new Update(args));
        }

        private static void ApplicationOnApplicationExit(object? sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = AppContext.BaseDirectory,
                FileName = @"cleanup.exe"
            };

            Process.Start(startInfo);
        }
    }
}

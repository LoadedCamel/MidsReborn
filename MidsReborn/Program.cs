using System.Globalization;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Fonts.BuildFontCollection();
            StrapUpdater.Run();

            ApplicationConfiguration.Initialize();
            Application.Run(new MrbAppContext(args));
        }
    }
}
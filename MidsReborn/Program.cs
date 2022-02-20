using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTQyNTM5QDMxMzkyZTMzMmUzMGErM3R4akxDVTI4WkNtWmZ1TEpvZEpXeUhvcEJva3B4blhwTHRMV3R4SXM9");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(args));
        }
    }
}
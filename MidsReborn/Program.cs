using System;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Forms;
using Mids_Reborn.UIv2;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzczMTM1QDMxMzgyZTM0MmUzME9URGZHeHFrdFFwZlhxQnZITjlreGZGek96bldQRGpPSmFDd3VWQ1lBaUU9");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                MidsContext.AssertVersioning();
                using frmMain f = new frmMain();
                using Form1 f1 = new Form1();
                Application.Run(f);
            }
            catch (Exception ex)
            {
                var exTarget = ex;
                while (exTarget?.InnerException != null)
                {
                    exTarget = ex.InnerException;
                }

                if (exTarget != null)
                {
                    // Zed: add extra info here.
                    var args = Environment.GetCommandLineArgs();
                    if (args.Skip(1).Contains("-debug"))
                    {
                        MessageBox.Show(
                            $"Error: {exTarget.Message}\r\nException type: {exTarget.GetType().Name}\r\nStack Trace:\r\n{exTarget.StackTrace}",
                            $"Error [Debug mode] [Mids Reborn v{Application.ProductVersion}]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Error: {exTarget.Message}\r\n{exTarget.StackTrace}", exTarget.GetType().Name,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    throw;
                }
            }
        }
    }
}
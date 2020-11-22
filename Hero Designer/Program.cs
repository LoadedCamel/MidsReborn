using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Base.Master_Classes;
using Hero_Designer.Forms;

namespace Hero_Designer
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                MidsContext.AssertVersioning();
                using frmMain f = new frmMain();
                Application.Run(f);
            }
            catch (Exception e)
            {
                try
                {
                    MidsContext.AssertVersioning();
                    using frmMain f = new frmMain();
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
                        string[] args = Environment.GetCommandLineArgs();
                        if (args.Skip(1).Contains("-debug"))
                        {
                            MessageBox.Show(
                                "Error: " + exTarget.Message + "\n" +
                                "Stack Trace: " + exTarget.StackTrace + "\n" +
                                "Exception type: " + exTarget.GetType().Name,
                                "Error [Debug mode enabled]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show(exTarget.Message, exTarget.GetType().Name, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }

                        throw;
                    }
                }
            }
        }
    }
}
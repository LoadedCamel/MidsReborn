using System;
using System.Diagnostics;
using System.Windows.Forms;
using Base.Master_Classes;

namespace Hero_Designer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Debugger.IsAttached || Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
            {
                MidsContext.AssertVersioning();
                using (frmMain f = new frmMain())
                {
                    Application.Run(f);
                }
            }
            else
            {
                try
                {
                    MidsContext.AssertVersioning();
                    using (frmMain f = new frmMain())
                    {
                        Application.Run(f);
                    }
                }
                catch (Exception ex)
                {
                    var exTarget = ex;
                    while (exTarget?.InnerException != null)
                    {
                        exTarget = ex.InnerException;
                    }

                    if (exTarget != null)
                        MessageBox.Show(exTarget.Message, exTarget.GetType().Name);
                    throw;
                }
            }
        }
    }
}

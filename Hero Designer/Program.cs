using System;
using System.Windows.Forms;
using Base.Master_Classes;

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
                using var frmMain = new frmMain();
                Application.Run(frmMain);
            }
            catch (Exception ex)
            {
                var exTarget = ex;
                while (exTarget?.InnerException != null) exTarget = ex.InnerException;

                if (exTarget != null)
                    MessageBox.Show(exTarget.Message, exTarget.GetType().Name);
                throw;
            }
        }
    }
}
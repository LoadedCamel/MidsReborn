using Microsoft.Win32;
using Mids_Reborn.UI.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn
{
    public class MrbAppContext : ApplicationContext
    {
        private bool _shutdownRan;

        public MrbAppContext(string[]? args)
        {
            Application.ApplicationExit += OnApplicationExit;
            SystemEvents.SessionEnding += OnSessionEnding;

            InitializeApplication(args);
        }

        private void InitializeApplication(string[]? args)
        {
            using (var splash = new Loader())
            {
                var result = splash.ShowDialog();
                if (result != DialogResult.OK)
                {
                    ExitThread();
                    return;
                }
            }

            // Once the splash screen has completed loading, proceed to show the main form.
            var mainForm = new MainWindow2(args);
            mainForm.FormClosed += (sender, e) => OnMainFormClosed();
            MainForm = mainForm;
            mainForm.Show();
        }

        private void RunShutdown()
        {
            if (_shutdownRan) return;
            _shutdownRan = true;

            try
            {
                var cfg = MidsContext.Config;
                if (cfg is null) return;
                if (MainForm is not null && MainForm.WindowState is FormWindowState.Normal)
                {
                    cfg.Bounds = MainForm.Bounds;
                    cfg.WindowState = MainForm.WindowState.ToString();
                }

                cfg.SaveConfig();
                AssetManager.Shutdown();
            }
            catch
            {
                // never throw on shutdown
            }
        }

        private void OnMainFormClosed()
        {
            RunShutdown();
            ExitThread();
        }

        private void OnApplicationExit(object? sender, EventArgs e) => RunShutdown();

        private void OnSessionEnding(object? sender, SessionEndingEventArgs e)
        {
            // Called on logoff/shutdown; keep it quick and non-blocking.
            RunShutdown();
        }
    }

}
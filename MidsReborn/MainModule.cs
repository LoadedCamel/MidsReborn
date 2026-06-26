using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.IO_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.UI.Forms;

namespace Mids_Reborn
{
    public sealed class MainModule
    {
        public class MidsController
        {
            public static Rectangle SzFrmCompare = new();
            public static Rectangle SzFrmData = new();
            public static Rectangle SzFrmRecipe = new();
            public static Rectangle SzFrmSets = new();
            public static Rectangle SzFrmStats = new();
            public static Rectangle SzFrmTotals = new();

            public static bool IsAppInitialized { get; private set; }
            private static frmBusy? _bFrm;

            public static Toon? Toon
            {
                get => MidsContext.Character as Toon;
                set => MidsContext.Character = value;
            }

            private static void BusyHide()
            {
                if (_bFrm == null)
                    return;
                _bFrm.Close();
                _bFrm = null;
            }

            private static void BusyMsg(ref MainWindow iFrm, string sMessage, string sTitle = "")
            {
                var bFrm = new frmBusy();
                if (!string.IsNullOrWhiteSpace(sTitle))
                {
                    bFrm.SetTitle(sTitle);
                }
                bFrm.Show(iFrm);
                bFrm.SetMessage(sMessage);
            }

            public static async Task ChangeDatabase(frmBusy? iFrm)
            {
                iFrm?.SetMessage(@"Restarting with selected database.");
                await Task.Delay(2000);
                BusyHide();
                Application.Restart();
            }

            public static async void SelectDefaultDatabase(Loader iFrm)
            {
                var installedDatabases = DatabaseAPI.GetInstalledDatabases();
                if (installedDatabases.Count == 0)
                {
                    MessageBox.Show(@"No databases were found. Please install a database.", @"No Databases Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                var defaultDatabase = installedDatabases.TryGetValue("Homecoming", out var homecomingPath)
                    ? new KeyValuePair<string, string>("Homecoming", homecomingPath)
                    : installedDatabases.First();
                if (MidsContext.Config == null) return;
                MidsContext.Config.DataPath = defaultDatabase.Value;
                MidsContext.Config.SavePath = defaultDatabase.Value;
                //MidsContext.Config.FirstRun = false;

                await LoadData(iFrm, MidsContext.Config.DataPath);
            }

            public static async Task LoadData(IMessenger messenger, string? path)
            {
                messenger.SetMessage("Initializing Data...");
                messenger.SetMessage("Loading Server Data...");
                if (!DatabaseAPI.LoadServerData(path))
                {
                    MessageBox.Show(@"There was an error reading the data. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                messenger.SetMessage("Loading Build Preferences");
                BuildPreferences.Load();
                DatabaseAPI.LoadTypeGrades(path);
                messenger.SetMessage("Loading Main Data...");
                if (!DatabaseAPI.LoadMainDatabase(path))
                {
                    MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                messenger.SetMessage("Loading Progression Data...");
                if (!DatabaseAPI.LoadLevelsDatabase(path))
                {
                    MessageBox.Show(@"Unable to proceed, failed to load progression data. Re-import or regenerate the selected database.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                
                if (!DatabaseAPI.LoadMaths(path))
                {
                    Application.Exit();
                }

                messenger.SetMessage("Loading Global Chance Modifiers...");
                if (!DatabaseAPI.LoadEffectIdsDatabase(path))
                {
                    Application.Exit();
                }

                messenger.SetMessage("Loading Enhancement Database...");
                if (!DatabaseAPI.LoadEnhancementClasses(path))
                {
                    Application.Exit();
                }

                DatabaseAPI.LoadEnhancementDb(path);
                DatabaseAPI.LoadOrigins(path);
                
                //DatabaseAPI.ShowSetTypes();
                //DatabaseAPI.LoadSetTypeStrings(path);

                messenger.SetMessage("Loading Recipe Database...");
                DatabaseAPI.LoadSalvage(path);
                DatabaseAPI.LoadRecipes(path);

                // if (File.Exists(AppDataPaths.CrypticPowersRepl))
                // {
                //     messenger.SetMessage("Loading Powers Replacement Table...");
                //     DatabaseAPI.LoadReplacementTable();
                // }

                messenger.SetMessage("Loading Cryptic-specific power names translation table");
                //DatabaseAPI.LoadCrypticReplacementTable();

                messenger.SetMessage("Loading Graphics...");
                await LoadGraphics(path);

                DatabaseAPI.MatchAllIDs(messenger);
                messenger.SetMessage("Matching Recipe IDs...");

                DatabaseAPI.AssignRecipeIDs();
                GC.Collect();
                IsAppInitialized = true;
            }

            private static async Task LoadGraphics(string? path)
            {
                try
                {
                    // Run the synchronous loading methods on a background thread
                    await Task.Run(() =>
                    {
                        AssetManager.Initialize(path);
                        AssetManager.LoadImages();
                    });
                }
                catch (InvalidOperationException ex)
                {
                    // Catch the specific error and display the message safely on the UI thread
                    MessageBox.Show($"Reason: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

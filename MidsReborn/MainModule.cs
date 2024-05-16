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
using Mids_Reborn.Forms;

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

            public static clsToonX? Toon
            {
                get => MidsContext.Character as clsToonX;
                set => MidsContext.Character = value;
            }

            private static void BusyHide()
            {
                if (_bFrm == null)
                    return;
                _bFrm.Close();
                _bFrm = null;
            }

            private static void BusyMsg(ref frmMain iFrm, string sMessage, string sTitle = "")
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

                var defaultDatabase = installedDatabases.First(db => db.Key != "Generic");
                if (MidsContext.Config == null) return;
                MidsContext.Config.DataPath = defaultDatabase.Value;
                MidsContext.Config.SavePath = defaultDatabase.Value;
                MidsContext.Config.FirstRun = false;

                await LoadData(iFrm, MidsContext.Config.DataPath);
            }

            /*public static void SelectDatabase(frmInitializing? iFrm)
            {
                using var dbSelector = new DatabaseSelector();
                var result = dbSelector.ShowDialog();
                string? dbSelected;
                if (result == DialogResult.OK)
                {
                    dbSelected = dbSelector.SelectedDatabase;
                }
                else
                {
                    MessageBox.Show(@"The default i24 (Generic) Database will be used as you did not select a database.", @"Database Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbSelected = Files.FDefaultPath;
                }

                
                MidsContext.Config.DataPath = dbSelected;
                MidsContext.Config.SavePath = dbSelected;
                LoadData(ref iFrm, MidsContext.Config.DataPath);
            }*/

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
                messenger.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load(path)) { }

                DatabaseAPI.LoadTypeGrades(path);
                messenger.SetMessage("Loading Main Data...");
                if (!DatabaseAPI.LoadLevelsDatabase(path))
                {
                    MessageBox.Show(@"Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://github.com/LoadedCamel/MidsReborn/releases.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadMainDatabase(path))
                {
                    MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (File.Exists(Files.CNamePowersRepl))
                {
                    messenger.SetMessage("Loading Powers Replacement Table...");
                    DatabaseAPI.LoadReplacementTable();
                }

                messenger.SetMessage("Loading Cryptic-specific power names translation table");
                DatabaseAPI.LoadCrypticReplacementTable();

                messenger.SetMessage("Loading Graphics...");
                await LoadGraphics(path);

                DatabaseAPI.MatchAllIDs(messenger);
                messenger.SetMessage("Matching Set Bonus IDs...");
                DatabaseAPI.AssignSetBonusIndexes();
                messenger.SetMessage("Matching Recipe IDs...");

                DatabaseAPI.AssignRecipeIDs();
                GC.Collect();
                IsAppInitialized = true;
            }

            /*public static void LoadData(ref Loader? iFrm, string? path)
            {
                iFrm?.SetMessage("Initializing Data...");
                iFrm?.SetMessage("Loading Server Data...");
                if (!DatabaseAPI.LoadServerData(path))
                {
                    MessageBox.Show(@"There was an error reading the data. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                iFrm?.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load(path)) { }

                DatabaseAPI.LoadTypeGrades(path);
                iFrm?.SetMessage("Loading Main Data...");
                if (!DatabaseAPI.LoadLevelsDatabase(path))
                {
                    MessageBox.Show(@"Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://github.com/LoadedCamel/MidsReborn/releases.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadMainDatabase(path))
                {
                    MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                
                if (!DatabaseAPI.LoadMaths(path))
                {
                    Application.Exit();
                }

                iFrm?.SetMessage("Loading Global Chance Modifiers...");
                if (!DatabaseAPI.LoadEffectIdsDatabase(path))
                {
                    Application.Exit();
                }

                iFrm?.SetMessage("Loading Enhancement Database...");
                if (!DatabaseAPI.LoadEnhancementClasses(path))
                {
                    Application.Exit();
                }

                DatabaseAPI.LoadEnhancementDb(path);
                DatabaseAPI.LoadOrigins(path);
                
                //DatabaseAPI.ShowSetTypes();
                //DatabaseAPI.LoadSetTypeStrings(path);

                iFrm?.SetMessage("Loading Recipe Database...");
                DatabaseAPI.LoadSalvage(path);
                DatabaseAPI.LoadRecipes(path);

                if (File.Exists(Files.CNamePowersRepl))
                {
                    iFrm?.SetMessage("Loading Powers Replacement Table...");
                    DatabaseAPI.LoadReplacementTable();
                }

                iFrm?.SetMessage("Loading Cryptic-specific power names translation table");
                DatabaseAPI.LoadCrypticReplacementTable();

                iFrm?.SetMessage("Loading Graphics...");
                LoadGraphics(path).GetAwaiter().GetResult();

                //MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(Files.MxdbFileBbCodeUpdate, path));
                if (iFrm != null)
                {
                    DatabaseAPI.MatchAllIDs(iFrm);
                    iFrm?.SetMessage("Matching Set Bonus IDs...");
                    DatabaseAPI.AssignSetBonusIndexes();
                    iFrm?.SetMessage("Matching Recipe IDs...");
                }

                DatabaseAPI.AssignRecipeIDs();
                GC.Collect();
                IsAppInitialized = true;
                if (iFrm != null) iFrm.LoadingComplete = true;
            }*/

            /*public static async void LoadData(string? path)
            {
                if (!DatabaseAPI.LoadServerData(path))
                {
                    MessageBox.Show(@"There was an error reading the data. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load(path)) { }

                DatabaseAPI.LoadTypeGrades(path);
                if (!DatabaseAPI.LoadLevelsDatabase(path))
                {
                    MessageBox.Show(@"Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://github.com/LoadedCamel/MidsReborn/releases.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadMainDatabase(path))
                {
                    MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                
                if (!DatabaseAPI.LoadMaths(path))
                {
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadEffectIdsDatabase(path))
                {
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadEnhancementClasses(path))
                {
                    Application.Exit();
                }

                DatabaseAPI.LoadEnhancementDb(path);
                DatabaseAPI.LoadOrigins(path);
                
                //DatabaseAPI.ShowSetTypes();
                //DatabaseAPI.LoadSetTypeStrings(path);

                DatabaseAPI.LoadSalvage(path);
                DatabaseAPI.LoadRecipes(path);

                if (File.Exists(Files.CNamePowersRepl))
                {
                    DatabaseAPI.LoadReplacementTable();
                }

                DatabaseAPI.LoadCrypticReplacementTable();

                await LoadGraphics(path);

                DatabaseAPI.MatchIds();
                DatabaseAPI.AssignSetBonusIndexes();
                DatabaseAPI.AssignRecipeIDs();
                IsAppInitialized = true;
            }*/

            private static async Task LoadGraphics(string? path)
            {
                await I9Gfx.Initialize(path);
                await I9Gfx.LoadImages();
            }
        }
    }
}
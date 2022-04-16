using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Forms;
using Mids_Reborn.Forms.OptionsMenuItems;
using mrbBase;
using mrbBase.Base.Master_Classes;

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
            private static frmBusy _bFrm;

            public static clsToonX Toon
            {
                get => (clsToonX) MidsContext.Character;
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

            public static async Task ChangeDatabase(frmBusy iFrm)
            {
                iFrm.SetMessage(@"Mids Reborn will now restart and load your selected database.");
                await Task.Delay(2000);
                BusyHide();
                Application.Restart();
            }

            public static void SelectDatabase(frmInitializing iFrm)
            {
                using var dbSelector = new DatabaseSelector();
                var result = dbSelector.ShowDialog();
                var dbSelected = result == DialogResult.OK ? dbSelector.SelectedDatabase : Files.FDefaultPath;
                MidsContext.Config.DataPath = dbSelected;
                LoadData(ref iFrm, MidsContext.Config.DataPath);
            }

            public static void LoadData(ref frmInitializing iFrm, string path)
            {
                IsAppInitialized = true;
                iFrm?.SetMessage("Loading Data...");
                iFrm?.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load(path)) { }

                DatabaseAPI.LoadTypeGrades(path);
                iFrm?.SetMessage("Loading Powerset Database...");
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

                iFrm?.SetMessage("Loading Powers Replacement Table...");
                DatabaseAPI.LoadReplacementTable();

                iFrm?.SetMessage("Loading Graphics...");
                LoadGraphics(path).GetAwaiter().GetResult();

                MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(Files.MxdbFileBbCodeUpdate, path));
                if (iFrm != null)
                {
                    DatabaseAPI.MatchAllIDs(iFrm);
                    iFrm?.SetMessage("Matching Set Bonus IDs...");
                    DatabaseAPI.AssignSetBonusIndexes();
                    iFrm?.SetMessage("Matching Recipe IDs...");
                }

                DatabaseAPI.AssignRecipeIDs();
                GC.Collect();
            }

            private static async Task LoadGraphics(string path)
            {
                await I9Gfx.Initialize(path);
                await I9Gfx.LoadImages();
            }
        }
    }
}
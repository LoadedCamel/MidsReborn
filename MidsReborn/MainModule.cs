using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Forms;
using Mids_Reborn.Forms.OptionsMenuItems;
using Mids_Reborn.Forms.OptionsMenuItems.DbEditor;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn
{
    public sealed class MainModule
    {
        public class MidsController
        {
            public static Rectangle SzFrmCompare = new Rectangle();
            public static Rectangle SzFrmData = new Rectangle();
            public static Rectangle SzFrmRecipe = new Rectangle();
            public static Rectangle SzFrmSets = new Rectangle();
            public static Rectangle SzFrmStats = new Rectangle();
            public static Rectangle SzFrmTotals = new Rectangle();

            public static bool IsAppInitialized { get; private set; }
            private static frmBusy bFrm;

            public static clsToonX Toon
            {
                get => (clsToonX) MidsContext.Character;
                set => MidsContext.Character = value;
            }

            private static void BusyHide()
            {
                if (bFrm == null)
                    return;
                bFrm.Close();
                bFrm = null;
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

            public static void LoadData(ref frmInitializing iFrm, string path, bool forConversion = false)
            {
                if (forConversion)
                {
                    DatabaseAPI.LoadDatabaseVersion(path);
                    IsAppInitialized = true;
                    iFrm?.SetMessage("Loading Data...");
                    iFrm?.SetMessage("Loading Attribute Modifiers...");
                    DatabaseAPI.Database.AttribMods = new Modifiers();
                    if (!DatabaseAPI.Database.AttribMods.Load(path))
                    {
                    }

                    iFrm?.SetMessage("Loading Powerset Database...");
                    if (!DatabaseAPI.LoadLevelsDatabase(Files.FDefaultPath))
                    {
                        MessageBox.Show(@"Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://midsreborn.com.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }

                    if (!DatabaseAPI.LoadMainDatabase(path, true))
                    {
                        MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    
                    if (!DatabaseAPI.LoadMaths(path))
                    {
                        Application.Exit();
                    }

                    iFrm?.SetMessage("Loading Enhancement Database...");
                    if (!DatabaseAPI.LoadEnhancementClasses(path))
                    {
                        Application.Exit();
                    }

                    DatabaseAPI.LoadEnhancementDb(path, true);
                    

                    DatabaseAPI.LoadOrigins(path);
                    DatabaseAPI.LoadSetTypeStrings(path);

                    iFrm?.SetMessage("Loading Recipe Database...");
                    DatabaseAPI.LoadSalvage(path);

                    DatabaseAPI.LoadRecipes(path, true);

                    iFrm?.SetMessage("Loading Graphics...");
                    var taskArray = new Task[9];
                    taskArray[0] = Task.Run(I9Gfx.LoadOriginImages);
                    taskArray[1] = Task.Run(I9Gfx.LoadArchetypeImages);
                    taskArray[2] = Task.Run(I9Gfx.LoadPowersetImages);
                    taskArray[3] = Task.Run(I9Gfx.LoadEnhancements);
                    taskArray[4] = Task.Run(I9Gfx.LoadSets);
                    taskArray[5] = Task.Run(I9Gfx.LoadBorders);
                    taskArray[6] = Task.Run(I9Gfx.LoadSetTypes);
                    taskArray[7] = Task.Run(I9Gfx.LoadEnhTypes);
                    taskArray[8] = Task.Run(I9Gfx.LoadClasses);
                    Task.WaitAll(taskArray);

                    MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(path));
                    if (iFrm != null)
                    {
                        //iFrm.Opacity = 1.0;
                        DatabaseAPI.MatchAllIDs(iFrm);
                        iFrm?.SetMessage("Matching Set Bonus IDs...");
                        DatabaseAPI.AssignSetBonusIndexes();
                        iFrm?.SetMessage("Matching Recipe IDs...");
                    }

                    DatabaseAPI.AssignRecipeIDs();
                }
                else
                {
                    DatabaseAPI.LoadDatabaseVersion(path);
                    IsAppInitialized = true;
                    iFrm?.SetMessage("Loading Data...");
                    iFrm?.SetMessage("Loading Attribute Modifiers...");
                    DatabaseAPI.Database.AttribMods = new Modifiers();
                    if (!DatabaseAPI.Database.AttribMods.Load(path)) { }

                    iFrm?.SetMessage("Loading Powerset Database...");
                    if (!DatabaseAPI.LoadLevelsDatabase(path))
                    {
                        MessageBox.Show("Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://github.com/Reborn-Team/Hero-Designer/releases.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    DatabaseAPI.LoadSetTypeStrings(path);

                    iFrm?.SetMessage("Loading Recipe Database...");
                    DatabaseAPI.LoadSalvage(path);
                    DatabaseAPI.LoadRecipes(path);

                    iFrm?.SetMessage("Loading Graphics...");
                    var taskArray = new Task[9];
                    taskArray[0] = Task.Run(I9Gfx.LoadOriginImages);
                    taskArray[1] = Task.Run(I9Gfx.LoadArchetypeImages);
                    taskArray[2] = Task.Run(I9Gfx.LoadPowersetImages);
                    taskArray[3] = Task.Run(I9Gfx.LoadEnhancements);
                    taskArray[4] = Task.Run(I9Gfx.LoadSets);
                    taskArray[5] = Task.Run(I9Gfx.LoadBorders);
                    taskArray[6] = Task.Run(I9Gfx.LoadSetTypes);
                    taskArray[7] = Task.Run(I9Gfx.LoadEnhTypes);
                    taskArray[8] = Task.Run(I9Gfx.LoadClasses);
                    Task.WaitAll(taskArray);

                    MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(Files.MxdbFileBbCodeUpdate, path));
                    if (iFrm != null)
                    {
                        //iFrm.Opacity = 1.0;
                        DatabaseAPI.MatchAllIDs(iFrm);
                        iFrm?.SetMessage("Matching Set Bonus IDs...");
                        DatabaseAPI.AssignSetBonusIndexes();
                        iFrm?.SetMessage("Matching Recipe IDs...");
                    }

                    DatabaseAPI.AssignRecipeIDs();
                }
                GC.Collect();
            }

            public static void LoadData(ref frmInitializing iFrm)
            {
                DatabaseAPI.LoadDatabaseVersion();
                IsAppInitialized = true;
                iFrm?.SetMessage("Loading Data...");
                iFrm?.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load()) {}

                iFrm?.SetMessage("Loading Powerset Database...");
                if (!DatabaseAPI.LoadLevelsDatabase())
                {
                    MessageBox.Show("Unable to proceed, failed to load leveling data! We suggest you re-download the application from https://github.com/Reborn-Team/Hero-Designer/releases.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadMainDatabase())
                {
                    MessageBox.Show(@"There was an error reading the database. Aborting!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                if (!DatabaseAPI.LoadMaths())
                {
                    Application.Exit();
                }

                iFrm?.SetMessage("Loading Global Chance Modifiers...");
                if (!DatabaseAPI.LoadEffectIdsDatabase())
                {
                    Application.Exit();
                }

                iFrm?.SetMessage("Loading Enhancement Database...");
                if (!DatabaseAPI.LoadEnhancementClasses())
                {
                    Application.Exit();
                }

                DatabaseAPI.LoadEnhancementDb();
                DatabaseAPI.LoadOrigins();
                DatabaseAPI.LoadSetTypeStrings();

                iFrm?.SetMessage("Loading Recipe Database...");
                DatabaseAPI.LoadSalvage();
                DatabaseAPI.LoadRecipes();

                iFrm?.SetMessage("Loading Graphics...");
                var taskArray = new Task[9];
                taskArray[0] = Task.Run(I9Gfx.LoadOriginImages);
                taskArray[1] = Task.Run(I9Gfx.LoadArchetypeImages);
                taskArray[2] = Task.Run(I9Gfx.LoadPowersetImages);
                taskArray[3] = Task.Run(I9Gfx.LoadEnhancements);
                taskArray[4] = Task.Run(I9Gfx.LoadSets);
                taskArray[5] = Task.Run(I9Gfx.LoadBorders);
                taskArray[6] = Task.Run(I9Gfx.LoadSetTypes);
                taskArray[7] = Task.Run(I9Gfx.LoadEnhTypes);
                taskArray[8] = Task.Run(I9Gfx.LoadClasses);
                Task.WaitAll(taskArray);

                MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(Files.MxdbFileBbCodeUpdate));
                if (iFrm != null)
                {
                    //iFrm.Opacity = 1.0;
                    DatabaseAPI.MatchAllIDs(iFrm);
                    iFrm?.SetMessage("Matching Set Bonus IDs...");
                    DatabaseAPI.AssignSetBonusIndexes();
                    iFrm?.SetMessage("Matching Recipe IDs...");
                }

                DatabaseAPI.AssignRecipeIDs();
                GC.Collect();
            }
        }
    }
}
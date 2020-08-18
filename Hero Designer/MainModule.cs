
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;

namespace Hero_Designer
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

            public static clsToonX Toon
            {
                get => (clsToonX) MidsContext.Character;
                set => MidsContext.Character = value;
            }

            private static void Quit()
            {
                // https://stackoverflow.com/a/12978034
                if (Application.MessageLoop)
                {
                    Application.Exit();
                }
                else
                {
                    Environment.Exit(1);
                }
            }

            public static void LoadData(ref frmLoading iFrm)
            {
                DatabaseAPI.LoadDatabaseVersion();
                IsAppInitialized = true;
                iFrm?.SetMessage("Loading Data...");
                iFrm?.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load())
                {
                    MessageBox.Show(
                        "Failed to load Attribute Modifiers database. Aborting.",
                        "Woops",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Quit();
                }

                iFrm?.SetMessage("Loading Powerset Database...");
                if (!DatabaseAPI.LoadLevelsDatabase())
                {
                    MessageBox.Show(
                        "Failed to load Leveling data file! The program is unable to proceed.\r\n" +
                        "We suggest you redownload the application from https://github.com/Crytilis/mids-reborn-hero-designer/releases",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Quit();
                }

                if (!DatabaseAPI.LoadMainDatabase())
                {
                    MessageBox.Show(
                        "There was an error reading the database. Aborting.",
                        "Dang",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Quit();
                }

                if (!DatabaseAPI.LoadMaths())
                {
                    MessageBox.Show(
                        "Failed to load Maths database. Aborting.",
                        "Dang",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Quit();
                }
                iFrm?.SetMessage("Loading Enhancement Database...");
                if (!DatabaseAPI.LoadEnhancementClasses())
                {
                    MessageBox.Show(
                        "Failed to load Enhancements' classes. Aborting.",
                        "Dang",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Quit();
                }
                
                DatabaseAPI.LoadEnhancementDb();
                DatabaseAPI.LoadOrigins();
                DatabaseAPI.LoadSetTypeStrings();

                iFrm?.SetMessage("Loading Recipe Database...");               
                DatabaseAPI.LoadSalvage();
                DatabaseAPI.LoadRecipes();

                iFrm?.SetMessage("Loading Legacy Enhancements Database...");
                bool oldEnhLoad = DatabaseAPI.LoadOldEnhNames();

                iFrm?.SetMessage("Loading Legacy Sets Database...");
                bool oldSetsLoad = DatabaseAPI.LoadOldSetNames();

                if (!(oldEnhLoad && oldSetsLoad))
                {
                    MessageBox.Show(
                        Application.ProductName + " will function but old builds recovery will be unavailable.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                iFrm?.SetMessage("Loading Graphics...");
                Task[] taskArray = new Task[9];
                taskArray[0] = Task.Run(() => { I9Gfx.LoadOriginImages(); });
                taskArray[1] = Task.Run(() => { I9Gfx.LoadArchetypeImages(); });
                taskArray[2] = Task.Run(() => { I9Gfx.LoadPowersetImages(); });
                taskArray[3] = Task.Run(() => { I9Gfx.LoadEnhancements(); });
                taskArray[4] = Task.Run(() => { I9Gfx.LoadSets(); });
                taskArray[5] = Task.Run(() => { I9Gfx.LoadBorders(); });
                taskArray[6] = Task.Run(() => { I9Gfx.LoadSetTypes(); });
                taskArray[7] = Task.Run(() => { I9Gfx.LoadEnhTypes(); });
                taskArray[8] = Task.Run(() => { I9Gfx.LoadClasses(); });                
                Task.WaitAll(taskArray);

                MidsContext.Config.Export.LoadCodes(Files.SelectDataFileLoad(Files.MxdbFileBbCodeUpdate));
                if (iFrm != null)
                {
                    iFrm.Opacity = 1.0;
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

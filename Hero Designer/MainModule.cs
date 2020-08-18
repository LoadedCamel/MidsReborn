using System;
using System.Drawing;
using System.Threading.Tasks;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

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

            public static void LoadData(ref frmInitializing iFrm)
            {
                DatabaseAPI.LoadDatabaseVersion();
                IsAppInitialized = true;
                iFrm?.SetMessage("Loading Data...");
                iFrm?.SetMessage("Loading Attribute Modifiers...");
                DatabaseAPI.Database.AttribMods = new Modifiers();
                if (!DatabaseAPI.Database.AttribMods.Load())
                {
                }

                iFrm?.SetMessage("Loading Powerset Database...");
                if (!DatabaseAPI.LoadLevelsDatabase())
                {
                    Interaction.MsgBox(
                        "Failed to load Leveling data file! The program is unable to proceed.\r\n" +
                        "We suggest you re-download the application from https://github.com/Reborn-Team/Hero-Designer/releases",
                        MsgBoxStyle.Critical, "Error");
                    ProjectData.EndApp();
                }

                if (!DatabaseAPI.LoadMainDatabase())
                {
                    Interaction.MsgBox("There was an error reading the database. Aborting.", MsgBoxStyle.Critical,
                        "Database Error");
                    ProjectData.EndApp();
                }

                if (!DatabaseAPI.LoadMaths())
                    ProjectData.EndApp();
                iFrm?.SetMessage("Loading Enhancement Database...");
                if (!DatabaseAPI.LoadEnhancementClasses())
                    ProjectData.EndApp();
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
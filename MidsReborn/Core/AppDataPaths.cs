using System.Diagnostics;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Core
{
    public static class AppDataPaths
    {
        public const string FileDb = "I12.mhd";
        public const string FileNLevels = "NLevels.mhd";
        public const string FileRLevels = "RLevels.mhd";
        public const string FileMaths = "Maths.mhd";
        public const string FileEClasses = "EClasses.mhd";
        public const string FileOrigins = "Origins.mhd";
        public const string FileSalvage = "Salvage.mhd";
        public const string FileRecipe = "Recipe.mhd";
        public const string FileEnhDb = "EnhDB.mhd";
        public const string FileBbCodeUpdate = "BBCode.mhd";
        public const string FileOverrides = "Compare.mhd";
        public const string FileModifiers = "AttribMod.mhd";
        public const string FileEffectIds = "GlobalMods.mhd";
        public const string FileGraphics = "I9.mhd";
        public const string FileSd = "SData.mhd";

        public const string ServerDataFile = "SData.json";
        public const string JsonFileModifiers = "AttribMod.json";
        public const string JsonFileTypeGrades = "TypeGrades.json";
        private const string JsonFileConfig = "appSettings.json";

        //public const string PatchRtf = "patch.rtf";
        private const string PowersReplTable = "PowersReplTable.mhd";
        private const string CrypticReplTable = "CrypticPowerNames.mhd";

        public const string ParentDatabaseFolder = "Databases\\";
        public const string ParentAssetsFolder = "Assets\\";
        public const string BuildsFolder = "Hero & Villain Builds\\";
        
        public static string FileData = string.Empty;
        public static string JsonConfig => Path.Combine(AppContext.BaseDirectory, JsonFileConfig);
        public static string? DefaultPath => Path.Combine(AppContext.BaseDirectory, ParentDatabaseFolder, "Homecoming\\");
        public static string PowersRepl => Path.Combine(AppDataPath, PowersReplTable);
        private static string? AppDataPath => MidsContext.Config.DataPath;
        public static string DefaultBuildsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), BuildsFolder);
        public static string CrypticPowersRepl => Path.Combine(AppDataPath, CrypticReplTable);

        public static string BaseDataPath => Path.Combine(AppContext.BaseDirectory, ParentDatabaseFolder);
        public static string BaseAssetsPath => Path.Combine(AppContext.BaseDirectory, ParentAssetsFolder);

        public static string SelectDataFileLoad(string iDataFile, string? iPath = "")
        {
            var filePath = Path.Combine(!string.IsNullOrWhiteSpace(iPath) ? iPath : AppDataPath, iDataFile);
            if (Debugger.IsAttached)
            {
                filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, ParentDatabaseFolder, DatabaseAPI.DatabaseName, iDataFile));
            }

            FileData += $"{filePath}\n";
            return filePath;
        }

        public static string SelectDataFileSave(string iDataFile, string? iPath = "")
        {
            var filePath = Path.Combine((!string.IsNullOrWhiteSpace(iPath) ? iPath : AppDataPath) ?? throw new InvalidOperationException(), iDataFile);
            if (!Debugger.IsAttached) return filePath;
            filePath = Path.Combine(Helpers.GetPathInDebug(), ParentDatabaseFolder, DatabaseAPI.DatabaseName, iDataFile);
            return filePath;
        }

        public static class Headers
        {
            public const string VersionComment = "Version:";

            internal static class Db
            {
                internal const string Start = "Mids Reborn Powers Database";
                internal const string Archetypes = "BEGIN:ARCHETYPES";
                internal const string Powersets = "BEGIN:POWERSETS";
                internal const string Powers = "BEGIN:POWERS";
                internal const string Summons = "BEGIN:SUMMONS";
            }

            internal static class EnhDb
            {
                internal const string Start = "Mids Reborn Enhancement Database";
            }

            internal static class Salvage
            {
                internal const string Start = "Mids Reborn Salvage Database";
            }

            internal static class Recipe
            {
                internal const string Start = "Mids Reborn Recipe Database";
            }

            internal static class AttribMod
            {
                internal const string Start = "Mids Reborn Attribute Modifier Tables";
            }

            internal static class ServerData
            {
                internal const string Start = "Mids Reborn Server Data";
            }

            internal static class TypeGrade
            {
                internal const string Start = "Mids Reborn Types and Grades";
            }

            public static class Save
            {
                public const string Compressed = "MRBz";
                public const string Uncompressed = "ToonDataVersion";
                public const string LegacyCompressed = "MHDz";
                public const string LegacyUncompressed = "HeroDataVersion";
            }
        }

        public static class Version
        {
            public const float Save = 1.4f;
            internal const float Config = 1.32f;
        }
    }
}
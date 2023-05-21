using System;
using System.Diagnostics;
using System.IO;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public static class Files
    {
        public const string MxdbFileDb = "I12.mhd";
        public const string MxdbFileNLevels = "NLevels.mhd";
        public const string MxdbFileRLevels = "RLevels.mhd";
        public const string MxdbFileMaths = "Maths.mhd";
        public const string MxdbFileEClasses = "EClasses.mhd";
        public const string MxdbFileOrigins = "Origins.mhd";
        public const string MxdbFileSalvage = "Salvage.mhd";
        public const string MxdbFileRecipe = "Recipe.mhd";
        public const string MxdbFileEnhDb = "EnhDB.mhd";
        public const string MxdbFileBbCodeUpdate = "BBCode.mhd";
        public const string MxdbFileOverrides = "Compare.mhd";
        public const string MxdbFileModifiers = "AttribMod.mhd";
        public const string MxdbFileEffectIds = "GlobalMods.mhd";
        public const string MxdbFileSd = "SData.mhd";
        public const string MxdbFileGraphics = "I9.mhd";

        public const string JsonFileModifiers = "AttribMod.json";
        public const string JsonFileTypeGrades = "TypeGrades.json";
        private const string JsonFileConfig = "appSettings.json";

        //public const string PatchRtf = "patch.rtf";
        private const string MxdbPowersReplTable = "PowersReplTable.mhd";
        private const string MxdbCrypticReplTable = "CrypticPowerNames.mhd";

        public const string RoamingFolder = "Data\\";
        public const string BuildsFolder = "Hero & Villain Builds\\";
        
        public static string FileData = string.Empty;
        public static string FNameJsonConfig => Path.Combine(AppContext.BaseDirectory, JsonFileConfig);
        public static string? FDefaultPath => Path.Combine(AppContext.BaseDirectory, RoamingFolder, "Generic\\");
        public static string FNamePowersRepl => Path.Combine(FPathAppData, MxdbPowersReplTable);
        private static string? FPathAppData => MidsContext.Config.DataPath;
        public static string FDefaultBuildsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), BuildsFolder);
        public static string CNamePowersRepl => Path.Combine(FPathAppData, MxdbCrypticReplTable);

        public static string BaseDataPath => Path.Combine(AppContext.BaseDirectory, RoamingFolder);

        public static string SelectDataFileLoad(string iDataFile, string? iPath = "")
        {
            var filePath = Path.Combine(!string.IsNullOrWhiteSpace(iPath) ? iPath : FPathAppData, iDataFile);
            if (Debugger.IsAttached)
            {
                filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, RoamingFolder, DatabaseAPI.DatabaseName, iDataFile));
            }

            FileData += $"{filePath}\n";
            return filePath;
        }

        public static string SelectDataFileSave(string iDataFile, string? iPath = "")
        {
            var filePath = Path.Combine(!string.IsNullOrWhiteSpace(iPath) ? iPath : FPathAppData, iDataFile);
            if (!Debugger.IsAttached) return filePath;
            filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..", RoamingFolder, DatabaseAPI.DatabaseName, iDataFile));
            if (!Directory.Exists(FileIO.StripFileName(filePath)))
            {
                Directory.CreateDirectory(FileIO.StripFileName(filePath));
            }

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
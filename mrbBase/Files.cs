using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mrbBase.Base.Master_Classes;

namespace mrbBase
{
    public static class Files
    {
        public const string MxdbFileDB = "I12.mhd";
        public const string MxdbFileLevels = "Levels.mhd";
        public const string MxdbFileNLevels = "NLevels.mhd";
        public const string MxdbFileRLevels = "RLevels.mhd";
        public const string MxdbFileMaths = "Maths.mhd";
        public const string MxdbFileEClasses = "EClasses.mhd";
        public const string MxdbFileOrigins = "Origins.mhd";
        public const string MxdbFileSetTypes = "SetTypes.mhd";
        public const string MxdbFileSalvage = "Salvage.mhd";
        public const string MxdbFileRecipe = "Recipe.mhd";
        public const string MxdbFileEnhDB = "EnhDB.mhd";
        public const string MxdbFileBbCodeUpdate = "BBCode.mhd";
        public const string MxdbFileOverrides = "Compare.mhd";
        public const string MxdbFileModifiers = "AttribMod.mhd";
        public const string MxdbFileLookups = "AttribLookup.mhd";
        public const string MxdbFileEffectIds = "GlobalMods.mhd";
        public const string MxdbFileGraphics = "I9.mhd";
        public const string JsonFileModifiers = "AttribMod.json";
        //public const string PatchRtf = "patch.rtf";
        private const string MxdbFileConfig = "Config.mhd";
        private const string JsonFileConfig = "Config.json";
        private const string JsonFileConfigSP = "ConfigSP.json";
        private const string MxdbPowersReplTable = "PowersReplTable.mhd";

        public const string RoamingFolder = "Data\\";
        public static string FileData = string.Empty;

        public static string FNameJsonConfig => Path.Combine(Path.Combine(GetAssemblyLoc(), RoamingFolder), JsonFileConfig);

        public static string GetConfigSpFile()
        {
            var sPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\RebornTeam\";
            return Path.Combine(sPath, JsonFileConfigSP);
        }

        public static string FDefaultPath => Path.Combine(Path.Combine(GetAssemblyLoc(), RoamingFolder), "Homecoming\\");

        private static string FNameConfig => Path.Combine(Path.Combine(GetAssemblyLoc(), RoamingFolder), MxdbFileConfig);

        public static string FNamePowersRepl => Path.Combine(FPathAppData, MxdbPowersReplTable);

        private static string FPathAppData => MidsContext.Config.DataPath;

        internal static string SearchUp(string folder, string fn)
        {
            string SearchUpRec(string foldername, string filename)
            {
                // if it is not properly rooted, give up, and use the path
                if (!Path.IsPathRooted(filename))
                {
                    Console.WriteLine(filename);
                    return filename;
                }

                // get the directory that holds the filename, filename should be a FULL path
                var fnDir = Path.GetDirectoryName(filename);
                // if the filename is already in a folder with the correct foldername, we need to go up twice instead of once.
                string targetRoot;
                if (fnDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).EndsWith(foldername))
                {
                    targetRoot = Path.GetDirectoryName(fnDir);
                    Console.WriteLine($"Targetroot: {targetRoot}");
                }
                else
                {
                    targetRoot = fnDir;
                    Console.WriteLine($"Targetroot: {targetRoot}");
                }

                var parent = Path.GetDirectoryName(targetRoot);
                Console.WriteLine($"Parent: {parent}");
                if (parent == null) return null;
                var attempt = Path.Combine(Path.Combine(parent, foldername), Path.GetFileName(filename));
                Console.WriteLine($"attempt: {attempt}");
                if (File.Exists(attempt))
                    return attempt;
                //if (Path.IsPathRooted(attempt)) return filename;
                var recursed = SearchUpRec(foldername, attempt);
                Console.WriteLine($"recursed: {recursed}");
                if (recursed != null && File.Exists(recursed)) return recursed;
                return File.Exists(filename) ? filename : null;
            }

            try
            {
                var result = SearchUpRec(folder, fn);
                return result != null && File.Exists(result) ? result : fn;
            }
            catch
            {
                return fn;
            }
        }

        /*public static string SelectDataFileLoad(string iDataFile)
        {
            var str = Path.Combine(FPathAppData, iDataFile);
            if (Debugger.IsAttached)
                str = SearchUp("Data", str);
            FileData = FileData + str + '\n';
            return str;
        }*/

        public static string GetAssemblyLoc()
        {
            var asmLoc = Assembly.GetExecutingAssembly().Location;
            var dirLoc = $"{Directory.GetParent(asmLoc)}\\";
            return dirLoc;
        }

        public static string SelectDataFileLoad(string iDataFile, string iPath = "")
        {
            var filePath = Path.Combine(!string.IsNullOrWhiteSpace(iPath) ? iPath : FPathAppData, iDataFile);
            if (Debugger.IsAttached)
            {
                filePath = Path.GetFullPath(Path.Combine(Path.Combine(AppContext.BaseDirectory, @"..\..\"), RoamingFolder, DatabaseAPI.DatabaseName, iDataFile));
            }

            FileData = FileData + filePath + '\n';
            return filePath;
        }

        public static string SelectDataFileSave(string iDataFile, string iPath = "")
        {
            var filePath = Path.Combine(!string.IsNullOrWhiteSpace(iPath) ? iPath : FPathAppData, iDataFile);
            if (Debugger.IsAttached)
            {
                filePath = Path.GetFullPath(Path.Combine(Path.Combine(AppContext.BaseDirectory, @"..\..\"), RoamingFolder, DatabaseAPI.DatabaseName, iDataFile));
                if (!Directory.Exists(FileIO.StripFileName(filePath)))
                    Directory.CreateDirectory(FileIO.StripFileName(filePath));
            }
            return filePath;
        }

        /*public static string SelectDataFileSave(string iDataFile)
        {
            try
            {
                var str = Path.Combine(FPathAppData, iDataFile);
                if (Debugger.IsAttached)
                    str = SearchUp("Data", str);
                if (!Directory.Exists(FileIO.StripFileName(str)))
                    Directory.CreateDirectory(FileIO.StripFileName(str));
                return str;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create output folder: " + ex.Message);
            }

            return string.Empty;
        }*/

        internal static string GetConfigFilename(bool forceMhd)
        {
            switch (forceMhd)
            {
                case false when File.Exists(FNameJsonConfig):
                {
                    return FNameJsonConfig;
                }
                case false when !File.Exists(FNameJsonConfig):
                {
                    MessageBox.Show(@"Config file doesn't exist, generating a new one.");
                    return FNameJsonConfig;
                }
                case true when File.Exists(FNameConfig):
                {
                    return FNameConfig;
                }
            }

            return FNameConfig;
        }

        internal static string SelectConfigFileSave()
        {
            try
            {
                if (!Directory.Exists(FileIO.StripFileName(FNameConfig)))
                {
                    Directory.CreateDirectory(FileIO.StripFileName(FNameConfig));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create output folder: " + ex.Message);
            }

            return FNameConfig;
        }

        public static class Headers
        {
            public const string VersionComment = "Version:";

            internal static class DB
            {
                internal const string Start = "Mids' Hero Designer Database MK II";
                internal const string Archetypes = "BEGIN:ARCHETYPES";
                internal const string Powersets = "BEGIN:POWERSETS";
                internal const string Powers = "BEGIN:POWERS";
                internal const string Summons = "BEGIN:SUMMONS";
            }

            internal static class EnhDB
            {
                internal const string Start = "Mids' Hero Designer Enhancement Database";
            }

            internal static class Salvage
            {
                internal const string Start = "Mids' Hero Designer Salvage Database";
            }

            internal static class Recipe
            {
                internal const string Start = "Mids' Hero Designer Recipe Database";
            }

            internal static class AttribMod
            {
                internal const string Start = "Mids' Hero Designer Attribute Modifier Tables";
            }

            public static class Save
            {
                public const string Compressed = "MHDz";
                public const string Uncompressed = "HeroDataVersion";
            }
        }

        public static class Version
        {
            public const float Save = 1.4f;
            internal const float Config = 1.32f;
        }
    }
}
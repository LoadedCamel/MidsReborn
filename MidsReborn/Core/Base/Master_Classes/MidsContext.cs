using System;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    public static class MidsContext
    {
        public const string AppName = "Mids' Reborn";
        private const int AppMajorVersion = 3;
        private const int AppMinorVersion = 6;
        private const int AppBuildVersion = 5;
        private const int AppRevisionVersion = 3;

        public const string AssemblyVersion = "3.6.5";
        public const string AssemblyFileVersion = "3.6.5.3";
        public static Version AppFileVersion { get; set; } = new(AppMajorVersion, AppMinorVersion, AppBuildVersion, AppRevisionVersion);

        public const string AppVersionStatus = "";
        public const string Title = "Mids' Reborn";

        public const int MathLevelBase = 49;
        public const int MathLevelExemp = -1;

        internal static bool EnhCheckMode = false;

        //public static readonly Version AppVersion = new Version(AppMajorVersion, AppMinorVersion, AppBuildVersion, AppRevisionVersion);

        internal static Archetype? Archetype;
        internal static Character? Character;
        internal static Build? Build;

        public static ConfigData Config => ConfigData.Current;
    }
}
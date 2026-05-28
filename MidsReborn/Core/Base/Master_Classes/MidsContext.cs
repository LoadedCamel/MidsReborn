using System;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    public static class MidsContext
    {
        public const string AppName = "Mids Reborn";
        private const int AppMajorVersion = 4;
        private const int AppMinorVersion = 0;
        private const int AppBuildVersion = 3;
        private const int AppRevisionVersion = 5;
        public const string AssemblyVersion = "4.0.3";
        public const string AssemblyFileVersion = "4.0.3.5";
      
        public static Version AppFileVersion { get; set; } = new(AppMajorVersion, AppMinorVersion, AppBuildVersion, AppRevisionVersion);

        public const string AppVersionStatus = "";
        public const string Title = "Mids Reborn";

        public const int MathLevelBase = 49;
        public const int MathLevelExemp = -1;

        internal static bool EnhCheckMode = false;

        internal static Archetype? Archetype;
        internal static Character? Character;
        internal static Build? Build;

        public static ConfigData? Config => ConfigData.Current;
    }
}
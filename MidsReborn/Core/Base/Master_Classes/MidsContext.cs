using System;
using System.Reflection;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    public static class MidsContext
    {
        public const string AppName = "Mids Reborn";
        public const string Title = "Mids Reborn";

        private static readonly Assembly ThisAssembly = typeof(MidsContext).Assembly;

        public static string AssemblyVersion => GetInformationalVersion();

        public static string AssemblyFileVersion => AppFileVersion.ToString();

        public static Version AppFileVersion { get; } = GetFileVersion();

        public const string AppVersionStatus = "";

        public const int MathLevelBase = 49;
        public const int MathLevelExemp = -1;

        internal static bool EnhCheckMode = false;

        internal static Archetype? Archetype;
        internal static Character? Character;
        internal static Build? Build;

        public static ConfigData? Config => ConfigData.Current;

        private static string GetInformationalVersion()
        {
            var informationalVersion = ThisAssembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                return informationalVersion.Split('+')[0];
            }

            return $"{AppFileVersion.Major}.{AppFileVersion.Minor}.{AppFileVersion.Build}";
        }

        private static Version GetFileVersion()
        {
            var fileVersion = ThisAssembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                ?.Version;

            if (Version.TryParse(fileVersion, out var parsedVersion))
            {
                return parsedVersion;
            }

            return ThisAssembly.GetName().Version ?? new Version(0, 0, 0, 0);
        }
    }
}

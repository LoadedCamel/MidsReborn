using System;
using mrbBase.Base.Data_Classes;

namespace mrbBase.Base.Master_Classes
{
    public static class MidsContext
    {
        public const string AppName = "Mids' Reborn";
        private const int AppMajorVersion = 3;
        private const int AppMinorVersion = 2;
        private const int AppBuildVersion = 17;
        private const int AppRevisionVersion = 72;

        public const string AssemblyVersion = "3.2.17";
        public const string AssemblyFileVersion = "3.2.17.72";
        public static Version AppFileVersion { get; set; } = new(AppMajorVersion, AppMinorVersion, AppBuildVersion, AppRevisionVersion);

        public const string AppVersionStatus = "";
        public const string Title = "Mids' Reborn";

        public const int MathLevelBase = 49;
        public const int MathLevelExemp = -1;

        public static bool EnhCheckMode = false;

        //public static readonly Version AppVersion = new Version(AppMajorVersion, AppMinorVersion, AppBuildVersion, AppRevisionVersion);

        public static Archetype Archetype;
        public static Character Character;
        public static Build Build;

        public static ConfigData Config => ConfigData.Current;

        public static ConfigDataSpecial ConfigSp => ConfigDataSpecial.Current;

        public static string GetCryptedValue(string type, string name)
        {
            switch (type)
            {
                case "Auth":
                    if (ConfigSp.Auth == null) return null;
                    ConfigSp.Auth.TryGetValue(name, out var authValue);
                    return authValue?.ToString();
                case "User":
                    if (ConfigSp.User == null) return null;
                    ConfigSp.User.TryGetValue(name, out var userValue);
                    return userValue?.ToString();
                case "BotUser":
                    if (ConfigSp.BotUser == null) return null;
                    ConfigSp.BotUser.TryGetValue(name, out var botValue);
                    return botValue?.ToString();
                default:
                    return null;
            }
        }
    }
}
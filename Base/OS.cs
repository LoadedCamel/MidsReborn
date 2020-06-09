
using System;
using System.Windows.Forms;

public static class OS
{
    const int Windows2000Major = 5;

    const int Windows2000Minor = 0;

    const int WindowsXpMajor = 5;

    const int WindowsXpMinor = 1;

    const int Windows2003Major = 5;

    const int Windows2003Minor = 2;

    const int WindowsVistaMajor = 6;

    const int WindowsVistaMinor = 0;

    public const string SaveFolderName = "Hero & Villain Builds";

    static string AddSlash(string iPath)

    {
        return !iPath.EndsWith("\\") ? iPath + "\\" : iPath;
    }

    static WindowsVersion GetWindowsVersion()

    {
        return Environment.OSVersion.Platform != PlatformID.Unix ? (Environment.OSVersion.Version.Major >= 5 ? (!(Environment.OSVersion.Version.Major == 5 & Environment.OSVersion.Version.Minor == 0) ? (!(Environment.OSVersion.Version.Major == 5 & Environment.OSVersion.Version.Minor == 1) ? (!(Environment.OSVersion.Version.Major == 5 & Environment.OSVersion.Version.Minor == 2) ? (!(Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor == 0) ? (!(Environment.OSVersion.Version.Major >= 6 & Environment.OSVersion.Version.Minor >= 0) ? WindowsVersion.WinXP : WindowsVersion.NewerThanVista) : WindowsVersion.Vista) : WindowsVersion.Win2K3) : WindowsVersion.WinXP) : WindowsVersion.Win2K) : WindowsVersion.OlderThan2K) : WindowsVersion.WinXP;
    }

    public static string GetQuickOsid()
    {
        WindowsVersion windowsVersion = GetWindowsVersion();
        return Enum.GetName(windowsVersion.GetType(), windowsVersion);
    }

    public static string GetMyDocumentsPath()
    {
        return AddSlash(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
    }

    public static string GetDefaultSaveFolder()
    {
        return GetMyDocumentsPath() + "Hero & Villain Builds\\";
    }

    public static string GetApplicationPath()
    {
        return AddSlash(Application.StartupPath);
    }

    public static string VistaUacErrorText()
    {
        //TODO: this isn't being used, but if we find a reason to use it, the app name needs to be updated
        string str1 = "In order for installation and updates to function correctly," + '\n' + "you may need to set the application's shortcut to run as an adminstrator." + '\n' + '\n' + "To do this, right-click on the shortcut to Mids' Hero Designer and select Properties->Compatability->Always run as Administrator.";
        var str2 = GetWindowsVersion() switch
        {
            WindowsVersion.Vista => (20 + "IMPORTANT: You are running Windows Vista! " + str1),
            WindowsVersion.NewerThanVista => (20 + "IMPORTANT: You are running an unidentified version of Windows! " +
                                              str1),
            _ => ""
        };
        return str2;
    }

    enum WindowsVersion

    {
        OlderThan2K,
        Win2K,
        WinXP,
        Win2K3,
        Vista,
        NewerThanVista
    }
}

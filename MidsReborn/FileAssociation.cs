using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mids_Reborn
{
    // 2 possibilities, the actual direct association, and the open with list options
    public static class FileAssociation
    {
        // Reg key must exist (should be set with the installer)
        //private const string HKCRMxdFileShellNew = @"HKEY_CLASSES_ROOT\.mxd\RebornTeam.Mids Reborn.mxd\ShellNew";

        // To mimic HKCR equivalent at a user-level
        private const string HKCUMxdFileShellNew = @"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd\RebornTeam.Mids Reborn.mxd\ShellNew";

        // Reg key must exist (should be set with the installer)
        // Default value must be "<MRBInstallPath>\MidsReborn.exe" "%1"
        private const string HKCURebornTeamMxdOpen = @"HKEY_CURRENT_USER\SOFTWARE\Classes\RebornTeam.Mids Reborn.mxd\shell\open\command";

        // Reg key must exist, installer may set this to a different value
        // Must be <MRBInstallPath>\MidsReborn.exe,0
        private const string HKCURebornTeamMxdIcon = @"HKEY_CURRENT_USER\SOFTWARE\Classes\RebornTeam.Mids Reborn.mxd\DefaultIcon";

        private static RegistryKey OpenRegistryKey(string regPath, bool writable=false)
        {
            if (regPath.StartsWith(@"HKEY_CLASSES_ROOT\") || regPath.StartsWith(@"HKCR\"))
            {
                return Registry.ClassesRoot.OpenSubKey(regPath.Replace(@"HKEY_CLASSES_ROOT\", "").Replace(@"HKCR\", ""), writable);
            }

            if (regPath.StartsWith(@"HKEY_CURRENT_USER\") || regPath.StartsWith(@"HKCU\"))
            {
                return Registry.CurrentUser.OpenSubKey(regPath.Replace(@"HKEY_CURRENT_USER\", "").Replace(@"HKCU\", ""), writable);
            }

            if (regPath.StartsWith(@"HKEY_LOCAL_MACHINE\") || regPath.StartsWith(@"HKLM\"))
            {
                return Registry.LocalMachine.OpenSubKey(regPath.Replace(@"HKEY_LOCAL_MACHINE\", "").Replace(@"HKLM\", ""), writable);
            }

            throw new ArgumentException($"{regPath} was in a not allowed format.");
        }
        
        private static bool CheckRegKeyExists(string regPath)
        {
            var regOpenKey = OpenRegistryKey(regPath);
            return regOpenKey != null;
        }

        public static bool CheckAssociations()
        {
            var regExistsHKCU = true;
            regExistsHKCU &= CheckRegKeyExists(HKCUMxdFileShellNew);
            regExistsHKCU &= CheckRegKeyExists(HKCURebornTeamMxdIcon);
            regExistsHKCU &= CheckRegKeyExists(HKCURebornTeamMxdOpen);

            return regExistsHKCU;
        }

        public static string CheckdAssociatedProgram()
        {
            if (!CheckRegKeyExists(HKCURebornTeamMxdOpen)) return "";
            var rk = OpenRegistryKey(HKCURebornTeamMxdOpen);
            var v = rk.GetValue(null).ToString();

            return v.Replace(" \"%1\"", "").TrimStart('\"');
        }

        public static bool SetAssociations()
        {
            try
            {
                if (!CheckRegKeyExists(HKCUMxdFileShellNew))
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes", true);
                    rk.CreateSubKey(@".mxd\RebornTeam.Mids Reborn.mxd\ShellNew");
                }
                
                if (!CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\RebornTeam.Mids Reborn.mxd"))
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes", true);
                    rk.CreateSubKey("RebornTeam.Mids Reborn.mxd");
                }
                
                if (!CheckRegKeyExists(HKCURebornTeamMxdIcon))
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\RebornTeam.Mids Reborn.mxd", true);
                    rk.CreateSubKey("DefaultIcon");
                }

                if (!CheckRegKeyExists(HKCURebornTeamMxdOpen))
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\RebornTeam.Mids Reborn.mxd", true);
                    rk.CreateSubKey(@"shell\open\command");
                }

                OpenRegistryKey(HKCURebornTeamMxdIcon, true).SetValue(null, $"{Application.ExecutablePath},0");
                OpenRegistryKey(HKCURebornTeamMxdOpen, true).SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");

                // Tell explorer the file association has been changed
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
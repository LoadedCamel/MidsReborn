using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mids_Reborn
{
    // 2 possibilities, the actual direct association, and the open with list options
    public static class FileAssociation
    {
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
            var regExistsHKCU = CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd");
            regExistsHKCU &= CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + GetMxdType() + @"\DefaultIcon");
            regExistsHKCU &= CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + GetMxdType() + @"\shell\open\command");

            return regExistsHKCU;
        }

        private static string GetMxdType()
        {
            return CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd")
                ? OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd").GetValue(null).ToString()
                : "RebornTeam.Mids Reborn";
        }
        public static string CheckdAssociatedProgram()
        {
            var mxdOpenKey = @"HKEY_CURRENT_USER\SOFTWARE\Classes\" + GetMxdType();
            if (!CheckRegKeyExists(mxdOpenKey)) return "";
            if (!CheckRegKeyExists(mxdOpenKey + @"\shell\open\command")) return "";
            var rk = OpenRegistryKey(mxdOpenKey + @"\shell\open\command");
            var v = rk.GetValue(null).ToString();
            var vc = rk.GetValue("command");
            var vcmd = vc == null ? "" : vc.ToString();
            if (vc != null & !vcmd.Contains("\"%1\"")) return "";

            return v.Replace(" \"%1\"", "").Trim('\"');
        }

        public static bool SetAssociations()
        {
            try
            {
                var mxdTypeKeyExists = CheckRegKeyExists(@"HKEY_CURRENT_USER\Classes\.mxd");
                var mxdType = GetMxdType();

                if (!mxdTypeKeyExists)
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\Classes", true);
                    rk.CreateSubKey(@".mxd\" + mxdType + @"\ShellNew");
                    rk.Close();
                }

                if (!CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + mxdType))
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes", true);
                    rk.CreateSubKey(mxdType + @"\DefaultIcon");
                    rk.CreateSubKey(mxdType + @"shell\open\command");
                    rk.Close();
                }

                if (OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + mxdType + @"\shell\open\command").GetValue("command") == null)
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + mxdType + @"\shell\open\command", true);
                    rk.SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");
                    rk.Close();
                }
                else
                {
                    var rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + mxdType + @"\shell\open\command", true);
                    rk.DeleteValue("command");
                    rk.SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");
                    rk.Close();
                }

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
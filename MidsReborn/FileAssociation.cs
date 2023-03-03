#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mids_Reborn
{
    // 2 possibilities, the actual direct association, and the open with list options
    public static class FileAssociation
    {
        private static RegistryKey? OpenRegistryKey(string regPath, bool writable=false)
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
            var regExistsHkcu = CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd");
            regExistsHkcu &= CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + GetMxdType() + @"\DefaultIcon");
            regExistsHkcu &= CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\" + GetMxdType() + @"\shell\open\command");

            return regExistsHkcu;
        }

        private static string? GetMxdType()
        {
            const string? typeName = "LoadedCamel.Mids Reborn";
            
            if (!CheckRegKeyExists(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd"))
            {
                return typeName;
            }

            var ret = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd").GetValue(null);
            return ret == null ? typeName : ret.ToString();
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
                RegistryKey? rk;
                var mxdTypeKeyExists = CheckRegKeyExists(@"HKEY_CURRENT_USER\Classes\.mxd");
                var mxdType = GetMxdType();

                if (!mxdTypeKeyExists)
                {
                    Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd");
                    Registry.CurrentUser.CreateSubKey($@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd\{mxdType}");
                    Registry.CurrentUser.CreateSubKey($@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd\{mxdType}\ShellNew");
                    Registry.CurrentUser.CreateSubKey($@"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}");
                }

                rk = OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Classes\.mxd", true);
                if (mxdType != null)
                {
                    rk?.SetValue(null, mxdType);
                    rk?.Close();

                    rk = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{mxdType}\DefaultIcon");
                    rk.SetValue(null, @$"{Application.ExecutablePath},0");

                    rk = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{mxdType}\shell\open\command");
                    rk.SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");

                    rk = OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}", true);
                    rk?.SetValue(null, "Mids Reborn Character Build File");

                    rk = OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}\shell", true);
                    rk?.SetValue(null, "open");
                    rk?.Close();

                    rk = OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}\shell\open", true);
                    rk?.SetValue(null, "&Open");
                    rk?.Close();

                    if (OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}\shell\open\command")
                            .GetValue("command") == null)
                    {
                        rk = OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}\shell\open\command", true);
                        rk?.SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");
                        rk?.Close();
                    }
                    else
                    {
                        rk = OpenRegistryKey(@$"HKEY_CURRENT_USER\SOFTWARE\Classes\{mxdType}\shell\open\command", true);
                        rk?.DeleteValue("command");
                        rk?.SetValue(null, $"\"{Application.ExecutablePath}\" \"%1\"");
                        rk?.Close();
                    }
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
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Mids_Reborn.Core
{
    /// <summary>
    /// A static class that can be used for file and schema association checks or assignment.
    /// </summary>
    internal static class Association
    {
        private const string BasePath = "Software\\Classes\\";
        private const string SubPath = "shell\\open\\command";
        private const string Legacy = @".mxd";
        private const string Current = @".mbd";
        private const string Schema = @"MidsReborn.URL";

        internal static bool FileTypeScan()
        {
            var mbdAssociated = false;
            var mxdAssociated = false;
            var legacy = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                .OpenSubKey(Path.Combine(BasePath, Legacy));
            var value = legacy?.OpenSubKey(SubPath)?.GetValue(string.Empty)?.ToString();
            if (value != null)
            {
                if (value.Contains(Application.ExecutablePath))
                {
                    mxdAssociated = true;
                }
            }

            var current = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                .OpenSubKey(Path.Combine(BasePath, Current));
            value = current?.OpenSubKey(SubPath)?.GetValue(string.Empty)?.ToString();
            if (value == null) return mbdAssociated && mxdAssociated;
            if (value.Contains(Application.ExecutablePath))
            {
                mbdAssociated = true;
            }

            return mbdAssociated && mxdAssociated;
        }

        internal static bool SchemaScan()
        {
            var schema = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(Path.Combine(BasePath, Schema));
            var value = schema?.OpenSubKey(SubPath)?.GetValue(string.Empty)?.ToString();
            return value != null && value.Contains(Application.ExecutablePath);
        }

        internal static bool RepairFileTypes()
        {
            try
            {
                var legacy = Registry.CurrentUser.CreateSubKey(Path.Combine(BasePath, Legacy));
                legacy.CreateSubKey("DefaultIcon")
                    .SetValue("", @$"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")},0");
                legacy.CreateSubKey(SubPath).SetValue("",
                    $"\"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")}\" \"%1\"");
                legacy.Close();

                var current = Registry.CurrentUser.CreateSubKey(Path.Combine(BasePath, Current));
                current.CreateSubKey("DefaultIcon")
                    .SetValue("", @$"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")},0");
                current.CreateSubKey(SubPath).SetValue("",
                    $"\"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")}\" \"%1\"");
                current.Close();

                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal static bool RepairSchema()
        {
            try
            {
                var schema = Registry.CurrentUser.CreateSubKey(Path.Combine(BasePath, Schema));
                schema.SetValue("", "URL:Mids Reborn Protocol");
                schema.SetValue("URL Protocol", "");
                schema.CreateSubKey("DefaultIcon")
                    .SetValue("", @$"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")},0");
                schema.CreateSubKey(SubPath).SetValue("",
                    $"\"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")}\" \"%1\"");
                schema.Close();

                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Mids_Reborn.Core.Utils
{
    /// <summary>
    /// Used to assign/repair file associations.
    /// Note: Should only be needed for archive installations.
    /// </summary>
    public static class Repair
    {
        public static void FileExtensions()
        {
            var mxdFile = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.mxd");
            mxdFile.CreateSubKey("DefaultIcon").SetValue("", @$"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")},0");
            mxdFile.CreateSubKey("shell\\open\\command").SetValue("", $"\"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")}\" \"%1\"");
            mxdFile.Close();

            var mbdFile = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.mbd");
            mbdFile.CreateSubKey("DefaultIcon").SetValue("", @$"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")},0");
            mbdFile.CreateSubKey("shell\\open\\command").SetValue("", $"\"{Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe")}\" \"%1\"");
            mbdFile.Close();

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}

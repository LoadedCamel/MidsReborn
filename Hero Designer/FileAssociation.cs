using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Base.Master_Classes;
using Microsoft.Win32;

namespace Hero_Designer
{
    // 2 possibilities, the actual direct association, and the open with list options
    public static class FileAssociation
    {
        const string MRUListValueName = "MRUList";
        const string FileKeyName = "Mids_Reborn_File";
        const string FileDescription = "Mids Reborn File";
        //So basically the issue is that the open command doesn't always get properly set in the registry. The registry entry you need to modify is:
        //HKEY_CURRENT_USER\Software\Classes\Applications\Hero Designer.exe\shell\open\command

        //If that either doesn't have a value or is set to the wrong value change it to the correct path and then try again. Alternatively you can modify the file association directly using the registry key:
        //HKEY_CURRENT_USER\Software\Classes\mxd_auto_file\shell\open\command

        // found in registry win 10: Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.mxd\OpenWithList
        static string GetOpenListSubKeyPath(string extension) => $@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\OpenWithList";
        // could take multiple open with options
        static RegistryKey GetFileOpenWithListKey(string extension)
            => Registry.CurrentUser.OpenSubKey(GetOpenListSubKeyPath(extension));

        //static RegistryKey FindOpenWithEntry(string extension, string path)
        //    => GetFileOpenWithList(extension).GetSubKeyNames();
        public enum AddToOpenResult
        {
            Unauthorized,
            Success
        }
        static bool CheckSubKeyValue(RegistryKey parentKey, string subkeyPath, Func<RegistryKey, bool> keyCheckIfExists)
        {
            using var subkey = parentKey.OpenSubKey(subkeyPath, writable: false);
            
            return subkey != null && keyCheckIfExists(subkey);
        }

        public static bool GetIsAssociated(string fullExePath) =>
            GetIsAssociated(".mhd", FileKeyName, fullExePath, FileDescription)
            && GetIsAssociated(".mxd", FileKeyName, fullExePath, FileDescription);

        static bool GetIsAssociated(string extension, string keyName, string openWith, string fileDescription)
        {
            using (var baseKey = Registry.ClassesRoot.OpenSubKey(extension, writable: false))
            {
                if (baseKey == null) return false;
                if ((string)baseKey.GetValue("") != keyName) return false;
            }
            using (var openMethod = Registry.ClassesRoot.OpenSubKey(keyName, writable: false))
            {
                if (openMethod == null) return false;
                if ((string)openMethod.GetValue("") != fileDescription) return false;
                using (var shell = openMethod.OpenSubKey("Shell"))
                {
                    if (shell == null) return false;
                    var expected = "\"" + openWith + "\"" + " \"%1\"";
                    if (!CheckSubKeyValue(shell, "edit\\command", key => (string)key.GetValue("") == expected)) return false;
                    if (!CheckSubKeyValue(shell, "open\\command", key => (string)key.GetValue("") == expected)) return false;
                }
            }

            using (var uc = Registry.CurrentUser.OpenSubKey($@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\UserChoice", writable: false))
            {
                if ((string)uc.GetValue("Progid") != keyName) return false;
            }

            return true;
        }

        public static AddToOpenResult AddToOpenWithLists(string fullExePath)
        {
            try
            {
                CheckAddFileOpenWithListItem(".mhd");
                CheckAddFileOpenWithListItem(".mxd");
                SetAssociation(".mhd", FileKeyName, fullExePath, FileDescription);
                SetAssociation(".mxd", FileKeyName, fullExePath, FileDescription);
                return AddToOpenResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return AddToOpenResult.Unauthorized;
            }
        }

        // only really works if the entry is already present(setup for OpenWith.exe at least)
        static bool CheckAddFileOpenWithListItem(string extension)
        {
            // doesn't do enough work if it has to create, not implemented
            //using (var owlKey = CreateOrOpenRegOpenWithList(extension))
            using (var owlKey = GetFileOpenWithListKey(extension))
            {
                if (owlKey == null) return false;
                var mru = owlKey.GetValue(MRUListValueName);
                var itemNames = owlKey.GetValueNames().Where(name => name != null && name != "(Default)" && name != MRUListValueName).ToArray();
                // already present
                if (itemNames.Any(name => string.Equals(MidsContext.AssemblyName, (string)owlKey.GetValue(name), StringComparison.Ordinal))) return false;
                // error condition-ish
                if (itemNames.Any(name => name.Length > 1))
                    return false;
                var nextLetter = itemNames.Any() ? (char)(itemNames.Max()[0] + 1) : 'a';
                if (itemNames.Length == 0)
                {

                }

                var mruListValue = (string)owlKey.GetValue(MRUListValueName);
                // error condition
                if (mruListValue != null && mruListValue.Contains(nextLetter))
                    return false;
                owlKey.SetValue(nextLetter.ToString(), MidsContext.AssemblyName);
                owlKey.SetValue(MRUListValueName, nextLetter + mruListValue);
            }
            return true;
        }
        //public bool IsAssociatedWithThisInstall()
        //{
        //    using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications\Hero Designer.exe"))
        //    {
        //        if (key == null) return false;
        //        return false;
        //    }

        //}
        // https://stackoverflow.com/questions/2681878/associate-file-extension-with-application
        // example : SetAssociation(".ucs", "UCS_Editor_File", Application.ExecutablePath, "UCS File"); 
        static void SetAssociation(string extension, string keyName, string openWith, string fileDescription)
        {
            var baseKey = Registry.ClassesRoot.CreateSubKey(extension);
            baseKey?.SetValue("", keyName);

            var openMethod = Registry.ClassesRoot.CreateSubKey(keyName);
            openMethod?.SetValue("", fileDescription);
            openMethod?.CreateSubKey("DefaultIcon")?.SetValue("", "\"" + openWith + "\",0");
            var shell = openMethod?.CreateSubKey("Shell");
            shell?.CreateSubKey("edit")?.CreateSubKey("command")?.SetValue("", "\"" + openWith + "\"" + " \"%1\"");
            shell?.CreateSubKey("open")?.CreateSubKey("command")?.SetValue("", "\"" + openWith + "\"" + " \"%1\"");
            baseKey?.Close();
            openMethod?.Close();
            shell?.Close();

            var currentUser = Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension);
            if (currentUser != null)
            {
                using var uc =
                    currentUser.OpenSubKey("UserChoice", RegistryKeyPermissionCheck.ReadWriteSubTree,
                        RegistryRights.FullControl) ?? currentUser.CreateSubKey("UserChoice", true);
                uc.SetValue("Progid", keyName, RegistryValueKind.String);
            }

            // Delete the key instead of trying to change it
            currentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + extension, true);
            if (currentUser != null)
            {
                currentUser.DeleteSubKey("UserChoice", false);
                currentUser.Close();
            }

            // Tell explorer the file association has been changed
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
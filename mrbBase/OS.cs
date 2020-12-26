using System;
using System.Windows.Forms;

namespace mrbBase
{
    public static class OS
    {
        public const string SaveParentFolder = "Mids Reborn Builds";

        private static string AddSlash(string iPath)

        {
            return !iPath.EndsWith("\\") ? iPath + "\\" : iPath;
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
    }
}
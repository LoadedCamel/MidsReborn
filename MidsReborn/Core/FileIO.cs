using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Core
{
    public static class FileIO
    {
        public static string AddSlash(string iPath)
        {
            return iPath.EndsWith("\\") ? iPath : iPath + "\\";
        }

        private static string StripSlash(string iPath)
        {
            return iPath.EndsWith("\\") ? iPath.Substring(0, iPath.Length - 1) : iPath;
        }

        public static string StripPath(string iFileName)
        {
            var lastIdx = iFileName.LastIndexOf("\\", StringComparison.Ordinal);
            return lastIdx <= -1 ? iFileName : iFileName.Substring(lastIdx + 1);
        }

        public static string StripFileName(string iFileName)
        {
            var length = iFileName.LastIndexOf("\\", StringComparison.Ordinal);
            return length <= -1 ? AddSlash(iFileName) : iFileName.Substring(0, length);
        }

        public static string[] IOGrab(StreamReader iStream)
        {
            if (iStream == null)
                return Array.Empty<string>();

            var str = iStream.ReadLine();
            if (string.IsNullOrEmpty(str))
                return Array.Empty<string>();
            var strArray2 = str.Split('\t');
            for (var index = 0; index <= strArray2.Length - 1; ++index)
                strArray2[index] = IOStrip(strArray2[index]);
            return strArray2;
        }

        public static string IOStrip(string iString)
        {
            return ((!iString.StartsWith("'") && !iString.StartsWith(" ") ? iString : iString.Substring(1)).EndsWith(" ")
                ? iString.Substring(0, iString.Length - 1)
                : iString).Replace(char.ConvertFromUtf32(34), "");
        }

        public static string IOSeekReturn(StreamReader istream, string iString)
        {
            string str;
            try
            {
                string[] strArray;
                do
                {
                    strArray = IOGrab(istream);
                } while (strArray[0] != iString);

                str = strArray.Length > 1 ? strArray[1] : "";
            }
            catch (Exception ex)
            {
                var num = (int) MessageBox.Show("An error has occured when reading the stream. Error: " + ex.Message);
                str = "";
            }

            return str;
        }

        public static bool IOSeek(StreamReader iStream, string iString)
        {
            try
            {
                do
                {
                } while (IOGrab(iStream)[0] != iString);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured when reading the stream. Error: " + ex.Message);
                return false;
            }
        }

        public static bool CopyFolder(string src, string dest)
        {
            if (!Directory.Exists(src))
                return false;

            if (!Directory.Exists(dest))
                try
                {
                    Directory.CreateDirectory(dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has occured when copying the folder. Error: " + ex.Message);
                    return false;
                }

            if (!FolderCopy(new DirectoryInfo(src), dest))
                return false;
            try
            {
                var str = StripSlash(src) + ".old";
                src = StripSlash(src);
                var num = 0;
                while (Directory.Exists(str))
                {
                    ++num;
                    str = src + ".old." + num;
                    if (num > 100)
                        return false;
                }

                Directory.Move(src, str);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured when copying the folder. Error: " + ex.Message);
                return true;
            }

            return true;
        }

        private static bool FolderCopy(DirectoryInfo iDi, string dest)
        {
            var directories = iDi.GetDirectories();
            var files = iDi.GetFiles();
            if (!Directory.Exists(dest))
                try
                {
                    Directory.CreateDirectory(dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has occured when copying the folder. Error: " + ex.Message);
                    return false;
                }

            for (var index = 0; index <= directories.Length - 1; ++index)
                if (!FolderCopy(directories[index], AddSlash(dest) + directories[index].Name))
                    return false;
            dest = AddSlash(dest);
            for (var index = 0; index <= files.Length - 1; ++index)
                try
                {
                    files[index].CopyTo(dest + files[index].Name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has occured when copying the folder. Error: " + ex.Message);
                    return false;
                }

            return true;
        }

        public static string ReadLineUnlimited(StreamReader iStream, char FakeLF)
        {
            var bytes = new byte[65536];
            var count = 0;
            var num = 0;
            var flag = false;
            while (num > -1)
            {
                num = iStream.Read();
                if (num == -1)
                    flag = true;
                if ((FakeLF != char.MinValue) & (num == FakeLF))
                {
                    num = -1;
                    if (iStream.Peek() == 13)
                        iStream.Read();
                    if (iStream.Peek() == 10)
                        iStream.Read();
                    if (iStream.Peek() == 13)
                        iStream.Read();
                    if (iStream.Peek() == 10)
                        iStream.Read();
                }
                else
                {
                    switch (num)
                    {
                        case 10:
                            num = -1;
                            break;
                        case 13:
                            num = -1;
                            if (iStream.Peek() == 10) iStream.Read();
                            break;
                        default:
                            if (num > -1)
                            {
                                if ((num > byte.MaxValue) | (num < 0))
                                    num = 0;
                                bytes[count] = (byte) num;
                                ++count;
                            }

                            break;
                    }
                }
            }

            var str = Encoding.ASCII.GetString(bytes, 0, count);
            if (string.IsNullOrEmpty(str) & flag)
                str = null;
            return str;
        }
    }
}
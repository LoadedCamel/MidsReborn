using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mrbBase
{
    public class Zlib
    {
        private const string ExternalCompressionName = "HeroDesigner.ZLib.exe";

        private static bool is64BitProcess = IntPtr.Size == 8;

        // void** compress2(int32_t a1, int32_t* a2, int32_t a3, int32_t a4, void** a5) {
        [DllImport("lib\\ZLIB1.DLL", EntryPoint = "compress2", CharSet = CharSet.Ansi, SetLastError = false)]
        private static extern int Compress(
            ref byte destBytes,
            ref int destLength,
            ref byte srcBytes,
            int srcLength,
            int compressionLevel);

        // void** uncompress(int32_t a1, int32_t* a2, int32_t a3, int32_t a4)
        [DllImport("lib\\ZLIB1.DLL", EntryPoint = "uncompress", CharSet = CharSet.Ansi, SetLastError = false)]
        private static extern int Uncompress(
            ref byte destBytes,
            ref int destLength,
            ref byte srceBytes,
            int srcLength);

        public static byte[] CompressChunk(ref byte[] iBytes)
        {
            var length = iBytes.Length;
            var destLength = (int) (length + length * 0.001 + 12.0);
            var array = new byte[destLength];
            var num1 = Compress(ref array[0], ref destLength, ref iBytes[0], length, 9);
            switch (num1)
            {
                case -5:
                    MessageBox.Show("Unable to compress data chunk, output buffer is too small.", "Compression Error");
                    break;
                case -4:
                    MessageBox.Show("Unable to compress data chunk, out of memory.", "Compression Error");
                    break;
                case -3:
                    MessageBox.Show(
                        "Unable to compress data chunk, it seems to be corrupted. This should be impossible during compression.",
                        "Compression Error");
                    break;
                case -2:
                    MessageBox.Show("Unable to compress data chunk, compression level was invalid.", "Compression Error");
                    break;
                case 0:
                    Array.Resize(ref array, destLength);
                    return array;
                default:
                    MessageBox.Show("Unable to compress data chunk, unknown Zlib error: " + num1 + ".",
                        "Compression Error");
                    return Array.Empty<byte>();
            }

            return Array.Empty<byte>();
        }

        public static byte[] UncompressChunk(ref byte[] iBytes, int outSize)
        {
            //Console.WriteLine(Environment.CurrentDirectory);
            // zlib doesn't seem to work in 64bit, try this, and fallback to trying zlib if failed
            //if (is64BitProcess && File.Exists(ExternalCompressionName))
            //{
            //    var hex = HeroDesigner.ZLib.Helpers.byteArrayToHexString(iBytes);
            //    var proc = Process.Start(new ProcessStartInfo(ExternalCompressionName, "uncompress " + iBytes.Length + " " + outSize + " \"" + hex + "\"")
            //    {
            //        UseShellExecute = false,
            //        RedirectStandardOutput = true,
            //    });
            //    var output = proc.StandardOutput.ReadToEnd();
            //    if (proc.ExitCode == 0)
            //    {
            //        var text = new string(output.SkipWhile(x => x != '-').SkipWhile(x => x == '-').TakeWhile(x => x != '-').ToArray()).Trim();
            //        var bytes = HeroDesigner.ZLib.Helpers.hexStringToByteArray(text);
            //        return bytes;
            //    }
            //}
            var length = iBytes.Length;
            var destLength = outSize;
            var array = new byte[destLength];

            if (Uncompress(ref array[0], ref destLength, ref iBytes[0], length) != 0)
                return Array.Empty<byte>();
            Array.Resize(ref array, destLength);
            return array;
        }

        public static byte[] UUDecodeBytes(byte[] iBytes)
        {
            byte[] numArray1;
            try
            {
                var memoryStream = new MemoryStream();
                var binaryWriter = new BinaryWriter(memoryStream);
                for (var index = 0; index <= iBytes.Length - 1; index += 4)
                {
                    var num1 = iBytes[index];
                    var num2 = iBytes[index + 1];
                    var num3 = iBytes[index + 2];
                    var num4 = iBytes[index + 3];
                    if (num1 == 96)
                        num1 = 32;
                    if (num2 == 96)
                        num2 = 32;
                    if (num3 == 96)
                        num3 = 32;
                    if (num4 == 96)
                        num4 = 32;
                    var num5 = (num1 - 32) * 4 + (num2 - 32) / 16;
                    var num6 = num2 % 16 * 16 + (num3 - 32) / 4;
                    var num7 = num3 % 4 * 64 + num4 - 32;
                    binaryWriter.Write(Convert.ToByte(num5));
                    binaryWriter.Write(Convert.ToByte(num6));
                    binaryWriter.Write(Convert.ToByte(num7));
                }

                binaryWriter.Close();
                memoryStream.Close();
                numArray1 = memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                var numArray2 = new byte[0];
                MessageBox.Show(
                    "Conversion to binary failed at the 'DecodeBytes' stage. The data may be corrupt or incomplete. Error:" +
                    ex.Message, "Error!");
                numArray1 = numArray2;
            }

            return numArray1;
        }

        public static byte[] HexDecodeBytes(byte[] iBytes)
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            for (var index = 0; index <= iBytes.Length - 1; index += 2)
            {
                var num = Convert.ToByte(
                    ((char) iBytes[index]).ToString(CultureInfo.InvariantCulture) +
                    ((char) iBytes[index + 1]).ToString(CultureInfo.InvariantCulture), 16);
                binaryWriter.Write(num);
            }

            binaryWriter.Close();
            memoryStream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] HexEncodeBytes(byte[] iBytes)
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            for (var index = 0; index <= iBytes.Length - 1; ++index)
            {
                var str = iBytes[index].ToString("X");
                if (str.Length < 2)
                    str = "0" + str;
                if (str.Length < 2)
                    str = "0" + str;
                binaryWriter.Write(Convert.ToByte((int) str[0]));
                binaryWriter.Write(Convert.ToByte((int) str[1]));
            }

            binaryWriter.Close();
            memoryStream.Close();
            return memoryStream.ToArray();
        }

        public static string BreakString(string iString, int length, bool bookend = false)
        {
            var str1 = string.Empty;
            for (var startIndex = 0; startIndex <= iString.Length - 1; startIndex += length)
                if (startIndex + length >= iString.Length)
                {
                    if (bookend)
                        str1 += "|";
                    str1 += iString.Substring(startIndex);
                    if (bookend)
                        str1 += "|";
                }
                else
                {
                    if (bookend)
                        str1 += "|";
                    var str2 = str1 + iString.Substring(startIndex, length);
                    if (bookend)
                        str2 += "|";
                    str1 = str2 + "\n";
                }

            return str1;
        }

        public static string UnbreakHex(string iString)
        {
            var empty = string.Empty;
            for (var index = 0; index <= iString.Length - 1; ++index)
            {
                var ch1 = iString[index];
                var ch2 = iString[index];
                if (ch2 >= 'A' && ch2 <= 'Z' || ch2 >= '0' && ch2 <= '9')
                    empty += ch1.ToString();
            }

            return empty;
        }

        public static string UnbreakString(string iString, bool bookend = false)
        {
            string str;
            if (bookend)
            {
                char[] chArray = {'\n', '\r'};
                var strArray = iString.Split(chArray);
                for (var index1 = 0; index1 <= strArray.Length - 1; ++index1)
                {
                    strArray[index1] = strArray[index1].Replace(" ", string.Empty);
                    strArray[index1] = strArray[index1].Replace('\n'.ToString(CultureInfo.InvariantCulture), string.Empty);
                    strArray[index1] = strArray[index1].Replace('\r'.ToString(CultureInfo.InvariantCulture), string.Empty);
                    strArray[index1] = strArray[index1].Replace("\t", string.Empty);
                    strArray[index1] = strArray[index1].Replace("\n", string.Empty);
                    var startIndex = 0;
                    for (var index2 = 0; index2 <= strArray[index1].Length - 1; ++index2)
                    {
                        if (strArray[index1][index2] != '|')
                            continue;
                        startIndex = index2 + 1;
                        break;
                    }

                    if (startIndex < strArray[index1].Length - 1)
                        strArray[index1] = strArray[index1].Substring(startIndex);
                    var length = strArray[index1].Length;
                    for (var index2 = strArray[index1].Length - 1; index2 >= 0; index2 += -1)
                    {
                        if (strArray[index1][index2] != '|')
                            continue;
                        length = index2;
                        break;
                    }

                    if (length > 0)
                        strArray[index1] = strArray[index1].Substring(0, length);
                }

                var empty = string.Empty;
                for (var index = 0; index <= strArray.Length - 1; ++index)
                    empty += strArray[index];
                str = empty;
            }
            else
            {
                iString = iString.Replace(" ", string.Empty);
                iString = iString.Replace('\n'.ToString(CultureInfo.InvariantCulture), string.Empty);
                iString = iString.Replace('\r'.ToString(CultureInfo.InvariantCulture), string.Empty);
                iString = iString.Replace("\t", string.Empty);
                iString = iString.Replace("\n", string.Empty);
                str = iString;
            }

            return str;
        }

        private enum ECompressionLevel
        {
            DefaultCompress = -1,
            None = 0,
            Fast = 1,
            Best = 9
        }
    }
}
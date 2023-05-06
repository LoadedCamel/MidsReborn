using System.Globalization;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Mids_Reborn.Core.Utils
{
    internal static class Compression
    {
        public static byte[] Compress(byte[] sourceBytes)
        {
            using var mStream = new MemoryStream();
            using var deflateStream = new DeflaterOutputStream(mStream, new Deflater(9));
            deflateStream.Write(sourceBytes, 0, sourceBytes.Length);
            deflateStream.Finish();
            var compressedBytes = mStream.ToArray();
            return compressedBytes;
        }

        public static byte[] Decompress(byte[] sourceBytes)
        {
            using var mStream = new MemoryStream(sourceBytes);
            using var outStream = new MemoryStream();
            using var inflaterStream = new InflaterInputStream(mStream);
            inflaterStream.CopyTo(outStream);
            var decompressedBytes = outStream.ToArray();
            return decompressedBytes;
        }

        public static string BreakString(string iString, int length, bool bookend = false)
        {
            var output = string.Empty;
            for (var startIndex = 0; startIndex <= iString.Length - 1; startIndex += length)
            {
                if (startIndex + length >= iString.Length)
                {
                    if (bookend)
                    {
                        output += "|";
                    }

                    output += iString.Substring(startIndex);
                    if (bookend)
                    {
                        output += "|";
                    }
                }
                else
                {
                    if (bookend)
                    {
                        output += "|";
                    }

                    var str2 = output + iString.Substring(startIndex, length);
                    if (bookend)
                    {
                        str2 += "|";
                    }

                    output = str2 + "\n";
                }
            }

            return output;
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
                        {
                            continue;
                        }

                        startIndex = index2 + 1;
                        break;
                    }

                    if (startIndex < strArray[index1].Length - 1)
                    {
                        strArray[index1] = strArray[index1].Substring(startIndex);
                    }

                    var length = strArray[index1].Length;
                    for (var index2 = strArray[index1].Length - 1; index2 >= 0; index2 += -1)
                    {
                        if (strArray[index1][index2] != '|')
                        {
                            continue;
                        }

                        length = index2;
                        break;
                    }

                    if (length > 0)
                    {
                        strArray[index1] = strArray[index1].Substring(0, length);
                    }
                }

                var empty = string.Empty;
                for (var index = 0; index <= strArray.Length - 1; ++index)
                {
                    empty += strArray[index];
                }

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
    }
}

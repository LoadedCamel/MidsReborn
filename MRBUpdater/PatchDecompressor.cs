#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using mrbBase.Utils;

namespace MRBUpdater
{
    public class PatchDecompressor
    {
        public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
        public event ProgressEventHandler? ProgressUpdate;
        //public event EventHandler<ProgressEventArgs> ProgressUpdate;
        public event EventHandler<ErrorEventArgs>? ErrorUpdate;

        public static void CleanOldEntries(string extractPath)
        {
            if (!extractPath.Contains("Data")) return;
            var files = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public void RecompileFileEntries(string patchPath, List<FileData> decompressedData)
        {
            for (var index = 0; index <= decompressedData.Count; index++)
            {
                var patchedFile = decompressedData[index];
                var fileInfo = new FileInfo(Path.Combine(patchPath, patchedFile.Path, patchedFile.FileName));
                try
                {
                    if (!Directory.Exists(patchPath)) Directory.CreateDirectory(patchPath);
                    fileInfo.Directory?.Create();
                    File.WriteAllBytes(fileInfo.FullName, patchedFile.Data);
                    Thread.Sleep(50);
                    ProgressUpdate?.Invoke(this, new ProgressEventArgs(patchedFile.FileName, index, decompressedData.Count));
                }
                catch (Exception ex)
                {
                    ErrorUpdate?.Invoke(this, new ErrorEventArgs(ex));
                }
            }
        }


        public List<FileData>? DecompressData(string patchFile)
        {
            FileStream fileStream;
            MemoryStream decompressed;
            InflaterInputStream inflaterStream;
            BinaryReader reader;
            var patchFiles = new List<FileData>();
            try
            {
                fileStream = new FileStream(patchFile, FileMode.Open, FileAccess.Read);
                decompressed = new MemoryStream();
                inflaterStream = new InflaterInputStream(fileStream);
                reader = new BinaryReader(inflaterStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Failed to read patch data: " + e.Message);
                return null;
            }

            try
            {
                var headerFound = true;
                var header = reader.ReadString();
                if (header != "Mids Reborn Patch Data")
                {
                    headerFound = false;
                }

                if (!headerFound)
                {
                    MessageBox.Show(@"Expected MRB header, got something else!", @"Error Reading Patch Data");
                    return null;
                }

                var fileCount = reader.ReadInt32();
                for (var i = 0; i < fileCount; i++)
                {
                    var dataLength = reader.ReadInt32();
                    patchFiles.Add(new FileData { FileName = reader.ReadString(), Path = reader.ReadString(), Data = reader.ReadBytes(dataLength) });
                }
                reader.Close();
                decompressed.Close();
                inflaterStream.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                reader.Close();
                decompressed.Close();
                inflaterStream.Close();
                fileStream.Close();
                MessageBox.Show($@"Message: {ex.Message}
Trace: {ex.StackTrace}");
                return null;
            }

            return patchFiles;
        }
    }
}

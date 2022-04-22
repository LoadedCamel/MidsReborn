#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using mrbBase.Utils;
using MRBUpdater.Utils;

namespace MRBUpdater
{
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs args);

    public delegate void CompletedFileEventHandler(object sender, ScanEventArgs file);

    public delegate void FileFailureEventHandler(object sender, string file);

    public class PatchDecompressorEvents
    {
        public ProgressEventHandler? UpdateProgress;
        public CompletedFileEventHandler? CompletedFile;
        public FileFailureEventHandler? FileFailure;

        public bool OnCompletedFile(string file)
        {
            var result = true;
            var handler = CompletedFile;
            if (handler == null) return result;
            var args = new ScanEventArgs(file);
            handler(this, args);
            result = args.ContinueRunning;
            return result;
        }

        public bool OnFileFailure(string file, Exception e)
        {
            var handler = FileFailure;
            var result = handler != null;
            if (!result) return result;
            var args = new ScanFailureEventArgs(file, e);
            handler?.Invoke(this, file);
            result = args.ContinueRunning;
            return result;
        }

        public TimeSpan ProgressInterval { get; set; } = TimeSpan.FromSeconds(3);

        #region InstanceFields

        #endregion
    }

    public class PatchDecompressor
    {
        public static float PercentCompleted { get; set; }

        #region Constructors

        public PatchDecompressor()
        {
        }

        public PatchDecompressor(PatchDecompressorEvents events)
        {
            _decompressorEvents = events;
        }

        #endregion

        #region Methods

        public static void CleanOldEntries(string extractPath)
        {
            if (!extractPath.Contains("Data")) return;
            var files = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private void CreateFile(FileData patchedFile, FileSystemInfo fileInfo)
        {
            var targetName = string.Empty;
            try
            {
                targetName = patchedFile.FileName;
                using var inputStream = new MemoryStream(patchedFile.Data);
                using var outputStream = new FileStream(fileInfo.FullName, FileMode.Create);
                if (_decompressorEvents is { UpdateProgress: { } })
                {
                    StreamUtils.Copy(inputStream, outputStream, new byte[4096], _decompressorEvents.UpdateProgress, _decompressorEvents.ProgressInterval, this, patchedFile.FileName, patchedFile.Data.Length);
                }
                else
                {
                    StreamUtils.Copy(inputStream, outputStream, new byte[4096]);
                }

                if (_decompressorEvents != null)
                {
                    _continueRunning = _decompressorEvents.OnCompletedFile(patchedFile.FileName);
                }
            }
            catch (Exception ex)
            {
                if (_decompressorEvents != null)
                {
                    _continueRunning = _decompressorEvents.OnFileFailure(targetName, ex);
                }
                else
                {
                    _continueRunning = false;
                    throw;
                }
            }
        }

        public void RecompileFileEntries(string patchPath, List<FileData> decompressedData)
        {
            // foreach (var patchFile in patchFiles)
            // {
            //     var fileInfo = new FileInfo(Path.Combine(PatchPath, patchFile.Path, patchFile.FileName));
            //     fileInfo.Directory?.Create();
            //     File.WriteAllBytes(fileInfo.FullName, patchFile.Data);
            // }
            _continueRunning = true;
            foreach (var patchedFile in decompressedData.Where(_ => _continueRunning))
            {
                CreateFile(patchedFile, new FileInfo(Path.Combine(patchPath, patchedFile.Path, patchedFile.FileName)));
            }
        }

        #endregion

        #region InternalProcessing

        public static List<FileData>? DecompressData(string patchFile)
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

        #endregion

        #region InstanceFields

        private PatchDecompressorEvents? _decompressorEvents;
        private bool _continueRunning;

        #endregion
    }
}

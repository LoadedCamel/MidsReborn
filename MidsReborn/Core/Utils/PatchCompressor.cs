#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Utils
{
    public class PatchCompressor
    {
        private PatchCompressor(EPatchType patchType)
        {
            PatchType = patchType;
        }

        public static PatchCompressor AppPatchCompressor { get; } = new(EPatchType.Application);
        public static PatchCompressor DbPatchCompressor { get; } = new(EPatchType.Database);

        public enum EPatchType
        {
            Application,
            Database
        }

        private EPatchType PatchType { get; set; }

        private const string PatchFolderName = @"Patches";
        private const string HashFileName = @"FileHash.json";
        public bool Generating { get; private set; }

        private string TopLevelFolder
        {
            get
            {
                var value = PatchType switch
                {
                    EPatchType.Application => AppContext.BaseDirectory,
                    EPatchType.Database => Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string PatchPath
        {
            get
            {
                var value = PatchType switch
                {
                    EPatchType.Application => Path.Combine(AppContext.BaseDirectory, PatchFolderName),
                    EPatchType.Database => Path.Combine(MidsContext.Config.DataPath, PatchFolderName),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string PatchFile
        {
            get
            {
                var value = PatchType switch
                {
                    EPatchType.Application => Path.Combine(PatchPath, $"{MidsContext.AssemblyFileVersion}.mru"),
                    EPatchType.Database => Path.Combine(PatchPath, $"{DatabaseAPI.Database.Version}.mru"),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string HashFile => Path.Combine(PatchPath, HashFileName);
        
        private IEnumerable<FileData> CompileList(string? path, EPatchType patchType)
        {
            var files = new List<string>();
            List<FileHash>? hashes = null;
            var fileQueue = new List<FileData>();
            var exclusionList = new List<string>();
            exclusionList = patchType switch
            {
                EPatchType.Application => new List<string> { "Patches", "Data", "Updater", "ICSharpCode" },
                EPatchType.Database => new List<string> { "Patches" },
                _ => exclusionList
            };

            files = patchType switch
            {
                EPatchType.Application => Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(x => !exclusionList.Any(x.Contains))
                    .ToList(),
                EPatchType.Database => Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(x => !exclusionList.Any(x.Contains))
                    .ToList(),
                _ => files
            };

            if (File.Exists(Path.Combine(PatchPath, HashFile)))
            {
                hashes = JsonConvert.DeserializeObject<List<FileHash>>(File.ReadAllText(HashFile));
            }

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var name = fileInfo.Name;
                var directory = fileInfo.DirectoryName?
                    .Replace(TopLevelFolder, string.Empty)
                    .Replace(TopLevelFolder.Remove(TopLevelFolder.Length - 1, 1), string.Empty);
                var data = File.ReadAllBytes(file);
                if (directory == null) continue;
                var newFile = new FileHash(directory, fileInfo.Name, FileHash.ComputeHash(file));
                if (hashes != null)
                {
                    var priorFile = hashes.FirstOrDefault(x => x.FileName == name && x.Directory == directory);
                    if (priorFile != null)
                    {
                        var hashCompareResult = FileHash.Compare(newFile.Hash, priorFile.Hash);
                        if (hashCompareResult) continue;
                        hashes.First(x => x.FileName == newFile.FileName).Hash = newFile.Hash;
                        fileQueue.Add(new FileData { FileName = name, Data = data, Path = directory });
                    }
                    else
                    {
                        hashes.Add(newFile);
                        fileQueue.Add(new FileData { FileName = name, Data = data, Path = directory });
                    }
                }
                else
                {
                    hashes = new List<FileHash> { newFile };
                    fileQueue.Add(new FileData { FileName = name, Data = data, Path = directory });
                }
            }

            File.WriteAllText(HashFile, JsonConvert.SerializeObject(hashes, Formatting.Indented));
            return fileQueue;
        }

        public async Task<bool> CreatePatchFile(string? path, EPatchType patchType)
        {
            var completionSource = new TaskCompletionSource<bool>();
            Generating = true;
            var compressedData = CompressData(path, patchType);
            if (compressedData == null) 
            {
                completionSource.TrySetResult(false);
            }
            else
            {
                DeletePriorPatch(path);
                var generated = GenerateFile(compressedData);
                completionSource.TrySetResult(generated);
                Generating = false;
                return await completionSource.Task;
            }

            Generating = false;
            return await completionSource.Task;
        }

        private byte[]? CompressData(string? path, EPatchType patchType)
        {
            byte[]? outData;
            MemoryStream patchStream;
            BinaryWriter writer;
            try
            {
                if (!Directory.Exists(PatchPath))
                {
                    Directory.CreateDirectory(PatchPath);
                }


                patchStream = new MemoryStream();
                writer = new BinaryWriter(patchStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Failed to create patch container: " + ex.Message);
                return null;
            }

            try
            {
                var files = CompileList(path, patchType).ToList();
                writer.Write("Mids Reborn Patch Data");
                writer.Write(files.Count);
                foreach (var file in files)
                {
                    writer.Write(file.Data.Length);
                    writer.Write(file.FileName);
                    Debug.WriteLine($"Adding {file.FileName} to patch");
                    writer.Write(file.Path);
                    writer.Write(file.Data);
                }

                writer.Close();
                outData = patchStream.ToArray();
                patchStream.Close();
            }
            catch (Exception e)
            {
                writer.Close();
                patchStream.Close();
                MessageBox.Show($@"Message: {e.Message}\r\nTrace: {e.StackTrace}");
                return null;
            }

            return outData;
        }

        private void DeletePriorPatch(string? path)
        {
            var filesToRemove = Directory.GetFiles(PatchPath, "*.mru").ToList();
            if (!filesToRemove.Any()) return;
            foreach (var file in filesToRemove)
            {
                File.Delete(file);
            }
        }

        private bool GenerateFile(byte[] byteArray)
        {
            FileStream fileStream;
            DeflaterOutputStream outputStream;
            try
            {
                fileStream = new FileStream(PatchFile, FileMode.Create);
                outputStream = new DeflaterOutputStream(fileStream, new Deflater(9));
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Patch generation failed: " + e.Message);
                return false;
            }

            try
            {
                outputStream.Write(byteArray, 0, byteArray.Length);
                outputStream.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                outputStream.Close();
                fileStream.Close();
                MessageBox.Show($@"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                return false;
            }

            return true;
        }
    }
}

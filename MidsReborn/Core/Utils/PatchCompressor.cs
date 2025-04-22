#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.UpdateSystem.Models;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Mids_Reborn.Core.Utils
{
    public class PatchCompressor
    {
        public delegate void ProgressChangedHandler(object? sender, ProgressEventArgs e);
        public event ProgressChangedHandler? ProgressChanged;
        private PatchCompressor(PatchType patchType)
        {
            PatchType = patchType;
        }

        public static PatchCompressor AppPatchCompressor { get; } = new(PatchType.Application);
        public static PatchCompressor DbPatchCompressor { get; } = new(PatchType.Database);

        private PatchType PatchType { get; set; }

        private const string PatchFolderName = @"Patches";

        private string TopLevelFolder
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => AppContext.BaseDirectory,
                    PatchType.Database => Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string PatchVersion
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => $"{MidsContext.AssemblyFileVersion}",
                    PatchType.Database => $"{DatabaseAPI.Database.Version}",
                    _ => string.Empty
                };

                return value;
            }
        }

        private string PatchDir
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => Path.Combine(AppContext.BaseDirectory, PatchFolderName, "App"),
                    PatchType.Database => Path.Combine(AppContext.BaseDirectory, PatchFolderName, "Db"),
                    _ => string.Empty
                };
                if (!Directory.Exists(value)) Directory.CreateDirectory(value);
                return value;
            }
        }

        private string PatchPath
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => AppContext.BaseDirectory,
                    PatchType.Database => MidsContext.Config.DataPath!,
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
                    PatchType.Application => Path.Combine(PatchDir, PatchName),
                    PatchType.Database => Path.Combine(PatchDir, PatchName),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string PatchName
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => $"{MidsContext.AppName.Replace(" ", string.Empty)}-{MidsContext.AssemblyFileVersion}-cumulative.mru".ToLower(),
                    PatchType.Database => $"{DatabaseAPI.DatabaseName}-{DatabaseAPI.Database.Version}-cumulative.mru".ToLower(),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string HashFile
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => Path.Combine(PatchDir, HashName),
                    PatchType.Database => Path.Combine(PatchDir, HashName),
                    _ => string.Empty
                };

                return value;
            }
        }

        private string HashName
        {
            get
            {
                var value = PatchType switch
                {
                    PatchType.Application => $"{MidsContext.AppName.Replace(" ", string.Empty)}-{MidsContext.AssemblyFileVersion}-cumulative.hash".ToLower(),
                    PatchType.Database => $"{DatabaseAPI.DatabaseName}-{DatabaseAPI.Database.Version}-cumulative.hash".ToLower(),
                    _ => string.Empty
                };

                return value;
            }
        }

        private List<FileData> CompileList()
        {
            var files = new List<string>();
            var hashes = new List<FileHash>();
            var fileQueue = new List<FileData>();
            var exclusionList = new List<string>();
            exclusionList = PatchType switch
            {
                PatchType.Application =>
                [
                    "Patches", "Data", "MRBBootstrap.exe", ".pdb", "MidsReborn.exe.WebView2", "appSettings"
                ],
                PatchType.Database => ["Patches"],
                _ => exclusionList
            };

            files = PatchType switch
            {
                PatchType.Application => Directory.EnumerateFiles(PatchPath, "*.*", SearchOption.AllDirectories)
                    .Where(x => !exclusionList.Any(x.Contains))
                    .ToList(),
                PatchType.Database => Directory.EnumerateFiles(PatchPath, "*.*", SearchOption.AllDirectories)
                    .Where(x => !exclusionList.Any(x.Contains))
                    .ToList(),
                _ => files
            };

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var name = fileInfo.Name;

                var directory = fileInfo.DirectoryName?
                    .Replace(TopLevelFolder, string.Empty)
                    .Replace(TopLevelFolder.Remove(TopLevelFolder.Length - 1, 1), string.Empty);

                if (directory == null) continue;

                var data = File.ReadAllBytes(file);

                var hashedFile = new FileHash(directory, name, FileHash.ComputeHash(data));

                hashes.Add(hashedFile);

                fileQueue.Add(new FileData { FileName = name, Data = data, Path = directory });
                
            }
            File.WriteAllText(HashFile, JsonConvert.SerializeObject(hashes, Formatting.Indented));
            return fileQueue;
        }

        public async Task<bool> CreatePatchFile()
        {
            CleanPrevious();
            var completionSource = new TaskCompletionSource<bool>();
            var hashedFiles = CompileList();
            var compressedData = await CompressedFileData(hashedFiles);
            if (compressedData == null) 
            {
                completionSource.TrySetResult(false);
            }
            else
            {
                var generated = await GenerateCompressedFile(compressedData);
                if (generated)
                {
                   var modifiedManifest = await PatchManifestBuilder.ModifyManifestAsync(PatchType, PatchType == PatchType.Application ? MidsContext.AppName : DatabaseAPI.DatabaseName, PatchVersion, PatchName);
                   if (modifiedManifest is not null)
                   {
                       await WriteModifiedManifest(modifiedManifest);
                   }
                }

                completionSource.TrySetResult(generated);
            }

            return await completionSource.Task;
        }

        private async Task<byte[]?> CompressedFileData(List<FileData> hashedFiles)
        {
            byte[]? outData;
            await using var patchStream = new MemoryStream();
            await using var writer = new BinaryWriter(patchStream);

            try
            {
                writer.Write("Mids Reborn Patch Data");
                writer.Write(hashedFiles.Count);
                for (var index = 0; index < hashedFiles.Count; index++)
                {
                    var file = hashedFiles[index];
                    writer.Write(file.Data.Length);
                    writer.Write(file.FileName);
                    writer.Write(file.Path);
                    writer.Write(file.Data);
                    await Task.Delay(50);
                    ProgressChanged?.Invoke(this, new ProgressEventArgs($"Adding To Patch Container: {file.FileName}", index,hashedFiles.Count));
                }

                outData = patchStream.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Message: {e.Message}\r\n\r\nTrace: {e.StackTrace}");
                return null;
            }

            return outData;
        }

        private async Task<bool> GenerateCompressedFile(byte[] byteArray)
        {
            await using var fileStream = new ProgressFileStream(PatchFile, FileMode.Create);
            await using var compressionStream = new DeflaterOutputStream(fileStream, new Deflater(9));
            const int chunkSize = 1024;
            try
            {
                ProgressChanged?.Invoke(this, new ProgressEventArgs($"Generating Patch From Container: {PatchName}", 0, byteArray.Length));
                await Task.Delay(50);
                for (var index = 0; index < byteArray.Length; index += chunkSize)
                {
                    var chunk = Math.Min(chunkSize, byteArray.Length - index);
                    await compressionStream.WriteAsync(byteArray, index, chunk);
                    ProgressChanged?.Invoke(this, new ProgressEventArgs($"Generating Patch From Container: {PatchName}", fileStream.Progress, byteArray.Length));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Patch generation failed: " + e.Message);
                return false;
            }
            return true;
        }

        private void CleanPrevious()
        {
            string[] extensions = [".mru", ".hash"];
            var files = Directory.GetFiles(PatchDir).Where(file => extensions.Any(file.EndsWith));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private async Task WriteModifiedManifest(Manifest? manifest)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            await using var fileStream = File.Create(Path.Combine(PatchDir, "update_manifest.json"));
            await JsonSerializer.SerializeAsync(fileStream, manifest, jsonOptions);
        }
    }
}

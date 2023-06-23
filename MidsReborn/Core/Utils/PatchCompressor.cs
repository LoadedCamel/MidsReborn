#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

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

        private enum EPatchType
        {
            Application,
            Database
        }

        private EPatchType PatchType { get; set; }

        private const string PatchFolderName = @"Patches";

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

        private string PatchVersion
        {
            get
            {
                var value = PatchType switch
                {
                    EPatchType.Application => $"{MidsContext.AssemblyFileVersion}",
                    EPatchType.Database => $"{DatabaseAPI.Database.Version}",
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
                    EPatchType.Application => Path.Combine(AppContext.BaseDirectory, PatchFolderName, "App"),
                    EPatchType.Database => Path.Combine(AppContext.BaseDirectory, PatchFolderName, "Db"),
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
                    EPatchType.Application => AppContext.BaseDirectory,
                    EPatchType.Database => MidsContext.Config.DataPath!,
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
                    EPatchType.Application => Path.Combine(PatchDir, PatchName),
                    EPatchType.Database => Path.Combine(PatchDir, PatchName),
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
                    EPatchType.Application => $"{MidsContext.AppName.Replace("' ", string.Empty)}-{MidsContext.AssemblyFileVersion}-cumulative.mru".ToLower(),
                    EPatchType.Database => $"{DatabaseAPI.DatabaseName}-{DatabaseAPI.Database.Version}-cumulative.mru".ToLower(),
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
                    EPatchType.Application => Path.Combine(PatchDir, HashName),
                    EPatchType.Database => Path.Combine(PatchDir, HashName),
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
                    EPatchType.Application => $"{MidsContext.AppName.Replace("' ", string.Empty)}-{MidsContext.AssemblyFileVersion}.hash".ToLower(),
                    EPatchType.Database => $"{DatabaseAPI.DatabaseName}-{DatabaseAPI.Database.Version}.hash".ToLower(),
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
                EPatchType.Application => new List<string> { "Patches", "Data", ".pdb", "deps.json", "MidsReborn.exe.WebView2" },
                EPatchType.Database => new List<string> { "Patches" },
                _ => exclusionList
            };

            files = PatchType switch
            {
                EPatchType.Application => Directory.EnumerateFiles(PatchPath, "*.*", SearchOption.AllDirectories)
                    .Where(x => !exclusionList.Any(x.Contains))
                    .ToList(),
                EPatchType.Database => Directory.EnumerateFiles(PatchPath, "*.*", SearchOption.AllDirectories)
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
                var data = File.ReadAllBytes(file);
                if (directory == null) continue;
                var hashedFile = new FileHash(directory, name, FileHash.ComputeHash(file));
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
            var compressedData = CompressData(hashedFiles);
            if (compressedData == null) 
            {
                completionSource.TrySetResult(false);
            }
            else
            {
                var generated = GenerateFile(compressedData);
                if (generated) GenerateManifest();
                completionSource.TrySetResult(generated);
                return await completionSource.Task;
            }

            return await completionSource.Task;
        }

        private static byte[]? CompressData(List<FileData> hashedFiles)
        {
            byte[]? outData;
            MemoryStream patchStream;
            BinaryWriter writer;
            try
            {
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
                writer.Write("Mids Reborn Patch Data");
                writer.Write(hashedFiles.Count);
                foreach (var file in hashedFiles)
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

        private void CleanPrevious()
        {
            string[] extensions = { ".mru", ".hash" };
            var files = Directory.GetFiles(PatchDir).Where(file => extensions.Any(file.EndsWith));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private void GenerateManifest()
        {
            using var writer = new XmlTextWriter(Path.Combine(PatchDir, "update_manifest.xml"), Encoding.UTF8);
            writer.WriteStartDocument();
            writer.Formatting = System.Xml.Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("manifest");
            writer.WriteStartElement("version");
            writer.WriteString(PatchVersion);
            writer.WriteEndElement();
            writer.WriteStartElement("file");
            writer.WriteString(PatchName);
            writer.WriteEndElement();
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Core.Utils;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile
{
    public class BuildManager
    {
        private static readonly Lazy<BuildManager> LazyInstance = new(() => new BuildManager());
        public static BuildManager Instance => LazyInstance.Value;

        internal CharacterBuildData? BuildData;
        private readonly IBuildNotifier _notifier;
        private readonly BuildPreferences _preferences;

        private BuildManager()
        {
            BuildData = CharacterBuildData.Instance;
            _notifier = new BuildNotifier();
            _preferences = BuildPreferences.Load();
        }
        
        public bool LoadFromFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var returnedVal = false;
            var data = File.ReadAllText(fileName);
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>{ new EnhancementDataConverter() }
                };
                BuildData = JsonConvert.DeserializeObject<CharacterBuildData>(data, settings) ?? throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                _notifier.ShowError(ex.Message);
            }

            if (BuildData is null)
            {
                _notifier.ShowError($"Cannot load {fileName} - error reading build data.\r\n\r\n{nameof(BuildData)}");
                return false;
            }

            var metaData = BuildData.BuiltWith;
            if (metaData == null)
            {
                _notifier.ShowError($"Cannot load {fileName} - error reading build metadata.\r\n\r\n{nameof(metaData)}");
                return false;
            }

            var fileInfo = new FileInfo(fileName);
            if (DatabaseAPI.DatabaseName != metaData.Database)
            {
                var databases = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder)).ToList();
                var selected = databases.FirstOrDefault(d => d.Contains(metaData.Database));
                if (selected is null)
                {
                    _notifier.ShowError($"This build requires the {metaData.Database} be installed prior to loading it.\r\nPlease install the database and try again.");
                    return false;
                }
                var result = _notifier.ShowWarningDialog($"This build was created using the {metaData.Database} database.\r\nDo you want to reload and switch to this database, then attempt to load the build?", fileInfo.Name);
                if (result != DialogResult.Yes) return returnedVal;
                MidsContext.Config.LastFileName = fileName;
                MidsContext.Config.DataPath = selected;
                MidsContext.Config.SavePath = selected;
                MidsContext.Config.SaveConfig();
                Application.Restart();
            }
            else
            {
                // Compare Database Version if Enabled
                if (MidsContext.Config.WarnOnOldDbMbd)
                {
                    var outDatedDb = Helpers.CompareVersions(DatabaseAPI.Database.Version, metaData.DatabaseVersion);
                    var newerDb = Helpers.CompareVersions(metaData.DatabaseVersion, DatabaseAPI.Database.Version);
                    var continueLoad = false;

                    if (outDatedDb)
                    {
                        if (_preferences.ShouldSkipWarning(fileName))
                        {
                            continueLoad = true;
                        }
                        else
                        {
                            var result = _notifier.ShowWarningDialog($"This build was created in an older version of the {metaData.Database} database.\r\nSome powers and/or enhancements may have changed, you may need to rebuild some of it.", @"Warning", true);
                            switch (result)
                            {
                                case DialogResult.Ignore:
                                    _preferences.AddIgnoredBuild(fileName);
                                    continueLoad = true;
                                    break;
                                case DialogResult.OK:
                                    continueLoad = true;
                                    break;
                            }
                        }
                    }

                    if (newerDb)
                    {
                        if (_preferences.ShouldSkipWarning(fileName))
                        {
                            continueLoad = true;
                        }
                        else
                        {
                            var result = _notifier.ShowWarningDialog($"This build was created in an newer version of the {metaData.Database} database.\r\nIt is recommended that you update the database.", @"Warning", true);
                            switch (result)
                            {
                                case DialogResult.Ignore:
                                    _preferences.AddIgnoredBuild(fileName);
                                    continueLoad = true;
                                    break;
                                case DialogResult.OK:
                                    continueLoad = true;
                                    break;
                            }
                        }
                    }

                    if (!outDatedDb && !newerDb) continueLoad = true;

                    if (continueLoad)
                    {
                        returnedVal = BuildData.LoadBuild();
                    }
                }
                else
                {
                    returnedVal = BuildData.LoadBuild();
                }
            }

            return returnedVal;
        }

        public bool SaveToFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            if (MidsContext.Character?.CurrentBuild == null) return false;
            var powerEntries = MidsContext.Character.CurrentBuild.Powers.GetRange(0, 24);
            if (powerEntries.All(x => x?.Power == null))
            {
                _notifier.ShowError(@"Unable to save build, you haven't selected any powers.");
                return false;
            }

            BuildData?.Update(MidsContext.Character);
            File.WriteAllText(fileName, JsonConvert.SerializeObject(BuildData, Formatting.Indented));
            return true;
        }

        public string GetShareData()
        {
            return BuildData != null ? BuildData.GenerateShareData() : string.Empty;
        }

        public string GetDataChunk()
        {
            return BuildData != null ? BuildData.GenerateChunkData() : string.Empty;
        }

        private bool LoadShareData(string? data, string? id = null)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;
            var returnedVal = false;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>{ new EnhancementDataConverter() }
                };
                BuildData = JsonConvert.DeserializeObject<CharacterBuildData>(data, settings) ?? throw new InvalidOperationException();
                if (id is not null) BuildData.Id = id;
            }
            catch (Exception ex)
            {
                _notifier.ShowError(ex.Message);
            }

            if (BuildData is null)
            {
                _notifier.ShowError($"Cannot load share data - An error occurred during reading build data.\r\n\r\n{nameof(BuildData)}");
                return false;
            }

            var metaData = BuildData.BuiltWith;
            if (metaData == null)
            {
                _notifier.ShowError($"Cannot load share data - An error occurred during reading build metadata.\r\n\r\n{nameof(metaData)}");
                return false;
            }

            if (DatabaseAPI.DatabaseName != metaData.Database)
            {
                var databases = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder)).ToList();
                var selected = databases.FirstOrDefault(d => d.Contains(metaData.Database));
                if (selected is null)
                {
                    _notifier.ShowError($"This build requires the {metaData.Database} be installed prior to loading it.\r\nPlease install the database and try again.");
                }

                _notifier.ShowWarning($"This build requires the {metaData.Database}, however you are currently using the {DatabaseAPI.DatabaseName} database. Please load the correct database and try again.");
                return false;
            }

            // Compare Database Version if Enabled
            if (MidsContext.Config.WarnOnOldDbMbd)
            {
                var outDatedDb = Helpers.CompareVersions(DatabaseAPI.Database.Version, metaData.DatabaseVersion);
                var newerDb = Helpers.CompareVersions(metaData.DatabaseVersion, DatabaseAPI.Database.Version);
                var continueLoad = false;

                if (outDatedDb)
                {
                    var result = _notifier.ShowWarningDialog($"This build was created in an older version of the {metaData.Database} database.\r\nSome powers and/or enhancements may have changed, you may need to rebuild some of it.", @"Warning");
                    continueLoad = result == DialogResult.OK;
                }

                if (newerDb)
                {
                    var result = _notifier.ShowWarningDialog($"This build was created in an newer version of the {metaData.Database} database.\r\nIt is recommended that you update the database.", @"Warning");
                    continueLoad = result == DialogResult.OK;
                }

                if (!outDatedDb && !newerDb) continueLoad = true;

                if (continueLoad)
                {
                    returnedVal = BuildData.LoadBuild();
                }
            }
            else
            {
                returnedVal = BuildData.LoadBuild();
            }

            return returnedVal;
        }

        public bool ValidateAndLoadImportData(DataClassifier.ClassificationResult classificationResult)
        {
            if (!classificationResult.IsValid)
            {
                _notifier.ShowError("Invalid or unknown data format.");
                return false;
            }

            // Split input to separate header and data
            var lines = classificationResult.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) // Ensure there is at least one header line and data line
            {
                _notifier.ShowError("Input format is incorrect. No data found after header.");
                return false;
            }

            if (lines.Length <= 0) return false;
            // Clean the header by removing surrounding pipes and trimming whitespace
            var header = lines[0].Trim('|').Trim();
            var headerItems = header.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (headerItems.Length != 5)
            {
                _notifier.ShowError("Header format is incorrect.");
                return false;
            }

            int uncompressedSize, compressedSize;
            try
            {
                uncompressedSize = int.Parse(headerItems[1]);
                compressedSize = int.Parse(headerItems[2]);
            }
            catch (FormatException)
            {
                _notifier.ShowError("Header contains invalid size information.");
                return false;
            }

            var data = string.Join("", lines.Skip(1)).Replace("|", "");

            // Process data based on the type determined by the classification
            switch (classificationResult.Type)
            {
                case DataClassifier.DataType.Mxd:
                    // Process as HEX
                    if (!Regex.IsMatch(data, @"\A\b[0-9A-F]+\b\Z", RegexOptions.IgnoreCase))
                    {
                        _notifier.ShowError("Data does not contain valid HEX values.");
                        return false;
                    }

                    var decodedBytes = ModernZlib.HexDecodeBytes(Encoding.ASCII.GetBytes(data));
                    if (decodedBytes.Length != compressedSize)
                    {
                        _notifier.ShowError("Compressed size mismatch.");
                        return false;
                    }

                    decodedBytes = ModernZlib.DecompressChunk(decodedBytes, uncompressedSize);
                    // Now use the decompressed bytes with the MxDReadSaveData method
                    var loadSuccess = MidsCharacterFileFormat.MxDReadSaveData(ref decodedBytes, false);
                    return loadSuccess;

                case DataClassifier.DataType.Mbd:
                    // Process as Base64
                    if (!Regex.IsMatch(data, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
                    {
                        _notifier.ShowError("Data is not valid BASE64.");
                        return false;
                    }


                    var decoded = Compression.DecompressFromBase64(data);
                    if (decoded.CompressedSize == compressedSize) return LoadShareData(decoded.OutString);
                    _notifier.ShowError("Compressed size mismatch.");
                    return false;

                default:
                    _notifier.ShowError("Unsupported data type detected.");
                    return false;
            }
        }

        public bool ValidateAndLoadSchemaData(string data, string? id = null)
        {
            if (!Regex.IsMatch(data, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            {
                _notifier.ShowError("Data is not valid BASE64.");
                return false;
            }
            var decoded = Compression.DecompressFromBase64(data);
            return LoadShareData(decoded.OutString, id);
        }

        internal BuildRecordDto GenerateDto()
        {
            var dto = new BuildRecordDto
            {
                Name = MidsContext.Character?.Name,
                Archetype = MidsContext.Character?.Archetype?.DisplayName,
                Description = MidsContext.Character?.Comment,
                Primary = MidsContext.Character?.Powersets[0]?.DisplayName,
                Secondary = MidsContext.Character?.Powersets[1]?.DisplayName,
                BuildData = GetShareData(),
                ImageData = InfoGraphic.GenerateImageData()
            };
            return dto;
        }
    }
}

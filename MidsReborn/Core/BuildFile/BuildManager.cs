using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile
{
    public class BuildManager
    {
        private static readonly Lazy<BuildManager> LazyInstance = new Lazy<BuildManager>(() => new BuildManager());
        public static BuildManager Instance => LazyInstance.Value;

        private CharacterBuildData? _buildData;
        private readonly IBuildNotifier _notifier;

        private BuildManager()
        {
            _buildData = CharacterBuildData.Instance;
            _notifier = new BuildNotifier();
        }

        public bool LoadFromFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var returnedVal = false;
            var schemaValidator = new Validator();
            var validationResult = schemaValidator.Validate(fileName);
            if (validationResult.Valid)
            {
                try
                {
                    _buildData = JsonConvert.DeserializeObject<CharacterBuildData>(validationResult.Data);
                }
                catch (Exception ex)
                {
                    _notifier.ShowError(ex.Message);
                }
            }
            else
            {
                _notifier.ShowError(validationResult.ErrorMessage);
                return false;
            }

            if (_buildData is null)
            {
                _notifier.ShowError($"Cannot load {fileName} - error reading build data.\r\n\r\n{nameof(_buildData)}");
                return false;
            }

            if (_buildData == null) return returnedVal;
            var metaData = _buildData.BuiltWith;
            if (metaData == null)
            {
                _notifier.ShowError($"Cannot load {fileName} - error reading build metadata.\r\n\r\n{nameof(metaData)}");
                return false;
            }

            // Compare App Version if Enabled
            if (MidsContext.Config?.WarnOnOldAppMbd == true)
            {
                var outDatedApp = Helpers.CompareVersions(MidsContext.AppFileVersion, metaData.Version);
                var newerApp = Helpers.CompareVersions(metaData.Version, MidsContext.AppFileVersion);

                if (outDatedApp)
                {
                    _notifier.ShowWarning($"This build was created with an older version of {MidsContext.AppName}, some features may not be available.");
                }

                if (newerApp)
                {
                    _notifier.ShowWarning($"This build was created with an newer version of {MidsContext.AppName}.\r\nIt is recommended that you update the application.");
                }
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

                    if (outDatedDb)
                    {
                        _notifier.ShowWarning($"This build was created in an older version of the {metaData.Database} database.\r\nSome powers may have changed, you may need to rebuild some of it.");
                    }

                    if (newerDb)
                    {
                        _notifier.ShowWarning($"This build was created in an newer version of the {metaData.Database} database.\r\nIt is recommended that you update the database.");
                    }

                    if (!outDatedDb && !newerDb)
                    {
                        returnedVal = _buildData.LoadBuild();
                    }
                }
                else
                {
                    returnedVal = _buildData.LoadBuild();
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

            CharacterBuildData.UpdateData(MidsContext.Character);
            File.WriteAllText(fileName, JsonConvert.SerializeObject(_buildData, Formatting.Indented));
            return true;
        }
    }
}

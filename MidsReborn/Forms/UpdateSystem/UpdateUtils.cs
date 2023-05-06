using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;
using Process = System.Diagnostics.Process;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public static class UpdateUtils
    {
        private static AppUpdate? _appUpdate;
        private static DbUpdate? _dbUpdate;
        private static List<UpdateObject>? _updates;
        private static string? _tempFile;

        public enum UpdateTypes
        {
            Application,
            Database
        }

        public struct UpdateObject
        {
            public UpdateTypes Type { get; set; }
            public string Name { get; set; }
            public string Uri { get; set; }
            public string Version { get; set; }
            public string ExtractTo { get; set; }

            public UpdateObject(UpdateTypes type, string name, string uri, string ver, string extract)
            {
                Type = type;
                Name = name;
                Uri = uri;
                Version = ver;
                ExtractTo = extract;
            }
        }

        public static void CheckForUpdates(frmMain parent)
        {
            _appUpdate = new AppUpdate();
            _dbUpdate = new DbUpdate();
            _updates = new List<UpdateObject>();

            if (_appUpdate.IsAvailable)
            {
                _updates.Add(new UpdateObject(UpdateTypes.Application, MidsContext.AppName, $"{MidsContext.Config?.UpdatePath}", _appUpdate.Version.ToString(), $"{AppContext.BaseDirectory}"));
            }

            if (_dbUpdate.IsAvailable)
            {
                _updates.Add(new UpdateObject(UpdateTypes.Database, DatabaseAPI.DatabaseName, $"{DatabaseAPI.ServerData.ManifestUri}", _dbUpdate.Version.ToString(), $"{Files.BaseDataPath}"));
            }

            if (_updates.Count <= 0)
            {
                if (_appUpdate.Status == AppUpdate.ManifestStatus.Success &
                    _dbUpdate.Status == DbUpdate.ManifestStatus.Success)
                {
                    MessageBox.Show(@"There are no updates available at this time.", @"Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            CreateTempFile();
            InitiateQuery(parent, _updates);
        }

        private static void CreateTempFile()
        {
            _tempFile = string.Empty;
            try
            {
                _tempFile = Path.GetTempFileName();
                File.WriteAllText(_tempFile, JsonConvert.SerializeObject(_updates));
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Error while creating the temporary update file: " + e.Message);
            }
        }

        private static void InitiateQuery(frmMain iParent, List<UpdateObject> updates)
        {
            var result = new UpdateQuery(iParent, updates);
            result.ShowDialog();
            switch (result.DialogResult)
            {
                case DialogResult.Abort:
                    result.Close();
                    break;
                case DialogResult.Continue:
                    if (_tempFile == null)
                    {
                        result.Close();
                    }
                    else
                    {
                        ExecuteUpdate(_tempFile);
                    }

                    break;
            }
        }

        private static void ExecuteUpdate(string file)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = Application.StartupPath,
                FileName = @"MRBUpdater.exe",
                Arguments = $"{file} {Environment.ProcessId}"
            };

            Process.Start(startInfo);
        }
    }
}

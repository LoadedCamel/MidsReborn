using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.UpdateSystem.Models;
using Newtonsoft.Json;
using Process = System.Diagnostics.Process;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public static class UpdateUtils
    {
        private static readonly List<UpdateObject> Updates = new();
        private static string? _tempFile;

        private static void CheckForApp()
        {
            var serializer = new XmlSerializer(typeof(UpdateResponse));
            try
            {
                var reader = new XmlTextReader(MidsContext.Config.UpdatePath!);
                if (serializer.Deserialize(reader) is not UpdateResponse updateResponse)
                {
                    return;
                }

                var isAvailable = Helpers.CompareVersions(Version.Parse(updateResponse.UpdateVersion), MidsContext.AppFileVersion);
                if (!isAvailable) return;
                Updates.Add(new UpdateObject(PatchType.Application, MidsContext.AppName, $"{MidsContext.Config.UpdatePath}", updateResponse.UpdateVersion, updateResponse.UpdateFile, $"{AppContext.BaseDirectory}"));
            }
            catch
            {
                // ignored
            }
        }

        private static void CheckForDatabase()
        {
            var serializer = new XmlSerializer(typeof(UpdateResponse));
            try
            {
                var reader = new XmlTextReader(DatabaseAPI.ServerData.ManifestUri);
                if (serializer.Deserialize(reader) is not UpdateResponse updateResponse)
                {
                    return;
                }

                var isAvailable = Helpers.CompareVersions(Version.Parse(updateResponse.UpdateVersion), DatabaseAPI.Database.Version);
                if (!isAvailable) return;
                Updates.Add(new UpdateObject(PatchType.Database, DatabaseAPI.DatabaseName, $"{DatabaseAPI.ServerData.ManifestUri}", updateResponse.UpdateVersion, updateResponse.UpdateFile, $"{Files.BaseDataPath}"));
            }
            catch
            {
                // ignored
            }
        }
        
        public static void CheckForUpdates(IWin32Window parent, bool checkDelay = false)
        {
            if (checkDelay)
            {
                if (!DelayCheckAvailable) return;
            }
            CheckForApp();
            CheckForDatabase();
            MidsContext.Config.AutomaticUpdates.LastChecked = DateTime.UtcNow;

            if (!Updates.Any())
            {
                var updateMsg = new MessageBoxEx(@"Update Check", @"There aren't any updates available at this time.", MessageBoxEx.MessageBoxButtons.Okay);
                updateMsg.ShowDialog(parent);
                return;
            }

            _tempFile = Path.GetTempFileName();
            File.WriteAllText(_tempFile, JsonConvert.SerializeObject(Updates));
            InitiateQuery(parent);
        }

        private static void InitiateQuery(IWin32Window parent)
        {
            var result = new UpdateQuery(Updates);
            result.ShowDialog(parent);
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
                Arguments = $"\"{file}\" {Environment.ProcessId}"
            };

            Process.Start(startInfo);
        }

        private static bool DelayCheckAvailable
        {
            get
            {
                var delay = MidsContext.Config.AutomaticUpdates.Delay;
                var lastChecked = MidsContext.Config.AutomaticUpdates.LastChecked;
                if (lastChecked == null) return true;
                return DateTime.Compare(DateTime.UtcNow, lastChecked.Value.AddDays(delay)) > 0;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private static readonly List<UpdateDetails> Updates = new();
        private static string? _tempFile;

        private static async Task CheckForApp()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(MidsContext.Config?.UpdatePath);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var serializer = new XmlSerializer(typeof(Manifest));
                using var reader = new StringReader(content);
                if (serializer.Deserialize(reader) is Manifest manifest)
                {
                    var isAvailable =
                        Helpers.CompareVersions(Version.Parse(manifest.Version), MidsContext.AppFileVersion);
                    if (!isAvailable) return;
                    Updates.Add(new UpdateDetails(PatchType.Application, MidsContext.AppName,
                        $"{MidsContext.Config?.UpdatePath}", manifest.Version, manifest.File,
                        $"{AppContext.BaseDirectory}"));
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        private static async Task CheckForDatabase()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(DatabaseAPI.ServerData.ManifestUri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var serializer = new XmlSerializer(typeof(Manifest));
                using var reader = new StringReader(content);
                if (serializer.Deserialize(reader) is Manifest manifest)
                {
                    var isAvailable =
                        Helpers.CompareVersions(Version.Parse(manifest.Version), DatabaseAPI.Database.Version);
                    if (!isAvailable) return;
                    Updates.Add(new UpdateDetails(PatchType.Database, DatabaseAPI.DatabaseName,
                        $"{DatabaseAPI.ServerData.ManifestUri}", manifest.Version, manifest.File,
                        $"{Files.BaseDataPath}"));
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }
        
        public static async Task CheckForUpdates(IWin32Window parent, bool checkDelay = false)
        {
            if (checkDelay)
            {
                if (!DelayCheckAvailable) return;
            }
            await CheckForApp();
            await CheckForDatabase();
            MidsContext.Config.AutomaticUpdates.LastChecked = DateTime.UtcNow;

            if (!Updates.Any())
            {
                var updateMsg = new MessageBoxEx(@"Update Check", @"There aren't any updates available at this time.", MessageBoxEx.MessageBoxExButtons.Okay);
                updateMsg.ShowDialog(parent);
                return;
            }

            _tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(_tempFile, JsonConvert.SerializeObject(Updates));
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

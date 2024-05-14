using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile
{
    public class BuildPreferences
    {
        [JsonProperty]
        public HashSet<string> WarningsIgnoredFor { get; } = new();

        private static readonly string PreferencesFilePath = Path.Combine(AppContext.BaseDirectory, "BuildPreferences.json");

        public static BuildPreferences Load()
        {
            if (!File.Exists(PreferencesFilePath)) return new BuildPreferences();
            var json = File.ReadAllText(PreferencesFilePath);
            return JsonConvert.DeserializeObject<BuildPreferences>(json) ?? new BuildPreferences();
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(PreferencesFilePath, json);
        }
    
        public bool ShouldSkipWarning(string buildFile)
        {
            return WarningsIgnoredFor.Contains(buildFile);
        }

        public void AddIgnoredBuild(string buildPath)
        {
            if (WarningsIgnoredFor.Any(x => x == buildPath)) return;
            WarningsIgnoredFor.Add(buildPath);
            Save();
        }
    }
}

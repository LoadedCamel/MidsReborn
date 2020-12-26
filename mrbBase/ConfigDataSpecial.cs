using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace mrbBase
{
    public class ConfigDataSpecial
    {
        public Dictionary<string, object> Auth { get; set; }
        public Dictionary<string, List<string>> ValidatedServers { get; set; }
        public Dictionary<string, object> User { get; set; }
        public Dictionary<string, Dictionary<string, string>> Servers { get; set; }
        public Dictionary<string, object> BotUser { get; set; }
        public List<string> ServerList { get; set; }
        public int ServerCount { get; set; }
        public bool IsInitialized { get; set; }
        private static ConfigDataSpecial _current { get; set; }

        internal static ConfigDataSpecial Current
        {
            get
            {
                ConfigDataSpecial configDataSpecial;
                if ((configDataSpecial = _current) == null)
                    throw new InvalidOperationException("Special Config was not initialized before access");
                return configDataSpecial;
            }
        }

        private ConfigDataSpecial() : this(true)
        {
        }

        public static void Initialize(ISerialize serializer)
        {
            var confPath = Files.GetConfigSpFile();
            if (File.Exists(confPath) && confPath.EndsWith(".json"))
            {
                try
                {
                    var value = serializer.Deserialize<ConfigDataSpecial>(File.ReadAllText(confPath));
                    _current = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load special json config.\r\n\r\nMessage: {ex.Message}\r\nStack Trace: {ex.StackTrace}");
                }
            }
            else
            {
                _current = new ConfigDataSpecial(false);
            }

            _current.InitializeComponent();
        }

        private ConfigDataSpecial(bool deserializing)
        {
            if (deserializing) return;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
        }

        public static (bool, T) LoadJson<T>(ISerialize serializer, string filename)
        {
            return !File.Exists(filename) ? (false, default) : (true, serializer.Deserialize<T>(File.ReadAllText(filename)));
        }

        private void SaveJson(ISerialize serializer, object obj, string filename)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                var json = serializer.Serialize(obj);
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save raw config: " + ex.Message, ex.GetType().Name);
            }
        }
        private void Save(ISerialize serializer, string iFilename)
        {
            SaveJson(serializer, this, iFilename);
        }
        public void SaveConfig(ISerialize serializer)
        {
            Save(serializer, Files.GetConfigSpFile());
        }
    }

    public class DiscordServerObject
    {
        [JsonProperty(PropertyName = "id")] public string id { get; set; }
        [JsonProperty(PropertyName = "name")] public string name { get; set; }
    }

    public class DiscordServerInfo
    {
        public string ServerNumber { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class DiscordServerChannels
    {
        [JsonProperty(PropertyName = "id")] public string id { get; set; }
        [JsonProperty(PropertyName = "name")] public string name { get; set; }
        [JsonProperty(PropertyName = "type")] public int type { get; set; }
        [JsonProperty(PropertyName = "guild_id")] public int guild_id { get; set; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ValidatedServer
    {
        [JsonProperty(PropertyName = "name")] public string name { get; set; }
        [JsonProperty(PropertyName = "channels")] public List<string> channels { get; set; }
    }
}
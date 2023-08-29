using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Mids_Reborn.Core
{
    public sealed class ServerData
    {
        private static ServerData? _instance;
        private static readonly object Mutex = new();

        private static ServerData GetInstance()
        {
            if (_instance != null) return _instance;
            lock (Mutex)
            {
                _instance = new ServerData();
            }

            return _instance;
        }

        public static ServerData Instance => GetInstance();

        public ServerData()
        {
            ManifestUri = "https://midsreborn.com/mids_updates/db/update_manifest.xml";
            BaseToHit = 0.75f;
            BaseFlySpeed = 31.5f;
            BaseJumpSpeed = 21f;
            BaseJumpHeight = 4f;
            BasePerception = 500f;
            BaseRunSpeed = 21f;
            MaxFlySpeed = 86f;
            MaxJumpSpeed = 114.40f;
            MaxRunSpeed = 135.67f;
            MaxSlots = 67;
            EnableInherentSlotting = false;
            HealthSlots = 2;
            HealthSlot1Level = 8;
            HealthSlot2Level = 16;
            StaminaSlots = 2;
            StaminaSlot1Level = 12;
            StaminaSlot2Level = 22;
            EnabledIncarnates = new Dictionary<string, bool>
            {
                { "Alpha", true },
                { "Destiny", true },
                { "Genesis", false },
                { "Hybrid", true },
                { "Interface", true },
                { "Judgement", true },
                { "Lore", true },
                { "Omega", false },
                { "Stance", false },
                { "Vitae", false },
            };
        }

        [JsonProperty]
        public string ManifestUri { get; set; }
        [JsonProperty]
        public float BaseToHit { get; set; }
        [JsonProperty]
        public float BaseFlySpeed { get; set; }
        [JsonProperty]
        public float BaseJumpSpeed { get; set; }
        [JsonProperty]
        public float BaseJumpHeight { get; set; }
        [JsonProperty]
        public float BaseRunSpeed { get; set; }
        [JsonProperty]
        public float BasePerception { get; set; }
        [JsonProperty]
        public float MaxFlySpeed { get; set; }
        [JsonProperty]
        public float MaxJumpSpeed { get; set; }
        [JsonProperty]
        public float MaxRunSpeed { get; set; }
        [JsonProperty]
        public int MaxSlots { get; set; }
        [JsonProperty]
        public bool EnableInherentSlotting { get; set; }
        [JsonProperty]
        public int HealthSlots { get; set; }
        [JsonProperty]
        public int HealthSlot1Level { get; set; }
        [JsonProperty]
        public int HealthSlot2Level { get; set; }
        [JsonProperty]
        public int StaminaSlots { get; set; }
        [JsonProperty]
        public int StaminaSlot1Level { get; set; }
        [JsonProperty]
        public int StaminaSlot2Level { get; set; }
        [JsonProperty]
        public Dictionary<string, bool> EnabledIncarnates { get; set; }

        public static void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(_instance, Formatting.Indented));
        }
        public static bool Load(string path)
        {
            try
            {
                _instance = JsonConvert.DeserializeObject<ServerData>(File.ReadAllText(path));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

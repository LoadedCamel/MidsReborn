using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core
{
    public sealed class CombatModLookup
    {
        [JsonProperty]
        public float[] ToHit { get; set; } = [];
        [JsonProperty]
        public float[] Magnitude { get; set; } = [];
        [JsonProperty]
        public float[] Duration { get; set; } = [];
        [JsonProperty]
        public float[] Accuracy { get; set; } = [];

        public bool HasToHit => ToHit.Length > 0;
        public bool HasAccuracy => Accuracy.Length > 0;
        public bool HasMagnitude => Magnitude.Length > 0;
        public bool HasDuration => Duration.Length > 0;
        public bool HasFullPlannerData => HasToHit && HasAccuracy && HasMagnitude && HasDuration;
    }

    public sealed class CombatModsBand
    {
        [JsonProperty]
        public int MinSize { get; set; }
        [JsonProperty]
        public int MaxSize { get; set; } = 100;
        [JsonProperty]
        public CombatModLookup HigherLevel { get; set; } = new();
        [JsonProperty]
        public CombatModLookup LowerLevel { get; set; } = new();
    }

    public sealed class CombatModsTableData
    {
        [JsonProperty]
        public float PvPToHitMod { get; set; }
        [JsonProperty]
        public float PvPElusivityMod { get; set; }
        [JsonProperty]
        public float[] ToHitLevelMod { get; set; } = [];
        [JsonProperty]
        public List<CombatModsBand> CombatMods { get; set; } = [];
    }

    public sealed class CombatModSnapshot
    {
        public float ToHit { get; set; } = 1f;
        public float Accuracy { get; set; } = 1f;
        public float Magnitude { get; set; } = 1f;
        public float Duration { get; set; } = 1f;
        public bool HasToHit { get; set; }
        public bool HasAccuracy { get; set; }
        public bool HasMagnitude { get; set; }
        public bool HasDuration { get; set; }
        public bool HasFullPlannerData => HasToHit && HasAccuracy && HasMagnitude && HasDuration;
    }

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
            MaxJumpHeight = 50f * BaseJumpHeight;
            MaxRunSpeed = 135.67f;
            MaxMaxFlySpeed = 8.19f * BaseFlySpeed;
            MaxMaxJumpSpeed = 7.917f * BaseJumpSpeed;
            MaxMaxRunSpeed = 8.398f * BaseRunSpeed;
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
        public float MaxJumpHeight { get; set; }
        [JsonProperty]
        public float MaxRunSpeed { get; set; }
        [JsonProperty]
        public float MaxMaxFlySpeed { get; set; }
        [JsonProperty]
        public float MaxMaxJumpSpeed { get; set; }
        [JsonProperty]
        public float MaxMaxRunSpeed { get; set; }
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
        [JsonProperty]
        public int EnemyRelativeLevelMin { get; set; } = int.MinValue;
        [JsonProperty]
        public int EnemyRelativeLevelMax { get; set; } = int.MinValue;
        [JsonProperty]
        public CombatModsTableData? CombatModsPlayer { get; set; }
        [JsonProperty]
        public CombatModsTableData? CombatModsVillain { get; set; }

        public (int Min, int Max) ResolveEnemyRelativeLevelBounds(string? databaseName = null)
        {
            if (EnemyRelativeLevelMin != int.MinValue &&
                EnemyRelativeLevelMax != int.MinValue &&
                EnemyRelativeLevelMin <= EnemyRelativeLevelMax)
            {
                return (EnemyRelativeLevelMin, EnemyRelativeLevelMax);
            }

            return (databaseName ?? "Generic") switch
            {
                "Homecoming" => (-4, 7),
                "Rebirth" => (-4, 7),
                "Generic" => (-4, 7),
                _ => (-4, 7)
            };
        }

        public bool HasPlayerCombatModTables()
        {
            return CombatModsPlayer != null && CombatModsPlayer.CombatMods.Any();
        }

        public bool TryGetPlayerCombatModSnapshot(int teamSize, int relativeLevel, out CombatModSnapshot snapshot)
        {
            snapshot = new CombatModSnapshot();
            var lookup = ResolvePlayerCombatModLookup(teamSize, relativeLevel);
            if (lookup == null)
            {
                return false;
            }

            var delta = Math.Abs(relativeLevel);
            snapshot.ToHit = LookupCombatModValue(lookup.ToHit, delta, 1f, out var hasToHit);
            snapshot.Accuracy = LookupCombatModValue(lookup.Accuracy, delta, 1f, out var hasAccuracy);
            snapshot.Magnitude = LookupCombatModValue(lookup.Magnitude, delta, 1f, out var hasMagnitude);
            snapshot.Duration = LookupCombatModValue(lookup.Duration, delta, 1f, out var hasDuration);
            snapshot.HasToHit = hasToHit;
            snapshot.HasAccuracy = hasAccuracy;
            snapshot.HasMagnitude = hasMagnitude;
            snapshot.HasDuration = hasDuration;
            return hasToHit || hasAccuracy || hasMagnitude || hasDuration;
        }

        private CombatModLookup? ResolvePlayerCombatModLookup(int teamSize, int relativeLevel)
        {
            if (CombatModsPlayer == null || CombatModsPlayer.CombatMods.Count == 0)
            {
                return null;
            }

            var band = CombatModsPlayer.CombatMods
                           .FirstOrDefault(candidate => teamSize >= candidate.MinSize && teamSize <= candidate.MaxSize) ??
                       CombatModsPlayer.CombatMods.FirstOrDefault();
            if (band == null)
            {
                return null;
            }

            return relativeLevel >= 0 ? band.HigherLevel : band.LowerLevel;
        }

        private static float LookupCombatModValue(float[] values, int delta, float fallback, out bool present)
        {
            present = delta >= 0 && delta < values.Length;
            return present ? values[delta] : fallback;
        }

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

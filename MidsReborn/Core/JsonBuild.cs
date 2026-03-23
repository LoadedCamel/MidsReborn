using System;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core;

public class JsonBuild
{
    [JsonProperty("ok")]
    public bool IsOk { get; set; } // Unused. Assuming --all-- are ok!

    [JsonRequired]
    [JsonProperty("build")]
    public BuildInfo CharacterBuildInfo { get; set; } = new();

    public class BuildInfo 
    {
        [JsonRequired]
        [JsonProperty("character")]
        public CharacterInfo CharacterInfo { get; set; } = new();

        [JsonRequired]
        [JsonProperty("powers")]
        public Power[] Powers { get; set; } = [];

        [JsonProperty("buildNum")]
        public int BuildIndex { get; set; } = 1;

        [JsonProperty("dbBuildNum")]
        public int DbBuildIndex { get; set; }
    }

    public class CharacterInfo
    {
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("archetype")]
        public string Archetype { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("origin")]
        public string Origin { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("level")]
        public int Level { get; set; } // Dummy - Mids assume everyone is max level
    }

    public class Power
    {
        [JsonProperty("subId")]
        public int? SubID { get; set; } // Unused

        [JsonProperty("powerId")]
        public int? PowerId { get; set; } // Unused

        [JsonRequired]
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("powerSetName")]
        public string PowerSetName { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("powerName")]
        public string PowerName { get; set; } = string.Empty;

        [JsonProperty("powerLevelBought")]
        public int LevelPicked { get; set; } = 1;

        [JsonProperty("powerNumBoostsBought")]
        public int SlotsCount { get; set; } // Unused

        [JsonProperty("boosts")]
        public Boost[] Boosts { get; set; } = [];

        public string FullName => string.IsNullOrWhiteSpace(CategoryName) & string.IsNullOrWhiteSpace(PowerSetName) & string.IsNullOrWhiteSpace(PowerName)
            ? ""
            : $"{CategoryName}.{PowerSetName}.{PowerName}";

        public string FullPowerSetName => string.IsNullOrWhiteSpace(CategoryName) & string.IsNullOrWhiteSpace(PowerSetName)
            ? ""
            : $"{CategoryName}.{PowerSetName}";
    }

    public class Boost
    {
        private int _index;
        private int _boostLevel;

        // Null index is auto-granted slot.
        // Assign index 0 in this case.
        [JsonProperty("idx")]
        public int? Index
        {
            get => _index;
            set => _index = value ?? 0;
        }

        [JsonRequired]
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("powerSetName")]
        public string PowerSetName { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("boostName")]
        public string BoostName { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("level")]
        public int Level { get; set; } = 1;

        [JsonProperty("numCombines")]
        public int? BoostLevel
        {
            get => _boostLevel;
            set => _boostLevel = value ?? 0;
        }

        public string PowerFullName => string.IsNullOrWhiteSpace(CategoryName) & string.IsNullOrWhiteSpace(PowerSetName) & string.IsNullOrWhiteSpace(BoostName)
            ? ""
            : $"{CategoryName}.{PowerSetName}.{BoostName}";

        public bool IsIOAttuned => !string.IsNullOrEmpty(PowerFullName) && PowerFullName.StartsWith("Attuned_");

        public Enums.eEnhRelative RelativeLevel => _boostLevel switch
        {
            1 => Enums.eEnhRelative.PlusOne,
            2 => Enums.eEnhRelative.PlusTwo,
            3 => Enums.eEnhRelative.PlusThree,
            4 => Enums.eEnhRelative.PlusFour,
            5 => Enums.eEnhRelative.PlusFive,
            _ => Enums.eEnhRelative.Even
        };

        public IPower? BoostPower =>
            DatabaseAPI.Database.Power
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e != null && e.FullName.Equals($"Boosts.{BoostName}", StringComparison.InvariantCultureIgnoreCase));

        public IEnhancement? Enhancement =>
            DatabaseAPI.Database.Enhancements
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e != null && e.UID.Equals(VaultImport.FormatEnhancementUid(BoostName), StringComparison.InvariantCultureIgnoreCase));
    }
}
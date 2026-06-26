using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mids_Reborn.Core;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    private sealed class OmniProgressionScheduleFile
    {
        [JsonProperty("character_levels")]
        public List<OmniProgressionScheduleRow> CharacterLevels { get; set; } = [];

        [JsonProperty("power_owned_levels")]
        public List<OmniPowerOwnedLevelScheduleRow> PowerOwnedLevels { get; set; } = [];
    }

    private sealed class OmniProgressionScheduleRow
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("power_picks")]
        public int PowerPicks { get; set; }

        [JsonProperty("enhancement_slot_picks")]
        public int EnhancementSlotPicks { get; set; }

        [JsonProperty("pool_power_set_unlocks")]
        public int PoolPowerSetUnlocks { get; set; }

        [JsonProperty("epic_power_set_unlocks")]
        public int EpicPowerSetUnlocks { get; set; }
    }

    private sealed class OmniPowerOwnedLevelScheduleRow
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("free_enhancement_slots_on_power")]
        public int FreeEnhancementSlotsOnPower { get; set; }
    }

    private BuildProgressionMetadata LoadBuildProgressionMetadata(string exportRoot, OmniDataProviderId providerId)
    {
        var path = Path.Combine(exportRoot, "progression_schedules.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Omni import requires progression_schedules.json so level, power pick, and slot rules can be stored in the database.",
                path);
        }

        var file = TryReadJson<OmniProgressionScheduleFile>(path);
        if (file?.CharacterLevels == null || file.CharacterLevels.Count == 0)
        {
            throw new InvalidDataException("Omni progression_schedules.json is missing character_levels.");
        }

        if (file.PowerOwnedLevels == null || file.PowerOwnedLevels.Count == 0)
        {
            throw new InvalidDataException("Omni progression_schedules.json is missing power_owned_levels.");
        }

        var characterLevels = file.CharacterLevels
            .Where(row => row.Level > 0)
            .OrderBy(row => row.Level)
            .Select(row => new CharacterLevelProgressionEntry
            {
                Level = row.Level,
                PowerPickDelta = row.PowerPicks,
                BoughtSlotDelta = row.EnhancementSlotPicks,
                PoolUnlockDelta = row.PoolPowerSetUnlocks,
                EpicUnlockDelta = row.EpicPowerSetUnlocks
            })
            .ToList();

        if (characterLevels.Count == 0)
        {
            throw new InvalidDataException("Omni progression_schedules.json does not contain any valid character level rows.");
        }

        var powerOwnedLevels = file.PowerOwnedLevels
            .Where(row => row is { Level: > 0, FreeEnhancementSlotsOnPower: > 0 })
            .OrderBy(row => row.Level)
            .Select(row => new PowerOwnedLevelProgressionEntry
            {
                Level = row.Level,
                FreeEnhancementSlotCount = row.FreeEnhancementSlotsOnPower
            })
            .ToList();

        if (powerOwnedLevels.Count == 0)
        {
            throw new InvalidDataException("Omni progression_schedules.json does not contain any valid power_owned_levels rows.");
        }

        var metadata = new BuildProgressionMetadata
        {
            MaxCharacterLevel = Math.Max(0, characterLevels.Max(row => row.Level) - 1),
            CharacterLevels = characterLevels,
            PowerOwnedLevels = powerOwnedLevels
        };

        // Preserve legacy Health/Stamina-style granted slots when a server profile still
        // relies on the old database-side knobs. New provider-specific grants can be added
        // directly to the metadata payload later without changing runtime logic.
        var legacyServerData = DatabaseAPI.ServerData;
        if (legacyServerData.EnableInherentSlotting)
        {
            metadata = BuildProgressionMetadata.CreateLegacy(
                new BuildProgressionPolicy(metadata).CreateChronologyLevels(),
                legacyServerData,
                providerId);
            metadata.CharacterLevels = characterLevels;
            metadata.PowerOwnedLevels = powerOwnedLevels;
        }

        return metadata;
    }
}

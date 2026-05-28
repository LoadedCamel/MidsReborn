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

    private BuildProgressionMetadata? LoadBuildProgressionMetadata(string exportRoot, OmniDataProviderId providerId)
    {
        var path = Path.Combine(exportRoot, "progression_schedules.json");
        var file = TryReadJson<OmniProgressionScheduleFile>(path);
        if (file?.CharacterLevels == null || file.CharacterLevels.Count == 0)
        {
            return null;
        }

        var metadata = new BuildProgressionMetadata
        {
            MaxCharacterLevel = Math.Max(0, file.CharacterLevels.Max(row => row.Level) - 1),
            CharacterLevels = file.CharacterLevels
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
                .ToList()
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
            metadata.CharacterLevels = file.CharacterLevels
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
        }

        return metadata;
    }
}

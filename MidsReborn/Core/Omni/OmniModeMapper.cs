using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mids_Reborn.Core;

namespace Mids_Reborn.Core.Omni;

public static class OmniModeMapper
{
    private static readonly Dictionary<int, (string Name, Enums.eModeFlags Flag)> ModeIds = new()
    {
        [0] = ("Peacebringer_Blaster_Mode", Enums.eModeFlags.Peacebringer_Blaster_Mode),
        [1] = ("Peacebringer_Tanker_Mode", Enums.eModeFlags.Peacebringer_Tanker_Mode),
        [2] = ("Peacebringer_Lightform_Mode", Enums.eModeFlags.Peacebringer_Lightform_Mode),
        [3] = ("Warshade_Blaster_Mode", Enums.eModeFlags.Warshade_Blaster_Mode),
        [4] = ("Warshade_Tanker_Mode", Enums.eModeFlags.Warshade_Tanker_Mode),
        [5] = ("Shivan_Mode", Enums.eModeFlags.Shivan_Mode),
        [6] = ("Disable_Travel", Enums.eModeFlags.Disable_Travel),
        [7] = ("Disable_Pool", Enums.eModeFlags.Disable_Pool),
        [8] = ("Disable_Temp", Enums.eModeFlags.Disable_Temp),
        [9] = ("Disable_Teleport", Enums.eModeFlags.Disable_Teleport),
        [11] = ("Disable_All", Enums.eModeFlags.Disable_All),
        [12] = ("Disable_Inspirations", Enums.eModeFlags.Disable_Inspirations),
        [20] = ("Arena", Enums.eModeFlags.Arena),
        [21] = ("Disable_Rez_Insp", Enums.eModeFlags.Disable_Rez_Insp),
        [22] = ("Disable_Toggle", Enums.eModeFlags.Disable_Toggle),
        [30] = ("Disable_Epic", Enums.eModeFlags.Disable_Epic),
        [32] = ("Raid_Attacker_Mode", Enums.eModeFlags.Raid_Attacker_Mode)
    };

    private static readonly Dictionary<string, Enums.eModeFlags> ModeFlags = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Arena"] = Enums.eModeFlags.Arena,
        ["Disable_All"] = Enums.eModeFlags.Disable_All,
        ["Disable_Enhancements"] = Enums.eModeFlags.Disable_Enhancements,
        ["Disable_Epic"] = Enums.eModeFlags.Disable_Epic,
        ["Disable_Inspirations"] = Enums.eModeFlags.Disable_Inspirations,
        ["Disable_Market_TP"] = Enums.eModeFlags.Disable_Market_TP,
        ["Disable_Pool"] = Enums.eModeFlags.Disable_Pool,
        ["Disable_Rez_Insp"] = Enums.eModeFlags.Disable_Rez_Insp,
        ["Disable_Teleport"] = Enums.eModeFlags.Disable_Teleport,
        ["Disable_Temp"] = Enums.eModeFlags.Disable_Temp,
        ["Disable_Toggle"] = Enums.eModeFlags.Disable_Toggle,
        ["Disable_Travel"] = Enums.eModeFlags.Disable_Travel,
        ["Domination"] = Enums.eModeFlags.Domination,
        ["Peacebringer_Blaster_Mode"] = Enums.eModeFlags.Peacebringer_Blaster_Mode,
        ["Bright_Nova"] = Enums.eModeFlags.Peacebringer_Blaster_Mode,
        ["Peacebringer_Lightform_Mode"] = Enums.eModeFlags.Peacebringer_Lightform_Mode,
        ["Peacebringer_Light_Mode"] = Enums.eModeFlags.Peacebringer_Lightform_Mode,
        ["Light_Form"] = Enums.eModeFlags.Peacebringer_Lightform_Mode,
        ["Peacebringer_Tanker_Mode"] = Enums.eModeFlags.Peacebringer_Tanker_Mode,
        ["White_Dwarf"] = Enums.eModeFlags.Peacebringer_Tanker_Mode,
        ["Raid_Attacker_Mode"] = Enums.eModeFlags.Raid_Attacker_Mode,
        ["Shivan_Mode"] = Enums.eModeFlags.Shivan_Mode,
        ["Warshade_Blaster_Mode"] = Enums.eModeFlags.Warshade_Blaster_Mode,
        ["Dark_Nova"] = Enums.eModeFlags.Warshade_Blaster_Mode,
        ["Warshade_Tanker_Mode"] = Enums.eModeFlags.Warshade_Tanker_Mode,
        ["Black_Dwarf"] = Enums.eModeFlags.Warshade_Tanker_Mode
    };

    private static readonly Dictionary<string, string> CanonicalNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kDefensiveAdaptation"] = "DefensiveAdaptation",
        ["kEfficientAdaptation"] = "EfficientAdaptation",
        ["kRestedAdaptation"] = "EfficientAdaptation",
        ["kOffensiveAdaptation"] = "OffensiveAdaptation",
        ["kDomination"] = "Domination",
        ["kScourge"] = "Scourge",
        ["kContainment"] = "Containment",
        ["kCriticalHit"] = "CriticalHit",
        ["kAssassination"] = "Assassination",
        ["kDefiance"] = "Defiance",
        ["kFastSnipe"] = "FastSnipe",
        ["kEngaged"] = "Engaged"
    };

    public static IReadOnlyList<string> KnownModeNames => ModeFlags.Keys
        .Concat(CanonicalNames.Values)
        .Concat(ModeIds.Values.Select(v => v.Name))
        .Concat(PlannerModeMapper.KnownModeNames)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public static int ModeCatalogCount { get; private set; } = ModeIds.Count;

    public static void LoadCatalog(string exportRoot)
    {
        if (string.IsNullOrWhiteSpace(exportRoot))
        {
            return;
        }

        var path = Path.Combine(exportRoot, "attribute_names.json");
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            var root = JObject.Parse(File.ReadAllText(path));
            if (root["pp_mode"] is not JArray modes)
            {
                return;
            }

            for (var index = 0; index < modes.Count; index++)
            {
                var rawName = GetModeName(modes[index]);
                if (string.IsNullOrWhiteSpace(rawName))
                {
                    continue;
                }

                var name = Normalize(rawName);
                ModeIds[index] = (name, ResolveFlag(name));
            }

            ModeCatalogCount = modes.Count;
        }
        catch (JsonException)
        {
            // Keep the built-in fallback table. The dry-run/apply report will still
            // surface unresolved effect mode ids if the export catalog cannot load.
        }
        catch (IOException)
        {
            // Keep the built-in fallback table if the catalog cannot be read.
        }
    }

    private static string GetModeName(JToken? token)
    {
        return token switch
        {
            null => string.Empty,
            JValue value => value.Value<string>() ?? string.Empty,
            JObject obj => obj.Value<string>("pch_name") ??
                           obj.Value<string>("name") ??
                           obj.Value<string>("display_name") ??
                           obj.Value<string>("pch_display_name") ??
                           string.Empty,
            _ => string.Empty
        };
    }

    public static Enums.eModeFlags ToFlags(IEnumerable<string> modes, out List<string> unknownModes)
    {
        unknownModes = [];
        var flags = Enums.eModeFlags.None;
        foreach (var mode in modes.Where(m => !string.IsNullOrWhiteSpace(m)))
        {
            if (TryToFlag(mode, out var flag))
            {
                flags |= flag;
                continue;
            }

            var normalized = Normalize(mode);
            if (TryFromModeName(normalized, out _, out _) ||
                PlannerModeMapper.TryGetPlannerMode(normalized, out _))
            {
                continue;
            }

            unknownModes.Add(normalized);
        }

        return flags;
    }

    public static bool TryToFlag(string mode, out Enums.eModeFlags flag)
    {
        var normalized = Normalize(mode);
        return ModeFlags.TryGetValue(normalized, out flag);
    }

    public static bool TryFromModeId(int modeId, out string name, out Enums.eModeFlags flag)
    {
        if (ModeIds.TryGetValue(modeId, out var mapped))
        {
            name = mapped.Name;
            flag = mapped.Flag;
            return true;
        }

        name = string.Empty;
        flag = Enums.eModeFlags.None;
        return false;
    }

    public static bool TryFromModeName(string mode, out int modeId, out Enums.eModeFlags flag)
    {
        var normalized = Normalize(mode);
        foreach (var (id, mapped) in ModeIds)
        {
            if (mapped.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase))
            {
                modeId = id;
                flag = mapped.Flag;
                return true;
            }
        }

        modeId = -1;
        flag = Enums.eModeFlags.None;
        return false;
    }

    public static bool IsSnipePlannerContext(string ownerFullName)
    {
        if (string.IsNullOrWhiteSpace(ownerFullName))
        {
            return false;
        }

        var normalized = ownerFullName.Replace('\\', '.').Replace('/', '.');
        return normalized.Contains("snipe", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("sniper", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("ranged_shot", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("blazing_bolt", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("moonbeam", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("zapp", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("proton_volley", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("psionic_lance", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("penetrating_ray", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("tombstone", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("direct_strike", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("masterful_throw", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("frozen_spear", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("mace_beam", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("lrm_rocket", StringComparison.OrdinalIgnoreCase);
    }

    public static string Normalize(string mode)
    {
        mode = mode.Trim().Trim('\'', '"');
        if (CanonicalNames.TryGetValue(mode, out var canonical))
        {
            return canonical;
        }

        return mode.StartsWith("k", StringComparison.Ordinal) && mode.Length > 1 && char.IsUpper(mode[1])
            ? mode[1..]
            : mode;
    }

    public static bool IsKnownBuildSourceMode(string mode)
    {
        return PlannerModeMapper.TryGetPlannerMode(Normalize(mode), out _);
    }

    public static bool TryGetPlannerMode(string mode, out PlannerMode plannerMode)
    {
        return PlannerModeMapper.TryGetPlannerMode(Normalize(mode), out plannerMode);
    }

    private static Enums.eModeFlags ResolveFlag(string mode)
    {
        var normalized = Normalize(mode);
        if (ModeFlags.TryGetValue(normalized, out var flag))
        {
            return flag;
        }

        return Enums.eModeFlags.None;
    }
}

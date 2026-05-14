using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Core
{
    public static partial class DatabaseAPI
    {
        private const int UntypedSetTypeIndex = 0;
        private const int FlightSetTypeIndex = 17;
        private const int LeapingSetTypeIndex = 18;
        private const int RunningSetTypeIndex = 19;
        private const int TeleportSetTypeIndex = 20;

        private static readonly string[] LegacySetTypeNames =
        [
            "Untyped",
            "Melee Damage",
            "Ranged Damage",
            "Targeted AoE Damage",
            "PBAoE Damage",
            "Sniper Attacks",
            "Pet Damage",
            "Defense Sets",
            "Resist Damage",
            "Healing",
            "Holds",
            "Stuns",
            "Immobilize",
            "Slow Movement",
            "Sleep",
            "Fear",
            "Confuse",
            "Flight",
            "Leaping",
            "Running",
            "Teleport",
            "Defense Debuff",
            "Endurance Modification",
            "Knockback",
            "Threat Duration",
            "To Hit Buff",
            "To Hit Debuff",
            "Recharge Intensive Pets",
            "Travel",
            "Accurate Healing",
            "Accurate Defense Debuff",
            "Accurate To-Hit Debuff",
            "Soldiers of Arachnos Archetype Sets",
            "Blaster Archetype Sets",
            "Brute Archetype Sets",
            "Controller Archetype Sets",
            "Corruptor Archetype Sets",
            "Defender Archetype Sets",
            "Dominator Archetype Sets",
            "Kheldian Archetype Sets",
            "Mastermind Archetype Sets",
            "Scrapper Archetype Sets",
            "Stalker Archetype Sets",
            "Tanker Archetype Sets",
            "Universal Damage",
            "Sentinel Archetype Sets",
            "Run (No Sprint)",
            "Jump (No Sprint)",
            "Flight (No Sprint)",
            "Teleport (No Sprint)"
        ];

        private static readonly string[] CanonicalSetTypeNames =
        [
            "Untyped",
            "Melee Damage",
            "Ranged Damage",
            "Ranged AoE Damage",
            "Melee AoE Damage",
            "Sniper Attacks",
            "Pet Damage",
            "Defense Sets",
            "Resist Damage",
            "Healing",
            "Holds",
            "Stuns",
            "Immobilize",
            "Slow Movement",
            "Sleep",
            "Fear",
            "Confuse",
            "Flight",
            "Leaping",
            "Running",
            "Teleport",
            "Defense Debuff",
            "Endurance Modification",
            "Knockback",
            "Threat Duration",
            "To Hit Buff",
            "To Hit Debuff",
            "Recharge Intensive Pets",
            "Universal Travel",
            "Accurate Healing",
            "Accurate Defense Debuff",
            "Accurate To-Hit Debuff",
            "Soldiers of Arachnos Archetype Sets",
            "Blaster Archetype Sets",
            "Brute Archetype Sets",
            "Controller Archetype Sets",
            "Corruptor Archetype Sets",
            "Defender Archetype Sets",
            "Dominator Archetype Sets",
            "Kheldian Archetype Sets",
            "Mastermind Archetype Sets",
            "Scrapper Archetype Sets",
            "Stalker Archetype Sets",
            "Tanker Archetype Sets",
            "Universal Damage",
            "Sentinel Archetype Sets"
        ];

        private static readonly string[] LegacySetTypeShortNames =
        [
            "Untyped",
            "MeleeST",
            "RangedST",
            "RangedAoE",
            "MeleeAoE",
            "Snipe",
            "Pets",
            "Defense",
            "Resistance",
            "Heal",
            "Hold",
            "Stun",
            "Immob",
            "Slow",
            "Sleep",
            "Fear",
            "Confuse",
            "Flight",
            "Jump",
            "Run",
            "Teleport",
            "DefDebuff",
            "EndMod",
            "Knockback",
            "Threat",
            "ToHit",
            "ToHitDeb",
            "PetRech",
            "Travel",
            "AccHeal",
            "AccDefDeb",
            "AccToHitDeb",
            "Arachnos",
            "Blaster",
            "Brute",
            "Controller",
            "Corruptor",
            "Defender",
            "Dominator",
            "Kheldian",
            "Mastermind",
            "Scrapper",
            "Stalker",
            "Tanker",
            "UniversalDamage",
            "Sentinel",
            "RunNoSprint",
            "JumpNoSprint",
            "FlightNoSprint",
            "TeleportNoSprint"
        ];

        private static readonly string[] CanonicalSetTypeShortNames =
        [
            "Untyped",
            "MeleeDamage",
            "RangedDamage",
            "RangedAoEDamage",
            "MeleeAoEDamage",
            "SniperAttacks",
            "PetDamage",
            "DefenseSets",
            "ResistDamage",
            "Healing",
            "Holds",
            "Stuns",
            "Immobilize",
            "SlowMovement",
            "Sleep",
            "Fear",
            "Confuse",
            "Flight",
            "Leaping",
            "Running",
            "Teleport",
            "DefenseDebuff",
            "EnduranceModification",
            "Knockback",
            "ThreatDuration",
            "ToHitBuff",
            "ToHitDebuff",
            "RechargeIntensivePets",
            "UniversalTravel",
            "AccurateHealing",
            "AccurateDefenseDebuff",
            "AccurateToHitDebuff",
            "SoldiersOfArachnosArchetypeSets",
            "BlasterArchetypeSets",
            "BruteArchetypeSets",
            "ControllerArchetypeSets",
            "CorruptorArchetypeSets",
            "DefenderArchetypeSets",
            "DominatorArchetypeSets",
            "KheldianArchetypeSets",
            "MastermindArchetypeSets",
            "ScrapperArchetypeSets",
            "StalkerArchetypeSets",
            "TankerArchetypeSets",
            "UniversalDamage",
            "SentinelArchetypeSets"
        ];

        private static readonly IReadOnlyDictionary<int, int> LegacySetTypeIndexAliases = new Dictionary<int, int>
        {
            [46] = RunningSetTypeIndex,
            [47] = LeapingSetTypeIndex,
            [48] = FlightSetTypeIndex,
            [49] = TeleportSetTypeIndex
        };

        private static readonly IReadOnlyDictionary<string, string> LegacySetTypeNameAliases = BuildLegacySetTypeNameAliases();
        private static readonly IReadOnlyDictionary<string, string> LegacySetTypeShortNameAliases = BuildLegacySetTypeShortNameAliases();
        private static readonly IReadOnlyDictionary<string, string[]> LegacySetTypeShortNameVariants = BuildLegacySetTypeShortNameVariants();

        public static int NormalizeSetTypeIndex(int index)
        {
            return LegacySetTypeIndexAliases.TryGetValue(index, out var normalizedIndex)
                ? normalizedIndex
                : index;
        }

        public static string CanonicalizeSetTypeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            var trimmed = name.Trim();
            return LegacySetTypeNameAliases.TryGetValue(trimmed, out var canonical)
                ? canonical
                : trimmed;
        }

        public static string CanonicalizeSetTypeShortName(string? shortName)
        {
            if (string.IsNullOrWhiteSpace(shortName))
            {
                return string.Empty;
            }

            var trimmed = shortName.Trim();
            return LegacySetTypeShortNameAliases.TryGetValue(trimmed, out var canonical)
                ? canonical
                : trimmed;
        }

        public static IEnumerable<string> GetSetTypeShortNameVariants(string? shortName)
        {
            var canonical = CanonicalizeSetTypeShortName(shortName);
            if (string.IsNullOrWhiteSpace(canonical))
            {
                yield break;
            }

            yield return canonical;

            if (!LegacySetTypeShortNameVariants.TryGetValue(canonical, out var legacyVariants))
            {
                yield break;
            }

            foreach (var variant in legacyVariants)
            {
                yield return variant;
            }
        }

        public static bool TryResolveSetType(IDatabase database, string? value, out TypeGrade setType)
        {
            setType = default;
            if (database?.SetTypes == null || database.SetTypes.Count == 0 || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var canonicalName = CanonicalizeSetTypeName(value);
            setType = database.SetTypes.FirstOrDefault(type => string.Equals(type.Name, canonicalName, StringComparison.OrdinalIgnoreCase));
            if (HasSetTypeValue(setType))
            {
                return true;
            }

            var canonicalShortName = CanonicalizeSetTypeShortName(value);
            setType = database.SetTypes.FirstOrDefault(type => string.Equals(type.ShortName, canonicalShortName, StringComparison.OrdinalIgnoreCase));
            if (HasSetTypeValue(setType))
            {
                return true;
            }

            var normalized = NormalizeSetTypeLookupKey(value);
            setType = database.SetTypes.FirstOrDefault(type =>
                NormalizeSetTypeLookupKey(type.Name) == normalized ||
                NormalizeSetTypeLookupKey(type.ShortName) == normalized);
            return HasSetTypeValue(setType);
        }

        public static void NormalizeSetTypeTaxonomy(IDatabase? database = null)
        {
            var targetDatabase = database ?? Database;
            if (targetDatabase == null)
            {
                return;
            }

            var validSetTypeCount = targetDatabase.SetTypes?.Count ?? 0;
            if (validSetTypeCount == 0)
            {
                return;
            }

            if (targetDatabase.Power != null)
            {
                foreach (var power in targetDatabase.Power)
                {
                    if (power?.SetTypes == null)
                    {
                        continue;
                    }

                    if (power.SetTypes.Count == 0)
                    {
                        BackfillPowerSetTypesFromOmniMetadata(targetDatabase, power);
                    }

                    if (power.SetTypes.Count == 0)
                    {
                        continue;
                    }

                    var normalizedSetTypes = power.SetTypes
                        .Select(NormalizeSetTypeIndex)
                        .Where(index => index >= 0 && index < validSetTypeCount)
                        .Distinct()
                        .ToList();

                    if (!power.SetTypes.SequenceEqual(normalizedSetTypes))
                    {
                        power.SetTypes = normalizedSetTypes;
                    }
                }
            }

            if (targetDatabase.EnhancementSets == null || targetDatabase.EnhancementSets.Count == 0)
            {
                return;
            }

            for (var index = 0; index < targetDatabase.EnhancementSets.Count; index++)
            {
                var enhancementSet = targetDatabase.EnhancementSets[index];
                var normalizedSetType = NormalizeSetTypeIndex(enhancementSet.SetType);
                if (normalizedSetType < 0 || normalizedSetType >= validSetTypeCount)
                {
                    normalizedSetType = UntypedSetTypeIndex;
                }

                if (enhancementSet.SetType != normalizedSetType)
                {
                    enhancementSet.SetType = normalizedSetType;
                    targetDatabase.EnhancementSets[index] = enhancementSet;
                }
            }
        }

        private static bool HasSetTypeValue(TypeGrade setType)
        {
            return !string.IsNullOrWhiteSpace(setType.Name);
        }

        private static void BackfillPowerSetTypesFromOmniMetadata(IDatabase database, IPower power)
        {
            if (power is not Power concretePower ||
                concretePower.OmniBoostPolicy.AllowedBoostSetCategories.Count == 0)
            {
                return;
            }

            var resolvedSetTypes = concretePower.OmniBoostPolicy.AllowedBoostSetCategories
                .Select(category => ResolveBoostSetCategoryToSetTypeIndex(database, category))
                .Where(index => index >= 0)
                .Distinct()
                .ToList();

            if (resolvedSetTypes.Count > 0)
            {
                power.SetTypes = resolvedSetTypes;
            }
        }

        private static int ResolveBoostSetCategoryToSetTypeIndex(IDatabase database, string? category)
        {
            if (TryResolveSetType(database, category, out var directMatch))
            {
                return directMatch.Index;
            }

            foreach (var alias in EnumerateBoostSetCategoryAliases(category))
            {
                if (TryResolveSetType(database, alias, out var aliasMatch))
                {
                    return aliasMatch.Index;
                }
            }

            return -1;
        }

        private static IEnumerable<string> EnumerateBoostSetCategoryAliases(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                yield break;
            }

            var trimmed = category.Trim();
            if (trimmed.EndsWith(" Sets", StringComparison.OrdinalIgnoreCase))
            {
                yield return trimmed[..^5];
            }

            if (trimmed.EndsWith("& Sprints", StringComparison.OrdinalIgnoreCase))
            {
                var withoutSprintSuffix = trimmed[..trimmed.LastIndexOf('&')].TrimEnd();
                if (!string.IsNullOrWhiteSpace(withoutSprintSuffix))
                {
                    yield return withoutSprintSuffix;
                }
            }
        }

        private static string NormalizeSetTypeLookupKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return new string(value
                .Trim()
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private static Dictionary<string, string> BuildLegacySetTypeNameAliases()
        {
            var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < LegacySetTypeNames.Length; index++)
            {
                var canonicalIndex = NormalizeSetTypeIndex(index);
                if (canonicalIndex < 0 || canonicalIndex >= CanonicalSetTypeNames.Length)
                {
                    continue;
                }

                var legacyName = LegacySetTypeNames[index];
                var canonicalName = CanonicalSetTypeNames[canonicalIndex];
                if (!string.Equals(legacyName, canonicalName, StringComparison.OrdinalIgnoreCase))
                {
                    aliases[legacyName] = canonicalName;
                }
            }

            return aliases;
        }

        private static Dictionary<string, string> BuildLegacySetTypeShortNameAliases()
        {
            var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < LegacySetTypeShortNames.Length; index++)
            {
                var canonicalIndex = NormalizeSetTypeIndex(index);
                if (canonicalIndex < 0 || canonicalIndex >= CanonicalSetTypeShortNames.Length)
                {
                    continue;
                }

                var legacyShortName = LegacySetTypeShortNames[index];
                var canonicalShortName = CanonicalSetTypeShortNames[canonicalIndex];
                if (!string.Equals(legacyShortName, canonicalShortName, StringComparison.OrdinalIgnoreCase))
                {
                    aliases[legacyShortName] = canonicalShortName;
                }
            }

            return aliases;
        }

        private static Dictionary<string, string[]> BuildLegacySetTypeShortNameVariants()
        {
            var variants = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < LegacySetTypeShortNames.Length; index++)
            {
                var canonicalIndex = NormalizeSetTypeIndex(index);
                if (canonicalIndex < 0 || canonicalIndex >= CanonicalSetTypeShortNames.Length)
                {
                    continue;
                }

                var canonicalShortName = CanonicalSetTypeShortNames[canonicalIndex];
                var legacyShortName = LegacySetTypeShortNames[index];
                if (string.Equals(canonicalShortName, legacyShortName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!variants.TryGetValue(canonicalShortName, out var values))
                {
                    values = [];
                    variants[canonicalShortName] = values;
                }

                values.Add(legacyShortName);
            }

            return variants.ToDictionary(
                pair => pair.Key,
                pair => pair.Value
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);
        }
    }
}

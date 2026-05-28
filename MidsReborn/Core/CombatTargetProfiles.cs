using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core;

public enum CombatTargetProfileId
{
    Boss = 0,
    Minion = 1,
    Lieutenant = 2,
    EliteBoss = 3,
    Archvillain = 4,
    BossEndgame = 5,
    EliteBossEndgame = 6,
    ArchvillainEndgame = 7,
    GiantMonster = 8,
    ChallengeBoss = 9,
    Hamidon = 10,
    HamidonMito = 11
}

public sealed record CombatTargetProfile(
    CombatTargetProfileId Id,
    string DisplayName,
    string ClassName,
    IReadOnlyCollection<string> Tags);

public static class CombatTargetProfiles
{
    private static readonly IReadOnlyDictionary<int, IReadOnlyCollection<string>> ClassAliases =
        new Dictionary<int, IReadOnlyCollection<string>>
        {
            [(int)CombatTargetProfileId.Minion] = new[]
            {
                "Class_Minion_Grunt",
                "Class_Minion_Small",
                "Class_Minion_Pets",
                "Class_Minion_Swarm",
                "Class_Minion_ControllerPets"
            },
            [(int)CombatTargetProfileId.Lieutenant] = new[]
            {
                "Class_Lt_Grunt"
            },
            [(int)CombatTargetProfileId.Boss] = new[]
            {
                "Class_Boss_Grunt"
            },
            [(int)CombatTargetProfileId.EliteBoss] = new[]
            {
                "Class_Boss_Elite"
            },
            [(int)CombatTargetProfileId.Archvillain] = new[]
            {
                "Class_Boss_Archvillain"
            },
            [(int)CombatTargetProfileId.BossEndgame] = new[]
            {
                "Class_Boss_PraetorianGrunt"
            },
            [(int)CombatTargetProfileId.EliteBossEndgame] = new[]
            {
                "Class_Boss_PraetorianElite"
            },
            [(int)CombatTargetProfileId.ArchvillainEndgame] = new[]
            {
                "Class_Boss_PraetorianArchvillain"
            },
            [(int)CombatTargetProfileId.GiantMonster] = new[]
            {
                "Class_Boss_Monster"
            },
            [(int)CombatTargetProfileId.ChallengeBoss] = new[]
            {
                "Class_Boss_ChallengeArchvillain"
            },
            [(int)CombatTargetProfileId.Hamidon] = new[]
            {
                "Class_Boss_Hamidon"
            },
            [(int)CombatTargetProfileId.HamidonMito] = new[]
            {
                "Class_Boss_Mito"
            }
        };

    private static readonly ReadOnlyCollection<CombatTargetProfile> Profiles = new List<CombatTargetProfile>
    {
        Create(
            CombatTargetProfileId.Minion,
            "Minion",
            "Class_Minion_Grunt",
            "Critter"),
        Create(
            CombatTargetProfileId.Lieutenant,
            "Lieutenant",
            "Class_Lt_Grunt",
            "Critter"),
        Create(
            CombatTargetProfileId.Boss,
            "Boss",
            "Class_Boss_Grunt",
            "Critter"),
        Create(
            CombatTargetProfileId.EliteBoss,
            "Elite Boss",
            "Class_Boss_Elite",
            "Critter"),
        Create(
            CombatTargetProfileId.Archvillain,
            "Archvillain",
            "Class_Boss_Archvillain",
            "Critter"),
        Create(
            CombatTargetProfileId.BossEndgame,
            "Boss (Endgame/HM)",
            "Class_Boss_PraetorianGrunt",
            "Critter",
            "IncarnateBoss"),
        Create(
            CombatTargetProfileId.EliteBossEndgame,
            "Elite Boss (Endgame/HM)",
            "Class_Boss_PraetorianElite",
            "Critter",
            "IncarnateBoss"),
        Create(
            CombatTargetProfileId.ArchvillainEndgame,
            "Archvillain (Endgame/HM)",
            "Class_Boss_PraetorianArchvillain",
            "Critter",
            "IncarnateBoss"),
        Create(
            CombatTargetProfileId.GiantMonster,
            "Giant Monster",
            "Class_Boss_Monster",
            "Critter",
            "Raid"),
        Create(
            CombatTargetProfileId.ChallengeBoss,
            "Challenge Boss / Raid GM",
            "Class_Boss_ChallengeArchvillain",
            "Critter",
            "IncarnateBoss",
            "Raid"),
        Create(
            CombatTargetProfileId.Hamidon,
            "Hamidon",
            "Class_Boss_Hamidon",
            "Critter",
            "IncarnateBoss",
            "Raid"),
        Create(
            CombatTargetProfileId.HamidonMito,
            "Hamidon Mito",
            "Class_Boss_Mito",
            "Critter",
            "Raid")
    }.AsReadOnly();

    public static IReadOnlyList<CombatTargetProfile> GetAll() => Profiles;

    public static CombatTargetProfile Get(CombatTargetProfileId id)
    {
        return Profiles.FirstOrDefault(profile => profile.Id == id) ??
               Profiles.First(profile => profile.Id == CombatTargetProfileId.Minion);
    }

    public static CombatTargetProfile Get(int id)
    {
        return Enum.IsDefined(typeof(CombatTargetProfileId), id)
            ? Get((CombatTargetProfileId)id)
            : Get(CombatTargetProfileId.Minion);
    }

    public static string GetDisplayName(int id)
    {
        return Get(id).DisplayName;
    }

    public static string GetClassName(int id)
    {
        return Get(id).ClassName;
    }

    public static bool HasTag(int id, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return false;
        }

        return Get(id).Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsCritter(int id)
    {
        return HasTag(id, "Critter");
    }

    public static bool MatchesClass(int id, string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            return false;
        }

        var normalized = className.Trim().Trim('\'', '"');
        if (ClassAliases.TryGetValue(id, out var aliases) &&
            aliases.Any(alias => alias.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return GetClassName(id).Equals(normalized, StringComparison.OrdinalIgnoreCase);
    }

    private static CombatTargetProfile Create(
        CombatTargetProfileId id,
        string displayName,
        string className,
        params string[] tags)
    {
        var profileTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CombatTargetProfile(id, displayName, className, profileTags);
    }
}

using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal static class PetActorMath
{
    public static ActorTotalsSnapshot Calculate(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IPower? setBonusPower)
    {
        return Calculate(
            className,
            mathPowers,
            buffedPowers,
            setBonusPower == null ? Array.Empty<IPower>() : new[] { setBonusPower });
    }

    public static ActorTotalsSnapshot Calculate(
        string className,
        IReadOnlyList<IPower> mathPowers,
        IReadOnlyList<IPower> buffedPowers,
        IEnumerable<IPower> externalPowers)
    {
        var selfEnhance = new Enums.BuffsX();
        var selfBuffs = new Enums.BuffsX();
        selfEnhance.Reset();
        selfBuffs.Reset();

        foreach (var power in mathPowers.Where(power => power != null))
        {
            DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(power, ref selfEnhance, PlannerRulesets.PlannerBucketPass.Enhancement);
        }

        foreach (var power in buffedPowers.Where(power => power != null))
        {
            DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(power, ref selfBuffs, PlannerRulesets.PlannerBucketPass.SelfBuff);
        }

        foreach (var externalPower in externalPowers.Where(power => power != null))
        {
            DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(externalPower, ref selfEnhance, PlannerRulesets.PlannerBucketPass.Enhancement);
            DatabaseAPI.GetPlannerRuleset().AccumulateBuckets(externalPower, ref selfBuffs, PlannerRulesets.PlannerBucketPass.SelfBuff);
        }

        var totals = new Character.TotalStatistics();
        var totalsCapped = new Character.TotalStatistics();
        totals.Init();
        totalsCapped.Init();

        var canFly = buffedPowers
            .Where(power => power != null)
            .SelectMany(power => power.Effects)
            .Any(effect => effect.EffectType == Enums.eEffectType.Fly && effect.Mag > 0);

        foreach (var power in buffedPowers.Where(power => power != null && power.PowerType == Enums.ePowerType.Toggle))
        {
            totals.EndUse += power.ToggleCost;
        }

        if (Math.Abs(selfBuffs.Defense[0]) > float.Epsilon)
        {
            for (var index = 1; index < selfBuffs.Defense.Length; index++)
            {
                selfBuffs.Defense[index] += selfBuffs.Defense[0];
            }
        }

        for (var index = 0; index < selfBuffs.Defense.Length; index++)
        {
            totals.Def[index] = selfBuffs.Defense[index];
            totals.Res[index] = selfBuffs.Resistance[index];
            totals.Elusivity[index] = selfBuffs.Elusivity[index];
        }

        for (var index = 0; index < selfBuffs.StatusProtection.Length; index++)
        {
            totals.Mez[index] = selfBuffs.StatusProtection[index];
            totals.MezRes[index] = selfBuffs.StatusResistance[index] * 100f;
        }

        for (var index = 0; index < selfBuffs.DebuffResistance.Length; index++)
        {
            totals.DebuffRes[index] = selfBuffs.DebuffResistance[index] * 100f;
        }

        totals.EndMax = selfBuffs.MaxEnd;
        totals.BuffAcc = selfEnhance.Effect[(int)Enums.eStatType.BuffAcc] + selfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
        totals.BuffEndRdx = selfEnhance.Effect[(int)Enums.eStatType.BuffEndRdx];
        totals.BuffHaste = selfEnhance.Effect[(int)Enums.eStatType.Haste] + selfBuffs.Effect[(int)Enums.eStatType.Haste];
        totals.BuffToHit = selfBuffs.Effect[(int)Enums.eStatType.ToHit];
        totals.Perception = Statistics.BasePerception * (1 + selfBuffs.Effect[(int)Enums.eStatType.Perception]);
        totals.StealthPvE = selfBuffs.Effect[(int)Enums.eStatType.StealthPvE];
        totals.StealthPvP = selfBuffs.Effect[(int)Enums.eStatType.StealthPvP];
        totals.ThreatLevel = selfBuffs.Effect[(int)Enums.eStatType.ThreatLevel];
        totals.HPRegen = selfBuffs.Effect[(int)Enums.eStatType.HPRegen];
        totals.EndRec = selfBuffs.Effect[(int)Enums.eStatType.EndRec];
        totals.Absorb = selfBuffs.Effect[(int)Enums.eStatType.Absorb];
        totals.BuffRange = selfBuffs.Effect[(int)Enums.eStatType.Range];

        totals.FlySpd = (1 + Math.Max(selfBuffs.Effect[(int)Enums.eStatType.FlySpeed], -0.9f)) * Statistics.BaseFlySpeed;
        totals.RunSpd = (1 + Math.Max(selfBuffs.Effect[(int)Enums.eStatType.RunSpeed], -0.9f)) * Statistics.BaseRunSpeed;
        totals.JumpSpd = (1 + Math.Max(selfBuffs.Effect[(int)Enums.eStatType.JumpSpeed], -0.9f)) * Statistics.BaseJumpSpeed;
        totals.JumpHeight = (1 + Math.Max(selfBuffs.Effect[(int)Enums.eStatType.JumpHeight], -0.9f)) * Statistics.BaseJumpHeight;

        totals.MaxFlySpd = Statistics.MaxFlySpeed + selfBuffs.Effect[(int)Enums.eStatType.MaxFlySpeed] * Statistics.BaseFlySpeed;
        totals.MaxRunSpd = Statistics.MaxRunSpeed + selfBuffs.Effect[(int)Enums.eStatType.MaxRunSpeed] * Statistics.BaseRunSpeed;
        totals.MaxJumpSpd = Statistics.MaxJumpSpeed + selfBuffs.Effect[(int)Enums.eStatType.MaxJumpSpeed] * Statistics.BaseJumpSpeed;

        totals.FlySpd = Math.Min(totals.FlySpd, DatabaseAPI.ServerData.MaxMaxFlySpeed);
        totals.RunSpd = Math.Min(totals.RunSpd, DatabaseAPI.ServerData.MaxMaxRunSpeed);
        totals.JumpSpd = Math.Min(totals.JumpSpd, DatabaseAPI.ServerData.MaxMaxJumpSpeed);

        totals.HPMax = selfBuffs.Effect[(int)Enums.eStatType.HPMax] + DatabaseAPI.GetClassHitPoints(className);
        if (!canFly)
        {
            totals.FlySpd = 0;
        }

        var maxDmgBuff = -1000f;
        var minDmgBuff = -1000f;
        var avgDmgBuff = 0f;
        for (var index = 0; index < selfBuffs.Damage.Length; index++)
        {
            if (index is <= 0 or >= 9)
            {
                continue;
            }

            if (selfEnhance.Damage[index] > maxDmgBuff)
            {
                maxDmgBuff = selfEnhance.Damage[index];
            }

            if (selfEnhance.Damage[index] < minDmgBuff)
            {
                minDmgBuff = selfEnhance.Damage[index];
            }

            avgDmgBuff += selfEnhance.Damage[index];
        }

        avgDmgBuff /= selfEnhance.Damage.Length;
        if (maxDmgBuff - avgDmgBuff < avgDmgBuff - minDmgBuff)
        {
            totals.BuffDam = maxDmgBuff;
        }
        else if (maxDmgBuff - avgDmgBuff > avgDmgBuff - minDmgBuff && minDmgBuff > 0)
        {
            totals.BuffDam = minDmgBuff;
        }
        else
        {
            totals.BuffDam = maxDmgBuff;
        }

        totalsCapped.Assign(totals);
        var archetype = DatabaseAPI.GetArchetypeByClassName(className);
        if (archetype != null)
        {
            DatabaseAPI.GetPlannerRuleset().ApplyFinalCaps(archetype, totals, totalsCapped);
        }

        return new ActorTotalsSnapshot
        {
            ClassName = className,
            Totals = totals,
            TotalsCapped = totalsCapped,
            DisplayStats = new ActorDisplayStats(className, totals, totalsCapped)
        };
    }
}

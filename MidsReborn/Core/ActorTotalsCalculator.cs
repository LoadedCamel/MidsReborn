using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

internal sealed class ActorTotalsCalculationRequest
{
    public string ClassName { get; init; } = string.Empty;
    public Archetype? Archetype { get; init; }
    public Enums.BuffsX SelfEnhance { get; init; }
    public Enums.BuffsX SelfBuffs { get; init; }
    public IReadOnlyCollection<IPower> IncludedBuffedPowers { get; init; } = Array.Empty<IPower>();
    public bool ApplyPvpDiminishingReturns { get; init; }
}

internal static class ActorTotalsCalculator
{
    public static ActorTotalsSnapshot Calculate(ActorTotalsCalculationRequest request)
    {
        var ruleset = DatabaseAPI.GetPlannerRuleset();
        var totals = new Character.TotalStatistics();
        var totalsCapped = new Character.TotalStatistics();
        totals.Init();
        totalsCapped.Init();

        var buffedPowers = request.IncludedBuffedPowers
            .Where(power => power != null)
            .ToArray();

        var canFly = buffedPowers
            .SelectMany(power => power.Effects)
            .Any(effect => effect.EffectType == Enums.eEffectType.Fly && effect.Mag > 0);

        foreach (var power in buffedPowers.Where(power => power.PowerType == Enums.ePowerType.Toggle))
        {
            totals.EndUse += power.ToggleCost;
        }

        var defense = request.SelfBuffs.Defense == null
            ? Array.Empty<float>()
            : (float[])request.SelfBuffs.Defense.Clone();

        if (defense.Length > 0 && Math.Abs(defense[0]) > float.Epsilon)
        {
            for (var index = 1; index < defense.Length; index++)
            {
                defense[index] += defense[0];
            }
        }

        for (var index = 0; index < request.SelfBuffs.Defense.Length; index++)
        {
            totals.Def[index] = defense[index];
            totals.Res[index] = request.SelfBuffs.Resistance[index];
            totals.Elusivity[index] = request.SelfBuffs.Elusivity[index];
        }

        for (var index = 0; index < request.SelfBuffs.StatusProtection.Length; index++)
        {
            totals.Mez[index] = request.SelfBuffs.StatusProtection[index];
            totals.MezRes[index] = request.SelfBuffs.StatusResistance[index] * 100f;
        }

        for (var index = 0; index < request.SelfBuffs.DebuffResistance.Length; index++)
        {
            totals.DebuffRes[index] = request.SelfBuffs.DebuffResistance[index] * 100f;
        }

        totals.EndMax = request.SelfBuffs.MaxEnd;
        totals.BuffAcc = request.SelfEnhance.Effect[(int)Enums.eStatType.BuffAcc] + request.SelfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
        totals.BuffEndRdx = request.SelfEnhance.Effect[(int)Enums.eStatType.BuffEndRdx];
        totals.BuffHaste = request.SelfEnhance.Effect[(int)Enums.eStatType.Haste] + request.SelfBuffs.Effect[(int)Enums.eStatType.Haste];
        totals.BuffToHit = request.SelfBuffs.Effect[(int)Enums.eStatType.ToHit];
        totals.Perception = Statistics.BasePerception * (1 + request.SelfBuffs.Effect[(int)Enums.eStatType.Perception]);
        totals.StealthPvE = request.SelfBuffs.Effect[(int)Enums.eStatType.StealthPvE];
        totals.StealthPvP = request.SelfBuffs.Effect[(int)Enums.eStatType.StealthPvP];
        totals.ThreatLevel = request.SelfBuffs.Effect[(int)Enums.eStatType.ThreatLevel];
        totals.HPRegen = request.SelfBuffs.Effect[(int)Enums.eStatType.HPRegen];
        totals.EndRec = request.SelfBuffs.Effect[(int)Enums.eStatType.EndRec];
        totals.Absorb = request.SelfBuffs.Effect[(int)Enums.eStatType.Absorb];
        totals.BuffRange = request.SelfBuffs.Effect[(int)Enums.eStatType.Range];

        totals.FlySpd = (1 + Math.Max(request.SelfBuffs.Effect[(int)Enums.eStatType.FlySpeed], -0.9f)) * Statistics.BaseFlySpeed;
        totals.RunSpd = (1 + Math.Max(request.SelfBuffs.Effect[(int)Enums.eStatType.RunSpeed], -0.9f)) * Statistics.BaseRunSpeed;
        totals.JumpSpd = (1 + Math.Max(request.SelfBuffs.Effect[(int)Enums.eStatType.JumpSpeed], -0.9f)) * Statistics.BaseJumpSpeed;
        totals.JumpHeight = (1 + Math.Max(request.SelfBuffs.Effect[(int)Enums.eStatType.JumpHeight], -0.9f)) * Statistics.BaseJumpHeight;

        totals.MaxFlySpd = Statistics.MaxFlySpeed + request.SelfBuffs.Effect[(int)Enums.eStatType.MaxFlySpeed] * Statistics.BaseFlySpeed;
        totals.MaxRunSpd = Statistics.MaxRunSpeed + request.SelfBuffs.Effect[(int)Enums.eStatType.MaxRunSpeed] * Statistics.BaseRunSpeed;
        totals.MaxJumpSpd = Statistics.MaxJumpSpeed + request.SelfBuffs.Effect[(int)Enums.eStatType.MaxJumpSpeed] * Statistics.BaseJumpSpeed;

        totals.FlySpd = Math.Min(totals.FlySpd, DatabaseAPI.ServerData.MaxMaxFlySpeed);
        totals.RunSpd = Math.Min(totals.RunSpd, DatabaseAPI.ServerData.MaxMaxRunSpeed);
        totals.JumpSpd = Math.Min(totals.JumpSpd, DatabaseAPI.ServerData.MaxMaxJumpSpeed);

        var archetype = request.Archetype ?? DatabaseAPI.GetArchetypeByClassName(request.ClassName);
        totals.HPMax = request.SelfBuffs.Effect[(int)Enums.eStatType.HPMax] + DatabaseAPI.GetClassHitPoints(archetype);
        if (!canFly)
        {
            totals.FlySpd = 0;
        }

        var maxDmgBuff = -1000f;
        var minDmgBuff = -1000f;
        var avgDmgBuff = 0f;
        for (var index = 0; index < request.SelfBuffs.Damage.Length; index++)
        {
            if (index is <= 0 or >= 9)
            {
                continue;
            }

            if (request.SelfEnhance.Damage[index] > maxDmgBuff)
            {
                maxDmgBuff = request.SelfEnhance.Damage[index];
            }

            if (request.SelfEnhance.Damage[index] < minDmgBuff)
            {
                minDmgBuff = request.SelfEnhance.Damage[index];
            }

            avgDmgBuff += request.SelfEnhance.Damage[index];
        }

        avgDmgBuff /= request.SelfEnhance.Damage.Length;
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

        if (request.ApplyPvpDiminishingReturns)
        {
            ruleset.ApplyPvpDiminishingReturns(totals);
        }

        totalsCapped.Assign(totals);
        if (archetype != null)
        {
            ruleset.ApplyFinalCaps(archetype, totals, totalsCapped);
        }

        return new ActorTotalsSnapshot
        {
            ClassName = request.ClassName,
            Totals = totals,
            TotalsCapped = totalsCapped,
            DisplayStats = new ActorDisplayStats(request.ClassName, totals, totalsCapped)
        };
    }
}

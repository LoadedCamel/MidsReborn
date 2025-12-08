using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
    public class PowerGrantsMap
    {
        public struct GrantCount
        {
            public float Probability;
            public float BaseProbability;
            public int Effects;
            public int StartIndex;
        }

        public Dictionary<int, GrantCount> Map { get; }

        private IPower SourcePower;
        private int BaseEffects;

        public PowerGrantsMap(IPower power)
        {
            SourcePower = power;
            var k = DatabaseAPI.GetPowerByFullName(power.FullName)!.Effects.Length;
            BaseEffects = k;
            Map = new Dictionary<int, GrantCount>();
            for (var i = 0; i < power.Effects.Length; i++)
            {
                if (power.Effects[i].EffectType != Enums.eEffectType.GrantPower)
                {
                    continue;
                }

                var numEffects = power.Effects[i].nSummon < 0 ? 0 :
                    DatabaseAPI.Database.Power[power.Effects[i].nSummon] == null ? 0 :
                    DatabaseAPI.Database.Power[power.Effects[i].nSummon]!.Effects.Length;

                Map.Add(i, new GrantCount
                {
                    Probability = power.Effects[i].Probability,
                    BaseProbability = power.Effects[i].BaseProbability,
                    Effects = numEffects,
                    StartIndex = k
                });

                k += numEffects;
            }
        }

        public int GetRealIndex(int fxIndex, bool needOffset = true)
        {
            return needOffset
                ? fxIndex + BaseEffects
                : fxIndex;
        }

        public IEffect? GetGrantRoot(int fxIndex, bool needOffset = true)
        {
            var realIndex = GetRealIndex(fxIndex, needOffset);
            
            return (from grc in Map where realIndex >= grc.Value.StartIndex && realIndex < grc.Value.StartIndex + grc.Value.Effects select SourcePower.Effects[grc.Key]).FirstOrDefault();
        }

        public float? GetGrantProbability(int fxIndex, bool getBaseProbability = false, bool needOffset = true)
        {
            var realIndex = GetRealIndex(fxIndex, needOffset);

            return getBaseProbability
                ? (from grc in Map where realIndex >= grc.Value.StartIndex && realIndex < grc.Value.StartIndex + grc.Value.Effects select SourcePower.Effects[grc.Key].BaseProbability).FirstOrDefault()
                : (from grc in Map where realIndex >= grc.Value.StartIndex && realIndex < grc.Value.StartIndex + grc.Value.Effects select SourcePower.Effects[grc.Key].Probability).FirstOrDefault();
        }

        public string[] GetRanges(bool grantDetail = true)
        {
            return grantDetail
                ? Map.Select(e => $"[{e.Value.StartIndex}; {e.Value.StartIndex + Math.Max(0, e.Value.Effects - 1)}] => {e.Key}").ToArray()
                : Map.Select(e => $"[{e.Value.StartIndex}; {e.Value.StartIndex + Math.Max(0, e.Value.Effects - 1)}]").ToArray();
        }
    }
}

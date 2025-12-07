using System.Diagnostics;
using System.Text;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core
{
    public enum ContributionBucket
    {
        Effect,
        EffectAux,
        Defense,
        Resistance,
        Damage,
        DebuffResistance,
        Mez,
        MezRes,
        StatusProtection,
        StatusResistance,
        Elusivity,
        MaxEnd
    }

    public enum ContributionChannel
    {
        Enhancements,
        Buffs
    }

    public readonly record struct ContributionKey(ContributionBucket Bucket, int Index);

    public sealed record ContributionSource(
        string Name,
        Enums.ePowerType PowerType,
        bool IsOn,
        bool IsSetBonusVirtual,
        bool IsPvpResist);

    public sealed record Contribution(
        ContributionKey Key,
        double Value,
        ContributionChannel Channel,
        ContributionSource Source,
        Enums.eEffectType? EffectType,
        Enums.eEffectType? ETModifies,
        Enums.eDamage? DamageType,
        Enums.eMez? MezType,
        Enums.eAspect? Aspect);

    /// <summary>
    /// Static, process-wide tracker (singleton) for accumulating per-source contributions
    /// during a single <see cref="Toon.GenerateBuffedPowerArray"/> build.
    /// </summary>
    public static class ContributionTracker
    {
        private static readonly object _gate = new();
        private static Dictionary<ContributionKey, List<Contribution>> _byKey = new();
        private static WeakReference<Character>? _character;

        /// <summary>
        /// Clears previous contributions and associates the tracker with the current character.
        /// Call once at the start of a build.
        /// </summary>
        public static void BeginBuild(Character? c)
        {
            lock (_gate)
            {
                _byKey = new Dictionary<ContributionKey, List<Contribution>>(128);
                _character = c is null ? null : new WeakReference<Character>(c);
            }
        }

        /// <summary>Record a contribution into a specific bucket/index.</summary>
        public static void Add(
            ContributionBucket bucket,
            int index,
            double value,
            IPower sourcePower,
            IEffect? fx,
            bool enhancementPass,
            bool sourceIsOn,
            bool isSetBonusVirtual = false,
            bool isPvpResist = false)
        {
            if (Math.Abs(value) < 1e-12) return;

            var key = new ContributionKey(bucket, index);
            var channel = enhancementPass ? ContributionChannel.Enhancements : ContributionChannel.Buffs;

            // --- Resolve a precise, user-facing name ---
            string friendly = string.Empty;

            if (sourcePower.GetPowerSet()?.SetType is Enums.ePowerSetType.Incarnate)
            {
                if (!string.IsNullOrWhiteSpace(sourcePower.SetName))
                {
                    friendly += $"({sourcePower.SetName}) ";
                }
            }

            if (!string.IsNullOrWhiteSpace(sourcePower.DisplayName))
            {
                friendly += sourcePower.DisplayName;
            }
            else
            {
                friendly += sourcePower.FullName;
            }
            
            var src = new ContributionSource(
                Name: friendly,
                PowerType: sourcePower.PowerType,
                IsOn: sourceIsOn,
                IsSetBonusVirtual: isSetBonusVirtual,
                IsPvpResist: isPvpResist);

            var c = new Contribution(
                Key: key,
                Value: value,
                Channel: channel,
                Source: src,
                EffectType: fx?.EffectType,
                ETModifies: fx?.ETModifies,
                DamageType: fx?.DamageType,
                MezType: fx?.MezType,
                Aspect: fx?.Aspect);

            lock (_gate)
            {
                if (!_byKey.TryGetValue(key, out var list))
                {
                    list = new List<Contribution>(4);
                    _byKey.Add(key, list);
                }
                list.Add(c);
            }
        }

        /// <summary>Return all raw contributions for a bucket/index.</summary>
        public static IReadOnlyList<Contribution> Get(ContributionBucket bucket, int index)
        {
            lock (_gate)
            {
                return _byKey.TryGetValue(new ContributionKey(bucket, index), out var list)
                    ? list.ToArray()
                    : Array.Empty<Contribution>();
            }
        }

        /// <summary>Return summed contributions grouped by (Source, Channel).</summary>
        public static IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)>
            GetSummed(ContributionBucket bucket, int index)
        {
            var list = Get(bucket, index);
            return list.GroupBy(c => (c.Source, c.Channel))
                       .Select(g => (g.Key.Source, g.Key.Channel, g.Sum(x => x.Value)));
        }
    }


}

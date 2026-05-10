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
    /// Compatibility adapter for legacy UI surfaces that still read the historical tracker shape.
    /// The planner snapshot/result model is the source of truth.
    /// </summary>
    public static class ContributionTracker
    {
        private static readonly object _gate = new();
        private static Dictionary<ContributionKey, List<Contribution>> _byKey = new();
        private static CalculationContributionSnapshot? _snapshot;
        private static WeakReference<Character>? _character;

        /// <summary>
        /// Clears any previously adapted contribution state for legacy callers.
        /// New planner code should prefer snapshot/result data over tracker lifecycle APIs.
        /// </summary>
        public static void BeginBuild(Character? c)
        {
            lock (_gate)
            {
                _byKey = new Dictionary<ContributionKey, List<Contribution>>(128);
                _snapshot = null;
                _character = c is null ? null : new WeakReference<Character>(c);
            }
        }

        /// <summary>
        /// Compatibility bridge for UI paths that still read from the tracker shape.
        /// The planner snapshot/result model is the source of truth.
        /// </summary>
        internal static void UseSnapshot(CalculationContributionSnapshot? snapshot)
        {
            lock (_gate)
            {
                _snapshot = snapshot;
            }
        }

        /// <summary>
        /// Legacy write surface retained for compatibility. Active planner flows should prefer
        /// building <see cref="CalculationContributionSnapshot"/> records instead.
        /// </summary>
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
                if (_snapshot != null)
                {
                    return _snapshot.GetBucketRecords(bucket, index)
                        .Where(record => record.Key.HasValue)
                        .Select(record => new Contribution(
                            record.Key!.Value,
                            record.Value,
                            record.Channel,
                            record.Source,
                            record.EffectType,
                            record.ETModifies,
                            record.DamageType,
                            record.MezType,
                            record.Aspect))
                        .ToArray();
                }

                return _byKey.TryGetValue(new ContributionKey(bucket, index), out var list)
                    ? list.ToArray()
                    : Array.Empty<Contribution>();
            }
        }

        /// <summary>Return summed contributions grouped by (Source, Channel).</summary>
        public static IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)>
            GetSummed(ContributionBucket bucket, int index)
        {
            lock (_gate)
            {
                if (_snapshot != null)
                {
                    return _snapshot.GetSummed(bucket, index).ToArray();
                }
            }

            var list = Get(bucket, index);
            return list.GroupBy(c => (c.Source, c.Channel))
                       .Select(g => (g.Key.Source, g.Key.Channel, g.Sum(x => x.Value)));
        }
    }


}

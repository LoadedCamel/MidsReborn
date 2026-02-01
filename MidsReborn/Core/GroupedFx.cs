using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using System.Text.RegularExpressions;
using FastDeepCloner;

namespace Mids_Reborn.Core
{
    #region List extensions

    public static class ListExtGFx
    {
        public static bool ContainsAll<T>(this List<T> list, IEnumerable<T> elements)
        {
            if (list.Count < 16)
            {
                foreach (var e in elements)
                    if (!list.Contains(e)) return false;
                return true;
            }

            var set = new HashSet<T>(list);
            foreach (var e in elements)
                if (!set.Contains(e)) return false;
            return true;
        }

        public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> elements)
        {
            var set = new HashSet<T>(list);
            foreach (var e in elements)
                if (set.Add(e)) list.Add(e);
        }
    }

    #endregion

    #region Dictionary extensions

    public static class DictionaryExtGFx
    {
        public static bool ContainsKeyPrefix<T1, T2>(this Dictionary<T1, T2> dict, string prefix, ref string? nameFound) where T1 : notnull
        {
            foreach (var k in dict.Keys)
            {
                var s = $"{k}";
                if (s.StartsWith(prefix, StringComparison.Ordinal))
                {
                    nameFound = s;
                    return true;
                }
            }
            nameFound = null;
            return false;
        }
    }

    #endregion

    public class GroupedFx : ICloneable
    {
        private const float Tolerance = 1e-4f;

        private struct EnhanceableFxId : IEquatable<EnhanceableFxId>
        {
            public Enums.eEffectType EffectType;
            public Enums.eEffectType? ETModifies;
            public Enums.eMez? MezType;
            public Enums.eToWho ToWho;
            public Enums.ePvX PvMode;

            public bool Equals(EnhanceableFxId other)
            {
                return EffectType == other.EffectType && ETModifies == other.ETModifies && ToWho == other.ToWho && PvMode == other.PvMode;
            }

            public override bool Equals(object? obj)
            {
                return obj is EnhanceableFxId other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)EffectType, ETModifies, (int)ToWho, (int)PvMode);
            }
        }

        public record struct FxId
        {
            public Enums.eEffectType EffectType;
            public Enums.eMez MezType;
            public Enums.eDamage DamageType;
            public Enums.eEffectType ETModifies;
            public Enums.eToWho ToWho;
            public Enums.ePvX PvMode;
            public int SummonId;
            public float Duration;
            public bool IgnoreScaling;

            public override string ToString()
            {
                return $"<FxId> {{Type: {EffectType}, Modifies: {ETModifies}, Mez: {MezType}, Damage: {DamageType}, ToWho: {ToWho}, PvMode: {PvMode}, IgnoreScaling: {IgnoreScaling}}}";
            }
        }

        public struct EnhancedMagSum
        {
            public float Base;
            public float Enhanced;

            public override string ToString()
            {
                return $"<EnhancedMagSum> {{Base: {Base}, Enhanced: {Enhanced}}}";
            }
        }

        private FxId FxIdentifier;
        private Enums.eSpecialCase SpecialCase;
        private float Mag;
        private string Alias;
        private List<int> IncludedEffects;
        private bool IsEnhancement;
        private IEffect? SingleEffectSource;
        private bool IsAggregated;
        private bool HasMisc;

        public int NumEffects => IncludedEffects.Count;
        public Enums.eEffectType EffectType => FxIdentifier.EffectType;
        public Enums.eEffectType ETModifies => FxIdentifier.ETModifies;
        public Enums.eMez MezType => FxIdentifier.MezType;
        public Enums.eDamage DamageType => FxIdentifier.DamageType;
        public Enums.eToWho ToWho => FxIdentifier.ToWho;
        public Enums.ePvX PvMode => FxIdentifier.PvMode;
        public bool IgnoreScaling => FxIdentifier.IgnoreScaling;
        public bool EnhancementEffect => IsEnhancement;

        public object Clone()
        {
            return (GroupedFx)MemberwiseClone();
        }

        public GroupedFx(FxId fxIdentifier, float mag, string alias,
            List<int> includedEffects, bool isEnhancement, Enums.eSpecialCase specialCase = Enums.eSpecialCase.None)
        {
            FxIdentifier = fxIdentifier;
            Mag = mag;
            Alias = alias;
            IncludedEffects = includedEffects;
            IsEnhancement = isEnhancement;
            SpecialCase = specialCase;
            SingleEffectSource = null;
            IsAggregated = false;
            HasMisc = false;
        }

        public GroupedFx(FxId fxIdentifier, List<GroupedFx> greList)
        {
            FxIdentifier = fxIdentifier;
            Mag = greList[0].Mag;
            Alias = greList[0].Alias;
            IsEnhancement = greList[0].IsEnhancement;
            SpecialCase = greList[0].SpecialCase;
            IsAggregated = true;
            SingleEffectSource = null;
            HasMisc = false;

            IncludedEffects = [];
            foreach (var gre in greList)
            {
                IncludedEffects.AddRangeUnique(gre.IncludedEffects);
            }

            if (IncludedEffects.Count <= 1)
            {
                IsAggregated = false;
            }

            IncludedEffects.Sort();
        }

        public GroupedFx(IEffect effect, int fxIndex)
        {
            SingleEffectSource = effect;
            FxIdentifier = new FxId
            {
                DamageType = effect.DamageType,
                EffectType = effect.EffectType,
                ETModifies = effect.ETModifies,
                MezType = effect.MezType,
                ToWho = effect.ToWho,
                PvMode = effect.PvMode,
                IgnoreScaling = effect.IgnoreScaling
            };
            Mag = effect.BuffedMag;
            Alias = "";
            IncludedEffects = [fxIndex];
            IsEnhancement = effect.isEnhancementEffect;
            SpecialCase = effect.SpecialCase;
            IsAggregated = false;
            HasMisc = false;
        }

        public GroupedFx()
        {
            SingleEffectSource = null;
            FxIdentifier = new FxId
            {
                DamageType = Enums.eDamage.None,
                EffectType = Enums.eEffectType.None,
                ETModifies = Enums.eEffectType.None,
                MezType = Enums.eMez.None,
                ToWho = Enums.eToWho.Unspecified,
                PvMode = Enums.ePvX.Any,
                IgnoreScaling = false
            };
            Mag = 0;
            Alias = "";
            IncludedEffects = [];
            IsEnhancement = false;
            SpecialCase = Enums.eSpecialCase.None;
            IsAggregated = false;
            HasMisc = false;

        }

        public override string ToString()
        {
            return $"<GroupedFx> {{{FxIdentifier}, effects: {IncludedEffects.Count}, Mag: {Mag}, EnhancementFx: {IsEnhancement}, Special case: {SpecialCase}, Aggregated: {IsAggregated}, HasMisc: {HasMisc}}}";
        }

        public int GetRankedEffectIndex(IEnumerable<int> rankedEffects, int index)
        {
            if (IncludedEffects.Count <= 0) return -1;

            return rankedEffects.TryFindIndex(e => e == IncludedEffects[index]);
        }

        public IEffect GetEffectAt(IPower power, int index = 0)
        {
            return power.Effects[IncludedEffects[index]];
        }

        private IEffect GetPowerEffectAt(IPower power, int index = 0)
        {
            return power.Effects[index];
        }

        // ===== Vector universes =====

        private static List<Enums.eDamage> GetAllDefensesEx()
        {
            return DatabaseAPI.RealmUsesToxicDef()
                ? new List<Enums.eDamage>
                {
                    Enums.eDamage.None,
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic,
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                }
                : new List<Enums.eDamage>
                {
                    Enums.eDamage.None,
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                };
        }

        private static List<Enums.eDamage> GetAllDefenses()
        {
            return DatabaseAPI.RealmUsesToxicDef()
                ? new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic,
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                }
                : new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                };
        }

        private static List<Enums.eDamage> GetPositionDefenses()
        {
            return new List<Enums.eDamage>
            {
                Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
            };
        }

        private static List<Enums.eDamage> GetTypedDefenses()
        {
            var damageTypes = Enum.GetValues(typeof(Enums.eDamage)).Cast<Enums.eDamage>().ToList();
            var returnedTypes = DatabaseAPI.RealmUsesToxicDefense switch
            {
                false => from type in damageTypes
                         where type is not Enums.eDamage.None
                             and not Enums.eDamage.Melee and not Enums.eDamage.Ranged and not Enums.eDamage.AoE
                             and not Enums.eDamage.Special and not Enums.eDamage.Unique1 and not Enums.eDamage.Unique2
                             and not Enums.eDamage.Unique3 and not Enums.eDamage.Toxic
                         select type,
                true => from type in damageTypes
                        where type is not Enums.eDamage.None
                            and not Enums.eDamage.Melee and not Enums.eDamage.Ranged and not Enums.eDamage.AoE
                            and not Enums.eDamage.Special and not Enums.eDamage.Unique1 and not Enums.eDamage.Unique2
                            and not Enums.eDamage.Unique3
                        select type
            };

            return returnedTypes.ToList();
        }

        private static List<Enums.eDamage> GetAllResistances()
        {
            return
            [
                Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic
            ];
        }

        private static List<Enums.eMez> GetAllMez()
        {
            return
            [
                Enums.eMez.Immobilized, Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep,
                Enums.eMez.Terrorized, Enums.eMez.Confused
            ];
        }

        private static List<Enums.eEffectType> GetAllMovement()
        {
            return
            [
                Enums.eEffectType.SpeedFlying, Enums.eEffectType.SpeedJumping, Enums.eEffectType.SpeedRunning
            ];
        }

        // ===== Stat naming (kept for tooltip transforms) =====

        public string GetStatName(IPower power)
        {
            var fx = power.Effects
                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                .Where(e => IncludedEffects.Contains(e.Key))
                .Select(e => e.Value)
                .ToList();

            if (fx.Count == 0) return "";

            var allDefenses = GetAllDefenses();
            var typedDefenses = GetTypedDefenses();
            var allResistances = GetAllResistances();
            var allMez = GetAllMez();
            var allMovement = GetAllMovement();

            var fxDamageTypes = fx.Select(e => e.DamageType).ToList();
            var fxMezTypes = fx.Select(e => e.MezType).ToList();
            var fxEffectTypes = fx.Select(e => e.ETModifies).ToList();
            var fxMainEffectTypes = fx.Select(e => e.EffectType).ToList();

            var groupedVector = FxIdentifier.EffectType switch
            {
                Enums.eEffectType.Defense or Enums.eEffectType.Elusivity =>
                    fxDamageTypes.ContainsAll(allDefenses) ? "All"
                    : fxDamageTypes.ContainsAll(typedDefenses) ? "All types"
                    : fxDamageTypes.Count > 1 ? "" : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Resistance or Enums.eEffectType.DamageBuff =>
                    fxDamageTypes.ContainsAll(allResistances) ? "All"
                    : fxDamageTypes.Count > 1 ? "" : $"{fxDamageTypes[0]}",

                Enums.eEffectType.MezResist =>
                    fxMezTypes.ContainsAll(allMez) ? "All"
                    : fxMezTypes.Count > 1 ? "" : $"{fxMezTypes[0]}",

                // NEW: Mez vectors same as others
                Enums.eEffectType.Mez =>
                    fxMezTypes.ContainsAll(allMez) ? "All"
                    : fxMezTypes.Count > 1 ? "" : $"{fxMezTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Mez or Enums.eEffectType.MezResist =>
                    fxMezTypes.ContainsAll(allMez) ? "All" : fxMezTypes.Count > 1 ? "" : $"{fxMezTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Elusivity =>
                    fxDamageTypes.ContainsAll(allDefenses) ? "All"
                    : fxDamageTypes.ContainsAll(typedDefenses) ? "All types"
                    : fxDamageTypes.Count > 1 ? "" : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Resistance =>
                    fxDamageTypes.ContainsAll(allResistances) ? "All"
                    : fxDamageTypes.Count > 1 ? "" : $"{fxDamageTypes[0]}",

                Enums.eEffectType.ResEffect => fxEffectTypes.Count > 1 ? "" : $"{fxEffectTypes[0]}",

                _ => ""
            };

            if (FxIdentifier.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning))
            {
                return groupedVector != "" ? $"{fx[0].EffectType} ({groupedVector})" : $"{fx[0].EffectType}";
            }

            if (fxMainEffectTypes.ContainsAll(allMovement))
            {
                return "Slow";
            }

            return groupedVector != "" ? $"{fx[0].EffectType} ({groupedVector})" : $"{fx[0].EffectType}";
        }

        private string GetGroupedVector(string statName, bool ignoreMulti = true) => "";
        private string GetGroupedVector(IPower power, bool ignoreMulti = true) => GetGroupedVector(GetStatName(power), ignoreMulti);

        // ===== Vector string builder used for labels =====

        private static string ComputeVectors(GroupedFx gre, IPower pEnh, Enums.eEffectType category, bool preferAll = true)
        {
            // Gather types from included effects
            var fx = gre.IncludedEffects
                .Where(i => i >= 0 && i < pEnh.Effects.Length)
                .Select(i => pEnh.Effects[i])
                .ToList();

            if (fx.Count == 0) return string.Empty;

            if (category == Enums.eEffectType.Mez || category == Enums.eEffectType.MezResist)
            {
                var set = fx.Select(e => e.MezType).Distinct().ToList();
                var all = GetAllMez();
                if (preferAll && set.ContainsAll(all)) return "All";
                return string.Join(", ", set.Select(m => $"{m}"));
            }

            if (category == Enums.eEffectType.Defense || category == Enums.eEffectType.Elusivity)
            {
                var set = fx.Select(e => e.DamageType).Distinct().ToList();
                var all = GetAllDefenses();
                var typed = GetTypedDefenses();
                var positions = GetPositionDefenses();

                if (preferAll && set.ContainsAll(all)) return "All";
                // If only positional, spell them out (no "All positions").
                if (set.All(d => positions.Contains(d))) return string.Join(", ", new[] { Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE }.Where(set.Contains));
                // Otherwise list whatever is present (typed and/or positions).
                return string.Join(", ", set);
            }

            if (category == Enums.eEffectType.Resistance)
            {
                var set = fx.Select(e => e.DamageType).Distinct().Where(d => d is not Enums.eDamage.Melee and not Enums.eDamage.Ranged and not Enums.eDamage.AoE and not Enums.eDamage.None).ToList();
                var all = GetAllResistances();
                if (preferAll && set.ContainsAll(all)) return "All";
                return string.Join(", ", set);
            }

            // For ETs that don’t use vectors (ToHit, RechargeTime, etc.)
            return string.Empty;
        }

        // ===== Compact list helpers (unchanged, still used in tooltip) =====

        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors, Enums.eEffectType effectType, Enums.eEffectType etModifies)
        {
            var allDefensesEx = GetAllDefensesEx()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allDefenses = GetAllDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var positionDefenses = GetPositionDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var typedDefenses = GetTypedDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allElusivity = GetAllDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var positionElusivity = GetPositionDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var typedElusivity = GetTypedDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var allResistances = GetAllResistances()
                .ToDictionary(e => $"{e} Resistance", _ => -1);

            var allMez = GetAllMez()
                .ToDictionary(e => $"{e}", _ => -1);

            var keyNameFound = "";
            for (var i = 0; i < vectors.Count; i++)
            {
                if (allDefensesEx.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allDefensesEx[keyNameFound] = i;
                }

                if (allDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allDefenses[keyNameFound] = i;
                }

                if (positionDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    positionDefenses[keyNameFound] = i;
                }

                if (typedDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    typedDefenses[keyNameFound] = i;
                }

                if (allElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allElusivity[keyNameFound] = i;
                }

                if (positionElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    positionElusivity[keyNameFound] = i;
                }

                if (typedElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    typedElusivity[keyNameFound] = i;
                }

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allResistances[keyNameFound] = i;
                }

                if (allMez.ContainsKey(vectors[i]))
                {
                    allMez[vectors[i]] = i;
                }
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

            switch (effectType)
            {
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Defense:
                    if (allDefensesEx.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All)");
                        ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
                    }
                    else if (allDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All)");
                        ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
                    }
                    else if (typedDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense (All types)");
                        ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
                    }
                    else if (typedDefenses.Count(e => e.Value >= 0) == typedDefenses.Count - 1)
                    {
                        var diff = typedDefenses.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Defense") ? e : $"{e} Defense")).First();
                        cVectors.Add($"Defense (All types but {diff.Replace(" Defense", "")})");
                        ignoredVectors.AddRangeUnique(typedDefenses.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Elusivity:
                    if (allElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity (All)");
                        ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
                    }
                    else if (typedElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity (All types)");
                        ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
                    }
                    else if (typedElusivity.Count(e => e.Value >= 0) == typedElusivity.Count - 1)
                    {
                        var diff = typedElusivity.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Elusivity") ? e : $"{e} Elusivity")).First();
                        cVectors.Add($"Elusivity (All types but {diff.Replace(" Elusivity", "")})");
                        ignoredVectors.AddRangeUnique(typedElusivity.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Resistance:
                    if (allResistances.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Resistance (All)");
                        ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
                    }
                    else if (allResistances.Count(e => e.Value >= 0) == allResistances.Count - 1)
                    {
                        var diff = allResistances.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Resistance") ? e : $"{e} Resistance")).First();
                        cVectors.Add($"Resistance (All but {diff.Replace(" Resistance", "")})");
                        ignoredVectors.AddRangeUnique(allResistances.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Mez:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Mez:
                    if (allMez.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Mez");
                        ignoredVectors.AddRangeUnique(allMez.Values.ToList());
                    }

                    break;
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));

            return CompactVectorsList(cVectors);
        }

        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors)
        {
            // (kept identical to your previous version except we don’t collapse to “All positions” anywhere)
            var allDefensesEx = GetAllDefensesEx().ToDictionary(e => $"{e} Defense", _ => -1);
            var allDefenses = GetAllDefenses().ToDictionary(e => $"{e} Defense", _ => -1);
            var typedDefenses = GetTypedDefenses().ToDictionary(e => $"{e} Defense", _ => -1);
            var allElusivity = GetAllDefenses().ToDictionary(e => $"{e} Elusivity", _ => -1);
            var typedElusivity = GetTypedDefenses().ToDictionary(e => $"{e} Elusivity", _ => -1);
            var allResistances = GetAllResistances().ToDictionary(e => $"{e} Resistance", _ => -1);
            var allMez = GetAllMez().ToDictionary(e => $"{e}", _ => -1);

            var keyNameFound = "";
            for (var i = 0; i < vectors.Count; i++)
            {
                if (allDefensesEx.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allDefensesEx[keyNameFound] = i;
                if (allDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allDefenses[keyNameFound] = i;
                if (typedDefenses.ContainsKeyPrefix(vectors[i], ref keyNameFound)) typedDefenses[keyNameFound] = i;

                if (allElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allElusivity[keyNameFound] = i;
                if (typedElusivity.ContainsKeyPrefix(vectors[i], ref keyNameFound)) typedElusivity[keyNameFound] = i;

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound)) allResistances[keyNameFound] = i;

                if (allMez.ContainsKey(vectors[i])) allMez[vectors[i]] = i;
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

            if (allDefensesEx.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All)");
                ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
            }
            else if (allDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All)");
                ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
            }
            else if (typedDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense (All types)");
                ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
            }

            if (allElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity (All)");
                ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
            }
            else if (typedElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity (All types)");
                ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
            }

            if (allResistances.All(e => e.Value >= 0))
            {
                cVectors.Add("Resistance (All)");
                ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
            }

            if (allMez.All(e => e.Value >= 0))
            {
                cVectors.Add("Mez");
                ignoredVectors.AddRangeUnique(allMez.Values.ToList());
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));
            return cVectors;
        }

        private static string InvertStringValue(string value)
        {
            return value.StartsWith('-') ? value[1..] : $"-{value}";
        }

        // ===== Tooltip =====

        public string GetTooltip(IPower power, bool simple = false)
        {
            var vectors = "";
            var statName = GetStatName(power);
            var groupedVector = GetGroupedVector(statName);

            if (!string.IsNullOrEmpty(groupedVector))
            {
                vectors = groupedVector;
            }
            else
            {
                var uniqueVectors = new List<string>();
                var vectorsChunks = power.Effects[IncludedEffects[0]].EffectType switch
                {
                    Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning => IncludedEffects
                            .Select(e => $"{power.Effects[e].EffectType}")
                            .ToList(),

                    Enums.eEffectType.Mez or Enums.eEffectType.MezResist => IncludedEffects
                        .Select(e => $"{power.Effects[e].MezType}")
                        .ToList(),

                    Enums.eEffectType.Enhancement when
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                            or Enums.eEffectType.MezResist => !string.IsNullOrEmpty(groupedVector)
                            ? [$"{groupedVector}"]
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].MezType}")
                                .ToList(),

                    Enums.eEffectType.Enhancement =>
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense
                            or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity &&
                        !string.IsNullOrEmpty(groupedVector)
                            ? [$"{groupedVector} {power.Effects[IncludedEffects[0]].ETModifies}"]
                            : IncludedEffects
                                .Select(e =>
                                    power.Effects[e].ETModifies is Enums.eEffectType.Defense
                                        or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                                        ? $"{power.Effects[e].DamageType} {power.Effects[e].ETModifies}"
                                        : $"{power.Effects[e].ETModifies}")
                                .ToList(),

                    Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                        or Enums.eEffectType.DamageBuff => !string.IsNullOrEmpty(groupedVector)
                            ? [$"{power.Effects[IncludedEffects[0]].EffectType}({groupedVector})"]
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].DamageType}")
                                .ToList(),

                    Enums.eEffectType.ResEffect => IncludedEffects
                        .Select(e => $"{power.Effects[e].ETModifies}")
                        .ToList(),

                    _ => []
                };

                uniqueVectors.AddRangeUnique(vectorsChunks);
                uniqueVectors = CompactVectorsList(uniqueVectors, power.Effects[IncludedEffects[0]].EffectType, power.Effects[IncludedEffects[0]].ETModifies);
                vectors = string.Join(", ", uniqueVectors);
            }

            var maxRange = IsAggregated && IncludedEffects.Count > 1 && IncludedEffects
                .Select(e => power.Effects[e].BuffedMag)
                .Any(e => e != power.Effects[IncludedEffects[0]].BuffedMag)
                ? IncludedEffects.Count
                : 1;

            var tip = "";
            for (var i = 0; i < maxRange; i++)
            {
                var baseEffectString = power.Effects[IncludedEffects[i]]
                    .BuildEffectString(simple, "", false, false, false, simple, false, true);

                var fxTip = power.Effects[IncludedEffects[i]].EffectType switch
                {
                    Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning =>
                        statName == "Slow"
                            ? InvertStringValue(Regex.Replace(baseEffectString,
                                @"(SpeedFlying|SpeedJumping|SpeedRunning)",
                                "Slow"))
                            : Regex.Replace(baseEffectString, @"(SpeedFlying|SpeedJumping|SpeedRunning)", vectors),

                    Enums.eEffectType.Mez or Enums.eEffectType.MezResist => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[i]].EffectType}({power.Effects[IncludedEffects[i]].MezType})",
                        $"{power.Effects[IncludedEffects[i]].EffectType}({vectors})"),

                    Enums.eEffectType.Enhancement when power.Effects[IncludedEffects[i]].ETModifies is Enums.eEffectType
                            .Mez
                        or Enums.eEffectType.MezResist => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[i]].EffectType}({power.Effects[IncludedEffects[i]].MezType})",
                        $"{power.Effects[IncludedEffects[i]].EffectType}({(vectors == "All" && power.Effects[IncludedEffects[i]].ETModifies == Enums.eEffectType.Mez ? "Mez" : vectors)})"),

                    Enums.eEffectType.Enhancement when
                        power.Effects[IncludedEffects[i]].ETModifies is Enums.eEffectType.Defense
                            or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity => baseEffectString.Replace(
                            $"{power.Effects[IncludedEffects[i]].EffectType}({power.Effects[IncludedEffects[i]].DamageType} {power.Effects[IncludedEffects[i]].ETModifies})",
                            $"{power.Effects[IncludedEffects[i]].EffectType}({vectors}{(power.Effects[IncludedEffects[^1]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity && vectors.Contains("All") ? $" {power.Effects[IncludedEffects[^1]].ETModifies}" : "")})"),

                    Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[i]].EffectType}({power.Effects[IncludedEffects[i]].ETModifies})",
                        $"{power.Effects[IncludedEffects[i]].EffectType}({vectors})"),

                    Enums.eEffectType.Resistance or Enums.eEffectType.Defense or Enums.eEffectType.Elusivity
                        or Enums.eEffectType.DamageBuff => baseEffectString.Replace(
                            $"{power.Effects[IncludedEffects[i]].EffectType}({power.Effects[IncludedEffects[i]].DamageType})",
                            $"{power.Effects[IncludedEffects[i]].EffectType}({vectors})"),

                    Enums.eEffectType.SilentKill => baseEffectString.Replace("SilentKill", "Self-Destructs")
                        .Replace(" in ", " after ")
                        .Replace(" to Self", "")
                        .Replace(" to Target", ""),

                    _ => baseEffectString
                };

                tip += $"{(string.IsNullOrEmpty(tip) ? "" : "\r\n")}{fxTip}";
            }

            return Regex.Replace(tip, @"(?<stat>[0-9A-Za-z\-]+)\(\k<stat>", "$1")
                .Replace("((", "(")
                .Replace("))", ")")
                .Replace("None Defense", "Base Defense");
        }

        // ===== Label / Value =====

        private static string BuildLabel(GroupedFx gre, IPower pEnh)
        {
            // Name pet/power for summon/grant/revoke
            if (gre.EffectType == Enums.eEffectType.EntCreate ||
                gre.EffectType == Enums.eEffectType.GrantPower ||
                gre.EffectType == Enums.eEffectType.RevokePower)
            {
                var effIdx = gre.IncludedEffects.FirstOrDefault(i => i >= 0 && i < pEnh.Effects.Length, -1);
                if (effIdx >= 0)
                {
                    var eff = pEnh.Effects[effIdx];
                    string? name = null;

                    if (eff.nSummon > -1 && eff.nSummon < DatabaseAPI.Database.Entities.Length)
                        name = DatabaseAPI.Database.Entities[eff.nSummon]?.DisplayName?.Trim();

                    if (string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(eff.Summon))
                        name = DatabaseAPI.GetPowerByFullName(eff.Summon)?.DisplayName?.Trim() ?? eff.Summon.Trim();

                    if (!string.IsNullOrEmpty(name)) return name;

                    return gre.EffectType switch
                    {
                        Enums.eEffectType.EntCreate => "Summon",
                        Enums.eEffectType.GrantPower => "Grant",
                        Enums.eEffectType.RevokePower => "Revoke",
                        _ => string.Empty
                    };
                }
            }

            // Strength-style for ETModified (Enhancement & ResEffect)
            if (gre.EffectType == Enums.eEffectType.Enhancement || gre.EffectType == Enums.eEffectType.ResEffect)
            {
                var cat = gre.ETModifies;
                string vec = cat switch
                {
                    Enums.eEffectType.Defense or Enums.eEffectType.Elusivity => ComputeVectors(gre, pEnh, cat),
                    Enums.eEffectType.Resistance => ComputeVectors(gre, pEnh, cat),
                    Enums.eEffectType.Mez or Enums.eEffectType.MezResist => ComputeVectors(gre, pEnh, Enums.eEffectType.Mez),
                    _ => ""
                };

                var core = $"{cat} Strength";
                return string.IsNullOrEmpty(vec) ? core : $"{core} ({vec})";
            }

            // Mez groups should show vectors like others
            if (gre.EffectType == Enums.eEffectType.Mez)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Mez);
                return string.IsNullOrEmpty(vec) ? "Mez" : $"Mez ({vec})";
            }

            if (gre.EffectType == Enums.eEffectType.MezResist)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.MezResist);
                return string.IsNullOrEmpty(vec) ? "Mez Resist" : $"Mez Resist ({vec})";
            }

            // Defense / Resistance / Elusivity labels enumerate vectors when not All
            if (gre.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Elusivity)
            {
                var vec = ComputeVectors(gre, pEnh, gre.EffectType);
                return string.IsNullOrEmpty(vec) ? $"{gre.EffectType}" : (vec == "All" ? $"{gre.EffectType} (All)" : $"{gre.EffectType} ({vec})");
            }

            if (gre.EffectType == Enums.eEffectType.Resistance)
            {
                var vec = ComputeVectors(gre, pEnh, Enums.eEffectType.Resistance);
                return string.IsNullOrEmpty(vec) ? "Resistance" : (vec == "All" ? "Resistance (All)" : $"Resistance ({vec})");
            }

            // Fallbacks
            var statName = gre.GetStatName(pEnh);
            if (!string.IsNullOrEmpty(statName)) return statName;

            return gre.EffectType switch
            {
                _ => gre.EffectType.ToString()
            };
        }

        private static string JoinDamageShort(IEnumerable<Enums.eDamage> types)
            => string.Join(",", types.Select(Enums.GetDamageNameShort));

        private static string JoinMezShort(IEnumerable<Enums.eMez> types)
            => string.Join(",", types.Select(t => Enums.GetMezNameShort((Enums.eMezShort)t)));

        private static (string value, string? alt, string? tip, bool isSpecial, bool isConditional, bool isUnique) BuildValueAndDecorations(GroupedFx gre, IPower pBase, IPower pEnh, int effectIndex, bool powerInBuild)
        {
            string tip = string.Empty;
            string value = string.Empty;

            if (gre.EffectType == Enums.eEffectType.EntCreate
                || gre.EffectType == Enums.eEffectType.GrantPower
                || gre.EffectType == Enums.eEffectType.RevokePower)
            {
                var eff = pEnh.Effects[Math.Clamp(effectIndex, 0, pEnh.Effects.Length - 1)];

                var count = Math.Max(1, gre.NumEffects);
                value = $"x{count}";

                tip = BuildEntCreateTooltip(gre, pEnh);

                return (value, null,
                    string.IsNullOrWhiteSpace(tip) ? null : tip,
                    eff.InherentSpecial || eff.InherentSpecial2,
                    eff.VariableModified,
                    false);
            }

            var baseMag = gre.GetMagSum(pBase, gre.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));
            var enhMag = gre.GetMagSum(pEnh, gre.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));

            bool asPercent = gre.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Resistance
                                                  or Enums.eEffectType.DamageBuff or Enums.eEffectType.ToHit
                                                  or Enums.eEffectType.RechargeTime or Enums.eEffectType.Elusivity
                                                  or Enums.eEffectType.Enhancement or Enums.eEffectType.MezResist;

            string fmt(float v) => asPercent ? $"{Utilities.FixDP(v * 100)}%" : Utilities.FixDP(v);

            value = $"{fmt(enhMag)}";
            var alt = baseMag is > Tolerance or < -Tolerance ? fmt(baseMag) : null;

            var tipTitle = BuildLabel(gre, pEnh);
            tip = $"{tipTitle}  Base: {fmt(baseMag)}  Enhanced: {fmt(enhMag)}";

            var isSpecial = gre.SpecialCase != Enums.eSpecialCase.None;
            var isConditional = false;
            var isUnique = gre.EnhancementEffect;

            return (value, alt, tip, isSpecial, isConditional, isUnique);
        }

        private static string BuildEntCreateTooltip(GroupedFx gre, IPower pEnh)
        {
            var effects = gre.IncludedEffects
                .Where(i => i >= 0 && i < pEnh.Effects.Length)
                .Select(i => pEnh.Effects[i])
                .OrderBy(e => e.DelayedTime)
                .ToList();

            if (effects.Count == 0) return "Summons";

            var first = effects[0];

            string name = first.SummonedEntityName;
            if (string.IsNullOrWhiteSpace(name))
            {
                if (first.nSummon > -1 && first.nSummon < DatabaseAPI.Database.Entities.Length)
                    name = DatabaseAPI.Database.Entities[first.nSummon]?.DisplayName?.Trim();
                if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(first.Summon))
                    name = DatabaseAPI.GetPowerByFullName(first.Summon)?.DisplayName?.Trim() ?? first.Summon.Trim();
                if (string.IsNullOrWhiteSpace(name)) name = "Pet";
            }

            int count = effects.Count;

            bool isVariable = effects.Any(e =>
            {
                var pow = e.GetPower();
                return pow != null && pow.VariableEnabled && (e.VariableModified || e.ToWho == Enums.eToWho.Self) && !e.IgnoreScaling;
            });

            var header = new System.Text.StringBuilder();
            header.Append("Summons ").Append(name);
            if (count > 1) header.Append(" x").Append(count);
            if (isVariable) header.Append(" (Variable)");

            static string Ordinal(int n)
            {
                int m = n % 100;
                if (m is 11 or 12 or 13) return n + "th";
                return (n % 10) switch { 1 => n + "st", 2 => n + "nd", 3 => n + "rd", _ => n + "th" };
            }
            static string When(IEffect e)
            {
                if (e.DelayedTime <= 0) return "immediately";
                var s = e.DelayedTime.ToString("0.#");
                return $"after {s} second" + (Math.Abs(e.DelayedTime - 1.0) < 1e-9 ? "" : "s");
            }

            bool allImmediate = effects.All(e => e.DelayedTime <= 0);
            var body = new System.Text.StringBuilder();

            if (!(count > 1 && allImmediate))
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    var when = When(effects[i]);
                    if (count > 1)
                        body.Append("• ").Append(Ordinal(i + 1)).Append(' ').Append(name).Append(' ').Append(when).AppendLine();
                    else
                        body.Append("• 1 ").Append(name).Append(' ').Append(when).AppendLine();
                }
            }

            bool ignoresBuffs = effects.Any(e => !e.Buffable);
            bool ignoresED = effects.Any(e => e.IgnoreED);
            bool noStackSameCaster = effects.Any(e => e.Stacking == Enums.eStacking.No);

            var flags = new System.Text.StringBuilder();
            if (ignoresBuffs) flags.AppendLine("[Ignores Enhancements & Buffs]");
            if (ignoresED) flags.AppendLine("[Not Affected By ED]");
            if (noStackSameCaster) flags.AppendLine("[Does Not Stack From Same Caster]");

            var tip = new System.Text.StringBuilder();
            tip.AppendLine(header.ToString());
            tip.Append(body);
            if (flags.Length > 0) tip.Append(flags);

            return tip.ToString().TrimEnd();
        }

        // ===== Magnitude =====

        public EnhancedMagSum GetMagSum(IPower pBase, IPower pEnh)
        {
            static float Sum(IReadOnlyList<float> vals)
            {
                if (vals.Count == 0) return 0f;
                bool allNeg = vals.All(v => v < 0);
                return allNeg ? vals.Sum() : vals.Where(v => v > 0).Sum();
            }

            var baseVals = IncludedEffects
                .Where(e => e >= 0 && e < pBase.Effects.Length)
                .Select(e => pBase.Effects[e].BuffedMag)
                .ToList();

            var enhVals = IncludedEffects
                .Where(e => e >= 0 && e < pEnh.Effects.Length)
                .Select(e => pEnh.Effects[e].BuffedMag)
                .ToList();

            return new EnhancedMagSum
            {
                Base = Sum(baseVals),
                Enhanced = Sum(enhVals)
            };
        }

        public float GetMagSum(IPower power, bool ignoreNegs = true)
        {
            if (power?.Effects == null || IncludedEffects.Count <= 0)
            {
                return 0;
            }

            var mags = IncludedEffects
                .Where(e => e >= 0 && e < power.Effects.Length)
                .Select(e => power.Effects[e].BuffedMag)
                .ToList();

            if (mags.Count == 0)
            {
                return 0;
            }

            var firstInBounds = IncludedEffects.FirstOrDefault(e => e >= 0 && e < power.Effects.Length, -1);
            if (firstInBounds >= 0)
            {
                var eff = power.Effects[firstInBounds];
                if (eff.EffectType is Enums.eEffectType.Defense
                    or Enums.eEffectType.Resistance
                    or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist
                    or Enums.eEffectType.ResEffect
                    or Enums.eEffectType.Enhancement)
                {
                    return eff.BuffedMag;
                }
            }

            var allNeg = mags.All(v => v < 0);
            if (HasMisc)
            {
                return allNeg ? mags.Min() : mags.Max();
            }
            
            if (allNeg)
            {
                return mags.Sum();
            }

            return ignoreNegs
                ? mags.Where(v => v > 0).Sum()
                : mags.Sum();
        }

        // ===== Grouping / assembly (unchanged) =====

        public static List<int> GetSimilarEffects(IPower power, FxId fxIdentifier, float mag,
            Enums.eSpecialCase specialCase = Enums.eSpecialCase.None, bool enhancementEffect = false)
        {
            return fxIdentifier.EffectType switch
            {
                Enums.eEffectType.EntCreate => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                e.Value.nSummon == fxIdentifier.SummonId && e.Value.ToWho == fxIdentifier.ToWho &&
                                (Math.Abs(e.Value.Duration - fxIdentifier.Duration) < Tolerance || fxIdentifier.Duration == 0) &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                    or Enums.eEffectType.SpeedRunning => power.Effects
                        .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                        .Where(e =>
                            e.Value.EffectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                                or Enums.eEffectType.SpeedRunning && e.Value.ToWho == fxIdentifier.ToWho &&
                            Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                            e.Value.isEnhancementEffect == enhancementEffect &&
                            e.Value.PvMode == fxIdentifier.PvMode &&
                            e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                        .Select(e => e.Key)
                        .ToList(),

                Enums.eEffectType.Enhancement when fxIdentifier.ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                e.Value.ETModifies == fxIdentifier.ETModifies && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.Enhancement => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                e.Value.ETModifies is not Enums.eEffectType.Mez and not Enums.eEffectType.MezResist &&
                                e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.MezResist or Enums.eEffectType.Defense or Enums.eEffectType.Resistance
                    or Enums.eEffectType.Elusivity or Enums.eEffectType.ResEffect => power.Effects
                        .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                        .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                    e.Value.ToWho == fxIdentifier.ToWho &&
                                    Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                    e.Value.isEnhancementEffect == enhancementEffect &&
                                    e.Value.PvMode == fxIdentifier.PvMode &&
                                    e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                        .Select(e => e.Key)
                        .ToList(),

                Enums.eEffectType.DamageBuff when specialCase == Enums.eSpecialCase.Defiance => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                e.Value.SpecialCase == specialCase && e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.DamageBuff => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                e.Value.SpecialCase != Enums.eSpecialCase.Defiance &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.Damage => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < Tolerance &&
                                e.Value.DamageType == fxIdentifier.DamageType &&
                                e.Value.SpecialCase == specialCase &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                _ => []
            };
        }

        public static List<GroupedFx> AssembleGroupedEffects(IPower? power, bool includeDamage = false)
        {
            if (power == null)
            {
                return [];
            }

            var rankedEffects = power.GetRankedEffects(true);
            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var ignoredEffects = new List<int>();
            var groupedRankedEffects = new List<GroupedFx>();

            foreach (var re in rankedEffects)
            {
                if (re <= -1)
                {
                    continue;
                }

                if (ignoredEffects.Contains(re))
                {
                    continue;
                }

                if (!includeDamage && power.Effects[re].EffectType == Enums.eEffectType.Damage)
                {
                    continue;
                }

                if (power.Effects[re].EffectType is Enums.eEffectType.Meter or Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode
                    or Enums.eEffectType.Null or Enums.eEffectType.NullBool or Enums.eEffectType.GlobalChanceMod
                    or Enums.eEffectType.ExecutePower)
                {
                    continue;
                }

                if (power.Effects[re].EffectType == Enums.eEffectType.ResEffect &&
                    power.Effects[re].ETModifies is Enums.eEffectType.Null or Enums.eEffectType.NullBool)
                {
                    continue;
                }

                if (!(power.Effects[re].Probability > 0 &&
                      (MidsContext.Config?.Suppression & power.Effects[re].Suppression) ==
                      Enums.eSuppress.None & power.Effects[re].CanInclude()))
                {
                    continue;
                }

                if (power.Effects[re].EffectType == Enums.eEffectType.RevokePower &&
                    power.Effects[re].nSummon <= -1 &&
                    string.IsNullOrWhiteSpace(power.Effects[re].Summon))
                {
                    continue;
                }

                if (power.Effects[re].EffectType == Enums.eEffectType.GrantPower &&
                    power.Effects[re].nSummon <= -1)
                {
                    continue;
                }

                if (power.Effects[re].PvMode == Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE |
                    power.Effects[re].PvMode == Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE)
                {
                    continue;
                }

                if (power.Effects[re].ActiveConditionals is { Count: > 0 })
                {
                    if (!power.Effects[re].ValidateConditional())
                    {
                        continue;
                    }
                }

                if (power.Effects[re].EffectType == Enums.eEffectType.Mez & power.Effects[re].MezType is not (Enums.eMez.Teleport or Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Repel or Enums.eMez.ToggleDrop))
                {
                    if (power.Effects[re].Duration <= 0)
                    {
                        continue;
                    }
                }

                var similarFxIds = new List<int>();

                switch (power.Effects[re].EffectType)
                {
                    case Enums.eEffectType.Damage:
                        similarFxIds = GetSimilarEffects(power,
                            new FxId
                            {
                                DamageType = power.Effects[re].DamageType,
                                EffectType = Enums.eEffectType.Damage,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = power.Effects[re].Duration,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            }, power.Effects[re].BuffedMag,
                            power.Effects[re].SpecialCase,
                            power.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = power.Effects[re].DamageType,
                                EffectType = Enums.eEffectType.Damage,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = power.Effects[re].Duration,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                                power.Effects[re].BuffedMag,
                                "Damage",
                                similarFxIds,
                                power.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.EntCreate:
                        similarFxIds = GetSimilarEffects(power,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.EntCreate,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = power.Effects[re].nSummon,
                                Duration = 0, //power.Effects[re].Duration
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            }, power.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            power.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.EntCreate,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = power.Effects[re].nSummon,
                                Duration = 0, //power.Effects[re].Duration
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                                power.Effects[re].BuffedMag,
                                "Summon",
                                similarFxIds,
                                power.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.SpeedRunning:
                        similarFxIds = GetSimilarEffects(power,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = power.Effects[re].EffectType,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            }, power.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            power.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = power.Effects[re].EffectType,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                                power.Effects[re].BuffedMag,
                                "Slow",
                                similarFxIds,
                                power.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.DamageBuff:
                        var isDefiance = power.Effects[re].SpecialCase == Enums.eSpecialCase.Defiance &&
                                         power.Effects[re].ValidateConditional("Active", "Defiance") |
                                         MidsContext.Character.CurrentBuild.PowerActive(defiancePower);

                        similarFxIds = GetSimilarEffects(power,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.DamageBuff,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            }, power.Effects[re].BuffedMag,
                            isDefiance ? Enums.eSpecialCase.Defiance : Enums.eSpecialCase.None,
                            power.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.DamageBuff,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                                power.Effects[re].BuffedMag,
                                isDefiance ? "Defiance" : $"{power.Effects[re].EffectType}",
                                similarFxIds,
                                !isDefiance && power.Effects[re].isEnhancementEffect,
                                Enums.eSpecialCase.Defiance));

                        break;

                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Elusivity:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                        similarFxIds = GetSimilarEffects(power,
                            new FxId
                            {
                                EffectType = power.Effects[re].EffectType,
                                ETModifies = power.Effects[re].ETModifies,
                                MezType = Enums.eMez.None,
                                DamageType = Enums.eDamage.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                            power.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            power.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                EffectType = power.Effects[re].EffectType,
                                ETModifies = power.Effects[re].ETModifies,
                                MezType = Enums.eMez.None,
                                DamageType = Enums.eDamage.None,
                                ToWho = power.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = power.Effects[re].PvMode,
                                IgnoreScaling = power.Effects[re].IgnoreScaling
                            },
                                power.Effects[re].BuffedMag,
                                power.Effects[re].EffectType == Enums.eEffectType.Enhancement
                                    ? $"{power.Effects[re].EffectType}({power.Effects[re].ETModifies})"
                                    : $"{power.Effects[re].EffectType}",
                                similarFxIds,
                                power.Effects[re].isEnhancementEffect
                            ));

                        break;

                    default:
                        groupedRankedEffects.Add(new GroupedFx(power.Effects[re], re));
                        break;
                }
            }

            var groupedRankedEffects2 = new List<GroupedFx>();
            var ignoredGroups = new List<int>();
            for (var i = 0; i < groupedRankedEffects.Count; i++)
            {
                if (ignoredGroups.Contains(i))
                {
                    continue;
                }

                if (groupedRankedEffects[i].NumEffects > 1)
                {
                    groupedRankedEffects2.Add(groupedRankedEffects[i]);
                    continue;
                }

                var similarGreList = groupedRankedEffects
                    .Select((e, id) => new KeyValuePair<int, GroupedFx>(id, e))
                    .Where(e => e.Value.FxIdentifier.Equals(groupedRankedEffects[i].FxIdentifier) &&
                                Math.Abs(e.Value.Mag - groupedRankedEffects[i].Mag) < Tolerance &&
                                e.Value.EnhancementEffect == groupedRankedEffects[i].EnhancementEffect &&
                                e.Value.SpecialCase == groupedRankedEffects[i].SpecialCase)
                    .ToList();

                ignoredGroups.AddRangeUnique(similarGreList.Select(e => e.Key).ToList());

                groupedRankedEffects2.Add(new GroupedFx(groupedRankedEffects[i].FxIdentifier, similarGreList.Select(e => e.Value).ToList()));
            }

            var greAggregated = Aggregate(groupedRankedEffects2);
            foreach (var gre in greAggregated)
            {
                if (!gre.IsAggregated)
                {
                    continue;
                }

                gre.Mag = gre.GetMagSum(power, gre.FxIdentifier.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));
            }

            return greAggregated
                .Where(e => Math.Abs(e.Mag) > Tolerance)
                .ToList();
        }

        public static List<GroupedFx> AssembleGroupedEffects(IPower? power, IEnumerable<int> includedEffects, bool includeDamage = false)
        {
            if (power == null)
            {
                return [];
            }

            var pw = power.Clone();
            pw.Effects = pw.Effects.Where((e, i) => includedEffects.Contains(i)).ToArray();

            var rankedEffects = pw.GetRankedEffects(true);
            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var ignoredEffects = new List<int>();
            var groupedRankedEffects = new List<GroupedFx>();

            foreach (var re in rankedEffects)
            {
                if (re <= -1)
                {
                    continue;
                }

                if (ignoredEffects.Contains(re))
                {
                    continue;
                }

                if (!includeDamage && pw.Effects[re].EffectType == Enums.eEffectType.Damage)
                {
                    continue;
                }

                if (pw.Effects[re].EffectType is Enums.eEffectType.Meter or Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode
                    or Enums.eEffectType.Null or Enums.eEffectType.NullBool or Enums.eEffectType.GlobalChanceMod
                    or Enums.eEffectType.ExecutePower)
                {
                    continue;
                }

                if (pw.Effects[re].EffectType == Enums.eEffectType.ResEffect &&
                    pw.Effects[re].ETModifies is Enums.eEffectType.Null or Enums.eEffectType.NullBool)
                {
                    continue;
                }

                if (!(pw.Effects[re].Probability > 0 &&
                      ((MidsContext.Config?.Suppression & pw.Effects[re].Suppression) == Enums.eSuppress.None) &
                      pw.Effects[re].CanInclude()))
                {
                    continue;
                }

                if (pw.Effects[re].EffectType == Enums.eEffectType.RevokePower &&
                    pw.Effects[re].nSummon <= -1 &&
                    string.IsNullOrWhiteSpace(pw.Effects[re].Summon))
                {
                    continue;
                }

                if (pw.Effects[re].EffectType == Enums.eEffectType.GrantPower &&
                    pw.Effects[re].nSummon <= -1)
                {
                    continue;
                }

                if (pw.Effects[re].PvMode == Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE |
                    (pw.Effects[re].PvMode == Enums.ePvX.PvE) && MidsContext.Config.Inc.DisablePvE)
                {
                    continue;
                }

                if (pw.Effects[re].ActiveConditionals is { Count: > 0 })
                {
                    if (!pw.Effects[re].ValidateConditional())
                    {
                        continue;
                    }
                }

                if ((pw.Effects[re].EffectType == Enums.eEffectType.Mez) & pw.Effects[re].MezType is not (Enums.eMez.Teleport or Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Repel or Enums.eMez.ToggleDrop))
                {
                    if (pw.Effects[re].Duration <= 0)
                    {
                        continue;
                    }
                }

                var similarFxIds = new List<int>();

                switch (pw.Effects[re].EffectType)
                {
                    case Enums.eEffectType.Damage:
                        similarFxIds = GetSimilarEffects(pw,
                            new FxId
                            {
                                DamageType = pw.Effects[re].DamageType,
                                EffectType = Enums.eEffectType.Damage,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = pw.Effects[re].Duration,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            }, pw.Effects[re].BuffedMag,
                            pw.Effects[re].SpecialCase,
                            pw.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = pw.Effects[re].DamageType,
                                EffectType = Enums.eEffectType.Damage,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = pw.Effects[re].Duration,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                                pw.Effects[re].BuffedMag,
                                "Damage",
                                similarFxIds,
                                pw.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.EntCreate:
                        similarFxIds = GetSimilarEffects(pw,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.EntCreate,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = pw.Effects[re].nSummon,
                                Duration = 0, //power.Effects[re].Duration
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            }, pw.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            pw.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.EntCreate,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = pw.Effects[re].nSummon,
                                Duration = 0, //power.Effects[re].Duration
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                                pw.Effects[re].BuffedMag,
                                "Summon",
                                similarFxIds,
                                pw.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.SpeedRunning:
                        similarFxIds = GetSimilarEffects(pw,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = pw.Effects[re].EffectType,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            }, pw.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            pw.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = pw.Effects[re].EffectType,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                                pw.Effects[re].BuffedMag,
                                "Slow",
                                similarFxIds,
                                pw.Effects[re].isEnhancementEffect));

                        break;

                    case Enums.eEffectType.DamageBuff:
                        var isDefiance = pw.Effects[re].SpecialCase == Enums.eSpecialCase.Defiance &&
                                         pw.Effects[re].ValidateConditional("Active", "Defiance") |
                                         MidsContext.Character.CurrentBuild.PowerActive(defiancePower);

                        similarFxIds = GetSimilarEffects(pw,
                            new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.DamageBuff,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            }, pw.Effects[re].BuffedMag,
                            isDefiance ? Enums.eSpecialCase.Defiance : Enums.eSpecialCase.None,
                            pw.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.DamageBuff,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                                pw.Effects[re].BuffedMag,
                                isDefiance ? "Defiance" : $"{pw.Effects[re].EffectType}",
                                similarFxIds,
                                !isDefiance && pw.Effects[re].isEnhancementEffect,
                                Enums.eSpecialCase.Defiance));

                        break;

                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Elusivity:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                        similarFxIds = GetSimilarEffects(pw,
                            new FxId
                            {
                                EffectType = pw.Effects[re].EffectType,
                                ETModifies = pw.Effects[re].ETModifies,
                                MezType = Enums.eMez.None,
                                DamageType = Enums.eDamage.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                            pw.Effects[re].BuffedMag,
                            Enums.eSpecialCase.None,
                            pw.Effects[re].isEnhancementEffect);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new FxId
                            {
                                EffectType = pw.Effects[re].EffectType,
                                ETModifies = pw.Effects[re].ETModifies,
                                MezType = Enums.eMez.None,
                                DamageType = Enums.eDamage.None,
                                ToWho = pw.Effects[re].ToWho,
                                SummonId = -1,
                                Duration = 0,
                                PvMode = pw.Effects[re].PvMode,
                                IgnoreScaling = pw.Effects[re].IgnoreScaling
                            },
                                pw.Effects[re].BuffedMag,
                                pw.Effects[re].EffectType == Enums.eEffectType.Enhancement
                                    ? $"{pw.Effects[re].EffectType}({pw.Effects[re].ETModifies})"
                                    : $"{pw.Effects[re].EffectType}",
                                similarFxIds,
                                pw.Effects[re].isEnhancementEffect
                            ));

                        break;

                    default:
                        groupedRankedEffects.Add(new GroupedFx(pw.Effects[re], re));
                        break;
                }
            }

            var groupedRankedEffects2 = new List<GroupedFx>();
            var ignoredGroups = new List<int>();
            for (var i = 0; i < groupedRankedEffects.Count; i++)
            {
                if (ignoredGroups.Contains(i))
                {
                    continue;
                }

                if (groupedRankedEffects[i].NumEffects > 1)
                {
                    groupedRankedEffects2.Add(groupedRankedEffects[i]);
                    continue;
                }

                var similarGreList = groupedRankedEffects
                    .Select((e, id) => new KeyValuePair<int, GroupedFx>(id, e))
                    .Where(e => e.Value.FxIdentifier.Equals(groupedRankedEffects[i].FxIdentifier) &&
                                Math.Abs(e.Value.Mag - groupedRankedEffects[i].Mag) < Tolerance &&
                                e.Value.EnhancementEffect == groupedRankedEffects[i].EnhancementEffect &&
                                e.Value.SpecialCase == groupedRankedEffects[i].SpecialCase)
                    .ToList();

                ignoredGroups.AddRangeUnique(similarGreList.Select(e => e.Key).ToList());

                groupedRankedEffects2.Add(new GroupedFx(groupedRankedEffects[i].FxIdentifier, similarGreList.Select(e => e.Value).ToList()));
            }

            var greAggregated = Aggregate(groupedRankedEffects2);
            foreach (var gre in greAggregated)
            {
                if (!gre.IsAggregated)
                {
                    continue;
                }

                gre.Mag = gre.GetMagSum(pw, gre.FxIdentifier.EffectType is not (Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight));
            }

            return greAggregated
                .Where(e => Math.Abs(e.Mag) > Tolerance)
                .ToList();
        }

        public static List<GroupedFx> AggregateGroupedEffectsPass2(IPower? power, List<GroupedFx> gre)
        {
            if (power == null)
            {
                return [];
            }

            var ret = new Dictionary<EnhanceableFxId, GroupedFx>();

            foreach (var g in gre)
            {
                var fxId = new EnhanceableFxId
                {
                    EffectType = g.EffectType,
                    ETModifies = g.ETModifies == Enums.eEffectType.None ? null : g.ETModifies,
                    MezType = g.MezType == Enums.eMez.None ? null : g.MezType,
                    PvMode = g.PvMode,
                    ToWho = g.ToWho
                };

                var v = ret.TryAdd(fxId, new GroupedFx
                {
                    FxIdentifier = new FxId
                    {
                        DamageType = Enums.eDamage.None,
                        Duration = 0,
                        EffectType = g.EffectType,
                        ETModifies = g.ETModifies,
                        IgnoreScaling = g.IgnoreScaling,
                        MezType = g.MezType,
                        PvMode = g.PvMode,
                        SummonId = 0,
                        ToWho = g.ToWho
                    },
                    Alias = $"{(g.EffectType == Enums.eEffectType.Enhancement ? $"Enhancement({(g.ETModifies == Enums.eEffectType.Mez ? g.MezType : g.ETModifies)})" : g.EffectType)}",
                    HasMisc = true,
                    IncludedEffects = g.IncludedEffects.Clone(),
                    IsAggregated = true,
                    IsEnhancement = g.IsEnhancement,
                    Mag = 0,
                    SingleEffectSource = null,
                    SpecialCase = Enums.eSpecialCase.None
                });

                if (!v)
                {
                    ret[fxId].IncludedEffects = ret[fxId].IncludedEffects.Concat(g.IncludedEffects).Distinct().ToList();
                }
            }

            var gList = ret.Values.ToList();
            foreach (var g in gList)
            {
                // Sort descending by mag
                g.IncludedEffects.Sort((a, b) =>
                {
                    var fx1 = power.Effects[a];
                    var fx2 = power.Effects[b];

                    return fx1.BuffedMag < fx2.BuffedMag ? 1 : -1;
                });
            }

            return gList;
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> GenerateListItems(List<GroupedFx> groupedRankedEffects, IPower pBase, IPower pEnh, List<int> rankedEffects, float displayBlockFontSize)
        {
            var ret = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>();
            var powerInBuild = MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.Database.Power.TryFindIndex(e => e?.FullName == pBase.FullName)) > -1;

            foreach (var gre in groupedRankedEffects)
            {
                var greIndex = gre.GetRankedEffectIndex(rankedEffects, 0);
                if (greIndex < 0) continue;

                var rankedEffect = FastItemBuilder.GetRankedEffect(rankedEffects.ToArray(), greIndex, pBase, pEnh);
                FinalizeListItem(ref rankedEffect, pBase, pEnh, gre, rankedEffects[greIndex], powerInBuild, displayBlockFontSize);

                ret.Add(new KeyValuePair<GroupedFx, PairedListEx.Item>(gre, rankedEffect));
            }

            return ret;
        }

        public static List<KeyValuePair<GroupedFx, EffectListItem>> GenerateEffectItems(List<GroupedFx> groupedRankedEffects, IPower pBase, IPower pEnh, List<int> rankedEffects)
        {
            var ret = new List<KeyValuePair<GroupedFx, EffectListItem>>();
            var powerInBuild = MidsContext.Character.CurrentBuild
                .FindInToonHistory(DatabaseAPI.Database.Power.TryFindIndex(e => e?.FullName == pBase.FullName)) > -1;

            foreach (var gre in groupedRankedEffects)
            {
                var greIndex = gre.GetRankedEffectIndex(rankedEffects, 0);
                if (greIndex < 0) continue;

                var label = BuildLabel(gre, pEnh);
                var (value, alt, tip, isSpecial, isConditional, isUnique) =
                    BuildValueAndDecorations(gre, pBase, pEnh, rankedEffects[greIndex], powerInBuild);

                var item = new EffectListItem(
                    label,
                    value,
                    alt,
                    tip,
                    isSpecial,
                    isConditional,
                    isUnique,
                    gre.EnhancementEffect);

                ret.Add(new KeyValuePair<GroupedFx, EffectListItem>(gre, item));
            }

            return ret;
        }

        public static List<KeyValuePair<GroupedFx, EffectListItem>> FilterEffectItemsExt(List<KeyValuePair<GroupedFx, EffectListItem>>? itemsDict, Func<FxId, bool> filter)
        {
            if (itemsDict == null) return new List<KeyValuePair<GroupedFx, EffectListItem>>();
            return itemsDict.Where(e => filter(e.Key.FxIdentifier)).ToList();
        }

        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict, Func<FxId, bool> filterFunc)
        {
            if (itemsDict == null)
            {
                return [];
            }

            return itemsDict
                .Where(e => filterFunc(e.Key.FxIdentifier))
                .Select(e => e.Value)
                .ToList();
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> FilterListItemsExt(List<KeyValuePair<GroupedFx, PairedListEx.Item>>? itemsDict, Func<FxId, bool> filterFunc)
        {
            if (itemsDict == null)
            {
                return new List<KeyValuePair<GroupedFx, PairedListEx.Item>>();
            }

            return itemsDict
                .Where(e => filterFunc(e.Key.FxIdentifier))
                .ToList();
        }

        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>>? itemsDict)
        {
            if (itemsDict == null)
            {
                return [];
            }

            return itemsDict
                .Select(e => e.Value)
                .ToList();
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> FilterListItemsExt(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict)
        {
            return itemsDict ?? [];
        }

        private static string GeneratePowerDescShort(IPower? power)
        {
            var effectShorts = new List<string>();
            var fxIdList = new List<FxId>();
            var effects = (IEffect[])power.Effects.Clone();
            effects = effects.OrderBy(e => e.ToWho)
                .Where(e => e.EffectType is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool
                                or Enums.eEffectType.Meter or Enums.eEffectType.Damage or Enums.eEffectType.MaxFlySpeed
                                or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.MaxRunSpeed
                                or Enums.eEffectType.ExecutePower or Enums.eEffectType.RevokePower
                                or Enums.eEffectType.GlobalChanceMod or Enums.eEffectType.SetMode
                                or Enums.eEffectType.SetCostume) &&
                            e.ETModifies is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool) &&
                            e.ToWho != Enums.eToWho.Unspecified &&
                            Math.Abs(e.BuffedMag) >= Tolerance &&
                            (e.PvMode == Enums.ePvX.Any || (e.PvMode == Enums.ePvX.PvE && !MidsContext.Config.Inc.DisablePvE) || (e.PvMode == Enums.ePvX.PvP && MidsContext.Config.Inc.DisablePvE)) &&
                            (e.ActiveConditionals is { Count: <= 0 } || e.ValidateConditional()))
                .ToArray();

            for (var i = 0; i < effects.Length; i++)
            {
                var fxIdentifier = effects[i].EffectType switch
                {
                    Enums.eEffectType.MezResist or Enums.eEffectType.Mez => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = effects[i].MezType,
                        ETModifies = Enums.eEffectType.None,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    },

                    Enums.eEffectType.ResEffect or Enums.eEffectType.Enhancement => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = Enums.eMez.None,
                        ETModifies = effects[i].ETModifies,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    },

                    _ => new FxId
                    {
                        EffectType = effects[i].EffectType,
                        DamageType = Enums.eDamage.None,
                        MezType = Enums.eMez.None,
                        ETModifies = Enums.eEffectType.None,
                        ToWho = effects[i].ToWho,
                        Duration = 0,
                        SummonId = -1
                    }
                };

                if (fxIdList.Contains(fxIdentifier))
                {
                    continue;
                }

                var toWho = effects.Length == 1 ||
                            (i < effects.Length - 1 && effects[i].ToWho != effects[i + 1].ToWho) ||
                            (i < effects.Length - 1 && effects[i + 1].EffectType is Enums.eEffectType.Mez or Enums.eEffectType.Enhancement) ||
                            (effects.Length > 1 && i == effects.Length - 1)
                    ? effects[i].ToWho switch
                    {
                        Enums.eToWho.Self => " (Self)",
                        Enums.eToWho.Target => " (Target)",
                        Enums.eToWho.All => " (All)",
                        _ => ""
                    }
                    : "";

                var mezType = effects[i].ToWho == Enums.eToWho.Self
                    ? $"{effects[i].MezType}"
                    : effects[i].MezType switch
                    {
                        Enums.eMez.Held => "Hold",
                        Enums.eMez.Stunned => "Stun",
                        Enums.eMez.Confused => "Confuse",
                        Enums.eMez.Immobilized => "Immobilize",
                        Enums.eMez.Terrorized => "Fear",
                        _ => $"{effects[i].MezType}"
                    };

                effectShorts.Add(effects[i].EffectType switch
                {
                    Enums.eEffectType.ResEffect => $"{(effects[i].BuffedMag < 0 ? "-" : "")}{effects[i].EffectType} ({effects[i].ETModifies}){toWho}",
                    Enums.eEffectType.MezResist => $"{effects[i].EffectType} ({effects[i].MezType}){toWho}",
                    Enums.eEffectType.Mez => $"{effects[i].ToWho} {mezType}",
                    Enums.eEffectType.Enhancement => $"{effects[i].ToWho} {(effects[i].BuffedMag > 0 ? "+" : "-")}{effects[i].ETModifies}",
                    _ => $"{(effects[i].BuffedMag < 0 ? "-" : "")}{effects[i].EffectType}{toWho}"
                });

                fxIdList.Add(fxIdentifier);
            }

            return string.Join(", ", effectShorts);
        }

        public static List<GroupedFx> Aggregate(List<GroupedFx> greList)
        {
            var ret = new List<GroupedFx>();
            var excludedGre = new List<int>();

            for (var i = 0; i < greList.Count; i++)
            {
                if (excludedGre.Contains(i))
                {
                    continue;
                }

                excludedGre.Add(i);
                var includedGre = new List<int> { i };
                for (var j = 0; j < greList.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (excludedGre.Contains(j))
                    {
                        continue;
                    }

                    if (greList[i].FxIdentifier.EffectType is Enums.eEffectType.SpeedFlying
                        or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning
                        or Enums.eEffectType.JumpHeight)
                    {
                        if (!greList[i].FxIdentifier.Equals(greList[j].FxIdentifier) |
                            greList[i].EnhancementEffect != greList[j].EnhancementEffect |
                            greList[i].SpecialCase != greList[j].SpecialCase)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!greList[i].FxIdentifier.Equals(greList[j].FxIdentifier) |
                            Math.Abs(greList[i].Mag - greList[j].Mag) > Tolerance |
                            greList[i].EnhancementEffect != greList[j].EnhancementEffect |
                            greList[i].SpecialCase != greList[j].SpecialCase)
                        {
                            continue;
                        }
                    }

                    includedGre.Add(j);
                    excludedGre.Add(j);
                }

                ret.Add(new GroupedFx(greList[i].FxIdentifier, includedGre.Select(e => greList[e]).ToList()));
            }

            return ret;
        }

        private GroupedFx CropIncludedEffects(IPower power)
        {
            var gre = (GroupedFx)Clone();
            gre.IncludedEffects = gre.IncludedEffects
                .Where(e => e >= 0 && e < power.Effects.Length)
                .ToList();

            return gre;
        }

        private static void FinalizeListItem(ref PairedListEx.Item rankedEffect, IPower pBase, IPower pEnh, GroupedFx gre, int effectIndex, bool powerInBuild, float displayBlockFontSize)
        {
            if (pBase.Effects.Any(e => e.EffectType == Enums.eEffectType.EntCreate) & pBase.AbsorbSummonEffects)
            {
                pBase.AbsorbPetEffects();
            }

            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var effectSource = gre.GetEffectAt(pEnh);
            var effectType = gre.EffectType;
            var greTooltip = !gre.HasMisc
                ? gre.GetTooltip(pEnh)
                : string.Join("\r\n", AssembleGroupedEffects(pEnh, gre.IncludedEffects).Select(e => e.GetTooltip(pEnh)));

            var magSum = effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                    or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight
                    ? gre.GetMagSum(pEnh, false)
                    : gre.GetMagSum(pEnh);
            var baseMagSum = effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight
                ? gre.CropIncludedEffects(pBase).GetMagSum(pBase, false)
                : gre.CropIncludedEffects(pBase).GetMagSum(pBase);

            var mezDurationDiff = (effectType == Enums.eEffectType.Mez) & (Math.Abs(
                (effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].Duration : 0) -
                (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].Duration : 0)) > Tolerance);

            var magDiff = false;
            var buffedMagDiff = false;
            if (pEnh.Effects[effectIndex].Buffable)
            {
                magDiff = (Math.Abs((effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].BuffedMag : 0) -
                                    (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].BuffedMag : 0)) > Tolerance) |
                         (Math.Abs(magSum - baseMagSum) > Tolerance) |
                         mezDurationDiff;
                buffedMagDiff = effectIndex < pEnh.Effects.Length &&
                                Math.Abs(pEnh.Effects[effectIndex].BuffedMag - pEnh.Effects[effectIndex].Mag) >
                                Tolerance;
            }
            else
            {
                var indexedFx = pEnh.Effects.Select((f, i) => new KeyValuePair<int, IEffect>(i, f)).ToList();
                var fxSourceAlt = gre.IncludedEffects
                    .Select(e => indexedFx[e])
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault(e => e.Value.Buffable & (e.Key < pBase.Effects.Length))
                    .Key;

                if (fxSourceAlt >= 0)
                {
                    magDiff = (Math.Abs((fxSourceAlt < pBase.Effects.Length ? pBase.Effects[fxSourceAlt].BuffedMag : 0) -
                                        (fxSourceAlt < pEnh.Effects.Length ? pEnh.Effects[fxSourceAlt].BuffedMag : 0)) > Tolerance) |
                              (Math.Abs(magSum - baseMagSum) > Tolerance) |
                              mezDurationDiff;
                    buffedMagDiff = fxSourceAlt < pEnh.Effects.Length &&
                                    Math.Abs(pEnh.Effects[fxSourceAlt].BuffedMag - pEnh.Effects[fxSourceAlt].Mag) >
                                    Tolerance;
                }
            }

            var toWhoShort = effectSource.ToWho switch
            {
                Enums.eToWho.Self => " (Self)",
                Enums.eToWho.Target => " (Target)",
                Enums.eToWho.All => " (All)",
                _ => ""
            };

            rankedEffect.UseUniqueColor = effectSource.isEnhancementEffect;
            rankedEffect.UseAlternateColor = !effectSource.isEnhancementEffect &&
                                          magDiff &&
                                          buffedMagDiff | mezDurationDiff &&
                                          gre.IncludedEffects.Select(e => pEnh.Effects[e].Buffable).Any(e => e) &
                                          powerInBuild;

            if (gre.IsAggregated && effectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
                    or Enums.eEffectType.SpeedRunning or Enums.eEffectType.JumpHeight)
            {
                rankedEffect.Value = effectSource.DisplayPercentage
                    ? $"{magSum * 100:###0.##}%{toWhoShort}"
                    : $"{magSum:###0.##}{toWhoShort}";
            }

            switch (effectType)
            {
                case Enums.eEffectType.Fly:
                case Enums.eEffectType.MovementControl:
                case Enums.eEffectType.MovementFriction:
                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{magSum * 100:###0.##}%{toWhoShort}"
                        : $"{magSum:###0.##}{toWhoShort}";

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Recovery:
                case Enums.eEffectType.Endurance:
                case Enums.eEffectType.Regeneration:
                    rankedEffect.Name = $"{effectType}";
                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{magSum * 100:###0.##}%{toWhoShort}"
                        : $"{magSum:###0.##}{toWhoShort}";

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.SilentKill when effectSource.ToWho == Enums.eToWho.Self:
                    rankedEffect.Name = "Lifespan";
                    rankedEffect.Value = $"{Math.Max(effectSource.Duration, Math.Max(effectSource.DelayedTime, effectSource.Absorbed_Duration)):####0.##} s";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.EntCreate:
                    rankedEffect.Name = "Summon";
                    rankedEffect.Value = effectSource.nSummon > -1
                        ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                        : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                    if (gre.IncludedEffects.Count > 1)
                    {
                        rankedEffect.Value += $" x{gre.IncludedEffects.Count}";
                    }

                    if (effectSource.nSummon > -1)
                    {
                        var entityTooltip = string.Join("\r\n", gre.IncludedEffects
                            .Select(e => pEnh.Effects[e])
                            .OrderBy(e => e.DelayedTime)
                            .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)));

                        var entityPowersets = DatabaseAPI.Database.Entities[effectSource.nSummon].GetNPowerset();
                        if (entityPowersets.Count > 0 && entityPowersets[0] > -1)
                        {
                            var entityPowerset = DatabaseAPI.Database.Powersets[entityPowersets[0]];
                            entityTooltip += "\r\n\r\nEntity has the following Powers:";
                            foreach (var p in entityPowerset.Power)
                            {
                                var epShortDesc = GeneratePowerDescShort(DatabaseAPI.Database.Power[p]);
                                entityTooltip += $"\r\n- {DatabaseAPI.Database.Power[p].DisplayName}";
                                if (!string.IsNullOrEmpty(epShortDesc))
                                {
                                    entityTooltip +=
                                        (string.IsNullOrWhiteSpace(DatabaseAPI.Database.Power[p].DescShort) ||
                                         DatabaseAPI.Database.Power[p].DescShort.Equals(
                                             DatabaseAPI.Database.Power[p].DisplayName,
                                             StringComparison.InvariantCultureIgnoreCase)) &&
                                        !string.IsNullOrEmpty(epShortDesc)
                                            ? $" ({epShortDesc})"
                                            : $" ({DatabaseAPI.Database.Power[p].DescShort})";
                                }
                            }

                            entityTooltip += "\r\n\r\nTo see the effects of these Powers, Left-Click on the Entity.";
                        }

                        rankedEffect.EntTag = DatabaseAPI.Database.Entities[effectSource.nSummon];
                        rankedEffect.ToolTip = entityTooltip;
                    }
                    else
                    {
                        rankedEffect.ToolTip = greTooltip;
                    }

                    break;

                case Enums.eEffectType.GrantPower:
                    rankedEffect.Name = "Grant";
                    if (effectSource.nSummon > -1)
                    {
                        rankedEffect.Value = DatabaseAPI.Database.Power[effectSource.nSummon].DisplayName;
                        var mainEffectTip =
                            effectSource.BuildEffectString(false, "", false, false, false, false, false, true);
                        var subEffectsTip = string.Join("\r\n",
                            DatabaseAPI.Database.Power[effectSource.nSummon].Effects
                                .Where(e => (e.PvMode == Enums.ePvX.Any ||
                                             (e.PvMode == Enums.ePvX.PvE && !MidsContext.Config.Inc.DisablePvE) ||
                                             (e.PvMode == Enums.ePvX.PvP && MidsContext.Config.Inc.DisablePvE)) &
                                            (e.ActiveConditionals.Count <= 0 || e.ValidateConditional()))
                                .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)
                                    .Replace("\r\n", "\n").Replace("\n", " -- ").Replace("  ", " ")));
                        rankedEffect.ToolTip = $"{mainEffectTip}\r\n----------\r\n{subEffectsTip}";
                    }

                    break;

                case Enums.eEffectType.LevelShift:
                    rankedEffect.Name = "LvlShift";
                    rankedEffect.Value = $"{(effectSource.Mag > 0 ? "+" : "")}{effectSource.Mag:##0.##}";

                    break;

                case Enums.eEffectType.RevokePower:
                    rankedEffect.Name = "Revoke";
                    rankedEffect.Value = effectSource.nSummon > -1
                        ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                        : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                    break;

                case Enums.eEffectType.DamageBuff:
                    var isDefiance = effectSource.SpecialCase == Enums.eSpecialCase.Defiance &&
                                     effectSource.ValidateConditional("Active", "Defiance") |
                                     MidsContext.Character.CurrentBuild.PowerActive(defiancePower);
                    rankedEffect.Name = isDefiance
                        ? "Defiance"
                        : FastItemBuilder.Str.ShortStr(displayBlockFontSize, Enums.GetEffectName(effectSource.EffectType),
                            Enums.GetEffectNameShort(effectSource.EffectType));
                    rankedEffect.Value = $"{effectSource.BuffedMag * 100:###0.##}%";
                    rankedEffect.ToolTip = isDefiance
                        ? effectSource.BuildEffectString(false, "DamageBuff (Defiance)", false, false, false, true)
                        : greTooltip;

                    break;

                case Enums.eEffectType.Mez:
                    if (gre.NumEffects == 1)
                    {
                        rankedEffect.Name = effectSource.MezType switch
                        {
                            Enums.eMez.Teleport => "TP",
                            Enums.eMez.Knockback => "KB",
                            Enums.eMez.Knockup => "KUp",
                            _ => $"{effectSource.MezType}"
                        };
                    }

                    rankedEffect.Value = effectSource.ToWho switch
                    {
                        Enums.eToWho.Target => effectSource.MezType is Enums.eMez.Knockback or Enums.eMez.Knockup or Enums.eMez.Teleport
                            ? $"{effectSource.BuffedMag:###0.##}{toWhoShort}"
                            : $"{effectSource.Duration:###0.##}s (Mag {effectSource.BuffedMag:###0.##}){toWhoShort}",

                        Enums.eToWho.Self => $"{effectSource.BuffedMag:###0.##}{toWhoShort}",

                        Enums.eToWho.All => $"{effectSource.Duration:###0.##}s (Mag {effectSource.BuffedMag:###0.##}){toWhoShort}",

                        _ => rankedEffect.Value
                    };

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Translucency:
                    rankedEffect.Name = "Trnslcncy";
                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{effectSource.BuffedMag * 100:###0.##}%{toWhoShort}"
                        : $"{effectSource.BuffedMag:###0.##}{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.SpeedFlying:
                case Enums.eEffectType.SpeedJumping:
                case Enums.eEffectType.SpeedRunning:
                    if (gre.GetStatName(pEnh) == "Slow")
                    {
                        rankedEffect.Name = "Slow";
                        rankedEffect.Value = InvertStringValue(rankedEffect.Value);
                    }
                    else if ((gre.IncludedEffects.Count > 1) & gre.IncludedEffects.Select(e => pEnh.Effects[e].EffectType).Any(e => e != pEnh.Effects[gre.IncludedEffects[0]].EffectType))
                    {
                        rankedEffect.Name = $"{((gre.IsAggregated ? magSum : effectSource.Mag) < 0 ? "-" : "")}Movement";
                    }

                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.MezResist:
                case Enums.eEffectType.Enhancement:
                case Enums.eEffectType.ResEffect:
                    rankedEffect.Name = effectType == Enums.eEffectType.Enhancement
                        ? gre.IncludedEffects.Count > 1
                            ? effectSource.Mag < 0
                                ? "Debuff"
                                : "Enhancement"
                            : $"{(effectSource.Mag < 0 ? "-" : "+")}{effectSource.ETModifies}"
                        : FastItemBuilder.Str.ShortStr(displayBlockFontSize,
                            Enums.GetEffectName(effectSource.EffectType),
                            Enums.GetEffectNameShort(effectSource.EffectType));

                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{effectSource.BuffedMag * 100:###0.##}%{toWhoShort}"
                        : $"{effectSource.BuffedMag:###0.##}{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.PerceptionRadius:
                    rankedEffect.Name = $"Pceptn{toWhoShort}";
                    rankedEffect.Value = $"{(effectSource.DisplayPercentage ? $"{magSum * 100:###0.##}%" : $"{magSum:###0.##}")} ({Statistics.BasePerception * magSum:###0.##}ft)";

                    break;

                case Enums.eEffectType.ToHit:
                    rankedEffect.Name = "ToHit";
                    rankedEffect.Value = $"{gre.Mag * 100:###0.##}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.RechargeTime:
                    rankedEffect.Value = $"{gre.Mag * 100:###0.##}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.Heal:
                    rankedEffect.Name = $"Heal{toWhoShort}";
                    rankedEffect.Value = effectSource.DisplayPercentage & (effectSource.DisplayPercentageOverride == Enums.eOverrideBoolean.TrueOverride)
                        ? $"{gre.Mag * 100:####0.##}% HP"
                        : $"{gre.Mag:####0.##} HP ({gre.Mag / MidsContext.Character.DisplayStats.HealthHitpointsNumeric(false) * 100:###0.##}%)";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                case Enums.eEffectType.MaxRunSpeed:
                case Enums.eEffectType.MaxJumpSpeed:
                case Enums.eEffectType.MaxFlySpeed:
                case Enums.eEffectType.EnduranceDiscount:
                case Enums.eEffectType.ThreatLevel:
                    rankedEffect.Value = $"{gre.Mag * 100:###0.##}%{toWhoShort}";
                    rankedEffect.ToolTip = greTooltip;

                    break;

                default:
                    var configDisablePvE = MidsContext.Config != null && MidsContext.Config.Inc.DisablePvE;

                    rankedEffect.Value = $"{magSum:####0.##}{(effectSource.DisplayPercentage ? "%" : "")}{toWhoShort}";
                    rankedEffect.Name = FastItemBuilder.Str.ShortStr(displayBlockFontSize, Enums.GetEffectName(effectSource.EffectType),
                        Enums.GetEffectNameShort(effectSource.EffectType));
                    rankedEffect.ToolTip = string.Join("\r\n", pEnh.Effects
                        .Where(e => configDisablePvE && (e.PvMode == Enums.ePvX.PvP) |
                                    !configDisablePvE && (e.PvMode == Enums.ePvX.PvE) |
                                    (e.PvMode == Enums.ePvX.Any) &&
                                    Math.Abs(e.BuffedMag) > Tolerance &&
                                    effectSource.ToWho == e.ToWho &&
                                    effectSource.EffectType == e.EffectType &&
                                    effectSource.MezType == e.MezType &&
                                    effectSource.ETModifies == e.ETModifies &&
                                    (effectSource.PvMode == e.PvMode) | (e.PvMode == Enums.ePvX.Any) &&
                                    effectSource.IgnoreScaling == e.IgnoreScaling)
                        .Select(e => e.BuildEffectString(false, "", false, false, false, true)));

                    break;
            }
        }
    }
}

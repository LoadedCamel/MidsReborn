using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mids_Reborn.Controls;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    #region List extensions

    public static class ListExtGFx
    {
        /// <summary>
        /// Check if a list contains all elements of another list
        /// </summary>
        /// <typeparam name="T">Elements type</typeparam>
        /// <param name="list">List to look for elements into</param>
        /// <param name="elements">Values to look from</param>
        /// <returns>true if list contains all elements, false otherwise</returns>
        public static bool ContainsAll<T>(this List<T> list, List<T> elements)
        {
            return elements.All(list.Contains);
        }

        /// <summary>
        /// Add a range of elements to a list, but avoid duplicating entries
        /// </summary>
        /// <typeparam name="T">Elements type</typeparam>
        /// <param name="list">List to add elements into</param>
        /// <param name="elements">Values to add</param>
        public static void AddRangeUnique<T>(this List<T> list, List<T> elements)
        {
            foreach (var e in elements)
            {
                if (!list.Contains(e))
                {
                    list.Add(e);
                }
            }
        }
    }

    #endregion

    #region Dictionary extensions

    public static class DictionaryExtGFx
    {
        /// <summary>
        /// Check if keys of a dictionary starts with a prefix.
        /// </summary>
        /// <typeparam name="T1">Keys type</typeparam>
        /// <typeparam name="T2">Values type</typeparam>
        /// <param name="dict"></param>
        /// <param name="prefix">Prefix string to look for</param>
        /// <param name="nameFound">Will put in nameFound the full matched key name, if any</param>
        /// <returns>True if a key name starts with prefix, false otherwise</returns>
        public static bool ContainsKeyPrefix<T1, T2>(this Dictionary<T1, T2> dict, string prefix, ref string nameFound)
        {
            foreach (var k in dict.Keys)
            {
                if (!$"{k}".StartsWith(prefix))
                {
                    continue;
                }

                nameFound = $"{k}";
                    
                return true;
            }

            return false;
        }
    }

    #endregion

    public class GroupedFx
    {
        public struct FxId
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

        public int NumEffects => IncludedEffects.Count;
        public Enums.eEffectType EffectType => FxIdentifier.EffectType;
        public Enums.eEffectType ETModifies => FxIdentifier.ETModifies;
        public Enums.eMez MezType => FxIdentifier.MezType;
        public Enums.eDamage DamageType => FxIdentifier.DamageType;
        public Enums.eToWho ToWho => FxIdentifier.ToWho;
        public Enums.ePvX PvMode => FxIdentifier.PvMode;
        public bool IgnoreScaling => FxIdentifier.IgnoreScaling;
        public bool EnhancementEffect => IsEnhancement;

        /// <summary>
        /// Build a grouped effect instance from an effect identifier
        /// </summary>
        /// <param name="fxIdentifier">Effect identifier struct (effect type, damage type, mez type, modified effect type)</param>
        /// <param name="mag">Effect magnitude (use Effect.BuffedMag)</param>
        /// <param name="alias">Display name (e.g. Defense(all))</param>
        /// <param name="includedEffects">Included effects indexes from source power</param>
        /// <param name="isEnhancement">Effect comes from an enhancement special</param>
        /// <param name="specialCase">Effect special case (typically Defiance)</param>
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
        }

        /// <summary>
        /// Build a grouped effect from an aggregate of multiple grouped effects
        /// </summary>
        /// <param name="fxIdentifier">Effect identifier struct</param>
        /// <param name="greList">List of grouped effects</param>
        public GroupedFx(FxId fxIdentifier, List<GroupedFx> greList)
        {
            FxIdentifier = fxIdentifier;
            Mag = greList[0].Mag;
            Alias = greList[0].Alias;
            IsEnhancement = greList[0].IsEnhancement;
            SpecialCase = greList[0].SpecialCase;
            IsAggregated = true;
            SingleEffectSource = null;

            IncludedEffects = new List<int>();
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

        /// <summary>
        /// Build a grouped effect instance from a single effect
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="fxIndex">Effect index in the source power</param>
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
            IncludedEffects = new List<int> {fxIndex};
            IsEnhancement = effect.isEnhancementEffect;
            SpecialCase = effect.SpecialCase;
            IsAggregated = false;
        }

        public override string ToString()
        {
            return $"<GroupedFx> {{{FxIdentifier}, effects: {IncludedEffects.Count}, Mag: {Mag}, EnhancementFx: {IsEnhancement}, Special case: {SpecialCase}, Aggregated: {IsAggregated}}}";
        }

        /// <summary>
        /// From an effect index, try to find the matching index in the ranked effects table.
        /// Returns -1 if IncludedEffects is empty.
        /// </summary>
        /// <param name="rankedEffects">Ranked effects table</param>
        /// <param name="index">Effects index, from IncludedEffects</param>
        /// <returns>Ranked effect index</returns>
        public int GetRankedEffectIndex(IEnumerable<int> rankedEffects, int index)
        {
            if (IncludedEffects.Count <= 0) return -1;

            return rankedEffects.TryFindIndex(e => e == IncludedEffects[index]);
        }

        /// <summary>
        /// Get matching effect from an effect index (in IncludedEffects)
        /// </summary>
        /// <param name="power">Source power</param>
        /// <param name="index">Effect index, in IncludedEffects</param>
        /// <returns>Matching effect from source power</returns>
        public IEffect GetEffectAt(IPower power, int index = 0)
        {
            return power.Effects[IncludedEffects[index]];
        }

        /// <summary>
        /// Get mathing effect from an effect index (in power effects)
        /// </summary>
        /// <param name="power">Source power</param>
        /// <param name="index">Effect index</param>
        /// <returns>Matching effect from source power</returns>
        private IEffect GetPowerEffectAt(IPower power, int index = 0)
        {
            return power.Effects[index];
        }

        /// <summary>
        /// Get all defense vectors, including None
        /// </summary>
        /// <returns>List of defense vectors. If real uses toxic defense, it will be included.</returns>
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

        /// <summary>
        /// Get all defense vectors
        /// </summary>
        /// <returns>List of defense vectors. If real uses toxic defense, it will be included.</returns>
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

        /// <summary>
        /// Get all position defense vectors
        /// </summary>
        /// <returns>List of position defense vectors.</returns>
        private static List<Enums.eDamage> GetPositionDefenses()
        {
            return new List<Enums.eDamage>
            {
                Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
            };
        }

        /// <summary>
        /// Get all typed defense vectors
        /// </summary>
        /// <returns>List of defense vectors. If real uses toxic defense, it will be included.</returns>
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
            // return new List<Enums.eDamage>
            // {
            //     Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            //     Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
            //     DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.Toxic : Enums.eDamage.Psionic
            // };
            // Old Commented out above
        }

        /// <summary>
        /// Get all damage resistance vectors
        /// </summary>
        /// <returns>List of damage resistance vectors.</returns>
        private static List<Enums.eDamage> GetAllResistances()
        {
            return new List<Enums.eDamage>
            {
                Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic
            };
        }

        /// <summary>
        /// Get all mez vectors
        /// </summary>
        /// <returns>List of main vectors: Immobilized, Held, Stunned, Sleep, Terrorized, Confused.</returns>
        private static List<Enums.eMez> GetAllMez()
        {
            return new List<Enums.eMez>
            {
                Enums.eMez.Immobilized, Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep,
                Enums.eMez.Terrorized, Enums.eMez.Confused
            };
        }

        /// <summary>
        /// Get all movement vectors
        /// </summary>
        /// <returns>List of movement vectors: SpeedFlying, SpeedJumping, SpeedRunning</returns>
        private static List<Enums.eEffectType> GetAllMovement()
        {
            return new List<Enums.eEffectType>
            {
                Enums.eEffectType.SpeedFlying, Enums.eEffectType.SpeedJumping, Enums.eEffectType.SpeedRunning
            };
        }

        /// <summary>
        /// Try to find a fitting alias for the grouped effect.
        /// For Defense, all present vectors will output Defense(all), all positions Defense(All positions), all types Defense(All types)
        /// Mixed vectors in a partial set will show as 'Multi'
        /// </summary>
        /// <remarks>Defense will check against Defense(Toxic) if the realm supports it.</remarks>
        /// <remarks>Will only work for Defense, Resistance, Elusivity, Enhancement(Mez), Enhancement(MezResist), MezResist.</remarks>
        /// <param name="power">Source power</param>
        /// <returns>EffectType with multiple vectors indicator</returns>
        public string GetStatName(IPower power)
        {
            var fx = power.Effects
                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                .Where(e => IncludedEffects.Contains(e.Key))
                .Select(e => e.Value)
                .ToList();

            var allDefenses = GetAllDefenses();
            var positionDefenses = GetPositionDefenses();
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
                Enums.eEffectType.Defense or Enums.eEffectType.Elusivity => fxDamageTypes.ContainsAll(allDefenses)
                    ? "All"
                    : fxDamageTypes.ContainsAll(positionDefenses)
                        ? "All positions"
                        : fxDamageTypes.ContainsAll(typedDefenses)
                            ? "All types"
                            : fxDamageTypes.Count > 1
                                ? "Multi"
                                : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Resistance or Enums.eEffectType.DamageBuff => fxDamageTypes.ContainsAll(
                    allResistances)
                    ? "All"
                    : fxDamageTypes.Count > 1
                        ? "Multi"
                        : $"{fxDamageTypes[0]}",

                Enums.eEffectType.MezResist => fxMezTypes.ContainsAll(allMez)
                    ? "All"
                    : fxMezTypes.Count > 1
                        ? "Multi"
                        : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => fxMezTypes.ContainsAll(allMez)
                    ? "All"
                    : fxMezTypes.Count > 1
                        ? "Multi"
                        : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Elusivity => fxDamageTypes.ContainsAll(allDefenses)
                    ? "All"
                    : fxDamageTypes.ContainsAll(positionDefenses)
                        ? "All positions"
                        : fxDamageTypes.ContainsAll(typedDefenses)
                            ? "All types"
                            : fxDamageTypes.Count > 1
                                ? "Multi"
                                : $"{fxDamageTypes[0]}",

                Enums.eEffectType.Enhancement when FxIdentifier.ETModifies is Enums.eEffectType.Resistance => fxDamageTypes.ContainsAll(
                        allResistances)
                        ? "All"
                        : fxDamageTypes.Count > 1
                            ? "Multi"
                            : $"{fxDamageTypes[0]}",

                Enums.eEffectType.ResEffect => fxEffectTypes.Count > 1 ? "Multi" : $"{fxEffectTypes[0]}",

                _ => ""
            };

            if (FxIdentifier.EffectType is Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning)
            {
                if (fxMainEffectTypes.ContainsAll(allMovement))
                {
                    return "Slow";
                }
            }

            return groupedVector != "" ? $"{fx[0].EffectType} ({groupedVector})" : $"{fx[0].EffectType}";
        }

        private string GetGroupedVector(string statName, bool ignoreMulti = true)
        {
            return statName switch
            {
                _ when statName.Contains("All types") => "All types",
                _ when statName.Contains("All positions") => "All positions",
                _ when statName.Contains("All") => "All",
                _ when statName.Contains("Multi") & !ignoreMulti => "Multi",
                _ => ""
            };
        }

        private string GetGroupedVector(IPower power, bool ignoreMulti = true)
        {
            return GetGroupedVector(GetStatName(power), ignoreMulti);
        }

        /// <summary>
        /// Compact display of a list of vectors
        /// Defense, Elusivity, Resistance and Mez will show stat(All) when possible
        /// </summary>
        /// <remarks>Behavior unknown with Elusivity, Resistance and Mez</remarks>
        /// <param name="vectors">List of vectors, as strings</param>
        /// <returns>Compact form of the list of vectors</returns>
        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors, Enums.eEffectType effectType, Enums.eEffectType etModifies)
        {
            // Defense
            var allDefensesEx = GetAllDefensesEx()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allDefenses = GetAllDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var positionDefenses = GetPositionDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var typedDefenses = GetTypedDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            // Elusivity
            var allElusivity = GetAllDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var positionElusivity = GetPositionDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var typedElusivity = GetTypedDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            // Resistance
            var allResistances = GetAllResistances()
                .ToDictionary(e => $"{e} Resistance", _ => -1);

            // Mez
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

                //////////////////////

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

                //////////////////////

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allResistances[keyNameFound] = i;
                }

                //////////////////////

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
                        cVectors.Add("Defense(All)");
                        ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
                    }
                    else if (allDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense(All)");
                        ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
                    }
                    else if (positionDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense(All positions)");
                        ignoredVectors.AddRangeUnique(positionDefenses.Values.ToList());
                    }
                    else if (typedDefenses.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Defense(All types)");
                        ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
                    }
                    else if (typedDefenses.Count(e => e.Value >= 0) == typedDefenses.Count - 1)
                    {
                        var diff = typedDefenses.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Defense") ? e : $"{e} Defense")).First();
                        cVectors.Add($"Defense(All types but {diff.Replace(" Defense", "")})");
                        ignoredVectors.AddRangeUnique(typedDefenses.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Elusivity:
                    if (allElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity(All)");
                        ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
                    }
                    else if (positionElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity(All positions)");
                        ignoredVectors.AddRangeUnique(positionElusivity.Values.ToList());
                    }
                    else if (typedElusivity.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Elusivity(All types)");
                        ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
                    }
                    else if (typedElusivity.Count(e => e.Value >= 0) == typedElusivity.Count - 1)
                    {
                        var diff = typedElusivity.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Elusivity") ? e : $"{e} Elusivity")).First();
                        cVectors.Add($"Elusivity(All types but {diff.Replace(" Elusivity", "")})");
                        ignoredVectors.AddRangeUnique(typedElusivity.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
                    }

                    break;

                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Enhancement when etModifies == Enums.eEffectType.Resistance:
                    if (allResistances.All(e => e.Value >= 0))
                    {
                        cVectors.Add("Resistance(All)");
                        ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
                    }
                    else if (allResistances.Count(e => e.Value >= 0) == allResistances.Count - 1)
                    {
                        var diff = allResistances.Select(e => e.Key).Except(vectors.Select(e => e.EndsWith(" Resistance") ? e : $"{e} Resistance")).First();
                        cVectors.Add($"Resistance(All but {diff.Replace(" Resistance", "")})");
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

            // Run pass 2 for multi-effect enhancements
            return CompactVectorsList(cVectors);
        }

        private static List<string> CompactVectorsList(IReadOnlyList<string> vectors)
        {
            // Defense
            var allDefensesEx = GetAllDefensesEx()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var allDefenses = GetAllDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var positionDefenses = GetPositionDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            var typedDefenses = GetTypedDefenses()
                .ToDictionary(e => $"{e} Defense", _ => -1);

            // Elusivity
            var allElusivity = GetAllDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var positionElusivity = GetPositionDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            var typedElusivity = GetTypedDefenses()
                .ToDictionary(e => $"{e} Elusivity", _ => -1);

            // Resistance
            var allResistances = GetAllResistances()
                .ToDictionary(e => $"{e} Resistance", _ => -1);

            // Mez
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

                //////////////////////

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

                //////////////////////

                if (allResistances.ContainsKeyPrefix(vectors[i], ref keyNameFound))
                {
                    allResistances[keyNameFound] = i;
                }

                //////////////////////

                if (allMez.ContainsKey(vectors[i]))
                {
                    allMez[vectors[i]] = i;
                }
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

            // Defense, Enhancement(Defense)
            if (allDefensesEx.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense(All)");
                ignoredVectors.AddRangeUnique(allDefensesEx.Values.ToList());
            }
            else if (allDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense(All)");
                ignoredVectors.AddRangeUnique(allDefenses.Values.ToList());
            }
            else if (positionDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense(All positions)");
                ignoredVectors.AddRangeUnique(positionDefenses.Values.ToList());
            }
            else if (typedDefenses.All(e => e.Value >= 0))
            {
                cVectors.Add("Defense(All types)");
                ignoredVectors.AddRangeUnique(typedDefenses.Values.ToList());
            }
            else if (typedDefenses.Count(e => e.Value >= 0) == typedDefenses.Count - 1)
            {
                var diff = typedDefenses.Select(e => e.Key)
                    .Except(vectors.Select(e => e.EndsWith(" Defense") ? e : $"{e} Defense")).First();
                cVectors.Add($"Defense(All types but {diff.Replace(" Defense", "")})");
                ignoredVectors.AddRangeUnique(typedDefenses.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
            }

            // Elusivity, Enhancement(Elusivity)
            if (allElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity(All)");
                ignoredVectors.AddRangeUnique(allElusivity.Values.ToList());
            }
            else if (positionElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity(All positions)");
                ignoredVectors.AddRangeUnique(positionElusivity.Values.ToList());
            }
            else if (typedElusivity.All(e => e.Value >= 0))
            {
                cVectors.Add("Elusivity(All types)");
                ignoredVectors.AddRangeUnique(typedElusivity.Values.ToList());
            }
            else if (typedElusivity.Count(e => e.Value >= 0) == typedElusivity.Count - 1)
            {
                var diff = typedElusivity.Select(e => e.Key)
                    .Except(vectors.Select(e => e.EndsWith(" Elusivity") ? e : $"{e} Elusivity")).First();
                cVectors.Add($"Elusivity(All types but {diff.Replace(" Elusivity", "")})");
                ignoredVectors.AddRangeUnique(typedElusivity.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
            }

            // Resistance, Enhancement(Resistance)
            if (allResistances.All(e => e.Value >= 0))
            {
                cVectors.Add("Resistance(All)");
                ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
            }
            else if (allResistances.Count(e => e.Value >= 0) == allResistances.Count - 1)
            {
                var diff = allResistances.Select(e => e.Key)
                    .Except(vectors.Select(e => e.EndsWith(" Resistance") ? e : $"{e} Resistance")).First();
                cVectors.Add($"Resistance(All but {diff.Replace(" Resistance", "")})");
                ignoredVectors.AddRangeUnique(allResistances.Where(e => e.Value >= 0).Select(e => e.Value).ToList());
            }

            // Mez, Enhancement(Mez)
            if (allMez.All(e => e.Value >= 0))
            {
                cVectors.Add("Mez");
                ignoredVectors.AddRangeUnique(allMez.Values.ToList());
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));

            return cVectors;
        }

        /// <summary>
        /// Change sign of a numeric value, as a string.
        /// </summary>
        /// <param name="value">Input value (string)</param>
        /// <returns>Negative value if it is positive, stripped of the minus sign if it is negative.</returns>
        private static string InvertStringValue(string value)
        {
            return value.StartsWith("-") ? value[1..] : $"-{value}";
        }

        /// <summary>
        /// Generate tooltip for a grouped effect.
        /// </summary>
        /// <param name="power">Source power</param>
        /// <returns>Build effect string from each effect, then concatenate into a single string (one effect per line)</returns>
        public string GetTooltip(IPower power)
        {
            if (IsAggregated)
            {
                return string.Join("\r\n", IncludedEffects
                    .Select(e => power.Effects[e].BuildEffectString(false, "", false, false, false, false, false, true)));
            }

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
                    Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning  => IncludedEffects
                        .Select(e => $"{power.Effects[e].EffectType}")
                        .ToList(),
                    
                    Enums.eEffectType.Mez or Enums.eEffectType.MezResist => IncludedEffects
                        .Select(e => $"{power.Effects[e].MezType}") // Cannot use .Cast<string>()
                        .ToList(),

                    Enums.eEffectType.Enhancement when
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                            or Enums.eEffectType.MezResist => !string.IsNullOrEmpty(groupedVector)
                            ? new List<string> {$"{groupedVector}"}
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].MezType}")
                                .ToList(),

                    Enums.eEffectType.Enhancement =>
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense
                            or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity &&
                        !string.IsNullOrEmpty(groupedVector)
                            ? new List<string> {$"{groupedVector} {power.Effects[IncludedEffects[0]].ETModifies}"}
                            : IncludedEffects
                                .Select(e =>
                                    power.Effects[e].ETModifies is Enums.eEffectType.Defense
                                        or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                                        ? $"{power.Effects[e].DamageType} {power.Effects[e].ETModifies}"
                                        : $"{power.Effects[e].ETModifies}")
                                .ToList(),

                    Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                        or Enums.eEffectType.DamageBuff => !string.IsNullOrEmpty(groupedVector)
                            ? new List<string> {$"{power.Effects[IncludedEffects[0]].EffectType}({groupedVector})"}
                            : IncludedEffects
                                .Select(e => $"{power.Effects[e].DamageType}")
                                .ToList(),

                    Enums.eEffectType.ResEffect => IncludedEffects
                        .Select(e => $"{power.Effects[e].ETModifies}")
                        .ToList(),

                    _ => new List<string>()
                };

                uniqueVectors.AddRangeUnique(vectorsChunks);
                uniqueVectors = CompactVectorsList(uniqueVectors, power.Effects[IncludedEffects[0]].EffectType, power.Effects[IncludedEffects[0]].ETModifies);
                vectors = string.Join(", ", uniqueVectors);
            }

            // Change stat name inside effect string with list of vectors
            // Use the first effect of the group as base
            var baseEffectString = power.Effects[IncludedEffects[0]].BuildEffectString(false, "", false, false, false, false, false, true);

            var tip = power.Effects[IncludedEffects[0]].EffectType switch
            {
                Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedRunning => statName == "Slow"
                    ? InvertStringValue(Regex.Replace(baseEffectString, @"(SpeedFlying|SpeedJumping|SpeedRunning)", "Slow")) // Slow is positive when speeds are negative
                    : Regex.Replace(baseEffectString, @"(SpeedFlying|SpeedJumping|SpeedRunning)", vectors),

                Enums.eEffectType.Mez or Enums.eEffectType.MezResist => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].MezType})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                Enums.eEffectType.Enhancement when power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].MezType})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({(vectors == "All" && power.Effects[IncludedEffects[0]].ETModifies == Enums.eEffectType.Mez ? "Mez" : vectors)})"),

                Enums.eEffectType.Enhancement when
                    power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense
                        or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].DamageType} {power.Effects[IncludedEffects[0]].ETModifies})",
                        $"{power.Effects[IncludedEffects[0]].EffectType}({vectors}{(power.Effects[IncludedEffects[^1]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity && vectors.Contains("All") ? $" {power.Effects[IncludedEffects[^1]].ETModifies}" : "")})"),

                Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].ETModifies})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                Enums.eEffectType.Resistance or Enums.eEffectType.Defense or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.DamageBuff => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].DamageType})",
                        $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                Enums.eEffectType.SilentKill => baseEffectString.Replace("SilentKill", "Self-Destructs")
                    .Replace(" in ", " after ")
                    .Replace(" to Self", "")
                    .Replace(" to Target", ""),

                _ => baseEffectString
            };

            return Regex.Replace(tip, @"(?<stat>[0-9A-Za-z\-]+)\(\k<stat>", "$1") // statName(statName (both same expression match)
                .Replace("((", "(")
                .Replace("))", ")")
                .Replace("None Defense", "Base Defense");
        }

        /// <summary>
        /// Get similar effects from power: similar effect type (see below), same mag, same origin (power vs enhancement special), same special case (for DamageBuff)
        /// Group up Enhancement(Mez)/Enhancement(MezResist) by mez type, other enhancement effects by effect type (those to the exclusion of Mez/MezResist)
        /// Group up DamageBuff effects by damage type, and special case (for Defiance)
        /// Group up Defense, Resistance, Elusivity by damage type
        /// Group up ResEffect by sub effect type (ETModifies)
        /// </summary>
        /// <param name="power">Power to inspect effects from</param>
        /// <param name="fxIdentifier">Effect identifier struct</param>
        /// <param name="mag">Magnitude value (use Effect.BuffedMag)</param>
        /// <param name="specialCase">Effect special case to use. Note: only used for Defiance.</param>
        /// <param name="enhancementEffect">True if effect comes from an enhancement, false if from power itself.</param>
        /// <returns>List of matching effect indices from source power</returns>
        public static List<int> GetSimilarEffects(IPower power, FxId fxIdentifier, float mag,
            Enums.eSpecialCase specialCase = Enums.eSpecialCase.None, bool enhancementEffect = false)
        {
            return fxIdentifier.EffectType switch
            {
                Enums.eEffectType.EntCreate => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                e.Value.nSummon == fxIdentifier.SummonId && e.Value.ToWho == fxIdentifier.ToWho &&
                                (Math.Abs(e.Value.Duration - fxIdentifier.Duration) < float.Epsilon || fxIdentifier.Duration == 0) &&
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
                            Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
                            e.Value.isEnhancementEffect == enhancementEffect &&
                            e.Value.PvMode == fxIdentifier.PvMode &&
                            e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                        .Select(e => e.Key)
                        .ToList(),

                // Keep Enhancement(Mez) / Enhancement(MezResist) separate from other Enhancement effects
                Enums.eEffectType.Enhancement when fxIdentifier.ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType &&
                                e.Value.ETModifies == fxIdentifier.ETModifies && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
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
                                Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
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
                                    Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
                                    e.Value.isEnhancementEffect == enhancementEffect &&
                                    e.Value.PvMode == fxIdentifier.PvMode &&
                                    e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                        .Select(e => e.Key)
                        .ToList(),

                Enums.eEffectType.DamageBuff when specialCase == Enums.eSpecialCase.Defiance => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
                                e.Value.SpecialCase == specialCase && e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.DamageBuff => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho &&
                                Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon &&
                                e.Value.SpecialCase != Enums.eSpecialCase.Defiance &&
                                e.Value.isEnhancementEffect == enhancementEffect &&
                                e.Value.PvMode == fxIdentifier.PvMode &&
                                e.Value.IgnoreScaling == fxIdentifier.IgnoreScaling)
                    .Select(e => e.Key)
                    .ToList(),

                _ => new List<int>()
            };
        }

        /// <summary>
        /// Build grouped effects from ranked effects for a power
        /// </summary>
        /// <param name="power">Source power to build effects from. Use the enhanced power, not base.</param>
        /// <returns>List of grouped effects from source power</returns>
        public static List<GroupedFx> AssembleGroupedEffects(IPower power)
        {
            var rankedEffects = power.GetRankedEffects(true);
            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var ignoredEffects = new List<int>();
            var groupedRankedEffects = new List<GroupedFx>();
            if (power == null)
            {
                return new List<GroupedFx>();
            }

            // Pass 1: build grouped effects from power effects

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

                if (power.Effects[re].EffectType is Enums.eEffectType.Damage
                    or Enums.eEffectType.Meter or Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode
                    or Enums.eEffectType.Null or Enums.eEffectType.NullBool or Enums.eEffectType.GlobalChanceMod
                    or Enums.eEffectType.ExecutePower)
                {
                    continue;
                }

                if (power.Effects[re].EffectType == Enums.eEffectType.ResEffect &
                    power.Effects[re].ETModifies is Enums.eEffectType.Null or Enums.eEffectType.NullBool)
                {
                    continue;
                }

                if (!(power.Effects[re].Probability > 0 &
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

                if (power.Effects[re].PvMode == Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE |
                    power.Effects[re].PvMode == Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE)
                {
                    continue;
                }

                if (power.Effects[re].ActiveConditionals is {Count: > 0})
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
            
            // Pass 2: aggregate similar grouped effect containing a single effect
            
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
                                Math.Abs(e.Value.Mag - groupedRankedEffects[i].Mag) < float.Epsilon &&
                                e.Value.EnhancementEffect == groupedRankedEffects[i].EnhancementEffect &&
                                e.Value.SpecialCase == groupedRankedEffects[i].SpecialCase)
                    .ToList();

                ignoredGroups.AddRangeUnique(similarGreList.Select(e => e.Key).ToList());

                groupedRankedEffects2.Add(new GroupedFx(groupedRankedEffects[i].FxIdentifier, similarGreList.Select(e => e.Value).ToList()));
            }

            // Pass 3: generate aggregated GroupedFx, recalc mag sum

            var greAggregated = Aggregate(groupedRankedEffects2);
            foreach (var gre in greAggregated)
            {
                if (!gre.IsAggregated)
                {
                    continue;
                }

                gre.Mag = gre.GetMagSum(power);
            }

            return greAggregated;
        }

        /// <summary> Generate ItemPairs usable in the DataView, with their associated grouped effect and effect identifier.</summary>
        /// <param name="groupedRankedEffects">Raw grouped effects (use output from AssembleGroupedEffects)</param>
        /// <returns>Ranked effects matching grouped effects in the form of ItemPairs, with their associated grouped effect, and effect identifier</returns>
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

        /// <summary>
        /// Filter a list of elaborate ItemPairs to a flat list of ItemPairs, matching a filter criteria
        /// </summary>
        /// <param name="itemsDict">ItemPairs (use output from GenerateListItems)</param>
        /// <returns>List of ItemPair matching criteria in filterFunc</returns>
        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict, Func<FxId, bool> filterFunc)
        {
            if (itemsDict == null)
            {
                return new List<PairedListEx.Item>();
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

        /// <summary>
        /// Filter a list of elaborate ItemPairs to a flat list of ItemPairs
        /// This overload will return all items.
        /// </summary>
        /// <param name="itemsDict">ItemPairs (use output from GenerateListItems)</param>
        /// <returns>Full list of ItemPairs</returns>
        public static List<PairedListEx.Item> FilterListItems(List<KeyValuePair<GroupedFx, PairedListEx.Item>>? itemsDict)
        {
            if (itemsDict == null)
            {
                return new List<PairedListEx.Item>();
            }

            return itemsDict
                .Select(e => e.Value)
                .ToList();
        }

        public static List<KeyValuePair<GroupedFx, PairedListEx.Item>> FilterListItemsExt(List<KeyValuePair<GroupedFx, PairedListEx.Item>> itemsDict)
        {
            return itemsDict ?? new List<KeyValuePair<GroupedFx, PairedListEx.Item>>();
        }

        /// <summary>
        /// Generate a short power description from a power given its effects.
        /// </summary>
        /// <param name="power">Source power</param>
        /// <returns>Brief of power effects. Self effects will always come first.</returns>
        private static string GeneratePowerDescShort(IPower? power)
        {
            var effectShorts = new List<string>();
            var fxIdList = new List<FxId>();
            var effects = (IEffect[]) power.Effects.Clone();
            effects = effects.OrderBy(e => e.ToWho)
                .Where(e => e.EffectType is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool
                                or Enums.eEffectType.Meter or Enums.eEffectType.Damage or Enums.eEffectType.MaxFlySpeed
                                or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.MaxRunSpeed
                                or Enums.eEffectType.ExecutePower or Enums.eEffectType.RevokePower
                                or Enums.eEffectType.GlobalChanceMod or Enums.eEffectType.SetMode
                                or Enums.eEffectType.SetCostume) &&
                            e.ETModifies is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool) &&
                            e.ToWho != Enums.eToWho.Unspecified &&
                            Math.Abs(e.BuffedMag) >= float.Epsilon &&
                            (e.PvMode == Enums.ePvX.Any || (e.PvMode == Enums.ePvX.PvE && !MidsContext.Config.Inc.DisablePvE) || (e.PvMode == Enums.ePvX.PvP && MidsContext.Config.Inc.DisablePvE)) &&
                            (e.ActiveConditionals is {Count: <= 0} || e.ValidateConditional()))
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
                    Enums.eEffectType.MezResist => $"({effects[i].EffectType} ({effects[i].MezType}){toWho}",
                    Enums.eEffectType.Mez => $"{effects[i].ToWho} {mezType}",
                    Enums.eEffectType.Enhancement => $"{effects[i].ToWho} {(effects[i].BuffedMag > 0 ? "+" : "-")}{effects[i].ETModifies}",
                    _ => $"{(effects[i].BuffedMag < 0 ? "-" : "")}{effects[i].EffectType}{toWho}"
                });

                fxIdList.Add(fxIdentifier);
            }

            return string.Join(", ", effectShorts);
        }

        /// <summary>
        /// Get the total magnitude of a grouped effect.
        /// If all values are negative sum up everything.
        /// If some are positive, ignore the negative ones.
        /// This overload will return magnitude sum based on base and enhanced powers.
        /// </summary>
        /// <param name="pBase">Base power</param>
        /// <param name="pEnh">Enhanced power</param>
        /// <returns>Magnitude sum for this grouped effect based on both base and enhanced power as an EnhancedMagSum struct.</returns>
        public EnhancedMagSum GetMagSum(IPower pBase, IPower pEnh)
        {
            var allNegBase = IncludedEffects
                .Select(e => GetPowerEffectAt(pBase, e).BuffedMag)
                .All(e => e < 0);

            var allNegEnh = IncludedEffects
                .Select(e => GetPowerEffectAt(pEnh, e).BuffedMag)
                .All(e => e < 0);

            return new EnhancedMagSum
            {
                Base = allNegBase
                    ? IncludedEffects
                        .Select(e => GetPowerEffectAt(pBase, e).BuffedMag)
                        .Sum()
                    : IncludedEffects
                        .Select(e => GetPowerEffectAt(pBase, e).BuffedMag)
                        .Where(e => e > 0)
                        .Sum(),
                Enhanced = allNegEnh
                    ? IncludedEffects
                        .Select(e => GetPowerEffectAt(pEnh, e).BuffedMag)
                        .Sum()
                    : IncludedEffects
                        .Select(e => GetPowerEffectAt(pEnh, e).BuffedMag)
                        .Where(e => e > 0)
                        .Sum()
            };
        }

        /// <summary>
        /// Get the total magnitude of a grouped effect.
        /// If all values are negative sum up everything.
        /// If some are positive, ignore the negative ones.
        /// This overload will return magnitude sum based on a single power.
        /// </summary>
        /// <remarks>This is intended to be used with the enhanced power as a shortcut.</remarks>
        /// <param name="power">Source power</param>
        /// <returns>Magnitude sum for this grouped effect based on source power, as a float.</returns>
        public float GetMagSum(IPower power)
        {
            var allNegEnh = IncludedEffects
                .Select(e => GetPowerEffectAt(power, e).BuffedMag)
                .All(e => e < 0);

            if (GetEffectAt(power).EffectType is Enums.eEffectType.Defense
                or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity or Enums.eEffectType.Mez
                or Enums.eEffectType.MezResist or Enums.eEffectType.ResEffect or Enums.eEffectType.Enhancement)
            {
                return GetEffectAt(power).BuffedMag;
            }

            return allNegEnh
                ? IncludedEffects
                    .Select(e => GetPowerEffectAt(power, e).BuffedMag)
                    .Sum()
                : IncludedEffects
                    .Select(e => GetPowerEffectAt(power, e).BuffedMag)
                    .Where(e => e > 0)
                    .Sum();
        }
        
        /// <summary>
        /// Create an aggregated list of GroupedFx from multiple similar ones.
        /// </summary>
        /// <param name="greList">Source list of grouped effects</param>
        /// <returns>List of merged grouped effects. Those created this way will have IsAggregated = true</returns>
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
                var includedGre = new List<int> {i};
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

                    if (!greList[i].FxIdentifier.Equals(greList[j].FxIdentifier))
                    {
                        continue;
                    }

                    includedGre.Add(j);
                    excludedGre.Add(j);
                }

                ret.Add(new GroupedFx(greList[i].FxIdentifier, includedGre.Select(e => greList[e]).ToList()));
            }

            return ret;
        }

        /// <summary>Post-processing of a ranked effect to fine-tune display</summary>
        /// <param name="rankedEffect">Base ranked effect (as ref)</param>
        /// <param name="pBase">Source power (base)</param>
        /// <param name="pEnh">Source power (enhanced)</param>
        /// <param name="gre">Associated grouped effect</param>
        /// <param name="effectIndex">Effect index in the enhanced power</param>
        /// <param name="powerInBuild">True if power has been picked in build</param>
        /// <param name="displayBlockFontSize">Display block font size</param>
        private static void FinalizeListItem(ref PairedListEx.Item rankedEffect, IPower pBase, IPower pEnh, GroupedFx gre, int effectIndex, bool powerInBuild, float displayBlockFontSize)
        {
            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var effectSource = gre.GetEffectAt(pEnh);
            var effectType = gre.EffectType;
            var greTooltip = gre.GetTooltip(pEnh);
            var magSum = gre.GetMagSum(pEnh);
            var mezDurationDiff = effectType == Enums.eEffectType.Mez & Math.Abs(
                (effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].Duration : 0) -
                (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].Duration : 0)) > float.Epsilon;
            var magDiff = Math.Abs((effectIndex < pBase.Effects.Length ? pBase.Effects[effectIndex].BuffedMag : 0) -
                                   (effectIndex < pEnh.Effects.Length ? pEnh.Effects[effectIndex].BuffedMag : 0)) > float.Epsilon |
                          mezDurationDiff;
            var toWhoShort = effectSource.ToWho switch
            {
                Enums.eToWho.Self => " (Slf)",
                Enums.eToWho.Target => " (Tgt)",
                _ => ""
            };

            rankedEffect.UseUniqueColor = effectSource.isEnhancementEffect;
            rankedEffect.UseAlternateColor = !effectSource.isEnhancementEffect &
                                          magDiff &
                                          ((effectIndex < pEnh.Effects.Length && Math.Abs(pEnh.Effects[effectIndex].BuffedMag - pEnh.Effects[effectIndex].Mag) > float.Epsilon) | mezDurationDiff) &
                                          effectSource.Buffable &
                                          powerInBuild;

            switch (effectType)
            {
                case Enums.eEffectType.Fly:
                case Enums.eEffectType.MovementControl:
                case Enums.eEffectType.MovementFriction:
                    rankedEffect.Value = effectSource.DisplayPercentage
                        ? $"{magSum * 100:###0.##}%{toWhoShort}"
                        : $"{magSum:###0.##}{toWhoShort}";

                    break;

                case Enums.eEffectType.Recovery:
                case Enums.eEffectType.Endurance:
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

                case Enums.eEffectType.EntCreate: // when !power.AbsorbSummonEffects | !power.AbsorbSummonAttributes:
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
                                             e.PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE ||
                                             e.PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE) &
                                            (e.ActiveConditionals.Count <= 0 || e.ValidateConditional()))
                                .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)
                                    .Replace("\r\n", "\n").Replace("\n", " -- ").Replace("  ", " ")));
                        rankedEffect.ToolTip = $"{mainEffectTip}\r\n----------\r\n{subEffectsTip}";
                    }

                    break;

                case Enums.eEffectType.CombatModShift:
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
                        Enums.eToWho.Target => effectSource.MezType is Enums.eMez.Knockback or Enums.eMez.Knockup
                            or Enums.eMez.Teleport
                            ? $"{effectSource.BuffedMag:###0.##} (Tgt)"
                            : $"{effectSource.Duration:###0.##}s (Mag {effectSource.BuffedMag:###0.##}, to Tgt)",

                        Enums.eToWho.Self => rankedEffect.Value = $"{effectSource.BuffedMag:###0.##} (Slf)",
                        
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
                    else if (gre.IncludedEffects.Count > 1)
                    {
                        rankedEffect.Name = $"{(effectSource.Mag < 0 ? "-" : "")}Movement";
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
                case Enums.eEffectType.Heal:
                    rankedEffect.Value = $"{gre.Mag * 100:###0.##}%";

                    break;

                default:
                    var configDisablePvE = MidsContext.Config != null && MidsContext.Config.Inc.DisablePvE;

                    rankedEffect.Value = $"{magSum:####0.##}{(effectSource.DisplayPercentage ? "%" : "")}{toWhoShort}";
                    //rankedEffect.UseAlternateColor = !effectSource.isEnhancementEffect && Math.Abs(magSumEnh - magSumBase) > float.Epsilon & effectSource.Buffable & powerInBuild;
                    rankedEffect.Name = FastItemBuilder.Str.ShortStr(displayBlockFontSize, Enums.GetEffectName(effectSource.EffectType),
                        Enums.GetEffectNameShort(effectSource.EffectType));
                    rankedEffect.ToolTip = string.Join("\r\n", pEnh.Effects
                        .Where(e => (configDisablePvE & e.PvMode == Enums.ePvX.PvP |
                                     !configDisablePvE & e.PvMode == Enums.ePvX.PvE |
                                     e.PvMode == Enums.ePvX.Any) &
                                    Math.Abs(e.BuffedMag) > float.Epsilon &
                                    effectSource.ToWho == e.ToWho &
                                    effectSource.EffectType == e.EffectType &
                                    effectSource.MezType == e.MezType &
                                    effectSource.ETModifies == e.ETModifies &
                                    (effectSource.PvMode == e.PvMode | e.PvMode == Enums.ePvX.Any) &
                                    effectSource.IgnoreScaling == e.IgnoreScaling)
                        .Select(e => e.BuildEffectString(false, "", false, false, false, true)));

                    break;
            }
        }
    }
}

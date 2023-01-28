using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
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

    public struct fxIdentifier
    {
        public Enums.eEffectType EffectType;
        public Enums.eMez MezType;
        public Enums.eDamage DamageType;
        public Enums.eEffectType ETModifies;
        public Enums.eToWho ToWho;

        public override string ToString()
        {
            return $"<fxIdentifier> {{{EffectType}, {ETModifies}, {MezType}, {DamageType}, {ToWho}}}";
        }
    }

    public class GroupedFx
    {
        private fxIdentifier FxIdentifier;
        private Enums.eSpecialCase SpecialCase;
        private float Mag;
        private string Alias;
        private List<int> IncludedEffects;
        private bool IsEnhancement;
        private IEffect? SingleEffectSource;

        public int NumEffects => IncludedEffects.Count;
        public Enums.eEffectType EffectType => FxIdentifier.EffectType;
        public Enums.eEffectType ETModifies => FxIdentifier.ETModifies;
        public Enums.eMez MezType => FxIdentifier.MezType;
        public Enums.eDamage DamageType => FxIdentifier.DamageType;
        public Enums.eToWho ToWho => FxIdentifier.ToWho;

        /// <summary>
        /// Build a grouped effect instance from an effect identifier
        /// </summary>
        /// <param name="fxIdentifier">Effect identifier struct (effect type, damage type, mez type, modified effect type)</param>
        /// <param name="mag">Effect magnitude (use Effect.BuffedMag)</param>
        /// <param name="alias">Display name (e.g. Defense(all))</param>
        /// <param name="includedEffects">Included effects indexes from source power</param>
        /// <param name="isEnhancement">Effect comes from an enhancement special</param>
        /// <param name="specialCase">Effect special case (typically Defiance)</param>
        public GroupedFx(fxIdentifier fxIdentifier, float mag, string alias,
            List<int> includedEffects, bool isEnhancement, Enums.eSpecialCase specialCase = Enums.eSpecialCase.None)
        {
            FxIdentifier = fxIdentifier;
            Mag = mag;
            Alias = alias;
            IncludedEffects = includedEffects;
            IsEnhancement = isEnhancement;
            SpecialCase = specialCase;
            SingleEffectSource = null;
        }

        /// <summary>
        /// Build a grouped effect instance from a single effect
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="fxIndex">Effect index in the source power</param>
        public GroupedFx(IEffect effect, int fxIndex)
        {
            SingleEffectSource = effect;
            FxIdentifier = new fxIdentifier
            {
                DamageType = effect.DamageType,
                EffectType = effect.EffectType,
                ETModifies = effect.ETModifies,
                MezType = effect.MezType,
                ToWho = effect.ToWho
            };
            Mag = effect.BuffedMag;
            Alias = "";
            IncludedEffects = new List<int> { fxIndex };
            IsEnhancement = effect.isEnhancementEffect;
            SpecialCase = effect.SpecialCase;
        }

        public override string ToString()
        {
            return $"<GroupedFx> {{{FxIdentifier}, effects: {IncludedEffects.Count}, Mag: {Mag}, EnhancementFx: {IsEnhancement}, Special case: {SpecialCase}}}";
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
            return new List<Enums.eDamage>
            {
                Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
                DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.Toxic : Enums.eDamage.Psionic
            };
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
        /// <returns>List of main vectors: immobilized, held, stunned, sleep, terrorized, confused.</returns>
        private static List<Enums.eMez> GetAllMez()
        {
            return new List<Enums.eMez>
            {
                Enums.eMez.Immobilized, Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep,
                Enums.eMez.Terrorized, Enums.eMez.Confused
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

            var fxDamageTypes = fx.Select(e => e.DamageType).ToList();
            var fxMezTypes = fx.Select(e => e.MezType).ToList();
            var fxEffectTypes = fx.Select(e => e.ETModifies).ToList();
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

            for (var i = 0; i < vectors.Count; i++)
            {
                if (allDefensesEx.ContainsKey(vectors[i]))
                {
                    allDefensesEx[vectors[i]] = i;
                }

                if (allDefenses.ContainsKey(vectors[i]))
                {
                    allDefenses[vectors[i]] = i;
                }

                if (positionDefenses.ContainsKey(vectors[i]))
                {
                    positionDefenses[vectors[i]] = i;
                }

                if (typedDefenses.ContainsKey(vectors[i]))
                {
                    typedDefenses[vectors[i]] = i;
                }

                //////////////////////
                
                if (allElusivity.ContainsKey(vectors[i]))
                {
                    allElusivity[vectors[i]] = i;
                }

                if (positionElusivity.ContainsKey(vectors[i]))
                {
                    positionElusivity[vectors[i]] = i;
                }

                if (typedElusivity.ContainsKey(vectors[i]))
                {
                    typedElusivity[vectors[i]] = i;
                }

                //////////////////////

                if (allResistances.ContainsKey(vectors[i]))
                {
                    allResistances[vectors[i]] = i;
                }

                //////////////////////

                if (allMez.ContainsKey(vectors[i]))
                {
                    allMez[vectors[i]] = i;
                }
            }

            var ignoredVectors = new List<int>();
            var cVectors = new List<string>();

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

            //////////////////////

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

            //////////////////////

            if (allResistances.All(e => e.Value >= 0))
            {
                cVectors.Add("Resistance(All)");
                ignoredVectors.AddRangeUnique(allResistances.Values.ToList());
            }

            //////////////////////

            if (allMez.All(e => e.Value >= 0))
            {
                cVectors.Add("Mez");
                ignoredVectors.AddRangeUnique(allMez.Values.ToList());
            }

            cVectors.AddRange(vectors.Where((_, i) => !ignoredVectors.Contains(i)));

            return cVectors;
        }

        /// <summary>
        /// Generate tooltip for a grouped effect.
        /// </summary>
        /// <param name="power">Source power</param>
        /// <returns>Build effect string from each effect, then concatenate into a single string (one effect per line)</returns>
        public string GetTooltip(IPower power)
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
                    Enums.eEffectType.Mez or Enums.eEffectType.MezResist => IncludedEffects
                        .Select(e => $"{power.Effects[e].MezType}") // Cannot use .Cast<string>()
                        .ToList(),

                    Enums.eEffectType.Enhancement when
                        power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                            or Enums.eEffectType.MezResist => !string.IsNullOrEmpty(groupedVector)
                        ? new List<string> { $"{groupedVector}" }
                        : IncludedEffects
                            .Select(e => $"{power.Effects[e].MezType}")
                            .ToList(),

                    Enums.eEffectType.Enhancement => power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity && !string.IsNullOrEmpty(groupedVector)
                        ? new List<string> { $"{groupedVector} {power.Effects[IncludedEffects[0]].ETModifies}" }
                        : IncludedEffects
                            .Select(e => power.Effects[e].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity ? $"{power.Effects[e].DamageType} {power.Effects[e].ETModifies}" : $"{power.Effects[e].ETModifies}")
                            .ToList(),

                    Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
                        or Enums.eEffectType.DamageBuff => !string.IsNullOrEmpty(groupedVector)
                        ? new List<string> { $"{power.Effects[IncludedEffects[0]].EffectType}({groupedVector})" }
                        : IncludedEffects
                            .Select(e => $"{power.Effects[e].DamageType}")
                            .ToList(),

                    Enums.eEffectType.ResEffect => IncludedEffects
                        .Select(e => $"{power.Effects[e].ETModifies}")
                        .ToList(),

                    _ => new List<string>()
                };

                uniqueVectors.AddRangeUnique(vectorsChunks);
                uniqueVectors = CompactVectorsList(uniqueVectors);
                vectors = string.Join(", ", uniqueVectors);
            }

            // Change stat name inside effect string with list of vectors
            // Use the first effect of the group as base
            var baseEffectString = power.Effects[IncludedEffects[0]].BuildEffectString(false, "", false, false, false, false, false, true);

            return power.Effects[IncludedEffects[0]].EffectType switch
            {
                Enums.eEffectType.Mez or Enums.eEffectType.MezResist => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].MezType})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                Enums.eEffectType.Enhancement when power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Mez
                    or Enums.eEffectType.MezResist => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].MezType})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({(vectors == "All" && power.Effects[IncludedEffects[0]].ETModifies == Enums.eEffectType.Mez ? "Mez" : vectors)})"),

                Enums.eEffectType.Enhancement when power.Effects[IncludedEffects[0]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].DamageType} {power.Effects[IncludedEffects[0]].ETModifies})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({vectors}{(power.Effects[IncludedEffects[^1]].ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity && vectors.Contains("All") ? $" {power.Effects[IncludedEffects[^1]].ETModifies}" : "")})"),

                Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect => baseEffectString.Replace(
                    $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].ETModifies})",
                    $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                Enums.eEffectType.Resistance or Enums.eEffectType.Defense or Enums.eEffectType.Elusivity
                    or Enums.eEffectType.DamageBuff => baseEffectString.Replace(
                        $"{power.Effects[IncludedEffects[0]].EffectType}({power.Effects[IncludedEffects[0]].DamageType})",
                        $"{power.Effects[IncludedEffects[0]].EffectType}({vectors})"),

                _ => baseEffectString
            };
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
        public static List<int> GetSimilarEffects(IPower power, fxIdentifier fxIdentifier, float mag, Enums.eSpecialCase specialCase = Enums.eSpecialCase.None, bool enhancementEffect = false)
        {
            return fxIdentifier.EffectType switch
            {
                // Keep Enhancement(Mez) / Enhancement(MezResist) separate from other Enhancement effects
                Enums.eEffectType.Enhancement when fxIdentifier.ETModifies is Enums.eEffectType.Mez or Enums.eEffectType.MezResist => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ETModifies == fxIdentifier.ETModifies && e.Value.ToWho == fxIdentifier.ToWho && Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon && e.Value.isEnhancementEffect == enhancementEffect)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.Enhancement => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ETModifies is not Enums.eEffectType.Mez or Enums.eEffectType.MezResist && e.Value.ToWho == fxIdentifier.ToWho && Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon && e.Value.isEnhancementEffect == enhancementEffect)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.MezResist or Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity or Enums.eEffectType.ResEffect => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho && Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon && e.Value.isEnhancementEffect == enhancementEffect)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.DamageBuff when specialCase == Enums.eSpecialCase.Defiance => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho && Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon && e.Value.SpecialCase == specialCase && e.Value.isEnhancementEffect == enhancementEffect)
                    .Select(e => e.Key)
                    .ToList(),

                Enums.eEffectType.DamageBuff => power.Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e => e.Value.EffectType == fxIdentifier.EffectType && e.Value.ToWho == fxIdentifier.ToWho && Math.Abs(e.Value.BuffedMag - mag) < float.Epsilon && e.Value.SpecialCase != Enums.eSpecialCase.Defiance && e.Value.isEnhancementEffect == enhancementEffect)
                    .Select(e => e.Key)
                    .ToList(),

                _ => new List<int>()
            };
        }
    }
}

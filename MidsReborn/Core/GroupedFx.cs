using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
    public static class ListExtGFx
    {
        public static bool ContainsAll<T>(this List<T> list, List<T> elements)
        {
            return elements.All(list.Contains);
        }

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

        public override string ToString()
        {
            return $"<fxIdentifier> {{{EffectType}, {ETModifies}, {MezType}, {DamageType}}}";
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
                MezType = effect.MezType
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

            var allDefenses = new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
                    DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.Toxic : Enums.eDamage.Psionic, Enums.eDamage.Melee,
                    Enums.eDamage.Ranged, Enums.eDamage.AoE
                };

            var positionDefenses = new List<Enums.eDamage>
                {
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                };

            var typedDefenses = new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic,
                    DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.Toxic : Enums.eDamage.Psionic
                };

            var allResistances = new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic
                };

            var allMez = new List<Enums.eMez>
                {
                    Enums.eMez.Immobilized, Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep,
                    Enums.eMez.Terrorized, Enums.eMez.Confused
                };

            var fxDamageTypes = fx.Select(e => e.DamageType).ToList();
            var fxMezTypes = fx.Select(e => e.MezType).ToList();
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

                _ => ""
            };

            return groupedVector != "" ? $"{fx[0].EffectType} ({groupedVector})" : $"{fx[0].EffectType}";
        }

        /// <summary>
        /// Generate tooltip for a grouped effect.
        /// </summary>
        /// <param name="power">Source power</param>
        /// <returns>Build effect string from each effect, then concatenate into a single string (one effect per line)</returns>
        public string GetTooltip(IPower power)
        {
            var fx = power.Effects
                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                .Where(e => IncludedEffects.Contains(e.Key))
                .Select(e => e.Value)
                .ToList();

            return string.Join("\r\n", fx.Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)));
        }
    }
}

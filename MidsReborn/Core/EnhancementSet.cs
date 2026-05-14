using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core
{
    public class EnhancementSet
    {
        public BonusItem[] Bonus = new BonusItem[5];
        public string Desc;
        public string DisplayName;
        public int[] Enhancements;
        public string Image;
        public int ImageIdx;
        public int LevelMax;
        public int LevelMin;
        //public Enums.eSetType SetType;
        public int SetType;
        public string ShortName;
        public BonusItem[] SpecialBonus = new BonusItem[6];
        public string Uid = string.Empty;

        public EnhancementSet()
        {
            DisplayName = string.Empty;
            ShortName = string.Empty;
            Desc = string.Empty;
            SetType = 0;
            Enhancements = new int[0];
            Image = string.Empty;
            InitBonus();
            InitBonusPvP();
            LevelMin = 0;
            LevelMax = 52;
        }

        public EnhancementSet(EnhancementSet iIOSet)
        {
            DisplayName = iIOSet.DisplayName;
            ShortName = iIOSet.ShortName;
            Desc = iIOSet.Desc;
            SetType = iIOSet.SetType;
            Image = iIOSet.Image;
            LevelMin = iIOSet.LevelMin;
            LevelMax = iIOSet.LevelMax;
            Enhancements = (int[]) iIOSet.Enhancements.Clone();
            Bonus = new BonusItem[iIOSet.Bonus.Length];
            for (var index = 0; index <= Bonus.Length - 1; ++index)
                Bonus[index].Assign(iIOSet.Bonus[index]);
            SpecialBonus = new BonusItem[iIOSet.SpecialBonus.Length];
            for (var index = 0; index <= SpecialBonus.Length - 1; ++index)
                SpecialBonus[index].Assign(iIOSet.SpecialBonus[index]);
            Uid = iIOSet.Uid;
        }

        public EnhancementSet(BinaryReader reader)
        {
            DisplayName = reader.ReadString();
            ShortName = reader.ReadString();
            Uid = reader.ReadString();
            Desc = reader.ReadString();

            SetType = reader.ReadInt32();

            Image = reader.ReadString();
            LevelMin = reader.ReadInt32();
            LevelMax = reader.ReadInt32();
            Enhancements = new int[reader.ReadInt32() + 1];
            for (var index = 0; index <= Enhancements.Length - 1; ++index)
                Enhancements[index] = reader.ReadInt32();
            InitBonus();
            InitBonusPvP();
            Bonus = new BonusItem[reader.ReadInt32() + 1];
            for (var index1 = 0; index1 < Bonus.Length; ++index1)
            {
                Bonus[index1].Special = reader.ReadInt32();
                Bonus[index1].AltString = reader.ReadString();
                Bonus[index1].PvMode = (Enums.ePvX) reader.ReadInt32();
                Bonus[index1].Slotted = reader.ReadInt32();
                Bonus[index1].Name = new string[reader.ReadInt32() + 1];
                Bonus[index1].Index = new int[Bonus[index1].Name.Length];
                for (var index2 = 0; index2 < Bonus[index1].Name.Length; ++index2)
                {
                    Bonus[index1].Name[index2] = reader.ReadString();
                    Bonus[index1].Index[index2] = reader.ReadInt32();
                }
            }

            SpecialBonus = new BonusItem[reader.ReadInt32() + 1];
            for (var index1 = 0; index1 < SpecialBonus.Length; ++index1)
            {
                SpecialBonus[index1].Special = reader.ReadInt32();
                SpecialBonus[index1].AltString = reader.ReadString();
                SpecialBonus[index1].Name = new string[reader.ReadInt32() + 1];
                SpecialBonus[index1].Index = new int[SpecialBonus[index1].Name.Length];
                for (var index2 = 0; index2 < SpecialBonus[index1].Name.Length; ++index2)
                {
                    SpecialBonus[index1].Name[index2] = reader.ReadString();
                    SpecialBonus[index1].Index[index2] = reader.ReadInt32();
                }
            }
        }

        public void InitBonus()
        {
            for (var index = 0; index <= Bonus.Length - 1; ++index)
            {
                Bonus[index].Special = -1;
                Bonus[index].AltString = string.Empty;
                Bonus[index].Name = Array.Empty<string>();
                Bonus[index].Index = Array.Empty<int>();
            }

            for (var index = 0; index <= SpecialBonus.Length - 1; ++index)
            {
                SpecialBonus[index].Special = -1;
                SpecialBonus[index].AltString = string.Empty;
                SpecialBonus[index].Name = Array.Empty<string>();
                SpecialBonus[index].Index = Array.Empty<int>();
            }
        }

        public void InitBonusPvP()
        {
            Array.Resize(ref Bonus, 11);
            for (var index = 0; index <= Bonus.Length - 1; ++index)
            {
                Bonus[index].Special = -1;
                Bonus[index].AltString = string.Empty;
                Bonus[index].Name = new string[0];
                Bonus[index].Index = new int[0];
            }

            for (var index = 0; index <= SpecialBonus.Length - 1; ++index)
            {
                //Array.Resize(ref SpecialBonus, 13);
                SpecialBonus[index].Special = -1;
                SpecialBonus[index].AltString = string.Empty;
                SpecialBonus[index].Name = new string[0];
                SpecialBonus[index].Index = new int[0];
            }
        }

        public string GetEnhancementSetRarity()
        {
            var setId = Enhancements
                .Where(enhIdx => enhIdx >= 0 && enhIdx < DatabaseAPI.Database.Enhancements.Length)
                .Select(enhIdx => DatabaseAPI.Database.Enhancements[enhIdx].nIDSet)
                .FirstOrDefault(candidateSetId => candidateSetId >= 0);

            return setId >= 0 && DatabaseAPI.TryGetEnhancementSetResolvedRarity(setId, out var rarity)
                ? rarity.ToString()
                : Recipe.RecipeRarity.Common.ToString();
        }

        public List<IEffect> GetEffectDetailedData(int index, bool special)
        {
            var ret = new List<IEffect>();
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 | index > bonusItemArray.Length - 1)
            {
                return ret;
            }

            for (var i = 0; i < bonusItemArray[index].Name.Length; i++)
            {
                if (bonusItemArray[index].Index[i] < 0) continue;
                if (bonusItemArray[index].Index[i] > DatabaseAPI.Database.Power.Length - 1) continue;

                var linkedPower = DatabaseAPI.Database.Power[bonusItemArray[index].Index[i]];
                ret.AddRange((IEnumerable<IEffect>) linkedPower.Effects.Clone());
            }

            return ret;
        }

        public Dictionary<string, List<IEffect>> GetEffectDetailedData2(int index, bool special)
        {
            var ret = new Dictionary<string, List<IEffect>>();
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 | index > bonusItemArray.Length - 1)
            {
                return ret;
            }

            for (var i = 0; i < bonusItemArray[index].Name.Length; i++)
            {
                if (bonusItemArray[index].Index[i] < 0) continue;
                if (bonusItemArray[index].Index[i] > DatabaseAPI.Database.Power.Length - 1) continue;

                var linkedPower = DatabaseAPI.Database.Power[bonusItemArray[index].Index[i]];
                if (!ret.ContainsKey(linkedPower.FullName))
                {
                    ret.Add(linkedPower.FullName, new List<IEffect>());
                }
                ret[linkedPower.FullName].AddRange((IEnumerable<IEffect>)linkedPower.Effects.Clone());
            }

            return ret;
        }

        public List<Power> GetEnhancementSetLinkedPowers(int index, bool special)
        {
            var ret = new List<Power>();
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 | index > bonusItemArray.Length - 1)
            {
                return ret;
            }

            for (var i = 0; i < bonusItemArray[index].Name.Length; i++)
            {
                if (bonusItemArray[index].Index[i] < 0) continue;
                if (bonusItemArray[index].Index[i] > DatabaseAPI.Database.Power.Length - 1) continue;

                var linkedPower = (Power) DatabaseAPI.Database.Power[bonusItemArray[index].Index[i]];
                ret.Add(linkedPower.Clone());
            }

            return ret;
        }

        public IPower? GetLinkedPower(int index, bool special)
        {
            IPower? power = null;
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 | index > bonusItemArray.Length - 1)
            {
                return power;
            }

            for (var i = 0; i < bonusItemArray[index].Name.Length; i++)
            {
                if (bonusItemArray[index].Index[i] < 0) continue;
                if (bonusItemArray[index].Index[i] > DatabaseAPI.Database.Power.Length - 1) continue;

                power = DatabaseAPI.Database.Power[bonusItemArray[index].Index[i]];
            }

            return power;
        }

        public bool HasPetSpecial
        {
            get
            {
                var isPetSet = DatabaseAPI.GetSetTypeByIndex(SetType).Name.Contains("Pet");
                var special = GetLinkedPower(Enhancements.Length - 1, true);
                return isPetSet && special != null;
            }
        }

        public IEnhancement? GetPetSpecialEnhancement()
        {
            IEnhancement? petSpecial = null;
            var lastEnh = Enhancements.Length - 1;
            var isPetSet = DatabaseAPI.GetSetTypeByIndex(SetType).Name.Contains("Pet");
            var special = GetLinkedPower(lastEnh, true);

            if (isPetSet && special != null)
            {
                petSpecial = DatabaseAPI.Database.Enhancements[Enhancements[lastEnh]];
            }
            return petSpecial;
        }

        public string GetEffectString(int index, bool special, bool longForm = false, bool fromPopup = false, bool bonusSection = false, bool status = false, List<Enums.eEffectType>? effectsFilter = null)
        {
            if (fromPopup && bonusSection)
            {
                return ExecuteWithPopupBonusFormattingContext(() =>
                    GetEffectStringCore(index, special, longForm, fromPopup, bonusSection, status, effectsFilter));
            }

            return GetEffectStringCore(index, special, longForm, fromPopup, bonusSection, status, effectsFilter);
        }

        private string GetEffectStringCore(int index, bool special, bool longForm, bool fromPopup, bool bonusSection, bool status, List<Enums.eEffectType>? effectsFilter)
        {
            if (!special && fromPopup && bonusSection && TryBuildLegacyPopupBonusEffectString(index, out var legacyPopupBonus))
            {
                return legacyPopupBonus;
            }

            BonusItem[] bonusItemArray;
            bonusItemArray = special ? SpecialBonus : Bonus;

            string str1;
            if (index < 0 | index > bonusItemArray.Length - 1)
            {
                str1 = string.Empty;
            }
            else if (!string.IsNullOrEmpty(bonusItemArray[index].AltString))
            {
                str1 = $"+{bonusItemArray[index].AltString}";
            }
            else
            {
                if (fromPopup && bonusSection)
                {
                    var groupedEffectString = GetGroupedBonusEffectString(bonusItemArray[index], effectsFilter);
                    if (!string.IsNullOrWhiteSpace(groupedEffectString))
                    {
                        return groupedEffectString;
                    }

                    if (!special)
                    {
                        var fallbackEffectString = GetFallbackBonusEffectString(bonusItemArray[index]);
                        if (!string.IsNullOrWhiteSpace(fallbackEffectString))
                        {
                            return fallbackEffectString;
                        }
                    }
                }

                var effectList = new List<string>();
                for (var index1 = 0; index1 < bonusItemArray[index].Name.Length; index1++)
                {
                    if (bonusItemArray[index].Index[index1] < 0 | bonusItemArray[index].Index[index1] > DatabaseAPI.Database.Power.Length - 1)
                    {
                        return string.Empty;
                    }

                    var power = OmniPowerRouting.CreateDisplayPower(DatabaseAPI.Database.Power[bonusItemArray[index].Index[index1]]);
                    var empty2 = string.Empty;
                    var returnMask = Array.Empty<int>();
                    power.GetEffectStringGrouped(0, ref empty2, ref returnMask, !longForm, true, false, fromPopup, true);
                    if (!string.IsNullOrEmpty(empty2))
                    {
                        effectList.Add(empty2);
                    }

                    var fxFilter = effectsFilter ?? new List<Enums.eEffectType>{ Enums.eEffectType.Null, Enums.eEffectType.NullBool, Enums.eEffectType.DesignerStatus };
                    for (var index2 = 0; index2 < power.Effects.Length; index2++)
                    {
                        if (fxFilter.Contains(power.Effects[index2].EffectType))
                        {
                            continue;
                        }

                        var flag = false;
                        foreach (var m in returnMask)
                        {
                            if (index2 == m)
                            {
                                flag = true;
                            }
                        }

                        if (flag)
                        {
                            continue;
                        }

                        var str2 = longForm
                            ? power.Effects[index2].BuildEffectString(true, "", false, false, false, fromPopup, false, false, true)
                            : power.Effects[index2].BuildEffectStringShort(false, true);
                        

                        if (effectList.Any(s => s == str2)) continue;
                        
                        if (str2.Contains("EndRec"))
                        {
                            str2 = str2.Replace("EndRec", "Recovery");
                        }

                        effectList.Add(str2);
                    }
                }

                str1 = string.Join(", ", effectList.ToArray());
                if (bonusSection && !status)
                {
                    Utilities.ModifiedEffectString(ref str1, 1);
                }
                else
                {
                    Utilities.ModifiedEffectString(ref str1, 2);
                }

                if (!special && fromPopup && bonusSection && string.IsNullOrWhiteSpace(str1))
                {
                    var groupedEffectString = GetGroupedBonusEffectString(bonusItemArray[index], effectsFilter);
                    if (!string.IsNullOrWhiteSpace(groupedEffectString))
                    {
                        return groupedEffectString;
                    }

                    var fallbackEffectString = GetFallbackBonusEffectString(bonusItemArray[index]);
                    if (!string.IsNullOrWhiteSpace(fallbackEffectString))
                    {
                        return fallbackEffectString;
                    }
                }
            }

            return str1;
        }

        private bool TryBuildLegacyPopupBonusEffectString(int index, out string effectString)
        {
            effectString = string.Empty;
            if (index < 0 || index >= Bonus.Length)
            {
                return false;
            }

            var bonusItem = Bonus[index];
            var powerIndexes = bonusItem.Index
                .Where(powerIndex => powerIndex >= 0 && powerIndex < DatabaseAPI.Database.Power.Length)
                .ToArray();
            if (powerIndexes.Length == 0)
            {
                return false;
            }

            var parts = new List<string>(powerIndexes.Length);
            foreach (var powerIndex in powerIndexes)
            {
                var part = TryBuildLegacyPopupBonusPowerString(OmniPowerRouting.CreateDisplayPower(DatabaseAPI.Database.Power[powerIndex]));
                if (string.IsNullOrWhiteSpace(part))
                {
                    return false;
                }

                if (!parts.Contains(part, StringComparer.Ordinal))
                {
                    parts.Add(part);
                }
            }

            if (parts.Count == 0)
            {
                return false;
            }

            effectString = string.Join(", ", parts);
            return true;
        }

        private static string? GetGroupedBonusEffectString(BonusItem bonusItem, List<Enums.eEffectType>? effectsFilter)
        {
            var fxFilter = effectsFilter ?? new List<Enums.eEffectType>
            {
                Enums.eEffectType.Null,
                Enums.eEffectType.NullBool,
                Enums.eEffectType.DesignerStatus
            };

            var effectList = new List<string>();
            foreach (var powerIndex in bonusItem.Index)
            {
                if (powerIndex < 0 | powerIndex > DatabaseAPI.Database.Power.Length - 1)
                {
                    return string.Empty;
                }

                var resolvedPower = PlannerEffectResolver.ResolvePower(new Power(DatabaseAPI.Database.Power[powerIndex])).ResolvedPower;
                var power = OmniPowerRouting.CreateDisplayPower(resolvedPower);
                var groupedEffects = GroupedFx.BuildPopupTooltipText(
                        power,
                        groupFilter: (_, effect) => !fxFilter.Contains(effect.EffectType) &&
                                                    effect.EffectClass != Enums.eEffectClass.Ignored &&
                                                    effect.EffectType != Enums.eEffectType.GrantPower)
                    .Replace("\r\n", ", ")
                    .Replace("\n", ", ")
                    .Replace("EndRec", "Recovery");

                if (!string.IsNullOrWhiteSpace(groupedEffects))
                {
                    effectList.Add(groupedEffects);
                }
            }

            var ret = string.Join(", ", effectList.Distinct(StringComparer.Ordinal));
            ret = Regex.Replace(ret, @"Knockback \(Mag -(?<mag>[\d.]+)\), Knockup \(Mag -\k<mag>\)", "Knockback Protection (Mag ${mag})");

            return ret;
        }

        private static string? TryBuildLegacyPopupBonusPowerString(IPower power)
        {
            var effects = power.Effects
                .Where(effect => effect is { EffectType: not Enums.eEffectType.NullBool and not Enums.eEffectType.DesignerStatus } &&
                                 !effect.Absorbed_Effect)
                .ToArray();
            if (effects.Length == 0)
            {
                return null;
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.Regeneration))
            {
                var effect = effects.First(effect => effect.EffectType == Enums.eEffectType.Regeneration);
                var percent = GetLegacyPopupMagnitude(effect);
                var hpPerSecond = DisplayValueFormatter.FormatRate(DatabaseAPI.GetClassHitPoints() / 100f * ((percent / 100f) * DatabaseAPI.GetClassBaseRegen() * Statistics.BaseMagic));
                return $"{DisplayValueFormatter.FormatPercentValue(percent)}% ({hpPerSecond} HP/sec) Regeneration";
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.HitPoints))
            {
                var effect = effects.First(effect => effect.EffectType == Enums.eEffectType.HitPoints);
                var percent = effect.Aspect == Enums.eAspect.Max
                    ? GetLegacyPopupMaximumHitPointsPercent(effect)
                    : GetLegacyPopupMagnitude(effect, 0.1f);
                var rawHp = DisplayValueFormatter.FormatNumber(DatabaseAPI.GetClassHitPoints() * (percent / 100f));
                return $"{rawHp} HP ({DisplayValueFormatter.FormatPercentValue(percent)}%) HitPoints";
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.Accuracy))
            {
                var effect = effects.First(effect => effect.EffectType == Enums.eEffectType.Accuracy);
                return $"+{DisplayValueFormatter.FormatPercentValue(GetLegacyPopupMagnitude(effect))}% Enhancement(Accuracy)";
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.RechargeTime))
            {
                var effect = effects.First(effect => effect.EffectType == Enums.eEffectType.RechargeTime);
                return $"+{DisplayValueFormatter.FormatPercentValue(GetLegacyPopupMagnitude(effect))}% Enhancement(RechargeTime)";
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.DamageBuff))
            {
                var typedEffects = effects.Where(effect => effect.EffectType == Enums.eEffectType.DamageBuff).ToArray();
                if (typedEffects.Length > 0)
                {
                    var percent = GetLegacyPopupMagnitude(typedEffects[0]);
                    var types = GroupDamageTypes(typedEffects.Select(effect => effect.DamageType));
                    return $"{DisplayValueFormatter.FormatPercentValue(percent)}% DamageBuff({types})";
                }
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.Resistance))
            {
                var typedEffects = effects
                    .Where(effect => effect.EffectType == Enums.eEffectType.Resistance &&
                                     effect.MezType == Enums.eMez.None &&
                                     effect.DamageType != Enums.eDamage.None)
                    .ToArray();
                var mezEffects = effects
                    .Where(effect => (effect.EffectType == Enums.eEffectType.Resistance ||
                                      effect.EffectType == Enums.eEffectType.MezResist) &&
                                     effect.MezType != Enums.eMez.None)
                    .ToArray();
                var statusEffects = effects
                    .Where(effect => effect.EffectType == Enums.eEffectType.Resistance &&
                                     effect.MezType == Enums.eMez.None &&
                                     effect.DamageType == Enums.eDamage.None)
                    .ToArray();

                var parts = new List<string>();
                if (typedEffects.Length > 0)
                {
                    var percent = GetLegacyPopupMagnitude(typedEffects[0]);
                    parts.Add($"{DisplayValueFormatter.FormatPercentValue(percent)}% Resistance({GroupDamageTypes(typedEffects.Select(effect => effect.DamageType))})");
                }

                if (mezEffects.Length > 0)
                {
                    var percent = GetLegacyPopupMagnitude(mezEffects[0]);
                    parts.Add($"{DisplayValueFormatter.FormatPercentValue(percent)}% MezResist(All)");
                }
                else if (statusEffects.Length > 0)
                {
                    var percent = GetLegacyPopupMagnitude(statusEffects[0]);
                    parts.Add($"{DisplayValueFormatter.FormatPercentValue(percent)}% Resistance(None)");
                }

                if (parts.Count > 0)
                {
                    return string.Join(", ", parts);
                }
            }

            if (effects.Any(effect => effect.EffectType == Enums.eEffectType.None))
            {
                var effect = effects.First(effect => effect.EffectType == Enums.eEffectType.None);
                var effectLabel = InferFallbackEffectLabel(power);
                if (!string.IsNullOrWhiteSpace(effectLabel))
                {
                    var percent = GetLegacyPopupMagnitude(effect);
                    var typeSuffix = effect.DamageType == Enums.eDamage.None
                        ? string.Empty
                        : $"({Enum.GetName(typeof(Enums.eDamage), effect.DamageType)})";
                    return $"{DisplayValueFormatter.FormatPercentValue(percent)}% {effectLabel}{typeSuffix}";
                }
            }

            return null;
        }

        private static float GetLegacyPopupMaximumHitPointsPercent(IEffect effect)
        {
            // Omni maximum-HP set bonuses now surface their self-target
            // activation effects for planner math, but popup parity should use
            // the exact percentage encoded by the template scale.
            return effect.Scale * effect.nMagnitude * 10f;
        }

        private static float GetLegacyPopupMagnitude(IEffect effect, float magnitudeMultiplier = 1f)
        {
            var magnitude = Math.Abs(effect.BuffedMag) > float.Epsilon
                ? effect.BuffedMag
                : effect.Scale * effect.nMagnitude;
            return magnitude * (effect.DisplayPercentage ? 100f : 1f) * magnitudeMultiplier;
        }

        private static string GroupDamageTypes(IEnumerable<Enums.eDamage> damageTypes)
        {
            var types = damageTypes
                .Where(type => type != Enums.eDamage.None)
                .Distinct()
                .OrderBy(GetDamageDisplayOrder)
                .ToArray();
            if (types.Length == 0)
            {
                return "None";
            }

            var allPlayerDamageTypes = new[]
            {
                Enums.eDamage.Smashing,
                Enums.eDamage.Lethal,
                Enums.eDamage.Fire,
                Enums.eDamage.Cold,
                Enums.eDamage.Energy,
                Enums.eDamage.Negative,
                Enums.eDamage.Toxic,
                Enums.eDamage.Psionic
            };

            if (allPlayerDamageTypes.All(types.Contains))
            {
                return "All";
            }

            return string.Join(",", types.Select(type => Enum.GetName(typeof(Enums.eDamage), type)));
        }

        private static int GetDamageDisplayOrder(Enums.eDamage damageType)
        {
            return damageType switch
            {
                Enums.eDamage.Smashing => 0,
                Enums.eDamage.Lethal => 1,
                Enums.eDamage.Fire => 2,
                Enums.eDamage.Cold => 3,
                Enums.eDamage.Energy => 4,
                Enums.eDamage.Negative => 5,
                Enums.eDamage.Toxic => 6,
                Enums.eDamage.Psionic => 7,
                _ => 100 + (int)damageType
            };
        }

        private static string? GetFallbackBonusEffectString(BonusItem bonusItem)
        {
            var effectLines = bonusItem.Index
                .Where(powerIndex => powerIndex >= 0 && powerIndex < DatabaseAPI.Database.Power.Length)
                .Select(powerIndex => TryBuildFallbackEffectString(DatabaseAPI.Database.Power[powerIndex]))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            return effectLines.Length == 0 ? null : string.Join(", ", effectLines!);
        }

        private static string? TryBuildFallbackEffectString(IPower power)
        {
            foreach (var effect in power.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.None || !string.IsNullOrWhiteSpace(effect.Special))
                {
                    continue;
                }

                var effectLabel = InferFallbackEffectLabel(power);
                if (string.IsNullOrWhiteSpace(effectLabel))
                {
                    continue;
                }

                var magnitudeValue = GetLegacyPopupMagnitude(effect);
                var magnitude = $"{DisplayValueFormatter.FormatPercentValue(magnitudeValue)}%";
                var typeSuffix = effect.DamageType == Enums.eDamage.None
                    ? string.Empty
                    : $"({Enum.GetName(typeof(Enums.eDamage), effect.DamageType)})";
                return $"{magnitude} {effectLabel}{typeSuffix}";
            }

            return null;
        }

        private static string? InferFallbackEffectLabel(IPower power)
        {
            var lookup = $"{power.FullName} {power.DisplayName}";
            if (lookup.Contains("Defense", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(lookup, @"(?:^|[_\.])Def(?:[_\.]|$)", RegexOptions.IgnoreCase))
            {
                return "Defense";
            }

            if (lookup.Contains("Resistance", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(lookup, @"(?:^|[_\.])Res(?:[_\.]|$)", RegexOptions.IgnoreCase))
            {
                return "Resistance";
            }

            if (lookup.Contains("Recharge", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(lookup, @"(?:^|[_\.])Rech(?:[_\.]|$)", RegexOptions.IgnoreCase))
            {
                return "RechargeTime";
            }

            if (lookup.Contains("Accuracy", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(lookup, @"(?:^|[_\.])Acc(?:[_\.]|$)", RegexOptions.IgnoreCase))
            {
                return "Accuracy";
            }

            if (lookup.Contains("ToHit", StringComparison.OrdinalIgnoreCase))
            {
                return "ToHit";
            }

            if (lookup.Contains("Damage", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(lookup, @"(?:^|[_\.])Dam(?:[_\.]|$)", RegexOptions.IgnoreCase))
            {
                return "DamageBuff";
            }

            return null;
        }

        private static T ExecuteWithPopupBonusFormattingContext<T>(Func<T> callback)
        {
            var savedCharacter = MidsContext.Character;
            if (savedCharacter == null)
            {
                return callback();
            }

            var savedBuild = MidsContext.Build;
            var savedArchetype = MidsContext.Archetype;
            try
            {
                if (savedCharacter.Archetype != null)
                {
                    MidsContext.Archetype = savedCharacter.Archetype;
                }

                MidsContext.Character = null;
                MidsContext.Build = null;
                return callback();
            }
            finally
            {
                MidsContext.Character = savedCharacter;
                MidsContext.Build = savedBuild;
                MidsContext.Archetype = savedArchetype;
            }
        }

        public void StoreTo(BinaryWriter writer)
        {
            writer.Write(DisplayName);
            writer.Write(ShortName);
            writer.Write(Uid);
            writer.Write(Desc);

            writer.Write(SetType);

            writer.Write(Image);
            writer.Write(LevelMin);
            writer.Write(LevelMax);
            writer.Write(Enhancements.Length - 1);
            for (var index = 0; index <= Enhancements.Length - 1; ++index)
                writer.Write(Enhancements[index]);
            writer.Write(Bonus.Length - 1);
            for (var index1 = 0; index1 <= Bonus.Length - 1; ++index1)
            {
                writer.Write(Bonus[index1].Special);
                writer.Write(Bonus[index1].AltString);
                writer.Write((int) Bonus[index1].PvMode);
                writer.Write(Bonus[index1].Slotted);
                writer.Write(Bonus[index1].Name.Length - 1);
                for (var index2 = 0; index2 <= Bonus[index1].Name.Length - 1; ++index2)
                {
                    writer.Write(Bonus[index1].Name[index2]);
                    writer.Write(Bonus[index1].Index[index2]);
                }
            }

            writer.Write(SpecialBonus.Length - 1);
            for (var index1 = 0; index1 <= SpecialBonus.Length - 1; ++index1)
            {
                writer.Write(SpecialBonus[index1].Special);
                writer.Write(SpecialBonus[index1].AltString);
                writer.Write(SpecialBonus[index1].Name.Length - 1);
                for (var index2 = 0; index2 <= SpecialBonus[index1].Name.Length - 1; ++index2)
                {
                    writer.Write(SpecialBonus[index1].Name[index2]);
                    writer.Write(SpecialBonus[index1].Index[index2]);
                }
            }
        }

        private static string GenerateShortName(string displayName)
        {
            var strArray = displayName.Split(' ');
            var stringBuilder = new StringBuilder();
            foreach (var str1 in strArray)
            {
                var str2 = str1;
                if (str2.Length > 4)
                    str2 = str2.Replace("a", string.Empty).Replace("e", string.Empty).Replace("i", string.Empty)
                        .Replace("o", string.Empty).Replace("u", string.Empty);
                if (string.IsNullOrEmpty(str2))
                    str2 = str1;
                stringBuilder.Append(str2.Length > 3 ? str2.Substring(0, 3) : str2);
            }

            return stringBuilder.Length > 9 ? stringBuilder.ToString().Substring(0, 9) : stringBuilder.ToString();
        }

        public struct BonusItem
        {
            public int Special;
            public string[] Name;
            public int[] Index;
            public string AltString;
            public Enums.ePvX PvMode;
            public int Slotted;

            public void Assign(BonusItem iBi)
            {
                Special = iBi.Special;
                AltString = iBi.AltString;
                Name = new string[iBi.Name.Length];
                Index = new int[iBi.Index.Length];
                Array.Copy(iBi.Name, Name, iBi.Name.Length);
                Array.Copy(iBi.Index, Index, iBi.Index.Length);
                PvMode = iBi.PvMode;
                Slotted = iBi.Slotted;
            }
        }
    }
}

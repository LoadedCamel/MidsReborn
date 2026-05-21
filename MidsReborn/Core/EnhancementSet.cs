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

        public Enums.ePvX GetEffectiveBonusPvMode(int index, bool special = false)
        {
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 || index >= bonusItemArray.Length)
            {
                return Enums.ePvX.Any;
            }

            return ResolveEffectiveBonusPvMode(bonusItemArray[index], special);
        }

        public bool BonusAppliesInContext(int index, bool special, bool pvpContext)
        {
            var pvMode = GetEffectiveBonusPvMode(index, special);
            return pvMode == Enums.ePvX.Any ||
                   (pvMode == Enums.ePvX.PvP && pvpContext) ||
                   (pvMode == Enums.ePvX.PvE && !pvpContext);
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

        public IReadOnlyList<string> GetPopupEffectStrings(int index, bool special, bool longForm = false, bool status = false, List<Enums.eEffectType>? effectsFilter = null)
        {
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 || index >= bonusItemArray.Length)
            {
                return Array.Empty<string>();
            }

            var bonusItem = bonusItemArray[index];
            if (!string.IsNullOrWhiteSpace(bonusItem.AltString))
            {
                return [GroupedFx.FormatPresentationText($"+{bonusItem.AltString}")];
            }

            var effectLines = BuildBonusEffectLines(bonusItem, longForm, effectsFilter);
            if (effectLines.Count > 0)
            {
                return effectLines;
            }

            var effectString = GetEffectString(index, special, longForm, true, true, status, effectsFilter);
            return string.IsNullOrWhiteSpace(effectString) ? Array.Empty<string>() : [effectString];
        }

        private string GetEffectStringCore(int index, bool special, bool longForm, bool fromPopup, bool bonusSection, bool status, List<Enums.eEffectType>? effectsFilter)
        {
            var bonusItemArray = special ? SpecialBonus : Bonus;
            if (index < 0 || index >= bonusItemArray.Length)
            {
                return string.Empty;
            }

            var bonusItem = bonusItemArray[index];
            if (!string.IsNullOrWhiteSpace(bonusItem.AltString))
            {
                return GroupedFx.FormatPresentationText($"+{bonusItem.AltString}");
            }

            var effectLines = BuildBonusEffectLines(bonusItem, longForm, effectsFilter);
            if (effectLines.Count > 0)
            {
                return string.Join(", ", effectLines);
            }

            var fallbackEffectString = GetFallbackBonusEffectString(bonusItem);
            return string.IsNullOrWhiteSpace(fallbackEffectString)
                ? string.Empty
                : GroupedFx.FormatPresentationText(fallbackEffectString);
        }

        private static IReadOnlyList<string> BuildBonusEffectLines(BonusItem bonusItem, bool longForm, List<Enums.eEffectType>? effectsFilter)
        {
            return ExecuteWithPopupBonusFormattingContext(() =>
            {
                var lines = new List<string>();
                foreach (var powerIndex in bonusItem.Index.Where(powerIndex => powerIndex >= 0 && powerIndex < DatabaseAPI.Database.Power.Length))
                {
                    var line = BuildBonusPowerLine(powerIndex, longForm, effectsFilter);
                    if (!string.IsNullOrWhiteSpace(line) && !lines.Contains(line, StringComparer.Ordinal))
                    {
                        lines.Add(line);
                    }
                }

                return (IReadOnlyList<string>)lines;
            });
        }

        private static string? BuildBonusPowerLine(int powerIndex, bool longForm, List<Enums.eEffectType>? effectsFilter)
        {
            if (powerIndex < 0 || powerIndex >= DatabaseAPI.Database.Power.Length)
            {
                return null;
            }

            var power = Omni.OmniPowerRouting.CreateDisplayPower(
                PlannerEffectResolver.ResolvePower(new Power(DatabaseAPI.Database.Power[powerIndex])).ResolvedPower);

            var groupedLines = GroupedFx.BuildPopupTooltipLines(
                power,
                groupFilter: BuildBonusGroupFilter(effectsFilter),
                lineJoinMode: GroupedFx.GroupedFxLineJoinMode.MultiLine);
            var groupedText = string.Join(", ", groupedLines);

            if (!string.IsNullOrWhiteSpace(groupedText))
            {
                return groupedText;
            }

            return TryBuildFallbackEffectString(DatabaseAPI.Database.Power[powerIndex]);
        }

        private static Func<GroupedFx, IEffect, bool> BuildBonusGroupFilter(List<Enums.eEffectType>? effectsFilter)
        {
            var fxFilter = effectsFilter ?? new List<Enums.eEffectType>
            {
                Enums.eEffectType.Null,
                Enums.eEffectType.NullBool,
                Enums.eEffectType.DesignerStatus
            };

            return (_, effect) => !fxFilter.Contains(effect.EffectType) &&
                                  effect.EffectClass != Enums.eEffectClass.Ignored &&
                                  effect.EffectType != Enums.eEffectType.GrantPower;
        }

        private static float GetLegacyPopupMagnitude(IEffect effect, float magnitudeMultiplier = 1f)
        {
            var magnitude = Math.Abs(effect.BuffedMag) > float.Epsilon
                ? effect.BuffedMag
                : effect.Scale * effect.nMagnitude;
            return magnitude * (effect.DisplayPercentage ? 100f : 1f) * magnitudeMultiplier;
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
                return GroupedFx.FormatPresentationText($"{magnitude} {effectLabel}{typeSuffix}");
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

        private static Enums.ePvX ResolveEffectiveBonusPvMode(BonusItem bonusItem, bool special)
        {
            if (!special && bonusItem.PvMode != Enums.ePvX.Any)
            {
                return bonusItem.PvMode;
            }

            var sawPvpOnlyPower = false;
            var sawNonPvpPower = false;

            foreach (var powerIndex in bonusItem.Index)
            {
                var linkedPowerPvMode = InferLinkedPowerPvMode(powerIndex);
                if (linkedPowerPvMode == Enums.ePvX.PvP)
                {
                    sawPvpOnlyPower = true;
                }
                else
                {
                    sawNonPvpPower = true;
                }
            }

            if (sawPvpOnlyPower && !sawNonPvpPower)
            {
                return Enums.ePvX.PvP;
            }

            return special ? Enums.ePvX.Any : bonusItem.PvMode;
        }

        private static Enums.ePvX InferLinkedPowerPvMode(int powerIndex)
        {
            if (powerIndex < 0 || powerIndex >= DatabaseAPI.Database.Power.Length)
            {
                return Enums.ePvX.Any;
            }

            var linkedPower = DatabaseAPI.Database.Power[powerIndex];
            var fullSetName = linkedPower.FullSetName ?? string.Empty;
            var fullName = linkedPower.FullName ?? string.Empty;

            if (string.Equals(fullSetName, "Set_Bonus.PVP_Set_Bonus", StringComparison.OrdinalIgnoreCase) ||
                fullName.StartsWith("Set_Bonus.PVP_Set_Bonus.", StringComparison.OrdinalIgnoreCase))
            {
                return Enums.ePvX.PvP;
            }

            return Enums.ePvX.Any;
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

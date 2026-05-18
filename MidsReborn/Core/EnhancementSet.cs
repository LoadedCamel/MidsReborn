using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;

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
            var enhIdx = Enhancements[0];
            var recipeIdx = DatabaseAPI.Database.Enhancements[enhIdx].RecipeIDX;
            return DatabaseAPI.Database.Recipes[recipeIdx].Rarity.ToString();
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
            var bonusItemArray = special ? SpecialBonus : Bonus;

            if ((index < 0) | (index > bonusItemArray.Length - 1))
            {
                return string.Empty;
            }
            
            if (!string.IsNullOrEmpty(bonusItemArray[index].AltString))
            {
                return $"+{bonusItemArray[index].AltString}".Replace("  ", " ");
            }

            var fxFilter = effectsFilter ?? [Enums.eEffectType.Null, Enums.eEffectType.NullBool, Enums.eEffectType.DesignerStatus];
            var p = new Power();
            var fxList = new List<IEffect>();
            foreach (var bpIdx in bonusItemArray[index].Index)
            {
                if ((bpIdx < 0) | (bpIdx >= DatabaseAPI.Database.Power.Length))
                {
                    continue;
                }

                // Bug: will backreference DatabaseAPI.Database.Power[bpIdx],
                // and keep updating it on each call if not Cloned.
                var bp = DatabaseAPI.Database.Power[bpIdx].Clone();
                if (bp == null)
                {
                    continue;
                }

                var subFxArr = bp.Effects.Where(e => !fxFilter.Contains(e.EffectType)).ToArray();
                foreach (var fx in subFxArr)
                {
                    var similarFxIdx = fxList.TryFindIndex(e => e.EffectType == fx.EffectType &&
                                                                   e.MezType == fx.MezType &&
                                                                   e.DamageType == fx.DamageType &&
                                                                   e.ETModifies == fx.ETModifies &&
                                                                   e.ToWho == fx.ToWho &&
                                                                   e.nSummon == fx.nSummon &&
                                                                   Math.Abs(e.nDuration - fx.nDuration) < float.Epsilon &&
                                                                   e.Buffable == fx.Buffable &&
                                                                   e.Stacking == fx.Stacking &&
                                                                   e.IgnoreED == fx.IgnoreED &&
                                                                   e.IgnoreScaling == fx.IgnoreScaling &&
                                                                   e.SpecialCase == fx.SpecialCase);

                    if (similarFxIdx >= 0)
                    {
                        fxList[similarFxIdx].Scale += fxList[similarFxIdx].AttribType == Enums.eAttribType.Duration
                            ? fx.Scale
                            : fx.Scale * fx.nMagnitude;

                        continue;
                    }

                    fxList.Add(fx);
                }
            }

            p.Effects = fxList.ToArray();
            var gre = GroupedFx.AssembleGroupedEffects(p, true);

            return string.Join(", ", gre.Select(e => e.GetTooltip(p, true)));
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
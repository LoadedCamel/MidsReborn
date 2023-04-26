using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Core
{
    public class I9Slot : ICloneable
    {
        private const float SuperiorMult = 1.25f;

        public int Enh;
        public Enums.eEnhGrade Grade;
        public int IOLevel;
        public Enums.eEnhRelative RelativeLevel;
        public bool Obtained;

        public I9Slot()
        {
            Enh = -1;
            RelativeLevel = Enums.eEnhRelative.Even;
            Grade = Enums.eEnhGrade.None;
            IOLevel = 1;
            Obtained = false;
        }

        public object Clone()
        {
            return new I9Slot
            {
                Enh = Enh,
                Grade = Grade,
                IOLevel = IOLevel,
                RelativeLevel = RelativeLevel,
                Obtained = Obtained
            };
        }

        public float GetEnhancementEffect(Enums.eEnhance iEffect, int subEnh, float mag)
        {
            if (Enh < 0)
            {
                return 0.0f;
            }

            var num2 = 0.0f;
            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            foreach (var sEffect in enhancement.Effect)
            {
                if (sEffect.Mode != Enums.eEffMode.Enhancement || sEffect.BuffMode == Enums.eBuffDebuff.DeBuffOnly && !(mag <= 0.0) || sEffect.BuffMode == Enums.eBuffDebuff.BuffOnly && !(mag >= 0.0) || sEffect.Schedule == Enums.eSchedule.None || (Enums.eEnhance) sEffect.Enhance.ID != iEffect || subEnh >= 0 && subEnh != sEffect.Enhance.SubID)
                {
                    continue;
                }

                var scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
                if (Math.Abs(sEffect.Multiplier) > 0.01)
                {
                    scheduleMult *= sEffect.Multiplier;
                }

                num2 += scheduleMult;
            }

            return num2;
        }

        private float GetScheduleMult(Enums.eType iType, Enums.eSchedule iSched)
        {
            if (Grade < Enums.eEnhGrade.None)
            {
                Grade = Enums.eEnhGrade.None;
            }

            if (RelativeLevel < Enums.eEnhRelative.None)
            {
                RelativeLevel = Enums.eEnhRelative.None;
            }

            if (Grade > Enums.eEnhGrade.SingleO)
            {
                Grade = Enums.eEnhGrade.SingleO;
            }

            if (RelativeLevel > Enums.eEnhRelative.PlusFive)
            {
                RelativeLevel = Enums.eEnhRelative.PlusFive;
            }

            var num1 = 0.0f;
            if (IOLevel <= 0)
            {
                IOLevel = 0;
            }

            if (IOLevel > DatabaseAPI.Database.MultIO.Length - 1)
            {
                IOLevel = DatabaseAPI.Database.MultIO.Length - 1;
            }

            if (iSched == Enums.eSchedule.None || iSched == Enums.eSchedule.Multiple)
            {
                num1 = 0.0f;
            }
            else
            {
                switch (iType)
                {
                    case Enums.eType.Normal:
                        num1 = Grade switch
                        {
                            Enums.eEnhGrade.None => 0.0f,
                            Enums.eEnhGrade.TrainingO => DatabaseAPI.Database.MultTO[0][(int)iSched],
                            Enums.eEnhGrade.DualO => DatabaseAPI.Database.MultDO[0][(int)iSched],
                            Enums.eEnhGrade.SingleO => DatabaseAPI.Database.MultSO[0][(int)iSched],
                            _ => num1
                        };
                        break;
                    case Enums.eType.InventO:
                        num1 = DatabaseAPI.Database.MultIO[IOLevel][(int)iSched];
                        break;
                    case Enums.eType.SpecialO:
                        num1 = DatabaseAPI.Database.MultSO[0][(int)iSched];
                        break;
                    case Enums.eType.SetO:
                        num1 = DatabaseAPI.Database.MultIO[IOLevel][(int)iSched];
                        break;
                }
            }

            var num2 = num1 * GetRelativeLevelMultiplier();
            if (Enh > -1 && DatabaseAPI.Database.Enhancements[Enh].Superior)
            {
                num2 *= 1.25f;
            }

            return num2;
        }

        private float GetRelativeLevelMultiplier()

        {
            float num1;
            if (RelativeLevel == Enums.eEnhRelative.None)
            {
                num1 = 0.0f;
            }
            else
            {
                var num2 = (int) (RelativeLevel - 4);
                num1 = num2 >= 0 ? (float) (num2 * 0.0500000007450581 + 1.0) : (float) (1.0 + num2 * 0.100000001490116);
            }

            return num1;
        }

        public string GetEnhancementString()
        {
            string str1;
            if (Enh < 0)
            {
                str1 = string.Empty;
            }
            else
            {
                var enhancement = DatabaseAPI.Database.Enhancements[Enh];
                if (enhancement.Effect.Length == 0)
                {
                    str1 = enhancement.Desc;
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    var flag = false;
                    var effect = enhancement.Effect;
                    var index1 = 0;
                    if (index1 >= effect.Length)
                    {
                        str1 = stringBuilder.ToString();
                    }
                    else
                    {
                        var sEffect = effect[index1];
                        if (sEffect.Mode == Enums.eEffMode.FX)
                            flag = true;
                        string str2;
                        if (sEffect.Mode == Enums.eEffMode.Enhancement && sEffect.Schedule != Enums.eSchedule.None)
                        {
                            var scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
                            if (sEffect.Multiplier > 0.0)
                            {
                                scheduleMult *= sEffect.Multiplier;
                            }

                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append(", ");
                            }

                            switch (enhancement.TypeID)
                            {
                                case Enums.eType.Normal:
                                    var relativeString1 = Enums.GetRelativeString(RelativeLevel, false);
                                    if (!string.IsNullOrEmpty(relativeString1) & (relativeString1 != "X"))
                                    {
                                        stringBuilder.Append(relativeString1 + " " + DatabaseAPI.Database.EnhGradeStringLong[(int) Grade] + " - ");
                                        break;
                                    }

                                    if (relativeString1 == "X")
                                    {
                                        stringBuilder.Append("Disabled " + DatabaseAPI.Database.EnhGradeStringLong[(int) Grade] + " - ");
                                        break;
                                    }

                                    stringBuilder.Append(DatabaseAPI.Database.EnhGradeStringLong[(int) Grade] + " - ");
                                    break;
                                case Enums.eType.SpecialO:
                                    var relativeString2 = Enums.GetRelativeString(RelativeLevel, false);
                                    if (!string.IsNullOrEmpty(relativeString2) & (relativeString2 != "X"))
                                    {
                                        stringBuilder.Append(relativeString2 + " " + enhancement.GetSpecialName() + " - ");
                                        break;
                                    }

                                    if (relativeString2 == "X")
                                    {
                                        stringBuilder.Append("Disabled " + enhancement.GetSpecialName() + " - ");
                                        break;
                                    }

                                    stringBuilder.Append(enhancement.GetSpecialName() + " - ");
                                    break;
                            }

                            stringBuilder.Append("Schedule: ");
                            stringBuilder.Append($"{sEffect.Schedule}");
                            stringBuilder.Append($" ({scheduleMult * 100:##0.###}%)");
                            str2 = stringBuilder.ToString();
                        }
                        else if (!flag)
                        {
                            str2 = stringBuilder.ToString();
                        }
                        else
                        {
                            var power = enhancement.GetPower();
                            for (var index2 = 0; index2 <= power.Effects.Length - 1; ++index2)
                            {
                                if (stringBuilder.Length > 0)
                                {
                                    stringBuilder.Append(", ");
                                }

                                stringBuilder.Append(power.Effects[index2].BuildEffectString(true));
                            }

                            str2 = "Effect: " + stringBuilder;
                        }

                        str1 = str2;
                    }
                }
            }

            return str1;
        }

        private string GetEffectsStringLong(IEnhancement enhancement, IPower? enhBoostPower)
        {
            string str1;
            var stringBuilder = new StringBuilder();
            var flag1 = false;
            var flag2 = false;
            var flag3 = false;
            var flag4 = false;
            var flag5 = false;

            foreach (var sEffect in enhancement.Effect)
            {
                switch (sEffect.Mode)
                {
                    case Enums.eEffMode.FX:
                        flag1 = true;
                        break;
                    case Enums.eEffMode.Enhancement when sEffect.Schedule != Enums.eSchedule.None:
                        {
                            var scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
                            if (Math.Abs(sEffect.Multiplier) > float.Epsilon)
                            {
                                scheduleMult = (float)Math.Round(scheduleMult * sEffect.Multiplier * 1000) / 1000;
                            }

                            var id = (Enums.eEnhance)sEffect.Enhance.ID;
                            string str2;
                            if (id == Enums.eEnhance.Mez)
                            {
                                var subId = (Enums.eMez)sEffect.Enhance.SubID;
                                str2 = Enum.GetName(subId.GetType(), subId);
                            }
                            else
                            {
                                str2 = Enum.GetName(id.GetType(), id);
                            }

                            switch (sEffect.Enhance.ID)
                            {
                                case 7:
                                case 8:
                                case 17:
                                    str2 = !flag2 ? "Heal" : string.Empty;
                                    flag2 = true;
                                    break;
                                case 10:
                                case 11 when !flag5:
                                    str2 = !flag3 ? "Jump" : string.Empty;
                                    flag3 = true;
                                    break;
                                case 5:
                                case 16:
                                    str2 = !flag4 ? "EndMod" : string.Empty;
                                    flag4 = true;
                                    break;
                                default:
                                    {
                                        if (((enhancement.Name.IndexOf("Slow", StringComparison.Ordinal) > -1 ? 1 : 0) & (sEffect.BuffMode != Enums.eBuffDebuff.DeBuffOnly ? 0 : sEffect.Enhance.ID == 6 || sEffect.Enhance.ID == 11 ? 1 : sEffect.Enhance.ID == 19 ? 1 : 0)) != 0 || sEffect.Enhance.ID == 21)
                                        {
                                            str2 = !flag5 ? "Slow Movement" : string.Empty;
                                            flag5 = true;
                                        }

                                        break;
                                    }
                            }

                            if (!string.IsNullOrEmpty(str2))
                            {
                                if (stringBuilder.Length > 0)
                                {
                                    stringBuilder.Append("\n");
                                }

                                stringBuilder.Append($"{str2} enhancement (Sched. {Enum.GetName(sEffect.Schedule.GetType(), sEffect.Schedule)}: {scheduleMult * 100:##0.###}%{(Math.Abs(sEffect.Multiplier) > float.Epsilon & sEffect.Multiplier != 1 & sEffect.Multiplier != 0.625 & sEffect.Multiplier != 0.5 & sEffect.Multiplier != 0.4375 ? $" [x{sEffect.Multiplier}]" : "")})");
                            }

                            break;
                        }
                    case Enums.eEffMode.PowerEnh:
                    case Enums.eEffMode.PowerProc:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!flag1)
            {
                str1 = stringBuilder.ToString();
            }
            else
            {
                IPower power = new Power(enhBoostPower);
                power.ApplyGrantPowerEffects();
                var returnMask = Array.Empty<int>();

                for (var index1 = 0; index1 < power.Effects.Length; index1++)
                {
                    if (power.Effects[index1].EffectType == Enums.eEffectType.GrantPower && power.Effects[index1].CanGrantPower())
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append("\n");
                        }

                        stringBuilder.Append(power.Effects[index1].BuildEffectString(true, "", false, false, false, true, false, false, true));
                        
                        var empty = string.Empty;

                        var groupedEffectsArray = power.Effects.Where(x => x.EffectType.Equals(Enums.eEffectType.DamageBuff) || x.EffectType.Equals(Enums.eEffectType.Defense) || x.EffectType.Equals(Enums.eEffectType.Resistance) || x.EffectType.Equals(Enums.eEffectType.Elusivity) || x.EffectType.Equals(Enums.eEffectType.Mez)).ToArray();
                        for (var effectId = 0; effectId < groupedEffectsArray.Length; effectId++)
                        {
                            if (power.Effects[index1] == groupedEffectsArray[effectId])
                            {
                                groupedEffectsArray[effectId].Stacking = Enums.eStacking.Yes;
                                groupedEffectsArray[effectId].Buffable = true;
                            }

                            if (groupedEffectsArray[effectId].Absorbed_EffectID == index1)
                            {
                                power.GetEffectStringGrouped(effectId, ref empty, ref returnMask, false, false, false, true, true);
                            }

                            if (returnMask.Length <= 0)
                            {
                                continue;
                            }

                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n");
                            }

                            stringBuilder.AppendFormat("  {0}", empty);
                            break;
                        }

                        var empty2 = string.Empty;
                        var groupedMezEffectsArray = power.Effects.Where(x => x.EffectType == Enums.eEffectType.MezResist).ToArray();
                        if (groupedMezEffectsArray.Length > 0)
                        {
                            for (var effectId = 0; effectId < power.Effects.Length; effectId++)
                            {
                                var flag6 = returnMask.Any(m => m == effectId);
                                
                                if (power.Effects[effectId].Absorbed_EffectID != index1 || flag6)
                                {
                                    continue;
                                }
                                if (stringBuilder.Length > 0)
                                {
                                    stringBuilder.Append("\n");
                                }

                                power.GetEffectStringGrouped(effectId, ref empty2, ref returnMask, false, false, false,
                                    true, true);
                                stringBuilder.AppendFormat("  {0}",  empty2);
                                break;
                            }
                        }
                        else
                        {

                            for (var index2 = 0; index2 < power.Effects.Length; index2++)
                            {
                                var flag6 = returnMask.Any(m => m == index2);

                                if (power.Effects[index2].Absorbed_EffectID != index1 || flag6)
                                {
                                    continue;
                                }

                                if (stringBuilder.Length > 0)
                                {
                                    stringBuilder.Append("\n");
                                }

                                power.Effects[index2].Stacking = Enums.eStacking.Yes;
                                power.Effects[index2].Buffable = true;

                                stringBuilder.AppendFormat("  {0}", power.Effects[index2].BuildEffectString(true, "", false, false, false, true, false, false, true));
                            }
                        }
                    }
                    else if (!power.Effects[index1].Absorbed_Effect) // (!power.Effects[index1].Absorbed_Effect && power.Effects[index1].EffectType != Enums.eEffectType.Enhancement)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append("\n");
                        }

                        var effectString = power.Effects[index1].BuildEffectString(true, "", false, false, false, true).Trim();
                        if (effectString.Contains("Null"))
                        {
                            var enhId = DatabaseAPI.GetEnhancementByBoostName(power.FullName);
                            var enhSetSpecials = DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[enhId].nIDSet];
                            var enhIndex = enhSetSpecials.Enhancements.TryFindIndex(e => e == enhId);
                            if (enhSetSpecials.SpecialBonus.Length > 0)
                            {
                                effectString = enhSetSpecials.SpecialBonus[^1].Index.Length switch
                                {
                                    0 => enhSetSpecials.GetEffectString(enhSetSpecials.SpecialBonus.Length - 2, true, true, true, true),
                                    _ => enhSetSpecials.GetEffectString(enhSetSpecials.SpecialBonus.Length - 1, true, true, true, true)
                                };

                                if (string.IsNullOrEmpty(effectString))
                                {
                                    effectString = enhSetSpecials.GetEffectString(enhIndex, true, true, true, true);
                                }
                                effectString = effectString.Replace(", ", "\n");
                            }
                        }

                        if (!stringBuilder.ToString().Contains(effectString))
                        {
                            stringBuilder.Append(effectString);
                        }
                    }
                }

                str1 = stringBuilder.ToString().Replace("Slf", "Self").Replace("Tgt", "Target");
            }

            return str1;
        }

        public string GetEnhancementStringLong()
        {
            if (Enh < 0)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            var enhPowerEffects = GetEffectsStringLong(enhancement, enhancement.GetPower());
            if ((enhancement.nIDSet < 0) | !string.IsNullOrWhiteSpace(enhPowerEffects.Trim()))
            {
                return enhPowerEffects;
            }

            var enhSet = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet];
            var enhPosInSet = Array.IndexOf(enhSet.Enhancements, Enh);
            if (enhPosInSet < 0)
            {
                return string.Empty;
            }

            var setBonusesForEnh = enhSet.SpecialBonus[enhPosInSet];

            var result = "";
            foreach (var idx in setBonusesForEnh.Index)
            {
                var power = DatabaseAPI.Database.Power[idx];
                var effectList = power.Effects.Select(effect => effect.BuildEffectString(true, "", false, false, false, true, false, false, true)).Where(tEffectString => !string.IsNullOrEmpty(tEffectString)).ToList();

                result = effectList.Count switch
                {
                    > 1 => string.Join("\n", effectList),
                    1 => effectList[0],
                    _ => ""
                };

            }

            return result;
        }

        public string GetRelativeString(bool onlySign)
        {
            if (onlySign)
                switch (RelativeLevel)
                {
                    case Enums.eEnhRelative.MinusThree:
                        return "---";
                    case Enums.eEnhRelative.MinusTwo:
                        return "--";
                    case Enums.eEnhRelative.MinusOne:
                        return "-";
                    case Enums.eEnhRelative.Even:
                        return string.Empty;
                    case Enums.eEnhRelative.PlusOne:
                        return "+";
                    case Enums.eEnhRelative.PlusTwo:
                        return "++";
                    case Enums.eEnhRelative.PlusThree:
                        return "+++";
                    case Enums.eEnhRelative.PlusFour:
                        return "+4";
                    case Enums.eEnhRelative.PlusFive:
                        return "+5";
                    case Enums.eEnhRelative.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            else
                switch (RelativeLevel)
                {
                    case Enums.eEnhRelative.MinusThree:
                        return "-3";
                    case Enums.eEnhRelative.MinusTwo:
                        return "-2";
                    case Enums.eEnhRelative.MinusOne:
                        return "-1";
                    case Enums.eEnhRelative.Even:
                        return string.Empty;
                    case Enums.eEnhRelative.PlusOne:
                        return "+1";
                    case Enums.eEnhRelative.PlusTwo:
                        return "+2";
                    case Enums.eEnhRelative.PlusThree:
                        return "+3";
                    case Enums.eEnhRelative.PlusFour:
                        return "+4";
                    case Enums.eEnhRelative.PlusFive:
                        return "+5";
                    case Enums.eEnhRelative.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return string.Empty;
        }
    }
}
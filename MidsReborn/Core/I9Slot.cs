using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

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
                if (sEffect.Mode != Enums.eEffMode.Enhancement || sEffect.BuffMode == Enums.eBuffDebuff.DeBuffOnly && !(mag <= 0.0) || sEffect.BuffMode == Enums.eBuffDebuff.BuffOnly && !(mag >= 0.0) || sEffect.Schedule == Enums.eSchedule.None || (Enums.eEnhance)sEffect.Enhance.ID != iEffect || subEnh >= 0 && subEnh != sEffect.Enhance.SubID)
                {
                    continue;
                }

                num2 += GetScheduleValue(enhancement.TypeID, sEffect);
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

            if (IOLevel <= 0)
            {
                IOLevel = 0;
            }

            if (IOLevel > DatabaseAPI.Database.MultIO.Length - 1)
            {
                IOLevel = DatabaseAPI.Database.MultIO.Length - 1;
            }

            var num1 = EnhancementScheduleMath.GetRuntimeScheduleBaseScale(iType, Grade, IOLevel, iSched);

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
                var num2 = (int)(RelativeLevel - 4);
                num1 = num2 >= 0 ? (float)(num2 * 0.0500000007450581 + 1.0) : (float)(1.0 + num2 * 0.100000001490116);
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
                    var effects = enhancement.Effect;
                    var scheduleEffects = effects
                        .Where(e => e.Mode == Enums.eEffMode.Enhancement && e.Schedule != Enums.eSchedule.None)
                        .ToArray();
                    if (effects.Length == 0)
                    {
                        str1 = stringBuilder.ToString();
                    }
                    else
                    {
                        if (effects.Any(e => e.Mode == Enums.eEffMode.FX))
                        {
                            flag = true;
                        }

                        string str2;
                        if (scheduleEffects.Length > 0)
                        {
                            AppendEnhancementPrefix(stringBuilder, enhancement);
                            stringBuilder.Append(BuildScheduleSummary(enhancement));
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

        public string GetResolvedEnhancementDescription()
        {
            if (Enh < 0)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            var powerDescription = enhancement.GetPower()?.DescLongFormatted;
            var description = !string.IsNullOrWhiteSpace(powerDescription)
                ? powerDescription
                : enhancement.Desc;

            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }

            return Regex.Replace(
                description,
                @"\{Boost\.Attrib\.([A-Za-z0-9_]+)\.Scale\}",
                match => TryGetBoostAttribScale(match.Groups[1].Value, out var scale)
                    ? Utilities.FixDP(Math.Abs(scale) * 100f)
                    : match.Value,
                RegexOptions.IgnoreCase);
        }

        private void AppendEnhancementPrefix(StringBuilder stringBuilder, IEnhancement enhancement)
        {
            switch (enhancement.TypeID)
            {
                case Enums.eType.Normal:
                    var relativeString1 = Enums.GetRelativeString(RelativeLevel, false);
                    if (!string.IsNullOrEmpty(relativeString1) & (relativeString1 != "X"))
                    {
                        stringBuilder.Append(relativeString1 + " " + DatabaseAPI.Database.EnhGradeStringLong[(int)Grade] + " - ");
                        break;
                    }

                    if (relativeString1 == "X")
                    {
                        stringBuilder.Append("Disabled " + DatabaseAPI.Database.EnhGradeStringLong[(int)Grade] + " - ");
                        break;
                    }

                    stringBuilder.Append(DatabaseAPI.Database.EnhGradeStringLong[(int)Grade] + " - ");
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
        }

        private string BuildScheduleSummary(IEnhancement enhancement)
        {
            var scheduleValues = enhancement.Effect
                .Where(effect => effect.Mode == Enums.eEffMode.Enhancement && effect.Schedule != Enums.eSchedule.None)
                .Select(effect => new
                {
                    effect.Schedule,
                    Value = GetScheduleValue(enhancement.TypeID, effect)
                })
                .GroupBy(item => new
                {
                    item.Schedule,
                    RoundedValue = (float)Math.Round(item.Value, 5)
                })
                .Select(group => $"{group.Key.Schedule} ({group.Key.RoundedValue * 100:##0.###}%)")
                .ToArray();

            return scheduleValues.Length switch
            {
                0 => string.Empty,
                1 => $"Schedule: {scheduleValues[0]}",
                _ => $"Schedules: {string.Join(", ", scheduleValues)}"
            };
        }

        private float GetScheduleValue(Enums.eType enhancementType, Enums.sEffect effect)
        {
            var scheduleMult = GetScheduleMult(enhancementType, effect.Schedule);
            if (Math.Abs(effect.Multiplier) > float.Epsilon)
            {
                scheduleMult *= EnhancementScheduleMath.NormalizeImportedScaleToMultiplier(
                    enhancementType,
                    effect.Schedule,
                    effect.Multiplier);
            }

            return scheduleMult;
        }

        private bool TryGetBoostAttribScale(string boostAttribName, out float scale)
        {
            scale = 0f;
            if (Enh < 0)
            {
                return false;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            if (TryGetBoostAttribScaleFromEnhancementRows(enhancement, boostAttribName, out scale))
            {
                return true;
            }

            return TryGetBoostAttribScaleFromBoostPower(enhancement, enhancement.GetPower(), boostAttribName, out scale);
        }

        private bool TryGetBoostAttribScaleFromEnhancementRows(
            IEnhancement enhancement,
            string boostAttribName,
            out float scale)
        {
            scale = 0f;
            foreach (var enhance in EnumerateBoostAttribCandidates(boostAttribName))
            {
                var best = enhancement.Effect
                    .Where(effect => effect.Mode == Enums.eEffMode.Enhancement &&
                                     effect.Schedule != Enums.eSchedule.None &&
                                     (Enums.eEnhance)effect.Enhance.ID == enhance)
                    .Select(effect => Math.Abs(GetScheduleValue(enhancement.TypeID, effect)))
                    .DefaultIfEmpty(0f)
                    .Max();

                if (best > float.Epsilon)
                {
                    scale = best;
                    return true;
                }
            }

            return false;
        }

        private bool TryGetBoostAttribScaleFromBoostPower(
            IEnhancement enhancement,
            IPower? boostPower,
            string boostAttribName,
            out float scale)
        {
            scale = 0f;
            var preparedPower = PreparePopupBoostPower(boostPower);
            if (preparedPower?.Effects == null || preparedPower.Effects.Length == 0)
            {
                return false;
            }

            foreach (var enhance in EnumerateBoostAttribCandidates(boostAttribName))
            {
                var best = preparedPower.Effects
                    .Where(effect => effect != null && MapEnhanceFromEffect(effect) == enhance)
                    .Select(effect => Math.Abs(GetScheduleValueFromBoostPowerEffect(enhancement.TypeID, effect)))
                    .DefaultIfEmpty(0f)
                    .Max();

                if (best > float.Epsilon)
                {
                    scale = best;
                    return true;
                }
            }

            return false;
        }

        private float GetScheduleValueFromBoostPowerEffect(Enums.eType enhancementType, IEffect effect)
        {
            var enhance = MapEnhanceFromEffect(effect);
            if (enhance == Enums.eEnhance.None)
            {
                return 0f;
            }

            var schedule = Enhancement.GetSchedule(
                enhance,
                enhance == Enums.eEnhance.Mez ? (int)effect.MezType : -1);
            var sourceScale = GetEffectSourceScale(effect);
            if (sourceScale <= float.Epsilon)
            {
                return 0f;
            }

            var scheduleMult = GetScheduleMult(enhancementType, schedule);
            var multiplier = EnhancementScheduleMath.NormalizeImportedScaleToMultiplier(
                enhancementType,
                schedule,
                sourceScale);
            return scheduleMult * multiplier;
        }

        private static float GetEffectSourceScale(IEffect effect)
        {
            var scale = Math.Abs(effect.Scale) > 0.0001f
                ? effect.Scale
                : effect.nMagnitude;
            return Math.Abs(scale);
        }

        private static Enums.eEnhance[] EnumerateBoostAttribCandidates(string boostAttribName)
        {
            return boostAttribName.ToLowerInvariant() switch
            {
                "accuracy" => [Enums.eEnhance.Accuracy],
                "damage" => [Enums.eEnhance.Damage, Enums.eEnhance.Resistance],
                "defense" => [Enums.eEnhance.Defense],
                "endurance" => [Enums.eEnhance.EnduranceDiscount, Enums.eEnhance.Endurance, Enums.eEnhance.Recovery],
                "heal" => [Enums.eEnhance.Heal, Enums.eEnhance.Absorb, Enums.eEnhance.HitPoints, Enums.eEnhance.Regeneration],
                "interrupt" => [Enums.eEnhance.Interrupt],
                "jump" => [Enums.eEnhance.JumpHeight, Enums.eEnhance.SpeedJumping],
                "movement" => [Enums.eEnhance.SpeedRunning, Enums.eEnhance.SpeedFlying, Enums.eEnhance.SpeedJumping, Enums.eEnhance.JumpHeight, Enums.eEnhance.Slow],
                "mez" => [Enums.eEnhance.Mez],
                "range" => [Enums.eEnhance.Range],
                "rechargetime" => [Enums.eEnhance.RechargeTime, Enums.eEnhance.X_RechargeTime],
                "resistance" => [Enums.eEnhance.Resistance],
                "tohit" => [Enums.eEnhance.ToHit],
                _ => []
            };
        }

        public string GetPopupEnhancementStringLong()
        {
            if (Enh < 0)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            var popupEffects = GetPopupEffectsStringLong(enhancement, enhancement.GetPower());
            if (!string.IsNullOrWhiteSpace(popupEffects))
            {
                return popupEffects;
            }

            return enhancement.nIDSet < 0
                ? string.Empty
                : GetPopupSetBackedSpecialString(enhancement);
        }

        private string GetPopupEffectsStringLong(IEnhancement enhancement, IPower? enhBoostPower)
        {
            var stringBuilder = new StringBuilder();
            var hasFxEffects = false;
            var hasHealFamily = false;
            var hasJumpFamily = false;
            var hasEndFamily = false;
            var hasSlowMovement = false;

            foreach (var sEffect in enhancement.Effect)
            {
                switch (sEffect.Mode)
                {
                    case Enums.eEffMode.FX:
                        hasFxEffects = true;
                        break;
                    case Enums.eEffMode.Enhancement when sEffect.Schedule != Enums.eSchedule.None:
                    {
                        var scheduleMult = (float)Math.Round(GetScheduleValue(enhancement.TypeID, sEffect) * 1000) / 1000;
                        var label = GetPopupScheduledEffectLabel(
                            enhancement,
                            sEffect,
                            ref hasHealFamily,
                            ref hasJumpFamily,
                            ref hasEndFamily,
                            ref hasSlowMovement);

                        if (string.IsNullOrEmpty(label))
                        {
                            break;
                        }

                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append("\n");
                        }

                        stringBuilder.Append(
                            $"{label} enhancement (Sched. {Enum.GetName(sEffect.Schedule.GetType(), sEffect.Schedule)}: {scheduleMult * 100:##0.###}%{(Math.Abs(sEffect.Multiplier) > float.Epsilon & sEffect.Multiplier != 1 & sEffect.Multiplier != 0.625 & sEffect.Multiplier != 0.5 & sEffect.Multiplier != 0.4375 ? $" [x{sEffect.Multiplier}]" : "")})");
                        break;
                    }
                }
            }

            if (!hasFxEffects)
            {
                return stringBuilder.ToString();
            }

            var groupedGrantEffects = GetPopupGroupedEffectsStringLong(enhBoostPower, true);
            if (!string.IsNullOrWhiteSpace(groupedGrantEffects))
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append("\n");
                }

                stringBuilder.Append(groupedGrantEffects);
            }

            var popupBoostPower = PreparePopupBoostPower(enhBoostPower);
            if (popupBoostPower != null)
            {
                AppendPopupPowerEffects(stringBuilder, popupBoostPower);
            }

            if (stringBuilder.Length > 0)
            {
                return stringBuilder.ToString().Replace("Slf", "Self").Replace("Tgt", "Target");
            }

            return GetPopupGroupedEffectsStringLong(enhBoostPower);
        }

        private static string GetPopupScheduledEffectLabel(
            IEnhancement enhancement,
            Enums.sEffect sEffect,
            ref bool hasHealFamily,
            ref bool hasJumpFamily,
            ref bool hasEndFamily,
            ref bool hasSlowMovement)
        {
            var id = (Enums.eEnhance)sEffect.Enhance.ID;
            string label;
            if (id == Enums.eEnhance.Mez)
            {
                var subId = (Enums.eMez)sEffect.Enhance.SubID;
                label = Enum.GetName(subId.GetType(), subId);
            }
            else
            {
                label = Enum.GetName(id.GetType(), id);
            }

            switch (sEffect.Enhance.ID)
            {
                case 7:
                case 8:
                case 17:
                    label = !hasHealFamily ? "Heal" : string.Empty;
                    hasHealFamily = true;
                    break;
                case 10:
                case 11 when !hasSlowMovement:
                    label = !hasJumpFamily ? "Jump" : string.Empty;
                    hasJumpFamily = true;
                    break;
                case 5:
                case 16:
                    label = !hasEndFamily ? "EndMod" : string.Empty;
                    hasEndFamily = true;
                    break;
                default:
                    if (((enhancement.Name.IndexOf("Slow", StringComparison.Ordinal) > -1 ? 1 : 0) &
                         (sEffect.BuffMode != Enums.eBuffDebuff.DeBuffOnly
                             ? 0
                             : sEffect.Enhance.ID == 6 || sEffect.Enhance.ID == 11
                                 ? 1
                                 : sEffect.Enhance.ID == 19
                                     ? 1
                                     : 0)) != 0 ||
                        sEffect.Enhance.ID == 21)
                    {
                        label = !hasSlowMovement ? "Slow Movement" : string.Empty;
                        hasSlowMovement = true;
                    }

                    break;
            }

            return label;
        }

        private void AppendPopupPowerEffects(StringBuilder stringBuilder, IPower popupBoostPower)
        {
            var returnMask = Array.Empty<int>();

            for (var index1 = 0; index1 < popupBoostPower.Effects.Length; index1++)
            {
                if (popupBoostPower.Effects[index1].EffectType == Enums.eEffectType.GrantPower &&
                    popupBoostPower.Effects[index1].CanGrantPower())
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append("\n");
                    }

                    stringBuilder.Append(popupBoostPower.Effects[index1].BuildEffectString(true, "", false, false, false, true, false, false, true));

                    var empty = string.Empty;
                    var groupedEffectsArray = popupBoostPower.Effects.Where(x =>
                        x.EffectType.Equals(Enums.eEffectType.DamageBuff) ||
                        x.EffectType.Equals(Enums.eEffectType.Defense) ||
                        x.EffectType.Equals(Enums.eEffectType.Resistance) ||
                        x.EffectType.Equals(Enums.eEffectType.Elusivity) ||
                        x.EffectType.Equals(Enums.eEffectType.Mez)).ToArray();

                    for (var effectId = 0; effectId < groupedEffectsArray.Length; effectId++)
                    {
                        if (popupBoostPower.Effects[index1] == groupedEffectsArray[effectId])
                        {
                            groupedEffectsArray[effectId].Stacking = Enums.eStacking.Yes;
                            groupedEffectsArray[effectId].Buffable = true;
                        }

                        if (groupedEffectsArray[effectId].Absorbed_EffectID == index1)
                        {
                            popupBoostPower.GetEffectStringGrouped(effectId, ref empty, ref returnMask, false, false, false, true, true);
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
                    var groupedMezEffectsArray = popupBoostPower.Effects.Where(x => x.EffectType == Enums.eEffectType.MezResist).ToArray();
                    if (groupedMezEffectsArray.Length > 0)
                    {
                        for (var effectId = 0; effectId < popupBoostPower.Effects.Length; effectId++)
                        {
                            var alreadyGrouped = returnMask.Any(m => m == effectId);
                            if (popupBoostPower.Effects[effectId].Absorbed_EffectID != index1 || alreadyGrouped)
                            {
                                continue;
                            }

                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n");
                            }

                            popupBoostPower.GetEffectStringGrouped(effectId, ref empty2, ref returnMask, false, false, false, true, true);
                            stringBuilder.AppendFormat("  {0}", empty2);
                            break;
                        }
                    }
                    else
                    {
                        for (var index2 = 0; index2 < popupBoostPower.Effects.Length; index2++)
                        {
                            var alreadyGrouped = returnMask.Any(m => m == index2);
                            if (popupBoostPower.Effects[index2].Absorbed_EffectID != index1 || alreadyGrouped)
                            {
                                continue;
                            }

                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n");
                            }

                            popupBoostPower.Effects[index2].Stacking = Enums.eStacking.Yes;
                            popupBoostPower.Effects[index2].Buffable = true;
                            stringBuilder.AppendFormat("  {0}", popupBoostPower.Effects[index2].BuildEffectString(true, "", false, false, false, true, false, false, true));
                        }
                    }
                }
                else if (!popupBoostPower.Effects[index1].Absorbed_Effect)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append("\n");
                    }

                    var effectString = popupBoostPower.Effects[index1].BuildEffectString(true, "", false, false, false, true).Trim();
                    if (effectString.Contains("Null", StringComparison.Ordinal))
                    {
                        effectString = TryResolvePopupNullWrapperEffect(popupBoostPower.FullName);
                    }

                    if (!string.IsNullOrWhiteSpace(effectString) && !stringBuilder.ToString().Contains(effectString, StringComparison.Ordinal))
                    {
                        stringBuilder.Append(effectString);
                    }
                }
            }
        }

        private static string TryResolvePopupNullWrapperEffect(string boostPowerFullName)
        {
            var enhId = DatabaseAPI.GetEnhancementByBoostName(boostPowerFullName);
            if (enhId < 0)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[enhId];
            if (enhancement.nIDSet < 0)
            {
                return string.Empty;
            }

            var enhancementSet = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet];
            var enhIndex = enhancementSet.Enhancements.TryFindIndex(e => e == enhId);
            if (enhancementSet.SpecialBonus.Length <= 0)
            {
                return string.Empty;
            }

            var effectString = enhancementSet.SpecialBonus[^1].Index.Length switch
            {
                0 => enhancementSet.GetEffectString(enhancementSet.SpecialBonus.Length - 2, true, true, true, true),
                _ => enhancementSet.GetEffectString(enhancementSet.SpecialBonus.Length - 1, true, true, true, true)
            };

            if (string.IsNullOrEmpty(effectString) && enhIndex > -1)
            {
                effectString = enhancementSet.GetEffectString(enhIndex, true, true, true, true);
            }

            return effectString.Replace(", ", "\n");
        }

        private string GetPopupGroupedEffectsStringLong(IPower? enhBoostPower, bool absorbedGrantEffectsOnly = false)
        {
            var power = PreparePopupBoostPower(enhBoostPower);
            if (power == null)
            {
                return string.Empty;
            }

            var groupedEffects = GroupedFx.AssembleGroupedEffects(power, true)
                .Where(g =>
                {
                    var effect = g.GetEffectAt(power);
                    if (effect.EffectClass == Enums.eEffectClass.Ignored ||
                        effect.EffectType == Enums.eEffectType.GrantPower)
                    {
                        return false;
                    }

                    return !absorbedGrantEffectsOnly ||
                           effect.Absorbed_Effect ||
                           effect.Absorbed_EffectID >= 0;
                })
                .Select(g => g.GetTooltip(power, true))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct();

            return string.Join("\n", groupedEffects);
        }

        private static Power? PreparePopupBoostPower(IPower? enhBoostPower)
        {
            if (enhBoostPower == null)
            {
                return null;
            }

            var power = new Power(enhBoostPower);
            if (power.HasGrantPowerEffect)
            {
                power.ApplyGrantPowerEffects();
            }

            if (!power.AppliedExecutes)
            {
                power.ProcessExecutes();
            }

            return power;
        }

        private string GetPopupSetBackedSpecialString(IEnhancement enhancement)
        {
            var enhancementSet = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet];
            var enhPosInSet = Array.IndexOf(enhancementSet.Enhancements, Enh);
            if (enhPosInSet < 0)
            {
                return string.Empty;
            }

            return enhancementSet.GetEffectString(enhPosInSet, true, true, true, true);
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

            if (enhBoostPower != null && enhBoostPower.Effects.All(e => e.EffectType != Enums.eEffectType.GrantPower))
            {
                return GetGroupedEffectsStringLong(enhBoostPower);
            }

            foreach (var sEffect in enhancement.Effect)
            {
                switch (sEffect.Mode)
                {
                    case Enums.eEffMode.FX:
                        flag1 = true;
                        break;
                    case Enums.eEffMode.Enhancement when sEffect.Schedule != Enums.eSchedule.None:
                        {
                            var scheduleMult = (float)Math.Round(GetScheduleValue(enhancement.TypeID, sEffect) * 1000) / 1000;

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
                var groupedGrantEffects = GetGroupedEffectsStringLong(enhBoostPower, true);
                if (!string.IsNullOrWhiteSpace(groupedGrantEffects))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append("\n");
                    }

                    stringBuilder.Append(groupedGrantEffects);
                    str1 = stringBuilder.ToString().Replace("Slf", "Self").Replace("Tgt", "Target");
                    return str1;
                }

                IPower power = PlannerEffectResolver.ResolvePower(new Power(enhBoostPower)).ResolvedPower;
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
                                stringBuilder.AppendFormat("  {0}", empty2);
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

        private string GetGroupedEffectsStringLong(IPower? enhBoostPower, bool absorbedGrantEffectsOnly = false)
        {
            if (enhBoostPower == null)
            {
                return "";
            }

            var power = PlannerEffectResolver.ResolvePower(new Power(enhBoostPower)).ResolvedPower;
            ApplySlotEnhancementMagnitudes(power);

            var groupedEffects = GroupedFx.AssembleGroupedEffects(power, true)
                .Where(g =>
                {
                    var effect = g.GetEffectAt(power);
                    if (effect.EffectClass == Enums.eEffectClass.Ignored ||
                        effect.EffectType == Enums.eEffectType.GrantPower)
                    {
                        return false;
                    }

                    return !absorbedGrantEffectsOnly ||
                           effect.Absorbed_Effect ||
                           effect.Absorbed_EffectID >= 0;
                })
                .Select(g => g.GetTooltip(power, true))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct();

            return string.Join("\n", groupedEffects);
        }

        private void ApplySlotEnhancementMagnitudes(IPower power)
        {
            if (Enh < 0)
            {
                return;
            }

            foreach (var effect in power.Effects.Where(effect => effect != null))
            {
                var enhance = MapEnhanceFromEffect(effect);
                if (enhance == Enums.eEnhance.None)
                {
                    continue;
                }

                var signedSource = effect.Scale * effect.nMagnitude;
                var sign = signedSource < 0 ? -1f : 1f;
                var subEnhance = enhance == Enums.eEnhance.Mez ? (int)effect.MezType : -1;
                var enhancedValue = GetEnhancementEffect(enhance, subEnhance, sign);
                if (Math.Abs(enhancedValue) > float.Epsilon)
                {
                    effect.Math_Mag = sign * Math.Abs(enhancedValue);
                }
            }
        }

        private static Enums.eEnhance MapEnhanceFromEffect(IEffect effect)
        {
            var sourceEffectType = effect.EffectType;
            if ((sourceEffectType == Enums.eEffectType.Enhancement || sourceEffectType == Enums.eEffectType.ResEffect) &&
                effect.ETModifies != Enums.eEffectType.None)
            {
                sourceEffectType = effect.ETModifies;
            }

            return sourceEffectType switch
            {
                Enums.eEffectType.Accuracy => Enums.eEnhance.Accuracy,
                Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff => Enums.eEnhance.Damage,
                Enums.eEffectType.Defense => Enums.eEnhance.Defense,
                Enums.eEffectType.EnduranceDiscount => Enums.eEnhance.EnduranceDiscount,
                Enums.eEffectType.Endurance => Enums.eEnhance.Endurance,
                Enums.eEffectType.Fly or Enums.eEffectType.SpeedFlying or Enums.eEffectType.MaxFlySpeed => Enums.eEnhance.SpeedFlying,
                Enums.eEffectType.Heal => Enums.eEnhance.Heal,
                Enums.eEffectType.HitPoints => Enums.eEnhance.HitPoints,
                Enums.eEffectType.InterruptTime => Enums.eEnhance.Interrupt,
                Enums.eEffectType.JumpHeight => Enums.eEnhance.JumpHeight,
                Enums.eEffectType.SpeedJumping or Enums.eEffectType.MaxJumpSpeed => Enums.eEnhance.SpeedJumping,
                Enums.eEffectType.Mez => Enums.eEnhance.Mez,
                Enums.eEffectType.Range => Enums.eEnhance.Range,
                Enums.eEffectType.RechargeTime => Enums.eEnhance.RechargeTime,
                Enums.eEffectType.Recovery => Enums.eEnhance.Recovery,
                Enums.eEffectType.Regeneration => Enums.eEnhance.Regeneration,
                Enums.eEffectType.Resistance => Enums.eEnhance.Resistance,
                Enums.eEffectType.SpeedRunning or Enums.eEffectType.MaxRunSpeed => Enums.eEnhance.SpeedRunning,
                Enums.eEffectType.ToHit => Enums.eEnhance.ToHit,
                Enums.eEffectType.Slow => Enums.eEnhance.Slow,
                Enums.eEffectType.Absorb => Enums.eEnhance.Absorb,
                _ => Enums.eEnhance.None
            };
        }

        public string GetEnhancementStringLong()
        {
            if (Enh < 0)
            {
                return string.Empty;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[Enh];
            var enhPowerEffects = GetEffectsStringLong(enhancement, enhancement.GetPower());
            if (enhancement.nIDSet < 0 | !string.IsNullOrWhiteSpace(enhPowerEffects.Trim()))
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
                var effectList = GetGroupedEffectsStringLong(power);

                if (!string.IsNullOrWhiteSpace(effectList))
                {
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        result += "\n";
                    }

                    result += effectList;
                }
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn
{
    public class clsToonX : Character
    {
        private const double BuildFormatChange1 = 1.29999995231628;
        private const double BuildFormatChange2 = 1.39999997615814;
        private IPower?[] _buffedPower = Array.Empty<IPower>();
        private IPower[] _mathPower = Array.Empty<IPower>();
        private Enums.BuffsX _selfBuffs;
        private Enums.BuffsX _selfEnhance;

        private struct FxIdentifierKey
        {
            public Enums.eEffectType EffectType;
            public Enums.eDamage DamageType;
            public Enums.eMez MezType;
            public Enums.eEffectType ETModifies;
            public string Summon;
        }

        private struct GrantedPowerInfo
        {
            public IEffect GrantPowerFX;
            public IPower TargetPower;
            public IPower SourcePower;
        }

        private void ApplyPvpDr()
        {
            if (MidsContext.Config.Inc.DisablePvE)
            {
                for (var index = 0; index < Totals.Def.Length; index++)
                {
                    Totals.Def[index] = CalculatePvpDr(Totals.Def[index], 1.2f, 1f);
                }
            }
        }

        private static PopUp.StringValue BuildEDItem(int index, float[] value, Enums.eSchedule[] schedule, string edName, float[] afterED)
        {
            var stringValue1 = new PopUp.StringValue();
            var flag1 = value[index] > (double) DatabaseAPI.Database.MultED[(int) schedule[index]][0];
            var flag2 = value[index] > (double) DatabaseAPI.Database.MultED[(int) schedule[index]][1];
            var flag3 = value[index] > (double) DatabaseAPI.Database.MultED[(int) schedule[index]][2];
            PopUp.StringValue stringValue2;
            if (value[index] > 0)
            {
                var color = new Color();
                var str1 = edName + ":";
                var num1 = value[index] * 100f;
                var num2 = Enhancement.ApplyED(schedule[index], value[index]) * 100f;
                var str2 = $"{Convert.ToDecimal(num1 + afterED[index] * 100f):0.##}%";
                var str3 = $"{Convert.ToDecimal(num2 + afterED[index] * 100f):0.##}%";
                //var str2 = Strings.Format((float) (num1 + afterED[index] * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%";
                //var str3 = Strings.Format(num2 + afterED[index] * 100f, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%";
                string str4;
                if (Math.Round(num1 - num2, 3) > 0)
                {
                    str4 = str3 + "  (Pre-ED: " + str2 + ")";
                    if (flag3)
                        color = Color.FromArgb(byte.MaxValue, 0, 0);
                    else if (flag2)
                        color = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0);
                    else if (flag1)
                        color = Color.FromArgb(0, byte.MaxValue, 0);
                }
                else
                {
                    str4 = str3;
                    color = PopUp.Colors.Title;
                }

                stringValue1.Text = str1;
                stringValue1.TextColumn = str4;
                stringValue1.tColor = PopUp.Colors.Title;
                stringValue1.tColorColumn = color;
                stringValue1.tSize = 0.9f;
                stringValue1.tIndent = 1;
                stringValue1.tFormat = FontStyle.Bold;
                stringValue2 = stringValue1;
            }
            else
            {
                stringValue1.Text = "";
                stringValue1.tColor = Color.White;
                stringValue1.tFormat = FontStyle.Bold;
                stringValue1.tIndent = 0;
                stringValue1.tColorColumn = Color.White;
                stringValue1.TextColumn = "";
                stringValue2 = stringValue1;
            }

            return stringValue2;
        }

        public IPowerset? PickDefaultSecondaryPowerset()
        {
            return Powersets[1] == null || Powersets[1].nID < 0
                ? DatabaseAPI.Database.Powersets
                    .First(ps =>
                        ps.ATClass == MidsContext.Character.Archetype.ClassName &
                        ps.SetType == Enums.ePowerSetType.Secondary)
                : Powersets[1];
        }

        public void BuildPower(int iSet, int powerID, bool noPoolShuffle = false)
        {
            if (iSet < 0 || powerID < 0)
            {
                return;
            }

            var ps1 = PickDefaultSecondaryPowerset();
            var inToonHistory = CurrentBuild.FindInToonHistory(powerID);
            ResetLevel();
            var numArray = DatabaseAPI.NidPowersAtLevelBranch(0, ps1.nID);
            var flag1 = numArray.Length > 1;
            var message = "";
            if (inToonHistory > -1)
            {
                if (CanRemovePower(inToonHistory, true, out message))
                {
                    if (inToonHistory < CurrentBuild.Powers.Count)
                    {
                        CurrentBuild.Powers[inToonHistory].Reset();
                    }

                    RequestedLevel = CurrentBuild.Powers[inToonHistory].Level;
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ResetLevel();
                Lock();
            }
            else
            {
                if (DatabaseAPI.Database.Powersets[iSet].SetType != Enums.ePowerSetType.Secondary & !flag1 && CurrentBuild.Powers[1].NIDPowerset < 0 & !CurrentBuild.PowerUsed(ps1.Powers[0]) && numArray.Length > 0)
                {
                    SetPower_NID(1, numArray[0]);
                }

                var i = -1;
                switch (MidsContext.Config.BuildMode)
                {
                    case Enums.dmModes.LevelUp:
                    {
                        i = GetFirstAvailablePowerIndex(DatabaseAPI.Database.Power[powerID].Level - 1);
                        switch (i)
                        {
                            case < 0:
                                message = "You cannot place any additional powers unless you first remove one.";
                                break;
                            default:
                            {
                                if (CurrentBuild.Powers[i].Level <= Level)
                                {
                                    if (!TestPower(powerID))
                                        i = -1;
                                }
                                else
                                {
                                    i = -1;
                                }

                                break;
                            }
                        }
                        break;
                    }
                    case Enums.dmModes.Normal:
                    case Enums.dmModes.Respec:
                    {
                        i = GetFirstAvailablePowerIndex(Math.Max(RequestedLevel, DatabaseAPI.Database.Power[powerID].Level - 1));
                        break;
                    }
                }

                var flag2 = false;
                switch (i)
                {
                    case 0:
                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Primary)
                        {
                            if (DatabaseAPI.Database.Power[powerID].Level == 1)
                            {
                                flag2 = true;
                                break;
                            }

                            message = "You must place a level 1 Primary power here.";
                            break;
                        }

                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Secondary)
                        {
                            if (CurrentBuild.Powers[1].NIDPowerset < 0)
                            {
                                i = 1;
                                flag2 = true;
                            }
                            else
                            {
                                message = "You must place a level 1 Primary power here.";
                            }
                        }

                        break;
                    case 1:
                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Secondary)
                        {
                            if (DatabaseAPI.Database.Power[powerID].Level == 1)
                            {
                                flag2 = true;
                                break;
                            }

                            message = "You must place a level 1 Secondary power here.";
                            break;
                        }

                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Primary)
                        {
                            if (CurrentBuild.Powers[0].NIDPowerset < 0)
                            {
                                i = 0;
                                flag2 = true;
                            }
                            else
                            {
                                message = "You must place a level 1 Secondary power here.";
                            }
                        }

                        break;
                    default:
                        flag2 = i > 1;
                        break;
                }

                if (flag2)
                {
                    SetPower_NID(i, powerID);
                    Lock();

                    if (MidsContext.Character.CurrentBuild.Powers[i]?.Power is {VariableEnabled: true})
                    {
                        var initialVariableValue = Math.Max(MidsContext.Character.CurrentBuild.Powers[i].Power.VariableMin, MidsContext.Character.CurrentBuild.Powers[i].Power.VariableStart);
                        MidsContext.Character.CurrentBuild.Powers[i].VariableValue = initialVariableValue;
                        MidsContext.Character.CurrentBuild.Powers[i].Power.Stacks = initialVariableValue;
                    }
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            Validate();
            if (!noPoolShuffle)
            {
                PoolShuffle();
            }

            ResetLevel();
        }

        public int BuildSlot(int powerIDX, int slotIDX = -1)
        {
            int num1;
            if ((powerIDX < 0) | (powerIDX >= CurrentBuild.Powers.Count))
            {
                num1 = -1;
            }
            else
            {
                var num2 = -1;
                if (slotIDX > -1)
                {
                    if (CurrentBuild.Powers[powerIDX].CanRemoveSlot(slotIDX, out var message))
                    {
                        CurrentBuild.RemoveSlotFromPower(powerIDX, slotIDX);
                        if (!CurrentBuild.Powers[powerIDX].Chosen & (CurrentBuild.Powers[powerIDX].Slots.Length == 0))
                        {
                            CurrentBuild.Powers[powerIDX].Level = -1;
                        }

                        ResetLevel();
                        Lock();
                    }
                    else if (!string.IsNullOrWhiteSpace(message) & !MidsContext.EnhCheckMode)
                    {
                        MessageBox.Show(message, @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var iLevel = SlotCheck(CurrentBuild.Powers[powerIDX]);
                    if (iLevel > -1)
                        num2 = CurrentBuild.Powers[powerIDX].AddSlot(iLevel);
                }

                ResetLevel();
                Validate();
                num1 = num2;
            }

            return num1;
        }

        private static float CalculatePvpDr(float val, float a, float b)
        {
            return val * (float) (1.0 - Math.Abs(Math.Atan(a * (double) val)) * (2.0 / Math.PI) * b);
        }

        public static string FixSpelling(string iString)
        {
            iString = iString?.Replace("Armour", "Armor");
            iString = iString?.Replace("Electric Mastery", "Electrical Mastery");
            return iString;
        }

        public void FlipAllSlots()
        {
            var num = CurrentBuild.Powers.Count - 1;
            for (var iPowerSlot = 0; iPowerSlot <= num; ++iPowerSlot)
                FlipSlots(iPowerSlot);
            GenerateBuffedPowerArray();
        }

        public void FlipSlots(int iPowerSlot)
        {
            if (iPowerSlot < 0)
                return;
            var num = CurrentBuild.Powers[iPowerSlot].SlotCount - 1;
            for (var index = 0; index <= num; ++index)
                CurrentBuild.Powers[iPowerSlot].Slots[index].Flip();
        }

        private void RemoveGrantEffectIndirect(ref IPower[] basePower, IPower targetPower, string summon)
        {
            var basePowerPicked = basePower.Where(bp => bp != null).ToList();
            var basePowerIdx = basePowerPicked
                .FindIndexes(bp => bp.FullName == targetPower.FullName)
                .ToList();
            if (basePowerIdx.Count <= 0) return;

            var fxListBase = basePowerPicked[basePowerIdx[0]].Effects.ToList();
            var gFxIdxBase = fxListBase.FindIndexes(fx => fx.EffectType == Enums.eEffectType.GrantPower & fx.Summon == summon).ToList();
            if (gFxIdxBase.Count <= 0) return;
            
            fxListBase.Remove(fxListBase[gFxIdxBase[0]]);
            basePowerPicked[basePowerIdx[0]].Effects = fxListBase.ToArray();
        }

        private void ApplyGlobalEnhancements()
        {
            var grantedPowers = new List<GrantedPowerInfo>();
            
            // Fetch buffed powers that are non empty, non incarnates
            var allowedSets = new List<Enums.ePowerSetType> { Enums.ePowerSetType.Ancillary, Enums.ePowerSetType.Pool, Enums.ePowerSetType.Primary, Enums.ePowerSetType.Secondary };
            var mainPowers = _mathPower.Where(p => p is { StaticIndex: >= 0 } && allowedSets.Any(x => x == p.GetPowerSet().SetType)).ToList();

            // Inventory and collect GrantPower effects that lead to GlobalBoost powers
            foreach (var p in mainPowers)
            {
                var grantedPowersFx = p.Effects
                    .Where(fx => fx.EffectType == Enums.eEffectType.GrantPower)
                    .ToList();

                if (grantedPowersFx.Count <= 0) continue;

                foreach (var gFx in grantedPowersFx)
                {
                    var gPower = DatabaseAPI.GetPowerByFullName(gFx.Summon);
                    if (gPower is not { PowerType: Enums.ePowerType.GlobalBoost }) continue;

                    // Verify if the list doesn't have yet the target granted power
                    // Effect definition may differ a little.
                    var hasPower = false;
                    foreach (var gp in grantedPowers)
                    {
                        if (gp.TargetPower.FullName == gPower.FullName)
                        {
                            hasPower = true;
                            break;
                        }
                    }

                    if (hasPower) continue;
                    
                    grantedPowers.Add(new GrantedPowerInfo
                    {
                        GrantPowerFX = (IEffect)gFx.Clone(),
                        TargetPower = gPower.Clone(),
                        SourcePower = p.Clone()
                    });

                    // Special flag to get rid of the GrantPower effect from source
                    // Power attributes > Basic > MxD Special Flags > Ignore when setting graph scale
                    if (!gPower.SkipMax) continue;

                    var fxList = p.Effects.ToList();
                    fxList.Remove(gFx);
                    p.Effects = fxList.ToArray();
                    RemoveGrantEffectIndirect(ref _mathPower, p, gFx.Summon);
                    if (MidsContext.Character.CurrentBuild.Powers == null) continue;

                    var buildPowerPicked = MidsContext.Character.CurrentBuild.Powers
                        .Where(pe => pe.Power != null)
                        .ToList();
                    var buildPowerIdx = buildPowerPicked
                        .FindIndexes(pe => pe.Power.FullName == p.FullName)
                        .ToList();
                    if (buildPowerIdx.Count <= 0) continue;

                    var buildPower = buildPowerPicked[buildPowerIdx[0]].Power;
                    var gFxIdxBuild = buildPower.Effects
                        .FindIndexes(fx => fx.EffectType == Enums.eEffectType.GrantPower & fx.Summon == gFx.Summon)
                        .ToList();
                    if (gFxIdxBuild.Count <= 0) continue;

                    buildPower.Effects[gFxIdxBuild[0]].EffectClass = Enums.eEffectClass.Ignored;
                    buildPower.Effects[gFxIdxBuild[0]].Probability = 0;
                    buildPower.Effects[gFxIdxBuild[0]].Scale = 0;
                }
            }

            foreach (var gp in grantedPowers)
            {
                foreach (var p in mainPowers)
                {
                    // Check if power already has the target granted power
                    var hasBoost = p.FullName == gp.SourcePower.FullName;
                    if (hasBoost) continue;

                    foreach (var fx in p.Effects)
                    {
                        if (fx.EffectType != Enums.eEffectType.GrantPower) continue;
                        if (fx.Summon == gp.TargetPower.FullName)
                        {
                            hasBoost = true;
                            break;
                        }
                    }

                    if (hasBoost) continue;

                    // Check if power is eligible to the boost effect:
                    // Must have at least one type of effect in common
                    var bPowerFxIdentifiers = p.Effects
                        .Select(pfx => new FxIdentifierKey
                        {
                            EffectType = pfx.EffectType,
                            DamageType = Enums.eDamage.None,
                            MezType = pfx.MezType,
                            ETModifies = pfx.ETModifies,
                            Summon = pfx.Summon
                        });

                    var gPowerFxIdentifiers = gp.TargetPower.Effects
                        .Select(pfx => new FxIdentifierKey
                        {
                            EffectType = pfx.EffectType,
                            DamageType = Enums.eDamage.None,
                            MezType = pfx.MezType,
                            ETModifies = pfx.ETModifies,
                            Summon = pfx.Summon
                        });

                    if (!bPowerFxIdentifiers.Intersect(gPowerFxIdentifiers).Any()) continue;

                    var fxList = p.Effects.ToList();
                    foreach (var gpFx in gp.TargetPower.Effects)
                    {
                        fxList.Add((IEffect)gpFx.Clone());
                    }
                    p.Effects = fxList.ToArray();
                }
            }
        }

        private static void GBD_Stage(ref IPower tPwr, ref Enums.BuffsX nBuffs, bool enhancementPass)
        {
            if (tPwr == null || tPwr.PowerType == Enums.ePowerType.GlobalBoost)
            {
                return;
            }

            var shortFx = new Enums.ShortFX();
            var sFxSelf = new Enums.ShortFX();

            for (var index1 = 0; index1 < nBuffs.Effect.Length; index1++)
            {
                var iEffect = (Enums.eEffectType) index1;
                if (iEffect == Enums.eEffectType.Damage)
                {
                    continue;
                }

                if (enhancementPass & iEffect != Enums.eEffectType.DamageBuff)
                {
                    shortFx.Assign(tPwr.GetEnhancementMagSum(iEffect, -1));
                }
                else
                {
                    switch (iEffect)
                    {
                        case Enums.eEffectType.MaxRunSpeed:
                            shortFx.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.SpeedRunning, false, false, false, true));
                            sFxSelf.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.MaxRunSpeed, false, true));
                            nBuffs.Effect[(int) Enums.eStatType.MaxRunSpeed] += sFxSelf.Sum;
                            break;
                        case Enums.eEffectType.MaxJumpSpeed:
                            shortFx.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.SpeedJumping, false, false, false, true));
                            sFxSelf.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.MaxJumpSpeed, false, true));
                            nBuffs.Effect[(int) Enums.eStatType.MaxJumpSpeed] += sFxSelf.Sum;
                            break;
                        case Enums.eEffectType.MaxFlySpeed:
                            shortFx.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, false, false, true));
                            sFxSelf.Assign(tPwr.GetEffectMagSum(Enums.eEffectType.MaxFlySpeed, false, true));
                            nBuffs.Effect[(int) Enums.eStatType.MaxFlySpeed] += sFxSelf.Sum;
                            break;
                        default:
                            shortFx.Assign(tPwr.GetEffectMagSum(iEffect));
                            break;
                    }
                }

                for (var shortFxIdx = 0; shortFxIdx < shortFx.Value.Length; shortFxIdx++)
                {
                    if (tPwr.Effects[shortFx.Index[shortFxIdx]].Absorbed_PowerType == Enums.ePowerType.GlobalBoost)
                    {
                        continue;
                    }

                    var effect = tPwr.Effects[shortFx.Index[shortFxIdx]];
                    if (effect.ToWho != Enums.eToWho.Self && effect.ToWho != Enums.eToWho.All)
                    {
                        continue;
                    }

                    if (!enhancementPass)
                    {
                        switch (effect.EffectType)
                        {
                            case Enums.eEffectType.Mez:
                                nBuffs.StatusProtection[(int) effect.MezType] += shortFx.Value[shortFxIdx];
                                break;
                            case Enums.eEffectType.MezResist:
                                nBuffs.StatusResistance[(int) effect.MezType] += shortFx.Value[shortFxIdx];
                                break;
                            case Enums.eEffectType.ResEffect:
                                nBuffs.DebuffResistance[(int) effect.ETModifies] += shortFx.Value[shortFxIdx];
                                break;
                        }
                    }

                    if ((tPwr.Effects[shortFx.Index[shortFxIdx]].EffectType == Enums.eEffectType.DamageBuff | tPwr.Effects[shortFx.Index[shortFxIdx]].EffectType == Enums.eEffectType.Enhancement) & enhancementPass)
                    {
                        switch (effect.ETModifies)
                        {
                            case Enums.eEffectType.Mez:
                                nBuffs.Mez[(int) effect.MezType] += shortFx.Value[shortFxIdx];
                                break;
                            case Enums.eEffectType.Defense:
                                nBuffs.Defense[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                                break;
                            case Enums.eEffectType.Resistance:
                                nBuffs.Resistance[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                                break;
                            default:
                            {
                                if (iEffect == Enums.eEffectType.DamageBuff)
                                {
                                    if (!(effect.isEnhancementEffect & effect.EffectClass == Enums.eEffectClass.Tertiary | effect.ValidateConditional("Active", "Defiance") | effect.SpecialCase == Enums.eSpecialCase.Defiance))
                                    {
                                        nBuffs.Damage[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                                    }
                                }
                                else if (!(effect.ETModifies == Enums.eEffectType.Accuracy & enhancementPass))
                                {
                                    if (effect.ETModifies == Enums.eEffectType.SpeedRunning | effect.ETModifies == Enums.eEffectType.SpeedFlying | effect.ETModifies == Enums.eEffectType.SpeedJumping | effect.ETModifies == Enums.eEffectType.JumpHeight)
                                    {
                                        if (effect.buffMode != Enums.eBuffMode.Debuff)
                                        {
                                            nBuffs.Effect[(int) effect.ETModifies] += shortFx.Value[shortFxIdx];
                                        }
                                        else
                                        {
                                            nBuffs.EffectAux[(int) effect.ETModifies] += shortFx.Value[shortFxIdx];
                                        }
                                    }
                                    else
                                    {
                                        nBuffs.Effect[index1] += shortFx.Value[shortFxIdx];
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else if (effect.EffectType == Enums.eEffectType.Endurance & effect.Aspect == Enums.eAspect.Max)
                    {
                        nBuffs.MaxEnd += shortFx.Value[shortFxIdx];
                    }
                    else if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.Mez & !enhancementPass)
                    {
                        nBuffs.Mez[(int) effect.MezType] += shortFx.Value[shortFxIdx];
                    }
                    else if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.MezResist & !enhancementPass)
                    {
                        nBuffs.MezRes[(int) effect.MezType] += shortFx.Value[shortFxIdx];
                    }
                    else if ((iEffect == Enums.eEffectType.Defense && effect.DamageType != Enums.eDamage.None) & !enhancementPass)
                    {
                        nBuffs.Defense[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                    }
                    else if ((iEffect == Enums.eEffectType.Resistance && effect.DamageType != Enums.eDamage.None) & !enhancementPass)
                    {
                        nBuffs.Resistance[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                    }
                    else if ((iEffect == Enums.eEffectType.Elusivity && effect.DamageType != Enums.eDamage.None) & !enhancementPass)
                    {
                        nBuffs.Elusivity[(int) effect.DamageType] += shortFx.Value[shortFxIdx];
                    }
                    else if (!(effect.ETModifies == Enums.eEffectType.Accuracy & enhancementPass))
                    {
                        if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.Accuracy & !enhancementPass)
                        {
                            nBuffs.Effect[(int)Enums.eStatType.BuffAcc] += shortFx.Value[shortFxIdx];
                        }
                        else if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.SpeedRunning & effect.Aspect == Enums.eAspect.Max)
                        {
                            nBuffs.Effect[(int)Enums.eStatType.MaxRunSpeed] += shortFx.Value[shortFxIdx];
                        }
                        else if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.SpeedFlying & effect.Aspect == Enums.eAspect.Max)
                        {
                            nBuffs.Effect[(int)Enums.eStatType.MaxFlySpeed] += shortFx.Value[shortFxIdx];
                        }
                        else if (effect.EffectType != Enums.eEffectType.ResEffect & effect.ETModifies == Enums.eEffectType.SpeedJumping & effect.Aspect == Enums.eAspect.Max)
                        {
                            nBuffs.Effect[(int)Enums.eStatType.MaxJumpSpeed] += shortFx.Value[shortFxIdx];
                        }
                        else if (iEffect == Enums.eEffectType.ToHit & !enhancementPass)
                        {
                            if (!(effect.isEnhancementEffect & effect.EffectClass == Enums.eEffectClass.Tertiary))
                            {
                                nBuffs.Effect[index1] += shortFx.Value[shortFxIdx];
                            }
                        }
                        else if (!enhancementPass)
                        {
                            // Zed: force absorb to be flat value.
                            // E.g. Bio Armor Parasitic Aura and Ablative Carapace use percentages.
                            // Particle shielding does not.
                            if (index1 == (int)Enums.eStatType.Absorb & effect.DisplayPercentage)
                            {
                                shortFx.Value[shortFxIdx] *= MidsContext.Character.Totals.HPMax; // MidsContext.Character.Archetype.Hitpoints
                            }

                            nBuffs.Effect[index1] += shortFx.Value[shortFxIdx];
                            if (IsClickPower(effect.GetPower()) & !effect.BuildEffectString().Contains("From Enh"))
                            {
                                nBuffs.Effect[index1] -= effect.Mag;
                            }
                        }
                    }
                }
            }
        }

        private static bool IsClickPower(IPower power)
        {
            return power.PowerType == Enums.ePowerType.Click && !power.ClickBuff;
        }

        private void GBD_Totals()
        {
            Totals.Init();
            TotalsCapped.Init();
            var canFly = false;
            for (var index1 = 0; index1 < CurrentBuild.Powers.Count; index1++)
            {
                if (CurrentBuild.Powers[index1] == null) continue;
                if (!(CurrentBuild.Powers[index1].StatInclude & (_buffedPower[index1] != null)))
                {
                    continue;
                }

                if (_buffedPower[index1] == null) continue;
                if (_buffedPower[index1].PowerType == Enums.ePowerType.Toggle)
                {
                    Totals.EndUse += _buffedPower[index1].ToggleCost;
                }

                foreach (var buffedPwr in _buffedPower[index1].Effects)
                {
                    if ((buffedPwr.EffectType == Enums.eEffectType.Fly) & (buffedPwr.Mag > 0.0))
                    {
                        canFly = true;
                    }
                }
            }

            if (Math.Abs(_selfBuffs.Defense[0]) > float.Epsilon)
            {
                for (var index = 1; index < _selfBuffs.Defense.Length; index++)
                {
                    _selfBuffs.Defense[index] += _selfBuffs.Defense[0];
                }
            }

            for (var index = 0; index < _selfBuffs.Defense.Length; index++)
            {
                Totals.Def[index] = _selfBuffs.Defense[index];
                Totals.Res[index] = _selfBuffs.Resistance[index];
                Totals.Elusivity[index] = _selfBuffs.Elusivity[index];
            }

            for (var index = 0; index < _selfBuffs.StatusProtection.Length; index++)
            {
                Totals.Mez[index] = _selfBuffs.StatusProtection[index];
                Totals.MezRes[index] = _selfBuffs.StatusResistance[index] * 100f;
            }

            for (var index = 0; index < _selfBuffs.DebuffResistance.Length; index++)
            {
                Totals.DebuffRes[index] = _selfBuffs.DebuffResistance[index] * 100f;
            }

            Totals.EndMax = _selfBuffs.MaxEnd;
            Totals.BuffAcc = _selfEnhance.Effect[(int)Enums.eStatType.BuffAcc] + _selfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
            Totals.BuffEndRdx = _selfEnhance.Effect[(int)Enums.eStatType.BuffEndRdx];
            Totals.BuffHaste = _selfEnhance.Effect[(int)Enums.eStatType.Haste] + _selfBuffs.Effect[(int)Enums.eStatType.Haste];
            Totals.BuffToHit = _selfBuffs.Effect[(int)Enums.eStatType.ToHit];
            Totals.Perception = Statistics.BasePerception * (1 + _selfBuffs.Effect[(int)Enums.eStatType.Perception]);
            Totals.StealthPvE = _selfBuffs.Effect[(int)Enums.eStatType.StealthPvE];
            Totals.StealthPvP = _selfBuffs.Effect[(int)Enums.eStatType.StealthPvP];
            Totals.ThreatLevel = _selfBuffs.Effect[(int)Enums.eStatType.ThreatLevel];
            Totals.HPRegen = _selfBuffs.Effect[(int)Enums.eStatType.HPRegen];
            Totals.EndRec = _selfBuffs.Effect[(int)Enums.eStatType.EndRec];
            Totals.Absorb = _selfBuffs.Effect[(int)Enums.eStatType.Absorb];

            Totals.FlySpd = Statistics.BaseFlySpeed + Math.Max(_selfBuffs.Effect[(int)Enums.eStatType.FlySpeed], -0.9f) * Statistics.BaseFlySpeed;
            // This number(21.0) looks wrong, like it should match the multiplier above (31.5), changing it
            // Statistics.BaseFlySpeed -> Statistics.BaseRunSpeed, because increasing speed caps do not use 1.5x modifier fly speed gives.
            Totals.MaxFlySpd = Statistics.MaxFlySpeed + _selfBuffs.Effect[(int)Enums.eStatType.MaxFlySpeed] * Statistics.BaseRunSpeed;
            if (Totals.MaxFlySpd > 171.990005493164) // 128.990005493164
                Totals.MaxFlySpd = 171.99f; // 8.19f * 21.0f == 171.99f -- Note: although the cap can reach 8.19, there is currently no way to go beyond 7.1425 (149.99 fps)
            
            Totals.RunSpd = Statistics.BaseRunSpeed + Math.Max(_selfBuffs.Effect[(int)Enums.eStatType.RunSpeed], -0.9f) * Statistics.BaseRunSpeed;
            Totals.MaxRunSpd = Statistics.MaxRunSpeed + _selfBuffs.Effect[(int)Enums.eStatType.MaxRunSpeed] * Statistics.BaseRunSpeed;
            if (Totals.MaxRunSpd > 176.35777) // 135.669998168945
                Totals.MaxRunSpd = 135.67f; // 8.398f * 21.0f == 135.67f
            
            Totals.JumpSpd = (float) (Statistics.BaseJumpSpeed +
                                      (double) Math.Max(_selfBuffs.Effect[(int)Enums.eStatType.JumpSpeed], -0.9f) * Statistics.BaseJumpSpeed);
            Totals.MaxJumpSpd = (float) (114.400001525879 + _selfBuffs.Effect[(int)Enums.eStatType.MaxJumpSpeed] * Statistics.BaseJumpSpeed);
            if (Totals.MaxJumpSpd > 166.256666) // 114.400001525879
                Totals.MaxJumpSpd = 166.257f; // 7.917f * 21.0f // Statistics.MaxJumpSpeed == 114.4f
            
            Totals.JumpHeight = 4 + Math.Max(_selfBuffs.Effect[(int)Enums.eStatType.JumpHeight], -0.9f) * 4;
            
            Totals.HPMax = _selfBuffs.Effect[(int)Enums.eStatType.HPMax] + Archetype.Hitpoints;
            if (!canFly)
                Totals.FlySpd = 0.0f;
            var maxDmgBuff = -1000f;
            var minDmgBuff = -1000f;
            var avgDmgBuff = 0.0f;
            for (var index = 0; index < _selfBuffs.Damage.Length; index++)
            {
                if (0 >= index || index >= 9)
                    continue;
                if (_selfEnhance.Damage[index] > (double) maxDmgBuff)
                {
                    maxDmgBuff = _selfEnhance.Damage[index];
                }

                if (_selfEnhance.Damage[index] < (double) minDmgBuff)
                {
                    minDmgBuff = _selfEnhance.Damage[index];
                }

                avgDmgBuff += _selfEnhance.Damage[index];
            }

            avgDmgBuff /= _selfEnhance.Damage.Length;
            if (maxDmgBuff - (double) avgDmgBuff < avgDmgBuff - (double) minDmgBuff)
            {
                Totals.BuffDam = maxDmgBuff;
            }
            else if ((maxDmgBuff - (double) avgDmgBuff > avgDmgBuff - (double) minDmgBuff) & (minDmgBuff > 0.0))
            {
                Totals.BuffDam = minDmgBuff;
            }
            else
            {
                Totals.BuffDam = maxDmgBuff;
            }

            ApplyPvpDr();
            TotalsCapped.Assign(Totals);
            TotalsCapped.BuffDam = Math.Min(TotalsCapped.BuffDam, Archetype.DamageCap - 1f);
            TotalsCapped.BuffHaste = Math.Min(TotalsCapped.BuffHaste, Archetype.RechargeCap - 1f);
            TotalsCapped.HPRegen = Math.Min(TotalsCapped.HPRegen, Archetype.RegenCap - 1f);
            TotalsCapped.EndRec = Math.Min(TotalsCapped.EndRec, Archetype.RecoveryCap - 1f);
            var num11 = TotalsCapped.Res.Length;
            for (var index = 0; index < num11; index++)
            {
                TotalsCapped.Res[index] = Math.Min(TotalsCapped.Res[index], Archetype.ResCap);
            }

            if (Archetype.HPCap > 0.0)
            {
                TotalsCapped.HPMax = Math.Min(TotalsCapped.HPMax, Archetype.HPCap);
                TotalsCapped.Absorb = Math.Min(TotalsCapped.Absorb, TotalsCapped.HPMax);
            }

            TotalsCapped.RunSpd = Math.Min(TotalsCapped.RunSpd, 135.67f);
            TotalsCapped.JumpSpd = Math.Min(TotalsCapped.JumpSpd, 114.4f);
            TotalsCapped.FlySpd = Math.Min(TotalsCapped.FlySpd, 86f);
            TotalsCapped.Perception = Math.Min(TotalsCapped.Perception, Archetype.PerceptionCap);
        }

        private bool GBPA_AddEnhFX(ref IPower iPower, int iIndex)
        {
            if (MidsContext.Config.I9.IgnoreEnhFX || iIndex < 0 || iPower == null)
            {
                return false;
            }

            for (var index1 = 0; index1 < CurrentBuild.Powers[iIndex].SlotCount; index1++)
            {
                if (CurrentBuild.Powers[iIndex].Slots[index1].Enhancement.Enh <= -1)
                {
                    continue;
                }

                var enhIndex = CurrentBuild.Powers[iIndex].Slots[index1].Enhancement.Enh;
                var enh = DatabaseAPI.Database.Enhancements[enhIndex];
                var power1 = enh?.GetPower();

                var hasPower = enh.Effect.Any(e => e.Mode == Enums.eEffMode.FX);
                if (!hasPower)
                {
                    continue;
                }

                // Procs are actually disabled when turned on...
                // Keep 'always-on' specials active
                if (CurrentBuild.Powers[iIndex].ProcInclude & enh.IsProc)
                {
                    continue;
                }

                if (power1 == null)
                {
                    return false;
                }

                if (enh.IsProc)
                {
                    for (var index2 = 0; index2 < power1.Effects.Length; index2++)
                    {
                        var effect = power1.Effects[index2];
                        var toAdd = (IEffect)effect.Clone();
                        toAdd.isEnhancementEffect = true;
                        toAdd.IgnoreScaling = true;
                        toAdd.ToWho = effect.ToWho;
                        toAdd.Absorbed_Effect = true;
                        toAdd.Ticks = effect.Ticks;
                        toAdd.Buffable = false;
                        iPower.Effects = iPower.Effects.Append(toAdd).ToArray();
                        if (enh.GetPower().Effects[index2].EffectType == Enums.eEffectType.GrantPower)
                        {
                            iPower.HasGrantPowerEffect = true;
                        }
                    }
                }
                else
                {
                    for (var index2 = 0; index2 < power1.Effects.Length; index2++)
                    {
                        var effect = power1.Effects[index2];
                        var toAdd = (IEffect)effect.Clone();
                        toAdd.isEnhancementEffect = true;
                        toAdd.IgnoreScaling = false;
                        toAdd.ToWho = effect.ToWho;
                        toAdd.Absorbed_Effect = true;
                        toAdd.Ticks = effect.Ticks;
                        toAdd.Buffable = false;
                        iPower.Effects = iPower.Effects.Append(toAdd).ToArray();
                        if (enh.GetPower().Effects[index2].EffectType == Enums.eEffectType.GrantPower)
                        {
                            iPower.HasGrantPowerEffect = true;
                        }
                    }
                }
            }

            return true;
        }

        private bool GBPA_AddSubPowerEffects(ref IPower ret, int hIDX)
        {
            if (ret.NIDSubPower.Length <= 0)
                return false;
            var length = ret.Effects.Length;
            if (hIDX < 0)
                return false;
            var effectCount = 0;
            for (var index = 0; index <= CurrentBuild.Powers[hIDX].SubPowers.Length - 1; ++index)
                if ((CurrentBuild.Powers[hIDX].SubPowers[index].nIDPower > -1) &
                    CurrentBuild.Powers[hIDX].SubPowers[index].StatInclude)
                    effectCount += DatabaseAPI.Database.Power[ret.NIDSubPower[index]].Effects.Length;

            var power = ret;
            //var effectArray = (IEffect[]) Utils.CopyArray(power.Effects, new IEffect[ret.Effects.Length + effectCount]);
            var effectArray = new IEffect[ret.Effects.Length + effectCount];
            Array.Copy(power.Effects, effectArray, power.Effects.Length);
            power.Effects = effectArray;
            foreach (var sp in CurrentBuild.Powers[hIDX].SubPowers.Where(sp => sp.nIDPower > -1 && sp.StatInclude))
                for (var index2 = 0; index2 <= DatabaseAPI.Database.Power[sp.nIDPower].Effects.Length - 1; ++index2)
                {
                    ret.Effects[length] = (IEffect) DatabaseAPI.Database.Power[sp.nIDPower].Effects[index2].Clone();
                    ret.Effects[length].Absorbed_EffectID = index2;
                    ret.Effects[length].Absorbed_Effect = true;
                    ret.Effects[length].Absorbed_Power_nID = sp.nIDPower;
                    ret.Effects[length].Absorbed_PowerType = DatabaseAPI.Database.Power[sp.nIDPower].PowerType;
                    ++length;
                }

            return true;
        }

        private void GBPA_ApplyArchetypeCaps(ref IPower powerMath)
        {
            if (powerMath.RechargeTime > (double) Archetype.RechargeCap)
            {
                powerMath.RechargeTime = Archetype.RechargeCap;
            }

            for (var index = 0; index <= powerMath.Effects.Length - 1; ++index)
            {
                if (powerMath.Effects[index].EffectType == Enums.eEffectType.Damage && powerMath.Effects[index].Math_Mag > (double) Archetype.DamageCap)
                {
                    powerMath.Effects[index].Math_Mag = Archetype.DamageCap;
                }
            }
        }

        private static void HandleDefaultIncarnateEnh(ref IPower powerMath, IEffect effect1, IEffect[] buffedPowerEffects)
        {
            for (var index2 = 0; index2 <= powerMath.Effects.Length - 1; ++index2)
            {
                var effect = powerMath.Effects[index2];
                if (!effect.Buffable)
                    continue;

                var duration = 0.0f;
                var mag = 0.0f;
                if (effect.EffectType is Enums.eEffectType.Resistance or Enums.eEffectType.Damage && effect1.EffectType == Enums.eEffectType.DamageBuff)
                {
                    if (effect.DamageType == effect1.DamageType)
                        effect.Math_Mag += effect1.Mag;
                }
                else if (effect.EffectType == effect1.ETModifies)
                {
                    switch (effect1.ETModifies)
                    {
                        case Enums.eEffectType.Damage:
                            if (effect.DamageType == effect1.DamageType)
                                effect.Math_Mag += effect1.Mag;
                            mag = 0.0f;
                            break;
                        case Enums.eEffectType.Defense:
                            if (effect.DamageType == effect1.DamageType)
                                effect.Math_Mag += effect1.Mag;
                            mag = 0.0f;
                            break;
                        case Enums.eEffectType.Mez:
                            if (effect1.MezType == effect.MezType)
                                for (var mezIndex = 0;
                                    mezIndex <= Enum.GetValues(effect.MezType.GetType()).Length - 1;
                                    ++mezIndex)
                                    if (effect.AttribType == Enums.eAttribType.Duration)
                                    {
                                        if (effect.MezType == (Enums.eMez) mezIndex)
                                            effect.Math_Duration += effect1.Mag;
                                        duration = 0.0f;
                                        mag = 0.0f;
                                    }
                                    else if (effect.MezType == (Enums.eMez) mezIndex)
                                    {
                                        effect.Math_Mag += effect1.Mag;
                                        mag = 0.0f;
                                    }

                            break;
                        default:
                            var effect2 = effect;
                            if (effect2.EffectType == Enums.eEffectType.Enhancement &&
                                effect2.ETModifies is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.JumpHeight or Enums.eEffectType.SpeedFlying)
                            {
                                if (buffedPowerEffects[index2].Mag > 0.0)
                                    mag = effect1.Mag;
                                if (buffedPowerEffects[index2].Mag < 0.0) mag = effect1.Mag;

                                break;
                            }

                            if ((effect2.EffectType == Enums.eEffectType.SpeedRunning) |
                                (effect2.EffectType == Enums.eEffectType.SpeedJumping) |
                                (effect2.EffectType == Enums.eEffectType.JumpHeight) |
                                (effect2.EffectType == Enums.eEffectType.SpeedFlying))
                            {
                                if (buffedPowerEffects[index2].Mag > 0.0)
                                    mag = effect1.Mag;
                                if (buffedPowerEffects[index2].Mag < 0.0) mag = effect1.Mag;

                                break;
                            }

                            mag = effect1.Mag;
                            break;
                    }

                    effect.Math_Mag += mag;
                    effect.Math_Duration += duration;
                }
            }
        }

        private static void HandleGrantPowerIncarnate(ref IPower powerMath, IEffect effect1, IReadOnlyList<IPower> buffedPowers, int effIdx, Archetype? at, int hIDX)
        {
            powerMath.AbsorbEffects(DatabaseAPI.Database.Power[effect1.nSummon], effect1.Duration, 0.0f, at, 1, true, effIdx);
            for (var index2 = 0; index2 <= powerMath.Effects.Length - 1; ++index2)
            {
                powerMath.Effects[index2].ToWho = Enums.eToWho.Target;
                powerMath.Effects[index2].Absorbed_Effect = true;
                powerMath.Effects[index2].isEnhancementEffect = effect1.isEnhancementEffect;
                powerMath.Effects[index2].BaseProbability *= effect1.BaseProbability;
                powerMath.Effects[index2].Ticks = effect1.Ticks;
            }

            if (hIDX <= -1) return;
            {
                var length2 = buffedPowers[hIDX].Effects.Length;
                buffedPowers[hIDX].AbsorbEffects(DatabaseAPI.Database.Power[effect1.nSummon], effect1.Duration, 0.0f, at, 1, true, effIdx);
                for (var index2 = length2; index2 <= buffedPowers[hIDX].Effects.Length - 1; ++index2)
                {
                    buffedPowers[hIDX].Effects[index2].ToWho = effect1.ToWho;
                    buffedPowers[hIDX].Effects[index2].Absorbed_Effect = true;
                    buffedPowers[hIDX].Effects[index2].isEnhancementEffect = effect1.isEnhancementEffect;
                    buffedPowers[hIDX].Effects[index2].BaseProbability *= effect1.BaseProbability;
                    buffedPowers[hIDX].Effects[index2].Ticks = effect1.Ticks;
                }
            }
        }

        private void GBPA_ApplyIncarnateEnhancements(ref IPower powerMath, int hIDX, IPower? power, bool ignoreED, ref Enums.eEffectType effectType)
        {
            if (powerMath == null)
                return;
            if (power == null)
                return;
            if (power.Effects.Length == 0)
                return;
            if (!powerMath.Slottable)
                return;

            for (var effIdx = 0; effIdx <= power.Effects.Length - 1; ++effIdx)
            {
                var effect1 = power.Effects[effIdx];
                var disqualified = false;
                if (effect1.EffectClass == Enums.eEffectClass.Ignored)
                {
                    disqualified = true;
                }
                else
                {
                    switch (effectType)
                    {
                        case Enums.eEffectType.Enhancement when effect1.EffectType != Enums.eEffectType.Enhancement && effect1.EffectType != Enums.eEffectType.DamageBuff:
                            disqualified = true;
                            break;
                        case Enums.eEffectType.GrantPower when effect1.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.DamageBuff:
                            disqualified = true;
                            break;
                        default:
                        {
                            if (effect1.IgnoreED != ignoreED)
                            {
                                disqualified = true;
                            }
                            else if (power.PowerType != Enums.ePowerType.GlobalBoost && (!effect1.Absorbed_Effect || effect1.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                            {
                                disqualified = true;
                            }
                            else if (effect1.EffectType == Enums.eEffectType.GrantPower && effect1.Absorbed_Effect)
                            {
                                disqualified = true;
                            }

                            break;
                        }
                    }
                }

                //var disqualified = effect1.EffectClass == Enums.eEffectClass.Ignored || effectType == Enums.eEffectType.Enhancement && effect1.EffectType != Enums.eEffectType.Enhancement && effect1.EffectType != Enums.eEffectType.DamageBuff || effectType == Enums.eEffectType.GrantPower && (effect1.EffectType == Enums.eEffectType.Enhancement || effect1.EffectType == Enums.eEffectType.DamageBuff) || effect1.IgnoreED != ignoreED || power.PowerType != Enums.ePowerType.GlobalBoost && (!effect1.Absorbed_Effect || effect1.Absorbed_PowerType != Enums.ePowerType.GlobalBoost) || effect1.EffectType == Enums.eEffectType.GrantPower && effect1.Absorbed_Effect;

                if (disqualified)
                    continue;
                var power1 = (effect1.Absorbed_Effect & (effect1.Absorbed_Power_nID > -1)) switch
                {
                    true => DatabaseAPI.Database.Power[effect1.Absorbed_Power_nID],
                    _ => power
                };

                //var isAllowed = powerMath.BoostsAllowed.Any(pmb => power1.BoostsAllowed.Any(pba => pba == pmb));
                var isAllowed = powerMath.BoostsAllowed.Intersect(power1.BoostsAllowed).Any();
                if (!isAllowed)
                    continue;
                if (effectType == Enums.eEffectType.Enhancement && effect1.EffectType is Enums.eEffectType.DamageBuff or Enums.eEffectType.Enhancement)
                {
                    var incAcc = powerMath.IgnoreEnhancement(Enums.eEnhance.Accuracy);
                    var incRech = powerMath.IgnoreEnhancement(Enums.eEnhance.RechargeTime);
                    var incEndDisc = powerMath.IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);
                    switch (effect1.ETModifies)
                    {
                        case Enums.eEffectType.Accuracy:
                        {
                            if (incAcc)
                            {
                                powerMath.Accuracy += effect1.BuffedMag;
                            }

                            continue;
                        }
                        case Enums.eEffectType.EnduranceDiscount:
                        {
                            if (incEndDisc)
                            {
                                powerMath.EndCost += effect1.BuffedMag;
                            }

                            continue;
                        }
                        case Enums.eEffectType.InterruptTime:
                            powerMath.InterruptTime += effect1.BuffedMag;
                            continue;
                        case Enums.eEffectType.Range:
                            powerMath.Range += effect1.BuffedMag;
                            continue;
                        case Enums.eEffectType.RechargeTime:
                        {
                            if (incRech)
                            {
                                powerMath.RechargeTime += effect1.BuffedMag;
                            }

                            continue;
                        }
                        default:
                            HandleDefaultIncarnateEnh(ref powerMath, effect1, _buffedPower[hIDX].Effects);
                            break;
                    }
                }
                else if (effect1.EffectType == Enums.eEffectType.GrantPower)
                {
                    HandleGrantPowerIncarnate(ref powerMath, effect1, _buffedPower, effIdx, Archetype, hIDX);
                }
                else
                {
                    powerMath.AbsorbEffects(power, effect1.Duration, 0.0f, Archetype, 1, true, effIdx, effIdx);
                    for (var index2 = powerMath.Effects.Length; index2 <= powerMath.Effects.Length - 1; ++index2)
                    {
                        powerMath.Effects[index2].ToWho = Enums.eToWho.Target;
                        powerMath.Effects[index2].Absorbed_Effect = true;
                        powerMath.Effects[index2].isEnhancementEffect = effect1.isEnhancementEffect;
                        powerMath.Effects[index2].BaseProbability *= effect1.BaseProbability;
                        powerMath.Effects[index2].Ticks = effect1.Ticks;
                    }

                    if (hIDX <= -1)
                    {
                        continue;
                    }


                    var length2 = _buffedPower[hIDX].Effects.Length;
                    _buffedPower[hIDX].AbsorbEffects(power, effect1.Duration, 0.0f, Archetype, 1, true, effIdx, effIdx);
                    for (var index2 = length2; index2 <= _buffedPower[hIDX].Effects.Length - 1; ++index2)
                    {
                        _buffedPower[hIDX].Effects[index2].ToWho = effect1.ToWho;
                        _buffedPower[hIDX].Effects[index2].Absorbed_Effect = true;
                        _buffedPower[hIDX].Effects[index2].isEnhancementEffect = effect1.isEnhancementEffect;
                        _buffedPower[hIDX].Effects[index2].BaseProbability *= effect1.BaseProbability;
                        _buffedPower[hIDX].Effects[index2].Ticks = effect1.Ticks;
                    }
                    
                }
            }
        }

        private IPower GBPA_ApplyPowerOverride(ref IPower ret)
        {
            if (!ret.HasPowerOverrideEffect)
                return ret;
            for (var index = 0; index <= ret.Effects.Length - 1; ++index)
            {
                if (ret.Effects[index].EffectType != Enums.eEffectType.PowerRedirect || !((ret.Effects[index].nOverride > -1) & (Math.Abs(ret.Effects[index].Probability - 1f) < 0.01) & ret.Effects[index].CanInclude()))
                    continue;
                var level = ret.Level;
                ret = new Power(DatabaseAPI.Database.Power[ret.Effects[index].nOverride])
                {
                    Level = level
                };
                return ret;
            }

            return ret;
        }

        private bool GBPA_MultiplyVariable(ref IPower iPower, int hIDX)
        {
            if (iPower == null)
            {
                return false;
            }

            if (hIDX < 0)
            {
                return false;
            }

            if (!iPower.VariableEnabled)
            {
                return false;
            }

            var num = iPower.Effects.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (iPower.Effects[index].VariableModified & !iPower.Effects[index].IgnoreScaling)
                {
                    iPower.Effects[index].Scale *= CurrentBuild.Powers[hIDX].VariableValue;
                }
            }

            return true;
        }

        private bool GBPA_Pass0_InitializePowerArray()
        {
            _buffedPower = new IPower[CurrentBuild.Powers.Count];
            _mathPower = new IPower[CurrentBuild.Powers.Count];
            for (var hIDX = 0; hIDX <= CurrentBuild.Powers.Count - 1; ++hIDX)
            {
                if (CurrentBuild.Powers[hIDX] == null) continue;
                if (CurrentBuild.Powers[hIDX]?.NIDPower > -1)
                {
                    _mathPower[hIDX] = GBPA_SubPass0_AssemblePowerEntry(CurrentBuild.Powers[hIDX].NIDPower, hIDX, 1);
                    //_refPower[hIDX] = GBPA_SubPass0_AssemblePowerEntry(CurrentBuild.Powers[hIDX].NIDPower, hIDX, 1);
                }
            }

            for (var index1 = 0; index1 <= CurrentBuild.Powers.Count - 1; ++index1)
            {
                if (CurrentBuild.Powers[index1] == null || CurrentBuild.Powers[index1]?.NIDPower <= -1)
                    continue;
                var num3 = CurrentBuild.Powers.Count - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    switch ((index1 != index2) & CurrentBuild.Powers[index2]?.StatInclude & (CurrentBuild.Powers[index2]?.NIDPower > -1))
                    {
                        case false:
                            continue;
                        default:
                        {
                            var effectType = Enums.eEffectType.GrantPower;
                            GBPA_ApplyIncarnateEnhancements(ref _mathPower[index1], -1, _mathPower[index2], false,
                                ref effectType);
                            break;
                        }
                    }
                }
            }

            for (var hIDX = 0; hIDX <= CurrentBuild.Powers.Count - 1; ++hIDX)
            {
                if (CurrentBuild.Powers[hIDX] == null || CurrentBuild.Powers[hIDX].NIDPower <= -1)
                    continue;
                GBPA_MultiplyVariable(ref _mathPower[hIDX], hIDX);
                _buffedPower[hIDX] = new Power(_mathPower[hIDX]);
                _buffedPower[hIDX]?.SetMathMag();
            }

            return true;
        }

        private bool GBPA_Pass1_EnhancePreED(ref IPower powerMath, int hIDX)
        {
            if (hIDX < 0)
            {
                return false;
            }

            if (CurrentBuild.Powers[hIDX].NIDPowerset < 0)
            {
                return false;
            }

            powerMath.Accuracy = 0.0f;
            powerMath.EndCost = 0.0f;
            powerMath.InterruptTime = 0.0f;
            powerMath.Range = 0.0f;
            powerMath.RechargeTime = 0.0f;
            foreach (var effect in powerMath.Effects)
            {
                effect.Math_Mag = 0.0f;
                effect.Math_Duration = 0.0f;
            }

            var isAcc = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.Accuracy);
            var isRech = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.RechargeTime);
            var isEnd = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);

            var effectTypeCount = Enum.GetValues(typeof(Enums.eEffectType)).Length;

            for (var index = 0; index < CurrentBuild.Powers[hIDX].SlotCount; index++)
            {
                if (!((CurrentBuild.Powers[hIDX].Slots[index].Enhancement.Enh > -1) & (CurrentBuild.Powers[hIDX].Slots[index].Level < MidsContext.Config.ForceLevel)))
                {
                    continue;
                }

                var enhancement = CurrentBuild.Powers[hIDX].Slots[index].Enhancement;
                if (isAcc)
                {
                    powerMath.Accuracy += enhancement.GetEnhancementEffect(Enums.eEnhance.Accuracy, -1, 1f);
                }

                if (isEnd)
                {
                    powerMath.EndCost += enhancement.GetEnhancementEffect(Enums.eEnhance.EnduranceDiscount, -1, 1f);
                }

                powerMath.InterruptTime += enhancement.GetEnhancementEffect(Enums.eEnhance.Interrupt, -1, 1f);
                powerMath.Range += enhancement.GetEnhancementEffect(Enums.eEnhance.Range, -1, 1f);
                if (isRech)
                {
                    powerMath.RechargeTime += enhancement.GetEnhancementEffect(Enums.eEnhance.RechargeTime, -1, 1f);
                }

                for (var effIdx = 0; effIdx <= powerMath.Effects.Length - 1; ++effIdx)
                {
                    if (!powerMath.Effects[effIdx].Buffable)
                    {
                        continue;
                    }

                    for (var index2 = 0; index2 < effectTypeCount; index2++)
                    {
                        if (powerMath.Effects[effIdx].EffectType != (Enums.eEffectType)index2)
                        {
                            continue;
                        }

                        var eEnhance = Enums.eEnhance.None;
                        var num6 = 0.0f;
                        var eEffectType2 = (Enums.eEffectType)index2;
                        var flag6 = Enums.IsEnumValue(Enum.GetName(typeof(Enums.eEffectType), eEffectType2), eEnhance);
                        
                        var flag7 = false;
                        if (!flag6)
                        {
                            if ((powerMath.Effects[effIdx].EffectType == Enums.eEffectType.Enhancement) & (powerMath.Effects[effIdx].ETModifies == Enums.eEffectType.Accuracy))
                            {
                                flag6 = true;
                                flag7 = true;
                            }
                            else if ((powerMath.Effects[effIdx].EffectType == Enums.eEffectType.ResEffect) & (powerMath.Effects[effIdx].ETModifies == Enums.eEffectType.Defense))
                            {
                                flag6 = true;
                            }
                        }

                        if (!flag6)
                        {
                            continue;
                        }

                        Enums.eEnhance iEffect;
                        if (!flag7)
                        {
                            iEffect = (Enums.eEnhance)Enums.StringToFlaggedEnum(Enum.GetName(typeof(Enums.eEffectType), eEffectType2), eEnhance);
                        }
                        else
                        {
                            iEffect = Enums.eEnhance.Accuracy;
                        }

                        float num7;
                        if (eEffectType2 != Enums.eEffectType.Mez)
                        {
                            if (!((eEffectType2 == Enums.eEffectType.ResEffect) & (powerMath.Effects[effIdx].ETModifies == Enums.eEffectType.Defense)))
                            {
                                num7 = enhancement.GetEnhancementEffect(iEffect, -1, _buffedPower[hIDX].Effects[effIdx].Math_Mag);
                            }
                            else
                            {
                                num7 = enhancement.GetEnhancementEffect(Enums.eEnhance.Defense, -1, _buffedPower[hIDX].Effects[effIdx].Math_Mag);
                            }
                        }
                        else
                        {
                            num7 = enhancement.GetEnhancementEffect(iEffect, (int)powerMath.Effects[effIdx].MezType, _buffedPower[hIDX].Effects[effIdx].Math_Mag);
                        }

                        if ((eEffectType2 == Enums.eEffectType.Damage) & (powerMath.Effects[effIdx].DamageType == Enums.eDamage.Special))
                        {
                            num7 = 0.0f;
                        }
                        else if (eEffectType2 == Enums.eEffectType.Mez && powerMath.Effects[effIdx].AttribType == Enums.eAttribType.Duration)
                        {
                            num6 = num7;
                            num7 = 0.0f;
                        }
                        
                        powerMath.Effects[effIdx].Math_Mag += num7;
                        powerMath.Effects[effIdx].Math_Duration += num6;
                    }
                }
            }

            var num8 = CurrentBuild.Powers.Count - 1;
            for (var index = 0; index <= num8; ++index)
            {
                if (CurrentBuild.Powers[index] == null) continue;
                if (!(CurrentBuild.Powers[index].StatInclude & (CurrentBuild.Powers[index].NIDPower > -1))) continue;
                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPower[index], false, ref effectType);
            }

            return false;
        }

        private static bool GBPA_Pass2_ApplyED(ref IPower powerMath)
        {
            powerMath.Accuracy = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Accuracy), powerMath.Accuracy);
            powerMath.EndCost = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.EnduranceDiscount), powerMath.EndCost);
            powerMath.InterruptTime = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Interrupt), powerMath.InterruptTime);
            powerMath.Range = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Range), powerMath.Range);
            powerMath.RechargeTime = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.RechargeTime), powerMath.RechargeTime);
            foreach (var eff in powerMath.Effects)
            {
                if (eff.isEnhancementEffect) continue;
                var num3 = Enum.GetValues(Enums.eEffectType.None.GetType()).Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    if (eff.EffectType != (Enums.eEffectType) index2) continue;
                    var eEnhance = Enums.eEnhance.None;
                    var eEffectType = (Enums.eEffectType) index2;
                    var isOk = Enums.IsEnumValue(Enum.GetName(eEffectType.GetType(), eEffectType), eEnhance);
                    var isSpecial = false;
                    if (!isOk)
                    {
                        if ((eff.EffectType == Enums.eEffectType.Enhancement) & (eff.ETModifies == Enums.eEffectType.Accuracy))
                        {
                            isOk = true;
                            isSpecial = true;
                        }
                        else if ((eff.EffectType == Enums.eEffectType.ResEffect) & (eff.ETModifies == Enums.eEffectType.Defense))
                        {
                            isOk = true;
                        }
                    }

                    if (!isOk) continue;
                    Enums.eEnhance iEnh;
                    if (!isSpecial)
                    {
                        iEnh = (Enums.eEnhance)Enums.StringToFlaggedEnum(Enum.GetName(eEffectType.GetType(), eEffectType), eEnhance);
                    }
                    else
                    {
                        iEnh = Enums.eEnhance.Accuracy;
                    }

                    if (eEffectType == Enums.eEffectType.Mez)
                    {
                        eff.Math_Mag = Enhancement.ApplyED(Enhancement.GetSchedule(iEnh, (int) eff.MezType), eff.Math_Mag);
                        eff.Math_Duration = Enhancement.ApplyED(Enhancement.GetSchedule(iEnh, (int) eff.MezType), eff.Math_Duration);
                    }
                    else
                    {
                        if (!((eEffectType == Enums.eEffectType.ResEffect) & (eff.ETModifies == Enums.eEffectType.Defense)))
                        {
                            eff.Math_Mag = Enhancement.ApplyED(Enhancement.GetSchedule(iEnh), eff.Math_Mag);
                        }
                        else
                        {
                            eff.Math_Mag = Enhancement.ApplyED(Enhancement.GetSchedule(Enums.eEnhance.Defense), eff.Math_Mag);
                        }
                    }
                }
            }

            return true;
        }

        private bool GBPA_Pass3_EnhancePostED(ref IPower powerMath, int hIDX)
        {
            var okAcc = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.Accuracy);
            var okRecharge = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.RechargeTime);
            var okEnd = DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].IgnoreEnhancement(Enums.eEnhance.EnduranceDiscount);
            var num1 = _selfEnhance.Effect.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var eEffectType = (Enums.eEffectType) index1;
                switch (eEffectType)
                {
                    case Enums.eEffectType.Accuracy:
                        if (okAcc)
                        {
                            powerMath.Accuracy += _selfEnhance.Effect[index1];
                        }

                        break;
                    case Enums.eEffectType.EnduranceDiscount:
                        if (okEnd)
                        {
                            powerMath.EndCost += _selfEnhance.Effect[index1];
                        }

                        break;
                    case Enums.eEffectType.InterruptTime:
                        powerMath.InterruptTime += _selfEnhance.Effect[index1];
                        break;
                    case Enums.eEffectType.Range:
                        powerMath.Range += _selfEnhance.Effect[index1];
                        break;
                    case Enums.eEffectType.RechargeTime:
                        if (okRecharge)
                        {
                            powerMath.RechargeTime += _selfEnhance.Effect[index1];
                        }

                        break;
                    default:
                        var num2 = powerMath.Effects.Length - 1;
                        for (var index2 = 0; index2 <= num2; ++index2)
                        {
                            if (!powerMath.Effects[index2].Buffable)
                            {
                                continue;
                            }

                            var num3 = 0.0f;
                            var mag = 0.0f;
                            if (powerMath.Effects[index2].EffectType != eEffectType)
                            {
                                continue;
                            }

                            switch (eEffectType)
                            {
                                case Enums.eEffectType.Damage:
                                    var num5 = Enum.GetValues(powerMath.Effects[index2].DamageType.GetType()).Length - 1;
                                    for (var index3 = 0; index3 <= num5; ++index3)
                                    {
                                        if (powerMath.Effects[index2].DamageType == (Enums.eDamage) index3)
                                        {
                                            powerMath.Effects[index2].Math_Mag += _selfEnhance.Damage[(int) powerMath.Effects[index2].DamageType];
                                        }
                                    }

                                    mag = 0.0f;
                                    break;
                                case Enums.eEffectType.Defense:
                                    for (var dmgTypeIndex = 0; dmgTypeIndex <= Enum.GetValues(powerMath.Effects[index2].DamageType.GetType()).Length - 1; ++dmgTypeIndex)
                                    {
                                        if (powerMath.Effects[index2].DamageType == (Enums.eDamage) dmgTypeIndex)
                                        {
                                            powerMath.Effects[index2].Math_Mag += _selfEnhance.Defense[(int) powerMath.Effects[index2].DamageType];
                                        }
                                    }

                                    mag = 0.0f;
                                    break;
                                case Enums.eEffectType.Mez:
                                    var num7 = Enum.GetValues(powerMath.Effects[index2].MezType.GetType()).Length - 1;
                                    for (var index3 = 0; index3 <= num7; ++index3)
                                    {
                                        if (powerMath.Effects[index2].AttribType == Enums.eAttribType.Duration)
                                        {
                                            if (powerMath.Effects[index2].MezType == (Enums.eMez) index3)
                                            {
                                                powerMath.Effects[index2].Math_Duration +=
                                                    _selfEnhance.Mez[(int) powerMath.Effects[index2].MezType];
                                            }

                                            num3 = 0.0f;
                                            mag = 0.0f;
                                        }
                                        else if (powerMath.Effects[index2].MezType == (Enums.eMez) index3)
                                        {
                                            powerMath.Effects[index2].Math_Mag +=
                                                _selfEnhance.Mez[(int) powerMath.Effects[index2].MezType];
                                            mag = 0.0f;
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Resistance:
                                    for (var dmgTypeIndex = 0; dmgTypeIndex <= Enum.GetValues(powerMath.Effects[index2].DamageType.GetType()).Length - 1; ++dmgTypeIndex)
                                    {
                                        if (powerMath.Effects[index2].DamageType == (Enums.eDamage) dmgTypeIndex)
                                        {
                                            powerMath.Effects[index2].Math_Mag += _selfEnhance.Resistance[(int) powerMath.Effects[index2].DamageType];
                                        }
                                    }

                                    break;
                                default:
                                    var effect = powerMath.Effects[index2];
                                    if ((effect.EffectType == Enums.eEffectType.Enhancement) & ((effect.ETModifies == Enums.eEffectType.SpeedRunning) | (effect.ETModifies == Enums.eEffectType.SpeedJumping) | (effect.ETModifies == Enums.eEffectType.JumpHeight) | (effect.ETModifies == Enums.eEffectType.SpeedFlying)))
                                    {
                                        if (_buffedPower[hIDX].Effects[index2].Mag > 0.0)
                                        {
                                            mag = _selfEnhance.Effect[(int) effect.ETModifies];
                                        }

                                        if (_buffedPower[hIDX].Effects[index2].Mag < 0.0)
                                        {
                                            mag = _selfEnhance.EffectAux[(int) effect.ETModifies];
                                        }

                                        break;
                                    }

                                    if ((effect.EffectType == Enums.eEffectType.SpeedRunning) | (effect.EffectType == Enums.eEffectType.SpeedJumping) | (effect.EffectType == Enums.eEffectType.JumpHeight) | (effect.EffectType == Enums.eEffectType.SpeedFlying))
                                    {
                                        if (_buffedPower[hIDX].Effects[index2].Mag > 0.0)
                                        {
                                            mag = _selfEnhance.Effect[(int) effect.EffectType];
                                        }

                                        if (_buffedPower[hIDX].Effects[index2].Mag < 0.0)
                                        {
                                            mag = _selfEnhance.EffectAux[(int) effect.EffectType];
                                        }

                                        break;
                                    }

                                    mag = _selfEnhance.Effect[index1];
                                    break;
                            }

                            powerMath.Effects[index2].Math_Mag += mag;
                            powerMath.Effects[index2].Math_Duration += num3;
                        }

                        break;
                }
            }

            for (var index = 0; index <= CurrentBuild.Powers.Count - 1; ++index)
            {
                if (CurrentBuild.Powers[index] == null) continue;
                if (!(CurrentBuild.Powers[index].StatInclude & (CurrentBuild.Powers[index].NIDPower > -1)))
                    continue;
                var effectType = Enums.eEffectType.Enhancement;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, hIDX, _mathPower[index], true, ref effectType);
            }

            return true;
        }

        private static bool GBPA_Pass4_Add(ref IPower powerMath)
        {
            ++powerMath.EndCost;
            ++powerMath.InterruptTime;
            ++powerMath.Range;
            ++powerMath.RechargeTime;
            foreach (var effect in powerMath.Effects)
            {
                ++effect.Math_Mag;
                ++effect.Math_Duration;
            }
            return true;
        }

        private static void GBPA_Pass5_ResyncEffects(ref IPower powerMath, ref IPower powerBuffed)
        {
            var l = Math.Min(powerMath.Effects.Length, powerBuffed.Effects.Length);
            var fxMath = powerMath.Effects.ToList();
            var fxBuffed = powerBuffed.Effects.ToList();
            for (var i = 0; i < l; i++)
            {
                if (fxMath[i].EffectType == fxBuffed[i].EffectType & fxMath[i].DamageType == fxBuffed[i].DamageType & fxMath[i].MezType == fxBuffed[i].MezType & fxMath[i].ETModifies == fxBuffed[i].ETModifies & fxMath[i].Summon == fxBuffed[i].Summon)
                {
                    continue;
                }

                fxMath.RemoveAt(i);
            }

            powerMath.Effects = fxMath.ToArray();
        }

        private static bool GBPA_Pass5_MultiplyPreBuff(ref IPower powerMath, ref IPower powerBuffed)
        {
            if (powerBuffed == null) return false;
            powerBuffed.EndCost /= powerMath.EndCost;
            powerBuffed.InterruptTime /= powerMath.InterruptTime;
            powerBuffed.Range *= powerMath.Range;
            powerBuffed.RechargeTime /= powerMath.RechargeTime;

            if (powerMath.Effects.Length > powerBuffed.Effects.Length)
            {
                GBPA_Pass5_ResyncEffects(ref powerMath, ref powerBuffed);
            }

            for (var index = 0; index < powerMath.Effects.Length; index++)
            {
                powerBuffed.Effects[index].Math_Mag = powerBuffed.Effects[index].Mag * powerMath.Effects[index].Math_Mag;
                powerBuffed.Effects[index].Math_Duration = powerBuffed.Effects[index].Duration * powerMath.Effects[index].Math_Duration;
            }

            return true;
        }

        private bool GBPA_Pass6_MultiplyPostBuff(ref IPower powerMath, ref IPower powerBuffed)
        {
            if (powerMath == null) return false;
            if (powerBuffed == null) return false;
            if (MidsContext.Config is null) return false;
            var nToHit = !powerMath.IgnoreBuff(Enums.eEnhance.ToHit) ? 0.0f : _selfBuffs.Effect[(int)Enums.eStatType.ToHit];
            var nAcc = !powerMath.IgnoreBuff(Enums.eEnhance.Accuracy) ? 0.0f : _selfBuffs.Effect[(int)Enums.eStatType.BuffAcc];
            powerBuffed.Accuracy = (float) (powerBuffed.Accuracy * (1.0 + powerMath.Accuracy + nAcc) * (MidsContext.Config.ScalingToHit + (double) nToHit));
            powerBuffed.AccuracyMult = powerBuffed.Accuracy * (1f + powerMath.Accuracy + nAcc);
            return true;
        }

        private IPower GBPA_SubPass0_AssemblePowerEntry(int nIDPower, int hIDX, int stackingOverride = -1)
        {
            if (nIDPower < 0)
                return null;
            IPower power2 = new Power(DatabaseAPI.Database.Power[nIDPower]);
            GBPA_ApplyPowerOverride(ref power2);
            GBPA_AddEnhFX(ref power2, hIDX);
            power2.AbsorbPetEffects(hIDX, stackingOverride);
            power2.ApplyGrantPowerEffects();
            GBPA_AddSubPowerEffects(ref power2, hIDX);
            return power2;
        }

        private void GenerateBuffData(ref Enums.BuffsX nBuffs, bool enhancementPass)
        {
            for (var i = 0; i < CurrentBuild.Powers.Count; i++)
            {
                if (CurrentBuild.Powers[i] == null) continue;
                if (!(CurrentBuild.Powers[i].StatInclude & (CurrentBuild.Powers[i].NIDPower > -1)) || DatabaseAPI.Database.Power[CurrentBuild.Powers[i].NIDPower].PowerType == Enums.ePowerType.GlobalBoost)
                    continue;

                if (_buffedPower[i] == null) continue;
                GBD_Stage(ref _buffedPower[i], ref nBuffs, enhancementPass);
            }

            var bonusVirtualPower = CurrentBuild.SetBonusVirtualPower;
            GBD_Stage(ref bonusVirtualPower, ref nBuffs, enhancementPass);
            if (!MidsContext.Config.Inc.DisablePvE)
                return;
            var index1 = DatabaseAPI.NidFromUidPower("Temporary_Powers.Temporary_Powers.PVP_Resist_Bonus");
            if (index1 <= -1)
                return;
            IPower tPwr = new Power(DatabaseAPI.Database.Power[index1]);
            GBD_Stage(ref tPwr, ref nBuffs, enhancementPass);
        }

        public void GenerateBuffedPowerArray()
        {
            CurrentBuild.GenerateSetBonusData();
            _selfBuffs.Reset();
            _selfEnhance.Reset();
            ModifyEffects = new Dictionary<string, float>();
            _buffedPower = new IPower[CurrentBuild.Powers.Count];
            _mathPower = new IPower[CurrentBuild.Powers.Count];

            GBPA_Pass0_InitializePowerArray();

            GenerateModifyEffectsArray();
            GenerateBuffData(ref _selfEnhance, true);
            for (var hIDX = 0; hIDX < _mathPower.Length; hIDX++)
            {
                if (_mathPower[hIDX] == null)
                {
                    continue;
                }

                GBPA_Pass1_EnhancePreED(ref _mathPower[hIDX], hIDX);
                GBPA_Pass2_ApplyED(ref _mathPower[hIDX]);
                GBPA_Pass3_EnhancePostED(ref _mathPower[hIDX], hIDX);
                GBPA_Pass4_Add(ref _mathPower[hIDX]);
                GBPA_ApplyArchetypeCaps(ref _mathPower[hIDX]);
                GBPA_Pass5_MultiplyPreBuff(ref _mathPower[hIDX], ref _buffedPower[hIDX]);
            }

            GenerateBuffData(ref _selfBuffs, false);

            for (var index = 0; index < _mathPower.Length; index++)
            {
                if (_mathPower[index] != null)
                {
                    GBPA_Pass6_MultiplyPostBuff(ref _mathPower[index], ref _buffedPower[index]);
                }
            }

            ApplyGlobalEnhancements();
            GBD_Totals();
        }

        /// <summary>
        /// Generate MathPower, BuffedPower for the listed powers.
        /// These are going to be assimilated into build power # basePowerHistoryIdx in order to apply buff calculations
        /// This is typically used for pet powers that normally do not receive buffs from build.
        /// </summary>
        /// <param name="powers">List of powers to process</param>
        /// <param name="basePowerHistoryIdx">Index in build history of the power to attach to</param>
        /// <returns>KeyValuePair(keys=MathPower, values=BuffedPower</returns>
        public KeyValuePair<List<IPower>, List<IPower>>? GenerateBuffedPowers(List<IPower> powers, int basePowerHistoryIdx)
        {
            if (basePowerHistoryIdx < 0)
            {
                // If root power is not picked in build, get unbuffed powers data from the db directly.
                var powersList = powers
                    .Select(e => DatabaseAPI.Database.Power.DefaultIfEmpty(new Power { StaticIndex = -1 }).FirstOrDefault(f => f?.StaticIndex == e.StaticIndex))
                    .Cast<IPower>()
                    .ToList();

                return new KeyValuePair<List<IPower>, List<IPower>>(powersList.Clone(), powersList.Clone());
            }

            var mathPowers = new List<IPower>();
            var buffedPowers = new List<IPower>();
            powers.Add(CurrentBuild.Powers[basePowerHistoryIdx].Power.Clone()); // Restore original attached power
            for (var i = 0; i < powers.Count; i++)
            {
                CurrentBuild.Powers[basePowerHistoryIdx].NIDPower = DatabaseAPI.Database.Power.TryFindIndex(e => e.StaticIndex == powers[i].StaticIndex);
                GenerateBuffedPowerArray();

                if (i >= powers.Count - 1)
                {
                    continue;
                }

                mathPowers.Add(_mathPower[basePowerHistoryIdx].Clone());
                buffedPowers.Add(_buffedPower[basePowerHistoryIdx].Clone());
            }

            return new KeyValuePair<List<IPower>, List<IPower>>(mathPowers, buffedPowers);
        }

        private void GenerateModifyEffectsArray()
        {
            for (var index = 0; index <= CurrentBuild.Powers.Count - 1; ++index)
            {
                if (CurrentBuild.Powers[index] == null) continue;
                if (!(CurrentBuild.Powers[index].StatInclude & (CurrentBuild.Powers[index].NIDPower > -1)) || _buffedPower[index] == null)
                    continue;
                foreach (var effect in _buffedPower[index].Effects)
                {
                    if (_buffedPower[index] == null) continue;
                    if (!((effect.EffectType == Enums.eEffectType.GlobalChanceMod) & !string.IsNullOrEmpty(effect.Reward)))
                        continue;
                    if (ModifyEffects.ContainsKey(effect.Reward))
                    {
                        Dictionary<string, float>? modifyEffects;
                        string reward;
                        (modifyEffects = ModifyEffects)[reward = effect.Reward] = modifyEffects[reward] + effect.Scale;
                    }
                    else
                    {
                        ModifyEffects[effect.Reward] = effect.Scale;
                    }
                }
            }

            if (CurrentBuild.SetBonusVirtualPower == null)
                return;
            foreach (var effect in CurrentBuild.SetBonusVirtualPower.Effects)
            {
                if (!((effect.EffectType == Enums.eEffectType.GlobalChanceMod) & !string.IsNullOrEmpty(effect.Reward)))
                    continue;
                if (ModifyEffects.ContainsKey(effect.Reward))
                {
                    Dictionary<string, float>? modifyEffects;
                    string reward;
                    (modifyEffects = ModifyEffects)[reward = effect.Reward] = modifyEffects[reward] + effect.Scale;
                }
                else
                {
                    ModifyEffects[effect.Reward] = effect.Scale;
                }
            }
        }

        public IPower GetBasePower(int iPower, int nIDPower = -1)
        {
            if (iPower > -1)
            {
                if (CurrentBuild.Powers[iPower] != null)
                {
                    if (CurrentBuild.Powers.Count - 1 < iPower || CurrentBuild.Powers[iPower].NIDPower < 0)
                    {
                        return null;
                    }

                    nIDPower = CurrentBuild.Powers[iPower].NIDPower;
                }
            }
            else if (nIDPower <= -1 || nIDPower > DatabaseAPI.Database.Power.Length - 1)
            {
                return null;
            }

            var powerMath = GBPA_SubPass0_AssemblePowerEntry(nIDPower, iPower);
            /*for (var index = 0; index < CurrentBuild.Powers.Count; index++)
            {
                if (CurrentBuild.Powers[index] == null)
                {
                    continue;
                }

                if (!(iPower != index & CurrentBuild.Powers[index].StatInclude &
                      CurrentBuild.Powers[index].NIDPower > -1 & index < _mathPower.Length))
                {
                    continue;
                }

                var effectType = Enums.eEffectType.GrantPower;
                GBPA_ApplyIncarnateEnhancements(ref powerMath, -1, _mathPower[index], false,
                    ref effectType);
            }*/

            powerMath = new Power(DatabaseAPI.GetPowerByFullName(powerMath.FullName));
            powerMath?.ProcessExecutes();
            powerMath?.AbsorbPetEffects();

            return powerMath;
        }

        private static int GetClassByName(string iName)
        {
            foreach (var enhCls in DatabaseAPI.Database.EnhancementClasses)
            {
                if (string.Equals(enhCls.ShortName, iName, StringComparison.OrdinalIgnoreCase))
                    return enhCls.ID;
                if (!string.Equals(enhCls.Name, iName, StringComparison.OrdinalIgnoreCase))
                    continue;
                return enhCls.ID;
            }

            return -1;
        }

        public IPower GetEnhancedPower(int iPower)
        {
            if (!((iPower < 0) | (_buffedPower.Length - 1 < iPower)))
                return _buffedPower[iPower];
            else
                return null;
        }

        public int[] GetEnhancements(int iPowerSlot)
        {
            if (!(iPowerSlot < 0 || iPowerSlot >= CurrentBuild.Powers.Count) &&
                CurrentBuild.Powers[iPowerSlot].SlotCount > 0)
                return CurrentBuild.Powers[iPowerSlot].Slots.Select(slot => slot.Enhancement.Enh).ToArray();

            return Array.Empty<int>();
        }

        private bool ImportInternalDataUC(StreamReader iStream, float nVer)
        {
            float olderFile;
            if (nVer < 1.0)
            {
                olderFile = nVer;
                if (olderFile >= 0.899999976158142 && olderFile < 1.0)
                    olderFile = 0.0f;
                MessageBox.Show(@"The data being loaded was saved by either an older version of the application, or a different rendition of the application and may not load correctly.", @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olderFile = 0.0f;
            }

            // create method before any other variables are declared, inside other method to reduce class clutter
            // also to lower cognitive load, this method depends on local properties, not the huge amount of variables present in the containing/calling method
            // only 1 variable closed over: olderFile
            ReadOnlyCollection<int> readPowerEntries(string[] data)
            {
                var count = Convert.ToInt32(data[0]);
                var offset = 1;
                var indexLookup = new int[count + 1];
                for (var index2 = 0; index2 <= count; ++index2)
                {
                    var tPower = new PowerEntry
                    {
                        Level = Convert.ToInt32(data[offset])
                    };
                    offset += 1;
                    tPower.NIDPowerset = Math.Abs(olderFile - 0.100000001490116) > float.Epsilon
                        ? Convert.ToInt32(data[offset]) >= 0.0
                            ? Powersets[Convert.ToInt32(data[offset])].nID
                            : -1
                        : Convert.ToInt32(data[offset]);
                    offset += 1;
                    tPower.IDXPower = Convert.ToInt32(data[offset]);
                    tPower.NIDPower = !((tPower.NIDPowerset > -1) & (tPower.IDXPower > -1))
                        ? -1
                        : DatabaseAPI.Database.Powersets[tPower.NIDPowerset].Power[tPower.IDXPower];
                    if (Math.Abs(olderFile) < float.Epsilon || olderFile >= 1.0)
                    {
                        offset += +1;
                        tPower.StatInclude = Math.Abs(Convert.ToInt32(data[offset])) > float.Epsilon;
                        offset = offset + 1 + 1;
                    }

                    offset += 1;
                    indexLookup[index2] = tPower.NIDPower;
                    if (tPower.PowerSet.SetType == Enums.ePowerSetType.Inherent ||
                        (tPower.NIDPowerset == Powersets[1].nID) & (tPower.IDXPower == 0))
                        continue;
                    RequestedLevel = tPower.Level;
                    BuildPower(tPower.NIDPowerset, tPower.NIDPower, true);
                }

                return new ReadOnlyCollection<int>(indexLookup.ToList());
            }

            void readSlotEntries(string[] data, ReadOnlyCollection<int> idxLookup)
            {
                var offset = 1; // previously line 4665
                for (var index2 = 0; index2 <= Convert.ToInt32(data[0]); ++index2)
                {
                    var slotEntry = new SlotEntry
                    {
                        Level = Convert.ToInt32(data[offset])
                    };
                    offset += 2;
                    var tPowerID = Convert.ToInt32(data[offset]);
                    offset += 1;
                    var tSlotIdx = Convert.ToInt32(data[offset]);
                    offset += 1;
                    if (olderFile > 0.0 && olderFile < 1.0)
                        offset += 2;
                    if (olderFile > 0.0 && olderFile < 1.10000002384186)
                    {
                        slotEntry.Enhancement = new I9Slot
                        {
                            Enh = DatabaseAPI.GetFirstValidEnhancement(GetClassByName(data[offset])),
                            Grade = MidsContext.Config.CalcEnhOrigin,
                            RelativeLevel = MidsContext.Config.CalcEnhLevel,
                            IOLevel = MidsContext.Config.I9.DefaultIOLevel
                        };
                        slotEntry.FlippedEnhancement = new I9Slot();
                    }
                    else
                    {
                        slotEntry.LoadFromString(data[offset], ":");
                    }

                    offset += 1;
                    if (tPowerID <= -1 || idxLookup[tPowerID] <= -1)
                        continue;
                    var nSlotID = 0;
                    var inToonHistory = CurrentBuild.FindInToonHistory(idxLookup[tPowerID]); // AKA nPowerID
                    if (tSlotIdx > 0)
                        nSlotID = BuildSlot(inToonHistory);
                    else if (tSlotIdx == 0)
                        nSlotID = tSlotIdx;
                    if (!((inToonHistory > -1) & (inToonHistory < CurrentBuild.Powers.Count)) ||
                        !((CurrentBuild.Powers[inToonHistory].Slots.Length > nSlotID) & (nSlotID > -1)))
                        continue;
                    var slot = CurrentBuild.Powers[inToonHistory].Slots[nSlotID];
                    slot.Enhancement.Enh = slotEntry.Enhancement.Enh;
                    slot.Enhancement.Grade = slotEntry.Enhancement.Grade;
                    slot.Enhancement.IOLevel = slotEntry.Enhancement.IOLevel;
                    slot.Enhancement.RelativeLevel = slotEntry.Enhancement.RelativeLevel;
                    // is this a bug from the original source? why wouldn't it be slot.Level = slotEntry.Level?
                    // commented out setter that does nothing
                    //slot.Level = slot.Level;
                    slot.FlippedEnhancement.Enh = slotEntry.FlippedEnhancement.Enh;
                    slot.FlippedEnhancement.Grade = slotEntry.FlippedEnhancement.Grade;
                    slot.FlippedEnhancement.IOLevel = slotEntry.FlippedEnhancement.IOLevel;
                    slot.FlippedEnhancement.RelativeLevel = slotEntry.FlippedEnhancement.RelativeLevel;
                }
            }

            // why are these two being saved then set back later, is something mutating them?
            var buildMode = MidsContext.Config.BuildMode;
            var buildOption = MidsContext.Config.BuildOption;
            MidsContext.Config.BuildMode = Enums.dmModes.Normal;
            MidsContext.Config.BuildOption = Enums.dmItem.Slot;
            var ret = IoGrab2(iStream);
            Name = ret[0];
            Archetype = DatabaseAPI.GetArchetypeByName(ret[2]);
            Origin = DatabaseAPI.GetOriginByName(Archetype, ret[1]);

            {
                // stage: powersets
                var setData = IoGrab2(iStream);
                for (var index = 0; index <= Powersets.Length - 1; ++index)
                {
                    Powersets[index] = DatabaseAPI.GetPowersetByName(setData[index], Archetype.DisplayName);
                    if (setData[index].IndexOf("Inherent", StringComparison.Ordinal) > -1)
                        Powersets[index] = DatabaseAPI.GetInherentPowerset();
                }
            }

            {
                // stage: poolLocks
                var poolData = IoGrab2(iStream);
                for (var index = 0; index <= PoolLocked.Length - 1; ++index)
                    PoolLocked[index] = Math.Abs(Convert.ToInt32(poolData[index + 1])) > float.Epsilon;
            }

            // fast forward stream
            IoGrab2(iStream);
            // fast forward stream
            IoGrab2(iStream);
            NewBuild(); // NewHistory()
            // stage: read powerEntries
            var lookup = readPowerEntries(IoGrab2(iStream));
            // stage: read slots
            readSlotEntries(IoGrab2(iStream), lookup);
            // stage: wrap up
            if (Archetype == null)
                Archetype = DatabaseAPI.Database.Classes[0];
            if (Origin > Archetype.Origin.Length - 1)
                Origin = Archetype.Origin.Length - 1;
            Lock();
            PoolShuffle();
            Validate();
            MidsContext.Config.BuildMode = buildMode;
            MidsContext.Config.BuildOption = buildOption;
            MidsContext.Archetype = Archetype;
            return true;
        }

        private static string[]? IoGrab2(StreamReader iStream, string delimiter = ";", char fakeLf = '\0')
        {
            var str = FileIO.ReadLineUnlimited(iStream, fakeLf);
            var strArray = str.Split(Convert.ToChar(delimiter));
            if (strArray.Length < 2)
                strArray = str.Split(';');
            for (var index = 0; index <= strArray.Length - 1; ++index)
                strArray[index] = FileIO.IOStrip(strArray[index]);
            return strArray;
        }

        public bool Load(string iFileName, ref Stream? mStream)
        {
            var buildString = "";
            
            // From file
            if (mStream == null || !string.IsNullOrEmpty(iFileName))
            {
                mStream = new FileStream(iFileName, FileMode.Open, FileAccess.Read);
                buildString = File.ReadAllText(iFileName);
            }
            // From clipboard (old datachunk)
            else if (Clipboard.ContainsText())
            {
                buildString = Clipboard.GetText();
                if (!Regex.IsMatch(buildString, @"\|MxD[uz];[0-9]+\;[0-9]+;[0-9]+;[A-Z]+;\|"))
                {
                    MessageBox.Show("Clipboard doesn't contain a valid datachunk.", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return false;
                }

                // This ignores previously set mStream
                mStream = new MemoryStream();
                var streamWriter = new StreamWriter(mStream);
                streamWriter.Write(buildString);
                streamWriter.Flush();
                mStream.Position = 0;
            }
            else
            {
                MessageBox.Show("Cannot load build: no data to load from.", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return false;
            }

            //Stream iStream1 = mStream != null ? mStream : (Stream) new FileStream(iFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            switch (MidsCharacterFileFormat.MxDExtractAndLoad(mStream))
            {
                case MidsCharacterFileFormat.eLoadReturnCode.Failure:
                    mStream.Close();
                    
                    return false;
                case MidsCharacterFileFormat.eLoadReturnCode.Success:
                    mStream.Close();
                    ResetLevel();
                    PoolShuffle();
                    I9Gfx.OriginIndex = Origin;
                    Validate();
                    ReadMetadata(buildString);

                    return true;
                case MidsCharacterFileFormat.eLoadReturnCode.IsOldFormat:
                    mStream.Close();
                    break;
            }

            StreamReader iStream2;
            try
            {
                iStream2 = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return false;
            }

            var flag = ReadInternalData(iStream2);
            iStream2.Close();
            ResetLevel();
            PoolShuffle();
            I9Gfx.OriginIndex = Origin;
            Validate();
            ReadMetadata(buildString);

            return flag;
        }

        public PopUp.PopupData PopPowerInfo(int hIDX, int pIDX)
        {
            var popupData = new PopUp.PopupData();
            if (pIDX < 0)
            {
                if (hIDX < 0 || CurrentBuild.Powers[hIDX].NIDPower < 0)
                {
                    return popupData;
                }

                pIDX = CurrentBuild.Powers[hIDX].NIDPower;
            }

            var power = DatabaseAPI.Database.Power[pIDX];
            var index1 = popupData.Add();
            popupData.Sections[index1].Add(power.DisplayName, PopUp.Colors.Title, 1.25f);
            if (power.PowerSetID > -1)
            {
                popupData.Sections[index1].Add($"Powerset: {DatabaseAPI.Database.Powersets[power.PowerSetID].DisplayName}", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
            }

            if (hIDX > -1)
            {
                if (CurrentBuild.Powers[hIDX].Chosen)
                {
                    popupData.Sections[index1].Add($"Available: Level {power.Level}", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                    popupData.Sections[index1].Add($"Placed: Level {CurrentBuild.Powers[hIDX].Level + 1}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                }
                else
                {
                    popupData.Sections[index1].Add($"Inherent: Level {CurrentBuild.Powers[hIDX].Level + 1}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                }
            }
            else
            {
                popupData.Sections[index1].Add($"Available: Level {power.Level}", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            popupData.Sections[index1].Add(power.DescShort, PopUp.Colors.Text);
            var flag1 = false;
            if (hIDX < 0 & pIDX > -1)
            {
                if (DatabaseAPI.Database.Power[pIDX].NIDSubPower.Length > 0)
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1] = new PopUp.Section();
                    popupData.Sections[index1].Add("Powers:", PopUp.Colors.Title);
                    if (pIDX > -1)
                    {
                        foreach (var subPowerNid in DatabaseAPI.Database.Power[pIDX].NIDSubPower)
                        {
                            if (subPowerNid > -1)
                            {
                                popupData.Sections[index1].Add(DatabaseAPI.Database.Power[subPowerNid].DisplayName, PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                            }
                        }
                    }
                }

                if (!DatabaseAPI.Database.Power[pIDX].Requires.ClassOk(Archetype.Idx))
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1].Add("You cannot take this power because you are a " + Archetype.DisplayName + ".", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);
                }
            }

            var hasEnhEffect = false;
            if (hIDX <= -1)
            {
                return popupData;
            }

            {
                if (CurrentBuild.Powers[hIDX].NIDPower > -1)
                {
                    if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].Effects.Length > 0)
                    {
                        if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].NIDSubPower.Length > 0)
                        {
                            index1 = popupData.Add();
                            popupData.Sections[index1] = CurrentBuild.Powers[hIDX].PopSubPowerListing("Powers:", PopUp.Colors.Text, PopUp.Colors.Text);
                        }
                    }
                    else if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].NIDSubPower.Length > 0)
                    {
                        index1 = popupData.Add();
                        popupData.Sections[index1] = CurrentBuild.Powers[hIDX].PopSubPowerListing("Powers:", PopUp.Colors.Disabled, PopUp.Colors.Effect);
                    }
                }

                if (CurrentBuild.Powers[hIDX].Slots.Length > 0)
                {
                    for (var slotIdx = 0; slotIdx <= CurrentBuild.Powers[hIDX].Slots.Length - 1; ++slotIdx)
                    {
                        if (CurrentBuild.Powers[hIDX].Slots[slotIdx].Enhancement.Enh > -1 && DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[slotIdx].Enhancement.Enh].HasEnhEffect)
                        {
                            hasEnhEffect = true;
                        }
                    }

                    if (hasEnhEffect)
                    {
                        index1 = popupData.Add();
                        popupData.Sections[index1] = PopSlottedEnhInfo(hIDX);
                    }
                }

                foreach (var sb in CurrentBuild.SetBonus)
                {
                    if (sb.PowerIndex != hIDX)
                    {
                        continue;
                    }

                    for (var senInfoIdx = 0; senInfoIdx < sb.SetInfo.Length; senInfoIdx++)
                    {
                        if (!flag1)
                        {
                            flag1 = true;
                            index1 = popupData.Add();
                            popupData.Sections[index1].Add("Active Enhancement Sets:", PopUp.Colors.Text);
                        }

                        var setInfo = sb.SetInfo;
                        var enhancementSet = DatabaseAPI.Database.EnhancementSets[sb.SetInfo[senInfoIdx].SetIDX];
                        popupData.Sections[index1].Add($"{enhancementSet.DisplayName} ({setInfo[senInfoIdx].SlottedCount}/{enhancementSet.Enhancements.Length})", PopUp.Colors.Title);
                        for (var bonusIdx = 0; bonusIdx < enhancementSet.Bonus.Length; bonusIdx++)
                        {
                            if (setInfo[senInfoIdx].SlottedCount >= enhancementSet.Bonus[bonusIdx].Slotted & (enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE | enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE | enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.Any))
                            {
                                var enhString = enhancementSet.GetEffectString(bonusIdx, false, true, true, true);
                                if (!string.IsNullOrWhiteSpace(enhString))
                                {
                                    popupData.Sections[index1].Add(enhString.Replace(", ", "\n"), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                                }
                            }
                        }

                        foreach (var enh in sb.SetInfo[senInfoIdx].EnhIndexes)
                        {
                            var isSpecial = DatabaseAPI.IsSpecialEnh(enh);
                            if (isSpecial > -1)
                            {
                                popupData.Sections[index1].Add(enhancementSet.GetEffectString(isSpecial, true, true, true), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                            }
                        }
                    }
                }

                if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].UIDSubPower.Length > 0)
                {
                    var index2 = popupData.Add();
                    var iText = "This virtual power contains additional powers which can be individually selected.\r\n" + "To change which powers are selected, either Control+Shift+Click or Double-Click on this power.\r\n\r\nRemember that the selected powers will only be active if this power's toggle button is switched on.";
                    popupData.Sections[index2].Add(iText, PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                var empty = string.Empty;
                if (PowerState(CurrentBuild.Powers[hIDX].NIDPower, ref empty) != ListLabelV3.LlItemState.Invalid || empty == "")
                {
                    return popupData;
                }

                {
                    var index2 = popupData.Add();
                    popupData.Sections[index2].Add(empty, PopUp.Colors.Alert);
                    if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].Requires.ClassOk(Archetype.Idx))
                    {
                        return popupData;
                    }

                    var index3 = popupData.Add();
                    popupData.Sections[index3].Add("You cannot take this power because you are a " + Archetype.DisplayName + ".", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);
                }
            }
            return popupData;
        }

        public PopUp.PopupData PopPowersetInfo(int nIDPowerset, string extraString = "")
        {
            var popupData = new PopUp.PopupData();
            var powerset = DatabaseAPI.Database.Powersets[nIDPowerset];
            var index1 = popupData.Add();
            popupData.Sections[index1].Add(powerset.DisplayName, PopUp.Colors.Title, 1.25f);
            if (powerset.nArchetype > -1)
            {
                popupData.Sections[index1].Add("Archetype: " + DatabaseAPI.Database.Classes[powerset.nArchetype].DisplayName, PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
            }
            else
            {
                popupData.Sections[index1].Add("Archetype: All", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
            }

            popupData.Sections[index1].Add("Set Type: " + Enum.GetName(powerset.SetType.GetType(), powerset.SetType), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
            popupData.Sections[index1].Add(powerset.Description.Replace("  ", " "), PopUp.Colors.Text);
            if (extraString != "")
            {
                var index2 = popupData.Add();
                popupData.Sections[index2].Add(extraString, PopUp.Colors.Invention, 1f, FontStyle.Bold, 1);
            }

            if (powerset.Powers.Length <= 0)
            {
                return popupData;
            }

            {
                if (!powerset.Powers[0].Requires.ClassOk(Archetype.Idx))
                {
                    var index2 = popupData.Add();
                    popupData.Sections[index2].Add("You cannot take powers from this pool because you are a " + Archetype.DisplayName + ".", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);
                }
                else if (PowersetMutexClash(Powersets[0].Power[0]))
                {
                    var index2 = popupData.Add();
                    popupData.Sections[index2].Add("You cannot take the " + Powersets[0].DisplayName + " and " + Powersets[1].DisplayName + " sets together.", PopUp.Colors.Alert);
                }
            }
            return popupData;
        }

        private PopUp.Section? PopSlottedEnhInfo(int hIDX)
        {
            var section = new PopUp.Section();
            section.Add("Buff/Debuff", PopUp.Colors.Text, "Value", PopUp.Colors.Text);
            if (hIDX < 0)
            {
                return section;
            }

            var eEnhance = Enums.eEnhance.None;
            var eMez = Enums.eMez.None;
            var nBuff = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var nDebuff = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var nAny = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var schedBuff = new Enums.eSchedule[Enum.GetValues(eEnhance.GetType()).Length];
            var schedDebuff = new Enums.eSchedule[Enum.GetValues(eEnhance.GetType()).Length];
            var schedAny = new Enums.eSchedule[Enum.GetValues(eEnhance.GetType()).Length];
            var afterED1 = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var afterED2 = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var afterED3 = new float[Enum.GetValues(eEnhance.GetType()).Length];
            var nMez = new float[Enum.GetValues(eMez.GetType()).Length];
            var schedMez = new Enums.eSchedule[Enum.GetValues(eEnhance.GetType()).Length];
            var afterED4 = new float[Enum.GetValues(eEnhance.GetType()).Length];
            for (var index = 0; index <= nBuff.Length - 1; ++index)
            {
                nBuff[index] = 0.0f;
                nDebuff[index] = 0.0f;
                nAny[index] = 0.0f;
                schedBuff[index] = Enhancement.GetSchedule((Enums.eEnhance) index);
                schedDebuff[index] = schedBuff[index];
                schedAny[index] = schedBuff[index];
            }

            schedDebuff[3] = Enums.eSchedule.A;
            for (var tSub = 0; tSub <= nMez.Length - 1; ++tSub)
            {
                nMez[tSub] = 0.0f;
                schedMez[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
            }

            for (var index1 = 0; index1 <= CurrentBuild.Powers[hIDX].SlotCount - 1; ++index1)
            {
                if (CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh <= -1)
                {
                    continue;
                }

                for (var index2 = 0; index2 <= DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect.Length - 1; ++index2)
                {
                    var effect = DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect;
                    var index3 = index2;
                    if (effect[index3].Mode != Enums.eEffMode.Enhancement)
                    {
                        continue;
                    }

                    if (effect[index3].Enhance.ID == 12)
                    {
                        nMez[effect[index3].Enhance.SubID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[index3].Enhance.SubID, 1f);
                    }
                    else
                    {
                        switch (DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect[index2].BuffMode)
                        {
                            case Enums.eBuffDebuff.BuffOnly:
                                nBuff[effect[index3].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance) effect[index3].Enhance.ID, -1, 1f);
                                break;
                            case Enums.eBuffDebuff.DeBuffOnly:
                                if ((effect[index3].Enhance.ID != 6) & (effect[index3].Enhance.ID != 19) & (effect[index3].Enhance.ID != 11))
                                {
                                    nDebuff[effect[index3].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance) effect[index3].Enhance.ID, -1, -1f);
                                }
                                break;
                            default:
                                nAny[effect[index3].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance) effect[index3].Enhance.ID, -1, 1f);
                                break;
                        }
                    }
                }
            }

            if (!MidsContext.Config.DisableAlphaPopup)
            {
                for (var index1 = 0; index1 <= CurrentBuild.Powers.Count - 1; ++index1)
                {
                    if (CurrentBuild.Powers[index1] == null || CurrentBuild.Powers[index1].Power == null || !CurrentBuild.Powers[index1].StatInclude)
                    {
                        continue;
                    }

                    IPower power1 = new Power(CurrentBuild.Powers[index1]?.Power);
                    power1.AbsorbPetEffects();
                    power1.ApplyGrantPowerEffects();
                    for (var index2 = 0; index2 <= power1.Effects.Length - 1; ++index2)
                    {
                        var effect = power1.Effects[index2];
                        if ((power1.PowerType != Enums.ePowerType.GlobalBoost) & (!effect.Absorbed_Effect | (effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost)))
                        {
                            continue;
                        }

                        var power2 = power1;
                        if (effect.Absorbed_Effect & (effect.Absorbed_Power_nID > -1))
                        {
                            power2 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                        }

                        var eBuffDebuff = Enums.eBuffDebuff.Any;
                        var flag = false;
                        foreach (var str1 in CurrentBuild.Powers[hIDX]?.Power.BoostsAllowed)
                        {
                            if (power2 != null && power2.BoostsAllowed.Any(str2 => str1 == str2))
                            {
                                if (str1.Contains("Buff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                                }

                                if (str1.Contains("Debuff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                                }

                                flag = true;
                            }

                            if (flag)
                            {
                                break;
                            }
                        }

                        if (!flag)
                        {
                            continue;
                        }

                        if (effect.EffectType == Enums.eEffectType.Enhancement)
                        {
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    afterED1[3] += effect.Mag;
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    afterED2[3] += effect.Mag;
                                                    break;
                                                default:
                                                    afterED3[3] += effect.Mag;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    nBuff[3] += effect.Mag;
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    nDebuff[3] += effect.Mag;
                                                    break;
                                                default:
                                                    nAny[3] += effect.Mag;
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        afterED4[(int) effect.MezType] += effect.Mag;
                                        break;
                                    }

                                    nMez[(int) effect.MezType] += effect.Mag;
                                    break;
                                default:
                                    int index3;
                                    if (effect.ETModifies != Enums.eEffectType.RechargeTime)
                                    {
                                        index3 = Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString()));
                                    }
                                    else
                                    {
                                        index3 = 14;
                                    }

                                    if (effect.IgnoreED)
                                    {
                                        afterED3[index3] += effect.Mag;
                                        break;
                                    }

                                    nAny[index3] += effect.Mag;
                                    break;
                            }
                        }
                        else if ((effect.EffectType == Enums.eEffectType.DamageBuff) & (effect.DamageType == Enums.eDamage.Smashing))
                        {
                            if (effect.IgnoreED)
                            {
                                foreach (var str in power2?.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        afterED3[18] += effect.Mag;
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    afterED3[2] += effect.Mag;
                                    break;
                                }
                            }
                            else
                            {
                                foreach (var str in power2.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        nAny[18] += effect.Mag;
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    nAny[2] += effect.Mag;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            nBuff[8] = 0.0f;
            nDebuff[8] = 0.0f;
            nAny[8] = 0.0f;
            nBuff[17] = 0.0f;
            nDebuff[17] = 0.0f;
            nAny[17] = 0.0f;
            nBuff[16] = 0.0f;
            nDebuff[16] = 0.0f;
            nAny[16] = 0.0f;
            var num6 = nBuff.Length - 1;
            for (var index = 0; index <= num6; ++index)
            {
                PopUp.StringValue[] sContent;
                if (nBuff[index] > 0.0)
                {
                    //section.Content = (PopUp.StringValue[]) Utils.CopyArray(section.Content, new PopUp.StringValue[section.Content.Length + 1]);
                    sContent = new PopUp.StringValue[section.Content.Length + 1];
                    Array.Copy(section.Content, sContent, section.Content.Length);
                    section.Content = sContent;
                    section.Content[^1] = BuildEDItem(index, nBuff, schedBuff, Enum.GetName(eEnhance.GetType(), index), afterED1);
                }

                if (nDebuff[index] > 0.0)
                {
                    //section.Content = (PopUp.StringValue[]) Utils.CopyArray(section.Content, new PopUp.StringValue[section.Content.Length + 1]);
                    sContent = new PopUp.StringValue[section.Content.Length + 1];
                    Array.Copy(section.Content, sContent, section.Content.Length);
                    section.Content = sContent;
                    section.Content[^1] = BuildEDItem(index, nDebuff, schedDebuff, Enum.GetName(eEnhance.GetType(), index) + " Debuff", afterED2);
                }

                if (!(nAny[index] > 0.0))
                {
                    continue;
                }

                //section.Content = (PopUp.StringValue[]) Utils.CopyArray(section.Content, new PopUp.StringValue[section.Content.Length + 1]);
                sContent = new PopUp.StringValue[section.Content.Length + 1];
                Array.Copy(section.Content, sContent, section.Content.Length);
                section.Content = sContent;
                section.Content[^1] = BuildEDItem(index, nAny, schedAny, Enum.GetName(eEnhance.GetType(), index), afterED3);
            }

            for (var index = 0; index <= nMez.Length - 1; ++index)
            {
                if (!(nMez[index] > 0.0))
                {
                    continue;
                }

                //section.Content = (PopUp.StringValue[]) Utils.CopyArray(section.Content, new PopUp.StringValue[section.Content.Length + 1]);
                var sContent = new PopUp.StringValue[section.Content.Length + 1];
                Array.Copy(section.Content, sContent, section.Content.Length);
                section.Content = sContent;
                if (section.Content != null)
                {
                    section.Content[^1] = BuildEDItem(index, nMez, schedMez, Enum.GetName(eMez.GetType(), index), afterED4);
                }
            }

            if (MidsContext.Config.DisableAlphaPopup)
            {
                section.Add("Enhancement values exclude Alpha ability (see Data View for full info, or change this option in the Configuration panel)", PopUp.Colors.Text, 0.8f, FontStyle.Regular, 1);
            }

            return section;
        }

        public ListLabelV3.LlItemState PowerState(int nIDPower, ref string message)
        {
            if (nIDPower < 0)
                return ListLabelV3.LlItemState.Disabled;
            var power = DatabaseAPI.Database.Power[nIDPower];
            var inToonHistory = CurrentBuild.FindInToonHistory(nIDPower);
            var flag1 = inToonHistory > -1;
            var num1 = Level;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Normal && RequestedLevel > -1) 
            {
                num1 = RequestedLevel;
            }
            else if (MidsContext.Config.BuildMode == Enums.dmModes.Respec && RequestedLevel > -1)
            {
                num1 = RequestedLevel;
            }

            var nLevel = num1;
            if (flag1)
            {
                nLevel = CurrentBuild.Powers[inToonHistory].Level;
            }

            message = "";
            var flag2 = CurrentBuild.MeetsRequirement(power, nLevel);
            if (PowersetMutexClash(nIDPower))
            {
                message = "You cannot take the " + Powersets[0].DisplayName + " and " + Powersets[1].DisplayName +
                          " sets together.";
                return ListLabelV3.LlItemState.Heading;
            }

            if (flag1)
            {
                var num2 = 0;
                Enums.PowersetType powersetType;
                int[] numArray;
                do
                {
                    Enums.ePowerSetType ePowerSetType;
                    int index1;
                    if (num2 == 0)
                    {
                        ePowerSetType = Enums.ePowerSetType.Primary;
                        powersetType = Enums.PowersetType.Primary;
                        index1 = 0;
                    }
                    else
                    {
                        ePowerSetType = Enums.ePowerSetType.Secondary;
                        powersetType = Enums.PowersetType.Secondary;
                        index1 = 1;
                    }

                    if ((power.GetPowerSet().SetType == ePowerSetType) & (power.Level - 1 == 0))
                    {
                        numArray = DatabaseAPI.NidPowersAtLevelBranch(0, Powersets[(int) powersetType].nID);
                        var flag3 = false;
                        var num3 = 0;
                        var num4 = numArray.Length - 1;
                        for (var index2 = 0; index2 <= num4; ++index2)
                            if (CurrentBuild.Powers[index1].NIDPower == numArray[index2])
                                flag3 = true;
                            else if (CurrentBuild.FindInToonHistory(numArray[index2]) > -1)
                                ++num3;

                        if (((CurrentBuild.Powers[index1].NIDPowerset > 0) & !flag3) | (num3 == numArray.Length))
                            goto label_22;
                    }

                    ++num2;
                } while (num2 <= 1);

                goto label_23;
                label_22:
                message = "This power has been placed in a way that is not possible in-game. One of the " +
                          Convert.ToString(numArray.Length) + " level 1 powers from your " +
                          Enum.GetName(powersetType.GetType(), powersetType) + " set must be taken at level 1.";
                return ListLabelV3.LlItemState.Invalid;
                label_23:
                if (!flag2)
                {
                    if ((power.GetPowerSet().SetType == Enums.ePowerSetType.Ancillary) |
                        (power.GetPowerSet().SetType == Enums.ePowerSetType.Pool))
                    {
                        message = "This power has been placed in a way that is not possible in-game.";
                        switch (power.PowerSetIndex)
                        {
                            case 2:
                                message +=
                                    "\r\nYou must take one of the first two powers in a pool before taking the third.";
                                break;
                            case 3:
                                message +=
                                    "\r\nYou must take two of the first three powers in a pool before taking the fourth.";
                                break;
                            case 4:
                                message +=
                                    "\r\nYou must take two of the first three powers in a pool before taking the fifth.";
                                break;
                        }
                    }
                    else
                    {
                        if (power.InherentType != Enums.eGridType.None)
                        {
                            return ListLabelV3.LlItemState.Enabled;
                        }

                        message =
                            "This power has been placed in a way that is not possible in-game.\r\nCheck that any powers that it requires have been taken first, and that if this is a branching powerset, the power does not conflict with another.";

                        return ListLabelV3.LlItemState.Invalid;

                    }

                    return ListLabelV3.LlItemState.Invalid;
                }

                if (num1 <= power.Level - 1)
                    return ListLabelV3.LlItemState.SelectedDisabled;
                return num1 <= power.Level - 1 ? ListLabelV3.LlItemState.Enabled : ListLabelV3.LlItemState.Selected;
            }

            if (flag2 && num1 >= power.Level - 1)
                return ListLabelV3.LlItemState.Enabled;
            return ListLabelV3.LlItemState.Disabled;
        }

        private bool ReadInternalData(StreamReader iStream)
        {
            iStream.BaseStream.Seek(0L, SeekOrigin.Begin);
            string[]? strArray;
            string a;
            try
            {
                do
                {
                    strArray = IoGrab2(iStream, ";", '|');
                    if (strArray != null)
                        a = strArray.Length <= 0 ? "" : strArray[0];
                    else
                        throw new Exception("Reached end of data without finding header.");
                } while (!(string.Equals(a, Files.Headers.Save.Uncompressed, StringComparison.OrdinalIgnoreCase) | string.Equals(a, Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase) || string.Equals(a, Files.Headers.Save.LegacyUncompressed, StringComparison.OrdinalIgnoreCase) | string.Equals(a, Files.Headers.Save.LegacyCompressed, StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            if (string.Equals(a, Files.Headers.Save.Uncompressed, StringComparison.OrdinalIgnoreCase) || string.Equals(a, Files.Headers.Save.LegacyUncompressed, StringComparison.OrdinalIgnoreCase))
            {
                iStream.BaseStream.Seek(0L, SeekOrigin.Begin);
                return ReadInternalDataUC(iStream);
            }

            if (!string.Equals(a, Files.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase) || !string.Equals(a, Files.Headers.Save.LegacyCompressed, StringComparison.OrdinalIgnoreCase))
                return false;
            var asciiEncoding = new ASCIIEncoding();
            var outSize = Convert.ToInt32(strArray[1]);
            var num1 = Convert.ToInt32(strArray[2]);
            var num2 = Convert.ToInt32(strArray[3]);
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            //var iBytes = (byte[]) Utils.CopyArray(Zlib.UUDecodeBytes((byte[]) Utils.CopyArray(asciiEncoding.GetBytes(Zlib.UnbreakString(iStream.ReadToEnd())), new byte[num2])), new byte[num1]);
            var encoding = asciiEncoding.GetBytes(Zlib.UnbreakString(iStream.ReadToEnd()));
            var zlibDecode = Zlib.UUDecodeBytes(encoding);
            var dest1 = new byte[num2];
            Array.Copy(zlibDecode, dest1, zlibDecode.Length);
            var iBytes = new byte[num1];
            Array.Copy(dest1, iBytes, dest1.Length);
            iBytes = Zlib.UncompressChunk(ref iBytes, outSize);
            binaryWriter.Write(iBytes);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            if (ReadInternalDataUC(reader))
            {
                binaryWriter.Close();
                memoryStream.Close();
                return true;
            }

            binaryWriter.Close();
            memoryStream.Close();
            return false;
        }

        private bool ReadInternalDataUC(StreamReader iStream)
        {
            string[] strArray1;
            do
            {
                strArray1 = IoGrab2(iStream, "|");
            } while (strArray1[0] != Files.Headers.Save.Uncompressed || strArray1[0] != Files.Headers.Save.LegacyUncompressed);

            strArray1[1] = strArray1[1].Replace(",", ".");
            var nVer = Convert.ToSingle(strArray1[1]);
            bool flag;
            if (nVer < BuildFormatChange1)
            {
                flag = ImportInternalDataUC(iStream, nVer);
            }
            else
            {
                if ((nVer < BuildFormatChange2) & (Math.Abs(nVer - BuildFormatChange1) > float.Epsilon))
                {
                    MessageBox.Show(@"The data being loaded was saved by either an older version or different rendition of the application, attempting conversion.", @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (nVer > BuildFormatChange2)
                {
                    MessageBox.Show($"The data being loaded was saved by a newer version of the application (File format v {nVer.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###")} expected {1.4f.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###")} ", "Just FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    /*MessageBox.Show(("The data being loaded was saved by a newer version of the application (File format v" +
                                     Strings.Format(nVer, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###") + ", expected " +
                                     Strings.Format(1.4f, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###") +
                                     "). It may not load correctly."), @"Just FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
                }

                var strArray2 = IoGrab2(iStream, "|");
                Name = strArray2[0];
                Archetype = DatabaseAPI.GetArchetypeByName(strArray2[2]);
                Origin = DatabaseAPI.GetOriginByName(Archetype, strArray2[1]);
                var strArray3 = IoGrab2(iStream, "|");
                for (var index = 0; index <= Powersets.Length - 1; ++index)
                {
                    Powersets[index] =
                        DatabaseAPI.GetPowersetByName(FixSpelling(strArray3[index]), Archetype.DisplayName);
                    if (strArray3[index].IndexOf("Inherent", StringComparison.Ordinal) > -1)
                        Powersets[index] = DatabaseAPI.GetInherentPowerset();
                }

                var strArray4 = IoGrab2(iStream, "|");
                NewBuild();
                var index1 = 0;
                var tIDX = CurrentBuild.Powers.Count - (nVer < BuildFormatChange2 ? 2 : 1);
                for (var index2 = 0; index2 <= tIDX; ++index2)
                {
                    var power = CurrentBuild.Powers[index2];
                    if (index1 + 6 <= strArray4.Length)
                    {
                        power.StatInclude = Math.Abs(Convert.ToDouble(strArray4[index1])) > 0.01;
                        var index3 = index1 + 1 + 1;
                        power.Level = Convert.ToInt32(strArray4[index3]);
                        var index4 = index3 + 1;
                        power.NIDPowerset = Convert.ToDouble(strArray4[index4]) >= 0.0 ? Powersets[Convert.ToInt32(strArray4[index4])].nID : -1;
                        var index5 = index4 + 1;
                        power.IDXPower = Convert.ToInt32(strArray4[index5]);
                        power.NIDPower = !((power.NIDPowerset > -1) & (power.IDXPower > -1)) ? -1 : DatabaseAPI.Database.Powersets[power.NIDPowerset].Power[power.IDXPower];
                        var index6 = index5 + 1;
                        power.Slots = new SlotEntry[Convert.ToInt32(strArray4[index6]) + 1];
                        var index7 = index6 + 1;
                        var num6 = power.Slots.Length - 1;
                        for (var index8 = 0; index8 <= num6; ++index8)
                        {
                            power.Slots[index8].LoadFromString(strArray4[index7], "~");
                            var index9 = index7 + 1;
                            power.Slots[index8].Level = Convert.ToInt32(strArray4[index9]);
                            index7 = index9 + 1;
                        }

                        power.VariableValue = Convert.ToInt32(strArray4[index7]);
                        index1 = index7 + 1;
                        if (!(nVer >= BuildFormatChange2)) continue;
                        {
                            power.SubPowers = new PowerSubEntry[Convert.ToInt32(strArray4[index1]) + 1];
                            ++index1;
                            var num7 = power.SubPowers.Length - 1;
                            for (var index8 = 0; index8 <= num7; ++index8)
                            {
                                power.SubPowers[index8] = new PowerSubEntry
                                {
                                    Power = Convert.ToInt32(strArray4[index1])
                                };
                                index1++;
                                power.SubPowers[index8].Powerset = Convert.ToInt32(strArray4[index1]);
                                index1++;
                                power.SubPowers[index8].StatInclude = Math.Abs(Convert.ToDouble(strArray4[index1])) > 0.01;
                                index1++;
                                power.SubPowers[index8].nIDPower = !((power.SubPowers[index8].Powerset > -1) & (power.SubPowers[index8].Power > -1)) ? -1 : DatabaseAPI.Database.Powersets[power.SubPowers[index8].Powerset].Power[power.SubPowers[index8].Power];
                            }

                            power.SubPowers = Array.Empty<PowerSubEntry>();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                MidsContext.Archetype = Archetype;
                Validate();
                Lock();
                flag = true;
            }

            return flag;
        }

        private void SetPower_NID(int Index, int nIDPower)
        {
            if ((Index < 0) | (Index >= CurrentBuild.Powers.Count))
                return;
            var power1 = DatabaseAPI.Database.Power[nIDPower];
            if (power1 != null)
            {
                var power2 = CurrentBuild.Powers[Index];
                power2.NIDPowerset = power1.PowerSetID;
                power2.IDXPower = power1.PowerSetIndex;
                power2.NIDPower = power1.PowerIndex;
                if (power1.NIDSubPower.Length > 0)
                {
                    power2.SubPowers = new PowerSubEntry[power1.NIDSubPower.Length];
                    var num = power2.SubPowers.Length - 1;
                    for (var index = 0; index <= num; ++index)
                        if (power2.SubPowers[index] != null)
                            power2.SubPowers[index] = new PowerSubEntry
                            {
                                nIDPower = power1.NIDSubPower[index],
                                Powerset = DatabaseAPI.Database.Power[power2.SubPowers[index].nIDPower].PowerSetID,
                                Power = DatabaseAPI.Database.Power[power2.SubPowers[index].nIDPower].PowerSetIndex
                            };
                }

                if (power1.Slottable & (power2.Slots.Length == 0))
                    power2.AddSlot(power2.Level);
                if (power1.AlwaysToggle)
                    CurrentBuild.Powers[Index].StatInclude = true;
                if (power1.PowerType == Enums.ePowerType.Auto_)
                    CurrentBuild.Powers[Index].StatInclude = true;
            }

            CurrentBuild.Powers[Index].ValidateSlots();
        }

        public bool StringToInternalData(string iString)
        {
            bool flag1;
            if ((iString?.IndexOf(Files.Headers.Save.Compressed, StringComparison.Ordinal) == -1) & (iString?.IndexOf(Files.Headers.Save.Uncompressed, StringComparison.Ordinal) == -1) || (iString?.IndexOf(Files.Headers.Save.LegacyCompressed, StringComparison.Ordinal) == -1) & (iString?.IndexOf(Files.Headers.Save.LegacyUncompressed, StringComparison.Ordinal) == -1))
            {
                if ((iString.IndexOf("Primary", StringComparison.Ordinal) > -1) &
                    (iString.IndexOf("Secondary", StringComparison.Ordinal) > -1))
                {
                    if (clsUniversalImport.InterpretForumPost(iString))
                    {
                        MessageBox.Show(@"Form post interpreted OK!", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show(@"Unable to interpret data. Please check that you copied the build data from the forum correctly and that it's a valid format.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        flag1 = false;
                    }
                }
                else
                {
                    MessageBox.Show(@"Unable to recognize data. Please check that you copied the build data from the forum correctly and that it's a valid format.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag1 = false;
                }
            }
            else
            {
                StreamWriter streamWriter;
                try
                {
                    streamWriter = new StreamWriter(FileIO.AddSlash(Application.StartupPath) + "import.tmp", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                try
                {
                    if (iString != null && (iString.IndexOf(Files.Headers.Save.Compressed, StringComparison.Ordinal) < 0 || iString.IndexOf(Files.Headers.Save.LegacyCompressed, StringComparison.Ordinal) < 0))
                    {
                        iString = iString.Replace("+\r\n+", "");
                        iString = iString.Replace("+ \r\n+", "");
                    }

                    iString = iString
                        .Replace("[Email]", "")
                        .Replace("[/Email]", "")
                        .Replace("[email]", "")
                        .Replace("[/email]", "")
                        .Replace("[EMAIL]", "")
                        .Replace("[/EMAIL]", "")
                        .Replace("[URL]", "")
                        .Replace("[/URL]", "")
                        .Replace("[url]", "")
                        .Replace("[/url]", "")
                        .Replace("[Url]", "")
                        .Replace("[/Url]", "");
                    streamWriter.Write(iString);
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    streamWriter.Close();
                    return false;
                }

                Stream? mStream = null;
                if (Load(FileIO.AddSlash(Application.StartupPath) + "import.tmp", ref mStream))
                {
                    MessageBox.Show(@"Build data imported!", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    flag1 = true;
                }
                else
                {
                    MessageBox.Show(@"Build data couldn't be imported.  Please check that you copied the build data from the forum correctly.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag1 = false;
                }
            }

            return flag1;
        }

        private bool TestPower(int nIDPower)
        {
            if (CurrentBuild.FindInToonHistory(nIDPower) > -1)
                return false;
            var message = "";
            return PowerState(nIDPower, ref message) == ListLabelV3.LlItemState.Enabled;
        }
    }
}
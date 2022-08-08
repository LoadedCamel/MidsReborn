using System;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Import
{
    public class EffectData
    {
        private readonly string _csvString = string.Empty;

        public readonly IEffect Data;
        public readonly int Index = -1;
        public readonly bool IsLocked;
        public readonly bool IsValid;
        public bool IndexChanged;
        public bool IsNew;
        public int Nid = -1;

        public EffectData(string iString)
        {
            if (iString == null)
            {
                IsValid = false;
            }
            else
            {
                var array = CSV.ToArray(iString);
                if (array.Length < 35)
                {
                    IsValid = false;
                }
                else
                {
                    IsValid = true;
                    if (!DatabaseAPI.Database.PowersetGroups.ContainsKey(array[0].Split(".".ToCharArray())[0]))
                    {
                        IsValid = false;
                    }
                    else
                    {
                        Index = DatabaseAPI.NidFromUidPower(array[0]);
                        if (Index < 0)
                        {
                            IsValid = false;
                        }
                        else
                        {
                            Data = new Effect(DatabaseAPI.Database.Power[Index]);
                            //Data.ImportFromCSV(iString);
                            if (DatabaseAPI.Database.Power[Index].NeverAutoUpdate)
                                IsLocked = true;
                            _csvString = iString;
                        }
                    }
                }
            }
        }

        public bool Apply()
        {
            bool flag;
            if (!IsValid)
            {
                flag = false;
            }
            else
            {
                if (IsNew)
                {
                    var effects = DatabaseAPI.Database.Power[Index].Effects;
                    Array.Resize(ref effects, DatabaseAPI.Database.Power[Index].Effects.Length + 1);
                    DatabaseAPI.Database.Power[Index].Effects = effects;
                    Nid = DatabaseAPI.Database.Power[Index].Effects.Length - 1;
                    DatabaseAPI.Database.Power[Index].Effects[Nid] = new Effect(DatabaseAPI.Database.Power[Index]);
                }

                if (!IsNew & (Index < 0))
                {
                    flag = false;
                }
                else
                {
                    //DatabaseAPI.Database.Power[Index].Effects[Nid].ImportFromCSV(_csvString);
                    flag = true;
                }
            }

            return flag;
        }

        public bool CheckSimilar(ref IEffect iEffect)
        {
            return IsValid && iEffect.EffectType == Data.EffectType && iEffect.DamageType == Data.DamageType &&
                   iEffect.MezType == Data.MezType && iEffect.ETModifies == Data.ETModifies &&
                   iEffect.PvMode == Data.PvMode && iEffect.ToWho == Data.ToWho &&
                   iEffect.SpecialCase == Data.SpecialCase && 
                   iEffect.ActiveConditionals == Data.ActiveConditionals && 
                   iEffect.AttribType == Data.AttribType &&
                   iEffect.Aspect == Data.Aspect && iEffect.Reward == Data.Reward &&
                   iEffect.EffectId == Data.EffectId && iEffect.Summon == Data.Summon;
        }

        public bool CheckDifference(ref IEffect iEffect, out string message)
        {
            message = string.Empty;
            bool flag1;
            if (!IsValid)
            {
                flag1 = false;
            }
            else if (IsNew)
            {
                message = "New";
                flag1 = true;
            }
            else
            {
                var flag2 = false;
                if (iEffect.EffectType != Data.EffectType)
                {
                    message += $"EffectType: {iEffect.EffectType} => {Data.EffectType}";
                    flag2 = true;
                }

                if (iEffect.DisplayPercentage != Data.DisplayPercentage)
                {
                    message += $"DisplayPercentage: {iEffect.DisplayPercentage} => {Data.DisplayPercentage}";
                    flag2 = true;
                }

                if (iEffect.DamageType != Data.DamageType)
                {
                    message += $"DamageType: {iEffect.DamageType} => {Data.DamageType}";
                    flag2 = true;
                }

                if (iEffect.MezType != Data.MezType)
                {
                    message += $"MezType: {iEffect.MezType} => {Data.MezType}";
                    flag2 = true;
                }

                if (iEffect.ETModifies != Data.ETModifies)
                {
                    message += $"ETModifies: {iEffect.ETModifies} => {Data.ETModifies}";
                    flag2 = true;
                }

                if (iEffect.Summon != Data.Summon)
                {
                    message += $"Summon: {iEffect.Summon} => {Data.Summon}";
                    flag2 = true;
                }

                if (iEffect.Ticks != Data.Ticks)
                {
                    message += $"Ticks: {iEffect.Ticks} => {Data.Ticks}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.DelayedTime - Data.DelayedTime) > 0.001)
                {
                    message += $"DelayedTime: {iEffect.DelayedTime} => {Data.DelayedTime}";
                    flag2 = true;
                }

                if (iEffect.Stacking != Data.Stacking)
                {
                    message += $"Stacking: {iEffect.Stacking} => {Data.Stacking}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.BaseProbability - Data.BaseProbability) > 0.001)
                {
                    message += $"BaseProbability: {iEffect.BaseProbability} => {Data.BaseProbability}";
                    flag2 = true;
                }

                if (iEffect.CancelOnMiss != Data.CancelOnMiss)
                {
                    message += $"CancelOnMiss: {iEffect.CancelOnMiss} => {Data.CancelOnMiss}";
                    flag2 = true;
                }

                if (iEffect.Suppression != Data.Suppression)
                {
                    message += $"Suppression: {iEffect.Suppression} => {Data.Suppression}";
                    flag2 = true;
                }

                if (iEffect.Buffable != Data.Buffable)
                {
                    message += $"Buffable: {iEffect.Buffable} => {Data.Buffable}";
                    flag2 = true;
                }

                if (iEffect.Resistible != Data.Resistible)
                {
                    message += $"Resistible: {iEffect.Resistible} => {Data.Resistible}";
                    flag2 = true;
                }

                if (iEffect.SpecialCase != Data.SpecialCase)
                {
                    message += $"SpecialCase: {iEffect.SpecialCase} => {Data.SpecialCase}";
                    flag2 = true;
                }

                if (iEffect.ActiveConditionals != Data.ActiveConditionals)
                {
                    message += $"ActiveConditionals: {iEffect.ActiveConditionals} => {Data.ActiveConditionals}";
                    flag2 = true;
                }

                if (iEffect.PvMode != Data.PvMode)
                {
                    message += $"PvMode: {iEffect.PvMode} => {Data.PvMode}";
                    flag2 = true;
                }

                if (iEffect.ToWho != Data.ToWho)
                {
                    message += $"ToWho: {iEffect.ToWho} => {Data.ToWho}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.Scale - Data.Scale) > 0.001)
                {
                    message += $"Scale: {iEffect.Scale} => {Data.Scale}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.nMagnitude - Data.nMagnitude) > 0.001)
                {
                    message += $"nMagnitude: {iEffect.nMagnitude} => {Data.nMagnitude}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.nDuration - Data.nDuration) > 0.001)
                {
                    message += $"nDuration: {iEffect.nDuration} => {Data.nDuration}";
                    flag2 = true;
                }

                if (iEffect.AttribType != Data.AttribType)
                {
                    message += $"AttribType: {iEffect.AttribType} => {Data.AttribType}";
                    flag2 = true;
                }

                if (iEffect.Aspect != Data.Aspect)
                {
                    message += $"Aspect: {iEffect.Aspect} => {Data.Aspect}";
                    flag2 = true;
                }

                if (iEffect.ModifierTable != Data.ModifierTable)
                {
                    message += $"ModifierTable: {iEffect.ModifierTable} => {Data.ModifierTable}";
                    flag2 = true;
                }

                if (iEffect.NearGround != Data.NearGround)
                {
                    message += $"NearGround: {iEffect.NearGround} => {Data.NearGround}";
                    flag2 = true;
                }

                if (iEffect.Reward != Data.Reward)
                {
                    message += $"Reward: {iEffect.Reward} => {Data.Reward}";
                    flag2 = true;
                }

                if (iEffect.EffectId != Data.EffectId)
                {
                    message += $"EffectId: {iEffect.EffectId} => {Data.EffectId}";
                    flag2 = true;
                }

                if (iEffect.IgnoreED != Data.IgnoreED)
                {
                    message += $"IgnoreED: {iEffect.IgnoreED} => {Data.IgnoreED}";
                    flag2 = true;
                }

                if (iEffect.Override != Data.Override)
                {
                    message += $"Override: {iEffect.Override} => {Data.Override}";
                    flag2 = true;
                }

                if (iEffect.MagnitudeExpression != Data.MagnitudeExpression)
                {
                    message += $"MagnitudeExpression: {iEffect.MagnitudeExpression} => {Data.MagnitudeExpression}";
                    flag2 = true;
                }

                if (Math.Abs(iEffect.ProcsPerMinute - Data.ProcsPerMinute) > 1.40129846432482E-45)
                {
                    message += $"PPM: {iEffect.ProcsPerMinute} => {Data.ProcsPerMinute}";
                    flag1 = true;
                }
                else
                {
                    flag1 = flag2;
                }
            }

            return flag1;
        }
    }
}
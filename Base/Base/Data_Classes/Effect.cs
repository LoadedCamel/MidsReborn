
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;

namespace Base.Data_Classes
{
    public class Effect : IEffect, IComparable, ICloneable
    {
        static readonly Regex UidClassRegex = new Regex("arch source(.owner)?> (Class_[^ ]*)", RegexOptions.IgnoreCase);
        IPower power;

        public string MagnitudeExpression { get; set; }

        public float ProcsPerMinute { get; set; }

        public bool CancelOnMiss { get; set; }

        public float Probability
        {
            get
            {
                float num1 = BaseProbability;
                if (ProcsPerMinute > 0.0 && num1 < 0.01 && power != null)
                {
                    float num2 = (float)(power.AoEModifier * 0.75 + 0.25);
                    float procsPerMinute = ProcsPerMinute;
                    var Global_Recharge = (MidsContext.Character.DisplayStats.BuffHaste(false) - 100) / 100;
                    var rechargeval = power.BaseRechargeTime / (power.BaseRechargeTime / power.RechargeTime - Global_Recharge);
                    switch (power.PowerType)
                    {
                        case Enums.ePowerType.Click:
                            num1 = Math.Min(Math.Max((power.PowerType != Enums.ePowerType.Click ? procsPerMinute * power.ActivatePeriod / (60f * num2): procsPerMinute * (rechargeval + power.CastTimeReal)) / (60f * num2), (float)(0.0500000007450581 + 0.0149999996647239 * ProcsPerMinute)), 0.9f);
                            break;
                        case Enums.ePowerType.Auto_:
                            num1 = Math.Min(Math.Max((power.PowerType != Enums.ePowerType.Auto_ ? procsPerMinute * 10 / (60f * num2): procsPerMinute * 10) / (60f * num2), (float)(0.0500000007450581 + 0.0149999996647239 * ProcsPerMinute)), 0.9f);
                            break;
                        case Enums.ePowerType.Toggle:
                            num1 = Math.Min(Math.Max((power.PowerType != Enums.ePowerType.Toggle ? procsPerMinute * 10 / (60f * num2): procsPerMinute * 10) / (60f * num2), (float)(0.0500000007450581 + 0.0149999996647239 * ProcsPerMinute)), 0.9f);
                            break;
                    }
                    //num1 = Math.Min(Math.Max((power.PowerType != Enums.ePowerType.Click ? procsPerMinute * 10 : procsPerMinute * (rechargeval + power.CastTimeReal)) / (60f * num2), (float)(0.0500000007450581 + 0.0149999996647239 * ProcsPerMinute)), 0.9f);
                }
                if (MidsContext.Character != null && !string.IsNullOrEmpty(EffectId) && MidsContext.Character.ModifyEffects.ContainsKey(EffectId))
                    num1 += MidsContext.Character.ModifyEffects[EffectId];
                if (num1 > 1.0)
                    num1 = 1f;
                if (num1 < 0.0)
                    num1 = 0.0f;
                return num1;
            }
            set => BaseProbability = value;
        }

        public float Mag
        {
            get
            {
                float num1 = 0.0f;
                switch (AttribType)
                {
                    case Enums.eAttribType.Magnitude:
                        if (Math.Abs(Math_Mag - 0.0f) > 0.01)
                            return Math_Mag;
                        float num2 = nMagnitude;
                        if (EffectType == Enums.eEffectType.Damage)
                            num2 = -num2;
                        num1 = Scale * num2 * DatabaseAPI.GetModifier(this);
                        break;
                    case Enums.eAttribType.Duration:
                        if (Math.Abs(Math_Mag - 0.0f) > 0.01)
                            return Math_Mag;
                        num1 = nMagnitude;
                        if (EffectType == Enums.eEffectType.Damage)
                        {
                            num1 = -num1;
                        }
                        break;
                    case Enums.eAttribType.Expression:
                        num1 = ParseMagnitudeExpression() * DatabaseAPI.GetModifier(this);
                        if (EffectType == Enums.eEffectType.Damage)
                        {
                            num1 = -num1;
                        }
                        break;
                }
                return num1;
            }
        }

        public float MagPercent => !DisplayPercentage ? Mag : Mag * 100f;

        public float Duration
        {
            get
            {
                float num;
                switch (AttribType)
                {
                    case Enums.eAttribType.Magnitude:
                        num = (double)Math.Abs(Math_Duration - 0.0f) > 0.01 ? Math_Duration : nDuration;
                        break;
                    case Enums.eAttribType.Duration:
                        num = (double)Math.Abs(Math_Duration - 0.0f) <= 0.01 ? Scale * DatabaseAPI.GetModifier(this) : Math_Duration;
                        break;
                    default:
                        num = 0.0f;
                        break;
                }
                return num;
            }
        }

        public bool DisplayPercentage
        {
            get
            {
                bool flag;
                switch (DisplayPercentageOverride)
                {
                    case Enums.eOverrideBoolean.TrueOverride:
                        flag = true;
                        break;
                    case Enums.eOverrideBoolean.FalseOverride:
                        flag = false;
                        break;
                    default:
                        if (EffectType == Enums.eEffectType.SilentKill)
                        {
                            flag = false;
                            break;
                        }
                        switch (Aspect)
                        {
                            case Enums.eAspect.Max:
                                if (EffectType == Enums.eEffectType.HitPoints || EffectType == Enums.eEffectType.Endurance || (EffectType == Enums.eEffectType.SpeedRunning || EffectType == Enums.eEffectType.SpeedJumping) || EffectType == Enums.eEffectType.SpeedFlying)
                                    return false;
                                break;
                            case Enums.eAspect.Abs:
                                return false;
                            case Enums.eAspect.Cur:
                                if (EffectType == Enums.eEffectType.Mez || EffectType == Enums.eEffectType.StealthRadius || EffectType == Enums.eEffectType.StealthRadiusPlayer)
                                    return false;
                                break;
                        }
                        flag = true;
                        break;
                }
                return flag;
            }
        }

        public bool VariableModified
        {
            get
            {
                bool flag;
                if (VariableModifiedOverride)
                {
                    flag = true;
                }
                else
                {
                    if (power != null)
                    {
                        var ps = power.GetPowerSet();
                        if (ps == null)
                            return false;
                        if (ps.nArchetype > -1)
                        {
                            if (!DatabaseAPI.Database.Classes[ps.nArchetype].Playable)
                                return false;
                        }
                        else if (ps.SetType == Enums.ePowerSetType.None
                                || ps.SetType == Enums.ePowerSetType.Accolade
                                //|| ps.SetType == Enums.ePowerSetType.Pet
                                || ps.SetType == Enums.ePowerSetType.SetBonus
                                || ps.SetType == Enums.ePowerSetType.Temp)
                            return false;
                    }
                    if (EffectType == Enums.eEffectType.EntCreate & ToWho == Enums.eToWho.Target & Stacking == Enums.eStacking.Yes)
                    {
                        flag = true;
                    }
                    else if (EffectType == Enums.eEffectType.DamageBuff & ToWho == Enums.eToWho.Target & Stacking == Enums.eStacking.Yes)
                    {
                        flag = true;
                    }
                    else
                    {
                        if (power != null)
                        {
                            for (int index = 0; index <= power.Effects.Length - 1; ++index)
                            {
                                if (power.Effects[index].EffectType == Enums.eEffectType.EntCreate & power.Effects[index].ToWho == Enums.eToWho.Target & power.Effects[index].Stacking == Enums.eStacking.Yes)
                                    return false;
                            }
                        }
                        flag = ToWho == Enums.eToWho.Self && Stacking == Enums.eStacking.Yes;
                    }
                }
                return flag;
            }
            set
            {
            }
        }

        public bool InherentSpecial => SpecialCase == Enums.eSpecialCase.Assassination || SpecialCase == Enums.eSpecialCase.Hidden || (SpecialCase == Enums.eSpecialCase.Containment || SpecialCase == Enums.eSpecialCase.CriticalHit) || SpecialCase == Enums.eSpecialCase.Domination || SpecialCase == Enums.eSpecialCase.Scourge || SpecialCase == Enums.eSpecialCase.Supremacy;

        public float BaseProbability { get; set; }

        public bool IgnoreED { get; set; }

        public string Reward { get; set; }

        public string EffectId { get; set; }

        public string Special { get; set; }

        public IPower GetPower() => power;
        public void SetPower(IPower power) => this.power = power;

        public IEnhancement Enhancement { get; set; }

        public int nID { get; set; }

        public Enums.eEffectClass EffectClass { get; set; }

        public Enums.eEffectType EffectType { get; set; }

        public Enums.eOverrideBoolean DisplayPercentageOverride { get; set; }

        public Enums.eDamage DamageType { get; set; }

        public Enums.eMez MezType { get; set; }

        public Enums.eEffectType ETModifies { get; set; }

        public string Summon { get; set; }

        public int nSummon
        {
            get
            {
                if (!SummonId.HasValue)
                    SummonId = EffectType == Enums.eEffectType.EntCreate ? DatabaseAPI.NidFromUidEntity(Summon) : DatabaseAPI.NidFromUidPower(Summon);
                return SummonId.Value;
            }
            set => SummonId = value;
        }

        int? SummonId { get; set; }


        public int Ticks { get; set; }

        public float DelayedTime { get; set; }

        public Enums.eStacking Stacking { get; set; }

        public Enums.eSuppress Suppression { get; set; }

        public bool Buffable { get; set; }

        public bool Resistible { get; set; }

        public Enums.eSpecialCase SpecialCase { get; set; }

        public string UIDClassName { get; set; }

        public int nIDClassName { get; set; }

        public bool VariableModifiedOverride { get; set; }

        public bool isEnhancementEffect { get; set; }

        public Enums.ePvX PvMode { get; set; }

        public Enums.eToWho ToWho { get; set; }

        public float Scale { get; set; }

        public float nMagnitude { get; set; }

        public float nDuration { get; set; }

        public Enums.eAttribType AttribType { get; set; }

        public Enums.eAspect Aspect { get; set; }

        public string ModifierTable { get; set; }

        public int nModifierTable { get; set; }

        public string PowerFullName { get; set; }

        public bool NearGround { get; set; }

        public bool RequiresToHitCheck { get; set; }

        public float Math_Mag { get; set; }

        public float Math_Duration { get; set; }

        public bool Absorbed_Effect { get; set; }

        public Enums.ePowerType Absorbed_PowerType { get; set; }

        public int Absorbed_Power_nID { get; set; }

        public float Absorbed_Duration { get; set; }

        public int Absorbed_Class_nID { get; set; }

        public float Absorbed_Interval { get; set; }

        public int Absorbed_EffectID { get; set; }

        public Enums.eBuffMode buffMode { get; set; }

        public int UniqueID { get; set; }

        public string Override { get; set; }

        public int nOverride
        {
            get
            {
                if (!OverrideId.HasValue)
                    OverrideId = DatabaseAPI.NidFromUidPower(Override);
                return OverrideId.Value;
            }
            set => OverrideId = value;
        }

        int? OverrideId { get; set; }


        public bool isDamage()
        {
            return EffectType == Enums.eEffectType.Defense || EffectType == Enums.eEffectType.DamageBuff || (EffectType == Enums.eEffectType.Resistance || EffectType == Enums.eEffectType.Damage) || EffectType == Enums.eEffectType.Elusivity;
        }

        public string BuildEffectStringShort(bool noMag = false, bool simple = false, bool useBaseProbability = false)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            string iValue = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string effectNameShort1 = Enums.GetEffectNameShort(EffectType);
            if (power != null && power.VariableEnabled && VariableModified)
                str4 = " (V)";
            if (!simple)
            {
                switch (ToWho)
                {
                    case Enums.eToWho.Target:
                        str3 = " to Tgt";
                        break;
                    case Enums.eToWho.Self:
                        str3 = " to Slf";
                        break;
                }
            }
            if (useBaseProbability)
            {
                if (BaseProbability < 1.0)
                    iValue = (BaseProbability * 100f).ToString("#0") + "% chance";
            }
            else if (Probability < 1.0)
                iValue = (Probability * 100f).ToString("#0") + "% chance";
            if (!noMag)
            {
                str1 = Utilities.FixDP(MagPercent);
                if (DisplayPercentage)
                    str1 += "%";
            }
            string str5;
            switch (EffectType)
            {
                case Enums.eEffectType.None:
                    str5 = Special;
                    if (Special == "Debt Protection" && !noMag)
                    {
                        str5 = str1 + "% " + str5;
                    }
                    break;
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    string name1 = Enum.GetName(typeof(Enums.eDamageShort), (Enums.eDamageShort)DamageType);
                    if (EffectType == Enums.eEffectType.Damage)
                    {
                        if (Ticks > 0)
                        {
                            str1 = Ticks + " * " + str1;
                            if (Duration > 0.0)
                                str2 = " over " + Utilities.FixDP(Duration) + " seconds";
                            else if (Absorbed_Duration > 0.0)
                                str2 = " over " + Utilities.FixDP(Absorbed_Duration) + " seconds";
                        }
                        str5 = str1 + " " + name1 + " " + effectNameShort1 + str3 + str2;
                        break;
                    }
                    string str6 = "(" + name1 + ")";
                    if (DamageType == Enums.eDamage.None)
                        str6 = string.Empty;
                    str5 = str1 + " " + effectNameShort1 + str6 + str3 + str2;
                    break;
                case Enums.eEffectType.Endurance:
                    if (noMag)
                    {
                        str5 = "+Max End";
                        break;
                    }
                    str5 = str1 + " " + effectNameShort1 + str3 + str2;
                    break;
                case Enums.eEffectType.Enhancement:
                    string str7 = ETModifies != Enums.eEffectType.Mez ? (!(ETModifies == Enums.eEffectType.Defense | ETModifies == Enums.eEffectType.Resistance) ? Enums.GetEffectNameShort(ETModifies) : Enums.GetDamageNameShort(DamageType) + " " + Enums.GetEffectNameShort(ETModifies)) : Enums.GetMezNameShort((Enums.eMezShort)MezType);
                    str5 = str1 + " " + effectNameShort1 + "(" + str7 + ")" + str3 + str2;
                    break;
                case Enums.eEffectType.GrantPower:
                    IPower powerByName = DatabaseAPI.GetPowerByName(Summon);
                    string str8 = powerByName == null ? " " + Summon : " " + powerByName.DisplayName;
                    str5 = effectNameShort1 + str8 + str3;
                    break;
                case Enums.eEffectType.Heal:
                case Enums.eEffectType.HitPoints:
                    if (noMag)
                    {
                        str5 = "+Max HP";
                        break;
                    }
                    if (Aspect == Enums.eAspect.Cur)
                    {
                        str5 = Utilities.FixDP(Mag * 100f) + "% " + effectNameShort1 + str3 + str2;
                        break;
                    }
                    if (!DisplayPercentage)
                    {
                        str5 = str1 + " (" + Utilities.FixDP((float)(Mag / (double)MidsContext.Archetype.Hitpoints * 100.0)) + "%)" + effectNameShort1 + str3 + str2;
                        break;
                    }
                    str5 = Utilities.FixDP(Mag / 100f * MidsContext.Archetype.Hitpoints) + " (" + str1 + ") " + effectNameShort1 + str3 + str2;
                    break;
                case Enums.eEffectType.Mez:
                    string name2 = Enum.GetName(MezType.GetType(), MezType);
                    if (Duration > 0.0 && (!simple || MezType != Enums.eMez.None && MezType != Enums.eMez.Knockback && MezType != Enums.eMez.Knockup))
                        str2 = Utilities.FixDP(Duration) + " second ";
                    string str9 = " (Mag " + str1 + ")";
                    str5 = str2 + name2 + str9 + str3;
                    break;
                case Enums.eEffectType.MezResist:
                    string name3 = Enum.GetName(MezType.GetType(), MezType);
                    if (!noMag)
                        str1 = " " + str1;
                    str5 = effectNameShort1 + "(" + name3 + ")" + str1 + str3 + str2;
                    break;
                case Enums.eEffectType.Recovery:
                    if (noMag)
                    {
                        str5 = "+Recovery";
                        break;
                    }
                    if (DisplayPercentage)
                    {
                        str5 = str1 + " (" + Utilities.FixDP(Mag * (MidsContext.Archetype.BaseRecovery * Statistics.BaseMagic)) + " /s) " + effectNameShort1 + str3 + str2;
                        break;
                    }
                    str5 = str1 + " " + effectNameShort1 + str3 + str2;
                    break;
                case Enums.eEffectType.Regeneration:
                    if (noMag)
                    {
                        str5 = "+Regeneration";
                        break;
                    }
                    if (DisplayPercentage)
                    {
                        str5 = str1 + " (" + Utilities.FixDP((float)(MidsContext.Archetype.Hitpoints / 100.0 * (Mag * (double)MidsContext.Archetype.BaseRegen * 1.66666662693024))) + " HP/s) " + effectNameShort1 + str3 + str2;
                        break;
                    }
                    str5 = str1 + " " + effectNameShort1 + str3 + str2;
                    break;
                case Enums.eEffectType.ResEffect:
                    string effectNameShort2 = Enums.GetEffectNameShort(ETModifies);
                    str5 = str1 + " " + effectNameShort1 + "(" + effectNameShort2 + ")" + str3 + str2;
                    break;
                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    str5 = str1 + "ft " + effectNameShort1 + str3 + str2;
                    break;
                case Enums.eEffectType.EntCreate:
                    int index = DatabaseAPI.NidFromUidEntity(Summon);
                    string str10 = index <= -1 ? " " + Summon : " " + DatabaseAPI.Database.Entities[index].DisplayName;
                    str5 = Duration <= 9999.0 ? effectNameShort1 + str10 + str3 + str2 : effectNameShort1 + str10 + str3;
                    break;
                case Enums.eEffectType.GlobalChanceMod:
                    str5 = str1 + " " + effectNameShort1 + " " + Reward + str3 + str2;
                    break;
                default:
                    str5 = str1 + " " + effectNameShort1 + str3 + str2;
                    break;
            }
            string iStr = string.Empty;
            if (!string.IsNullOrEmpty(iValue))
                iStr = " (" + BuildCs(iValue, iStr) + ")";
            return str5.Trim() + iStr + str4;
        }

        static string BuildCs(string iValue, string iStr, bool noComma = false)

        {
            if (string.IsNullOrEmpty(iValue))
                return iStr;
            string str = ", ";
            if (noComma)
                str = " ";
            if (!string.IsNullOrEmpty(iStr))
                iStr += str;
            iStr += iValue;
            return iStr;
        }

        public string BuildEffectString(
          bool simple = false,
          string specialCat = "",
          bool noMag = false,
          bool grouped = false,
          bool useBaseProbability = false)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            string iValue1 = string.Empty;
            string str3 = string.Empty;
            string iValue2 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            string iValue3 = string.Empty;
            string iValue4 = string.Empty;
            string str6 = string.Empty;
            string strCondition = string.Empty;
            string str7 = string.Empty;
            string iValue5 = string.Empty;
            string str8 = string.Empty;
            string iValue6 = string.Empty;
            if (power != null && power.VariableEnabled && VariableModified)
                str7 = " (Variable)";
            if (isEnhancementEffect)
                str8 = "(From Enh) ";
            string effectName = Enums.GetEffectName(EffectType);
            if (!simple)
            {
                switch (ToWho)
                {
                    case Enums.eToWho.Target:
                        str3 = " to Target";
                        break;
                    case Enums.eToWho.Self:
                        str3 = " to Self";
                        break;
                    case Enums.eToWho.Unspecified:
                        break;
                    case Enums.eToWho.All:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (RequiresToHitCheck)
                    iValue5 = " requires ToHit check";
            }
            if (ProcsPerMinute > 0.0 && Probability < 0.01)
                iValue1 = ((double)ProcsPerMinute) + "PPM";
            else if (useBaseProbability)
            {
                if (BaseProbability < 1.0)
                    iValue1 = (double)BaseProbability < 0.975000023841858 ? (BaseProbability * 100f).ToString("#0") + "% chance" : (BaseProbability * 100f).ToString("#0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0") + "% chance";
            }
            else if (Probability < 1.0)
            {
                iValue1 = Probability < 0.975000023841858 ? (Probability * 100f).ToString("#0") + "% chance" : (Probability * 100f).ToString("#0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0") + "% chance";
                if (CancelOnMiss)
                    iValue1 += ", CancelOnMiss";
            }
            bool noComma = false;
            if (!Resistible && !simple & ToWho != Enums.eToWho.Self | EffectType == Enums.eEffectType.Damage)
            {
                iValue4 = "Non-resistible";
                noComma = true;
            }
            switch (PvMode)
            {
                case Enums.ePvX.PvE:
                    iValue2 = noComma ? "by Critters" : "to Critters";
                    if (EffectType == Enums.eEffectType.Heal & Aspect == Enums.eAspect.Abs & Mag > 0.0 & PvMode == Enums.ePvX.PvE)
                        iValue2 = "in PvE";
                    if (ToWho == Enums.eToWho.Self)
                    {
                        iValue2 = "in PvE";
                    }
                    break;
                case Enums.ePvX.PvP:
                    iValue2 = noComma ? "by Players" : "to Players";
                    if (ToWho == Enums.eToWho.Self)
                    {
                        iValue2 = "in PvP";
                    }
                    break;
            }
            if (!simple)
            {
                if (!Buffable & EffectType != Enums.eEffectType.DamageBuff)
                    str5 = " [Ignores Enhancements & Buffs]";
                if (Stacking == Enums.eStacking.No)
                    str4 = "\n  Effect does not stack from same caster";
                if (DelayedTime > 0.0)
                    iValue3 = "after " + Utilities.FixDP(DelayedTime) + " seconds";
            }
            if (SpecialCase != Enums.eSpecialCase.None & SpecialCase != Enums.eSpecialCase.Defiance)
                str6 = Enum.GetName(SpecialCase.GetType(), SpecialCase);
            if (!simple || Scale > 0.0 && EffectType == Enums.eEffectType.Mez)
            {
                string strMez = " for ";
                switch (EffectType)
                {
                    case Enums.eEffectType.Damage:
                        strMez = " over ";
                        break;
                    case Enums.eEffectType.SilentKill:
                        strMez = " in ";
                        break;
                }
                str2 = !(Duration > 0.0 & (EffectType != Enums.eEffectType.Damage | Ticks > 0)) ? (!(Absorbed_Duration > 0.0 & (EffectType != Enums.eEffectType.Damage | Ticks > 0)) ? string.Empty + " " : string.Empty + strMez + Utilities.FixDP(Absorbed_Duration) + " seconds") : string.Empty + strMez + Utilities.FixDP(Duration) + " seconds";
                if (Absorbed_Interval > 0.0 && Absorbed_Interval < 900.0)
                    str2 = str2 + " every " + Utilities.FixDP(Absorbed_Interval) + " seconds";
            }
            if (!noMag & EffectType != Enums.eEffectType.SilentKill)
                str1 = !DisplayPercentage ? Utilities.FixDP(Mag) : Utilities.FixDP(Mag * 100f) + "%";
            if (!simple)
            {
                strCondition = string.Empty;
                if ((Suppression & Enums.eSuppress.ActivateAttackClick) == Enums.eSuppress.ActivateAttackClick)
                    strCondition += "\n  Suppressed when Attacking.";
                if ((Suppression & Enums.eSuppress.Attacked) == Enums.eSuppress.Attacked)
                    strCondition += "\n  Suppressed when Attacked.";
                if ((Suppression & Enums.eSuppress.HitByFoe) == Enums.eSuppress.HitByFoe)
                    strCondition += "\n  Suppressed when Hit.";
                if ((Suppression & Enums.eSuppress.MissionObjectClick) == Enums.eSuppress.MissionObjectClick)
                    strCondition += "\n  Suppressed when MissionObjectClick.";
                if ((Suppression & Enums.eSuppress.Held) == Enums.eSuppress.Held || (Suppression & Enums.eSuppress.Immobilized) == Enums.eSuppress.Immobilized || ((Suppression & Enums.eSuppress.Sleep) == Enums.eSuppress.Sleep || (Suppression & Enums.eSuppress.Stunned) == Enums.eSuppress.Stunned) || (Suppression & Enums.eSuppress.Terrorized) == Enums.eSuppress.Terrorized)
                    strCondition += "\n  Suppressed when Mezzed.";
                if ((Suppression & Enums.eSuppress.Knocked) == Enums.eSuppress.Knocked)
                    strCondition += "\n  Suppressed when Knocked.";
            }
            else if ((Suppression & Enums.eSuppress.ActivateAttackClick) == Enums.eSuppress.ActivateAttackClick || (Suppression & Enums.eSuppress.Attacked) == Enums.eSuppress.Attacked || (Suppression & Enums.eSuppress.HitByFoe) == Enums.eSuppress.HitByFoe)
                iValue6 = "Combat Suppression";
            string str10;
            switch (EffectType)
            {
                case Enums.eEffectType.None:
                    str10 = Special;
                    if (Special == "Debt Protection")
                    {
                        str10 = str1 + "% " + str10;
                    }
                    break;
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    if (!string.IsNullOrEmpty(specialCat))
                    {
                        str10 = str1 + " " + specialCat + " " + str3 + str2;
                        break;
                    }
                    string str11 = grouped ? "%VALUE%" : Enum.GetName(DamageType.GetType(), DamageType);
                    if (EffectType == Enums.eEffectType.Damage)
                    {
                        if (Ticks > 0)
                            str1 = Ticks + " x " + str1;
                        str10 = str1 + " " + str11 + " " + effectName + str3 + str2;
                        break;
                    }
                    string str12 = "(" + str11 + ")";
                    if (DamageType == Enums.eDamage.None)
                        str12 = string.Empty;
                    str10 = str1 + " " + effectName + str12 + str3 + str2;
                    break;
                case Enums.eEffectType.Endurance:
                    if (noMag)
                    {
                        str10 = "+Max End";
                        break;
                    }
                    if (Aspect == Enums.eAspect.Max)
                    {
                        str10 = str1 + "% Max End" + str3 + str2;
                        break;
                    }
                    str10 = str1 + " " + effectName + str3 + str2;
                    break;
                case Enums.eEffectType.Enhancement:
                    string str13 = ETModifies != Enums.eEffectType.Mez ? (!(ETModifies == Enums.eEffectType.Defense | ETModifies == Enums.eEffectType.Resistance) ? Enums.GetEffectName(ETModifies) : Enums.GetDamageName(DamageType) + " " + Enums.GetEffectName(ETModifies)) : Enums.GetMezName((Enums.eMezShort)MezType);
                    str10 = str1 + " " + effectName + "(" + str13 + ")" + str3 + str2;
                    break;
                case Enums.eEffectType.GrantPower:
                    iValue4 = string.Empty;
                    IPower powerByName = DatabaseAPI.GetPowerByName(Summon);
                    string str14 = powerByName == null ? " " + Summon : " " + powerByName.DisplayName;
                    str10 = effectName + str14 + str3;
                    break;
                case Enums.eEffectType.Heal:
                case Enums.eEffectType.HitPoints:
                    if (noMag)
                    {
                        str10 = "+Max HP";
                        break;
                    }
                    if (Ticks > 0)
                        str1 = Ticks + " x " + str1;
                    if (Aspect == Enums.eAspect.Cur)
                    {
                        str10 = Utilities.FixDP(Mag * 100f) + "% " + effectName + str3 + str2;
                        break;
                    }
                    if (!DisplayPercentage)
                    {
                        str10 = str1 + " HP (" + Utilities.FixDP((float)(Mag / (double)MidsContext.Archetype.Hitpoints * 100.0)) + "%) " + effectName + str3 + str2;
                        break;
                    }
                    str10 = Utilities.FixDP(Mag / 100f * MidsContext.Archetype.Hitpoints) + " HP (" + str1 + ") " + effectName + str3 + str2;
                    break;
                case Enums.eEffectType.Mez:
                    string name1 = Enum.GetName(MezType.GetType(), MezType);
                    if (Duration > 0.0 & (!simple | MezType != Enums.eMez.None & MezType != Enums.eMez.Knockback & MezType != Enums.eMez.Knockup))
                        str2 = Utilities.FixDP(Duration) + " second ";
                    if (!noMag)
                        str1 = " (Mag " + str1 + ")";
                    str10 = str2 + name1 + str1 + str3;
                    break;
                case Enums.eEffectType.MezResist:
                    string name2 = Enum.GetName(MezType.GetType(), MezType);
                    if (!noMag)
                        str1 = " " + str1;
                    str10 = effectName + "(" + name2 + ")" + str1 + str3 + str2;
                    break;
                case Enums.eEffectType.Recovery:
                    if (noMag)
                    {
                        str10 = "+Recovery";
                        break;
                    }
                    if (DisplayPercentage)
                    {
                        str10 = str1 + " (" + Utilities.FixDP(Mag * (MidsContext.Archetype.BaseRecovery * 1.666667f)) + " End/sec) " + effectName + str3 + str2;
                        break;
                    }
                    str10 = str1 + " " + effectName + str3 + str2;
                    break;
                case Enums.eEffectType.Regeneration:
                    if (noMag)
                    {
                        str10 = "+Regeneration";
                        break;
                    }
                    if (DisplayPercentage)
                    {
                        str10 = str1 + " (" + Utilities.FixDP((float)(MidsContext.Archetype.Hitpoints / 100.0 * (Mag * (double)MidsContext.Archetype.BaseRegen * 1.66666662693024))) + " HP/sec) " + effectName + str3 + str2;
                        break;
                    }
                    str10 = str1 + " " + effectName + str3 + str2;
                    break;
                case Enums.eEffectType.ResEffect:
                    string name3 = Enum.GetName(ETModifies.GetType(), ETModifies);
                    str10 = str1 + " " + effectName + "(" + name3 + ")" + str3 + str2;
                    break;
                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    str10 = str1 + "ft " + effectName + str3 + str2;
                    break;
                case Enums.eEffectType.EntCreate:
                    iValue4 = string.Empty;
                    int index = DatabaseAPI.NidFromUidEntity(Summon);
                    string str15 = index <= -1 ? " " + Summon : " " + DatabaseAPI.Database.Entities[index].DisplayName;
                    str10 = (double)Duration <= 9999.0 ? effectName + str15 + str3 + str2 : effectName + str15 + str3;
                    break;
                case Enums.eEffectType.GlobalChanceMod:
                    str10 = str1 + " " + effectName + " " + Reward + str3 + str2;
                    break;
                default:
                    str10 = str1 + " " + effectName + str3 + str2;
                    break;
            }
            string iStr1 = string.Empty;
            if (string.IsNullOrEmpty(iValue1 + iValue4 + iValue2 + iValue3 + str6 + iValue5 + iValue6))
                return str8 + str10 + iStr1 + str5 + str7 + str4 + strCondition;
            string iStr2 = BuildCs(iValue1, iStr1);
            string iStr3 = BuildCs(iValue3, iStr2);
            string iStr4 = BuildCs(iValue6, iStr3);
            string iStr5 = BuildCs(iValue4, iStr4);
            if (!string.IsNullOrEmpty(iValue2))
                iStr5 = !string.IsNullOrEmpty(str6) ? BuildCs(iValue2 + ", if " + str6, iStr5, noComma) : BuildCs(iValue2, iStr5, noComma);
            else if (!string.IsNullOrEmpty(str6))
                iStr5 = BuildCs("if " + str6, iStr5);
            iStr1 = " (" + BuildCs(iValue5, iStr5) + ")";
            return str8 + str10 + iStr1 + str5 + str7 + str4 + strCondition;
        }

        public List<string> NewEffects = new List<string>()
        {
            "TrainBeasts",
            "TameBeasts",
            "EnchantDemons",
            "AbyssalEmpowerment",
            "EquipMercenary",
            "TacticalUpgrade",
            "EnchantUndead",
            "DarkEmpowerment",
            "TrainNinjas",
            "KujiInZen",
            "EquipRobot",
            "UpgradeRobot",
            "EquipThugs",
            "UpgradeEquipment"
        };

        public void StoreTo(ref BinaryWriter writer)
        {
            writer.Write(PowerFullName);
            writer.Write(UniqueID);
            writer.Write((int)EffectClass);
            writer.Write((int)EffectType);
            writer.Write((int)DamageType);
            writer.Write((int)MezType);
            writer.Write((int)ETModifies);
            writer.Write(Summon);
            writer.Write(DelayedTime);
            writer.Write(Ticks);
            writer.Write((int)Stacking);
            writer.Write(BaseProbability);
            writer.Write((int)Suppression);
            writer.Write(Buffable);
            writer.Write(Resistible);
            writer.Write((int)SpecialCase);
            writer.Write(VariableModifiedOverride);
            writer.Write((int)PvMode);
            writer.Write((int)ToWho);
            writer.Write((int)DisplayPercentageOverride);
            writer.Write(Scale);
            writer.Write(nMagnitude);
            writer.Write(nDuration);
            writer.Write((int)AttribType);
            writer.Write((int)Aspect);
            DatabaseAPI.Database.EffectIds.RemoveRange(DatabaseAPI.Database.EffectIds.Count - NewEffects.Count, NewEffects.Count);
            DatabaseAPI.Database.EffectIds.AddRange(NewEffects.Except(DatabaseAPI.Database.EffectIds));
            writer.Write(ModifierTable);
            writer.Write(NearGround);
            writer.Write(CancelOnMiss);
            writer.Write(RequiresToHitCheck);
            writer.Write(UIDClassName);
            writer.Write(nIDClassName);
            writer.Write(MagnitudeExpression);
            writer.Write(Reward);
            writer.Write(EffectId);
            writer.Write(IgnoreED);
            writer.Write(Override);
            writer.Write(ProcsPerMinute);
        }

        public bool ImportFromCSV(string iCSV)
        {
            bool flag;
            if (iCSV == null)
                flag = false;
            else if (string.IsNullOrEmpty(iCSV))
            {
                flag = false;
            }
            else
            {
                string[] array = CSV.ToArray(iCSV);
                if (array.Length < 34)
                {
                    flag = false;
                }
                else
                {
                    if (UniqueID < 1)
                        UniqueID = int.Parse(array[34]);
                    PowerFullName = array[0];
                    Aspect = (Enums.eAspect)Enums.StringToFlaggedEnum(array[2], Aspect, true);
                    AttribType = (Enums.eAttribType)Enums.StringToFlaggedEnum(array[6], AttribType, true);
                    EffectId = array[37];
                    Reward = array[29];
                    MagnitudeExpression = array[27];
                    IgnoreED = int.Parse(array[38]) > 0;
                    EffectType = Enums.eEffectType.None;
                    ETModifies = Enums.eEffectType.None;
                    MezType = Enums.eMez.None;
                    DamageType = Enums.eDamage.None;
                    Special = string.Empty;
                    Summon = string.Empty;
                    if (Enums.IsEnumValue(array[3], Enums.eEffectType.None))
                    {
                        EffectType = (Enums.eEffectType)Enums.StringToFlaggedEnum(array[3], Enums.eEffectType.None, true);
                        switch (Aspect)
                        {
                            case Enums.eAspect.Res:
                                ETModifies = EffectType;
                                EffectType = Enums.eEffectType.ResEffect;
                                break;
                            case Enums.eAspect.Str:
                                ETModifies = EffectType;
                                EffectType = Enums.eEffectType.Enhancement;
                                break;
                        }
                    }
                    else if (Enums.IsEnumValue(array[3], Enums.eCSVImport_Damage.None))
                    {
                        DamageType = (Enums.eDamage)Enums.StringToFlaggedEnum(array[3], Enums.eCSVImport_Damage.None, true);
                        switch (Aspect)
                        {
                            case Enums.eAspect.Res:
                                EffectType = Enums.eEffectType.Resistance;
                                break;
                            case Enums.eAspect.Abs:
                                EffectType = Enums.eEffectType.Damage;
                                break;
                            case Enums.eAspect.Str:
                                EffectType = Enums.eEffectType.DamageBuff;
                                break;
                            case Enums.eAspect.Cur:
                                EffectType = Enums.eEffectType.Damage;
                                break;
                            default:
                                int num = (int)MessageBox.Show("Unable to interpret Damage-based attribute:\n" + array[0], "Interpretation Failed");
                                break;
                        }
                    }
                    else if (Enums.IsEnumValue(array[3], Enums.eCSVImport_Damage_Def.None))
                    {
                        DamageType = (Enums.eDamage)Enums.StringToFlaggedEnum(array[3], Enums.eCSVImport_Damage_Def.None, true);
                        switch (Aspect)
                        {
                            case Enums.eAspect.Str:
                                ETModifies = Enums.eEffectType.Defense;
                                EffectType = Enums.eEffectType.Enhancement;
                                break;
                            case Enums.eAspect.Cur:
                                EffectType = Enums.eEffectType.Defense;
                                break;
                        }
                    }
                    else if (Enums.IsEnumValue(array[3], Enums.eCSVImport_Damage_Elusivity.None))
                    {
                        DamageType = (Enums.eDamage)Enums.StringToFlaggedEnum(array[3], Enums.eCSVImport_Damage_Elusivity.None, true);
                        if (Aspect == Enums.eAspect.Str)
                        {
                            EffectType = Enums.eEffectType.Elusivity;
                        }
                        else
                        {
                            int num = (int)MessageBox.Show("Unable to interpret Elusivity field - not STR based:\n" + array[0], "Interpretation Failed");
                        }
                    }
                    else if (Enums.IsEnumValue(array[3], MezType))
                    {
                        MezType = (Enums.eMez)Enums.StringToFlaggedEnum(array[3], MezType, true);
                        switch (Aspect)
                        {
                            case Enums.eAspect.Res:
                                EffectType = Enums.eEffectType.MezResist;
                                break;
                            case Enums.eAspect.Abs:
                                EffectType = Enums.eEffectType.Mez;
                                break;
                            case Enums.eAspect.Str:
                                ETModifies = Enums.eEffectType.Mez;
                                EffectType = Enums.eEffectType.Enhancement;
                                break;
                            case Enums.eAspect.Cur:
                                EffectType = Enums.eEffectType.Mez;
                                break;
                            default:
                                int num = (int)MessageBox.Show("Unable to interpret Mez-based attribute:\n" + array[0], "Interpretation Failed");
                                break;
                        }
                    }
                    switch (EffectType)
                    {
                        case Enums.eEffectType.GrantPower:
                            Summon = Reward;
                            break;
                        case Enums.eEffectType.EntCreate:
                            Summon = array[13];
                            break;
                    }
                    nDuration = float.Parse(array[16]);
                    ModifierTable = array[1];
                    nMagnitude = float.Parse(array[17]);
                    Scale = float.Parse(array[5]);
                    NearGround = int.Parse(array[21]) > 0;
                    CancelOnMiss = int.Parse(array[22]) > 0;
                    Override = array[40];
                    ProcsPerMinute = float.Parse(array[59]);
                    Ticks = 0;
                    if (float.Parse(array[19]) > 0.0)
                        Ticks = (int)(1.0 + Math.Floor(nDuration / (double)float.Parse(array[19])));
                    DelayedTime = float.Parse(array[15]);
                    Stacking = array[18].ToLower() == "stack" ? Enums.eStacking.Yes : Enums.eStacking.No;
                    BaseProbability = float.Parse(array[20]);
                    Suppression = (Enums.eSuppress)Enums.StringToFlaggedEnum(array[9].Replace(" ", ","), Suppression);
                    if (Suppression == Enums.eSuppress.None)
                        Suppression = (Enums.eSuppress)Enums.StringToFlaggedEnum(array[10].Replace(" ", ","), Suppression);
                    Buffable = int.Parse(array[7]) > 0;
                    Resistible = int.Parse(array[8]) > 0;
                    string lower = array[26].ToLower();
                    if (power != null)
                    {
                        var ps = power.GetPowerSet();
                        if (ps != null && ps.nArchetype > -1)
                        {
                            if (lower.Contains("kDD".ToLower()))
                            {
                                SpecialCase = Enums.eSpecialCase.Combo;
                            }
                            else
                            {
                                switch (DatabaseAPI.Database.Classes[ps.nArchetype].PrimaryGroup.ToLower())
                                {
                                    case "scrapper_melee":
                                        var str = lower;
                                        if (EffectType == Enums.eEffectType.Damage && !string.IsNullOrEmpty(lower) && Probability <= 0.1)
                                        {
                                            switch (str)
                                            {
                                                case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || enttype target> player eq || ! arch source> class_scrapper == &&":
                                                case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || enttype target> player eq || !":
                                                    SpecialCase = Enums.eSpecialCase.CriticalHit;
                                                    goto label_77;
                                                case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || arch source> class_scrapper == &&":
                                                case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq ||":
                                                    SpecialCase = Enums.eSpecialCase.CriticalMinion;
                                                    goto label_77;
                                            }

                                            SpecialCase = Enums.eSpecialCase.CriticalHit;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "controller_control":
                                        if (EffectType == Enums.eEffectType.Damage && lower.Contains("kheld"))
                                        {
                                            SpecialCase = Enums.eSpecialCase.Containment;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "dominator_control":
                                        if (lower.Contains("kStealth source".ToLower()))
                                        {
                                            SpecialCase = Enums.eSpecialCase.Domination;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "stalker_melee":
                                        if (lower.Contains("kMeter source> 0".ToLower()))
                                            SpecialCase = Enums.eSpecialCase.Assassination;
                                        if (lower.Contains("kHeld target> 0".ToLower()))
                                            SpecialCase = Enums.eSpecialCase.Mezzed;
                                        if (MagnitudeExpression.IndexOf("TeamSize", StringComparison.OrdinalIgnoreCase) > -1)
                                        {
                                            SpecialCase = Enums.eSpecialCase.None;
                                            BaseProbability = 0.1f;
                                            AttribType = Enums.eAttribType.Magnitude;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "blaster_ranged":
                                        if (lower.Contains("kRange_Finder_Mode".ToLower()))
                                        {
                                            SpecialCase = Enums.eSpecialCase.TargetDroneActive;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "corruptor_ranged":
                                        if (lower.Contains("kHitPoints%".ToLower()))
                                        {
                                            SpecialCase = Enums.eSpecialCase.Scourge;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "arachnos_soldiers":
                                        if (lower.Contains("kMeter source> 0".ToLower()))
                                        {
                                            SpecialCase = Enums.eSpecialCase.Hidden;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    case "widow_training":
                                        if (lower.Contains("kMeter source> 0".ToLower()))
                                        {
                                            SpecialCase = Enums.eSpecialCase.Hidden;
                                            goto label_77;
                                        }
                                        else
                                            goto label_77;
                                    default:
                                        switch (lower)
                                        {
                                            case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || enttype target> player eq || ! arch source> class_scrapper == &&":
                                                SpecialCase = Enums.eSpecialCase.CriticalHit;
                                                goto default;
                                            case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || arch source> class_scrapper == &&":
                                                SpecialCase = Enums.eSpecialCase.CriticalMinion;
                                                goto default;
                                            case null:
                                                break;
                                            default:
                                                goto case null;
                                        }
                                        break;
                                }
                            }
                        }
                    label_77:
                        if (SpecialCase == Enums.eSpecialCase.None)
                        {
                            string str = lower;
                            if ((lower.Contains("arch source> class_scrapper eq") || lower.Contains("arch source> class_scrapper ==")) && Probability < 0.9)
                            {
                                switch (str)
                                {
                                    case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || enttype target> player eq || ! arch source> class_scrapper == &&":
                                    case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || enttype target> player eq || !":
                                        SpecialCase = Enums.eSpecialCase.CriticalHit;
                                        goto label_101;
                                    case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq || arch source> class_scrapper == &&":
                                    case "arch target> class_minion_grunt eq arch target> class_minion_small eq || arch target> class_minion_pets eq || arch target> class_minion_swarm eq ||":
                                        SpecialCase = Enums.eSpecialCase.CriticalMinion;
                                        goto label_101;
                                }

                                SpecialCase = Enums.eSpecialCase.CriticalHit;
                            }
                            else if ((lower.Contains("arch source> class_controller eq") || lower.Contains("arch source> class_controller ==".ToLower())) && lower.Contains("kheld") && EffectType == Enums.eEffectType.Damage)
                                SpecialCase = Enums.eSpecialCase.Containment;
                            else if (lower.Contains("kmeter source> .9") && lower.Contains("kheld"))
                                SpecialCase = Enums.eSpecialCase.Mezzed;
                            else if (lower.Contains("kmeter source> 0"))
                                SpecialCase = Enums.eSpecialCase.Assassination;
                            else if (lower.Contains("arch source> class_corruptor eq") && lower.Contains("khitpoints%"))
                                SpecialCase = Enums.eSpecialCase.Scourge;
                            else if (lower.Contains("arch source> class_dominator") && !lower.Contains("arch source> class_dominator eq !") && lower.Contains("kstealth source>"))
                                SpecialCase = Enums.eSpecialCase.Domination;
                            else if (lower.Contains("khitpoints%"))
                                SpecialCase = Enums.eSpecialCase.Scourge;
                            else if (lower.Contains("kdd"))
                                SpecialCase = Enums.eSpecialCase.Combo;
                        }
                    }
                label_101:
                    if (lower.Contains("Electronic target.HasTag?".ToLower()))
                        SpecialCase = Enums.eSpecialCase.Robot;
                    if (lower.IndexOf("source.TeamSize> 1", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.TeamSize1;
                    else if (lower.IndexOf("source.TeamSize> 2", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.TeamSize2;
                    else if (lower.IndexOf("source.TeamSize> 3", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.TeamSize3;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Combo_Level_1 source.ownPower? ! Temporary_Powers.Temporary_Powers.Combo_Level_2 source.ownPower? ! && Temporary_Powers.Temporary_Powers.Combo_Level_3 source.ownPower? !", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel0;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Combo_Level_1 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel1;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Combo_Level_2 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel2;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Combo_Level_3 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel3;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_1 source.ownPower? ! Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_2 source.ownPower? ! && Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_3 source.ownPower? !", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfBody0;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_1 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfBody1;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_2 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfBody2;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Body_Level_3 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfBody3;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_1 source.ownPower? ! Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_2 source.ownPower? ! && Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_3 source.ownPower? !", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfMind0;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_1 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfMind1;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_2 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfMind2;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Mind_Level_3 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfMind3;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_1 source.ownPower? ! Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_2 source.ownPower? ! && Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_3 source.ownPower? !", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfSoul0;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_1 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfSoul1;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_2 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfSoul2;
                    else if (lower.IndexOf("Temporary_Powers.Temporary_Powers.Perfection_of_Soul_Level_3 source.ownPower?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.PerfectionOfSoul3;
                    else if (lower.IndexOf("temporary_powers.temporary_powers.tidal_power source.ownPowerNum? 0 ==", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel0;
                    else if (lower.IndexOf("temporary_powers.temporary_powers.tidal_power source.ownPowerNum? 1 ==", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel1;
                    else if (lower.IndexOf("temporary_powers.temporary_powers.tidal_power source.ownPowerNum? 2 ==", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel2;
                    else if (lower.IndexOf("temporary_powers.temporary_powers.tidal_power source.ownPowerNum? 3 ==", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ComboLevel3;
                    else if (lower.IndexOf("temporary_powers.temporary_powers.tidal_power source.ownPowerNum? 2 <=", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.NotComboLevel3;
                    else if (lower.IndexOf("cur.kToHit source> .97 >=", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.ToHit97;
                    else if (lower.Contains("temporary_powers.temporary_powers.time_crawl_debuff target.ownpower? !"))
                        SpecialCase = Enums.eSpecialCase.NotDelayed;
                    else if (lower.Contains("temporary_powers.temporary_powers.time_crawl_debuff target.ownpower?"))
                        SpecialCase = Enums.eSpecialCase.Delayed;
                    else if (lower.Contains("temporary_powers.temporary_powers.temporal_selection_buff target.ownpower? !"))
                        SpecialCase = Enums.eSpecialCase.NotAccelerated;
                    else if (lower.Contains("temporary_powers.temporary_powers.temporal_selection_buff target.ownpower?"))
                        SpecialCase = Enums.eSpecialCase.Accelerated;
                    else if (lower.Contains("temporary_powers.temporary_powers.beam_rifle_debuff target.ownpower? !"))
                        SpecialCase = Enums.eSpecialCase.NotDisintegrated;
                    else if (lower.Contains("temporary_powers.temporary_powers.beam_rifle_debuff target.ownpower?"))
                        SpecialCase = Enums.eSpecialCase.Disintegrated;
                    else if (lower.Contains("kfastmode source.mode?"))
                        SpecialCase = Enums.eSpecialCase.FastMode;
                    else if (lower.IndexOf("kOffensiveAdaptation source.Mode? ! kDefensiveAdaptation source.Mode? ! &&", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.NotDefensiveNorOffensiveAdaptation;
                    else if (lower.IndexOf("kDefensiveAdaptation source.Mode? !", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.NotDefensiveAdaptation;
                    else if (lower.IndexOf("kDefensiveAdaptation source.Mode?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.DefensiveAdaptation;
                    else if (lower.IndexOf("kRestedAdaptation source.Mode?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.EfficientAdaptation;
                    else if (lower.IndexOf("kOffensiveAdaptation source.Mode?", StringComparison.OrdinalIgnoreCase) > -1)
                        SpecialCase = Enums.eSpecialCase.OffensiveAdaptation;
                    if (!string.IsNullOrEmpty(lower) && (!lower.Contains("!") || lower.Contains("Raid target.HasTag?".ToLower())) && UidClassRegex.IsMatch(array[26]))
                    {
                        UIDClassName = UidClassRegex.Matches(array[26])[0].Groups[2].Value;
                        nIDClassName = DatabaseAPI.NidFromUidClass(UIDClassName);
                    }
                    PvMode = !lower.Contains("entref target> entref source> eq ! enttype target> player eq &&".ToLower()) ? (!(lower.Contains("enttype target> player eq || !".ToLower()) | SpecialCase == Enums.eSpecialCase.CriticalMinion) ? (!lower.Contains("enttype target> critter eq".ToLower()) ? (!lower.Contains("enttype target> player eq".ToLower()) ? (!lower.Contains("isPVPMap? !".ToLower()) ? (!lower.Contains("isPVPMap?".ToLower()) ? Enums.ePvX.Any : Enums.ePvX.PvP) : Enums.ePvX.PvE) : Enums.ePvX.PvP) : Enums.ePvX.PvE) : Enums.ePvX.PvE) : Enums.ePvX.Any;
                    if (lower.Contains("@ToHitRoll".ToLower()))
                    {
                        RequiresToHitCheck = true;
                        if (lower.Contains("Raid target.HasTag? @ToHitRoll".ToLower()))
                            SpecialCase = Enums.eSpecialCase.VersusSpecial;
                    }
                    switch (array[4].ToLower())
                    {
                        case "self":
                            ToWho = Enums.eToWho.Self;
                            goto default;
                        case "target":
                            ToWho = power == null ? Enums.eToWho.Target : ((power.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster || lower == "entref target> entref source> eq !" ? Enums.eToWho.Target : Enums.eToWho.All);
                            goto default;
                        case null:
                            var ps = power?.GetPowerSet();
                            if (power != null && ps != null)
                            {
                                if (string.Equals(ps.ATClass, "CLASS_BLASTER", StringComparison.OrdinalIgnoreCase))
                                {
                                    nModifierTable = DatabaseAPI.NidFromUidAttribMod(ModifierTable);
                                    if (EffectType == Enums.eEffectType.DamageBuff & Scale < 1.0 && Scale > 0.0 && ToWho == Enums.eToWho.Self && SpecialCase == Enums.eSpecialCase.None)
                                        SpecialCase = Enums.eSpecialCase.Defiance;
                                }
                                else if (ps.SetType == Enums.ePowerSetType.Inherent && EffectType == Enums.eEffectType.DamageBuff && (AttribType == Enums.eAttribType.Expression & Math.Abs(Scale - 0.0f) < 0.01 && power.Requires.ClassName.Length > 0) && string.Equals(power.Requires.ClassName[0], "CLASS_BRUTE", StringComparison.OrdinalIgnoreCase))
                                {
                                    Stacking = Enums.eStacking.Yes;
                                    AttribType = Enums.eAttribType.Magnitude;
                                    Scale = 0.02f;
                                }
                            }
                            flag = true;
                            break;
                        default:
                            goto case null;
                    }
                }
            }
            return flag;
        }

        public int SetTicks(float iDuration, float iInterval)
        {
            Ticks = 0;
            if (iInterval > 0.0)
                Ticks = (int)(1.0 + Math.Floor(iDuration / (double)iInterval));
            return Ticks;
        }

        public bool CanInclude()
        {
            if (MidsContext.Character == null)
                return true;
            switch (SpecialCase)
            {
                case Enums.eSpecialCase.None:
                    return true;
                case Enums.eSpecialCase.Hidden:
                    if (MidsContext.Character.IsStalker || MidsContext.Character.IsArachnos)
                        return true;
                    break;
                case Enums.eSpecialCase.Domination:
                    if (MidsContext.Character.Domination)
                        return true;
                    break;
                case Enums.eSpecialCase.Scourge:
                    if (MidsContext.Character.Scourge)
                        return true;
                    break;
                case Enums.eSpecialCase.CriticalHit:
                    if (MidsContext.Character.CriticalHits || MidsContext.Character.IsStalker)
                        return true;
                    break;
                case Enums.eSpecialCase.CriticalBoss:
                    if (MidsContext.Character.CriticalHits)
                        return true;
                    break;
                case Enums.eSpecialCase.Assassination:
                    if (MidsContext.Character.IsStalker && MidsContext.Character.Assassination)
                        return true;
                    break;
                case Enums.eSpecialCase.Containment:
                    if (MidsContext.Character.Containment)
                        return true;
                    break;
                case Enums.eSpecialCase.Defiance:
                    if (MidsContext.Character.Defiance)
                        return true;
                    break;
                case Enums.eSpecialCase.TargetDroneActive:
                    if (MidsContext.Character.IsBlaster && MidsContext.Character.TargetDroneActive)
                        return true;
                    break;
                case Enums.eSpecialCase.NotDisintegrated:
                    if (!MidsContext.Character.DisintegrateActive)
                        return true;
                    break;
                case Enums.eSpecialCase.Disintegrated:
                    if (MidsContext.Character.DisintegrateActive)
                        return true;
                    break;
                case Enums.eSpecialCase.NotAccelerated:
                    if (!MidsContext.Character.AcceleratedActive)
                        return true;
                    break;
                case Enums.eSpecialCase.Accelerated:
                    if (MidsContext.Character.AcceleratedActive)
                        return true;
                    break;
                case Enums.eSpecialCase.NotDelayed:
                    if (!MidsContext.Character.DelayedActive)
                        return true;
                    break;
                case Enums.eSpecialCase.Delayed:
                    if (MidsContext.Character.DelayedActive)
                        return true;
                    break;
                case Enums.eSpecialCase.ComboLevel0:
                    if (MidsContext.Character.ActiveComboLevel == 0)
                        return true;
                    break;
                case Enums.eSpecialCase.ComboLevel1:
                    if (MidsContext.Character.ActiveComboLevel == 1)
                        return true;
                    break;
                case Enums.eSpecialCase.ComboLevel2:
                    if (MidsContext.Character.ActiveComboLevel == 2)
                        return true;
                    break;
                case Enums.eSpecialCase.ComboLevel3:
                    if (MidsContext.Character.ActiveComboLevel == 3)
                        return true;
                    break;
                case Enums.eSpecialCase.FastMode:
                    if (MidsContext.Character.FastModeActive)
                        return true;
                    break;
                case Enums.eSpecialCase.NotAssassination:
                    if (!MidsContext.Character.Assassination)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfBody0:
                    if (MidsContext.Character.PerfectionOfBodyLevel == 0)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfBody1:
                    if (MidsContext.Character.PerfectionOfBodyLevel == 1)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfBody2:
                    if (MidsContext.Character.PerfectionOfBodyLevel == 2)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfBody3:
                    if (MidsContext.Character.PerfectionOfBodyLevel == 3)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfMind0:
                    if (MidsContext.Character.PerfectionOfMindLevel == 0)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfMind1:
                    if (MidsContext.Character.PerfectionOfMindLevel == 1)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfMind2:
                    if (MidsContext.Character.PerfectionOfMindLevel == 2)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfMind3:
                    if (MidsContext.Character.PerfectionOfMindLevel == 3)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfSoul0:
                    if (MidsContext.Character.PerfectionOfSoulLevel == 0)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfSoul1:
                    if (MidsContext.Character.PerfectionOfSoulLevel == 1)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfSoul2:
                    if (MidsContext.Character.PerfectionOfSoulLevel == 2)
                        return true;
                    break;
                case Enums.eSpecialCase.PerfectionOfSoul3:
                    if (MidsContext.Character.PerfectionOfSoulLevel == 3)
                        return true;
                    break;
                case Enums.eSpecialCase.TeamSize1:
                    if (MidsContext.Config.TeamSize > 1)
                        return true;
                    break;
                case Enums.eSpecialCase.TeamSize2:
                    if (MidsContext.Config.TeamSize > 2)
                        return true;
                    break;
                case Enums.eSpecialCase.TeamSize3:
                    if (MidsContext.Config.TeamSize > 3)
                        return true;
                    break;
                case Enums.eSpecialCase.NotComboLevel3:
                    if (MidsContext.Character.ActiveComboLevel != 3)
                        return true;
                    break;
                case Enums.eSpecialCase.ToHit97:
                    if (MidsContext.Character.DisplayStats.BuffToHit >= 22.0)
                        return true;
                    break;
                case Enums.eSpecialCase.DefensiveAdaptation:
                    if (MidsContext.Character.DefensiveAdaptation)
                        return true;
                    break;
                case Enums.eSpecialCase.EfficientAdaptation:
                    if (MidsContext.Character.EfficientAdaptation)
                        return true;
                    break;
                case Enums.eSpecialCase.OffensiveAdaptation:
                    if (MidsContext.Character.OffensiveAdaptation)
                        return true;
                    break;
                case Enums.eSpecialCase.NotDefensiveAdaptation:
                    if (!MidsContext.Character.DefensiveAdaptation)
                        return true;
                    break;
                case Enums.eSpecialCase.NotDefensiveNorOffensiveAdaptation:
                    if (!MidsContext.Character.OffensiveAdaptation && !MidsContext.Character.DefensiveAdaptation)
                        return true;
                    break;
                case Enums.eSpecialCase.BoxingBuff:
                    if (MidsContext.Character.BoxingBuff)
                        return true;
                    break;
                case Enums.eSpecialCase.KickBuff:
                    if (MidsContext.Character.KickBuff)
                        return true;
                    break;
                case Enums.eSpecialCase.Supremacy:
                    if (MidsContext.Character.Supremacy)
                        return true;
                    break;
                case Enums.eSpecialCase.PetTier2:
                    if (MidsContext.Character.PetTier2)
                        return true;
                    break;
                case Enums.eSpecialCase.PetTier3:
                    if (MidsContext.Character.PetTier3)
                        return true;
                    break;
                case Enums.eSpecialCase.PetBuffPwr:
                    if (MidsContext.Character.PetBuffPwr)
                        return true;
                    break;
            }
            return false;
        }

        public bool PvXInclude()
        {
            return MidsContext.Archetype == null || (PvMode != Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE || PvMode != Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE) && (nIDClassName == -1 || nIDClassName == MidsContext.Archetype.Idx);
        }

        float ParseMagnitudeExpression()

        {
            float num1;
            if (MagnitudeExpression.IndexOf(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /", StringComparison.OrdinalIgnoreCase) > -1)
            {
                float num2 = (float)((Math.Max(Math.Min(power.RechargeTime, 30f), 0.0f) * 0.800000011920929 + 1.79999995231628) / 5.0) / power.AoEModifier * Scale;
                if (MagnitudeExpression.Length > ".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 2)
                    num2 *= float.Parse(MagnitudeExpression.Substring(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 1).Substring(0, 2));
                num1 = num2;
            }
            else
                num1 = 0.0f;
            return num1;
        }

        public int CompareTo(object obj)
        {
            int num1;
            if (obj == null)
            {
                num1 = 1;
            }
            else
            {
                if (!(obj is Effect effect))
                    throw new ArgumentException("Compare failed, object is not a Power Effect class");
                int num2 = 0;
                if (VariableModified & !effect.VariableModified)
                    num2 = 1;
                else if (!VariableModified & effect.VariableModified)
                    num2 = -1;
                if (num2 == 0)
                {
                    if (Suppression < effect.Suppression)
                        num2 = 1;
                    else if (Suppression > effect.Suppression)
                        num2 = -1;
                }
                num1 = !(effect.EffectType == Enums.eEffectType.None & EffectType != Enums.eEffectType.None) ? (!(effect.EffectType != Enums.eEffectType.None & EffectType == Enums.eEffectType.None) ? (EffectType <= effect.EffectType ? (EffectType >= effect.EffectType ? (!IgnoreED || effect.IgnoreED ? (IgnoreED || !effect.IgnoreED ? EffectId == effect.EffectId ? (Reward == effect.Reward ? (MagnitudeExpression == effect.MagnitudeExpression ? (!effect.isDamage() ? (effect.EffectType != Enums.eEffectType.ResEffect ? (effect.EffectType != Enums.eEffectType.Mez && effect.EffectType != Enums.eEffectType.MezResist ? (effect.EffectType != Enums.eEffectType.Enhancement ? (effect.EffectType != Enums.eEffectType.None ? num2 : string.CompareOrdinal(Special, effect.Special)) : (ETModifies <= effect.ETModifies ? (ETModifies >= effect.ETModifies ? ((double)Mag <= (double)effect.Mag ? ((double)Mag >= (double)effect.Mag ? ((double)Duration <= (double)effect.Duration ? ((double)Duration >= (double)effect.Duration ? num2 : -1) : 1) : -1) : 1) : 1) : 1)) : (MezType <= effect.MezType ? (MezType >= effect.MezType ? ((double)Mag <= (double)effect.Mag ? ((double)Mag >= (double)effect.Mag ? ((double)Duration <= (double)effect.Duration ? ((double)Duration >= (double)effect.Duration ? num2 : -1) : 1) : -1) : 1) : -1) : 1)) : (ETModifies <= effect.ETModifies ? (ETModifies >= effect.ETModifies ? ((double)Mag <= (double)effect.Mag ? ((double)Mag >= (double)effect.Mag ? num2 : -1) : 1) : -1) : 1)) : (DamageType <= effect.DamageType ? (DamageType >= effect.DamageType ? ((double)Mag <= (double)effect.Mag ? ((double)Mag >= (double)effect.Mag ? num2 : -1) : 1) : -1) : 1)) : string.CompareOrdinal(MagnitudeExpression, effect.MagnitudeExpression)) : string.CompareOrdinal(Reward, effect.Reward)) : string.CompareOrdinal(EffectId, effect.EffectId) : -1) : 1) : -1) : 1) : 1) : -1;
            }
            return num1;
        }

        public object Clone()
        {
            return new Effect(this);
        }

        public Effect()

        {
            BaseProbability = 1f;
            MagnitudeExpression = string.Empty;
            Reward = string.Empty;
            EffectClass = Enums.eEffectClass.Primary;
            EffectType = Enums.eEffectType.None;
            DisplayPercentageOverride = Enums.eOverrideBoolean.NoOverride;
            DamageType = Enums.eDamage.None;
            MezType = Enums.eMez.None;
            ETModifies = Enums.eEffectType.None;
            Summon = string.Empty;
            Stacking = Enums.eStacking.No;
            Suppression = Enums.eSuppress.None;
            Buffable = true;
            Resistible = true;
            SpecialCase = Enums.eSpecialCase.None;
            UIDClassName = string.Empty;
            nIDClassName = -1;
            PvMode = Enums.ePvX.Any;
            ToWho = Enums.eToWho.Unspecified;
            AttribType = Enums.eAttribType.Magnitude;
            Aspect = Enums.eAspect.Str;
            ModifierTable = "Melee_Ones";
            PowerFullName = string.Empty;
            Absorbed_PowerType = Enums.ePowerType.Auto_;
            Absorbed_Power_nID = -1;
            Absorbed_Class_nID = -1;
            Absorbed_EffectID = -1;
            Override = string.Empty;
            buffMode = Enums.eBuffMode.Normal;
            Special = string.Empty;
            EffectId = "Ones";
        }

        public Effect(IPower power)
          : this()
        {
            this.power = power;
        }

        public Effect(BinaryReader reader)
          : this()
        {
            PowerFullName = reader.ReadString();
            UniqueID = reader.ReadInt32();
            EffectClass = (Enums.eEffectClass)reader.ReadInt32();
            EffectType = (Enums.eEffectType)reader.ReadInt32();
            DamageType = (Enums.eDamage)reader.ReadInt32();
            MezType = (Enums.eMez)reader.ReadInt32();
            ETModifies = (Enums.eEffectType)reader.ReadInt32();
            Summon = reader.ReadString();
            DelayedTime = reader.ReadSingle();
            Ticks = reader.ReadInt32();
            Stacking = (Enums.eStacking)reader.ReadInt32();
            BaseProbability = reader.ReadSingle();
            Suppression = (Enums.eSuppress)reader.ReadInt32();
            Buffable = reader.ReadBoolean();
            Resistible = reader.ReadBoolean();
            SpecialCase = (Enums.eSpecialCase)reader.ReadInt32();
            VariableModifiedOverride = reader.ReadBoolean();
            PvMode = (Enums.ePvX)reader.ReadInt32();
            ToWho = (Enums.eToWho)reader.ReadInt32();
            DisplayPercentageOverride = (Enums.eOverrideBoolean)reader.ReadInt32();
            Scale = reader.ReadSingle();
            nMagnitude = reader.ReadSingle();
            nDuration = reader.ReadSingle();
            AttribType = (Enums.eAttribType)reader.ReadInt32();
            Aspect = (Enums.eAspect)reader.ReadInt32();
            ModifierTable = reader.ReadString();
            nModifierTable = DatabaseAPI.NidFromUidAttribMod(ModifierTable);
            NearGround = reader.ReadBoolean();
            CancelOnMiss = reader.ReadBoolean();
            RequiresToHitCheck = reader.ReadBoolean();
            UIDClassName = reader.ReadString();
            nIDClassName = reader.ReadInt32();
            MagnitudeExpression = reader.ReadString();
            Reward = reader.ReadString();
            EffectId = reader.ReadString();
            IgnoreED = reader.ReadBoolean();
            Override = reader.ReadString();
            ProcsPerMinute = reader.ReadSingle();
            if (DatabaseAPI.Database.EffectIds.Contains(EffectId))
                return;
            DatabaseAPI.Database.EffectIds.Add(EffectId);
        }

        Effect(IEffect template)

          : this()
        {
            PowerFullName = template.PowerFullName;
            power = template.GetPower();
            Enhancement = template.Enhancement;
            UniqueID = template.UniqueID;
            EffectClass = template.EffectClass;
            EffectType = template.EffectType;
            DisplayPercentageOverride = template.DisplayPercentageOverride;
            DamageType = template.DamageType;
            MezType = template.MezType;
            ETModifies = template.ETModifies;
            Summon = template.Summon;
            Ticks = template.Ticks;
            DelayedTime = template.DelayedTime;
            Stacking = template.Stacking;
            BaseProbability = template.BaseProbability;
            Suppression = template.Suppression;
            Buffable = template.Buffable;
            Resistible = template.Resistible;
            SpecialCase = template.SpecialCase;
            VariableModifiedOverride = template.VariableModifiedOverride;
            isEnhancementEffect = template.isEnhancementEffect;
            PvMode = template.PvMode;
            ToWho = template.ToWho;
            Scale = template.Scale;
            nMagnitude = template.nMagnitude;
            nDuration = template.nDuration;
            AttribType = template.AttribType;
            Aspect = template.Aspect;
            ModifierTable = template.ModifierTable;
            nModifierTable = template.nModifierTable;
            NearGround = template.NearGround;
            CancelOnMiss = template.CancelOnMiss;
            ProcsPerMinute = template.ProcsPerMinute;
            Absorbed_Duration = template.Absorbed_Duration;
            Absorbed_Effect = template.Absorbed_Effect;
            Absorbed_PowerType = template.Absorbed_PowerType;
            Absorbed_Class_nID = template.Absorbed_Class_nID;
            Absorbed_Interval = template.Absorbed_Interval;
            Absorbed_EffectID = template.Absorbed_EffectID;
            buffMode = template.buffMode;
            Math_Duration = template.Math_Duration;
            Math_Mag = template.Math_Mag;
            RequiresToHitCheck = template.RequiresToHitCheck;
            UIDClassName = template.UIDClassName;
            nIDClassName = template.nIDClassName;
            MagnitudeExpression = template.MagnitudeExpression;
            Reward = template.Reward;
            EffectId = template.EffectId;
            IgnoreED = template.IgnoreED;
            Override = template.Override;
        }
    }
}

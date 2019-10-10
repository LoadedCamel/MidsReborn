using System;
using System.Globalization;
using System.Text;
using Base.Data_Classes;

public class I9Slot : ICloneable
{
    const float SuperiorMult = 1.25f;

    public int Enh;
    public Enums.eEnhRelative RelativeLevel;
    public Enums.eEnhGrade Grade;
    public int IOLevel;

    public I9Slot()
    {
        Enh = -1;
        RelativeLevel = Enums.eEnhRelative.Even;
        Grade = Enums.eEnhGrade.None;
        IOLevel = 1;
    }

    public float GetEnhancementEffect(Enums.eEnhance iEffect, int subEnh, float mag)
    {
        if (Enh < 0)
        {
            return 0.0f;
        }

        float num2 = 0.0f;
        IEnhancement enhancement = DatabaseAPI.Database.Enhancements[Enh];
        foreach (Enums.sEffect sEffect in enhancement.Effect)
        {
            if (sEffect.Mode != Enums.eEffMode.Enhancement || (sEffect.BuffMode == Enums.eBuffDebuff.DeBuffOnly && !(mag <= 0.0)) ||
                ((sEffect.BuffMode == Enums.eBuffDebuff.BuffOnly && !(mag >= 0.0)) ||
                 (sEffect.Schedule == Enums.eSchedule.None || (Enums.eEnhance) sEffect.Enhance.ID != iEffect)) ||
                (subEnh >= 0 && subEnh != sEffect.Enhance.SubID))
                continue;
            float scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
            if (Math.Abs(sEffect.Multiplier) > 0.01)
                scheduleMult *= sEffect.Multiplier;
            num2 += scheduleMult;
        }
        return num2;
    }

    float GetScheduleMult(Enums.eType iType, Enums.eSchedule iSched)

    {
        if (Grade < Enums.eEnhGrade.None)
            Grade = Enums.eEnhGrade.None;
        if (RelativeLevel < Enums.eEnhRelative.None)
            RelativeLevel = Enums.eEnhRelative.None;
        if (Grade > Enums.eEnhGrade.SingleO)
            Grade = Enums.eEnhGrade.SingleO;
        if (RelativeLevel > Enums.eEnhRelative.PlusFive)
            RelativeLevel = Enums.eEnhRelative.PlusFive;
        float num1 = 0.0f;
        if (IOLevel <= 0)
            IOLevel = 0;
        if (IOLevel > DatabaseAPI.Database.MultIO.Length - 1)
            IOLevel = DatabaseAPI.Database.MultIO.Length - 1;
        if (iSched == Enums.eSchedule.None || iSched == Enums.eSchedule.Multiple)
        {
            num1 = 0.0f;
        }
        else
        {
            switch (iType)
            {
                case Enums.eType.Normal:
                    switch (Grade)
                    {
                        case Enums.eEnhGrade.None:
                            num1 = 0.0f;
                            break;
                        case Enums.eEnhGrade.TrainingO:
                            num1 = DatabaseAPI.Database.MultTO[0][(int)iSched];
                            break;
                        case Enums.eEnhGrade.DualO:
                            num1 = DatabaseAPI.Database.MultDO[0][(int)iSched];
                            break;
                        case Enums.eEnhGrade.SingleO:
                            num1 = DatabaseAPI.Database.MultSO[0][(int)iSched];
                            break;
                    }
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
        float num2 = num1 * GetRelativeLevelMultiplier();
        if (Enh > -1 && DatabaseAPI.Database.Enhancements[Enh].Superior)
            num2 *= 1.25f;
        return num2;
    }

    float GetRelativeLevelMultiplier()

    {
        float num1;
        if (RelativeLevel == Enums.eEnhRelative.None)
        {
            num1 = 0.0f;
        }
        else
        {
            int num2 = (int)(RelativeLevel - 4);
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
            IEnhancement enhancement = DatabaseAPI.Database.Enhancements[Enh];
            if (enhancement.Effect.Length == 0)
            {
                str1 = enhancement.Desc;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                bool flag = false;
                Enums.sEffect[] effect = enhancement.Effect;
                int index1 = 0;
                if (index1 >= effect.Length)
                {
                    str1 = stringBuilder.ToString();
                }
                else
                {
                    Enums.sEffect sEffect = effect[index1];
                    if (sEffect.Mode == Enums.eEffMode.FX)
                        flag = true;
                    string str2;
                    if (sEffect.Mode == Enums.eEffMode.Enhancement && sEffect.Schedule != Enums.eSchedule.None)
                    {
                        float scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
                        if (sEffect.Multiplier > 0.0)
                            scheduleMult *= sEffect.Multiplier;
                        if (stringBuilder.Length > 0)
                            stringBuilder.Append(", ");
                        switch (enhancement.TypeID)
                        {
                            case Enums.eType.Normal:
                                string relativeString1 = Enums.GetRelativeString(RelativeLevel, false);
                                if (!string.IsNullOrEmpty(relativeString1) & relativeString1 != "X")
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
                                string relativeString2 = Enums.GetRelativeString(RelativeLevel, false);
                                if (!string.IsNullOrEmpty(relativeString2) & relativeString2 != "X")
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
                        stringBuilder.Append(sEffect.Schedule.ToString());
                        stringBuilder.AppendFormat(" ({0}%)", (scheduleMult * 100f).ToString(NumberFormatInfo.CurrentInfo));
                        str2 = stringBuilder.ToString();
                    }
                    else if (!flag)
                    {
                        str2 = stringBuilder.ToString();
                    }
                    else
                    {
                        var power = enhancement.GetPower();
                        for (int index2 = 0; index2 <= power.Effects.Length - 1; ++index2)
                        {
                            if (stringBuilder.Length > 0)
                                stringBuilder.Append(", ");
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

    public string GetEnhancementStringLong()
    {
        string str1;
        if (Enh < 0)
        {
            str1 = string.Empty;
        }
        else
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            IEnhancement enhancement = DatabaseAPI.Database.Enhancements[Enh];
            if (enhancement.Effect.Length == 0)
            {
                str1 = enhancement.Desc;
            }
            else
            {
                foreach (Enums.sEffect sEffect in enhancement.Effect)
                {
                    switch (sEffect.Mode)
                    {
                        case Enums.eEffMode.FX:
                            flag1 = true;
                            break;
                        case Enums.eEffMode.Enhancement when sEffect.Schedule != Enums.eSchedule.None:
                        {
                            float scheduleMult = GetScheduleMult(enhancement.TypeID, sEffect.Schedule);
                            if (sEffect.Multiplier > 0.0)
                                scheduleMult *= sEffect.Multiplier;
                            Enums.eEnhance id = (Enums.eEnhance)sEffect.Enhance.ID;
                            string str2;
                            if (id == Enums.eEnhance.Mez)
                            {
                                Enums.eMez subId = (Enums.eMez)sEffect.Enhance.SubID;
                                str2 = Enum.GetName(subId.GetType(), subId);
                            }
                            else
                                str2 = Enum.GetName(id.GetType(), id);
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
                                    if (((enhancement.Name.IndexOf("Slow", StringComparison.Ordinal) > -1 ? 1 : 0) & (sEffect.BuffMode != Enums.eBuffDebuff.DeBuffOnly ? 0 : (sEffect.Enhance.ID == 6 || sEffect.Enhance.ID == 11 ? 1 : (sEffect.Enhance.ID == 19 ? 1 : 0)))) != 0 || sEffect.Enhance.ID == 21)
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
                                    stringBuilder.Append("\n");
                                stringBuilder.AppendFormat("{0}  enhancement (Sched. {1}: {2}%)", str2, Enum.GetName(sEffect.Schedule.GetType(), sEffect.Schedule), (scheduleMult * 100f).ToString(NumberFormatInfo.CurrentInfo));
                            }

                            break;
                        }
                        case Enums.eEffMode.PowerEnh:
                            break;
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
                    IPower power = new Power(enhancement.GetPower());
                    power.ApplyGrantPowerEffects();
                    int[] returnMask = new int[0];
                    for (int index1 = 0; index1 <= power.Effects.Length - 1; ++index1)
                    {
                        if (power.Effects[index1].EffectType == Enums.eEffectType.GrantPower && power.Effects[index1].CanGrantPower())
                        {
                            if (stringBuilder.Length > 0)
                                stringBuilder.Append("\n");
                            stringBuilder.Append(power.Effects[index1].BuildEffectString(true));
                            string empty = string.Empty;
                            for (int idEffect = 0; idEffect <= power.Effects.Length - 1; ++idEffect)
                            {
                                power.Effects[idEffect].Stacking = Enums.eStacking.Yes;
                                power.Effects[idEffect].Buffable = true;
                                if (power.Effects[idEffect].Absorbed_EffectID == index1)
                                    power.GetEffectStringGrouped(idEffect, ref empty, ref returnMask, false, false);
                                if (returnMask.Length <= 0)
                                    continue;
                                if (stringBuilder.Length > 0)
                                    stringBuilder.Append("\n");
                                stringBuilder.AppendFormat("  {0}", empty);
                                break;
                            }
                            for (int index2 = 0; index2 <= power.Effects.Length - 1; ++index2)
                            {
                                bool flag6 = false;
                                for (int index3 = 0; index3 <= returnMask.Length - 1; ++index3)
                                {
                                    if (returnMask[index3] != index2)
                                        continue;
                                    flag6 = true;
                                    break;
                                }

                                if (power.Effects[index2].Absorbed_EffectID != index1 || flag6)
                                    continue;
                                if (stringBuilder.Length > 0)
                                    stringBuilder.Append("\n");
                                power.Effects[index2].Stacking = Enums.eStacking.Yes;
                                power.Effects[index2].Buffable = true;
                                stringBuilder.AppendFormat("  {0}", power.Effects[index2].BuildEffectString());
                            }
                        }
                        else if (!power.Effects[index1].Absorbed_Effect && power.Effects[index1].EffectType != Enums.eEffectType.Enhancement)
                        {
                            if (stringBuilder.Length > 0)
                                stringBuilder.Append("\n");
                            stringBuilder.Append(power.Effects[index1].BuildEffectString(true));
                        }
                    }
                    str1 = stringBuilder.ToString().Replace("Slf", "Self").Replace("Tgt", "Target");
                }
            }
        }
        return str1;
    }

    public object Clone()
    {
        return new I9Slot
        {
            Enh = Enh,
            Grade = Grade,
            IOLevel = IOLevel,
            RelativeLevel = RelativeLevel
        };
    }

    public string GetRelativeString(bool onlySign)
    {
        if (onlySign)
        {
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
        }
        else
        {
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
        }
        return string.Empty;
    }
}

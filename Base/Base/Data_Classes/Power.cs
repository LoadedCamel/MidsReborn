
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Base.Master_Classes;

namespace Base.Data_Classes
{
    public class Power : IPower, IComparable
    {
        private bool Contains;
        public IPowerset GetPowerSet() => !(PowerSetID < 0 | PowerSetID > DatabaseAPI.Database.Powersets.Length) ? DatabaseAPI.Database.Powersets[PowerSetID] : null;

        public float CastTimeReal { get; set; }

        public float ToggleCost
                => !(PowerType == Enums.ePowerType.Toggle & ActivatePeriod > 0.0) ? EndCost : EndCost / ActivatePeriod;

        public bool IsEpic
                => Requires.NPowerID.Length > 0 && Requires.NPowerID[0][0] != -1;


        public int LocationIndex { get; private set; }

        public bool IsModified { get; set; }

        public bool IsNew { get; set; }

        public int PowerIndex { get; set; }

        public int PowerSetID { get; set; }

        public int PowerSetIndex { get; set; }

        public bool HasAbsorbedEffects { get; set; }

        public int StaticIndex { get; set; }

        public int[] NGroupMembership { get; set; }

        public string FullName { get; set; }

        public string GroupName { get; set; }

        public string SetName { get; set; }

        public string PowerName { get; set; }

        public string DisplayName { get; set; }

        public int Available { get; set; }

        public Requirement Requires { get; set; }

        public Enums.eModeFlags ModesRequired { get; set; }

        public Enums.eModeFlags ModesDisallowed { get; set; }

        public Enums.ePowerType PowerType { get; set; }

        public float Accuracy { get; set; }

        public float AccuracyMult { get; set; }

        public Enums.eVector AttackTypes { get; set; }

        public string[] GroupMembership { get; set; }

        public Enums.eEntity EntitiesAffected { get; set; }

        public Enums.eEntity EntitiesAutoHit { get; set; }

        public Enums.eEntity Target { get; set; }

        public bool TargetLoS { get; set; }

        public float Range { get; set; }

        public Enums.eEntity TargetSecondary { get; set; }

        public float RangeSecondary { get; set; }

        public float EndCost { get; set; }

        public float InterruptTime { get; set; }

        public float RechargeTime { get; set; }

        public float BaseRechargeTime { get; set; }

        public float ActivatePeriod { get; set; }

        public Enums.eEffectArea EffectArea { get; set; }

        public float Radius { get; set; }

        public int Arc { get; set; }

        public int MaxTargets { get; set; }

        public string MaxBoosts { get; set; }

        public Enums.eCastFlags CastFlags { get; set; }

        public Enums.eNotify AIReport { get; set; }

        public int NumCharges { get; set; }

        public int UsageTime { get; set; }

        public int LifeTime { get; set; }

        public int LifeTimeInGame { get; set; }

        public int NumAllowed { get; set; }

        public bool DoNotSave { get; set; }

        public string[] BoostsAllowed { get; set; }

        public int[] Enhancements { get; set; }

        public bool CastThroughHold { get; set; }

        public bool IgnoreStrength { get; set; }

        public string DescShort { get; set; }

        public string DescLong { get; set; }

        public bool SortOverride { get; set; }

        public bool HiddenPower { get; set; }

        public Enums.eSetType[] SetTypes { get; set; }

        public bool ClickBuff { get; set; }

        public bool AlwaysToggle { get; set; }

        public int Level { get; set; }

        public bool AllowFrontLoading { get; set; }

        public bool VariableEnabled { get; set; }

        public string VariableName { get; set; }

        public int VariableMin { get; set; }

        public int VariableMax { get; set; }

        public int[] NIDSubPower { get; set; }

        public string[] UIDSubPower { get; set; }

        public bool SubIsAltColor { get; set; }

        public Enums.eEnhance[] IgnoreEnh { get; set; }

        public Enums.eEnhance[] Ignore_Buff { get; set; }

        public bool SkipMax { get; set; }

        public bool MutexAuto { get; set; }

        public bool MutexIgnore { get; set; }

        public bool AbsorbSummonEffects { get; set; }

        public bool AbsorbSummonAttributes { get; set; }

        public bool ShowSummonAnyway { get; set; }

        public bool NeverAutoUpdate { get; set; }

        public bool NeverAutoUpdateRequirements { get; set; }

        public bool IncludeFlag { get; set; }

        public string ForcedClass { get; set; }

        public Enums.eGridType InherentType { get; set; }

        public int ForcedClassID { get; set; }

        public IEffect[] Effects { get; set; }

        public Enums.eBuffMode BuffMode { get; set; }

        public bool HasGrantPowerEffect { get; set; }

        public bool HasPowerOverrideEffect { get; set; }

        public bool BoostBoostable { get; set; }

        public bool BoostUsePlayerLevel { get; set; }

        public Power()
        {
            DescLong = string.Empty;
            DescShort = string.Empty;
            Enhancements = new int[0];
            MaxBoosts = string.Empty;
            DisplayName = string.Empty;
            FullName = string.Empty;
            BoostsAllowed = new string[0];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = new IEffect[0];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = new string[0];
            PowerName = string.Empty;
            SetName = string.Empty;
            GroupName = string.Empty;
            NGroupMembership = new int[0];
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            SetTypes = new Enums.eSetType[0];
            VariableName = string.Empty;
            UIDSubPower = new string[0];
            NIDSubPower = new int[0];
            Ignore_Buff = new Enums.eEnhance[0];
            IgnoreEnh = new Enums.eEnhance[0];
            SubIsAltColor = false;
            BoostsAllowed = new string[0];
            Requires = new Requirement();
            int num = -2;
            for (int index = 0; index <= DatabaseAPI.Database.Power.Length - 1; ++index)
            {
                if (DatabaseAPI.Database.Power[index] != null && DatabaseAPI.Database.Power[index].StaticIndex > -1 && DatabaseAPI.Database.Power[index].StaticIndex > num)
                    num = DatabaseAPI.Database.Power[index].StaticIndex;
            }
            StaticIndex = num + 1;
        }

        public Power(IPower template)
        {
            DescLong = string.Empty;
            DescShort = string.Empty;
            Enhancements = new int[0];
            MaxBoosts = string.Empty;
            DisplayName = string.Empty;
            FullName = string.Empty;
            BoostsAllowed = new string[0];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = new IEffect[0];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = new string[0];
            Requires = new Requirement();
            PowerName = string.Empty;
            SetName = string.Empty;
            GroupName = string.Empty;
            NGroupMembership = new int[0];
            StaticIndex = -1;
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            SetTypes = new Enums.eSetType[0];
            VariableName = string.Empty;
            UIDSubPower = new string[0];
            NIDSubPower = new int[0];
            Ignore_Buff = new Enums.eEnhance[0];
            IgnoreEnh = new Enums.eEnhance[0];
            SubIsAltColor = false;
            if (template == null)
                return;
            IsModified = template.IsModified;
            IsNew = template.IsNew;
            PowerIndex = template.PowerIndex;
            PowerSetID = template.PowerSetID;
            PowerSetIndex = template.PowerSetIndex;
            BuffMode = template.BuffMode;
            HasGrantPowerEffect = template.HasGrantPowerEffect;
            HasPowerOverrideEffect = template.HasPowerOverrideEffect;
            NGroupMembership = new int[template.NGroupMembership.Length];
            Array.Copy(template.NGroupMembership, NGroupMembership, NGroupMembership.Length);
            StaticIndex = template.StaticIndex;
            FullName = template.FullName;
            GroupName = template.GroupName;
            SetName = template.SetName;
            PowerName = template.PowerName;
            DisplayName = template.DisplayName;
            Available = template.Available;
            Requires = new Requirement(template.Requires);
            ModesRequired = template.ModesRequired;
            ModesDisallowed = template.ModesDisallowed;
            PowerType = template.PowerType;
            Accuracy = template.Accuracy;
            AccuracyMult = template.AccuracyMult;
            AttackTypes = template.AttackTypes;
            GroupMembership = new string[template.GroupMembership.Length];
            Array.Copy(template.GroupMembership, GroupMembership, GroupMembership.Length);
            EntitiesAffected = template.EntitiesAffected;
            EntitiesAutoHit = template.EntitiesAutoHit;
            Target = template.Target;
            TargetLoS = template.TargetLoS;
            Range = template.Range;
            TargetSecondary = template.TargetSecondary;
            RangeSecondary = template.RangeSecondary;
            EndCost = template.EndCost;
            InterruptTime = template.InterruptTime;
            CastTime = template.CastTimeReal;
            RechargeTime = template.RechargeTime;
            BaseRechargeTime = template.BaseRechargeTime;
            ActivatePeriod = template.ActivatePeriod;
            EffectArea = template.EffectArea;
            Radius = template.Radius;
            Arc = template.Arc;
            MaxTargets = template.MaxTargets;
            MaxBoosts = template.MaxBoosts;
            CastFlags = template.CastFlags;
            AIReport = template.AIReport;
            NumCharges = template.NumCharges;
            UsageTime = template.UsageTime;
            LifeTime = template.LifeTime;
            LifeTimeInGame = template.LifeTimeInGame;
            NumAllowed = template.NumAllowed;
            DoNotSave = template.DoNotSave;
            BoostsAllowed = new string[template.BoostsAllowed.Length];
            Array.Copy(template.BoostsAllowed, BoostsAllowed, BoostsAllowed.Length);
            Enhancements = new int[template.Enhancements.Length];
            Array.Copy(template.Enhancements, Enhancements, Enhancements.Length);
            CastThroughHold = template.CastThroughHold;
            IgnoreStrength = template.IgnoreStrength;
            DescShort = template.DescShort;
            DescLong = template.DescLong;
            SetTypes = new Enums.eSetType[template.SetTypes.Length];
            Array.Copy(template.SetTypes, SetTypes, SetTypes.Length);
            Effects = new IEffect[template.Effects.Length];
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                Effects[index] = (IEffect)template.Effects[index].Clone();
                Effects[index].SetPower(this);
            }
            ClickBuff = template.ClickBuff;
            AlwaysToggle = template.AlwaysToggle;
            Level = template.Level;
            AllowFrontLoading = template.AllowFrontLoading;
            VariableEnabled = template.VariableEnabled;
            VariableName = template.VariableName;
            VariableMin = template.VariableMin;
            VariableMax = template.VariableMax;
            NIDSubPower = new int[template.NIDSubPower.Length];
            Array.Copy(template.NIDSubPower, NIDSubPower, NIDSubPower.Length);
            UIDSubPower = new string[template.UIDSubPower.Length];
            Array.Copy(template.UIDSubPower, UIDSubPower, UIDSubPower.Length);
            SubIsAltColor = template.SubIsAltColor;
            IgnoreEnh = new Enums.eEnhance[template.IgnoreEnh.Length];
            Array.Copy(template.IgnoreEnh, IgnoreEnh, IgnoreEnh.Length);
            Ignore_Buff = new Enums.eEnhance[template.Ignore_Buff.Length];
            Array.Copy(template.Ignore_Buff, Ignore_Buff, Ignore_Buff.Length);
            SkipMax = template.SkipMax;
            InherentType = template.InherentType;
            LocationIndex = template.DisplayLocation;
            MutexAuto = template.MutexAuto;
            MutexIgnore = template.MutexIgnore;
            AbsorbSummonEffects = template.AbsorbSummonEffects;
            AbsorbSummonAttributes = template.AbsorbSummonAttributes;
            ShowSummonAnyway = template.ShowSummonAnyway;
            NeverAutoUpdate = template.NeverAutoUpdate;
            NeverAutoUpdateRequirements = template.NeverAutoUpdateRequirements;
            IncludeFlag = template.IncludeFlag;
            ForcedClass = template.ForcedClass;
            ForcedClassID = template.ForcedClassID;
            SortOverride = template.SortOverride;
            BoostUsePlayerLevel = template.BoostUsePlayerLevel;
            BoostBoostable = template.BoostBoostable;
            HasAbsorbedEffects = template.HasAbsorbedEffects;
            HiddenPower = template.HiddenPower;
        }

        public Power(BinaryReader reader)
        {
            Enhancements = new int[0];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = new IEffect[0];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = new string[0];
            Requires = new Requirement();
            NGroupMembership = new int[0];
            StaticIndex = -1;
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            SetTypes = new Enums.eSetType[0];
            VariableName = string.Empty;
            UIDSubPower = new string[0];
            NIDSubPower = new int[0];
            Ignore_Buff = new Enums.eEnhance[0];
            IgnoreEnh = new Enums.eEnhance[0];
            SubIsAltColor = false;
            BoostsAllowed = new string[0];
            StaticIndex = reader.ReadInt32();
            FullName = reader.ReadString();
            GroupName = reader.ReadString();
            SetName = reader.ReadString();
            PowerName = reader.ReadString();
            DisplayName = reader.ReadString();
            Available = reader.ReadInt32();
            Requires = new Requirement(reader);
            ModesRequired = (Enums.eModeFlags)reader.ReadInt32();
            ModesDisallowed = (Enums.eModeFlags)reader.ReadInt32();
            PowerType = (Enums.ePowerType)reader.ReadInt32();
            Accuracy = reader.ReadSingle();
            AccuracyMult = Accuracy;
            AttackTypes = (Enums.eVector)reader.ReadInt32();
            GroupMembership = new string[reader.ReadInt32() + 1];
            for (int index = 0; index < GroupMembership.Length; ++index)
                GroupMembership[index] = reader.ReadString();
            EntitiesAffected = (Enums.eEntity)reader.ReadInt32();
            EntitiesAutoHit = (Enums.eEntity)reader.ReadInt32();
            Target = (Enums.eEntity)reader.ReadInt32();
            TargetLoS = reader.ReadBoolean();
            Range = reader.ReadSingle();
            TargetSecondary = (Enums.eEntity)reader.ReadInt32();
            RangeSecondary = reader.ReadSingle();
            EndCost = reader.ReadSingle();
            InterruptTime = reader.ReadSingle();
            CastTime = reader.ReadSingle();
            RechargeTime = reader.ReadSingle();
            BaseRechargeTime = reader.ReadSingle();
            ActivatePeriod = reader.ReadSingle();
            EffectArea = (Enums.eEffectArea)reader.ReadInt32();
            Radius = reader.ReadSingle();
            Arc = reader.ReadInt32();
            MaxTargets = reader.ReadInt32();
            MaxBoosts = reader.ReadString();
            CastFlags = (Enums.eCastFlags)reader.ReadInt32();
            AIReport = (Enums.eNotify)reader.ReadInt32();
            NumCharges = reader.ReadInt32();
            UsageTime = reader.ReadInt32();
            LifeTime = reader.ReadInt32();
            LifeTimeInGame = reader.ReadInt32();
            NumAllowed = reader.ReadInt32();
            DoNotSave = reader.ReadBoolean();
            BoostsAllowed = new string[reader.ReadInt32() + 1];
            for (int index = 0; index <= BoostsAllowed.Length - 1; ++index)
                BoostsAllowed[index] = reader.ReadString();
            CastThroughHold = reader.ReadBoolean();
            IgnoreStrength = reader.ReadBoolean();
            DescShort = reader.ReadString();
            DescLong = reader.ReadString();
            Enhancements = new int[reader.ReadInt32() + 1];
            for (int index = 0; index <= Enhancements.Length - 1; ++index)
                Enhancements[index] = reader.ReadInt32();
            SetTypes = new Enums.eSetType[reader.ReadInt32() + 1];
            for (int index = 0; index <= SetTypes.Length - 1; ++index)
                SetTypes[index] = (Enums.eSetType)reader.ReadInt32();
            ClickBuff = reader.ReadBoolean();
            AlwaysToggle = reader.ReadBoolean();
            Level = reader.ReadInt32();
            AllowFrontLoading = reader.ReadBoolean();
            VariableEnabled = reader.ReadBoolean();
            VariableName = reader.ReadString();
            VariableMin = reader.ReadInt32();
            VariableMax = reader.ReadInt32();
            UIDSubPower = new string[reader.ReadInt32() + 1];
            for (int index = 0; index <= UIDSubPower.Length - 1; ++index)
                UIDSubPower[index] = reader.ReadString();
            IgnoreEnh = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (int index = 0; index <= IgnoreEnh.Length - 1; ++index)
                IgnoreEnh[index] = (Enums.eEnhance)reader.ReadInt32();
            Ignore_Buff = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (int index = 0; index <= Ignore_Buff.Length - 1; ++index)
                Ignore_Buff[index] = (Enums.eEnhance)reader.ReadInt32();
            SkipMax = reader.ReadBoolean();
            InherentType = (Enums.eGridType) reader.ReadInt32();
            DisplayLocation = reader.ReadInt32();
            MutexAuto = reader.ReadBoolean();
            MutexIgnore = reader.ReadBoolean();
            AbsorbSummonEffects = reader.ReadBoolean();
            AbsorbSummonAttributes = reader.ReadBoolean();
            ShowSummonAnyway = reader.ReadBoolean();
            NeverAutoUpdate = reader.ReadBoolean();
            NeverAutoUpdateRequirements = reader.ReadBoolean();
            IncludeFlag = reader.ReadBoolean();
            ForcedClass = reader.ReadString();
            SortOverride = reader.ReadBoolean();
            BoostBoostable = reader.ReadBoolean();
            BoostUsePlayerLevel = reader.ReadBoolean();
            Effects = new IEffect[reader.ReadInt32() + 1];
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                var eff = (IEffect)new Effect(reader)
                {
                    nID = index
                };
                eff.SetPower(this);

                Effects[index] = eff;
            }
            HiddenPower = reader.ReadBoolean();
        }

        public string FullSetName
        {
            get
            {
                string[] strArray = FullName.Split('.');
                return strArray.Length <= 1 ? string.Empty : strArray[0] + "." + strArray[1];
            }
        }

        public float CastTime
        {
            get => !MidsContext.Config.UseArcanaTime ? CastTimeReal : (float)(Math.Ceiling(CastTimeReal / 0.132f) + 1.0) * 0.132f;
            set => CastTimeReal = value;
        }
        public bool Slottable
        {
            get
            {
                var ps = GetPowerSet();
                return Enhancements.Length > 0 && (ps.SetType == Enums.ePowerSetType.Primary || ps.SetType == Enums.ePowerSetType.Secondary || ps.SetType == Enums.ePowerSetType.Ancillary || ps.SetType == Enums.ePowerSetType.Inherent || ps.SetType == Enums.ePowerSetType.Pool);
            }
        }
        public float AoEModifier => EffectArea != Enums.eEffectArea.Cone ? (EffectArea != Enums.eEffectArea.Sphere ? 1f : (float)(1.0 + Radius * 0.150000005960464)) : (float)(1.0 + Radius * 0.150000005960464 - Radius * 0.000366669992217794 * (360 - Arc));

        public int DisplayLocation
        {
            get => LocationIndex;
            set => LocationIndex = value;
        }

        public bool HasMutexID(int index)
        {
            for (int index1 = 0; index1 <= NGroupMembership.Length - 1; ++index1)
            {
                if (NGroupMembership[index1] == index)
                    return true;
            }
            return false;
        }
        public void StoreTo(ref BinaryWriter writer)
        {
            writer.Write(StaticIndex);
            writer.Write(FullName);
            writer.Write(GroupName);
            writer.Write(SetName);
            writer.Write(PowerName);
            writer.Write(DisplayName);
            writer.Write(Available);
            Requires.StoreTo(writer);
            writer.Write((int)ModesRequired);
            writer.Write((int)ModesDisallowed);
            writer.Write((int)PowerType);
            writer.Write(Accuracy);
            writer.Write((int)AttackTypes);
            writer.Write(GroupMembership.Length - 1);
            for (int index = 0; index <= GroupMembership.Length - 1; ++index)
                writer.Write(GroupMembership[index]);
            writer.Write((int)EntitiesAffected);
            writer.Write((int)EntitiesAutoHit);
            writer.Write((int)Target);
            writer.Write(TargetLoS);
            writer.Write(Range);
            writer.Write((int)TargetSecondary);
            writer.Write(RangeSecondary);
            writer.Write(EndCost);
            writer.Write(InterruptTime);
            writer.Write(CastTimeReal);
            writer.Write(RechargeTime);
            writer.Write(BaseRechargeTime);
            writer.Write(ActivatePeriod);
            writer.Write((int)EffectArea);
            writer.Write(Radius);
            writer.Write(Arc);
            writer.Write(MaxTargets);
            writer.Write(MaxBoosts);
            writer.Write((int)CastFlags);
            writer.Write((int)AIReport);
            writer.Write(NumCharges);
            writer.Write(UsageTime);
            writer.Write(LifeTime);
            writer.Write(LifeTimeInGame);
            writer.Write(NumAllowed);
            writer.Write(DoNotSave);
            writer.Write(BoostsAllowed.Length - 1);
            for (int index = 0; index <= BoostsAllowed.Length - 1; ++index)
                writer.Write(BoostsAllowed[index]);
            writer.Write(CastThroughHold);
            writer.Write(IgnoreStrength);
            writer.Write(DescShort);
            writer.Write(DescLong);
            writer.Write(Enhancements.Length - 1);
            for (int index = 0; index <= Enhancements.Length - 1; ++index)
                writer.Write(Enhancements[index]);
            writer.Write(SetTypes.Length - 1);
            for (int index = 0; index <= SetTypes.Length - 1; ++index)
                writer.Write((int)SetTypes[index]);
            writer.Write(ClickBuff);
            writer.Write(AlwaysToggle);
            writer.Write(Level);
            writer.Write(AllowFrontLoading);
            writer.Write(VariableEnabled);
            writer.Write(VariableName);
            writer.Write(VariableMin);
            writer.Write(VariableMax);
            writer.Write(UIDSubPower.Length - 1);
            for (int index = 0; index <= UIDSubPower.Length - 1; ++index)
                writer.Write(UIDSubPower[index]);
            writer.Write(IgnoreEnh.Length - 1);
            for (int index = 0; index <= IgnoreEnh.Length - 1; ++index)
                writer.Write((int)IgnoreEnh[index]);
            writer.Write(Ignore_Buff.Length - 1);
            for (int index = 0; index <= Ignore_Buff.Length - 1; ++index)
                writer.Write((int)Ignore_Buff[index]);
            writer.Write(SkipMax);
            writer.Write((int) InherentType);
            writer.Write(LocationIndex);
            writer.Write(MutexAuto);
            writer.Write(MutexIgnore);
            writer.Write(AbsorbSummonEffects);
            writer.Write(AbsorbSummonAttributes);
            writer.Write(ShowSummonAnyway);
            writer.Write(NeverAutoUpdate);
            writer.Write(NeverAutoUpdateRequirements);
            writer.Write(IncludeFlag);
            writer.Write(ForcedClass);
            writer.Write(SortOverride);
            writer.Write(BoostBoostable);
            writer.Write(BoostUsePlayerLevel);
            writer.Write(Effects.Length - 1);
            for (int index = 0; index <= Effects.Length - 1; ++index)
                Effects[index].StoreTo(ref writer);
            writer.Write(HiddenPower);
        }

        public float FXGetDamageValue()
        {
            float num1 = 0.0f;
            foreach (IEffect effect in Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum &&
                     !(Math.Abs(effect.Probability) > 0.999000012874603)) ||
                    effect.EffectClass == Enums.eEffectClass.Ignored ||
                     (effect.DamageType == Enums.eDamage.Special && effect.ToWho == Enums.eToWho.Self) ||
                    !(effect.Probability > 0.0) || !effect.CanInclude() || !effect.PvXInclude())
                    continue;
                float num2 = effect.Mag;
                if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
                    num2 *= effect.Probability;
                if (PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
                    num2 = (float)(num2 * (double)ActivatePeriod / 10.0);
                if (effect.Ticks > 1)
                {
                    if (effect.CancelOnMiss && MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average && effect.Probability < 1.0)
                        num2 *= (float)((1.0 - Math.Pow(effect.Probability, effect.Ticks)) / (1.0 - effect.Probability));
                    else
                        num2 *= effect.Ticks;
                }
                num1 += num2;
            }
            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.DPS:
                    if (PowerType == Enums.ePowerType.Toggle && ActivatePeriod > 0.0)
                    {
                        num1 /= ActivatePeriod;
                        break;
                    }
                    if (RechargeTime + (double)CastTime + InterruptTime > 0.0)
                    {
                        num1 /= RechargeTime + CastTime + InterruptTime;
                    }
                    break;
                case ConfigData.EDamageReturn.DPA:
                    if (PowerType == Enums.ePowerType.Toggle && ActivatePeriod > 0.0)
                    {
                        num1 /= ActivatePeriod;
                        break;
                    }
                    if (CastTime > 0.0)
                    {
                        num1 /= CastTime;
                    }
                    break;
            }
            return num1;
        }

        public string FXGetDamageString()
        {
            string[] names = Enum.GetNames(Enums.eDamage.None.GetType());
            float[] numArray1 = new float[names.Length];
            float[,] numArray2 = new float[names.Length, 2];
            float[,] numArray3 = new float[names.Length, 2];
            string str1 = string.Empty;
            float iNum = 0.0f;
            foreach (IEffect effect in Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum &&
                     !(Math.Abs(effect.Probability) > 0.999000012874603)) ||
                    effect.EffectClass == Enums.eEffectClass.Ignored ||
                     (effect.DamageType == Enums.eDamage.Special && effect.ToWho == Enums.eToWho.Self) ||
                    !(effect.Probability > 0.0) || !effect.CanInclude() || !effect.PvXInclude())
                    continue;
                float num1 = effect.Mag;
                if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
                    num1 *= effect.Probability;
                if (PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
                    num1 = (float)(num1 * (double)ActivatePeriod / 10.0);
                switch (MidsContext.Config.DamageMath.ReturnValue)
                {
                    case ConfigData.EDamageReturn.DPS:
                        if (PowerType == Enums.ePowerType.Toggle && ActivatePeriod > 0.0)
                        {
                            num1 /= ActivatePeriod;
                            break;
                        }
                        if (RechargeTime + (double)CastTime > 0.0)
                        {
                            num1 /= RechargeTime + CastTime;
                        }
                        break;
                    case ConfigData.EDamageReturn.DPA:
                        if (PowerType == Enums.ePowerType.Toggle && ActivatePeriod > 0.0)
                        {
                            num1 /= ActivatePeriod;
                            break;
                        }
                        if (CastTime > 0.0)
                        {
                            num1 /= CastTime;
                        }
                        break;
                    case ConfigData.EDamageReturn.Numeric:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (effect.Ticks != 0)
                {
                    float num2 = !effect.CancelOnMiss || MidsContext.Config.DamageMath.Calculate != ConfigData.EDamageMath.Average || (double)effect.Probability >= 1.0 ? effect.Ticks : (float)((1.0 - Math.Pow(effect.Probability, effect.Ticks)) / (1.0 - effect.Probability));
                    int index = 0;
                    if (Math.Abs(numArray2[(int)effect.DamageType, 0] - 0.0f) > 0.01)
                        index = 1;
                    numArray2[(int)effect.DamageType, index] = num1;
                    numArray3[(int)effect.DamageType, index] = num2;
                    iNum += num1 * num2;
                }
                else
                {
                    iNum += num1;
                    numArray1[(int)effect.DamageType] += num1;
                }
            }

            if (!(iNum > 0.0))
                return str1;
            {
                for (int index = 0; index <= numArray1.Length - 1; ++index)
                {
                    if (!(numArray1[index] > 0.0 | numArray2[index, 0] > 0.0))
                        continue;
                    if (!string.IsNullOrEmpty(str1))
                        str1 += ", ";
                    string str2 = str1 + Enums.GetDamageName((Enums.eDamage)index) + "(";
                    if (numArray1[index] > 0.0)
                        str2 += Utilities.FixDP(numArray1[index]);
                    if (Math.Abs(numArray2[index, 0] - 0.0f) > 0.01)
                    {
                        if (numArray1[index] > 0.0)
                            str2 += "+";
                        str2 = str2 + Utilities.FixDP(numArray2[index, 0]) + "x" + Utilities.FixDP(numArray3[index, 0]);
                        if (Math.Abs(numArray2[index, 1] - 0.0f) > 0.01)
                            str2 = str2 + "+" + Utilities.FixDP(numArray2[index, 1]) + "x" + Utilities.FixDP(numArray3[index, 1]);
                    }
                    str1 = str2 + ")";
                }
                str1 = str1 + " = " + Utilities.FixDP(iNum);
            }
            return str1;
        }

        public int[] GetRankedEffects()
        {
            int[] numArray1 = new int[Effects.Length];
            for (int index1 = 0; index1 <= Effects.Length - 1; ++index1)
            {
                if ((MidsContext.Config.Suppression & Effects[index1].Suppression) == Enums.eSuppress.None & (!MidsContext.Config.Inc.DisablePvE & Effects[index1].PvMode != Enums.ePvX.PvP | MidsContext.Config.Inc.DisablePvE & Effects[index1].PvMode != Enums.ePvX.PvE))
                {
                    numArray1[index1] = (int)(Effects[index1].EffectClass + 1);
                    if (Math.Abs(Effects[index1].Probability - 1f) < 0.01)
                        numArray1[index1] += 10;
                    if (HasAbsorbedEffects & Effects[index1].EffectType != Enums.eEffectType.EntCreate)
                        numArray1[index1] += 50;
                    if (Effects[index1].DelayedTime > 1.0)
                        numArray1[index1] += -100;
                    if (Effects[index1].DelayedTime > 0.0 & Effects[index1].DelayedTime <= 1.0)
                        numArray1[index1] += -25;
                    if (Effects[index1].InherentSpecial)
                        numArray1[index1] += -100;
                    if (Effects[index1].ToWho == Enums.eToWho.Self & (Effects[index1].Mag > 0.0 | Effects[index1].EffectType == Enums.eEffectType.Mez))
                        numArray1[index1] += 10;
                    if (Effects[index1].ToWho == Enums.eToWho.Target & Effects[index1].Mag < 0.0)
                        numArray1[index1] += 10;
                    if (Effects[index1].ToWho == Enums.eToWho.Target & Effects[index1].Mag > 0.0 & Effects[index1].Absorbed_Effect)
                        numArray1[index1] += 10;
                    if (Effects[index1].isEnhancementEffect)
                        numArray1[index1] += -30;
                    if (Effects[index1].VariableModified)
                        numArray1[index1] += 30;
                    switch (Effects[index1].EffectType)
                    {
                        case Enums.eEffectType.None:
                            numArray1[index1] += -1000;
                            continue;
                        case Enums.eEffectType.Damage:
                            numArray1[index1] += -500;
                            continue;
                        case Enums.eEffectType.DamageBuff:
                            numArray1[index1] += 10;
                            continue;
                        case Enums.eEffectType.Defense:
                            numArray1[index1] += 25;
                            continue;
                        case Enums.eEffectType.Endurance:
                            numArray1[index1] += 15;
                            continue;
                        case Enums.eEffectType.Enhancement:
                            switch (Effects[index1].ETModifies)
                            {
                                case Enums.eEffectType.SpeedFlying:
                                    numArray1[index1] += 5;
                                    continue;
                                case Enums.eEffectType.JumpHeight:
                                    numArray1[index1] -= 5;
                                    continue;
                                case Enums.eEffectType.SpeedJumping:
                                    numArray1[index1] += 5;
                                    continue;
                                case Enums.eEffectType.SpeedRunning:
                                    numArray1[index1] += 5;
                                    continue;
                                default:
                                    numArray1[index1] += 9;
                                    continue;
                            }
                        case Enums.eEffectType.Fly:
                            numArray1[index1] += 3;
                            continue;
                        case Enums.eEffectType.SpeedFlying:
                            numArray1[index1] += 5;
                            continue;
                        case Enums.eEffectType.GrantPower:
                            numArray1[index1] += -20;
                            continue;
                        case Enums.eEffectType.Heal:
                            numArray1[index1] += 15;
                            continue;
                        case Enums.eEffectType.HitPoints:
                            numArray1[index1] += 10;
                            continue;
                        case Enums.eEffectType.JumpHeight:
                            numArray1[index1] += 5;
                            continue;
                        case Enums.eEffectType.SpeedJumping:
                            numArray1[index1] += 5;
                            continue;
                        case Enums.eEffectType.Mez:
                            if (!Effects[index1].Buffable)
                                numArray1[index1] += -1;
                            if (Effects[index1].MezType == Enums.eMez.OnlyAffectsSelf | Effects[index1].MezType == Enums.eMez.Untouchable)
                            {
                                numArray1[index1] -= 9;
                                continue;
                            }
                            if (Effects[index1].MezType == Enums.eMez.Knockback | Effects[index1].MezType == Enums.eMez.Knockup)
                            {
                                numArray1[index1] += Convert.ToInt32(8f * Effects[index1].Probability);
                                continue;
                            }
                            numArray1[index1] += Convert.ToInt32(9f * Effects[index1].Probability);
                            continue;
                        case Enums.eEffectType.MezResist:
                            numArray1[index1] += 5;
                            continue;
                        case Enums.eEffectType.MovementControl:
                            numArray1[index1] += 3;
                            continue;
                        case Enums.eEffectType.MovementFriction:
                            numArray1[index1] += 3;
                            continue;
                        case Enums.eEffectType.Recovery:
                            numArray1[index1] += 10;
                            continue;
                        case Enums.eEffectType.Regeneration:
                            numArray1[index1] += 10;
                            continue;
                        case Enums.eEffectType.Resistance:
                            numArray1[index1] += 20;
                            continue;
                        case Enums.eEffectType.RevokePower:
                            numArray1[index1] += -20;
                            continue;
                        case Enums.eEffectType.SpeedRunning:
                            numArray1[index1] += 5;
                            continue;
                        case Enums.eEffectType.SetMode:
                            numArray1[index1] = -500;
                            continue;
                        case Enums.eEffectType.StealthRadius:
                            numArray1[index1] += 7;
                            continue;
                        case Enums.eEffectType.StealthRadiusPlayer:
                            numArray1[index1] += 6;
                            continue;
                        case Enums.eEffectType.EntCreate:
                            if (HasAbsorbedEffects)
                            {
                                numArray1[index1] += -500;
                                continue;
                            }
                            int[] numArray2 = numArray1;
                            int index2 = index1;
                            numArray2[index2] = numArray2[index2];
                            continue;
                        case Enums.eEffectType.ToHit:
                            numArray1[index1] += 10;
                            continue;
                        case Enums.eEffectType.Translucency:
                            numArray1[index1] += -20;
                            continue;
                        case Enums.eEffectType.GlobalChanceMod:
                            ++numArray1[index1];
                            continue;
                        case Enums.eEffectType.DesignerStatus:
                        case Enums.eEffectType.Null:
                        case Enums.eEffectType.NullBool:
                            numArray1[index1] += int.MinValue;
                            continue;
                        default:
                            numArray1[index1] += 9;
                            continue;
                    }
                }

                numArray1[index1] = int.MinValue;
            }
            int iID1 = -1;
            int num1 = -1;
            int num2 = 0;
            int num3 = 0;
            for (int iID2 = 0; iID2 <= numArray1.Length - 1; ++iID2)
            {
                if (numArray1[iID2] > num2)
                {
                    num1 = iID1;
                    num3 = num2;
                    iID1 = iID2;
                    num2 = numArray1[iID2];
                }
                else if (numArray1[iID2] > num3 & GreOverride(iID1, iID2))
                {
                    num1 = iID2;
                    num3 = numArray1[iID2];
                }
            }
            return new[] { iID1, num1 };
        }

        bool GreOverride(int iID1, int iID2)

        {
            return iID1 < 0 & iID2 > -1 || iID2 >= 0 && (!(Effects[iID1].EffectType == Effects[iID2].EffectType & Effects[iID1].ETModifies == Effects[iID2].ETModifies & Effects[iID1].MezType == Effects[iID2].MezType) || Math.Abs(Effects[iID1].Mag - Effects[iID2].Mag) >= 0.01 && Effects[iID1].ToWho != Effects[iID2].ToWho);
        }

        public int GetDurationEffectID()
        {
            int idx = -1;
            Enums.eEffectClass eEffectClass = Enums.eEffectClass.Ignored;
            float probability = 0.0f;
            float duration = 0.0f;
            bool isDmg = false;
            Enums.eEffectType eEffectType = Enums.eEffectType.None;
            float mag = 0.0f;
            bool buffable = false;
            bool isDefiance = false;
            bool isEntCreate = false;
            int delayTime = int.MaxValue;
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                bool applies = false;
                if (!(!MidsContext.Config.Inc.DisablePvE & Effects[index].PvMode != Enums.ePvX.PvP |
                      MidsContext.Config.Inc.DisablePvE & Effects[index].PvMode != Enums.ePvX.PvE) ||
                    !((Effects[index].Duration > 0.0 | Effects[index].EffectType == Enums.eEffectType.EntCreate) &
                      Effects[index].EffectClass != Enums.eEffectClass.Ignored))
                    continue;
                if (Effects[index].EffectClass < eEffectClass)
                    applies = true;
                if (Effects[index].Probability > (double)probability & Effects[index].SpecialCase != Enums.eSpecialCase.Defiance)
                    applies = true;
                if (Effects[index].Buffable & !buffable)
                    applies = true;
                if (Effects[index].SpecialCase != Enums.eSpecialCase.Defiance & isDefiance)
                    applies = true;
                if (Math.Abs(Effects[index].Probability - probability) < 0.01 & Effects[index].Duration > (double)duration & !Effects[index].isEnhancementEffect & Effects[index].SpecialCase != Enums.eSpecialCase.Defiance && eEffectType != Enums.eEffectType.Mez | Effects[index].EffectType == Enums.eEffectType.Mez)
                {
                    if (eEffectType == Enums.eEffectType.Mez & Effects[index].EffectType == Enums.eEffectType.Mez)
                    {
                        if (Effects[index].Mag > (double)mag || Effects[index].SpecialCase == Enums.eSpecialCase.Domination && MidsContext.Character.Domination)
                            applies = true;
                    }
                    else
                        applies = true;
                }
                if (delayTime > (double)Effects[index].DelayedTime & Effects[index].DelayedTime > 5.0)
                    applies = true;
                if (Math.Abs(Effects[index].Probability - 1f) < 0.01 & isDmg & Effects[index].EffectType == Enums.eEffectType.Mez)
                    applies = true;
                if (Effects[index].EffectType == Enums.eEffectType.Mez && Effects[index].MezType == Enums.eMez.Taunt && Effects[index].EffectClass != Enums.eEffectClass.Primary)
                    applies = false;
                if (((Effects[index].EffectClass > eEffectClass ? 1 : 0) & (Effects[index].SpecialCase != Enums.eSpecialCase.Domination ? 1 : (!MidsContext.Character.Domination ? 1 : 0))) != 0)
                    applies = false;
                if (Effects[index].EffectType == Enums.eEffectType.EntCreate & !isEntCreate & Effects[index].DelayedTime < 20.0 & eEffectType != Enums.eEffectType.Mez)
                    applies = true;
                if (isEntCreate & Effects[index].EffectType != Enums.eEffectType.EntCreate & (Effects[index].Duration < (double)duration | Math.Abs(duration) < 0.01 | Effects[index].DelayedTime > (double)delayTime | Effects[index].Mag < 0.0 & Effects[index].ToWho == Enums.eToWho.Self))
                    applies = false;
                if (isEntCreate & Effects[index].EffectType != Enums.eEffectType.EntCreate & Effects[index].Absorbed_Duration > (double)duration)
                    applies = true;
                if (eEffectType == Enums.eEffectType.Mez & mag < 0.0 & (Effects[index].EffectType == Enums.eEffectType.Resistance | Effects[index].EffectType == Enums.eEffectType.Regeneration) & Effects[index].Mag > 0.0)
                    applies = true;
                if (Effects[index].EffectType == Enums.eEffectType.SetMode)
                    applies = false;
                if (!applies)
                    continue;
                idx = index;
                eEffectClass = Effects[index].EffectClass;
                probability = Effects[index].Probability;
                duration = !(isEntCreate & Effects[index].EffectType != Enums.eEffectType.EntCreate & Effects[index].Absorbed_Duration > (double)duration) ? Effects[index].Duration : Effects[index].Absorbed_Duration;
                isDmg = Effects[index].EffectType == Enums.eEffectType.Damage;
                eEffectType = Effects[index].EffectType;
                mag = Effects[index].Mag;
                buffable = Effects[index].Buffable;
                isDefiance = Effects[index].SpecialCase == Enums.eSpecialCase.Defiance;
                isEntCreate = Effects[index].EffectType == Enums.eEffectType.EntCreate;
                delayTime = (int)Effects[index].DelayedTime;
            }
            return idx;
        }

        public float[] GetDef(int buffDebuff = 0)
        {
            float[] numArray = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            bool flag = false;
            Enums.ePvX ePvX = !MidsContext.Config.Inc.DisablePvE ? Enums.ePvX.PvE : Enums.ePvX.PvP;
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (!(Effects[index].EffectType == Enums.eEffectType.Defense & Effects[index].Probability > 0.0 &
                      Effects[index].CanInclude()) ||
                    !(buffDebuff == 0 | buffDebuff < 0 & Effects[index].Mag < 0.0 | buffDebuff > 0 & Effects[index].Mag > 0.0) ||
                    (Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None ||
                    !(Effects[index].PvMode == ePvX | Effects[index].PvMode == Enums.ePvX.Any))
                    continue;
                numArray[(int)Effects[index].DamageType] += Effects[index].Mag;
                if (Effects[index].DamageType != Enums.eDamage.None)
                    flag = true;
            }

            if (flag)
                return numArray;
            {
                float num = numArray[0];
                for (int index = 0; index <= numArray.Length - 1; ++index)
                    numArray[index] = num;
            }
            return numArray;
        }

        public float[] GetRes(bool pvE = true)
        {
            float[] resists = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            bool hasDamage = false;
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (!(Effects[index].EffectType == Enums.eEffectType.Resistance & Effects[index].Probability > 0.0 &
                      Effects[index].CanInclude()) ||
                    !((Effects[index].PvMode != Enums.ePvX.PvP & pvE) | Effects[index].PvMode != Enums.ePvX.PvE & !pvE))
                    continue;
                resists[(int)Effects[index].DamageType] += Effects[index].Mag;
                if (Effects[index].DamageType != Enums.eDamage.None)
                    hasDamage = true;
            }

            if (hasDamage)
                return resists;
            {
                float num = resists[0];
                for (int index = 0; index <= resists.Length - 1; ++index)
                    resists[index] = num;
            }
            return resists;
        }

        public bool HasDefEffects()
        {
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (Effects[index].EffectType == Enums.eEffectType.Defense & Effects[index].Probability > 0.0 & (Effects[index].Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None & (Effects[index].PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE | Effects[index].PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE))
                    return true;
            }
            return false;
        }

        public bool HasResEffects()
        {
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (Effects[index].EffectType == Enums.eEffectType.Resistance & Effects[index].Probability > 0.0 & (Effects[index].Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None & (Effects[index].PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE | Effects[index].PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE))
                    return true;
            }
            return false;
        }

        public Enums.ShortFX GetEnhancementMagSum(Enums.eEffectType iEffect, int subType = 0)
        {
            Enums.ShortFX shortFx = new Enums.ShortFX();
            for (int iIndex = 0; iIndex <= Effects.Length - 1; ++iIndex)
            {
                if (!Effects[iIndex].PvXInclude() || !(Effects[iIndex].Probability > 0.0) ||
                    Effects[iIndex].ETModifies != iEffect || !Effects[iIndex].CanInclude() ||
                    (Effects[iIndex].EffectType != Enums.eEffectType.Enhancement &&
                     Effects[iIndex].EffectType != Enums.eEffectType.DamageBuff) ||
                    (Effects[iIndex].Absorbed_Effect && Effects[iIndex].Absorbed_PowerType == Enums.ePowerType.GlobalBoost))
                    continue;
                if (iEffect == Enums.eEffectType.Mez && Effects[iIndex].ToWho != Enums.eToWho.Target)
                {
                    if ((Enums.eMez)subType == Effects[iIndex].MezType || subType < 0)
                        shortFx.Add(iIndex, Effects[iIndex].Mag);
                }
                else if (Effects[iIndex].ToWho != Enums.eToWho.Target)
                    shortFx.Add(iIndex, Effects[iIndex].Mag);
            }
            return shortFx;
        }

        public Enums.ShortFX GetEffectMagSum(
          Enums.eEffectType iEffect,
          bool includeDelayed = false,
          bool onlySelf = false,
          bool onlyTarget = false,
          bool maxMode = false)
        {
            Enums.ShortFX shortFx = new Enums.ShortFX();
            for (int iIndex = 0; iIndex <= Effects.Length - 1; ++iIndex)
            {
                bool flag = onlySelf == onlyTarget || !onlySelf && Effects[iIndex].ToWho == Enums.eToWho.Target || !onlyTarget && Effects[iIndex].ToWho == Enums.eToWho.Self || onlySelf & Effects[iIndex].ToWho != Enums.eToWho.Target || onlyTarget & Effects[iIndex].ToWho != Enums.eToWho.Self;
                if (iEffect == Enums.eEffectType.SpeedFlying & !maxMode && Effects[iIndex].Aspect == Enums.eAspect.Max || iEffect == Enums.eEffectType.SpeedRunning & !maxMode & Effects[iIndex].Aspect == Enums.eAspect.Max || iEffect == Enums.eEffectType.SpeedJumping & !maxMode & Effects[iIndex].Aspect == Enums.eAspect.Max)
                    flag = false;
                if ((MidsContext.Config.Suppression & Effects[iIndex].Suppression) != Enums.eSuppress.None)
                    flag = false;
                if (!flag || !(Effects[iIndex].Probability > 0.0) || (maxMode && Effects[iIndex].Aspect != Enums.eAspect.Max) ||
                    Effects[iIndex].EffectType != iEffect || Effects[iIndex].EffectClass == Enums.eEffectClass.Ignored ||
                     Effects[iIndex].EffectClass == Enums.eEffectClass.Special ||
                    (!(Effects[iIndex].DelayedTime <= 5.0) && !includeDelayed) || !Effects[iIndex].CanInclude() ||
                    !Effects[iIndex].PvXInclude())
                    continue;
                float mag = Effects[iIndex].Mag;
                if (Effects[iIndex].Ticks > 1 && Effects[iIndex].Stacking == Enums.eStacking.Yes)
                    mag *= Effects[iIndex].Ticks;
                shortFx.Add(iIndex, mag);
            }
            return shortFx;
        }

        public Enums.ShortFX GetDamageMagSum(
          Enums.eEffectType iEffect,
          Enums.eDamage iSub,
          bool includeDelayed = false)
        {
            Enums.ShortFX shortFx = new Enums.ShortFX();
            for (int iIndex = 0; iIndex <= Effects.Length - 1; ++iIndex)
            {
                if (!Effects[iIndex].CanInclude() ||
                    !(Effects[iIndex].EffectType == iEffect & Effects[iIndex].EffectClass != Enums.eEffectClass.Ignored) ||
                    !Effects[iIndex].PvXInclude() ||
                    !((Effects[iIndex].DelayedTime <= 5.0 | includeDelayed) & Effects[iIndex].DamageType == iSub))
                    continue;
                float mag = Effects[iIndex].Mag;
                if (PowerType == Enums.ePowerType.Toggle & Effects[iIndex].isEnhancementEffect)
                    mag /= 10f;
                shortFx.Add(iIndex, mag);
            }
            return shortFx;
        }

        public Enums.ShortFX GetEffectMag(
          Enums.eEffectType iEffect,
          Enums.eToWho iTarget = Enums.eToWho.Unspecified,
          bool allowDelay = false)
        {
            Enums.ShortFX shortFx = new Enums.ShortFX();
            for (int iIndex = 0; iIndex <= Effects.Length - 1; ++iIndex)
            {
                if (Effects[iIndex].EffectType != iEffect || Effects[iIndex].EffectClass == Enums.eEffectClass.Ignored ||
                    Effects[iIndex].InherentSpecial || !Effects[iIndex].PvXInclude() ||
                    (!(Effects[iIndex].DelayedTime <= 5.0) && !allowDelay) ||
                    (iTarget != Enums.eToWho.Unspecified && Effects[iIndex].ToWho != Enums.eToWho.All && iTarget != Effects[iIndex].ToWho))
                    continue;
                float mag = Effects[iIndex].Mag;
                if (Effects[iIndex].Ticks > 1)
                    mag *= Effects[iIndex].Ticks;
                if (Effects[iIndex].DisplayPercentage && (Effects[iIndex].EffectType == Enums.eEffectType.Heal || Effects[iIndex].EffectType == Enums.eEffectType.HitPoints))
                    shortFx.Add(iIndex, mag / 100f * MidsContext.Archetype.Hitpoints);
                else if (Effects[iIndex].EffectType == Enums.eEffectType.Heal || Effects[iIndex].EffectType == Enums.eEffectType.HitPoints)
                    shortFx.Add(iIndex, (float)(mag / (double)MidsContext.Archetype.Hitpoints * 100.0));
                else
                    shortFx.Add(iIndex, mag);
                return shortFx;
            }
            return shortFx;
        }

        public bool AffectsTarget(Enums.eEffectType iEffect)
        {
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (Effects[index].EffectType == iEffect && Effects[index].ToWho == Enums.eToWho.Target)
                    return true;
            }
            return false;
        }

        public bool AffectsSelf(Enums.eEffectType iEffect)
        {
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (Effects[index].EffectType == iEffect && Effects[index].ToWho == Enums.eToWho.Self)
                    return true;
            }
            return false;
        }

        public bool I9FXPresentP(Enums.eEffectType iEffect, Enums.eMez iMez = Enums.eMez.None)
        {
            for (int index = 0; index <= Effects.Length - 1; ++index)
            {
                if (!(Effects[index].EffectType == iEffect & Effects[index].Mag > 0.0) ||
                    Effects[index].EffectType == Enums.eEffectType.Damage &
                    Effects[index].DamageType == Enums.eDamage.Special) continue;
                if (iMez == Enums.eMez.None || Effects[index].MezType == iMez)
                    return true;
            }
            return false;
        }

        public static Enums.ShortFX[] SplitFX(ref Enums.ShortFX iSfx, ref IPower iPower)
        {
            Enums.ShortFX[] shortFxArray1 = new Enums.ShortFX[0];
            Enums.ShortFX[] shortFxArray2;
            if (!iSfx.Present)
            {
                shortFxArray2 = shortFxArray1;
            }
            else
            {
                Enums.ShortFX[] array = new Enums.ShortFX[1];
                array[0].Add(iSfx.Index[0], iSfx.Value[0]);
                for (int index1 = 1; index1 <= iSfx.Value.Length - 1; ++index1)
                {
                    int index2 = -1;
                    for (int index3 = 0; index3 <= array.Length - 1; ++index3)
                    {
                        if (!(Math.Abs(iSfx.Value[index1] - array[index3].Value[0]) < 0.01) ||
                            !(iPower.Effects[iSfx.Index[index1]].PvMode == iPower.Effects[array[index3].Index[0]].PvMode &
                              iPower.Effects[iSfx.Index[index1]].ToWho == iPower.Effects[array[index3].Index[0]].ToWho &
                              iPower.Effects[iSfx.Index[index1]].Stacking == iPower.Effects[array[index3].Index[0]].Stacking &
                              iPower.Effects[iSfx.Index[index1]].Aspect == iPower.Effects[array[index3].Index[0]].Aspect &
                              iPower.Effects[iSfx.Index[index1]].Buffable == iPower.Effects[array[index3].Index[0]].Buffable &
                              iPower.Effects[iSfx.Index[index1]].Resistible == iPower.Effects[array[index3].Index[0]].Resistible))
                            continue;
                        index2 = index3;
                        break;
                    }
                    if (index2 < 0)
                    {
                        Array.Resize(ref array, array.Length + 1);
                        index2 = array.Length - 1;
                    }
                    array[index2].Add(iSfx.Index[index1], iSfx.Value[index1]);
                }
                shortFxArray2 = array;
            }
            return shortFxArray2;
        }

        public static string SplitFXGroupTip(ref Enums.ShortFX iSfx, ref IPower iPower, bool shortForm)
        {
            string str = iPower.Effects[iSfx.Index[0]].BuildEffectString(false, string.Empty, false, true);
            string newValue = string.Empty;
            if (!iPower.Effects[iSfx.Index[0]].isDamage())
                return str.Replace("%VALUE%", newValue);
            bool[] iDamage = new bool[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            for (int index = 0; index <= iSfx.Index.Length - 1; ++index)
                iDamage[(int)iPower.Effects[iSfx.Index[index]].DamageType] = true;
            newValue = !(iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Defense | iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Elusivity) ? Enums.GetGroupedDamage(iDamage, shortForm) : Enums.GetGroupedDefense(iDamage, shortForm);
            return str.Replace("%VALUE%", newValue);
        }

        public bool UpdateFromCSV(string iCSV)
        {
            bool flag;
            if (string.IsNullOrEmpty(iCSV))
                flag = false;
            else if (NeverAutoUpdate)
            {
                flag = true;
            }
            else
            {
                string[] array = CSV.ToArray(iCSV);
                try
                {
                    if (array[0].Split(".".ToCharArray())[2].StartsWith("Hide"))
                        array = CSV.ToArray(iCSV.Replace("'Hidden" + char.ConvertFromUtf32(34) + char.ConvertFromUtf32(34), "'Hidden'"));
                }
                catch (Exception)
                {
                    Console.WriteLine("Power Import: Fixed stupid Hide bug.");
                }
                if (array.Length < 93)
                {
                    flag = false;
                }
                else
                {
                    FullName = array[0];
                    string[] strArray = FullName.Split('.');
                    if (strArray.Length > 2)
                    {
                        if (!string.IsNullOrEmpty(strArray[1]))
                            PowerName = strArray[2];
                        if (!string.IsNullOrEmpty(strArray[1]))
                            SetName = strArray[1];
                        if (!string.IsNullOrEmpty(strArray[0]))
                            GroupName = strArray[0];
                    }

                    string lower = GroupName.ToLower();
                    if (string.IsNullOrEmpty(ForcedClass))
                    {
                        if (lower == "pets")
                        {
                            ForcedClass = "Class_Minion_Pets";
                        }
                        else if (lower == "mastermind_pets")
                        {
                            ForcedClass = "Class_Minion_Henchman";
                        }
                    }
                    DisplayName = array[1];
                    Available = int.Parse(array[2]);
                    string iReq = array[3];
                    if (!NeverAutoUpdateRequirements)
                        Requires = ImportRequirementString(iReq);
                    ModesRequired = (Enums.eModeFlags)Enums.StringToFlaggedEnum(array[4], ModesRequired);
                    ModesDisallowed = (Enums.eModeFlags)Enums.StringToFlaggedEnum(array[5], ModesDisallowed);
                    string str = array[6];
                    try
                    {
                        PowerType = (Enums.ePowerType)Enum.Parse(PowerType.GetType(), str.Replace("Auto", "Auto_").Replace("Global Boost", "GlobalBoost"), true);
                    }
                    catch
                    {
                        PowerType = Enums.ePowerType.Auto_;
                    }
                    Accuracy = float.Parse(array[7]);
                    AttackTypes = (Enums.eVector)Enums.StringToFlaggedEnum(array[8], AttackTypes);
                    GroupMembership = Enums.StringToArray(array[9]);
                    EntitiesAffected = (Enums.eEntity)Enums.StringToFlaggedEnum(array[11], EntitiesAffected);
                    EntitiesAutoHit = (Enums.eEntity)Enums.StringToFlaggedEnum(array[12], EntitiesAutoHit);
                    Target = (Enums.eEntity)Enums.StringToFlaggedEnum(array[13], Target);
                    TargetLoS = !string.Equals(array[14], "NONE", StringComparison.OrdinalIgnoreCase);
                    Range = float.Parse(array[17]);
                    TargetSecondary = (Enums.eEntity)Enums.StringToFlaggedEnum(array[18], TargetSecondary, true);
                    RangeSecondary = float.Parse(array[19]);
                    EndCost = float.Parse(array[20]);
                    InterruptTime = float.Parse(array[22]);
                    CastTimeReal = float.Parse(array[23]);
                    RechargeTime = float.Parse(array[24]);
                    ActivatePeriod = float.Parse(array[25]);
                    EffectArea = (Enums.eEffectArea)Enums.StringToFlaggedEnum(array[26], EffectArea, true);
                    Radius = float.Parse(array[27]);
                    Arc = int.Parse(array[28]);
                    MaxTargets = int.Parse(array[29]);
                    MaxBoosts = array[30];
                    CastFlags = (Enums.eCastFlags)Enums.StringToFlaggedEnum(array[31], CastFlags);
                    AIReport = (Enums.eNotify)Enums.StringToFlaggedEnum(array[32], AIReport, true);
                    NumCharges = int.Parse(array[33]);
                    UsageTime = int.Parse(array[34]);
                    LifeTime = int.Parse(array[35]);
                    LifeTimeInGame = int.Parse(array[36]);
                    NumAllowed = int.Parse(array[37]);
                    DoNotSave = int.Parse(array[38]) > 0;
                    BoostsAllowed = Enums.StringToArray(array[39]);
                    CastThroughHold = int.Parse(array[41]) > 0;
                    IgnoreStrength = int.Parse(array[45]) > 0;
                    DescShort = array[46];
                    DescLong = array[47];
                    BoostBoostable = int.Parse(array[69]) > 0;
                    BoostUsePlayerLevel = int.Parse(array[70]) > 0;
                    IgnoreEnh = Enums.StringToEnumArray<Enums.eEnhance>(array[76], typeof(Enums.eEnhance));
                    flag = true;
                }
            }
            return flag;
        }

        Requirement ImportRequirementString(string iReq)

        {
            Requirement requirement1;
            if (NeverAutoUpdateRequirements)
            {
                requirement1 = Requires;
            }
            else
            {
                Requirement requirement2 = new Requirement();
                if (iReq == null)
                    requirement1 = requirement2;
                else if (iReq.Length == 0)
                {
                    requirement1 = requirement2;
                }
                else
                {
                    requirement2.ClassNameNot = new string[0];
                    requirement2.ClassName = new string[0];
                    requirement2.PowerID = new string[0][];
                    requirement2.PowerIDNot = new string[0][];
                    iReq = iReq.ToUpper();
                    for (int index1 = 0; index1 <= 1; ++index1)
                    {
                        string str = "$ARCHETYPE @";
                        if (index1 == 1)
                            str = "$ARCHTYPE @";
                        Contains = iReq.Contains(str);
                        for (int index2 = 0; index2 <= DatabaseAPI.Database.Classes.Length - 1; ++index2)
                        {
                            string oldValue1 = str + DatabaseAPI.Database.Classes[index2].ClassName.ToUpper() + " ==";
                            string oldValue2 = oldValue1 + " !";
                            if (iReq.Contains(oldValue2))
                            {
                                Array.Resize(ref requirement2.ClassNameNot, requirement2.ClassNameNot.Length + 1);
                                requirement2.ClassNameNot[requirement2.ClassNameNot.Length - 1] = DatabaseAPI.Database.Classes[index2].ClassName;
                                iReq = iReq.Replace(oldValue2, "true");
                            }
                            else if (iReq.Contains(oldValue1))
                            {
                                Array.Resize(ref requirement2.ClassName, requirement2.ClassName.Length + 1);
                                requirement2.ClassName[requirement2.ClassName.Length - 1] = DatabaseAPI.Database.Classes[index2].ClassName;
                                iReq = iReq.Replace(oldValue1, "true");
                            }
                        }

                        if (!Contains)
                            continue;
                        {
                            int startIndex = iReq.IndexOf(str, StringComparison.Ordinal);
                            for (int index2 = startIndex + str.Length; index2 <= iReq.Length - 1; ++index2)
                            {
                                if (iReq[index2] != ' ')
                                    continue;
                                iReq = iReq.Replace(iReq.Substring(startIndex, index2 - startIndex), "true");
                                break;
                            }
                            iReq = iReq.Replace("true ==", "true");
                            iReq = iReq.Replace("true !", "true");
                        }
                    }
                    string[] strArray1 = new string[33];
                    int index3 = 0;
                    string[] strArray2 = iReq.Split(null);
                    for (int index1 = 0; index1 <= strArray2.Length - 1; ++index1)
                    {
                        strArray2[index1] = strArray2[index1].ToLower();
                        switch (strArray2[index1])
                        {
                            case "&&" when index3 > 1 && strArray1[index3 - 1] == "true" & strArray1[index3 - 2] == "true":
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case "&&" when index3 > 1 && strArray1[index3 - 1] == "true" & strArray1[index3 - 2] != "true":
                                requirement2.AddPowers(strArray1[index3 - 2], string.Empty);
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case "&&" when index3 > 1 && strArray1[index3 - 1] != "true" & strArray1[index3 - 2] == "true":
                                requirement2.AddPowers(strArray1[index3 - 1], string.Empty);
                                --index3;
                                strArray1[index3] = string.Empty;
                                break;
                            case "&&":
                            {
                                if (index3 > 1 && strArray1[index3 - 1] != "true" & strArray1[index3 - 2] != "true")
                                {
                                    requirement2.AddPowers(strArray1[index3 - 2], strArray1[index3 - 1]);
                                    --index3;
                                    strArray1[index3] = string.Empty;
                                    strArray1[index3 - 1] = "true";
                                }

                                break;
                            }
                            case "!":
                                strArray1[index3 - 1] = "!" + strArray1[index3 - 1];
                                break;
                            case "||":
                            {
                                if (index3 > 1)
                                {
                                    if (strArray1[index3 - 1] == "true" & strArray1[index3 - 2] == "true")
                                    {
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                        strArray1[index3 - 1] = "true";
                                    }
                                    else if (strArray1[index3 - 1] != "true" & strArray1[index3 - 2] == "true")
                                    {
                                        requirement2.AddPowers(strArray1[index3 - 1], string.Empty);
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                    }
                                    else if (strArray1[index3 - 1] == "true" & strArray1[index3 - 2] != "true")
                                    {
                                        requirement2.AddPowers(strArray1[index3 - 2], string.Empty);
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                        strArray1[index3 - 1] = "true";
                                    }
                                    else
                                    {
                                        requirement2.AddPowers(strArray1[index3 - 2], string.Empty);
                                        requirement2.AddPowers(strArray1[index3 - 1], string.Empty);
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                        strArray1[index3 - 1] = "true";
                                    }
                                }

                                break;
                            }
                            case "owned?":
                            case "auth>":
                            case "productowned?":
                            case "tokenowned?":
                            case "char>":
                                strArray1[index3 - 1] = "true";
                                break;
                            case ">=":
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case ">":
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case "source>":
                                strArray1[index3 - 1] = "true";
                                break;
                            default:
                            {
                                if (strArray2[index1] != "eq")
                                {
                                    switch (strArray2[index1])
                                    {
                                        case "ispvpmap?":
                                        {
                                            if (index1 < strArray2.GetUpperBound(0) && strArray2[index1 + 1] == "!")
                                                strArray2[index1 + 1] = string.Empty;
                                            strArray1[index3] = "true";
                                            ++index3;
                                            break;
                                        }
                                        case "isarchitectmap?":
                                        {
                                            if (index1 < strArray2.GetUpperBound(0) && strArray2[index1 + 1] == "!")
                                                strArray2[index1 + 1] = string.Empty;
                                            strArray1[index3] = "true";
                                            ++index3;
                                            break;
                                        }
                                        default:
                                        {
                                            if (!string.IsNullOrEmpty(strArray2[index1]))
                                            {
                                                strArray1[index3] = strArray2[index1];
                                                ++index3;
                                            }

                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                    if (index3 == 1 && strArray1[0] != "true")
                    {
                        requirement2.AddPowers(strArray1[0], string.Empty);
                        strArray1[0] = "true";
                    }
                    if (index3 != 0 && index3 > 1 | strArray1[0] != "true")
                    {
                        string str = "Tokens remain in the stack (this can cause problems): \n";
                        for (int index1 = 0; index1 <= index3; ++index1)
                            str = str + strArray1[index1] + " ";
                        int num = (int)MessageBox.Show(str + "\n\niReq: " + iReq + "\n\nSee clsPowerV2/ImportRequirementString to tweak.");
                    }
                    for (int index1 = 0; index1 <= requirement2.PowerID.Length - 1; ++index1)
                    {
                        for (int index2 = 0; index2 <= requirement2.PowerID[index1].Length - 1; ++index2)
                        {
                            if (string.IsNullOrEmpty(requirement2.PowerID[index1][index2]))
                                continue;
                            for (int index4 = 0; index4 <= DatabaseAPI.Database.Power.Length - 1; ++index4)
                            {
                                if (!string.Equals(DatabaseAPI.Database.Power[index4].FullName, requirement2.PowerID[index1][index2],
                                    StringComparison.OrdinalIgnoreCase))
                                    continue;
                                requirement2.PowerID[index1][index2] = DatabaseAPI.Database.Power[index4].FullName;
                                break;
                            }
                        }
                    }
                    requirement1 = requirement2;
                }
            }
            return requirement1;
        }

        public bool IgnoreEnhancement(Enums.eEnhance iEffect)
        {
            if (IgnoreEnh.Length == 0) return true;
            for (int index = 0; index <= IgnoreEnh.Length - 1; ++index)
            {
                if (IgnoreEnh[index] == iEffect)
                    return false;
            }
            return true;
        }

        public bool IgnoreBuff(Enums.eEnhance iEffect)
        {
            if (Ignore_Buff.Length == 0) return true;
            for (int index = 0; index <= Ignore_Buff.Length - 1; ++index)
            {
                if (Ignore_Buff[index] == iEffect)
                    return false;
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Power power))
                throw new ArgumentException("Comparison failed - Passed object was not an Archetype Class!");
            int num = string.Compare(FullSetName, power.FullSetName, StringComparison.OrdinalIgnoreCase);
            return num == 0 ? (Level <= power.Level ? (Level >= power.Level ? (Level != power.Level || !SortOverride || power.SortOverride ? (Level != power.Level || SortOverride || !power.SortOverride ? string.Compare(FullName, power.FullName, StringComparison.OrdinalIgnoreCase) : 1) : -1) : -1) : 1) : num;
        }

        public void SetMathMag()
        {
            foreach (var index in Effects)
            {
                index.Math_Duration = index.Duration;
                index.Math_Mag = index.Mag;
            }
        }

        public bool GetEffectStringGrouped(
          int idEffect,
          ref string returnString,
          ref int[] returnMask,
          bool shortForm,
          bool simple,
          bool noMag = false)
        {
            bool flag;
            if (idEffect < 0 | idEffect > Effects.Length - 1)
            {
                flag = false;
            }
            else
            {
                string str = string.Empty;
                int[] array = new int[0];
                IEffect effect = (IEffect)Effects[idEffect].Clone();
                if (effect.EffectType == Enums.eEffectType.DamageBuff || effect.EffectType == Enums.eEffectType.Defense || effect.EffectType == Enums.eEffectType.Resistance || effect.EffectType == Enums.eEffectType.Elusivity)
                {
                    bool[] iDamage = new bool[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                    for (int index1 = 0; index1 <= Effects.Length - 1; ++index1)
                    {
                        for (int index2 = 0; index2 <= iDamage.Length - 1; ++index2)
                        {
                            effect.DamageType = (Enums.eDamage)index2;
                            if (effect.CompareTo(Effects[index1]) != 0)
                                continue;
                            iDamage[index2] = true;
                            Array.Resize(ref array, array.Length + 1);
                            array[array.Length - 1] = index1;
                        }
                    }
                    if (array.Length <= 1)
                        return false;
                    effect.DamageType = Enums.eDamage.Special;
                    string newValue = effect.EffectType == Enums.eEffectType.Defense ? Enums.GetGroupedDefense(iDamage, shortForm) : Enums.GetGroupedDamage(iDamage, shortForm);
                    str = shortForm ? effect.BuildEffectStringShort(noMag, simple).Replace("Spec", newValue) : effect.BuildEffectString(simple).Replace("Special", newValue);
                }
                else if (effect.EffectType == Enums.eEffectType.Mez | effect.EffectType == Enums.eEffectType.MezResist)
                {
                    bool[] iMez = new bool[Enum.GetValues(Enums.eMez.None.GetType()).Length];
                    for (int index1 = 0; index1 <= Effects.Length - 1; ++index1)
                    {
                        for (int index2 = 0; index2 <= iMez.Length - 1; ++index2)
                        {
                            effect.MezType = (Enums.eMez)index2;
                            if (effect.CompareTo(Effects[index1]) != 0)
                                continue;
                            iMez[index2] = true;
                            Array.Resize(ref array, array.Length + 1);
                            array[array.Length - 1] = index1;
                        }
                    }
                    if (array.Length <= 1)
                        return false;
                    effect.MezType = Enums.eMez.None;
                    string newValue = Enums.GetGroupedMez(iMez, shortForm);
                    if (newValue == "Knocked" && effect.Mag < 0.0)
                        newValue = "Knockback Protection";
                    str = shortForm ? effect.BuildEffectStringShort(noMag, simple).Replace("None", newValue) : effect.BuildEffectString(simple).Replace("None", newValue);
                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.MezResist:
                        {
                            if (newValue == "Mez")
                                str = str.Replace("MezResist(Mez)", "Status Resistance");
                            break;
                        }
                        case Enums.eEffectType.Mez when newValue == "Mez" & effect.Mag < 0.0:
                            str = str.Replace("Mez", "Status Protection").Replace("-", string.Empty);
                            break;
                        case Enums.eEffectType.Mez:
                        {
                            if (newValue != "Knockback Protection")
                                str = str.Replace("(Mag -", "protection (Mag ");
                            break;
                        }
                        case Enums.eEffectType.None:
                            break;
                        case Enums.eEffectType.Accuracy:
                            break;
                        case Enums.eEffectType.ViewAttrib:
                            break;
                        case Enums.eEffectType.Damage:
                            break;
                        case Enums.eEffectType.DamageBuff:
                            break;
                        case Enums.eEffectType.Defense:
                            break;
                        case Enums.eEffectType.DropToggles:
                            break;
                        case Enums.eEffectType.Endurance:
                            break;
                        case Enums.eEffectType.EnduranceDiscount:
                            break;
                        case Enums.eEffectType.Enhancement:
                            break;
                        case Enums.eEffectType.Fly:
                            break;
                        case Enums.eEffectType.SpeedFlying:
                            break;
                        case Enums.eEffectType.GrantPower:
                            break;
                        case Enums.eEffectType.Heal:
                            break;
                        case Enums.eEffectType.HitPoints:
                            break;
                        case Enums.eEffectType.InterruptTime:
                            break;
                        case Enums.eEffectType.JumpHeight:
                            break;
                        case Enums.eEffectType.SpeedJumping:
                            break;
                        case Enums.eEffectType.Meter:
                            break;
                        case Enums.eEffectType.MovementControl:
                            break;
                        case Enums.eEffectType.MovementFriction:
                            break;
                        case Enums.eEffectType.PerceptionRadius:
                            break;
                        case Enums.eEffectType.Range:
                            break;
                        case Enums.eEffectType.RechargeTime:
                            break;
                        case Enums.eEffectType.Recovery:
                            break;
                        case Enums.eEffectType.Regeneration:
                            break;
                        case Enums.eEffectType.ResEffect:
                            break;
                        case Enums.eEffectType.Resistance:
                            break;
                        case Enums.eEffectType.RevokePower:
                            break;
                        case Enums.eEffectType.Reward:
                            break;
                        case Enums.eEffectType.SpeedRunning:
                            break;
                        case Enums.eEffectType.SetCostume:
                            break;
                        case Enums.eEffectType.SetMode:
                            break;
                        case Enums.eEffectType.Slow:
                            break;
                        case Enums.eEffectType.StealthRadius:
                            break;
                        case Enums.eEffectType.StealthRadiusPlayer:
                            break;
                        case Enums.eEffectType.EntCreate:
                            break;
                        case Enums.eEffectType.ThreatLevel:
                            break;
                        case Enums.eEffectType.ToHit:
                            break;
                        case Enums.eEffectType.Translucency:
                            break;
                        case Enums.eEffectType.XPDebtProtection:
                            break;
                        case Enums.eEffectType.SilentKill:
                            break;
                        case Enums.eEffectType.Elusivity:
                            break;
                        case Enums.eEffectType.GlobalChanceMod:
                            break;
                        case Enums.eEffectType.CombatModShift:
                            break;
                        case Enums.eEffectType.UnsetMode:
                            break;
                        case Enums.eEffectType.Rage:
                            break;
                        case Enums.eEffectType.MaxRunSpeed:
                            break;
                        case Enums.eEffectType.MaxJumpSpeed:
                            break;
                        case Enums.eEffectType.MaxFlySpeed:
                            break;
                        case Enums.eEffectType.DesignerStatus:
                            break;
                        case Enums.eEffectType.PowerRedirect:
                            break;
                        case Enums.eEffectType.TokenAdd:
                            break;
                        case Enums.eEffectType.ExperienceGain:
                            break;
                        case Enums.eEffectType.InfluenceGain:
                            break;
                        case Enums.eEffectType.PrestigeGain:
                            break;
                        case Enums.eEffectType.AddBehavior:
                            break;
                        case Enums.eEffectType.RechargePower:
                            break;
                        case Enums.eEffectType.RewardSourceTeam:
                            break;
                        case Enums.eEffectType.VisionPhase:
                            break;
                        case Enums.eEffectType.CombatPhase:
                            break;
                        case Enums.eEffectType.ClearFog:
                            break;
                        case Enums.eEffectType.SetSZEValue:
                            break;
                        case Enums.eEffectType.ExclusiveVisionPhase:
                            break;
                        case Enums.eEffectType.Absorb:
                            break;
                        case Enums.eEffectType.XAfraid:
                            break;
                        case Enums.eEffectType.XAvoid:
                            break;
                        case Enums.eEffectType.BeastRun:
                            break;
                        case Enums.eEffectType.ClearDamagers:
                            break;
                        case Enums.eEffectType.EntCreate_x:
                            break;
                        case Enums.eEffectType.Glide:
                            break;
                        case Enums.eEffectType.Hoverboard:
                            break;
                        case Enums.eEffectType.Jumppack:
                            break;
                        case Enums.eEffectType.MagicCarpet:
                            break;
                        case Enums.eEffectType.NinjaRun:
                            break;
                        case Enums.eEffectType.Null:
                            break;
                        case Enums.eEffectType.NullBool:
                            break;
                        case Enums.eEffectType.Stealth:
                            break;
                        case Enums.eEffectType.SteamJump:
                            break;
                        case Enums.eEffectType.Walk:
                            break;
                        case Enums.eEffectType.XPDebt:
                            break;
                        case Enums.eEffectType.ForceMove:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (effect.EffectType == Enums.eEffectType.Enhancement)
                {
                    int num = 0;
                    if (Effects.Length == 4)
                    {
                        for (int index = 0; index <= Effects.Length - 1; ++index)
                        {
                            if (Effects[index].EffectType == Enums.eEffectType.Enhancement && Effects[index].ETModifies == Enums.eEffectType.SpeedRunning | Effects[index].ETModifies == Enums.eEffectType.SpeedFlying | Effects[index].ETModifies == Enums.eEffectType.SpeedJumping | Effects[index].ETModifies == Enums.eEffectType.JumpHeight)
                                ++num;
                        }
                        if (num == Effects.Length)
                        {
                            array = new int[Effects.Length];
                            for (int index = 0; index <= array.Length - 1; ++index)
                                array[index] = index;
                            effect.ETModifies = Enums.eEffectType.Slow;
                            str = shortForm ? effect.BuildEffectStringShort(noMag, simple) : effect.BuildEffectString(simple);
                            if (BuffMode != Enums.eBuffMode.Debuff)
                                str = str.Replace("Slow", "Movement");
                        }
                    }
                }
                returnMask = new int[array.Length];
                Array.Copy(array, returnMask, array.Length);
                returnString = str;
                flag = true;
            }
            return flag;
        }

        public int[] AbsorbEffects(
          IPower source,
          float nDuration,
          float nDelay,
          Archetype archetype,
          int stacking,
          bool isGrantPower = false,
          int fxid = -1,
          int effectId = -1)
        {
            int num1 = -1;
            int length = Effects.Length;
            int[] array = new int[0];
            float num2 = 0.0f;
            if (source.PowerSetID > -1 && DatabaseAPI.Database.Powersets[source.PowerSetID].SetType == Enums.ePowerSetType.Pet)
            {
                foreach (IPower power in DatabaseAPI.Database.Powersets[source.PowerSetID].Powers)
                {
                    foreach (IEffect effect in power.Effects)
                    {
                        if (effect.EffectType == Enums.eEffectType.SilentKill & effect.ToWho == Enums.eToWho.Self & effect.DelayedTime > 0.0)
                            num2 = effect.DelayedTime;
                    }
                }
            }
            if ((((double)num2 > 0.0 ? 1 : 0) & ((double)nDuration < 0.01 ? 1 : ((double)nDuration > (double)num2 ? 1 : 0))) != 0)
                nDuration = num2;
            if (effectId == -1)
            {
                for (int index = 0; index <= source.Effects.Length - 1; ++index)
                {
                    if (!isGrantPower & source.EntitiesAffected == Enums.eEntity.Caster &
                                         source.Effects[index].EffectType != Enums.eEffectType.EntCreate)
                        continue;
                    if (source.Effects[index].EffectType == Enums.eEffectType.EntCreate && source.Effects[index].nSummon > -1)
                    {
                        Array.Resize(ref array, array.Length + 1);
                        array[array.Length - 1] = index;
                    }
                    ++num1;
                    IEffect[] effects = Effects;
                    Array.Resize(ref effects, num1 + length + 1);
                    Effects = effects;
                    IEffect effect = (IEffect)source.Effects[index].Clone();
                    effect.Absorbed_Effect = true;
                    effect.Absorbed_PowerType = source.PowerType;
                    effect.Absorbed_Class_nID = archetype.Idx;
                    effect.Absorbed_EffectID = fxid;
                    effect.Absorbed_Power_nID = source.PowerIndex;
                    if (source.PowerType == Enums.ePowerType.Auto_ || source.PowerType == Enums.ePowerType.Toggle)
                        effect.SetTicks(nDuration, source.ActivatePeriod);
                    if ((source.EntitiesAutoHit & Enums.eEntity.Friend) == Enums.eEntity.Friend)
                    {
                        effect.ToWho = Enums.eToWho.Self;
                        if (effect.Stacking == Enums.eStacking.Yes)
                            effect.Scale *= stacking;
                    }
                    if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        if (effect.Stacking == Enums.eStacking.Yes)
                            effect.Scale *= stacking;
                    }
                    effect.Absorbed_Duration = nDuration;
                    if (source.RechargeTime > 0.0 & source.PowerType == Enums.ePowerType.Click)
                        effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                    if (nDelay > 0.0)
                        effect.DelayedTime = nDelay;
                    if (effect.Absorbed_Duration > 0.0 & num2 > 0.0)
                        effect.nDuration = effect.Absorbed_Duration;
                    Effects[num1 + length] = effect;
                }
            }
            else if (isGrantPower || source.EntitiesAffected != Enums.eEntity.Caster || source.Effects[effectId].EffectType == Enums.eEffectType.EntCreate)
            {
                if (source.Effects[effectId].EffectType == Enums.eEffectType.EntCreate && source.Effects[effectId].nSummon > -1)
                {
                    Array.Resize(ref array, array.Length + 1);
                    array[array.Length - 1] = effectId;
                }
                int num3 = num1 + 1;
                IEffect[] effects = Effects;
                Array.Resize(ref effects, num3 + length + 1);
                Effects = effects;
                IEffect effect = (IEffect)source.Effects[effectId].Clone();
                effect.Absorbed_Effect = true;
                effect.Absorbed_PowerType = source.PowerType;
                effect.Absorbed_Class_nID = archetype.Idx;
                effect.Absorbed_EffectID = fxid;
                effect.Absorbed_Power_nID = source.PowerIndex;
                if (source.PowerType == Enums.ePowerType.Auto_ || source.PowerType == Enums.ePowerType.Toggle)
                    effect.SetTicks(nDuration, source.ActivatePeriod);
                if ((source.EntitiesAutoHit & Enums.eEntity.Friend) == Enums.eEntity.Friend)
                {
                    effect.ToWho = Enums.eToWho.Self;
                    if (effect.Stacking == Enums.eStacking.Yes)
                        effect.Scale *= stacking;
                }
                if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
                {
                    effect.ToWho = Enums.eToWho.Target;
                    if (effect.Stacking == Enums.eStacking.Yes)
                        effect.Scale *= stacking;
                }
                effect.Absorbed_Duration = nDuration;
                if (source.RechargeTime > 0.0 & source.PowerType == Enums.ePowerType.Click)
                    effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                if (nDelay > 0.0)
                    effect.DelayedTime = nDelay;
                if (effect.Absorbed_Duration > 0.0 & num2 > 0.0)
                    effect.nDuration = effect.Absorbed_Duration;
                Effects[num3 + length] = effect;
            }
            return array;
        }

        public void ApplyGrantPowerEffects()
        {
            bool flag = true;
            int num1 = 0;
            int num2 = 0;
            if (!HasGrantPowerEffect)
                return;
            for (; flag & num1 < 100; ++num1)
            {
                flag = false;
                int[] array1 = new int[0];
                int[] array2 = new int[0];
                for (int index = num2; index < Effects.Length; ++index)
                {
                    if (Effects[index].EffectType != Enums.eEffectType.GrantPower || !Effects[index].CanGrantPower() || 
                        Effects[index].EffectClass == Enums.eEffectClass.Ignored || Effects[index].nSummon <= -1)
                        continue;
                    Array.Resize(ref array1, array1.Length + 1);
                    Array.Resize(ref array2, array2.Length + 1);
                    array1[array1.Length - 1] = index;
                    array2[array2.Length - 1] = Effects[index].nSummon;
                }
                num2 = Effects.Length;
                for (int index1 = 0; index1 <= array1.Length - 1; ++index1)
                {
                    flag = true;
                    Effects[array1[index1]].EffectClass = Enums.eEffectClass.Ignored;
                    int length = Effects.Length;
                    AbsorbEffects(DatabaseAPI.Database.Power[array2[index1]], Effects[array1[index1]].Duration, 0.0f, MidsContext.Archetype, 1, true, array1[index1]);
                    for (int index2 = length; index2 < Effects.Length; ++index2)
                    {
                        if (Effects[array1[index1]].Absorbed_Power_nID > -1)
                            Effects[index2].Absorbed_PowerType = Effects[array1[index1]].Absorbed_PowerType;
                        if (Effects[index2].EffectType != Enums.eEffectType.GrantPower)
                            Effects[index2].ToWho = Effects[array1[index1]].ToWho;
                        if (Effects[index2].ToWho == Enums.eToWho.All && (EntitiesAffected & Enums.eEntity.Caster) != Enums.eEntity.Caster)
                            Effects[index2].ToWho = Enums.eToWho.Target;
                        Effects[index2].isEnhancementEffect = Effects[array1[index1]].isEnhancementEffect;
                        if (Effects[array1[index1]].Probability < 1.0)
                            Effects[index2].Probability = Effects[array1[index1]].Probability * Effects[index2].Probability;
                    }
                }
            }
        }

        int[] GetValidEnhancementsFromSets()

        {
            List<int> intList = new List<int>();
            foreach (EnhancementSet enhancementSet in DatabaseAPI.Database.EnhancementSets)
            {
                bool flag = SetTypes.Any(setType => enhancementSet.SetType == setType);
                if (!flag)
                    continue;
                intList.AddRange(enhancementSet.Enhancements);
            }
            return intList.ToArray();
        }

        public int[] GetValidEnhancements(Enums.eType iType, Enums.eSubtype iSubType = Enums.eSubtype.None)
        {
            List<int> intList = new List<int>();
            int[] numArray;
            if (iType == Enums.eType.SetO)
            {
                numArray = GetValidEnhancementsFromSets();
            }
            else
            {
                for (int index1 = 0; index1 <= DatabaseAPI.Database.Enhancements.Length - 1; ++index1)
                {
                    IEnhancement enhancement1 = DatabaseAPI.Database.Enhancements[index1];
                    if (enhancement1.TypeID != iType)
                        continue;
                    bool flag = false;
                    foreach (int index2 in enhancement1.ClassID)
                    {
                        foreach (int enhancement2 in Enhancements)
                        {
                            if (DatabaseAPI.Database.EnhancementClasses[index2].ID == enhancement2 && (enhancement1.SubTypeID == Enums.eSubtype.None || iSubType == Enums.eSubtype.None || enhancement1.SubTypeID == iSubType))
                                flag = true;
                        }
                    }
                    if (flag)
                        intList.Add(index1);
                }
                numArray = intList.ToArray();
            }
            return numArray;
        }

        public bool IsEnhancementValid(int iEnh)
        {
            if (iEnh < 0 || iEnh > DatabaseAPI.Database.Enhancements.Length - 1) return false;
            return GetValidEnhancements(DatabaseAPI.Database.Enhancements[iEnh].TypeID)
                .Any(validEnhancement => validEnhancement == iEnh);

        }

        public void AbsorbPetEffects(int hIdx = -1)
        {
            if (!AbsorbSummonAttributes && !AbsorbSummonEffects)
                return;
            List<int> intList = new List<int>();
            for (int index = 0; index < Effects.Length; ++index)
            {
                if (Effects[index].EffectType == Enums.eEffectType.EntCreate && Effects[index].nSummon > -1 && Math.Abs(Effects[index].Probability - 1f) < 0.01 && DatabaseAPI.Database.Entities.Length > Effects[index].nSummon)
                    intList.Add(index);
            }
            if (intList.Count > 0)
                HasAbsorbedEffects = true;
            foreach (var t in intList)
            {
                IEffect effect = Effects[t];
                int nSummon1 = effect.nSummon;
                int stacking = 1;
                if (VariableEnabled && effect.VariableModified && hIdx > -1 && MidsContext.Character != null && MidsContext.Character.CurrentBuild.Powers[hIdx].VariableValue > stacking)
                    stacking = MidsContext.Character.CurrentBuild.Powers[hIdx].VariableValue;
                var nPowerset = DatabaseAPI.Database.Entities[nSummon1].GetNPowerset();
                if (nPowerset.Count == 0)
                    continue;
                if (AbsorbSummonAttributes && nPowerset[0] > -1 && nPowerset[0] < DatabaseAPI.Database.Powersets.Length)
                {
                    IPowerset powerset = DatabaseAPI.Database.Powersets[nPowerset[0]];
                    if (powerset.Power.Length > 0)
                    {
                        foreach (IPower power in powerset.Powers)
                        {
                            ActivatePeriod = power.ActivatePeriod;
                            AttackTypes = power.AttackTypes;
                            EffectArea = power.EffectArea;
                            EntitiesAffected = power.EntitiesAffected;
                            if (EntitiesAutoHit != Enums.eEntity.None)
                                EntitiesAutoHit = power.EntitiesAutoHit;
                            Ignore_Buff = power.Ignore_Buff;
                            IgnoreEnh = power.IgnoreEnh;
                            MaxTargets = power.MaxTargets;
                            Radius = power.Radius;
                            Target = power.Target;
                            ActivatePeriod = power.ActivatePeriod;
                            if (DatabaseAPI.Database.Power[PowerIndex].EntitiesAutoHit == Enums.eEntity.None ||
                                DatabaseAPI.Database.Power[PowerIndex].EntitiesAutoHit == Enums.eEntity.Caster)
                                continue;
                            Accuracy = power.Accuracy;
                            break;
                        }
                    }
                }

                if (!AbsorbSummonEffects)
                    continue;
                foreach (int setIndex in nPowerset)
                {
                    if (setIndex < 0 || setIndex >= DatabaseAPI.Database.Powersets.Length)
                        continue;
                    foreach (IPower power1 in DatabaseAPI.Database.Powersets[setIndex].Powers)
                    {
                        foreach (int absorbEffect in AbsorbEffects(power1, effect.Duration, effect.DelayedTime, DatabaseAPI.Database.Classes[DatabaseAPI.Database.Entities[nSummon1].GetNClassId()], stacking))
                        {
                            int nSummon2 = power1.Effects[absorbEffect].nSummon;
                            if (DatabaseAPI.Database.Entities[nSummon2].GetNPowerset()[0] < 0)
                                continue;
                            foreach (IPower power2 in DatabaseAPI.Database.Powersets[DatabaseAPI.Database.Entities[nSummon2].GetNPowerset()[0]].Powers)
                                AbsorbEffects(power2, effect.Duration, effect.DelayedTime, DatabaseAPI.Database.Classes[DatabaseAPI.Database.Entities[nSummon1].GetNClassId()], stacking);
                        }
                    }
                }
            }
        }
        public override string ToString()
            => DisplayName;

        public bool AllowedForClass(int classId)
        {
            //If a power neither requires a class nor excludes one, just return true.
            if ((this.Requires.NClassName.Length == 0) && (this.Requires.NClassNameNot.Length == 0))
                return true;

            //Check if the power has a class requirement.
            if (this.Requires.NClassName.Length > 0)
                return this.Requires.NClassName.Contains(classId);

            //Check if the power has a class exclusion.
            if (this.Requires.NClassNameNot.Length > 0)
                return !this.Requires.NClassNameNot.Contains(classId);

            return true;
        }
    }
}

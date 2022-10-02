#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Power : IPower, IComparable
    {
        private bool Contains;
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
            SetTypes = new List<int>();
            VariableName = string.Empty;
            UIDSubPower = new string[0];
            NIDSubPower = new int[0];
            Ignore_Buff = new Enums.eEnhance[0];
            IgnoreEnh = new Enums.eEnhance[0];
            SubIsAltColor = false;
            BoostsAllowed = new string[0];
            Requires = new Requirement();
            var num = -2;
            for (var index = 0; index <= DatabaseAPI.Database.Power.Length - 1; ++index)
                if (DatabaseAPI.Database.Power[index] != null && DatabaseAPI.Database.Power[index].StaticIndex > -1 &&
                    DatabaseAPI.Database.Power[index].StaticIndex > num)
                {
                    num = DatabaseAPI.Database.Power[index].StaticIndex;
                }

            StaticIndex = num + 1;
            Active = false;
            Taken = false;
            Stacks = 0;
            VariableStart = 0;
        }

        public Power(IPower? template)
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
            //SetTypes = new Enums.eSetType[0];
            SetTypes = new List<int>();
            VariableName = string.Empty;
            UIDSubPower = new string[0];
            NIDSubPower = new int[0];
            Ignore_Buff = new Enums.eEnhance[0];
            IgnoreEnh = new Enums.eEnhance[0];
            SubIsAltColor = false;
            if (template == null)
            {
                return;
            }

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

            SetTypes = template.SetTypes;
            // SetTypes = new Enums.eSetType[template.SetTypes.Length];
            // Array.Copy(template.SetTypes, SetTypes, SetTypes.Length);

            Effects = new IEffect[template.Effects.Length];
            for (var index = 0; index <= Effects.Length - 1; ++index)
            {
                Effects[index] = (IEffect) template.Effects[index].Clone();
                Effects[index].SetPower(this);
            }

            ClickBuff = template.ClickBuff;
            AlwaysToggle = template.AlwaysToggle;
            Level = template.Level;
            AllowFrontLoading = template.AllowFrontLoading;
            VariableEnabled = template.VariableEnabled;
            VariableOverride = template.VariableOverride;
            VariableName = template.VariableName;
            VariableMin = template.VariableMin;
            VariableMax = template.VariableMax;
            VariableStart = template.VariableStart;
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

            //SetTypes = new Enums.eSetType[0];

            SetTypes = new List<int>();

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
            for (var index = 0; index < GroupMembership.Length; ++index)
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
            for (var index = 0; index <= BoostsAllowed.Length - 1; ++index)
                BoostsAllowed[index] = reader.ReadString();
            CastThroughHold = reader.ReadBoolean();
            IgnoreStrength = reader.ReadBoolean();
            DescShort = reader.ReadString();
            DescLong = reader.ReadString();
            Enhancements = new int[reader.ReadInt32() + 1];
            for (var index = 0; index <= Enhancements.Length - 1; ++index)
                Enhancements[index] = reader.ReadInt32();

            // SetTypes = new Enums.eSetType[reader.ReadInt32() + 1];
            // for (var index = 0; index <= SetTypes.Length - 1; ++index)
            // {
            //     SetTypes[index] = (Enums.eSetType)reader.ReadInt32();
            // }

            var setTypeCount = reader.ReadInt32();
            for (var i = 0; i <= setTypeCount; i++)
            {
                var setType = reader.ReadInt32();
                SetTypes.Add(setType);
            }

            ClickBuff = reader.ReadBoolean();
            AlwaysToggle = reader.ReadBoolean();
            Level = reader.ReadInt32();
            AllowFrontLoading = reader.ReadBoolean();
            VariableEnabled = reader.ReadBoolean();
            VariableOverride = reader.ReadBoolean();
            VariableName = reader.ReadString();
            VariableMin = reader.ReadInt32();
            VariableMax = reader.ReadInt32();
            UIDSubPower = new string[reader.ReadInt32() + 1];
            for (var index = 0; index <= UIDSubPower.Length - 1; ++index)
                UIDSubPower[index] = reader.ReadString();
            IgnoreEnh = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (var index = 0; index <= IgnoreEnh.Length - 1; ++index)
                IgnoreEnh[index] = (Enums.eEnhance)reader.ReadInt32();
            Ignore_Buff = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (var index = 0; index <= Ignore_Buff.Length - 1; ++index)
                Ignore_Buff[index] = (Enums.eEnhance)reader.ReadInt32();
            SkipMax = reader.ReadBoolean();
            InherentType = (Enums.eGridType)reader.ReadInt32();
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
            for (var index = 0; index <= Effects.Length - 1; ++index)
            {
                var eff = (IEffect)new Effect(reader)
                {
                    nID = index
                };
                eff.SetPower(this);

                Effects[index] = eff;
            }

            HiddenPower = reader.ReadBoolean();
            Active = reader.ReadBoolean();
            Taken = reader.ReadBoolean();
            Stacks = reader.ReadInt32();
            VariableStart = reader.ReadInt32();
        }

        public IPowerset? GetPowerSet()
        {
            if (!((PowerSetID < 0) | (PowerSetID > DatabaseAPI.Database.Powersets.Length)))
            {
                return DatabaseAPI.Database.Powersets[PowerSetID];
            }
            else
            {
                return null;
            }
        }

        public float CastTimeReal { get; set; }
        
        public float ToggleCost
            => !((PowerType == Enums.ePowerType.Toggle) & (ActivatePeriod > 0.0)) ? EndCost : EndCost / ActivatePeriod;

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

        public List<int> SetTypes { get; set; }

        public bool ClickBuff { get; set; }

        public bool AlwaysToggle { get; set; }

        public int Level { get; set; }

        public bool AllowFrontLoading { get; set; }

        public bool VariableEnabled { get; set; }

        public bool VariableOverride { get; set; }

        public string VariableName { get; set; }

        public int VariableMin { get; set; }

        public int VariableMax { get; set; }

        public int VariableStart { get; set; }

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

        public bool HasProcSlotted { get; set; }

        public string FullSetName
        {
            get
            {
                var strArray = FullName.Split('.');
                return strArray.Length <= 1 ? string.Empty : strArray[0] + "." + strArray[1];
            }
        }

        public float CastTime
        {
            get => !MidsContext.Config.UseArcanaTime
                ? CastTimeReal
                : (float) (Math.Ceiling(CastTimeReal / 0.132f) + 1.0) * 0.132f;
            set => CastTimeReal = value;
        }

        public bool Slottable
        {
            get
            {
                var ps = GetPowerSet();
                return Enhancements.Length > 0 && (ps.SetType == Enums.ePowerSetType.Primary ||
                                                   ps.SetType == Enums.ePowerSetType.Secondary ||
                                                   ps.SetType == Enums.ePowerSetType.Ancillary ||
                                                   ps.SetType == Enums.ePowerSetType.Inherent ||
                                                   ps.SetType == Enums.ePowerSetType.Pool);
            }
        }

        public float AoEModifier => EffectArea != Enums.eEffectArea.Cone
            ? EffectArea != Enums.eEffectArea.Sphere ? 1f : (float) (1.0 + Radius * 0.150000005960464)
            : (float) (1.0 + Radius * 0.150000005960464 - Radius * 0.000366669992217794 * (360 - Arc));

        public int DisplayLocation
        {
            get => LocationIndex;
            set => LocationIndex = value;
        }

        public bool HasMutexID(int index)
        {
            for (var index1 = 0; index1 <= NGroupMembership.Length - 1; ++index1)
                if (NGroupMembership[index1] == index)
                {
                    return true;
                }

            return false;
        }

        public bool Active { get; set; }
        public bool Taken { get; set; }

        private int _VirtualStacks;
        private int _Stacks;
        public int VirtualStacks
        {
            get => _VirtualStacks;
            set => _VirtualStacks = value;
        }

        public int Stacks
        {
            get => _VirtualStacks;
            set
            {
                _Stacks = value;
                _VirtualStacks = value;
            }
        }

        public int InternalStacks => _Stacks;

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
            writer.Write((int) ModesRequired);
            writer.Write((int) ModesDisallowed);
            writer.Write((int) PowerType);
            writer.Write(Accuracy);
            writer.Write((int) AttackTypes);
            writer.Write(GroupMembership.Length - 1);
            for (var index = 0; index <= GroupMembership.Length - 1; ++index)
                writer.Write(GroupMembership[index]);
            writer.Write((int) EntitiesAffected);
            writer.Write((int) EntitiesAutoHit);
            writer.Write((int) Target);
            writer.Write(TargetLoS);
            writer.Write(Range);
            writer.Write((int) TargetSecondary);
            writer.Write(RangeSecondary);
            writer.Write(EndCost);
            writer.Write(InterruptTime);
            writer.Write(CastTimeReal);
            writer.Write(RechargeTime);
            writer.Write(BaseRechargeTime);
            writer.Write(ActivatePeriod);
            writer.Write((int) EffectArea);
            writer.Write(Radius);
            writer.Write(Arc);
            writer.Write(MaxTargets);
            writer.Write(MaxBoosts);
            writer.Write((int) CastFlags);
            writer.Write((int) AIReport);
            writer.Write(NumCharges);
            writer.Write(UsageTime);
            writer.Write(LifeTime);
            writer.Write(LifeTimeInGame);
            writer.Write(NumAllowed);
            writer.Write(DoNotSave);
            writer.Write(BoostsAllowed.Length - 1);
            for (var index = 0; index <= BoostsAllowed.Length - 1; ++index)
                writer.Write(BoostsAllowed[index]);
            writer.Write(CastThroughHold);
            writer.Write(IgnoreStrength);
            writer.Write(DescShort);
            writer.Write(DescLong);
            writer.Write(Enhancements.Length - 1);
            for (var index = 0; index <= Enhancements.Length - 1; ++index)
                writer.Write(Enhancements[index]);

            // writer.Write(SetTypes.Length - 1);
            // for (var index = 0; index <= SetTypes.Length - 1; ++index)
            // {
            //     writer.Write((int) SetTypes[index]);
            // }

            writer.Write(SetTypes.Count - 1);
            foreach (var setType in SetTypes)
            {
                writer.Write(setType);
            }


            writer.Write(ClickBuff);
            writer.Write(AlwaysToggle);
            writer.Write(Level);
            writer.Write(AllowFrontLoading);
            writer.Write(VariableEnabled);
            writer.Write(VariableOverride);
            writer.Write(VariableName);
            writer.Write(VariableMin);
            writer.Write(VariableMax);
            writer.Write(UIDSubPower.Length - 1);
            for (var index = 0; index <= UIDSubPower.Length - 1; ++index)
                writer.Write(UIDSubPower[index]);
            writer.Write(IgnoreEnh.Length - 1);
            for (var index = 0; index <= IgnoreEnh.Length - 1; ++index)
                writer.Write((int) IgnoreEnh[index]);
            writer.Write(Ignore_Buff.Length - 1);
            for (var index = 0; index <= Ignore_Buff.Length - 1; ++index)
                writer.Write((int) Ignore_Buff[index]);
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
            for (var index = 0; index <= Effects.Length - 1; ++index)
                Effects[index].StoreTo(ref writer);
            writer.Write(HiddenPower);
            writer.Write(Active);
            writer.Write(Taken);
            writer.Write(Stacks);
            writer.Write(VariableStart);
        }

        //public PowerEntry? GetPowerEntry() => MidsContext.Character.CurrentBuild.Powers.FirstOrDefault(x => x.Power == this);
        public PowerEntry? GetPowerEntry() => MidsContext.Character.CurrentBuild.Powers.FirstOrDefault(x => x.Power != null && x.Power.DisplayName == DisplayName);

        public float FXGetDamageValue(bool absorb = false)
        {
            var totalDamage = 0f;
            IPower power = new Power(this);
            if (absorb)
            {
                power.AbsorbPetEffects();
            }

            foreach (var effect in power.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum && !(Math.Abs(effect.Probability) > 0.999000012874603) ||
                    (effect.EffectClass == Enums.eEffectClass.Ignored || effect.DamageType == Enums.eDamage.Special &&
                        effect.ToWho == Enums.eToWho.Self) || (!(effect.Probability > 0) || !effect.CanInclude()) ||
                    !effect.PvXInclude())
                {
                    continue;
                }

                var effectDmg = effect.BuffedMag;

                if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
                {
                    effectDmg *= effect.Probability;
                }

                if (power.PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
                {
                    effectDmg = (float)(effectDmg * power.ActivatePeriod / 10d);
                }

                if (effect.Ticks > 1)
                {
                    effectDmg *= effect.CancelOnMiss &&
                            MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average &&
                            effect.Probability < 1
                        ? (float)((1 - Math.Pow(effect.Probability, effect.Ticks)) / (1 - effect.Probability))
                        : effect.Ticks;
                }

                totalDamage += effectDmg;
            }

            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.DPS:
                    if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                    {
                        totalDamage /= power.ActivatePeriod;
                        break;
                    }

                    if (power.RechargeTime + (double)power.CastTime + power.InterruptTime > 0)
                    {
                        totalDamage /= power.RechargeTime + power.CastTime + power.InterruptTime;
                    }

                    break;
                case ConfigData.EDamageReturn.DPA:
                    if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                    {
                        totalDamage /= power.ActivatePeriod;
                        break;
                    }

                    if (power.CastTime > 0)
                    {
                        totalDamage /= power.CastTime;
                    }

                    break;
            }

            return totalDamage;
        }

        public string FXGetDamageString(bool absorb = false)
        {
            var names = Enum.GetNames(typeof(Enums.eDamage));
            var numArray1 = new float[names.Length];
            var numArray2 = new float[names.Length, 2];
            var numArray3 = new float[names.Length, 2];
            var dmgString = string.Empty;
            var totalDamage = 0f;

            IPower power = new Power(this);
            if (absorb)
            {
                power.AbsorbPetEffects();
            }

            var hasPercentDamage = power.Effects
                .Any(e => e.EffectType == Enums.eEffectType.Damage && e.DisplayPercentage | e.Aspect == Enums.eAspect.Str);

            foreach (var effect in power.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum &&
                    !(Math.Abs(effect.Probability) > 0.999000012874603) ||
                    effect.EffectClass == Enums.eEffectClass.Ignored ||
                    effect.DamageType == Enums.eDamage.Special && effect.ToWho == Enums.eToWho.Self ||
                    !(effect.Probability > 0) || !effect.CanInclude() || !effect.PvXInclude())
                {
                    continue;
                }

                var effectMag = effect.BuffedMag;

                if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
                {
                    effectMag *= effect.Probability;
                }

                if (power.PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
                {
                    effectMag = (float)(effectMag * power.ActivatePeriod / 10d);
                }

                switch (MidsContext.Config.DamageMath.ReturnValue)
                {
                    case ConfigData.EDamageReturn.DPS:
                        if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                        {
                            effectMag /= power.ActivatePeriod;
                            break;
                        }

                        if (power.RechargeTime + (double)power.CastTime > 0)
                        {
                            effectMag /= power.RechargeTime + power.CastTime;
                        }

                        break;
                    case ConfigData.EDamageReturn.DPA:
                        if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                        {
                            effectMag /= power.ActivatePeriod;
                            break;
                        }

                        if (power.CastTime > 0)
                        {
                            effectMag /= power.CastTime;
                        }

                        break;
                        //case ConfigData.EDamageReturn.Numeric:
                        //    break;
                }

                if (effect.Ticks != 0)
                {
                    var num2 = !effect.CancelOnMiss ||
                           MidsContext.Config.DamageMath.Calculate != ConfigData.EDamageMath.Average ||
                           effect.Probability >= 1
                        ? effect.Ticks
                        : (float)((1 - Math.Pow(effect.Probability, effect.Ticks)) / (1 - effect.Probability));

                    var index = 0;
                    if (Math.Abs(numArray2[(int)effect.DamageType, 0]) > 0.01)
                    {
                        index = 1;
                    }

                    numArray2[(int)effect.DamageType, index] = effectMag;
                    numArray3[(int)effect.DamageType, index] = num2;
                    totalDamage += effectMag * num2;
                }
                else
                {
                    totalDamage += effectMag;
                    numArray1[(int)effect.DamageType] += effectMag;
                }
            }

            if (totalDamage <= 0)
            {
                return dmgString;
            }

            for (var index = 0; index < numArray1.Length; index++)
            {
                if (!((numArray1[index] > 0) | (numArray2[index, 0] > 0)))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(dmgString))
                {
                    dmgString += ", ";
                }

                var str2 = $"{dmgString}{Enums.GetDamageName((Enums.eDamage)index)}(";
                if (numArray1[index] > 0)
                {
                    str2 += hasPercentDamage ? $"{Utilities.FixDP(numArray1[index] * 100)}%" : Utilities.FixDP(numArray1[index]);
                }

                if (Math.Abs(numArray2[index, 0]) > 0.01)
                {
                    if (numArray1[index] > 0)
                    {
                        str2 += "+";
                    }

                    str2 += $"{(hasPercentDamage ? $"{Utilities.FixDP(numArray2[index, 0] * 100)}%" : Utilities.FixDP(numArray2[index, 0]))}x{Utilities.FixDP(numArray3[index, 0])}";
                    if (Math.Abs(numArray2[index, 1]) > 0.01)
                    {
                        str2 += $"+{(hasPercentDamage ? $"{Utilities.FixDP(numArray2[index, 1] * 100)}%" : Utilities.FixDP(numArray2[index, 1]))}x{Utilities.FixDP(numArray3[index, 1])}";
                    }
                }

                dmgString = $"{str2})";
            }

            return $"{dmgString} = {(hasPercentDamage ? $"{Utilities.FixDP(totalDamage * 100)}% | {Utilities.FixDP(totalDamage * MidsContext.Character.Totals.HPMax)}" : Utilities.FixDP(totalDamage))}";
        }

        public int[] GetRankedEffects()
        {
            var numArray1 = new int[Effects.Length];
            for (var index1 = 0; index1 < Effects.Length; index1++)
            {
                if (MidsContext.Config != null && (MidsContext.Config.Suppression & Effects[index1].Suppression) == Enums.eSuppress.None & (!MidsContext.Config.Inc.DisablePvE & (Effects[index1].PvMode != Enums.ePvX.PvP) | MidsContext.Config.Inc.DisablePvE & (Effects[index1].PvMode != Enums.ePvX.PvE)))
                {
                    numArray1[index1] = (int) (Effects[index1].EffectClass + 1);
                    if (Math.Abs(Effects[index1].Probability - 1f) < 0.01)
                    {
                        numArray1[index1] += 10;
                    }

                    if (HasAbsorbedEffects & (Effects[index1].EffectType != Enums.eEffectType.EntCreate))
                    {
                        numArray1[index1] += 50;
                    }

                    if (Effects[index1].DelayedTime > 1)
                    {
                        numArray1[index1] -= 100;
                    }

                    if ((Effects[index1].DelayedTime > 0) & (Effects[index1].DelayedTime <= 1))
                    {
                        numArray1[index1] -= 25;
                    }

                    if (Effects[index1].InherentSpecial)
                    {
                        numArray1[index1] -= 100;
                    }

                    if (Effects[index1].InherentSpecial2)
                    {
                        numArray1[index1] -= 100;
                    }

                    if ((Effects[index1].ToWho == Enums.eToWho.Self) & ((Effects[index1].BuffedMag > 0) | (Effects[index1].EffectType == Enums.eEffectType.Mez)))
                    {
                        numArray1[index1] += 10;
                    }

                    if ((Effects[index1].ToWho == Enums.eToWho.Target) & (Effects[index1].BuffedMag < 0))
                    {
                        numArray1[index1] += 10;
                    }

                    if ((Effects[index1].ToWho == Enums.eToWho.Target) & (Effects[index1].BuffedMag > 0) & Effects[index1].Absorbed_Effect)
                    {
                        numArray1[index1] += 10;
                    }

                    if ((Effects[index1].ToWho == Enums.eToWho.Self) & (Effects[index1].BuffedMag > 0) & Effects[index1].Absorbed_Effect)
                    {
                        numArray1[index1] += 10;
                    }

                    if (Effects[index1].isEnhancementEffect)
                    {
                        numArray1[index1] -= 30;
                    }

                    if (Effects[index1].VariableModified)
                    {
                        numArray1[index1] += 30;
                    }

                    switch (Effects[index1].EffectType)
                    {
                        case Enums.eEffectType.None:
                            numArray1[index1] -= 1000;
                            continue;
                        case Enums.eEffectType.Damage:
                            numArray1[index1] -= 500;
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
                            numArray1[index1] -= 20;
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
                            {
                                numArray1[index1] -= 1;
                            }

                            if ((Effects[index1].MezType == Enums.eMez.OnlyAffectsSelf) | (Effects[index1].MezType == Enums.eMez.Untouchable))
                            {
                                numArray1[index1] -= 9;
                                continue;
                            }

                            if ((Effects[index1].MezType == Enums.eMez.Knockback) |
                                (Effects[index1].MezType == Enums.eMez.Knockup))
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
                            numArray1[index1] -= 20;
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
                                numArray1[index1] -= 500;
                            }
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

            var iID1 = -1;
            var num1 = -1;
            var num2 = 0;
            var num3 = 0;
            for (var iID2 = 0; iID2 < numArray1.Length; iID2++)
                if (numArray1[iID2] > num2)
                {
                    num1 = iID1;
                    num3 = num2;
                    iID1 = iID2;
                    num2 = numArray1[iID2];
                }
                else if ((numArray1[iID2] > num3) & GreOverride(iID1, iID2))
                {
                    num1 = iID2;
                    num3 = numArray1[iID2];
                }

            return new[] {iID1, num1};
        }

        public int GetDurationEffectID()
        {
            var idx = -1;
            var eEffectClass = Enums.eEffectClass.Ignored;
            var probability = 0.0f;
            var duration = 0.0f;
            var isDmg = false;
            var eEffectType = Enums.eEffectType.None;
            var mag = 0.0f;
            var buffable = false;
            var isDefiance = false;
            var isEntCreate = false;
            var delayTime = int.MaxValue;
            for (var index = 0; index <= Effects.Length - 1; ++index)
            {
                var applies = false;
                if (!((!MidsContext.Config.Inc.DisablePvE & (Effects[index].PvMode != Enums.ePvX.PvP)) | (MidsContext.Config.Inc.DisablePvE & (Effects[index].PvMode != Enums.ePvX.PvE))) || !(((Effects[index].Duration > 0.0) | (Effects[index].EffectType == Enums.eEffectType.EntCreate)) & (Effects[index].EffectClass != Enums.eEffectClass.Ignored)))
                {
                    continue;
                }

                if (Effects[index].EffectClass < eEffectClass)
                {
                    applies = true;
                }

                if ((Effects[index].Probability > (double) probability) & (Effects[index].SpecialCase != Enums.eSpecialCase.Defiance || !Effects[index].ValidateConditional("Active", "Defiance")))
                {
                    applies = true;
                }


                //write in where active conditionals true clause doesn't coincide with defiance

                if (Effects[index].Buffable & !buffable)
                {
                    applies = true;
                }

                if ((Effects[index].SpecialCase != Enums.eSpecialCase.Defiance || !Effects[index].ValidateConditional("Active", "Defiance")) & isDefiance)
                {
                    applies = true;
                }

                if ((Math.Abs(Effects[index].Probability - probability) < 0.01) & (Effects[index].Duration > (double) duration) & !Effects[index].isEnhancementEffect & (Effects[index].SpecialCase != Enums.eSpecialCase.Defiance || !Effects[index].ValidateConditional("Active", "Defiance")) && (eEffectType != Enums.eEffectType.Mez) | (Effects[index].EffectType == Enums.eEffectType.Mez))
                {
                    if ((eEffectType == Enums.eEffectType.Mez) & (Effects[index].EffectType == Enums.eEffectType.Mez))
                    {
                        if (Effects[index].BuffedMag > (double) mag || Effects[index].SpecialCase == Enums.eSpecialCase.Domination && MidsContext.Character.Domination || Effects[index].ValidateConditional("Active", "Domination") && MidsContext.Character.Domination)
                        {
                            applies = true;
                        }
                    }
                    else
                    {
                        applies = true;
                    }
                }

                if ((delayTime > (double) Effects[index].DelayedTime) & (Effects[index].DelayedTime > 5.0))
                {
                    applies = true;
                }

                if ((Math.Abs(Effects[index].Probability - 1f) < 0.01) & isDmg & (Effects[index].EffectType == Enums.eEffectType.Mez))
                {
                    applies = true;
                }

                if (Effects[index].EffectType == Enums.eEffectType.Mez && Effects[index].MezType == Enums.eMez.Taunt && Effects[index].EffectClass != Enums.eEffectClass.Primary)
                {
                    applies = false;
                }

                if (((Effects[index].EffectClass > eEffectClass ? 1 : 0) & (Effects[index].SpecialCase != Enums.eSpecialCase.Domination ? 1 : !MidsContext.Character.Domination ? 1 : 0)) != 0)
                {
                    applies = false;
                }

                /*if ((Effects[index].EffectClass > eEffectClass ? 1 : 0) == 0)
                {
                    if (Effects[index].SpecialCase == Enums.eSpecialCase.None && !Effects[index].ValidateConditional())
                    {
                        applies = false;
                    }
                    else
                    {
                        applies = true;
                    }
                }*/
                if (((Effects[index].EffectClass > eEffectClass ? 1 : 0) & (!Effects[index].ValidateConditional("Active", "Domination") ? 1 : !MidsContext.Character.Domination ? 1 : 0)) != 0)
                {
                    applies = false;
                }

                if (((Effects[index].EffectClass > eEffectClass ? 1 : 0) & (!Effects[index].ValidateConditional("Active", "Assassination") ? 1 : !MidsContext.Character.Assassination ? 1 : 0)) != 0)
                {
                    applies = false;
                }

                if ((Effects[index].EffectType == Enums.eEffectType.EntCreate) & !isEntCreate &
                    (Effects[index].DelayedTime < 20.0) & (eEffectType != Enums.eEffectType.Mez))
                {
                    applies = true;
                }

                if (isEntCreate & (Effects[index].EffectType != Enums.eEffectType.EntCreate) &
                    ((Effects[index].Duration < (double) duration) | (Math.Abs(duration) < 0.01) |
                     (Effects[index].DelayedTime > (double) delayTime) |
                     ((Effects[index].BuffedMag < 0.0) & (Effects[index].ToWho == Enums.eToWho.Self))))
                {
                    applies = false;
                }

                if (isEntCreate & (Effects[index].EffectType != Enums.eEffectType.EntCreate) &
                    (Effects[index].Absorbed_Duration > (double) duration))
                {
                    applies = true;
                }

                if ((eEffectType == Enums.eEffectType.Mez) & (mag < 0) &
                    ((Effects[index].EffectType == Enums.eEffectType.Resistance) |
                     (Effects[index].EffectType == Enums.eEffectType.Regeneration)) & (Effects[index].BuffedMag > 0))
                {
                    applies = true;
                }

                if (Effects[index].EffectType == Enums.eEffectType.SetMode)
                {
                    applies = false;
                }

                if (!applies)
                {
                    continue;
                }

                idx = index;
                eEffectClass = Effects[index].EffectClass;
                probability = Effects[index].Probability;
                duration = !(isEntCreate & (Effects[index].EffectType != Enums.eEffectType.EntCreate) &
                             (Effects[index].Absorbed_Duration > (double) duration))
                    ? Effects[index].Duration
                    : Effects[index].Absorbed_Duration;
                isDmg = Effects[index].EffectType == Enums.eEffectType.Damage;
                eEffectType = Effects[index].EffectType;
                mag = Effects[index].BuffedMag;
                buffable = Effects[index].Buffable;
                if (Effects[index].SpecialCase == Enums.eSpecialCase.Defiance || Effects[index].ValidateConditional("Active", "Defiance"))
                {
                    isDefiance = true;
                }
                else
                {
                    isDefiance = false;
                }
                //isDefiance = Effects[index].SpecialCase == Enums.eSpecialCase.Defiance;
                isEntCreate = Effects[index].EffectType == Enums.eEffectType.EntCreate;
                delayTime = (int) Effects[index].DelayedTime;
            }

            return idx;
        }

        public float[] GetDef(int buffDebuff = 0)
        {
            var numArray = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            var flag = false;
            var ePvX = !MidsContext.Config.Inc.DisablePvE ? Enums.ePvX.PvE : Enums.ePvX.PvP;
            foreach (var fx in Effects)
            {
                if (!(fx.EffectType == Enums.eEffectType.Defense & fx.Probability > 0 &
                      fx.CanInclude()) ||
                    !(buffDebuff == 0 | buffDebuff < 0 & fx.BuffedMag < 0 |
                      buffDebuff > 0 & fx.BuffedMag > 0.0) ||
                    (fx.Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None ||
                    !(fx.PvMode == ePvX | fx.PvMode == Enums.ePvX.Any))
                {
                    continue;
                }

                numArray[(int) fx.DamageType] += fx.BuffedMag;
                if (fx.DamageType != Enums.eDamage.None)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                return numArray;
            }

            var num = numArray[0];
            for (var index = 0; index < numArray.Length; index++)
            {
                numArray[index] = num;
            }

            return numArray;
        }

        public float[] GetRes(bool pvE = true)
        {
            var resists = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            var hasDamage = false;
            foreach (var fx in Effects)
            {
                if (!(fx.EffectType == Enums.eEffectType.Resistance & fx.Probability > 0 &
                      fx.CanInclude()) ||
                    !(fx.PvMode != Enums.ePvX.PvP & pvE |
                      fx.PvMode != Enums.ePvX.PvE & !pvE))
                {
                    continue;
                }

                resists[(int) fx.DamageType] += fx.BuffedMag;
                if (fx.DamageType != Enums.eDamage.None)
                {
                    hasDamage = true;
                }
            }

            if (hasDamage)
            {
                return resists;
            }

            var num = resists[0];
            for (var index = 0; index < resists.Length; index++)
            {
                resists[index] = num;
            }

            return resists;
        }

        public bool HasDefEffects()
        {
            return Effects.Any(t => t.EffectType == Enums.eEffectType.Defense & t.Probability > 0 & (t.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None & (t.PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE | t.PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE));
        }

        public bool HasResEffects()
        {
            return Effects.Any(t => t.EffectType == Enums.eEffectType.Resistance & t.Probability > 0 & (t.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None & (t.PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE | t.PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE));
        }

        public bool HasAttribModEffects()
        {
            return Effects.Any(t => t.EffectType == Enums.eEffectType.ModifyAttrib);
        }

        public Enums.ShortFX GetEnhancementMagSum(Enums.eEffectType iEffect, int subType = 0)
        {
            var shortFx = new Enums.ShortFX();
            for (var iIndex = 0; iIndex < Effects.Length; iIndex++)
            {
                if (!Effects[iIndex].PvXInclude() || !(Effects[iIndex].Probability > 0) ||
                    (Effects[iIndex].ETModifies != iEffect || !Effects[iIndex].CanInclude()) ||
                    Effects[iIndex].EffectType != Enums.eEffectType.Enhancement &&
                    Effects[iIndex].EffectType != Enums.eEffectType.DamageBuff ||
                    Effects[iIndex].Absorbed_Effect &&
                    Effects[iIndex].Absorbed_PowerType == Enums.ePowerType.GlobalBoost)
                {
                    continue;
                }

                if (iEffect == Enums.eEffectType.Mez && Effects[iIndex].ToWho != Enums.eToWho.Target)
                {
                    if ((Enums.eMez) subType == Effects[iIndex].MezType || subType < 0)
                    {
                        shortFx.Add(iIndex, Effects[iIndex].BuffedMag);
                    }
                }
                else if (Effects[iIndex].ToWho is not Enums.eToWho.Target)
                {
                    shortFx.Add(iIndex, Effects[iIndex].BuffedMag);
                }
            }

            return shortFx;
        }

        public Enums.ShortFX GetEffectMagSum(Enums.eEffectType iEffect, bool includeDelayed = false, bool onlySelf = false, bool onlyTarget = false, bool maxMode = false)
        {
            var shortFx = new Enums.ShortFX();
            for (var iIndex = 0; iIndex < Effects.Length; iIndex++)
            {
                var flag = false;

                switch (Effects[iIndex].ToWho)
                {
                    case Enums.eToWho.Target when !onlySelf:
                    case Enums.eToWho.Self when !onlyTarget:
                    case Enums.eToWho.All:
                        flag = true;
                        break;
                }

                if ((iEffect == Enums.eEffectType.SpeedFlying) & !maxMode &&
                    Effects[iIndex].Aspect == Enums.eAspect.Max ||
                    (iEffect == Enums.eEffectType.SpeedRunning) & !maxMode &
                    (Effects[iIndex].Aspect == Enums.eAspect.Max) || (iEffect == Enums.eEffectType.SpeedJumping) &
                    !maxMode & (Effects[iIndex].Aspect == Enums.eAspect.Max))
                {
                    flag = false;
                }

                if ((MidsContext.Config.Suppression & Effects[iIndex].Suppression) != Enums.eSuppress.None)
                {
                    flag = false;
                }

                if (!flag || !(Effects[iIndex].Probability > 0) ||
                    maxMode && Effects[iIndex].Aspect != Enums.eAspect.Max || Effects[iIndex].EffectType != iEffect ||
                    Effects[iIndex].EffectClass == Enums.eEffectClass.Ignored ||
                    Effects[iIndex].EffectClass == Enums.eEffectClass.Special ||
                    !(Effects[iIndex].DelayedTime <= 5) && !includeDelayed || !Effects[iIndex].CanInclude() ||
                    !Effects[iIndex].PvXInclude())
                {
                    continue;
                }

                if (Effects[iIndex].ActiveConditionals is { Count: > 0 })
                {
                    if (!Effects[iIndex].ValidateConditional())
                    {
                        continue;
                    }
                }

                var mag = Effects[iIndex].BuffedMag;
                if (Effects[iIndex].Ticks > 1 && Effects[iIndex].Stacking == Enums.eStacking.Yes)
                {
                    mag *= Effects[iIndex].Ticks;
                }

                shortFx.Add(iIndex, mag);
            }

            return shortFx;
        }

        public Enums.ShortFX GetEffectMagSum(Enums.eEffectType iEffect, Enums.eEffectType etModifies, Enums.eDamage damageType, Enums.eMez mezType, bool includeDelayed = false, bool onlySelf = false, bool onlyTarget = false, bool maxMode = false)
        {
            var shortFx = new Enums.ShortFX();
            for (var i = 0; i < Effects.Length; i++)
            {
                var fx = Effects[i];
                if (fx.EffectType != iEffect || fx.ETModifies != etModifies || fx.DamageType != damageType || fx.MezType != mezType)
                {
                    continue;
                }

                var includeFlag = fx.ToWho switch
                {
                    Enums.eToWho.Target when !onlySelf => true,
                    Enums.eToWho.Self when !onlyTarget => true,
                    Enums.eToWho.All => true,
                    _ => false
                };

                if ((iEffect == Enums.eEffectType.SpeedFlying) & !maxMode &&
                    fx.Aspect == Enums.eAspect.Max ||
                    (iEffect == Enums.eEffectType.SpeedRunning) & !maxMode &
                    (fx.Aspect == Enums.eAspect.Max) || (iEffect == Enums.eEffectType.SpeedJumping) &
                    !maxMode & (fx.Aspect == Enums.eAspect.Max))
                {
                    includeFlag = false;
                }

                if ((MidsContext.Config.Suppression & fx.Suppression) != Enums.eSuppress.None)
                {
                    includeFlag = false;
                }

                if (!includeFlag || fx.Probability <= 0 ||
                    maxMode && fx.Aspect != Enums.eAspect.Max || fx.EffectType != iEffect ||
                    fx.EffectClass is Enums.eEffectClass.Ignored or Enums.eEffectClass.Special ||
                    fx.DelayedTime > 5 && !includeDelayed || !fx.CanInclude() || !fx.PvXInclude())
                {
                    continue;
                }

                if (Effects[i].ActiveConditionals is {Count: > 0})
                {
                    if (!Effects[i].ValidateConditional())
                    {
                        continue;
                    }
                }

                /*if (fx.ActiveConditionals.Count > 0)
                {
                    if (!fx.ValidateConditional())
                    {
                        continue;
                    }
                }*/

                var mag = fx.BuffedMag;
                if (fx.Ticks > 1 && fx.Stacking == Enums.eStacking.Yes)
                {
                    mag *= fx.Ticks;
                }

                if (Math.Abs(mag) < float.Epsilon)
                {
                    continue;
                }

                shortFx.Add(i, mag);
            }

            return shortFx;
        }

        public Enums.ShortFX GetDamageMagSum(Enums.eEffectType iEffect, Enums.eDamage iSub, bool includeDelayed = false)
        {
            var shortFx = new Enums.ShortFX();
            for (var iIndex = 0; iIndex < Effects.Length; iIndex++)
            {
                if (!Effects[iIndex].CanInclude() || !((Effects[iIndex].EffectType == iEffect) & (Effects[iIndex].EffectClass != Enums.eEffectClass.Ignored)) || !Effects[iIndex].PvXInclude() || !(((Effects[iIndex].DelayedTime <= 5) | includeDelayed) & (Effects[iIndex].DamageType == iSub)))
                {
                    continue;
                }

                if (Effects[iIndex].ActiveConditionals is { Count: > 0 })
                {
                    if (!Effects[iIndex].ValidateConditional())
                    {
                        continue;
                    }
                }

                var mag = Effects[iIndex].BuffedMag;
                if ((PowerType == Enums.ePowerType.Toggle) & Effects[iIndex].isEnhancementEffect)
                {
                    mag /= 10f;
                }

                shortFx.Add(iIndex, mag);
            }

            return shortFx;
        }

        public Enums.ShortFX GetEffectMag(Enums.eEffectType iEffect, Enums.eToWho iTarget = Enums.eToWho.Unspecified, bool allowDelay = false)
        {
            var shortFx = new Enums.ShortFX();
            for (var iIndex = 0; iIndex < Effects.Length; iIndex++)
            {
                if (Effects[iIndex].EffectType != iEffect || Effects[iIndex].EffectClass == Enums.eEffectClass.Ignored || Effects[iIndex].InherentSpecial || Effects[iIndex].InherentSpecial2 || !Effects[iIndex].PvXInclude() || !(Effects[iIndex].DelayedTime <= 5) && !allowDelay || iTarget != Enums.eToWho.Unspecified && Effects[iIndex].ToWho != Enums.eToWho.All && iTarget != Effects[iIndex].ToWho)
                {
                    continue;
                }

                var mag = Effects[iIndex].BuffedMag;
                if (Effects[iIndex].Ticks > 1)
                {
                    mag *= Effects[iIndex].Ticks;
                }

                if (Effects[iIndex].DisplayPercentage && Effects[iIndex].EffectType is Enums.eEffectType.Heal or Enums.eEffectType.HitPoints)
                {
                    shortFx.Add(iIndex, mag / 100f * MidsContext.Archetype.Hitpoints);
                }
                else if (Effects[iIndex].EffectType is Enums.eEffectType.Heal or Enums.eEffectType.HitPoints)
                {
                    shortFx.Add(iIndex, (float) (mag / (double) MidsContext.Archetype.Hitpoints * 100));
                }
                else
                {
                    shortFx.Add(iIndex, mag);
                }

                return shortFx;
            }

            return shortFx;
        }

        public bool AffectsTarget(Enums.eEffectType iEffect)
        {
            return Effects.Any(t => t.EffectType == iEffect && t.ToWho == Enums.eToWho.Target);
        }

        public bool AffectsSelf(Enums.eEffectType iEffect)
        {
            return Effects.Any(t => t.EffectType == iEffect && t.ToWho == Enums.eToWho.Self);
        }

        public bool I9FXPresentP(Enums.eEffectType iEffect, Enums.eMez iMez = Enums.eMez.None)
        {
            return Effects.Where(t => t.EffectType == iEffect & t.BuffedMag > 0 && !(t.EffectType == Enums.eEffectType.Damage & t.DamageType == Enums.eDamage.Special)).Any(t => iMez == Enums.eMez.None || t.MezType == iMez);
        }

        public bool IgnoreEnhancement(Enums.eEnhance iEffect)
        {
            return IgnoreEnh.Length == 0 || IgnoreEnh.All(t => t != iEffect);
        }

        public bool IgnoreBuff(Enums.eEnhance iEffect)
        {
            return Ignore_Buff.Length == 0 || Ignore_Buff.All(t => t != iEffect);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Power power))
            {
                throw new ArgumentException("Comparison failed - Passed object was not an Archetype Class!");
            }

            var num = string.Compare(FullSetName, power.FullSetName, StringComparison.OrdinalIgnoreCase);
            return num == 0
                ? Level <= power.Level ? Level >= power.Level
                    ? Level != power.Level || !SortOverride || power.SortOverride
                        ? Level != power.Level || SortOverride || !power.SortOverride
                            ? string.Compare(FullName, power.FullName, StringComparison.OrdinalIgnoreCase)
                            : 1
                        : -1
                    : -1 : 1
                : num;
        }

        public void SetMathMag()
        {
            foreach (var index in Effects)
            {
                index.Math_Duration = index.Duration;
                index.Math_Mag = index.BuffedMag;
            }
        }

        public bool GetEffectStringGrouped(int idEffect, ref string returnString, ref int[] returnMask, bool shortForm, bool simple, bool noMag = false, bool fromPopup = false, bool ignoreConditions = false)
        {
            bool flag;
            if ((idEffect < 0) | (idEffect > Effects.Length - 1))
            {
                flag = false;
            }
            else
            {
                var str = string.Empty;
                var array = Array.Empty<int>();
                var effect = (IEffect) Effects[idEffect].Clone();
                switch (effect.EffectType)
                {
                    case Enums.eEffectType.DamageBuff or Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity:
                    {
                        var iDamage = new bool[Enum.GetValues(typeof(Enums.eDamage)).Length];
                        for (var index1 = 0; index1 <= Effects.Length - 1; ++index1)
                        {
                            for (var index2 = 0; index2 <= iDamage.Length - 1; ++index2)
                            {
                                effect.DamageType = (Enums.eDamage) index2;
                                if (effect.CompareTo(Effects[index1]) != 0)
                                {
                                    continue;
                                }

                                iDamage[index2] = true;
                                Array.Resize(ref array, array.Length + 1);
                                array[^1] = index1;
                            }
                        }

                        if (array.Length <= 1)
                        {
                            return false;
                        }

                        effect.DamageType = Enums.eDamage.Special;
                        var newValue = effect.EffectType == Enums.eEffectType.Defense ? Enums.GetGroupedDefense(iDamage, shortForm) : Enums.GetGroupedDamage(iDamage, shortForm);
                        str = shortForm ? effect.BuildEffectStringShort(noMag, simple).Replace("Spec", newValue) : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true, ignoreConditions).Replace("Special", newValue);
                        break;
                    }
                    case Enums.eEffectType.Mez or Enums.eEffectType.MezResist:
                    {
                        var iMez = new bool[Enum.GetValues(typeof(Enums.eMez)).Length];
                        for (var index1 = 0; index1 <= Effects.Length - 1; ++index1)
                        {
                            for (var index2 = 0; index2 <= iMez.Length - 1; ++index2)
                            {
                                effect.MezType = (Enums.eMez) index2;
                                if (effect.CompareTo(Effects[index1]) != 0)
                                {
                                    continue;
                                }

                                iMez[index2] = true;
                                Array.Resize(ref array, array.Length + 1);
                                array[^1] = index1;
                            }
                        }

                        if (array.Length <= 1)
                        {
                            return false;
                        }

                        effect.MezType = Enums.eMez.None;
                        var newValue = Enums.GetGroupedMez(iMez, shortForm);

                        if (newValue == "Knocked" && effect.BuffedMag < 0.0)
                        {
                            newValue = "Knockback Protection";
                        }

                        if (shortForm)
                        {
                            str = effect.BuildEffectStringShort(noMag, simple).Replace("None", newValue);
                        }
                        else
                        {
                            str = effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true, ignoreConditions).Replace("None", newValue);
                        }

                        switch (effect.EffectType)
                        {
                            case Enums.eEffectType.MezResist:
                            {
                                if (newValue == "Mez")
                                {
                                    str = str.Replace("MezResist(Mez)", "Status Resistance");
                                }

                                break;
                            }
                            case Enums.eEffectType.Mez when (newValue == "Mez") & (effect.BuffedMag < 0.0):
                                str = str.Replace("Mez", "Status Protection").Replace("-", string.Empty);
                                break;
                            case Enums.eEffectType.Mez:
                            {
                                if (newValue != "Knockback Protection")
                                {
                                    str = str.Replace("(Mag -", "protection (Mag ");
                                }

                                break;
                            }
                        }

                        break;
                    }
                    case Enums.eEffectType.Enhancement:
                    {
                        var num = 0;
                        if (Effects.Length == 4)
                        {
                            for (var index = 0; index <= Effects.Length - 1; ++index)
                            {
                                if (Effects[index].EffectType == Enums.eEffectType.Enhancement && (Effects[index].ETModifies == Enums.eEffectType.SpeedRunning) | (Effects[index].ETModifies == Enums.eEffectType.SpeedFlying) | (Effects[index].ETModifies == Enums.eEffectType.SpeedJumping) | (Effects[index].ETModifies == Enums.eEffectType.JumpHeight))
                                {
                                    ++num;
                                }
                            }

                            if (num == Effects.Length)
                            {
                                array = new int[Effects.Length];
                                for (var index = 0; index <= array.Length - 1; ++index)
                                {
                                    array[index] = index;
                                }

                                effect.ETModifies = Enums.eEffectType.Slow;
                                str = shortForm ? effect.BuildEffectStringShort(noMag, simple) : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true, ignoreConditions);
                                if (BuffMode != Enums.eBuffMode.Debuff)
                                {
                                    str = str.Replace("Slow", "Movement");
                                }
                            }
                        }

                        break;
                    }
                }

                returnMask = new int[array.Length];
                Array.Copy(array, returnMask, array.Length);
                returnString = str;
                flag = true;
            }

            return flag;
        }

        public int[] AbsorbEffects(IPower? source, float nDuration, float nDelay, Archetype? archetype, int stacking, bool isGrantPower = false, int fxid = -1, int effectId = -1)
        {
            var num1 = -1;
            var length = Effects.Length;
            var array = new int[0];
            var num2 = 0f;
            if (source.PowerSetID > -1 && DatabaseAPI.Database.Powersets[source.PowerSetID].SetType == Enums.ePowerSetType.Pet)
            {
                foreach (var power in DatabaseAPI.Database.Powersets[source.PowerSetID].Powers)
                {
                    foreach (var effect in power.Effects)
                    {
                        if (effect.EffectType == Enums.eEffectType.SilentKill & effect.ToWho == Enums.eToWho.Self & effect.DelayedTime > 0)
                        {
                            num2 = effect.DelayedTime;
                        }
                    }
                }
            }

            if (((num2 > 0 ? 1 : 0) & (nDuration < 0.01 ? 1 : nDuration > num2 ? 1 : 0)) != 0)
            {
                nDuration = num2;
            }

            if (effectId == -1)
            {
                for (var index = 0; index <= source.Effects.Length - 1; ++index)
                {
                    if (!isGrantPower & source.EntitiesAffected == Enums.eEntity.Caster & source.Effects[index].EffectType != Enums.eEffectType.EntCreate)
                    {
                        continue;
                    }

                    if (source.Effects[index].EffectType == Enums.eEffectType.EntCreate && source.Effects[index].nSummon > -1)
                    {
                        Array.Resize(ref array, array.Length + 1);
                        array[array.Length - 1] = index;
                    }

                    ++num1;
                    var effects = Effects;
                    Array.Resize(ref effects, num1 + length + 1);
                    Effects = effects;
                    var effect = (IEffect) source.Effects[index].Clone();
                    effect.Absorbed_Effect = true;
                    effect.Absorbed_PowerType = source.PowerType;
                    effect.Absorbed_Class_nID = archetype.Idx;
                    effect.Absorbed_EffectID = fxid;
                    effect.Absorbed_Power_nID = source.PowerIndex;
                    if (source.PowerType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle)
                    {
                        effect.SetTicks(nDuration, source.ActivatePeriod);
                    }

                    if ((source.EntitiesAutoHit & Enums.eEntity.Friend) == Enums.eEntity.Friend & (source.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster)
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            effect.Scale *= stacking;
                        }
                    }

                    if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet & (source.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster)
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            effect.Scale *= stacking;
                        }
                    }

                    effect.Absorbed_Duration = nDuration;
                    if (source.RechargeTime > 0 & source.PowerType == Enums.ePowerType.Click)
                    {
                        effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                    }

                    if (nDelay > 0)
                    {
                        effect.DelayedTime = nDelay;
                    }

                    if (effect.Absorbed_Duration > 0 & num2 > 0)
                    {
                        effect.nDuration = effect.Absorbed_Duration;
                    }

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

                var num3 = num1 + 1;
                var effects = Effects;
                Array.Resize(ref effects, num3 + length + 1);
                Effects = effects;
                var effect = (IEffect) source.Effects[effectId].Clone();
                effect.Absorbed_Effect = true;
                effect.Absorbed_PowerType = source.PowerType;
                effect.Absorbed_Class_nID = archetype.Idx;
                effect.Absorbed_EffectID = fxid;
                effect.Absorbed_Power_nID = source.PowerIndex;
                if (source.PowerType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle)
                {
                    effect.SetTicks(nDuration, source.ActivatePeriod);
                }

                if ((source.EntitiesAutoHit & Enums.eEntity.Friend) == Enums.eEntity.Friend)
                {
                    effect.ToWho = Enums.eToWho.Target;
                    if (effect.Stacking == Enums.eStacking.Yes)
                    {
                        effect.Scale *= stacking;
                    }
                }

                if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
                {
                    effect.ToWho = Enums.eToWho.Target;
                    if (effect.Stacking == Enums.eStacking.Yes)
                    {
                        effect.Scale *= stacking;
                    }
                }

                effect.Absorbed_Duration = nDuration;
                if (source.RechargeTime > 0 & source.PowerType == Enums.ePowerType.Click)
                {
                    effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                }

                if (nDelay > 0)
                {
                    effect.DelayedTime = nDelay;
                }

                if (effect.Absorbed_Duration > 0 & num2 > 0)
                {
                    effect.nDuration = effect.Absorbed_Duration;
                }

                Effects[num3 + length] = effect;
            }

            return array;
        }

        public void ApplyGrantPowerEffects()
        {
            var flag = true;
            var num1 = 0;
            var num2 = 0;
            if (HasGrantPowerEffect)
            {
                for (; flag & (num1 < 100); ++num1)
                {
                    flag = false;
                    var array1 = new int[0];
                    var array2 = new int[0];
                    for (var index = num2; index < Effects.Length; ++index)
                    {
                        if (Effects[index].EffectType != Enums.eEffectType.GrantPower || !Effects[index].CanGrantPower() || Effects[index].EffectClass == Enums.eEffectClass.Ignored || Effects[index].nSummon <= -1)
                        {
                            continue;
                        }

                        Array.Resize(ref array1, array1.Length + 1);
                        Array.Resize(ref array2, array2.Length + 1);
                        array1[array1.Length - 1] = index;
                        array2[array2.Length - 1] = Effects[index].nSummon;
                    }

                    num2 = Effects.Length;
                    for (var index1 = 0; index1 <= array1.Length - 1; ++index1)
                    {
                        flag = true;
                        Effects[array1[index1]].EffectClass = Enums.eEffectClass.Ignored;
                        var length = Effects.Length;
                        AbsorbEffects(DatabaseAPI.Database.Power[array2[index1]], Effects[array1[index1]].Duration, 0.0f, MidsContext.Archetype, 1, true, array1[index1]);
                        for (var index2 = length; index2 < Effects.Length; ++index2)
                        {
                            if (Effects[array1[index1]].Absorbed_Power_nID > -1)
                            {
                                Effects[index2].Absorbed_PowerType = Effects[array1[index1]].Absorbed_PowerType;
                            }

                            if (Effects[index2].EffectType != Enums.eEffectType.GrantPower)
                            {
                                Effects[index2].ToWho = Effects[array1[index1]].ToWho;
                            }

                            if (Effects[index2].ToWho == Enums.eToWho.All && ((EntitiesAffected & Enums.eEntity.Caster) != Enums.eEntity.Caster || (EntitiesAffected & Enums.eEntity.Friend) != Enums.eEntity.Friend))
                            {
                                Effects[index2].ToWho = Enums.eToWho.Target;
                            }
                            else if (Effects[index2].ToWho == Enums.eToWho.All &&
                                     ((EntitiesAffected & Enums.eEntity.Caster) != Enums.eEntity.Caster ||
                                      (EntitiesAffected & Enums.eEntity.Foe) != Enums.eEntity.Foe))
                            {
                                Effects[index2].ToWho = Enums.eToWho.Target;
                            }

                            Effects[index2].isEnhancementEffect = Effects[array1[index1]].isEnhancementEffect;
                            if (Effects[array1[index1]].Probability < 1.0)
                            {
                                Effects[index2].Probability = Effects[array1[index1]].Probability * Effects[index2].Probability;
                            }
                        }
                    }
                }
            }

            ProcessExecutes();
        }

        public List<int> GetValidEnhancements(Enums.eType iType, int iSubType = 0)
        {
            var intList = new List<int>();
            List<int> allowedEnh;

            switch (iType)
            {
                case Enums.eType.SetO:
                    allowedEnh = GetValidEnhancementsFromSets().ToList();
                    break;
                default:
                    for (var index1 = 0; index1 <= DatabaseAPI.Database.Enhancements.Length - 1; ++index1)
                    {
                        var enhancement1 = DatabaseAPI.Database.Enhancements[index1];
                        if (enhancement1.TypeID != iType)
                        {
                            continue;
                        }

                        var flag = false;
                        foreach (var index2 in enhancement1.ClassID)
                        {
                            foreach (var enhancement2 in Enhancements)
                            {
                                if (DatabaseAPI.Database.EnhancementClasses[index2].ID == enhancement2 && (enhancement1.SubTypeID == 0 || iSubType == 0 || enhancement1.SubTypeID == iSubType))
                                {
                                    flag = true;
                                }
                            }
                        }

                        if (flag)
                        {
                            intList.Add(index1);
                        }
                    }

                    allowedEnh = intList;
                    break;
            }

            return allowedEnh;
        }

        public bool IsEnhancementValid(int iEnh)
        {
            if (iEnh < 0 || iEnh > DatabaseAPI.Database.Enhancements.Length - 1)
            {
                return false;
            }

            return GetValidEnhancements(DatabaseAPI.Database.Enhancements[iEnh].TypeID).Any(validEnhancement => validEnhancement == iEnh);
        }

        public void AbsorbPetEffects(int hIdx = -1, int stackingOverride = -1)
        {
            if (!AbsorbSummonAttributes && !AbsorbSummonEffects)
            {
                return;
            }
            var intList = new List<int>();
            for (var index = 0; index < Effects.Length; ++index)
            {
                if (Effects[index].EffectType == Enums.eEffectType.EntCreate && Effects[index].nSummon > -1 && Math.Abs(Effects[index].Probability - 1f) < 0.01 && DatabaseAPI.Database.Entities.Length > Effects[index].nSummon)
                {
                    intList.Add(index);
                }
            }

            if (intList.Count > 0)
            {
                HasAbsorbedEffects = true;
            }

            foreach (var t in intList)
            {
                var effect = Effects[t];
                var nSummon1 = effect.nSummon;
                var stacking = 1;
                if (VariableEnabled && effect.VariableModified && (hIdx > -1 && MidsContext.Character != null) && MidsContext.Character.CurrentBuild.Powers[hIdx].VariableValue > stacking)
                {
                    stacking = MidsContext.Character.CurrentBuild.Powers[hIdx].VariableValue;
                }

                if (stackingOverride > 0)
                {
                    stacking = stackingOverride;
                }

                var nPowerset = DatabaseAPI.Database.Entities[nSummon1].GetNPowerset();
                if (nPowerset.Count == 0)
                {
                    continue;
                }

                if (AbsorbSummonAttributes && nPowerset[0] > -1 && nPowerset[0] < DatabaseAPI.Database.Powersets.Length)
                {
                    var powerset = DatabaseAPI.Database.Powersets[nPowerset[0]];
                    if (powerset.Power.Length > 0)
                    {
                        foreach (var power in powerset.Powers)
                        {
                            AttackTypes = power.AttackTypes;
                            EffectArea = power.EffectArea;
                            EntitiesAffected = power.EntitiesAffected;
                            if (EntitiesAutoHit != Enums.eEntity.None)
                            {
                                EntitiesAutoHit = power.EntitiesAutoHit;
                            }

                            Ignore_Buff = power.Ignore_Buff;
                            IgnoreEnh = power.IgnoreEnh;
                            MaxTargets = power.MaxTargets;
                            Radius = power.Radius;
                            Target = power.Target;
                            //ActivatePeriod = power.ActivatePeriod;
                            if (DatabaseAPI.Database.Power[PowerIndex].EntitiesAutoHit is Enums.eEntity.None or Enums.eEntity.Caster)
                            {
                                continue;
                            }

                            Accuracy = power.Accuracy;
                            break;
                        }
                    }
                }

                if (!AbsorbSummonEffects)
                {
                    continue;
                }

                foreach (var setIndex in nPowerset)
                {
                    if (setIndex < 0 || setIndex >= DatabaseAPI.Database.Powersets.Length)
                    {
                        continue;
                    }

                    foreach (var power1 in DatabaseAPI.Database.Powersets[setIndex].Powers)
                    {
                        foreach (var absorbEffect in AbsorbEffects(power1, effect.Duration, effect.DelayedTime, DatabaseAPI.Database.Classes[DatabaseAPI.Database.Entities[nSummon1].GetNClassId()], stacking))
                        {
                            var nSummon2 = power1.Effects[absorbEffect].nSummon;
                            if (DatabaseAPI.Database.Entities[nSummon2].GetNPowerset()[0] < 0)
                            {
                                continue;
                            }

                            foreach (var power2 in DatabaseAPI.Database.Powersets[DatabaseAPI.Database.Entities[nSummon2].GetNPowerset()[0]].Powers)
                            {
                                AbsorbEffects(power2, effect.Duration, effect.DelayedTime, DatabaseAPI.Database.Classes[DatabaseAPI.Database.Entities[nSummon1].GetNClassId()], stacking);
                            }
                        }
                    }
                }
            }
        }

        public bool AllowedForClass(int classId)
        {
            //If a power neither requires a class nor excludes one, just return true.
            if (Requires.NClassName.Length == 0 && Requires.NClassNameNot.Length == 0)
            {
                return true;
            }

            //Check if the power has a class requirement.
            if (Requires.NClassName.Length > 0)
            {
                return Requires.NClassName.Contains(classId);
            }

            //Check if the power has a class exclusion.
            if (Requires.NClassNameNot.Length > 0)
            {
                return !Requires.NClassNameNot.Contains(classId);
            }

            return true;
        }

        private bool GreOverride(int iID1, int iID2)
        {
            return (iID1 < 0) & (iID2 > -1) || iID2 >= 0 &&
                (!((Effects[iID1].EffectType == Effects[iID2].EffectType) &
                   (Effects[iID1].ETModifies == Effects[iID2].ETModifies) &
                   (Effects[iID1].MezType == Effects[iID2].MezType)) ||
                 Math.Abs(Effects[iID1].BuffedMag - Effects[iID2].BuffedMag) >= 0.01 && Effects[iID1].ToWho != Effects[iID2].ToWho);
        }

        public static Enums.ShortFX[] SplitFX(ref Enums.ShortFX iSfx, ref IPower? iPower)
        {
            var shortFxArray1 = new Enums.ShortFX[0];
            Enums.ShortFX[] shortFxArray2;
            if (!iSfx.Present)
            {
                shortFxArray2 = shortFxArray1;
            }
            else
            {
                var array = new Enums.ShortFX[1];
                array[0].Add(iSfx.Index[0], iSfx.Value[0]);
                for (var index1 = 1; index1 <= iSfx.Value.Length - 1; ++index1)
                {
                    var index2 = -1;
                    for (var index3 = 0; index3 <= array.Length - 1; ++index3)
                    {
                        if (!(Math.Abs(iSfx.Value[index1] - array[index3].Value[0]) < 0.01) ||
                            !((iPower.Effects[iSfx.Index[index1]].PvMode ==
                               iPower.Effects[array[index3].Index[0]].PvMode) &
                              (iPower.Effects[iSfx.Index[index1]].ToWho ==
                               iPower.Effects[array[index3].Index[0]].ToWho) &
                              (iPower.Effects[iSfx.Index[index1]].Stacking ==
                               iPower.Effects[array[index3].Index[0]].Stacking) &
                              (iPower.Effects[iSfx.Index[index1]].Aspect ==
                               iPower.Effects[array[index3].Index[0]].Aspect) &
                              (iPower.Effects[iSfx.Index[index1]].Buffable ==
                               iPower.Effects[array[index3].Index[0]].Buffable) &
                              (iPower.Effects[iSfx.Index[index1]].Resistible ==
                               iPower.Effects[array[index3].Index[0]].Resistible)))
                        {
                            continue;
                        }

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

        public static string SplitFXGroupTip(ref Enums.ShortFX iSfx, ref IPower? iPower, bool shortForm, bool fromPopup = true)
        {
            var str = iPower.Effects[iSfx.Index[0]].BuildEffectString(false, string.Empty, false, true, false, fromPopup);
            var newValue = string.Empty;
            if (!iPower.Effects[iSfx.Index[0]].isDamage())
            {
                return str.Replace("%VALUE%", newValue);
            }

            var iDamage = new bool[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            for (var index = 0; index <= iSfx.Index.Length - 1; ++index)
            {
                iDamage[(int) iPower.Effects[iSfx.Index[index]].DamageType] = true;
            }

            if (!((iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Defense) | (iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Elusivity)))
            {
                newValue = Enums.GetGroupedDamage(iDamage, shortForm);
            }
            else
            {
                newValue = Enums.GetGroupedDefense(iDamage, shortForm);
            }

            return str.Replace("%VALUE%", newValue);
        }

        /*public static List<string> SplitFXGroupTipL(ref Enums.ShortFX iSfx, ref IPower iPower, bool shortForm)
        {
            var str = iPower.Effects[iSfx.Index[0]].BuildEffectString(false, string.Empty, false, true);
            var newValue = string.Empty;
            //if (!iPower.Effects[iSfx.Index[0]].isDamage())
            //{
            //    return str.Replace("%VALUE%", newValue);
            //}

            var iDamage = new bool[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            for (var index = 0; index <= iSfx.Index.Length - 1; ++index)
            {
                iDamage[(int)iPower.Effects[iSfx.Index[index]].DamageType] = true;
            }

            newValue = !((iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Defense) | (iPower.Effects[iSfx.Index[0]].EffectType == Enums.eEffectType.Elusivity)) ? Enums.GetGroupedDamage(iDamage, shortForm) : Enums.GetGroupedDefense(iDamage, shortForm);
            return str.Replace("%VALUE%", newValue);
        }*/

        private Requirement ImportRequirementString(string iReq)
        {
            Requirement requirement1;
            if (NeverAutoUpdateRequirements)
            {
                requirement1 = Requires;
            }
            else
            {
                var requirement2 = new Requirement();
                if (iReq == null)
                {
                    requirement1 = requirement2;
                }
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
                    for (var index1 = 0; index1 <= 1; ++index1)
                    {
                        var str = "$ARCHETYPE @";
                        if (index1 == 1)
                        {
                            str = "$ARCHTYPE @";
                        }

                        Contains = iReq.Contains(str);
                        for (var index2 = 0; index2 <= DatabaseAPI.Database.Classes.Length - 1; ++index2)
                        {
                            var oldValue1 = str + DatabaseAPI.Database.Classes[index2].ClassName.ToUpper() + " ==";
                            var oldValue2 = oldValue1 + " !";
                            if (iReq.Contains(oldValue2))
                            {
                                Array.Resize(ref requirement2.ClassNameNot, requirement2.ClassNameNot.Length + 1);
                                requirement2.ClassNameNot[requirement2.ClassNameNot.Length - 1] =
                                    DatabaseAPI.Database.Classes[index2].ClassName;
                                iReq = iReq.Replace(oldValue2, "true");
                            }
                            else if (iReq.Contains(oldValue1))
                            {
                                Array.Resize(ref requirement2.ClassName, requirement2.ClassName.Length + 1);
                                requirement2.ClassName[requirement2.ClassName.Length - 1] =
                                    DatabaseAPI.Database.Classes[index2].ClassName;
                                iReq = iReq.Replace(oldValue1, "true");
                            }
                        }

                        if (!Contains)
                        {
                            continue;
                        }

                        {
                            var startIndex = iReq.IndexOf(str, StringComparison.Ordinal);
                            for (var index2 = startIndex + str.Length; index2 <= iReq.Length - 1; ++index2)
                            {
                                if (iReq[index2] != ' ')
                                {
                                    continue;
                                }

                                iReq = iReq.Replace(iReq.Substring(startIndex, index2 - startIndex), "true");
                                break;
                            }

                            iReq = iReq.Replace("true ==", "true");
                            iReq = iReq.Replace("true !", "true");
                        }
                    }

                    var strArray1 = new string[33];
                    var index3 = 0;
                    var strArray2 = iReq.Split(null);
                    for (var index1 = 0; index1 <= strArray2.Length - 1; ++index1)
                    {
                        strArray2[index1] = strArray2[index1].ToLower();
                        switch (strArray2[index1])
                        {
                            case "&&" when index3 > 1 &&
                                           (strArray1[index3 - 1] == "true") & (strArray1[index3 - 2] == "true"):
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case "&&" when index3 > 1 &&
                                           (strArray1[index3 - 1] == "true") & (strArray1[index3 - 2] != "true"):
                                requirement2.AddPowers(strArray1[index3 - 2], string.Empty);
                                --index3;
                                strArray1[index3] = string.Empty;
                                strArray1[index3 - 1] = "true";
                                break;
                            case "&&" when index3 > 1 &&
                                           (strArray1[index3 - 1] != "true") & (strArray1[index3 - 2] == "true"):
                                requirement2.AddPowers(strArray1[index3 - 1], string.Empty);
                                --index3;
                                strArray1[index3] = string.Empty;
                                break;
                            case "&&":
                            {
                                if (index3 > 1 && (strArray1[index3 - 1] != "true") & (strArray1[index3 - 2] != "true"))
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
                                    if ((strArray1[index3 - 1] == "true") & (strArray1[index3 - 2] == "true"))
                                    {
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                        strArray1[index3 - 1] = "true";
                                    }
                                    else if ((strArray1[index3 - 1] != "true") & (strArray1[index3 - 2] == "true"))
                                    {
                                        requirement2.AddPowers(strArray1[index3 - 1], string.Empty);
                                        --index3;
                                        strArray1[index3] = string.Empty;
                                    }
                                    else if ((strArray1[index3 - 1] == "true") & (strArray1[index3 - 2] != "true"))
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
                                            {
                                                strArray2[index1 + 1] = string.Empty;
                                            }

                                            strArray1[index3] = "true";
                                            ++index3;
                                            break;
                                        }
                                        case "isarchitectmap?":
                                        {
                                            if (index1 < strArray2.GetUpperBound(0) && strArray2[index1 + 1] == "!")
                                            {
                                                strArray2[index1 + 1] = string.Empty;
                                            }

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

                    if (index3 != 0 && (index3 > 1) | (strArray1[0] != "true"))
                    {
                        var str = "Tokens remain in the stack (this can cause problems): \n";
                        for (var index1 = 0; index1 <= index3; ++index1)
                            str = str + strArray1[index1] + " ";
                        var num = (int) MessageBox.Show(str + "\n\niReq: " + iReq +
                                                        "\n\nSee clsPowerV2/ImportRequirementString to tweak.");
                    }

                    for (var index1 = 0; index1 <= requirement2.PowerID.Length - 1; ++index1)
                    for (var index2 = 0; index2 <= requirement2.PowerID[index1].Length - 1; ++index2)
                    {
                        if (string.IsNullOrEmpty(requirement2.PowerID[index1][index2]))
                        {
                            continue;
                        }

                        for (var index4 = 0; index4 <= DatabaseAPI.Database.Power.Length - 1; ++index4)
                        {
                            if (!string.Equals(DatabaseAPI.Database.Power[index4].FullName,
                                requirement2.PowerID[index1][index2],
                                StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            requirement2.PowerID[index1][index2] = DatabaseAPI.Database.Power[index4].FullName;
                            break;
                        }
                    }

                    requirement1 = requirement2;
                }
            }

            return requirement1;
        }

        private int[] GetValidEnhancementsFromSets()
        {
            var intList = new List<int>();
            foreach (var enhancementSet in DatabaseAPI.Database.EnhancementSets)
            {
                var flag = SetTypes.Any(setType => enhancementSet.SetType == setType);
                if (!flag)
                {
                    continue;
                }

                intList.AddRange(enhancementSet.Enhancements);
            }

            return intList.ToArray();
        }

        public void ProcessExecutes()
        {
            ProcessExecutesInner(this);
            foreach (var fx in Effects)
            {
                fx.SetPower(this);
            }
        }

        public List<IEffect>? ProcessExecutesInner(IPower? power = null, int rLevel = 0)
        {
            // Max recursion level
            if (rLevel > 5)
            {
                return new List<IEffect>();
            }

            power = power ??= this;
            var pEffects = new List<IEffect>();
            var k = 0;

            foreach (var fx in power.Effects)
            {
                if (fx.EffectType != Enums.eEffectType.ExecutePower)
                {
                    pEffects.Add(fx.Clone<IEffect>());
                    k++;
                    continue;
                }

                if (string.IsNullOrEmpty(fx.Summon))
                {
                    continue;
                }

                var fxPower = DatabaseAPI.GetPowerByFullName(fx.Summon);
                if (fxPower == null)
                {
                    continue;
                }

                var subEffects = ProcessExecutesInner(fxPower, rLevel + 1);
                if (subEffects == null)
                {
                    continue;
                }

                pEffects.AddRange(subEffects.Select(t => t.Clone<IEffect>()));

                for (var j = k; j < k + subEffects.Count; j++)
                {
                    var sFx = pEffects[j];
                    
                    // Cap effect duration to root power
                    sFx.nDuration = fx.nDuration > 0
                        ? Math.Min(sFx.nDuration, fx.nDuration)
                        : sFx.nDuration;
                    sFx.DelayedTime += fx.DelayedTime;
                    sFx.Probability = fx.Probability is 0 or 1
                        ? sFx.Probability
                        : Math.Min(sFx.Probability, fx.Probability);
                    sFx.ProcsPerMinute = fx.ProcsPerMinute;
                    if (fx.ActiveConditionals is { Count: > 0 })
                    {
                        sFx.ActiveConditionals?.AddRange(fx.ActiveConditionals);
                    }

                    if (fx.Ticks > 0 && sFx.Ticks == 0)
                    {
                        sFx.Ticks = fx.Ticks;
                    }
                }

                k += subEffects.Count;
            }

            if (rLevel > 0)
            {
                return pEffects;
            }

            Effects = pEffects.ToArray();
            
            return null;
        }

        /// <summary>
        /// Build effects tooltip matching effectType from current power.
        /// If power holds all vectors from effectType and all have the same <see cref="EffectIdentifier"/> key,
        /// then it will show for the stat name groupName instead.
        /// If effect holds Any, PvE and PvP effects (at least 2 of these), display will be separated in PvX groups.
        /// If effective vectors have less (or equal) than 3 members, it will show up as "All but ..." instead.
        /// </summary>
        /// <param name="effectType">Type of effect to match against</param>
        /// <param name="groupName">Group name to display for "All" effects. Usually it is effectType + " (All)" - this is the default value if set to blank.</param>
        /// <param name="includeEnhEffects">Include effects from enhancements (if any). Default is false.</param>
        /// <remarks>For Defense, it will include Toxic defense if necessary <seealso cref="DatabaseAPI.RealmUsesToxicDef"/></remarks>
        /// <returns>Tooltip string for the power effects</returns>
        /// <example>
        /// BuildTooltipStringAllVectorsEffects(Enums.eEffectType.Resistance) on Shriek:
        /// -9% Resistance (All) to Target for 8 seconds [Ignore Enhancements & Buffs]
        /// Effect does not stack from same caster
        /// -6% Resistance (All) to Target for 10 seconds [Ignore Enhancements & Buffs]
        /// Effect does not stack from same caster
        /// </example>
        /// <example>
        /// BuildTooltipStringAllVectorsEffects(Enums.eEffectType.Resistance) on Steamy Mist:
        /// 23.43% Resistance (Fire, Cold, Energy) to Self for 2.25 seconds (when Res_Dmg)
        ///   Effect does not stack from same caster
        ///   Suppressed when Mezzed.
        /// </example>
        /// <example>
        /// BuildTooltipStringAllVectorsEffects(Enums.eEffectType.Defense) on Combat Jumping:
        /// 3.12% Defense (All) to Self for 0.75 seconds (when Buff_Def, in PvE)
        ///   Effect does not stack from same caster
        ///   Suppressed when Mezzed.
        /// ---------------------
        /// 3.12% Defense (All) to Self for 0.75 seconds (when Buff_Def, in PvP)
        ///   Effect does not stack from same caster
        /// </example>
        /// <example>
        /// BuildTooltipStringAllVectorsEffects(Enums.eEffectType.Resistance) on Dark Embrace:
        /// 31.34% Resistance (Smashing, Lethal) to Self for 0.75 seconds (when Res_Dmg, in PvE)
        ///   Effect does not stack from same caster
        ///   Suppressed when Mezzed.
        /// 17.09% Resistance (Toxic, Negative) to Self for 0.75 seconds (when Res_Dmg, in PvE)
        ///   Effect does not stack from same caster
        ///   Suppressed when Mezzed.
        /// </example>
        /// <example>
        /// BuildTooltipStringAllVectorsEffects(Enums.eEffectType.Defense) on Weave:
        /// 6.68% Defense (All but Toxic) to Self for 0.75 seconds (when Buff_Def, in PvE)
        ///   Effect does not stack from same caster
        ///   Suppressed when Mezzed.
        /// ---------------------
        /// 6.68% Defense (All but Toxic) to Self for 0.75 seconds (when Buff_Def, in PvP)
        ///   Effect does not stack from same caster
        /// </example>
        public string BuildTooltipStringAllVectorsEffects(Enums.eEffectType effectType, string groupName = "", bool includeEnhEffects = false)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = $"{effectType} (All)";
            }

            var damageVectors = effectType switch
            {
                Enums.eEffectType.Resistance or Enums.eEffectType.DamageBuff => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic
                },

                Enums.eEffectType.Defense when !DatabaseAPI.RealmUsesToxicDef() => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Psionic,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                },

                _ => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                }
            };

            var effectsList = string.Empty;
            var pvModes = new List<Enums.ePvX> { Enums.ePvX.Any, Enums.ePvX.PvE, Enums.ePvX.PvP };

            foreach (var pvMode in pvModes)
            {
                // Select identifiers from effects
                var effectIdentifiers2 = Effects
                    .Where(e => e.EffectType == effectType && e.PvMode == pvMode && (includeEnhEffects || !e.isEnhancementEffect))
                    .Select(e => e.GenerateIdentifier())
                    .ToList();

                // Distinct() with custom comparer
                var effectIdentifiers = new List<EffectIdentifier>();
                foreach (var fxId2 in effectIdentifiers2)
                {
                    if (effectIdentifiers.Any(e => e.Compare(fxId2)))
                    {
                        continue;
                    }

                    effectIdentifiers.Add(fxId2);
                }

                var pvxEffects = new List<string>();
                foreach (var effectId in effectIdentifiers)
                {
                    // Select distinct damage vectors from effects of the specified kind
                    var effectsVectors = Effects
                        .Where(e => (includeEnhEffects || !e.isEnhancementEffect) & e.GenerateIdentifier().Compare(effectId))
                        .Select(e => e.DamageType)
                        .Distinct()
                        .ToList();

                    // Check if effects contains all the effectType vectors (listed above)
                    var effectsInMode = string.Empty;
                    if (!damageVectors.Except(effectsVectors).Any())
                    {
                        var effectsInModeBase = Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .First(e => (includeEnhEffects || !e.Value.isEnhancementEffect) & e.Value.GenerateIdentifier().Compare(effectId));
                            
                            effectsInMode = effectsInModeBase.Value.BuildEffectString(false, groupName, false, false, false, true) + GetDifferentAttributesSubPower(effectsInModeBase.Key);
                    }
                    else
                    {
                        // If 3 or less vectors are excluded, use the "All but..." format instead and only list those non-matched
                        var vectors = (damageVectors.Count - effectsVectors.Count <= 3)
                            ? $"All but {string.Join(", ", damageVectors.Except(effectsVectors))}"
                            : string.Join(", ", effectsVectors);

                        // Pick all matching effects
                        var effectsInModeBase = Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .First(e => (includeEnhEffects || !e.Value.isEnhancementEffect) & e.Value.GenerateIdentifier().Compare(effectId));
                            
                            effectsInMode = effectsInModeBase.Value.BuildEffectString(false, $"{effectType} ({vectors})", false, false, false, true) + GetDifferentAttributesSubPower(effectsInModeBase.Key);
                    }

                    effectsInMode = effectsInMode.Trim().Replace("\n\n", "\n");
                    if (string.IsNullOrEmpty(effectsInMode))
                    {
                        continue;
                    }

                    pvxEffects.Add(effectsInMode);
                }

                if (pvxEffects.Count <= 0)
                {
                    continue;
                }
                
                effectsList += (!string.IsNullOrEmpty(effectsList) ? "\n---------------------\n" : "") + string.Join("\n", pvxEffects);
            }

            return effectsList;
        }

        public string BuildTooltipStringAllVectorsEffects(Enums.eEffectType effectType, Enums.eEffectType etModifies, Enums.eDamage damageType, Enums.eMez mezType, string groupName = "", bool includeEnhEffects = false)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = $"{effectType} (All)";
            }

            var damageVectors = (effectType == Enums.eEffectType.Enhancement ? etModifies : effectType) switch
            {
                Enums.eEffectType.Resistance or Enums.eEffectType.DamageBuff => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic
                },

                Enums.eEffectType.Defense when !DatabaseAPI.RealmUsesToxicDef() => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Psionic,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                },

                _ => new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Psionic,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                }
            };

            var effectsList = string.Empty;
            var pvModes = new List<Enums.ePvX> { Enums.ePvX.Any };

            foreach (var pvMode in pvModes)
            {
                // Select identifiers from effects
                var effectIdentifiers2 = Effects
                    .Where(e => e.EffectType == effectType && e.ETModifies == etModifies &&
                                e.MezType == mezType && e.DamageType == damageType
                                && e.PvMode == pvMode && (includeEnhEffects || !e.isEnhancementEffect))
                    .Select(e => e.GenerateIdentifier())
                    .ToList();

                // Distinct() with custom comparer
                var effectIdentifiers = new List<EffectIdentifier>();
                foreach (var fxId2 in effectIdentifiers2)
                {
                    if (effectIdentifiers.Any(e => e.Compare(fxId2)))
                    {
                        continue;
                    }

                    effectIdentifiers.Add(fxId2);
                }

                var pvxEffects = new List<string>();
                foreach (var effectId in effectIdentifiers)
                {
                    // Select distinct damage vectors from effects of the specified kind
                    var effectsVectors = Effects
                        .Where(e => (includeEnhEffects || !e.isEnhancementEffect) & e.GenerateIdentifier().Compare(effectId))
                        .Select(e => e.DamageType)
                        .Distinct()
                        .ToList();

                    // Check if effects contains all the effectType vectors (listed above)
                    var effectsInMode = string.Empty;
                    if (!damageVectors.Except(effectsVectors).Any())
                    {
                        var effectsInModeBase = Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .First(e => (includeEnhEffects || !e.Value.isEnhancementEffect) & e.Value.GenerateIdentifier().Compare(effectId));
                            
                        effectsInMode = effectsInModeBase.Value.BuildEffectString(false, groupName, false, false, false, true) + GetDifferentAttributesSubPower(effectsInModeBase.Key);
                    }
                    else
                    {
                        // If 3 or less vectors are excluded, use the "All but..." format instead and only list those non-matched
                        var vectors = (damageVectors.Count - effectsVectors.Count <= 3)
                            ? $"All but {string.Join(", ", damageVectors.Except(effectsVectors))}"
                            : string.Join(", ", effectsVectors);

                        // Pick all matching effects
                        var fxLabel = effectType switch
                        {
                            Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect => $"{effectType}({etModifies})",
                            Enums.eEffectType.Mez or Enums.eEffectType.MezResist => $"{effectType}({mezType})",
                            _ => $"{effectType} ({vectors})"
                        };

                        var effectsInModeBase = Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .First(e => (includeEnhEffects || !e.Value.isEnhancementEffect) & e.Value.GenerateIdentifier().Compare(effectId));

                        effectsInMode = effectsInModeBase.Value.BuildEffectString(false, $"{fxLabel}", false, false, false, true) + GetDifferentAttributesSubPower(effectsInModeBase.Key);
                    }

                    effectsInMode = effectsInMode.Trim().Replace("\n\n", "\n");
                    if (string.IsNullOrEmpty(effectsInMode))
                    {
                        continue;
                    }

                    pvxEffects.Add(effectsInMode);
                }

                if (pvxEffects.Count <= 0)
                {
                    continue;
                }

                effectsList += (!string.IsNullOrEmpty(effectsList) ? "\n---------------------\n" : "") + string.Join("\n", pvxEffects);
            }

            return effectsList;
        }

        /// <summary>
        /// Fetch summoned entities powerset info.
        /// For each EntCreate effect, will get the summoned entity nId, how many powers there are in its first associated powerset,
        /// and for each power in it, its full name and how many effects this power holds.
        /// Finally it will return effects indexes and the source power from the entity powerset.
        /// </summary>
        /// <returns>Dictionary of (effect index => entity power full name)</returns>
        /// <remarks>Powers from entities are added in sequence. So if first power has 3 effects, and the EntCreate is #6, effects #7, #8 and #9 will belong to this first power.</remarks>
        public Dictionary<int, string> GetEffectsInSummons()
        {
            var powerSummons = new Dictionary<int, KeyValuePair<KeyValuePair<int, int>, Dictionary<string, int>>>();
            
            // May crash if done in a single pass
            var powerSummonsEffects = Effects
                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                .Where(e => AbsorbSummonEffects & AbsorbSummonAttributes &
                            e.Value.EffectType == Enums.eEffectType.EntCreate & e.Value.nSummon > -1 &
                            e.Value.nSummon < DatabaseAPI.Database.Entities.Length)
                .ToList();

            powerSummonsEffects = powerSummonsEffects
                .Where(e => DatabaseAPI.Database.Entities[e.Value.nSummon].GetNPowerset().Count > 0)
                .ToList();

            // Dictionary(index of EntCreate effect => KeyValuePair(KeyValuePair(entityPowerset[0].nId, entityPowerset[0].Powers.Length), Dictionary(entityPowerset[0].Powers[n].FullName, entityPowerset[0].Powers[n].Effects.Length)))
            powerSummons = powerSummonsEffects
                .ToDictionary(e => e.Key, e =>
                    new KeyValuePair<KeyValuePair<int, int>, Dictionary<string, int>>(
                        new KeyValuePair<int, int>(
                            DatabaseAPI.Database.Entities[e.Value.nSummon].GetNPowerset()[0],
                            DatabaseAPI.Database
                                .Powersets[DatabaseAPI.Database.Entities[e.Value.nSummon].GetNPowerset()[0]]
                                .Powers.Length),
                        DatabaseAPI.Database
                            .Powersets[DatabaseAPI.Database.Entities[e.Value.nSummon].GetNPowerset()[0]].Powers
                            .Where(p => p != null)
                            .ToDictionary(p => p.FullName, p => p.Effects.Length)));
            
            var effectsInSummons = new Dictionary<int, string>();
            var k = 0;
            foreach (var ps in powerSummons)
            {
                var baseIndex = ps.Key;
                var powers = ps.Value.Value;
                foreach (var p in powers)
                {
                    for (var i = 0; i < p.Value; i++)
                    {
                        var pIndex = baseIndex + 1 + i + k;
                        if (!effectsInSummons.ContainsKey(pIndex))
                        {
                            effectsInSummons.Add(pIndex, p.Key);
                        }
                    }

                    k += p.Value;
                }
            }

            return effectsInSummons;
        }

        /// <summary>
        /// If effect is from an absorbed entity, check which attribute differs and return a comma-separated string of them, with an extra starting comma.
        /// Returns empty string if no difference.
        /// </summary>
        /// <param name="fxIndex">Index of effect in basePower</param>
        /// <returns>String containing value that differs from host power. Values listed will be those from the entity.</returns>
        /// <remarks>Will only check for these attributes: Range, Secondary Range, Radius, Arc, Max Targets.</remarks>
        public string GetDifferentAttributesSubPower(int fxIndex)
        {
            if (fxIndex <= -1)
            {
                return "";
            }

            var effectsInSummons = GetEffectsInSummons();
            if (!effectsInSummons.ContainsKey(fxIndex))
            {
                return "";
            }

            var extraAttribs = new List<string>();
            var subPower = DatabaseAPI.GetPowerByFullName(effectsInSummons[fxIndex]);
            if (subPower == null)
            {
                return "";
            }

            if (Math.Abs(subPower.Range - Range) > float.Epsilon && subPower.Range > float.Epsilon)
            {
                extraAttribs.Add($"Range: {subPower.Range:###0.##}ft");
            }

            if (Math.Abs(subPower.RangeSecondary - RangeSecondary) > float.Epsilon && subPower.RangeSecondary > float.Epsilon)
            {
                extraAttribs.Add($"Secondary Range: {subPower.RangeSecondary:###0.##}ft");
            }

            if (Math.Abs(subPower.Radius - Radius) > float.Epsilon && subPower.Radius > float.Epsilon)
            {
                extraAttribs.Add($"Radius: {subPower.Radius:###0.##}ft");
            }

            if (subPower.Arc != Arc && subPower.Arc > float.Epsilon)
            {
                extraAttribs.Add($"Arc: {subPower.Arc:###0.##}deg");
            }

            if (subPower.MaxTargets != MaxTargets && subPower.MaxTargets > 0)
            {
                extraAttribs.Add($"Max Targets: {subPower.MaxTargets}");
            }

            return extraAttribs.Count <= 0
                ? ""
                : $", {string.Join(", ", extraAttribs)}";
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
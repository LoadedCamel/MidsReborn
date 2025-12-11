using FastDeepCloner;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Power : IPower
    {
        private bool Contains;
        public bool AbsorbedPetEffects { get; set; }
        public bool AppliedExecutes { get; set; }

        public Power()
        {
            DescLong = string.Empty;
            DescShort = string.Empty;
            Enhancements = [];
            MaxBoosts = string.Empty;
            DisplayName = string.Empty;
            FullName = string.Empty;
            BoostsAllowed = [];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = [];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = [];
            PowerName = string.Empty;
            SetName = string.Empty;
            GroupName = string.Empty;
            NGroupMembership = [];
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            SetTypes = [];
            VariableName = string.Empty;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
            SubIsAltColor = false;
            BoostsAllowed = [];
            Requires = new Requirement();
            var num = -2;
            foreach (var p in DatabaseAPI.Database.Power)
            {
                if (p is {StaticIndex: > -1} && p.StaticIndex > num)
                {
                    num = p.StaticIndex;
                }
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
            Enhancements = [];
            MaxBoosts = string.Empty;
            DisplayName = string.Empty;
            FullName = string.Empty;
            BoostsAllowed = [];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = [];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = [];
            Requires = new Requirement();
            PowerName = string.Empty;
            SetName = string.Empty;
            GroupName = string.Empty;
            NGroupMembership = [];
            StaticIndex = -1;
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            //SetTypes = new Enums.eSetType[0];
            SetTypes = [];
            VariableName = string.Empty;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
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
            for (var index = 0; index < Effects.Length; index++)
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
            Enhancements = [];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = [];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = [];
            Requires = new Requirement();
            NGroupMembership = [];
            StaticIndex = -1;
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;

            //SetTypes = new Enums.eSetType[0];

            SetTypes = [];

            VariableName = string.Empty;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
            SubIsAltColor = false;
            BoostsAllowed = [];
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
            {
                GroupMembership[index] = reader.ReadString();
            }

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
            {
                BoostsAllowed[index] = reader.ReadString();
            }

            CastThroughHold = reader.ReadBoolean();
            IgnoreStrength = reader.ReadBoolean();
            DescShort = reader.ReadString();
            DescLong = reader.ReadString();
            Enhancements = new int[reader.ReadInt32() + 1];
            for (var index = 0; index <= Enhancements.Length - 1; ++index)
            {
                Enhancements[index] = reader.ReadInt32();
            }

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
            for (var index = 0; index < UIDSubPower.Length; index++)
            {
                UIDSubPower[index] = reader.ReadString();
            }

            IgnoreEnh = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (var index = 0; index < IgnoreEnh.Length; index++)
            {
                IgnoreEnh[index] = (Enums.eEnhance)reader.ReadInt32();
            }

            Ignore_Buff = new Enums.eEnhance[reader.ReadInt32() + 1];
            for (var index = 0; index < Ignore_Buff.Length; index++)
            {
                Ignore_Buff[index] = (Enums.eEnhance)reader.ReadInt32();
            }

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
            for (var index = 0; index < Effects.Length; index++)
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

            return null;
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

        public bool HasEntity
        {
            get
            {
                return Effects.Any(effect => effect.nSummon > -1);
            }
        }

        public string FullSetName
        {
            get
            {
                var strArray = FullName.Split('.');
                return strArray.Length <= 1 ? string.Empty : $"{strArray[0]}.{strArray[1]}";
            }
        }

        public float CastTime
        {
            get => !MidsContext.Config.UseArcanaTime
                ? CastTimeReal
                : (float) (Math.Ceiling(CastTimeReal / 0.132f) + 1) * 0.132f;
            set => CastTimeReal = value;
        }

        public float CastTimeBase => CastTimeReal;

        public float ArcanaCastTime => (float) (Math.Ceiling(CastTimeReal / 0.132f) + 1) * 0.132f;

        public bool Slottable
        {
            get
            {
                var ps = GetPowerSet();
                return Enhancements.Length > 0 && ps is {SetType: Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent or Enums.ePowerSetType.Pool};
            }
        }

        public float AoEModifier => EffectArea != Enums.eEffectArea.Cone
            ? EffectArea != Enums.eEffectArea.Sphere ? 1 : (float) (1 + Radius * 0.150000005960464)
            : (float) (1 + Radius * 0.150000005960464 - Radius * 0.000366669992217794 * (360 - Arc));

        public int DisplayLocation
        {
            get => LocationIndex;
            set => LocationIndex = value;
        }

        public bool HasMutexID(int index)
        {
            return NGroupMembership.Any(t => t == index);
        }

        public bool IsSummonPower => Effects.Any(x => x.EffectType is Enums.eEffectType.EntCreate);
        public bool IsPetPower => FullName.StartsWith("Pets") || FullName.StartsWith("Mastermind_Pets") || FullName.StartsWith("Villain_Pets") || FullName.StartsWith("Kheldian_Pets");

        public int ParentIdx { get; set; } = -1;

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

        public string DescLongFormatted
        {
            get
            {
                var cfgSettings = ConfigData.GetCombatSettings();
                var formattedDesc = DescLong.Replace("  ", " ").Trim();
                var r = new Regex(@"\{link\:([a-zA-Z0-9\.\-_]+)\}");
                var g = r.Matches(formattedDesc)
                    .Select(e => e.Groups[1].Value)
                    .Where(e => cfgSettings.ContainsKey(e))
                    .ToList();

                if (g.Count == 0)
                {
                    return formattedDesc;
                }

                formattedDesc = cfgSettings.Aggregate(formattedDesc, (current, k) => current.Replace($"{{link:{k.Key}}}", "", StringComparison.InvariantCultureIgnoreCase));
                // If power has a slider, consider the first found variable as the primary key
                // that will match the slider's one.
                if (VariableEnabled)
                {
                    formattedDesc = $"{formattedDesc.Trim()} Mids' note: you can use the {ConfigData.GetCombatSettingName(g[0], cfgSettings)} setting in the Combat Settings window to change the behavior of this power.";
                
                    if (g.Count > 1)
                    {
                        formattedDesc += $" It is also affected by {string.Join(", ", g.Skip(1).Select(e => ConfigData.GetCombatSettingName(e, cfgSettings)))}.";
                    }
                }
                else
                {
                    // None, just list what it is affected by.
                    formattedDesc = $"{formattedDesc.Trim()} Mids' note: you can use the Combat Settings window to change the behavior of this power. It is affected by {string.Join(", ", g.Select(e => ConfigData.GetCombatSettingName(e, cfgSettings)))}";
                }

                return formattedDesc;
            }
        }

        public string? CSPrimaryKey
        {
            get
            {
                var cfgSettings = ConfigData.GetCombatSettings();
                var r = new Regex(@"\{{link\:([a-zA-Z0-9\.\-_]+)\}}");
                var g = r.Matches(DescLong)
                    .Select(e => e.Groups[1].Value)
                    .Where(e => cfgSettings.ContainsKey(e))
                    .ToList();

                return g.Count == 0 ? null : g[0];
            }
        }

        public List<string> CSKeys
        {
            get
            {
                var cfgSettings = ConfigData.GetCombatSettings();
                var r = new Regex(@"\{{link\:([a-zA-Z0-9\.\-_]+)\}}");
                return r.Matches(DescLong)
                    .Select(e => e.Groups[1].Value)
                    .Where(e => cfgSettings.ContainsKey(e))
                    .ToList();
            }
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
            foreach (var sp in UIDSubPower)
            {
                writer.Write(sp);
            }

            writer.Write(IgnoreEnh.Length - 1);
            foreach (var ie in IgnoreEnh)
            {
                writer.Write((int) ie);
            }

            writer.Write(Ignore_Buff.Length - 1);
            foreach (var ib in Ignore_Buff)
            {
                writer.Write((int) ib);
            }

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
            foreach (var fx in Effects)
            {
                fx.StoreTo(ref writer);
            }

            writer.Write(HiddenPower);
            writer.Write(Active);
            writer.Write(Taken);
            writer.Write(Stacks);
            writer.Write(VariableStart);
        }

        public List<SummonedEntity>? GetEntities()
        {
            if (!IsSummonPower)
                return null;
            return (from effect in Effects
                where effect.EffectType is Enums.eEffectType.EntCreate && effect.nSummon > -1
                select DatabaseAPI.Database.Entities[effect.nSummon]).ToList();
        }

        public float FXGetDamageValue(bool absorb = false)
        {
            var totalDamage = 0f;
            IPower power = new Power(this);
            if (absorb)
            {
                power.AbsorbPetEffects();
            }

            if (!power.AppliedExecutes)
            {
                power.ProcessExecutes();
            }

            foreach (var effect in power.Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum && !(Math.Abs(effect.Probability) > 0.999000012874603) ||
                    effect.EffectClass == Enums.eEffectClass.Ignored || effect is {DamageType: Enums.eDamage.Special, ToWho: Enums.eToWho.Self} || effect.Probability <= 0 || !effect.CanInclude() ||
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
                    if (power is {PowerType: Enums.ePowerType.Toggle, ActivatePeriod: > 0})
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
                    if (power is {PowerType: Enums.ePowerType.Toggle, ActivatePeriod: > 0})
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

        public string GetDamageTip()
        {
            var tip = string.Empty;
            var hasSpecialEnhFx = -1;
            var includedFxForToggle = -1;
            var hasPvePvpEffect = 0;
            var damageTotals = new Dictionary<Enums.eDamage, float>();

            if (Effects.Length <= 0)
            {
                return "";
            }

            foreach (var effect in Effects)
            {
                if (effect.EffectType != Enums.eEffectType.Damage)
                {
                    continue;
                }

                if (effect.CanInclude() & effect.PvXInclude() & Math.Abs(effect.BuffedMag) >= 0.0001)
                {
                    if (tip != string.Empty)
                    {
                        tip += "\r\n";
                    }

                    var str = effect.BuildEffectString(false, "", false, false, false, false, false, true);
                    if (effect.EffectType == Enums.eEffectType.Damage)
                    {
                        var fxDmg = effect.GetDamage();
                        if (fxDmg.Type != Enums.eDamage.None & fxDmg.Value > float.Epsilon)
                        {
                            if (damageTotals.ContainsKey(fxDmg.Type))
                            {
                                damageTotals[fxDmg.Type] += fxDmg.Value;
                            }
                            else
                            {
                                damageTotals.Add(fxDmg.Type, fxDmg.Value);
                            }
                        }
                    }
                    
                    if (effect.isEnhancementEffect & PowerType == Enums.ePowerType.Toggle)
                    {
                        hasSpecialEnhFx++;
                        str += " (Special, only every 10s)";
                    }
                    else if (PowerType == Enums.ePowerType.Toggle)
                    {
                        includedFxForToggle++;
                    }

                    tip += str;
                }
                else
                {
                    hasPvePvpEffect++;
                }
            }

            if (hasPvePvpEffect > 0)
            {
                if (tip != string.Empty)
                {
                    tip += "\r\n";
                }

                tip += "\r\nThis power deals different damage in PvP and PvE modes.";
            }

            if (!(PowerType == Enums.ePowerType.Toggle & hasSpecialEnhFx == -1 & includedFxForToggle == -1) && PowerType == Enums.ePowerType.Toggle & includedFxForToggle > -1 && !string.IsNullOrEmpty(tip))
            {
                tip = $"Applied every {ActivatePeriod} s:\r\n{tip}";
            }

            if (damageTotals.Count > 0)
            {
                tip += $"\r\n\r\nTotal: {damageTotals.Sum(e => e.Value):####0.##} ({string.Join(", ", damageTotals.Select(e => $"{e.Key}: {e.Value:####0.##}"))})";
            }

            return tip;
        }

        public string FXGetDamageString(bool absorb = false)
        {
            // Get the names of all damage types
            var damageTypeNames = Enum.GetNames(typeof(Enums.eDamage));

            // Arrays to store damage values and tick counts
            var totalDamageArray = new float[damageTypeNames.Length];
            var tickDamageArray = new float[damageTypeNames.Length, 2];
            var tickCountArray = new float[damageTypeNames.Length, 2];

            // String to store the resulting damage string
            var damageString = string.Empty;

            // Variable to store total calculated damage
            var totalDamage = 0f;

            // Initialize the power object
            IPower power = new Power(this);

            // Absorb pet effects if requested
            if (absorb)
            {
                power.AbsorbPetEffects();
            }

            // Check if any effects display percentage damage or have Strength aspect
            var hasPercentDamage = power.Effects
                .Any(e => e.EffectType == Enums.eEffectType.Damage && (e.DisplayPercentage || e.Aspect == Enums.eAspect.Str));

            // Iterate through each effect in the power
            foreach (var effect in power.Effects)
            {
                // Skip effects that do not meet various conditions
                if (effect.EffectType != Enums.eEffectType.Damage ||
                    MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum &&
                    !(Math.Abs(effect.Probability) > 0.999000012874603) ||
                    effect.EffectClass == Enums.eEffectClass.Ignored ||
                    effect is { DamageType: Enums.eDamage.Special, ToWho: Enums.eToWho.Self } ||
                    !(effect.Probability > 0) || !effect.CanInclude() || !effect.PvXInclude())
                {
                    continue;
                }

                // Get the absolute magnitude of the effect
                var effectMagnitude = Math.Abs(effect.BuffedMag);

                // Adjust for average damage calculation
                if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
                {
                    effectMagnitude *= effect.Probability;
                }

                // Further adjust for toggle powers with enhancement effects
                if (power.PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
                {
                    effectMagnitude = (float)(effectMagnitude * power.ActivatePeriod / 10d);
                }

                // Skip negligible effects
                if (Math.Abs(effectMagnitude) < 0.0001)
                {
                    continue;
                }

                // Adjust effect magnitude for DPS or DPA based on power type and configuration
                switch (MidsContext.Config.DamageMath.ReturnValue)
                {
                    case ConfigData.EDamageReturn.DPS:
                        if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                        {
                            effectMagnitude /= power.ActivatePeriod;
                            break;
                        }

                        if (power.RechargeTime + (double)power.CastTime > 0)
                        {
                            effectMagnitude /= power.RechargeTime + power.CastTime;
                        }

                        break;
                    case ConfigData.EDamageReturn.DPA:
                        if (power.PowerType == Enums.ePowerType.Toggle && power.ActivatePeriod > 0)
                        {
                            effectMagnitude /= power.ActivatePeriod;
                            break;
                        }

                        if (power.CastTime > 0)
                        {
                            effectMagnitude /= power.CastTime;
                        }

                        break;
                }

                // Handle effects with ticks
                if (effect.Ticks != 0)
                {
                    var effectiveTicks = !effect.CancelOnMiss ||
                                         MidsContext.Config.DamageMath.Calculate != ConfigData.EDamageMath.Average ||
                                         effect.Probability >= 1
                        ? effect.Ticks
                        : (float)((1 - Math.Pow(effect.Probability, effect.Ticks)) / (1 - effect.Probability));

                    var index = 0;
                    if (Math.Abs(tickDamageArray[(int)effect.DamageType, 0]) > 0.01)
                    {
                        index = 1;
                    }

                    tickDamageArray[(int)effect.DamageType, index] = effectMagnitude;
                    tickCountArray[(int)effect.DamageType, index] = effectiveTicks;
                    totalDamage += effectMagnitude * effectiveTicks;
                }
                else
                {
                    totalDamage += effectMagnitude;
                    totalDamageArray[(int)effect.DamageType] += effectMagnitude;
                }
            }

            // Return "0" if total damage is negligible
            if (Math.Abs(totalDamage) < 0.0001)
            {
                return "0";
            }

            // Construct the damage string for each damage type
            for (var index = 0; index < totalDamageArray.Length; index++)
            {
                if (!(totalDamageArray[index] > 0 || tickDamageArray[index, 0] > 0))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(damageString))
                {
                    damageString += ", ";
                }

                var damageEntry = $"{Enums.GetDamageName((Enums.eDamage)index)}(";
                if (totalDamageArray[index] > 0)
                {
                    damageEntry += hasPercentDamage
                        ? $"{Utilities.FixDP(totalDamageArray[index] * 100)}%"
                        : Utilities.FixDP(totalDamageArray[index]);
                }

                if (Math.Abs(tickDamageArray[index, 0]) > 0.01)
                {
                    if (totalDamageArray[index] > 0)
                    {
                        damageEntry += "+";
                    }

                    damageEntry +=
                        $"{(hasPercentDamage ? $"{Utilities.FixDP(tickDamageArray[index, 0] * 100)}%" : Utilities.FixDP(tickDamageArray[index, 0]))}x{Utilities.FixDP(tickCountArray[index, 0])}";
                    if (Math.Abs(tickDamageArray[index, 1]) > 0.01)
                    {
                        damageEntry +=
                            $"+{(hasPercentDamage ? $"{Utilities.FixDP(tickDamageArray[index, 1] * 100)}%" : Utilities.FixDP(tickDamageArray[index, 1]))}x{Utilities.FixDP(tickCountArray[index, 1])}";
                    }
                }

                damageString += $"{damageEntry})";
            }

            // Return the final formatted damage string with total damage
            return
                $"{damageString} = {(hasPercentDamage ? $"{Utilities.FixDP(totalDamage * 100)}% | {Utilities.FixDP(totalDamage * MidsContext.Character.Totals.HPMax)}" : Utilities.FixDP(totalDamage))}";
        }

        public int[] GetRankedEffects(bool newMode)
        {
            var weightedEffects = Effects
                .Select((e, i) => new KeyValuePair<KeyValuePair<IEffect, int>, int>(new KeyValuePair<IEffect, int>(e, i), 0))
                .ToList();

            for (var i = 0; i < weightedEffects.Count; i++)
            {
                if ((MidsContext.Config.Suppression & weightedEffects[i].Key.Key.Suppression) == Enums.eSuppress.None ||
                    MidsContext.Config.Inc.DisablePvE & weightedEffects[i].Key.Key.PvMode == Enums.ePvX.PvE ||
                    !MidsContext.Config.Inc.DisablePvE & weightedEffects[i].Key.Key.PvMode == Enums.ePvX.PvP)
                {
                    weightedEffects[i] = new KeyValuePair<KeyValuePair<IEffect, int>, int>(
                        new KeyValuePair<IEffect, int>(weightedEffects[i].Key.Key, weightedEffects[i].Key.Value),
                        int.MinValue);
                    continue;
                }

                // Filter out effects with ToWho == None
                if (weightedEffects[i].Key.Key.ToWho is not (Enums.eToWho.Target or Enums.eToWho.Self))
                {
                    continue;
                }

                var weight = (int) weightedEffects[i].Key.Key.EffectClass + 1;
                if (Math.Abs(weightedEffects[i].Key.Key.Probability - 1) < 0.01)
                {
                    weight += 10;
                }

                if (HasAbsorbedEffects & weightedEffects[i].Key.Key.EffectType != Enums.eEffectType.EntCreate)
                {
                    weight += 50;
                }

                if (weightedEffects[i].Key.Key.DelayedTime > 1)
                {
                    weight -= 100;
                }

                if (weightedEffects[i].Key.Key.DelayedTime > 0 & weightedEffects[i].Key.Key.DelayedTime <= 1)
                {
                    weight -= 25;
                }

                if (weightedEffects[i].Key.Key.InherentSpecial)
                {
                    weight -= 100;
                }

                if (weightedEffects[i].Key.Key.InherentSpecial2)
                {
                    weight -= 100;
                }

                if (weightedEffects[i].Key.Key.ToWho == Enums.eToWho.Self & (weightedEffects[i].Key.Key.BuffedMag > 0 | weightedEffects[i].Key.Key.EffectType == Enums.eEffectType.Mez))
                {
                    weight += 10;
                }

                if (weightedEffects[i].Key.Key.ToWho == Enums.eToWho.Target & weightedEffects[i].Key.Key.BuffedMag < 0)
                {
                    weight += 10;
                }

                if (weightedEffects[i].Key.Key.ToWho == Enums.eToWho.Target & weightedEffects[i].Key.Key.BuffedMag > 0 & weightedEffects[i].Key.Key.Absorbed_Effect)
                {
                    weight += 10;
                }

                if (weightedEffects[i].Key.Key.ToWho == Enums.eToWho.Self & weightedEffects[i].Key.Key.BuffedMag > 0 & weightedEffects[i].Key.Key.Absorbed_Effect)
                {
                    weight += 10;
                }

                if (weightedEffects[i].Key.Key.isEnhancementEffect)
                {
                    weight -= 30;
                }

                if (weightedEffects[i].Key.Key.VariableModified)
                {
                    weight += 30;
                }

                weight += weightedEffects[i].Key.Key.EffectType switch
                {
                    Enums.eEffectType.None => -1000,
                    Enums.eEffectType.Damage => -500,
                    Enums.eEffectType.DamageBuff => 10,
                    Enums.eEffectType.Defense => 25,
                    Enums.eEffectType.Endurance => 15,
                    Enums.eEffectType.Enhancement => weightedEffects[i].Key.Key.ETModifies switch
                    {
                        Enums.eEffectType.SpeedFlying => 5,
                        Enums.eEffectType.SpeedJumping => 5,
                        Enums.eEffectType.SpeedRunning => 5,
                        Enums.eEffectType.JumpHeight => -5,
                        _ => 9
                    },
                    Enums.eEffectType.Fly => 3,
                    Enums.eEffectType.SpeedFlying => 5,
                    Enums.eEffectType.GrantPower => -20,
                    Enums.eEffectType.Heal => 15,
                    Enums.eEffectType.Absorb => 13,
                    Enums.eEffectType.HitPoints => 10,
                    Enums.eEffectType.JumpHeight => 5,
                    Enums.eEffectType.SpeedJumping => 5,
                    Enums.eEffectType.Mez when !weightedEffects[i].Key.Key.Buffable => -1,
                    Enums.eEffectType.Mez when weightedEffects[i].Key.Key.MezType is Enums.eMez.OnlyAffectsSelf or Enums.eMez.Untouchable => -9,
                    Enums.eEffectType.Mez when weightedEffects[i].Key.Key.MezType is Enums.eMez.Knockback or Enums.eMez.Knockup => Convert.ToInt32(8f * weightedEffects[i].Key.Key.Probability),
                    Enums.eEffectType.Mez => Convert.ToInt32(9f * weightedEffects[i].Key.Key.Probability),
                    Enums.eEffectType.MezResist => 5,
                    Enums.eEffectType.MovementControl => 3,
                    Enums.eEffectType.MovementFriction => 3,
                    Enums.eEffectType.Recovery => 10,
                    Enums.eEffectType.Resistance => 20,
                    Enums.eEffectType.RevokePower => -20,
                    Enums.eEffectType.SpeedRunning => 5,
                    Enums.eEffectType.SetMode => -500,
                    Enums.eEffectType.StealthRadius => 7,
                    Enums.eEffectType.StealthRadiusPlayer => 6,
                    Enums.eEffectType.EntCreate when HasAbsorbedEffects => -500,
                    Enums.eEffectType.ToHit => 10,
                    Enums.eEffectType.Translucency => -20,
                    Enums.eEffectType.GlobalChanceMod => 1,
                    Enums.eEffectType.DesignerStatus => int.MinValue,
                    Enums.eEffectType.Null => int.MinValue,
                    Enums.eEffectType.NullBool => int.MinValue,
                    _ => 9
                };

                weightedEffects[i] = new KeyValuePair<KeyValuePair<IEffect, int>, int>(
                    new KeyValuePair<IEffect, int>(weightedEffects[i].Key.Key, weightedEffects[i].Key.Value), weight);
            }

            weightedEffects = weightedEffects.OrderByDescending(e => e.Key.Value).ToList();

            return weightedEffects.Select(e => e.Key.Value).ToArray();
        }

        internal class EffectDurationComparer : IComparer<IEffect>
        {
            public int Compare(IEffect? a, IEffect? b)
            {
                if (a?.Duration.CompareTo(b?.Duration) != 0)
                {
                    var ret = a?.Duration.CompareTo(b?.Duration);
                    
                    return ret ?? 0;
                }

                if (a?.BuffedMag.CompareTo(b?.BuffedMag) != 0)
                {
                    var ret = a?.BuffedMag.CompareTo(b?.BuffedMag);

                    return ret ?? 0;
                }

                return 0;
            }
        }

        public int GetDurationEffectID()
        {
            return Effects
                .Where(e => e.EffectType is not (Enums.eEffectType.Null or Enums.eEffectType.NullBool or Enums.eEffectType.DesignerStatus))
                .Any(e => e.EffectType == Enums.eEffectType.Mez)
                ? Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e =>
                        ((e.Value.PvMode == Enums.ePvX.Any) |
                         ((e.Value.PvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                         ((e.Value.PvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE)) &
                        (e.Value.EffectType == Enums.eEffectType.Mez) &
                        (e.Value.EffectClass != Enums.eEffectClass.Ignored) & (e.Value.Duration > 0) &
                        e.Value.ValidateConditional() &
                        (e.Value.Probability > float.Epsilon) &
                        (e.Value.SpecialCase != Enums.eSpecialCase.Defiance))
                    .OrderByDescending(e => e.Value, new EffectDurationComparer())
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault()
                    .Key
                : Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e =>
                        ((e.Value.PvMode == Enums.ePvX.Any) |
                         ((e.Value.PvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                         ((e.Value.PvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE)) &
                        (e.Value.EffectClass != Enums.eEffectClass.Ignored) & (e.Value.Duration > 0) &
                        e.Value.ValidateConditional() &
                        (e.Value.Probability > float.Epsilon) &
                        (e.Value.SpecialCase != Enums.eSpecialCase.Defiance))
                    .OrderByDescending(e => e.Value, new EffectDurationComparer())
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault()
                    .Key;
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

                if (((iEffect == Enums.eEffectType.SpeedFlying) & !maxMode &&
                     Effects[iIndex].Aspect == Enums.eAspect.Max) ||
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
                    (maxMode && Effects[iIndex].Aspect != Enums.eAspect.Max) || Effects[iIndex].EffectType != iEffect ||
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

                if (((iEffect == Enums.eEffectType.SpeedFlying) & !maxMode &&
                     fx.Aspect == Enums.eAspect.Max) ||
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
                    (maxMode && fx.Aspect != Enums.eAspect.Max) || fx.EffectType != iEffect ||
                    fx.EffectClass is Enums.eEffectClass.Ignored or Enums.eEffectClass.Special ||
                    (fx.DelayedTime > 5 && !includeDelayed) || !fx.CanInclude() || !fx.PvXInclude())
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
                if (fx is { Ticks: > 1, Stacking: Enums.eStacking.Yes })
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

        public int CompareTo(object? obj)
        {
            if (obj is not Power power)
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
            if (idEffect < 0 | idEffect > Effects.Length - 1)
            {
                return false;
            }

            var str = string.Empty;
            var array = Array.Empty<int>();
            var effect = (IEffect) Effects[idEffect].Clone();
            switch (effect.EffectType)
            {
                case Enums.eEffectType.DamageBuff or Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity:
                {
                    var iDamage = new bool[Enum.GetValues(typeof(Enums.eDamage)).Length];
                    for (var index1 = 0; index1 < Effects.Length; index1++)
                    {
                        for (var index2 = 0; index2 < iDamage.Length; index2++)
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
                    var newValue = effect.EffectType == Enums.eEffectType.Defense
                        ? Enums.GetGroupedDefense(iDamage, shortForm)
                        : Enums.GetGroupedDamage(iDamage, shortForm);
                    str = shortForm
                        ? effect.BuildEffectStringShort(noMag, simple).Replace("Spec", newValue)
                        : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true,
                            ignoreConditions).Replace("Special", newValue);
                    break;
                }
                case Enums.eEffectType.RechargePower:
                {
                    for (var index1 = 0; index1 < Effects.Length; index1++)
                    {
                        if (Effects[index1].EffectType != Enums.eEffectType.RechargePower)
                        {
                            continue;
                        }

                        Array.Resize(ref array, array.Length + 1);
                        array[^1] = index1;
                    }

                    str = shortForm
                        ? effect.BuildEffectStringShort(noMag, simple).Replace("RechargePower", "RechargePower(Stalker's Build-Ups)")
                        : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true, ignoreConditions).Replace("RechargePower", "RechargePower(Stalker's Build-Ups)");
                    break;
                }
                case Enums.eEffectType.Mez or Enums.eEffectType.MezResist:
                {
                    var iMez = new bool[Enum.GetValues(typeof(Enums.eMez)).Length];
                    for (var index1 = 0; index1 < Effects.Length; index1++)
                    {
                        for (var index2 = 0; index2 < iMez.Length; index2++)
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

                    if (newValue == "Knocked" && effect.BuffedMag < 0)
                    {
                        newValue = "Knockback Protection";
                    }

                    str = shortForm
                        ? effect.BuildEffectStringShort(noMag, simple).Replace("None", newValue)
                        : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true,
                            ignoreConditions).Replace("None", newValue);

                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.MezResist:
                            if (newValue == "Mez")
                            {
                                str = str.Replace("MezResist(Mez)", "Status Resistance");
                            }

                            break;
                        case Enums.eEffectType.Mez when newValue == "Mez" & effect.BuffedMag < 0:
                            str = str.Replace("Mez", "Status Protection").Replace("-", string.Empty);
                            break;
                        case Enums.eEffectType.Mez:
                            if (newValue != "Knockback Protection")
                            {
                                str = str.Replace("(Mag -", "protection (Mag ");
                            }

                            break;
                    }

                    break;
                }
                case Enums.eEffectType.Enhancement:
                {
                    var num = 0;
                    if (Effects.Length == 4)
                    {
                        num += Effects.Count(t =>
                            t.EffectType == Enums.eEffectType.Enhancement &&
                            t.ETModifies == Enums.eEffectType.SpeedRunning |
                            t.ETModifies == Enums.eEffectType.SpeedFlying |
                            t.ETModifies == Enums.eEffectType.SpeedJumping |
                            t.ETModifies == Enums.eEffectType.JumpHeight);

                        if (num == Effects.Length)
                        {
                            array = new int[Effects.Length];
                            for (var index = 0; index < array.Length; index++)
                            {
                                array[index] = index;
                            }

                            effect.ETModifies = Enums.eEffectType.Slow;
                            str = shortForm
                                ? effect.BuildEffectStringShort(noMag, simple)
                                : effect.BuildEffectString(simple, "", false, false, false, fromPopup, false, true,
                                    ignoreConditions);
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
            
            return true;
        }

        public int[] AbsorbEffects(IPower? source, float nDuration, float nDelay, Archetype? archetype, int stacking, bool isGrantPower = false, int fxid = -1, int effectId = -1)
        {
            var lst = new List<int>();
            var num2 = 0f;
            if (source?.PowerSetID > -1 && DatabaseAPI.Database.Powersets[source.PowerSetID]?.SetType == Enums.ePowerSetType.Pet)
            {
                foreach (var power in DatabaseAPI.Database.Powersets[source.PowerSetID]!.Powers)
                {
                    if (power == null)
                    {
                        continue;
                    }

                    foreach (var effect in power.Effects)
                    {
                        if ((effect.EffectType == Enums.eEffectType.SilentKill) & (effect.ToWho == Enums.eToWho.Self) & (effect.DelayedTime > 0))
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
                var fxList = Effects.ToList();
                for (var index = 0; index < source.Effects.Length; index++)
                {
                    if (!isGrantPower & (source.EntitiesAffected == Enums.eEntity.Caster) & (source.Effects[index].EffectType != Enums.eEffectType.EntCreate))
                    {
                        continue;
                    }

                    if (source.Effects[index].EffectType == Enums.eEffectType.EntCreate && source.Effects[index].nSummon > -1)
                    {
                        lst.Add(index);
                    }

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

                    if (((source.EntitiesAutoHit & Enums.eEntity.Friend) == Enums.eEntity.Friend) & ((source.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster))
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            effect.Scale *= stacking;
                        }
                    }

                    if (((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet) & ((source.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster))
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            effect.Scale *= stacking;
                        }
                    }

                    effect.Absorbed_Duration = nDuration;
                    if ((source.RechargeTime > 0) & (source.PowerType == Enums.ePowerType.Click))
                    {
                        effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                    }

                    if (nDelay > 0)
                    {
                        effect.DelayedTime = nDelay;
                    }

                    if ((effect.Absorbed_Duration > 0) & (num2 > 0))
                    {
                        effect.nDuration = effect.Absorbed_Duration;
                    }

                    fxList.Add(effect);
                }

                Effects = fxList.ToArray();
            }
            else if (isGrantPower || source.EntitiesAffected != Enums.eEntity.Caster || source.Effects[effectId].EffectType == Enums.eEffectType.EntCreate)
            {
                if (source.Effects[effectId].EffectType == Enums.eEffectType.EntCreate && source.Effects[effectId].nSummon > -1)
                {
                    lst.Add(effectId);
                }

                var fxList = Effects.ToList();
                var effect = source.Effects[effectId].Clone<IEffect>();
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
                if ((source.RechargeTime > 0) & (source.PowerType == Enums.ePowerType.Click))
                {
                    effect.Absorbed_Interval = source.RechargeTime + source.CastTime;
                }

                if (nDelay > 0)
                {
                    effect.DelayedTime = nDelay;
                }

                if ((effect.Absorbed_Duration > 0) & (num2 > 0))
                {
                    effect.nDuration = effect.Absorbed_Duration;
                }

                fxList.Add(effect);

                Effects = fxList.ToArray();
            }

            return lst.ToArray();
        }

        public void ApplyGrantPowerEffects()
        {
            var flag = true;
            var num1 = 0;
            if (HasGrantPowerEffect)
            {
                for (; flag & (num1 < 100); num1++)
                {
                    flag = false;
                    var lFxIndex = new List<int>();
                    var lFxSummons = new List<int>();
                    for (var index = 0; index < Effects.Length; index++)
                    {
                        if (Effects[index].EffectType != Enums.eEffectType.GrantPower || !Effects[index].CanGrantPower() || Effects[index].EffectClass == Enums.eEffectClass.Ignored || Effects[index].nSummon <= -1)
                        {
                            continue;
                        }

                        lFxIndex.Add(index);
                        lFxSummons.Add(Effects[index].nSummon);
                    }

                    for (var index1 = 0; index1 < lFxIndex.Count; index1++)
                    {
                        flag = true;
                        Effects[lFxIndex[index1]].EffectClass = Enums.eEffectClass.Ignored;
                        
                        AbsorbEffects(DatabaseAPI.Database.Power[lFxSummons[index1]], Effects[lFxIndex[index1]].Duration, 0, MidsContext.Archetype, 1, true, lFxIndex[index1]);
                        
                        if (Effects[lFxIndex[index1]].Absorbed_Power_nID > -1)
                        {
                            Effects[^1].Absorbed_PowerType = Effects[lFxIndex[index1]].Absorbed_PowerType;
                        }

                        if (Effects[^1].EffectType != Enums.eEffectType.GrantPower)
                        {
                            Effects[^1].ToWho = Effects[lFxIndex[index1]].ToWho;
                        }

                        if ((Effects[^1].ToWho == Enums.eToWho.All && ((EntitiesAffected & Enums.eEntity.Caster) != Enums.eEntity.Caster || (EntitiesAffected & Enums.eEntity.Friend) != Enums.eEntity.Friend)) || (Effects[^1].ToWho == Enums.eToWho.All &&
                                ((EntitiesAffected & Enums.eEntity.Caster) != Enums.eEntity.Caster ||
                                 (EntitiesAffected & Enums.eEntity.Foe) != Enums.eEntity.Foe)))
                        {
                            Effects[^1].ToWho = Enums.eToWho.Target;
                        }

                        /*
                        Effects[^1].isEnhancementEffect = Effects[lFxIndex[^1]].isEnhancementEffect;
                        if (!(Effects[lFxIndex[index1]].Probability < 1))
                        {
                           continue;
                        }

                        Effects[^1].Probability *= Effects[lFxIndex[index1]].Probability;
                        */

                        Effects[^1].isEnhancementEffect = Effects[lFxIndex[index1]].isEnhancementEffect;
                        if (!(Effects[lFxIndex[index1]].BaseProbability < 1))
                        {
                            continue;
                        }

                        // Needed?
                        // Handled by clsToonX.GBPA_ApplyIncarnateEnhancements() for _BuffedPowers (in use by the dataview)
                        Effects[^1].EffectiveProbability = Effects[^1].Probability * Effects[lFxIndex[index1]].Probability;
                    }
                }
            }

            ProcessExecutes();
        }

        public List<int> GetValidEnhancements(Enums.eType iType, int iSubType = 0)
        {
            return iType switch
            {
                Enums.eType.SetO => GetValidEnhancementsFromSets().ToList(),
                _ => DatabaseAPI.Database.Enhancements.Select((enhancement, index) => new { enhancement, index })
                    .Where(e => e.enhancement.TypeID == iType &&
                                e.enhancement.ClassID.Any(classId =>
                                    Enhancements.Contains(DatabaseAPI.Database.EnhancementClasses[classId].ID)) &&
                                (e.enhancement.SubTypeID == 0 || iSubType == 0 || e.enhancement.SubTypeID == iSubType))
                    .Select(e => e.index)
                    .ToList()
            };
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
            for (var index = 0; index < Effects.Length; index++)
            {
                if (Effects[index].EffectType == Enums.eEffectType.EntCreate && Effects[index].nSummon > -1 && Math.Abs(Effects[index].Probability - 1) < 0.01 && DatabaseAPI.Database.Entities.Length > Effects[index].nSummon)
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
                if (VariableEnabled && effect.VariableModified && hIdx > -1 && MidsContext.Character != null && MidsContext.Character.CurrentBuild.Powers[hIdx].VariableValue > stacking)
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

                AbsorbedPetEffects = true;
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

        public static Enums.ShortFX[] SplitFX(ref Enums.ShortFX iSfx, ref IPower iPower)
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

        public static string SplitFXGroupTip(ref Enums.ShortFX iSfx, ref IPower iPower, bool shortForm, bool fromPopup = true)
        {
            var str = iPower.Effects[iSfx.Index[0]].BuildEffectString(false, string.Empty, false, true, false, fromPopup);
            var newValue = string.Empty;
            if (!iPower.Effects[iSfx.Index[0]].isDamage())
            {
                return str.Replace("%VALUE%", newValue);
            }

            var iDamage = new bool[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
            for (var index = 0; index < iSfx.Index.Length; index++)
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

        private int[] GetValidEnhancementsFromSets()
        {
            return DatabaseAPI.Database.EnhancementSets
                .Where(e => SetTypes.Any(f => e.SetType == f))
                .SelectMany(e => e.Enhancements)
                .ToArray();
        }

        public void ProcessExecutes()
        {
            ProcessExecutesInner(this);
            foreach (var fx in Effects)
            {
                fx.SetPower(this);
            }

            AppliedExecutes = true;
        }

        public List<IEffect>? ProcessExecutesInner(IPower? power = null, int rLevel = 0)
        {
            // Max recursion level
            if (rLevel > 5)
            {
                return [];
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
                    // Clone duration if caller has one > 0
                    sFx.nDuration = fx.nDuration > 0
                        ? fx.nDuration
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

                    sFx.SetPower(this);
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

        public static string? GetRootPowerName(IPower? basePower, IPower? enhancedPower)
        {
            if (basePower == null)
            {
                return null;
            }

            var rootPowerName = MidsContext.Character?.Powersets
                .Where(e => e != null)
                .SelectMany(e => e.Power.Select(p => DatabaseAPI.Database.Power[p]))
                .Where(e => e != null)
                .Select(e => new KeyValuePair<string, IEffect[]>(e.FullName, e.Effects))
                .DefaultIfEmpty(new KeyValuePair<string, IEffect[]>("", []))
                .FirstOrDefault(e => e.Value.Any(fx =>
                    fx.EffectType == Enums.eEffectType.PowerRedirect &&
                    fx.Override == basePower.FullName |
                    (enhancedPower != null && fx.Override == enhancedPower.FullName)))
                .Key;

            var rootPowerBase = string.IsNullOrEmpty(rootPowerName)
                ? null
                : DatabaseAPI.GetPowerByFullName(rootPowerName);

            /*var rootPowerHidx = string.IsNullOrEmpty(rootPowerName)
                ? -1
                : MidsContext.Character.CurrentBuild.Powers.TryFindIndex(e => e is { Power: not null } && e.Power.FullName == rootPowerName);

            var rootPowerEnh = string.IsNullOrEmpty(rootPowerName) & rootPowerHidx >= 0
                ? null
                : MainModule.MidsController.Toon?.GetEnhancedPower(rootPowerHidx);*/

            return !string.IsNullOrEmpty(rootPowerName) && rootPowerBase != null && basePower.FullName != rootPowerName
                ? rootPowerName
                : null;
        }

        public static string? GetRootPowerName(int historyIdx, IPower? basePower, IPower? enhancedPower)
        {
            if (basePower == null)
            {
                return null;
            }

            var rootPowerName = historyIdx >= 0 & historyIdx < MidsContext.Character?.CurrentBuild?.Powers.Count
                ? MidsContext.Character?.CurrentBuild?.Powers[historyIdx]?.Power?.FullName
                : MidsContext.Character?.Powersets
                    .Where(e => e != null)
                    .SelectMany(e => e.Power.Select(p => DatabaseAPI.Database.Power[p]))
                    .Where(e => e != null)
                    .Select(e => new KeyValuePair<string, IEffect[]>(e.FullName, e.Effects))
                    .DefaultIfEmpty(new KeyValuePair<string, IEffect[]>("", []))
                    .FirstOrDefault(e => e.Value.Any(fx =>
                        fx.EffectType == Enums.eEffectType.PowerRedirect &&
                        fx.Override == basePower.FullName |
                        (enhancedPower != null && fx.Override == enhancedPower.FullName)))
                    .Key;

            var rootPowerBase = string.IsNullOrEmpty(rootPowerName)
                ? null
                : DatabaseAPI.GetPowerByFullName(rootPowerName);

            return !string.IsNullOrEmpty(rootPowerName) && rootPowerBase != null && basePower.FullName != rootPowerName
                ? rootPowerName
                : null;
        }

        public void ApplyModifyEffects()
        {
            foreach (var fx in Effects)
            {
                fx.UpdateAttrib();
            }
        }

        public string ExportToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
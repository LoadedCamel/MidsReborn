using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    internal sealed record DamageTypeContribution(Enums.eDamage DamageType, float DisplayedTotal)
    {
        public string Label => DamageType == Enums.eDamage.None ? "Untyped" : Enums.GetDamageName(DamageType);

        public DamageTypeContribution Scale(float factor) => this with
        {
            DisplayedTotal = DisplayedTotal * factor
        };
    }

    internal sealed record DamageSourceContribution(
        string Label,
        string Detail,
        float DisplayedTotal,
        Enums.eDamage DamageType,
        bool IsProc,
        int Occurrences)
    {
        public DamageSourceContribution Scale(float factor) => this with
        {
            DisplayedTotal = DisplayedTotal * factor
        };
    }

    internal sealed record DamageBreakdownSummary(
        float DisplayedTotal,
        float TotalExcludingProc,
        IReadOnlyList<DamageTypeContribution> ByType,
        IReadOnlyList<DamageSourceContribution> BySource,
        bool HasDamageEffects,
        bool HasPercentDamage,
        float PercentOfTargetHpTotal,
        float DisplayMultiplier)
    {
        public static readonly DamageBreakdownSummary Empty =
            new(0f, 0f, Array.Empty<DamageTypeContribution>(), Array.Empty<DamageSourceContribution>(), false, false, 0f, 1f);

        public DamageBreakdownSummary ScaleToDisplayMultiplier(float displayMultiplier)
        {
            if (!HasPercentDamage)
            {
                return this;
            }

            var safeCurrent = Math.Abs(DisplayMultiplier) < 0.0001f ? 1f : DisplayMultiplier;
            var safeTarget = Math.Abs(displayMultiplier) < 0.0001f ? safeCurrent : displayMultiplier;
            if (Math.Abs(safeCurrent - safeTarget) < 0.0001f)
            {
                return this with { DisplayMultiplier = safeTarget };
            }

            var factor = safeTarget / safeCurrent;
            return this with
            {
                DisplayedTotal = DisplayedTotal * factor,
                TotalExcludingProc = TotalExcludingProc * factor,
                ByType = ByType.Select(contribution => contribution.Scale(factor)).ToArray(),
                BySource = BySource.Select(contribution => contribution.Scale(factor)).ToArray(),
                DisplayMultiplier = safeTarget
            };
        }
    }

    public class Power : IPower, IComparable
    {
        private const string AdvancedRequirementsMarker = "MRB_ADVANCED_POWER_REQUIREMENTS";
        private const string OmniTargetRequiresMarker = "MRB_OMNI_POWER_TARGET_REQUIRES";
        private const string TargetRoutingPolicyMarker = "MRB_OMNI_POWER_TARGET_ROUTING";
        private const string TargetRoutingSourceGatesMarker = "MRB_OMNI_POWER_TARGET_ROUTING_SOURCE_GATES";
        private const string TargetRoutingDeferredTargetMarker = "MRB_OMNI_POWER_TARGET_ROUTING_DEFERRED_TARGET";
        private const string TargetRoutingDeferredSourceMarker = "MRB_OMNI_POWER_TARGET_ROUTING_DEFERRED_SOURCE";
        private const string ActivationEffectsRuntimeMarker = "MRB_OMNI_POWER_ACTIVATION_EFFECTS";
        private const string ProcPolicyMarker = "MRB_OMNI_POWER_PROC_POLICY";
        private const string StackingLifetimeMarker = "MRB_OMNI_POWER_STACKING_LIFETIME";
        private const string LifetimeMetadataMarker = "MRB_OMNI_POWER_LIFETIME_METADATA";
        private const string BoostPolicyMetadataMarker = "MRB_OMNI_POWER_BOOST_POLICY";
        private const string RootTimeMarker = "MRB_POWER_ROOT_TIME";
        private const string RechargeGroupsMarker = "MRB_POWER_RECHARGE_GROUPS";
        private const string TypedEnhancementRestrictionsMarker = "MRB_POWER_TYPED_ENHANCEMENT_RESTRICTIONS";
        private const string IgnoreEnhancementAxesMarker = "MRB_POWER_IGNORE_ENHANCEMENT_AXES";
        private const string IgnoreBuffEnhancementAxesMarker = "MRB_POWER_IGNORE_BUFF_ENHANCEMENT_AXES";
        private const string PowerIconNameMarker = "MRB_POWER_ICON_NAME";
        private const string ShowInSpecialPowerPickerMarker = "MRB_POWER_SHOW_IN_SPECIAL_POWER_PICKER";
        private const string ShowStatToggleMarker = "MRB_POWER_SHOW_STAT_TOGGLE";
        private const string VariableDisplayMetadataMarker = "MRB_POWER_VARIABLE_DISPLAY";
        private const string OptionalMetadataEnvelopeMarker = "MRB_POWER_OPTIONAL_METADATA_ENVELOPE";
        private const int OptionalMetadataEnvelopeVersion = 1;
        private bool Contains;
        public bool AppliedPowersOverride { get; set; } = false;
        public bool AbsorbedPetEffects { get; set; } = false;
        public bool AppliedExecutes { get; set; } = false;
        public bool AppliedSubPowers { get; set; } = false;

        public Power()
        {
            DescLong = string.Empty;
            DescShort = string.Empty;
            Enhancements = [];
            MaxBoosts = string.Empty;
            DisplayName = string.Empty;
            FullName = string.Empty;
            BoostsAllowed = [];
            RechargeGroups = [];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = [];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = [];
            PowerName = string.Empty;
            IconName = string.Empty;
            SetName = string.Empty;
            GroupName = string.Empty;
            NGroupMembership = [];
            PowerSetIndex = -1;
            PowerSetID = -1;
            PowerIndex = -1;
            SetTypes = [];
            VariableName = string.Empty;
            VariableDisplayDivisor = 1d;
            VariableDisplayPrecision = 0;
            VariableDisplayStep = 1d;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
            TypedEnhancementRestrictions = [];
            IgnoreEnhancementAxes = [];
            IgnoreBuffEnhancementAxes = [];
            SubIsAltColor = false;
            BoostsAllowed = [];
            Requires = new Requirement();
            AdvancedRequirements = AdvancedConditionSet.FromLegacyRequirement(Requires);
            var num = -2;
            foreach (var p in DatabaseAPI.Database.Power)
            {
                if (p is { StaticIndex: > -1 } && p.StaticIndex > num)
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
            RechargeGroups = [];
            BuffMode = Enums.eBuffMode.Normal;
            Effects = [];
            ForcedClass = string.Empty;
            MutexAuto = true;
            TargetLoS = true;
            GroupMembership = [];
            Requires = new Requirement();
            AdvancedRequirements = AdvancedConditionSet.FromLegacyRequirement(Requires);
            PowerName = string.Empty;
            IconName = string.Empty;
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
            VariableDisplayDivisor = 1d;
            VariableDisplayPrecision = 0;
            VariableDisplayStep = 1d;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
            TypedEnhancementRestrictions = [];
            IgnoreEnhancementAxes = [];
            IgnoreBuffEnhancementAxes = [];
            SubIsAltColor = false;
            if (template == null)
            {
                return;
            }

            IsModified = template.IsModified;
            IsNew = template.IsNew;
            AppliedPowersOverride = template.AppliedPowersOverride;
            AbsorbedPetEffects = template.AbsorbedPetEffects;
            AppliedExecutes = template.AppliedExecutes;
            AppliedSubPowers = template.AppliedSubPowers;
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
            IconName = template.IconName;
            DisplayName = template.DisplayName;
            Available = template.Available;
            Requires = new Requirement(template.Requires);
            AdvancedRequirements = template.AdvancedRequirements?.Clone() ??
                                   AdvancedConditionSet.FromLegacyRequirement(Requires);
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
            RootTime = template.RootTime;
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
            RechargeGroups = new string[template.RechargeGroups.Length];
            Array.Copy(template.RechargeGroups, RechargeGroups, RechargeGroups.Length);
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
                Effects[index] = (IEffect)template.Effects[index].Clone();
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
            VariableDisplayDivisor = template.VariableDisplayDivisor;
            VariableDisplayPrecision = template.VariableDisplayPrecision;
            VariableDisplayStep = template.VariableDisplayStep;
            NIDSubPower = new int[template.NIDSubPower.Length];
            Array.Copy(template.NIDSubPower, NIDSubPower, NIDSubPower.Length);
            UIDSubPower = new string[template.UIDSubPower.Length];
            Array.Copy(template.UIDSubPower, UIDSubPower, UIDSubPower.Length);
            SubIsAltColor = template.SubIsAltColor;
            IgnoreEnh = new Enums.eEnhance[template.IgnoreEnh.Length];
            Array.Copy(template.IgnoreEnh, IgnoreEnh, IgnoreEnh.Length);
            Ignore_Buff = new Enums.eEnhance[template.Ignore_Buff.Length];
            Array.Copy(template.Ignore_Buff, Ignore_Buff, Ignore_Buff.Length);
            TypedEnhancementRestrictions = template.TypedEnhancementRestrictions
                .Where(restriction => restriction.IsValid)
                .ToArray();
            IgnoreEnhancementAxes = EnhancementPolicyAxes.Normalize(template.IgnoreEnhancementAxes);
            IgnoreBuffEnhancementAxes = EnhancementPolicyAxes.Normalize(template.IgnoreBuffEnhancementAxes);
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
            ShowInSpecialPowerPicker = template.ShowInSpecialPowerPicker;
            ShowStatToggle = template.ShowStatToggle;
            ForcedClass = template.ForcedClass;
            ForcedClassID = template.ForcedClassID;
            SortOverride = template.SortOverride;
            BoostUsePlayerLevel = template.BoostUsePlayerLevel;
            BoostBoostable = template.BoostBoostable;
            ActivationEffectsRuntime = template is Power concretePower && concretePower.ActivationEffectsRuntime.Length > 0
                ? concretePower.ActivationEffectsRuntime.Select(effect => (IEffect)effect.Clone()).ToArray()
                : [];
            foreach (var effect in ActivationEffectsRuntime)
            {
                effect.SetPower(this);
            }
            OmniRequiredModesRaw = template is Power requiredModesPower
                ? requiredModesPower.OmniRequiredModesRaw
                    .Where(mode => !string.IsNullOrWhiteSpace(mode))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray()
                : [];
            OmniDisallowedModesRaw = template is Power disallowedModesPower
                ? disallowedModesPower.OmniDisallowedModesRaw
                    .Where(mode => !string.IsNullOrWhiteSpace(mode))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray()
                : [];
            OmniTargetRequiresRaw = template is Power targetPower ? targetPower.OmniTargetRequiresRaw : string.Empty;
            TargetRoutingPolicy = template is Power routingPower
                ? routingPower.TargetRoutingPolicy.Clone()
                : PlannerTargetRoutingPolicy.Default;
            OmniDisplayClassName = template is Power displayPower ? displayPower.OmniDisplayClassName : string.Empty;
            ProcPolicy = template is Power procPower ? procPower.ProcPolicy : ImportedProcPolicy.Default;
            OmniStackingLifetime = template is Power stackPower ? stackPower.OmniStackingLifetime : null;
            OmniLifetimeMetadata = template is Power lifetimePower
                ? lifetimePower.OmniLifetimeMetadata.Clone()
                : ImportedPowerLifetimeMetadata.Default;
            OmniBoostPolicy = template is Power boostPower
                ? boostPower.OmniBoostPolicy.Clone()
                : ImportedBoostPolicyMetadata.Default;
            ApplyImportedBoostPolicyFlags();
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
            VariableDisplayDivisor = 1d;
            VariableDisplayPrecision = 0;
            VariableDisplayStep = 1d;
            UIDSubPower = [];
            NIDSubPower = [];
            Ignore_Buff = [];
            IgnoreEnh = [];
            TypedEnhancementRestrictions = [];
            SubIsAltColor = false;
            BoostsAllowed = [];
            RechargeGroups = [];
            StaticIndex = reader.ReadInt32();
            FullName = reader.ReadString();
            GroupName = reader.ReadString();
            SetName = reader.ReadString();
            PowerName = reader.ReadString();
            DisplayName = reader.ReadString();
            IconName = string.Empty;
            Available = reader.ReadInt32();
            Requires = new Requirement(reader);
            AdvancedRequirements = AdvancedConditionSet.FromLegacyRequirement(Requires);
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
            RootTime = 0f;
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

            IgnoreEnhancementAxes = [];
            IgnoreBuffEnhancementAxes = [];

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
            if (!TryReadOptionalMetadataEnvelope(reader))
            {
                ReadOptionalMetadata(reader);
            }
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

        public string IconName { get; set; }

        public string DisplayName { get; set; }

        public int Available { get; set; }

        public Requirement Requires { get; set; }

        public AdvancedConditionSet AdvancedRequirements { get; set; }

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

        public float RootTime { get; set; }

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

        public string[] RechargeGroups { get; set; }

        public TypedEnhancementRestriction[] TypedEnhancementRestrictions { get; set; }

        public EnhancementPolicyAxis[] IgnoreEnhancementAxes { get; set; }

        public EnhancementPolicyAxis[] IgnoreBuffEnhancementAxes { get; set; }

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

        public double VariableDisplayDivisor { get; set; } = 1d;

        public int VariableDisplayPrecision { get; set; }

        public double VariableDisplayStep { get; set; } = 1d;

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

        public bool ShowInSpecialPowerPicker { get; set; }

        public bool ShowStatToggle { get; set; } = true;

        public string ForcedClass { get; set; }

        public Enums.eGridType InherentType { get; set; }

        public int ForcedClassID { get; set; }

        public IEffect[] Effects { get; set; }

        public IEffect[] ActivationEffectsRuntime { get; set; } = [];
        internal string[] OmniRequiredModesRaw { get; set; } = [];
        internal string[] OmniDisallowedModesRaw { get; set; } = [];

        public string OmniTargetRequiresRaw { get; set; } = string.Empty;
        internal PlannerTargetRoutingPolicy TargetRoutingPolicy { get; set; } = PlannerTargetRoutingPolicy.Default;

        public string OmniDisplayClassName { get; set; } = string.Empty;

        internal ImportedProcPolicy ProcPolicy { get; set; } = ImportedProcPolicy.Default;
        internal bool? OmniStackingLifetime { get; set; }
        internal ImportedPowerLifetimeMetadata OmniLifetimeMetadata { get; set; } = ImportedPowerLifetimeMetadata.Default;
        internal ImportedBoostPolicyMetadata OmniBoostPolicy { get; set; } = ImportedBoostPolicyMetadata.Default;
        internal bool UsesPlayerLevelForBoostMath => BoostUsePlayerLevel || OmniBoostPolicy.Attuned == true;
        internal bool AllowsBoostersForBoostMath => BoostBoostable || OmniBoostPolicy.Boostable == true;
        internal int? ImportedMinSlotLevelZeroBased => OmniBoostPolicy.MinSlotLevel.HasValue
            ? Math.Max(0, OmniBoostPolicy.MinSlotLevel.Value - 1)
            : null;
        internal int? ImportedMaxSlotLevelZeroBased => OmniBoostPolicy.MaxSlotLevel.HasValue
            ? Math.Max(0, OmniBoostPolicy.MaxSlotLevel.Value - 1)
            : null;
        internal int? ImportedMinimumUseLevelZeroBased => OmniBoostPolicy.MinimumUseLevel.HasValue
            ? Math.Max(0, OmniBoostPolicy.MinimumUseLevel.Value - 1)
            : null;
        internal int? ImportedMaximumUseLevelZeroBased => OmniBoostPolicy.MaximumUseLevel.HasValue
            ? Math.Max(0, OmniBoostPolicy.MaximumUseLevel.Value - 1)
            : null;
        internal int? ImportedMaxBoostLevelZeroBased => OmniBoostPolicy.MaxBoostLevel.HasValue
            ? Math.Max(0, OmniBoostPolicy.MaxBoostLevel.Value - 1)
            : null;

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
                : (float)(Math.Ceiling(CastTimeReal / 0.132f) + 1) * 0.132f;
            set => CastTimeReal = value;
        }

        public float CastTimeBase => CastTimeReal;

        public float ArcanaCastTime => (float)(Math.Ceiling(CastTimeReal / 0.132f) + 1) * 0.132f;

        public bool Slottable
        {
            get
            {
                var ps = GetPowerSet();
                return Enhancements.Length > 0 && ps is { SetType: Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent or Enums.ePowerSetType.Pool };
            }
        }

        public float AoEModifier => EffectArea != Enums.eEffectArea.Cone
            ? EffectArea != Enums.eEffectArea.Sphere ? 1 : (float)(1 + Radius * 0.150000005960464)
            : (float)(1 + Radius * 0.150000005960464 - Radius * 0.000366669992217794 * (360 - Arc));

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
            var legacyRequirements = AdvancedRequirements is { Rows.Count: > 0 }
                ? AdvancedRequirements.ToLegacyRequirement()
                : Requires;
            legacyRequirements.StoreTo(writer);
            writer.Write((int)ModesRequired);
            writer.Write((int)ModesDisallowed);
            writer.Write((int)PowerType);
            writer.Write(Accuracy);
            writer.Write((int)AttackTypes);
            writer.Write(GroupMembership.Length - 1);
            for (var index = 0; index <= GroupMembership.Length - 1; ++index)
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
                writer.Write((int)ie);
            }

            writer.Write(Ignore_Buff.Length - 1);
            foreach (var ib in Ignore_Buff)
            {
                writer.Write((int)ib);
            }

            writer.Write(SkipMax);
            writer.Write((int)InherentType);
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
            StoreOptionalMetadataEnvelope(writer);
        }

        private void StoreOptionalMetadataEnvelope(BinaryWriter writer)
        {
            BinaryMetadataEnvelope.Write(writer, OptionalMetadataEnvelopeMarker, OptionalMetadataEnvelopeVersion,
                payloadWriter => WriteOptionalMetadataPayload(payloadWriter));
        }

        private void WriteOptionalMetadataPayload(BinaryWriter writer)
        {
            AdvancedConditionSet.StoreMarked(writer, AdvancedRequirementsMarker,
                AdvancedRequirements is { Rows.Count: > 0 }
                    ? AdvancedRequirements
                    : AdvancedConditionSet.FromLegacyRequirement(Requires));
            StoreMarkedString(writer, OmniTargetRequiresMarker, OmniTargetRequiresRaw);
            if (!string.IsNullOrWhiteSpace(IconName))
            {
                StoreMarkedString(writer, PowerIconNameMarker, IconName);
            }

            StoreTargetRoutingPolicy(writer);
            StoreMarkedSingle(writer, RootTimeMarker, RootTime);
            StoreMarkedStringArray(writer, RechargeGroupsMarker, RechargeGroups);
            StoreMarkedStringArray(writer, TypedEnhancementRestrictionsMarker, TypedEnhancementLegality.Serialize(TypedEnhancementRestrictions));
            StoreMarkedStringArray(writer, IgnoreEnhancementAxesMarker, EnhancementPolicyAxes.Serialize(IgnoreEnhancementAxes));
            StoreMarkedStringArray(writer, IgnoreBuffEnhancementAxesMarker, EnhancementPolicyAxes.Serialize(IgnoreBuffEnhancementAxes));
            StoreActivationEffectsRuntime(writer);
            StoreProcPolicy(writer);
            StoreStackingLifetime(writer);
            StoreLifetimeMetadata(writer);
            StoreBoostPolicyMetadata(writer);
            StoreMarkedBoolean(writer, ShowInSpecialPowerPickerMarker, ShowInSpecialPowerPicker);
            StoreMarkedBoolean(writer, ShowStatToggleMarker, ShowStatToggle);
            StoreVariableDisplayMetadata(writer);
        }

        private bool TryReadOptionalMetadataEnvelope(BinaryReader reader)
        {
            return BinaryMetadataEnvelope.TryRead(reader, OptionalMetadataEnvelopeMarker, OptionalMetadataEnvelopeVersion,
                (_, payloadReader) => ReadOptionalMetadata(payloadReader));
        }

        private void ReadOptionalMetadata(BinaryReader reader)
        {
            if (AdvancedConditionSet.TryReadMarked(reader, AdvancedRequirementsMarker, out var advancedRequirements))
            {
                AdvancedRequirements = advancedRequirements;
                Requires = AdvancedRequirements.ToLegacyRequirement();
            }

            if (TryReadMarkedString(reader, OmniTargetRequiresMarker, out var omniTargetRequires))
            {
                OmniTargetRequiresRaw = omniTargetRequires;
            }

            if (TryReadMarkedString(reader, PowerIconNameMarker, out var iconName))
            {
                IconName = iconName;
            }

            TryReadTargetRoutingPolicy(reader);
            if (TryReadMarkedSingle(reader, RootTimeMarker, out var rootTime))
            {
                RootTime = rootTime;
            }

            if (TryReadMarkedStringArray(reader, RechargeGroupsMarker, out var rechargeGroups))
            {
                RechargeGroups = rechargeGroups;
            }

            if (TryReadMarkedStringArray(reader, TypedEnhancementRestrictionsMarker, out var typedEnhancementRestrictions))
            {
                TypedEnhancementRestrictions = TypedEnhancementLegality.Deserialize(typedEnhancementRestrictions);
            }

            if (TryReadMarkedStringArray(reader, IgnoreEnhancementAxesMarker, out var ignoreEnhancementAxes))
            {
                IgnoreEnhancementAxes = EnhancementPolicyAxes.Deserialize(ignoreEnhancementAxes);
            }

            if (TryReadMarkedStringArray(reader, IgnoreBuffEnhancementAxesMarker, out var ignoreBuffEnhancementAxes))
            {
                IgnoreBuffEnhancementAxes = EnhancementPolicyAxes.Deserialize(ignoreBuffEnhancementAxes);
            }

            TryReadActivationEffectsRuntime(reader);
            TryReadProcPolicy(reader);
            TryReadStackingLifetime(reader);
            TryReadLifetimeMetadata(reader);
            TryReadBoostPolicyMetadata(reader);
            ApplyImportedBoostPolicyFlags();
            if (!TryReadMarkedBoolean(reader, ShowInSpecialPowerPickerMarker, out var showInSpecialPowerPicker))
            {
                ShowInSpecialPowerPicker = SpecialPowerCatalog.ShouldBackfillSpecialPowerPicker(this);
            }
            else
            {
                ShowInSpecialPowerPicker = showInSpecialPowerPicker;
            }

            if (!TryReadMarkedBoolean(reader, ShowStatToggleMarker, out var showStatToggle))
            {
                ShowStatToggle = true;
            }
            else
            {
                ShowStatToggle = showStatToggle;
            }

            TryReadVariableDisplayMetadata(reader);
        }

        private static void StoreMarkedString(BinaryWriter writer, string marker, string value)
        {
            writer.Write(marker);
            writer.Write(value ?? string.Empty);
        }

        private void StoreVariableDisplayMetadata(BinaryWriter writer)
        {
            if (Math.Abs(VariableDisplayDivisor - 1d) < 0.0000001d &&
                VariableDisplayPrecision == 0 &&
                Math.Abs(VariableDisplayStep - 1d) < 0.0000001d)
            {
                return;
            }

            writer.Write(VariableDisplayMetadataMarker);
            writer.Write(string.Join("|",
                VariableDisplayDivisor.ToString(CultureInfo.InvariantCulture),
                VariableDisplayPrecision.ToString(CultureInfo.InvariantCulture),
                VariableDisplayStep.ToString(CultureInfo.InvariantCulture)));
        }

        private static void StoreMarkedSingle(BinaryWriter writer, string marker, float value)
        {
            writer.Write(marker);
            writer.Write(value);
        }

        private static void StoreMarkedBoolean(BinaryWriter writer, string marker, bool value)
        {
            writer.Write(marker);
            writer.Write(value);
        }

        private static void StoreMarkedStringArray(BinaryWriter writer, string marker, IReadOnlyList<string>? values)
        {
            writer.Write(marker);
            var safeValues = values?
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? [];
            writer.Write(safeValues.Length);
            foreach (var value in safeValues)
            {
                writer.Write(value);
            }
        }

        private static bool TryReadMarkedString(BinaryReader reader, string marker, out string value)
        {
            return BinaryMetadataEnvelope.TryReadMarkedString(reader, marker, out value);
        }

        private void TryReadVariableDisplayMetadata(BinaryReader reader)
        {
            if (!TryReadMarkedString(reader, VariableDisplayMetadataMarker, out var rawMetadata) ||
                string.IsNullOrWhiteSpace(rawMetadata))
            {
                return;
            }

            var pieces = rawMetadata.Split('|');
            if (pieces.Length > 0 &&
                double.TryParse(pieces[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var divisor) &&
                Math.Abs(divisor) > 0.0000001d)
            {
                VariableDisplayDivisor = divisor;
            }

            if (pieces.Length > 1 &&
                int.TryParse(pieces[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var precision))
            {
                VariableDisplayPrecision = Math.Max(0, precision);
            }

            if (pieces.Length > 2 &&
                double.TryParse(pieces[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var step) &&
                step > 0d)
            {
                VariableDisplayStep = step;
            }
        }

        private static bool TryReadMarkedSingle(BinaryReader reader, string marker, out float value)
        {
            return BinaryMetadataEnvelope.TryReadMarkedSingle(reader, marker, out value);
        }

        private static bool TryReadMarkedBoolean(BinaryReader reader, string marker, out bool value)
        {
            return BinaryMetadataEnvelope.TryReadMarkedBoolean(reader, marker, out value);
        }

        private static bool TryReadMarkedStringArray(BinaryReader reader, string marker, out string[] values)
        {
            if (!BinaryMetadataEnvelope.TryReadMarkedStringArray(reader, marker, out values))
            {
                values = [];
                return false;
            }

            values = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            return true;
        }

        private void StoreActivationEffectsRuntime(BinaryWriter writer)
        {
            writer.Write(ActivationEffectsRuntimeMarker);
            writer.Write(ActivationEffectsRuntime.Length);
            foreach (var effect in ActivationEffectsRuntime)
            {
                effect.StoreTo(ref writer);
            }
        }

        private void TryReadActivationEffectsRuntime(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, ActivationEffectsRuntimeMarker))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var count = reader.ReadInt32();
                if (count <= 0)
                {
                    ActivationEffectsRuntime = [];
                    return;
                }

                var effects = new IEffect[count];
                for (var index = 0; index < count; index++)
                {
                    var effect = (IEffect)new Effect(reader)
                    {
                        nID = index
                    };
                    effect.SetPower(this);
                    effects[index] = effect;
                }

                ActivationEffectsRuntime = effects;
            }
            catch
            {
                reader.BaseStream.Position = position;
                ActivationEffectsRuntime = [];
            }
        }

        private void StoreProcPolicy(BinaryWriter writer)
        {
            writer.Write(ProcPolicyMarker);
            writer.Write(1001);
            writer.Write((int)ProcPolicy.Eligibility);
            writer.Write((int)ProcPolicy.Allowance);
            writer.Write(ProcPolicy.MainTargetOnly);
            writer.Write(ProcPolicy.IgnoreChainEffect);
            writer.Write(ProcPolicy.IgnoreOverCap);
            writer.Write(ProcPolicy.RawEligibilityValue ?? string.Empty);
        }

        private void TryReadProcPolicy(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, ProcPolicyMarker))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var versionOrAllowance = reader.ReadInt32();
                if (versionOrAllowance >= 1000)
                {
                    ProcPolicy = new ImportedProcPolicy(
                        (ImportedProcEligibilityMode)reader.ReadInt32(),
                        (ProcAllowanceMode)reader.ReadInt32(),
                        reader.ReadBoolean(),
                        reader.ReadBoolean(),
                        reader.ReadBoolean(),
                        reader.ReadString());
                    return;
                }

                var legacyAllowance = (ProcAllowanceMode)versionOrAllowance;
                ProcPolicy = new ImportedProcPolicy(
                    ImportedProcPolicyNormalizer.ParseStoredEligibility(null, legacyAllowance.ToString()),
                    legacyAllowance,
                    reader.ReadBoolean(),
                    reader.ReadBoolean(),
                    reader.ReadBoolean(),
                    string.Empty);
            }
            catch
            {
                reader.BaseStream.Position = position;
                ProcPolicy = ImportedProcPolicy.Default;
            }
        }

        private void StoreTargetRoutingPolicy(BinaryWriter writer)
        {
            writer.Write(TargetRoutingPolicyMarker);
            writer.Write((int)TargetRoutingPolicy.AllowedRecipients);
            writer.Write((int)TargetRoutingPolicy.SelfRequirement);
            writer.Write(TargetRoutingPolicy.OriginalTargetRequires ?? string.Empty);
            writer.Write(TargetRoutingPolicy.RecipientClauses.Count);
            foreach (var clause in TargetRoutingPolicy.RecipientClauses)
            {
                writer.Write((int)clause.Link);
                writer.Write((int)clause.Kind);
                writer.Write(clause.Value ?? string.Empty);
                writer.Write(clause.Negated);
            }

            AdvancedConditionSet.StoreMarked(writer, TargetRoutingSourceGatesMarker, TargetRoutingPolicy.BuildSourceGates);
            AdvancedConditionSet.StoreMarked(writer, TargetRoutingDeferredTargetMarker, TargetRoutingPolicy.DeferredTargetRows);
            AdvancedConditionSet.StoreMarked(writer, TargetRoutingDeferredSourceMarker, TargetRoutingPolicy.DeferredSourceRows);
        }

        private void TryReadTargetRoutingPolicy(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                TargetRoutingPolicy = PlannerTargetRoutingPolicy.Default;
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, TargetRoutingPolicyMarker))
                {
                    reader.BaseStream.Position = position;
                    TargetRoutingPolicy = PlannerTargetRoutingPolicy.Default;
                    return;
                }

                var allowedRecipients = (PlannerRecipientFlags)reader.ReadInt32();
                var selfRequirement = (PlannerSelfRequirement)reader.ReadInt32();
                var originalTargetRequires = reader.ReadString();
                var clauseCount = reader.ReadInt32();
                var clauses = new List<PlannerRecipientClause>(clauseCount);
                for (var index = 0; index < clauseCount; index++)
                {
                    clauses.Add(new PlannerRecipientClause
                    {
                        Link = (AdvancedConditionLink)reader.ReadInt32(),
                        Kind = (PlannerRecipientClauseKind)reader.ReadInt32(),
                        Value = reader.ReadString(),
                        Negated = reader.ReadBoolean()
                    });
                }

                var buildSourceGates = AdvancedConditionSet.TryReadMarked(reader, TargetRoutingSourceGatesMarker, out var sourceGates)
                    ? sourceGates
                    : new AdvancedConditionSet();
                var deferredTargetRows = AdvancedConditionSet.TryReadMarked(reader, TargetRoutingDeferredTargetMarker, out var deferredTarget)
                    ? deferredTarget
                    : new AdvancedConditionSet();
                var deferredSourceRows = AdvancedConditionSet.TryReadMarked(reader, TargetRoutingDeferredSourceMarker, out var deferredSource)
                    ? deferredSource
                    : new AdvancedConditionSet();

                TargetRoutingPolicy = new PlannerTargetRoutingPolicy(
                    allowedRecipients,
                    selfRequirement,
                    buildSourceGates,
                    clauses,
                    deferredTargetRows,
                    deferredSourceRows,
                    originalTargetRequires,
                    hasImportedMetadata: !string.IsNullOrWhiteSpace(originalTargetRequires));
            }
            catch
            {
                reader.BaseStream.Position = position;
                TargetRoutingPolicy = PlannerTargetRoutingPolicy.Default;
            }
        }

        private void StoreStackingLifetime(BinaryWriter writer)
        {
            writer.Write(StackingLifetimeMarker);
            writer.Write(1);
            writer.Write(OmniStackingLifetime.HasValue);
            if (OmniStackingLifetime.HasValue)
            {
                writer.Write(OmniStackingLifetime.Value);
            }
        }

        private void TryReadStackingLifetime(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, StackingLifetimeMarker))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported power stacking-lifetime version {version}.");
                }

                var hasValue = reader.ReadBoolean();
                OmniStackingLifetime = hasValue ? reader.ReadBoolean() : null;
            }
            catch
            {
                reader.BaseStream.Position = position;
                OmniStackingLifetime = null;
            }
        }

        private void StoreLifetimeMetadata(BinaryWriter writer)
        {
            writer.Write(LifetimeMetadataMarker);
            writer.Write(1);
            writer.Write(OmniLifetimeMetadata.MaxPowerLifetime.HasValue);
            if (OmniLifetimeMetadata.MaxPowerLifetime.HasValue)
            {
                writer.Write(OmniLifetimeMetadata.MaxPowerLifetime.Value);
            }

            writer.Write(OmniLifetimeMetadata.MaxPowerLifetimeInGame.HasValue);
            if (OmniLifetimeMetadata.MaxPowerLifetimeInGame.HasValue)
            {
                writer.Write(OmniLifetimeMetadata.MaxPowerLifetimeInGame.Value);
            }
        }

        private void TryReadLifetimeMetadata(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, LifetimeMetadataMarker))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported power lifetime-metadata version {version}.");
                }

                var hasMaxLifetime = reader.ReadBoolean();
                int? maxLifetime = hasMaxLifetime ? reader.ReadInt32() : null;
                var hasMaxLifetimeInGame = reader.ReadBoolean();
                int? maxLifetimeInGame = hasMaxLifetimeInGame ? reader.ReadInt32() : null;
                OmniLifetimeMetadata = new ImportedPowerLifetimeMetadata(maxLifetime, maxLifetimeInGame);
            }
            catch
            {
                reader.BaseStream.Position = position;
                OmniLifetimeMetadata = ImportedPowerLifetimeMetadata.Default;
            }
        }

        private void StoreBoostPolicyMetadata(BinaryWriter writer)
        {
            writer.Write(BoostPolicyMetadataMarker);
            writer.Write(1);
            writer.Write(OmniBoostPolicy.RawBoostInfoJson ?? string.Empty);
            writer.Write(OmniBoostPolicy.AllowedBoostSetCategories.Count);
            foreach (var category in OmniBoostPolicy.AllowedBoostSetCategories)
            {
                writer.Write(category);
            }
        }

        private void TryReadBoostPolicyMetadata(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!BinaryMetadataEnvelope.TryConsumeMarker(reader, BoostPolicyMetadataMarker))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported power boost-policy version {version}.");
                }

                var rawBoostInfoJson = reader.ReadString();
                var count = reader.ReadInt32();
                var allowedBoostSetCategories = new string[count];
                for (var index = 0; index < count; index++)
                {
                    allowedBoostSetCategories[index] = reader.ReadString();
                }

                OmniBoostPolicy = new ImportedBoostPolicyMetadata(rawBoostInfoJson, allowedBoostSetCategories);
            }
            catch
            {
                reader.BaseStream.Position = position;
                OmniBoostPolicy = ImportedBoostPolicyMetadata.Default;
            }
        }

        internal void ApplyImportedBoostPolicyFlags()
        {
            OmniBoostPolicy.ApplyTo(this);
        }

        public PowerEntry? GetPowerEntry() => MidsContext.Character.CurrentBuild.Powers.FirstOrDefault(x => x is { Power: not null } && x.Power.DisplayName == DisplayName);

        public static bool ShouldIncludeDamageEffect(IEffect effect)
        {
            return effect.EffectType == Enums.eEffectType.Damage &&
                   (MidsContext.Config.DamageMath.Calculate != ConfigData.EDamageMath.Minimum ||
                    Math.Abs(effect.Probability) > 0.999000012874603) &&
                   effect.EffectClass != Enums.eEffectClass.Ignored &&
                   effect is not { DamageType: Enums.eDamage.Special, ToWho: Enums.eToWho.Self } &&
                   effect.Probability > 0 &&
                   effect.CanInclude() &&
                   effect.PvXInclude();
        }

        public static float GetDamageEffectBaseMagnitude(IEffect effect, IPower power, bool absolute, bool applyReturnScaling)
        {
            var effectMagnitude = absolute ? Math.Abs(effect.BuffedMag) : effect.BuffedMag;

            if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
            {
                effectMagnitude *= effect.Probability;
            }

            var recurrence = effect.PseudoPetRecurrence;
            if (recurrence is not { IsValid: true } && power.PowerType == Enums.ePowerType.Toggle && effect.isEnhancementEffect)
            {
                effectMagnitude = (float)(effectMagnitude * power.ActivatePeriod / 10d);
            }

            if (!applyReturnScaling)
            {
                return effectMagnitude;
            }

            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.DPS:
                    if (recurrence is { IsValid: true })
                    {
                        return effectMagnitude / recurrence.SourceUsageTime;
                    }

                    if (power is { PowerType: Enums.ePowerType.Toggle, ActivatePeriod: > 0 })
                    {
                        return effectMagnitude / power.ActivatePeriod;
                    }

                    if (power.RechargeTime + (double)power.CastTime + power.InterruptTime > 0)
                    {
                        return effectMagnitude / (power.RechargeTime + power.CastTime + power.InterruptTime);
                    }

                    break;

                case ConfigData.EDamageReturn.DPA:
                    if (recurrence is { IsValid: true })
                    {
                        return effectMagnitude / recurrence.SourceUsageTime;
                    }

                    if (power is { PowerType: Enums.ePowerType.Toggle, ActivatePeriod: > 0 })
                    {
                        return effectMagnitude / power.ActivatePeriod;
                    }

                    if (power.CastTime > 0)
                    {
                        return effectMagnitude / power.CastTime;
                    }

                    break;
            }

            return effectMagnitude;
        }

        public static float GetDamageEffectEffectiveTicks(IEffect effect)
        {
            var tickCount = effect.PseudoPetRecurrence is { IsValid: true } recurrence
                ? recurrence.TotalExpectedTicks
                : effect.Ticks;

            if (tickCount <= 1)
            {
                return Math.Max(1, tickCount);
            }

            return effect.CancelOnMiss &&
                   MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average &&
                   effect.Probability < 1
                ? (float)((1 - Math.Pow(effect.Probability, tickCount)) / (1 - effect.Probability))
                : tickCount;
        }

        public static float GetDamageEffectTotal(IEffect effect, IPower power, bool absolute, bool applyReturnScaling)
        {
            var baseMagnitude = GetDamageEffectBaseMagnitude(effect, power, absolute, applyReturnScaling);
            var ticks = GetDamageEffectEffectiveTicks(effect);
            return ticks > 1 ? baseMagnitude * ticks : baseMagnitude;
        }

        public static IReadOnlyList<IEffect> GetIncludedDamageEffects(IPower power, bool absorb = false)
        {
            return PrepareDamagePower(power, absorb).Effects
                .Where(ShouldIncludeDamageEffect)
                .ToArray();
        }

        private static bool HasPercentDamageDisplay(IPower power)
        {
            return power.Effects.Any(effect =>
                effect.EffectType == Enums.eEffectType.Damage &&
                (effect.DisplayPercentage || effect.Aspect == Enums.eAspect.Str));
        }

        private static float GetDamageDisplayMultiplier(IPower power)
        {
            if (!HasPercentDamageDisplay(power))
            {
                return 1f;
            }

            return Math.Max(1f, MidsContext.Character?.Totals.HPMax ?? 1f);
        }

        private static bool IsProcDamageEffect(IEffect effect)
        {
            return effect.isEnhancementEffect && (effect.IgnoreScaling || effect.IsFromProc);
        }

        private static string BuildDamageContributionLabel(IEffect effect)
        {
            if (effect.isEnhancementEffect)
            {
                var enhancementName = ResolveEnhancementContributionName(effect);
                if (!string.IsNullOrWhiteSpace(enhancementName))
                {
                    return enhancementName;
                }

                return effect.IsFromProc || effect.IgnoreScaling ? "Enhancement Proc" : "Enhancement Damage";
            }

            if (TryBuildArchetypeDamageContributionLabel(effect, out var archetypeLabel))
            {
                return archetypeLabel;
            }

            if (TryBuildTaggedCriticalContributionLabel(effect, out var taggedCriticalLabel))
            {
                return taggedCriticalLabel;
            }

            if (effect is Effect concreteEffect)
            {
                if (concreteEffect.HasPlannerModeCondition(PlannerMode.CriticalHit))
                {
                    return "Critical Hit";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.Assassination))
                {
                    return "Assassination";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.StalkerHidden))
                {
                    return "Hidden Strike";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.Containment))
                {
                    return "Containment";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.Scourge))
                {
                    return "Scourge";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.Domination))
                {
                    return "Domination";
                }

                if (concreteEffect.HasPlannerModeCondition(PlannerMode.PackMentality))
                {
                    return "Pack Mentality";
                }
            }

            if (effect.PvMode == Enums.ePvX.PvP)
            {
                return effect.OmniSource.Contains(":child[", StringComparison.OrdinalIgnoreCase)
                    ? "PvP Bonus Hit"
                    : "PvP Hit";
            }

            return "Base Hit";
        }

        private static string ResolveEnhancementContributionName(IEffect effect)
        {
            var enhancementName = effect.Enhancement?.Name?.Trim();
            if (IsUsableEnhancementContributionName(enhancementName))
            {
                return enhancementName;
            }

            var enhancementPowerName = effect.Enhancement?.GetPower()?.DisplayName?.Trim();
            if (IsUsableEnhancementContributionName(enhancementPowerName))
            {
                return enhancementPowerName;
            }

            var omniPowerName = TryResolveOmniSourcePowerDisplayName(effect.OmniSource);
            if (IsUsableEnhancementContributionName(omniPowerName))
            {
                return omniPowerName;
            }

            return string.Empty;
        }

        private static bool IsUsableEnhancementContributionName(string? candidate)
        {
            return !string.IsNullOrWhiteSpace(candidate) &&
                   !candidate.Equals("New Enhancement", StringComparison.OrdinalIgnoreCase);
        }

        private static string TryResolveOmniSourcePowerDisplayName(string omniSource)
        {
            if (string.IsNullOrWhiteSpace(omniSource))
            {
                return string.Empty;
            }

            var separatorIndex = omniSource.IndexOf(':');
            var fullName = separatorIndex >= 0
                ? omniSource[..separatorIndex]
                : omniSource;

            return DatabaseAPI.GetPowerByFullName(fullName)?.DisplayName ?? string.Empty;
        }

        private static string BuildDamageContributionDetail(IEffect effect)
        {
            if (TryBuildArchetypeDamageContributionDetail(effect, out var archetypeDetail))
            {
                return archetypeDetail;
            }

            var parts = new List<string>();

            var damageDescriptor = BuildDamageDescriptor(effect);
            if (!string.IsNullOrWhiteSpace(damageDescriptor))
            {
                parts.Add(damageDescriptor);
            }

            var chanceDescriptor = BuildDamageChanceDescriptor(effect);
            if (!string.IsNullOrWhiteSpace(chanceDescriptor))
            {
                parts.Add(chanceDescriptor);
            }

            var scopeDescriptor = BuildDamageScopeDescriptor(effect);
            if (!string.IsNullOrWhiteSpace(scopeDescriptor) &&
                (effect.isEnhancementEffect || effect.Probability < 1f || effect.PvMode == Enums.ePvX.Any))
            {
                parts.Add(scopeDescriptor);
            }

            var flagsDescriptor = BuildDamageFlagsDescriptor(effect);
            if (!string.IsNullOrWhiteSpace(flagsDescriptor))
            {
                parts.Add(flagsDescriptor);
            }

            return string.Join(", ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
        }

        private static bool TryBuildArchetypeDamageContributionLabel(IEffect effect, out string label)
        {
            label = string.Empty;
            if (effect is not Effect concreteEffect ||
                !effect.ModifierTable.Contains("InherentDamage", StringComparison.OrdinalIgnoreCase) ||
                !TryGetArchetypeCondition(concreteEffect, out var archetypeClass))
            {
                return false;
            }

            label = archetypeClass.Equals("Class_Scrapper", StringComparison.OrdinalIgnoreCase)
                ? "Scrapper Crit"
                : $"{FormatArchetypeClassName(archetypeClass)} Bonus";

            return !string.IsNullOrWhiteSpace(label);
        }

        private static bool TryBuildTaggedCriticalContributionLabel(IEffect effect, out string label)
        {
            label = string.Empty;
            var tags = effect.EffectTags ?? [];
            if (tags.Count == 0)
            {
                return false;
            }

            var hasScrapperCritTag = tags.Any(tag => tag.Contains("ScrapperCrit", StringComparison.OrdinalIgnoreCase));
            var hasGenericCritTag = hasScrapperCritTag ||
                                    tags.Any(tag =>
                                        tag.StartsWith("Crit", StringComparison.OrdinalIgnoreCase) ||
                                        tag.Contains("Critical", StringComparison.OrdinalIgnoreCase));
            if (!hasGenericCritTag)
            {
                return false;
            }

            label = hasScrapperCritTag ? "Scrapper Crit" : "Critical Hit";
            return true;
        }

        private static bool TryBuildArchetypeDamageContributionDetail(IEffect effect, out string detail)
        {
            detail = string.Empty;
            if (effect is not Effect concreteEffect ||
                !effect.ModifierTable.Contains("InherentDamage", StringComparison.OrdinalIgnoreCase) ||
                !TryGetArchetypeCondition(concreteEffect, out _))
            {
                return false;
            }

            var damageDescriptor = BuildDamageDescriptor(effect);
            var chanceDescriptor = BuildDamageChanceDescriptor(effect);
            var scopeDescriptor = BuildDamageScopeDescriptor(effect);
            detail = string.Join(", ",
                new[] { damageDescriptor, chanceDescriptor, scopeDescriptor }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));
            return !string.IsNullOrWhiteSpace(detail);
        }

        private static bool TryGetArchetypeCondition(Effect effect, out string archetypeClass)
        {
            archetypeClass = effect.AdvancedConditions?.Rows
                .FirstOrDefault(row =>
                    row.Kind == AdvancedConditionKind.CharacterArchetype &&
                    !row.Negated &&
                    row.Operator == AdvancedConditionOperator.Equals &&
                    !string.IsNullOrWhiteSpace(row.Value))
                ?.Value ?? string.Empty;
            return !string.IsNullOrWhiteSpace(archetypeClass);
        }

        private static string FormatArchetypeClassName(string archetypeClass)
        {
            var value = archetypeClass ?? string.Empty;
            if (value.StartsWith("Class_", StringComparison.OrdinalIgnoreCase))
            {
                value = value["Class_".Length..];
            }

            return value.Replace('_', ' ').Trim();
        }

        private static string BuildDamageDescriptor(IEffect effect)
        {
            var damageType = effect.DamageType == Enums.eDamage.None
                ? "Damage"
                : $"{Enums.GetDamageName(effect.DamageType)} Damage";

            if (effect.isEnhancementEffect && (effect.IsFromProc || effect.IgnoreScaling))
            {
                return $"{damageType} Proc";
            }

            return damageType;
        }

        private static string BuildDamageChanceDescriptor(IEffect effect)
        {
            if (effect.ProcsPerMinute > 0 && effect.Probability > 0 && effect.Probability < 1)
            {
                return $"{effect.ProcsPerMinute.ToString("0.##", CultureInfo.InvariantCulture)} PPM / {DisplayValueFormatter.FormatPercentFromScale(effect.Probability, 0)}% chance";
            }

            if (effect.Probability > 0 && effect.Probability < 1)
            {
                return $"{DisplayValueFormatter.FormatPercentFromScale(effect.Probability, 0)}% chance";
            }

            return string.Empty;
        }

        private static string BuildDamageScopeDescriptor(IEffect effect)
        {
            if (effect is Effect concreteEffect &&
                concreteEffect.AdvancedConditions is { Rows.Count: > 0 })
            {
                var targetRow = concreteEffect.AdvancedConditions.Rows.FirstOrDefault(row =>
                    row.Kind == AdvancedConditionKind.TargetEntityType &&
                    !row.Negated &&
                    row.Operator == AdvancedConditionOperator.Equals);

                var targetScopeDescriptor = targetRow?.TargetScope switch
                {
                    AdvancedConditionTargetScope.Player => "vs players",
                    AdvancedConditionTargetScope.Foe => "vs foes",
                    AdvancedConditionTargetScope.Ally => "for allies",
                    AdvancedConditionTargetScope.Self => "to self",
                    AdvancedConditionTargetScope.Pet => "for pets",
                    _ => string.Empty
                };

                if (!string.IsNullOrWhiteSpace(targetScopeDescriptor))
                {
                    return targetScopeDescriptor;
                }

                if (targetRow != null)
                {
                    var normalizedTargetValue = targetRow.Value.Trim('\'', '"');
                    if (normalizedTargetValue.Equals("critter", StringComparison.OrdinalIgnoreCase))
                    {
                        return "vs foes";
                    }

                    if (normalizedTargetValue.Equals("player", StringComparison.OrdinalIgnoreCase))
                    {
                        return "vs players";
                    }
                }
            }

            return effect.PvMode switch
            {
                Enums.ePvX.PvP => "vs players",
                Enums.ePvX.PvE => "vs foes",
                _ => string.Empty
            };
        }

        private static string BuildDamageFlagsDescriptor(IEffect effect)
        {
            if (effect.Buffable || effect.EffectType == Enums.eEffectType.DamageBuff)
            {
                return string.Empty;
            }

            return effect.IgnoreED
                ? "[Ignores Enhancements, Buffs & ED]"
                : "[Ignores Enhancements & Buffs]";
        }

        internal static string FormatDamageTooltip(
            DamageBreakdownSummary summary,
            IReadOnlyList<string>? compositionLines = null)
        {
            if (!summary.HasDamageEffects)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.Append("Total: ");
            builder.Append(DisplayValueFormatter.FormatNumber(summary.DisplayedTotal));

            if (summary.ByType.Count > 0)
            {
                builder.Append("\r\nBy Type: ");
                builder.Append(string.Join(", ",
                    summary.ByType.Select(contribution =>
                        $"{contribution.Label}: {DisplayValueFormatter.FormatNumber(contribution.DisplayedTotal)}")));
            }

            if (compositionLines is { Count: > 0 })
            {
                builder.Append("\r\n\r\nBreakdown:");
                foreach (var line in compositionLines.Where(line => !string.IsNullOrWhiteSpace(line)))
                {
                    builder.Append("\r\n- ");
                    builder.Append(line);
                }
            }

            if (summary.BySource.Count > 0)
            {
                builder.Append("\r\n\r\nHit Components:");
                foreach (var contribution in summary.BySource.Take(10))
                {
                    builder.Append("\r\n- ");
                    builder.Append(contribution.Label);
                    if (contribution.Occurrences > 1)
                    {
                        builder.Append(" x");
                        builder.Append(contribution.Occurrences);
                    }

                    builder.Append(": ");
                    builder.Append(DisplayValueFormatter.FormatNumber(contribution.DisplayedTotal));

                    if (!string.IsNullOrWhiteSpace(contribution.Detail))
                    {
                        builder.Append(" (");
                        builder.Append(contribution.Detail);
                        builder.Append(')');
                    }
                }

                if (summary.BySource.Count > 10)
                {
                    builder.Append("\r\n- ... ");
                    builder.Append(summary.BySource.Count - 10);
                    builder.Append(" more contribution");
                    if (summary.BySource.Count - 10 != 1)
                    {
                        builder.Append("s");
                    }
                }
            }

            return builder.ToString();
        }

        internal static DamageBreakdownSummary GetDamageBreakdown(IPower sourcePower, bool absorb = false)
        {
            var power = PrepareDamagePower(sourcePower, absorb);
            if (power.Effects.Length == 0)
            {
                return DamageBreakdownSummary.Empty;
            }

            var hasPercentDamage = HasPercentDamageDisplay(power);
            var hasDamageEffects = false;
            var displayMultiplier = hasPercentDamage ? GetDamageDisplayMultiplier(power) : 1f;
            var displayedTotal = 0f;
            var totalExcludingProc = 0f;
            var rawDisplayedTotal = 0f;
            var damageTotals = new Dictionary<Enums.eDamage, float>();
            var orderedDamageTypes = new List<Enums.eDamage>();
            var sourceContributions = new List<DamageSourceContribution>();

            foreach (var effect in power.Effects)
            {
                if (!ShouldIncludeDamageEffect(effect))
                {
                    continue;
                }

                hasDamageEffects = true;

                var effectTotal = GetDamageEffectTotal(effect, power, absolute: true, applyReturnScaling: true);
                if (Math.Abs(effectTotal) < 0.0001f)
                {
                    continue;
                }

                rawDisplayedTotal += effectTotal;
                displayedTotal += effectTotal;
                if (!IsProcDamageEffect(effect))
                {
                    totalExcludingProc += effectTotal;
                }

                if (!damageTotals.ContainsKey(effect.DamageType))
                {
                    damageTotals[effect.DamageType] = 0f;
                    orderedDamageTypes.Add(effect.DamageType);
                }

                damageTotals[effect.DamageType] += effectTotal;

                sourceContributions.Add(new DamageSourceContribution(
                    BuildDamageContributionLabel(effect),
                    BuildDamageContributionDetail(effect),
                    effectTotal,
                    effect.DamageType,
                    IsProcDamageEffect(effect),
                    1));
            }

            if (hasPercentDamage)
            {
                displayedTotal *= displayMultiplier;
                totalExcludingProc *= displayMultiplier;
            }

            var byType = orderedDamageTypes
                .Select(damageType =>
                {
                    var total = damageTotals[damageType];
                    if (hasPercentDamage)
                    {
                        total *= displayMultiplier;
                    }

                    return new DamageTypeContribution(damageType, total);
                })
                .Where(contribution => Math.Abs(contribution.DisplayedTotal) >= 0.0001f)
                .ToArray();

            var bySource = sourceContributions
                .Select(contribution => hasPercentDamage
                    ? contribution.Scale(displayMultiplier)
                    : contribution)
                .GroupBy(contribution => new
                {
                    contribution.Label,
                    contribution.Detail,
                    contribution.DamageType,
                    contribution.IsProc
                })
                .Select(group => new DamageSourceContribution(
                    group.Key.Label,
                    group.Key.Detail,
                    group.Sum(item => item.DisplayedTotal),
                    group.Key.DamageType,
                    group.Key.IsProc,
                    group.Sum(item => item.Occurrences)))
                .Where(contribution => Math.Abs(contribution.DisplayedTotal) >= 0.0001f)
                .OrderByDescending(contribution => Math.Abs(contribution.DisplayedTotal))
                .ThenBy(contribution => contribution.Label, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new DamageBreakdownSummary(
                DisplayedTotal: displayedTotal,
                TotalExcludingProc: totalExcludingProc,
                ByType: byType,
                BySource: bySource,
                HasDamageEffects: hasDamageEffects,
                HasPercentDamage: hasPercentDamage,
                PercentOfTargetHpTotal: hasPercentDamage ? rawDisplayedTotal * 100f : 0f,
                DisplayMultiplier: displayMultiplier);
        }

        private static bool ShouldProcessExecutesForDamage(IPower power)
        {
            return DatabaseAPI.GetPlannerRuleset().ShouldProcessExecutesInDamageHelpers(power);
        }

        private static bool HasExistingSummonAbsorption(IPower power)
        {
            if (power.HasAbsorbedEffects)
            {
                return true;
            }

            if (power.Effects.Length == 0)
            {
                return false;
            }

            var summonEffectIndexes = power.Effects
                .Select((effect, index) => new { effect, index })
                .Where(entry => entry.effect.EffectType is Enums.eEffectType.EntCreate or Enums.eEffectType.EntCreate_x)
                .Select(entry => entry.index)
                .ToHashSet();

            if (summonEffectIndexes.Count == 0)
            {
                return false;
            }

            var hasSummonAbsorption = power.Effects.Any(effect =>
                effect.Absorbed_Effect &&
                effect.Absorbed_EffectID >= 0 &&
                summonEffectIndexes.Contains(effect.Absorbed_EffectID));

            if (hasSummonAbsorption)
            {
                power.HasAbsorbedEffects = true;
            }

            return hasSummonAbsorption;
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
            var power = PrepareDamagePower(this, absorb);

            foreach (var effect in power.Effects)
            {
                if (!ShouldIncludeDamageEffect(effect))
                {
                    continue;
                }

                totalDamage += GetDamageEffectTotal(effect, power, absolute: false, applyReturnScaling: true);
            }

            return totalDamage;
        }

        internal DamageBreakdownSummary GetDamageBreakdown(bool absorb = false)
        {
            return GetDamageBreakdown((IPower)this, absorb);
        }

        public string GetDamageTip()
        {
            var summary = GetDamageBreakdown(this);
            return FormatDamageTooltip(summary);
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
            var power = PrepareDamagePower(this, absorb);

            // Check if any effects display percentage damage or have Strength aspect
            var hasPercentDamage = power.Effects
                .Any(e => e.EffectType == Enums.eEffectType.Damage && (e.DisplayPercentage || e.Aspect == Enums.eAspect.Str));

            // Iterate through each effect in the power
            foreach (var effect in power.Effects)
            {
                // Skip effects that do not meet various conditions
                if (!ShouldIncludeDamageEffect(effect))
                {
                    continue;
                }

                var effectMagnitude = GetDamageEffectBaseMagnitude(effect, power, absolute: true, applyReturnScaling: true);

                // Skip negligible effects
                if (Math.Abs(effectMagnitude) < 0.0001)
                {
                    continue;
                }

                var tickCount = GetDamageEffectEffectiveTicks(effect);
                if (tickCount > 1)
                {
                    var index = 0;
                    if (Math.Abs(tickDamageArray[(int)effect.DamageType, 0]) > 0.01)
                    {
                        index = 1;
                    }

                    tickDamageArray[(int)effect.DamageType, index] = effectMagnitude;
                    tickCountArray[(int)effect.DamageType, index] = tickCount;
                    totalDamage += effectMagnitude * tickCount;
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
                        ? $"{DisplayValueFormatter.FormatPercentFromScale(totalDamageArray[index])}%"
                        : DisplayValueFormatter.FormatNumber(totalDamageArray[index]);
                }

                if (Math.Abs(tickDamageArray[index, 0]) > 0.01)
                {
                    if (totalDamageArray[index] > 0)
                    {
                        damageEntry += "+";
                    }

                    damageEntry +=
                        $"{(hasPercentDamage ? $"{DisplayValueFormatter.FormatPercentFromScale(tickDamageArray[index, 0])}%" : DisplayValueFormatter.FormatNumber(tickDamageArray[index, 0]))}x{DisplayValueFormatter.FormatNumber(tickCountArray[index, 0])}";
                    if (Math.Abs(tickDamageArray[index, 1]) > 0.01)
                    {
                        damageEntry +=
                            $"+{(hasPercentDamage ? $"{DisplayValueFormatter.FormatPercentFromScale(tickDamageArray[index, 1])}%" : DisplayValueFormatter.FormatNumber(tickDamageArray[index, 1]))}x{DisplayValueFormatter.FormatNumber(tickCountArray[index, 1])}";
                    }
                }

                damageString += $"{damageEntry})";
            }

            // Return the final formatted damage string with total damage
            return
                $"{damageString} = {(hasPercentDamage ? $"{DisplayValueFormatter.FormatPercentFromScale(totalDamage)}% | {DisplayValueFormatter.FormatNumber(totalDamage * MidsContext.Character.Totals.HPMax)}" : DisplayValueFormatter.FormatNumber(totalDamage))}";
        }

        private static IPower PrepareDamagePower(IPower sourcePower, bool absorbRequested)
        {
            IPower power = new Power(sourcePower);
            var hasRedirectEffects = power.Effects.Any(effect =>
                effect.EffectType == Enums.eEffectType.PowerRedirect &&
                (effect.nOverride > -1 || !string.IsNullOrWhiteSpace(effect.Override)));
            if ((power.HasPowerOverrideEffect || hasRedirectEffects) && !power.AppliedPowersOverride)
            {
                power = PlannerEffectResolver.ResolvePower(power, new PlannerEffectResolutionContext(useRulesetDefaults: false)
                {
                    ApplyRedirects = true,
                    AbsorbPetEffects = false,
                    ExpandGrantPowers = false,
                    ExpandExecutePowers = false,
                    IncludeTrace = false,
                    MaxExpansionDepth = PlannerEffectResolutionContext.DefaultMaxExpansionDepth
                }).ResolvedPower;
            }

            var shouldAbsorbPseudoPets = DatabaseAPI.GetPlannerRuleset()
                .ShouldAbsorbPseudoPetEffectsForDamage(power, absorbRequested) &&
                !HasExistingSummonAbsorption(power);
            var shouldProcessExecutes = ShouldProcessExecutesForDamage(power);
            if (shouldAbsorbPseudoPets || shouldProcessExecutes)
            {
                power = PlannerEffectResolver.ResolvePower(power, new PlannerEffectResolutionContext(useRulesetDefaults: false)
                {
                    ApplyRedirects = false,
                    AbsorbPetEffects = shouldAbsorbPseudoPets,
                    ExpandGrantPowers = false,
                    ExpandExecutePowers = shouldProcessExecutes,
                    IncludeTrace = false,
                    MaxExpansionDepth = PlannerEffectResolutionContext.DefaultMaxExpansionDepth
                }).ResolvedPower;
            }

            return power;
        }

        public int[] GetRankedEffects(bool newMode)
        {
            // context
            bool pvpContext = MidsContext.Config.Inc.DisablePvE; // true => PvP, false => PvE

            // local helpers (pure, allocation-free)
            static int BaseWeight(Enums.eEffectClass cls) => ((int)cls) + 1;

            static int TypeWeight(IEffect fx)
            {
                // Per-type nudges roughly aligned with CoH salience:
                // Def/Res/ToHit/Recharge/Heal/Absorb above the fold; damage carrier & boilerplate below.
                return fx.EffectType switch
                {
                    Enums.eEffectType.None => -1000,
                    Enums.eEffectType.SetMode => -500,
                    Enums.eEffectType.Damage => -300,  // not used for "buff headline" in Mids
                    Enums.eEffectType.GrantPower => -40,
                    Enums.eEffectType.RevokePower => -40,
                    Enums.eEffectType.Translucency => -20,
                    Enums.eEffectType.Enhancement => 6,    // carrier; usually not the headline
                    Enums.eEffectType.DamageBuff => 10,
                    Enums.eEffectType.ToHit => 22,    // additive inside inner clamp
                    Enums.eEffectType.Defense => 28,
                    Enums.eEffectType.Resistance => 24,
                    Enums.eEffectType.Recovery => 18,
                    Enums.eEffectType.Endurance => 15,
                    Enums.eEffectType.HitPoints => 14,
                    Enums.eEffectType.Heal => 20,
                    Enums.eEffectType.Absorb => 16,    // shield-like
                    Enums.eEffectType.SpeedRunning => 5,
                    Enums.eEffectType.SpeedFlying => 5,
                    Enums.eEffectType.SpeedJumping => 5,
                    Enums.eEffectType.JumpHeight => 3,
                    Enums.eEffectType.MezProtect => 8,
                    Enums.eEffectType.MezResist => 8,
                    Enums.eEffectType.Mez when !fx.Buffable => -2,
                    Enums.eEffectType.Mez => (int)Math.Round(9f * fx.Probability),
                    _ => 9
                };
            }

            static int DirectionalNudge(IEffect fx)
            {
                int w = 0;
                if (fx.ToWho == Enums.eToWho.Self && (fx.BuffedMag > 0 || fx.EffectType == Enums.eEffectType.Mez || fx.EffectType == Enums.eEffectType.MezProtect)) w += 10;
                if (fx.ToWho == Enums.eToWho.Target && fx.BuffedMag < 0) w += 10;
                if (fx.Absorbed_Effect) w += 10; // baked-in via pet/absorbed often "feels" primary
                return w;
            }

            static int MagnitudeScore(IEffect fx)
            {
                // Normalize magnitudes onto a comparable integer scale by effect family.
                // These factors are conservative and can be tuned with telemetry later.
                float mag = Math.Abs(fx.BuffedMag);
                float scaled = fx.EffectType switch
                {
                    Enums.eEffectType.Defense => mag * 100f,   // e.g., 0.10 => 10
                    Enums.eEffectType.Resistance => mag * 100f,
                    Enums.eEffectType.ToHit => mag * 100f,
                    Enums.eEffectType.Recovery => mag * 50f,
                    Enums.eEffectType.Heal => mag * 50f,    // often already absolute
                    Enums.eEffectType.Absorb => mag * 50f,    // absolute or % flat upstream
                    Enums.eEffectType.DamageBuff => mag * 40f,
                    Enums.eEffectType.Endurance => mag * 40f,    // MaxEnd gets added elsewhere
                    Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.RechargeTime => mag * 120f, // Recharge is king
                    Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.ToHit => mag * 85f,
                    Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Accuracy => mag * 65f,   // multiplicative later
                    _ => mag * 25f
                };
                // Clamp to a sane band to avoid outliers dominating.
                scaled = Math.Min(scaled, 200f);
                return (int)Math.Round(scaled);
            }

            var ranked = new List<(int Index, int Weight)>(Effects.Length);

            for (int i = 0; i < Effects.Length; i++)
            {
                var fx = Effects[i];

                // Suppression: skip when current suppression overlaps effect's suppression flags.
                if ((MidsContext.Config.Suppression & fx.Suppression) != Enums.eSuppress.None)
                    continue;

                // PvE/PvP gating
                if ((pvpContext && fx.PvMode == Enums.ePvX.PvE) || (!pvpContext && fx.PvMode == Enums.ePvX.PvP))
                    continue;

                // Do NOT filter ToWho: Self, Target, All, Unspecified are all valid in Mids usage.

                int weight = 0;
                weight += BaseWeight(fx.EffectClass);

                // Certain > probabilistic
                if (Math.Abs(fx.Probability - 1f) < 0.01f) weight += 10;

                // Pet/absorbed baked-in effects (unless EntCreate itself)
                if (HasAbsorbedEffects && fx.EffectType != Enums.eEffectType.EntCreate) weight += 40;

                // Delay penalties
                if (fx.DelayedTime > 1) weight -= 100;
                else if (fx.DelayedTime > 0) weight -= 25;

                // Inherent specials are boilerplate
                if (fx.InherentSpecial) weight -= 80;
                if (fx.InherentSpecial2) weight -= 80;

                // Enhancement "carriers" less headline-worthy
                if (fx.isEnhancementEffect) weight -= 20;

                // Variables pop a bit more
                if (fx.VariableModified) weight += 25;

                // Per-type & directional nudges
                weight += TypeWeight(fx);
                weight += DirectionalNudge(fx);

                // Magnitude-aware score (normalized per type)
                weight += MagnitudeScore(fx);

                ranked.Add((i, weight));
            }

            var ordered = ranked
                .OrderByDescending(x => x.Weight)
                .ThenBy(x => x.Index) // stable
                .Select(x => x.Index);

            // newMode => full list (modern surfaces). false => legacy "top two".
            return newMode ? ordered.ToArray() : ordered.Take(2).ToArray();
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
            var durationEffectId = Effects.Any(e => e.EffectType == Enums.eEffectType.Mez)
                ? Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e =>
                        (e.Value.PvMode == Enums.ePvX.Any |
                         e.Value.PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE |
                         e.Value.PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE) &
                        e.Value.EffectType == Enums.eEffectType.Mez &
                        e.Value.EffectClass != Enums.eEffectClass.Ignored & e.Value.Duration > 0 &
                        e.Value.ValidateConditional() &
                        e.Value.Probability > float.Epsilon &
                        !DefiancePlanner.IsModernContributorEffect(e.Value))
                    .OrderByDescending(e => e.Value, new EffectDurationComparer())
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault()
                    .Key
                : Effects
                    .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                    .Where(e =>
                        (e.Value.PvMode == Enums.ePvX.Any |
                         e.Value.PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE |
                         e.Value.PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE) &
                        e.Value.EffectClass != Enums.eEffectClass.Ignored & e.Value.Duration > 0 &
                        e.Value.ValidateConditional() &
                        e.Value.Probability > float.Epsilon &
                        !DefiancePlanner.IsModernContributorEffect(e.Value))
                    .OrderByDescending(e => e.Value, new EffectDurationComparer())
                    .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                    .FirstOrDefault()
                    .Key;

            if (durationEffectId > -1)
            {
                return durationEffectId;
            }

            if (!FullName.Equals(PlannerStateCatalog.DominationPowerFullName, StringComparison.OrdinalIgnoreCase))
            {
                return -1;
            }

            // Planner Domination strips some live-only support plumbing. If that leaves no
            // conditionally-valid duration row, still surface the canonical active-buff
            // timing from the remaining imported payload so the info panel shows 90s.
            return Effects
                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                .Where(e =>
                    e.Value.EffectClass != Enums.eEffectClass.Ignored &&
                    e.Value.Duration > 0 &&
                    !DefiancePlanner.IsModernContributorEffect(e.Value))
                .OrderByDescending(e => e.Value, new EffectDurationComparer())
                .DefaultIfEmpty(new KeyValuePair<int, IEffect>(-1, new Effect()))
                .FirstOrDefault()
                .Key;
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
                      buffDebuff > 0 & fx.BuffedMag > 0) ||
                    (fx.Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None ||
                    !(fx.PvMode == ePvX | fx.PvMode == Enums.ePvX.Any))
                {
                    continue;
                }

                numArray[(int)fx.DamageType] += fx.BuffedMag;
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

                resists[(int)fx.DamageType] += fx.BuffedMag;
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

        public bool HasDamageBuffEffects()
        {
            return Effects.Any(t => t.EffectType == Enums.eEffectType.DamageBuff);
        }

        public bool HasDamageEffects()
        {
            if (Effects.Any(effect => effect.EffectType == Enums.eEffectType.PowerRedirect))
            {
                return GetIncludedDamageEffects(this).Count > 0;
            }

            return Effects.Any(t => t.EffectType == Enums.eEffectType.Damage);
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
                    Effects[iIndex].isEnhancementEffect ||
                    Effects[iIndex].Absorbed_Effect &&
                    Effects[iIndex].Absorbed_PowerType == Enums.ePowerType.GlobalBoost)
                {
                    continue;
                }

                if (iEffect == Enums.eEffectType.Mez && Effects[iIndex].ToWho != Enums.eToWho.Target)
                {
                    if ((Enums.eMez)subType == Effects[iIndex].MezType || subType < 0)
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

                var mag = Effects[iIndex].BuffedMag;
                var tickCopies = PlannerStackRules.GetPlannerVisibleCopyCount(Effects[iIndex], Effects[iIndex].Ticks);
                if (tickCopies > 1)
                {
                    mag *= tickCopies;
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

                var mag = fx.BuffedMag;
                var tickCopies = PlannerStackRules.GetPlannerVisibleCopyCount(fx, fx.Ticks);
                if (tickCopies > 1)
                {
                    mag *= tickCopies;
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
            var displayClassName = string.IsNullOrWhiteSpace(OmniDisplayClassName)
                ? DatabaseAPI.ResolveClassName()
                : OmniDisplayClassName;
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
                    shortFx.Add(iIndex, mag / 100f * DatabaseAPI.GetClassHitPoints(displayClassName));
                }
                else if (Effects[iIndex].EffectType is Enums.eEffectType.Heal or Enums.eEffectType.HitPoints)
                {
                    shortFx.Add(iIndex, (float)(mag / (double)DatabaseAPI.GetClassHitPoints(displayClassName) * 100));
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

        public bool IgnoreEnhancementAxis(EnhancementPolicyAxis axis)
        {
            return IgnoreEnhancementAxes.Length == 0 || IgnoreEnhancementAxes.All(candidate => candidate != axis);
        }

        public bool IgnoreBuffAxis(EnhancementPolicyAxis axis)
        {
            return IgnoreBuffEnhancementAxes.Length == 0 || IgnoreBuffEnhancementAxes.All(candidate => candidate != axis);
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

        public int[] AbsorbEffects(IPower? source, float nDuration, float nDelay, Archetype? archetype, int stacking, bool isGrantPower = false, int fxid = -1, int effectId = -1)
        {
            var num1 = -1;
            var length = Effects.Length;
            var array = Array.Empty<int>();
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
                for (var index = 0; index < source.Effects.Length; index++)
                {
                    if (!isGrantPower & source.EntitiesAffected == Enums.eEntity.Caster & source.Effects[index].EffectType != Enums.eEffectType.EntCreate)
                    {
                        continue;
                    }

                    if (source.Effects[index].EffectType == Enums.eEffectType.EntCreate && source.Effects[index].nSummon > -1)
                    {
                        Array.Resize(ref array, array.Length + 1);
                        array[^1] = index;
                    }

                    num1++;
                    var effects = Effects;
                    Array.Resize(ref effects, num1 + length + 1);
                    Effects = effects;
                    var effect = (IEffect)source.Effects[index].Clone();
                    effect.AddResolvedEffectKind(PlannerResolvedEffectKind.Base);
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
                        var copyCount = PlannerStackRules.GetPlannerVisibleCopyCount(effect, stacking);
                        if (copyCount > 1)
                        {
                            effect.Scale *= copyCount;
                        }
                    }

                    if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet & (source.EntitiesAutoHit & Enums.eEntity.Caster) != Enums.eEntity.Caster)
                    {
                        effect.ToWho = Enums.eToWho.Target;
                        var copyCount = PlannerStackRules.GetPlannerVisibleCopyCount(effect, stacking);
                        if (copyCount > 1)
                        {
                            effect.Scale *= copyCount;
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
                    array[^1] = effectId;
                }

                var num3 = num1 + 1;
                var effects = Effects;
                Array.Resize(ref effects, num3 + length + 1);
                Effects = effects;
                var effect = (IEffect)source.Effects[effectId].Clone();
                effect.AddResolvedEffectKind(PlannerResolvedEffectKind.Base);
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
                    var copyCount = PlannerStackRules.GetPlannerVisibleCopyCount(effect, stacking);
                    if (copyCount > 1)
                    {
                        effect.Scale *= copyCount;
                    }
                }

                if ((source.EntitiesAutoHit & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
                {
                    effect.ToWho = Enums.eToWho.Target;
                    var copyCount = PlannerStackRules.GetPlannerVisibleCopyCount(effect, stacking);
                    if (copyCount > 1)
                    {
                        effect.Scale *= copyCount;
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
            PlannerEffectResolver.ExpandEffects(this, new PlannerEffectResolutionContext(useRulesetDefaults: false)
            {
                ApplyRedirects = false,
                AbsorbPetEffects = false,
                ExpandGrantPowers = true,
                ExpandExecutePowers = true,
                IncludeTrace = false,
                MaxExpansionDepth = PlannerEffectResolutionContext.DefaultMaxExpansionDepth
            });
        }

        public List<int> GetValidEnhancements(Enums.eType iType, int iSubType = 0)
        {
            return DatabaseAPI.Database.Enhancements
                .Select((enhancement, index) => new { enhancement, index })
                .Where(e => e.enhancement.TypeID == iType &&
                            (e.enhancement.SubTypeID == 0 || iSubType == 0 || e.enhancement.SubTypeID == iSubType) &&
                            DatabaseAPI.ValidateEnhancementForPower(this, e.index).IsValid)
                .Select(e => e.index)
                .ToList();
        }

        public bool IsEnhancementValid(int iEnh)
        {
            return DatabaseAPI.ValidateEnhancementForPower(this, iEnh).IsValid;
        }

        public void AbsorbPetEffects(int hIdx = -1, int stackingOverride = -1, bool pseudoOnly = false)
        {
            if (!AbsorbSummonAttributes && !AbsorbSummonEffects)
            {
                return;
            }
            var intList = new List<int>();
            for (var index = 0; index < Effects.Length; index++)
            {
                if (Effects[index].EffectType != Enums.eEffectType.EntCreate ||
                    Effects[index].nSummon < 0 ||
                    Effects[index].nSummon >= DatabaseAPI.Database.Entities.Length ||
                    Effects[index].Probability <= 0 ||
                    (!pseudoOnly && Math.Abs(Effects[index].Probability - 1) >= 0.01))
                {
                    continue;
                }

                var entity = DatabaseAPI.Database.Entities[Effects[index].nSummon];
                if (pseudoOnly && entity is not { IsPseudoPet: true })
                {
                    continue;
                }

                intList.Add(index);
            }

            if (intList.Count > 0)
            {
                HasAbsorbedEffects = true;
            }

            foreach (var t in intList)
            {
                var effect = Effects[t];
                var nSummon1 = effect.nSummon;
                var absorbDuration = GetAbsorbedSummonDuration(effect, pseudoOnly);
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
                            IgnoreBuffEnhancementAxes = EnhancementPolicyAxes.Normalize(power.IgnoreBuffEnhancementAxes);
                            IgnoreEnhancementAxes = EnhancementPolicyAxes.Normalize(power.IgnoreEnhancementAxes);
                            TypedEnhancementRestrictions = power.TypedEnhancementRestrictions
                                .Where(restriction => restriction.IsValid)
                                .ToArray();
                            MaxTargets = power.MaxTargets;
                            Radius = power.Radius;
                            Target = power.Target;
                            //ActivatePeriod = power.ActivatePeriod;
                            if (PowerIndex < 0 ||
                                PowerIndex >= DatabaseAPI.Database.Power.Length ||
                                DatabaseAPI.Database.Power[PowerIndex].EntitiesAutoHit is Enums.eEntity.None or Enums.eEntity.Caster)
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
                        var classIndex = DatabaseAPI.Database.Entities[nSummon1].GetNClassId();
                        if (classIndex < 0 || classIndex >= DatabaseAPI.Database.Classes.Length)
                        {
                            continue;
                        }

                        var startIndex = Effects.Length;
                        var absorbedEntCreates = AbsorbEffects(power1, absorbDuration, effect.DelayedTime, DatabaseAPI.Database.Classes[classIndex], stacking);
                        ApplySummonWrapperMetadata(
                            effect,
                            startIndex,
                            absorbDuration,
                            CreatePseudoPetRecurrence(effect, DatabaseAPI.Database.Entities[nSummon1], power1, pseudoOnly),
                            pseudoOnly ? PlannerResolvedEffectKind.PseudoPetChild : PlannerResolvedEffectKind.None);
                        foreach (var absorbEffect in absorbedEntCreates)
                        {
                            var nSummon2 = power1.Effects[absorbEffect].nSummon;
                            var nestedPowersetIndices = nSummon2 >= 0 && nSummon2 < DatabaseAPI.Database.Entities.Length
                                ? DatabaseAPI.Database.Entities[nSummon2].GetNPowerset()
                                : Array.Empty<int>();
                            if (nSummon2 < 0 ||
                                nSummon2 >= DatabaseAPI.Database.Entities.Length ||
                                nestedPowersetIndices.Count == 0 ||
                                nestedPowersetIndices[0] < 0 ||
                                nestedPowersetIndices[0] >= DatabaseAPI.Database.Powersets.Length)
                            {
                                continue;
                            }

                            foreach (var power2 in DatabaseAPI.Database.Powersets[nestedPowersetIndices[0]].Powers)
                            {
                                var nestedStartIndex = Effects.Length;
                                AbsorbEffects(power2, absorbDuration, effect.DelayedTime, DatabaseAPI.Database.Classes[classIndex], stacking);
                                ApplySummonWrapperMetadata(
                                    effect,
                                    nestedStartIndex,
                                    absorbDuration,
                                    CreatePseudoPetRecurrence(effect, DatabaseAPI.Database.Entities[nSummon1], power2, pseudoOnly),
                                    pseudoOnly
                                        ? PlannerResolvedEffectKind.PseudoPetChild | PlannerResolvedEffectKind.DeliveryChild
                                        : PlannerResolvedEffectKind.None);
                            }
                        }
                    }
                }

                AbsorbedPetEffects = true;
            }
        }

        private float GetAbsorbedSummonDuration(IEffect wrapper, bool pseudoOnly)
        {
            return wrapper.Duration;
        }

        private PseudoPetRecurrenceInfo? CreatePseudoPetRecurrence(IEffect wrapper, SummonedEntity entity, IPower petPower, bool pseudoOnly)
        {
            if (!pseudoOnly ||
                entity is not { IsPseudoPet: true })
            {
                return null;
            }

            var recurrenceWindow = wrapper.Duration > 0
                ? wrapper.Duration
                : petPower.ActivatePeriod > 0
                    ? petPower.ActivatePeriod
                    : 0f;
            var spawnCount = PowerType == Enums.ePowerType.Toggle &&
                             UsageTime > 0 &&
                             ActivatePeriod > 0
                ? (int)Math.Floor(UsageTime / ActivatePeriod)
                : 1;
            var sourceUsageTime = UsageTime > 0
                ? UsageTime
                : ActivatePeriod > 0
                    ? ActivatePeriod
                    : recurrenceWindow;
            var sourceActivatePeriod = ActivatePeriod > 0 ? ActivatePeriod : recurrenceWindow;
            var petTickInterval = petPower.ActivatePeriod > 0 ? petPower.ActivatePeriod : recurrenceWindow;
            var ticksPerSpawn = petPower.ActivatePeriod > 0 && wrapper.Duration > 0
                ? 1 + (int)Math.Floor(wrapper.Duration / petPower.ActivatePeriod)
                : 1;
            if (spawnCount <= 0 || ticksPerSpawn <= 0)
            {
                return null;
            }

            if (sourceUsageTime <= 0 || sourceActivatePeriod <= 0 || petTickInterval <= 0)
            {
                return null;
            }

            return new PseudoPetRecurrenceInfo
            {
                EntityName = string.IsNullOrWhiteSpace(entity.DisplayName) ? entity.UID : entity.DisplayName,
                PetPowerName = string.IsNullOrWhiteSpace(petPower.DisplayName) ? petPower.FullName : petPower.DisplayName,
                SourceUsageTime = sourceUsageTime,
                SourceActivatePeriod = sourceActivatePeriod,
                EntCreateDuration = wrapper.Duration > 0 ? wrapper.Duration : petTickInterval,
                PetTickInterval = petTickInterval,
                SpawnCount = spawnCount,
                TicksPerSpawn = ticksPerSpawn,
                TotalExpectedTicks = spawnCount * ticksPerSpawn
            };
        }

        private void ApplySummonWrapperMetadata(
            IEffect wrapper,
            int startIndex,
            float absorbedDuration,
            PseudoPetRecurrenceInfo? recurrence = null,
            PlannerResolvedEffectKind resultKind = PlannerResolvedEffectKind.None)
        {
            for (var index = startIndex; index < Effects.Length; index++)
            {
                var child = Effects[index];
                if (resultKind != PlannerResolvedEffectKind.None)
                {
                    child.AddResolvedEffectKind(resultKind);
                }

                child.BaseProbability = Math.Max(0, Math.Min(1, child.BaseProbability * wrapper.BaseProbability));
                if (wrapper.ProcsPerMinute > 0 && child.ProcsPerMinute <= 0)
                {
                    child.ProcsPerMinute = wrapper.ProcsPerMinute;
                }

                if (absorbedDuration > 0)
                {
                    child.Absorbed_Duration = absorbedDuration;
                }

                if (recurrence is { IsValid: true })
                {
                    child.PseudoPetRecurrence = recurrence;
                }

                if (wrapper.AdvancedConditions is { Rows.Count: > 0 })
                {
                    child.AdvancedConditions = PlannerEffectResolver.MergeConditions(wrapper.AdvancedConditions, child.AdvancedConditions);
                    child.NormalizeConditionState();
                }

                foreach (var tag in wrapper.EffectTags.Where(tag => !string.IsNullOrWhiteSpace(tag)))
                {
                    if (!child.EffectTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                    {
                        child.EffectTags.Add(tag);
                    }
                }

                if (!string.IsNullOrWhiteSpace(wrapper.EffectId) && string.IsNullOrWhiteSpace(child.EffectId))
                {
                    child.EffectId = wrapper.EffectId;
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
                              (PlannerStackRules.GetDisplayGroupingKey(iPower.Effects[iSfx.Index[index1]]) ==
                               PlannerStackRules.GetDisplayGroupingKey(iPower.Effects[array[index3].Index[0]])) &
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
                iDamage[(int)iPower.Effects[iSfx.Index[index]].DamageType] = true;
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
                                requirement2.ClassNameNot[^1] =
                                    DatabaseAPI.Database.Classes[index2].ClassName;
                                iReq = iReq.Replace(oldValue2, "true");
                            }
                            else if (iReq.Contains(oldValue1))
                            {
                                Array.Resize(ref requirement2.ClassName, requirement2.ClassName.Length + 1);
                                requirement2.ClassName[^1] =
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
                        var num = (int)MessageBox.Show(str + "\n\niReq: " + iReq +
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

        public void ProcessExecutes()
        {
            PlannerEffectResolver.ExpandEffects(this, new PlannerEffectResolutionContext(useRulesetDefaults: false)
            {
                ApplyRedirects = false,
                AbsorbPetEffects = false,
                ExpandGrantPowers = false,
                ExpandExecutePowers = true,
                IncludeTrace = false,
                MaxExpansionDepth = PlannerEffectResolutionContext.DefaultMaxExpansionDepth
            });
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
        public string BuildTooltipStringAllVectorsEffects(Enums.eEffectType effectType, string groupName = "", bool includeEnhEffects = false, bool activeOnly = true)
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

            var effects = activeOnly
                ? Effects
                    .Where(e => e.PvXInclude())
                    .ToArray()
                : Effects;
            var effectsList = string.Empty;
            var pvModes = new List<Enums.ePvX> { Enums.ePvX.Any, Enums.ePvX.PvE, Enums.ePvX.PvP };

            foreach (var pvMode in pvModes)
            {
                // Select identifiers from effects
                var effectIdentifiers2 = effects
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
                    var effectsVectors = effects
                        .Where(e => (includeEnhEffects || !e.isEnhancementEffect) & e.GenerateIdentifier().Compare(effectId))
                        .Select(e => e.DamageType)
                        .Distinct()
                        .ToList();

                    // Check if effects contains all the effectType vectors (listed above)
                    var effectsInMode = string.Empty;
                    if (!damageVectors.Except(effectsVectors).Any())
                    {
                        var effectsInModeBase = effects
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
                        var effectsInModeBase = effects
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
                            Enums.eEffectType.Mez or Enums.eEffectType.MezProtect or Enums.eEffectType.MezResist => $"{effectType}({mezType})",
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
                extraAttribs.Add($"Range: {DisplayValueFormatter.FormatDistance(subPower.Range, 2)}ft");
            }

            if (Math.Abs(subPower.RangeSecondary - RangeSecondary) > float.Epsilon && subPower.RangeSecondary > float.Epsilon)
            {
                extraAttribs.Add($"Secondary Range: {DisplayValueFormatter.FormatDistance(subPower.RangeSecondary, 2)}ft");
            }

            if (Math.Abs(subPower.Radius - Radius) > float.Epsilon && subPower.Radius > float.Epsilon)
            {
                extraAttribs.Add($"Radius: {DisplayValueFormatter.FormatDistance(subPower.Radius, 2)}ft");
            }

            if (subPower.Arc != Arc && subPower.Arc > float.Epsilon)
            {
                extraAttribs.Add($"Arc: {DisplayValueFormatter.FormatNumber(subPower.Arc, 2)}deg");
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

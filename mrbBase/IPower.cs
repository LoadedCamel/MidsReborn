#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using mrbBase.Base.Data_Classes;

namespace mrbBase
{
    public interface IPower : IComparable
    {
        bool HasProcSlotted { get; set; }
        string FullSetName { get; }

        float CastTime { get; set; }

        float CastTimeReal { get; set; }

        float ToggleCost { get; }

        bool IsEpic { get; }

        bool Slottable { get; }

        int LocationIndex { get; }

        bool IsModified { get; set; }

        bool IsNew { get; set; }

        int PowerIndex { get; set; }

        int PowerSetID { get; set; }

        int PowerSetIndex { get; set; }

        bool HasAbsorbedEffects { get; set; }

        int StaticIndex { get; set; }

        int[] NGroupMembership { get; set; }

        string FullName { get; set; }

        string GroupName { get; set; }

        string SetName { get; set; }

        string PowerName { get; set; }

        string DisplayName { get; set; }

        int Available { get; set; }

        Requirement Requires { get; set; }

        Enums.eModeFlags ModesRequired { get; set; }

        Enums.eModeFlags ModesDisallowed { get; set; }

        Enums.ePowerType PowerType { get; set; }

        float Accuracy { get; set; }

        float AccuracyMult { get; set; }

        Enums.eVector AttackTypes { get; set; }

        string[] GroupMembership { get; set; }

        Enums.eEntity EntitiesAffected { get; set; }

        Enums.eEntity EntitiesAutoHit { get; set; }

        Enums.eEntity Target { get; set; }

        bool TargetLoS { get; set; }

        float Range { get; set; }

        Enums.eEntity TargetSecondary { get; set; }

        float RangeSecondary { get; set; }

        float EndCost { get; set; }

        float InterruptTime { get; set; }

        float RechargeTime { get; set; }

        float BaseRechargeTime { get; set; }

        float ActivatePeriod { get; set; }

        Enums.eEffectArea EffectArea { get; set; }

        float Radius { get; set; }

        float AoEModifier { get; }

        int Arc { get; set; }

        int MaxTargets { get; set; }

        string MaxBoosts { get; set; }

        Enums.eCastFlags CastFlags { get; set; }

        Enums.eNotify AIReport { get; set; }

        int NumCharges { get; set; }

        int UsageTime { get; set; }

        int LifeTime { get; set; }

        int LifeTimeInGame { get; set; }

        int NumAllowed { get; set; }

        bool DoNotSave { get; set; }

        string[] BoostsAllowed { get; set; }

        int[] Enhancements { get; set; }

        bool CastThroughHold { get; set; }

        bool IgnoreStrength { get; set; }

        string DescShort { get; set; }

        string DescLong { get; set; }

        bool SortOverride { get; set; }

        bool HiddenPower { get; set; }

        Enums.eSetType[] SetTypes { get; set; }

        bool ClickBuff { get; set; }

        bool AlwaysToggle { get; set; }

        int Level { get; set; }

        bool AllowFrontLoading { get; set; }

        bool VariableEnabled { get; set; }

        bool VariableOverride { get; set; }

        string VariableName { get; set; }

        int VariableMin { get; set; }

        int VariableMax { get; set; }

        int VariableStart { get; set; }

        int[] NIDSubPower { get; set; }

        string[] UIDSubPower { get; set; }

        bool SubIsAltColor { get; set; }

        Enums.eEnhance[] IgnoreEnh { get; set; }

        Enums.eEnhance[] Ignore_Buff { get; set; }

        bool SkipMax { get; set; }

        int DisplayLocation { get; set; }

        bool MutexAuto { get; set; }

        bool MutexIgnore { get; set; }

        bool AbsorbSummonEffects { get; set; }

        bool AbsorbSummonAttributes { get; set; }

        bool ShowSummonAnyway { get; set; }

        bool NeverAutoUpdate { get; set; }

        bool NeverAutoUpdateRequirements { get; set; }

        bool IncludeFlag { get; set; }

        bool BoostBoostable { get; set; }

        bool BoostUsePlayerLevel { get; set; }

        string ForcedClass { get; set; }

        Enums.eGridType InherentType { get; set; }

        int ForcedClassID { get; set; }

        IEffect[] Effects { get; set; }

        Enums.eBuffMode BuffMode { get; set; }

        bool HasGrantPowerEffect { get; set; }

        bool HasPowerOverrideEffect { get; set; }
        IPowerset GetPowerSet();

        void StoreTo(ref BinaryWriter writer);

        PowerEntry? GetPowerEntry();

        float FXGetDamageValue();

        string FXGetDamageString();

        int[] GetRankedEffects();

        int GetDurationEffectID();

        float[] GetDef(int buffDebuff = 0);

        float[] GetRes(bool pvE = true);

        bool HasMutexID(int index);

        bool Active { get; set; }

        bool Taken { get; set; }
    
        int Stacks { get; set; }

        int InternalStacks { get; }

        int VirtualStacks { get; set; }

        bool HasDefEffects();

        bool HasResEffects();

        bool HasAttribModEffects();

        Enums.ShortFX GetEnhancementMagSum(Enums.eEffectType iEffect, int subType = 0);

        Enums.ShortFX GetEffectMagSum(Enums.eEffectType iEffect, bool includeDelayed = false, bool onlySelf = false, bool onlyTarget = false, bool maxMode = false);
        
        Enums.ShortFX GetDamageMagSum(
            Enums.eEffectType iEffect,
            Enums.eDamage iSub,
            bool includeDelayed = false);

        Enums.ShortFX GetEffectMag(Enums.eEffectType iEffect, Enums.eToWho iTarget = Enums.eToWho.Unspecified, bool allowDelay = false);

        bool AffectsTarget(Enums.eEffectType iEffect);

        bool AffectsSelf(Enums.eEffectType iEffect);

        bool I9FXPresentP(Enums.eEffectType iEffect, Enums.eMez iMez = Enums.eMez.None);

        bool IgnoreEnhancement(Enums.eEnhance iEffect);

        bool IgnoreBuff(Enums.eEnhance iEffect);

        void SetMathMag();

        bool GetEffectStringGrouped(
            int idEffect,
            ref string returnString,
            ref int[] returnMask,
            bool shortForm,
            bool simple,
            bool noMag = false,
            bool fromPopup = false,
            bool ignoreConditions = false);

        int[] AbsorbEffects(
            IPower source,
            float nDuration,
            float nDelay,
            Archetype archetype,
            int stacking,
            bool isGrantPower = false,
            int fxid = -1,
            int effectId = -1);

        void ApplyGrantPowerEffects();

        List<int> GetValidEnhancements(Enums.eType iType, Enums.eSubtype iSubType = Enums.eSubtype.None);

        bool IsEnhancementValid(int iEnh);

        void AbsorbPetEffects(int hIdx = -1, int stackingOverride = -1);

        bool AllowedForClass(int classId);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core
{
    public interface IEffect : IComparable, ICloneable
    {
        double Rand { get; }

        int UniqueID { get; set; }

        float Probability { get; set; }

        bool HasConditions { get; }

        string ConditionIdentity { get; }

        float Mag { get; }

        float BuffedMag { get; }

        float MagPercent { get; }

        float Duration { get; }

        bool DisplayPercentage { get; }

        bool VariableModified { get; set; }

        bool InherentSpecial { get; }

        bool InherentSpecial2 { get; }

        float BaseProbability { get; set; }

        bool IgnoreED { get; set; }

        string Reward { get; set; }

        string EffectId { get; set; }

        List<string> EffectTags { get; set; }

        string OmniSource { get; set; }
        bool GrantBoosted { get; set; }
        bool UseCombatModMagnitude { get; set; }
        bool UseCombatModDuration { get; set; }

        string Special { get; set; }

        IEnhancement Enhancement { get; set; }

        int nID { get; set; }

        Enums.eEffectClass EffectClass { get; set; }

        Enums.eEffectType EffectType { get; set; }

        Enums.eOverrideBoolean DisplayPercentageOverride { get; set; }

        Enums.eDamage DamageType { get; set; }

        Enums.eMez MezType { get; set; }

        Enums.eEffectType ETModifies { get; set; }

        string Summon { get; set; }

        int nSummon { get; set; }

        int Ticks { get; set; }

        float DelayedTime { get; set; }

        Enums.eStacking Stacking { get; set; }

        Enums.eSuppress Suppression { get; set; }

        bool Buffable { get; set; }

        bool Resistible { get; set; }

        string UIDClassName { get; set; }

        int nIDClassName { get; set; }

        bool VariableModifiedOverride { get; set; }

        bool IgnoreScaling { get; set; }

        bool isEnhancementEffect { get; set; }

        Enums.ePvX PvMode { get; set; }

        Enums.eToWho ToWho { get; set; }

        float Scale { get; set; }

        float nMagnitude { get; set; }

        float nDuration { get; set; }

        Enums.eAttribType AttribType { get; set; }

        Enums.eAspect Aspect { get; set; }

        string ModifierTable { get; set; }

        string PowerFullName { get; set; }

        bool NearGround { get; set; }

        bool RequiresToHitCheck { get; set; }

        float Math_Mag { get; set; }

        float Math_Duration { get; set; }

        bool Absorbed_Effect { get; set; }

        Enums.ePowerType Absorbed_PowerType { get; set; }

        int Absorbed_Power_nID { get; set; }

        float Absorbed_Duration { get; set; }

        int Absorbed_Class_nID { get; set; }

        float Absorbed_Interval { get; set; }

        int Absorbed_EffectID { get; set; }

        PseudoPetRecurrenceInfo? PseudoPetRecurrence { get; set; }

        Enums.eBuffMode buffMode { get; set; }

        string Override { get; set; }

        int nOverride { get; set; }

        string ModeName { get; set; }

        int ModeId { get; set; }

        Enums.eModeFlags ModeFlag { get; set; }

        string RevokedPower { get; set; }

        Expressions Expressions { get; set; }

        string MagnitudeExpression { get; set; }

        bool CancelOnMiss { get; set; }

        float ProcsPerMinute { get; set; }

        AdvancedConditionSet AdvancedConditions { get; set; }
        bool Validated { get; set; }
        bool IsFromProc { get; }
        IPower? GetPower();
        void SetPower(IPower? power);

        bool isDamage();

        bool ValidateConditional();
        bool ValidateConditional(string powerName);
        bool ValidateConditional(string cType, string powerName);
        bool ValidateConditional(int index);

        void NormalizeConditionState();

        string BuildEffectStringShort(bool NoMag = false, bool simple = false, bool useBaseProbability = false);

        string BuildEffectString(bool Simple = false, string SpecialCat = "", bool noMag = false, bool Grouped = false, bool useBaseProbability = false, bool fromPopup = false, bool editorDisplay = false, bool dvDisplay = false, bool ignoreConditions = false);

        void StoreTo(ref BinaryWriter writer);

        //bool ImportFromCSV(string iCSV);

        int SetTicks(float iDuration, float iInterval);

        bool CanInclude();

        bool CanGrantPower();

        bool PvXInclude();

        string SummonedEntityName { get; }

        EffectIdentifier GenerateIdentifier();

        bool AffectsPetsOnly();
        
        Damage GetDamage();

        public float MinProcChance { get; }
    }
}

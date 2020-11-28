using System;
using System.Collections.Generic;
using System.IO;
using Base.Data_Classes;

public interface IEffect : IComparable, ICloneable
{
    int UniqueID { get; set; }

    float Probability { get; set; }

    float Mag { get; }

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

    string Special { get; set; }

    IEnhancement Enhancement { get; set; }

    int nID { get; set; }

    Enums.eEffectClass EffectClass { get; set; }

    Enums.eEffectType EffectType { get; set; }

    Enums.ePowerAttribs PowerAttribs { get; set; }

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

    Enums.eSpecialCase SpecialCase { get; set; }

    string UIDClassName { get; set; }

    int nIDClassName { get; set; }

    bool VariableModifiedOverride { get; set; }

    bool isEnhancementEffect { get; set; }

    Enums.ePvX PvMode { get; set; }

    Enums.eToWho ToWho { get; set; }

    float Scale { get; set; }

    float nMagnitude { get; set; }

    float nDuration { get; set; }

    Enums.eAttribType AttribType { get; set; }

    Enums.eAspect Aspect { get; set; }

    string ModifierTable { get; set; }

    int nModifierTable { get; set; }

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

    Enums.eBuffMode buffMode { get; set; }

    string Override { get; set; }

    int nOverride { get; set; }

    string MagnitudeExpression { get; set; }

    bool CancelOnMiss { get; set; }

    float ProcsPerMinute { get; set; }

    float AtrOrigAccuracy { get; set; }
    float AtrOrigActivatePeriod { get; set; }
    int AtrOrigArc { get; set; }
    float AtrOrigCastTime { get; set; }
    Enums.eEffectArea AtrOrigEffectArea { get; set; }
    float AtrOrigEnduranceCost { get; set; }
    float AtrOrigInterruptTime { get; set; }
    int AtrOrigMaxTargets { get; set; }
    float AtrOrigRadius { get; set; }
    float AtrOrigRange { get; set; }
    float AtrOrigRechargeTime { get; set; }
    float AtrOrigSecondaryRange { get; set; }

    float AtrModAccuracy { get; set; }
    float AtrModActivatePeriod { get; set; }
    int AtrModArc { get; set; }
    float AtrModCastTime { get; set; }
    Enums.eEffectArea AtrModEffectArea { get; set; }
    float AtrModEnduranceCost { get; set; }
    float AtrModInterruptTime { get; set; }
    int AtrModMaxTargets { get; set; }
    float AtrModRadius { get; set; }
    float AtrModRange { get; set; }
    float AtrModRechargeTime { get; set; }
    float AtrModSecondaryRange { get; set; }
    List<KeyValue<string, string>> ActiveConditionals { get; set; }  
    bool Validated { get; set; }
    IPower GetPower();
    void SetPower(IPower power);

    bool isDamage();

    bool ValidateConditional(string powername);
    bool ValidateConditional(string ctype = "", string powername = "");

    string BuildEffectStringShort(bool NoMag = false, bool simple = false, bool useBaseProbability = false);

    string BuildEffectString(
        bool Simple = false,
        string SpecialCat = "",
        bool noMag = false,
        bool Grouped = false,
        bool useBaseProbability = false);

    void StoreTo(ref BinaryWriter writer);

    bool ImportFromCSV(string iCSV);

    int SetTicks(float iDuration, float iInterval);

    bool CanInclude();

    bool CanGrantPower();

    bool PvXInclude();
}
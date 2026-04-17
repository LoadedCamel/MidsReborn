using Mids_Reborn.Core.Base.Master_Classes;
using System.Text.RegularExpressions;
using static Mids_Reborn.Core.Expressions;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Effect : IEffect, IComparable, ICloneable
    {
        private const string AdvancedConditionsMarker = "MRB_ADVANCED_EFFECT_CONDITIONS";
        private static readonly Regex UidClassRegex = new("arch source(.owner)?> (Class_[^ ]*)", RegexOptions.IgnoreCase);

        private IPower? power;

        public double Rand => new Random().NextDouble();

        public Effect()
        {
            Validated = false;
            BaseProbability = 1f;
            Expressions = new Expressions();
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
            nModifierTable = DatabaseAPI.NidFromUidAttribMod(ModifierTable);
            PowerFullName = string.Empty;
            Absorbed_PowerType = Enums.ePowerType.Auto_;
            Absorbed_Power_nID = -1;
            Absorbed_Class_nID = -1;
            Absorbed_EffectID = -1;
            Override = string.Empty;
            buffMode = Enums.eBuffMode.Normal;
            Special = string.Empty;
            EffectId = "Ones";
            ActiveConditionals = [];
            AdvancedConditions = new AdvancedConditionSet();
        }

        public Effect(IPower power) : this()
        {
            this.power = power;
        }

        public Effect(BinaryReader reader) : this()
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
            IgnoreScaling = reader.ReadBoolean();
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

            //MagnitudeExpression = reader.ReadString();

            //AssignExpression(MagnitudeExpression);
            // Here we create the instance of the Expression class and read the data back;

            Expressions = new Expressions
            {
                Duration = reader.ReadString(),
                Magnitude = reader.ReadString(),
                Probability = reader.ReadString()
            };

            Reward = reader.ReadString();
            EffectId = reader.ReadString();
            IgnoreED = reader.ReadBoolean();
            Override = reader.ReadString();
            ProcsPerMinute = reader.ReadSingle();
            var conditionalCount = reader.ReadInt32();
            for (var cIndex = 0; cIndex < conditionalCount; cIndex++)
            {
                var cKey = reader.ReadString();
                var cValue = reader.ReadString();
                ActiveConditionals.Add(new KeyValue<string, string>(cKey, cValue));
            }

            AdvancedConditions = AdvancedConditionSet.FromLegacyActiveConditionals(ActiveConditionals);
            if (AdvancedConditionSet.TryReadMarked(reader, AdvancedConditionsMarker, out var advancedConditions))
            {
                AdvancedConditions = advancedConditions;
                ActiveConditionals = AdvancedConditions.ToLegacyActiveConditionals();
            }
        }

        private Effect(IEffect template) : this()
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
            IgnoreScaling = template.IgnoreScaling;
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

            //MagnitudeExpression = template.MagnitudeExpression;

            Expressions = template.Expressions;
            Reward = template.Reward;
            EffectId = template.EffectId;
            IgnoreED = template.IgnoreED;
            Override = template.Override;
            ActiveConditionals = template.ActiveConditionals;
            AdvancedConditions = template.AdvancedConditions?.Clone() ?? AdvancedConditionSet.FromLegacyActiveConditionals(ActiveConditionals);
        }

        private int? SummonId { get; set; }

        private int? OverrideId { get; set; }

        public string MagnitudeExpression { get; set; }

        public Expressions Expressions { get; set; }

        public AdvancedConditionSet AdvancedConditions { get; set; }

        public float ProcsPerMinute { get; set; }

        public bool CancelOnMiss { get; set; }

        private float ActualProbability
        {
            get
            {
                var probability = BaseProbability;

                // Sometimes BaseProbability sticks at 0.75 when PPM is > 0,
                // preventing PPM calculation
                if (ProcsPerMinute > 0 && power != null)
                {
                    var areaFactor = (float)(power.AoEModifier * 0.75 + 0.25);

                    var globalRecharge = (MidsContext.Character.DisplayStats.BuffHaste(false) - 100) / 100;
                    var rechargeVal = Math.Abs(power.RechargeTime) < float.Epsilon
                        ? 0
                        : power.BaseRechargeTime / (power.BaseRechargeTime / power.RechargeTime - globalRecharge);

                    probability = power.PowerType == Enums.ePowerType.Click
                        ? ProcsPerMinute * (rechargeVal + power.CastTimeReal) / (60f * areaFactor)
                        : ProcsPerMinute * 10 / (60f * areaFactor);

                    probability = Math.Max(MinProcChance, Math.Min(MaxProcChance, probability));
                }

                if (MidsContext.Character != null && !string.IsNullOrEmpty(EffectId) && MidsContext.Character.ModifyEffects.ContainsKey(EffectId))
                {
                    probability += MidsContext.Character.ModifyEffects[EffectId];
                }

                return Math.Max(0, Math.Min(1, probability));
            }
        }

        public float MinProcChance => ProcsPerMinute > 0 ? ProcsPerMinute * 0.015f + 0.05f : 0.05f;
        public const float MaxProcChance = 0.9f;

        public float Probability
        {
            get
            {
                switch (AttribType)
                {
                    case Enums.eAttribType.Expression when !string.IsNullOrWhiteSpace(Expressions.Probability):
                        var retValue = Parse(this, ExpressionType.Probability, out var error);
                        return error.Found ? 0 : Math.Max(0, Math.Min(1, retValue));

                    default:
                        return ActualProbability;
                }
            }

            set => BaseProbability = value;
        }

        public float Mag
        {
            get
            {
                return (EffectType == Enums.eEffectType.Damage ? -1 : 1) * AttribType switch
                {
                    Enums.eAttribType.Magnitude => Scale * nMagnitude * DatabaseAPI.GetModifier(this),
                    Enums.eAttribType.Duration => nMagnitude,
                    Enums.eAttribType.Expression when !string.IsNullOrWhiteSpace(Expressions.Magnitude) => Parse(this, ExpressionType.Magnitude, out _),
                    Enums.eAttribType.Expression => Scale * nMagnitude,
                    _ => 0
                };
            }
        }

        public float BuffedMag => Math.Abs(Math_Mag) > float.Epsilon ? Math_Mag : Mag;

        public float MagPercent => !DisplayPercentage ? BuffedMag : BuffedMag * 100f;

        public float Duration
        {
            get
            {
                return AttribType switch
                {
                    Enums.eAttribType.Magnitude => Math.Abs(Math_Duration) > 0.01 ? Math_Duration : nDuration,
                    Enums.eAttribType.Expression when !string.IsNullOrWhiteSpace(Expressions.Duration) => Parse(this, ExpressionType.Duration, out _),
                    Enums.eAttribType.Expression or Enums.eAttribType.Magnitude => Math.Abs(Math_Duration) > 0.01 ? Math_Duration : nDuration,
                    Enums.eAttribType.Duration => Math.Abs(Math_Duration) <= 0.01 ? Scale * DatabaseAPI.GetModifier(this) : Math_Duration,
                    _ => 0
                };
            }
        }

        public bool DisplayPercentage
        {
            //Computed at display-time unless an override has been set
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
                                if (EffectType is Enums.eEffectType.HitPoints or Enums.eEffectType.Absorb or Enums.eEffectType.Endurance or Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedFlying)
                                    return false;
                                break;
                            case Enums.eAspect.Abs:
                                return false;
                            case Enums.eAspect.Cur:
                                if (EffectType is Enums.eEffectType.Mez or Enums.eEffectType.StealthRadius or Enums.eEffectType.StealthRadiusPlayer)
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
                    flag = false;
                }
                else
                {
                    var ps = power?.GetPowerSet();
                    if (ps != null)
                        if (ps.nArchetype > -1)
                        {
                            if (!DatabaseAPI.Database.Classes[ps.nArchetype].Playable)
                                return false;
                        }
                        else if (ps.SetType is Enums.ePowerSetType.None or Enums.ePowerSetType.Accolade or Enums.ePowerSetType.Pet or Enums.ePowerSetType.SetBonus or Enums.ePowerSetType.Temp)
                        {
                            return false;
                        }

                    if ((EffectType == Enums.eEffectType.EntCreate) & (ToWho == Enums.eToWho.Target) & (Stacking == Enums.eStacking.Yes) & !IgnoreScaling)
                    {
                        flag = true;
                    }
                    else if ((EffectType == Enums.eEffectType.DamageBuff) & (ToWho == Enums.eToWho.Target) & (Stacking == Enums.eStacking.Yes) & !IgnoreScaling)
                    {
                        flag = true;
                    }
                    else
                    {
                        if (power != null)
                        {
                            for (var index = 0; index <= power.Effects.Length - 1; ++index)
                            {
                                if ((power.Effects[index].EffectType == Enums.eEffectType.EntCreate) & (power.Effects[index].ToWho == Enums.eToWho.Target) & (power.Effects[index].Stacking == Enums.eStacking.Yes))
                                {
                                    return false;
                                }
                            }
                        }

                        flag = ToWho == Enums.eToWho.Self && Stacking == Enums.eStacking.Yes;
                    }
                }

                return flag;
            }
            set { }
        }

        public bool InherentSpecial => SpecialCase is Enums.eSpecialCase.Assassination or Enums.eSpecialCase.Hidden or Enums.eSpecialCase.Containment or Enums.eSpecialCase.CriticalHit or Enums.eSpecialCase.Domination or Enums.eSpecialCase.Scourge or Enums.eSpecialCase.Supremacy;

        public bool InherentSpecial2 => ValidateConditional("active", "Assassination") ||
                                        ValidateConditional("active", "Containment") ||
                                        ValidateConditional("active", "CriticalHit") ||
                                        ValidateConditional("active", "Domination") ||
                                        ValidateConditional("active", "Scourge") ||
                                        ValidateConditional("active", "Supremacy");

        public bool IgnoreScaling { get; set; }

        public float BaseProbability { get; set; }

        public bool IgnoreED { get; set; }

        public string Reward { get; set; }

        public string EffectId { get; set; }

        public string Special { get; set; }

        public IPower GetPower()
        {
            return power;
        }

        public void SetPower(IPower power)
        {
            this.power = power;
        }

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
                    SummonId = EffectType == Enums.eEffectType.EntCreate
                        ? DatabaseAPI.NidFromUidEntity(Summon)
                        : DatabaseAPI.NidFromUidPower(Summon);
                return SummonId.Value;
            }
            set => SummonId = value;
        }


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

        public List<KeyValue<string, string>> ActiveConditionals { get; set; }
        public bool Validated { get; set; }

        public bool IsFromProc => ProcsPerMinute > 0.0f;

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

        public bool isDamage()
        {
            return EffectType is Enums.eEffectType.Defense or Enums.eEffectType.DamageBuff or Enums.eEffectType.Resistance or Enums.eEffectType.Damage or Enums.eEffectType.Elusivity;
        }

        public string BuildEffectStringShort(bool noMag = false, bool simple = false, bool useBaseProbability = false)
        {
            // precompute commonly reused fragments 
            string magText = string.Empty;
            string chanceText = string.Empty;
            string toWhoText = string.Empty;
            string extraSuffix = string.Empty;
            string trailing = string.Empty; // often " over X seconds" etc.

            // Final effect label (short, player-friendly)
            string effectLabel = GetEffectLabelShort();

            // Variable flag shown when the power is variable-scaled and we are not set to "IgnoreScaling"
            if (power is { VariableEnabled: true } && VariableModified && !IgnoreScaling)
                extraSuffix = " (V)";

            // Short target suffix unless "simple"
            if (!simple)
            {
                toWhoText = ToWho switch
                {
                    Enums.eToWho.Target => " to Tgt",
                    Enums.eToWho.Self => " to Slf",
                    _ => string.Empty
                };
            }

            // Chance (use current or base)
            if (useBaseProbability)
            {
                if (BaseProbability < 1.0)
                    chanceText = $"{(BaseProbability * 100f):#0}% chance";
            }
            else
            {
                if (Probability < 1.0)
                    chanceText = $"{(Probability * 100f):#0}% chance";
            }

            // Magnitude (percent or raw) unless suppressed
            if (!noMag)
            {
                magText = Utilities.FixDP(MagPercent);
                if (DisplayPercentage)
                    magText += "%";
            }

            // build main string by EffectType 
            string result;

            switch (EffectType)
            {
                case Enums.eEffectType.None:
                    {
                        // Special-only line (e.g., "Debt Protection")
                        result = Special;
                        if (Special == "Debt Protection" && !noMag) result = $"{magText}% {result}";
                        break;
                    }

                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    {
                        // For Damage type, include ticks and 'over' duration if present
                        var dmgShort = Enum.GetName(typeof(Enums.eDamageShort), (Enums.eDamageShort)DamageType);

                        if (EffectType == Enums.eEffectType.Damage)
                        {
                            var leadMag = magText;
                            if (Ticks > 0)
                            {
                                leadMag = $"{Ticks} * {leadMag}";
                                if (Duration > 0.0) trailing = $" over {Utilities.FixDP(Duration)} seconds";
                                else if (Absorbed_Duration > 0.0) trailing = $" over {Utilities.FixDP(Absorbed_Duration)} seconds";
                            }

                            result = $"{leadMag} {dmgShort} {effectLabel}{toWhoText}{trailing}";
                            break;
                        }

                        // For non-Damage: append (type) when present
                        var typed = DamageType == Enums.eDamage.None ? string.Empty : $"({dmgShort})";
                        result = $"{magText} {effectLabel}{typed}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.Endurance:
                    {
                        if (noMag) { result = "+Max End"; break; }
                        result = $"{magText} {effectLabel}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.GrantPower:
                case Enums.eEffectType.ExecutePower:
                    {
                        var pow = DatabaseAPI.GetPowerByFullName(Summon);
                        var shown = pow == null ? $" {Summon}" : $" {pow.DisplayName}";
                        result = $"{effectLabel}{shown}{toWhoText}";
                        break;
                    }

                case Enums.eEffectType.Heal:
                case Enums.eEffectType.HitPoints:
                    {
                        if (noMag) { result = "+Max HP"; break; }

                        if (Aspect == Enums.eAspect.Cur)
                        {
                            // Current-value heal uses % of HP directly
                            result = $"{Utilities.FixDP(BuffedMag * 100)}% {effectLabel}{toWhoText}{trailing}";
                        }
                        else if (!DisplayPercentage)
                        {
                            // Non-% display: show (percent of Max HP) after raw value
                            var pctOfMax = Utilities.FixDP((float)(BuffedMag / (double)MidsContext.Archetype.Hitpoints * 100));
                            result = $"{magText} ({pctOfMax}%) {effectLabel}{toWhoText}{trailing}";
                        }
                        else
                        {
                            // % display: also show raw HP from %
                            var rawHp = Utilities.FixDP(BuffedMag / 100f * MidsContext.Archetype.Hitpoints);
                            result = $"{rawHp} ({magText}) {effectLabel}{toWhoText}{trailing}";
                        }
                        break;
                    }

                case Enums.eEffectType.Mez:
                    {
                        var mezName = Enum.GetName(MezType.GetType(), MezType); // uses eMez long token
                        if (Duration > 0.0 && (!simple || (MezType != Enums.eMez.None && MezType != Enums.eMez.Knockback && MezType != Enums.eMez.Knockup)))
                            trailing = $"{Utilities.FixDP(Duration)} second ";

                        var magPart = $" (Mag {magText})";
                        result = $"{trailing}{mezName}{magPart}{toWhoText}";
                        break;
                    }

                case Enums.eEffectType.MezResist:
                    {
                        var mezName = Enum.GetName(MezType.GetType(), MezType);
                        var magPart = noMag ? string.Empty : $" {magText}";
                        result = $"{effectLabel}({mezName}){magPart}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.Recovery:
                    {
                        if (noMag) { result = "+Recovery"; break; }

                        if (DisplayPercentage)
                        {
                            var perSec = Utilities.FixDP(BuffedMag * (MidsContext.Archetype.BaseRecovery * Statistics.BaseMagic));
                            result = $"{magText} ({perSec} /s) {effectLabel}{toWhoText}{trailing}";
                        }
                        else
                        {
                            result = $"{magText} {effectLabel}{toWhoText}{trailing}";
                        }
                        break;
                    }

                case Enums.eEffectType.Regeneration:
                    {
                        if (noMag) { result = "+Regeneration"; break; }

                        if (DisplayPercentage)
                        {
                            var hps = Utilities.FixDP((float)(MidsContext.Archetype.Hitpoints / 100.0 * (BuffedMag * (double)MidsContext.Archetype.BaseRegen * 1.66666662693024)));
                            result = $"{magText} ({hps} HP/s) {effectLabel}{toWhoText}{trailing}";
                        }
                        else
                        {
                            result = $"{magText} {effectLabel}{toWhoText}{trailing}";
                        }
                        break;
                    }

                case Enums.eEffectType.ResEffect:
                    {
                        result = $"{magText} {effectLabel}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    {
                        result = $"{magText}ft {effectLabel}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.EntCreate:
                    {
                        var idx = DatabaseAPI.NidFromUidEntity(Summon);
                        var entityName = idx <= -1 ? $" {Summon}" : $" {DatabaseAPI.Database.Entities[idx].DisplayName}";
                        result = Duration <= 9999
                            ? $"{effectLabel}{entityName}{toWhoText}{trailing}"
                            : $"{effectLabel}{entityName}{toWhoText}";
                        break;
                    }

                case Enums.eEffectType.GlobalChanceMod:
                    {
                        // Prints: "<mag> GlobalChanceMod <Reward> "
                        result = $"{magText} {effectLabel} {Reward}{toWhoText}{trailing}";
                        break;
                    }

                default:
                    {
                        // Default (works for most buff/debuff types)
                        result = $"{magText} {effectLabel}{toWhoText}{trailing}";
                        break;
                    }
            }

            // Chance suffix (when present)
            string chanceSuffix = string.Empty;
            if (!string.IsNullOrEmpty(chanceText))
                chanceSuffix = $" ({BuildCs(chanceText, string.Empty)})";

            // Final trim + extras
            var final = $"{result.Trim()}{chanceSuffix}{extraSuffix}";
            return final;
        }

        public string BuildEffectString(bool simple = false, string specialCat = "", bool noMag = false, bool grouped = false, bool useBaseProbability = false, bool fromPopup = false, bool editorDisplay = false, bool dvDisplay = false, bool ignoreConditions = false)
        {
            // precompute common fragments
            var sBuild = string.Empty;
            var sSubEffect = string.Empty;
            var sSubSubEffect = string.Empty;
            var sMag = string.Empty;
            var sDuration = string.Empty;
            var sChance = string.Empty;
            var sTarget = string.Empty;
            var sPvx = string.Empty;
            var sStack = string.Empty;
            var sBuff = string.Empty;
            var sDelay = string.Empty;
            var sResist = string.Empty;
            var sSpecial = string.Empty;
            var sSuppress = string.Empty;
            var sVariable = string.Empty;
            var sToHit = string.Empty;
            var sEnh = string.Empty;
            var sSuppressShort = string.Empty;
            var sConditional = string.Empty;
            var sNearGround = string.Empty;
            var sMagExp = string.Empty;
            var sProbExp = string.Empty;

            // Variable flag
            if (power is { VariableEnabled: true } && (VariableModified | ToWho == Enums.eToWho.Self))
                if (!IgnoreScaling)
                    sVariable = " (Variable)";

            if (isEnhancementEffect) sEnh = "(From Enh) ";

            // Central label (LONG) - replaces all prior enum-name lookups
            var sEffect = GetEffectLabelLong();

            // Target & ToHit
            if (!simple)
            {
                var aff = power?.EntitiesAffected ?? Enums.eEntity.None;
                var aut = power?.EntitiesAutoHit ?? Enums.eEntity.None;
                sTarget = ToWho.ToPhrase(aff, aut);
                if (RequiresToHitCheck) sToHit = " requires ToHit check";
            }

            // ----- Chance / PPM / Base-vs-current probability -----
            if (AttribType == Enums.eAttribType.Expression && !string.IsNullOrWhiteSpace(Expressions.Probability))
            {
                var pct = (decimal)Math.Max(0, Math.Min(100, Parse(this, ExpressionType.Probability, out _) * 100));
                sChance = editorDisplay ? $"{decimal.Round(pct)}% Variable Chance" : $"{decimal.Round(pct)}% chance";
                if (editorDisplay) sProbExp = $"Probability Expression: {Expressions.Probability}";
            }

            if (string.IsNullOrEmpty(sChance))
            {
                if (ProcsPerMinute > 0 && Probability < 0)
                {
                    sChance = $"{ProcsPerMinute} PPM";
                }
                else if (useBaseProbability)
                {
                    if (BaseProbability < 1 && BaseProbability >= 0)
                    {
                        sChance = BaseProbability >= 0.975f
                            ? $"{BaseProbability * 100:#0.0}% chance"
                            : $"{BaseProbability * 100:#0}% chance";
                        sChance += EffectId is "" or "Ones" ? "" : " ";
                        if (EffectId is not "" and not "Ones") sChance += $"when {EffectId}";
                        if (CancelOnMiss) sChance += ", Cancels on Miss";
                        if (ProcsPerMinute > 0)
                            sChance = fromPopup | editorDisplay
                                ? $"{ProcsPerMinute} PPM"
                                : $"{ProcsPerMinute} PPM/{Probability:P0} chance";
                    }
                }
                else
                {
                    if (Probability < 1 && Probability >= 0)
                    {
                        sChance = Probability >= 0.975f
                            ? $"{Probability * 100:#0.0}% chance"
                            : $"{Probability * 100:#0}% chance";
                        sChance += EffectId is "" or "Ones" ? "" : " ";
                        if (EffectId is not "" and not "Ones" && !fromPopup) sChance += $"when {EffectId}";
                        if (CancelOnMiss) sChance += ", Cancels on Miss";
                        if (ProcsPerMinute > 0)
                            sChance = fromPopup | editorDisplay
                                ? $"{ProcsPerMinute} PPM"
                                : $"{ProcsPerMinute} PPM/{Probability:P0} chance";
                    }
                }
            }

            // Resistibility banner (non-resistible)
            var resistPresent = false;
            if (!Resistible)
            {
                if ((!simple & ToWho != Enums.eToWho.Self) | EffectType == Enums.eEffectType.Damage)
                {
                    sResist = "Non-resistible";
                    resistPresent = true;
                }
            }

            if (NearGround) sNearGround = " (Must be near ground)";

            // PvX tail
            switch (PvMode)
            {
                case Enums.ePvX.PvE:
                    sPvx = resistPresent ? "by Critters" : "to Critters";
                    if (EffectType == Enums.eEffectType.Heal & Aspect == Enums.eAspect.Abs & Mag > 0 &
                        PvMode == Enums.ePvX.PvE) sPvx = "in PvE";
                    if (ToWho == Enums.eToWho.Self) sPvx = "in PvE";
                    break;
                case Enums.ePvX.PvP:
                    sPvx = resistPresent ? "by Players" : "to Players";
                    if (ToWho == Enums.eToWho.Self) sPvx = "in PvP";
                    break;
                case Enums.ePvX.Any:
                    if (ToWho == Enums.eToWho.Self & MidsContext.Config.ShowSelfBuffsAny) sPvx = "in PvE/PvP";
                    break;
            }

            // Buffability, stacking, delay
            if (!simple)
            {
                if (!Buffable & EffectType != Enums.eEffectType.DamageBuff)
                    sBuff = IgnoreED ? " [Ignores Enhancements, Buffs & ED]" : " [Ignores Enhancements & Buffs]";

                if (Stacking == Enums.eStacking.No)
                    sStack = "\n  Effect does not stack from same caster";

                if (DelayedTime > 0)
                    sDelay = $"after {Utilities.FixDP(DelayedTime)} seconds";
            }

            // Conditions / SpecialCase
            if (!ignoreConditions)
            {
                if (SpecialCase != Enums.eSpecialCase.None & SpecialCase != Enums.eSpecialCase.Defiance)
                    sSpecial = Enum.GetName(SpecialCase.GetType(), SpecialCase);

                if (ActiveConditionals.Count > 0)
                {
                    var getCondition = new Regex("(:.*)");
                    var getConditionItem = new Regex("(.*:)");
                    var conList = new List<string>();

                    foreach (var cVp in ActiveConditionals)
                    {
                        var condition = getCondition.Replace(cVp.Key, "").Replace(":", "");
                        var conditionItemName = getConditionItem.Replace(cVp.Key, "").Replace(":", "");
                        var conditionPower = condition == "Config"
                            ? null
                            : DatabaseAPI.GetPowerByFullName(conditionItemName);
                        var conditionOperator = cVp.Value switch
                        {
                            "True" => "is ",
                            "False" => "not ",
                            _ => ""
                        };

                        switch (condition)
                        {
                            case "Stacks":
                                conList.Add(
                                    $"{(MidsContext.Config.CoDEffectFormat ? conditionPower?.FullName : conditionPower?.DisplayName)} {condition} {cVp.Value}");
                                break;
                            case "Team":
                                conList.Add($"{conditionItemName}s on {condition} {cVp.Value}");
                                break;
                            case "Config":
                            {
                                var cfgKey = MidsContext.Config.CoDEffectFormat
                                    ? conditionItemName.Replace("PlayerSettings", "player")
                                        .Replace("TargetSettings", "target")
                                    : conditionItemName;
                                var cfgText =
                                    $"{condition}:{(MidsContext.Config.CoDEffectFormat ? cfgKey : ConfigData.CombatContext.FormatSettingName(conditionItemName))} {conditionOperator} {cVp.Value}"
                                        .Replace("  ", " ")
                                        .Replace("Player IsAlive = True", "Player is Alive",
                                            StringComparison.InvariantCultureIgnoreCase)
                                        .Replace("Player IsAlive = False", "Player is Dead",
                                            StringComparison.InvariantCultureIgnoreCase)
                                        .Replace("Target IsAlive = True", "Target is Alive",
                                            StringComparison.InvariantCultureIgnoreCase)
                                        .Replace("Target IsAlive = False", "Target is Dead",
                                            StringComparison.InvariantCultureIgnoreCase);
                                conList.Add(cfgText);
                                break;
                            }
                            default:
                                conList.Add(
                                    $"{(MidsContext.Config.CoDEffectFormat ? conditionPower?.FullName : conditionPower?.DisplayName)} {conditionOperator}{condition}");
                                break;
                        }
                    }

                    sConditional = string.Empty;
                    foreach (var c in conList)
                    {
                        if (sConditional == string.Empty) sConditional += c.Replace(" OR ", " ");
                        else if (c.Contains("OR ")) sConditional += $" OR {c.Replace(" OR ", " ")}";
                        else sConditional += $" AND {c}";
                    }
                }
            }

            // Duration / interval banner (same rules as before)
            if (!simple || (Scale > 0 && EffectType is Enums.eEffectType.Mez or Enums.eEffectType.Endurance
                                      && !(fromPopup && EffectType == Enums.eEffectType.Endurance &&
                                           Aspect == Enums.eAspect.Max)))
            {
                sDuration = string.Empty;
                var sForOver = EffectType switch
                {
                    Enums.eEffectType.Damage or Enums.eEffectType.Endurance => " over ",
                    Enums.eEffectType.SilentKill => " in ",
                    Enums.eEffectType.Mez when MezType is Enums.eMez.Knockback or Enums.eMez.Knockup => "For ",
                    _ => " for "
                };

                if (Duration > 0 & (EffectType != Enums.eEffectType.Damage | Ticks > 0))
                    sDuration += $"{sForOver}{Utilities.FixDP(Duration)} seconds";
                else if (Absorbed_Duration > 0 & (EffectType != Enums.eEffectType.Damage | Ticks > 0))
                    sDuration += $"{sForOver}{Utilities.FixDP(Absorbed_Duration)} seconds";
                else
                    sDuration += " ";

                if (Absorbed_Interval > 0 & Absorbed_Interval < 900)
                    sDuration +=
                        $" every {Utilities.FixDP(Absorbed_Interval)} seconds{(EffectType == Enums.eEffectType.Mez && (MezType is Enums.eMez.Knockback or Enums.eMez.Knockup) ? ": " : "")}";
            }

            // Magnitude text (Variable/Expression aware)
            if (!noMag & EffectType != Enums.eEffectType.SilentKill)
            {
                if (Expressions.Magnitude != "" & AttribType == Enums.eAttribType.Expression)
                {
                    var mag = BuffedMag * (DisplayPercentage ? 100 : 1);
                    var absAllowed = new List<Enums.eEffectType>
                    {
                        Enums.eEffectType.Damage, Enums.eEffectType.DamageBuff, Enums.eEffectType.Defense,
                        Enums.eEffectType.Resistance
                    };

                    if (editorDisplay)
                    {
                        sMag = mag > float.Epsilon && absAllowed.Any(x => x == EffectType)
                            ? $"{Math.Abs(mag):####0.##}{(DisplayPercentage ? "%" : "")} Variable"
                            : $"{mag:####0.##}{(DisplayPercentage ? "%" : "")} Variable";

                        sMagExp = $"Mag Expression: {Expressions.Magnitude.Replace("modifier>current", ModifierTable)}";
                    }
                    else
                    {
                        sMag = mag > float.Epsilon && absAllowed.Any(x => x == EffectType)
                            ? $"{Math.Abs(mag):####0.##}{(DisplayPercentage ? "%" : "")}"
                            : $"{mag:####0.#}{(DisplayPercentage ? "%" : "")}";
                    }
                }
                else if (EffectType == Enums.eEffectType.PerceptionRadius)
                {
                    var perceptionDistance = Statistics.BasePerception * BuffedMag;
                    sMag = MidsContext.Config.CoDEffectFormat & !fromPopup
                        ? $"({Scale * (AttribType == Enums.eAttribType.Magnitude ? nMagnitude : 1):####0.####} x {ModifierTable}){(DisplayPercentage ? "%" : "")} ({perceptionDistance}ft)"
                        : DisplayPercentage
                            ? $"{Utilities.FixDP(BuffedMag * 100)}% ({perceptionDistance}ft)"
                            : $"{perceptionDistance}ft";
                }
                else
                {
                    sMag = MidsContext.Config.CoDEffectFormat & EffectType != Enums.eEffectType.Mez & !fromPopup
                        ? $"({Scale * (AttribType == Enums.eAttribType.Magnitude ? nMagnitude : 1):####0.####} x {ModifierTable}){(DisplayPercentage ? "%" : "")}"
                        : $"{(EffectType == Enums.eEffectType.Enhancement & ETModifies != Enums.eEffectType.EnduranceDiscount ? BuffedMag > 0 ? "+" : "-" : "")}{Utilities.FixDP(BuffedMag * (DisplayPercentage ? 100 : 1))}{(DisplayPercentage ? "%" : "")}";
                }

                if (Expressions.Duration != "" & AttribType == Enums.eAttribType.Expression & editorDisplay)
                    sMagExp +=
                        $"{(sMagExp == "" ? "" : " - ")}Duration Expression: {Expressions.Duration.Replace("modifier>current", ModifierTable)}";
            }

            // Suppression banners
            if (!simple)
            {
                sSuppress = string.Empty;
                if ((Suppression & Enums.eSuppress.ActivateAttackClick) == Enums.eSuppress.ActivateAttackClick)
                    sSuppress += "\n  Suppressed when Attacking.";
                if ((Suppression & Enums.eSuppress.Attacked) == Enums.eSuppress.Attacked)
                    sSuppress += "\n  Suppressed when Attacked.";
                if ((Suppression & Enums.eSuppress.HitByFoe) == Enums.eSuppress.HitByFoe)
                    sSuppress += "\n  Suppressed when Hit.";
                if ((Suppression & Enums.eSuppress.MissionObjectClick) == Enums.eSuppress.MissionObjectClick)
                    sSuppress += "\n  Suppressed when MissionObjectClick.";
                if ((Suppression & Enums.eSuppress.Held) == Enums.eSuppress.Held ||
                    (Suppression & Enums.eSuppress.Immobilized) == Enums.eSuppress.Immobilized ||
                    (Suppression & Enums.eSuppress.Sleep) == Enums.eSuppress.Sleep ||
                    (Suppression & Enums.eSuppress.Stunned) == Enums.eSuppress.Stunned ||
                    (Suppression & Enums.eSuppress.Terrorized) == Enums.eSuppress.Terrorized)
                    sSuppress += "\n  Suppressed when Mezzed.";
                if ((Suppression & Enums.eSuppress.Knocked) == Enums.eSuppress.Knocked)
                    sSuppress += "\n  Suppressed when Knocked.";
                if ((Suppression & Enums.eSuppress.Confused) == Enums.eSuppress.Confused)
                    sSuppress += "\n  Suppressed when Confused.";
            }
            else
            {
                if ((Suppression & Enums.eSuppress.ActivateAttackClick) == Enums.eSuppress.ActivateAttackClick ||
                    (Suppression & Enums.eSuppress.Attacked) == Enums.eSuppress.Attacked ||
                    (Suppression & Enums.eSuppress.HitByFoe) == Enums.eSuppress.HitByFoe)
                    sSuppressShort = "Combat Suppression";
            }

            // TYPE-SPECIFIC RENDERING 
            switch (EffectType)
            {
                case Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                {
                    if (string.IsNullOrEmpty(specialCat))
                    {
                        sSubEffect = grouped ? "%VALUE%" : Enum.GetName(DamageType.GetType(), DamageType);

                        if (EffectType == Enums.eEffectType.Damage)
                        {
                            if (Ticks > 0) sMag = $"{Ticks} x {sMag}";
                            sBuild = $"{sMag} {sSubEffect} {sEffect}{sTarget}{sDuration}";
                        }
                        else
                        {
                            var paren = DamageType == Enums.eDamage.None ? string.Empty : $"({sSubEffect})";
                            sBuild = $"{sMag} {sEffect}{paren}{sTarget}{sDuration}";
                        }
                    }
                    else
                    {
                        sBuild = $"{sMag} {specialCat} {sTarget}{sDuration}";
                    }

                    break;
                }

                case Enums.eEffectType.StealthRadius:
                case Enums.eEffectType.StealthRadiusPlayer:
                    sBuild = $"{sMag}ft {sEffect}{sTarget}{sDuration}";
                    break;

                case Enums.eEffectType.Mez:
                {
                    sSubEffect = Enum.GetName(MezType.GetType(), MezType);
                    if (AttribType == Enums.eAttribType.Magnitude & nDuration > 0 & Aspect == Enums.eAspect.Str)
                    {
                        sBuild =
                            $"{(MidsContext.Config.CoDEffectFormat & !fromPopup ? $"({Scale * nMagnitude:####0.####} x {ModifierTable})%" : sMag)} {sSubEffect}{sTarget}{sDuration}";
                    }
                    else
                    {
                        if (Duration > 0 & (!simple | (MezType != Enums.eMez.None & MezType != Enums.eMez.Knockback &
                                                       MezType != Enums.eMez.Knockup)))
                            sDuration =
                                $"{(MidsContext.Config.CoDEffectFormat & !fromPopup ? $"({Scale:####0.####} x {ModifierTable})" : Utilities.FixDP(Duration))} second ";
                        if (!noMag)
                            sMag =
                                $" ({(MezType is Enums.eMez.Knockback or Enums.eMez.Knockup && MidsContext.Config.CoDEffectFormat ? $"{Scale * nMagnitude:####0.####} x {ModifierTable}" : $"Mag {sMag}")})";

                        sBuild = $"{sDuration}{sSubEffect}{sMag}{sTarget}";
                    }

                    break;
                }

                case Enums.eEffectType.MezResist:
                {
                    sSubEffect = Enum.GetName(typeof(Enums.eMez), MezType);
                    if (!noMag) sMag = $" {sMag}";
                    // Keep explicit mez parentheses for protection lines
                    sBuild = $"{sMag} {sEffect}{sTarget}{sDuration}";
                    break;
                }

                case Enums.eEffectType.ResEffect:
                {
                    sBuild = $"{sMag} {sEffect}{sTarget}{sDuration}";
                    break;
                }

                case Enums.eEffectType.Enhancement:
                {
                    sBuild = $"{sMag} {sEffect}{sTarget}{sDuration}";
                    break;
                }

                case Enums.eEffectType.None:
                {
                    sBuild = Special;
                    if (Special == "Debt Protection") sBuild = $"{sMag}% {sBuild}";
                    break;
                }

                case Enums.eEffectType.Heal:
                case Enums.eEffectType.HitPoints:
                {
                    if (!noMag)
                    {
                        if (Ticks > 0) sMag = $"{Ticks} x {sMag}";
                        if (Aspect == Enums.eAspect.Cur)
                        {
                            sBuild = $"{Utilities.FixDP(BuffedMag * 100)}% {sEffect}{sTarget}{sDuration}";
                        }
                        else
                        {
                            sBuild = DisplayPercentage
                                ? $"{Utilities.FixDP(BuffedMag / 100 * MidsContext.Archetype.Hitpoints)} HP ({sMag}) {sEffect}{sTarget}{sDuration}"
                                : $"{sMag} HP ({Utilities.FixDP(BuffedMag / MidsContext.Archetype.Hitpoints * 100)}%) {sEffect}{sTarget}{sDuration}";
                        }
                    }
                    else
                    {
                        sBuild = "+Max HP";
                    }

                    break;
                }

                case Enums.eEffectType.Regeneration:
                    sBuild = !noMag
                        ? (DisplayPercentage
                            ? $"{sMag} ({Utilities.FixDP(MidsContext.Archetype.Hitpoints / 100f * (BuffedMag * MidsContext.Archetype.BaseRegen * Statistics.BaseMagic))} HP/sec) {sEffect}{sTarget}{sDuration}"
                            : $"{sMag} {sEffect}{sTarget}{sDuration}")
                        : "+Regeneration";
                    break;

                case Enums.eEffectType.Recovery:
                    sBuild = !noMag
                        ? (DisplayPercentage
                            ? $"{sMag} ({Utilities.FixDP(BuffedMag * (MidsContext.Archetype.BaseRecovery * Statistics.BaseMagic))} End/sec) {sEffect}{sTarget}{sDuration}"
                            : $"{sMag} {sEffect}{sTarget}{sDuration}")
                        : "+Recovery";
                    break;

                case Enums.eEffectType.EntCreate:
                {
                    sResist = string.Empty;
                    var summon = DatabaseAPI.NidFromUidEntity(Summon);
                    var tSummon = summon > -1
                        ? " " + (MidsContext.Config.CoDEffectFormat
                            ? $"({DatabaseAPI.Database.Entities[summon].UID})"
                            : DatabaseAPI.Database.Entities[summon].DisplayName)
                        : " " + Summon;
                    sBuild = $"{sEffect}{tSummon}{sTarget}{(Duration > 9999 ? "" : sDuration)}";
                    break;
                }

                case Enums.eEffectType.Endurance:
                {
                    if (Ticks > 0) sMag = $"{Ticks} x {sMag}";
                    if (noMag) sBuild = "+Max End";
                    else if (Aspect == Enums.eAspect.Max) sBuild = $"{sMag}% Max End{sTarget}{sDuration}";
                    else sBuild = $"{sMag} {sEffect}{sTarget}{sDuration}";
                    break;
                }

                case Enums.eEffectType.GrantPower:
                case Enums.eEffectType.ExecutePower:
                {
                    sResist = string.Empty;
                    var pID = DatabaseAPI.GetPowerByFullName(Summon);
                    var tGrant = pID != null
                        ? $" {(MidsContext.Config.CoDEffectFormat ? $"({pID.FullName})" : pID.DisplayName)}"
                        : $" {Summon}";
                    sBuild =
                        $"{sEffect}{tGrant}{sTarget}{(Math.Abs(Duration) < float.Epsilon ? "" : $" for {Duration}s")}{(Ticks > 0 & EffectType == Enums.eEffectType.ExecutePower ? $" ({Ticks} tick{(Ticks == 1 ? "" : "s")})" : "")}";
                    break;
                }

                case Enums.eEffectType.GlobalChanceMod:
                    sBuild = $"{sMag} {sEffect} {Reward}{sTarget}{sDuration}";
                    break;

                case Enums.eEffectType.PowerRedirect:
                {
                    sBuild = !string.IsNullOrWhiteSpace(Override)
                        ? $"{sEffect}{sTarget} ({DatabaseAPI.GetPowerByFullName(Override).DisplayName})"
                        : $"{sEffect}{sTarget} ({Override})";
                    break;
                }

                default:
                    sBuild = $"{sMag} {sEffect}{sTarget}{sDuration}";
                    break;
            }

            // trailing conditionals / banners
            var sExtra = string.Empty;
            var sExtra2 = string.Empty;

            if (!string.IsNullOrEmpty(sChance + sResist + sPvx + sDelay + sSpecial + sConditional + sToHit +
                                      sSuppressShort))
            {
                sExtra = BuildCs(sChance, sExtra);
                sExtra = BuildCs(sDelay, sExtra);
                sExtra = BuildCs(sSuppressShort, sExtra);
                sExtra = BuildCs(sResist, sExtra);

                sExtra2 = BuildCs(sChance, sExtra2);
                sExtra2 = BuildCs(sDelay, sExtra2);
                sExtra2 = BuildCs(sSuppressShort, sExtra2);
                sExtra2 = BuildCs(sResist, sExtra2);

                if (!string.IsNullOrEmpty(sPvx))
                {
                    sExtra = !string.IsNullOrEmpty(sSpecial)
                        ? BuildCs(sPvx + ", if " + sSpecial, sExtra, resistPresent)
                        : BuildCs(sPvx, sExtra, resistPresent);
                    sExtra2 = !string.IsNullOrEmpty(sConditional)
                        ? BuildCs(sPvx + ", if " + sConditional, sExtra2, resistPresent)
                        : BuildCs(sPvx, sExtra2, resistPresent);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sSpecial)) sExtra = BuildCs("if " + sSpecial, sExtra);
                    if (!string.IsNullOrEmpty(sConditional)) sExtra2 = BuildCs("if " + sConditional, sExtra2);
                }

                sExtra = BuildCs(sToHit, sExtra);
                sExtra = " (" + sExtra + ")";
                sExtra2 = BuildCs(sToHit, sExtra2);
                sExtra2 = " (" + sExtra2 + ")";

                if (AttribType == Enums.eAttribType.Expression && !editorDisplay && !dvDisplay)
                {
                    const string sType = " [Expression Based]";
                    sExtra += sType;
                    sExtra2 += sType;
                }
            }

            sExtra = BuildCs(sNearGround, sExtra);
            if (sExtra.Equals(" ()")) sExtra = "";

            // final string (with editor diagnostics)
            var sFinal = string.Empty;
            if (AttribType == Enums.eAttribType.Expression && editorDisplay)
                sFinal =
                    $"{(sEnh + sBuild + (sConditional != "" ? sExtra2 : sExtra) + sBuff + sVariable + sStack + sSuppress).Replace("--", "-").Trim()}\r\n{sMagExp}\n{sProbExp}";
            else
                sFinal = (sEnh + sBuild + (sConditional != "" ? sExtra2 : sExtra) + sBuff + sVariable + sStack +
                          sSuppress).Replace("--", "-").Trim();

            sFinal = sFinal
                .Replace("( ", "(").Replace("  ", " ")
                .Replace("(, ", "(")
                .Replace("chance )", "chance)")
                .Replace("SelfFor ", "Self for ")
                .Replace("TargetFor ", "Target for ");

            return sFinal;
        }

        private string GetEffectLabelShort()
        {
            if (EffectType is Enums.eEffectType.Enhancement) return EnhancementShort();
            if (EffectType is Enums.eEffectType.ResEffect) return ResEffectShort();
            if (EffectType is Enums.eEffectType.MezResist) return MezResistShort();
            return Enums.GetEffectNameShort(EffectType);
        }

        private string GetEffectLabelLong()
        {
            if (EffectType == Enums.eEffectType.Enhancement) return EnhancementLong();
            if (EffectType == Enums.eEffectType.ResEffect) return ResEffectLong();
            if (EffectType is Enums.eEffectType.MezResist) return MezResistLong();
            return Enums.GetEffectName(EffectType);
        }

        private string EnhancementShort()
        {
            if (ETModifies == Enums.eEffectType.Mez)
                return $"{Enums.GetMezNameShort((Enums.eMezShort)MezType)} Str";

            return ETModifies switch
            {
                Enums.eEffectType.Heal => "Heal Str",
                Enums.eEffectType.Defense => "Defense Str",
                Enums.eEffectType.ToHit => "To-Hit Str",
                Enums.eEffectType.Resistance => "Resistance Str",
                Enums.eEffectType.Endurance => "Endurance Str",     // sign tells restore vs drain
                Enums.eEffectType.Recovery => "Recovery Str",
                Enums.eEffectType.Regeneration => "Regeneration Str",
                Enums.eEffectType.Slow => "Slow Str",
                Enums.eEffectType.RechargeTime => "Recharge Str",      // sign tells faster vs slower
                Enums.eEffectType.Damage => DamageType == Enums.eDamage.None
                                                    ? "Damage Str"
                                                    : $"{Enums.GetDamageNameShort(DamageType)} Dam Str",
                _ => $"{EnhancementTargetNameShort()} Str"
            };
        }

        private string ResEffectShort()
        {
            if (ETModifies == Enums.eEffectType.Mez)
                return $"{Enums.GetMezNameShort((Enums.eMezShort)MezType)} Str Res";

            return ETModifies switch
            {
                Enums.eEffectType.Heal => "Heal Str Res",
                Enums.eEffectType.Defense => "Def Str Res",
                Enums.eEffectType.ToHit => "To-Hit Str Res",
                Enums.eEffectType.Resistance => "Resistance Str Res",
                Enums.eEffectType.Endurance => "End Str Res",
                Enums.eEffectType.Recovery => "Recovery Str Res",
                Enums.eEffectType.Regeneration => "Regen Str Res",
                Enums.eEffectType.Slow => "Slow Str Res",
                Enums.eEffectType.RechargeTime => "RechTime Str Res",
                Enums.eEffectType.Damage => DamageType == Enums.eDamage.None
                                                    ? "Damage Str Res"
                                                    : $"{Enums.GetDamageNameShort(DamageType)} Dam Str Res",
                _ => $"{EnhancementTargetNameShort()} Str Res"
            };
        }

        private string MezResistShort()
        {
            return $"Mez Resist ({Enums.GetMezNameShort((Enums.eMezShort)MezType)})";
        }

        private string EnhancementLong()
        {
            if (ETModifies == Enums.eEffectType.Mez)
                return $"{Enums.GetMezName(MezType)} Strength";

            return ETModifies switch
            {
                Enums.eEffectType.Heal => "Heal Strength",
                Enums.eEffectType.Defense => "Defense Strength",
                Enums.eEffectType.ToHit => "To-Hit Strength",
                Enums.eEffectType.Resistance => "Resistance Strength",
                Enums.eEffectType.Endurance => "Endurance Strength",     // neutral; sign determines drain/restore
                Enums.eEffectType.Recovery => "Recovery Strength",
                Enums.eEffectType.Regeneration => "Regeneration Strength",
                Enums.eEffectType.Slow => "Slow Strength",
                Enums.eEffectType.RechargeTime => "RechargeTime Strength",
                Enums.eEffectType.Damage => DamageType == Enums.eDamage.None
                                                    ? "Damage Strength"
                                                    : $"{Enums.GetDamageName(DamageType)} Damage Strength",
                _ => $"{EnhancementTargetNameLong()} Strength"
            };
        }

        private string ResEffectLong()
        {
            if (ETModifies == Enums.eEffectType.Mez)
                return $"{Enums.GetMezName(MezType)} Strength Resistance";

            return ETModifies switch
            {
                Enums.eEffectType.Heal => "Heal Strength Resistance",
                Enums.eEffectType.Defense => "Defense Strength Resistance",
                Enums.eEffectType.ToHit => "To-Hit Strength Resistance",
                Enums.eEffectType.Resistance => "Resistance Strength Resistance",
                Enums.eEffectType.Endurance => "Endurance Strength Resistance",
                Enums.eEffectType.Recovery => "Recovery Strength Resistance",
                Enums.eEffectType.Regeneration => "Regeneration Strength Resistance",
                Enums.eEffectType.Slow => "Slow Strength Resistance",
                Enums.eEffectType.RechargeTime => "RechargeTime Strength Resistance",
                Enums.eEffectType.Damage => DamageType == Enums.eDamage.None
                                                    ? "Damage Strength Resistance"
                                                    : $"{Enums.GetDamageName(DamageType)} Damage Strength Resistance",
                _ => $"{EnhancementTargetNameLong()} Strength Resistance"
            };
        }

        private string MezResistLong()
        {
            return $"Mez Resistance ({Enums.GetMezName(MezType)})";
        }


        private string EnhancementTargetNameShort()
        {
            return ETModifies is Enums.eEffectType.None or Enums.eEffectType.Null or Enums.eEffectType.NullBool
                ? "Effect"
                : Enums.GetEffectNameShort(ETModifies);
        }

        private string EnhancementTargetNameLong()
        {
            return ETModifies is Enums.eEffectType.None or Enums.eEffectType.Null or Enums.eEffectType.NullBool
                ? "Effect"
                : Enums.GetEffectName(ETModifies);
        }


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
            writer.Write(IgnoreScaling);
            writer.Write((int)PvMode);
            writer.Write((int)ToWho);
            writer.Write((int)DisplayPercentageOverride);
            writer.Write(Scale);
            writer.Write(nMagnitude);
            writer.Write(nDuration);
            writer.Write((int)AttribType);
            writer.Write((int)Aspect);
            writer.Write(ModifierTable);
            writer.Write(NearGround);
            writer.Write(CancelOnMiss);
            writer.Write(RequiresToHitCheck);
            writer.Write(UIDClassName);
            writer.Write(nIDClassName);

            //Here we write in the Expression class properties
            writer.Write(Expressions.Duration);
            writer.Write(Expressions.Magnitude);
            writer.Write(Expressions.Probability);

            writer.Write(Reward);
            writer.Write(EffectId);
            writer.Write(IgnoreED);
            writer.Write(Override);
            writer.Write(ProcsPerMinute);

            var legacyConditionals = AdvancedConditions is { Rows.Count: > 0 }
                ? AdvancedConditions.ToLegacyActiveConditionals()
                : ActiveConditionals;
            writer.Write(legacyConditionals.Count);
            foreach (var cVp in legacyConditionals)
            {
                writer.Write(cVp.Key);
                writer.Write(cVp.Value);
            }

            AdvancedConditionSet.StoreMarked(writer, AdvancedConditionsMarker,
                AdvancedConditions is { Rows.Count: > 0 }
                    ? AdvancedConditions
                    : AdvancedConditionSet.FromLegacyActiveConditionals(ActiveConditionals));
        }

        public int SetTicks(float iDuration, float iInterval)
        {
            Ticks = 0;
            if (iInterval > 0)
            {
                Ticks = (int)(1 + Math.Floor(iDuration / (double)iInterval));
            }

            return Ticks;
        }

        public bool ValidateConditional(string cPowername)
        {
            return BooleanExprPreprocessor.Parse(this, cPowername);
        }

        public bool ValidateConditional(string cType, string cPowername)
        {
            return BooleanExprPreprocessor.Parse(this, cType, cPowername);
        }

        public bool ValidateConditional(int index)
        {
            if (ActiveConditionals is not { Count: > 0 })
            {
                return true;
            }

            var set = AdvancedConditionSet.FromLegacyActiveConditionals(ActiveConditionals);
            ActiveConditionals[index].Validated = index < set.Rows.Count && AdvancedConditionEvaluator.EvaluateRow(this, set.Rows[index]);

            return ActiveConditionals[index].Validated;
        }

        public bool ValidateConditional()
        {
            return BooleanExprPreprocessor.Parse(this);
        }

        public bool CanInclude()
        {
            if (MidsContext.Character == null | ActiveConditionals == null | (ActiveConditionals?.Count == 0 && SpecialCase == Enums.eSpecialCase.None))
            {
                return true;
            }

            #region SpecialCase Processing

            if (SpecialCase != Enums.eSpecialCase.None)
            {
                switch (SpecialCase)
                {
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
                    case Enums.eSpecialCase.NotBoxingBuff:
                        if (MidsContext.Character.NotBoxingBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.KickBuff:
                        if (MidsContext.Character.KickBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotKickBuff:
                        if (MidsContext.Character.NotKickBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.CrossPunchBuff:
                        if (MidsContext.Character.CrossPunchBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotCrossPunchBuff:
                        if (MidsContext.Character.NotCrossPunchBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.Supremacy:
                        if (MidsContext.Character.Supremacy && !MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.SupremacyAndBuffPwr:
                        if (MidsContext.Character.Supremacy && MidsContext.Character.PackMentality)
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
                    case Enums.eSpecialCase.PackMentality:
                        if (MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotPackMentality:
                        if (!MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.FastSnipe:
                        if (MidsContext.Character.FastSnipe)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotFastSnipe:
                        if (!MidsContext.Character.FastSnipe)
                            return true;
                        break;
                }
            }

            #endregion

            if (AdvancedConditions is { Rows.Count: > 0 } || ActiveConditionals is { Count: > 0 })
            {
                Validated = BooleanExprPreprocessor.Parse(this);
                return Validated;
            }

            #region Conditional Processing

            if (ActiveConditionals.Count > 0)
            {
                var getCondition = new Regex("(:.*)");
                var getConditionItem = new Regex("(.*:)");
                foreach (var cVp in ActiveConditionals)
                {
                    var condition = getCondition.Replace(cVp.Key, "");
                    var conditionItemName = getConditionItem.Replace(cVp.Key, "").Replace(":", "");
                    var conditionPower = DatabaseAPI.GetPowerByFullName(conditionItemName);
                    var cVal = cVp.Value.Split(' ');
                    switch (condition)
                    {
                        case "Active":
                            if (conditionPower != null)
                            {
                                bool? boolVal = Convert.ToBoolean(cVp.Value);
                                if (MidsContext.Character.CurrentBuild.PowerActive(conditionPower) == boolVal)
                                {
                                    cVp.Validated = true;
                                }
                                else
                                {
                                    cVp.Validated = false;
                                }
                            }

                            break;
                        case "Taken":
                            if (conditionPower != null)
                            {
                                cVp.Validated = MidsContext.Character.CurrentBuild.PowerUsed(conditionPower)
                                    .Equals(Convert.ToBoolean(cVp.Value));
                            }

                            break;
                        case "Stacks":
                            if (conditionPower != null)
                            {
                                switch (cVal[0])
                                {
                                    case "=":

                                        cVp.Validated = conditionPower.Stacks.Equals(Convert.ToInt32(cVal[1]));

                                        break;
                                    case ">":
                                        cVp.Validated = conditionPower.Stacks > Convert.ToInt32(cVal[1]);

                                        break;
                                    case "<":
                                        cVp.Validated = conditionPower.Stacks < Convert.ToInt32(cVal[1]);

                                        break;
                                }
                            }

                            break;
                        case "Team":
                            switch (cVal[0])
                            {
                                case "=":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) && MidsContext
                                        .Config.TeamMembers[conditionItemName].Equals(Convert.ToInt32(cVal[1])))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                                case ">":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                        MidsContext.Config.TeamMembers[conditionItemName] >
                                        Convert.ToInt32(cVal[1]))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                                case "<":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                        MidsContext.Config.TeamMembers[conditionItemName] <
                                        Convert.ToInt32(cVal[1]))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                            }

                            break;
                    }
                }

                int allValid = ActiveConditionals.Count;
                foreach (var condition in ActiveConditionals)
                {
                    if (!condition.Validated)
                        allValid -= 1;
                }

                if (allValid == ActiveConditionals.Count)
                {
                    Validated = true;
                }
                else
                {
                    Validated = false;
                }

                return Validated;

            }

            #endregion

            return false;
        }

        public bool CanGrantPower()
        {
            if (MidsContext.Character == null | ActiveConditionals == null | (ActiveConditionals?.Count == 0 && SpecialCase == Enums.eSpecialCase.None))
            {
                return true;
            }

            #region SpecialCase Processing

            if (SpecialCase != Enums.eSpecialCase.None)
            {
                switch (SpecialCase)
                {
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
                    case Enums.eSpecialCase.NotBoxingBuff:
                        if (MidsContext.Character.NotBoxingBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.KickBuff:
                        if (MidsContext.Character.KickBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotKickBuff:
                        if (MidsContext.Character.NotKickBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.CrossPunchBuff:
                        if (MidsContext.Character.CrossPunchBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotCrossPunchBuff:
                        if (MidsContext.Character.NotCrossPunchBuff)
                            return true;
                        break;
                    case Enums.eSpecialCase.Supremacy:
                        if (MidsContext.Character.Supremacy && !MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.SupremacyAndBuffPwr:
                        if (MidsContext.Character.Supremacy && MidsContext.Character.PackMentality)
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
                    case Enums.eSpecialCase.PackMentality:
                        if (MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotPackMentality:
                        if (!MidsContext.Character.PackMentality)
                            return true;
                        break;
                    case Enums.eSpecialCase.FastSnipe:
                        if (MidsContext.Character.FastSnipe)
                            return true;
                        break;
                    case Enums.eSpecialCase.NotFastSnipe:
                        if (!MidsContext.Character.FastSnipe)
                            return true;
                        break;
                }
            }

            #endregion

            if (AdvancedConditions is { Rows.Count: > 0 } || ActiveConditionals is { Count: > 0 })
            {
                Validated = BooleanExprPreprocessor.Parse(this);
                return Validated;
            }

            #region Conditional Processing

            if (ActiveConditionals.Count > 0)
            {
                var getCondition = new Regex("(:.*)");
                var getConditionItem = new Regex("(.*:)");
                foreach (var cVp in ActiveConditionals)
                {
                    var condition = getCondition.Replace(cVp.Key, "");
                    var conditionItemName = getConditionItem.Replace(cVp.Key, "").Replace(":", "");
                    var conditionPower = DatabaseAPI.GetPowerByFullName(conditionItemName);
                    var cVal = cVp.Value.Split(' ');
                    switch (condition)
                    {
                        case "Active":
                            if (conditionPower != null)
                            {
                                bool? boolVal = Convert.ToBoolean(cVp.Value);
                                if (MidsContext.Character.CurrentBuild.PowerActive(conditionPower) == boolVal)
                                {
                                    cVp.Validated = true;
                                }
                                else
                                {
                                    cVp.Validated = false;
                                }
                            }

                            break;
                        case "Taken":
                            if (conditionPower != null)
                            {
                                cVp.Validated = MidsContext.Character.CurrentBuild.PowerUsed(conditionPower)
                                    .Equals(Convert.ToBoolean(cVp.Value));
                            }

                            break;
                        case "Stacks":
                            if (conditionPower != null)
                            {
                                switch (cVal[0])
                                {
                                    case "=":

                                        cVp.Validated = conditionPower.Stacks.Equals(Convert.ToInt32(cVal[1]));

                                        break;
                                    case ">":
                                        cVp.Validated = conditionPower.Stacks > Convert.ToInt32(cVal[1]);

                                        break;
                                    case "<":
                                        cVp.Validated = conditionPower.Stacks < Convert.ToInt32(cVal[1]);

                                        break;
                                }
                            }

                            break;
                        case "Team":
                            switch (cVal[0])
                            {
                                case "=":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) && MidsContext
                                        .Config.TeamMembers[conditionItemName].Equals(Convert.ToInt32(cVal[1])))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                                case ">":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                        MidsContext.Config.TeamMembers[conditionItemName] >
                                        Convert.ToInt32(cVal[1]))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                                case "<":
                                    if (MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                        MidsContext.Config.TeamMembers[conditionItemName] <
                                        Convert.ToInt32(cVal[1]))
                                    {
                                        cVp.Validated = true;
                                    }
                                    else
                                    {
                                        cVp.Validated = false;
                                    }

                                    break;
                            }

                            break;
                    }
                }

                int allValid = ActiveConditionals.Count;
                foreach (var condition in ActiveConditionals)
                {
                    if (!condition.Validated)
                        allValid -= 1;
                }

                if (allValid == ActiveConditionals.Count)
                {
                    Validated = true;
                }
                else
                {
                    Validated = false;
                }

                return Validated;

            }

            #endregion

            return false;
        }

        public bool PvXInclude()
        {
            return MidsContext.Archetype == null ||
                   (PvMode != Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE ||
                    PvMode != Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE) &&
                   (nIDClassName == -1 || nIDClassName == MidsContext.Archetype.Idx);
        }

        public int CompareTo(object obj)
        {
            //Less than zero This instance is less than obj. 
            //Zero This instance is equal to obj. 
            //Greater than zero This instance is greater than obj. 
            //
            //A.CompareTo(A) is required to return zero.
            //
            //If A.CompareTo(B) returns zero then B.CompareTo(A) is required to return zero.
            //
            //If A.CompareTo(B) returns zero and B.CompareTo(C) returns zero then A.CompareTo(C) is required to return zero.
            //
            //If A.CompareTo(B) returns a value other than zero then B.CompareTo(A) is required to return a value of the opposite sign.
            //
            //If A.CompareTo(B) returns a value x not equal to zero, and B.CompareTo(C) returns a value y of the same sign as x, then A.CompareTo(C) is required to return a value of the same sign as x and y.

            if (obj == null)
                return 1;

            if (obj is Effect effect)
            {
                var nVariableFlag = 0;
                if (VariableModified & effect.VariableModified == false)
                {
                    nVariableFlag = 1;
                }
                else if (VariableModified == false & effect.VariableModified)
                {
                    nVariableFlag = -1;
                }

                if (nVariableFlag == 0)
                {
                    if (Suppression < effect.Suppression)
                    {
                        nVariableFlag = 1;
                    }
                    else if (Suppression > effect.Suppression)
                    {
                        nVariableFlag = -1;
                    }
                }

                if (effect.EffectType == Enums.eEffectType.None & EffectType != Enums.eEffectType.None)
                    return -1;
                if (effect.EffectType != Enums.eEffectType.None & EffectType == Enums.eEffectType.None)
                    return 1;

                if (EffectType > effect.EffectType)
                    return 1;
                if (EffectType < effect.EffectType)
                    return -1;

                if (IgnoreED && !effect.IgnoreED)
                    return 1;
                if (!IgnoreED && effect.IgnoreED)
                    return -1;

                if (EffectId != effect.EffectId)
                    return string.CompareOrdinal(EffectId, effect.EffectId);
                if (Reward != effect.Reward)
                    return string.CompareOrdinal(Reward, effect.Reward);

                if (Expressions.Magnitude != effect.Expressions.Magnitude)
                {
                    return string.CompareOrdinal(Expressions.Magnitude, effect.Expressions.Magnitude);
                }
                if (Expressions.Duration != effect.Expressions.Duration)
                {
                    return string.CompareOrdinal(Expressions.Duration, effect.Expressions.Duration);
                }

                if (Expressions.Probability != effect.Expressions.Probability)
                {
                    return string.CompareOrdinal(Expressions.Probability, effect.Expressions.Probability);
                }

                //EffectType is the same, go more detailed.
                if (effect.isDamage())
                {
                    if (DamageType > effect.DamageType)
                        return 1;
                    if (DamageType < effect.DamageType)
                        return -1;
                    if (Mag > effect.Mag)
                        return 1;
                    if (Mag < effect.Mag)
                        return -1;
                    return nVariableFlag;
                }
                if (effect.EffectType == Enums.eEffectType.ResEffect)
                {
                    if (ETModifies > effect.ETModifies)
                        return 1;
                    if (ETModifies < effect.ETModifies)
                        return -1;
                    if (Mag > effect.Mag)
                        return 1;
                    if (Mag < effect.Mag)
                        return -1;
                    return nVariableFlag;
                }
                if (effect.EffectType is Enums.eEffectType.Mez or Enums.eEffectType.MezResist)
                {
                    if (MezType > effect.MezType)
                        return 1;
                    if (MezType < effect.MezType)
                        return -1;
                    if (Mag > effect.Mag)
                        return 1;
                    if (Mag < effect.Mag)
                        return -1;
                    if (Duration > effect.Duration)
                        return 1;
                    if (Duration < effect.Duration)
                        return -1;
                    return nVariableFlag;
                }
                if (effect.EffectType == Enums.eEffectType.Enhancement)
                {
                    if (ETModifies > effect.ETModifies)
                        return 1;
                    if (ETModifies < effect.ETModifies)
                        return -1;
                    if (Mag > effect.Mag)
                        return 1;
                    if (Mag < effect.Mag)
                        return -1;
                    if (Duration > effect.Duration)
                        return 1;
                    if (Duration < effect.Duration)
                        return -1;
                    return nVariableFlag;
                }
                if (effect.EffectType == Enums.eEffectType.None)
                {
                    return string.CompareOrdinal(Special, effect.Special);
                }
                return nVariableFlag;
            }

            throw new ArgumentException("Compare failed, object is not a Power Effect class");
        }

        public bool AffectsPetsOnly()
        {
            var isSetBonusEffect = Reward.Contains("Set_Bonus");
            var effectPower = GetPower();
            var enhSet = DatabaseAPI.GetEnhancementSetByBoostName(effectPower.SetName);
            var isPetEnh = DatabaseAPI.GetSetTypeByIndex(enhSet.SetType).Name.Contains("Pet");
            return isSetBonusEffect && isPetEnh;
        }

        public Damage GetDamage()
        {
            if (EffectType != Enums.eEffectType.Damage ||
                MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum && !(Math.Abs(Probability) > 0.999000012874603) ||
                EffectClass == Enums.eEffectClass.Ignored ||
                this is { DamageType: Enums.eDamage.Special, ToWho: Enums.eToWho.Self } ||
                Probability <= 0 ||
                !CanInclude() ||
                !PvXInclude())
            {
                return new Damage { Type = Enums.eDamage.None, Value = 0 };
            }

            var effectDmg = BuffedMag;

            if (MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average)
            {
                effectDmg *= Probability;
            }

            if (power.PowerType == Enums.ePowerType.Toggle && isEnhancementEffect)
            {
                effectDmg = (float)(effectDmg * power.ActivatePeriod / 10d);
            }

            if (Ticks > 1)
            {
                effectDmg *= CancelOnMiss &&
                             MidsContext.Config.DamageMath.Calculate == ConfigData.EDamageMath.Average &&
                             Probability < 1
                    ? (float)((1 - Math.Pow(Probability, Ticks)) / (1 - Probability))
                    : Ticks;
            }

            return new Damage { Type = DamageType, Value = effectDmg };
        }

        public object Clone()
        {
            return new Effect(this);
        }

        private static string BuildCs(string iValue, string iStr, bool noComma = false)
        {
            if (string.IsNullOrEmpty(iValue))
            {
                return iStr;
            }

            if (!string.IsNullOrEmpty(iStr))
            {
                iStr += noComma ? " " : ", ";
            }

            iStr += iValue;

            return iStr;
        }

        private string _summonedEntName;

        public string SummonedEntityName
        {
            get
            {
                _summonedEntName = nSummon switch
                {
                    <= -1 => Summon,
                    > -1 when nSummon <= DatabaseAPI.Database.Entities.Length => DatabaseAPI.Database.Entities[nSummon]
                        .DisplayName,
                    _ => ""
                };

                return _summonedEntName;
            }
            /*set
            {
                _summonedEntName = value;
                if (nSummon <= -1)
                {
                    _summonedEntName = Summon;
                }
                else
                {
                    _summonedEntName = DatabaseAPI.Database.Entities[nSummon].DisplayName;
                }
            }*/
        }

        public EffectIdentifier GenerateIdentifier()
        {
            return new EffectIdentifier
            {
                Mag = BuffedMag,
                EffectType = EffectType,
                ETModifies = ETModifies,
                ToWho = ToWho,
                PvMode = PvMode,
                Suppression = Suppression,
                Conditionals = ActiveConditionals,
                IgnoreScaling = IgnoreScaling,
                IgnoreED = IgnoreED,
                Buffable = Buffable,
                Probability = Probability,
                Duration = Duration,
                Ticks = Ticks,
                SpecialCase = SpecialCase,
                Stacking = Stacking,
                RequiresToHitCheck = RequiresToHitCheck,
                CancelOnMiss = CancelOnMiss,
                Resistible = Resistible,
                DelayedTime = DelayedTime,
                PPM = ProcsPerMinute
            };
        }
    }

    public struct Damage
    {
        public Enums.eDamage Type;
        public float Value;
    }

    public struct DamageExt
    {
        public Enums.eDamage Type;
        public float Value;
        public int Ticks;
        public bool HasPercentage;

        public override string ToString()
        {
            var dmg = Value * (HasPercentage ? 100 : 1);
            dmg = Ticks <= 0 ? dmg : dmg / Ticks;
            var dmgStr = $"{Utilities.FixDP(dmg)}{(HasPercentage ? "%" : "")}";

            return Ticks <= 0
                ? dmgStr
                : $"{Ticks}x{dmgStr}";
        }

        public string Stringify(bool longFormat = true)
        {
            var dmg = Value * (HasPercentage ? 100 : 1);
            dmg = Ticks <= 0 ? dmg : dmg / Ticks;
            var dmgStr = Utilities.FixDP(dmg);
            dmgStr = Ticks <= 0
                ? dmgStr
                : $"{Ticks}x{dmgStr}";

            return longFormat ? $"{Type} ({dmgStr})" : dmgStr;
        }
    }

    public class KeyValue<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyValue()
        {
            Validated = false;
        }

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public bool Validated { get; set; }
    }

    public struct EffectIdentifier
    {
        public float Mag;
        public Enums.eEffectType EffectType;
        public Enums.eEffectType ETModifies;
        public Enums.eToWho ToWho;
        public Enums.ePvX PvMode;
        public Enums.eSuppress Suppression;
        public List<KeyValue<string, string>> Conditionals;
        public bool IgnoreScaling;
        public bool IgnoreED;
        public bool Buffable;
        public float Probability;
        public float Duration;
        public int Ticks;
        public Enums.eSpecialCase SpecialCase;
        public Enums.eStacking Stacking;
        public bool RequiresToHitCheck;
        public bool CancelOnMiss;
        public bool Resistible;
        public float DelayedTime;
        public float PPM;

        /// <summary>
        /// Compare an EffectIdentifier instance against another.
        /// </summary>
        /// <remarks>Only atomic fields are taken in account here, comparing conditionals yields to aberrant results</remarks>
        /// <param name="target">The other EffectIdentifier instance to compare with.</param>
        /// <returns>true if all atomic fields are equals, false otherwise. For floats, they're considered equals when distance is below float.Epsilon .</returns>
        public bool Compare(EffectIdentifier target)
        {
            return Math.Abs(Math.Round(Mag, 3) - Math.Round(target.Mag, 3)) < float.Epsilon &
                   EffectType == target.EffectType &
                   ETModifies == target.ETModifies &
                   ToWho == target.ToWho &
                   PvMode == target.PvMode &
                   Suppression == target.Suppression &
                   IgnoreScaling == target.IgnoreScaling &
                   IgnoreED == target.IgnoreED &
                   Buffable == target.Buffable &
                   Math.Abs(Probability - target.Probability) < float.Epsilon &
                   Math.Abs(Duration - target.Duration) < float.Epsilon &
                   Ticks == target.Ticks &
                   SpecialCase == target.SpecialCase &
                   Stacking == target.Stacking &
                   RequiresToHitCheck == target.RequiresToHitCheck &
                   CancelOnMiss == target.CancelOnMiss &
                   Resistible == target.Resistible &
                   Math.Abs(DelayedTime - target.DelayedTime) < float.Epsilon &
                   Math.Abs(PPM - target.PPM) < float.Epsilon;
            //Conditionals.Equals(target.Conditionals);
        }
    }
}

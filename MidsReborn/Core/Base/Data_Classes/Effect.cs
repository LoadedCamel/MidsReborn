using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.PlannerRulesets;
using System.Text.RegularExpressions;
using static Mids_Reborn.Core.Expressions;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Effect : IEffect, IComparable, ICloneable
    {
        private const string AdvancedConditionsMarker = "MRB_ADVANCED_EFFECT_CONDITIONS";
        private const string ModePayloadMarker = "MRB_EFFECT_MODE_PAYLOAD";
        private const string EffectTagsMarker = "MRB_EFFECT_TAGS";
        private const string OmniSourceMarker = "MRB_EFFECT_OMNI_SOURCE";
        private const string CombatModFlagsMarker = "MRB_EFFECT_COMBAT_MOD_FLAGS";
        private const string GrantBoostedMarker = "MRB_EFFECT_GRANT_BOOSTED";
        private static readonly Regex UidClassRegex = new("arch source(.owner)?> (Class_[^ ]*)", RegexOptions.IgnoreCase);

        private IPower? power;

        private static string FormatChancePercent(float probability)
        {
            return $"{DisplayValueFormatter.FormatPercentFromScale(probability, probability >= 0.975f ? 1 : 0)}%";
        }

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
            PowerFullName = string.Empty;
            Absorbed_PowerType = Enums.ePowerType.Auto_;
            Absorbed_Power_nID = -1;
            Absorbed_Class_nID = -1;
            Absorbed_EffectID = -1;
            Override = string.Empty;
            buffMode = Enums.eBuffMode.Normal;
            Special = string.Empty;
            EffectId = "Ones";
            EffectTags = [];
            OmniSource = string.Empty;
            GrantBoosted = false;
            ModeName = string.Empty;
            ModeId = -1;
            ModeFlag = Enums.eModeFlags.None;
            RevokedPower = string.Empty;
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

            TryReadModePayload(reader);
            if (!TryReadEffectTags(reader) && !string.IsNullOrWhiteSpace(EffectId))
            {
                EffectTags = [EffectId];
            }

            TryReadOmniSource(reader);
            TryReadCombatModFlags(reader);
            TryReadGrantBoosted(reader);
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
            NearGround = template.NearGround;
            CancelOnMiss = template.CancelOnMiss;
            ProcsPerMinute = template.ProcsPerMinute;
            Absorbed_Duration = template.Absorbed_Duration;
            PseudoPetRecurrence = template.PseudoPetRecurrence;
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
            EffectTags = template.EffectTags?.ToList() ?? [];
            OmniSource = template.OmniSource;
            GrantBoosted = template.GrantBoosted;
            UseCombatModMagnitude = template.UseCombatModMagnitude;
            UseCombatModDuration = template.UseCombatModDuration;
            IgnoreED = template.IgnoreED;
            Override = template.Override;
            ModeName = template.ModeName;
            ModeId = template.ModeId;
            ModeFlag = template.ModeFlag;
            RevokedPower = template.RevokedPower;
            ActiveConditionals = template.ActiveConditionals;
            AdvancedConditions = template.AdvancedConditions?.Clone() ?? AdvancedConditionSet.FromLegacyActiveConditionals(ActiveConditionals);
        }

        private int? SummonId { get; set; }

        private int? OverrideId { get; set; }

        public string MagnitudeExpression { get; set; }

        public Expressions Expressions { get; set; }

        public AdvancedConditionSet AdvancedConditions { get; set; }

        public string OmniSource { get; set; }
        public bool GrantBoosted { get; set; }
        public bool UseCombatModMagnitude { get; set; }
        public bool UseCombatModDuration { get; set; }

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
                    probability = DatabaseAPI.GetPlannerRuleset()
                        .CalculateProcProbability(power, ProcsPerMinute, probability);
                }

                probability = DatabaseAPI.GetPlannerRuleset()
                    .ApplyChanceModifiers(MidsContext.Character, power, this, probability);

                return Math.Max(0, Math.Min(1, probability));
            }
        }

        public float MinProcChance => DatabaseAPI.GetPlannerRuleset().GetMinProcChance(ProcsPerMinute);
        public float MaxProcChance => DatabaseAPI.GetPlannerRuleset().GetMaxProcChance(ProcsPerMinute);

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

        public List<string> EffectTags { get; set; }

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

        public PseudoPetRecurrenceInfo? PseudoPetRecurrence { get; set; }

        public Enums.eBuffMode buffMode { get; set; }

        public int UniqueID { get; set; }

        public string Override { get; set; }

        public string ModeName { get; set; }

        public int ModeId { get; set; }

        public Enums.eModeFlags ModeFlag { get; set; }

        public string RevokedPower { get; set; }

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

        private string GetDisplayClassName()
        {
            if (power is Power concretePower && !string.IsNullOrWhiteSpace(concretePower.OmniDisplayClassName))
            {
                return concretePower.OmniDisplayClassName;
            }

            return DatabaseAPI.ResolveClassName();
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
                    chanceText = $"{DisplayValueFormatter.FormatPercentFromScale(BaseProbability, 0)}% chance";
            }
            else
            {
                if (Probability < 1.0)
                    chanceText = $"{DisplayValueFormatter.FormatPercentFromScale(Probability, 0)}% chance";
            }

            // Magnitude (percent or raw) unless suppressed
            if (!noMag)
            {
                magText = DisplayValueFormatter.FormatMagnitude(MagPercent);
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
                                if (Duration > 0.0) trailing = $" over {DisplayValueFormatter.FormatSeconds(Duration)} seconds";
                                else if (Absorbed_Duration > 0.0) trailing = $" over {DisplayValueFormatter.FormatSeconds(Absorbed_Duration)} seconds";
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
                        result = $"{effectLabel}{shown}{FormatGrantBoostedSuffix()}{toWhoText}";
                        break;
                    }

                case Enums.eEffectType.SetMode:
                case Enums.eEffectType.UnsetMode:
                    {
                        var shown = FormatModePayload();
                        result = string.IsNullOrWhiteSpace(shown)
                            ? $"{effectLabel}{toWhoText}{trailing}"
                            : $"{effectLabel} {shown}{toWhoText}{trailing}";
                        break;
                    }

                case Enums.eEffectType.RevokePower:
                    {
                        var revoke = FormatPowerPayload(RevokedPower, Override);
                        result = string.IsNullOrWhiteSpace(revoke)
                            ? $"{effectLabel}{toWhoText}"
                            : $"{effectLabel} {revoke}{toWhoText}";
                        break;
                    }

                case Enums.eEffectType.Heal:
                case Enums.eEffectType.HitPoints:
                    {
                        if (noMag) { result = "+Max HP"; break; }

                        if (Aspect == Enums.eAspect.Cur)
                        {
                            // Current-value heal uses % of HP directly
                            result = $"{DisplayValueFormatter.FormatPercentFromScale(BuffedMag)}% {effectLabel}{toWhoText}{trailing}";
                        }
                        else if (!DisplayPercentage)
                        {
                            // Non-% display: show (percent of Max HP) after raw value
                            var baseHitPoints = DatabaseAPI.GetClassHitPoints(GetDisplayClassName());
                            var pctOfMax = DisplayValueFormatter.FormatPercentValue(BuffedMag / baseHitPoints * 100d);
                            result = $"{magText} ({pctOfMax}%) {effectLabel}{toWhoText}{trailing}";
                        }
                        else
                        {
                            // % display: also show raw HP from %
                            var rawHp = DisplayValueFormatter.FormatNumber(BuffedMag / 100f * DatabaseAPI.GetClassHitPoints(GetDisplayClassName()));
                            result = $"{rawHp} ({magText}) {effectLabel}{toWhoText}{trailing}";
                        }
                        break;
                    }

                case Enums.eEffectType.Mez:
                    {
                        var mezName = Enum.GetName(MezType.GetType(), MezType); // uses eMez long token
                        if (Duration > 0.0 && (!simple || (MezType != Enums.eMez.None && MezType != Enums.eMez.Knockback && MezType != Enums.eMez.Knockup)))
                            trailing = $"{DisplayValueFormatter.FormatSeconds(Duration)} second ";

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
                            var perSec = DisplayValueFormatter.FormatRate(BuffedMag * (DatabaseAPI.GetClassBaseRecovery(GetDisplayClassName()) * Statistics.BaseMagic));
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
                            var hps = DisplayValueFormatter.FormatRate(DatabaseAPI.GetClassHitPoints(GetDisplayClassName()) / 100.0 * (BuffedMag * DatabaseAPI.GetClassBaseRegen(GetDisplayClassName()) * 1.66666662693024));
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
                // var aff = power?.EntitiesAffected ?? Enums.eEntity.None;
                // var aut = power?.EntitiesAutoHit ?? Enums.eEntity.None;
                // sTarget = ToWho.ToPhrase(aff, aut);
                sTarget = ToWho switch
                {
                    Enums.eToWho.Target => " to Target",
                    Enums.eToWho.Self => " to Self",
                    _ => sTarget
                };
                if (RequiresToHitCheck) sToHit = " requires ToHit check";
            }

            // ----- Chance / PPM / Base-vs-current probability -----
            if (AttribType == Enums.eAttribType.Expression && !string.IsNullOrWhiteSpace(Expressions.Probability))
            {
                var pct = (decimal)Math.Max(0, Math.Min(100, Parse(this, ExpressionType.Probability, out _) * 100));
                var formattedChance = DisplayValueFormatter.FormatPercentValue((double)pct, 0);
                sChance = editorDisplay ? $"{formattedChance}% Variable Chance" : $"{formattedChance}% chance";
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
                        sChance = $"{FormatChancePercent(BaseProbability)} chance";
                        sChance += EffectId is "" or "Ones" ? "" : " ";
                        if (EffectId is not "" and not "Ones") sChance += $"when {EffectId}";
                        if (CancelOnMiss) sChance += ", Cancels on Miss";
                        if (ProcsPerMinute > 0)
                            sChance = fromPopup | editorDisplay
                                ? $"{ProcsPerMinute} PPM"
                                : $"{ProcsPerMinute} PPM/{DisplayValueFormatter.FormatPercentFromScale(Probability, 0)}% chance";
                    }
                }
                else
                {
                    if (Probability < 1 && Probability >= 0)
                    {
                        sChance = $"{FormatChancePercent(Probability)} chance";
                        sChance += EffectId is "" or "Ones" ? "" : " ";
                        if (EffectId is not "" and not "Ones" && !fromPopup) sChance += $"when {EffectId}";
                        if (CancelOnMiss) sChance += ", Cancels on Miss";
                        if (ProcsPerMinute > 0)
                            sChance = fromPopup | editorDisplay
                                ? $"{ProcsPerMinute} PPM"
                                : $"{ProcsPerMinute} PPM/{DisplayValueFormatter.FormatPercentFromScale(Probability, 0)}% chance";
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
                    sDelay = $"after {DisplayValueFormatter.FormatSeconds(DelayedTime)} seconds";
            }

            // Conditions / SpecialCase
            if (!ignoreConditions)
            {
                if (SpecialCase != Enums.eSpecialCase.None & SpecialCase != Enums.eSpecialCase.Defiance)
                    sSpecial = Enum.GetName(SpecialCase.GetType(), SpecialCase);

                if (AdvancedConditions is { Rows.Count: > 0 })
                {
                    sConditional = FormatAdvancedConditionSummary(AdvancedConditions);
                }
                else if (ActiveConditionals.Count > 0)
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
                    sDuration += $"{sForOver}{DisplayValueFormatter.FormatSeconds(Duration)} seconds";
                else if (Absorbed_Duration > 0 & (EffectType != Enums.eEffectType.Damage | Ticks > 0))
                    sDuration += $"{sForOver}{DisplayValueFormatter.FormatSeconds(Absorbed_Duration)} seconds";
                else
                    sDuration += " ";

                if (Absorbed_Interval > 0 & Absorbed_Interval < 900)
                    sDuration +=
                        $" every {DisplayValueFormatter.FormatSeconds(Absorbed_Interval)} seconds{(EffectType == Enums.eEffectType.Mez && (MezType is Enums.eMez.Knockback or Enums.eMez.Knockup) ? ": " : "")}";

                if (PseudoPetRecurrence is { IsValid: true } recurrence)
                {
                    sDuration += $" ({recurrence.ToDisplayText()})";
                }
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
                            ? $"{DisplayValueFormatter.FormatNumber(Math.Abs(mag), 2)}{(DisplayPercentage ? "%" : "")} Variable"
                            : $"{DisplayValueFormatter.FormatNumber(mag, 2)}{(DisplayPercentage ? "%" : "")} Variable";

                        sMagExp = $"Mag Expression: {Expressions.Magnitude.Replace("modifier>current", ModifierTable)}";
                    }
                    else
                    {
                        sMag = mag > float.Epsilon && absAllowed.Any(x => x == EffectType)
                            ? $"{DisplayValueFormatter.FormatNumber(Math.Abs(mag), 2)}{(DisplayPercentage ? "%" : "")}"
                            : $"{DisplayValueFormatter.FormatNumber(mag, 1)}{(DisplayPercentage ? "%" : "")}";
                    }
                }
                else if (EffectType == Enums.eEffectType.PerceptionRadius)
                {
                    var perceptionDistance = Statistics.BasePerception * BuffedMag;
                    sMag = MidsContext.Config.CoDEffectFormat & !fromPopup
                        ? $"({Scale * (AttribType == Enums.eAttribType.Magnitude ? nMagnitude : 1):####0.####} x {ModifierTable}){(DisplayPercentage ? "%" : "")} ({perceptionDistance}ft)"
                        : DisplayPercentage
                            ? $"{DisplayValueFormatter.FormatPercentFromScale(BuffedMag)}% ({DisplayValueFormatter.FormatDistance(perceptionDistance)}ft)"
                            : $"{DisplayValueFormatter.FormatDistance(perceptionDistance)}ft";
                }
                else
                {
                    sMag = MidsContext.Config.CoDEffectFormat & EffectType != Enums.eEffectType.Mez & !fromPopup
                        ? $"({Scale * (AttribType == Enums.eAttribType.Magnitude ? nMagnitude : 1):####0.####} x {ModifierTable}){(DisplayPercentage ? "%" : "")}"
                        : $"{(EffectType == Enums.eEffectType.Enhancement & ETModifies != Enums.eEffectType.EnduranceDiscount ? BuffedMag > 0 ? "+" : "-" : "")}{DisplayValueFormatter.FormatNumber(BuffedMag * (DisplayPercentage ? 100 : 1))}{(DisplayPercentage ? "%" : "")}";
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
                            if (PseudoPetRecurrence is { IsValid: true } recurrence) sMag = $"{recurrence.TicksPerSpawn} x {sMag}";
                            else if (Ticks > 0) sMag = $"{Ticks} x {sMag}";
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
                                $"{(MidsContext.Config.CoDEffectFormat & !fromPopup ? $"({Scale:####0.####} x {ModifierTable})" : DisplayValueFormatter.FormatSeconds(Duration))} second ";
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
                            sBuild = $"{DisplayValueFormatter.FormatPercentFromScale(BuffedMag)}% {sEffect}{sTarget}{sDuration}";
                        }
                        else
                        {
                            sBuild = DisplayPercentage
                                ? $"{DisplayValueFormatter.FormatNumber(BuffedMag / 100 * DatabaseAPI.GetClassHitPoints(GetDisplayClassName()))} HP ({sMag}) {sEffect}{sTarget}{sDuration}"
                                : $"{sMag} HP ({DisplayValueFormatter.FormatPercentValue(BuffedMag / DatabaseAPI.GetClassHitPoints(GetDisplayClassName()) * 100d)}%) {sEffect}{sTarget}{sDuration}";
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
                            ? $"{sMag} ({DisplayValueFormatter.FormatRate(DatabaseAPI.GetClassHitPoints(GetDisplayClassName()) / 100f * (BuffedMag * DatabaseAPI.GetClassBaseRegen(GetDisplayClassName()) * Statistics.BaseMagic))} HP/sec) {sEffect}{sTarget}{sDuration}"
                            : $"{sMag} {sEffect}{sTarget}{sDuration}")
                        : "+Regeneration";
                    break;

                case Enums.eEffectType.Recovery:
                    sBuild = !noMag
                        ? (DisplayPercentage
                            ? $"{sMag} ({DisplayValueFormatter.FormatRate(BuffedMag * (DatabaseAPI.GetClassBaseRecovery(GetDisplayClassName()) * Statistics.BaseMagic))} End/sec) {sEffect}{sTarget}{sDuration}"
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
                        $"{sEffect}{tGrant}{FormatGrantBoostedSuffix()}{sTarget}{(Math.Abs(Duration) < float.Epsilon ? "" : $" for {Duration}s")}{(Ticks > 0 & EffectType == Enums.eEffectType.ExecutePower ? $" ({Ticks} tick{(Ticks == 1 ? "" : "s")})" : "")}";
                    break;
                }

                case Enums.eEffectType.GlobalChanceMod:
                    sBuild = $"{sMag} {sEffect} {Reward}{sTarget}{sDuration}";
                    break;

                case Enums.eEffectType.PowerRedirect:
                {
                    sBuild = !string.IsNullOrWhiteSpace(Override)
                        ? $"{sEffect}{sTarget} ({DatabaseAPI.GetPowerByFullName(Override)?.DisplayName ?? Override})"
                        : $"{sEffect}{sTarget} ({Override})";
                    break;
                }

                case Enums.eEffectType.SetMode:
                case Enums.eEffectType.UnsetMode:
                {
                    var mode = FormatModePayload();
                    sBuild = string.IsNullOrWhiteSpace(mode)
                        ? $"{sEffect}{sTarget}{sDuration}"
                        : $"{sEffect} {mode}{sTarget}{sDuration}";
                    break;
                }

                case Enums.eEffectType.RevokePower:
                {
                    var revoke = FormatPowerPayload(RevokedPower, Override);
                    sBuild = string.IsNullOrWhiteSpace(revoke)
                        ? $"{sEffect}{sTarget}"
                        : $"{sEffect} {revoke}{sTarget}";
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

        private static string FormatAdvancedConditionSummary(AdvancedConditionSet conditions)
        {
            var parts = new List<string>();
            for (var i = 0; i < conditions.Rows.Count; i++)
            {
                var row = conditions.Rows[i];
                var rowText = FormatAdvancedConditionRow(row);
                if (string.IsNullOrWhiteSpace(rowText))
                {
                    continue;
                }

                var link = parts.Count == 0
                    ? string.Empty
                    : row.Link == AdvancedConditionLink.Or ? "OR " : "AND ";
                parts.Add($"{link}{rowText}");
            }

            return string.Join(" ", parts);
        }

        private static string FormatAdvancedConditionRow(AdvancedConditionRow row)
        {
            return row.Kind switch
            {
                AdvancedConditionKind.PowerActive => FormatPowerCondition("Power active", row.Subject, row.Value, row.Negated),
                AdvancedConditionKind.PowerTaken => FormatPowerCondition("Power taken", row.Subject, row.Value, row.Negated),
                AdvancedConditionKind.SourceOwnPower => FormatOwnPowerCondition(row.Subject, row.Negated),
                AdvancedConditionKind.SourceMode => $"Mode is {(row.Negated ? "not " : string.Empty)}{FormatModeName(row)}",
                AdvancedConditionKind.TargetMode => $"Target mode is {(row.Negated ? "not " : string.Empty)}{FormatModeName(row)}",
                AdvancedConditionKind.TargetEntityType when row.TargetScope != AdvancedConditionTargetScope.Unknown => $"Target is {FormatTargetScope(row.TargetScope)}",
                AdvancedConditionKind.TargetEntityType => $"Target entity {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.CharacterArchetype => $"Archetype {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.CharacterLevel => $"Level {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.PowerStacks => $"{FormatPowerName(row.Subject)} stacks {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.TeamMembers => $"Team members {CleanConditionValue(row.Subject)} {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.CombatSetting => $"{ConfigData.CombatContext.FormatSettingName(row.Subject)} {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.PowerCount => $"{CleanConditionValue(row.Subject)} count {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.PowerRequirementGroup => FormatPowerRequirementGroup(row),
                AdvancedConditionKind.BoostsSlotted => $"Boosts slotted {CleanConditionValue(row.Subject)} {FormatOperatorPhrase(row.Operator)} {CleanConditionValue(row.Value)}",
                AdvancedConditionKind.AdvancedExpression => FormatRawAdvancedExpression(row.RawExpression),
                _ => FormatRawAdvancedExpression(row.RawExpression)
            };
        }

        private static string FormatPowerCondition(string label, string powerFullName, string value, bool negated)
        {
            var falseValue = bool.TryParse(value, out var boolValue) && !boolValue;
            return $"{label} is {(negated ^ falseValue ? "not " : string.Empty)}{FormatPowerName(powerFullName)}";
        }

        private static string FormatOwnPowerCondition(string powerFullName, bool negated)
        {
            return $"{(negated ? "Does not have" : "Has")} {FormatPowerName(powerFullName)}";
        }

        private static string FormatPowerRequirementGroup(AdvancedConditionRow row)
        {
            var first = FormatPowerName(row.Subject);
            if (string.IsNullOrWhiteSpace(row.Value))
            {
                return $"Requires {first}";
            }

            return $"Requires {first} and {FormatPowerName(row.Value)}";
        }

        private static string FormatPowerName(string powerFullName)
        {
            if (string.IsNullOrWhiteSpace(powerFullName))
            {
                return string.Empty;
            }

            return DatabaseAPI.GetPowerByFullName(powerFullName)?.DisplayName ?? powerFullName;
        }

        private static string FormatModeName(AdvancedConditionRow row)
        {
            var mode = row.Subject;
            if (row.RawExpression.Contains("kEngaged", StringComparison.OrdinalIgnoreCase))
            {
                mode = "Engaged";
            }

            return CleanConditionValue(mode)
                .Replace("FastSnipe", "Fast Snipe", StringComparison.OrdinalIgnoreCase)
                .Replace("CriticalHit", "Critical Hit", StringComparison.OrdinalIgnoreCase)
                .Replace("DefensiveAdaptation", "Defensive Adaptation", StringComparison.OrdinalIgnoreCase)
                .Replace("EfficientAdaptation", "Efficient Adaptation", StringComparison.OrdinalIgnoreCase)
                .Replace("OffensiveAdaptation", "Offensive Adaptation", StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatTargetScope(AdvancedConditionTargetScope scope)
        {
            return scope switch
            {
                AdvancedConditionTargetScope.Self => "Self",
                AdvancedConditionTargetScope.Pet => "Pet",
                AdvancedConditionTargetScope.Player => "Player",
                AdvancedConditionTargetScope.Ally => "Ally",
                AdvancedConditionTargetScope.Foe => "Foe",
                _ => "Unknown"
            };
        }

        private static string FormatOperatorPhrase(AdvancedConditionOperator op)
        {
            return op switch
            {
                AdvancedConditionOperator.NotEquals => "is not",
                AdvancedConditionOperator.GreaterThan => ">",
                AdvancedConditionOperator.LessThan => "<",
                AdvancedConditionOperator.GreaterThanOrEqual => ">=",
                AdvancedConditionOperator.LessThanOrEqual => "<=",
                _ => "is"
            };
        }

        private static string FormatRawAdvancedExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return string.Empty;
            }

            var cleaned = expression.Trim();
            if (cleaned.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                cleaned.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            cleaned = Regex.Replace(
                cleaned,
                @"(?:source\.)?Mode\?\(([^)]+)\)",
                match => $"Mode is {FormatModeName(match.Groups[1].Value)}",
                RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(
                cleaned,
                @"(?:source\.)?ownPower\?\(([^)]+)\)",
                match => $"Has {FormatPowerName(match.Groups[1].Value)}",
                RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(
                cleaned,
                @"\b([^\s()]+)\s+Mode\?",
                match => $"Mode is {FormatModeName(match.Groups[1].Value)}",
                RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(
                cleaned,
                @"\b([^\s()]+)\s+ownPower\?",
                match => $"Has {FormatPowerName(match.Groups[1].Value)}",
                RegexOptions.IgnoreCase);

            return CleanConditionValue(cleaned)
                .Replace("||", "OR", StringComparison.OrdinalIgnoreCase)
                .Replace("&&", "AND", StringComparison.OrdinalIgnoreCase)
                .Replace("( ", "(")
                .Replace(" )", ")")
                .Replace("  ", " ");
        }

        private static string FormatModeName(string mode)
        {
            return CleanConditionValue(mode)
                .Replace("FastSnipe", "Fast Snipe", StringComparison.OrdinalIgnoreCase)
                .Replace("CriticalHit", "Critical Hit", StringComparison.OrdinalIgnoreCase)
                .Replace("DefensiveAdaptation", "Defensive Adaptation", StringComparison.OrdinalIgnoreCase)
                .Replace("EfficientAdaptation", "Efficient Adaptation", StringComparison.OrdinalIgnoreCase)
                .Replace("OffensiveAdaptation", "Offensive Adaptation", StringComparison.OrdinalIgnoreCase);
        }

        private static string CleanConditionValue(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Trim('\'', '"')
                .Replace("kEngaged", "Engaged", StringComparison.OrdinalIgnoreCase)
                .Replace("kFastSnipe", "Fast Snipe", StringComparison.OrdinalIgnoreCase)
                .Replace('_', ' ');
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

        private string FormatModePayload()
        {
            if (!string.IsNullOrWhiteSpace(ModeName))
            {
                return ModeName;
            }

            if (ModeFlag != Enums.eModeFlags.None)
            {
                return ModeFlag.ToString();
            }

            return ModeId >= 0
                ? $"Mode {ModeId}"
                : string.Empty;
        }

        private static string FormatPowerPayload(params string?[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                return DatabaseAPI.GetPowerByFullName(candidate)?.DisplayName ?? candidate;
            }

            return string.Empty;
        }

        private string FormatGrantBoostedSuffix()
        {
            return EffectType == Enums.eEffectType.GrantPower && GrantBoosted
                ? " (inherits source slotting)"
                : string.Empty;
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
            StoreModePayload(writer);
            StoreEffectTags(writer);
            StoreOmniSource(writer);
            StoreCombatModFlags(writer);
            StoreGrantBoosted(writer);
        }

        private void StoreModePayload(BinaryWriter writer)
        {
            writer.Write(ModePayloadMarker);
            writer.Write(1);
            writer.Write(ModeName ?? string.Empty);
            writer.Write(ModeId);
            writer.Write((int)ModeFlag);
            writer.Write(RevokedPower ?? string.Empty);
        }

        private void TryReadModePayload(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!string.Equals(reader.ReadString(), ModePayloadMarker, StringComparison.Ordinal))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported effect mode payload version {version}.");
                }

                ModeName = reader.ReadString();
                ModeId = reader.ReadInt32();
                ModeFlag = (Enums.eModeFlags)reader.ReadInt32();
                RevokedPower = reader.ReadString();
            }
            catch (EndOfStreamException)
            {
                reader.BaseStream.Position = position;
            }
            catch (IOException)
            {
                reader.BaseStream.Position = position;
            }
        }

        private void StoreEffectTags(BinaryWriter writer)
        {
            writer.Write(EffectTagsMarker);
            writer.Write(1);
            var tags = DistinctTags(EffectTags).ToArray();
            writer.Write(tags.Length);
            foreach (var tag in tags)
            {
                writer.Write(tag);
            }
        }

        private bool TryReadEffectTags(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return false;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!string.Equals(reader.ReadString(), EffectTagsMarker, StringComparison.Ordinal))
                {
                    reader.BaseStream.Position = position;
                    return false;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported effect tags version {version}.");
                }

                var count = reader.ReadInt32();
                EffectTags = [];
                for (var index = 0; index < count; index++)
                {
                    var tag = reader.ReadString();
                    if (!string.IsNullOrWhiteSpace(tag) &&
                        !EffectTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                    {
                        EffectTags.Add(tag);
                    }
                }

                return true;
            }
            catch (EndOfStreamException)
            {
                reader.BaseStream.Position = position;
                return false;
            }
            catch (IOException)
            {
                reader.BaseStream.Position = position;
                return false;
            }
        }

        private void StoreOmniSource(BinaryWriter writer)
        {
            writer.Write(OmniSourceMarker);
            writer.Write(1);
            writer.Write(OmniSource ?? string.Empty);
        }

        private void TryReadOmniSource(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!string.Equals(reader.ReadString(), OmniSourceMarker, StringComparison.Ordinal))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported effect Omni source version {version}.");
                }

                OmniSource = reader.ReadString();
            }
            catch (EndOfStreamException)
            {
                reader.BaseStream.Position = position;
            }
            catch (IOException)
            {
                reader.BaseStream.Position = position;
            }
        }

        private void StoreCombatModFlags(BinaryWriter writer)
        {
            writer.Write(CombatModFlagsMarker);
            writer.Write(1);
            writer.Write(UseCombatModMagnitude);
            writer.Write(UseCombatModDuration);
        }

        private void StoreGrantBoosted(BinaryWriter writer)
        {
            writer.Write(GrantBoostedMarker);
            writer.Write(1);
            writer.Write(GrantBoosted);
        }

        private void TryReadCombatModFlags(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!string.Equals(reader.ReadString(), CombatModFlagsMarker, StringComparison.Ordinal))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported effect combat-mod flags version {version}.");
                }

                UseCombatModMagnitude = reader.ReadBoolean();
                UseCombatModDuration = reader.ReadBoolean();
            }
            catch (EndOfStreamException)
            {
                reader.BaseStream.Position = position;
            }
            catch (IOException)
            {
                reader.BaseStream.Position = position;
            }
        }

        private void TryReadGrantBoosted(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek)
            {
                return;
            }

            var position = reader.BaseStream.Position;
            try
            {
                if (!string.Equals(reader.ReadString(), GrantBoostedMarker, StringComparison.Ordinal))
                {
                    reader.BaseStream.Position = position;
                    return;
                }

                var version = reader.ReadInt32();
                if (version > 1)
                {
                    throw new InvalidDataException($"Unsupported effect grant-boosted version {version}.");
                }

                GrantBoosted = reader.ReadBoolean();
            }
            catch (EndOfStreamException)
            {
                reader.BaseStream.Position = position;
            }
            catch (IOException)
            {
                reader.BaseStream.Position = position;
            }
        }

        private static IEnumerable<string> DistinctTags(IEnumerable<string>? tags)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tag in tags ?? [])
            {
                if (!string.IsNullOrWhiteSpace(tag) && seen.Add(tag.Trim()))
                {
                    yield return tag.Trim();
                }
            }
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
            var hasAdvancedConditionals = AdvancedConditions is { Rows.Count: > 0 };
            var hasLegacyConditionals = ActiveConditionals is { Count: > 0 };
            if (MidsContext.Character == null ||
                (!hasAdvancedConditionals && !hasLegacyConditionals && SpecialCase == Enums.eSpecialCase.None))
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

            if (hasAdvancedConditionals || hasLegacyConditionals)
            {
                Validated = BooleanExprPreprocessor.Parse(this);
                return Validated;
            }

            #region Conditional Processing

            if (ActiveConditionals is { Count: > 0 })
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
            return DatabaseAPI.GetPlannerRuleset().EffectMatchesCurrentMode(this);
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
            if (!Power.ShouldIncludeDamageEffect(this))
            {
                return new Damage { Type = Enums.eDamage.None, Value = 0 };
            }

            var owner = power ?? GetPower();
            return new Damage
            {
                Type = DamageType,
                Value = Power.GetDamageEffectTotal(this, owner, absolute: false, applyReturnScaling: false)
            };
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
            var dmgStr = $"{DisplayValueFormatter.FormatNumber(dmg)}{(HasPercentage ? "%" : "")}";

            return Ticks <= 0
                ? dmgStr
                : $"{Ticks}x{dmgStr}";
        }

        public string Stringify(bool longFormat = true)
        {
            var dmg = Value * (HasPercentage ? 100 : 1);
            dmg = Ticks <= 0 ? dmg : dmg / Ticks;
            var dmgStr = DisplayValueFormatter.FormatNumber(dmg);
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

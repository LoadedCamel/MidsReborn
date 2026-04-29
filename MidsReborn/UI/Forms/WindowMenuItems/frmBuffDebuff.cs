using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public partial class frmBuffDebuff : Form
{
    #region Private enums
    private enum MagType
    {
        Positive,
        Negative
    }

    private enum EffectGroup
    {
        Mitigation,
        Sustain,
        Dps,
        Misc
    }

    private enum EffectBuffType
    {
        Buff,
        Debuff
    }

    public enum ValueDisplayMode
    {
        Raw,
        Duration,
        ValuePerEnd,
        ValuePerRecharge,
        ValuePerRechargeEnd
    }

    private enum ValueGroupMode
    {
        None,
        AvgPerMinute
    }

    private enum GroupMode
    {
        Power,
        Stat
    }
    #endregion

    private bool _loading = true;
    private EffectBuffType? _effectBuffType;
    private EffectGroup? _effectGroup;
    private ValueDisplayMode _valueDisplayMode = ValueDisplayMode.Raw;
    private ValueGroupMode _valueGroupMode = ValueGroupMode.None;
    private GroupMode _groupMode = GroupMode.Power;

    private const int PreLabelGap = 14;
    private const int LabelGap = 24;
    private const int GraphGap = 22;

    #region FxId class
    private class FxId : IEquatable<FxId>
    {
        public Enums.eEffectType EffectType;
        public Enums.eMez? MezType;
        public Enums.eDamage? DamageType;
        public Enums.eEffectType? ETModifies;
        public EffectBuffType BuffType;
        public MagType? MagType;
        public Enums.eAspect? Aspect;
        public Enums.eAspect[]? AllowedAspects;
        public string? Label;
        public string? ShortLabel;

        public override string ToString()
        {
            return $"<FxId>{{EffectType={EffectType}, MezType={(MezType == null ? "(null)" : MezType)}, ETModifies={(ETModifies == null ? "(null)" : ETModifies)}, BuffType={BuffType}, MagType={(MagType == null ? "(null)" : MagType)}, Aspect={(Aspect == null ? "(null)" : Aspect)}, AllowedAspects={(AllowedAspects == null ? "(null)" : string.Join(", ", AllowedAspects.Select(e => $"{e}")))}, Label={Label ?? "(null)"}, ShortLabel={ShortLabel ?? "(null)"}";
        }

        public bool Equals(FxId? other)
        {
            return EffectType == other?.EffectType && MezType == other.MezType && DamageType == other.DamageType &&
                   ETModifies == other.ETModifies && BuffType == other.BuffType && MagType == other.MagType &&
                   Aspect == other.Aspect && AllowedAspects == other.AllowedAspects;
        }

        // Get group from FxId
        // Equality comparisons only on non-null fields from both ends
        public EffectGroup GetEffectGroup(Dictionary<EffectGroup, FxId[]> buffs)
        {
            foreach (var g in buffs)
            {
                foreach (var k in g.Value)
                {
                    if (k.EffectType != EffectType)
                    {
                        continue;
                    }

                    if ((k.MezType != null) & (MezType != null) & (k.MezType != MezType))
                    {
                        continue;
                    }

                    if ((k.DamageType != null) & (DamageType != null) & (k.DamageType != DamageType))
                    {
                        continue;
                    }

                    if ((k.ETModifies != null) & (ETModifies != null) & (k.ETModifies != ETModifies))
                    {
                        continue;
                    }

                    if (k.BuffType != BuffType)
                    {
                        continue;
                    }

                    if ((k.MagType != null) & (MagType != null) & (k.MagType != MagType))
                    {
                        continue;
                    }

                    if ((k.Aspect != null) & (Aspect != null) & (k.Aspect != Aspect))
                    {
                        continue;
                    }

                    if ((k.AllowedAspects != null) & (AllowedAspects != null) & !(AllowedAspects ?? []).Intersect(k.AllowedAspects ?? []).Any())
                    {
                        continue;
                    }
                    
                    return g.Key;
                }
            }

            return EffectGroup.Misc;
        }

        // Get label from FxId
        public string GetEffectLabel(Dictionary<EffectGroup, FxId[]> buffs)
        {
            foreach (var g in buffs)
            {
                foreach (var k in g.Value)
                {
                    if (k.EffectType != EffectType)
                    {
                        continue;
                    }

                    if ((k.MezType != null) & (MezType != null) & (k.MezType != MezType))
                    {
                        continue;
                    }

                    if ((k.DamageType != null) & (DamageType != null) & (k.DamageType != DamageType))
                    {
                        continue;
                    }

                    if ((k.ETModifies != null) & (ETModifies != null) & (k.ETModifies != ETModifies))
                    {
                        continue;
                    }

                    if (k.BuffType != BuffType)
                    {
                        continue;
                    }

                    if ((k.MagType != null) & (MagType != null) & (k.MagType != MagType))
                    {
                        continue;
                    }

                    if ((k.Aspect != null) & (Aspect != null) & (k.Aspect != Aspect))
                    {
                        continue;
                    }

                    if ((k.AllowedAspects != null) & (AllowedAspects != null) & !(AllowedAspects ?? []).Intersect(k.AllowedAspects ?? []).Any())
                    {
                        continue;
                    }

                    return k.Label ?? k.EffectType switch
                    {
                        Enums.eEffectType.Mez when k.MagType == frmBuffDebuff.MagType.Positive => $"Mez ({k.MezType})",
                        Enums.eEffectType.Mez => $"Mez Protection ({k.MezType})",
                        Enums.eEffectType.Enhancement when k is { ETModifies: Enums.eEffectType.Mez, MagType: frmBuffDebuff.MagType.Positive } => $"Mez Boost ({k.MezType})",
                        Enums.eEffectType.Heal when k.MagType == frmBuffDebuff.MagType.Positive => "Heal",
                        Enums.eEffectType.Absorb when k.MagType == frmBuffDebuff.MagType.Positive => "Absorb",
                        Enums.eEffectType.HitPoints when k is { MagType: frmBuffDebuff.MagType.Positive, Aspect: Enums.eAspect.Max } => "+MaxHP",
                        Enums.eEffectType.Enhancement when k is { ETModifies: Enums.eEffectType.Accuracy, MagType: frmBuffDebuff.MagType.Positive } => "+Accuracy",
                        Enums.eEffectType.Enhancement when k.ETModifies == Enums.eEffectType.Accuracy => "-Accuracy",
                        Enums.eEffectType.ResEffect => $"{(k.MagType == frmBuffDebuff.MagType.Positive ? "+" : "-")}Res({k.ETModifies})",

                        Enums.eEffectType.Enhancement when k.MagType == frmBuffDebuff.MagType.Positive => $"+{k.EffectType} Boost",
                        Enums.eEffectType.Enhancement => $"-{k.EffectType} Debuff",
                        _ when k.MagType == frmBuffDebuff.MagType.Positive => $"+{k.EffectType}",
                        _ => $"-{k.EffectType}"
                    };
                }
            }

            return EffectType.ToString();
        }

        // Get stat unit suffix
        public string GetStatUnit() =>
            EffectType switch
            {
                Enums.eEffectType.Mez or Enums.eEffectType.Fly => "",
                Enums.eEffectType.Endurance when Aspect is Enums.eAspect.Max or Enums.eAspect.Abs => "",
                Enums.eEffectType.Heal when Aspect == Enums.eAspect.Abs => " HP",
                Enums.eEffectType.Absorb when Aspect == Enums.eAspect.Abs => " HP",
                Enums.eEffectType.HitPoints when Aspect == Enums.eAspect.Abs => " HP",
                Enums.eEffectType.Recovery => " end/s",
                _ => "%"
            };

        // Converts FxId stat to eCustomGraphStat
        public CustomGraphStat.eCustomGraphStat GetGraphStat() =>
            EffectType switch
            {
                Enums.eEffectType.Enhancement => ETModifies switch
                {
                    Enums.eEffectType.Accuracy => CustomGraphStat.eCustomGraphStat.EnhAccuracy,
                    Enums.eEffectType.Endurance => CustomGraphStat.eCustomGraphStat.EnhEndurance,
                    Enums.eEffectType.EnduranceDiscount => CustomGraphStat.eCustomGraphStat.EnhEnduranceDiscount,
                    Enums.eEffectType.SpeedFlying => CustomGraphStat.eCustomGraphStat.EnhSpeedFlying,
                    Enums.eEffectType.JumpHeight => CustomGraphStat.eCustomGraphStat.EnhJumpHeight,
                    Enums.eEffectType.SpeedJumping => CustomGraphStat.eCustomGraphStat.EnhSpeedJumping,
                    Enums.eEffectType.Mez => CustomGraphStat.eCustomGraphStat.EnhMez,
                    Enums.eEffectType.PerceptionRadius => CustomGraphStat.eCustomGraphStat.EnhPerceptionRadius,
                    Enums.eEffectType.SpeedRunning => CustomGraphStat.eCustomGraphStat.EnhSpeedRunning,
                    Enums.eEffectType.ToHit => CustomGraphStat.eCustomGraphStat.EnhToHit,
                    Enums.eEffectType.Absorb => CustomGraphStat.eCustomGraphStat.EnhAbsorb,
                    Enums.eEffectType.RechargeTime => CustomGraphStat.eCustomGraphStat.Recharge,
                    Enums.eEffectType.Range => CustomGraphStat.eCustomGraphStat.Range
                },

                Enums.eEffectType.Defense => CustomGraphStat.eCustomGraphStat.Defense,
                Enums.eEffectType.Resistance => CustomGraphStat.eCustomGraphStat.Resistance,
                Enums.eEffectType.Regeneration => CustomGraphStat.eCustomGraphStat.Regeneration,
                Enums.eEffectType.HitPoints => CustomGraphStat.eCustomGraphStat.MaxHP,
                Enums.eEffectType.Heal => CustomGraphStat.eCustomGraphStat.Heal,
                Enums.eEffectType.Absorb => CustomGraphStat.eCustomGraphStat.Absorb,
                Enums.eEffectType.Endurance => CustomGraphStat.eCustomGraphStat.MaxEnd,
                Enums.eEffectType.SpeedRunning => CustomGraphStat.eCustomGraphStat.SpeedRunning,
                Enums.eEffectType.SpeedJumping => CustomGraphStat.eCustomGraphStat.SpeedJumping,
                Enums.eEffectType.JumpHeight => CustomGraphStat.eCustomGraphStat.JumpHeight,
                Enums.eEffectType.SpeedFlying => CustomGraphStat.eCustomGraphStat.SpeedFlying,
                Enums.eEffectType.PerceptionRadius => CustomGraphStat.eCustomGraphStat.PerceptionRadius,
                Enums.eEffectType.ToHit => CustomGraphStat.eCustomGraphStat.ToHit,
                Enums.eEffectType.Accuracy => CustomGraphStat.eCustomGraphStat.Accuracy,
                Enums.eEffectType.DamageBuff => CustomGraphStat.eCustomGraphStat.Damage,
                Enums.eEffectType.EnduranceDiscount => CustomGraphStat.eCustomGraphStat.EndRdx,
                Enums.eEffectType.Mez => CustomGraphStat.eCustomGraphStat.StatusProtection,
                Enums.eEffectType.MezResist => CustomGraphStat.eCustomGraphStat.StatusResistance,
                Enums.eEffectType.ResEffect => CustomGraphStat.eCustomGraphStat.DebuffResistance,
                Enums.eEffectType.Elusivity => CustomGraphStat.eCustomGraphStat.Elusivity,

                Enums.eEffectType.Fly => CustomGraphStat.eCustomGraphStat.Fly,
                Enums.eEffectType.MaxRunSpeed => CustomGraphStat.eCustomGraphStat.MaxRunSpeed,
                Enums.eEffectType.Recovery => CustomGraphStat.eCustomGraphStat.Recovery,

                _ => CustomGraphStat.eCustomGraphStat.None // Neutral key
            };

        // Create FxId from IEffect
        public static FxId CreateFxIdFromEffect(IEffect fx, Dictionary<EffectGroup, FxId[]> buffs)
        {
            var template = new FxId
            {
                EffectType = fx.EffectType,
                MezType = fx.MezType == Enums.eMez.None ? null : fx.MezType,
                DamageType = null, // None for everything
                ETModifies = fx.ETModifies == Enums.eEffectType.None ? null : fx.ETModifies,
                Aspect = fx.Aspect,
                BuffType = GetBuffType(fx, buffs),
                MagType = fx.BuffedMag >= 0 ? frmBuffDebuff.MagType.Positive : frmBuffDebuff.MagType.Negative
            };

            template.Label = template.GetEffectLabel(buffs);

            return template;
        }

        // Get Buff/Debuff type from IEffect
        private static EffectBuffType GetBuffType(IEffect fx, Dictionary<EffectGroup, FxId[]> buffs) =>
            (from k in buffs
             from id in k.Value
             let effectType = fx.EffectType
             let mezType = (Enums.eMez?)(fx.MezType == Enums.eMez.None ? null : fx.MezType)
             let etModifies = (Enums.eEffectType?)(fx.ETModifies == Enums.eEffectType.None ? null : fx.ETModifies)
             let aspect = (Enums.eAspect?)(fx.Aspect == Enums.eAspect.Max ? fx.Aspect : null)
             let magType = fx.BuffedMag >= 0 ? frmBuffDebuff.MagType.Positive : frmBuffDebuff.MagType.Negative
             where id.EffectType == effectType && id.MezType == mezType && id.ETModifies == etModifies &&
                   (id.Aspect == aspect) | (id.AllowedAspects ?? []).Contains(fx.Aspect) && id.MagType == magType
             select id.BuffType).FirstOrDefault();
    }
    #endregion

    #region Included buffs/debuffs definitions

    // Mitigation: +Def, +Res, -ToHit, -Dmg, -End, -Rech, Mez, Enhancement(Mez), Mez Protection
    private readonly Dictionary<EffectGroup, FxId[]> Buffs = new()
    {
        { EffectGroup.Mitigation, [
                new()
                {
                    EffectType = Enums.eEffectType.Defense,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.Resistance,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.ToHit,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.DamageBuff,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff,
                    Label = "Damage Buff",
                    ShortLabel = "DmgBuff"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Endurance,
                    MagType = MagType.Negative,
                    AllowedAspects = [Enums.eAspect.Str, Enums.eAspect.Abs],
                    BuffType = EffectBuffType.Debuff,
                    Label = "Endurance",
                    ShortLabel = "End"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Endurance,
                    MagType = MagType.Negative,
                    Aspect = Enums.eAspect.Max,
                    BuffType = EffectBuffType.Debuff,
                    Label = "Max Endurance",
                    ShortLabel = "Max End"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.RechargeTime,
                    MagType = MagType.Negative
                },
                new()
                {
                    EffectType = Enums.eEffectType.Mez,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Mez",
                    ShortLabel = "Mez"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Mez,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Buff,
                    Label = "Mez Protection",
                    ShortLabel = "Mez Prot."
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.Mez,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Mez Boost",
                    ShortLabel = "Mez Boost"
                }
            ]
        },
        // Sustain: Heal, Absorb, +Regen, +MaxHP, +End, +Recovery, +EndDiscount
        { EffectGroup.Sustain, [
                new()
                {
                    EffectType = Enums.eEffectType.Heal,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Heal",
                    ShortLabel = "Heal"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Absorb,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Absorb",
                    ShortLabel = "Absorb"

                },
                new()
                {
                    EffectType = Enums.eEffectType.Regeneration,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.HitPoints,
                    MagType = MagType.Positive,
                    Aspect = Enums.eAspect.Max,
                    BuffType = EffectBuffType.Buff,
                    Label = "+MaxHP",
                    ShortLabel = "+MaxHP"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Endurance,
                    MagType = MagType.Positive,
                    AllowedAspects = [Enums.eAspect.Str, Enums.eAspect.Abs],
                    BuffType = EffectBuffType.Buff,
                    Label = "Endurance",
                    ShortLabel = "End"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Endurance,
                    MagType = MagType.Positive,
                    Aspect = Enums.eAspect.Max,
                    BuffType = EffectBuffType.Buff,
                    Label = "Max Endurance",
                    ShortLabel = "Max End"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Recovery,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.EnduranceDiscount,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                }
            ]
        },
        // Dps (team + self): -Res, +Dmg, +Rech, -Regen, +ToHit
        { EffectGroup.Dps, [
                new()
                {
                    EffectType = Enums.eEffectType.Resistance,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.DamageBuff,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Damage Buff",
                    ShortLabel = "DmgBuff"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.RechargeTime,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.Regeneration,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.ToHit,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                }
            ]
        },
        // Misc: Slow, -Jump, -Fly, Mez Resistance, +Acc, -Acc, +Movement, +MaxMovement, -Recovery, Heal Resistance, -Heal, +Range
        { EffectGroup.Misc, [
                new()
                {
                    EffectType = Enums.eEffectType.SpeedRunning,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.SpeedJumping,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.SpeedFlying,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.JumpHeight,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.Fly,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.MezResist,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.Accuracy,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Accuracy",
                    ShortLabel = "Acc"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.Accuracy,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff,
                    Label = "Accuracy",
                    ShortLabel = "Acc"
                },
                new()
                {
                    EffectType = Enums.eEffectType.SpeedRunning,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.SpeedJumping,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.SpeedFlying,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.MaxRunSpeed,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff
                },
                new()
                {
                    EffectType = Enums.eEffectType.Recovery,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff
                },
                new()
                {
                    EffectType = Enums.eEffectType.ResEffect,
                    ETModifies = Enums.eEffectType.Heal,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Buff,
                    Label = "Heal Increase",
                    ShortLabel = "Heal Boost"
                },
                new()
                {
                    EffectType = Enums.eEffectType.ResEffect,
                    ETModifies = Enums.eEffectType.Absorb,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Buff,
                    Label = "Absorb Increase",
                    ShortLabel = "Abs. boost"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.Range,
                    MagType = MagType.Positive,
                    BuffType = EffectBuffType.Buff,
                    Label = "Range",
                    ShortLabel = "Range"
                },
                new()
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    ETModifies = Enums.eEffectType.Range,
                    MagType = MagType.Negative,
                    BuffType = EffectBuffType.Debuff,
                    Label = "Range",
                    ShortLabel = "Range"
                }
            ]
        }
    };

    #endregion

    public frmBuffDebuff()
    {
        InitializeComponent();
        Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
    }

    private void frmBuffDebuff_Load(object sender, EventArgs e)
    {
        cbBuffType.SelectedIndex = 0;
        cbGroup.SelectedIndex = 0;
        cbValueDisplayType.SelectedIndex = 0;
        cbValueGroupMode.SelectedIndex = 0;
        cbValueGroupMode2.SelectedIndex = 0;

        UpdateData();

        _loading = false;
    }

    // Performs layout updates for effects data (Multiple Label + CtlMultiGraph)
    public void UpdateData()
    {
        var ctlList = GetValues(_effectBuffType, _effectGroup, _valueDisplayMode, _valueGroupMode, _groupMode);
        PowerEffectsPanel.SuspendLayout();
        PowerEffectsPanel.Controls.Clear();
        foreach (var s in ctlList)
        {
            PowerEffectsPanel.Controls.AddRange(s.ToArray());
        }
        PowerEffectsPanel.ResumeLayout(true);
        foreach (var g in PowerEffectsPanel.Controls)
        {
            if (g is CtlMultiGraph graph)
            {
                graph.Draw();
            }
        }
    }

    // Get enhanced powers from build
    private static IPower[] GetEnhancedPowers()
    {
        return MidsContext.Character?.CurrentBuild == null
            ? []
            : MidsContext.Character.CurrentBuild.Powers
                .Select((e, i) => new KeyValuePair<int, PowerEntry?>(i, e))
                .Where(e => e.Value?.Power is { Slottable: true })
                .Select(e => MainModule.MidsController.Toon?.GetEnhancedPower(e.Key).Clone())
                .Where(e => e != null)
                .Cast<IPower>()
                .ToArray();
    }

    private string GetGreLabel(KeyValuePair<FxId, GroupedFx> gre, IPower pw)
    {
        var greTip = gre.Value.GetTooltip(pw, true);

        return gre.Key.EffectType switch
        {
            Enums.eEffectType.Enhancement when greTip.Contains(" slow", StringComparison.InvariantCultureIgnoreCase) => "Enhancement(Slow)",
            Enums.eEffectType.Enhancement => gre.Key.ETModifies switch
            {
                Enums.eEffectType.Mez when gre.Key.MagType == MagType.Positive => $"{gre.Key.MezType} Boost",
                Enums.eEffectType.Mez when gre.Key.MagType == MagType.Negative => $"{gre.Key.MezType} Dampen",
                _ => ""
            },
            Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedFlying when greTip.Contains(" slow", StringComparison.InvariantCultureIgnoreCase) => "Slow",
            Enums.eEffectType.Mez => $"{gre.Key.MezType}{(gre.Key.MagType == MagType.Negative ? " Protection" : "")}",
            Enums.eEffectType.MezResist => $"{gre.Key.MezType} Resistance",
            Enums.eEffectType.ResEffect => $"{gre.Key.ETModifies} Resistance",
            Enums.eEffectType.Endurance => gre.Key.Label ?? "",
            _ => ""
        };
    }

    private string GetGreLabelShort(KeyValuePair<FxId, GroupedFx> gre, IPower pw)
    {
        var greTip = gre.Value.GetTooltip(pw, true);

        return gre.Key.EffectType switch
        {
            Enums.eEffectType.Enhancement when greTip.Contains(" slow", StringComparison.InvariantCultureIgnoreCase) => "Enh(Slow)",
            Enums.eEffectType.Enhancement => gre.Key.ETModifies switch
            {
                Enums.eEffectType.Mez when gre.Key.MagType == MagType.Positive => $"{gre.Key.MezType} Boost",
                Enums.eEffectType.Mez when gre.Key.MagType == MagType.Negative => $"{gre.Key.MezType} Dampen",
                _ => ""
            },
            Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping or Enums.eEffectType.SpeedFlying when greTip.Contains(" slow", StringComparison.InvariantCultureIgnoreCase) => "Slow",
            Enums.eEffectType.Mez => $"{gre.Key.MezType}{(gre.Key.MagType == MagType.Negative ? " Prot." : "")}",
            Enums.eEffectType.MezResist => $"{gre.Key.MezType} Res",
            Enums.eEffectType.ResEffect => $"{gre.Key.ETModifies} Res",
            Enums.eEffectType.Endurance => gre.Key.ShortLabel ?? "",
            _ => ""
        };
    }

    // Generate graph and label controls from powers according to filters and view mode
    // Partially implemented
    private List<List<Control>> GetValues(EffectBuffType? buffType, EffectGroup? group, ValueDisplayMode valueDisplayMode,
        ValueGroupMode valueGroupMode, GroupMode groupMode, bool includeEnhFx = true)
    {
        var enhPowers = GetEnhancedPowers();

        // Effects by power, key 1 is index in enhPowers, key 2 is FxId for GroupedFx
        var powerEffects = enhPowers
            .Select((e, i) => new KeyValuePair<int, IPower>(i, e))
            .Select(e => new KeyValuePair<int, KeyValuePair<IPower, List<GroupedFx>>>(e.Key,
                new KeyValuePair<IPower, List<GroupedFx>>(e.Value,
                    GroupedFx.AssembleGroupedEffects(enhPowers[e.Key]))))
            .Select(e => new KeyValuePair<int, List<KeyValuePair<FxId, GroupedFx>>>(e.Key,
                e.Value.Value.Select(f =>
                        new KeyValuePair<FxId, GroupedFx>(FxId.CreateFxIdFromEffect(f.GetEffectAt(enhPowers[e.Key]), Buffs), f))
                    .Where(g => g.Key.GetGraphStat() != CustomGraphStat.eCustomGraphStat.None)
                    .Where(g => buffType == null || g.Key.BuffType == buffType)
                    .Where(g => group == null || g.Key.GetEffectGroup(Buffs) == group)
                    .Where(g => includeEnhFx | !g.Value.EnhancementEffect)
                    .ToList()))
            .ToList();

        // Unique FxId keys
        var fxIdList = powerEffects.SelectMany(e => e.Value.Select(f => f.Key))
            .Distinct()
            .ToArray();

        // Regroup by stat type
        var statEffects = fxIdList
            .ToDictionary(e => e, e => powerEffects.Select(f =>
                new KeyValuePair<int, List<KeyValuePair<FxId, GroupedFx>>>(f.Key,
                    f.Value.Where(g => g.Key.Equals(e)).ToList())).ToList());

        // Max by stat (absolute values)
        var statMax = new Dictionary<FxId, float>();
        foreach (var s in statEffects)
        {
            var items = s.Value.SelectMany(e => e.Value.Select(f =>
                    f.Value.GetEffectAt(enhPowers[e.Key]).BuffedMag * (f.Key.GetStatUnit().Contains('%') ? 100f : 1f)))
                .ToList();

            // Consider very large values as outliers and ignore them,
            // if smaller values are present
            if (items.Any(e => e < CustomGraphStat.Scales[^1]))
            {
                items = items
                    .Where(e => e < CustomGraphStat.Scales[^1])
                    .ToList();
            }

            if (items.Count > 0)
            {
                statMax.Add(s.Key, items.Max());
            }
            else
            {
                statMax.Add(s.Key, CustomGraphStat.Scales[^1]);
            }
        }

        // Max/scale by stat
        // var statScales = statMax.ToDictionary(e => e.Key, e => CustomGraphStat.FindScale(e.Value));

        var ret = new List<List<Control>>();
        var labelIndex = 1;
        var graphIndex = 1;
        var y = 4;

        switch (groupMode)
        {
            case GroupMode.Power:
                foreach (var p in powerEffects)
                {
                    if (labelIndex > 1)
                    {
                        y += PreLabelGap;
                    }

                    var lst = new List<Control>();
                    var lbl = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                        ForeColor = Color.Goldenrod,
                        Location = new Point(16, y),
                        Name = $"label{labelIndex}",
                        Text = @$"{enhPowers[p.Key].DisplayName}"
                    };

                    lst.Add(lbl);

                    labelIndex++;
                    y += LabelGap;

                    p.Value.Sort((a, b) => string.Compare(GetGreLabel(a, enhPowers[p.Key]), GetGreLabel(b, enhPowers[p.Key]), StringComparison.InvariantCultureIgnoreCase));
                    foreach (var gre in p.Value)
                    {
                        var stat = gre.Key.GetGraphStat();
                        var fxRef = gre.Value.GetEffectAt(enhPowers[p.Key]);
                        var toWho = fxRef.ToWho switch
                        {
                            Enums.eToWho.Self or Enums.eToWho.Target => $"{fxRef.ToWho}",
                            _ => ""
                        };

                        var graph = CustomGraphStat.GenerateGraph(stat, CustomGraphStat.eCustomGraphMode.Single, false, $"graph{graphIndex}");
                        graph.Location = new Point(4, y);
                        graph.Size = new Size(450, 20);

                        var vMax = statMax[gre.Key];
                        var unit = gre.Key.GetStatUnit();
                        var multiplier = unit.Contains('%') ? 100f : 1f;
                        var val = valueDisplayMode switch
                        {
                            ValueDisplayMode.Duration => fxRef.Duration,
                            ValueDisplayMode.ValuePerEnd => fxRef.BuffedMag / (enhPowers[p.Key].EndCost < float.Epsilon ? 1 : enhPowers[p.Key].EndCost),
                            ValueDisplayMode.ValuePerRecharge => fxRef.BuffedMag / (enhPowers[p.Key].RechargeTime < float.Epsilon ? 1 : enhPowers[p.Key].RechargeTime),
                            ValueDisplayMode.ValuePerRechargeEnd => fxRef.BuffedMag / (enhPowers[p.Key].EndCost < float.Epsilon ? 1 : enhPowers[p.Key].EndCost) / (enhPowers[p.Key].RechargeTime < float.Epsilon ? 1 : enhPowers[p.Key].RechargeTime),
                            _ => fxRef.BuffedMag * multiplier
                        };

                        var endCost = enhPowers[p.Key].PowerType == Enums.ePowerType.Toggle
                            ? enhPowers[p.Key].ActivatePeriod <= float.Epsilon
                                ? enhPowers[p.Key].EndCost
                                : enhPowers[p.Key].EndCost / enhPowers[p.Key].ActivatePeriod
                            : enhPowers[p.Key].EndCost;

                        var label = GetGreLabel(gre, enhPowers[p.Key]);
                        var shortLabel = GetGreLabelShort(gre, enhPowers[p.Key]);

                        if (gre.Key.EffectType == Enums.eEffectType.Mez)
                        {
                            val = Math.Abs(val);
                        }

                        graph.SetGraphItemManual(stat, CustomGraphStat.eCustomGraphMode.Single, valueDisplayMode, val,
                            fxRef.Duration, fxRef.isEnhancementEffect, enhPowers[p.Key].RechargeTime, endCost, enhPowers[p.Key].DisplayName,
                            enhPowers[p.Key].PowerType == Enums.ePowerType.Toggle, toWho, vMax, gre.Key.GetStatUnit(),
                            label, shortLabel);

                        lst.Add(graph);

                        graphIndex++;
                        y += GraphGap;
                    }

                    ret.Add(lst);
                }
                break;

            case GroupMode.Stat:
                foreach (var s in statEffects)
                {
                    if (labelIndex > 1)
                    {
                        y += PreLabelGap;
                    }

                    var lst = new List<Control>();
                    var lbl = new Label
                    {
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                        ForeColor = Color.Goldenrod,
                        Location = new Point(16, y),
                        Name = $"label{labelIndex}",
                        Text = @$"{s.Key.Label}"
                    };

                    lst.Add(lbl);

                    labelIndex++;
                    y += LabelGap;

                    foreach (var g in s.Value)
                    {
                        g.Value.Sort((a, b) => string.Compare(GetGreLabel(a, enhPowers[g.Key]), GetGreLabel(b, enhPowers[g.Key]), StringComparison.InvariantCultureIgnoreCase));
                        foreach (var gre in g.Value)
                        {
                            var stat = gre.Key.GetGraphStat();
                            var fxRef = gre.Value.GetEffectAt(enhPowers[g.Key]);
                            var toWho = fxRef.ToWho switch
                            {
                                Enums.eToWho.Self or Enums.eToWho.Target => $"{fxRef.ToWho}",
                                _ => ""
                            };

                            var graph = CustomGraphStat.GenerateGraph(stat, CustomGraphStat.eCustomGraphMode.Single, false);
                            graph.Location = new Point(4, y);
                            graph.Size = new Size(450, 20);

                            var vMax = statMax[gre.Key];
                            var unit = gre.Key.GetStatUnit();
                            var multiplier = unit.Contains('%') ? 100f : 1f;
                            var val = valueDisplayMode switch
                            {
                                ValueDisplayMode.Duration => fxRef.Duration,
                                ValueDisplayMode.ValuePerEnd => fxRef.BuffedMag / (enhPowers[g.Key].EndCost < float.Epsilon ? 1 : enhPowers[g.Key].EndCost),
                                ValueDisplayMode.ValuePerRecharge => fxRef.BuffedMag / (enhPowers[g.Key].RechargeTime < float.Epsilon ? 1 : enhPowers[g.Key].RechargeTime),
                                ValueDisplayMode.ValuePerRechargeEnd => fxRef.BuffedMag / (enhPowers[g.Key].EndCost < float.Epsilon ? 1 : enhPowers[g.Key].EndCost) / (enhPowers[g.Key].RechargeTime < float.Epsilon ? 1 : enhPowers[g.Key].RechargeTime),
                                _ => fxRef.BuffedMag * multiplier
                            };

                            var endCost = enhPowers[g.Key].PowerType == Enums.ePowerType.Toggle
                                ? enhPowers[g.Key].ActivatePeriod <= float.Epsilon
                                    ? enhPowers[g.Key].EndCost
                                    : enhPowers[g.Key].EndCost / enhPowers[g.Key].ActivatePeriod
                                : enhPowers[g.Key].EndCost;

                            var label = GetGreLabel(gre, enhPowers[g.Key]);
                            var shortLabel = GetGreLabelShort(gre, enhPowers[g.Key]);

                            if (gre.Key.EffectType == Enums.eEffectType.Mez)
                            {
                                val = Math.Abs(val);
                            }

                            graph.SetGraphItemManual(stat, CustomGraphStat.eCustomGraphMode.Single, valueDisplayMode,
                                val, fxRef.Duration, fxRef.isEnhancementEffect, enhPowers[g.Key].RechargeTime, endCost,
                                enhPowers[g.Key].DisplayName, enhPowers[g.Key].PowerType == Enums.ePowerType.Toggle,
                                toWho, vMax, gre.Key.GetStatUnit(), label, shortLabel);

                            lst.Add(graph);

                            graphIndex++;
                            y += GraphGap;
                        }
                    }

                    ret.Add(lst);
                }

                break;
        }

        return ret;
    }



    public void UpdateColorTheme(Enums.Alignment e)
    {
        BtnClose.UseAlt = e is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
    }

    private void cbBuffType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        _effectBuffType = cbBuffType.SelectedIndex switch
        {
            1 => EffectBuffType.Buff,
            2 => EffectBuffType.Debuff,
            _ => null
        };

        UpdateData();
    }

    private void cbGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        _effectGroup = cbGroup.SelectedIndex switch
        {
            1 => EffectGroup.Mitigation,
            2 => EffectGroup.Sustain,
            3 => EffectGroup.Dps,
            4 => EffectGroup.Misc,
            _ => null
        };

        UpdateData();
    }

    private void cbValueDisplayType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        _valueDisplayMode = cbValueDisplayType.SelectedIndex switch
        {
            1 => ValueDisplayMode.Duration,
            2 => ValueDisplayMode.ValuePerEnd,
            3 => ValueDisplayMode.ValuePerRecharge,
            4 => ValueDisplayMode.ValuePerRechargeEnd,
            _ => ValueDisplayMode.Raw
        };

        UpdateData();
    }

    private void cbValueGroupMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        _valueGroupMode = cbValueGroupMode.SelectedIndex switch
        {
            1 => ValueGroupMode.AvgPerMinute,
            _ => ValueGroupMode.None
        };

        UpdateData();
    }

    private void cbValueGroupMode2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loading)
        {
            return;
        }

        _groupMode = cbValueGroupMode2.SelectedIndex switch
        {
            1 => GroupMode.Stat,
            _ => GroupMode.Power
        };
    }

    private void BtnClose_Click(object sender, EventArgs e)
    {
        Close();
    }
}
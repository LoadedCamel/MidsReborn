using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DataView2 : UserControl
    {
        #region Private enums & structs
        private enum BoostType
        {
            Reduction,
            Equal,
            Enhancement,
            Extra
        }

        private enum BuffEffectType
        {
            Buff,
            Debuff,
            NonZero,
            Any
        }

        private struct ColorRange
        {
            public Color LowerBoundColor;
            public Color UpperBoundColor;
        }

        private struct TrackGradientsScheme
        {
            public ColorRange ElapsedInnerColor;
            public ColorRange ElapsedPenColorBottom;
            public ColorRange ElapsedPenColorTop;
        }
        #endregion

        private IPower _basePower;
        private IPower _enhancedPower;
        private int HistoryIdx = -1;
        private bool NoLevel;
        private PowerEntry BuildPowerEntry;
        private bool FreezeScalerCB;

        // Track bar colors for power scalers
        private readonly TrackGradientsScheme TrackColors = new()
        {
            ElapsedInnerColor = new ColorRange { LowerBoundColor = Color.FromArgb(0, 51, 0), UpperBoundColor = Color.FromArgb(0, 128, 0) },
            ElapsedPenColorBottom = new ColorRange { LowerBoundColor = Color.FromArgb(58, 94, 58), UpperBoundColor = Color.FromArgb(144, 238, 44) },
            ElapsedPenColorTop = new ColorRange { LowerBoundColor = Color.FromArgb(0, 102, 51), UpperBoundColor = Color.FromArgb(0, 255, 127) }
        };

        public bool Locked;

        // Group labels (effects tab)
        private static readonly List<string> GroupLabels = new() { "Resistance", "Defense", "Buffs", "Debuffs", "Summons/Grants", "Misc." };

        #region Effect vector type sub-class
        private class EffectVectorType
        {
            public Enums.eEffectType? EffectType;
            public Enums.eMez? MezType;
            public Enums.eDamage? DamageType;
            public Enums.eEffectType? ETModifies;
            public BuffEffectType VectorDirection;
            public Enums.eToWho ToWho;

            public EffectVectorType(Enums.eEffectType effectType, BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public EffectVectorType(Enums.eEffectType effectType, Enums.eMez mezType, BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                MezType = mezType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public EffectVectorType(Enums.eEffectType effectType, Enums.eDamage damageType, BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                DamageType = damageType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }
            
            public EffectVectorType(Enums.eEffectType effectType, Enums.eEffectType etModifies, BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                ETModifies = etModifies;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public bool Validate(IEffect effect)
            {
                return (EffectType == null || EffectType == effect.EffectType) &&
                       (MezType == null || MezType == effect.MezType) &&
                       (DamageType == null || DamageType == effect.DamageType) &&
                       (ETModifies == null || ETModifies == effect.ETModifies) &&
                       (ToWho == Enums.eToWho.All || ToWho == effect.ToWho) &&
                       VectorDirection switch
                       {
                           BuffEffectType.NonZero => Math.Abs(effect.BuffedMag) > float.Epsilon,
                           BuffEffectType.Buff => effect.BuffedMag > 0,
                           BuffEffectType.Debuff => effect.BuffedMag < 0,
                           _ => true
                       };
            }
        }
        #endregion

        #region Effect vector groups (effect tab)
        private static readonly List<List<EffectVectorType>> EffectVectorsGroups = new()
        {
            // +Defenses to Self
            new List<EffectVectorType>
            {
                new(Enums.eEffectType.Resistance, BuffEffectType.Buff, Enums.eToWho.Self)
            },

            // +Resistances to Self
            new List<EffectVectorType>
            {
                new(Enums.eEffectType.Defense, BuffEffectType.Buff, Enums.eToWho.Self)
            },

            // Buffs
            new List<EffectVectorType>
            {
                // Need to add any other effect here ?
                new(Enums.eEffectType.Resistance, BuffEffectType.Buff, Enums.eToWho.Target),
                new(Enums.eEffectType.Defense, BuffEffectType.Buff, Enums.eToWho.Target),
                new(Enums.eEffectType.Recovery, BuffEffectType.Buff),
                new(Enums.eEffectType.Regeneration, BuffEffectType.Buff),
                new(Enums.eEffectType.Accuracy, BuffEffectType.Buff),
                new(Enums.eEffectType.ToHit, BuffEffectType.Buff),
                new(Enums.eEffectType.DamageBuff, BuffEffectType.Buff),
                new(Enums.eEffectType.Enhancement, Enums.eEffectType.RechargeTime, BuffEffectType.Buff),
                new(Enums.eEffectType.StealthRadius, BuffEffectType.Buff),
                new(Enums.eEffectType.StealthRadiusPlayer, BuffEffectType.Buff),
                new(Enums.eEffectType.HitPoints, BuffEffectType.Buff),
                new(Enums.eEffectType.Elusivity, BuffEffectType.Buff)

            },

            // Debuffs
            new List<EffectVectorType>
            {
                // Need to add any other effect here ?
                new(Enums.eEffectType.Resistance, BuffEffectType.Debuff),
                new(Enums.eEffectType.Defense, BuffEffectType.Debuff),
                new(Enums.eEffectType.Recovery, BuffEffectType.Debuff),
                new(Enums.eEffectType.Regeneration, BuffEffectType.Debuff),
                new(Enums.eEffectType.Accuracy, BuffEffectType.Debuff),
                new(Enums.eEffectType.ToHit, BuffEffectType.Debuff),
                new(Enums.eEffectType.DamageBuff, BuffEffectType.Debuff),
                new(Enums.eEffectType.Enhancement, Enums.eEffectType.RechargeTime, BuffEffectType.Debuff),
                new(Enums.eEffectType.HitPoints, BuffEffectType.Debuff)
            },

            // Summons & GrantPower
            new List<EffectVectorType>
            {
                new(Enums.eEffectType.EntCreate, BuffEffectType.Any),
                new(Enums.eEffectType.GrantPower, BuffEffectType.Any)
            },

            // Misc
            new List<EffectVectorType>
            {
                // Heal, Endurance, Movement, MaxMovement, JumpPack, Fly, OnlyAffectSelf, MezResist, Mez, ThreatLevel
                new(Enums.eEffectType.Heal),
                new(Enums.eEffectType.Enhancement, Enums.eEffectType.Heal),
                new(Enums.eEffectType.Absorb),
                new(Enums.eEffectType.Endurance),
                new(Enums.eEffectType.SpeedJumping),
                new(Enums.eEffectType.SpeedFlying),
                new(Enums.eEffectType.SpeedRunning),
                new(Enums.eEffectType.MaxRunSpeed),
                new(Enums.eEffectType.MaxFlySpeed),
                new(Enums.eEffectType.MaxJumpSpeed),
                new(Enums.eEffectType.Jumppack),
                new(Enums.eEffectType.Fly),
                new(Enums.eEffectType.Mez, Enums.eMez.CombatPhase), // ???
                new(Enums.eEffectType.Mez, Enums.eMez.Confused),
                new(Enums.eEffectType.Mez, Enums.eMez.Held),
                new(Enums.eEffectType.Mez, Enums.eMez.Immobilized),
                new(Enums.eEffectType.Mez, Enums.eMez.Intangible),
                new(Enums.eEffectType.Mez, Enums.eMez.Knockback),
                new(Enums.eEffectType.Mez, Enums.eMez.Knockup),
                new(Enums.eEffectType.Mez, Enums.eMez.OnlyAffectsSelf),
                new(Enums.eEffectType.Mez, Enums.eMez.Placate),
                new(Enums.eEffectType.Mez, Enums.eMez.Repel),
                new(Enums.eEffectType.Mez, Enums.eMez.Sleep),
                new(Enums.eEffectType.Mez, Enums.eMez.Stunned),
                new(Enums.eEffectType.Mez, Enums.eMez.Taunt),
                new(Enums.eEffectType.Mez, Enums.eMez.Terrorized),
                new(Enums.eEffectType.Mez, Enums.eMez.Untouchable),
                new(Enums.eEffectType.MezResist),
                new(Enums.eEffectType.Enhancement, Enums.eEffectType.Mez),
                new(Enums.eEffectType.Enhancement, Enums.eEffectType.MezResist)
            }
        };
        #endregion

        #region Grouped effect sub-class
        private class GroupedEffect
        {
            private List<Enums.eDamage> DamageTypes = new();
            private List<Enums.eMez> MezTypes = new();
            private List<Enums.eEffectType> ETModifiesTypes = new();
            private readonly Enums.eEffectType EffectType;
            private readonly Enums.eEffectType SubEffectType;
            private readonly float BuffedMag;
            private readonly Enums.eToWho ToWho;
            private readonly bool DisplayPercentage;

            public GroupedEffect(Enums.eEffectType effectType, float buffedMag, bool displayPercentage, Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = Enums.eEffectType.None;
                BuffedMag = buffedMag;
                DisplayPercentage = displayPercentage;
                ToWho = toWho;
            }

            public GroupedEffect(Enums.eEffectType effectType, Enums.eEffectType subEffectType, float buffedMag, bool displayPercentage, Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = subEffectType;
                BuffedMag = buffedMag;
                DisplayPercentage = displayPercentage;
                ToWho = toWho;
            }

            public void AddDamageType(Enums.eDamage vector)
            {
                DamageTypes.Add(vector);
            }

            public void AddMezType(Enums.eMez vector)
            {
                MezTypes.Add(vector);
            }

            public void AddETModifyType(Enums.eEffectType vector)
            {
                ETModifiesTypes.Add(vector);
            }

            public void SetVectors(List<Enums.eDamage> vectors)
            {
                DamageTypes = vectors;
            }

            public void SetVectors(List<Enums.eMez> vectors)
            {
                MezTypes = vectors;
            }

            public void SetVectors(List<Enums.eEffectType> vectors)
            {
                ETModifiesTypes = vectors;
            }

            private bool ContainsMulti<T>(IEnumerable<T> items, ICollection<T> baseList)
            {
                return items.All(baseList.Contains);
            }

            private bool ContainsMultiOnly<T>(IEnumerable<T> items, IEnumerable<T> baseList)
            {
                // return new HashSet<T>(items).SetEquals(baseList);
                return baseList.Except(items).Any();
            }

            private string MezTypesString()
            {
                return ContainsMultiOnly(
                    new List<Enums.eMez>
                    {
                        Enums.eMez.Immobilized,
                        Enums.eMez.Held,
                        Enums.eMez.Stunned,
                        Enums.eMez.Sleep,
                        Enums.eMez.Terrorized,
                        Enums.eMez.Confused
                    }, MezTypes)
                    ? "All"
                    : string.Join(", ", MezTypes);
            }

            private string DamageTypesString()
            {
                var damageTypes = new List<Enums.eDamage>
                {
                    Enums.eDamage.Smashing,
                    Enums.eDamage.Lethal,
                    Enums.eDamage.Fire,
                    Enums.eDamage.Cold,
                    Enums.eDamage.Energy,
                    Enums.eDamage.Negative,
                    Enums.eDamage.Psionic,
                    Enums.eDamage.Toxic,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE
                };

                switch (EffectType)
                {
                    case Enums.eEffectType.Enhancement when SubEffectType == Enums.eEffectType.Defense:
                    case Enums.eEffectType.Defense:
                        if (ContainsMultiOnly(damageTypes.GetRange(0, 7), DamageTypes))
                        {
                            return "All Dmg";
                        }
                        else if (ContainsMultiOnly(damageTypes.GetRange(8, 3), DamageTypes))
                        {
                            return "All Pos";
                        }
                        else if (ContainsMultiOnly(damageTypes.GetRange(0, 7).Union(damageTypes.GetRange(8, 3)), DamageTypes))
                        {
                            return "All";
                        }
                        else
                        {
                            return string.Join(", ", DamageTypes);
                        }

                    default:
                        return ContainsMultiOnly(damageTypes.GetRange(0, 8), DamageTypes)
                            ? "All"
                            : string.Join(", ", DamageTypes);
                }
            }

            private string ETModifiesString()
            {
                return string.Join(", ", ETModifiesTypes);
            }

            private string ToWhoString(bool addSpace = true)
            {
                return ToWho switch
                {
                    Enums.eToWho.Self => $"{(addSpace ? " " : "")}(Self)",
                    Enums.eToWho.Target => $"{(addSpace ? " " : "")}(Tgt)",
                    _ => ""
                };
            }

            public BoostType GetBoostType()
            {
                return BuffedMag switch
                {
                    > 0 => BoostType.Enhancement,
                    < 0 => BoostType.Reduction,
                    _ => BoostType.Equal
                };
            }

            public string GetStatName()
            {
                if (EffectType == Enums.eEffectType.Enhancement)
                {
                    switch (SubEffectType)
                    {
                        case Enums.eEffectType.Mez:
                        case Enums.eEffectType.MezResist:
                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Elusivity:
                            return $"{EffectType} to {SubEffectType}";

                        default:
                            return $"{EffectType} to {ETModifiesString()}";
                    }
                }

                switch (EffectType)
                {
                    case Enums.eEffectType.Mez:
                    case Enums.eEffectType.MezResist:
                        return $"{EffectType}({MezTypesString()}";

                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                        return $"{EffectType}({DamageTypesString()})";

                    default:
                        return $"{EffectType}";
                }
            }

            public string GetMagString()
            {
                return EffectType == Enums.eEffectType.Enhancement
                    ? $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")}"
                    : $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")}";
            }

            public override string ToString()
            {
                if (EffectType == Enums.eEffectType.Enhancement)
                {
                    switch (SubEffectType)
                    {
                        case Enums.eEffectType.Mez:
                        case Enums.eEffectType.MezResist:
                            return $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {SubEffectType}({MezTypesString()}){ToWhoString()}";

                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Elusivity:
                            return $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {SubEffectType}({DamageTypesString()}){ToWhoString()}";

                        default:
                            return $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {ETModifiesString()}{ToWhoString()}";
                    }
                }

                switch (EffectType)
                {
                    case Enums.eEffectType.Mez:
                    case Enums.eEffectType.MezResist:
                        return $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}({MezTypesString()}){ToWhoString()}";

                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                        return $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}({DamageTypesString()}){ToWhoString()}";

                    default:
                        return $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}{ToWhoString()}";
                }
            }
        }
        #endregion

        #region Effect group filter sub-class
        private class EffectsGroupFilter
        {
            private Dictionary<string, List<GroupedEffect>> _effectGroups = new();
            private readonly List<Enums.eEffectType> _hasGroups = new()
            {
                Enums.eEffectType.Resistance,
                Enums.eEffectType.Defense,
                Enums.eEffectType.Mez,
                Enums.eEffectType.MezResist,
                Enums.eEffectType.Elusivity,
                Enums.eEffectType.Enhancement
            };

            private struct FxVectorIdentifier
            {
                public Enums.eEffectType EffectType;
                public Enums.eEffectType SubEffectType;
                public Enums.eToWho ToWho;
                public float BuffedMag;
            }

            public Dictionary<string, List<GroupedEffect>> Groups => _effectGroups;

            private EffectsGroupFilter(Dictionary<string, List<GroupedEffect>> groups)
            {
                _effectGroups = groups;
            }

            public static EffectsGroupFilter FromPower(IPower power)
            {
                var groups = new List<List<GroupedEffect>>();
                var groupsEffectTypes = new List<Dictionary<FxVectorIdentifier, int>>();
                var fxGroups = new List<List<IEffect>>();
                for (var i = 0; i < 6; i++)
                {
                    groups[i] = new List<GroupedEffect>();
                    fxGroups[i] = new List<IEffect>();
                    groupsEffectTypes[i] = new Dictionary<FxVectorIdentifier, int>();
                }

                // Assign effects to groups according to settings
                foreach (var fx in power.Effects)
                {
                    for (var i = 0; i < EffectVectorsGroups.Count; i++)
                    {
                        foreach (var vector in EffectVectorsGroups[i])
                        {
                            if (!vector.Validate(fx)) continue;

                            fxGroups[i].Add(fx);
                        }
                    }
                }

                // Merge similar effects
                for (var i = 0 ; i < fxGroups.Count ; i++)
                {
                    foreach (var fx in fxGroups[i])
                    {
                        var fxIdentifier = new FxVectorIdentifier
                        {
                            EffectType = fx.EffectType,
                            SubEffectType = fx.ETModifies,
                            ToWho = fx.ToWho,
                            BuffedMag = fx.BuffedMag
                        };

                        if (groupsEffectTypes[i].ContainsKey(fxIdentifier))
                        {
                            var index = groupsEffectTypes[i][fxIdentifier];
                            switch (fx.EffectType)
                            {
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Mez:
                                case Enums.eEffectType.Mez:
                                case Enums.eEffectType.MezResist:
                                    groups[i][index].AddMezType(fx.MezType);
                                    break;

                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Damage:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.DamageBuff:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Resistance:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Defense:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Elusivity:
                                case Enums.eEffectType.DamageBuff:
                                case Enums.eEffectType.Defense:
                                case Enums.eEffectType.Resistance:
                                case Enums.eEffectType.Elusivity:
                                    groups[i][index].AddDamageType(fx.DamageType);
                                    break;

                                case Enums.eEffectType.Enhancement:
                                case Enums.eEffectType.ResEffect:
                                    groups[i][index].AddETModifyType(fx.ETModifies);
                                    break;
                            }
                        }
                        else
                        {
                            groupsEffectTypes[i].Add(new FxVectorIdentifier
                            {
                                EffectType = fx.EffectType,
                                SubEffectType = fx.ETModifies,
                                ToWho = fx.ToWho,
                                BuffedMag = fx.BuffedMag
                            }, groups[i].Count);

                            groups[i].Add(fx.EffectType == Enums.eEffectType.Enhancement | fx.EffectType == Enums.eEffectType.ResEffect
                                ? new GroupedEffect(fx.EffectType, fx.ETModifies, fx.BuffedMag, fx.DisplayPercentage, fx.ToWho)
                                : new GroupedEffect(fx.EffectType, fx.BuffedMag, fx.DisplayPercentage, fx.ToWho));
                            
                            switch (fx.EffectType)
                            {
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Mez:
                                case Enums.eEffectType.Mez:
                                case Enums.eEffectType.MezResist:
                                    groups[i][groups[i].Count - 1].AddMezType(fx.MezType);
                                    break;

                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Damage:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.DamageBuff:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Resistance:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Defense:
                                case Enums.eEffectType.Enhancement when fx.ETModifies == Enums.eEffectType.Elusivity:
                                case Enums.eEffectType.DamageBuff:
                                case Enums.eEffectType.Defense:
                                case Enums.eEffectType.Resistance:
                                case Enums.eEffectType.Elusivity:
                                    groups[i][groups[i].Count - 1].AddDamageType(fx.DamageType);
                                    break;

                                case Enums.eEffectType.Enhancement:
                                case Enums.eEffectType.ResEffect:
                                    groups[i][groups[i].Count - 1].AddETModifyType(fx.ETModifies);
                                    break;
                            }
                        }
                    }
                }

                var labeledGroups = new Dictionary<string, List<GroupedEffect>>();
                for (var i = 0; i < groups.Count; i++)
                {
                    labeledGroups.Add(GroupLabels[i], groups[i]);
                }

                labeledGroups = labeledGroups
                    .Where(e => e.Value.Count > 0)
                    .ToDictionary(e => e.Key, e => e.Value);

                return new EffectsGroupFilter(labeledGroups);
            }
        }
        #endregion

        public DataView2()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
        }

        // Move the extra effects from the longest array (pBase) to the shortest (pEnh),
        // Resulting in pEnh always being the longest one.
        private static List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((IEffect)baseEffects[i].Clone());
            }

            var baseFxList = new List<IEffect>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((IEffect)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return new List<IEffect[]> { baseEffects, enhEffects };
        }

        public void SetData(IPower basePower = null, IPower enhancedPower = null, bool noLevel = false, bool locked = false, int historyIdx = -1)
        {
            _basePower = basePower;
            _enhancedPower = enhancedPower;
            Locked = locked;
            NoLevel = noLevel;
            HistoryIdx = historyIdx;
            BuildPowerEntry = HistoryIdx > -1
                ? MidsContext.Character.CurrentBuild.Powers[HistoryIdx]
                : null;

            // Data may differ from DB.
            if (_basePower == null) return;

            var dbPower = DatabaseAPI.GetPowerByFullName(_basePower.FullName);
            if (dbPower == null) return;

            if (_basePower != null)
            {
                _basePower.ActivatePeriod = dbPower.ActivatePeriod;
            }

            if (_enhancedPower == null) return;
            _enhancedPower.ActivatePeriod = dbPower.ActivatePeriod;

            if (_basePower.Effects.Length <= _enhancedPower.Effects.Length) return;

            var swappedFx = SwapExtraEffects(_basePower.Effects, _enhancedPower.Effects);
            _basePower.Effects = (IEffect[])swappedFx[0].Clone();
            _enhancedPower.Effects = (IEffect[])swappedFx[1].Clone();
        }

        private void InitScaler()
        {
            // Scales tab ? DataView2_Load ?
            if (_basePower is { VariableEnabled: true } && HistoryIdx > -1)
            {
                FreezeScalerCB = true;
                labelPowerScaler1.Text = string.IsNullOrWhiteSpace(_basePower.VariableName)
                    ? "Targets"
                    : _basePower.VariableName;
                powerScaler1.Minimum = _basePower.VariableMin;
                powerScaler1.Maximum = _basePower.VariableMax;
                powerScaler1.Value = MidsContext.Character.CurrentBuild.Powers[HistoryIdx].VariableValue;
                // Show range tooltip when mouseover ?
                // Show current value when moving, mousedown ?
                FreezeScalerCB = false;
                panelPowerScaler1.Visible = true;
            }
            else
            {
                panelPowerScaler1.Visible = false;
            }
        }

        #region Text to RTF methods

        private string Text2RTF(string s)
        {
            return $"{RTF.StartRTF()}{s.Replace("\r\n", "\n").Replace("\n", RTF.Crlf())}{RTF.EndRTF()}";
        }

        private string List2RTF(List<string> ls)
        {
            var ret = RTF.StartRTF();
            for (var i = 0 ; i < ls.Count ; i++)
            {
                if (i == 0)
                {
                    ret += RTF.Crlf();
                }

                ret += ls[i];
            }

            ret += RTF.EndRTF();

            return ret;
        }
        #endregion

        private BoostType GetBoostType(float valueBase, float valueEnhanced)
        {
            var diff = valueEnhanced - valueBase;
            
            return diff switch
            {
                < 0 => BoostType.Reduction,
                > 0 => BoostType.Enhancement,
                _ => BoostType.Equal
            };
        }

        private static ListViewItem CreateStatLvItem(string statName, string value, BoostType boostType, string tip = "")
        {
            var valueColor = boostType switch
            {
                BoostType.Reduction => Color.FromArgb(255, 20, 20),
                BoostType.Enhancement => Color.FromArgb(0, 240, 80),
                BoostType.Extra => Color.FromArgb(0, 220, 220),
                _ => Color.WhiteSmoke,
            };

            var lvItem = new ListViewItem
            {
                ForeColor = Color.FromArgb(160, 160, 160),
                Text = statName,
                ToolTipText = tip
            };

            lvItem.SubItems.Add(
                new ListViewItem.ListViewSubItem
                {
                    ForeColor = valueColor,
                    Text = value
                }
            );

            return lvItem;
        }

        private static ListViewItem CreateStatLvItem()
        {
            var lvItem = new ListViewItem
            {
                Text = ""
            };

            lvItem.SubItems.Add(
                new ListViewItem.ListViewSubItem
                {
                    Text = ""
                }
            );

            return lvItem;
        }

        private static Color InterpolateColor(decimal value, decimal valueMin, decimal valueMax, ColorRange colorRange)
        {
            return Color.FromArgb(
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.R - colorRange.LowerBoundColor.R) + colorRange.LowerBoundColor.R),
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.G - colorRange.LowerBoundColor.G) + colorRange.LowerBoundColor.G),
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.B - colorRange.LowerBoundColor.B) + colorRange.LowerBoundColor.B)
            );
        }

        #region Info Tab

        private void DisplayInfo()
        {
            infoTabTitle.Text = $"{(BuildPowerEntry != null ? $"[{BuildPowerEntry.Level}] " : "")}{_basePower?.DisplayName ?? "Info"}";
            richInfoSmall.Rtf = Text2RTF(_basePower?.DescShort ?? "");
            richInfoLarge.Rtf = Text2RTF(_basePower?.DescLong ?? "");

            if (_basePower == null) return;

            // Add basic power info
            listInfosL.BeginUpdate();
            listInfosR.BeginUpdate();
            listInfosL.Items.Add(CreateStatLvItem("End Cost", $"{_enhancedPower.EndCost:##.##}", GetBoostType(_basePower.EndCost, _enhancedPower?.EndCost ?? _basePower.EndCost)));
            listInfosL.Items.Add(CreateStatLvItem("Recharge", $"{_enhancedPower.RechargeTime:#####.##}s", GetBoostType(_basePower.RechargeTime, _enhancedPower?.RechargeTime ?? _basePower.RechargeTime)));
            listInfosL.Items.Add(CreateStatLvItem("Range", $"{_enhancedPower.Range:####.##}ft", GetBoostType(_basePower.Range, _enhancedPower?.Range ?? _basePower.Range)));
            listInfosL.Items.Add(CreateStatLvItem("Case Time", $"{_enhancedPower.CastTime:##.##}s", GetBoostType(_basePower.CastTime, _enhancedPower?.CastTime ?? _basePower.CastTime)));

            listInfosR.Items.Add(CreateStatLvItem("Accuracy", $"{_enhancedPower.Accuracy:P2}", GetBoostType(_basePower.Accuracy, _enhancedPower?.Accuracy ?? _basePower.Accuracy)));

            // Check if there is a mez effect, display duration in the right column.
            var hasMez = _basePower.Effects.Any(e => e.EffectType == Enums.eEffectType.Mez);
            if (hasMez)
            {
                var baseDuration = _basePower.Effects
                    .Where(e => e.EffectType == Enums.eEffectType.Mez)
                    .Select(e => e.Duration)
                    .Max();

                var enhancedDuration = _enhancedPower.Effects
                    .Where(e => e.EffectType == Enums.eEffectType.Mez)
                    .Select(e => e.Duration)
                    .Max();

                listInfosR.Items.Add(CreateStatLvItem("Duration", $"{enhancedDuration:###.##}s", GetBoostType(baseDuration, enhancedDuration)));

                listInfosR.Items.Add(CreateStatLvItem());
                listInfosR.Items.Add(CreateStatLvItem());
            }
            else
            {
                listInfosR.Items.Add(CreateStatLvItem());
                listInfosR.Items.Add(CreateStatLvItem());
                listInfosR.Items.Add(CreateStatLvItem());
            }

            // Misc & special effects (4 max)
            var effectsHidden = new[]
            {
                Enums.eEffectType.GrantPower,
                Enums.eEffectType.RevokePower,
                Enums.eEffectType.PowerRedirect,
                Enums.eEffectType.Null,
                Enums.eEffectType.SetMode,
                Enums.eEffectType.EntCreate,
                Enums.eEffectType.Damage
            };

            var miscEffectsIndexes = _enhancedPower.Effects.FindIndexes(e => !effectsHidden.Contains(e.EffectType)).ToList();
            for (var i = 0 ; i < Math.Min(4, miscEffectsIndexes.Count) ; i++)
            {
                if (miscEffectsIndexes[i] >= _basePower.Effects.Length || _basePower.Effects[miscEffectsIndexes[i]].EffectType != _enhancedPower.Effects[miscEffectsIndexes[i]].EffectType)
                {
                    var fx = _enhancedPower.Effects[miscEffectsIndexes[i]];
                    var fxType = fx.EffectType switch
                    {
                        Enums.eEffectType.Enhancement => $"{fx.EffectType}({fx.ETModifies})",
                        Enums.eEffectType.MezResist => $"{fx.EffectType}({fx.MezType})",
                        Enums.eEffectType.Mez => $"{fx.EffectType}({fx.MezType})",
                        Enums.eEffectType.Resistance => $"{fx.EffectType}({fx.DamageType})",
                        Enums.eEffectType.Defense => $"{fx.EffectType}({fx.DamageType})",
                        _ => $"{fx.EffectType}"
                    };

                    /*var enhValue = fx.EffectType switch
                    {
                        Enums.eEffectType.Mez when fx.MezType == Enums.eMez.Knockback | fx.MezType == Enums.eMez.Knockup => fx.BuffedMag,
                        Enums.eEffectType.Mez => fx.Duration,
                        _ => fx.BuffedMag
                    };*/

                    if (i % 2 == 0)
                    {
                        listInfosL.Items.Add(CreateStatLvItem(fxType, fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLvItem(fxType, fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
                    }
                }
                else
                {
                    var fxEnh = _enhancedPower.Effects[miscEffectsIndexes[i]];
                    var fxBase = _basePower.Effects[miscEffectsIndexes[i]];
                    var fxType = fxEnh.EffectType switch
                    {
                        Enums.eEffectType.Enhancement => $"{fxEnh.EffectType}({fxEnh.ETModifies})",
                        Enums.eEffectType.MezResist => $"{fxEnh.EffectType}({fxEnh.MezType})",
                        Enums.eEffectType.Mez => $"{fxEnh.EffectType}({fxEnh.MezType})",
                        Enums.eEffectType.Resistance => $"{fxEnh.EffectType}({fxEnh.DamageType})",
                        Enums.eEffectType.Defense => $"{fxEnh.EffectType}({fxEnh.DamageType})",
                        _ => $"{fxEnh.EffectType}"
                    };

                    var enhValue = fxEnh.EffectType switch
                    {
                        Enums.eEffectType.Mez when fxEnh.MezType == Enums.eMez.Knockback | fxEnh.MezType == Enums.eMez.Knockup => fxEnh.BuffedMag,
                        Enums.eEffectType.Mez => fxEnh.Duration,
                        _ => fxEnh.BuffedMag
                    };

                    var baseValue = fxBase.EffectType switch
                    {
                        Enums.eEffectType.Mez when fxBase.MezType == Enums.eMez.Knockback | fxBase.MezType == Enums.eMez.Knockup => fxBase.BuffedMag,
                        Enums.eEffectType.Mez => fxBase.Duration,
                        _ => fxBase.BuffedMag
                    };

                    if (i % 2 == 0)
                    {
                        listInfosL.Items.Add(CreateStatLvItem(fxType, fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}", GetBoostType(baseValue, enhValue)));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLvItem(fxType, fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}", GetBoostType(baseValue, enhValue)));
                    }
                }
            }

            listInfosL.EndUpdate();
            listInfosR.EndUpdate();

            var baseDamage = _basePower.FXGetDamageValue();
            var enhancedDamage = _enhancedPower.FXGetDamageValue();
            ctlDamageDisplay1.nBaseVal = baseDamage;
            ctlDamageDisplay1.nMaxEnhVal = baseDamage * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
            ctlDamageDisplay1.nEnhVal = enhancedDamage;
            ctlDamageDisplay1.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                ? $"{_enhancedPower.FXGetDamageString()} ({Utilities.FixDP(baseDamage)})"
                : _basePower.FXGetDamageString();
        }

        #endregion

        #region Effects Tab
        
        private void DisplayEffects()
        {
            var effectGroups = EffectsGroupFilter.FromPower(_enhancedPower);
            var labels = effectGroups.Groups.Keys.ToList();
            for (var i = 0 ; i < effectGroups.Groups.Count ; i+=2)
            {
                if (i < effectGroups.Groups.Count - 1)
                {
                    switch (i)
                    {
                        case 0:
                            lblEffectsBlock1.Text = $"{labels[i]}/{labels[i + 1]}";
                            break;

                        case 2:
                            lblEffectsBlock2.Text = $"{labels[i]}/{labels[i + 1]}";
                            break;

                        case 4:
                            lblEffectsBlock3.Text = $"{labels[i]}/{labels[i + 1]}";
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            lblEffectsBlock1.Text = labels[i];
                            break;

                        case 2:
                            lblEffectsBlock2.Text = labels[i];
                            break;

                        case 4:
                            lblEffectsBlock3.Text = labels[i];
                            break;
                    }
                }
            }

            lvEffectsBlock1L.BeginUpdate();
            lvEffectsBlock1R.BeginUpdate();
            lvEffectsBlock2L.BeginUpdate();
            lvEffectsBlock2R.BeginUpdate();
            lvEffectsBlock3L.BeginUpdate();
            lvEffectsBlock3R.BeginUpdate();

            var groupedItems = effectGroups.Groups.Values.ToList();
            for (var i = 0; i < groupedItems.Count; i++)
            {
                var target = i switch
                {
                    0 => lvEffectsBlock1L,
                    1 => lvEffectsBlock1R,
                    2 => lvEffectsBlock2L,
                    3 => lvEffectsBlock2R,
                    4 => lvEffectsBlock3L,
                    _ => lvEffectsBlock3R
                };

                for (var j = 0 ; j < groupedItems[i].Count; j++)
                {
                    var boostType = groupedItems[i][j].GetBoostType();
                    var stat = groupedItems[i][j].GetStatName();
                    var mag = groupedItems[i][j].GetMagString();
                    var lvItem = CreateStatLvItem(stat, mag, boostType, $"{groupedItems[i][j]}");

                    target.Items.Add(lvItem);
                }
            }

            lvEffectsBlock1L.EndUpdate();
            lvEffectsBlock1R.EndUpdate();
            lvEffectsBlock2L.EndUpdate();
            lvEffectsBlock2R.EndUpdate();
            lvEffectsBlock3L.EndUpdate();
            lvEffectsBlock3R.EndUpdate();
        }

        #endregion

        #region Totals Tab
        private void DisplayTotals()
        {
            var displayStats = MidsContext.Character.DisplayStats;

            dV2TotalsPane1L.ClearItems();
            dV2TotalsPane1R.ClearItems();
            var damageVectors = Enum.GetNames(typeof(Enums.eDamage));
            for (var i = 1; i < damageVectors.Length; i++)
            {
                if (damageVectors[i] == "Toxic")
                {
                    continue;
                }

                var target = i < 6 ? dV2TotalsPane1L : dV2TotalsPane1R;
                target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.Defense(i), displayStats.Defense(0), true));
            }

            dV2TotalsPane2L.ClearItems();
            dV2TotalsPane2R.ClearItems();
            for (var i = 1; i < damageVectors.Length; i++)
            {
                var target = i < 6 ? dV2TotalsPane2L : dV2TotalsPane2R;
                target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.DamageResistance(i, false), displayStats.DamageResistance(i, true), true));
            }

            // Misc effects ??
        }

        #endregion

        #region Enhance Tab

        private string RelativeLevelString(Enums.eEnhRelative relativeLevel, bool showZero = false)
        {
            return relativeLevel switch
            {
                // Move to a separate method
                Enums.eEnhRelative.MinusThree => "-3",
                Enums.eEnhRelative.MinusTwo => "-2",
                Enums.eEnhRelative.MinusOne => "-1",
                Enums.eEnhRelative.PlusOne => "+1",
                Enums.eEnhRelative.PlusTwo => "+2",
                Enums.eEnhRelative.PlusThree => "+3",
                Enums.eEnhRelative.PlusFour => "+4",
                Enums.eEnhRelative.PlusFive => "+5",
                _ => showZero ? "+0" : ""
            };
        }

        private SKBitmap CreateEnhancementsBitmap(bool alternate, int flippingEnhancement = -1, int flipAngle = 0)
        {
            // Enhancement need to fetch the first half from alt, second half from main if flipping
            var enhancements = BuildPowerEntry.Slots.Select(s => alternate ? s.FlippedEnhancement : s.Enhancement).ToList();
            using var bitmap = new SKBitmap(new SKImageInfo(skglEnhActive.Width, skglEnhActive.Height, SKColorType.Rgba8888, SKAlphaType.Premul)); // using ?
            using var canvas = new SKCanvas(bitmap);
            var offsetY = (float)Math.Max(0, Math.Round((skglEnhActive.Height - 30) / 2d));
            var offsetX = (float)skglEnhActive.Width - 183;
            // flipAngle = Math.Max(0, Math.Min(90, flipAngle);

            for (var i = 0 ; i < enhancements.Count ; i++)
            {
                if (enhancements[i].Enh <= -1) continue;

                var imgIdx = DatabaseAPI.Database.Enhancements[enhancements[i].Enh].ImageIdx;
                var enhGrade = I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[enhancements[i].Enh].TypeID);
                var grade = enhancements[i].Grade; // ???
                var sourceRect = I9Gfx.GetOverlayRect(enhGrade).ToSKRect();
                
                // Horizontally compress enhancement pic to mimic a 3D rotation
                var destRect = flippingEnhancement == i
                    ? new SKRect(offsetX + 30 * i + 15 * (float)Math.Sin(flipAngle / 180d * Math.PI),
                        offsetY,
                        offsetX + 30 * (i + 1) - 15 * (float)Math.Sin(flipAngle / 180d * Math.PI),
                        offsetY + 30)
                    : new SKRect(offsetX + 30 * i, offsetY, offsetX + 30 * (i + 1), offsetY + 30);

                // Draw border
                canvas.DrawBitmap(I9Gfx.Borders.Bitmap.ToSKBitmap(), sourceRect, destRect);

                // Draw enhancement
                canvas.DrawBitmap(I9Gfx.Enhancements[imgIdx].ToSKBitmap(), sourceRect, destRect);

                // Draw enhancement level
                var levelString = $"{enhancements[i].IOLevel}{RelativeLevelString(enhancements[i].RelativeLevel)}";
                using var textPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    TextSize = 7,
                    Color = SKColors.Cyan,
                    FilterQuality = SKFilterQuality.High,
                    HintingLevel = SKPaintHinting.Normal
                };

                using var outlinePaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    TextSize = 7,
                    StrokeWidth = 2,
                    Color = SKColors.Black,
                    FilterQuality = SKFilterQuality.High,
                    HintingLevel = SKPaintHinting.Normal
                };

                var textBounds = new SKRect();
                textPaint.MeasureText(levelString, ref textBounds);
                var textLocation = new SKPoint(offsetX + 30 * i + 15 - textBounds.Width / 2, offsetY);

                using var textPath = textPaint.GetTextPath(levelString, textLocation.X, textLocation.Y);
                using var outlinePath = outlinePaint.GetTextPath(levelString, textLocation.X, textLocation.Y);

                // ???
                canvas.DrawPath(textPath, textPaint);
                canvas.DrawPath(outlinePath, outlinePaint);
            }

            return bitmap;
        }

        private void DisplayEnhance()
        {
            /*using (var bitmap = new Bitmap(pnlEnhActive.Width, pnlEnhActive.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.DrawImageUnscaled(CreateEnhancementsBitmap(false).ToBitmap(), new Point(0, 0));
                }
            }

            using (var bitmap = new Bitmap(pnlEnhAlt.Width, pnlEnhAlt.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.DrawImageUnscaled(CreateEnhancementsBitmap(true).ToBitmap(), new Point(0, 0));
                }
            }*/
        }

        #endregion

        #region Event callbacks

        private void tabBox_TabIndexChanged(object sender, EventArgs e)
        {
            switch (tabBox.TabIndex)
            {
                case 0:
                    // L=39 / L=23
                    tabBox.ActiveTabColor = Color.FromArgb(12, 56, 100);
                    tabBox.InactiveTabColor = Color.FromArgb(7, 33, 59);
                    break;

                case 1:
                    // L=51 / L=30
                    tabBox.ActiveTabColor = Color.Indigo;
                    tabBox.InactiveTabColor = Color.FromArgb(45, 0, 77);
                    break;

                // Green and Teal colors need some refreshers.
                case 2:
                    // L=50 / L=30
                    tabBox.ActiveTabColor = Color.Green;
                    tabBox.InactiveTabColor = Color.FromArgb(0, 77, 0);
                    break;

                case 3:
                    // L=50 / L=30
                    tabBox.ActiveTabColor = Color.Teal;
                    tabBox.InactiveTabColor = Color.FromArgb(0, 77, 77);
                    break;

                case 4:
                    // L=58 / L=35
                    tabBox.ActiveTabColor = Color.FromArgb(148, 117, 46);
                    tabBox.InactiveTabColor = Color.FromArgb(69, 71, 28);
                    break;
            }
        }

        private void powerScaler_ValueChanged(object sender, EventArgs e)
        {
            var target = (ColorSlider)sender;

            target.ElapsedInnerColor = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedInnerColor);
            target.ElapsedPenColorBottom = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorBottom);
            target.ElapsedPenColorTop = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorTop);

            if (FreezeScalerCB) return;

            MainModule.MidsController.Toon.GenerateBuffedPowerArray();

            // Display updated infos (???)
        }

        #endregion
    }
}
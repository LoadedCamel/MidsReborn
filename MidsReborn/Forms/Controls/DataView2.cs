using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
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
        private FlipAnimator _flipAnimator;

        private static readonly SKBitmap
            NewSlotBitmap = FlipAnimator.Bitmaps.CreateBitmap(@"Images\Newslot.png"); // ???

        // Track bar colors for power scalers
        private readonly TrackGradientsScheme TrackColors = new()
        {
            ElapsedInnerColor = new ColorRange
                { LowerBoundColor = Color.FromArgb(0, 51, 0), UpperBoundColor = Color.FromArgb(0, 128, 0) },
            ElapsedPenColorBottom = new ColorRange
                { LowerBoundColor = Color.FromArgb(58, 94, 58), UpperBoundColor = Color.FromArgb(144, 238, 44) },
            ElapsedPenColorTop = new ColorRange
                { LowerBoundColor = Color.FromArgb(0, 102, 51), UpperBoundColor = Color.FromArgb(0, 255, 127) }
        };

        public bool Locked;

        // Group labels (effects tab)
        private static readonly List<string> GroupLabels = new()
            { "Resistance", "Defense", "Buffs", "Debuffs", "Summons/Grants", "Misc." };

        #region Effect vector type sub-class

        private class EffectVectorType
        {
            public Enums.eEffectType? EffectType;
            public Enums.eMez? MezType;
            public Enums.eDamage? DamageType;
            public Enums.eEffectType? ETModifies;
            public BuffEffectType VectorDirection;
            public Enums.eToWho ToWho;

            public EffectVectorType(Enums.eEffectType effectType,
                BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public EffectVectorType(Enums.eEffectType effectType, Enums.eMez mezType,
                BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                MezType = mezType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public EffectVectorType(Enums.eEffectType effectType, Enums.eDamage damageType,
                BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
            {
                EffectType = effectType;
                DamageType = damageType;
                VectorDirection = vectorDirection;
                ToWho = toWho;
            }

            public EffectVectorType(Enums.eEffectType effectType, Enums.eEffectType etModifies,
                BuffEffectType vectorDirection = BuffEffectType.NonZero, Enums.eToWho toWho = Enums.eToWho.All)
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

            public GroupedEffect(Enums.eEffectType effectType, float buffedMag, bool displayPercentage,
                Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = Enums.eEffectType.None;
                BuffedMag = buffedMag;
                DisplayPercentage = displayPercentage;
                ToWho = toWho;
            }

            public GroupedEffect(Enums.eEffectType effectType, Enums.eEffectType subEffectType, float buffedMag,
                bool displayPercentage, Enums.eToWho toWho)
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
                        else if (ContainsMultiOnly(damageTypes.GetRange(0, 7).Union(damageTypes.GetRange(8, 3)),
                                     DamageTypes))
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
                            return
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {SubEffectType}({MezTypesString()}){ToWhoString()}";

                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Elusivity:
                            return
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {SubEffectType}({DamageTypesString()}){ToWhoString()}";

                        default:
                            return
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType} to {ETModifiesString()}{ToWhoString()}";
                    }
                }

                switch (EffectType)
                {
                    case Enums.eEffectType.Mez:
                    case Enums.eEffectType.MezResist:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}({MezTypesString()}){ToWhoString()}";

                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}({DamageTypesString()}){ToWhoString()}";

                    default:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:#####.##}")} {EffectType}{ToWhoString()}";
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
                for (var i = 0; i < fxGroups.Count; i++)
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

                            groups[i].Add(fx.EffectType == Enums.eEffectType.Enhancement |
                                          fx.EffectType == Enums.eEffectType.ResEffect
                                ? new GroupedEffect(fx.EffectType, fx.ETModifies, fx.BuffedMag, fx.DisplayPercentage,
                                    fx.ToWho)
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

        #region Flip animator sub-class

        private class FlipAnimator
        {
            internal struct SKSlotBitmap
            {
                public SKBitmap Bitmap;
                public bool IsEmpty;
                public bool ValidSlot;
                public Enums.eEnhGrade EnhType;
            }

            internal enum Tray
            {
                Main,
                Alt
            }

            private List<SKSlotBitmap> EnhMainBitmaps = new();
            private List<SKSlotBitmap> EnhAltBitmaps = new();
            public bool Active;
            public float Angle;
            public const float KerningAngle = 30;
            public int NbEnhMain => EnhMainBitmaps.Count;
            public int NbEnhAlt => EnhAltBitmaps.Count;

            public float FullCycleAngle =>
                180 + KerningAngle * (Math.Max(EnhMainBitmaps.Count, EnhAltBitmaps.Count) - 1);

            public static class Bitmaps
            {
                private static SKPaint GenerateColorFilter(SKSlotBitmap slot)
                {
                    var validSlotBlendAdd = slot.ValidSlot ? 0 : -0.4f;
                    var validSlotBlendMult = slot.ValidSlot ? 1 : 0.4f;

                    return slot.IsEmpty
                        ? new SKPaint
                        {
                            ColorFilter = SKColorFilter.CreateColorMatrix(new[]
                            {
                                0.21f, 0.72f, 0.07f, validSlotBlendAdd, 0,
                                0.21f, 0.72f, 0.07f, validSlotBlendAdd, 0,
                                0.21f, 0.72f, 0.07f, validSlotBlendAdd, 0,
                                0, 0, 0, 1, 0
                            })
                        }
                        : new SKPaint
                        {
                            ColorFilter = SKColorFilter.CreateColorMatrix(new[]
                            {
                                validSlotBlendMult, 0, 0, validSlotBlendAdd, 0,
                                0, validSlotBlendMult, 0, validSlotBlendAdd, 0,
                                0, 0, validSlotBlendMult, validSlotBlendAdd, 0,
                                0, 0, 0, 1f, 0
                            })
                        };
                }

                public static SKImage DrawSingle(SKSlotBitmap enhMain, SKSlotBitmap enhAlt, float angleDeg)
                {
                    var surface = SKSurface.Create(new SKImageInfo(30, 30));
                    surface.Canvas.Clear(SKColors.Black);
                    var sourceRect = new SKRect(0, 0, 30, 30);
                    var destRect = new SKRect(
                        15 - 15 * (float)Math.Abs(Math.Cos(angleDeg / 180 * Math.PI)),
                        0,
                        15 + 15 * (float)Math.Abs(Math.Cos(angleDeg / 180 * Math.PI)),
                        30);
                    if (angleDeg >= 0 & angleDeg < 90 | angleDeg >= 270 & angleDeg < 360)
                    {
                        using var paint = GenerateColorFilter(enhMain);
                        // Border/level are pre-integrated in enhancement.Bitmap
                        surface.Canvas.DrawBitmap(enhMain.Bitmap, sourceRect, destRect, paint);
                    }
                    else
                    {
                        using var paint = GenerateColorFilter(enhAlt);
                        surface.Canvas.DrawBitmap(enhAlt.Bitmap, sourceRect, destRect, paint);
                    }

                    return surface.Snapshot();
                }

                public static SKBitmap CreateBitmap(string enhFile)
                {
                    return SKBitmap.Decode(File.ReadAllBytes(enhFile));
                }

                public static SKBitmap CreateBitmap(int enhIndex, int ioLevel = -1,
                    Enums.eEnhRelative relativeLevel = Enums.eEnhRelative.Even)
                {
                    var bitmap = new SKBitmap(new SKImageInfo(30, 30, SKColorType.Rgba8888, SKAlphaType.Premul));
                    using var canvas = new SKCanvas(bitmap);

                    var imgIdx = DatabaseAPI.Database.Enhancements[enhIndex].ImageIdx;
                    var enhGrade =
                        I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[enhIndex].TypeID); // Enums.eEnhGrade ?
                    var sourceRect = I9Gfx.GetOverlayRect(enhGrade).ToSKRect();
                    var destRect = new SKRect(0, 0, 30, 30);

                    // Draw border
                    canvas.DrawBitmap(I9Gfx.Borders.Bitmap.ToSKBitmap(), sourceRect, destRect);

                    // Draw enhancement
                    canvas.DrawBitmap(I9Gfx.Enhancements[imgIdx].ToSKBitmap(), sourceRect, destRect);

                    // Draw enhancement level
                    if (ioLevel == -1)
                    {
                        return bitmap;
                    }

                    var levelString = $"{ioLevel}{RelativeLevelString(relativeLevel)}";
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
                    var textLocation = new SKPoint(15 - textBounds.Width / 2, 1);

                    using var textPath = textPaint.GetTextPath(levelString, textLocation.X, textLocation.Y);
                    using var outlinePath = outlinePaint.GetTextPath(levelString, textLocation.X, textLocation.Y);

                    canvas.DrawPath(textPath, textPaint);
                    canvas.DrawPath(outlinePath, outlinePaint);

                    return bitmap;
                }
            }

            public FlipAnimator(PowerEntry buildPowerEntry)
            {
                for (var i = 0; i < buildPowerEntry.Slots.Length; i++)
                {
                    var enhSlot = buildPowerEntry.Slots[i].Enhancement;
                    var emptySlot = enhSlot.Enh < 0;
                    var slotBitmap = emptySlot
                        ? NewSlotBitmap
                        : Bitmaps.CreateBitmap(enhSlot.Enh, enhSlot.IOLevel, enhSlot.RelativeLevel);
                    EnhMainBitmaps.Add(new SKSlotBitmap
                    {
                        Bitmap = slotBitmap,
                        IsEmpty = emptySlot,
                        ValidSlot = true, // To be done later
                        EnhType = enhSlot.Grade
                    });
                }

                for (var i = 0; i < buildPowerEntry.Slots.Length; i++)
                {
                    var enhSlot = buildPowerEntry.Slots[i].Enhancement;
                    var emptySlot = enhSlot.Enh < 0;
                    var slotBitmap = emptySlot
                        ? NewSlotBitmap
                        : Bitmaps.CreateBitmap(enhSlot.Enh, enhSlot.IOLevel, enhSlot.RelativeLevel);
                    EnhMainBitmaps.Add(new SKSlotBitmap
                    {
                        Bitmap = slotBitmap,
                        IsEmpty = emptySlot,
                        ValidSlot = true, // To be done later
                        EnhType = enhSlot.Grade
                    });
                }

                // Fillers (?)
                for (var i = Math.Min(EnhMainBitmaps.Count, EnhAltBitmaps.Count);
                     i < Math.Max(EnhMainBitmaps.Count, EnhAltBitmaps.Count);
                     i++)
                {
                    if (EnhMainBitmaps.Count < EnhAltBitmaps.Count)
                    {
                        EnhMainBitmaps.Add(new SKSlotBitmap
                        {
                            Bitmap = NewSlotBitmap,
                            IsEmpty = true,
                            ValidSlot = true,
                            EnhType = Enums.eEnhGrade.None
                        });
                    }
                    else
                    {
                        EnhAltBitmaps.Add(new SKSlotBitmap
                        {
                            Bitmap = NewSlotBitmap,
                            IsEmpty = true,
                            ValidSlot = true,
                            EnhType = Enums.eEnhGrade.None
                        });
                    }
                }
            }

            public void SwapSets()
            {
                var tempBitmaps = EnhMainBitmaps.Clone();
                EnhMainBitmaps = EnhAltBitmaps.Clone();
                EnhAltBitmaps = tempBitmaps;
            }

            public SKSlotBitmap GetBitmap(Tray tray, int slotId)
            {
                var traySource = tray == Tray.Alt ? EnhAltBitmaps : EnhMainBitmaps;
                if (slotId > traySource.Count || slotId < 0)
                {
                    return traySource[0];
                }

                return traySource[slotId];
            }
        }

        #endregion

        #region Scales graph sub-class

        private static class VariableStatsGraph
        {
            public static class PowerStats
            {
                public struct FxIdentifier
                {
                    public Enums.eEffectType EffectType;
                    public Enums.eMez MezType;
                    public Enums.eDamage DamageType;
                    public Enums.eEffectType ETModifies;
                }

                public struct DataPoint
                {
                    public int Stacks;
                    public float Value;
                }

                public struct Range
                {
                    public float Min;
                    public float Max;
                }

                private static List<FxIdentifier> GetVariableStats(int historyIdx)
                {
                    var ret = new List<FxIdentifier>();
                    if (historyIdx < 0 || historyIdx >= MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        return ret;
                    }

                    if (MidsContext.Character.CurrentBuild.Powers[historyIdx].Power == null)
                    {
                        return ret;
                    }

                    var powerEntry = MidsContext.Character.CurrentBuild.Powers[historyIdx];
                    var power = powerEntry.Power;

                    foreach (var fx in power.Effects)
                    {
                        var fxIdent = new FxIdentifier
                        {
                            EffectType = fx.EffectType,
                            MezType = fx.MezType,
                            DamageType = fx.EffectType == Enums.eEffectType.Damage | fx.EffectType == Enums.eEffectType.Defense | fx.EffectType == Enums.eEffectType.Resistance
                                ? Enums.eDamage.None
                                : fx.DamageType,
                            ETModifies = fx.ETModifies
                        };

                        if (ret.Contains(fxIdent))
                        {
                            continue;
                        }

                        if (power.VariableEnabled & !fx.IgnoreScaling & (fx.AttribType == Enums.eAttribType.Duration | fx.AttribType == Enums.eAttribType.Magnitude))
                        {
                            ret.Add(fxIdent);
                        }
                        else if (power.VariableEnabled & fx.AttribType == Enums.eAttribType.Expression)
                        {
                            if (fx.MagnitudeExpression.Contains($"{power.FullName}>stacks"))
                            {
                                ret.Add(fxIdent);
                            }
                        }
                    }

                    return ret;
                }

                internal static Dictionary<FxIdentifier, List<DataPoint>> GetPOI(int historyIdx)
                {
                    var ret = new Dictionary<FxIdentifier, List<DataPoint>>();
                    var variableStats = GetVariableStats(historyIdx);
                    if (variableStats.Count <= 0) return ret;

                    foreach (var stat in variableStats)
                    {
                        ret.Add(stat, new List<DataPoint>());
                    }

                    var powerEntry = MidsContext.Character.CurrentBuild.Powers[historyIdx];
                    var power = powerEntry.Power;
                    var variableRange = Math.Abs(power.VariableMax - power.VariableMin);
                    for (var i = power.VariableMin;
                         i < power.VariableMax;
                         i = variableRange < 10 ? ++i : (int)Math.Round(i + (decimal)variableRange / 10))
                    {
                        powerEntry.VirtualVariableValue = i;
                        power.VirtualStacks = i;
                        MainModule.MidsController.Toon.GenerateBuffedPowerArray();
                        
                        // Warning: can be null
                        var pEnh = MainModule.MidsController.Toon.GetEnhancedPower(historyIdx);
                        if (pEnh == null)
                        {
                            continue;
                        }

                        foreach (var stat in variableStats)
                        {
                            if (stat.EffectType == Enums.eEffectType.Damage)
                            {
                                var dmg = pEnh.FXGetDamageValue();
                                ret[stat].Add(new DataPoint
                                {
                                    Stacks = i,
                                    Value = dmg
                                });
                            }
                            else
                            {
                                // Note: this won't work for Enhancement() or Mez() effects.
                                var fxTotal = pEnh.GetEffectMagSum(stat.EffectType);

                                ret[stat].Add(new DataPoint
                                {
                                    Stacks = i,
                                    Value = fxTotal.Sum
                                });
                            }
                        }
                    }

                    powerEntry.VirtualVariableValue = powerEntry.InternalVariableValue;
                    power.VirtualStacks = power.InternalStacks;
                    MainModule.MidsController.Toon.GenerateBuffedPowerArray();

                    return ret;
                }

                internal static Range GetValuesRange(Dictionary<FxIdentifier, List<DataPoint>> dataPoints)
                {
                    if (dataPoints.Keys.Count <= 0)
                    {
                        return new Range { Min = 0, Max = 0 };
                    }

                    var ret = new Range { Min = float.MaxValue, Max = float.MinValue };

                    foreach (var points in dataPoints.Values)
                    {
                        foreach (var p in points)
                        {
                            if (p.Value < ret.Min)
                            {
                                ret.Min = p.Value;
                            }

                            if (p.Value > ret.Max)
                            {
                                ret.Max = p.Value;
                            }
                        }
                    }

                    return ret;
                }

                internal static Dictionary<FxIdentifier, Range> GetValuesRangeEach(Dictionary<FxIdentifier, List<DataPoint>> dataPoints)
                {
                    var ret = new Dictionary<FxIdentifier, Range>();

                    foreach (var points in dataPoints)
                    {
                        var range = new Range { Min = float.MaxValue, Max = float.MinValue };
                        foreach (var p in points.Value)
                        {
                            if (p.Value < range.Min)
                            {
                                range.Min = p.Value;
                            }

                            if (p.Value > range.Max)
                            {
                                range.Max = p.Value;
                            }
                        }

                        ret.Add(points.Key, range);
                    }

                    return ret;
                }
            }

            private static SKColor GetGraphColorFromEffectType(Enums.eEffectType effectType)
            {
                switch (effectType)
                {
                    case Enums.eEffectType.Defense:
                        return SKColors.Magenta;

                    case Enums.eEffectType.Resistance:
                        return new SKColor(0, 192, 192);

                    case Enums.eEffectType.Regeneration:
                        return new SKColor(64, 255, 64);

                    case Enums.eEffectType.HitPoints:
                        return new SKColor(44, 180, 44);

                    case Enums.eEffectType.Recovery:
                        return SKColors.DodgerBlue;

                    case Enums.eEffectType.Endurance:
                        return new SKColor(59, 158, 255);

                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.SpeedRunning:
                    case Enums.eEffectType.JumpHeight:
                        return new SKColor(0, 192, 128);

                    case Enums.eEffectType.MaxFlySpeed:
                    case Enums.eEffectType.MaxJumpSpeed:
                    case Enums.eEffectType.MaxRunSpeed:
                        return new SKColor(0, 140, 94);

                    case Enums.eEffectType.StealthRadius:
                    case Enums.eEffectType.StealthRadiusPlayer:
                    case Enums.eEffectType.PerceptionRadius:
                        return new SKColor(106, 121, 136);

                    case Enums.eEffectType.RechargeTime:
                        return new SKColor(255, 128, 0);

                    case Enums.eEffectType.ToHit:
                        return new SKColor(255, 255, 128);

                    case Enums.eEffectType.Accuracy:
                        return SKColors.Yellow;

                    case Enums.eEffectType.Damage:
                        return SKColors.Red;

                    case Enums.eEffectType.DamageBuff:
                        return new SKColor(255, 64, 64);

                    case Enums.eEffectType.EnduranceDiscount:
                        return SKColors.RoyalBlue;

                    case Enums.eEffectType.ThreatLevel:
                        return SKColors.MediumPurple;

                    case Enums.eEffectType.Elusivity:
                        return new SKColor(163, 1, 231);

                    case Enums.eEffectType.Mez:
                        return new SKColor(113, 86, 168);

                    case Enums.eEffectType.MezResist:
                        return SKColors.Yellow;

                    default:
                        return new SKColor(13, 170, 222);
                }
            }

            private static SKColor GetGraphCurveColor(PowerStats.FxIdentifier fxIdentifier)
            {
                return fxIdentifier.EffectType == Enums.eEffectType.Enhancement
                    ? GetGraphColorFromEffectType(fxIdentifier.ETModifies)
                    : GetGraphColorFromEffectType(fxIdentifier.EffectType);
            }

            public static SKImage DrawScalesGraphSurface(int historyIdx, int w, int h)
            {
                var padding = new SKSize(5, 5);
                const int gridCells = 6;
                var dataPoints = PowerStats.GetPOI(historyIdx);

                var surface = SKSurface.Create(new SKImageInfo(w, h));
                surface.Canvas.Clear(SKColors.Black);

                if (dataPoints.Keys.Count <= 0) return surface.Snapshot();

                // Grid

                var pathGrid = new SKPath();
                for (var i = 1; i < gridCells - 1; i++)
                {
                    pathGrid.MoveTo(padding.Width + (w - 2 * padding.Width) / gridCells * i, padding.Height);
                    pathGrid.LineTo(padding.Width + (w - 2 * padding.Width) / gridCells * i, h - padding.Height);
                }

                for (var i = 1; i < gridCells - 1; i++)
                {
                    pathGrid.MoveTo(padding.Width, padding.Height + (h - 2 * padding.Height) / gridCells * i);
                    pathGrid.LineTo(w - padding.Width, padding.Height + (h - 2 * padding.Height) / gridCells * i);
                }

                surface.Canvas.DrawPath(pathGrid, new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = new SKColor(160, 160, 160),
                    StrokeWidth = 1,
                    StrokeCap = SKStrokeCap.Butt,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 20)
                });

                // Axis

                var pathAxis = new SKPath();
                pathAxis.MoveTo(padding.Width, padding.Height);
                pathAxis.LineTo(padding.Width, h - padding.Height);
                pathAxis.LineTo(w - padding.Width, h - padding.Height);
                surface.Canvas.DrawPath(pathAxis, new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.WhiteSmoke,
                    StrokeWidth = 1,
                    StrokeCap = SKStrokeCap.Butt,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 20)
                });

                // Points/Lines

                const float displayScaleFactor = 0.9f;
                var absoluteRange = PowerStats.GetValuesRange(dataPoints);
                var ranges = PowerStats.GetValuesRangeEach(dataPoints);
                var scaleFactors = ranges.ToDictionary(range => range.Key,
                    range => Math.Abs(range.Value.Min - range.Value.Max) < float.Epsilon
                        ? displayScaleFactor
                        : displayScaleFactor * (absoluteRange.Max - absoluteRange.Min) / (range.Value.Max - range.Value.Min));

                var bottomLine = h - padding.Height - (1 - displayScaleFactor) * (h - 2 * padding.Height);

                foreach (var series in dataPoints)
                {
                    var curveColor = GetGraphCurveColor(series.Key);
                    var path = new SKPath();
                    for (var i = 0; i < series.Value.Count; i++)
                    {
                        if (i == 0)
                        {
                            path.MoveTo(padding.Width + i * (w - 2 * padding.Width), bottomLine - scaleFactors[series.Key] * Math.Abs(series.Value[i].Value));
                        }
                        else
                        {
                            path.LineTo(padding.Width + i * (w - 2 * padding.Width), bottomLine - scaleFactors[series.Key] * Math.Abs(series.Value[i].Value));
                        }
                    }

                    surface.Canvas.DrawPath(path, new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = curveColor,
                        StrokeWidth = 1,
                        StrokeCap = SKStrokeCap.Butt
                    });
                }

                return surface.Snapshot();
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

        public void SetData(IPower basePower = null, IPower enhancedPower = null, bool noLevel = false,
            bool locked = false, int historyIdx = -1)
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

            _flipAnimator = new FlipAnimator(BuildPowerEntry);
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
            for (var i = 0; i < ls.Count; i++)
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

        private static ListViewItem CreateStatLvItem(string statName, string value, BoostType boostType,
            string tip = "")
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
                (int)Math.Round(
                    (value - valueMin) / (valueMax - valueMin) *
                    (colorRange.UpperBoundColor.R - colorRange.LowerBoundColor.R) + colorRange.LowerBoundColor.R),
                (int)Math.Round(
                    (value - valueMin) / (valueMax - valueMin) *
                    (colorRange.UpperBoundColor.G - colorRange.LowerBoundColor.G) + colorRange.LowerBoundColor.G),
                (int)Math.Round(
                    (value - valueMin) / (valueMax - valueMin) *
                    (colorRange.UpperBoundColor.B - colorRange.LowerBoundColor.B) + colorRange.LowerBoundColor.B)
            );
        }

        private static string RelativeLevelString(Enums.eEnhRelative relativeLevel, bool showZero = false)
        {
            return relativeLevel switch
            {
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

        #region Info Tab

        private void DisplayInfo()
        {
            infoTabTitle.Text =
                $"{(BuildPowerEntry != null ? $"[{BuildPowerEntry.Level}] " : "")}{_basePower?.DisplayName ?? "Info"}";
            richInfoSmall.Rtf = Text2RTF(_basePower?.DescShort ?? "");
            richInfoLarge.Rtf = Text2RTF(_basePower?.DescLong ?? "");

            if (_basePower == null) return;

            // Add basic power info
            listInfosL.BeginUpdate();
            listInfosR.BeginUpdate();
            listInfosL.Items.Add(CreateStatLvItem("End Cost", $"{_enhancedPower.EndCost:##.##}",
                GetBoostType(_basePower.EndCost, _enhancedPower?.EndCost ?? _basePower.EndCost)));
            listInfosL.Items.Add(CreateStatLvItem("Recharge", $"{_enhancedPower.RechargeTime:#####.##}s",
                GetBoostType(_basePower.RechargeTime, _enhancedPower?.RechargeTime ?? _basePower.RechargeTime)));
            listInfosL.Items.Add(CreateStatLvItem("Range", $"{_enhancedPower.Range:####.##}ft",
                GetBoostType(_basePower.Range, _enhancedPower?.Range ?? _basePower.Range)));
            listInfosL.Items.Add(CreateStatLvItem("Case Time", $"{_enhancedPower.CastTime:##.##}s",
                GetBoostType(_basePower.CastTime, _enhancedPower?.CastTime ?? _basePower.CastTime)));

            listInfosR.Items.Add(CreateStatLvItem("Accuracy", $"{_enhancedPower.Accuracy:P2}",
                GetBoostType(_basePower.Accuracy, _enhancedPower?.Accuracy ?? _basePower.Accuracy)));

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

                listInfosR.Items.Add(CreateStatLvItem("Duration", $"{enhancedDuration:###.##}s",
                    GetBoostType(baseDuration, enhancedDuration)));
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

            var miscEffectsIndexes =
                _enhancedPower.Effects.FindIndexes(e => !effectsHidden.Contains(e.EffectType)).ToList();
            for (var i = 0; i < Math.Min(4, miscEffectsIndexes.Count); i++)
            {
                if (miscEffectsIndexes[i] >= _basePower.Effects.Length ||
                    _basePower.Effects[miscEffectsIndexes[i]].EffectType !=
                    _enhancedPower.Effects[miscEffectsIndexes[i]].EffectType)
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
                        listInfosL.Items.Add(CreateStatLvItem(fxType,
                            fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLvItem(fxType,
                            fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
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
                        Enums.eEffectType.Mez when fxEnh.MezType == Enums.eMez.Knockback |
                                                   fxEnh.MezType == Enums.eMez.Knockup => fxEnh.BuffedMag,
                        Enums.eEffectType.Mez => fxEnh.Duration,
                        _ => fxEnh.BuffedMag
                    };

                    var baseValue = fxBase.EffectType switch
                    {
                        Enums.eEffectType.Mez when fxBase.MezType == Enums.eMez.Knockback |
                                                   fxBase.MezType == Enums.eMez.Knockup => fxBase.BuffedMag,
                        Enums.eEffectType.Mez => fxBase.Duration,
                        _ => fxBase.BuffedMag
                    };

                    if (i % 2 == 0)
                    {
                        listInfosL.Items.Add(CreateStatLvItem(fxType,
                            fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}",
                            GetBoostType(baseValue, enhValue)));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLvItem(fxType,
                            fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}",
                            GetBoostType(baseValue, enhValue)));
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
            for (var i = 0; i < effectGroups.Groups.Count; i += 2)
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

                for (var j = 0; j < groupedItems[i].Count; j++)
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
                target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.Defense(i),
                    displayStats.Defense(0), true));
            }

            dV2TotalsPane2L.ClearItems();
            dV2TotalsPane2R.ClearItems();
            for (var i = 1; i < damageVectors.Length; i++)
            {
                var target = i < 6 ? dV2TotalsPane2L : dV2TotalsPane2R;
                target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.DamageResistance(i, false),
                    displayStats.DamageResistance(i, true), true));
            }

            // Misc effects ??
        }

        #endregion

        #region Enhance Tab
        
        private void DisplayEnhance()
        {
            // ???
            skglEnhActive.Invalidate();
            skglEnhAlt.Invalidate();

            var edFiguresBuffs = Build.EDFigures.GetBuffsForBuildPower(HistoryIdx);
        }

        #endregion

        #region Scales Tab

        private void DisplayScales()
        {
            skglScalesGraph.Invalidate();
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

                case 2:
                    // L=33 / L=20
                    tabBox.ActiveTabColor = Color.FromArgb(2, 85, 55);
                    tabBox.InactiveTabColor = Color.FromArgb(1, 51, 33);
                    break;

                case 3:
                    // L=45 / L=27
                    tabBox.ActiveTabColor = Color.FromArgb(0, 98, 116);
                    tabBox.InactiveTabColor = Color.FromArgb(0, 59, 69);
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

            target.ElapsedInnerColor = InterpolateColor(target.Value, target.Minimum, target.Maximum,
                TrackColors.ElapsedInnerColor);
            target.ElapsedPenColorBottom = InterpolateColor(target.Value, target.Minimum, target.Maximum,
                TrackColors.ElapsedPenColorBottom);
            target.ElapsedPenColorTop = InterpolateColor(target.Value, target.Minimum, target.Maximum,
                TrackColors.ElapsedPenColorTop);

            if (FreezeScalerCB) return;

            MainModule.MidsController.Toon.GenerateBuffedPowerArray();

            // Display updated infos (???)
        }

        #endregion

        private void skglEnh_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Black);
            for (var i = 0; i < Math.Max(_flipAnimator.NbEnhMain, _flipAnimator.NbEnhAlt); i++)
            {
                var skImage = FlipAnimator.Bitmaps.DrawSingle(
                    _flipAnimator.GetBitmap(FlipAnimator.Tray.Main, i),
                    _flipAnimator.GetBitmap(FlipAnimator.Tray.Alt, i),
                    Math.Min(180, Math.Max(0, _flipAnimator.Angle - FlipAnimator.KerningAngle * i)));
                e.Surface.Canvas.DrawImage(skImage, new SKPoint(30 + 33 * i, 30));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_flipAnimator.Angle >= _flipAnimator.FullCycleAngle)
            {
                timer1.Stop();
                // Swap main and alt slots
                _flipAnimator.SwapSets();
                _flipAnimator.Angle = 0;
                _flipAnimator.Active = false;
            }
            else
            {
                _flipAnimator.Angle = Math.Min(_flipAnimator.Angle + 15, _flipAnimator.FullCycleAngle);
                skglEnhActive.Invalidate();
                skglEnhAlt.Invalidate();
            }
        }

        private void skglControl_Click(object sender, EventArgs e)
        {
            if (_flipAnimator.Active)
            {
                return;
            }

            _flipAnimator.Active = true;
            timer1.Start();
        }

        private void skglScalesGraph_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            var target = (SKGLControl)sender;

            e.Surface.Canvas.Clear(SKColors.Black);
            var graph = VariableStatsGraph.DrawScalesGraphSurface(HistoryIdx, target.Width, target.Height);
            e.Surface.Canvas.DrawImage(graph, new SKPoint(0, 0));
        }
    }
}
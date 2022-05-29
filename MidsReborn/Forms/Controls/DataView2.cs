using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using FontAwesome.Sharp;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Syncfusion.Windows.Forms.Tools;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DataView2 : UserControl
    {
        public delegate void UnlockEventHandler();
        public delegate void TabChangedEventHandler(int tabIndex);
        public delegate void FileModifiedEventHandler();
        public delegate void RefreshInfoEventHandler();

        public event UnlockEventHandler Unlock;
        public event TabChangedEventHandler TabChanged;
        public event FileModifiedEventHandler FileModified;
        public event RefreshInfoEventHandler RefreshInfo;
        
        #region Private enums & structs

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

        private struct TabsRendered
        {
            public bool Info;
            public InfoType InfoType;
            public bool Effects;
            public bool Totals;
            public bool Enhance;
            public bool Scales;

            public void Reset()
            {
                Info = false;
                InfoType = InfoType.Power;
                Effects = false;
                Totals = false;
                Enhance = false;
                Scales = false;
            }
        }

        private struct GridViewMouseEventInfo
        {
            public DataGridView Target;
            public Point Loc;
            public InfoType InfoType;
        }

        private struct TotalsPaneMouseEventInfo
        {
            public string ContainerControlName;
            public Point Loc;
        }

        private enum InfoType
        {
            Power,
            Enhancement
        }

        private struct CellData
        {
            public string Label;
            public string Value;
            public Color ValueColor;
            public string TooltipText;
        }

        private enum TotalsMiscEffectsType
        {
            Elusivity,
            DebuffResistances,
            MezResistances,
            MezProtection
        }

        private enum Tray
        {
            Main,
            Alt
        }

        #endregion

        #region Public enums & structs

        public enum BoostType
        {
            Reduction,
            Equal,
            Enhancement,
            Extra
        }

        #endregion

        private static IPower _basePower;
        private static IPower _enhancedPower;
        private static int HistoryIdx = -1;
        private bool NoLevel;
        private static PowerEntry BuildPowerEntry;
        private static I9Slot EnhSlot;
        private static int EnhLevel;
        private bool FreezeScalerCB;
        private FlipAnimator _flipAnimator;
        private TabsRendered _tabsRendered;
        private GridViewMouseEventInfo GridMouseOverEventLoc;
        private InfoType LayoutType;
        private bool SmallSize;

        private readonly TabControlAdv _tabControlAdv;

        private static readonly SKBitmap NewSlotBitmap = FlipAnimator.Bitmaps.CreateBitmap(@"Images\Newslot.png"); // ???

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

        public DataView2()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            InitializeComponent();

            _tabControlAdv = tabBox;
            _tabControlAdv.SelectedIndexChanged += tabBox_TabIndexChanged;

            _tabsRendered = new TabsRendered();
            _tabsRendered.Reset();

            dV2TotalsPane1L.BarHover += DvPaneMisc_BarHover;
            dV2TotalsPane1R.BarHover += DvPaneMisc_BarHover;
            dV2TotalsPane2L.BarHover += DvPaneMisc_BarHover;
            dV2TotalsPane2R.BarHover += DvPaneMisc_BarHover;

            dV2TotalsPane3L.MouseClick += DvPaneMisc_MouseClick;
            dV2TotalsPane3L.BarHover += DvPaneMisc_BarHover;

            dV2TotalsPane3R.MouseClick += DvPaneMisc_MouseClick;
            dV2TotalsPane3R.BarHover += DvPaneMisc_BarHover;
        }

        // Set data for power
        public void SetData(IPower enhancedPower = null, bool noLevel = false,
            bool locked = false, int historyIdx = -1)
        {
            Locked = locked;
            if (Locked)
            {
                SetLockStatus();

                return;
            }

            if ((enhancedPower?.PowerIndex ?? -1) == (_enhancedPower?.PowerIndex ?? -1) & LayoutType == InfoType.Power)
            {
                return;
            }

            _enhancedPower = enhancedPower;
            NoLevel = noLevel;
            HistoryIdx = historyIdx;
            BuildPowerEntry = HistoryIdx > -1
                ? MidsContext.Character.CurrentBuild.Powers[HistoryIdx]
                : null;
            LayoutType = InfoType.Power;
            _basePower = _enhancedPower == null ? null : DatabaseAPI.Database.Power[_enhancedPower.PowerIndex];
            _flipAnimator = new FlipAnimator(BuildPowerEntry);

            Tabs.RenderTabs(this);
        }

        // Set data for enhancement
        public void SetData(I9Slot enh, int level = -1)
        {
            if (Locked)
            {
                return;
            }

            LayoutType = InfoType.Enhancement;
            EnhSlot = enh;
            EnhLevel = level;

            Tabs.RenderTabs(this);
        }

        // Only update current data
        // E.g. when clicking on main UI buttons
        public void UpdateData()
        {
            Tabs.RenderTabs(this);
        }

        public void Lock()
        {
            Locked = true;
            SetLockStatus();
        }

        public void UpdateColorTheme()
        {
            if (_tabControlAdv.SelectedIndex != 0) return;

            if (MidsContext.Character.IsHero())
            {
                tabPageAdv1.BackColor = Color.FromArgb(12, 56, 100);
                tabPageAdv1.TabBackColor = Color.FromArgb(12, 56, 100);
                _tabControlAdv.ActiveTabColor = Color.FromArgb(12, 56, 100);
                _tabControlAdv.InactiveTabColor = Color.FromArgb(7, 33, 59);
            }
            else
            {
                tabPageAdv1.BackColor = Color.FromArgb(100, 12, 20);
                tabPageAdv1.TabBackColor = Color.FromArgb(100, 12, 20);
                _tabControlAdv.ActiveTabColor = Color.FromArgb(100, 12, 20);
                _tabControlAdv.InactiveTabColor = Color.FromArgb(59, 7, 12);
            }
        }

        private void SetLockStatus()
        {
            if (_tabsRendered.Effects)
            {
                ipbLock2.Visible = true;
            }
            else if (_tabsRendered.Totals)
            {
                ipbLock3.Visible = true;
            }
            else if (_tabsRendered.Enhance)
            {
                ipbLock4.Visible = true;
            }
            else if (_tabsRendered.Scales)
            {
                ipbLock5.Visible = true;
            }
            else
            {
                ipbLock.Visible = true;
            }
        }

        public void ReInit()
        {
            richInfoSmall.Text = string.Empty;
            richInfoLarge.Text = string.Empty;
            GridMouseOverEventLoc = new GridViewMouseEventInfo
            {
                Target = listInfos,
                Loc = new Point(-1, -1),
                InfoType = InfoType.Power
            };

            skDamageGraph1.LockDraw();
            skDamageGraph1.nBaseVal = 0;
            skDamageGraph1.nMaxEnhVal = 0;
            skDamageGraph1.nEnhVal = 0;
            skDamageGraph1.Text = string.Empty;
            skDamageGraph1.UnlockDraw();

            Tabs.Totals.PaneMouseEventInfo = new TotalsPaneMouseEventInfo
            {
                ContainerControlName = "",
                Loc = new Point(-1, -1)
            };
        }

        public void UpdateDamageGraphSettings()
        {
            Tabs.Info.UpdateDamageGraphSettings();
        }

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
            // +Resistances to Self
            new List<EffectVectorType>
            {
                new(Enums.eEffectType.Resistance, BuffEffectType.Buff, Enums.eToWho.Self)
            },

            // +Defenses to Self
            new List<EffectVectorType>
            {
                new(Enums.eEffectType.Defense, BuffEffectType.Buff, Enums.eToWho.Self)
            },

            // Buffs
            new List<EffectVectorType>
            {
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
            private List<string> GrantPowers = new();
            private List<string> Summons = new();
            private readonly Enums.eEffectType EffectType;
            private readonly Enums.eEffectType SubEffectType;
            private readonly float BuffedMag;
            private readonly Enums.eToWho ToWho;
            private readonly bool DisplayPercentage;
            private readonly string Summon;

            public GroupedEffect(Enums.eEffectType effectType, float buffedMag, bool displayPercentage,
                Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = Enums.eEffectType.None;
                BuffedMag = buffedMag;
                DisplayPercentage = displayPercentage;
                ToWho = toWho;
                Summon = string.Empty;
            }

            public GroupedEffect(Enums.eEffectType effectType, Enums.eEffectType subEffectType, float buffedMag,
                bool displayPercentage, Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = subEffectType;
                BuffedMag = buffedMag;
                DisplayPercentage = displayPercentage;
                ToWho = toWho;
                Summon = string.Empty;
            }

            public GroupedEffect(Enums.eEffectType effectType, string summonedEntity, Enums.eToWho toWho)
            {
                EffectType = effectType;
                SubEffectType = Enums.eEffectType.None;
                BuffedMag = 1;
                DisplayPercentage = false;
                ToWho = toWho;
                Summon = summonedEntity;
            }

            public void AddGrant(string grantedPower)
            {
                GrantPowers.Add(grantedPower);
            }

            public void AddSummon(string summonedEntity)
            {
                Summons.Add(summonedEntity);
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

            public string GetGrants(bool multiLine = false)
            {
                return multiLine
                    ? string.Join("\r\n", GrantPowers)
                    : string.Join(", ", GrantPowers);
            }

            public string GetSummons(bool multiLine = false)
            {
                return multiLine
                    ? string.Join("\r\n", Summons)
                    : string.Join(", ", Summons);
            }

            private bool ContainsMulti<T>(IEnumerable<T> items, ICollection<T> baseList)
            {
                return items.All(baseList.Contains);
            }

            private bool ContainsMultiOnly<T>(IEnumerable<T> items, IEnumerable<T> baseList)
            {
                var refList = baseList.ToList();
                var itemsList = items.ToList();

                return itemsList.Count == refList.Count && refList.Except(itemsList).Any();
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

                        if (ContainsMultiOnly(damageTypes.GetRange(8, 3), DamageTypes))
                        {
                            return "All Pos";
                        }

                        return ContainsMultiOnly(damageTypes.GetRange(0, 7).Union(damageTypes.GetRange(8, 3)), DamageTypes)
                            ? "All"
                            : string.Join(", ", DamageTypes);

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
                    Enums.eToWho.Self => $"{(addSpace ? " " : "")}(Slf)",
                    Enums.eToWho.Target => $"{(addSpace ? " " : "")}(Tgt)",
                    _ => ""
                };
            }

            public Enums.eToWho GetToWho()
            {
                return ToWho;
            }

            public string GetSummon()
            {
                return Summon;
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

            public string GetStatName(bool longFormat = false)
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
                            return $"{EffectType} to {SubEffectType}{(longFormat ? $" to {ToWho}" : "")}";

                        default:
                            return $"{EffectType} to {ETModifiesString()}{(longFormat ? $" to {ToWho}" : "")}";
                    }
                }

                switch (EffectType)
                {
                    case Enums.eEffectType.Mez:
                    case Enums.eEffectType.MezResist:
                        return $"{EffectType}({MezTypesString()}){(longFormat ? $" to {ToWho}" : "")}";

                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                        return $"{EffectType}({DamageTypesString()}){(longFormat ? $" to {ToWho}" : "")}";

                    default:
                        return $"{EffectType}{(longFormat ? $" to {ToWho}" : "")}";
                }
            }

            public float GetMag()
            {
                return BuffedMag;
            }

            public string GetMagString()
            {
                return EffectType == Enums.eEffectType.Enhancement
                    ? $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")}"
                    : $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")}";
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
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType} to {SubEffectType}({MezTypesString()}){ToWhoString()}";

                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Elusivity:
                            return
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType} to {SubEffectType}({DamageTypesString()}){ToWhoString()}";

                        default:
                            return
                                $"{(BuffedMag > 0 ? "+" : "")}{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType} to {ETModifiesString()}{ToWhoString()}";
                    }
                }

                switch (EffectType)
                {
                    case Enums.eEffectType.Mez:
                    case Enums.eEffectType.MezResist:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType}({MezTypesString()}){ToWhoString()}";

                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType}({DamageTypesString()}){ToWhoString()}";

                    default:
                        return
                            $"{(DisplayPercentage ? $"{BuffedMag:P2}" : $"{BuffedMag:###0.##}")} {EffectType}{ToWhoString()}";
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

            private static string GetSummonPowerName(IEffect fx)
            {
                var tPowerId = DatabaseAPI.GetPowerByFullName(fx.Summon);
                
                return tPowerId != null
                    ? tPowerId.DisplayName
                    : $" {fx.Summon}";
            }

            public static EffectsGroupFilter FromPower(IPower power)
            {
                var groups = new List<List<GroupedEffect>>();
                var groupsEffectTypes = new List<Dictionary<FxVectorIdentifier, int>>();
                var fxGroups = new List<List<IEffect>>();
                for (var i = 0; i < 6; i++)
                {
                    groups.Add(new List<GroupedEffect>());
                    groupsEffectTypes.Add(new Dictionary<FxVectorIdentifier, int>());
                    fxGroups.Add(new List<IEffect>());
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

                            switch (fx.EffectType)
                            {
                                case Enums.eEffectType.Enhancement:
                                case Enums.eEffectType.ResEffect:
                                    groups[i].Add(new GroupedEffect(fx.EffectType, fx.ETModifies, fx.BuffedMag, fx.DisplayPercentage, fx.ToWho));
                                    break;

                                case Enums.eEffectType.GrantPower:
                                case Enums.eEffectType.EntCreate:
                                    groups[i].Add(new GroupedEffect(fx.EffectType, GetSummonPowerName(fx), fx.ToWho));
                                    break;

                                default:
                                    groups[i].Add(new GroupedEffect(fx.EffectType, fx.BuffedMag, fx.DisplayPercentage, fx.ToWho));
                                    break;
                            }

                            switch (fx.EffectType)
                            {
                                case Enums.eEffectType.GrantPower:
                                    groups[i][groups[i].Count - 1].AddGrant(GetSummonPowerName(fx));
                                    break;

                                case Enums.eEffectType.EntCreate:
                                    groups[i][groups[i].Count - 1].AddSummon(GetSummonPowerName(fx));
                                    break;

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

        #region Flip animator sub-class (enhance tab)

        private class FlipAnimator
        {
            internal struct SKSlotBitmap
            {
                public SKBitmap Bitmap;
                public bool IsEmpty;
                public bool ValidSlot;
                public Enums.eEnhGrade EnhType;
            }

            internal enum IncarnateSlot
            {
                Alpha,
                Judgement,
                Interface,
                Destiny,
                Lore,
                Hybrid
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
                private const int EnhImgSize = 30;
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
                                0, 0, 0, 1, 0
                            })
                        };
                }

                public static SKImage DrawSingle(SKSlotBitmap enhMain, SKSlotBitmap enhAlt, float angleDeg)
                {
                    var surface = SKSurface.Create(new SKImageInfo(30, 30));
                    surface.Canvas.Clear(SKColors.Black);
                    var sourceRect = new SKRect(0, 0, 30, 30);
                    var destRect = new SKRect(
                        15 - 15 * (float) Math.Abs(Math.Cos(angleDeg / 180 * Math.PI)),
                        0,
                        15 + 15 * (float) Math.Abs(Math.Cos(angleDeg / 180 * Math.PI)),
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

                public static SKBitmap CreateBitmap(Bitmap sourceBitmap)
                {
                    return sourceBitmap.ToSKBitmap();
                }

                public static SKBitmap CreateBitmap(Image sourceImage)
                {
                    return CreateBitmap(new Bitmap(sourceImage));
                }

                public static string RelativeLevelString(Enums.eEnhRelative relativeLevel, bool showZero = false)
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

                public static SKBitmap CreateBitmap(int enhIndex, int ioLevel = -1,
                    Enums.eEnhRelative relativeLevel = Enums.eEnhRelative.Even)
                {
                    var bitmap = new SKBitmap(new SKImageInfo(EnhImgSize, EnhImgSize, SKColorType.Rgba8888, SKAlphaType.Premul)); // Format ?
                    using var canvas = new SKCanvas(bitmap);

                    var imgIdx = DatabaseAPI.Database.Enhancements[enhIndex].ImageIdx;
                    var enhGrade = I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[enhIndex].TypeID); // Enums.eEnhGrade ?
                    var sourceRect = I9Gfx.GetOverlayRect(enhGrade).ToSKRect();
                    var destRect = new SKRect(0, 0, EnhImgSize, EnhImgSize);

                    // Draw border
                    canvas.DrawBitmap(I9Gfx.Borders.Bitmap.ToSKBitmap(), sourceRect, destRect);

                    // Draw enhancement
                    canvas.DrawBitmap(I9Gfx.Enhancements[imgIdx].ToSKBitmap(), new SKRect(0, 0, EnhImgSize, EnhImgSize), destRect);

                    if (ioLevel == -1)
                    {
                        return bitmap;
                    }

                    // Draw enhancement level
                    canvas.DrawOutlineText($"{ioLevel + 1}{RelativeLevelString(relativeLevel)}", new SKPoint(15, 8), SKColors.Cyan, SKTextAlign.Center, 255, 11f);

                    return bitmap;
                }

                public static SKBitmap CreateBitmap(IncarnateSlot incarnateSlot)
                {
                    var bitmap = new SKBitmap(new SKImageInfo(EnhImgSize, EnhImgSize)); // SKColorType.Rgba8888, SKAlphaType.Premul
                    using var canvas = new SKCanvas(bitmap);
                    var imgIndex = incarnateSlot switch
                    {
                        IncarnateSlot.Alpha => 26,
                        IncarnateSlot.Destiny => 27,
                        IncarnateSlot.Hybrid => 28, 
                        IncarnateSlot.Interface => 29,
                        IncarnateSlot.Judgement => 30,
                        IncarnateSlot.Lore => 31,
                        _ => 0
                    };

                    if (imgIndex == 0)
                    {
                        return bitmap;
                    }

                    var imgSourceRect = I9Gfx.GetImageRect(imgIndex);
                    var sourceRect = new SKRect(imgSourceRect.Left, imgSourceRect.Top, imgSourceRect.Right, imgSourceRect.Bottom);
                    var destRect = new SKRect(0, 0, EnhImgSize, EnhImgSize);
                    canvas.DrawBitmap(I9Gfx.Classes.Bitmap.ToSKBitmap(), sourceRect, destRect);

                    return bitmap;
                }
            }

            public static class Utils
            {
                public static IncarnateSlot? GetIncarnateSlotFromPowerset(string powerset)
                {
                    if (powerset.StartsWith("Incarnate.Alpha"))
                    {
                        return IncarnateSlot.Alpha;
                    }

                    if (powerset.StartsWith("Incarnate.Judgement"))
                    {
                        return IncarnateSlot.Judgement;
                    }

                    if (powerset.StartsWith("Incarnate.Interface"))
                    {
                        return IncarnateSlot.Interface;
                    }

                    if (powerset.StartsWith("Incarnate.Lore"))
                    {
                        return IncarnateSlot.Lore;
                    }

                    if (powerset.StartsWith("Incarnate.Destiny"))
                    {
                        return IncarnateSlot.Destiny;
                    }

                    if (powerset.StartsWith("Incarnate.Hybrid"))
                    {
                        return IncarnateSlot.Hybrid;
                    }

                    return null;
                }
            }

            public FlipAnimator(PowerEntry buildPowerEntry)
            {
                if (buildPowerEntry == null) return;

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
                        ValidSlot = true,
                        EnhType = enhSlot.Grade
                    });

                    enhSlot = buildPowerEntry.Slots[i].FlippedEnhancement;
                    emptySlot = enhSlot.Enh < 0;
                    slotBitmap = emptySlot
                        ? NewSlotBitmap
                        : Bitmaps.CreateBitmap(enhSlot.Enh, enhSlot.IOLevel, enhSlot.RelativeLevel);
                    EnhAltBitmaps.Add(new SKSlotBitmap
                    {
                        Bitmap = slotBitmap,
                        IsEmpty = emptySlot,
                        ValidSlot = true,
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
                         i <= power.VariableMax;
                         i = variableRange < 10 ? ++i : (int) Math.Round(i + (decimal) variableRange / 10))
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

            public static class Utilities
            {
                public static SKPoint GetGraphCoordPoint(KeyValuePair<PowerStats.FxIdentifier, List<PowerStats.DataPoint>> series, float graphWidth, float scaleFactor, SKSize padding, float bottomLine, int variableValue)
                {
                    return new SKPoint(padding.Width + variableValue * (graphWidth - 2 * padding.Width) / (series.Value.Count - 1), bottomLine - scaleFactor * Math.Abs(series.Value[variableValue].Value));
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

            public static SKImage DrawScalesGraphSurface(DataView2 root, int historyIdx, int w, int h)
            {
                var padding = new SKSize(5, 5);
                const int gridCells = 6;

                var surface = SKSurface.Create(new SKImageInfo(w, h));
                surface.Canvas.Clear(SKColors.Black);

                if (Tabs.Scales.DataPoints.Keys.Count <= 0) return surface.Snapshot();

                // Grid

                var pathGrid = new SKPath();
                for (var i = 1; i < gridCells; i++)
                {
                    pathGrid.MoveTo(padding.Width + (w - 2 * padding.Width) / gridCells * i, padding.Height);
                    pathGrid.LineTo(padding.Width + (w - 2 * padding.Width) / gridCells * i, h - padding.Height);
                }

                for (var i = 1; i < gridCells; i++)
                {
                    pathGrid.MoveTo(padding.Width, padding.Height + (h - 2 * padding.Height) / gridCells * i);
                    pathGrid.LineTo(w - padding.Width, padding.Height + (h - 2 * padding.Height) / gridCells * i);
                }

                surface.Canvas.DrawPath(pathGrid, new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = new SKColor(130, 130, 130),
                    StrokeWidth = 1,
                    StrokeCap = SKStrokeCap.Butt,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 10)
                });

                // Axis

                var pathAxis = new SKPath();
                pathAxis.MoveTo(padding.Width, padding.Height);
                pathAxis.LineTo(padding.Width, h - padding.Height);
                pathAxis.LineTo(w - padding.Width, h - padding.Height);
                surface.Canvas.DrawPath(pathAxis, new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.WhiteSmoke,
                    StrokeWidth = 1,
                    StrokeCap = SKStrokeCap.Butt,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 4)
                });

                // Points/Lines

                const float displayScaleFactor = 0.9f;
                var bottomLine = h - padding.Height - (1 - displayScaleFactor) * (h - 2 * padding.Height);
                //var absoluteRange = PowerStats.GetValuesRange(dataPoints);
                var ranges = PowerStats.GetValuesRangeEach(Tabs.Scales.DataPoints);

                var scaleFactors = ranges.ToDictionary(range => range.Key,
                    range => Math.Abs(range.Value.Min - range.Value.Max) < float.Epsilon || Math.Max(range.Value.Max, range.Value.Min) < float.Epsilon
                        ? displayScaleFactor
                        : (bottomLine - padding.Height) / Math.Max(range.Value.Max, range.Value.Min));

                var seriesHasPercentage = ranges.ToDictionary(range => range.Key,
                    range => Math.Abs(range.Value.Min) <= 1 & Math.Abs(range.Value.Max) <= 1);

                foreach (var series in Tabs.Scales.DataPoints)
                {
                    var curveColor = GetGraphCurveColor(series.Key);
                    var path = new SKPath();
                    for (var i = 0; i < series.Value.Count; i++)
                    {
                        if (i == 0)
                        {
                            path.MoveTo(Utilities.GetGraphCoordPoint(series, w, scaleFactors[series.Key], padding, bottomLine, i));
                        }
                        else
                        {
                            path.LineTo(Utilities.GetGraphCoordPoint(series, w, scaleFactors[series.Key], padding, bottomLine, i));
                        }
                    }

                    surface.Canvas.DrawPath(path, new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        Color = curveColor,
                        StrokeWidth = 1,
                        StrokeCap = SKStrokeCap.Butt
                    });
                }

                // Current value - crosshairs + text
                var crosshairColor = new SKColor(245, 245, 245, 230);
                var variableValue = BuildPowerEntry.VariableValue;
                const float crosshairRadius = 8;
                const float crosshairRadiusInner = 2;
                const float closeNeighborRadius = 12;
                const float textOffset = 17;
                using var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.WhiteSmoke
                };

                var pointsOffsetList = new Dictionary<SKPoint, int>();
                foreach (var series in Tabs.Scales.DataPoints)
                {
                    var centerPoint = Utilities.GetGraphCoordPoint(series, w, scaleFactors[series.Key], padding, bottomLine, variableValue);
                    var closeNeighbor = (from s in Tabs.Scales.DataPoints
                        let centerPoint2 = Utilities.GetGraphCoordPoint(s, w, scaleFactors[s.Key], padding, bottomLine, variableValue)
                        let diff = centerPoint.Y - centerPoint2.Y
                        where Math.Abs(diff) <= closeNeighborRadius & !Equals(s.Key, series.Key)
                        select diff > 0 ? 1 : -1).FirstOrDefault();  // From above : From below

                    var path = new SKPath();
                    path.MoveTo(new SKPoint(centerPoint.X - crosshairRadius, centerPoint.Y));
                    path.LineTo(new SKPoint(centerPoint.X - crosshairRadiusInner, centerPoint.Y));
                    path.MoveTo(new SKPoint(centerPoint.X + crosshairRadius, centerPoint.Y));
                    path.LineTo(new SKPoint(centerPoint.X + crosshairRadiusInner, centerPoint.Y));
                    path.MoveTo(new SKPoint(centerPoint.X, centerPoint.Y - crosshairRadius));
                    path.LineTo(new SKPoint(centerPoint.X, centerPoint.Y - crosshairRadiusInner));
                    path.MoveTo(new SKPoint(centerPoint.X, centerPoint.Y + crosshairRadius));
                    path.LineTo(new SKPoint(centerPoint.X, centerPoint.Y + crosshairRadiusInner));

                    surface.Canvas.DrawPath(path, new SKPaint
                    {
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        Color = crosshairColor,
                        StrokeWidth = 1,
                        StrokeCap = SKStrokeCap.Butt
                    });

                    var textValue = seriesHasPercentage[series.Key]
                        ? $"{series.Value[variableValue].Value:P2}"
                        : $"{series.Value[variableValue].Value:###0.##}";

                    var textRect = new SKRect();
                    textPaint.MeasureText(textValue, ref textRect);
                    var textX = Math.Max(textRect.Width + 4, Math.Min(w - padding.Width - textRect.Width - 4, centerPoint.X - textRect.Width / 2f));
                    var textY = Math.Max(padding.Height + 6, centerPoint.Y - 6 + textOffset * closeNeighbor);
                    if (pointsOffsetList.ContainsKey(centerPoint))
                    {
                        pointsOffsetList[centerPoint]++;
                        textY += pointsOffsetList[centerPoint] * (textY < h / 2f ? textOffset : -textOffset);
                    }
                    else
                    {
                        pointsOffsetList.Add(centerPoint, 0);
                    }

                    surface.Canvas.DrawTextShort(textValue, 11, textX, textY, textPaint);
                }

                return surface.Snapshot();
            }
        }

        #endregion

        #region Abbreviate effects/mez sub-class

        private static class AbbreviateNames
        {
            public static string AbbreviateMez(Enums.eMez mezType)
            {
                switch (mezType)
                {
                    case Enums.eMez.None:
                    case Enums.eMez.Afraid:
                    case Enums.eMez.Avoid:
                    case Enums.eMez.Held:
                    case Enums.eMez.Repel:
                    case Enums.eMez.Taunt:
                    case Enums.eMez.Sleep:
                        return $"{mezType}";

                    case Enums.eMez.Confused:
                        return "Conf.";

                    case Enums.eMez.Immobilized:
                        return "Immob.";

                    case Enums.eMez.Knockback:
                        return "KBk";

                    case Enums.eMez.Knockup:
                        return "KUp";

                    case Enums.eMez.OnlyAffectsSelf:
                        return "OnlySlf";

                    case Enums.eMez.Placate:
                        return "Plact";

                    case Enums.eMez.Stunned:
                        return "Stun";

                    case Enums.eMez.Terrorized:
                        return "Fear";

                    case Enums.eMez.Untouchable:
                        return "Untch.";

                    case Enums.eMez.Teleport:
                        return "TP";

                    case Enums.eMez.ToggleDrop:
                        return "TglDrp";

                    case Enums.eMez.CombatPhase:
                        return "Phase";

                    case Enums.eMez.Intangible:
                        return "Intangb.";

                    default:
                        return "";
                }
            }

            public static string AbbreviateFx(Enums.eEffectType effectType)
            {
                var index = (int) effectType;

                return $"{(Enums.eEffectTypeShort) index}";
            }
        }

        #endregion

        #region Power stats sub-class

        private class PowerStats
        {
            public enum BuffType
            {
                Buff,
                Debuff,
                Any,
                Mez
            }

            public struct BuffInfo
            {
                public float Value;
                public float ValueAfterED;
                public Enums.eSchedule Schedule;
            }

            public struct BuffStat
            {
                public Enums.eEnhance Stat;
                public Enums.eMez Mez;
                public BuffType BuffType;
            }

            private Dictionary<BuffStat, BuffInfo> _powerStats;

            public PowerStats()
            {
                _powerStats = new Dictionary<BuffStat, BuffInfo>();
            }

            public void AddUpdate(BuffStat buffKey, BuffInfo buffInfo, bool sum = true)
            {
                if (!_powerStats.ContainsKey(buffKey))
                {
                    _powerStats.Add(buffKey, buffInfo);
                }
                else
                {
                    _powerStats[buffKey] = new BuffInfo
                    {
                        Schedule = buffInfo.Schedule,
                        Value = sum ? _powerStats[buffKey].Value + buffInfo.Value : buffInfo.Value,
                        ValueAfterED = sum ? _powerStats[buffKey].ValueAfterED + buffInfo.ValueAfterED : buffInfo.ValueAfterED
                    };
                }
            }

            public float? GetValue(Enums.eEnhance stat, Enums.eMez mez, BuffType buffType, bool afterED = true)
            {
                var buffKey = new BuffStat
                {
                    BuffType = buffType,
                    Mez = mez,
                    Stat = stat
                };

                if (!_powerStats.ContainsKey(buffKey))
                {
                    return null;
                }

                if (afterED & _powerStats[buffKey].Value > 0 & _powerStats[buffKey].ValueAfterED == 0)
                {
                    var valueAfterED = Enhancement.ApplyED(_powerStats[buffKey].Schedule, _powerStats[buffKey].Value);
                    AddUpdate(buffKey,
                        new BuffInfo
                        {
                            Schedule = _powerStats[buffKey].Schedule,
                            Value = _powerStats[buffKey].Value,
                            ValueAfterED = Enhancement.ApplyED(_powerStats[buffKey].Schedule, _powerStats[buffKey].Value)
                        },
                        false);

                        return valueAfterED;
                }

                return afterED ? _powerStats[buffKey].ValueAfterED : _powerStats[buffKey].Value;
            }
        }

        #endregion

        #region Tab renderers sub-class

        private static class Tabs
        {
            public static Color InterpolateColor(decimal value, decimal valueMin, decimal valueMax, ColorRange colorRange)
            {
                return Color.FromArgb(
                    (int) Math.Round(
                        (value - valueMin) / (valueMax - valueMin) *
                        (colorRange.UpperBoundColor.R - colorRange.LowerBoundColor.R) + colorRange.LowerBoundColor.R),
                    (int) Math.Round(
                        (value - valueMin) / (valueMax - valueMin) *
                        (colorRange.UpperBoundColor.G - colorRange.LowerBoundColor.G) + colorRange.LowerBoundColor.G),
                    (int) Math.Round(
                        (value - valueMin) / (valueMax - valueMin) *
                        (colorRange.UpperBoundColor.B - colorRange.LowerBoundColor.B) + colorRange.LowerBoundColor.B)
                );
            }

            private static void SetTextBoxVScrollVisibility(RichTextBox textBox)
            {
                var textBoxRect = TextRenderer.MeasureText(textBox.Text, textBox.Font, new Size(textBox.Width, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

                textBox.ScrollBars = textBoxRect.Height - textBox.Font.Size > textBox.Height
                    ? RichTextBoxScrollBars.Vertical
                    : RichTextBoxScrollBars.None;
            }

            public static Dictionary<int, int> SlottedSets()
            {
                var ret = new Dictionary<int, int>();
                if (BuildPowerEntry == null)
                {
                    return ret;
                }

                if (BuildPowerEntry.Power == null)
                {
                    return ret;
                }

                
                for (var i = 0; i < BuildPowerEntry.Slots.Length; i++)
                {
                    if (BuildPowerEntry.Slots[i].Enhancement.Enh < 0)
                    {
                        continue;
                    }

                    var enh = DatabaseAPI.Database.Enhancements[BuildPowerEntry.Slots[i].Enhancement.Enh];
                    if (enh.TypeID != Enums.eType.SetO)
                    {
                        continue;
                    }

                    var setId = enh.nIDSet;
                    if (!ret.ContainsKey(setId))
                    {
                        ret.Add(setId, 1);
                    }
                    else
                    {
                        ret[setId]++;
                    }
                }

                return ret;
            }

            public static void RenderTabs(DataView2 root, bool checkActive = false)
            {
                var columnsLayout = root.LayoutType switch
                {
                    InfoType.Enhancement when EnhSlot.Enh > -1 && DatabaseAPI.Database.Enhancements[EnhSlot.Enh].TypeID == Enums.eType.SetO => new[] { 20, 331, 0, 0 }, // Simulate 2 columns layout
                    InfoType.Enhancement => new[] { 150, 201, 0, 0},
                    _ => new[] { 97, 78, 98, 78 }
                };

                for (var i = 0; i < root.listInfos.Columns.Count; i++)
                {
                    root.listInfos.Columns[i].Width = columnsLayout[i];
                }

                if (!checkActive)
                {
                    root._tabsRendered.Reset();

                    switch (root._tabControlAdv.SelectedIndex)
                    {
                        case 0:
                            Info.Render(root, root.LayoutType);
                            root._tabsRendered.Info = true;
                            root._tabsRendered.InfoType = root.LayoutType;
                            break;

                        case 1:
                            Effects.Render(root, root.LayoutType);
                            root._tabsRendered.Effects = true;
                            break;

                        case 2:
                            Totals.Render(root, root.LayoutType);
                            root._tabsRendered.Totals = true;
                            break;

                        case 3:
                            Enhance.Render(root, root.LayoutType);
                            root._tabsRendered.Enhance = true;
                            break;

                        case 4:
                            Scales.Render(root, root.LayoutType);
                            root._tabsRendered.Scales = true;
                            break;
                    }
                }
                else
                {
                    switch (root._tabControlAdv.SelectedIndex)
                    {
                        case 0:
                            if (!root._tabsRendered.Info | root._tabsRendered.InfoType != root.LayoutType)
                            {
                                Info.Render(root, root.LayoutType);
                                root._tabsRendered.Info = true;
                                root._tabsRendered.InfoType = root.LayoutType;
                            }
                            break;

                        case 1:
                            if (!root._tabsRendered.Effects)
                            {
                                Effects.Render(root, root.LayoutType);
                                root._tabsRendered.Effects = true;
                            }
                            break;

                        case 2:
                            if (!root._tabsRendered.Totals)
                            {
                                Totals.Render(root, root.LayoutType);
                                root._tabsRendered.Totals = true;
                            }
                            break;

                        case 3:
                            if (!root._tabsRendered.Enhance)
                            {
                                Enhance.Render(root, root.LayoutType);
                                root._tabsRendered.Enhance = true;
                            }
                            break;

                        case 4:
                            if (!root._tabsRendered.Scales)
                            {
                                Scales.Render(root, root.LayoutType);
                                root._tabsRendered.Scales = true;
                            }
                            break;
                    }
                }
            }

            public static class RTFText
            {
                public static string Text2RTF(string s)
                {
                    return $"{RTF.StartRTF()}{s.Replace("\r\n", "\n").Replace("\n", RTF.Crlf())}{RTF.EndRTF()}";
                }

                public static string List2RTF(List<string> ls)
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

                public static string ConvertNewlinesToRTF(string str)
                {
                    return str
                        .Replace("\r\n", "\n")
                        .Replace("\r", "\n")
                        .Replace("\n", RTF.Crlf());
                }

                public static string GetEnhancementStringLongRTF(I9Slot enhSlot)
                {
                    var str = enhSlot.GetEnhancementStringLong();
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = $"{RTF.Color(RTF.ElementID.Enhancement)}{RTF.Italic(ConvertNewlinesToRTF(str))}{RTF.Color(RTF.ElementID.Text)}";
                    }

                    return str;
                }

                public static string GetEnhancementStringRTF(I9Slot enhSlot)
                {
                    var str = enhSlot.GetEnhancementString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = $"{RTF.Color(RTF.ElementID.Enhancement)}{ConvertNewlinesToRTF(str)}{RTF.Color(RTF.ElementID.Text)}";
                    }

                    return str;
                }
            }

            public static class Boosts
            {
                public static BoostType GetBoostType(float valueBase, float valueEnhanced)
                {
                    var diff = valueEnhanced - valueBase;

                    return diff switch
                    {
                        < 0 => BoostType.Reduction,
                        > 0 => BoostType.Enhancement,
                        _ => BoostType.Equal
                    };
                }

                // Invert on means enhanced value is better when lower than base.
                // E.g. Endurance cost, cast time, recharge time
                public static Color GetBoostColor(float baseValue, float enhancedValue, bool invert = false)
                {
                    switch (GetBoostType(baseValue, enhancedValue))
                    {
                        case BoostType.Reduction when !invert:
                        case BoostType.Enhancement when invert:
                            return Color.FromArgb(255, 20, 20);

                        case BoostType.Enhancement:
                        case BoostType.Reduction:
                            return Color.FromArgb(0, 240, 80);

                        case BoostType.Extra:
                            return Color.FromArgb(0, 220, 220);

                        default:
                            return Color.WhiteSmoke;
                    }
                }

                public static Color GetBoostColor(BoostType boostType, bool invert = false)
                {
                    switch (boostType)
                    {
                        case BoostType.Reduction when !invert:
                        case BoostType.Enhancement when invert:
                            return Color.FromArgb(255, 20, 20);

                        case BoostType.Enhancement:
                        case BoostType.Reduction:
                            return Color.FromArgb(0, 240, 80);

                        case BoostType.Extra:
                            return Color.FromArgb(0, 220, 220);

                        default:
                            return Color.WhiteSmoke;
                    }
                }
            }

            public static class Info
            {
                private static DataView2 Root;
                private static InfoType LayoutType;
                private static Dictionary<int, int> SlottedSets;

                public static void Render(DataView2 root, InfoType layoutType)
                {
                    Root = root;
                    LayoutType = layoutType;
                    SlottedSets = SlottedSets();
                    Root.ipbLock.Visible = Root.Locked;
                    Root.ipbResize.IconChar = Root.SmallSize
                        ? IconChar.ChevronDown
                        : IconChar.ChevronUp;

                    UpdateDamageGraphSettings();
                    if (LayoutType == InfoType.Power)
                    {
                        PowerInfo();
                    }
                    else if (layoutType == InfoType.Enhancement)
                    {
                        EnhancementInfo();
                    }
                }

                public static void UpdateDamageGraphSettings()
                {
                    // Damage graph colors don't stick in the designer.
                    // Set them here instead.

                    // Test color set:
                    // var baseDamageColor = Color.FromArgb(51, 221, 122);
                    // var enhDamageColor = Color.FromArgb(197, 32, 32);
                    const float darkLumFactor = 0.45f;
                    var baseDamageColor = MidsContext.Config.RtFont.ColorDamageBarBase;
                    var enhDamageColor = MidsContext.Config.RtFont.ColorDamageBarEnh;
                    var baseDamageColorDark = MultiplyLum(baseDamageColor, darkLumFactor);
                    var enhDamageColorDark = MultiplyLum(enhDamageColor, darkLumFactor);

                    Root.skDamageGraph1.LockDraw();
                    Root.skDamageGraph1.ColorBackStart = Color.Black;
                    Root.skDamageGraph1.ColorBackEnd = Color.FromArgb(64, 0, 0); // Move to config ?
                    Root.skDamageGraph1.ColorBaseStart = baseDamageColorDark;
                    Root.skDamageGraph1.ColorBaseEnd = baseDamageColor;
                    Root.skDamageGraph1.ColorEnhStart = enhDamageColorDark;
                    Root.skDamageGraph1.ColorEnhEnd = enhDamageColor;
                    Root.skDamageGraph1.UnlockDraw();
                }

                // Change color luminosity
                private static Color MultiplyLum(Color c, float factor)
                {
                    var skc = c.ToSKColor();
                    skc.ToHsl(out var h, out var s, out var l);
                    l *= factor;

                    var ret = SKColor.FromHsl(h, s, l);

                    return Color.FromArgb(c.A, ret.Red, ret.Green, ret.Blue);
                }

                private static void PowerInfo()
                {
                    var powerStats = GetSlottedEnhancementEffects();

                    Root.lblDamage.Visible = true;
                    Root.skDamageGraph1.Visible = true;
                    Root.listSpecialBonuses.Visible = false;

                    Root.infoTabTitle.Invalidate();
                    Root.richInfoSmall.Rtf = RTFText.Text2RTF(_basePower?.DescShort ?? "");
                    Root.richInfoLarge.Rtf = RTFText.Text2RTF(_basePower?.DescLong ?? "");

                    if (_basePower == null) return;

                    Root.listInfos.Rows.Clear();
                    for (var i = 0; i < 5; i++)
                    {
                        Root.listInfos.Rows.Add();
                        Root.listInfos.Rows[i].Height = 20;
                        Root.listInfos.SetCellContent(i, 0);
                        Root.listInfos.SetCellContent(i, 1);
                        Root.listInfos.SetCellContent(i, 2);
                        Root.listInfos.SetCellContent(i, 3);
                    }

                    // Basic power attributes
                    var cellsData = new List<CellData>();
                    
                    var endCost = _enhancedPower.EndCost < float.Epsilon
                        ? "Free"
                        : _enhancedPower.PowerType == Enums.ePowerType.Toggle
                            ? $"{_enhancedPower.EndCost / _enhancedPower.ActivatePeriod:###0.##}/s"
                            : $"{_enhancedPower.EndCost:###0.##}";

                    var endCostTooltip = _enhancedPower.PowerType == Enums.ePowerType.Toggle & _enhancedPower.EndCost > 0
                        ? $"End cost per tick: {_enhancedPower.EndCost:###0.##}\r\nActivation period: {_enhancedPower.ActivatePeriod:#####0.##}s{(_enhancedPower.ActivatePeriod > 0 & Math.Abs(_enhancedPower.ActivatePeriod - 1) > float.Epsilon ? $" ({1 / _enhancedPower.ActivatePeriod:##0.#} ticks/s)" : "")}"
                        : "";

                    cellsData.Add(new CellData
                    {
                        Label = "End Cost:",
                        TooltipText = endCostTooltip,
                        Value = endCost,
                        ValueColor = Boosts.GetBoostColor(_basePower.EndCost, _enhancedPower.EndCost, true)
                    });

                    var enhancedPowerAcc = _enhancedPower.Accuracy * MidsContext.Config.BaseAcc;
                    var basePowerAcc = _basePower.Accuracy * MidsContext.Config.BaseAcc;
                    var totalToHit = MidsContext.Config.BaseAcc + MidsContext.Character.DisplayStats.BuffToHit / 100;
                    var powerAccuracyBuff = powerStats.GetValue(Enums.eEnhance.Accuracy, Enums.eMez.None, PowerStats.BuffType.Any);
                    var totalAccuracy = _basePower.AccuracyMult * (1 + (powerAccuracyBuff ?? 0) + MidsContext.Character.DisplayStats.BuffAccuracy / 100);
                    var accuracyTooltip = $"Chance to Hit: {enhancedPowerAcc:P2}\r\nBase Chance: {basePowerAcc:P2}\r\n\r\nTotal Accuracy: {totalAccuracy:P2}\r\nTotal ToHit: {totalToHit:P2}";
                    cellsData.Add(new CellData
                    {
                        Label = "Accuracy:",
                        TooltipText = accuracyTooltip,
                        Value = $"{enhancedPowerAcc:P2}",
                        ValueColor = Boosts.GetBoostColor(basePowerAcc, enhancedPowerAcc)
                    });

                    var castTimeTooltip = $"Cast Time: {_enhancedPower.CastTime:#####0.##}s\r\nArcana Time: {(Math.Ceiling(_enhancedPower.CastTime / 0.132) + 1) * 0.132}s";
                    cellsData.Add(new CellData
                    {
                        Label = "Cast Time:",
                        TooltipText = castTimeTooltip,
                        Value = $"{(_enhancedPower.CastTime == 0 ? "Instant" : $"{_enhancedPower.CastTime:#####0.##}s")}",
                        ValueColor = Boosts.GetBoostColor(_basePower.CastTime, _enhancedPower.CastTime, true)
                    });

                    cellsData.Add(new CellData
                    {
                        Label = "Recharge:",
                        TooltipText = "",
                        Value = $"{(_enhancedPower.RechargeTime == 0 ? "Instant" : $"{_enhancedPower.RechargeTime:#####0.##}s")}",
                        ValueColor = Boosts.GetBoostColor(_basePower.RechargeTime, _enhancedPower.RechargeTime, true)
                    });

                    var targetType = "";
                    var targetTypeTooltip = "";
                    if ((_enhancedPower.EntitiesAffected & Enums.eEntity.Friend) > 0 ||
                        (_enhancedPower.EntitiesAffected & Enums.eEntity.Teammate) > 0 ||
                        (_enhancedPower.EntitiesAffected & Enums.eEntity.DeadFriend) > 0 ||
                        (_enhancedPower.EntitiesAffected & Enums.eEntity.DeadTeammate) > 0)
                    {
                        targetType = "Ally";
                        targetTypeTooltip = "This power buffs allies.";
                    }
                    else if ((_enhancedPower.EntitiesAffected & Enums.eEntity.MyPet) > 0 ||
                             (_enhancedPower.EntitiesAffected & Enums.eEntity.DeadMyPet) > 0)
                    {
                        targetType = "Pet";
                        targetTypeTooltip = "This power affects caster's pets.";
                    }
                    else if (_enhancedPower.Effects.All(e => e.ToWho == Enums.eToWho.Self))
                    {
                        targetType = "Self";
                        targetTypeTooltip = "This power affects caster only.";
                    }
                    else if (_enhancedPower.Effects.All(e => e.ToWho == Enums.eToWho.Target))
                    {
                        targetType = "Foe";
                        targetTypeTooltip = "This power affects enemies only.";
                    }
                    else
                    {
                        targetType = "Multi";
                        targetTypeTooltip = "This power has multiple effects on caster and foes, possibly teammates.\nCheck the Effects tab for details.";
                    }

                    cellsData.Add(new CellData
                    {
                        Label = "Target:",
                        TooltipText = targetTypeTooltip,
                        Value = targetType,
                        ValueColor = Boosts.GetBoostColor(BoostType.Equal)
                    });

                    if (_enhancedPower.ActivatePeriod > 0)
                    {
                        var baseActivatePeriod = _enhancedPower.ActivatePeriod > 0 & _basePower.ActivatePeriod == 0
                            ? _enhancedPower.ActivatePeriod
                            : _basePower.ActivatePeriod;
                        cellsData.Add(new CellData
                        {
                            Label = "Act. Period:",
                            TooltipText = $"{(Math.Abs(_enhancedPower.ActivatePeriod - 1) > float.Epsilon ? $"Activation period: {_enhancedPower.ActivatePeriod:#####0.##}s ({1 / _enhancedPower.ActivatePeriod:##0.#} ticks/s)" : "")}",
                            Value = $"{_enhancedPower.ActivatePeriod:#####0.##}s",
                            ValueColor = Boosts.GetBoostColor(baseActivatePeriod, _enhancedPower.ActivatePeriod, true)
                        });
                    }

                    if (_enhancedPower.MaxTargets > 1)
                    {
                        var baseMaxTargets = _enhancedPower.MaxTargets > 0 & _basePower.MaxTargets == 0
                            ? _enhancedPower.MaxTargets
                            : _basePower.MaxTargets;
                        cellsData.Add(new CellData
                        {
                            Label = "Max Targets:",
                            TooltipText = "",
                            Value = $"{_enhancedPower.MaxTargets}",
                            ValueColor = Boosts.GetBoostColor(baseMaxTargets, _enhancedPower.MaxTargets)
                        });
                    }

                    if (_enhancedPower.Range > 0)
                    {
                        cellsData.Add(new CellData
                        {
                            Label = "Range:",
                            TooltipText = "",
                            Value = $"{_enhancedPower.Range:###0.##}ft",
                            ValueColor = Boosts.GetBoostColor(_basePower.Range, _enhancedPower.Range)
                        });
                    }

                    if (_enhancedPower.Radius > 0)
                    {
                        cellsData.Add(new CellData
                        {
                            Label = "Radius:",
                            TooltipText = "",
                            Value = $"{_enhancedPower.Radius:###0.##}ft",
                            ValueColor = Boosts.GetBoostColor(_basePower.Radius, _enhancedPower.Radius)
                        });
                    }

                    if (_enhancedPower.Arc > 0)
                    {
                        cellsData.Add(new CellData
                        {
                            Label = "Arc:",
                            TooltipText = "",
                            Value = $"{_enhancedPower.Arc:##0.##}deg",
                            ValueColor = Boosts.GetBoostColor(_basePower.Arc, _enhancedPower.Arc)
                        });
                    }

                    for (var i = 0; i < cellsData.Count; i++)
                    {
                        var colOffset = i % 2 * 2;
                        var row = (int) Math.Floor((decimal)i / 2);
                        Root.listInfos.SetCellContent(cellsData[i].Label, cellsData[i].TooltipText, row, colOffset);
                        Root.listInfos.SetCellContent(cellsData[i].Value, cellsData[i].ValueColor, cellsData[i].TooltipText, row, colOffset + 1);
                    }
                    
                    // Damage graph
                    var baseDamage = _basePower.FXGetDamageValue(true);
                    var enhancedDamage = _enhancedPower.FXGetDamageValue();
                    var dmgType = "Damage" + MidsContext.Config.DamageMath.ReturnValue switch
                    {
                        ConfigData.EDamageReturn.DPS => " Per Second",
                        ConfigData.EDamageReturn.DPA => " Per Animation",
                        _ => ""
                    };

                    dmgType += $"{(MidsContext.Config.DataDamageGraphPercentageOnly ? " (% only)" : "")}:"; // ???
                    Root.lblDamage.Text = dmgType;

                    // ???
                    // See ESD Arrow with procs
                    if (_basePower.NIDSubPower.Length > 0 & Math.Abs(baseDamage) < float.Epsilon & Math.Abs(enhancedDamage) < float.Epsilon)
                    {
                        Debug.WriteLine($"Set empty damage graph for power {_enhancedPower.FullName}");
                        Root.skDamageGraph1.LockDraw();
                        Root.skDamageGraph1.nBaseVal = 0;
                        Root.skDamageGraph1.nMaxEnhVal = 0;
                        Root.skDamageGraph1.nEnhVal = 0;
                        Root.skDamageGraph1.Text = string.Empty;
                        Root.skDamageGraph1.UnlockDraw();
                    }
                    else
                    {
                        Debug.WriteLine($"baseDamage: {baseDamage}, maxEnhVal: {baseDamage * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f))}, enhancedDamage: {enhancedDamage}");
                        Root.skDamageGraph1.LockDraw();
                        Root.skDamageGraph1.nBaseVal = baseDamage; // Math.Max(0, baseDamage) ? (see Toxins)
                        Root.skDamageGraph1.nMaxEnhVal = Math.Max(baseDamage * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f)), enhancedDamage); // ???
                        Root.skDamageGraph1.nEnhVal = enhancedDamage; // Math.Max(0, enhancedDamage ? (see Toxins)
                        Root.skDamageGraph1.nHighEnh = Math.Max(414, enhancedDamage); // Maximum graph value
                        Root.skDamageGraph1.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                            ? @$"{_enhancedPower.FXGetDamageString()} ({Utilities.FixDP(baseDamage)})"
                            : _basePower.FXGetDamageString(true);
                        Root.skDamageGraph1.UnlockDraw();
                    }
                }

                private static void EnhancementInfo()
                {
                    if (Root.Locked || EnhLevel < 0)
                    {
                        return;
                    }

                    Root.lblDamage.Visible = false;
                    Root.skDamageGraph1.Visible = false;
                    Root.listSpecialBonuses.Visible = true;
                    Root.listInfos.Rows.Clear();

                    var enhDesc = "";
                    //var enhName = "";
                    if (EnhSlot.Enh > -1)
                    {
                        enhDesc = DatabaseAPI.Database.Enhancements[EnhSlot.Enh].LongName;
                        if ((enhDesc.Length > 38) & (EnhLevel > -1))
                        {
                            enhDesc = DatabaseAPI.GetEnhancementNameShortWSet(EnhSlot.Enh);
                        }

                        //enhName = enhDesc;
                    }
                    else
                    {
                        //enhName = enhDesc;
                        // _basePower may be wrong or undefined
                        enhDesc = _basePower != null ? _basePower.DisplayName : "";
                        Root.richInfoSmall.Rtf = $"{RTF.StartRTF()}{(_basePower != null ? $"{_basePower.DescShort}\r\n" : "")}{RTF.Color(RTF.ElementID.Faded)}Shift+Click to move slot. Right-Click to place enh.{RTF.EndRTF()}";
                    }

                    if (!MidsContext.Config.ShowSlotLevels)
                    {
                        enhDesc += $" (Slot Level {EnhLevel + 1})";
                    }

                    //Root.infoTabTitle.Text = enhName;
                    Root.infoTabTitle.Invalidate();
                    if (EnhSlot.Enh < 0)
                    {
                        return;
                    }

                    Root.richInfoSmall.Text = enhDesc;

                    var shortDescRtf = "";
                    var longDescRtf = "";
                    var procChance = "";
                    var dbEnh = DatabaseAPI.Database.Enhancements[EnhSlot.Enh];
                    var typeId = dbEnh.TypeID;

                    if (typeId == Enums.eType.InventO | typeId == Enums.eType.SetO)
                    {
                        shortDescRtf = $"{RTF.Color(RTF.ElementID.Invention)}Invention Level: {EnhSlot.IOLevel + 1}{Enums.GetRelativeString(EnhSlot.RelativeLevel, false)}{RTF.Color(RTF.ElementID.Text)}";
                    }

                    switch (typeId)
                    {
                        case Enums.eType.SetO:
                            if (dbEnh.Unique)
                            {
                                shortDescRtf += $"{RTF.Color(RTF.ElementID.Warning)} (Unique) {RTF.Color(RTF.ElementID.Text)}";
                            }

                            if (dbEnh.EffectChance < 1 & dbEnh.EffectChance > 0)
                            {

                                procChance = $"{procChance}{RTF.Color(RTF.ElementID.Enhancement)}{dbEnh.EffectChance * 100:#0.##)} % chance of ";
                            }

                            break;

                        case Enums.eType.SpecialO:
                            // Missing case for Titan, Hydra, D-Sync ?
                            shortDescRtf += $"{RTF.Color(RTF.ElementID.Enhancement)}Hamidon/Synthetic Hamidon Origin Enhancement";

                            break;

                        default:
                            if (shortDescRtf != "")
                            {
                                shortDescRtf += " - ";
                            }

                            shortDescRtf += RTFText.GetEnhancementStringRTF(EnhSlot);

                            break;
                    }


                    if (typeId == Enums.eType.SetO)
                    {
                        longDescRtf = EnhancementSetCollection.GetSetInfoShortRTF(dbEnh.nIDSet);
                    }
                    else
                    {
                        var fxDesc = $"{procChance}{dbEnh.Desc}";
                        if (fxDesc != "")
                        {
                            fxDesc += "\r\n";
                        }

                        longDescRtf = $"{fxDesc}{RTFText.GetEnhancementStringLongRTF(EnhSlot)}";
                    }

                    Root.richInfoSmall.Rtf = $"{RTF.StartRTF()}{RTF.ToRTF(shortDescRtf)}{RTF.Crlf()}{RTF.Color(RTF.ElementID.Faded)}Shift+Click to move slot. Right-Click to place enh.{RTF.EndRTF()}";
                    Root.richInfoLarge.Rtf = $"{RTF.StartRTF()}{RTF.ToRTF(longDescRtf)}{RTF.EndRTF()}";
                    if (typeId != Enums.eType.SetO)
                    {
                        Root.listSpecialBonuses.Rows.Clear();
                        var enh = DatabaseAPI.Database.Enhancements[EnhSlot.Enh];
                        var enhEffects = enh.Effect;
                        for (var i = 0; i < 5; i++)
                        {
                            Root.listInfos.Rows.Add();
                            Root.listInfos.Rows[i].Height = 20;
                            if (i < enhEffects.Length)
                            {
                                Root.listInfos.SetCellContent(
                                    $"{(enhEffects[i].Enhance.SubID > -1 ? $"{(Enums.eEnhance) enhEffects[i].Enhance.SubID}" : $"{(Enums.eEnhance) enhEffects[i].Enhance.ID}")}",
                                    Color.FromArgb(0, 255, 0), "", i, 0);
                                Root.listInfos.SetCellContent(
                                    $"{100 * EnhSlot.GetEnhancementEffect((Enums.eEnhance)enhEffects[i].Enhance.ID, enhEffects[i].Enhance.SubID, enhEffects[i].BuffMode == Enums.eBuffDebuff.DeBuffOnly ? -1 : 1):##0.###}% (Sched. {enhEffects[i].Schedule})", "", i, 1);
                            }
                            else
                            {
                                Root.listInfos.SetCellContent(i, 0);
                                Root.listInfos.SetCellContent(i, 1);
                            }
                        }

                        return;
                    }

                    var dbSet = DatabaseAPI.Database.EnhancementSets[dbEnh.nIDSet];
                    var setSize = dbSet.Bonus.Length;
                    var setInfo = EnhancementSetCollection.GetSetInfoLong(dbEnh.nIDSet);
                    var chunks = setInfo.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    var slottedForSet = SlottedSets.ContainsKey(dbEnh.nIDSet)
                        ? SlottedSets[dbEnh.nIDSet]
                        : 0;

                    Root.listInfos.Rows.Clear();
                    for (var i = 0; i < 5; i++)
                    {
                        Root.listInfos.Rows.Add();
                        Root.listInfos.Rows[i].Height = 20;

                        for (var j = 0; j < 2; j++)
                        {
                            if (j == 0 & i < setSize)
                            {
                                Root.listInfos.SetCellContent($"{i + 2}:", "", i, j);
                            }
                            else if (j == 1 & i < setSize)
                            {
                                var fxString = chunks[i + 1].Substring(13);
                                var fxColor = slottedForSet switch
                                {
                                    > 0 when slottedForSet >= i + 2 => Color.FromArgb(0, 255, 0),
                                    > 0 when slottedForSet < i + 2 => Color.FromArgb(140, 140, 140),
                                    _ => Color.FromArgb(0, 192, 90)
                                };

                                Utilities.ModifiedEffectString(ref fxString, 1);
                                Root.listInfos.SetCellContent($"{fxString}", fxColor, "", i, j);
                            }
                            else
                            {
                                Root.listInfos.SetCellContent(i, j);
                            }
                        }
                    }

                    var activeSpecialBonuses = dbSet.SpecialBonus
                        .Select(b => dbSet.GetEffectString(Array.FindIndex(dbSet.SpecialBonus, e => e.Equals(b)), true))
                        .Count(b => !string.IsNullOrEmpty(b));
                    Root.listSpecialBonuses.Rows.Clear();
                    for (var i = 0; i < 4; i++)
                    {
                        Root.listSpecialBonuses.Rows.Add();
                        Root.listSpecialBonuses.Rows[i].Height = 22;

                        for (var j = 0; j < 2; j++)
                        {
                            if (j == 0 & i + setSize + 1 < chunks.Length)
                            {
                                Root.listSpecialBonuses.SetCellContent($"Special{(activeSpecialBonuses > 1 ? $" #{i + 1}" : "")}:", "", i, j);
                            }
                            else if (j == 1 & i + setSize + 1 < chunks.Length)
                            {
                                var fxFullText = chunks[i + setSize + 1].Substring(2);
                                Utilities.ModifiedEffectString(ref fxFullText, 1);
                                
                                var parts = fxFullText.Split(new[] { ": "}, StringSplitOptions.RemoveEmptyEntries);
                                var fxString = parts[1];
                                Root.listSpecialBonuses.Rows[i].Cells[0].ToolTipText = chunks[i + setSize + 1].Substring(2);
                                Root.listSpecialBonuses.Rows[i].Cells[1].ToolTipText = chunks[i + setSize + 1].Substring(2);
                                Utilities.ModifiedEffectString(ref fxString, 1);
                                Root.listSpecialBonuses.SetCellContent($"{fxString}", Color.Cyan, "", i, j);
                            }
                            else
                            {
                                Root.listSpecialBonuses.SetCellContent(i, j);
                            }
                        }
                    }
                }

                private static string CheckEffectTypeAffects(IPower power, Enums.eEffectType effectType)
                {
                    var allVectors = new List<Enums.eDamage>
                    {
                        Enums.eDamage.Smashing,
                        Enums.eDamage.Lethal,
                        Enums.eDamage.Fire,
                        Enums.eDamage.Cold,
                        Enums.eDamage.Energy,
                        Enums.eDamage.Negative,
                        Enums.eDamage.Toxic,
                        Enums.eDamage.Psionic
                    };

                    var allVectorsDef = new List<Enums.eDamage>
                    {
                        Enums.eDamage.Smashing,
                        Enums.eDamage.Lethal,
                        Enums.eDamage.Fire,
                        Enums.eDamage.Cold,
                        Enums.eDamage.Energy,
                        Enums.eDamage.Negative,
                        Enums.eDamage.Psionic
                    };

                    var dmgVectors = effectType == Enums.eEffectType.Defense | effectType == Enums.eEffectType.Elusivity
                        ? allVectorsDef.Clone()
                        : allVectors.Clone();

                    var positionVectors = new List<Enums.eDamage>
                    {
                        Enums.eDamage.Melee,
                        Enums.eDamage.Ranged,
                        Enums.eDamage.AoE
                    };

                    var buffTypes = power.Effects
                        .Where(e => e.EffectType == effectType)
                        .Select(e => e.DamageType)
                        .ToList();

                    if (effectType == Enums.eEffectType.Defense | effectType == Enums.eEffectType.Elusivity)
                    {
                        allVectors = allVectorsDef.Concat(positionVectors).ToList();
                    }

                    switch (effectType)
                    {
                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Resistance:
                            //return buffTypes.Intersect(allVectors).Count() == allVectors.Count ? "All" : string.Join(", ", buffTypes);
                            return buffTypes.Intersect(allVectors).Count() == allVectors.Count
                                ? "All"
                                : buffTypes.Count == 1
                                    ? $"{buffTypes[0]}"
                                    : "Multi";

                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Elusivity:
                            if (buffTypes.Count == positionVectors.Count &
                                buffTypes.Intersect(positionVectors).Count() == positionVectors.Count)
                            {
                                return "All pos.";
                            }

                            if (buffTypes.Count == dmgVectors.Count &
                                buffTypes.Intersect(dmgVectors).Count() == dmgVectors.Count)
                            {
                                return "All dmg";
                            }

                            if (buffTypes.Count == allVectors.Count &
                                buffTypes.Intersect(allVectors).Count() == allVectors.Count)
                            {
                                return "All";
                            }

                            //return string.Join(", ", buffTypes);
                            return buffTypes.Count == 1 ? $"{buffTypes[0]}" : "Multi";

                        default:
                            return "";
                    }
                }

                // From clsToonX.PopSlottedEnhInfo()
                private static PowerStats GetSlottedEnhancementEffects()
                {
                    var powerStats = new PowerStats();

                    if (HistoryIdx < 0)
                    {
                        return powerStats;
                    }

                    for (var index1 = 0; index1 < MidsContext.Character.CurrentBuild.Powers[HistoryIdx].SlotCount; index1++)
                    {
                        var enhSlot = MidsContext.Character.CurrentBuild.Powers[HistoryIdx].Slots[index1].Enhancement;
                        var enhId = enhSlot.Enh;
                        if (enhId <= -1)
                        {
                            continue;
                        }

                        var enh = DatabaseAPI.Database.Enhancements[enhId];
                        for (var index2 = 0; index2 < enh.Effect.Length; index2++)
                        {
                            var effects = enh.Effect;
                            if (effects[index2].Mode != Enums.eEffMode.Enhancement)
                            {
                                continue;
                            }

                            if (effects[index2].Enhance.ID == (int) Enums.eEnhance.Mez)
                            {
                                var mezKey = new PowerStats.BuffStat
                                {
                                    Stat = Enums.eEnhance.None,
                                    Mez = (Enums.eMez) effects[index2].Enhance.SubID,
                                    BuffType = PowerStats.BuffType.Mez
                                };

                                powerStats.AddUpdate(mezKey,
                                    new PowerStats.BuffInfo
                                    {
                                        Schedule = Enhancement.GetSchedule(Enums.eEnhance.Mez, effects[index2].Enhance.SubID),
                                        Value = enhSlot.GetEnhancementEffect(Enums.eEnhance.Mez, effects[index2].Enhance.SubID, 1),
                                        ValueAfterED = 0
                                    });
                            }
                            else
                            {
                                switch (effects[index2].BuffMode)
                                {
                                    case Enums.eBuffDebuff.BuffOnly:
                                        var buffKey = new PowerStats.BuffStat
                                        {
                                            Stat = (Enums.eEnhance) effects[index2].Enhance.ID,
                                            Mez = Enums.eMez.None,
                                            BuffType = PowerStats.BuffType.Buff
                                        };

                                        powerStats.AddUpdate(buffKey,
                                            new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule((Enums.eEnhance) effects[index2].Enhance.ID),
                                                Value = enhSlot.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, 1),
                                                ValueAfterED = 0
                                            });

                                        break;
                                    case Enums.eBuffDebuff.DeBuffOnly:
                                        if (effects[index2].Enhance.ID != (int) Enums.eEnhance.SpeedFlying &
                                            effects[index2].Enhance.ID != (int) Enums.eEnhance.SpeedRunning &
                                            effects[index2].Enhance.ID != (int) Enums.eEnhance.SpeedJumping)
                                        {
                                            var debuffKey = new PowerStats.BuffStat
                                            {
                                                Stat = (Enums.eEnhance) effects[index2].Enhance.ID,
                                                Mez = Enums.eMez.None,
                                                BuffType = PowerStats.BuffType.Debuff
                                            };

                                            powerStats.AddUpdate(debuffKey, new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule((Enums.eEnhance) effects[index2].Enhance.ID),
                                                Value = enhSlot.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, 1),
                                                ValueAfterED = 0
                                            });
                                        }

                                        break;
                                    default:
                                        var buffAnyKey = new PowerStats.BuffStat
                                        {
                                            Stat = (Enums.eEnhance) effects[index2].Enhance.ID,
                                            Mez = Enums.eMez.None,
                                            BuffType = PowerStats.BuffType.Any
                                        };

                                        powerStats.AddUpdate(buffAnyKey,
                                            new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule((Enums.eEnhance) effects[index2].Enhance.ID),
                                                Value = enhSlot.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, 1),
                                                ValueAfterED = 0
                                            });

                                        break;
                                }
                            }
                        }
                    }

                    if (MidsContext.Config.DisableAlphaPopup) return powerStats;
                    
                    foreach (var buildPowerEntry in MidsContext.Character.CurrentBuild.Powers)
                    {
                        if (buildPowerEntry.Power == null || !buildPowerEntry.StatInclude)
                        {
                            continue;
                        }

                        IPower power1 = new Power(buildPowerEntry.Power);
                        power1.AbsorbPetEffects();
                        power1.ApplyGrantPowerEffects();
                        foreach (var effect in power1.Effects)
                        {
                            if ((power1.PowerType != Enums.ePowerType.GlobalBoost) & (!effect.Absorbed_Effect |
                                    (effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost)))
                            {
                                continue;
                            }

                            if (effect.Absorbed_Effect & (effect.Absorbed_Power_nID > -1))
                            {
                                power1 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                            }

                            var eBuffDebuff = Enums.eBuffDebuff.Any;
                            var flag = false;
                            foreach (var str1 in MidsContext.Character.CurrentBuild.Powers[HistoryIdx].Power.BoostsAllowed)
                            {
                                if (power1.BoostsAllowed.All(str2 => str1 != str2)) continue;

                                if (str1.Contains("Buff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                                }

                                if (str1.Contains("Debuff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                                }

                                flag = true;
                                break;
                            }

                            if (!flag)
                            {
                                continue;
                            }

                            if (effect.EffectType == Enums.eEffectType.Enhancement)
                            {
                                switch (effect.ETModifies)
                                {
                                    case Enums.eEffectType.Defense:
                                        if (effect.DamageType == Enums.eDamage.Smashing)
                                        {
                                            var buffKey = new PowerStats.BuffStat
                                            {
                                                BuffType = eBuffDebuff switch
                                                {
                                                    Enums.eBuffDebuff.BuffOnly => PowerStats.BuffType.Buff,
                                                    Enums.eBuffDebuff.DeBuffOnly => PowerStats.BuffType.Debuff,
                                                    _ => PowerStats.BuffType.Any
                                                },
                                                Stat = Enums.eEnhance.Defense,
                                                Mez = Enums.eMez.None
                                            };

                                            powerStats.AddUpdate(buffKey, new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule(Enums.eEnhance.Defense),
                                                Value = effect.Mag * (effect.IgnoreED ? 0 : 1),
                                                ValueAfterED = effect.Mag * (effect.IgnoreED ? 1 : 0)
                                            });
                                        }

                                        break;
                                    case Enums.eEffectType.Mez:
                                        var mezKey = new PowerStats.BuffStat
                                        {
                                            BuffType = eBuffDebuff switch
                                            {
                                                Enums.eBuffDebuff.BuffOnly => PowerStats.BuffType.Buff,
                                                Enums.eBuffDebuff.DeBuffOnly => PowerStats.BuffType.Debuff,
                                                _ => PowerStats.BuffType.Any
                                            },
                                            Stat = Enums.eEnhance.None,
                                            Mez = effect.MezType
                                        };

                                        powerStats.AddUpdate(mezKey, new PowerStats.BuffInfo
                                        {
                                            Schedule = Enhancement.GetSchedule(Enums.eEnhance.Mez, (int) effect.MezType),
                                            Value = effect.Mag * (effect.IgnoreED ? 0 : 1),
                                            ValueAfterED = effect.Mag * (effect.IgnoreED ? 1 : 0)
                                        });

                                        break;
                                    default:
                                        var index3 = effect.ETModifies != Enums.eEffectType.RechargeTime
                                            ? (int) Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString())
                                            : (int) Enums.eEnhance.RechargeTime;

                                        var fxKey = new PowerStats.BuffStat
                                        {
                                            BuffType = PowerStats.BuffType.Any,
                                            Stat = (Enums.eEnhance) index3,
                                            Mez = Enums.eMez.None
                                        };

                                        powerStats.AddUpdate(fxKey, new PowerStats.BuffInfo
                                        {
                                            Schedule = Enhancement.GetSchedule(fxKey.Stat),
                                            Value = effect.Mag * (effect.IgnoreED ? 0 : 1),
                                            ValueAfterED = effect.Mag * (effect.IgnoreED ? 1 : 0),
                                        });

                                        break;
                                }
                            }
                            else if ((effect.EffectType == Enums.eEffectType.DamageBuff) &
                                     (effect.DamageType == Enums.eDamage.Smashing))
                            {
                                if (effect.IgnoreED)
                                {
                                    foreach (var str in power1.BoostsAllowed)
                                    {
                                        if (str.StartsWith("Res_Damage"))
                                        {
                                            powerStats.AddUpdate(new PowerStats.BuffStat
                                            {
                                                BuffType = PowerStats.BuffType.Any,
                                                Mez = Enums.eMez.None,
                                                Stat = Enums.eEnhance.Resistance
                                            }, new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule(Enums.eEnhance.Resistance),
                                                Value = 0,
                                                ValueAfterED = effect.Mag
                                            });

                                            break;
                                        }

                                        if (!str.StartsWith("Damage"))
                                        {
                                            continue;
                                        }

                                        powerStats.AddUpdate(new PowerStats.BuffStat
                                        {
                                            BuffType = PowerStats.BuffType.Any,
                                            Mez = Enums.eMez.None,
                                            Stat = Enums.eEnhance.Damage
                                        }, new PowerStats.BuffInfo
                                        {
                                            Schedule = Enhancement.GetSchedule(Enums.eEnhance.Damage),
                                            Value = 0,
                                            ValueAfterED = effect.Mag
                                        });

                                        break;
                                    }
                                }
                                else
                                {
                                    foreach (var str in power1.BoostsAllowed)
                                    {
                                        if (str.StartsWith("Res_Damage"))
                                        {
                                            powerStats.AddUpdate(new PowerStats.BuffStat
                                            {
                                                BuffType = PowerStats.BuffType.Any,
                                                Mez = Enums.eMez.None,
                                                Stat = Enums.eEnhance.Resistance
                                            }, new PowerStats.BuffInfo
                                            {
                                                Schedule = Enhancement.GetSchedule(Enums.eEnhance.Resistance),
                                                Value = effect.Mag,
                                                ValueAfterED = 0
                                            });

                                            break;
                                        }

                                        if (!str.StartsWith("Damage"))
                                        {
                                            continue;
                                        }

                                        powerStats.AddUpdate(new PowerStats.BuffStat
                                        {
                                            BuffType = PowerStats.BuffType.Any,
                                            Mez = Enums.eMez.None,
                                            Stat = Enums.eEnhance.Damage
                                        }, new PowerStats.BuffInfo
                                        {
                                            Schedule = Enhancement.GetSchedule(Enums.eEnhance.Damage),
                                            Value = effect.Mag,
                                            ValueAfterED = 0
                                        });

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    return powerStats;
                }
            }

            public static class Effects
            {
                private static DataView2 Root;
                private static InfoType LayoutType;

                public static void Render(DataView2 root, InfoType layoutType)
                {
                    Root = root;
                    LayoutType = layoutType;

                    DisplayEffects();
                }

                private static void DisplayEffects()
                {
                    Root.ipbLock2.Visible = Root.Locked;
                    Root.ipbResize2.IconChar = Root.SmallSize
                        ? IconChar.ChevronDown
                        : IconChar.ChevronUp;
                    Root.effectsTabTitle.Invalidate();

                    var effectGroups = EffectsGroupFilter.FromPower(_enhancedPower);
                    var labels = effectGroups.Groups.Keys.ToList();
                    Root.lblEffectsBlock1.Text = "";
                    Root.lblEffectsBlock2.Text = "";
                    Root.lblEffectsBlock3.Text = "";
                    for (var i = 0; i < effectGroups.Groups.Count; i += 2)
                    {
                        if (i < effectGroups.Groups.Count - 1)
                        {
                            switch (i)
                            {
                                case 0:
                                    Root.lblEffectsBlock1.Text = @$"{labels[i]}/{labels[i + 1]}";
                                    break;

                                case 2:
                                    Root.lblEffectsBlock2.Text = @$"{labels[i]}/{labels[i + 1]}";
                                    break;

                                case 4:
                                    Root.lblEffectsBlock3.Text = @$"{labels[i]}/{labels[i + 1]}";
                                    break;
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    Root.lblEffectsBlock1.Text = labels[i];
                                    break;

                                case 2:
                                    Root.lblEffectsBlock2.Text = labels[i];
                                    break;

                                case 4:
                                    Root.lblEffectsBlock3.Text = labels[i];
                                    break;
                            }
                        }
                    }

                    var groupedItems = effectGroups.Groups.Values.ToList();
                    Root.gridEffectsBlock1.Rows.Clear();
                    Root.gridEffectsBlock2.Rows.Clear();
                    Root.gridEffectsBlock3.Rows.Clear();
                    for (var i = 0; i < 5; i++)
                    {
                        Root.gridEffectsBlock1.Rows.Add();
                        Root.gridEffectsBlock2.Rows.Add();
                        Root.gridEffectsBlock3.Rows.Add();
                        for (var j = 0; j < 4; j++)
                        {
                            Root.gridEffectsBlock1.SetCellContent(i, j);
                            Root.gridEffectsBlock2.SetCellContent(i, j);
                            Root.gridEffectsBlock3.SetCellContent(i, j);
                        }
                    }

                    var rSingleMez = new Regex(@"^Mez\(([A-Za-z\-]+)\)$", RegexOptions.CultureInvariant);
                    var rGenericSubEffects = new Regex(@"^([A-Za-z\-_]+)\(([A-Za-z\-\,\s]+)\)$", RegexOptions.CultureInvariant);
                    for (var i = 0; i < groupedItems.Count; i++)
                    {
                        var target = (int)Math.Floor(i / 2f) switch
                        {
                            0 => Root.gridEffectsBlock1,
                            1 => Root.gridEffectsBlock2,
                            2 => Root.gridEffectsBlock3,
                            _ => Root.gridEffectsBlock1
                        };

                        for (var j = 0; j < Math.Min(5, groupedItems[i].Count); j++)
                        {
                            var boostType = groupedItems[i][j].GetBoostType();
                            var statLong = groupedItems[i][j].GetStatName();
                            var stat = Utilities.CompactEffectString(statLong, 3);
                            var statLongTarget = Utilities.CompactEffectString(groupedItems[i][j].GetStatName(true), 3);
                            var mag = groupedItems[i][j].GetMagString();
                            var rowOffset = i % 2 * 2;
                            var invertBoost = false;

                            if (statLong.StartsWith("GrantPower"))
                            {
                                target.SetCellContent($"{stat}:", $"{statLongTarget.Replace(":", "")}", j, rowOffset);
                                target.SetCellContent(groupedItems[i][j].GetGrants(), Boosts.GetBoostColor(BoostType.Extra), $"Granted Powers:\r\n{groupedItems[i][j].GetGrants(true)}", j, rowOffset + 1);

                                continue;
                            }

                            if (statLong.StartsWith("EntCreate"))
                            {
                                target.SetCellContent($"{stat}:", $"{statLongTarget.Replace(":", "")}", j, rowOffset);
                                target.SetCellContent(groupedItems[i][j].GetSummons(), Boosts.GetBoostColor(BoostType.Extra), $"Summoned Entities:\r\n{groupedItems[i][j].GetSummons()}", j, rowOffset + 1);

                                continue;
                            }

                            if (groupedItems[i][j].GetMag() < 0)
                            {
                                // Effects to target can be either buff or debuff, depending on who or what you cast it...
                                // As a result, effects to target will never be inverted.
                                if (groupedItems[i][j].GetToWho() == Enums.eToWho.Self)
                                {
                                    if (statLong.StartsWith("Mez("))
                                    {
                                        invertBoost = true;
                                    }
                                }
                            }

                            if (rSingleMez.IsMatch(stat))
                            {
                                // E.g. Mez(Stunned) --> Stunned
                                var m = rSingleMez.Match(stat);
                                stat = m.Groups[1].Value;
                            }
                            else if (rGenericSubEffects.IsMatch(stat))
                            {
                                // E.g. Defense(Smash) --> Defense
                                var m = rGenericSubEffects.Match(stat);
                                stat = m.Groups[1].Value;
                            }
                            
                            target.SetCellContent($"{stat}:", $"{statLongTarget.Replace(":", "")}", j, rowOffset);
                            target.SetCellContent(mag, Boosts.GetBoostColor(boostType, invertBoost), "", j, rowOffset + 1);
                        }
                    }
                }
            }

            public static class Totals
            {
                public static class Colors
                {
                    public static readonly Color ButtonNormalColor = Color.FromArgb(1, 41, 26);
                    public static readonly Color ButtonSelectedColor = Color.FromArgb(2, 81, 58);
                    public static readonly Color ButtonHighlightColor = Color.FromArgb(3, 153, 98);
                }

                private static DataView2 Root;
                private static InfoType LayoutType;
                private static readonly List<Enums.eMez> MezList = new()
                {
                    Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
                    Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
                    Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
                };

                private static readonly List<Enums.eDamage> ElusivityDamageList = new()
                {
                    Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
                    Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Toxic,
                    Enums.eDamage.Melee, Enums.eDamage.Ranged, Enums.eDamage.AoE
                };

                private static readonly List<Enums.eEffectType> DebuffEffectsList = new()
                {
                    Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
                    Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
                    Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
                };

                public static TotalsMiscEffectsType MiscEffectsType;

                public static TotalsPaneMouseEventInfo PaneMouseEventInfo = new()
                {
                    Loc = new Point(-1, -1),
                    ContainerControlName = ""
                };

                public static void Render(DataView2 root, InfoType layoutType)
                {
                    Root = root;
                    LayoutType = layoutType;

                    DisplayTotals();
                }

                public static void SwitchMiscEffectsType(TotalsMiscEffectsType displayType = TotalsMiscEffectsType.Elusivity)
                {
                    MiscEffectsType = displayType;

                    DisplayMiscEffects();
                }

                public static void SwitchMiscEffectsType(int displayType = 0)
                {
                    MiscEffectsType = displayType switch
                    {
                        1 => TotalsMiscEffectsType.DebuffResistances,
                        2 => TotalsMiscEffectsType.MezResistances,
                        3 => TotalsMiscEffectsType.MezProtection,
                        _ => TotalsMiscEffectsType.Elusivity
                    };

                    DisplayMiscEffects();
                }

                private static void DisplayTotals()
                {
                    Root.ipbLock3.Visible = Root.Locked;
                    Root.ipbResize3.IconChar = Root.SmallSize
                        ? IconChar.ChevronDown
                        : IconChar.ChevronUp;

                    var displayStats = MidsContext.Character.DisplayStats;

                    Root.dV2TotalsPane1L.LockDraw();
                    Root.dV2TotalsPane1R.LockDraw();

                    Root.dV2TotalsPane1L.ClearItems();
                    Root.dV2TotalsPane1R.ClearItems();
                    var damageVectors = Enum.GetNames(typeof(Enums.eDamage));
                    var k = 0;
                    for (var i = 1; i < damageVectors.Length; i++)
                    {
                        if (damageVectors[i] == "Toxic" || damageVectors[i] == "Special" || damageVectors[i].StartsWith("Unique"))
                        {
                            continue;
                        }

                        //var target = k < Root.dV2TotalsPane1L.MaxItems ? Root.dV2TotalsPane1L : Root.dV2TotalsPane1R; // Stack, vertically first
                        var target = k % 2 == 0 ? Root.dV2TotalsPane1L : Root.dV2TotalsPane1R; // Stack, horizontally first
                        target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.Defense(i) / 100f,
                            displayStats.Defense(i) / 100f, true));
                        k++;
                    }

                    Root.dV2TotalsPane1L.UnlockDraw();
                    Root.dV2TotalsPane1R.UnlockDraw();

                    Root.dV2TotalsPane2L.LockDraw();
                    Root.dV2TotalsPane2R.LockDraw();

                    Root.dV2TotalsPane2L.ClearItems();
                    Root.dV2TotalsPane2R.ClearItems();
                    k = 0;
                    for (var i = 1; i < damageVectors.Length; i++)
                    {
                        if (damageVectors[i] == "Special" || damageVectors[i] == "Melee" || damageVectors[i] == "Ranged" || damageVectors[i] == "AoE" || damageVectors[i].StartsWith("Unique"))
                        {
                            continue;
                        }

                        //var target = k < Root.dV2TotalsPane2L.MaxItems ? Root.dV2TotalsPane2L : Root.dV2TotalsPane2R;
                        var target = k % 2 == 0 ? Root.dV2TotalsPane2L : Root.dV2TotalsPane2R;
                        target.AddItem(new DV2TotalsPane.Item(damageVectors[i], displayStats.DamageResistance(i, false) / 100f,
                            displayStats.DamageResistance(i, true) / 100f, true));
                        k++;
                    }

                    Root.dV2TotalsPane2L.UnlockDraw();
                    Root.dV2TotalsPane2R.UnlockDraw();

                    DisplayMiscEffects();
                }

                private static void DisplayMiscEffects()
                {
                    var headerGroupText = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => "Debuff Resistances",
                        TotalsMiscEffectsType.MezResistances => "Mez Resistances",
                        TotalsMiscEffectsType.MezProtection => "Mez Protection",
                        _ => "Elusivity"
                    };

                    Root.label8.Text = $"Misc Effects ({headerGroupText}):";
                    
                    var cappedValues = MiscEffectsType switch
                    {
                        // Debuff resistances
                        TotalsMiscEffectsType.DebuffResistances => DebuffEffectsList.Select(e => Math.Min(
                                e == Enums.eEffectType.Defense
                                    ? Statistics.MaxDefenseDebuffRes
                                    : Statistics.MaxGenericDebuffRes,
                                MidsContext.Character.Totals.DebuffRes[(int)e]))
                            .ToList(),
                        
                        // Status resistances
                        TotalsMiscEffectsType.MezResistances => MezList.Select(m => MidsContext.Character.Totals.MezRes[(int)m]).ToList(),

                        // Status protection
                        TotalsMiscEffectsType.MezProtection => MezList.Select(m => -MidsContext.Character.Totals.Mez[(int)m]).ToList(),

                        // Elusivity
                        _ => ElusivityDamageList.Cast<int>().Select(t => (MidsContext.Character.Totals.Elusivity[t] + (MidsContext.Config.Inc.DisablePvE ? 0.4f : 0)) * 100).ToList()
                    };

                    var uncappedValues = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int) e]).ToList(),
                        _ => cappedValues
                    };

                    var labels = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => DebuffEffectsList.Select(e => e.ToString()).ToList(),
                        TotalsMiscEffectsType.Elusivity => ElusivityDamageList.Select(e => e.ToString()).ToList(),
                        _ => MezList.Select(e => e.ToString()).ToList()
                    };

                    var backgroundColorEnd = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => Color.FromArgb(0, 127, 127),
                        TotalsMiscEffectsType.MezResistances => Color.FromArgb(127, 127, 0),
                        TotalsMiscEffectsType.MezProtection => Color.FromArgb(127, 64, 0),
                        _ => Color.FromArgb(141, 2, 200)
                    };

                    var barColorMain = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => Color.FromArgb(0, 255, 255),
                        TotalsMiscEffectsType.MezResistances => Color.FromArgb(255, 255, 0),
                        TotalsMiscEffectsType.MezProtection => Color.FromArgb(255, 128, 0),
                        _ => Color.FromArgb(163, 1, 231)
                    };

                    var barColorUncapped = MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => Color.FromArgb(0, 90, 127),
                        TotalsMiscEffectsType.MezResistances => Color.FromArgb(255, 255, 0),
                        TotalsMiscEffectsType.MezProtection => Color.FromArgb(255, 128, 0),
                        _ => Color.FromArgb(163, 1, 231)
                    };

                    Root.dV2TotalsPane3L.LockDraw();
                    Root.dV2TotalsPane3R.LockDraw();

                    Root.dV2TotalsPane3L.EnableUncappedValues = MiscEffectsType == TotalsMiscEffectsType.DebuffResistances;
                    Root.dV2TotalsPane3L.BackgroundColorEnd = backgroundColorEnd;
                    Root.dV2TotalsPane3L.BarColorMain = barColorMain;
                    Root.dV2TotalsPane3L.BarColorUncapped = barColorUncapped;

                    Root.dV2TotalsPane3R.EnableUncappedValues = MiscEffectsType == TotalsMiscEffectsType.DebuffResistances;
                    Root.dV2TotalsPane3R.BackgroundColorEnd = backgroundColorEnd;
                    Root.dV2TotalsPane3R.BarColorMain = barColorMain;
                    Root.dV2TotalsPane3R.BarColorUncapped = barColorUncapped;

                    Root.dV2TotalsPane3L.ClearItems();
                    Root.dV2TotalsPane3R.ClearItems();
                    for (var i = 0; i < cappedValues.Count; i++)
                    {
                        var target = i % 2 == 0 ? Root.dV2TotalsPane3L : Root.dV2TotalsPane3R;
                        target.AddItem(new DV2TotalsPane.Item(labels[i], cappedValues[i] / 100f,
                            uncappedValues[i] / 100f, true));
                    }

                    Root.dV2TotalsPane3L.UnlockDraw();
                    Root.dV2TotalsPane3R.UnlockDraw();
                }
            }

            public static class Enhance
            {
                private static DataView2 Root;
                private static InfoType LayoutType;
                private static MultiStateFlag ViewMode = new(2, 0, MultiStateFlag.Mode.RampUp);

                public static void Render(DataView2 root, InfoType layoutType)
                {
                    Root = root;
                    LayoutType = layoutType;
                    DisplayEnhance();
                }

                private static void DisplayEnhance()
                {
                    Root.ipbLock4.Visible = Root.Locked;
                    Root.ipbResize4.IconChar = Root.SmallSize
                        ? IconChar.ChevronDown
                        : IconChar.ChevronUp;

                    SetViewMode();

                    Root.enhanceTabTitle.Invalidate();
                    Root.skglEnhActive.Invalidate();
                    Root.skglEnhAlt.Invalidate();
                }

                private static void FillEDFigures()
                {
                    var edFiguresBuffs = Build.EDFigures.GetBuffsForBuildPower(HistoryIdx);

                    var edRtfText = RTF.StartRTF() + RTF.Color(RTF.ElementID.Text);
                    edRtfText += DisplayEDFiguresForGroup(edFiguresBuffs.Buffs, "Buffs:");
                    edRtfText += DisplayEDFiguresForGroup(edFiguresBuffs.Debuffs, "Debuffs:");
                    edRtfText += DisplayEDFiguresForGroup(edFiguresBuffs.BuffDebuffs, "Buffs/Debuffs:");
                    edRtfText += RTF.Color(RTF.ElementID.Text) + RTF.EndRTF();

                    Root.richEnhValues.Rtf = edRtfText;
                }

                private static string FillSetBonuses(Tray tray)
                {
                    var hasEnhEffect = false;
                    if (MidsContext.Character.CurrentBuild.Powers == null)
                    {
                        return "";
                    }

                    if (HistoryIdx < 0 || HistoryIdx >= MidsContext.Character.CurrentBuild.Powers.Count)
                    {
                        return "";
                    }

                    if (MidsContext.Character.CurrentBuild.Powers[HistoryIdx] == null)
                    {
                        return "";
                    }

                    var ret = RTF.StartRTF();
                    var powerEntry = MidsContext.Character.CurrentBuild.Powers[HistoryIdx];
                    var slotCount = powerEntry.Slots.Length;
                    if (slotCount > 0)
                    {
                        for (var slotIdx = 0; slotIdx < slotCount; slotIdx++)
                        {
                            var enh = tray == Tray.Main
                                ? powerEntry.Slots[slotIdx].Enhancement.Enh
                                : powerEntry.Slots[slotIdx].FlippedEnhancement.Enh;

                            if (enh > -1 && DatabaseAPI.Database.Enhancements[enh].HasEnhEffect)
                            {
                                hasEnhEffect = true;
                            }
                        }

                        if (hasEnhEffect)
                        {
                            //popupData.Sections[index1] = PopSlottedEnhInfo(HistoryIdx);
                        }
                    }

                    var enhSetsIdx = new Dictionary<int, List<int>>();
                    if (slotCount > 0)
                    {
                        for (var slotIdx = 0; slotIdx < slotCount; slotIdx++)
                        {
                            var enh = tray == Tray.Main
                                ? powerEntry.Slots[slotIdx].Enhancement.Enh
                                : powerEntry.Slots[slotIdx].FlippedEnhancement.Enh;

                            if (enh > -1 && enh < DatabaseAPI.Database.Enhancements.Length)
                            {
                                var enhData = DatabaseAPI.Database.Enhancements[enh];
                                if (enhData.nIDSet <= -1)
                                {
                                    continue;
                                }

                                if (!enhSetsIdx.ContainsKey(enhData.nIDSet))
                                {
                                    enhSetsIdx.Add(enhData.nIDSet, new List<int>());
                                }

                                enhSetsIdx[enhData.nIDSet].Add(enh);
                            }
                        }
                    }

                    if (enhSetsIdx.Count <= 0)
                    {
                        return "";
                    }

                    ret += $"{RTF.Color(RTF.ElementID.Faded)}{RTF.Bold($"{(tray == Tray.Main ? "Active" : "Alt.")} Enhancement Sets:")}{RTF.Crlf()}";
                    const string bonusIndent = "  ";
                    foreach (var enhSetData in enhSetsIdx)
                    {
                        var enhancementSet = DatabaseAPI.Database.EnhancementSets[enhSetData.Key];
                        ret += $"{RTF.Color(RTF.ElementID.Text)}{enhancementSet.DisplayName} ({enhSetData.Value.Count}/{enhancementSet.Enhancements.Length}){RTF.Crlf()}";

                        for (var bonusIdx = 0; bonusIdx < enhancementSet.Bonus.Length; bonusIdx++)
                        {
                            if ((enhSetData.Value.Count >= enhancementSet.Bonus[bonusIdx].Slotted) &
                                (((enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE) |
                                 ((enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                                 (enhancementSet.Bonus[bonusIdx].PvMode == Enums.ePvX.Any)))
                            {
                                var enhString = enhancementSet.GetEffectString(bonusIdx, false, false, true, true);
                                if (string.IsNullOrWhiteSpace(enhString))
                                {
                                    continue;
                                }

                                ret += $"{RTF.Color(RTF.ElementID.Enhancement)}{bonusIndent}{enhString.Replace(", ", $"{RTF.Crlf()}{bonusIndent}")}{RTF.Crlf()}";
                            }
                        }

                        for (var specialBonusIdx = 0; specialBonusIdx < enhancementSet.SpecialBonus.Length; specialBonusIdx++)
                        {
                            var enhString = enhancementSet.GetEffectString(specialBonusIdx, true, false, true, true);
                            if (string.IsNullOrWhiteSpace(enhString))
                            {
                                continue;
                            }

                            if (!enhSetData.Value.Contains(enhancementSet.Enhancements[specialBonusIdx]))
                            {
                                continue;
                            }

                            ret += $"{RTF.Color(RTF.ElementID.Invention)}{bonusIndent}{enhString.Replace(", ", $"{RTF.Crlf()}{bonusIndent}")}{RTF.Crlf()}";
                        }
                    }

                    ret += RTF.EndRTF();

                    return ret;
                }

                private static void SetViewMode()
                {
                    switch (ViewMode.Current)
                    {
                        // Enhancement values/ED Figures only
                        case 0:
                            Root.richEnhValues.Size = new Size(353, 236);
                            Root.richEnhValues.Visible = true;
                            FillEDFigures();

                            Root.rtSetsCompareMain.Visible = false;
                            Root.rtSetsCompareAlt.Visible = false;

                            Root.label11.Text = "Enhancement Values";

                            break;

                        // Enhancement values/ED Figures vs Sets Compare (vertical halves)
                        case 1:
                            Root.richEnhValues.Size = new Size(353, 116);
                            Root.richEnhValues.Visible = true;
                            FillEDFigures();

                            Root.rtSetsCompareMain.Location = new Point(4, 167);
                            Root.rtSetsCompareMain.Size = new Size(175, 117);
                            Root.rtSetsCompareMain.Visible = true;

                            Root.rtSetsCompareAlt.Location = new Point(182, 167);
                            Root.rtSetsCompareAlt.Size = new Size(175, 117);
                            Root.rtSetsCompareAlt.Visible = true;

                            Root.rtSetsCompareMain.Rtf = FillSetBonuses(Tray.Main);
                            Root.rtSetsCompareAlt.Rtf = FillSetBonuses(Tray.Alt);

                            Root.label11.Text = "Enhancement Values | Compare Set Bonuses";

                            break;

                        // Sets Compare only
                        case 2:
                            Root.richEnhValues.Visible = false;

                            Root.rtSetsCompareMain.Location = new Point(4, 48);
                            Root.rtSetsCompareMain.Size = new Size(175, 236);
                            Root.rtSetsCompareMain.Visible = true;

                            Root.rtSetsCompareAlt.Location = new Point(182, 48);
                            Root.rtSetsCompareAlt.Size = new Size(175, 236);
                            Root.rtSetsCompareAlt.Visible = true;

                            Root.rtSetsCompareMain.Rtf = FillSetBonuses(Tray.Main);
                            Root.rtSetsCompareAlt.Rtf = FillSetBonuses(Tray.Alt);

                            Root.label11.Text = "Compare Set Bonuses";

                            break;
                    }

                    SetTextBoxVScrollVisibility(Root.richEnhValues);
                    SetTextBoxVScrollVisibility(Root.rtSetsCompareMain);
                    SetTextBoxVScrollVisibility(Root.rtSetsCompareAlt);
                }

                public static void NextViewMode()
                {
                    ViewMode.Next();
                    SetViewMode();
                }

                private static string DisplayEDFiguresForGroup(IReadOnlyList<Build.EDFigures.EDWeightedItem> buffDebuffs,
                    string header = "", bool endBlankLine = true)
                {
                    var edRtfText = "";

                    if (header != "" & buffDebuffs.Count > 0)
                    {
                        edRtfText += $"{RTF.Color(RTF.ElementID.Invention)}{header}{RTF.Color(RTF.ElementID.Text)}{RTF.Crlf()}";
                    }

                    var indent = header != "" ? "   " : "";

                    for (var i = 0; i < buffDebuffs.Count; i++)
                    {
                        if (i > 0)
                        {
                            edRtfText += RTF.Crlf();
                        }
                        
                        edRtfText += buffDebuffs[i].EDStrength switch
                        {
                            Build.EDFigures.EDStrength.Light => RTF.Color(RTF.ElementID.Enhancement),
                            Build.EDFigures.EDStrength.Medium => RTF.Color(RTF.ElementID.Alert),
                            Build.EDFigures.EDStrength.Strong => RTF.Color(RTF.ElementID.Warning),
                            _ => RTF.Color(RTF.ElementID.Text)
                        };

                        edRtfText += $"{indent}{buffDebuffs[i].StatName}: {buffDebuffs[i].PostEDValue * 100:##0.##}% (Pre-ED: {buffDebuffs[i].Value * 100:##0.##}%)";
                    }

                    if (!endBlankLine)
                    {
                        return edRtfText;
                    }

                    edRtfText += buffDebuffs.Count > 0 ? $"{RTF.Color(RTF.ElementID.Text)}{RTF.Crlf()}{RTF.Crlf()}" : "";

                    return edRtfText;
                }
            }

            public static class Scales
            {
                private static DataView2 Root;
                private static InfoType LayoutType;
                private static Dictionary<VariableStatsGraph.PowerStats.FxIdentifier, List<VariableStatsGraph.PowerStats.DataPoint>> _DataPoints;

                public static void Render(DataView2 root, InfoType layoutType)
                {
                    Root = root;
                    LayoutType = layoutType;
                    _DataPoints = VariableStatsGraph.PowerStats.GetPOI(HistoryIdx);

                    DisplayScales();
                }

                public static void UpdateGraph()
                {
                    Root.skglScalesGraph.Invalidate();
                }

                public static Dictionary<VariableStatsGraph.PowerStats.FxIdentifier, List<VariableStatsGraph.PowerStats.DataPoint>> DataPoints => _DataPoints;

                private static void DisplayScales()
                {
                    Root.ipbLock5.Visible = Root.Locked;
                    Root.ipbResize5.IconChar = Root.SmallSize
                        ? IconChar.ChevronDown
                        : IconChar.ChevronUp;

                    if (BuildPowerEntry is {Power: {VariableEnabled: true}})
                    {
                        Root.powerScaler1.Value = BuildPowerEntry.VariableValue;
                        Root.powerScaler1.Minimum = BuildPowerEntry.Power.VariableMin;
                        Root.powerScaler1.Maximum = BuildPowerEntry.Power.VariableMax;
                    }

                    if (!Root.powerScaler2.Visible & !Root.powerScaler3.Visible)
                    {
                        Root.skglScalesGraph.Location = new Point(4, 112);
                        Root.skglScalesGraph.Size = new Size(353, 264);
                    }
                    else if (!Root.powerScaler3.Visible)
                    {
                        Root.skglScalesGraph.Location = new Point(4, 134);
                        Root.skglScalesGraph.Size = new Size(353, 242);
                    }
                    else
                    {
                        Root.skglScalesGraph.Location = new Point(4, 156);
                        Root.skglScalesGraph.Size = new Size(353, 220);
                    }

                    Root.scalesTabTitle.Invalidate();
                    Root.skglScalesGraph.Invalidate();
                }
            }
        }

        #endregion

        #region Event callbacks

        private void tabBox_TabIndexChanged(object sender, EventArgs e)
        {
            switch (_tabControlAdv.SelectedIndex)
            {
                case 0:
                    // L=39 / L=23
                    if (MidsContext.Character.IsHero())
                    {
                        tabPageAdv1.BackColor = Color.FromArgb(12, 56, 100);
                        tabPageAdv1.TabBackColor = Color.FromArgb(12, 56, 100);
                        _tabControlAdv.ActiveTabColor = Color.FromArgb(12, 56, 100);
                        _tabControlAdv.InactiveTabColor = Color.FromArgb(7, 33, 59);
                    }
                    else
                    {
                        tabPageAdv1.BackColor = Color.FromArgb(100, 12, 20);
                        tabPageAdv1.TabBackColor = Color.FromArgb(100, 12, 20);
                        _tabControlAdv.ActiveTabColor = Color.FromArgb(100, 12, 20);
                        _tabControlAdv.InactiveTabColor = Color.FromArgb(59, 7, 12);
                    }

                    break;

                case 1:
                    // L=51 / L=30
                    _tabControlAdv.ActiveTabColor = Color.Indigo;
                    _tabControlAdv.InactiveTabColor = Color.FromArgb(45, 0, 77);
                    break;

                case 2:
                    // L=33 / L=20
                    _tabControlAdv.ActiveTabColor = Color.FromArgb(2, 85, 55);
                    _tabControlAdv.InactiveTabColor = Color.FromArgb(1, 51, 33);
                    break;

                case 3:
                    // L=45 / L=27
                    _tabControlAdv.ActiveTabColor = Color.FromArgb(0, 98, 116);
                    _tabControlAdv.InactiveTabColor = Color.FromArgb(0, 59, 69);
                    break;

                case 4:
                    // L=58 / L=35
                    _tabControlAdv.ActiveTabColor = Color.FromArgb(148, 117, 46);
                    _tabControlAdv.InactiveTabColor = Color.FromArgb(69, 71, 28);
                    break;
            }

            Tabs.RenderTabs(this, true);
            TabChanged?.Invoke(_tabControlAdv.SelectedIndex);
        }

        protected void powerScaler_ValueChanged(object sender, EventArgs e)
        {
            var target = (ColorSlider) sender;

            target.ElapsedInnerColor = Tabs.InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedInnerColor);
            target.ElapsedPenColorBottom = Tabs.InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorBottom);
            target.ElapsedPenColorTop = Tabs.InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorTop);

            if (FreezeScalerCB) return;
            if (target.Name != "powerScaler1") return;

            BuildPowerEntry.VariableValue = (int) target.Value;
            Tabs.Scales.UpdateGraph();
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
        }

        protected void skglEnh_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            var target = (SKGLControl) sender;
            const int enhImgSize = 30;
            const int margin = 3;

            e.Surface.Canvas.Clear(SKColors.Black);
            if (_flipAnimator == null)
            {
                return;
            }

            var nbMaxSlots = Math.Max(_flipAnimator.NbEnhMain, _flipAnimator.NbEnhAlt);
            var nbSlots = target.Name == "skglEnhActive" ? _flipAnimator.NbEnhMain : _flipAnimator.NbEnhAlt;
            for (var i = 0; i < nbMaxSlots; i++)
            {
                var mainBitmap = _flipAnimator.GetBitmap(Tray.Main, i);
                var altBitmap = _flipAnimator.GetBitmap(Tray.Alt, i);

                var skImage = FlipAnimator.Bitmaps.DrawSingle(
                    target.Name == "skglEnhActive" ? mainBitmap : altBitmap,
                    target.Name == "skglEnhActive" ? altBitmap : mainBitmap,
                    Math.Min(180, Math.Max(0, _flipAnimator.Angle - FlipAnimator.KerningAngle * i)));
                //e.Surface.Canvas.DrawImage(skImage, new SKPoint(2 + (enhImgSize + margin) * i, (target.Height - enhImgSize) / 2f)); // Left align, vertical center
                e.Surface.Canvas.DrawImage(skImage, new SKPoint((Width - (nbSlots * (enhImgSize + margin) - margin)) / 2f + (enhImgSize + margin) * i, (target.Height - enhImgSize) / 2f)); // Horizontal center, vertical center
            }
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            // Rotation seem slower first time it's run.
            if (_flipAnimator.Angle >= _flipAnimator.FullCycleAngle)
            {
                timer1.Stop();
                // Swap main and alt slots
                _flipAnimator.SwapSets();
                _flipAnimator.Angle = 0;
                _flipAnimator.Active = false;

                MainModule.MidsController.Toon.FlipSlots(HistoryIdx);
                UpdateData();
                FileModified?.Invoke();
                RefreshInfo?.Invoke();
            }
            else
            {
                _flipAnimator.Angle = Math.Min(_flipAnimator.Angle + 15, _flipAnimator.FullCycleAngle);
                skglEnhActive.Invalidate();
                skglEnhAlt.Invalidate();
            }
        }

        protected void skglControl_Click(object sender, EventArgs e)
        {
            if (_flipAnimator.Active)
            {
                return;
            }

            if (HistoryIdx <= 1 || MidsContext.Character.CurrentBuild.Powers[HistoryIdx].Slots.Length == 0)
            {
                return;
            }

            _flipAnimator.Active = true;
            timer1.Start();
        }

        protected void skglScalesGraph_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            var target = (SKGLControl) sender;

            e.Surface.Canvas.Clear(SKColors.Black);
            using var graph = VariableStatsGraph.DrawScalesGraphSurface(this, HistoryIdx, target.Width, target.Height);
            e.Surface.Canvas.DrawImage(graph, new SKPoint(0, 0));
        }

        private void DataView2_Load(object sender, EventArgs e)
        {
            richInfoSmall.Text = string.Empty;
            richInfoLarge.Text = string.Empty;
            GridMouseOverEventLoc = new GridViewMouseEventInfo
            {
                Target = listInfos,
                Loc = new Point(-1, -1),
                InfoType = InfoType.Power
            };

            // BackColor doesn't stick when set in the designer
            ipbResize.BackColor = Color.Black;
            ipbResize2.BackColor = Color.Black;
            ipbResize3.BackColor = Color.Black;
            ipbResize4.BackColor = Color.Black;
            ipbResize5.BackColor = Color.Black;

            // MaxItems doesn't stick when set in the designer
            dV2TotalsPane1L.MaxItems = 6;
            dV2TotalsPane1R.MaxItems = 6;
            dV2TotalsPane2L.MaxItems = 6;
            dV2TotalsPane2R.MaxItems = 6;
            dV2TotalsPane3L.MaxItems = 6;
            dV2TotalsPane3R.MaxItems = 6;

            skDamageGraph1.LockDraw();
            skDamageGraph1.nBaseVal = 0;
            skDamageGraph1.nMaxEnhVal = 0;
            skDamageGraph1.nEnhVal = 0;
            skDamageGraph1.Text = string.Empty;
            skDamageGraph1.UnlockDraw();
        }

        private void dataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            var target = (DataGridView) sender;
            var tooltipTextSource = target.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText;

            // Do not set tooltip text when mouse is stationary to reduce tooltip flicker.
            if (GridMouseOverEventLoc.Target.Name == target.Name
                & GridMouseOverEventLoc.Loc.Equals(e.Location)
                & GridMouseOverEventLoc.InfoType == LayoutType)
            {
                return;
            }

            toolTip1.SetToolTip(target, tooltipTextSource != "" ? target.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText : "");
            GridMouseOverEventLoc = new GridViewMouseEventInfo
            {
                Target = target,
                Loc = e.Location,
                InfoType = LayoutType
            };
        }

        private void dataGridView_MouseLeave(object sender, EventArgs e)
        {
            var target = (DataGridView) sender;

            toolTip1.SetToolTip(target, "");
            GridMouseOverEventLoc = new GridViewMouseEventInfo
            {
                Target = target,
                Loc = new Point(-1, -1),
                InfoType = InfoType.Power
            };
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            var target = (DataGridView) sender;
            target.ClearSelection();
        }

        private void tabTitle_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            const int iconSize = 16;
            const int itemsPadding = 3;
            const float textSize = 13;
            string defaultPowerInfoText = "Info";

            var target = (SKGLControl) sender;

            if (LayoutType == InfoType.Enhancement)
            {
                var enhName = "";
                if (EnhSlot.Enh > -1)
                {
                    enhName = DatabaseAPI.Database.Enhancements[EnhSlot.Enh].LongName;
                    if ((enhName.Length > 38) & (EnhLevel > -1))
                    {
                        enhName = DatabaseAPI.GetEnhancementNameShortWSet(EnhSlot.Enh);
                    }
                }
                else
                {
                    // _basePower may be wrong or undefined
                    enhName = _basePower != null ? _basePower.DisplayName : "";
                }

                var textRect = new SKRect(0, 0, 0, 0);
                using var textPaint = new SKPaint
                {
                    Color = SKColors.WhiteSmoke,
                    TextSize = textSize
                };

                textPaint.MeasureText(enhName, ref textRect);
                var maxWidth = target.Width;
                var totalWidth = (EnhSlot.Enh > -1 ? iconSize + itemsPadding : 0) + textRect.Width;
                var x = (int) Math.Max(0, Math.Round((maxWidth - totalWidth) / 2));

                e.Surface.Canvas.Clear(SKColors.Black);
                if (EnhSlot.Enh > -1)
                {
                    var enhImg = FlipAnimator.Bitmaps.CreateBitmap(EnhSlot.Enh);
                    using var enhIcon = new SKBitmap(new SKImageInfo(16, 16));
                    enhImg.ScalePixels(enhIcon, SKFilterQuality.High);
                    e.Surface.Canvas.DrawBitmap(enhIcon, new SKRect(0, 0, 16, 16), new SKRect(x, 0, x + 16, 16));
                }

                e.Surface.Canvas.DrawText(
                    SKTextBlob.Create(enhName, new SKFont(SKTypeface.Default, textSize)),
                    x + (EnhSlot.Enh > -1 ? iconSize + itemsPadding : 0), target.Height - 2 - (target.Height - textRect.Height) / 2,
                    textPaint
                );
            }
            else if (LayoutType == InfoType.Power)
            {
                var powerInfo = $@"{(BuildPowerEntry != null ? $"[{BuildPowerEntry.Level + 1}] " : "")}{_basePower?.DisplayName ?? defaultPowerInfoText}";
                var powerNid = BuildPowerEntry?.NIDPower ?? -1;
                var powersetNid = powerNid == -1
                    ? -1
                    : DatabaseAPI.Database.Power[powerNid].GetPowerSet().nID;

                using var textPaint = new SKPaint
                {
                    Color = SKColors.WhiteSmoke,
                    TextSize = textSize
                };

                var textRect = new SKRect(0, 0, 0, 0);
                textPaint.MeasureText(powerInfo, ref textRect);
                var maxWidth = target.Width;
                var hasIcon = !(powersetNid == -1 | powerInfo == (_basePower?.DisplayName ?? defaultPowerInfoText));
                var totalWidth = (hasIcon ? iconSize + itemsPadding : 0) + textRect.Width;
                var x = (int) Math.Max(0, Math.Round((maxWidth - totalWidth) / 2));

                e.Surface.Canvas.Clear(SKColors.Black);
                if (powersetNid > -1)
                {
                    var powersetFullName = DatabaseAPI.Database.Powersets[powersetNid].FullName;
                    using var powersetImg = FlipAnimator.Utils.GetIncarnateSlotFromPowerset(powersetFullName) switch
                    {
                        FlipAnimator.IncarnateSlot.Alpha => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Alpha),
                        FlipAnimator.IncarnateSlot.Judgement => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Judgement),
                        FlipAnimator.IncarnateSlot.Interface => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Interface),
                        FlipAnimator.IncarnateSlot.Lore => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Lore),
                        FlipAnimator.IncarnateSlot.Destiny => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Destiny),
                        FlipAnimator.IncarnateSlot.Hybrid => FlipAnimator.Bitmaps.CreateBitmap(FlipAnimator.IncarnateSlot.Hybrid),
                        _ => FlipAnimator.Bitmaps.CreateBitmap(
                            I9Gfx.GetPowersetImage(DatabaseAPI.Database.Powersets[powersetNid]))
                    };

                    using var powersetIcon = new SKBitmap(new SKImageInfo(16, 16));
                    powersetImg.ScalePixels(powersetIcon, SKFilterQuality.High);
                    e.Surface.Canvas.DrawBitmap(powersetIcon, new SKRect(0, 0, 16, 16), new SKRect(x, 0, x + 16, 16));
                }

                e.Surface.Canvas.DrawText(
                    SKTextBlob.Create(powerInfo, new SKFont(SKTypeface.Default, textSize)),
                    x + (hasIcon ? iconSize + itemsPadding : 0), target.Height - 2 - (target.Height - textRect.Height) / 2,
                    textPaint
                );
            }
        }

        private void ipbLock_Click(object sender, EventArgs e)
        {
            Locked = false;
            if (_tabsRendered.Effects)
            {
                ipbLock2.Visible = false;
            }
            else if (_tabsRendered.Totals)
            {
                ipbLock3.Visible = false;
            }
            else if (_tabsRendered.Enhance)
            {
                ipbLock4.Visible = false;
            }
            else if (_tabsRendered.Scales)
            {
                ipbLock5.Visible = false;
            }
            else
            {
                ipbLock.Visible = false;
            }

            Unlock?.Invoke();
        }

        private void ipbResize_Click(object sender, EventArgs e)
        {
            SmallSize = !SmallSize;
            ipbResize.IconChar = SmallSize
                ? IconChar.ChevronDown
                : IconChar.ChevronUp;
        }

        private void DvPaneMisc_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            dV2TotalsPane3L.Visible = false;
            dV2TotalsPane3R.Visible = false;
            var targetBtn = Tabs.Totals.MiscEffectsType switch
            {
                TotalsMiscEffectsType.DebuffResistances => btnMiscTotals2,
                TotalsMiscEffectsType.MezResistances => btnMiscTotals3,
                TotalsMiscEffectsType.MezProtection => btnMiscTotals4,
                _ => btnMiscTotals1
            };

            // Mark active mode button
            var modeButtons = new[] { btnMiscTotals1, btnMiscTotals2, btnMiscTotals3, btnMiscTotals4 };
            foreach (var btn in modeButtons)
            {
                btn.BackColor = btn.Name == targetBtn.Name ? Tabs.Totals.Colors.ButtonSelectedColor : Tabs.Totals.Colors.ButtonNormalColor;
            }

            panelMiscTypeSelector.Visible = true;
        }

        private void DvPaneMisc_BarHover(object sender, string containerControlName, Point mouseLoc, int barIndex, string label, float value, float uncappedValue)
        {
            var target = (SKGLControl) sender;

            if (barIndex < 0)
            {
                toolTip1.SetToolTip(target, "");

                return;
            }

            if (containerControlName == Tabs.Totals.PaneMouseEventInfo.ContainerControlName & mouseLoc.Equals(Tabs.Totals.PaneMouseEventInfo.Loc))
            {
                return;
            }

            Tabs.Totals.PaneMouseEventInfo = new TotalsPaneMouseEventInfo
            {
                ContainerControlName = containerControlName,
                Loc = mouseLoc
            };

            switch (containerControlName)
            {
                case "dV2TotalsPane1L":
                case "dV2TotalsPane1R":
                    label = $"Defense to {label}";
                    break;

                case "dV2TotalsPane2L":
                case "dV2TotalsPane2R":
                    label = $"Resistance to {label}";
                    break;

                case "dV2TotalsPane3L":
                case "dV2TotalsPane3R":
                    label = Tabs.Totals.MiscEffectsType switch
                    {
                        TotalsMiscEffectsType.DebuffResistances => $"Resistance to {label} Debuff",
                        TotalsMiscEffectsType.MezResistances => $"Resistance to {label}",
                        TotalsMiscEffectsType.MezProtection => $"Protection against {label}",
                        _ => $"Elusivity({label})"
                    };
                    break;
            }

            var percentage = !(containerControlName == "dV2TotalsPane3L" | containerControlName == "dV2TotalsPane3R") ||
                             Tabs.Totals.MiscEffectsType != TotalsMiscEffectsType.MezProtection
                ? "%"
                : "";

            toolTip1.SetToolTip(target, Math.Abs(value - uncappedValue) < 0.01
                ? $"{label}: {value:###0.##}{percentage}"
                : $"{label}: {uncappedValue:###0.##}{percentage}\r\n Capped at {value:###0.##}{percentage}");
        }

        private void miscEffectsSelectorBtn_MouseEnter(object sender, EventArgs e)
        {
            var target = (Button) sender;
            var ret = int.TryParse(target.Tag.ToString(), out var tagValue);
            if (!ret)
            {
                tagValue = -1;
            }

            if (tagValue == (int) Tabs.Totals.MiscEffectsType)
            {
                return;
            }

            target.BackColor = Tabs.Totals.Colors.ButtonHighlightColor;
            label8.Text = $"[Switch to {target.Text} view]";
        }

        private void miscEffectsSelectorBtn_MouseLeave(object sender, EventArgs e)
        {
            var target = (Button) sender;
            var ret = int.TryParse(target.Tag.ToString(), out var tagValue);
            if (!ret)
            {
                tagValue = -1;
            }

            if (tagValue == (int) Tabs.Totals.MiscEffectsType)
            {
                return;
            }

            target.BackColor = Tabs.Totals.Colors.ButtonNormalColor;
            var headerGroupText = Tabs.Totals.MiscEffectsType switch
            {
                TotalsMiscEffectsType.DebuffResistances => "Debuff Resistances",
                TotalsMiscEffectsType.MezResistances => "Mez Resistances",
                TotalsMiscEffectsType.MezProtection => "Mez Protection",
                _ => "Elusivity"
            };

            label8.Text = $"Misc Effects ({headerGroupText}):";
        }

        private void miscEffectsSelectorBtn_Click(object sender, EventArgs e)
        {
            var target = (Button) sender;

            
            var ret = int.TryParse(target.Tag.ToString(), out var tagValue);
            if (!ret)
            {
                tagValue = 0;
            }

            panelMiscTypeSelector.Visible = false;
            dV2TotalsPane3L.Visible = true;
            dV2TotalsPane3R.Visible = true;
            Tabs.Totals.SwitchMiscEffectsType(tagValue);
        }

        private void panelMiscTypeSelector_Click(object sender, EventArgs e)
        {
            panelMiscTypeSelector.Visible = false;
            dV2TotalsPane3L.Visible = true;
            dV2TotalsPane3R.Visible = true;
        }

        private void miscEffectsSelectorBtn_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, DisplayRectangle, Color.FromArgb(77, 77, 77), ButtonBorderStyle.Solid);
        }

        private void enhanceRt_Click(object sender, EventArgs e)
        {
            Tabs.Enhance.NextViewMode();
        }

        private void enhanceRt_Enter(object sender, EventArgs e)
        {
            // [user32.dll].HideCaret() doesn't work.
            tabPageAdv4.Focus();
        }

        #endregion

    }
}